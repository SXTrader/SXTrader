using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Web.Services.Protocols;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.eventargs;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.muk;
//using BFUEStrategy;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{

    public class BFRiskWinChangedEventArgs : EventArgs
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

        public BFRiskWinChangedEventArgs(string match, double backGuV, double layGuV)
        {
            m_match = match;
            m_backGuV = backGuV;
            m_layGuV = layGuV;
        }
    }

    public class BFGoalScoredEventArgs : EventArgs
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

        public BFGoalScoredEventArgs(string team, int la, int lb)
        {
            m_team = team;
            m_la = la;
            m_lb = lb;
        }
    }
   

    public class BFUEStrategy : IDisposable
    {
        #region Variablen
        private SXALBetCollection m_betLay = new SXALBetCollection();
        private SXALBetCollection m_betBack = new SXALBetCollection();
        private SXALBetCollection m_00Lay = new SXALBetCollection();
        private SXALBetCollection m_00Back = new SXALBetCollection();

        private LTDConfigurationRW m_config = new LTDConfigurationRW();
        private SXALBet m_openBack;
        private String m_marketName;
        private String m_team_a;
        private String m_team_b;
        //private SETSTATE m_state = SETSTATE.UNSETTELED;
        private volatile Boolean m_marketWatcherStop = false;
        private volatile Boolean m_betWatcherStop = false;
        private Thread m_betWatcherThread;
        private Thread m_marketWatcherThread;
        private Thread m_betStatusWatcherThread;
        private Thread m_riskWinUpdater;
        private int m_scoreA = 0;
        private int m_scoreB = 0;
        private int m_playtime = 0;

        //private SXScoreTrader _scoreTrader;
        

        //private double m_LayAmount;
        //private double m_LayOdds;
        //private double m_LayRisk;
        //private double m_winLay;
        private IScore m_score;
        //private IScore m_score2;
        private float m_commission;
        private bool m_active;
        private bool m_ended = false;        
        private int m_tradeId;
        private bool disposed = false;

        private bool _closeTradeUnstoppable = false;
        private bool _stoppLossUnstoppable = false;
        private bool _unmatchedBetUnstoppable = false;

        private static int m_tradeIdPool = 0;


        private object m_goalEventLock = "goalevent";
        private object m_playtimeEventLock = "playtimeevent";
        private object m_exitStarterLock = "exitstarter";
        private object m_exitThreadLock = "exitthread";
        private object m_placeBetStarterLock = "placebetstarter";
        private object m_placeBetThreadLock = "placebetthread";
        #endregion

        #region Konstanten
        private const double LOWTOLERANZ = -0.1;
        private const double HIGHTOLERANZ = 0.1;
        private const double STARTAMOUNT = 4.0;
        #endregion

        #region Attribute

        /*
        private double LayRisk
        {
            get
            {
                
                //this.m_LayRisk = m_LayRisk - (betBack.matchedSize * betBack.price);
            }
        }
        */
        public Boolean Active
        {
            get
            {
                return m_active;
            }
            set
            {
                log(String.Format("Changing Active Flag from {0} to {1}", m_active, value));
                m_active = value;

                

                if (m_active)
                    doActivation();
                else
                    doDeactivation();
            }
        }

        public String MarketName
        {
            get
            {
                return m_marketName;
            }
        }

        public String Match
        {
            get
            {
                return String.Format("{0} - {1}", this.TeamA, this.TeamB);
            }
        }

        public IScore Score
        {
            get
            {
                return m_score;
            }
            set
            {
                m_score = value;
                if (m_score != null)
                {
                    log("Connect a combined liveticker");
                    log(String.Format("Combined Liveticker. Liveticker 1 {0}. Liveticker 2 {1}", (value as HLLiveScore).IsScore1Connected(), (value as HLLiveScore).IsScore2Connected()));
                    if (m_score.ScoreA > m_scoreA)
                    {
                        m_scoreA = m_score.ScoreA;
                        log(String.Format("Connect a Combined Liveticker. Score has changed to {0}", this.Score.getScore()));
                    }
                    if (m_score.ScoreB > m_scoreB)
                    {
                        m_scoreB = m_score.ScoreB;
                        log(String.Format("Connect a Combined Liveticker. Score has changed to {0}", this.Score.getScore()));
                    }
                  
                    if (this.Lay.Bets.Count > 0)
                    {
                        //_scoreTrader = new SXScoreTrader(this.Match, this.m_tradeId, m_score, this.Lay.BetSize);
                        //_ou25Trader = new SXOU25Trader(this.Match, this.m_tradeId, m_score, this.Lay.BetSize);
                    }

                    
                }
                else
                {
                    log("Combined Liveticker was deleted");
                }
            }
        }

        public IScore Score2
        {
            get
            {
                return m_score;
                //return m_score2;
            }
            set
            {
                m_score = value;
                if (m_score != null)
                {
                    log("Connect a combined liveticker");
                    log(String.Format("Combined Liveticker. Liveticker 1 {0}. Liveticker 2 {1}", (value as HLLiveScore).IsScore1Connected(), (value as HLLiveScore).IsScore2Connected()));
                    if (m_score.ScoreA > m_scoreA)
                    {
                        m_scoreA = m_score.ScoreA;
                        log(String.Format("Connect a Combined Liveticker. Score has changed to {0}", this.Score.getScore()));
                    }
                    if (m_score.ScoreB > m_scoreB)
                    {
                        m_scoreB = m_score.ScoreB;
                        log(String.Format("Connect a Combined Liveticker. Score has changed to {0}", this.Score.getScore()));
                    }
                  
                    
                }
                else
                {
                    log("Combined Liveticker was deleted");
                }
               
                ;
            }
        }

        public String TeamA
        {
            get
            {
                return m_team_a;
            }
        }

        public String TeamB
        {
            get
            {
                return m_team_b;
            }
        }

        public SCORESTATE ScoreState
        {
            get
            {
                if (m_score == null)
                    return SCORESTATE.initdraw;
                return m_score.getScoreState();
                /*
                if (m_scoreA == 0 && m_scoreB == 0)
                    return SCORESTATE.initdraw;
                else if (m_scoreA == m_scoreB)
                    return SCORESTATE.draw;
                else
                    return SCORESTATE.undraw;
                 * */
            }
        }

        public SETSTATE State
        {
            get
            {
                double drawPL = (this.Back.BetSize * this.Back.BetPrice - this.Back.BetSize) - (this.Lay.BetSize * this.Lay.BetPrice - this.Lay.BetSize);
                double undrawPL = this.Lay.BetSize - this.Back.BetSize;
                logBetAmount(String.Format("Evaluating Trade state. Profit/Loss for Draw is {0}. Profit/Loss for Undraw is {1}", drawPL, undrawPL));
                /*
                double riskBack = this.Back.BetSize;
                double winBack = this.Back.RiskWin;
                //winBack = winBack - (winBack * 0.05);
                double laybackwin = winBack - this.Lay.RiskWin;
                double backlaywin = this.Lay.BetSize - this.Back.BetSize;
                //backlaywin = backlaywin - (backlaywin * 0.05);
                 
                double diff = laybackwin - backlaywin;
                */

                double diff = drawPL - undrawPL; // GuV(UE) - GuV(nUE)
                try
                {
                    logBetAmount(String.Format("Evaluating Trade state. The Difference between Profit/Loss Undraw and Profit/Loss Draw is {0}", diff));
                }
                catch{}

                SETSTATE state = SETSTATE.UNSETTELED;
                if (LOWTOLERANZ < diff && diff < HIGHTOLERANZ)
                {
                    state = SETSTATE.SETTLED;
                    log("Trade state is settled because diff is within tolerance");
                }
                else
                {
                    //if (laybackwin > backlaywin)
                    if(drawPL > undrawPL)
                    {                        
                        state = SETSTATE.SETTLED;
                        log("Trade state is settled because the difference of Winning Back and Risk of Lay is greater than the"
                            + "difference of  Size of Lay and Size of Back");
                    }
                    else
                    {
                        if (m_openBack != null)
                        {
                            state = SETSTATE.SETTLING;
                            log("Trade state is settling because there's an open bet");
                        }
                        else
                        {
                            state = SETSTATE.UNSETTELED;
                            log("Trade state is unsettled");
                        }
                    }
                }
                return state;
            }
            /*
            set
            {
                m_state = value;
            }
             * */
        }

        public SXALBetCollection Back
        {
            get
            {
                return m_betBack;
            }
            set
            {
                m_betBack = value;
            }
            /*
            set
            {
                
                
                m_betBack = value;
                if (m_betLay == null || m_betBack == null)
                {
                    State = SETSTATE.UNSETTELED;
                }
                else
                {
                    if (m_betBack.betStatus == BetStatusEnum.M && m_betLay.betStatus == BetStatusEnum.M)
                    {
                        EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("{0} - {1}", this.TeamA, this.TeamB);
                        if (handler != null)
                            handler(this, new BFRiskWinChangedEventArgs(sb.ToString(), this.RiskWin));
                        State = SETSTATE.SETTLED;
                    }
                    else if (m_betBack.betStatus == BetStatusEnum.MU || m_betBack.betStatus == BetStatusEnum.U
                        || m_betLay.betStatus == BetStatusEnum.MU || m_betLay.betStatus == BetStatusEnum.U)
                    {
                        State = SETSTATE.SETTLING;
                    }
                    else
                    {
                        State = SETSTATE.UNSETTELED;
                    }
                 
                }
            
            }*/
        }

        public SXALBetCollection Lay
        {
            get
            {
                return m_betLay;
            }
            set
            {
                m_betLay = value;
            }
            /*
            set
            {

               
                m_betLay = value;
                if (m_betBack == null|| m_betLay == null)
                {
                    State = SETSTATE.UNSETTELED;                 
                }
                else
                {
                    if (m_betBack.betStatus == BetStatusEnum.M && m_betLay.betStatus == BetStatusEnum.M)
                    {
                        EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("{0} - {1}", this.TeamA, this.TeamB);
                        if (handler != null)
                            handler(this, new BFRiskWinChangedEventArgs(sb.ToString(), this.RiskWin));
                        State = SETSTATE.SETTLED;
                    }
                    else if (m_betBack.betStatus == BetStatusEnum.MU || m_betBack.betStatus == BetStatusEnum.U
                        || m_betLay.betStatus == BetStatusEnum.MU || m_betLay.betStatus == BetStatusEnum.U)
                    {
                        State = SETSTATE.SETTLING;
                    }
                    else
                    {
                        State = SETSTATE.UNSETTELED;
                    }
                }

                if (m_betLay != null && m_betLay.betStatus == BetStatusEnum.M)
                {
                    m_LayAmount = m_betLay.matchedSize;
                    m_LayOdds = m_betLay.price;
                    m_LayRisk = m_LayAmount * m_LayOdds - m_LayAmount;
                    m_winLay = m_LayAmount - m_LayAmount * 0.05;
                }
              
            }
             */ 
        }


        public String RiskWin
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.State == SETSTATE.SETTLED)
                {                                      
                    double backRisk = this.Lay.BetSize - this.Back.BetSize;
                    double layRisk = -this.Lay.RiskWin + this.Back.RiskWin;
                    //double backRisk = this.Back.avgPrice * this.Back.matchedSize - (m_LayAmount * m_LayOdds - m_LayAmount);
                    sb.AppendFormat("{0:f}/{1:f}", layRisk, backRisk);
                }
                else
                {
                    double backRisk = this.Lay.BetSize - this.Back.BetSize;
                    double layRisk = -this.Lay.RiskWin - this.Back.RiskWin;
                    //double layRisk = -(m_LayAmount * m_LayOdds - m_LayAmount);
                    sb.AppendFormat("{0:f}/{1:f}", layRisk, backRisk);
                }
                return sb.ToString() ;
            }
        }

        public LTDConfigurationRW Configuration
        {
            get
            {
                return m_config;
            }
            set
            {
                LTDConfigurationRW oldConfig = m_config;
                m_config = value;

                if (oldConfig != null && m_config != null)
                {
                    if (oldConfig.StrategyActivated == true && m_config.StrategyActivated == false)
                        doDeactivation();
                    else if (oldConfig.StrategyActivated == false && m_config.StrategyActivated == true)
                        doActivation();
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler<BFGoalScoredEventArgs> GoalScoredEvent;
        public event EventHandler<SXPlaytimeEventArgs> PlaytimeEvent;
        public event EventHandler<BFRiskWinChangedEventArgs> RiskWinChangedEvent;
        public event EventHandler<SXMessageEventArgs> MessageEvent;
        public event EventHandler<SXGameEndedEventArgs> GameEndedEvent;
        public event EventHandler<SXSetCloseTradeTimer> SetCloseTradeTimer;
        public event EventHandler<SXSetOpenBetTimer> SetOpenBetTimer;
        public event EventHandler<SXSetStopLossTimer> SetStopLossTimer;
        public event EventHandler<SXStopCloseTradeTimer> StopCloseTradeTimer;
        public event EventHandler<SXStopOpenBetTimer> StopOpenBetTimer;
        public event EventHandler<SXStopStopLossTimer> StopStopLossTimer;
        public event EventHandler<SXManualTradeRemoveEventArgs> ManualTradeRemove;
        #endregion

        public BFUEStrategy(SXALBet lay, SXALBet back, String market)
        {
            m_tradeId = ++m_tradeIdPool;

            log("Constructed a new trade");

            m_placeBetStarterLock = "placebetstarter" + market;
            m_placeBetThreadLock  = "placebetthread" + market;
            m_exitThreadLock      = "exitthread" + market;
            m_exitStarterLock     = "exitstarter" + market;
            m_playtimeEventLock   = "playtimeevent" + market;
            m_goalEventLock       = "goalevent" + market;

            if(lay != null)
                Lay.Bets.Add(lay.BetId, lay);
            if(back != null)
                Back.Bets.Add(back.BetId,back);
            splitMarketName(market);
            m_marketName = market;
            log("Constructing a new Trade");
            m_marketWatcherThread = new Thread(this.marketWatcherThread);
            m_marketWatcherThread.IsBackground = true;
            m_betWatcherThread = new Thread(/*this.betWatcherThread*/ this.closeTradeThread);
            m_betWatcherThread.IsBackground = true;
            m_betStatusWatcherThread = new Thread(this.openBetWatcher/*this.betStatusWatcherThread*/);
            m_betStatusWatcherThread.IsBackground = true;

            m_riskWinUpdater = new Thread(this.riskWinUpdater);
            m_riskWinUpdater.IsBackground = true;
            m_riskWinUpdater.Start();


            SXMinutePulse.Instance.Pulse += Instance_Pulse;
        
            //Timer aufsetzen                          
        }

        void Instance_Pulse(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.unmatchedBetUpdater));
        }


        ~BFUEStrategy()
        {
            Dispose(false);
        }


        public void initRiskWin()
        {
            try
            {
                log("Initialize Risk and Win");
                log(String.Format("Count of Lay Bets: {0}. Count of Back Bets: {1}", this.Lay.Bets.Count, this.Back.Bets.Count));
                EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} - {1}", this.TeamA, this.TeamB);
                double backWin = Back.RiskWin - Lay.RiskWin;
                double backLost = Lay.BetSize - Back.BetSize;
                if (handler != null)
                {
                    log("Notifying Listerners: RiskWin Changed");
                    handler(this, new BFRiskWinChangedEventArgs(sb.ToString(), Math.Round(backWin, 2), Math.Round(backLost, 2)));
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public void addBet(SXALBet bet)
        {
            try
            {
                if (SXALSoccerMarketManager.Instance.isMarketMatchOdds(bet.MarketId))
                {
                    if(addMatchOddsBet(bet))
                        initRiskWin();
                }
                else if (SXALSoccerMarketManager.Instance.isCorrectScoreMarket(bet.MarketId))
                {
                    addCSBet(bet);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

        }

        private bool addMatchOddsBet(SXALBet bet)
        {
            try
            {
                if (bet.BetType == SXALBetTypeEnum.B)
                {
                    return addBetInternal(m_betBack, bet);
                }
                else
                {
                    return addBetInternal(m_betLay, bet);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                return false;
            }
        }

        private void addCSBet(SXALBet bet)
        {
            try
            {
                if (bet.BetType == SXALBetTypeEnum.B)
                {
                    addBetInternal(m_00Back, bet);
                }
                else
                {
                    addBetInternal(m_00Lay, bet);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private bool addBetInternal(SXALBetCollection betCollection, SXALBet bet)
        {
            bool betAdded = false;
            //Existiert Wette schon?
            if (betCollection.Bets.ContainsKey(bet.BetId))
            {
                SXALBet existingBet = betCollection.Bets[bet.BetId];
                // Aktualisierung notwendig?
                if (existingBet.AvgPrice != bet.AvgPrice || existingBet.BetStatus != bet.BetStatus
                    || existingBet.MatchedDate != bet.MatchedDate || existingBet.MatchedSize != bet.MatchedSize
                    || existingBet.Price != bet.Price || existingBet.RemainingSize != bet.RemainingSize)
                {
                    betCollection.Bets[bet.BetId] = bet;
                    betAdded = true;
                }
            }
            else
            {
                betCollection.Bets.Add(bet.BetId, bet);
                betAdded = true;
            }

            return betAdded;
        }

        private BFUEStrategy(SXALBet lay)
        {
            m_tradeId = ++m_tradeIdPool;
            if(lay != null)
                Lay.Bets.Add(lay.BetId,lay);

            log("Constructed a new Trade");
        }

        private void riskWinUpdater()
        {
            double backWinOld = 0.0;
            double backLostOld = 0.0;

            log("Starting RiskWin Update Thread");
            while (true)
            {
                try
                {
                    double backWin = Back.RiskWin - Lay.RiskWin;
                    double backLost = Lay.BetSize - Back.BetSize;
                    if (backWin != backWinOld && backLost != backLostOld)
                    {
                        log("Updateing RiskWin");
                        log(String.Format("Count of Lay Bets: {0}. Count of Back Bets: {1}", this.Lay.Bets.Count, this.Back.Bets.Count));
                        EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                        if (handler != null)
                        {
                            log("Notifying Listerners: RiskWin Changed");
                            handler(this, new BFRiskWinChangedEventArgs(this.Match, Math.Round(backWin, 2), Math.Round(backLost, 2)));
                        }

                        backWinOld = backWin;
                        backLostOld = backLost;
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
                Thread.Sleep(5000);
            }
        }

        private void recheckBets()
        {
            try
            {
                SXALMUBet[] muBets = SXALKom.Instance.getBetsMU(this.Lay.MarketId);
                foreach (SXALMUBet muBet in muBets)
                {
                    /*
                    if (muBet.BetStatus == SXALBetStatusEnum.C || muBet.BetStatus == SXALBetStatusEnum.L ||
                        muBet.BetStatus == SXALBetStatusEnum.S || muBet.BetStatus == SXALBetStatusEnum.V)
                        continue;
                     */
                    SXALBet bet = SXALKom.Instance.getBetDetail(muBet.BetId);

                    addBet(bet);

                }

            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        /// <summary>
        /// Überprüft, ob in den Betcollections sich noch nicht vollständig abgeschlossene Wetten befinden.
        /// </summary>
        private void unmatchedBetUpdater(object stateInfo)
        {              
            try
            {
                //Wenn alle Lay-Wetten storniert oder ungültig sind, dann macht dieser Trade keinen Sinn mehr
                if(this.Lay.AllBetsCanceled)
                {
                    manualRemoveTrade();
                    return;
                }

                //Laywetten Draw
                for(int i = 0; i < m_betLay.Bets.Count; i++)
                //foreach (SXALBet bet in m_betLay.Bets.Values)
                {
                    SXALBet bet = m_betLay.Bets.Values[i];
                    if (bet.BetStatus == SXALBetStatusEnum.MU || bet.BetStatus == SXALBetStatusEnum.U)
                    {
                        log(String.Format("Bet {0} BetStatus is {1}. Reread Bet", bet.BetId, bet.BetStatus));
                        SXALBet tmp = SXALKom.Instance.getBetDetail(bet.BetId);
                        if (tmp != null)
                        {
                            log(String.Format("Bet {0} BetStatus is now {1}", tmp.BetId, tmp.BetStatus));
                            m_betLay.Bets[bet.BetId] = tmp;
                        }
                    }
                }

                //Backwettenwetten Draw
                //TODO: InvalidOperationsException!
                for (int i = 0; i < m_betBack.Bets.Count; i++)
                //foreach (SXALBet bet in m_betBack.Bets.Values)
                {
                    SXALBet bet = m_betBack.Bets.Values[i];
                    if (bet.BetStatus == SXALBetStatusEnum.MU || bet.BetStatus == SXALBetStatusEnum.U)
                    {
                        log(String.Format("Bet {0} BetStatus is {1}. Reread Bet", bet.BetId, bet.BetStatus));
                        SXALBet tmp = SXALKom.Instance.getBetDetail(bet.BetId);
                        if (tmp != null)
                        {
                            log(String.Format("Bet {0} BetStatus is now {1}", tmp.BetId, tmp.BetStatus));
                            m_betBack.Bets[bet.BetId] = tmp;
                        }
                    }
                }

                //Laywettenwetten 0 - 0
                foreach (SXALBet bet in m_00Lay.Bets.Values)
                {
                    if (bet.BetStatus == SXALBetStatusEnum.MU || bet.BetStatus == SXALBetStatusEnum.U)
                    {
                        log(String.Format("Bet {0} BetStatus is {1}. Reread Bet", bet.BetId, bet.BetStatus));
                        SXALBet tmp = SXALKom.Instance.getBetDetail(bet.BetId);
                        if (tmp != null)
                        {
                            log(String.Format("Bet {0} BetStatus is now {1}", tmp.BetId, tmp.BetStatus));
                            m_00Lay.Bets[bet.BetId] = tmp;
                        }
                    }
                }

                //Laywettenwetten 0 - 0
                foreach (SXALBet bet in m_00Back.Bets.Values)
                {
                    if (bet.BetStatus == SXALBetStatusEnum.MU || bet.BetStatus == SXALBetStatusEnum.U)
                    {
                        log(String.Format("Bet {0} BetStatus is {1}. Reread Bet", bet.BetId, bet.BetStatus));
                        SXALBet tmp = SXALKom.Instance.getBetDetail(bet.BetId);
                        if (tmp != null)
                        {
                            log(String.Format("Bet {0} BetStatus is now {1}", tmp.BetId, tmp.BetStatus));
                            m_00Back.Bets[bet.BetId] = tmp;
                        }
                    }
                }
            }
            catch (InvalidOperationException ioe)
            {
                ExceptionWriter.Instance.WriteException(ioe);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

        }

        void manualRemoveTrade()
        {
            try
            {
                if (m_ended == true)
                    return;

                log("Manual Remove Trade: Received a Manual Remove Trade Signal from Trade Control");
                m_ended = true;



                if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) == true)
                {
                    log("Manual Remove Trade: A Close Trade is running. Stopping it");

                    m_betWatcherThread.Abort();
                    m_betWatcherThread.Join();
                }

                if (SXThreadStateChecker.isStartedBackground(m_marketWatcherThread) == true)
                {
                    log("Manual Remove Trade: A Stopp/Loss is running. Stopping it");

                    m_marketWatcherThread.Abort();
                    m_marketWatcherThread.Join();
                }

                if (SXThreadStateChecker.isStartedBackground(m_betStatusWatcherThread) == true)
                {
                    log("Manual Remove Trade: A Open Bet Watcher is running. Stopping it");
                    m_betStatusWatcherThread.Abort();
                    m_betStatusWatcherThread.Join();
                }

                EventHandler<SXManualTradeRemoveEventArgs> handler = ManualTradeRemove;
                if (handler != null)
                {
                    log("Manual Remove Trade: Notify Listeners: Trade is to be removed");
                    log("Manual Remove Trade: Manual Removed Trades didn't have any Winnings or Loss");
                    
                    handler(this, new SXManualTradeRemoveEventArgs(this.Match, DateTime.Now, this.Lay.MarketId));

                }
            }
            catch (Exception exc)
            {
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        /// <summary>
        /// Stopp/Loss Starter
        /// </summary>
        private void startStoppLoss()
        {
            lock (m_exitStarterLock)
            {
                try
                {
                    log("Start Stopp/Loss Thread");

                    //TODO: Wenn schon Back-Wette, dann Wetten nachladen und überprüfen
                    if (this.Back.Bets.Count > 0)
                    {

                    }

                    if (this.State == SETSTATE.SETTLED)
                    {
                        log("Trade is already settled. Don't start Stopp/Loss Thread");
                        return;
                    }

                    log("Check for a running Close Trade Thread when score is undraw");
                    // Falls BackWetterThread läuft => diesen Abbrechen
                    while (true)
                    {
                        if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) && ScoreState != SCORESTATE.undraw && !_closeTradeUnstoppable)
                        {
                            log("There's an already running Close Trade Thread with score is undraw: Aborting it");                            
                            m_betWatcherThread.Abort();

                            EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                            if (stopHandler != null)
                            {
                                log("Notifying Listerners: Aborting a Close Trade Thread");
                                stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                            }
                            break;
                        }
                        else if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) && ScoreState != SCORESTATE.undraw && _closeTradeUnstoppable)
                        {
                            log("There's alread a running Close Trade Thread with score is undraw and it is in unstoppable state: Retrying");
                            Thread.Sleep(1000);
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Falls Offene Wette => raus
                    log("Check for a running Open Bet Thread");
                    if (SXThreadStateChecker.isStartedBackground(m_betStatusWatcherThread))
                    {
                        log("There's an alread running Open Bet Thread. Don't start Close Trade Thread");                        
                        return;
                    }

                    // Falls Thread schon läuft => keine neuer Start nötig
                    log("Check for a running Stopp/Loss Thread");
                    if (SXThreadStateChecker.isStartedBackground(m_marketWatcherThread))
                    //if (m_marketWatcherThread.ThreadState == (System.Threading.ThreadState.Background | System.Threading.ThreadState.Running))
                    {
                        log("There's already a running Stopp/Loss Thread. Don't start a new Close Trade Thread");
                        
                        return;
                    }

                    // Falls Strategie schon abgeschlossen oder im Abschluss => keine Marktbeobachtung
                    if (this.State == SETSTATE.SETTLED || this.State == SETSTATE.SETTLING)
                    {
                        log("Trade is already settled or is settling at the moment. Don't start Stopp/Loss Thread");
                        
                        return;
                    }

                    log("All checks passed. Start a Stopp/Loss Thread");
                    m_marketWatcherThread = new Thread(this.marketWatcherThread);
                    m_marketWatcherThread.IsBackground = true;

                    m_marketWatcherThread.Start();

                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0} - {1}", this.TeamA, this.TeamB);
                    
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        public void stopStoppLoss()
        {
            log("Set stopp/loss flag to stop");
            m_marketWatcherStop = true;
        }

        private void startCloseTrade()
        {
            try
            {
                lock (m_placeBetStarterLock)
                {
                    log("Start Close Trade Thread");
                    // Falls Strategie schon abgeschlossen oder im Abschluss => kein neuer Start nötig
                    if (this.State == SETSTATE.SETTLED || this.State == SETSTATE.SETTLING)
                    {
                        log("Trade is already settled or is settling. Don't start a new Close Trade Thread");
                        
                        return;
                    }

                    log("Check for an running Open Bet Thread");
                    if (SXThreadStateChecker.isStartedBackground(m_betStatusWatcherThread))
                    {
                        log("There's a running Open Bet Thread. Don't start Close Trade Thread");
                        return;
                    }


                    // Falls schon Marktbeobachter  und gleichstand=> dann kein Abschlussthread
                    log("Check for a running Stopp/Loss when score is draw");
                    while (true)
                    {
                        if (SXThreadStateChecker.isStartedBackground(m_marketWatcherThread) && ScoreState == SCORESTATE.draw)
                        {
                            log("There's a running Stopp/Loss with a draw score. Don't start Close Trade Thread");
                            
                            return;
                        }
                        else if (SXThreadStateChecker.isStartedBackground(m_marketWatcherThread) && !_stoppLossUnstoppable)
                        {
                            log("There's a running Stopp/Loss with a undraw score. Aborting it");
                            
                            m_marketWatcherThread.Abort();
                            m_marketWatcherThread.Join();

                            EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;
                            if (stopHandler != null)
                            {
                                log("Inform Listerners: Abort Stopp/Loss Thread");
                                stopHandler(this, new SXStopStopLossTimer(this.Match));
                            }
                            break;
                        }
                        else if (SXThreadStateChecker.isStartedBackground(m_marketWatcherThread) && _stoppLossUnstoppable)
                        {
                            log("There's a running Stopp/Loss wit a undraw score and state is unstoppable: Retrying");
                            Thread.Sleep(1000);
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Falls BackWetterThread schon läuft => Thread beenden
                    log("Check for a running Close Trade Thread");
                    if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) && !_closeTradeUnstoppable)
                    {
                        log("There's already a Close Trade Thread running. Restarting it");
                        
                        m_betWatcherThread.Abort();
                        m_betWatcherThread.Join();
                    }
                    else if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) && _closeTradeUnstoppable)
                    {
                        log("There's already a Close Trade Thread running in an unstoppable state! Working with it");
                        return;
                    }


                    log("All checks passed. Start a Close Trade Thread");
                    m_betWatcherThread = new Thread(/*this.betWatcherThread*/ this.closeTradeThread);
                    m_betWatcherThread.IsBackground = true;

                    m_betWatcherThread.Start();

                    
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public void stopCloseTradeTimer()
        {
            log("Set close trade flag to stop");
            m_betWatcherStop = true;
        }
        
        private void splitMarketName(String market)
        {
            try
            {
                String[] sSeps = { " - ", " v " };

                String[] teams = market.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);
                /*if (teams.Length == 1)
                    teams = market.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);
                 */
                m_team_a = teams[0].Trim();
                m_team_b = teams[teams.GetLength(0) - 1].Trim();
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }


        /// <summary>
        /// New Close Trade Thread
        /// </summary>
        public void closeTradeThread()
        {
            bool recheck = false;
            try
            {
                
                log("Close Trade: Starting a Close Trade Thread");
                lock (m_placeBetThreadLock)
                {
                    // Bevor wir das Traden starten, überprüfen wir alle Wetten erneut
                    recheckBets();
                    if (m_config.GoalScoredCloseTradeSeconds == -1)
                    {
                        log("No Close Trade after goal configured! Leaving");
                        return;
                    }
                    TimeSpan span = new TimeSpan(0, 0, m_config.GoalScoredCloseTradeSeconds);
                    while (true)
                    {
                        try
                        {
                            if (m_config.GoalScoredCloseTradeSeconds == -1)
                            {
                                log("No Close Trade after goal configured! Leaving");
                                return;
                            }

                            _closeTradeUnstoppable = false;
                            EventHandler<SXSetCloseTradeTimer> openBetHandler = SetCloseTradeTimer;
                            if (openBetHandler != null)
                            {
                                log("Close Trade: Notifying Listeners: Start Close Trade Countdown");
                                openBetHandler(this, new SXSetCloseTradeTimer(this.Match, span));
                            }

                            log(String.Format("Close Trade: Wait for {0} seconds", span.TotalSeconds));
                            Thread.Sleep(span);

                            log("Close Trade: Check if Strategy is deactivated");
                            if (!m_active)
                            {
                                log("Close Trade: Strategy is deactivated");
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Close Trade: Inform Listeners: Stop Close Trade Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }
                                log("Open Bet Watcher: Leaving Open Bet Watcher Thread");
                                _closeTradeUnstoppable = false;
                                return;
                            }

                            if(recheck)
                            {
                                recheckBets();
                                recheck = false;
                            }

                            _closeTradeUnstoppable = true;

                            span = new TimeSpan(0, 0, m_config.DontCloseRetrySeconds);

                            // Falls schon eine Back-Wette => Dann alle Wetten neu laden
                            if (this.Back.Bets.Count > 0)
                            {
                                log(String.Format("Close Trade: Reading all Bets for market {0}", this.Lay.MarketId));
                                SXALMUBet[] muBets = SXALKom.Instance.getBetsMU(this.Lay.MarketId);
                                if (muBets == null)
                                {
                                    log(String.Format("Found no Bets in the Status MU für the market {0}. Retrying", this.Lay.MarketId));
                                    continue;
                                }

                                log(String.Format("Number of Bets for the market {0} is {1}", this.Lay.MarketId, muBets.Length));

                                //Überprüfe alle Wetten
                                foreach (SXALMUBet muBet in muBets)
                                {
                                    if (muBet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, muBet.MarketId))
                                    {
                                        log("Bet is not for the Draw market:Skipping");
                                        continue;
                                    }

                                    switch (muBet.BetStatus)
                                    {
                                        // Wette ist vollständig erfüllt
                                        case SXALBetStatusEnum.M:
                                            {
                                                log(String.Format("Close Trade: Bet {0} is matched", muBet.BetId));
                                                updateFromMUBet(muBet);
                                                break;
                                            }
                                    }
                                }
                            }

                            // Control State of Trade
                            if (this.State == SETSTATE.SETTLED)
                            {
                                log("Close Trade: Trade is already settled. Leaving Close Trade Thread");
                                EventHandler<SXMessageEventArgs> message = MessageEvent;
                                if (message != null)
                                {
                                    log("Close Trade: Notifying Listeners: Market is already settled");
                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSettled, LayTheDraw.strModule));
                                }

                                this.m_betWatcherStop = true;
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Close Trade: Notifying Listeners: Stop Close Trade Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }
                                _closeTradeUnstoppable = false;
                                return;
                            }

                            if (m_betLay.Bets.Count == 0)
                            {
                                log("Close Trade: Notifying Listeners: No Lay Bets. Leaving Close Trade Thread");
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Close Trade: Notifying Listeners: Stop Close Trade Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }
                                _closeTradeUnstoppable = false;
                                return;
                            }

                            // Überprüfe, ob Markt verfügbar und gebe die best Back-Quote zurück
                            bool retry = false;
                            double odds = checkBackMarketForOdds(out retry, "Close Trade");
                            if (retry)
                                continue;

                            double money = getMoneyForBackBet(out retry, odds, "Close Trade");

                            if (money == Double.NaN)
                            {
                                log("Money is NaN. Leaving!");
                                return;
                            }

                            if (retry)
                                continue;

                            // Nach der Configuration überprüfen
                            if (this.State == SETSTATE.SETTLED)
                            {
                                log("Close Trade: Market is already settled. Leaving Close Trade Thread");
                                EventHandler<SXMessageEventArgs> message = MessageEvent;
                                if (message != null)
                                {
                                    log("Close Trade: Notifying Listeners: Market is settled");
                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSettled, LayTheDraw.strModule));
                                }
                                this.m_betWatcherStop = true;
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Close Trade: Notifying Listeners: Stop Close Trade Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }
                                _closeTradeUnstoppable = false;
                                return;
                            }

                            if (!m_active)
                            {
                                log("Close Trade: Strategy is not active. Leaving Close Trade Thread");
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Close Trade: Notifying Listeners: Stop Close Trade Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }
                                _closeTradeUnstoppable = false;
                                return;
                            }

                            if ((ScoreState == SCORESTATE.draw || ScoreState == SCORESTATE.initdraw) &&
                                m_playtime <= m_config.DontCloseDrawPlaytime)
                            {
                                log(String.Format("Close Trade: Score is draw and playtime is below configued value {0}. Leaving Close Trade Thread", m_config.DontCloseDrawPlaytime));
                                EventHandler<SXMessageEventArgs> message = MessageEvent;
                                if (message != null)
                                {
                                    log("Close Trade: Notifying Listeners: Don't Close Trade");
                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strDontCloseDrawPlaytimeMessage, m_config.DontCloseDrawPlaytime), LayTheDraw.strModule));
                                }

                                this.m_betWatcherStop = true;
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Close Trade: Notifying Listeners: Stop Close Trade Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }
                                _closeTradeUnstoppable = false;
                                return;
                            }
                            else if(ScoreState == SCORESTATE.draw && odds <= m_config.DontCloseTradeDrawOdds)
                            {
                                log(String.Format("Close Trade: Score is draw and odds is below configued value {0}. Leaving Close Trade Thread", m_config.DontCloseTradeDrawOdds));
                                EventHandler<SXMessageEventArgs> message = MessageEvent;
                                if (message != null)
                                {
                                    log("Close Trade: Notifying Listeners: Don't Close Trade");
                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strDontCloseDrawOddsMessage, m_config.DontCloseTradeDrawOdds), LayTheDraw.strModule));
                                }

                                this.m_betWatcherStop = true;
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Close Trade: Notifying Listeners: Stop Close Trade Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }
                                _closeTradeUnstoppable = false;
                                return;
                            }

                            SXALBet bet = null;
                            try
                            {
                                placeNewBackBet(out bet, odds, money, "Close Trade");
                            }
                            catch (SXALBetInProgressException bipe)
                            {
                                log(String.Format("Received a BetInProgressException with betId {0}. Rechecking Betstatus", bipe.BetId));
                                //Betfair Zeit geben den Markt abzuschliessen
                                Thread.Sleep(3000);
                                SXALMUBet[] muBets = null;
                                if (bipe.BetId == 0)
                                    muBets = SXALKom.Instance.getBetsMU(this.Lay.MarketId);
                                else
                                    muBets = SXALKom.Instance.getBetMU(bipe.BetId);

                                if (muBets != null)
                                {
                                    foreach (SXALMUBet muBet in muBets)
                                    {
                                        //TODO: SelectionId für Drawmarkt noch in Konstante
                                        if (muBet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, muBet.MarketId))
                                            continue;

                                        //Uns interessieren hier nur Back-Wetten
                                        if (muBet.BetType == SXALBetTypeEnum.L)
                                            continue;

                                        // Uns interessieren hier nur neue Wetten
                                        if (this.Back.Bets[muBet.BetId] != null)
                                            continue;


                                        bet = SXALKom.Instance.getBetDetail(muBet.BetId);
                                    }
                                }
                            }

                            if (bet != null)
                            {
                                log("Close Trade: New Back Bet was placed");
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Close Trade: Notifying Listeners: Stop Close Trade Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }

                                log(String.Format("Close Trade: Adding Back Bet to Back Bet List. List has now {0} Bets", this.Back.Bets.Count));
                                this.Back.Bets.Add(bet.BetId, bet); ;
                                this.m_betWatcherStop = true;

                                EventHandler<SXMessageEventArgs> message2 = MessageEvent;

                                //this.m_LayRisk = m_LayRisk - (betBack.matchedSize * betBack.price);                            
                                // Falls Wetter offen oder teiloffen => Wächterthread starten
                                switch (bet.BetStatus)
                                {
                                    case SXALBetStatusEnum.U:
                                        log("Close Trade: Bet is unmatched. Starting Open Bet Watcher Thread");
                                        m_openBack = bet;
                                        m_betStatusWatcherThread = new Thread(this.openBetWatcher/*this.betStatusWatcherThread*/);
                                        m_betStatusWatcherThread.IsBackground = true;
                                        m_betStatusWatcherThread.Start();

                                        if (message2 != null)
                                        {
                                            log("Close Trade: Notifying Listeners: Unmatched or partly matched bet");
                                            message2(this, new SXMessageEventArgs(DateTime.Now, m_marketName,
                                                String.Format(LayTheDraw.strBetUnmatched, m_marketName, money, odds), LayTheDraw.strModule));
                                        }

                                        //mhe 10.11.2011: Warum war hier kein return?
                                        _closeTradeUnstoppable = false;

                                        return;
                                    case SXALBetStatusEnum.MU:
                                        {
                                            log("Close Trade: Bet is partly matched. Starting Open Bet Watcher Thread");
                                            m_openBack = bet;
                                            this.Back.Bets.Add(bet.BetId, bet);
                                            m_betStatusWatcherThread = new Thread(this.openBetWatcher/*this.betStatusWatcherThread*/);
                                            m_betStatusWatcherThread.IsBackground = true;
                                            m_betStatusWatcherThread.Start();

                                            if (message2 != null)
                                            {
                                                log("Close Trade: Notifying Listeners: Unmatched or partly matched bet");
                                                message2(this, new SXMessageEventArgs(DateTime.Now, m_marketName,
                                                    String.Format(LayTheDraw.strBetUnmatched, m_marketName, money, odds), LayTheDraw.strModule));
                                            }

                                            //mhe 10.11.2011: Warum war hier kein return?
                                            _closeTradeUnstoppable = false;

                                            return;
                                        }
                                    case SXALBetStatusEnum.M:
                                        {
                                            log(String.Format("Close Trade: Betstatus is {0}. Leaving now Close Trade Thread", bet.BetStatus.ToString()));
                                            EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                                            log("Close Trade: Calculating new Risk and Win");
                                            double backWin = Back.RiskWin - Lay.RiskWin;
                                            double backLost = Lay.BetSize - Back.BetSize;
                                            if (handler != null)
                                            {
                                                log("Close Trade: Notifying Listeners: Changed RiskWin");
                                                handler(this, new BFRiskWinChangedEventArgs(this.Match, Math.Round(backWin, 2), Math.Round(backLost, 2)));
                                            }

                                            if (message2 != null)
                                            {
                                                log("Close Trade: Notifying Listeners: New Matched Bet was placed");
                                                message2(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strBetMatched, m_marketName, Back.BetSize, Back.BetPrice), LayTheDraw.strModule));
                                            }

                                            _closeTradeUnstoppable = false;
                                            return;
                                        }
                                    default:
                                        {
                                            log(String.Format("Close Trade: Reading all Bets for market {0}", this.Lay.MarketId));
                                            SXALMUBet[] muBets = SXALKom.Instance.getBetsMU(this.Lay.MarketId);
                                            if (muBets == null)
                                            {
                                                log(String.Format("Found no Bets in the Status MU für the market {0}. Retrying", this.Lay.MarketId));
                                                continue;
                                            }

                                            log(String.Format("Number of Bets for the market {0} is {1}", this.Lay.MarketId, muBets.Length));

                                            //Überprüfe alle Wetten
                                            foreach (SXALMUBet muBet in muBets)
                                            {
                                                if (muBet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, muBet.MarketId))
                                                {
                                                    log("Bet is not for the Draw market:Skipping");
                                                    continue;
                                                }

                                                switch (muBet.BetStatus)
                                                {
                                                    // Wette ist vollständig erfüllt
                                                    case SXALBetStatusEnum.M:
                                                        {
                                                            log(String.Format("Close Trade: Bet {0} is matched", muBet.BetId));
                                                            updateFromMUBet(muBet);
                                                            break;
                                                        }
                                                }
                                            }
                                            continue;
                                        }
                                }
                            }
                            else
                            {
                                log("Close Trade: No Back Bet was placed. Next iteration");
                            }
                        }
                        catch (SXALMarketDoesNotExistException smdnee)
                        {
                            log("Close Trade: Market does not exist");
                            ExceptionWriter.Instance.WriteException(smdnee);
                            EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                            if (stopHandler != null)
                            {
                                log("Close Trade: Inform Listeners: Stop Open Bet Countdown");
                                stopHandler(this, new SXStopOpenBetTimer(this.Match));
                            }
                            log("Close Trade: Leaving Open Bet Watcher Thread");
                            _unmatchedBetUnstoppable = false;
                            return;
                        }                        
                        catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
                        {
                            ExceptionWriter.Instance.WriteException(mnsnae);
                            EventHandler<SXMessageEventArgs> message = MessageEvent;
                            String msg = String.Format("Market {0} neither suspended nor active! ExpectionMessage {1}. Leaving Trade!", m_marketName, mnsnae.Message);
                            log(msg);
                            if (message != null)
                            {
                                message(this, new SXMessageEventArgs(DateTime.Now, Match, msg, "Lay The Draw"));
                            }

                            EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                            if (stopHandler != null)
                            {
                                log("Close Trade: Inform Listeners: Stop Open Bet Countdown");
                                stopHandler(this, new SXStopOpenBetTimer(this.Match));
                            }
                            _unmatchedBetUnstoppable = false;

                            return;
                        }
                        catch (ThreadAbortException)
                        {
                            EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                            if (stopHandler != null)
                            {
                                log("Close Trade: Inform Listeners: Stop Open Bet Countdown");
                                stopHandler(this, new SXStopOpenBetTimer(this.Match));
                            }

                            _closeTradeUnstoppable = false;
                            log("Close Trade: Received a Thread Abort Exception: Leaving");
                            return;
                            //throw tae;
                        }
                        catch (Exception exc)
                        {
                            ExceptionWriter.Instance.WriteException(exc);
                            Thread.Sleep(10000);
                            recheck = true;
                        }
                    }
                }
            }
            catch (SXALMarketDoesNotExistException smdnee)
            {
                log("Close Trade: Market does not exist");
                ExceptionWriter.Instance.WriteException(smdnee);
                EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                if (stopHandler != null)
                {
                    log("Close Trade: Inform Listeners: Stop Open Bet Countdown");
                    stopHandler(this, new SXStopOpenBetTimer(this.Match));
                }
                log("Close Trade: Leaving Open Bet Watcher Thread");
                _unmatchedBetUnstoppable = false;
                return;
            }
            catch (ThreadAbortException)
            {
                EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                if (stopHandler != null)
                {
                    log("Close Trade: Inform Listeners: Stop Open Bet Countdown");
                    stopHandler(this, new SXStopOpenBetTimer(this.Match));
                }

                _closeTradeUnstoppable = false;
                log("Close Trade: Received a Thread Abort Exception: Leaving");
                return;
                //throw tae;
            }
        }

        /// <summary>
        /// New Open Bet Watcher
        /// </summary>
        public void openBetWatcher()
        {
            log("Starting a Open Bet Watcher Thread");
            bool recheck = false;
            TimeSpan span = new TimeSpan(0, 0, m_config.UnmatchedWaitSeconds);

            recheckBets();

            while (true)
            {
                _unmatchedBetUnstoppable = false;
                try
                {
                    log(String.Format("Open Bet Watcher: Wait for {0} seconds", m_config.UnmatchedWaitSeconds));

                    EventHandler<SXSetOpenBetTimer> openBetHandler = SetOpenBetTimer;
                    if (openBetHandler != null)
                    {
                        log("Open Bet Watcher: Inform Listerners: Start Open Bet Countdown");
                        openBetHandler(this, new SXSetOpenBetTimer(this.Match, span));
                    }

                    Thread.Sleep(span);
                    log("Open Bet Watcher: Time has passed. Controlling open Bets");
                    _unmatchedBetUnstoppable = true;

                    if(recheck)
                    {
                        recheckBets();
                        recheck = false;
                    }

                    // Alle Wetten zum Markt lesen
                    log(String.Format("Open Bet Watcher: Reading all Bets for market {0}", this.Lay.MarketId));
                    SXALMUBet[] muBets = SXALKom.Instance.getBetsMU(this.Lay.MarketId);


                    if (muBets == null)
                    {
                        log(String.Format("Found no Bets in the Status MU für the market {0}. Retrying", this.Lay.MarketId));
                        continue;
                    }

                    log(String.Format("Number of Bets for the market {0} is {1}", this.Lay.MarketId, muBets.Length));

                    //Überprüfe alle Wetten
                    foreach (SXALMUBet muBet in muBets)
                    {
                        if (muBet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, muBet.MarketId))
                        {
                            log("Bet is not for the Draw market:Skipping");
                            continue;
                        }



                        switch (muBet.BetStatus)
                        {
                            // Wette ist vollständig erfüllt
                            case SXALBetStatusEnum.M:
                                {
                                    log(String.Format("Open Bet Watcher: Bet {0} is matched", muBet.BetId));
                                    updateFromMUBet(muBet);
                                    break;
                                }
                            // Wette ist immer noch nicht erfüllt => stornieren
                            case SXALBetStatusEnum.U:
                                {
                                    log(String.Format("Open Bet Watcher: Bet {0} is unmatched", muBet.BetId));
                                    log(String.Format("Open Bet Watcher: Canceling Bet {0}", muBet.BetId));
                                    if (!SXALKom.Instance.cancelBet(muBet.BetId))
                                    {
                                        log(String.Format("Open Bet Watcher: Cancelation of Bet {0} failed", muBet.BetId));
                                        continue;
                                    }
                                    break;
                                }
                            // Wette Teilweise erfüllt => unerfüllten Teil stornieren
                            case SXALBetStatusEnum.MU:
                                {
                                    log(String.Format("Bet {0} is partly matched", muBet.BetId));
                                    log(String.Format("Open Bet Watcher: Canceling unmatched Part of Bet {0}", muBet.BetId));
                                    if (!SXALKom.Instance.cancelBet(muBet.BetId))
                                    {
                                        log(String.Format("Open Bet Watcher: Cancelation of Bet {0} failed", muBet.BetId));
                                        continue;
                                    }
                                    updateFromMUBet(muBet);
                                    break;
                                }
                            // Alle anderen Status aus der Wettliste entfernen;
                            default:
                                {
                                    log(String.Format("Bet {0} has Status {1}. Removing from Betlist", muBet.BetId, muBet.BetStatus));
                                    removeBetFromList(muBet);
                                    break;
                                }
                        }

                        if (m_openBack != null)
                        {
                            // Wette erneut überprüfen.
                            SXALBet b = SXALKom.Instance.getBetDetail(m_openBack.BetId);
                            if (b != null)
                            {
                                if (b.BetStatus == SXALBetStatusEnum.M)
                                {
                                    // Alles klar;
                                    m_openBack = null;
                                    addBet(b);

                                    return;
                                }
                                else if (b.BetStatus == SXALBetStatusEnum.U || b.BetStatus == SXALBetStatusEnum.MU)
                                {
                                    //stornieren
                                    if (!SXALKom.Instance.cancelBet(b.BetId))
                                    {
                                        m_openBack = SXALKom.Instance.getBetDetail(m_openBack.BetId);
                                        log(String.Format("Could not cancel bet {1}. Retrying", b.BetId));
                                        continue;
                                    }
                                    else
                                        m_openBack = null;
                                }
                                else
                                {
                                    // Alle anderen Status
                                    log(String.Format("Bet {0} is now in Status {1}.", b.BetId, b.BetStatus));
                                    m_openBack = null;
                                }
                            }
                        }
                        else
                        {
                            EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                            if (stopHandler != null)
                            {
                                log("Open Bet Watcher: Inform Listeners: Stop Open Bet Countdown");
                                stopHandler(this, new SXStopOpenBetTimer(this.Match));
                            }

                            updateRiskWinNotify("Open Bet Watcher");
                            log("Open Bet Watcher: No Open Bet. Leaving.");
                            _unmatchedBetUnstoppable = false;
                            return;
                        }

                    }
                    // Falls Trade mittlerweile erfüllt => raus
                    if (this.State == SETSTATE.SETTLED)
                    {

                        EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                        if (stopHandler != null)
                        {
                            log("Open Bet Watcher: Inform Listeners: Stop Open Bet Countdown");
                            stopHandler(this, new SXStopOpenBetTimer(this.Match));
                        }

                        updateRiskWinNotify("Open Bet Watcher");
                        log("Open Bet Watcher: Trade is now settled. Leaving.");
                        _unmatchedBetUnstoppable = false;
                        return;
                    }

                    // Ansonsten neue Wette;
                    // Falls Trade deaktiviert => raus
                    log("Open Bet Watcher: Check if Strategy is deactivated");
                    if (!m_active)
                    {
                        log("Open Bet Watcher: Strategy is deactivated");
                        EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                        if (stopHandler != null)
                        {
                            log("Open Bet Watcher: Inform Listeners: Stop Open Bet Countdown");
                            stopHandler(this, new SXStopOpenBetTimer(this.Match));
                        }
                        log("Open Bet Watcher: Leaving Open Bet Watcher Thread");
                        _unmatchedBetUnstoppable = false;
                        return;
                    }

                    SXALBet bet = null;
                    try
                    {
                        log("Open Bet Watcher: Placing a new Back Bet");
                        bool retry = false;
                        double odds = checkBackMarketForOdds(out retry, "Open Bet Watcher");
                        if (retry)
                            continue;

                        double money = getMoneyForBackBet(out retry, odds, "Open Bet Watcher");
                        if (money == Double.NaN)
                        {
                            log("Money is NaN. Leaving!");
                            return;
                        }
                        if (retry)
                            continue;
                        if (!placeNewBackBet(out bet, odds, money, "Open Bet Watcher"))
                            continue;
                    }
                    catch (SXALBetInProgressException)
                    {
                        log("Open Bet Watcher: Bet is still in progress: Retrying");
                        continue;
                    }

                    //Überprüfe, ob wette erfolgreich platziert
                    if (bet == null)
                    {
                        log("Place Back Bet returned null: Retrying");
                        continue;
                    }

                    if (bet.BetStatus == SXALBetStatusEnum.M)
                    {
                        log("Open Bet Watcher: New Bet is matched. Leavining");
                        updateFromBet(bet);
                        // Trade istimmer noch offen => nochmal das ganze                      
                        if (this.State != SETSTATE.SETTLED)
                        {
                            log("Open Bet Watcher: Trade is still unsettled. Not leaving but retrying!");
                            continue;
                        }

                        EventHandler<SXStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                        if (stopHandler != null)
                        {
                            log("Open Bet Watcher: Inform Listeners: Stop Open Bet Countdown");
                            stopHandler(this, new SXStopOpenBetTimer(this.Match));
                        }
                        updateRiskWinNotify("Open Bet Watcher");

                        _unmatchedBetUnstoppable = false;

                        return;
                    }
                    else if (bet.BetStatus == SXALBetStatusEnum.MU)
                    {
                        log("Open Bet Watcher: new Bet is partially Matched: Update and retrying");
                        updateFromBet(bet);
                        updateRiskWinNotify("Open Bet Watcher");
                        _unmatchedBetUnstoppable = false;
                        continue;
                    }
                    else
                    {
                        log("Open Bet Watcher: New Back Bet is unmatched or has an other status: Retrying");
                        continue;
                    }

                }
                catch (ThreadAbortException)
                {
                    EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                    if (stopHandler != null)
                    {
                        log("Open Bet Watcher: Inform Listeners: Stop Open Bet Countdown");
                        stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                    }

                    log("Open Bet Watcher: Received a Thread Abort Exception: Leaving");
                    _unmatchedBetUnstoppable = false;
                    return;
                    //throw tae;
                }
                catch (SXALMarketDoesNotExistException smdnee)
                {
                    log("Open Bet Watcher: Market does not exist");
                    ExceptionWriter.Instance.WriteException(smdnee);
                    EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                    if (stopHandler != null)
                    {
                        log("Open Bet Watcher: Inform Listeners: Stop Open Bet Countdown");
                        stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                    }
                    log("Open Bet Watcher: Leaving Open Bet Watcher Thread");
                    _unmatchedBetUnstoppable = false;
                    return;
                }
                catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
                {
                    ExceptionWriter.Instance.WriteException(mnsnae);
                    EventHandler<SXMessageEventArgs> message = MessageEvent;
                    String msg = String.Format("Market {0} neither suspended nor active! ExpectionMessage {1}. Leaving Trade!", m_marketName, mnsnae.Message);
                    log(msg);
                    if (message != null)
                    {
                        message(this, new SXMessageEventArgs(DateTime.Now, Match, msg, "Lay The Draw"));
                    }

                    EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                    if (stopHandler != null)
                    {
                        log("Open Bet Watcher: Inform Listeners: Stop Open Bet Countdown");
                        stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                    }
                    _unmatchedBetUnstoppable = false;

                    return;
                }
                catch (Exception e)
                {
                    ExceptionWriter.Instance.WriteException(e);
                    _unmatchedBetUnstoppable = false;
                    Thread.Sleep(10000);
                    recheck = true;
                    continue;
                }
            }
        }

        private void updateRiskWinNotify(String threadModule)
        {
            try
            {
                log(String.Format("{0}: Calculating new Risk and Win", threadModule));
                //logBetAmount(String.Format("An former unmatched Bet is now matched. Odds of Bet are {0} "
                //    + "Size of Bet is {1}", this.Back.Bets[m_openBack.betId].avgPrice, this.Back.Bets[m_openBack.betId].matchedSize));
                EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                double backWin = Back.RiskWin - Lay.RiskWin;
                double backLost = Lay.BetSize - Back.BetSize;
                if (handler != null)
                {
                    log("Open Bet Watcher: Inform Listeners: RiskWin changed");
                    handler(this, new BFRiskWinChangedEventArgs(this.Match, Math.Round(backWin, 2), Math.Round(backLost, 2)));
                }

                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                {
                    log("Open Bet Watcher: Inform Listeners: Text Message Open Bet finished");
                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strPlaceBet, m_marketName, m_openBack.AvgPrice, m_openBack.MatchedSize), LayTheDraw.strModule));
                }
                log("Open Bet Watcher: Leaving Open Bet Watcher Thread");
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
        private double checkBackMarketForOdds(out bool retry, String threadName)
        {
            log(String.Format("{0}: Looking for new Market Informations",threadName));
            SXALMarketLite marketLite = SXALKom.Instance.getMarketInfo(this.Lay.MarketId);

            if (marketLite == null)
            {
                log(String.Format("{0}: No Market Informations. Next Iteration", threadName));
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                {
                    log(String.Format("{0}: Inform Listeners: No Market Informations",threadName));
                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strNoMarketInfo, LayTheDraw.strModule));
                }
                retry = true;
                return 0.0;
            }

            // Falls Markt pausiert => raus
            if (marketLite.MarketStatus == SXALMarketStatusEnum.SUSPENDED)
            {
                log(String.Format("{0}: Market is Suspended. Next Iteration", threadName));
                EventHandler<SXMessageEventArgs> message = MessageEvent;

                if (message != null)
                {
                    log(String.Format("{0}: Inform Listeners: Market is suspended",threadName));
                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSuspended, LayTheDraw.strModule));
                }
                retry = true;
                return 0.0;
            }

            // Hole Preisübersicht
            log(String.Format("{0}: Reading Market Prices", threadName));
            SXALMarketPrices marketPrices = SXALKom.Instance.getMarketPrices(this.Lay.MarketId);
            if (marketPrices == null)
            {
                log(String.Format("{0}: No market prices. Next Iteration"));
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                {
                    log(String.Format("{0}: Inform Listeners: No Market Prices"));
                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strNoMarketPrices, LayTheDraw.strModule));
                }
                retry = true;
                return 0.0;
            }

            // Falls Markt pausiert => raus
            if (marketPrices.MarketStatus == SXALMarketStatusEnum.SUSPENDED)
            {
                log(String.Format("{0}: Market is Suspended. Next Iteration", threadName));
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                {
                    log(String.Format("{0}: Inform Listeners: Market is suspended", threadName));
                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSuspended, LayTheDraw.strModule));
                }
                retry = true;
                return 0.0;
            }

            SXALRunnerPrices runnerPrice = marketPrices.RunnerPrices[2];
            if (runnerPrice.BestPricesToBack.Length > 0)
            {
                SXALPrice priceBack = runnerPrice.BestPricesToBack[0];
                retry = false;
                return priceBack.Price;
            }

            retry = true;
            return 0.0;
        }
        private double getMoneyForBackBet(out bool retry, double odds, String threadName)
        {
            if (odds < this.Lay.BetPrice - m_config.DontCloseOdds && this.ScoreState == SCORESTATE.undraw)
            {
                log(String.Format("{2}: Best Back Price {0} is lower than the minimum price of {1} if score is undraw. Next Iteration", odds, this.Lay.BetPrice - m_config.DontCloseOdds, threadName));
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                {
                    log(String.Format("{0}: Inform Listerners: Back Price too low for Close Traden when undraw", threadName));
                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strBelowMinOdds, this.Lay.BetPrice - m_config.DontCloseOdds), LayTheDraw.strModule));
                }
                retry = true;
                return 0.0; ;
            }

            // Falls Back Quote = LayQuote => Einsatz = Lay Einsatz
            if (odds == this.Lay.BetPrice)
            {
                log(String.Format("{1}: Back Price equals excactly Lay Price {0}. Take exact Lay Betsize for new Bet", this.Lay.BetPrice, threadName));
                retry = false;
                return this.Lay.BetSize;//m_LayAmount;
            }
            else
            {
                log(String.Format("{0}: Start calculation of Back Bet Amount", threadName));
                retry = false;
                return calculateBackBet(odds);
            }      
        }
        private bool placeNewBackBet(out SXALBet bet, double odds, double money, String threadName)
        {
            try
            {
                bet = null;


                EventHandler<SXMessageEventArgs> message2 = MessageEvent;
                if (message2 != null)
                {
                    log(String.Format("{0}: Inform Listeners: A new bet will be placed", threadName));
                    message2(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strPlaceBet, m_marketName, odds, money), LayTheDraw.strModule));
                }

                /*
                m_commission = marketPrices.marketBaseRate * 0.01f;
                log(String.Format("{1}: Commission Rate for market is {0}", m_commission, threadName));
                */
                try
                {
                    if (money < SXALBankrollManager.Instance.MinStake)
                    {
                        log(String.Format("{0}: Back bet will be below the minimum stake allowed by Betfair. Executing special 'Below Min Bet'-Logic", threadName));
                        bet = placeBackBetBelowMin(odds, money);
                    }
                    else
                    {
                        log(String.Format("{0}: Place back bet", threadName));

                        bet = placeBackBet(odds, money);                        
                    }
                }
                catch (SXALNoBetBelowMinAllowedException)
                {
                    log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                    bet = placeBackBet(odds, SXALKom.Instance.MinStake);
                }
                catch (SXALInsufficientFundsException)
                {
                    log("Not enough money on betting account. Retrying!");
                    return false;
                }
                //}
                //else return false;

                return true;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                throw new SXALBetInProgressException();
            }
        }
        private void removeBetFromList(SXALMUBet muBet)
        {
            log("Trying to Remove a bet from the internal Betlist");
            if (muBet == null)
            {
                log("No bet was given: Leaving");
                return;
            }

            switch (muBet.BetType)
            {
                case SXALBetTypeEnum.B:
                    {
                        log("Bet to Remove is a Back Bet");
                        if (this.Back.Bets.ContainsKey(muBet.BetId))
                        {
                            log("Bet was found. Removing it from the List of Back Bets");
                            this.Back.Bets.Remove(muBet.BetId);
                        }
                        break;
                    }
                case SXALBetTypeEnum.L:
                    {
                        log("Bet to Remove is a Lay Bet");
                        if (this.Lay.Bets.ContainsKey(muBet.BetId))
                        {
                            log("Bet was found. Removing it from the List of Lay Bets");
                            this.Lay.Bets.Remove(muBet.BetId);
                        }
                        break;
                    }
            }
        }
        private void updateFromBet(SXALBet bet)
        {
            if (bet == null)
            {
                return;
            }

            switch (bet.BetType)
            {
                case SXALBetTypeEnum.B:
                    {
                        if (this.Back.Bets.ContainsKey(bet.BetId))
                        {
                                    // Wette schon vorhanden: hat sich die größe geändert?
                            if (bet.MatchedSize != this.Back.Bets[bet.BetId].MatchedSize ||
                               bet.MatchedDate != this.Back.Bets[bet.BetId].MatchedDate)
                            {
                                log(String.Format("Back Bet {0} has changed. Updating it", bet.BetId));
                                logBetAmount(String.Format("Old Match Date {0}. Old Match Size {1}. New Match Date {2}. New Match Size {3}",
                                    this.Back.Bets[bet.BetId].MatchedDate, this.Back.Bets[bet.BetId].MatchedSize, bet.MatchedDate, bet.MatchedSize));
                                this.Back.Bets[bet.BetId] = bet;
                            }
                            else
                            {
                                log(String.Format("Adding Back Bet {0} to List of Back Bet List", bet.BetId));
                                logBetAmount(String.Format("Adding Back Bet {0}. Match Date {1}. Match Size {2}", bet.BetId, bet.PlacedDate, bet.MatchedSize));
                                this.Back.Bets.Add(bet.BetId, bet);
                            }
                        }
                        break;
                    }
                case SXALBetTypeEnum.L:
                    {
                        if (this.Lay.Bets.ContainsKey(bet.BetId))
                        {
                            // Wette schon vorhanden: hat sich die größe geändert?
                            if (bet.MatchedSize != this.Lay.Bets[bet.BetId].MatchedSize ||
                               bet.MatchedDate != this.Lay.Bets[bet.BetId].MatchedDate)
                            {
                                log(String.Format("Lay Bet {0} has changed. Updating it", bet.BetId));
                                logBetAmount(String.Format("Old Match Date {0}. Old Match Size {1}. New Match Date {2}. New Match Size {3}",
                                                                        this.Back.Bets[bet.BetId].MatchedDate, this.Back.Bets[bet.BetId].MatchedSize, bet.MatchedDate, bet.MatchedSize));
                                this.Lay.Bets[bet.BetId] = bet;
                            }
                            else
                            {
                                log(String.Format("Adding Lay Bet {0} to List of Lay Bet List", bet.BetId));
                                logBetAmount(String.Format("Adding Lay Bet {0}. Match Date {1}. Match Size {2}", bet.BetId, bet.PlacedDate, bet.MatchedSize));
                                this.Lay.Bets.Add(bet.BetId, bet);
                            }
                        }
                        break;
                    }
            }
        }
        private void updateFromMUBet(SXALMUBet muBet)
        {
            if (muBet == null)
                return;
            while (true)
            {
                try
                {
                    switch (muBet.BetType)
                    {
                        //BackWette
                        case SXALBetTypeEnum.B:
                            {
                                if (this.Back.Bets.ContainsKey(muBet.BetId))
                                {
                                    // Wette schon vorhanden: hat sich die größe geändert?
                                    if (muBet.Size != this.Back.Bets[muBet.BetId].MatchedSize ||
                                       muBet.MatchedDate != this.Back.Bets[muBet.BetId].MatchedDate)
                                    {
                                        SXALBet bet = SXALKom.Instance.getBetDetail(muBet.BetId);

                                        if (bet == null)
                                        {
                                            Thread.Sleep(100);
                                            continue;
                                        }

                                        log(String.Format("Back Bet {0} has changed. Updating it", bet.BetId));
                                        logBetAmount(String.Format("Old Match Date {0}. Old Match Size {1}. New Match Date {2}. New Match Size {3}",
                                            this.Back.Bets[bet.BetId].MatchedDate, this.Back.Bets[bet.BetId].MatchedSize, bet.MatchedDate, bet.MatchedSize));

                                        this.Back.Bets[bet.BetId] = bet;
                                    }
                                }
                                else
                                {
                                    SXALBet bet = SXALKom.Instance.getBetDetail(muBet.BetId);
                                    if (bet == null)
                                    {
                                        Thread.Sleep(100);
                                        continue;
                                    }

                                    log(String.Format("Adding Back Bet {0} to List of Back Bet List", bet.BetId));
                                    logBetAmount(String.Format("Adding Back Bet {0}. Placed Date {1}. Match Size {2}", bet.BetId, bet.PlacedDate, bet.MatchedSize));

                                    this.Back.Bets.Add(bet.BetId, bet);
                                }
                                return;
                            }
                        //LayWette
                        case SXALBetTypeEnum.L:
                            {
                                if (this.Lay.Bets.ContainsKey(muBet.BetId))
                                {
                                    // Wette schon vorhanden: hat sich die größe geändert?
                                    if (muBet.Size != this.Lay.Bets[muBet.BetId].MatchedSize ||
                                       muBet.MatchedDate != this.Lay.Bets[muBet.BetId].MatchedDate)
                                    {
                                        SXALBet bet = SXALKom.Instance.getBetDetail(muBet.BetId);
                                        if (bet == null)
                                        {
                                            Thread.Sleep(100);
                                            continue;
                                        }

                                        log(String.Format("Lay Bet {0} has changed. Updating it", bet.BetId));
                                        logBetAmount(String.Format("Old Match Date {0}. Old Match Size {1}. New Match Date {2}. New Match Size {3}",
                                                                                this.Back.Bets[bet.BetId].MatchedDate, this.Back.Bets[bet.BetId].MatchedSize, bet.MatchedDate, bet.MatchedSize));

                                        this.Lay.Bets[bet.BetId] = bet;
                                    }
                                }
                                else
                                {
                                    SXALBet bet = SXALKom.Instance.getBetDetail(muBet.BetId);
                                    if (bet == null)
                                    {
                                        Thread.Sleep(100);
                                        continue;
                                    }

                                    log(String.Format("Adding Lay Bet {0} to List of Lay Bet List", bet.BetId));
                                    logBetAmount(String.Format("Adding Lay Bet {0}. Match Date {1}. Match Size {2}", bet.BetId, bet.PlacedDate, bet.MatchedSize));

                                    this.Lay.Bets.Add(bet.BetId, bet);
                                }
                                return;
                            }
                    }
                }
                catch (KeyNotFoundException)
                {
                    return;
                }
                catch (Exception e)
                {
                    ExceptionWriter.Instance.WriteException(e);
                    Thread.Sleep(100);
                    continue;
                }
            }
        }

        /// <summary>
        /// Stopp/Loss Thread
        /// </summary>
        public void marketWatcherThread()
        {
            try
            {
                lock (m_exitThreadLock)
                {
                    log("Stopp/Loss: Starting a Stopp/Loss Thread");                    
                    double money = 0.0;
                    bool recheck = false;

                    if (stopLossCheckCalculate(out money))
                    {
                        log("Stopp/Loss: No Stopp/Loss: Leaving");
                        return;
                    }

                    recheckBets();

                    while (!this.m_marketWatcherStop)
                    {
                            _stoppLossUnstoppable = false;
                            if (m_config.ExitWatchActivationPlaytime == -1)
                            {
                                log("Stopp/Loss: Configured to no Stopp/Loss. Leaving Stopp/Loss Thread");
                                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                {
                                    log("Stopp/Loss: Notifying Listernes: Stop Stopp/Loss Countdown");
                                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                                }
                                return;
                            }

                            

                            EventHandler<SXMessageEventArgs> message = MessageEvent;
                            TimeSpan span = new TimeSpan(0, 0, m_config.CheckExitOddsSeconds);
                            EventHandler<SXSetStopLossTimer> openBetHandler = SetStopLossTimer;
                            if (openBetHandler != null)
                            {
                                log("Stopp/Loss: Notifying Listeners: Start Stopp/Loss Countdown");
                                openBetHandler(this, new SXSetStopLossTimer(this.Match, span));
                            }

                            log(String.Format("Stopp/Loss: Wait for {0} seconds", span.TotalSeconds));


                            if (!m_active)
                            {
                                log("Stopp/Loss: Strategy is not active. Leaving Stopp/Loss Thread");
                                EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;
                                if (stopHandler != null)
                                {
                                    log("Stopp/Loss: Notifying Listeners: Stop Stopp/Loss Countdown");
                                    stopHandler(this, new SXStopStopLossTimer(this.Match));
                                }
                                _closeTradeUnstoppable = false;
                                return;
                            }

                            Thread.Sleep(span);
                            _stoppLossUnstoppable = true;

                        if(recheck)
                        {
                            recheckBets();
                            recheck = false;
                        }

                            if (this.Back.Bets.Count > 0)
                            {
                                log(String.Format("Stopp/Loss: Reading all Bets for market {0}", this.Lay.MarketId));
                                SXALMUBet[] muBets = SXALKom.Instance.getBetsMU(this.Lay.MarketId);
                                if (muBets == null)
                                {
                                    log(String.Format("Found no Bets in the Status MU für the market {0}. Retrying", this.Lay.MarketId));
                                    continue;
                                }

                                log(String.Format("Number of Bets for the market {0} is {1}", this.Lay.MarketId, muBets.Length));

                                //Überprüfe alle Wetten
                                foreach (SXALMUBet muBet in muBets)
                                {
                                    if (muBet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, muBet.MarketId))
                                    {
                                        log("Bet is not for the Draw market:Skipping");
                                        continue;
                                    }

                                    switch (muBet.BetStatus)
                                    {
                                        // Wette ist vollständig erfüllt
                                        case SXALBetStatusEnum.M:
                                            {
                                                log(String.Format("Stopp/Loss: Bet {0} is matched", muBet.BetId));
                                                updateFromMUBet(muBet);
                                                break;
                                            }
                                    }
                                }
                            }

                            //Thread.Sleep(30000);
                            
                            if (this.State == SETSTATE.SETTLED)
                            {
                                log("Stopp/Loss: Trade already settled. Leaving Stopp/Loss Thread");

                                this.m_marketWatcherStop = true;
                                EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;
                                if (stopHandler != null)
                                {
                                    log("Stopp/Loss: Notifying Listeners. Stop Stopp/Loss Countdown");
                                    stopHandler(this, new SXStopStopLossTimer(this.Match));
                                }

                                if (message != null)
                                {
                                    log("Stopp/Loss: Notifying Listeners: Trade already settled");
                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSettled, LayTheDraw.strModule));
                                }

                                _stoppLossUnstoppable = false;
                                return;
                            }



                            if (m_betLay.Bets.Count == 0)
                            {
                                log("Stopp/Loss: No Lay Bets. Leaving Stopp/Loss Thread");
                                
                                EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;
                                if (stopHandler != null)
                                {
                                    log("Stopp/Loss: Notifying Listeners: Stop Stopp/Loss Countdown");
                                    stopHandler(this, new SXStopStopLossTimer(this.Match));
                                }
                                _stoppLossUnstoppable = false;
                                return;

                            }


                            // Falls Marketwatcher läuft => abschliessen
                            if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) == true)
                            {
                                log("Stopp/Loss: A Close Trade is already running. Leaving Stopp/Loss Thread");

                                if (message != null)
                                {
                                    log("Stopp/Loss: Notifying Listeners: A Close Trade is already running");
                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strNoStopLossRun, LayTheDraw.strModule));
                                }
                                
                                EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;
                                if (stopHandler != null)
                                {
                                    log("Stopp/Loss: Notifying Listeners: Stop Stopp/Loss Thread");
                                    stopHandler(this, new SXStopStopLossTimer(this.Match));
                                }

                                _stoppLossUnstoppable = false;
                                return;
                            }

                            log("Stopp/Loss: Reading Market Prices");
                            try
                            {
                                SXALMarketLite marketLite = SXALKom.Instance.getMarketInfo(m_betLay.MarketId);
                            
                                if (marketLite == null)
                                {
                                    log("Stopp/Loss: No market informations. Next Iteration");
                                    if (message != null)
                                    {
                                        log("Stopp/Loss: Notify Listeners: No market informations");
                                        message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strNoMarketInfo, LayTheDraw.strModule));
                                    }
                                    
                                    continue;
                                }

                                // Falls Markt pausiert => raus
                                if (marketLite.MarketStatus == SXALMarketStatusEnum.SUSPENDED)
                                {
                                    log("Stopp/Loss: Market is suspended. Next Iteration");
                                    if (message != null)
                                    {
                                        log("Stopp/Loss: Notifying Listeners: Market is suspended");
                                        message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSuspended, LayTheDraw.strModule));
                                    }

                                    continue;
                                }
                            }
                            catch (SXALMarketDoesNotExistException smdnee)
                            {
                                log(smdnee.Message);
                                ExceptionWriter.Instance.WriteException(smdnee);

                                EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;
                                if (stopHandler != null)
                                {
                                    log("Stopp/Loss: Notifying Listeners: Stop Stopp/Loss Thread");
                                    stopHandler(this, new SXStopStopLossTimer(this.Match));
                                }

                                _stoppLossUnstoppable = false;

                                if (message != null)
                                {                                    
                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, smdnee.Message, LayTheDraw.strModule));
                                }
                                return;
                            }
                            // Hole Preisübersicht

                            try
                            {
                                log("Stopp/Loss: Reading market prices");
                                SXALMarketPrices marketPrices = SXALKom.Instance.getMarketPrices(m_betLay.MarketId);
                                if (marketPrices == null)
                                {
                                    log("Stopp/Loss: No Market prices. Next Iteration");
                                    if (message != null)
                                    {
                                        log("Stopp/Loss: Notifying Listeners: No Market Prices");
                                        message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strNoMarketPrices, LayTheDraw.strModule));
                                    }

                                    continue;
                                }
                                // Falls Markt pausiert => rause
                                if (marketPrices.MarketStatus == SXALMarketStatusEnum.SUSPENDED)
                                {
                                    log("Stopp/Loss: Market is suspended. Next Iteration");
                                    if (message != null)
                                    {
                                        log("Stopp/Loss: Notifying Listeners: Market is suspended");
                                        message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSuspended, LayTheDraw.strModule));
                                    }
                                    
                                    continue;
                                }

                                if (marketPrices.RunnerPrices.Length < 3)
                                {
                                    log("Stopp/Loss: Not enough Back prices available. Next Iteration");
                                    
                                    continue;
                                }
                                SXALRunnerPrices runnerPrice = marketPrices.RunnerPrices[2];
                                if (runnerPrice.BestPricesToBack.Length > 0)
                                {
                                    SXALPrice priceBack = runnerPrice.BestPricesToBack[0];
                                    log(String.Format("Stopp/Loss: Reading best Back price {0}", priceBack.Price));

                                    //Überprüfen, ob beste Marktquote unterhalb der Begrenzung für den Abbruch des Stopp/Loss
                                    if(priceBack.Price <= m_config.NoStoppLossOdds)
                                    {
                                        log(String.Format("Stopp/Loss: Market Odds {0} are below lower limit for Stopp/Loss of {1}. Exit.", priceBack.Price, m_config.NoStoppLossOdds));
                                        this.m_marketWatcherStop = true;
                                        EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;
                                        if (stopHandler != null)
                                        {
                                            log("Stopp/Loss: Notifying Listeners: Stop Stopp/Loss Countdown");
                                            stopHandler(this, new SXStopStopLossTimer(this.Match));
                                        }

                                        _stoppLossUnstoppable = false;
                                        return;
                                    }

                                    //Price priceLay = runnerPrice.bestPricesToLay[0];
                                    // Ausstiegsquote berechnen
                                    double exitOdds = this.Lay.BetPrice / 100 * m_config.ExitCloseOdds;
                                    log(String.Format("Stopp/Loss: Maximum exit odss are calculated: {0} / 100 * {1} = {2}", this.Lay.BetPrice, m_config.ExitCloseOdds, exitOdds));
                                    // Falls Quote <= Ausstiegsquote Wette sofort abschliessen
                                    if (priceBack.Price <= exitOdds || priceBack.Price >= this.Lay.BetPrice)
                                    {
                                        log(String.Format("Stopp/Loss: Current Odds {0} are lower than calculated exitOdds {1} or higher than original Lay odds {2}", priceBack.Price, exitOdds, this.Lay.BetPrice));
                                        if (this.State == SETSTATE.SETTLED)
                                        {
                                            log("Stopp/Loss: Market is alread settled. Leaving Stopp/Loss Thread");                                            
                                            this.m_marketWatcherStop = true;
                                            EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;
                                            if (stopHandler != null)
                                            {
                                                log("Stopp/Loss: Notifying Listeners: Stop Stopp/Loss Countdown");
                                                stopHandler(this, new SXStopStopLossTimer(this.Match));
                                            }

                                            if (message != null)
                                            {
                                                log("Stopp/Loss: Notifying Listeners: Trades is settled");
                                                message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSettled, LayTheDraw.strModule));
                                            }

                                            _stoppLossUnstoppable = false;
                                            return;
                                        }
                                        
                                        log("Stopp/Loss: Start calculation of Back Bet Amount");

                                        double baseMoney = 0.0;
                                        if (stopLossCheckCalculate(out baseMoney))
                                        {
                                            log("Stopp/Loss: No Stopp/Loss: Leaving");
                                            return;
                                        }
                                        //double dMoney = calculateBackBet(priceBack.price);
                                        double dMoney = calculateBackBet(priceBack.Price, baseMoney);

                                        if (dMoney == Double.NaN)
                                        {
                                            log("Money is NaN. Leaving!");
                                            return;
                                        }

                                        dMoney = Math.Round(dMoney, 2);


                                        if (message != null)
                                        {
                                            log("Stopp/Loss: Notifying Listeners: Place a Stopp/Loss Back Bet");
                                            message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strPlaceBet, m_marketName, priceBack.Price, dMoney), LayTheDraw.strModule));
                                        }

                                        m_commission = marketPrices.MarketBaseRate * 0.01f;

                                        log(String.Format("Stopp/Loss: Commission Rate for market is {0}", m_commission));
                                        SXALBet betBack = null;
                                        try
                                        {
                                            if (dMoney < SXALBankrollManager.Instance.MinStake)
                                            {
                                                log("Stopp/Loss: Back bet will be below the minimum stake allowed by Betfair. Executing special 'Below Min Bet'-Logic");
                                                

                                                betBack = placeBackBetBelowMin(priceBack.Price, dMoney);
                                            }
                                            else
                                            {
                                                log("Stopp/Loss: Place a new Back Bet");
                                                try
                                                {
                                                    betBack = placeBackBet(priceBack.Price, dMoney);
                                                }
                                                catch (SXALNoBetBelowMinAllowedException)
                                                {
                                                    log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                                                    betBack = placeBackBet(priceBack.Price, SXALKom.Instance.MinStake);
                                                }
                                            }
                                        }
                                        catch (SXALInsufficientFundsException)
                                        {
                                            log("Not enough money on betting account. Retrying!");
                                            continue;
                                        }
                                        catch (SXALBetInProgressException bipe)
                                        {
                                            log(String.Format("Received a BetInProgressException with betId {0}. Rechecking Betstatus", bipe.BetId));
                                            //Betfair Zeit geben den Markt abzuschliessen
                                            Thread.Sleep(3000);
                                            SXALMUBet[] muBets = null;
                                            if (bipe.BetId == 0)
                                                muBets = SXALKom.Instance.getBetsMU(this.Lay.MarketId);
                                            else
                                                muBets = SXALKom.Instance.getBetMU(bipe.BetId);

                                            if (muBets != null)
                                            {
                                                foreach (SXALMUBet muBet in muBets)
                                                {
                                                    //TODO: SelectionId für Drawmarkt noch in Konstante
                                                    if (muBet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, muBet.MarketId))
                                                        continue;

                                                    //Uns interessieren hier nur Back-Wetten
                                                    if (muBet.BetType == SXALBetTypeEnum.L)
                                                        continue;

                                                    // Uns interessieren hier nur neue Wetten
                                                    if (this.Back.Bets[muBet.BetId] != null)
                                                        continue;


                                                    betBack = SXALKom.Instance.getBetDetail(muBet.BetId);

                                                }
                                            }
                                        }

                                        if (betBack != null)
                                        {
                                            log("Stopp/Loss: New Back Bet was placed");
                                            EventHandler<SXStopStopLossTimer> stopHandler = StopStopLossTimer;

                                            if (stopHandler != null)
                                            {
                                                log("Stopp/Loss: Notifying Listeners: Stop Stopp/Loss Countdown");
                                                stopHandler(this, new SXStopStopLossTimer(this.Match));
                                            }

                                            this.Back.Bets.Add(betBack.BetId, betBack);
                                            log(String.Format("Stopp/Loss: Adding Back Bet to Back Bet List. List has now {0} Bets", this.Back.Bets.Count));
                                            this.m_marketWatcherStop = true;
                                            //this.m_LayRisk = m_LayRisk - (this.Back.BetSize * this.Back.BetPrice);
                                            // Falls Wetter offen oder teiloffen => Wächterthread starten
                                            if (betBack.BetStatus == SXALBetStatusEnum.U || betBack.BetStatus == SXALBetStatusEnum.MU)
                                            {
                                                log("Stopp/Loss: Bet is unmatched or partly matched. Starting Open Bet Watcher Thread");
                                                m_openBack = betBack;
                                                
                                                m_betStatusWatcherThread = new Thread(this.openBetWatcher/*this.betStatusWatcherThread*/);
                                                m_betStatusWatcherThread.IsBackground = true;
                                                m_betStatusWatcherThread.Start();

                                                if (message != null)
                                                {
                                                    log("Stopp/Loss: Notifying Listeners: Unmatched or partly matched bet");
                                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strBetUnmatched, m_marketName, dMoney, priceBack.Price), LayTheDraw.strModule));
                                                }

                                                _stoppLossUnstoppable = false;
                                                return;
                                            }
                                            else
                                            {
                                                log(String.Format("Stopp/Loss: Betstatus is {0}. Leaving now Close Trade Thread", betBack.BetStatus.ToString()));
                                                
                                                EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                                                double backWin = Back.RiskWin - Lay.RiskWin;
                                                double backLost = Lay.BetSize - Back.BetSize;
                                                log("Stopp/Loss: Calculating new Risk and Win");
                                                if (handler != null)
                                                {
                                                    log("Stopp/Loss: Notifying Listeners: Changed RiskWin");
                                                    handler(this, new BFRiskWinChangedEventArgs(this.Match, Math.Round(backWin, 2), Math.Round(backLost, 2)));
                                                }

                                                if (message != null)
                                                {
                                                    log("Stopp/Loss: Notifying Listeners: New Matched Bet was placed");
                                                    message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strBetMatched, m_marketName, Back.BetSize, Back.BetPrice), LayTheDraw.strModule));
                                                }

                                                _stoppLossUnstoppable = false;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            log("Close Trade: No Back Bet was placed. Next iteration");
                                        }
                                    }
                                    else
                                    {
                                        log("Stopp/Loss: Odds are outside of range: Next Iteration");
                                        if (message != null)
                                        {
                                            log("Stopp/Loss: Notifying Listeners: Odds outside of range");
                                            message(this, new SXMessageEventArgs(DateTime.Now, m_marketName, String.Format(LayTheDraw.strOdds2HighForStopLoss, exitOdds), LayTheDraw.strModule));
                                        }
                                    }
                                }

                            
                        }
                        catch (SXALMarketDoesNotExistException mdnee)
                        {
                            ExceptionWriter.Instance.WriteException(mdnee);
                            //EventHandler<SXMessageEventArgs> message = MessageEvent;
                            String msg = String.Format("Market {0} does not exist! ExpectionMessage {1}. Leaving Trade!",m_marketName , mdnee.Message);
                            log(msg);
                            if (message != null)
                            {
                                message(this, new SXMessageEventArgs(DateTime.Now, Match, msg, "Lay The Draw"));
                            }

                            EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                            if (stopHandler != null)
                            {
                                log("Stop/Loss: Inform Listeners: Stop Stop/Loss Countdown");
                                stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                            }
                            _stoppLossUnstoppable = false;

                            return;
                        }
                        catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
                        {
                            ExceptionWriter.Instance.WriteException(mnsnae);
                            //EventHandler<SXMessageEventArgs> message = MessageEvent;
                            String msg = String.Format("Market {0} neither suspended nor active! ExpectionMessage {1}. Leaving Trade!",m_marketName, mnsnae.Message);
                            log(msg);
                            if (message != null)
                            {
                                message(this, new SXMessageEventArgs(DateTime.Now, Match, msg, "Lay The Draw"));
                            }

                            EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                            if (stopHandler != null)
                            {
                                log("Stop/Loss: Inform Listeners: Stop Stop/Loss Countdown");
                                stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                            }
                            _stoppLossUnstoppable = false;

                            return;
                        }
                        catch (ThreadAbortException)
                        {
                            EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                            if (stopHandler != null)
                            {
                                log("Stop/Loss: Inform Listeners: Stop Stop/Loss Countdown");
                                stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                            }
                            log("Stopp/Loss: Received a Thread Abort Exception: Leaving");
                            _stoppLossUnstoppable = false;
                            return;
                            //throw tae;
                        }
                        catch (Exception exc)
                        {
                            EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                            if (stopHandler != null)
                            {
                                log("Stop/Loss: Inform Listeners: Stop Stop/Loss Countdown");
                                stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                            }
                            ExceptionWriter.Instance.WriteException(exc);
                            Thread.Sleep(10000);
                            recheck = true;
                            _stoppLossUnstoppable = false;
                            continue;
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                log("Stopp/Loss: Received a Thread Abort Exception: Leaving");
                _stoppLossUnstoppable = false;
                return;
                //throw tae;
            }

            EventHandler<SXStopCloseTradeTimer> stopHandler2 = StopCloseTradeTimer;
            if (stopHandler2 != null)
            {
                log("Stopp/Loss: Notifying Listernes: Stop Stopp/Loss Countdown");
                stopHandler2(this, new SXStopCloseTradeTimer(this.Match));
            }
            _stoppLossUnstoppable = false;
        }


        private double calculateBackBet(double backOdds)
        {
            log("Calculating Size of Back Bet");
            log(String.Format("Odds for Back market are {0}", backOdds));
            log(String.Format("Number of Lay Bets in Trade is {0}", this.Lay.Bets.Count));
            log(String.Format("The average Odds for existing Lay Bets are {0}", this.Lay.BetPrice));
            logBetAmount(String.Format("The Bet Size for existing Lay Bets is {0}", this.Lay.BetSize));
            log(String.Format("Number of Back Bets in Trade is {0}", this.Back.Bets.Count));
            log(String.Format("The average Odds for existing Back Bets are {0}", this.Back.BetPrice));
            logBetAmount(String.Format("The Bet Size for existing Back Bets is {0}", this.Back.BetSize));
            double money = 0.0;
            double LayAmount = (Lay.BetSize - Back.BetSize);
            logBetAmount(String.Format("The Lay Amount for calculating the Back Size is {0}", LayAmount));

            money = (LayAmount / (backOdds / Lay.BetPrice));
            money = Math.Round(money, 2);
            logBetAmount(String.Format("The calculated amount to place a Back Bet with the parameters LayAmount({0}),"
                + "BackOdds({1}, Lay Odds ({2}) is {3} {4}", LayAmount, backOdds, Lay.BetPrice, money, SXALBankrollManager.Instance.Currency));

            if (money == 0.0)
                money = 0.01;
            return money;
        }

        private double calculateBackBet(double backOdds, double initMoney)
        {
            log("Calculating Size of Back Bet with passed initial Money");
            log(String.Format("Odds for Back market are {0}", backOdds));
            log(String.Format("Number of Lay Bets in Trade is {0}", this.Lay.Bets.Count));
            log(String.Format("The average Odds for existing Lay Bets are {0}", this.Lay.BetPrice));
            logBetAmount(String.Format("The Bet Size for existing Lay Bets is {0}", this.Lay.BetSize));
            log(String.Format("Number of Back Bets in Trade is {0}", this.Back.Bets.Count));
            log(String.Format("The average Odds for existing Back Bets are {0}", this.Back.BetPrice));
            logBetAmount(String.Format("The Bet Size for existing Back Bets is {0}", this.Back.BetSize));
            double money = 0.0;
            double LayAmount = initMoney;
            logBetAmount(String.Format("The Lay Amount for calculating the Back Size is {0}", initMoney));

            money = (LayAmount / (backOdds / Lay.BetPrice));
            money = Math.Round(money, 2);
            logBetAmount(String.Format("The calculated amount to place a Back Bet with the parameters LayAmount({0}),"
                + "BackOdds({1}, Lay Odds ({2}) is {3} {4}", LayAmount, backOdds, Lay.BetPrice, money, SXALBankrollManager.Instance.Currency));

            if (money == 0.0)
                money = 0.01;
            return money;
        }

        private bool stopLossCheckCalculate(out double money)
        {
            try
            {
                money = 0.0;
                if (m_config.Check00StoppLoss == false && m_score.getScoreState() != SCORESTATE.undraw)
                {
                    log("There's no check for Scoreline 0 - 0 Trade. Full Stop/Loss!");
                    money = (Lay.BetSize - Back.BetSize);
                    return false;
                }
                else if (m_config.Check00StoppLoss == true && m_score.getScoreState() == SCORESTATE.initdraw)
                {
                    log("There's a check for Scoreline 0 - 0 Trade and Score is 0 - 0. Checking special Stop/Loss rules");
                    // Gewinn aus Scoreline Trade holen
                    double win00 = ((m_00Back.BetPrice * m_00Back.BetSize) - m_00Back.BetSize) - ((m_00Lay.BetPrice * m_00Lay.BetSize) - m_00Lay.BetSize);
                    // Prozentanteil Verlust errechnen
                    double drawLoss = ((((m_betBack.BetPrice * m_betBack.BetSize) - m_betBack.BetSize) - ((m_betLay.BetPrice * m_betLay.BetSize) - m_betLay.BetSize)) / 100) * m_config.Win00Percentage;
                    drawLoss = Math.Abs(drawLoss);
                    if (win00 >= drawLoss && m_config.No00StoppLoss)
                    {
                        log("Win from Scoreline 0 - 0 Trade is bigger than the procentual Loss of Draw and No Stop/Loss: Leaving!");
                        return true;
                    }
                    else if (win00 >= drawLoss && !m_config.No00StoppLoss)
                    {
                        log("Win from Scoreline 0 - 0 Trade is bigger than the procentual Loss of Draw and No Stop/Loss: Calculating Money!");
                        money = (Lay.BetSize - Back.BetSize) / 100 * m_config.StoppLoss00BetPercentage;
                    }
                    else
                    {
                        log("Win from Scoreline 0 - 0 Trade is Lower than the procentual Loss of Draw and No Stop/Loss: Full Stop/Loss!");
                        money = (Lay.BetSize - Back.BetSize);
                    }
                    return false;
                }
                else if (m_config.Check00StoppLoss == true && m_score.getScoreState() == SCORESTATE.draw)
                {
                    log("Score is not 0 - 0 but Draw: Full Stop/Loss");
                    money = (Lay.BetSize - Back.BetSize);
                    return false;
                }

                log("Score is undraw: No Stop/Loss");
            }
            catch (Exception exc)
            {
                log("There was an exception. Please check ExceptionsOutput.txt");
                ExceptionWriter.Instance.WriteException(exc);
                money = 0.0;
                return false;
            }
            return true;
        }

        /*
        private double calculateBackBetOld(bool firstRun, bool allNeg, double backOdds,
                                     double backAmount, double stepAmount)
        {
            //TODO: Berechnung auf Kollektion umstellen
            double riskBack =  this.Back.BetSize + backAmount;
            double winBack = (this.Back.RiskWin) + backAmount * backOdds - backAmount;
            //winBack = winBack - (winBack * 0.05);
            double laybackwin = winBack - this.Lay.RiskWin;
            double backlaywin = this.Lay.BetSize - (this.Back.BetSize + backAmount);
            //backlaywin = backlaywin - (backlaywin * 0.05);
            double diff = laybackwin - backlaywin;

            // Erstdurchlauf
            if (firstRun)
            {
                // Falls diff > als Toleranz => Ergebniss stimmt
                if (diff > LOWTOLERANZ)
                    return backAmount;
                else // Einsatz Verdoppeln
                    return calculateBackBet(false, true, backOdds, backAmount + backAmount, backAmount);
            }
            else if (allNeg)
            {
                // Falls Weiterhin negativ, dann Einsatz verdoppeln
                if (diff < LOWTOLERANZ)
                    return calculateBackBet(false, true, backOdds, backAmount + backAmount, backAmount);
                else if (diff > HIGHTOLERANZ) // Falls ins positive gedreht, Einsatz - halben Schritt
                    return calculateBackBet(false, false, backOdds, backAmount - stepAmount / 2, stepAmount / 2);
                else // Innerhalb Toleranz 
                    return backAmount;

            }

            // normaler durchgang
            // Diff Negativ = Einsatz + Step/2
            if (diff < LOWTOLERANZ)
                return calculateBackBet(false, false, backOdds, backAmount + stepAmount / 2, stepAmount / 2);
            else if (diff > HIGHTOLERANZ) // Diff Positiv => Einstz - Step/2
                return calculateBackBet(false, false, backOdds, backAmount - stepAmount / 2, stepAmount / 2);

            return backAmount;
        }
        */

        private SXALBet placeBackBet(double price, double money)
        {
            return SXALKom.Instance.placeBackBet(m_betLay.MarketId, m_betLay.SelectionId, price, money);
        }

        private SXALBet placeBackBetBelowMin(double price, double money)
        {
            try
            {
                try
                {
                    return SXALKom.Instance.placeBackBet(m_betLay.MarketId, m_betLay.SelectionId, 0, price, money);
                }
                catch (SXALNoBetBelowMinAllowedException)
                {
                    log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                    return SXALKom.Instance.placeBackBet(m_betLay.MarketId, m_betLay.SelectionId, 0, price, SXALKom.Instance.MinStake);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                throw new SXALBetInProgressException();
            }
        }

        private void doDeactivation()
        {
            try
            {
                log("Deactivate strategy");
                if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) && !_closeTradeUnstoppable)
                {
                    log("Deactivation: Stopp a running Close Trade Thread");
                    
                    m_betWatcherThread.Abort();
                    m_betWatcherThread.Join();
                }
                if (SXThreadStateChecker.isStartedBackground(m_marketWatcherThread))
                {
                    log("Deactivation: Stopp a running Stopp/Loss Thread");
                    
                    m_marketWatcherThread.Join();
                    m_marketWatcherThread.Abort();
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void doActivation()
        {
            try
            {
                log("Activate Strategy");
                // Abgeschlossen oder im Abschluss => alles i.O
                if (this.State != SETSTATE.UNSETTELED)
                {
                    log("Activation: Market is settled");
                    
                    return;
                }

                if (m_score == null || (((HLLiveScore)m_score).IsScore1Connected() == false && ((HLLiveScore)m_score).IsScore2Connected() == false))
                {
                    log("Activation: No liveticker(s)");
                    
                    return;
                }

                // nicht gleichstand => dann überprüfe, ob Abschluss nötig ist
                if (this.ScoreState != SCORESTATE.initdraw && this.ScoreState != SCORESTATE.draw)
                {
                    log("Activation: Score is not draw. Check for trade out");
                    if (m_betWatcherThread != null)
                    {
                        // Falls Wettabschlussthread schon läuft => alles i.O.
                        if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) == true)
                        {
                            log("Activation: There's already a Close Trade Thread running");
                            
                            return;
                        }

                        //Ansonsten Wettabschlussthread starten
                        log("Activation: Start a Close Trade Thread");
                        startCloseTrade();
                    }
                }

            }
            catch (Exception exc)
            {
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
        
        #region EventHandler
        public void GoalEventHandler(object sender, GoalEventArgs e)
        {
            lock (m_goalEventLock)
            {
                try
                {
                    log("Goal Event: Received a Goal Event from liveticker");

                    if (m_scoreA == e.ScoreA && m_scoreB == e.ScoreB)
                    {
                        log("Goal Event: Score hasn't changed. Leaving");
                        return;
                    }

                    m_scoreA = e.ScoreA;
                    m_scoreB = e.ScoreB;

                    


                    EventHandler<BFGoalScoredEventArgs> handler = GoalScoredEvent;

                    if (handler != null)
                    {
                        log("Goal Event: Notify Listeners: Score has changed");
                        handler(this, new BFGoalScoredEventArgs(this.Match, e.ScoreA, e.ScoreB));
                    }

                    // Falls deaktiviert => raus
                    if (!m_active)
                    {
                        log("Goal Event: Strategy is not activated. Leaving");
                        
                        return;
                    }

                    log("Goal Event: Start a Close Trade Thread");
                    startCloseTrade();
                }
                catch (Exception exc)
                {
                    EventHandler<SXMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new SXMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
            
        }

        public void PlaytimeEventHandler(object sender, PlaytimeTickEventArgs e)
        {
            lock (m_playtimeEventLock)
            {
                try
                {
                    if (m_playtime >= e.Tick)
                        return;

                    m_playtime = e.Tick;
                    log(String.Format("Playtime Event: Playtime is now {0}", m_playtime));
                    /*
                    LTDConfigurationRW reader = null;
                    if(m_config != null)new LTDConfigurationRW();
                    */
                    EventHandler<SXPlaytimeEventArgs> handler = PlaytimeEvent;
                    if (handler != null)
                    {
                        log("Playtime Event: Notify Listeners: Playtime has changed");
                        handler(this, new SXPlaytimeEventArgs(this.Match, e.Tick));
                    }

                    if (e.Tick == -1)
                    {
                        log("Playtime Event: Invalid Playtime.");
                        return;
                    }

                    if (m_config.ExitWatchActivationPlaytime == -1)
                    {
                        log("Playtime Event: Trade configued to no Stopp/Loss");
                        return;
                    }

                    if (e.Tick >= m_config.ExitWatchActivationPlaytime)
                    {
                        log(String.Format("Playtime Event: Playtime {0} for Stopp/Loss observation has reached. Starting Stopp/Loss Thread", m_config.ExitWatchActivationPlaytime));
                        startStoppLoss();
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new SXMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        public void GoalBackEventHandler(object sender, GoalBackEventArgs e)
        {
            lock (m_goalEventLock)
            {
                try
                {
                    log("Goal Back Event: Received a Goal Back from the liveticker");
                    if (m_scoreA == e.ScoreA && m_scoreB == e.ScoreB)
                    {
                        log("Goal Back Event: Score hasn't changed. Leaving");
                        return;
                    }

                    m_scoreA = e.ScoreA;
                    m_scoreB = e.ScoreB;


                    

                    EventHandler<BFGoalScoredEventArgs> handler = GoalScoredEvent;
                    if (handler != null)
                    {
                        log("Goal Back Event: Notifying Listeners: Score has changed");
                        handler(this, new BFGoalScoredEventArgs(this.Match, e.ScoreA, e.ScoreB));
                    }

                    /*
                    // Falls ungleich 0 - 0 weitermachen
                    if (e.ScoreState != SCORESTATE.initdraw)
                    {
                        return;
                    }               
                    */

                    // Wenn BackWetterThread läuft => beenden
                    if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) == true)
                    {
                        log("Goal Back Event: A Close Trade is running. Stopping it");
                        
                        m_betWatcherThread.Abort();
                        m_betWatcherThread.Join();
                    }

                    if (e.ScoreState != SCORESTATE.initdraw)
                    {
                        log("Goal Back Event: Start a new Close Trade Thread because score is not 0 - 0.");
                        startCloseTrade();
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new SXMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        public void GameEndedEventHandler(object sender, GameEndedEventArgs e)
        {
            try
            {
                if (m_ended == true)
                    return;

                log("Game Ended Event: Received a Game ended from Liveticker");
                m_ended = true;

                

                if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) == true)
                {
                    log("Game Ended Event: A Close Trade is running. Stopping it");
                    
                    m_betWatcherThread.Abort();
                    m_betWatcherThread.Join();
                }

                if (SXThreadStateChecker.isStartedBackground(m_marketWatcherThread) == true)
                {
                    log("Game Ended Event: A Stopp/Loss is running. Stopping it");
                    
                    m_marketWatcherThread.Abort();
                    m_marketWatcherThread.Join();
                }

                if (SXThreadStateChecker.isStartedBackground(m_betStatusWatcherThread) == true)
                {
                    log("Game Ended Event: A Open Bet Watcher is running. Stopping it");
                    m_betStatusWatcherThread.Abort();
                    m_betStatusWatcherThread.Join();
                }

                EventHandler<SXGameEndedEventArgs> handler = GameEndedEvent;
                if (handler != null)
                {
                    log("Game Ended Event: Notify Listeners: Game has ended");
                    log("Game Ended Event: Calculate actual Winnings of Lossings");
                    char[] seps = { '/' };
                    double dGuV = 0.0;
                    // Unentschieden oder Mannschaft gewinnt?
                    if (this.ScoreState != SCORESTATE.undraw)
                    {
                        // Unendschieden
                        dGuV = this.Lay.BetSize - this.Back.BetSize;
                    }
                    else
                    {
                        dGuV = this.Back.RiskWin - this.Lay.RiskWin;
                    }

                    if (dGuV > 0)
                    {
                        dGuV = dGuV - (dGuV * m_commission);
                    }
                    dGuV = Math.Round(dGuV, 2);
                    handler(this, new SXGameEndedEventArgs(this.Match, DateTime.Now, dGuV));
                    
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
        #endregion

        public void log(string message)
        {
            try
            {
                TradeLog.Instance.writeLog(this.Match, "LayTheDraw", "Trader", String.Format("ID {0}: {1}", m_tradeId, message));
            }
            catch { }
        }

        public void logBetAmount(string message)
        {
            try
            {
                TradeLog.Instance.writeBetAmountLog(this.Match, "LayTheDraw", "Trader", String.Format("ID {0}: {1}", m_tradeId, message));
            }
            catch { }
        }

        #region IDisposable Member

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!this.disposed)
                {
                    log("Disposing Trade");
                    /*
                    log("Set bet watcher flag to stop");
                    log("Set market watcher flag to stop");
                    m_betWatcherStop = true;
                    m_marketWatcherStop = true;
                     * */
                    if (SXThreadStateChecker.isStartedBackground(m_marketWatcherThread))
                    //if (m_marketWatcherThread.ThreadState != (System.Threading.ThreadState.Background | System.Threading.ThreadState.Unstarted))
                    {
                        log("Stopping market Watcher Thread");
                        m_marketWatcherThread.Abort();
                        m_marketWatcherThread.Join(60000);
                    }
                    if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
                    //if (m_betWatcherThread.ThreadState != (System.Threading.ThreadState.Background | System.Threading.ThreadState.Unstarted))
                    {
                        log("Stopping bet Watcher Thread");
                        m_betWatcherThread.Abort();
                        m_betWatcherThread.Join(160000);
                    }
                    if (SXThreadStateChecker.isStartedBackground(m_betStatusWatcherThread))
                    //if (m_betStatusWatcherThread.ThreadState != (System.Threading.ThreadState.Background | System.Threading.ThreadState.Unstarted))
                    {
                        log("Stopping bet Status Watcher Thread");
                        m_betStatusWatcherThread.Abort();
                        m_betStatusWatcherThread.Join();
                    }

                    if (SXThreadStateChecker.isStartedBackground(m_riskWinUpdater))
                    {
                        log("Stopping Risk/Win Update Thread");
                        m_riskWinUpdater.Abort();
                        m_riskWinUpdater.Join();
                    }

                    if (this.Score != null)
                    {
                        this.Score.DecreaseRef();
                        this.Score = null;
                    }

                    SXMinutePulse.Instance.Pulse -= Instance_Pulse;
                    this.disposed = true;
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", LayTheDraw.strModule));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
