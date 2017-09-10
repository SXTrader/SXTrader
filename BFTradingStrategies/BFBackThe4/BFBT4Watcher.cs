using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.livescoreparser;
using System.Collections;
using net.sxtrader.bftradingstrategies.betfairif;
using BetFairIF.com.betfair.api.exchange;
using System.Diagnostics;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.BackThe4
{
    class BFBT4Watcher : IBFTSCommon
    {
        private SortedList m_list = null;
        private LiveScoreParser m_liveParser;
        private LiveScore2Parser m_liveParser2;
        private BT4ConfigurationRW m_config;
        private SoccerMarketManager m_smm;
        private DateTime m_dtsLastUpdate;

        
        //private Thread m_autoBetter;
        private SortedList<int, BFBT4Strategy> m_runningTrades;
        //private SortedList<int, BFBT4Preparer> m_preparingTrades;
        //ENDE
        
        private bool m_active;
        private bool m_buildList =false;


        public event EventHandler<BFWMessageEventArgs> MessageEvent;
        public event EventHandler<BFWGameEndedEventArgs> GamenEndedEvent;
        public event EventHandler<MatchAddedEventArgs> MatchAddedEvent;
        public event EventHandler<BFWPlaytimeEventArgs> PlaytimeEvent;
        public event EventHandler<BFWGoalSumChangedEventArgs> GoalSumChangedEvent;
        public event EventHandler<BFWWinLooseChangedEventArgs> WinLooseChangedEvent;
        public event EventHandler<BFWNoIScoreAddedEventArgs> NoIScoreAdded;
        public event EventHandler<BFWIScoreAddedEventArgs> IScoreAdded;
        public event EventHandler<BFWSetCloseTradeTimer> SetCloseTradeTimer;
        public event EventHandler<BFWSetOpenBetTimer> SetOpenBetTimer;
        public event EventHandler<BFWSetStopLossTimer> SetStopLossTimer;
        public event EventHandler<BFWStopCloseTradeTimer> StopCloseTradeTimer;
        public event EventHandler<BFWStopOpenBetTimer> StopOpenBetTimer;
        public event EventHandler<BFWStopStopLossTimer> StopStopLossTimer;

        public BFBT4Watcher(LiveScoreParser parser, LiveScore2Parser parser2)
        {
            DebugWriter.Instance.WriteMessage("Watcher BackThe4", "creating Watcher");
            m_list = new SortedList();
            m_config = new BT4ConfigurationRW();
            m_runningTrades = new SortedList<int, BFBT4Strategy>();
            //m_preparingTrades = new SortedList<int, BFBT4Preparer>();
            m_liveParser = parser;
            m_liveParser2 = parser2;
            m_smm = SoccerMarketManager.Instance;
            m_active = m_config.StrategyActivated;
            /*
            m_autoBetter = new Thread(autoBetter);
            m_autoBetter.IsBackground = true;
            m_autoBetter.Start();
            */

            BetWatchdog.Instance.BetsUpdated += new EventHandler<BetsUpdatedEventArgs>(BFBetsUpdated);
            BetfairKom.Instance.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            BetWatchdog.Instance.ExceptionMessageEvent  += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            m_liveParser.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            m_liveParser2.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
        }

        void Instance_ExceptionMessageEvent(object sender, SXExceptionMessageEventArgs e)
        {

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(e));
            }
        }

        public bool Active
        {
            get
            {
                return m_active;
            }
            set
            {
                DebugWriter.Instance.WriteMessage("Watcher BackThe4", String.Format("changing active to {0}", value.ToString()));
                m_active = value;

                IDictionaryEnumerator enumBetSets = m_list.GetEnumerator();
                while (enumBetSets.MoveNext() == true)
                {
                    BFBT4Strategy betSet = (BFBT4Strategy)enumBetSets.Value;
                    betSet.Active = m_active;
                }
            }
        }

        public SortedList BetSet
        {
            get
            {
                return m_list;
            }
        }

        public LiveScoreParser LiveParser
        {
            get
            {
                return m_liveParser;
            }
        }

        public LiveScore2Parser LiveParser2
        {
            get
            {
                return m_liveParser2;
            }
        }


        public void startListBuilder()
        {            
            //this.Active = reader.StrategyActivated;
            buildBetList();
        }

        public BT4ConfigurationRW getConfiguration(String match)
        {
            foreach (BFBT4Strategy strategy in m_list.Values)
            {
                if (strategy.Match.Equals(match))
                {
                    return strategy.Configuration;
                }
            }
            
            return null;
        }

        public void setConfiguration(String match, BT4ConfigurationRW config)
        {
            
            foreach (int id in m_list.Keys)
            {
                BFBT4Strategy strategy = (BFBT4Strategy)m_list[id];
                if (strategy.Match.Equals(match))
                {
                    strategy.Configuration = config;
                    m_list[id] = strategy;
                    return;
                }
            }
            
            /*
            foreach (BFBT4Strategy strategy in m_list.Values)
            {
                if (strategy.Match.Equals(match))
                {
                    strategy.Configuration = config;
                    return;
                }
                
            } 
             */
        }

        public bool hasLiveScore1(String match)
        {
            foreach (BFBT4Strategy betSet in m_list.Values)
            {
                if (betSet.Match == match)
                {
                    if (betSet.Score != null)
                        return true;
                }
            }

            return false;
        }

        public bool hasLiveScore2(String match)
        {
            foreach (BFBT4Strategy betSet in m_list.Values)
            {
                if (betSet.Match == match)
                {
                    if (betSet.Score2 != null)
                        return true;
                }
            }

            return false;
        }

        public void manualBFLSLink2(String match, IScore score)
        {
            LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;
            int marketId = 0;
            foreach (BFBT4Strategy betSet in m_list.Values)
            {                
                if (score != null && betSet.Match == match)
                {
                    score.IncreaseRef();
                    score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                    score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                    score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                    score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);

                    /*
                    betSet.GoalScoredEvent += new EventHandler<BFGoalScoredEventArgs>(BFGoalScoredHandler);
                    betSet.PlaytimeEvent += new EventHandler<BFPlaytimeEventArgs>(BFPlaytimeHandler);
                    betSet.RiskWinChangedEvent += new EventHandler<BFRiskWinChangedEventArgs>(BFRiskWinChangedHandler);
                    betSet.MessageEvent += new EventHandler<BFMessageEventArgs>(BFMessageHandler);
                    betSet.GameEndedEvent += new EventHandler<BFGameEndedEventArgs>(BFGameEndedHandler);
                     */
                    betSet.Score2 = score;
                    //betSet.initRiskWin();
                    betSet.Active = this.Active;


                    // Lokale XML-Datei laden
                    if (score.TeamA != "Team None" && score.TeamB != "Team None")
                    {
                        LiveScore2Parser.Instance.writeLocalXml(betSet.Match, score);
                        score.BetfairMatch = betSet.Match;
                    }
                }

                if (betSet.Match == match)
                {
                    marketId = betSet.Lay.MarketId;
                }

            }


            if (hasLiveScore1(match) && hasLiveScore2(match))
                withLivescore = LIVESCOREADDED.ALL;
            else if (hasLiveScore1(match) || hasLiveScore2(match))
                withLivescore = LIVESCOREADDED.PARTLY;

            if (score != null)
            {
                EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new BFWIScoreAddedEventArgs(match, withLivescore, marketId));
            }
            else
            {
                EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new BFWNoIScoreAddedEventArgs(match));
            }
        }

        public void manualBFLSLink(String match, IScore score)
        {
            LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;
            int marketId = 0;

            foreach (BFBT4Strategy betSet in m_list.Values)
            {
                if (betSet.Match != match)
                    continue;

                marketId = betSet.Lay.MarketId;

                if (score != null && betSet.Match == match)
                {
                    score.IncreaseRef();
                    score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                    score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                    score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                    score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);

                    /*
                    betSet.GoalScoredEvent += new EventHandler<BFGoalScoredEventArgs>(BFGoalScoredHandler);
                    betSet.PlaytimeEvent += new EventHandler<BFPlaytimeEventArgs>(BFPlaytimeHandler);
                    betSet.RiskWinChangedEvent += new EventHandler<BFRiskWinChangedEventArgs>(BFRiskWinChangedHandler);
                    betSet.MessageEvent += new EventHandler<BFMessageEventArgs>(BFMessageHandler);
                    betSet.GameEndedEvent += new EventHandler<BFGameEndedEventArgs>(BFGameEndedHandler);
                     */
                    betSet.Score = score;
                    //betSet.initRiskWin();
                    betSet.Active = this.Active;


                    // Lokale XML-Datei laden
                    if (score.TeamA != "Team None" && score.TeamB != "Team None")
                    {
                        LiveScoreParser.Instance.WriteLocalXml(betSet.Match, score);
                        score.BetfairMatch = betSet.Match;
                    }
                }

            }

            if (hasLiveScore1(match) && hasLiveScore2(match))
                withLivescore = LIVESCOREADDED.ALL;
            else if (hasLiveScore1(match) || hasLiveScore2(match))
                withLivescore = LIVESCOREADDED.PARTLY;

            if (score != null)
            {
                EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new BFWIScoreAddedEventArgs(match, withLivescore, marketId));
            }
            else
            {
                EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new BFWNoIScoreAddedEventArgs(match));
            }
        }
