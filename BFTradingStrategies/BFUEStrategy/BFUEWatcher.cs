using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.livescoreparser;
//using BFUEStrategy;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.enums;
using net.sxtrader.muk.interfaces;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.muk;
using System.Collections.Concurrent;


namespace net.sxtrader.bftradingstrategies.bfuestrategy
{

    public class BFWMatchNotFoundEventArgs : EventArgs
    {
        private BFUEStrategy m_betset;

        public BFUEStrategy BetSet
        {
            get
            {
                return m_betset;
            }
        }

        public BFWMatchNotFoundEventArgs(BFUEStrategy betset)
        {
            m_betset = betset;
        }
    }

    
    public class BFWRiskWinChangedEventArgs : EventArgs
    {
        private string m_match;
        private double m_backGuV;
        private double m_layGuV;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public double BackGuV
        {
            get
            {
                return m_backGuV;
            }
        }

        public double LayGuV
        {
            get
            {
                return m_layGuV;
            }
        }

        public BFWRiskWinChangedEventArgs(string match, double backGuV, double layGuV)
        {
            m_match = match;
            m_backGuV = backGuV;
            m_layGuV = layGuV;
        }
    }
    
    public class BFWGoalScoredEventArgs : EventArgs
    {

        private int m_la;
        private int m_lb;
        private String m_team;

        public SCORESTATE ScoreState
        {
            get
            {
                if (m_la == 0 && m_lb == 0)
                    return SCORESTATE.initdraw;
                else if (m_la == m_lb)
                    return SCORESTATE.draw;
                else
                    return SCORESTATE.undraw;

            }
        }

        public String Team
        {
            get
            {
                return m_team;
            }
        }

        public int ScoreA
        {
            get
            {
                return m_la;
            }
        }

        public int ScoreB
        {
            get
            {
                return m_lb;
            }
        }

