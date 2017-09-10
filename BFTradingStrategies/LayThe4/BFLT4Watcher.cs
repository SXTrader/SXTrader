using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.betfairif;
using System.Collections;
using System.Threading;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Diagnostics;
using BetFairIF.com.betfair.api.exchange;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.LayThe4
{

    public class BFLT4Watcher : IBFTSCommon
    {
        private SortedList m_list = null;
        private LiveScoreParser m_liveParser;
        private LiveScore2Parser m_liveParser2;
        //private Thread m_autoBetter;
        private DateTime m_dtsLastUpdate;
        private LT4ConfigurationRW m_config;
        private bool m_active;
        private bool m_autoBetterActive;
        private bool m_buildList = false;
        private SoccerMarketManager m_smm;
        private SortedList<int, BFLT4Strategy> m_runningTrades;
        //private SortedList<int, BFLT4Preparer> m_preparingTrades;

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

        public BFLT4Watcher(LiveScoreParser parser, LiveScore2Parser parser2)
        {
            m_list = new SortedList();
            m_config = new LT4ConfigurationRW();
            m_liveParser = parser;
            m_liveParser2 = parser2;
            m_smm = SoccerMarketManager.Instance;
            m_runningTrades = new SortedList<int, BFLT4Strategy>();
            //m_preparingTrades = new SortedList<int, BFLT4Preparer>();
            m_active = m_config.StrategyActivated;
            /*
            m_autoBetterActive = m_config.AutomaticTrading;
            m_autoBetter = new Thread(autoBetter);
            m_autoBetter.IsBackground = true;
            m_autoBetter.Start();
             */
            double Money = BankrollManager.Instance.MinStake;
            
            BetWatchdog.Instance.BetsUpdated += new EventHandler<BetsUpdatedEventArgs>(BFBetsUpdated);
            BetfairKom.Instance.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            BetWatchdog.Instance.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            m_liveParser.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            m_liveParser2.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
        }

        public bool AutoBetterActiv
        {
            get
            {
                return m_autoBetterActive;
            }
            set
            {
                m_autoBetterActive = value;
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
                m_active = value;

                IDictionaryEnumerator enumBetSets = m_list.GetEnumerator();
                while (enumBetSets.MoveNext() == true)
                {
                    BFLT4Strategy betSet = (BFLT4Strategy)enumBetSets.Value;
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
            LT4ConfigurationRW reader = new LT4ConfigurationRW();
            this.Active = reader.StrategyActivated;
            buildBetList();            
        }

        public bool hasLiveScore1(String match)
        {
            foreach (BFLT4Strategy betSet in m_list.Values)
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
            foreach (BFLT4Strategy betSet in m_list.Values)
            {
                if (betSet.Match == match)
                {
                    if (betSet.Score2 != null)
                        return true;
                }
            }

            return false;
        }

        private void buildBetList()
        {
            m_buildList = true;
            BetfairKom betfairKom = BetfairKom.Instance;

            SortedList localList = new SortedList();


            // Initial alles lesen
            MUBet[] bets = betfairKom.getBets();

            if (bets == null)
            {
                m_buildList = false;
                return;
            }

            // lokale Liste aufbauen
            foreach (MUBet bet in bets)
            {
                try
                {
                    BFLT4Strategy betSet = null;

                    // Details lesen
                    Bet betDetail = betfairKom.getBetDetail(bet.betId);

                    if (m_smm.isMarketTotalGoals(bet.marketId) == false)
                        continue;

                    if (m_smm.isMarketInplay(bet.marketId) == false)
                        continue;

                    String marketName = splitFullMarketName(betDetail.fullMarketName);

                    // Ist schon ein Eintrag für Markt in lokaler Liste vorhanden?
                    if ((betSet = (BFLT4Strategy)localList[betDetail.marketId]) != null)
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
                        betSet = new BFLT4Strategy(marketName);

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
                        handler(this, new SXExceptionMessageEventArgs("BFLT4Watcher::buildBetList", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                    continue;
                }
            }

            // Mit globaler Liste abgleichen
            foreach (BFLT4Strategy betSet in localList.Values)
            {
                //BFLT4Strategy betSet = (BFLT4Strategy)value;
                // Falls keine LayWette => nicht Teil der Strategy
                if (betSet.Lay.Bets.Count == 0)
                    continue;

                if (betSet.Back.OldesDts < betSet.Lay.OldesDts)
                    continue;

                try
                {
                    m_list.Add(betSet.Lay.MarketId, betSet);

                    // Verknüpfung zu LiveScore mit EventHandler
                    IScore score = m_liveParser.injectBetfair(betSet.TeamA, betSet.TeamB);
                    IScore score2 = m_liveParser2.injectBetfair(betSet.TeamA, betSet.TeamB);
                    LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;

                    if (score != null)
                    {
                        score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                        //score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEvent);
                        score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                        score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                        score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);
                        betSet.Score = score;
                    }

                    if (score2 != null)
                    {
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
                        message(this, new BFWMessageEventArgs(DateTime.Now, betSet.Match, LayThe4.strTradedAddedAndStarted, LayThe4.strModule));
                    if (withLivescore != LIVESCOREADDED.NONE)
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                        {
                            double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                            double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                            handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore, betSet.Lay.MarketId));
                        }

                        EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB), withLivescore, betSet.Lay.MarketId));
                    }
                    else
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                        {
                            double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                            double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                            handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore, betSet.Lay.MarketId));
                        }

                        EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWNoIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                    }

                    /*
                    EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                    if (handler != null)
                    {
                        double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                        double backLost = betSet.Lay.BetSize - betSet.Back.BetSize;
                        // handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), backWin.ToString(), backLost.ToString(), withLivescore));
                    }

                    if (score != null)
                    {
                        EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                    }
                    else
                    {
                        EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWNoIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                    }
                     * */
                }
                catch(ArgumentException)
                {
                    ((BFLT4Strategy)m_list[betSet.Lay.MarketId]).Back.Bets = betSet.Back.Bets;
                    ((BFLT4Strategy)m_list[betSet.Lay.MarketId]).Lay.Bets = betSet.Lay.Bets;
                }                

                betSet.Start();                
            }
            localList.Clear();
            m_buildList = false;
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

        public LT4ConfigurationRW getConfiguration(String match)
        {
            foreach (BFLT4Strategy strategy in m_list.Values)
            {
                if (strategy.Match.Equals(match))
                    return strategy.Configuration;
            }

            return null;
        }

        public void setConfiguration(String match, LT4ConfigurationRW config)
        {
            foreach (BFLT4Strategy strategy in m_list.Values)
            {
                if (strategy.Match.Equals(match))
                    strategy.Configuration = config;
            }
        }



        private String splitFullMarketName(String fullmarketname)
        {
            char[] cSeps = { '/' };
            String[] splittedMarketName = fullmarketname.Split(cSeps);
            splittedMarketName[1] = splittedMarketName[1].Replace(" v ", " - ");
            return splittedMarketName[1];
        }

        public void manualBFLSLink2(String match, IScore score)
        {
            LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;
            int marketId = 0;

            foreach (BFLT4Strategy betSet in m_list.Values)
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
                    betSet.Score2 = score;
                   // betSet.initRiskWin();
                    betSet.Active = this.Active;


                    // Lokale XML-Datei laden
                    if (score.TeamA != "Team None" && score.TeamB != "Team None")
                        LiveScore2Parser.Instance.writeLocalXml(betSet.Match, score);
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
            foreach (BFLT4Strategy betSet in m_list.Values)
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
                        LiveScoreParser.Instance.writeLocalXml(betSet.Match, score);
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

            foreach (BFLT4Strategy betSet in m_list.Values)
            {
                if (score != null && betSet.Match == match)
                {
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
            int waitTime = 0;
            int i = 0;
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

                // Falls kein automtisches Trading => weiter;
                if (!m_autoBetterActive)
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

                    BFLT4Preparer preparer = new BFLT4Preparer(market, m_liveParser);
                    preparer.NoTradePossible += new EventHandler<BFLT4PNoTradeEventArgs>(BFLT4PNoTradePossible);
                    preparer.TradeStarted += new EventHandler<BFLT4PTradeStartedEventArgs>(BFLT4PTradeStarted);
                    Debug.WriteLine(String.Format("Added match {0} to Preparer. MarketId {1}", market.Match, market.Id));

                    m_preparingTrades.Add(market.Id, preparer);
                    preparer.Start();
                }

            }
             */
        }

        void BFLT4PTradeStarted(object sender, BFLT4PTradeStartedEventArgs e)
        {
            /*
            m_preparingTrades.Remove(e.Bet.marketId);
            BFLT4Strategy strategy = new BFLT4Strategy(e.Bet,e.Match);
            String[] sepMatch = {" - "};
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

            m_runningTrades.Add(e.Bet.marketId, strategy);
            // Nachricht verbreiten
            EventHandler<BFWMessageEventArgs> message = MessageEvent;
            if (message != null)
                message(this, new BFWMessageEventArgs(DateTime.Now, e.Match, LayThe4.strTradedAddedAndStarted, LayThe4.strModule));

            EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
            if (handler != null)
            {
                double backWin = strategy.Back.RiskWin - strategy.Lay.RiskWin;
                double backLost = strategy.Lay.BetSize - strategy.Back.BetSize;
                handler(this, new MatchAddedEventArgs(e.Match, strategy.SumGoals.ToString(), backWin.ToString(), backLost.ToString()));
            }
            strategy.Start();
            // TODO: GameAddedEvent an Host
            //throw new NotImplementedException();
             */
        }

        void Instance_ExceptionMessageEvent(object sender, SXExceptionMessageEventArgs e)
        {

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(e));
            }
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
            m_runningTrades.Remove(((BFLT4Strategy)sender).Lay.MarketId);

            EventHandler<BFWGameEndedEventArgs> handler = GamenEndedEvent;
            if (handler != null)
                handler(this, new BFWGameEndedEventArgs(e.Match, e.Dts, e.WinLoose));
        }

        void BFLT4PNoTradePossible(object sender, BFLT4PNoTradeEventArgs e)
        {
            /*
            m_preparingTrades.Remove(e.MarketId);
            // Nachricht verbreiten
            EventHandler<BFWMessageEventArgs> message = MessageEvent;
            if (message != null)
                message(this, new BFWMessageEventArgs(DateTime.Now, e.Match,e.Reason, LayThe4.strModule));
            //throw new NotImplementedException();
             */
        }

        private void BFBetsUpdated(object sender, BetsUpdatedEventArgs e)
        {
            if (m_buildList)
                return;

            DebugWriter.Instance.WriteMessage("Watcher LayThe4", "updateing list of trades");
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
                    BFLT4Strategy betSet = null;

                    // Details lesen
                    Bet betDetail = betfairKom.getBetDetail(bet.betId);

                    if (m_smm.isMarketTotalGoals(bet.marketId) == false)
                        continue;

                    if (m_smm.isMarketInplay(bet.marketId) == false)
                        continue;

                    String marketName = splitFullMarketName(betDetail.fullMarketName);

                    // Ist schon ein Eintrag für Markt in lokaler Liste vorhanden?
                    if ((betSet = (BFLT4Strategy)localList[betDetail.marketId]) != null)
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
                        betSet = new BFLT4Strategy(marketName);

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

                        // Verknüpfung zu LiveScore mit EventHandler
                        

                        
                        localList.Add(betDetail.marketId, betSet);
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFLT4Watcher::BFBetsUpdated", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                    continue;
                }
            }

            // Mit globaler Liste abgleichen
            foreach (BFLT4Strategy betSet in localList.Values)
            //foreach (KeyValuePair<int, BFLT4Strategy> keyvalue in localList)
            {
                //BFLT4Strategy betSet = keyvalue.Value;
                // Falls keine LayWette => nicht Teil der Strategy
                if (betSet.Lay.Bets.Count == 0)
                    continue;
                
                if (betSet.Back.OldesDts < betSet.Lay.OldesDts)
                    continue;

                try
                {
                    m_list.Add(betSet.Lay.MarketId, betSet);
                    DebugWriter.Instance.WriteMessage("Watcher LayThe4", String.Format("Match {0} added", betSet.Match));

                    IScore score = m_liveParser.injectBetfair(betSet.TeamA, betSet.TeamB);
                    IScore score2 = m_liveParser2.injectBetfair(betSet.TeamA, betSet.TeamB);
                    LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;

                    if (score != null)
                    {
                        DebugWriter.Instance.WriteMessage("Watcher LayThe4", String.Format("binding livescore {0} to match {0}", score.getLiveMatch(), betSet.Match));
                        score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeTick);
                        //score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEvent);
                        score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                        score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                        score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);
                        betSet.Score = score;
                    }

                    if (score2 != null)
                    {
                        DebugWriter.Instance.WriteMessage("BFLT4Watcher::buildBetList", String.Format("binding livescore {0} to match {0}", score.getLiveMatch(), betSet.Match));
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
                        message(this, new BFWMessageEventArgs(DateTime.Now, betSet.Match, String.Format("Trade Added{0}", betSet.Match), LayThe4.strModule));


                    if (withLivescore != LIVESCOREADDED.NONE)
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                        {
                            double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                            double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                            handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore, betSet.Lay.MarketId));
                        }

                        EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB), withLivescore, betSet.Lay.MarketId));
                    }
                    else
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                        {
                            double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                            double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                            handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore, betSet.Lay.MarketId));
                        }

                        EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWNoIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                    }

                    /*
                    EventHandler<BFWMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new BFWMessageEventArgs(DateTime.Now, betSet.Match, LayThe4.strTradedAddedAndStarted, LayThe4.strModule));

                    EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                    if (handler != null)
                    {
                        double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                        double backLost = betSet.Lay.BetSize - betSet.Back.BetSize;
                        //handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), backWin.ToString(), backLost.ToString(), withLivescore));
                    }
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
                            handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore));
                        }

                        EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB), withLivescore));
                    }
                    else
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                        {
                            double backWin = betSet.Back.RiskWin - betSet.Lay.RiskWin;
                            double backLoss = betSet.Lay.BetSize - betSet.Back.BetSize;
                            handler(this, new MatchAddedEventArgs(betSet.Match, betSet.SumGoals.ToString(), Math.Round(backWin, 2).ToString(), Math.Round(backLoss, 2).ToString(), withLivescore));
                        }

                        EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWNoIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                    }
                     * 
                    if (score != null)
                    {
                        EventHandler<BFWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                    }
                    else
                    {
                        EventHandler<BFWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new BFWNoIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
                    }
                     * */
                    betSet.Active = this.Active;
                }
                catch (ArgumentException)
                {
                    ((BFLT4Strategy)m_list[betSet.Lay.MarketId]).Back.Bets = betSet.Back.Bets;
                    ((BFLT4Strategy)m_list[betSet.Lay.MarketId]).Lay.Bets = betSet.Lay.Bets;

                    DebugWriter.Instance.WriteMessage("Watcher LayThe4", String.Format("Match {0} Updated", betSet.Match));

                    EventHandler<BFWWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                    if (handler != null)
                        handler(this, new BFWWinLooseChangedEventArgs(betSet.Match, Math.Round(-(betSet.Lay.RiskWin - betSet.Back.RiskWin), 2), Math.Round(betSet.Lay.BetSize - betSet.Back.BetSize, 2)));
                }
               
                betSet.Start();
            }
            m_dtsLastUpdate = DateTime.Now;
            localList.Clear();
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion
    }
}