/*
        public void manualBFLSLink(String match, IScore score)
        {
            
             foreach (BFBT4Strategy betSet in m_list.Values)
                {
                    if (score != null && betSet.Match == match)
                    {
                        DebugWriter.Instance.WriteMessage("Watcher BackThe4", String.Format("binding manual livescore to match {0}", match));
                        score.IncreaseRef();

                        score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                        //score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEvent);
                        score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                        score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                        score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);



                        betSet.GameEndedEvent += new EventHandler<BFGameEndedEventArgs>(GameEndedEventHandler);
                        betSet.PlaytimeEvent += new EventHandler<BFPlaytimeEventArgs>(PlaytimeEventHandler);
                        betSet.GoalSumChangedEvent += new EventHandler<BFGoalSumChangedEventArgs>(GoalSumChangedEventHandler);
                        betSet.WinLooseChangedEvent += new EventHandler<BFWinLooseChangedEventArgs>(WinLooseChangedEventHandler);
                        betSet.MessageEvent += new EventHandler<BFMessageEventArgs>(MessageEventHandler);
                        betSet.Score = score;
                        betSet.Active = this.Active;
                        betSet.Start();


                        // Lokale XML-Datei laden
                        if (score.TeamA != "Team None" && score.TeamB != "Team None")
                            LiveScoreParser.Instance.writeLocalXml(betSet.Match, score);
                    }

            }

            if (score != null)
            {
                EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new BFWIScoreAddedEventArgs(match));
            }
            else
            {
                EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new BFWNoIScoreAddedEventArgs(match));
            }
        }
*/
        private void autoBetter()
        {
            /*
            // Nur für Testzwecke
            int waitTime = 0;
            while (true)
            {
                Thread.Sleep(waitTime);
                // Wartezeit 10 Minuten
                waitTime = 600000;

                // Falls Strategy global deaktivert => weiter;
                if (!m_active)
                {
                    waitTime = 60000;
                    continue;
                }               

                // Alle Märkte, welche innerhalb der nächsten 15 Minuten starten
                foreach (BFMarket market in m_smm.InPlayMarkets.Values)
                {
                    if (!market.IsTotalGoals)
                        continue;
                    //Event liegt zu weit in der Vergangenheit => Weiter
                    if (market.StartDTS < DateTime.Now)
                        continue;

                    // Später als 15 Minuten => Weiter;
                    TimeSpan diff = market.StartDTS.Subtract(DateTime.Now);
                    TimeSpan minutes15 = new TimeSpan(0, 15, 0);
                    if (diff > minutes15)
                        continue;
                    // Markt schon aktiv
                    if (m_runningTrades.ContainsKey(market.Id))
                        continue;

                    
                    // Markt schon in Vorbereitung
                    if (m_preparingTrades.ContainsKey(market.Id))
                        continue;

                    BFBT4Preparer preparer = new BFBT4Preparer(market, m_liveParser);
                    //preparer.NoTradePossible += new EventHandler<BFBT4PNoTradeEventArgs>(BFBT4PNoTradePossible);
                    preparer.TradeStarted += new EventHandler<BFBT4PTradeStartedEventArgs>(BFBT4PTradeStarted);
                    Debug.WriteLine(String.Format("Added match {0} to Preparer. MarketId {1}", market.Match, market.Id));

                    m_preparingTrades.Add(market.Id, preparer);
                    preparer.Start();
                }

            }
             */
        }

        private void buildBetList()
        {
            DebugWriter.Instance.WriteMessage("Watcher BackThe4", "build initial list of trades");
            EventHandler<SXExceptionMessageEventArgs> testhandler = ExceptionMessageEvent;            

            m_buildList = true;
            BetfairKom betfairKom = BetfairKom.Instance;

            SortedList localList = new SortedList();


            // Initial alles lesen
            DebugWriter.Instance.WriteMessage("Watcher BackThe4", "get all Bets");
            MUBet[] bets = betfairKom.getBets();

            if (bets == null)
            {
                m_buildList = false;
                return;
            }

            // lokale Liste aufbauen
            DebugWriter.Instance.WriteMessage("Watcher BackThe4", "iterating all Bets");
            foreach (MUBet bet in bets)
            {
                try
                {
                    BFBT4Strategy betSet = null;

                    // Details lesen
                    DebugWriter.Instance.WriteMessage("Watcher BackThe4", "read Bet Detail");
                    Bet betDetail = betfairKom.getBetDetail(bet.betId);

                    if (m_smm.isMarketTotalGoals(bet.marketId) == false)
                        continue;

                    if (m_smm.isMarketInplay(bet.marketId) == false)
                        continue;

                    String marketName = splitFullMarketName(betDetail.fullMarketName);

                    // Ist schon ein Eintrag für Markt in lokaler Liste vorhanden?
                    if ((betSet = (BFBT4Strategy)localList[betDetail.marketId]) != null)
                    {
                        // Wette dem Eintrag hinzufügen
                        if (betDetail.betType == BetTypeEnum.B)
                        {
                            try
                            {
                                betSet.Back.Bets.Add(betDetail.betId, betDetail);
                            }
                            catch (ArgumentException)
                            {
                                betSet.Back.Bets[betDetail.betId] = betDetail;
                            }
                            //betSet.Back = betDetail;
                        }
                        else if (betDetail.betType == BetTypeEnum.L)
                        {
                            try
                            {
                                betSet.Lay.Bets.Add(betDetail.betId, betDetail);
                            }
                            catch (ArgumentException)
                            {
                                betSet.Lay.Bets[betDetail.betId] = betDetail;
                            }
                            //betSet.Lay = betDetail;
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("Unknown Bettype:{0}", betDetail.betType.ToString());
                            throw new Exception(sb.ToString());
                        }
                    }
                    else //Eintrag noch nicht vorhanden
                    {
                        //Debug.WriteLine(String.Format("{0}: Added Match {1} with Marketid {2}", DateTime.Now, marketName, betDetail.marketId));
                        betSet = new BFBT4Strategy(marketName);

                        // Wette dem Eintrag hinzufügen
                        if (betDetail.betType == BetTypeEnum.B)
                        {
                            try
                            {
                                betSet.Back.Bets.Add(betDetail.betId, betDetail);
                            }
                            catch (ArgumentException)
                            {
                                betSet.Back.Bets[betDetail.betId] = betDetail;
                            }
                            //betSet.Back = betDetail;
                        }
                        else if (betDetail.betType == BetTypeEnum.L)
                        {
                            try
                            {
                                betSet.Lay.Bets.Add(betDetail.betId, betDetail);
                            }
                            catch (ArgumentException)
                            {
                                betSet.Lay.Bets[betDetail.betId] = betDetail;
                            }
                            //betSet.Lay = betDetail;
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("Unknown Bettype:{0}", betDetail.betType.ToString());
                            throw new Exception(sb.ToString());
                        }                       
                        localList.Add(betDetail.marketId, betSet);
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFBT4Watcher::buildBetList", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                    continue;
                }
            }

            // Mit globaler Liste abgleichen
            DebugWriter.Instance.WriteMessage("Watcher BackThe4", "Match local and global Betlist");
            //for(int i = 0; i < localList.Values.Count; i++)
            try
            {
                foreach (BFBT4Strategy betSet in localList.Values)
                {

                    // Falls keine LayWette => nicht Teil der Strategy
                    if (betSet.Back.Bets.Count == 0)
                        continue;

                    // Lay-Bets in Paar älter als Back-Bets => gehört nicht zur strategie
                    if (betSet.Lay.OldesDts.Ticks < betSet.Back.OldesDts.Ticks)
                        continue;
                    try
                    {
                        m_list.Add(betSet.Back.MarketId, betSet);
                        DebugWriter.Instance.WriteMessage("Watcher BackThe4", String.Format("Match {0} added", betSet.Match));
                        // Verknüpfung zu LiveScore mit EventHandler
                        IScore score = m_liveParser.injectBetfair(betSet.TeamA, betSet.TeamB);
                        IScore score2 = m_liveParser2.injectBetfair(betSet.TeamA, betSet.TeamB);
                        LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;

                        if (score != null)
                        {
                            DebugWriter.Instance.WriteMessage("Watcher BackThe4", String.Format("binding livescore {0} to match {0}", score.getLiveMatch(), betSet.Match));
                            score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                            //score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEvent);
                            score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                            score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                            score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);

                            betSet.Score = score;
                        }

                        if (score2 != null)
                        {
                            DebugWriter.Instance.WriteMessage("BFUEWatcher::buildBetList", String.Format("binding livescore {0} to match {0}", score.getLiveMatch(), betSet.Match));
                            score2.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                            score2.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                            score2.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                            score2.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);

                            betSet.Score2 = score2;
                        }

                        if (score != null && score2 != null)
                        {
                            withLivescore = LIVESCOREADDED.ALL;
                        }
                        else if (score != null || score2 != null)
                        {
                            withLivescore = LIVESCOREADDED.PARTLY;
                        }

                        betSet.GameEndedEvent += new EventHandler<BFGameEndedEventArgs>(GameEndedEventHandler);
                        betSet.PlaytimeEvent += new EventHandler<BFPlaytimeEventArgs>(PlaytimeEventHandler);
                        betSet.GoalSumChangedEvent += new EventHandler<BFGoalSumChangedEventArgs>(GoalSumChangedEventHandler);
                        betSet.WinLooseChangedEvent += new EventHandler<BFWinLooseChangedEventArgs>(WinLooseChangedEventHandler);
                        betSet.MessageEvent += new EventHandler<BFMessageEventArgs>(MessageEventHandler);
                        betSet.SetCloseTradeTimer += new EventHandler<BFSetCloseTradeTimer>(betSet_SetCloseTradeTimer);
                        betSet.SetOpenBetTimer += new EventHandler<BFSetOpenBetTimer>(betSet_SetOpenBetTimer);
                        betSet.SetStopLossTimer += new EventHandler<BFSetStopLossTimer>(betSet_SetStopLossTimer);
                        betSet.StopCloseTradeTimer += new EventHandler<BFStopCloseTradeTimer>(betSet_StopCloseTradeTimer);
                        betSet.StopOpenBetTimer += new EventHandler<BFStopOpenBetTimer>(betSet_StopOpenBetTimer);
                        betSet.StopStopLossTimer += new EventHandler<BFStopStopLossTimer>(betSet_StopStopLossTimer);
                        
                        EventHandler<BFWMessageEventArgs> message = MessageEvent;
                        if (message != null)
                            message(this, new BFWMessageEventArgs(DateTime.Now, betSet.Match, String.Format("Trade Added{0}", betSet.Match), BackThe4.strModule));


                        if (withLivescore != LIVESCOREADDED.NONE)
                        {
                            EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                            if (handler != null)
                            {
                                double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                                double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                                handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore, betSet.Back.MarketId));
                            }

                            EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                            if (addedHandler != null)
                                addedHandler(this, new BFWIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB), withLivescore, betSet.Back.MarketId));
                        }
                        else
                        {
                            EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                            if (handler != null)
                            {
                                double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                                double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                                handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore, betSet.Back.MarketId));
                            }

                            EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                            if (addedHandler != null)
                                addedHandler(this, new BFWNoIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                        }                                                
                    }
                    catch (ArgumentException)
                    {
                        ((BFBT4Strategy)m_list[betSet.Back.MarketId]).Back = betSet.Back;
                        ((BFBT4Strategy)m_list[betSet.Back.MarketId]).Lay = betSet.Lay; ;

                        DebugWriter.Instance.WriteMessage("Watcher BackThe4", String.Format("Match {0} Updated", betSet.Match));

                        EventHandler<BFWWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                        if (handler != null)
                            handler(this, new BFWWinLooseChangedEventArgs(betSet.Match, Math.Round(betSet.Lay.RiskWin - betSet.Back.RiskWin, 2), Math.Round(betSet.Lay.BetSize - betSet.Back.BetSize, 2)));

                    }                                      
                    betSet.Start();
                    betSet.Active = this.Active;
                }
            }
            catch (InvalidCastException ice)
            {
                EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                if (handler != null)
                {
                    handler(this, new SXExceptionMessageEventArgs("BFBT4Watcher::buildBetList", "An Exception has occured. Please check LogFiles for Details"));
                }

                ExceptionWriter.Instance.WriteException(ice);
                Debug.WriteLine(ice.Message);
            }

            m_buildList = false;
        }

        private String splitFullMarketName(String fullmarketname)
        {
            char[] cSeps = { '/' };
            String[] splittedMarketName = fullmarketname.Split(cSeps);
            splittedMarketName[1] = splittedMarketName[1].Replace(" v ", " - ");
            return splittedMarketName[1];
        }

        void betSet_StopStopLossTimer(object sender, BFStopStopLossTimer e)
        {
            EventHandler<BFWStopStopLossTimer> handler = StopStopLossTimer;
            if (handler != null)
            {
                handler(this, new BFWStopStopLossTimer(e.Match));
            }
        }

        void betSet_StopOpenBetTimer(object sender, BFStopOpenBetTimer e)
        {
            EventHandler<BFWStopOpenBetTimer> handler = StopOpenBetTimer;
            if (handler != null)
            {
                handler(this, new BFWStopOpenBetTimer(e.Match));
            }
        }

        void betSet_StopCloseTradeTimer(object sender, BFStopCloseTradeTimer e)
        {
            EventHandler<BFWStopCloseTradeTimer> handler = StopCloseTradeTimer;
            if (handler != null)
            {
                handler(this, new BFWStopCloseTradeTimer(e.Match));
            }
        }

        void betSet_SetStopLossTimer(object sender, BFSetStopLossTimer e)
        {
            EventHandler<BFWSetStopLossTimer> handler = SetStopLossTimer;
            if (handler != null)
            {
                handler(this, new BFWSetStopLossTimer(e.Match, e.Timer));
            }
        }

        void betSet_SetOpenBetTimer(object sender, BFSetOpenBetTimer e)
        {
            EventHandler<BFWSetOpenBetTimer> handler = SetOpenBetTimer;
            if (handler != null)
            {
                handler(this, new BFWSetOpenBetTimer(e.Match, e.Timer));
            }
        }

        void betSet_SetCloseTradeTimer(object sender, BFSetCloseTradeTimer e)
        {
            EventHandler<BFWSetCloseTradeTimer> handler = SetCloseTradeTimer;
            if (handler != null)
            {
                handler(this, new BFWSetCloseTradeTimer(e.Match, e.Timer));
            }
        }

        void BFBT4PTradeStarted(object sender, BFBT4PTradeStartedEventArgs e)
        {
            /*
            m_preparingTrades.Remove(e.Bet.marketId);
            BFBT4Strategy strategy = new BFBT4Strategy(e.Bet, e.Match);
            String[] sepMatch = { " - " };
            String[] teams = e.Match.Split(sepMatch, StringSplitOptions.None);
            IScore score = m_liveParser.injectBetfair(teams[0], teams[1]);
            if (score != null)
            {
                score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(strategy.PlaytimeTick);
                //score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEvent);
                score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(strategy.GoalEventHandler);
                score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(strategy.GoalBackEventHandler);
                score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(strategy.GameEndedEventHandler);
            }

            strategy.GameEndedEvent += new EventHandler<BFGameEndedEventArgs>(GameEndedEventHandler);
            strategy.PlaytimeEvent += new EventHandler<BFPlaytimeEventArgs>(PlaytimeEventHandler);
            strategy.GoalSumChangedEvent += new EventHandler<BFGoalSumChangedEventArgs>(GoalSumChangedEventHandler);
            strategy.WinLooseChangedEvent += new EventHandler<BFWinLooseChangedEventArgs>(WinLooseChangedEventHandler);
            strategy.MessageEvent += new EventHandler<BFMessageEventArgs>(MessageEventHandler);
            try
            {
                m_runningTrades.Add(e.Bet.marketId, strategy);
                // Nachricht verbreiten
                EventHandler<BFWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new BFWMessageEventArgs(DateTime.Now, e.Match, "Trade Added and Started {0} {1}", BackThe4.strModule));

                EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                if (handler != null)
                {
                    double backWin = strategy.Back.RiskWin - strategy.Lay.RiskWin;
                    double backLoss = strategy.Lay.BetSize - strategy.Back.BetSize;
                    handler(this, new MatchAddedEventArgs(e.Match, strategy.SumGoals.ToString(),  backWin.ToString(), backLoss.ToString()));
                }
                strategy.Start();
                m_list.Add(strategy.Back.MarketId, strategy);
                // TODO: GameAddedEvent an Host
                //throw new NotImplementedException();
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
             */
        }

        void MessageEventHandler(object sender, BFMessageEventArgs e)
        {
            EventHandler<BFWMessageEventArgs> message = MessageEvent;
            if (message != null)
                message(this, new BFWMessageEventArgs(e.MessageDTS, e.Match, e.Message, e.Module));
        }

        void WinLooseChangedEventHandler(object sender, BFWinLooseChangedEventArgs e)
        {
            EventHandler<BFWWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
            if (handler != null)
                handler(this, new BFWWinLooseChangedEventArgs(e.Match, e.BackGuV, e.LayGuV));
        }

        void GoalSumChangedEventHandler(object sender, BFGoalSumChangedEventArgs e)
        {
            EventHandler<BFWGoalSumChangedEventArgs> handler = GoalSumChangedEvent;
            if (handler != null)
                handler(this, new BFWGoalSumChangedEventArgs(e.Match, e.GoalSum));
        }

        void PlaytimeEventHandler(object sender, BFPlaytimeEventArgs e)
        {
            EventHandler<BFWPlaytimeEventArgs> handler = PlaytimeEvent;
            if (handler != null)
                handler(this, new BFWPlaytimeEventArgs(e.Match, e.Playtime));
        }

        void GameEndedEventHandler(object sender, BFGameEndedEventArgs e)
        {
            m_runningTrades.Remove(((BFBT4Strategy)sender).Lay.MarketId);

            EventHandler<BFWGameEndedEventArgs> handler = GamenEndedEvent;
            if (handler != null)
                handler(this, new BFWGameEndedEventArgs(e.Match, e.Dts, e.WinLoose));
        }

        void BFLT4PNoTradePossible(object sender, BFBT4PNoTradeEventArgs e)
        {
            /*
            m_preparingTrades.Remove(e.MarketId);
            // Nachricht verbreiten
            EventHandler<BFWMessageEventArgs> message = MessageEvent;
            if (message != null)
                message(this, new BFWMessageEventArgs(DateTime.Now, e.Match, e.Reason, BackThe4.strModule));
            //throw new NotImplementedException();
             */
        }

        private void BFBetsUpdated(object sender, BetsUpdatedEventArgs e)
        {            
            if (m_buildList)
                return;
            DebugWriter.Instance.WriteMessage("Watcher BackThe4", "updateing list of trades");

            BetfairKom betfairKom = BetfairKom.Instance;

            SortedList localList = new SortedList();


            // Initial alles lesen
            MUBet[] bets = betfairKom.getBets();

            if (bets == null)
                return;

            // lokale Liste aufbauen
            foreach (MUBet bet in bets)
            {
                try
                {
                    BFBT4Strategy betSet = null;

                    // Details lesen
                    Bet betDetail = betfairKom.getBetDetail(bet.betId);

                    if (m_smm.isMarketTotalGoals(bet.marketId) == false)
                        continue;

                    if (m_smm.isMarketInplay(bet.marketId) == false)
                        continue;

                    String marketName = splitFullMarketName(betDetail.fullMarketName);

                    // Ist schon ein Eintrag für Markt in lokaler Liste vorhanden?
                    if ((betSet = (BFBT4Strategy)localList[betDetail.marketId]) != null)
                    {
                        // Wette dem Eintrag hinzufügen
                        if (betDetail.betType == BetTypeEnum.B)
                        {
                            try
                            {
                                betSet.Back.Bets.Add(betDetail.betId, betDetail);
                            }
                            catch (ArgumentException)
                            {
                                betSet.Back.Bets[betDetail.betId] = betDetail;
                            }
                            //betSet.Back = betDetail;
                        }
                        else if (betDetail.betType == BetTypeEnum.L)
                        {
                            try
                            {
                                betSet.Lay.Bets.Add(betDetail.betId, betDetail);
                            }
                            catch (ArgumentException)
                            {
                                betSet.Lay.Bets[betDetail.betId] = betDetail;
                            }
                            //betSet.Lay = betDetail;
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("Unknown Bettype:{0}", betDetail.betType.ToString());
                            throw new Exception(sb.ToString());
                        }
                    }
                    else //Eintrag noch nicht vorhanden
                    {
                        Debug.WriteLine(String.Format("{0}: Added Match {1} with Marketid {2}", DateTime.Now, marketName, betDetail.marketId));
                        betSet = new BFBT4Strategy(marketName);

                        // Wette dem Eintrag hinzufügen
                        if (betDetail.betType == BetTypeEnum.B)
                        {
                            try
                            {
                                betSet.Back.Bets.Add(betDetail.betId, betDetail);
                            }
                            catch (ArgumentException)
                            {
                                betSet.Back.Bets[betDetail.betId] = betDetail;
                            }
                            //betSet.Back = betDetail;
                        }
                        else if (betDetail.betType == BetTypeEnum.L)
                        {
                            try
                            {
                                betSet.Lay.Bets.Add(betDetail.betId, betDetail);
                            }
                            catch (ArgumentException)
                            {
                                betSet.Lay.Bets[betDetail.betId] = betDetail;
                            }
                            //betSet.Lay = betDetail;
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("Unknown Bettype:{0}", betDetail.betType.ToString());
                            throw new Exception(sb.ToString());
                        }
                       
                        localList.Add(betDetail.marketId, betSet);
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFBT4Watcher::BFBetsUpdated", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                    continue;
                }
            }

            // Mit globaler Liste abgleichen
            foreach (BFBT4Strategy betSet in localList.Values)
            //foreach (KeyValuePair<int, BFBT4Strategy> keyvalue in localList)
            {
                //BFBT4Strategy betSet = keyvalue.Value;
                // Falls keine BackWette => nicht Teil der Strategy
                if (betSet.Back.Bets.Count == 0)
                    continue;

                if (betSet.Lay.OldesDts.Ticks < betSet.Back.OldesDts.Ticks)
                    continue;

                try
                {
                    m_list.Add(betSet.Back.MarketId, betSet);
                    DebugWriter.Instance.WriteMessage("Watcher BackThe4", String.Format("Match {0} added", betSet.Match));
                    // Verknüpfung zu LiveScore mit EventHandler
                    IScore score = m_liveParser.injectBetfair(betSet.TeamA, betSet.TeamB);
                    IScore score2 = m_liveParser2.injectBetfair(betSet.TeamA, betSet.TeamB);
                    LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;


                    if (score != null)
                    {
                        DebugWriter.Instance.WriteMessage("Watcher BackThe4", String.Format("binding livescore {0} to match {0}", score.getLiveMatch(), betSet.Match));
                        score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                        //score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEvent);
                        score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                        score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                        score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);
                        betSet.Score = score;

                    }

                    if (score2 != null)
                    {
                        DebugWriter.Instance.WriteMessage("BFBT4Watcher::buildBetList", String.Format("binding livescore {0} to match {0}", score.getLiveMatch(), betSet.Match));
                        score2.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                        score2.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                        score2.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                        score2.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);

                        betSet.Score2 = score2;
                    }

                    if (score != null && score2 != null)
                    {
                        withLivescore = LIVESCOREADDED.ALL;
                    }
                    else if (score != null || score2 != null)
                    {
                        withLivescore = LIVESCOREADDED.PARTLY;
                    }

                    betSet.GameEndedEvent += new EventHandler<BFGameEndedEventArgs>(GameEndedEventHandler);
                    betSet.PlaytimeEvent += new EventHandler<BFPlaytimeEventArgs>(PlaytimeEventHandler);
                    betSet.GoalSumChangedEvent += new EventHandler<BFGoalSumChangedEventArgs>(GoalSumChangedEventHandler);
                    betSet.WinLooseChangedEvent += new EventHandler<BFWinLooseChangedEventArgs>(WinLooseChangedEventHandler);
                    betSet.MessageEvent += new EventHandler<BFMessageEventArgs>(MessageEventHandler);
                    betSet.SetCloseTradeTimer += new EventHandler<BFSetCloseTradeTimer>(betSet_SetCloseTradeTimer);
                    betSet.SetOpenBetTimer += new EventHandler<BFSetOpenBetTimer>(betSet_SetOpenBetTimer);
                    betSet.SetStopLossTimer += new EventHandler<BFSetStopLossTimer>(betSet_SetStopLossTimer);
                    betSet.StopCloseTradeTimer += new EventHandler<BFStopCloseTradeTimer>(betSet_StopCloseTradeTimer);
                    betSet.StopOpenBetTimer += new EventHandler<BFStopOpenBetTimer>(betSet_StopOpenBetTimer);
                    betSet.StopStopLossTimer += new EventHandler<BFStopStopLossTimer>(betSet_StopStopLossTimer);



                    EventHandler<BFWMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new BFWMessageEventArgs(DateTime.Now, betSet.Match, String.Format("Trade Added{0}", betSet.Match), BackThe4.strModule));


                    if (withLivescore != LIVESCOREADDED.NONE)
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                        {
                            double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                            double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                            handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore, betSet.Back.MarketId));
                        }

                        EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB), withLivescore, betSet.Back.MarketId));
                    }
                    else
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                        {
                            double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                            double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                            handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore, betSet.Back.MarketId));
                        }

                        EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWNoIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                    }
                    betSet.Start();
                    betSet.Active = this.Active;
                }
                catch (ArgumentException)
                {
                    ((BFBT4Strategy)m_list[betSet.Back.MarketId]).Back.Bets = betSet.Back.Bets;
                    ((BFBT4Strategy)m_list[betSet.Back.MarketId]).Lay.Bets = betSet.Lay.Bets; ;

                    DebugWriter.Instance.WriteMessage("Watcher BackThe4", String.Format("Match {0} Updated", betSet.Match));

                    EventHandler<BFWWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                        if (handler != null)
                            handler(this, new BFWWinLooseChangedEventArgs(betSet.Match, Math.Round(-(betSet.Lay.RiskWin - betSet.Back.RiskWin), 2), Math.Round(betSet.Lay.BetSize - betSet.Back.BetSize, 2)));
                }                
            }
            m_dtsLastUpdate = DateTime.Now;
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion
    }
}
