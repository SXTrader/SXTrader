using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using net.sxtrader.bftradingstrategies.betfairif.mockups;
using System.Threading;
using net.sxtrader.bftradingstrategies.betfairif;
using BetFairIF.com.betfair.api.exchange;
using System.Diagnostics;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.LayThe4
{    
    class BFLT4Strategy :IBFTSCommon
    {
        private BetCollection m_betLay = new BetCollection();
        private BetCollection m_betBack = new BetCollection();
        private Bet m_openBack;
        private LT4ConfigurationRW m_config;
        private String m_match;
        private Thread m_betWatcherThread;
        private Thread m_emergencyExitThread;
        private Thread m_betWatchdogThread;
        private Thread m_activeTradingThread;
        private bool m_layChecked = false;
 
        private bool m_activeFirstRun = true;
        private bool m_emergencyRun = false;
        private bool m_watchActivated = false;
        private bool m_active;
        private String m_team_a;
        private String m_team_b;
        private int m_sumGoals;
        private int m_scoreA = 0;
        private int m_scoreB = 0;
        private int m_playtime = 0;
        private IScore m_score;
        private IScore m_score2;
        private float m_commission;
        private bool m_ended = false;

        private const double LOWTOLERANZ = -0.1;
        private const double HIGHTOLERANZ = 0.1;
        private const double STARTAMOUNT = 4.0;

        #region events
        public event EventHandler<BFGameEndedEventArgs> GameEndedEvent;
        public event EventHandler<BFPlaytimeEventArgs> PlaytimeEvent;
        public event EventHandler<BFGoalSumChangedEventArgs> GoalSumChangedEvent;
        public event EventHandler<BFWinLooseChangedEventArgs> WinLooseChangedEvent;
        public event EventHandler<BFMessageEventArgs> MessageEvent;
        public event EventHandler<BFSetCloseTradeTimer> SetCloseTradeTimer;
        public event EventHandler<BFSetOpenBetTimer> SetOpenBetTimer;
        public event EventHandler<BFSetStopLossTimer> SetStopLossTimer;
        public event EventHandler<BFStopCloseTradeTimer> StopCloseTradeTimer;
        public event EventHandler<BFStopOpenBetTimer> StopOpenBetTimer;
        public event EventHandler<BFStopStopLossTimer> StopStopLossTimer;
        #endregion

        public BFLT4Strategy(String match)
        {
            m_config = new LT4ConfigurationRW();
            m_match = match;

            splitMarketName(match);

            m_betWatcherThread = new Thread(betWatcher);
            m_betWatcherThread.IsBackground = true;

            m_emergencyExitThread = new Thread(emergencyExit);
            m_emergencyExitThread.IsBackground = true;

            m_betWatchdogThread = new Thread(watchdog);
            m_betWatchdogThread.IsBackground = true;

            m_activeTradingThread = new Thread(activeTrading);
            m_activeTradingThread.IsBackground = true;
        }

        public BFLT4Strategy(Bet lay, String match)
        {
            if (lay.betType == BetFairIF.com.betfair.api.exchange.BetTypeEnum.B)
                throw new Exception();

            m_betLay.Bets.Add(lay.betId, lay);

            m_config = new LT4ConfigurationRW();
            m_match = match;

            splitMarketName(match);

            m_betWatcherThread = new Thread(betWatcher);
            m_betWatcherThread.IsBackground = true;

            m_emergencyExitThread = new Thread(emergencyExit);
            m_emergencyExitThread.IsBackground = true;

            m_betWatchdogThread = new Thread(watchdog);
            m_betWatchdogThread.IsBackground = true;

            m_activeTradingThread = new Thread(activeTrading);
            m_activeTradingThread.IsBackground = true;
        }

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public LT4ConfigurationRW Configuration
        {
            get
            {
                return m_config;
            }
            set
            {
                m_config = value;
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

        public int SumGoals
        {
            get
            {
                return m_sumGoals;
            }
        }

        public BetCollection Back
        {
            get
            {
                return m_betBack;
            }
        }

        public BetCollection Lay
        {
            get
            {
                return m_betLay;
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
                m_scoreA = m_score.ScoreA;
                m_scoreB = m_score.ScoreB;
                DebugWriter.Instance.WriteMessage("BFUEStrategy::Score", String.Format("{0}: Livescore 1 linked to match {1}. Score is {2}", this.Match, m_score.getLiveMatch(), m_score.getScore()));
            }
        }

        public IScore Score2
        {
            get
            {
                return m_score2;
            }
            set
            {
                m_score2 = value;
                m_scoreA = m_score2.ScoreA;
                m_scoreB = m_score2.ScoreB;
                DebugWriter.Instance.WriteMessage("BFUEStrategy::Score2", String.Format("{0}: Livescore 2 linked to match {1}. Score is {2}", this.Match, m_score2.getLiveMatch(), m_score2.getScore()));
            }
        }

        public SETSTATE State
        {
            get
            {
                double riskBack = this.Back.BetSize;
                double winBack = this.Back.RiskWin;
                //winBack = winBack - (winBack * 0.05);
                double laybackwin = winBack - this.Lay.RiskWin;
                double backlaywin = this.Lay.BetSize - this.Back.BetSize;
                //backlaywin = backlaywin - (backlaywin * 0.05);
                double diff = laybackwin - backlaywin;

                SETSTATE state = SETSTATE.UNSETTELED;
                if (LOWTOLERANZ < diff && diff < HIGHTOLERANZ)
                    state = SETSTATE.SETTLED;
                else
                {
                    if (laybackwin > backlaywin)
                        state = SETSTATE.SETTLED;
                    else
                    {
                        state = SETSTATE.UNSETTELED;
                        /*                        if (m_openBack != null)
                                                    state = SETSTATE.SETTLING;
                                                else
                                                    state = SETSTATE.UNSETTELED;
                         */
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

        public Boolean Active
        {
            get
            {
                return m_active;
            }
            set
            {
                m_active = value;
                /*
                if (m_active)
                    doActivation();
                 */
            }
        }

        public void Start()
        {                        
            if (!m_config.ActivePassive)
            {
                m_betWatcherThread = new Thread(betWatcher);
                m_betWatcherThread.IsBackground = true;
                m_betWatcherThread.Start();
            }             
        }
        #region Eventhandler

        public void PlaytimeTick (object sender, PlaytimeTickEventArgs e)
        {
            if (m_playtime >= e.Tick)
                return;
            m_playtime = e.Tick;

            EventHandler<BFPlaytimeEventArgs> handler = PlaytimeEvent;
            if (handler != null)
            {
                handler(this, new BFPlaytimeEventArgs(this.m_match, e.Tick));
            }


            if (m_config.ActivePassive)
            {
                if(SXThreadStateChecker.isStartedBackground(m_activeTradingThread))
                {
                    ;
                }
                else
                    if (m_activeFirstRun)
                    {
                        m_activeFirstRun = false;
                        activeTradingStart();
                    }
            }

            if (m_watchActivated)
                return;
            if (!m_layChecked)
            {
                if (this.Lay.Bets.Count == 0)
                    return;
                Bet bet = this.Lay.Bets.Values[0];
                if (bet != null)
                {
                    this.Lay.Bets[bet.betId] = BetfairKom.Instance.getBetDetail(bet.betId);
                    EventHandler<BFWinLooseChangedEventArgs> winloose = WinLooseChangedEvent;
                    if (winloose != null)
                        winloose(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));
                    m_layChecked = true;
                }
            }
            if(e.Tick >= m_config.StartPlaytime)
            {
                m_watchActivated = true;
            }
        }

        public void GoalBackEventHandler(object sender, GoalBackEventArgs e)
        {
            if (m_scoreA == e.ScoreA && m_scoreB == e.ScoreB)
                return;

            m_scoreA = e.ScoreA;
            m_scoreB = e.ScoreB;

            m_sumGoals = e.ScoreA + e.ScoreB;

            EventHandler<BFGoalSumChangedEventArgs> handler = GoalSumChangedEvent;
            if (handler != null)
                handler(this, new BFGoalSumChangedEventArgs(this.m_match, m_sumGoals));

            if (m_sumGoals < m_config.Goals)
            {
                m_emergencyRun = false;
                m_emergencyExitThread.Abort();
                m_betWatcherThread = new Thread(betWatcher);
                m_betWatcherThread.IsBackground = true;
                m_betWatcherThread.Start();
            }
        }

        public void GoalEventHandler(object sender, GoalEventArgs e)
        {
            if (m_scoreA == e.ScoreA && m_scoreB == e.ScoreB)
                return;

            m_scoreA = e.ScoreA;
            m_scoreB = e.ScoreB;

            m_sumGoals = e.ScoreA + e.ScoreB;
         
            EventHandler<BFGoalSumChangedEventArgs> handler = GoalSumChangedEvent;
            if (handler != null)
                handler(this, new BFGoalSumChangedEventArgs(this.m_match, m_sumGoals));

            // Falls Notabschluss schon läuft => raus
            if (m_emergencyExitThread.ThreadState == (System.Threading.ThreadState.Background | System.Threading.ThreadState.Running)
                || m_emergencyExitThread.ThreadState == (System.Threading.ThreadState.Background | System.Threading.ThreadState.WaitSleepJoin))
                return;

            // Nur falls Passives Trading
            if (!m_config.ActivePassive)
            {
                // Falls watchdog schon läuft => raus
                if (m_betWatchdogThread.ThreadState == (System.Threading.ThreadState.Background | System.Threading.ThreadState.Running)
                    || m_betWatchdogThread.ThreadState == (System.Threading.ThreadState.Running | System.Threading.ThreadState.WaitSleepJoin))
                    return;
            }

            // Wenn Anzahl Tore über Austiegskriterium => Starten
            if (m_sumGoals >= m_config.Goals)
            {
                m_emergencyRun = true;
                m_betWatcherThread.Abort();
                m_emergencyExitThread = new Thread(emergencyExit);
                m_emergencyExitThread.IsBackground = true;
                m_emergencyExitThread.Start();
            }
            else
            {
                if (State != SETSTATE.SETTLED)
                {
                    m_activeFirstRun = true;
                }
                /*
                if (m_config.ActivePassive)
                {
                    activeTradingStart();
                }
                 * */
            }

        }

        public void GameEndedEventHandler(object sender, GameEndedEventArgs e)
        {
            if (m_ended == true)
                return;

            m_ended = true;

            this.Back.Reload();
            this.Lay.Reload();
            // Gewinn/Verlust errechnen
            double dGuV =0.0;
            if (m_sumGoals >= 4)
            {
                dGuV = -this.Lay.RiskWin + this.Back.RiskWin;
            }
            else
            {
                dGuV = this.Lay.BetSize - this.Back.BetSize;
            }

            if (dGuV > 0)
            {
                dGuV = dGuV - (dGuV * m_commission);
            }

            dGuV = Math.Round(dGuV, 2);

            EventHandler<BFGameEndedEventArgs> handler = GameEndedEvent;
            if (handler != null)
            {
                handler(this, new BFGameEndedEventArgs(this.m_match, DateTime.Now, dGuV));
            }

        }
        #endregion


        private void splitMarketName(String market)
        {
            String[] sSeps = { " - ", " v " };

            String[] teams = market.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);
            /*if (teams.Length == 1)
                teams = market.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);
             */
            m_team_a = teams[0].Trim();
            m_team_b = teams[teams.GetLength(0) - 1].Trim();
        }
        private void activeTradingStart()
        {
            double m_odds = this.Lay.BetPrice;
            this.Back.Reload();
            this.Lay.Reload();
            if (m_openBack != null)
            {
                while (true)
                {
                    Bet bet = BetfairKom.Instance.getBetDetail(m_openBack.betId);
                    if (bet == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    this.Back.Bets[m_openBack.betId] = bet;
                    m_openBack = null;
                    break;
                }
            }

            if (this.State == SETSTATE.SETTLED)
                return;

            if (m_emergencyRun)
                return;

            while (true)
            {
                m_odds += 0.01;
                //Absoluten erwartenden Profit berechnen
                double profitExpected = (m_betLay.BetSize * (m_config.ProfitPercent * 0.01)) / (m_sumGoals + 1);
                //Backeinsatz berechnen
                //double dMoney = calculateBackBet(true, true, m_odds, BankrollManager.Instance.MinStake, 0.0);
                double dMoney = calculateBackBet(true, true, m_odds, 0.01, 0.0);
                dMoney = Math.Round(dMoney, 2);
                double layWin = this.Lay.BetSize - (this.Back.BetSize + dMoney);
                double backWin = -this.Lay.RiskWin + (this.Back.RiskWin + (dMoney * m_odds - dMoney));

                //Wenn Gewinn kleiner als erwarteten Profit, dann weiter
                if (layWin < profitExpected || backWin < profitExpected)
                {                    
                    continue;
                }
                m_odds = Math.Round(m_odds, 2);
                Bet bet = null;
                if (dMoney < BankrollManager.Instance.MinStake)
                {
                    bet = BetfairKom.Instance.placeBackBetBelowMin(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, m_odds, dMoney);
                }
                else
                {
                    bet = BetfairKom.Instance.placeBackBet(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, m_odds, dMoney);
                }
                if (bet == null)
                {
                    m_activeFirstRun = false;
                    Thread.Sleep(10000);
                    return;
                }

                if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                {                    
                    this.Back.Bets.Add(bet.betId, bet);
                    EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                    if (handler != null)
                        handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));
                    EventHandler<BFMessageEventArgs> message = MessageEvent;
                    if (message != null)
                        message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(LayThe4.strPlaceBet, m_match, m_odds, dMoney), LayThe4.strModule));
                    return;
                }
                else
                {
                    m_openBack = bet;
                    this.Back.Bets.Add(bet.betId, bet);
                    m_activeTradingThread = new Thread(activeTrading);
                    m_activeTradingThread.IsBackground = true;
                    m_activeTradingThread.Start();
                    return;
                }
            }
        }

        private void activeTrading()
        {
            while (true)
            {
                try
                {
                    TimeSpan span = new TimeSpan(0, 0, m_config.ProfitWait);
                    EventHandler<BFSetOpenBetTimer> openBetHandler = SetOpenBetTimer;
                    if (openBetHandler != null)
                    {
                        openBetHandler(this, new BFSetOpenBetTimer(this.Match, span));
                    }

                    Thread.Sleep(span);

                    try
                    {
                        this.Back.Bets[m_openBack.betId] = BetfairKom.Instance.getBetDetail(m_openBack.betId);
                    }
                    catch (Exception exc)
                    {
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs("BFLT4Strategy::activeTrading", "An Exception has occured. Please check LogFiles for Details"));
                        }

                        ExceptionWriter.Instance.WriteException(exc);
                        continue;
                    }

                                        // Falls Wette abgeschlossen => alles i.O
                    if (this.Back.Bets[m_openBack.betId].betStatus == BetStatusEnum.M)
                    {
                        EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                        if (handler != null)
                            handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));

                        EventHandler<BFStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                        if (stopHandler != null)
                            stopHandler(this, new BFStopOpenBetTimer(this.Match));

                        m_openBack = null;
                        return;
                    }
                    else if (this.Back.Bets[m_openBack.betId].betStatus == BetStatusEnum.L)
                    {
                        m_activeFirstRun = true;

                        EventHandler<BFStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                        if (stopHandler != null)
                            stopHandler(this, new BFStopOpenBetTimer(this.Match));

                        m_openBack = null;
                        return;
                    }
                    else if (this.Back.Bets[m_openBack.betId].betStatus == BetStatusEnum.C)
                    {
                        m_activeFirstRun = true;
                        EventHandler<BFStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                        if (stopHandler != null)
                            stopHandler(this, new BFStopOpenBetTimer(this.Match));

                        m_openBack = null;
                        return;
                    }

                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFLT4Strategy::activeTrading", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                    Console.WriteLine(exc.Message);
                }
            }
        }

        private void watchdog()
        {           
            while (true)
            {
                try
                {
                    TimeSpan span = new TimeSpan(0, 0, m_config.ProfitWait);
                    EventHandler<BFSetOpenBetTimer> openBetHandler = SetOpenBetTimer;
                    if (openBetHandler != null)
                    {
                        openBetHandler(this, new BFSetOpenBetTimer(this.Match, span));
                    }
                    Thread.Sleep(m_config.ProfitWait * 1000);

                    try
                    {
                        this.Back.Bets[m_openBack.betId] = BetfairKom.Instance.getBetDetail(m_openBack.betId);
                    }
                    catch (Exception exc)
                    {
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs("BFLT4Strategy::watchdog", "An Exception has occured. Please check LogFiles for Details"));
                        }

                        ExceptionWriter.Instance.WriteException(exc);
                        continue;
                    }

                    // Falls Wette abgeschlossen => alles i.O
                    if (this.Back.Bets[m_openBack.betId].betStatus == BetStatusEnum.M)
                    {
                        EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                        if (handler != null)
                            handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));

                        EventHandler<BFStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                        if (stopHandler != null)
                            stopHandler(this, new BFStopOpenBetTimer(this.Match));
                        m_openBack = null;
                        return;
                    }

                    // Falls offener Betrag < MinStake => alles i.O.
                    /*
                    if (this.Back.Bets[m_openBack.betId].remainingSize <= BankrollManager.Instance.MinStake)
                    {
                        continue;
                    }
                    */

                    // Ansonsten storniere Wette
                    if (BetfairKom.Instance.cancelBet(m_openBack.betId) != true)
                    {
                        continue;
                    }

                    

                    Bet canceledBet = BetfairKom.Instance.getBetDetail(m_openBack.betId);
                    if (canceledBet.betStatus == BetStatusEnum.C)
                        this.Back.Bets.Remove(canceledBet.betId);
                    else
                        this.Back.Bets[canceledBet.betId] = canceledBet;

                    m_openBack = null;

                    MarketLite marketLite = BetfairKom.Instance.getMarketInfo(m_betLay.MarketId);

                    if (marketLite == null)
                    {
                        continue;
                    }

                    // Falls Markt pausiert => raus
                    if (marketLite.marketStatus == MarketStatusEnum.SUSPENDED)
                    {
                        Debug.Write(String.Format("Market {0} is suspended. Idle {1} Secs", m_match, m_config.ProfitWait));
                        continue;
                    }

                    // Hole Preisübersicht

                    MarketPrices marketPrices = BetfairKom.Instance.getMarketPrices(m_betLay.MarketId);
                    if (marketPrices == null)
                        continue;
                    // Falls Markt pausiert => raus
                    if (marketPrices.marketStatus == MarketStatusEnum.SUSPENDED)
                    {
                        Debug.Write(String.Format("Market {0} is suspended. Idle {1} Secs", m_match, m_config.ProfitWait));
                        continue;
                    }

                    RunnerPrices runnerPrice = marketPrices.runnerPrices[3];
                    if (runnerPrice.bestPricesToBack.Length > 0)
                    {
                        Price priceBack = runnerPrice.bestPricesToBack[0];
                        double dMoney = calculateBackBet(true, true, priceBack.price, this.Lay.BetSize, 0.0);
                        dMoney = Math.Round(dMoney, 2);
                        m_commission = marketPrices.marketBaseRate * 0.01f;
                        Debug.WriteLine(String.Format("Commission-Rate for Market Total Goals is {1}", m_commission));
                        // Abschluss  
                        Bet bet = null;
                        if (dMoney < BankrollManager.Instance.MinStake)
                        {
                            bet = BetfairKom.Instance.placeBackBetBelowMin(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                        }
                        else
                        {
                            bet = BetfairKom.Instance.placeBackBet(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                        }
                        if (bet == null)
                        {
                            continue;
                        }
                        if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                        {
                            this.Back.Bets.Add(bet.betId, bet);
                            EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                            if (handler != null)
                                handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));
                            EventHandler<BFMessageEventArgs> message = MessageEvent;
                            if (message != null)
                                message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(LayThe4.strPlaceBet, m_match, priceBack.price, dMoney), LayThe4.strModule));
                            EventHandler<BFStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                            if (stopHandler != null)
                                stopHandler(this, new BFStopOpenBetTimer(this.Match));
                            return;
                        }
                        else
                        {
                            m_openBack = bet;
                            continue;
                        }
                    }


                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFLT4Strategy::watchdog", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private void emergencyExit()
        {
            TimeSpan span = new TimeSpan(0, 0, m_config.ExitSeconds);

            while (true)
            {
                EventHandler<BFSetStopLossTimer> openBetHandler = SetStopLossTimer;
                if (openBetHandler != null)
                {
                    openBetHandler(this, new BFSetStopLossTimer(this.Match, span));
                }
                Thread.Sleep(span);

                if (m_betBack.Bets.Count > 0)
                {
                    EventHandler<BFStopStopLossTimer> stopHandler = StopStopLossTimer;
                    if (stopHandler != null)
                        stopHandler(this, new BFStopStopLossTimer(this.Match));
                    return;
                }
                this.Back.Reload();
                if (this.State == SETSTATE.SETTLED || this.State == SETSTATE.SETTLING)
                {
                    EventHandler<BFStopStopLossTimer> stopHandler = StopStopLossTimer;
                    if (stopHandler != null)
                        stopHandler(this, new BFStopStopLossTimer(this.Match));
                    return;
                }

                MarketLite marketLite = BetfairKom.Instance.getMarketInfo(m_betLay.MarketId);

                if (marketLite == null)
                {
                    continue;
                }

                // Falls Markt pausiert => raus
                if (marketLite.marketStatus == MarketStatusEnum.SUSPENDED)
                {                    
                    Debug.WriteLine(String.Format("Match {0} is suspended. Idle {1} Secs", m_match, m_config.ExitSeconds));
                    continue;
                }

                try
                {
                    MarketPrices marketPrices = BetfairKom.Instance.getMarketPrices(m_betLay.MarketId);
                    if (marketPrices == null)
                        continue;
                    // Falls Markt pausiert => raus
                    if (marketPrices.marketStatus == MarketStatusEnum.SUSPENDED)
                    {                        
                        Debug.WriteLine(String.Format("Match {0} is suspended. Idle {1} Secs", m_match, m_config.ExitSeconds));
                        continue;
                    }

                    RunnerPrices runnerPrice = marketPrices.runnerPrices[3];
                    if (runnerPrice.bestPricesToBack.Length > 0)
                    {
                        Price priceBack = runnerPrice.bestPricesToBack[0];
                        //double dMoney = calculateBackBet(true, true, priceBack.price, BankrollManager.Instance.MinStake, 0.0);
                        double dMoney = calculateBackBet(true, true, priceBack.price, 0.01, 0.0);
                        dMoney = Math.Round(dMoney, 2);
                        Bet bet = null;
                        m_commission = marketPrices.marketBaseRate * 0.01f;
                        Debug.WriteLine(String.Format("Commission-Rate for Market Total Goals is {1}", m_commission));

                        if (dMoney < BankrollManager.Instance.MinStake)
                        {
                            bet = BetfairKom.Instance.placeBackBetBelowMin(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                        }
                        else
                        {
                            bet = BetfairKom.Instance.placeBackBet(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                        }
                        if (bet == null)
                            continue;
                        if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                        {
                            //TODO: Event Trade closed
                            this.Back.Bets.Add(bet.betId, bet);
                            EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                            if(handler != null)
                                handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));

                            EventHandler<BFMessageEventArgs> message = MessageEvent;
                            if (message != null)
                                message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(LayThe4.strPlaceBet, m_match, priceBack.price, dMoney), LayThe4.strModule));

                            EventHandler<BFStopStopLossTimer> stopHandler = StopStopLossTimer;
                            if (stopHandler != null)
                                stopHandler(this, new BFStopStopLossTimer(this.Match));

                            return;
                        }
                        else
                        {
                            EventHandler<BFStopStopLossTimer> stopHandler = StopStopLossTimer;
                            if (stopHandler != null)
                                stopHandler(this, new BFStopStopLossTimer(this.Match));
                            m_openBack = bet;

                            m_betWatchdogThread = new Thread(watchdog);
                            m_betWatchdogThread.IsBackground = true;
                            m_betWatchdogThread.Start();

                            return;
                        }
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFLT4Strategy::emergencyExit", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }



        private void betWatcher()
        {
            TimeSpan span = new TimeSpan(0, 0, 1);

            while (true)
            {
                EventHandler<BFSetCloseTradeTimer> openBetHandler = SetCloseTradeTimer;
                if (openBetHandler != null)
                {
                    openBetHandler(this, new BFSetCloseTradeTimer(this.Match, span));
                }
                Thread.Sleep(span);
                if (!m_watchActivated)
                {
                    span = new TimeSpan(0, 0, 30);
                    continue;
                }

                if (!m_active)
                {
                    span = new TimeSpan(0, 0, 30);
                    continue;
                }

                if (this.State == SETSTATE.SETTLED || this.State == SETSTATE.SETTLING)
                {
                    EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                    if (stopHandler != null)
                        stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                    return;
                }

                MarketLite marketLite = BetfairKom.Instance.getMarketInfo(m_betLay.MarketId);

                if (marketLite == null)
                {
                    span = new TimeSpan(0, 0, 30);
                    continue;
                }

                // Falls Markt pausiert => raus
                if (marketLite.marketStatus == MarketStatusEnum.SUSPENDED)
                {
                    span = new TimeSpan(0, 0, m_config.NoProfitWait);
                    Debug.WriteLine(String.Format("Match {0} is suspended. Idle {1} Seconds", m_match, m_config.NoProfitWait));
                    continue;
                }

                // Hole Preisübersicht

                try
                {
                    MarketPrices marketPrices = BetfairKom.Instance.getMarketPrices(m_betLay.MarketId);
                    if (marketPrices == null)
                    {
                        span = new TimeSpan(0, 0, 30);
                        continue;
                    }
                    // Falls Markt pausiert => raus
                    if (marketPrices.marketStatus == MarketStatusEnum.SUSPENDED)
                    {
                        span = new TimeSpan(0, 0, m_config.NoProfitWait);
                        Debug.WriteLine(String.Format("Market {0} is suspended. Idle {1} Seconds", m_match, m_config.NoProfitWait));
                        continue;
                    }


                    RunnerPrices runnerPrice = marketPrices.runnerPrices[3];
                    if (runnerPrice.bestPricesToBack.Length > 0)
                    {
                        Price priceBack = runnerPrice.bestPricesToBack[0];
                        // Verlust
                        if (priceBack.price < m_betLay.BetPrice)
                        {
                            //Falls kein Abschluss => weiter
                            if (m_config.NoProfit)
                            {
                                span = new TimeSpan(0, 0, m_config.NoProfitWait);
                                Debug.WriteLine(String.Format("Market {0}: No profit. Idle {1} Seconds", m_match, m_config.NoProfitWait));
                                continue;
                            }
                            else
                            {
                                m_commission = marketPrices.marketBaseRate * 0.01f;
                                Debug.WriteLine(String.Format("Commission-Rate for Market Total Goals is {1}",m_commission));
                                //Backeinsatz berechnen
                                double dMoney = calculateBackBet(true, true, priceBack.price, this.Lay.BetSize, 0.0);
                                dMoney = Math.Round(dMoney, 2);
                                // Abschluss
                                Bet bet = null;
                                if (dMoney < BankrollManager.Instance.MinStake)
                                {
                                    bet = BetfairKom.Instance.placeBackBetBelowMin(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                                }
                                else
                                {
                                    bet = BetfairKom.Instance.placeBackBet(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                                }
                                if (bet == null)
                                {
                                    span = new TimeSpan(0, 0, 30);
                                    continue;
                                }
                                if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                                {
                                    //TODO: Event Trade closed
                                    this.Back.Bets.Add(bet.betId, bet);
                                    EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                                    if (handler != null)
                                        handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));
                                    EventHandler<BFMessageEventArgs> message = MessageEvent;
                                    if (message != null)
                                        message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(LayThe4.strPlaceBet, m_match, priceBack.price, dMoney), LayThe4.strModule));
                                    EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                    if (stopHandler != null)
                                        stopHandler(this, new BFStopCloseTradeTimer(this.Match));

                                    return;
                                }
                                else
                                {
                                    EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                    if (stopHandler != null)
                                        stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                                    m_openBack = bet;

                                    m_betWatchdogThread = new Thread(watchdog);
                                    m_betWatchdogThread.IsBackground = true;
                                    m_betWatchdogThread.Start();

                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (m_config.Profit)
                            {
                                //Absoluten erwartenden Profit berechnen
                                double profitExpected = (m_betLay.BetSize * (m_config.ProfitPercent * 0.01))/(m_sumGoals+1);
                                //Backeinsatz berechnen
                                double dMoney = calculateBackBet(true, true, priceBack.price, BankrollManager.Instance.MinStake, 0.0);
                                dMoney = Math.Round(dMoney, 2);
                                double layWin = this.Lay.BetSize - (this.Back.BetSize + dMoney);
                                double backWin = -this.Lay.RiskWin + (this.Back.RiskWin + (dMoney * priceBack.price - dMoney));

                                //Wenn Gewinn kleiner als erwarteten Profit, dann weiter
                                if (layWin < profitExpected || backWin < profitExpected)
                                {
                                    span = new TimeSpan(0, 0, m_config.ProfitWait);
                                    Debug.WriteLine(String.Format("Market {0}: Profit too low. Idle {1} Seconds", m_match, m_config.ProfitWait));
                                    continue;
                                }
                                //Sonst Abschluss
                                m_commission = marketPrices.marketBaseRate * 0.01f;
                                Debug.WriteLine(String.Format("Commission-Rate for Market Total Goals is {1}", m_commission));
                                Bet bet = null;
                                if (dMoney < BankrollManager.Instance.MinStake)
                                {
                                    bet = BetfairKom.Instance.placeBackBetBelowMin(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                                }
                                else
                                {
                                    bet = BetfairKom.Instance.placeBackBet(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                                }
                                if (bet == null)
                                {
                                    span = new TimeSpan(0, 0, 30);
                                    continue;
                                }
                                if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                                {
                                    //TODO: Event Trade closed
                                    this.Back.Bets.Add(bet.betId, bet);
                                    EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                                    if (handler != null)
                                        handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));
                                    EventHandler<BFMessageEventArgs> message = MessageEvent;
                                    if (message != null)
                                        message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(LayThe4.strPlaceBet, m_match, priceBack.price, dMoney), LayThe4.strModule));
                                    EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                    if (stopHandler != null)
                                        stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                                    return;
                                }
                                else
                                {
                                    EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                    if (stopHandler != null)
                                        stopHandler(this, new BFStopCloseTradeTimer(this.Match));

                                    m_openBack = bet;

                                    m_betWatchdogThread = new Thread(watchdog);
                                    m_betWatchdogThread.IsBackground = true;
                                    m_betWatchdogThread.Start();

                                    return;
                                }
                            }
                            else
                            {
                                //Backeinsatz berechnen
                                double dMoney = calculateBackBet(true, true, priceBack.price, BankrollManager.Instance.MinStake, 0.0);
                                dMoney = Math.Round(dMoney, 2);
                                m_commission = marketPrices.marketBaseRate * 0.01f;
                                Debug.WriteLine(String.Format("Commission-Rate for Market Total Goals is {1}", m_commission));
                                //Abschluss
                                Bet bet = null;
                                if (dMoney < BankrollManager.Instance.MinStake)
                                {
                                    bet = BetfairKom.Instance.placeBackBetBelowMin(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                                }
                                else
                                {
                                    bet = BetfairKom.Instance.placeBackBet(this.Lay.MarketId, this.Lay.SelectionId, this.Lay.AsianId, priceBack.price, dMoney);
                                }
                                if (bet == null)
                                {
                                    span = new TimeSpan(0, 0, 30);
                                    continue;
                                }
                                if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                                {
                                    //TODO: Event Trade closed
                                    this.Back.Bets.Add(bet.betId, bet);
                                    EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                                    if (handler != null)
                                        handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));
                                    EventHandler<BFMessageEventArgs> message = MessageEvent;
                                    if (message != null)
                                        message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(LayThe4.strPlaceBet, m_match, priceBack.price, dMoney), LayThe4.strModule));
                                    EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                    if (stopHandler != null)
                                        stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                                    return;
                                }
                                else
                                {
                                    EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                    if (stopHandler != null)
                                        stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                                    m_openBack = bet;

                                    m_betWatchdogThread = new Thread(watchdog);
                                    m_betWatchdogThread.IsBackground = true;
                                    m_betWatchdogThread.Start();

                                    return;
                                }
                            }
                        }
                        //    Price priceLay = runnerPrice.bestPricesToLay[0];
                    }
                    else
                    {
                        span = new TimeSpan(0, m_config.NoProfitWait, 0);
                        Debug.WriteLine(String.Format("Market {0}: No Back odds. Idle {1} Mins", m_match, m_config.NoProfitWait));
                        continue;
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFLT4Strategy::betWatcher", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
            
        }
        private double calculateBackBet(bool firstRun, bool allNeg, double backOdds,
                                             double backAmount, double stepAmount)
        {
            double riskBack = this.Back.BetSize + backAmount;
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


        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion
    }
}