        public String Score
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} - {1}", m_la, m_lb);
                return sb.ToString();
            }
        }

        public BFWGoalScoredEventArgs(string team, int la, int lb)
        {
            m_team = team;
            m_la = la;
            m_lb = lb;
        }
    }

    
    public class MatchAddedEventArgs : EventArgs
    {
        private string m_match;
        private long m_marketId;
        //private string m_money;
        private double m_backGuV;
        private double m_layGuV;
        private string m_score;
        private LIVESCOREADDED m_withLivescore;

        public long MarketId
        {
            get
            {
                return m_marketId;
            }
        }

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public double BackGuV
        {
            get
            {
                return m_backGuV;
            }
        }

        public double LayGuV
        {
            get
            {
                return m_layGuV;
            }
        }

        public String Score
        {
            get
            {
                return m_score;
            }
        }
   

        public LIVESCOREADDED WithLivescore
        {
            get
            {
                return m_withLivescore;
            }
        }

        public MatchAddedEventArgs(String teamA, String teamB, String score, double backGuV, double layGuV, LIVESCOREADDED state, long marketId)
        {
            m_match = String.Format("{0} - {1}", teamA, teamB);
            m_marketId = marketId;
            m_score = score;
            m_backGuV = backGuV;
            m_layGuV = layGuV;
            m_withLivescore = state;
        }
    }
    

    public class BFUEWatcher : IBFTSCommon
    {
        private SortedList<long, BFUEStrategy> m_list = null;
        
        private bool m_buildList = false;
        private LiveScoreParser m_liveParser;
        private LiveScore2Parser m_liveParser2;
//        private bool m_stopUpdater;
        private bool m_active;
        private DateTime m_dtsLastUpdate;
//        private Thread m_threadUpdater;
        private SXALSoccerMarketManager m_smm;

        private static volatile BFUEWatcher instance;
        private static Object syncRoot = "BFUEWatcher";

        private ConcurrentQueue<InsertQueueItem> _insertQueue;
        private System.Timers.Timer _queueTimer; 

        public static BFUEWatcher getInstance(LiveScoreParser parser, LiveScore2Parser parser2)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new BFUEWatcher(parser, parser2);
                }
            }

            return instance;
        }

        public LTDConfigurationRW getConfiguration(String match)
        {
            foreach (BFUEStrategy strategy in m_list.Values)
            {
                if (strategy.Match.Equals(match))
                {
                    return strategy.Configuration;
                }
            }

            return null;
        }

        public void setConfiguration(String match, LTDConfigurationRW config)
        {
            try
            {
                foreach (long id in m_list.Keys)
                {
                    BFUEStrategy strategy = (BFUEStrategy)m_list[id];
                    if (strategy.Match.Equals(match))
                    {
                        strategy.Configuration = config;
                        m_list[id] = strategy;
                        return;
                    }
                }
            }
            catch (Exception exc)
            {
                
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
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
                try
                {
                    m_active = value;
                    IDictionaryEnumerator enumBetSets = (IDictionaryEnumerator)m_list.GetEnumerator();
                    
                    while (enumBetSets.MoveNext() == true)
                    {
                        BFUEStrategy betSet = (BFUEStrategy)enumBetSets.Value;
                        betSet.Active = m_active;
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXWMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new SXWMessageEventArgs(DateTime.Now, "Common", "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        public SortedList<long, BFUEStrategy> BetSet
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

        #region Events
        public event EventHandler<BFWGoalScoredEventArgs> GoalScoredEvent;
        public event EventHandler<SXWPlaytimeEventArgs> PlaytimeEvent;
        public event EventHandler<BFWRiskWinChangedEventArgs> RiskWinChangedEvent;
        public event EventHandler<SXWMessageEventArgs> MessageEvent;
        public event EventHandler<SXWGameEndedEventArgs> GamenEndedEvent;
        public event EventHandler<SXWNoIScoreAddedEventArgs> NoIScoreAdded;
        public event EventHandler<SXWIScoreAddedEventArgs> IScoreAdded;
        public event EventHandler<SXWSetCloseTradeTimer> SetCloseTradeTimer;
        public event EventHandler<SXWSetOpenBetTimer> SetOpenBetTimer;
        public event EventHandler<SXWSetStopLossTimer> SetStopLossTimer;
        public event EventHandler<SXWStopCloseTradeTimer> StopCloseTradeTimer;
        public event EventHandler<SXWStopOpenBetTimer> StopOpenBetTimer;
        public event EventHandler<SXWStopStopLossTimer> StopStopLossTimer;
        public event EventHandler<SXWManualTradeRemoveEventArgs> ManualTradeRemove;
        
//        public static event EventHandler<BFWMatchNotFoundEventArgs> MatchNotFoundEvent;
        public static event EventHandler<MatchAddedEventArgs> MatchAddedEvent;
        #endregion


        private BFUEWatcher()
        {
        }
        private BFUEWatcher(LiveScoreParser parser, LiveScore2Parser parser2)
        {
            m_list = new SortedList<long, BFUEStrategy>();
            _insertQueue = new ConcurrentQueue<InsertQueueItem>();
            _queueTimer = new System.Timers.Timer(10000);
            _queueTimer.AutoReset = false;
            _queueTimer.Elapsed += _queueTimer_Elapsed;
            _queueTimer.Start();

            m_liveParser = parser;
            m_liveParser2 = parser2;
            m_dtsLastUpdate = DateTime.MinValue;
            m_smm = SXALSoccerMarketManager.Instance;
            SXALBetWatchdog.Instance.BetsUpdated += new EventHandler<SXALBetsUpdatedEventArgs>(BFBetsUpdated);

            SXALKom.Instance.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            SXALBetWatchdog.Instance.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            m_liveParser.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);
            m_liveParser2.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(Instance_ExceptionMessageEvent);

            SX5MinutePulse.Instance.Pulse += Instance_Pulse;
//            buildBetList();
        }

        private void _queueTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           try
           {
               processInsertBetQueue();
           }
           catch(Exception exc)
           {
               ExceptionWriter.Instance.WriteException(exc);
           }
           finally
           {
               //timer erneut Starten
               _queueTimer.Start();
           }
        }

        private void processInsertBetQueue()
        {
            //Nach MarktId sortierte Wetten, getrennt in Back und Lay
            SortedListQueueWorkItems theList = buildProcessList();
            processWorkItems(theList);
        }

        private void processWorkItems(SortedListQueueWorkItems theList)
        {
            DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Anzahl der zu verarbeiteten Worklistitems: {0}", theList.Count));
            foreach(long marketId in theList.Keys)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Verarbeite Markt {0}", marketId));
                QueueWorkItem item = theList[marketId];
                if(item.Lays.Bets.Count == 0)
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Keine Laywette zu Markt {0} in Worklistitem => Überspringe", marketId));
                    continue;
                }
                if(m_list.ContainsKey(marketId))
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Bereits ein Trade für Markt {0} vorhanden => Aktualisiere", marketId));
                    updateTrade(marketId, item);
                }
                else
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Kein Trade für Markt {0} vorhanden => Erzeugen", marketId));
                    insertTrade(marketId, item);
                }
            }
        }

        private void updateTrade(long marketId, QueueWorkItem item)
        {
            if(item.Configuration != null)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Aktualisiere Konfiguration für Trade {0}", m_list[marketId].Match));
                m_list[marketId].Configuration = item.Configuration;
            }

            if(item.Ticker != null)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Aktualisiere Ticker für Trade {0}", m_list[marketId].Match));
                m_list[marketId].Score = item.Ticker;
            }

            foreach(SXALBet b in item.Backs.Bets.Values)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Aktualisieren/Hinzufügen von Wette {0} zu Trade {1}",b.BetId, m_list[marketId].Match));
                m_list[marketId].addBet(b);
            }

            foreach (SXALBet b in item.Lays.Bets.Values)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Aktualisieren/Hinzufügen von Wette {0} zu Trade {1}", b.BetId, m_list[marketId].Match));
                m_list[marketId].addBet(b);
            }
        }

        private void insertTrade(long marketId, QueueWorkItem item)
        {
            if (String.IsNullOrEmpty(item.Lays.Bets.Values[0].FullMarketName))
                item.Lays.Bets.Values[0].FullMarketName = SXALSoccerMarketManager.Instance.getMarketById(item.Lays.Bets.Values[0].MarketId, false).Match;

            String marketName = splitFullMarketName(item.Lays.Bets.Values[0].FullMarketName);
            BFUEStrategy trade = new BFUEStrategy(null, null, marketName);

            foreach (SXALBet b in item.Backs.Bets.Values)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Aktualisieren/Hinzufügen von Wette {0} zu Trade {1}", b.BetId, trade.Match));
                trade.addBet(b);
            }

            foreach (SXALBet b in item.Lays.Bets.Values)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Aktualisieren/Hinzufügen von Wette {0} zu Trade {1}", b.BetId,trade.Match));
                trade.addBet(b);
            }


            //Liveticker
            IScore score = null;
            if (item.Ticker != null)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Erzeugen Trade {0}: Liveticker vorgegeben ", trade.Match));
                score = item.Ticker;
            }
            else
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Erzeugen Trade {0}: Liveticker vorgegeben nicht vorgegeben. Versuche automatisches Verbinden", trade.Match));
                score = linkLiveScore(trade.TeamA, trade.TeamB);
            }
            
            if (score != null)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Trade {0}: Verlinke Liveticker-Ereignisse", trade.Match));
                score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(trade.PlaytimeEventHandler);
                score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(trade.GoalEventHandler);
                score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(trade.GoalBackEventHandler);
                score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(trade.GameEndedEventHandler);

                trade.Score = score;
            }

            LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;

            if (((HLLiveScore)score).IsScore1Connected() && ((HLLiveScore)score).IsScore2Connected())
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Trade {0}: Alle Liveticker verbunden", trade.Match));
                withLivescore = LIVESCOREADDED.ALL;
            }
            else if (((HLLiveScore)score).IsScore1Connected() || ((HLLiveScore)score).IsScore2Connected())
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Trade {0}: Liveticker teilweise verbunden", trade.Match));
                withLivescore = LIVESCOREADDED.PARTLY;
            }
            else
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Trade {0}: Kein Liveticker verbunden", trade.Match));
            }

            DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Trade {0}: Verlinke Trade-Ereignisse", trade.Match));
            trade.GoalScoredEvent += new EventHandler<BFGoalScoredEventArgs>(BFGoalScoredHandler);
            trade.PlaytimeEvent += new EventHandler<SXPlaytimeEventArgs>(BFPlaytimeHandler);
            trade.RiskWinChangedEvent += new EventHandler<BFRiskWinChangedEventArgs>(BFRiskWinChangedHandler);
            trade.MessageEvent += new EventHandler<SXMessageEventArgs>(BFMessageHandler);
            trade.GameEndedEvent += new EventHandler<SXGameEndedEventArgs>(BFGameEndedHandler);
            trade.SetCloseTradeTimer += new EventHandler<SXSetCloseTradeTimer>(betSet_SetCloseTradeTimer);
            trade.SetOpenBetTimer += new EventHandler<SXSetOpenBetTimer>(betSet_SetOpenBetTimer);
            trade.SetStopLossTimer += new EventHandler<SXSetStopLossTimer>(betSet_SetStopLossTimer);
            trade.StopCloseTradeTimer += new EventHandler<SXStopCloseTradeTimer>(betSet_StopCloseTradeTimer);
            trade.StopOpenBetTimer += new EventHandler<SXStopOpenBetTimer>(betSet_StopOpenBetTimer);
            trade.StopStopLossTimer += new EventHandler<SXStopStopLossTimer>(betSet_StopStopLossTimer);
            trade.ManualTradeRemove += trade_ManualTradeRemove;

            // Nachricht verbreiten
            EventHandler<SXWMessageEventArgs> message = MessageEvent;
            if (message != null)
                message(this, new SXWMessageEventArgs(DateTime.Now, trade.Match, LayTheDraw.strAdded, LayTheDraw.strModule));

            if (withLivescore != LIVESCOREADDED.NONE)
            {
                //IScore tmpScore = score!=null?score:score2;
                EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                if (handler != null)
                    handler(this, new MatchAddedEventArgs(trade.TeamA, trade.TeamB, score.getScore(), Math.Round(trade.Back.RiskWin - trade.Lay.RiskWin, 2),
                        Math.Round(trade.Lay.BetSize - trade.Back.BetSize, 2), withLivescore, trade.Lay.MarketId));

                EventHandler<SXWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new SXWIScoreAddedEventArgs(String.Format("{0} - {1}", trade.TeamA, trade.TeamB), withLivescore, trade.Lay.MarketId));
            }
            else
            {
                EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                if (handler != null)
                    handler(this, new MatchAddedEventArgs(trade.TeamA, trade.TeamB, "0 - 0", Math.Round(trade.Back.RiskWin - trade.Lay.RiskWin, 2),
                        Math.Round(trade.Lay.BetSize - trade.Back.BetSize, 2), withLivescore, trade.Lay.MarketId));

                EventHandler<SXWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new SXWNoIScoreAddedEventArgs(String.Format("{0} - {1}", trade.TeamA, trade.TeamB)));
            }

            DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Trade {0}: Füge Trade der Tradeliste hinzu", trade.Match));
            m_list.Add(trade.Lay.MarketId, trade);
            trade.initRiskWin();
            if(item.Configuration != null)
            {
                DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Trade {0}: Konfiguration vorgegeben. Aktualisiere Konfiguration des Trades", trade.Match));
                trade.Configuration = item.Configuration;
            }
            trade.Active = this.Active;
        }

        void trade_ManualTradeRemove(object sender, SXManualTradeRemoveEventArgs e)
        {
            if(m_list.ContainsKey(e.MarketId))
            {
                m_list.Remove(e.MarketId);
            }
            EventHandler<SXWManualTradeRemoveEventArgs> handler = ManualTradeRemove;
            if (handler != null)
                handler(this, new SXWManualTradeRemoveEventArgs(e.Match, e.Dts, e.MarketId));
        }

        private SortedListQueueWorkItems buildProcessList()
        {
            SortedListQueueWorkItems theList = new SortedListQueueWorkItems();
            InsertQueueItem queueItem = null;
            while (_insertQueue.TryDequeue(out queueItem))
            {
                // Nur vom Type Odds nehmen 
                if (queueItem.Bet.MarketType != SXALMarketTypeEnum.O)
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1} ist kein Match Odds => Überspringe", queueItem.Bet.BetId, queueItem.Bet.FullMarketName));
                    continue;
                }

                if (m_smm.isMarketMatchOdds(queueItem.Bet.MarketId) == false)
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1} ist kein Match Odds => Überspringe", queueItem.Bet.BetId, queueItem.Bet.FullMarketName));
                    continue;
                }

                if (m_smm.isMarketInplay(queueItem.Bet.MarketId) == false)
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Markt zu Wette {0} zu {1} ist kein Inplay => Überspringe", queueItem.Bet.BetId, queueItem.Bet.FullMarketName));
                    continue;
                }
                //ist schon ein Eintrag zu diesen Markt in der Liste vorhanden?
                if (theList.ContainsKey(queueItem.Bet.MarketId))
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Zu Markt {0} bereits eine Prozessliste vorhandne => Aktualisiere", queueItem.Bet.MarketId));
                    long marketId = queueItem.Bet.MarketId;
                    //ist die Wette schon vorhanden?
                    if (queueItem.Bet.BetType == SXALBetTypeEnum.B)
                    {
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1} ist Backwette", queueItem.Bet.BetId, queueItem.Bet.FullMarketName));
                        if (theList[marketId].Backs.Bets.ContainsKey(queueItem.Bet.BetId))
                        {
                            DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} bereits vorhanden => Aktualisiere", queueItem.Bet.BetId));
                            //Wette aktualisieren
                            theList[marketId].Backs.Bets[queueItem.Bet.BetId] = queueItem.Bet;
                        }
                        else
                        {
                            DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} nicht vorhanden => hinzufügen", queueItem.Bet.BetId));
                            //Wette hinzufügen
                            theList[marketId].Backs.Bets.Add(queueItem.Bet.BetId, queueItem.Bet);
                        }
                    }
                    else
                    {
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1} ist Laywette", queueItem.Bet.BetId, queueItem.Bet.FullMarketName));
                        if (theList[marketId].Lays.Bets.ContainsKey(queueItem.Bet.BetId))
                        {
                            DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} bereits vorhanden => Aktualisiere", queueItem.Bet.BetId));
                            //Wette aktualisieren
                            theList[marketId].Lays.Bets[queueItem.Bet.BetId] = queueItem.Bet;
                        }
                        else
                        {
                            DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} nicht vorhanden => hinzufügen", queueItem.Bet.BetId));
                            //Wette hinzufügen
                            theList[marketId].Lays.Bets.Add(queueItem.Bet.BetId, queueItem.Bet);
                        }
                    }

                    //Konfiguration und Ticker
                    if (queueItem.Configuration != null)
                    {
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0}. Aktualisieren Konfiguration", queueItem.Bet.BetId));
                        theList[marketId].Configuration = queueItem.Configuration;
                    }

                    if (queueItem.Ticker != null)
                    {
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0}. Aktualisieren Ticker", queueItem.Bet.BetId));
                        theList[marketId].Ticker = queueItem.Ticker;
                    }
                }
                else
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Zu Markt {0} keine Prozessliste vorhanden => Hinzufügen", queueItem.Bet.MarketId));
                    //Kein Eintrag vorhandne => Hinzufügen
                    QueueWorkItem workItem = new QueueWorkItem();
                    if (queueItem.Bet.BetType == SXALBetTypeEnum.B)
                    {
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1} ist Backwette", queueItem.Bet.BetId, queueItem.Bet.FullMarketName));
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} nicht vorhanden => hinzufügen", queueItem.Bet.BetId));
                        workItem.Backs.Bets.Add(queueItem.Bet.BetId, queueItem.Bet);
                    }
                    else
                    {
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1} ist Laywette", queueItem.Bet.BetId, queueItem.Bet.FullMarketName));
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0} nicht vorhanden => hinzufügen", queueItem.Bet.BetId));
                        workItem.Lays.Bets.Add(queueItem.Bet.BetId, queueItem.Bet);
                    }

                    if (queueItem.Configuration != null)
                    {
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0}. Aktualisieren Konfiguration", queueItem.Bet.BetId));
                        workItem.Configuration = queueItem.Configuration;
                    }

                    if (queueItem.Ticker != null)
                    {
                        DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Wette {0}. Aktualisieren Ticker", queueItem.Bet.BetId));
                        workItem.Ticker = queueItem.Ticker;
                    }
                    theList.Add(queueItem.Bet.MarketId, workItem);
                }
            }
            return theList;
        }

        private void Instance_Pulse(object sender, EventArgs e)
        {
            //Durchsuche die Liste aller Trades und entferne diejenigen, deren Startzeit länger als 4 Stunden zurück liegt.
            for (int i = 0; i < m_list.Values.Count; i++)
            {
                BFUEStrategy trade = m_list.Values[i];//(BFUEStrategy)m_list.GetByIndex(i);
                
                if (DateTime.Now.Subtract(trade.Score.StartDTS).TotalHours > 4)
                {
                    
                    EventHandler<SXWGameEndedEventArgs> handler = GamenEndedEvent;
                    if (handler != null)
                        handler(this, new SXWGameEndedEventArgs(trade.Match, trade.Score.StartDTS, 0.0/*e.WinLoose*/));
                     
                    m_list.RemoveAt(i--);
                    trade.Dispose();
                }
            }
        }

        public void checkForTrades()
        {
            ThreadPool.QueueUserWorkItem(this.buildBetList);
            //buildBetList();
        }

        public bool hasLiveScore1(String match)
        {
            try
            {
                foreach (BFUEStrategy betSet in m_list.Values)
                {
                    if (betSet.Match == match)
                    {
                        HLLiveScore hlLiveScore = (HLLiveScore)betSet.Score;
                        if (hlLiveScore != null && hlLiveScore.IsScore2Connected())
                            return true;
                    }
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
            
            return false;
        }

        public bool hasLiveScore2(String match)
        {
            try
            {
                foreach (BFUEStrategy betSet in m_list.Values)
                {
                    if (betSet.Match == match)
                    {
                        HLLiveScore hlLiveScore = (HLLiveScore)betSet.Score;
                        if (hlLiveScore != null && hlLiveScore.IsScore2Connected())
                            //if (betSet.((HLLiveScore)Score) != null)
                            return true;
                    }
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
            return false;
        }


        public void manualBFLSLink2(String match, IScore score)
        {
            try
            {
                LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;
                long marketId = 0;
                foreach (BFUEStrategy betSet in m_list.Values)
                {
                    if (score != null && betSet.Match == match)
                    {
                        score.IncreaseRef();
                        score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeEventHandler);
                        score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                        score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                        score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);

                        ((HLLiveScore)betSet.Score).Score2 = score;
                        betSet.initRiskWin();
                        betSet.Active = this.Active;

                        // Lokale XML-Datei laden
                        if (score.TeamA != "Team None" && score.TeamB != "Team None")
                            LiveScore2Parser.writeLocalXml(betSet.Match, score);
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
                    EventHandler<SXWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                    if (addedHandler != null)
                        addedHandler(this, new SXWIScoreAddedEventArgs(match, withLivescore, marketId));
                }
                else
                {
                    EventHandler<SXWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                    if (addedHandler != null)
                        addedHandler(this, new SXWNoIScoreAddedEventArgs(match));
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }



        public void manualBFLSLink(String match, IScore score)
        {
            try
            {
                LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;
                long marketId = 0;

                foreach (BFUEStrategy betSet in m_list.Values)
                {
                    if (score != null && betSet.Match == match)
                    {
                        score.IncreaseRef();
                        score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(betSet.PlaytimeEventHandler);
                        score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(betSet.GoalEventHandler);
                        score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(betSet.GoalBackEventHandler);
                        score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(betSet.GameEndedEventHandler);

                        ((HLLiveScore)betSet.Score).Score1 = score;
                        betSet.initRiskWin();
                        betSet.Active = this.Active;

                        // Lokale XML-Datei laden
                        if (score.TeamA != "Team None" && score.TeamB != "Team None")
                            LiveScoreParser.WriteLocalXml(betSet.Match, score);
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
                    EventHandler<SXWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                    if (addedHandler != null)
                        addedHandler(this, new SXWIScoreAddedEventArgs(match, withLivescore, marketId));
                }
                else
                {
                    EventHandler<SXWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                    if (addedHandler != null)
                        addedHandler(this, new SXWNoIScoreAddedEventArgs(match));
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public void disconnectLS1(String match)
        {
            try
            {
                LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;
                long marketId = 0;

                foreach (BFUEStrategy strategy in m_list.Values)
                {
                    if (strategy.Match.Equals(match))
                    {
                        marketId = strategy.Lay.MarketId;
                        //((HLLiveScore)strategy.Score).Score1.DecreaseRef();
                        ((HLLiveScore)strategy.Score).Score1 = null;
                        break;
                    }
                }

                if (hasLiveScore1(match) && hasLiveScore2(match))
                    withLivescore = LIVESCOREADDED.ALL;
                else if (hasLiveScore1(match) || hasLiveScore2(match))
                    withLivescore = LIVESCOREADDED.PARTLY;
                else
                    withLivescore = LIVESCOREADDED.NONE;

                if (withLivescore != LIVESCOREADDED.NONE)
                {
                    EventHandler<SXWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                    if (addedHandler != null)
                        addedHandler(this, new SXWIScoreAddedEventArgs(match, withLivescore, marketId));
                }
                else
                {
                    EventHandler<SXWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                    if (addedHandler != null)
                        addedHandler(this, new SXWNoIScoreAddedEventArgs(match));
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public void disconnectLS2(String match)
        {
            try
            {
                LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;
                long marketId = 0;

                foreach (BFUEStrategy strategy in m_list.Values)
                {
                    if (strategy.Match.Equals(match))
                    {
                        marketId = strategy.Lay.MarketId;
                        //strategy.Score2.DecreaseRef();
                        ((HLLiveScore)strategy.Score).Score2 = null;
                        break;
                    }
                }

                if (hasLiveScore1(match) && hasLiveScore2(match))
                    withLivescore = LIVESCOREADDED.ALL;
                else if (hasLiveScore1(match) || hasLiveScore2(match))
                    withLivescore = LIVESCOREADDED.PARTLY;
                else
                    withLivescore = LIVESCOREADDED.NONE;

                if (withLivescore != LIVESCOREADDED.NONE)
                {
                    EventHandler<SXWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                    if (addedHandler != null)
                        addedHandler(this, new SXWIScoreAddedEventArgs(match, withLivescore, marketId));
                }
                else
                {
                    EventHandler<SXWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                    if (addedHandler != null)
                        addedHandler(this, new SXWNoIScoreAddedEventArgs(match));
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }


        public void startListBuilder()
        {
            LTDConfigurationReader reader = new LTDConfigurationReader();
            this.Active = reader.StrategyActivated;
            //buildBetList();
            ThreadPool.QueueUserWorkItem(this.buildBetList);
        }

        public void shutdown()
        {
            try
            {
                IDictionaryEnumerator enumBetSets = (IDictionaryEnumerator)m_list.GetEnumerator();
                while (enumBetSets.MoveNext() == true)
                {
                    BFUEStrategy betSet = (BFUEStrategy)enumBetSets.Value;
                    betSet.stopCloseTradeTimer();
                    betSet.stopStoppLoss();
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, "Common", "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void login()
        {
            SXALKom.Instance.login();            
            
        }
     
        public void setNewBet(SXALBet betDetail, LTDConfigurationRW config, IScore scoreToConnect)
        {            
            InsertQueueItem item = new InsertQueueItem(betDetail, config, scoreToConnect);
            _insertQueue.Enqueue(item);                        
        }

        public void setNewBet(SXALBet betDetail)
        {
            setNewBet(betDetail, null, null);           
        }

        private void buildBetList(Object stateInfo)
        {
            

            m_buildList = true;
            SXALKom sxalKom = SXALKom.Instance;

            //SortedList localList = new SortedList();


            // Initial alles lesen
            SXALMUBet[] bets = sxalKom.getBets();

            if (bets == null)
            {
                m_buildList = false;
                return;
            }

            // lokale Liste aufbauen
            foreach (SXALMUBet bet in bets)
            {
                try
                {                   
                    // Details lesen
                    SXALBet betDetail = sxalKom.getBetDetail(bet.BetId);

                    // Nur vom Type Odds nehmen 
                    if (betDetail.MarketType != SXALMarketTypeEnum.O)
                        continue;

                    // nicht markt 'Unentschieden'

                    if (betDetail.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, betDetail.MarketId))
                    {
                        continue;
                    }

                    InsertQueueItem item = new InsertQueueItem(betDetail, null, null);
                    _insertQueue.Enqueue(item);

                }
                catch (InvalidCastException ice)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFUEWatcher::buildBetList", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(ice);

                }
                catch (Exception exc)
                {
                    EventHandler<SXWMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new SXWMessageEventArgs(DateTime.Now, "Common", "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
            m_buildList = false;
        }

        void betSet_StopStopLossTimer(object sender, SXStopStopLossTimer e)
        {
            EventHandler<SXWStopStopLossTimer> handler = StopStopLossTimer;
            if (handler != null)
            {
                handler(this, new SXWStopStopLossTimer(e.Match));
            }
        }

        void betSet_StopOpenBetTimer(object sender, SXStopOpenBetTimer e)
        {
            EventHandler<SXWStopOpenBetTimer> handler = StopOpenBetTimer;
            if (handler != null)
            {
                handler(this, new SXWStopOpenBetTimer(e.Match));
            }
        }

        void betSet_StopCloseTradeTimer(object sender, SXStopCloseTradeTimer e)
        {
            EventHandler<SXWStopCloseTradeTimer> handler = StopCloseTradeTimer;
            if (handler != null)
            {
                handler(this, new SXWStopCloseTradeTimer(e.Match));
            }
        }

        void betSet_SetStopLossTimer(object sender, SXSetStopLossTimer e)
        {
            EventHandler<SXWSetStopLossTimer> handler = SetStopLossTimer;
            if (handler != null)
            {
                handler(this, new SXWSetStopLossTimer(e.Match, e.Timer));
            }
        }

        void betSet_SetOpenBetTimer(object sender, SXSetOpenBetTimer e)
        {
            EventHandler<SXWSetOpenBetTimer> handler = SetOpenBetTimer;
            if (handler != null)
            {
                handler(this, new SXWSetOpenBetTimer(e.Match, e.Timer));
            }
        }

        void betSet_SetCloseTradeTimer(object sender, SXSetCloseTradeTimer e)
        {
            EventHandler<SXWSetCloseTradeTimer> handler = SetCloseTradeTimer;
            if (handler != null)
            {
                handler(this, new SXWSetCloseTradeTimer(e.Match, e.Timer));
            }
        }

        private String splitFullMarketName(String fullmarketname)
        {
            char[] cSeps = {'/'};
            String[] splittedMarketName = fullmarketname.Split(cSeps);
            String tmp = String.Empty;
            if (splittedMarketName.Length > 1)
            {
                tmp = splittedMarketName[1].Replace(" v ", " - ");
            }
            else
                tmp = splittedMarketName[0].Replace(" v ", " - ");
            return tmp.Trim();
        }

        private void writeLocalXml(BFUEStrategy betSet, IScore score)
        {
            String strFile = SXDirs.ApplPath + @"\BFLSLocalMapping.xml";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(strFile);
            }
            catch (System.IO.FileNotFoundException)
            {
                //Datei nicht gefunden => erzeugen
                XmlTextWriter writer = new XmlTextWriter(strFile, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartElement("root");
                writer.Close();
                doc.Load(strFile);
            }

            try
            {
                //XPathNavigator
                XPathDocument xpathDoc = new XPathDocument(strFile);
                XPathNavigator xpathNav = xpathDoc.CreateNavigator();
                XPathExpression xpathExpr;

                String strExpr = String.Format("//Map[@betfair='{0}']", betSet.TeamA);
                xpathExpr = xpathNav.Compile(strExpr);
                XPathNodeIterator xpathIterA = xpathNav.Select(xpathExpr);
                XPathNavigator navA = null;
                XPathNavigator navB = null;
                strExpr = String.Format("//Map[@betfair='{0}']", betSet.TeamB);
                xpathExpr = xpathNav.Compile(strExpr);
                XPathNodeIterator xpathIterB = xpathNav.Select(xpathExpr);

                while (xpathIterA.MoveNext())
                {
                    navA = xpathIterA.Current.Clone();
                }

                while (xpathIterB.MoveNext())
                {
                    navB = xpathIterB.Current.Clone();
                }

                XmlNode root = doc.DocumentElement;
                XmlElement element = null;
                //Fall TeamA nicht vorhanden => anlegen
                if (navA == null)
                {
                    element = doc.CreateElement("Map");
                    element.SetAttribute("livescore", score.TeamA);
                    element.SetAttribute("betfair", betSet.TeamA);
                    root.AppendChild(element);
                }

                //Falls TeamB nicht vorhanden => anlegen
                if (navB == null)
                {
                    element = doc.CreateElement("Map");
                    element.SetAttribute("livescore", score.TeamB);
                    element.SetAttribute("betfair", betSet.TeamB);
                    root.AppendChild(element);
                }
                doc.Save(strFile);
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, "Common", "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public void initRiskWin()
        {
            try
            {
                IDictionaryEnumerator enumBetSets = (IDictionaryEnumerator)m_list.GetEnumerator();
                while (enumBetSets.MoveNext() == true)
                {
                    BFUEStrategy betSet = (BFUEStrategy)enumBetSets.Value;
                    betSet.initRiskWin();
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, "Common", "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private IScore linkLiveScore(String teamA, String teamB)
        {
            try
            {
                LiveScoreParser parser = LiveScoreParser.Instance;
                IScore score = parser.linkSportExchange(teamA, teamB);

                LiveScore2Parser parser2 = LiveScore2Parser.Instance;
                IScore score2 = parser2.linkSportExchange(teamA, teamB);

                HLLiveScore hlScore = new HLLiveScore(score, score2);
                return hlScore;
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, "Common", "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }

            return null;
        }

        #region EventHandler

        void Instance_ExceptionMessageEvent(object sender, SXExceptionMessageEventArgs e)
        {

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(e));
            }
        }

        private void BFBetsUpdated(object sender, SXALBetsUpdatedEventArgs e)
        {           
            ThreadPool.QueueUserWorkItem(this.internalBetsUpdated);
        }

        private void internalBetsUpdated(Object stateInfo)
        {
            try
            {
                if (m_buildList)
                {
                    return;
                }

                // Initial alles lesen            
                //MUBet[] bets = BetWatchdog.Instance.Bets;//betfairKom.getBets();


                SXALKom betfairKom = SXALKom.Instance;

                //SortedList localList = new SortedList();
                //List<long> changedTrades = new List<long>();

                // Initial alles lesen            
                SXALMUBet[] bets = SXALBetWatchdog.Instance.Bets;//betfairKom.getBets();

                if (bets == null)
                    return;

                foreach (SXALMUBet bet in bets)
                {
                    if (m_smm.isMarketMatchOdds(bet.MarketId) == false)
                        continue;

                    if (m_smm.isMarketInplay(bet.MarketId) == false)
                        continue;

                    SXALBet betDetail = betfairKom.getBetDetail(bet.BetId);

                    if (betDetail == null)
                    {
                        continue;
                    }

                    if (betDetail.MarketType != SXALMarketTypeEnum.O)
                        continue;


                    // nicht markt 'Unentschieden'
                    if (betDetail.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, betDetail.MarketId))
                    {
                        continue;
                    }


                    InsertQueueItem item = new InsertQueueItem(betDetail, null, null);
                    _insertQueue.Enqueue(item);
                
                    /*
                    // 1. Überprüfen, ob es schon einen Markt gibt
                    if (this.BetSet.ContainsKey(betDetail.MarketId))
                    {
                        // Auf den vorhandenen Markt arbeiten

                        if (betDetail.BetType == SXALBetTypeEnum.B)
                        {
                            //überprüfen ob Wette schon mal im Trade vorhanden
                            if (!(this.BetSet[betDetail.MarketId] as BFUEStrategy).Back.Bets.ContainsKey(betDetail.BetId))
                            {
                                (this.BetSet[betDetail.MarketId] as BFUEStrategy).Back.Bets.Add(betDetail.BetId, betDetail);
                                if (!changedTrades.Contains(betDetail.MarketId))
                                    changedTrades.Add(betDetail.MarketId);
                            }
                            else
                            {
                                // Überprüfe, ob sich etwas geändert hat
                                SXALBet tmpBet = (this.BetSet[betDetail.MarketId] as BFUEStrategy).Back.Bets[betDetail.BetId];
                                if (tmpBet.AvgPrice != betDetail.AvgPrice || tmpBet.BetStatus != betDetail.BetStatus ||
                                    tmpBet.CancelledDate != betDetail.CancelledDate || tmpBet.MatchedDate != betDetail.MatchedDate ||
                                    tmpBet.MatchedSize != betDetail.MatchedSize || tmpBet.PlacedDate != betDetail.PlacedDate ||
                                    tmpBet.Price != betDetail.Price || tmpBet.ProfitAndLoss != betDetail.ProfitAndLoss ||
                                    tmpBet.RemainingSize != betDetail.RemainingSize || tmpBet.RequestedSize != betDetail.RequestedSize ||
                                    tmpBet.SettledDate != betDetail.SettledDate)
                                {
                                    (this.BetSet[betDetail.MarketId] as BFUEStrategy).Back.Bets[betDetail.BetId] = betDetail;
                                    if (!changedTrades.Contains(betDetail.MarketId))
                                        changedTrades.Add(betDetail.MarketId);
                                }
                            }
                        }
                        else
                        {
                            //überprüfen ob Wette schon mal im Trade vorhanden
                            if (!(this.BetSet[betDetail.MarketId] as BFUEStrategy).Lay.Bets.ContainsKey(betDetail.BetId))
                            {
                                (this.BetSet[betDetail.MarketId] as BFUEStrategy).Lay.Bets.Add(betDetail.BetId, betDetail);
                                if (!changedTrades.Contains(betDetail.MarketId))
                                    changedTrades.Add(betDetail.MarketId);
                            }
                            else
                            {
                                // Überprüfe, ob sich etwas geändert hat
                                SXALBet tmpBet = (this.BetSet[betDetail.MarketId] as BFUEStrategy).Lay.Bets[betDetail.BetId];
                                if (tmpBet.AvgPrice != betDetail.AvgPrice || tmpBet.BetStatus != betDetail.BetStatus ||
                                    tmpBet.CancelledDate != betDetail.CancelledDate || tmpBet.MatchedDate != betDetail.MatchedDate ||
                                    tmpBet.MatchedSize != betDetail.MatchedSize || tmpBet.PlacedDate != betDetail.PlacedDate ||
                                    tmpBet.Price != betDetail.Price || tmpBet.ProfitAndLoss != betDetail.ProfitAndLoss ||
                                    tmpBet.RemainingSize != betDetail.RemainingSize || tmpBet.RequestedSize != betDetail.RequestedSize ||
                                    tmpBet.SettledDate != betDetail.SettledDate)
                                {
                                    (this.BetSet[betDetail.MarketId] as BFUEStrategy).Lay.Bets[betDetail.BetId] = betDetail;
                                    if (!changedTrades.Contains(betDetail.MarketId))
                                        changedTrades.Add(betDetail.MarketId);
                                }
                            }
                        }

                    }
                    else if (localList.ContainsKey(betDetail.MarketId))
                    {
                        // in existierender Lokalen Liste arbeiten
                        if (betDetail.BetType == SXALBetTypeEnum.B)
                        {
                            (localList[betDetail.MarketId] as BFUEStrategy).Back.Bets.Add(betDetail.BetId, betDetail);
                        }
                        else
                        {
                            (localList[betDetail.MarketId] as BFUEStrategy).Lay.Bets.Add(betDetail.BetId, betDetail);
                        }

                    }
                    else
                    {
                        if (String.IsNullOrEmpty(betDetail.FullMarketName))
                        {
                            betDetail.FullMarketName = m_smm.getMarketById(betDetail.MarketId, true).Match;
                        }
                        String marketName = splitFullMarketName(betDetail.FullMarketName);
                        // Einen neuen Markt erstellen
                        BFUEStrategy strategy = new BFUEStrategy(null, null, marketName);
                        if (betDetail.BetType == SXALBetTypeEnum.B)
                        {
                            strategy.Back.Bets.Add(betDetail.BetId, betDetail);
                        }
                        else
                        {
                            strategy.Lay.Bets.Add(betDetail.BetId, betDetail);
                        }

                        localList.Add(betDetail.MarketId, strategy);
                        if (!changedTrades.Contains(betDetail.MarketId))
                            changedTrades.Add(betDetail.MarketId);
                    }

                }


                foreach (BFUEStrategy strategy in localList.Values)
                {
                    if (strategy.Lay.Bets.Count == 0)
                        continue;

                    IScore score = linkLiveScore(strategy.TeamA, strategy.TeamB);//m_liveParser.injectBetfair(betSet.TeamA, betSet.TeamB);//m_liveParser.injectBetfair(betSet.TeamA, betSet.TeamB);

                    LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;
                    if (score != null)
                    {

                        score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(strategy.PlaytimeEventHandler);
                        score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(strategy.GoalEventHandler);
                        score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(strategy.GoalBackEventHandler);
                        score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(strategy.GameEndedEventHandler);

                        strategy.Score = score;
                    }

                    if (((HLLiveScore)score).IsScore1Connected() && ((HLLiveScore)score).IsScore2Connected())
                    {

                        withLivescore = LIVESCOREADDED.ALL;
                    }
                    else if (((HLLiveScore)score).IsScore1Connected() || ((HLLiveScore)score).IsScore2Connected())
                    {

                        withLivescore = LIVESCOREADDED.PARTLY;
                    }


                    strategy.GoalScoredEvent += new EventHandler<BFGoalScoredEventArgs>(BFGoalScoredHandler);
                    strategy.PlaytimeEvent += new EventHandler<SXPlaytimeEventArgs>(BFPlaytimeHandler);
                    strategy.RiskWinChangedEvent += new EventHandler<BFRiskWinChangedEventArgs>(BFRiskWinChangedHandler);
                    strategy.MessageEvent += new EventHandler<SXMessageEventArgs>(BFMessageHandler);
                    strategy.GameEndedEvent += new EventHandler<SXGameEndedEventArgs>(BFGameEndedHandler);
                    strategy.SetCloseTradeTimer += new EventHandler<SXSetCloseTradeTimer>(betSet_SetCloseTradeTimer);
                    strategy.SetOpenBetTimer += new EventHandler<SXSetOpenBetTimer>(betSet_SetOpenBetTimer);
                    strategy.SetStopLossTimer += new EventHandler<SXSetStopLossTimer>(betSet_SetStopLossTimer);
                    strategy.StopCloseTradeTimer += new EventHandler<SXStopCloseTradeTimer>(betSet_StopCloseTradeTimer);
                    strategy.StopOpenBetTimer += new EventHandler<SXStopOpenBetTimer>(betSet_StopOpenBetTimer);
                    strategy.StopStopLossTimer += new EventHandler<SXStopStopLossTimer>(betSet_StopStopLossTimer);


                    // Nachricht verbreiten
                    EventHandler<SXWMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new SXWMessageEventArgs(DateTime.Now, strategy.Match, LayTheDraw.strAdded, LayTheDraw.strModule));

                    if (withLivescore != LIVESCOREADDED.NONE)
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                            handler(this, new MatchAddedEventArgs(strategy.TeamA, strategy.TeamB, score.getScore(), Math.Round(strategy.Back.RiskWin - strategy.Lay.RiskWin, 2), Math.Round(strategy.Lay.BetSize - strategy.Back.BetSize, 2), withLivescore, strategy.Lay.MarketId));

                        EventHandler<SXWIScoreAddedEventArgs> addedHandler = IScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new SXWIScoreAddedEventArgs(String.Format("{0} - {1}", strategy.TeamA, strategy.TeamB), withLivescore, strategy.Lay.MarketId));
                    }
                    else
                    {
                        EventHandler<MatchAddedEventArgs> handler = MatchAddedEvent;
                        if (handler != null)
                            handler(this, new MatchAddedEventArgs(strategy.TeamA, strategy.TeamB, "0 - 0", Math.Round(strategy.Back.RiskWin - strategy.Lay.RiskWin, 2), Math.Round(strategy.Lay.BetSize - strategy.Back.BetSize, 2), withLivescore, strategy.Lay.MarketId));

                        EventHandler<SXWNoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                        if (addedHandler != null)
                            addedHandler(this, new SXWNoIScoreAddedEventArgs(String.Format("{0} - {1}", strategy.TeamA, strategy.TeamB)));
                    }

                    
                    try
                    {
                        m_list.Add(strategy.Lay.MarketId, strategy);
                        strategy.Active = this.Active;
                    }
                    catch (ArgumentException)
                    {                        
                    }
                }

                foreach (long changedTrade in changedTrades)
                {
                    (m_list[changedTrade] as BFUEStrategy).initRiskWin();
                }

                localList.Clear();
                localList = null;


                //TODO:  Scoreline 0 - 0 Märkte 
                foreach (SXALMUBet bet in bets)
                {
                    if (m_smm.isCorrectScoreMarket(bet.MarketId) == false)
                        continue;

                    if (bet.SelectionId != (int)SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, bet.MarketId))
                        continue;

                    if (m_smm.isMarketInplay(bet.MarketId) == false)
                        continue;

                    SXALBet betDetail = betfairKom.getBetDetail(bet.BetId);

                    //eine UE-Trade vorhanden?

                    SXALMarket bfMarket = m_smm.getWLDMarketByMatch(m_smm.getMatchById(bet.MarketId));
                    if (bfMarket == null)
                        continue;

                    if (!this.BetSet.ContainsKey(bfMarket.Id))
                        continue;

                    // UE-Trade vorhanden => lesen
                    BFUEStrategy strategy = this.BetSet[bfMarket.Id] as BFUEStrategy;
                    if (strategy == null)
                        continue;

                    strategy.addBet(betDetail);
                 */
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, "Common", "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
                    
        }
        
        private void BFGameEndedHandler(object sender, SXGameEndedEventArgs e)
        {            
            EventHandler<SXWGameEndedEventArgs> handler = GamenEndedEvent;
            if (handler != null)
                handler(this, new SXWGameEndedEventArgs(e.Match, e.Dts, e.WinLoose));
        }

        private void BFMessageHandler(object sender, SXMessageEventArgs e)
        {
            // internes Event nach außen veröffentlichen
            EventHandler<SXWMessageEventArgs> handler = MessageEvent;
            if (handler != null)
                handler(this, new SXWMessageEventArgs(e.MessageDTS, e.Match, e.Message, e.Module));
        }

        private void BFRiskWinChangedHandler(object sender, BFRiskWinChangedEventArgs e)
        {
            // internes Event nach aussen veröffentlichen
            EventHandler<BFWRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
            if (handler != null)
                handler(this, new BFWRiskWinChangedEventArgs(e.Match, e.BackGuV, e.LayGuV));
        }

        private void BFPlaytimeHandler(object sender, SXPlaytimeEventArgs e)
        {
            // internes Event nach aussen veröffentlichen
            EventHandler<SXWPlaytimeEventArgs> handler = PlaytimeEvent;
            if(handler != null)
                handler(this, new SXWPlaytimeEventArgs(e.Match,e.Playtime));
        }

        private void BFGoalScoredHandler(object sender, BFGoalScoredEventArgs e)
        {
            //internes Event nach aussen veröffentlichen
            EventHandler<BFWGoalScoredEventArgs> handler = GoalScoredEvent;            
            if (handler != null)
                handler(this, new BFWGoalScoredEventArgs(e.Team, e.ScoreA, e.ScoreB));
        }
        #endregion

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion

        private class InsertQueueItem
        {
            private SXALBet _bet;
            private LTDConfigurationRW _configuration;
            private IScore _ticker;

            public SXALBet Bet { get { return _bet; } }
            public LTDConfigurationRW Configuration { get { return _configuration; } }
            public IScore Ticker { get { return _ticker; } }

            public InsertQueueItem(SXALBet bet, LTDConfigurationRW configuration, IScore ticker)
            {
                _bet = bet;
                _configuration = configuration;
                _ticker = ticker;
            }
        }

        private class QueueWorkItem
        {
            private SXALBetCollection _backBets = new SXALBetCollection();
            private SXALBetCollection _layBets = new SXALBetCollection();
            

            public SXALBetCollection Backs { get { return _backBets; } }
            public SXALBetCollection Lays { get { return _layBets; } }
            public LTDConfigurationRW Configuration
            {
                get;
                set;
            }

            public IScore Ticker
            {
                get;
                set;
            }
        }

        private class SortedListQueueWorkItems : SortedList<long, QueueWorkItem> { }
    }
}
