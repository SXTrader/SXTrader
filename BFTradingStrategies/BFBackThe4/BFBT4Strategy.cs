using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
////////////////////////////////////////////////////////////
using net.sxtrader.bftradingstrategies.betfairif.mockups;
///////////////////////////////////////////////////////////
using net.sxtrader.bftradingstrategies.betfairif;
using net.sxtrader.bftradingstrategies.BackThe4;
using System.Threading;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using BetFairIF.com.betfair.api.exchange;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Diagnostics;


namespace net.sxtrader.bftradingstrategies.BackThe4
{
    public class BFBT4Strategy : IBFTSCommon
    {
        //TODO: Ersetzen durch reale Objekte
        private BetCollection m_betLay = new BetCollection();
        private BetCollection m_betBack = new BetCollection();
        private BT4ConfigurationRW m_config;
        private Thread m_betWatcherThread;
        private Thread m_stopLossWatchdog;
        private Thread m_betWatchdogThread;
        private String m_match;
        private String m_team_a;
        private String m_team_b;
        private int m_sumGoals;
        private IScore m_score;
        private IScore m_score2;


        /*
        private bool m_watchActivated = false;
        private bool m_stopLossActivated = false;
        private bool m_stopLossRun = false;
         */
        private bool m_active = false;
        private bool m_started = false;

        private bool m_stopLossSingleRun = false;

        private Bet m_openLay = null;

        private int m_scoreA = 0;
        private int m_scoreB = 0;
        private int m_playtime = 0;
        private float m_commission;
        private bool m_ended = false;


        private const double LOWTOLERANZ = -0.1;
        private const double HIGHTOLERANZ = 0.1;
        private const double STARTAMOUNT = 4.0;
        private const int MAXGOALS = 4;

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

        public BFBT4Strategy(Bet back, String match)
        {
            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", match), "creating trade object");
            if (back.betType == BetFairIF.com.betfair.api.exchange.BetTypeEnum.L)
                throw new Exception();

            m_betBack.Bets.Add(back.betId, back);

            m_config = new BT4ConfigurationRW();
            m_match = match;

            splitMarketName(match);

            m_betWatcherThread = new Thread(betWatcher);
            m_betWatcherThread.IsBackground = true;

            m_stopLossWatchdog = new Thread(stopLossWatchdog);
            m_stopLossWatchdog.IsBackground = true;

            m_betWatchdogThread = new Thread(watchdog);
            m_betWatchdogThread.IsBackground = true;
        }

        public BFBT4Strategy(String match)
        {
            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", match), "creating trade object");
            m_config = new BT4ConfigurationRW();
            m_match = match;

            splitMarketName(match);

            m_betWatcherThread = new Thread(betWatcher);
            m_betWatcherThread.IsBackground = true;

            m_stopLossWatchdog = new Thread(stopLossWatchdog);
            m_stopLossWatchdog.IsBackground = true;

            m_betWatchdogThread = new Thread(watchdog);
            m_betWatchdogThread.IsBackground = true;
        }

        public void Start()
        {
            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "starting up trade object");
            if (m_score == null)
            {
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "starting up. no livescore! stopping starting up");
            }
            //m_betWatcherThread.Start();
            m_started = true;


            if (!SXThreadStateChecker.isStartedBackground(m_stopLossWatchdog)) 
            {
                m_stopLossWatchdog = new Thread(stopLossWatchdog);
                m_stopLossWatchdog.IsBackground = true;
            }
            if (m_score != null)
            {
                
                m_sumGoals = m_score.ScoreA + m_score.ScoreB;
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("starting up. setting goal sum to {0}", m_sumGoals));

                EventHandler<BFGoalSumChangedEventArgs> handler = GoalSumChangedEvent;
                if (handler != null)
                    handler(this, new BFGoalSumChangedEventArgs(this.m_match, m_sumGoals));
            }

            if (State == SETSTATE.UNSETTELED && SumGoals >= m_config.CloseTradeGoals)
            {
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "starting up. goal sum higher than close trading trigger; starting close trading");

                if(SXThreadStateChecker.isStartedBackground(m_betWatchdogThread))
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "starting up. there's already a close trading running");
                    return;
                }

                // Falls watchdog schon läuft => raus
                if(SXThreadStateChecker.isStartedBackground(m_betWatchdogThread))
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "starting up. there's already a open bet watcher running");
                    return;
                }

                if(!SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
                {
                    m_betWatcherThread = new Thread(betWatcher);
                    m_betWatcherThread.IsBackground = true;
                    m_betWatcherThread.Start();
                }
            }

        }

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public BT4ConfigurationRW Configuration
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

        public Boolean Active
        {
            get
            {
                return m_active;
            }
            set
            {
                m_active = value;

                DebugWriter.Instance.WriteMessage("BFUEStrategy::Active", String.Format("{0}: Activity Flag changed to {1}", this.Match, value.ToString()));

                if (m_active)
                    doActivation();
                else
                    doDeactivation();
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
            set
            {
                m_betBack = value;
            }
        }

        public BetCollection Lay
        {
            get
            {
                return m_betLay;
            }
            set
            {
                m_betLay = value;
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
                DebugWriter.Instance.WriteMessage("BFBT4Strategy::Score", String.Format("{0}: Livescore 1 linked to match {1}. Score is {2}", this.Match, m_score.getLiveMatch(), m_score.getScore()));
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
                DebugWriter.Instance.WriteMessage("BFBT4Strategy::Score2", String.Format("{0}: Livescore 2 linked to match {1}. Score is {2}", this.Match, m_score2.getLiveMatch(), m_score2.getScore()));
            }
        }

        public SETSTATE State
        {
            get
            {
                /* Schritt 1 Lay-Berechnen */
                double lay = this.Lay.BetSize * this.Lay.BetPrice - this.Lay.BetSize;
                /* Schritt 2: Differenz aus Lay- und Backeinsatz berechnen */
                double diffLayBack = this.Lay.BetSize - this.Back.BetSize;
                /* Schritt 3: Differenz als Gewinn Back und Verlust Lay berechnen */
                double diffWinLoose = (this.Back.BetSize * this.Back.BetPrice - this.Back.BetSize) - lay;
                /* Schritt 4: Totale Differenz berechnen */
                double diffTotal = diffWinLoose - diffLayBack;
                
                SETSTATE state = SETSTATE.UNSETTELED;
                if (LOWTOLERANZ < diffTotal && diffTotal < HIGHTOLERANZ)
                    state = SETSTATE.SETTLED;
                else
                {

                    if (diffLayBack > diffWinLoose)
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

        private void watchdog()
        {
            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("open bet watcher. wating {0} minutes", m_config.TradeWatchdogMinutes));
            while (true)
            {
                TimeSpan span = new TimeSpan(0, m_config.TradeWatchdogMinutes, 0);
                try
                {
                    EventHandler<BFSetOpenBetTimer> openBetHandler = SetOpenBetTimer;
                    if (openBetHandler != null)
                    {
                        openBetHandler(this, new BFSetOpenBetTimer(this.Match, span));
                    }
                    Thread.Sleep(span);

                    // Falls Nicht gestarted => warten
                    if (!m_active)
                    {
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: Deactivated. Leaving", this.Match));
                        EventHandler<BFStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                        if (stopHandler != null)
                            stopHandler(this, new BFStopOpenBetTimer(this.Match));
                        return;
                    }
                    /*
                    if (!m_started)
                    {
                        DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("open bet watcher. trading not started yet. wating {0} seconds", 30));
                        span = new TimeSpan(0, 0, 30);
                        continue;
                    }
                    */


                    if (m_openLay != null)
                    {
                        try
                        {
                            this.Lay.Bets[m_openLay.betId] = BetfairKom.Instance.getBetDetail(m_openLay.betId);
                        }
                        catch (Exception exc)
                        {
                            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                            if (handler != null)
                            {
                                handler(this, new SXExceptionMessageEventArgs("BFBT4Strategy::watchdog", "An Exception has occured. Please check LogFiles for Details"));
                            }

                            ExceptionWriter.Instance.WriteException(exc);
                            continue;
                        }

                        // Falls Wette abgeschlossen => alles i.O
                        if (this.Lay.Bets[m_openLay.betId].betStatus == BetStatusEnum.M)
                        {
                            EventHandler<BFStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                            if (stopHandler != null)
                                stopHandler(this, new BFStopOpenBetTimer(this.Match));

                            EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                            if (handler != null)
                                handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));

                            DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: Open Bet {1} was matched. Average odds were {2}", this.Match, m_openLay.betId, m_openLay.avgPrice));

                            m_openLay = null;
                            return;
                        }

                        if (this.Lay.Bets[m_openLay.betId].betStatus == BetStatusEnum.MU ||
                           this.Lay.Bets[m_openLay.betId].betStatus == BetStatusEnum.U)
                        {
                            EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                            if (handler != null)
                                handler(this, new BFWinLooseChangedEventArgs(this.m_match, -this.Lay.RiskWin + this.Back.RiskWin, this.Lay.BetSize - this.Back.BetSize));

                            DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: Open Bet {1} was partally matched. Average odds were {2}", this.Match, m_openLay.betId, m_openLay.avgPrice));
                            // stornieren
                        }

                        if (BetfairKom.Instance.cancelBet(m_openLay.betId) != true)
                        {
                            DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: Cancel Bet failed. Retrying", this.Match));
                            continue;
                        }

                        Bet canceledBet = BetfairKom.Instance.getBetDetail(m_openLay.betId);
                        if (canceledBet.betStatus != BetStatusEnum.M && canceledBet.betStatus != BetStatusEnum.MU)
                        {
                            DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: Bet was canceled", this.Match));
                            this.Back.Bets.Remove(canceledBet.betId);
                        }
                        else
                        {
                            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "open bet watcher. bet wasn't canceled. retrying");
                            this.Back.Bets[canceledBet.betId] = canceledBet;
                            continue;
                        }
                    }

                    m_openLay = null;


                    // Erneut Wette absetzen
                    // Marktinfos holen
                    MarketLite marketLite = BetfairKom.Instance.getMarketInfo(m_betBack.MarketId);

                    if (marketLite == null)
                    {
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: No Market Information. Retrying", this.Match));
                        continue;
                    }

                    // Falls Markt pausiert => warte 30 Sekunden
                    if (marketLite.marketStatus == MarketStatusEnum.SUSPENDED)
                    {
                        DebugWriter.Instance.WriteMessage("BFBTStrategy::OpenBetWatcher", String.Format("{0}: Market is supspended. Retrying", this.Match));
                        continue;
                    }                  

                    // Hole Preisübersicht
                    try
                    {
                        MarketPrices marketPrices = BetfairKom.Instance.getMarketPrices(m_betBack.MarketId);
                        if (marketPrices == null)
                        {
                            DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: No Market Prices. Retrying", this.Match));
                            continue;
                        }
                        // Falls Markt pausiert => warte 30 Sekunden
                        if (marketPrices.marketStatus == MarketStatusEnum.SUSPENDED)
                        {
                            DebugWriter.Instance.WriteMessage("BFBTStrategy::OpenBetWatcher", String.Format("{0}: Market is supspended. Retrying", this.Match));
                            continue;
                        }

                        RunnerPrices runnerPrice = marketPrices.runnerPrices[3];

                        if (runnerPrice.bestPricesToLay.Length > 0)
                        {
                            Price priceLay = runnerPrice.bestPricesToLay[0];
                            // Verlust
                            if (priceLay.price > m_betBack.BetPrice)
                            {
                                //Falls kein Abschluss erwünscht => warte 
                                if (!m_config.CloseTradeLoss)
                                {
                                    DebugWriter.Instance.WriteMessage("BFBTStrategy::OpenBetWatcher", String.Format("{0}: No Profit. Retrying", this.Match));                                    
                                    span = new TimeSpan(0, 0, m_config.CloseTradeWaitSeconds);
                                    continue;
                                }
                            }

                            //Absoluten erwartenden Profit berechnen
                            double profitExpected = (m_betBack.BetSize * (m_config.CloseTradeProfit * 0.01)) / (m_sumGoals + 1);
                            //Layeinsatz berechnen 
                            double dTotal = (Back.BetSize * Back.BetPrice - Back.BetSize) - (Lay.BetPrice * Lay.BetSize - Lay.BetSize);
                            double dMoney = calculateLayBet(m_betBack.BetSize, dTotal, priceLay.price, Back.BetSize, Back.BetSize, true);

                            dMoney = Math.Round(dMoney, 2);

                            double layWin = (this.Lay.BetSize + dMoney) - this.Back.BetSize;
                            double backWin = this.Back.RiskWin - (this.Lay.RiskWin + (dMoney * priceLay.price - dMoney));

                            //Falls Profit erwartet wird
                            if (!m_config.CloseTradeLoss)
                            {
                                //Wenn Gewinn kleiner als erwarteten Profit, dann warten 30 Sekunden
                                if (layWin < profitExpected || backWin < profitExpected)
                                {
                                    DebugWriter.Instance.WriteMessage("BFBTStrategy::OpenBetWatcher", String.Format("{0}: Profit is too low. Retrying", this.Match));                                                                        
                                    continue;
                                }
                            }

                            // Falls mittlerweile genug Tore gefallen sind, ist kein Abschluss notwendig
                            if (m_sumGoals >= MAXGOALS)
                            {
                                EventHandler<BFStopOpenBetTimer> stopHandler = StopOpenBetTimer;
                                if (stopHandler != null)
                                    stopHandler(this, new BFStopOpenBetTimer(this.Match));
                                DebugWriter.Instance.WriteMessage("BFBTStrategy::OpenBetWatcher", String.Format("{0}: More or equals 4 Goals. Leaving", this.Match));
                                return;
                            }                            

                            //Abschluss  
                            m_commission = marketPrices.marketBaseRate * 0.01f;
                            Debug.WriteLine(String.Format("Commission-Rate for Market {0} is {1}", this.Match, m_commission));
                            Bet bet = null;
                            if(dMoney < BankrollManager.Instance.MinStake)
                            {
                                DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: Placing bet below MinStake with odds of {1}", this.Match, priceLay.price));
                                bet = BetfairKom.Instance.placeLayBetBelowMin(this.Back.MarketId, this.Back.SelectionId,this.Back.AsianId, priceLay.price, dMoney);
                            }
                            else
                            {
                                DebugWriter.Instance.WriteMessage("BFBT4Strategy::OpenBetWatcher", String.Format("{0}: Placing bet with odds of {1}", this.Match, priceLay.price));
                                bet = BetfairKom.Instance.placeLayBet(this.Back.MarketId, this.Back.SelectionId,this.Back.AsianId, priceLay.price, dMoney);
                            }

                            if (bet == null)
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("open bet watcher. no new bet was placed. waiting  {0} seconds",30));
                                span = new TimeSpan(0, 0, 30);
                                continue;
                            }

                            if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match),String.Format("open bet watcher. new bet was matched at a price of {0}", bet.avgPrice));
                                this.Back.Bets.Add(bet.betId, bet);
                                EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                                if (handler != null)
                                    handler(this, new BFWinLooseChangedEventArgs(this.m_match, Math.Round(-this.Lay.RiskWin + this.Back.RiskWin,2), Math.Round(this.Lay.BetSize - this.Back.BetSize,2)));
                                EventHandler<BFMessageEventArgs> message = MessageEvent;
                                if (message != null)
                                    message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(BackThe4.strPlaceBet, m_match, priceLay.price, dMoney), BackThe4.strModule));
                                return;
                            }
                            else
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("open bet watcher. new bet wasn't matched. wait {0} minutes", m_config.TradeWatchdogMinutes));
                                span = new TimeSpan(0, m_config.TradeWatchdogMinutes, 0);
                                m_openLay = bet;
                                continue;
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs("BFBT4Strategy::watchdog", "An Exception has occured. Please check LogFiles for Details"));
                        }

                        ExceptionWriter.Instance.WriteException(exc);
                        System.Diagnostics.Debug.WriteLine(exc.Message);
                        Console.WriteLine(exc.Message);
                    }

                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFBT4Strategy::watchdog", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                    System.Diagnostics.Debug.WriteLine(exc.Message);
                    Console.WriteLine(exc.Message);
                }
            }
            
        }

        private void betWatcher()
        {
            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("trade closing. wating {0} seconds", m_config.CloseTradeWaitSeconds));
            while (true)
            {
                try
                {
                    TimeSpan span = new TimeSpan(0, 0, m_config.CloseTradeWaitSeconds);
                    EventHandler<BFSetCloseTradeTimer> openBetHandler = SetCloseTradeTimer;
                    if (openBetHandler != null)
                    {
                        openBetHandler(this, new BFSetCloseTradeTimer(this.Match, span));
                    }

                    DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Close-Trade-Watcher waiting {1} seconds", this.Match, span.TotalSeconds));
                    // Benötigte Abschlusszeit abwarten;

                    Thread.Sleep(span);

                    // Falls noch nicht gestarted => warten:
                    if (!m_started)
                    {
                        EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                        if (stopHandler != null)
                            stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Deactivated. Leaving", this.Match));
                        return;
                    }

                    // Falls schon Abgeschlossen => beenden
                    if (this.State == SETSTATE.SETTLED || this.State == SETSTATE.SETTLING)
                    {
                        EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                        if (stopHandler != null)
                            stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Trade is settled. Leaving", this.Match));
                        return;
                    }                   

                    // Marktinfos holen
                    MarketLite marketLite = BetfairKom.Instance.getMarketInfo(Back.MarketId); ;

                    if (marketLite == null)
                    {
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: No market information. Retrying", this.Match));
                        continue;
                    }

                    // Falls Markt pausiert => warte 30 Sekunden
                    if (marketLite.marketStatus == MarketStatusEnum.SUSPENDED)
                    {
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Market is suspended. Retrying", this.Match));
                        continue;
                    }

                    // Hole Preisübersicht
                    try
                    {
                        MarketPrices marketPrices = BetfairKom.Instance.getMarketPrices(Back.MarketId);
                        if (marketPrices == null)
                        {
                            DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: No market prices. Retrying", this.Match));                            
                            continue;
                        }
                        // Falls Markt pausiert => warte 30 Sekunden
                        if (marketPrices.marketStatus == MarketStatusEnum.SUSPENDED)
                        {
                            DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Market is suspended. Retrying", this.Match));                            
                            continue;
                        }

                        RunnerPrices runnerPrice = marketPrices.runnerPrices[3];

                        if (runnerPrice.bestPricesToLay.Length > 0)
                        {
                            Price priceLay = runnerPrice.bestPricesToLay[0];
                            // Verlust
                            if (priceLay.price > Back.BetPrice)
                            {
                                //Falls kein Abschluss erwünscht => warte 
                                if (!m_config.CloseTradeLoss)
                                {
                                    DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: No profit. Retrying", this.Match));
                                    span = new TimeSpan(0, 0, m_config.CloseTradeWaitSeconds);
                                    continue;
                                }
                            }

                            //Absoluten erwartenden Profit berechnen
                            double profitExpected = (Back.BetSize * (m_config.CloseTradeProfit * 0.01)) / (m_sumGoals + 1);
                            //Layeinsatz berechnen 
                            double dTotal = (Back.BetSize * Back.BetPrice - Back.BetSize) - (Lay.BetPrice * Lay.BetSize - Lay.BetSize);
                            double dMoney = calculateLayBet(m_betBack.BetSize, dTotal, priceLay.price, Back.BetSize, Back.BetSize, true);

                            dMoney = Math.Round(dMoney, 2);

                            double layWin = (this.Lay.BetSize + dMoney) - this.Back.BetSize;
                            double backWin = this.Back.RiskWin - (this.Lay.RiskWin+(dMoney * priceLay.price - dMoney));

                            //Falls Profit erwartet wird
                            if (!m_config.CloseTradeLoss)
                            {
                                //Wenn Gewinn kleiner als erwarteten Profit, dann warten 30 Sekunden
                                if (layWin < profitExpected || backWin < profitExpected)
                                {
                                    DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Profit too low. Retrying", this.Match));
                                    continue;
                                }
                            }

                            // letzte Überprüfung, ob mittlerweile nicht schon genut Tore gefallen sind
                            if (m_sumGoals >= MAXGOALS)
                            {
                                DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: More or equals than 4 goals. Leaving", this.Match));
                                EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                    stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                                return;
                            }                            

                            //Abschluss
                            Bet bet = null;

                            m_commission = marketPrices.marketBaseRate * 0.01f;
                            Debug.WriteLine(String.Format("Commission-Rate for Market {0} is {1}", this.Match, m_commission));

                            if (dMoney < BankrollManager.Instance.MinStake)
                            {
                                DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Placing bet below MinStake with odds of {1}", this.Match, priceLay.price));
                                bet = BetfairKom.Instance.placeLayBetBelowMin(this.Back.MarketId, this.Back.SelectionId,this.Back.AsianId, priceLay.price, dMoney);
                            }
                            else
                            {
                                DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Placing bet with odds of {1}", this.Match, priceLay.price));
                                bet = BetfairKom.Instance.placeLayBet(this.Back.MarketId, this.Back.SelectionId,this.Back.AsianId, priceLay.price, dMoney);
                            }

                            if (bet == null)
                            {
                                DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: There was no bet placed. Retrying", this.Match));
                                continue;
                            }
                            else
                            {
                                EventHandler<BFStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                                if (stopHandler != null)
                                    stopHandler(this, new BFStopCloseTradeTimer(this.Match));
                            }

                            

                            if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                            {
                                DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Match {1} is matched. Leaving Close-Trade", this.Match, bet.betId));
                                this.Lay.Bets.Add(bet.betId, bet);
                                EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                                if (handler != null)
                                {
                                    backWin = Back.RiskWin - Lay.RiskWin;
                                    double backLost = Lay.BetSize - Back.BetSize;
                                    handler(this, new BFWinLooseChangedEventArgs(this.m_match, Math.Round(backWin,2), Math.Round(backLost,2)));
                                }
                                EventHandler<BFMessageEventArgs> message = MessageEvent;
                                if (message != null)
                                    message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(BackThe4.strPlaceBet, m_match, priceLay.price, dMoney), BackThe4.strModule));
                                return;
                            }
                            else
                            {
                                DebugWriter.Instance.WriteMessage("BFBT4Strategy::CloseTradeWatcher", String.Format("{0}: Match {1} is unmatched or partally matched. Starting Open-Bet-Watcher", this.Match, bet.betId));
                                //Abschlussüberwachung anstossen
                                m_openLay = bet;

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
                            handler(this, new SXExceptionMessageEventArgs("BFBT4Strategy::betWatcher", "An Exception has occured. Please check LogFiles for Details"));
                        }

                        ExceptionWriter.Instance.WriteException(exc);
                    }
                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BFBT4Strategy::betWatchter", "An Exception has occured. Please check LogFiles for Details"));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private void stopLossWatchdog()
        {
            if (m_stopLossSingleRun)
                return;

            m_stopLossSingleRun = true;
            while (true)
            {
                TimeSpan span = new TimeSpan(0, 0, m_config.CloseTradeWaitSeconds);
                EventHandler<BFSetStopLossTimer> openBetHandler = SetStopLossTimer;
                if (openBetHandler != null)
                {
                    openBetHandler(this, new BFSetStopLossTimer(this.Match, span));
                }

                DebugWriter.Instance.WriteMessage("BFBT4Strategy::StopLossWatcher", String.Format("{0}: Stop/Loss-Watcher waiting {1} seconds", this.Match, span.TotalSeconds));

                Thread.Sleep(span);

                // Falls noch nicht gestartet => warten
                if (!m_started)
                {
                    DebugWriter.Instance.WriteMessage("BFBT4Strategy::StopLossWatcher", String.Format("{0}: Deactivated. Leaving", this.Match));
                    EventHandler<BFStopStopLossTimer> stopHandler = StopStopLossTimer;
                    if (stopHandler != null)
                        stopHandler(this, new BFStopStopLossTimer(this.Match));
                    return;
                }

                if (this.State == SETSTATE.SETTLED)
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match),"stop/loss watcher. trade is settled.");
                    EventHandler<BFStopStopLossTimer> stopHandler = StopStopLossTimer;
                    if (stopHandler != null)
                        stopHandler(this, new BFStopStopLossTimer(this.Match));
                    return;
                }


                 // Marktinfos holen
                    MarketLite marketLite = BetfairKom.Instance.getMarketInfo(Back.MarketId); ;

                    if (marketLite == null)
                    {
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::StopLossWatcher", String.Format("{0}: No market Info. Retrying", this.Match));
                        continue;
                    }

                    // Falls Markt pausiert => warte 30 Sekunden
                    if (marketLite.marketStatus == MarketStatusEnum.SUSPENDED)
                    {
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::StopLossWatcher", String.Format("{0}: Market is suspended. Retrying", this.Match));
                        continue;
                    }

                    // Hole Preisübersicht
                    try
                    {
                        MarketPrices marketPrices = BetfairKom.Instance.getMarketPrices(m_betBack.MarketId);

                        if (marketPrices == null)
                        {
                            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("stop/loss watcher. no market prices. wating {0} seconds", 30));                            
                            continue;
                        }

                        // Falls Markt pausiert => warte 30 Sekunden
                        if (marketPrices.marketStatus == MarketStatusEnum.SUSPENDED)
                        {
                            DebugWriter.Instance.WriteMessage("BFBT4Strategy::StopLossWatcher", String.Format("{0}: Market is suspended. Retrying", this.Match));                            
                            continue;
                        }

                        RunnerPrices runnerPrice = marketPrices.runnerPrices[3];                        

                        //Ausstiegsquote berechnen
                        double emergencyExitOdds = Back.BetPrice * m_config.StopLossInitFactor;

                        if (runnerPrice.bestPricesToLay.Length > 0)
                        {
                            Price priceLay = runnerPrice.bestPricesToLay[0];
                            // Ausstiegsschwelle noch nicht erreicht?
                            if (priceLay.price < emergencyExitOdds)
                            {
                                span = new TimeSpan(0, 0, 30);
                                continue;
                            }

                            // Aktuelle Quote größer als maximal Quote;
                            if (priceLay.price > m_config.StopLossMax * this.Back.BetPrice)
                            {
                                span = new TimeSpan(0, 0, 30);
                                continue;
                            }

                            if(SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
                            {
                                m_betWatcherThread.Abort();
                                m_betWatchdogThread.Join();
                            }

                            //m_stopLossRun = true;

                            //Layeinsatz berechnen 

                            double dTotal = (Back.BetSize * Back.BetPrice - Back.BetSize) - (Lay.BetPrice * Lay.BetSize - Lay.BetSize);
                            double dMoney = calculateLayBet(m_betBack.BetSize, dTotal, priceLay.price, Back.BetSize, Back.BetSize, true);

                            dMoney = Math.Round(dMoney, 2);

                            double layWin = (this.Lay.BetSize + dMoney) - this.Back.BetSize;
                            double backWin = this.Back.RiskWin - (this.Lay.RiskWin + (dMoney * priceLay.price - dMoney));

                            // letzte Überprüfung, ob nicht genug Tore gefallen sind
                            if (m_sumGoals >= MAXGOALS)
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "stop/loss watcher. goal sum is higher or equals 4; no further trade required");
                                return;
                            }
                            if (this.State == SETSTATE.SETTLED)
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "stop/loss watcher. trade is settled.");
                                return;
                            }
                            // Falls noch nicht gestartet => warten
                            if (!m_started)
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("stop/loss watcher. trading not started yet. wating {0} seconds", 30));
                                continue;
                            }
                            //Abschluss
                            Bet bet = null;
                            if (dMoney < BankrollManager.Instance.MinStake)
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("stop/loss watcher. placing bet lower than minimum at price of {0}", priceLay.price));
                                bet = BetfairKom.Instance.placeLayBetBelowMin(this.Back.MarketId, this.Back.SelectionId, this.Back.AsianId, priceLay.price, dMoney);
                            }
                            else
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("stop/loss watcher. placing bet lower than minimum at price of {0}", priceLay.price)); 
                                bet = BetfairKom.Instance.placeLayBet(this.Back.MarketId, this.Back.SelectionId, this.Back.AsianId, priceLay.price, dMoney);
                            }

                            if (bet == null)
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("stop/loss watcher. no new bet was placed. waiting  {0} seconds", 30));
                                span = new TimeSpan(0, 0, 30);
                                continue;
                            }

                            if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
                            {
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("stop/loss watcher. new bet was matched at a price of {0}", bet.avgPrice));
                                this.Lay.Bets.Add(bet.betId, bet);
                                EventHandler<BFWinLooseChangedEventArgs> handler = WinLooseChangedEvent;
                                if (handler != null)
                                {
                                    backWin = Back.RiskWin - Lay.RiskWin;
                                    double backLost = Lay.BetSize - Back.BetSize;
                                    handler(this, new BFWinLooseChangedEventArgs(this.m_match, Math.Round(backWin,2), Math.Round(backLost,2)));
                                }
                                EventHandler<BFMessageEventArgs> message = MessageEvent;
                                if (message != null)
                                    message(this, new BFMessageEventArgs(DateTime.Now, m_match, String.Format(BackThe4.strPlaceBet, m_match, priceLay.price, dMoney), BackThe4.strModule));
                                return;
                            }
                            else
                            {
                                //Abschlussüberwachung anstossen
                                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "stop/loss watcher. new bet wasn't matched. starting open bet watcher");

                                m_openLay = bet;

                                m_betWatchdogThread = new Thread(watchdog);
                                m_betWatchdogThread.IsBackground = true;
                                m_betWatchdogThread.Start();
                                return;
                            }
                        }
                        else
                        {
                            span = new TimeSpan(0, 0, 30);
                        }
                    }
                    catch (Exception exc)
                    {
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs("BFBT4Strategy::stopLossWatchdog", "An Exception has occured. Please check LogFiles for Details"));
                        }

                        ExceptionWriter.Instance.WriteException(exc);
                    }
            }
        }


        double calculateLayBet(double einsatzTotal, double gewinnTotal, double quoteNeu, double einsatzAktuell, double schrittweite, bool allesPos)
        {
            /* Schritt 1 Lay-Berechnen */
            double lay = einsatzAktuell * quoteNeu - einsatzAktuell;
            /* Schritt 2: Differenz aus Lay- und Backeinsatz berechnen */
            double diffLayBack = einsatzAktuell - einsatzTotal;
            /* Schritt 3: Differenz als Gewinn Back und Verlust Lay berechnen */
            double diffWinLoose = gewinnTotal - lay;
            /* Schritt 4: Totale Differenz berechnen */
            double diffTotal = diffWinLoose - diffLayBack;

            /* Schritt 5a: Wenn Erebnis -0,1 < DiffTotal < 0,1 dann Ende*/
            if (diffTotal >= LOWTOLERANZ && diffTotal <= HIGHTOLERANZ)
            {
                return einsatzAktuell;
            }
            else if(diffTotal > HIGHTOLERANZ)
            {
                if (allesPos)
                {
                    return calculateLayBet(einsatzTotal, gewinnTotal, quoteNeu, einsatzAktuell + einsatzAktuell, schrittweite * 2, true);
                }
                else
                {
                    return calculateLayBet(einsatzTotal, gewinnTotal, quoteNeu, einsatzAktuell + schrittweite / 2, schrittweite / 2, false);
                }
            }
            else if (diffTotal < LOWTOLERANZ)
            {
                return calculateLayBet(einsatzTotal, gewinnTotal, quoteNeu, einsatzAktuell - schrittweite / 2, schrittweite / 2, false);
            }

            return 0.0;
        }


        public void GameEndedEventHandler(object sender, GameEndedEventArgs e)
        {
            if (m_ended == true)
                return;

            m_ended = true;

            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "game ended");
            this.Back.Reload();
            // Gewinn/Verlust errechnen
            double dGuV = 0.0;
            if (m_sumGoals <= MAXGOALS)
            {
                //dGuV = this.Back.BetSize - this.Lay.BetSize;
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
            // Marktprovision abziehen

            EventHandler<BFGameEndedEventArgs> handler = GameEndedEvent;
            if (handler != null)
            {
                handler(this, new BFGameEndedEventArgs(this.m_match, DateTime.Now, dGuV));
            }

            // Threads beenden, falls sie noch laufen
            if (SXThreadStateChecker.isStartedBackground(m_betWatchdogThread))
                m_betWatchdogThread.Abort();

            if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
                m_betWatcherThread.Abort();

            if (SXThreadStateChecker.isStartedBackground(m_stopLossWatchdog))
                m_stopLossWatchdog.Abort();
        }


        public void GoalEventHandler(object sender, GoalEventArgs e)
        {
            if (m_scoreA == e.ScoreA && m_scoreB == e.ScoreB)
                return;

            m_scoreA = e.ScoreA;
            m_scoreB = e.ScoreB;

            m_sumGoals = e.ScoreA + e.ScoreB;

            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("goal! new score is {0}. goalsum is {1}", e.Score, m_sumGoals));
            EventHandler<BFGoalSumChangedEventArgs> handler = GoalSumChangedEvent;
            if (handler != null)
                handler(this, new BFGoalSumChangedEventArgs(this.m_match, m_sumGoals));

            // Falls deaktiviert => raus
            if (!m_active)
            {
                DebugWriter.Instance.WriteMessage("BFBT4Strategy::GoalEvent", String.Format("{0}: Trade deactivated. Leaving.", this.Match));
                return;
            }

            /*
            // Falls watchdog schon läuft => raus
            if(SXThreadStateChecker.isStartedBackground(m_betWatchdogThread))         
            {
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal! there's already a open bet watcher running");
                return;
            }
            
                // Falls Abschluss schon läuft => raus
            if(SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
            {
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal! there's already a close trading running");
                return;
            }
            
            // Wenn Tore größer als Maximalanzahl für Strategie => raus
            if (m_sumGoals >= MAXGOALS)
            {
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal! the goal sum is greater or equal 4");
                return;
            }

            // Wenn Anzahl Tore kleiner Abschlusskriterium => raus
            if (m_sumGoals < m_config.CloseTradeGoals)
            {
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal! not enough goals for a close trading");
                return;

            }
            else
            {
                if (State != SETSTATE.SETTLED && m_started)
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal! starting close trading");
                    m_betWatcherThread = new Thread(betWatcher);
                    m_betWatcherThread.IsBackground = true;
                    m_betWatcherThread.Start();
                }
               
            }
            */

            startPlaceBetTimer();

        }

        public void GoalBackEventHandler(object sender, GoalBackEventArgs e)
        {
            if (m_scoreA == e.ScoreA && m_scoreB == e.ScoreB)
                return;

            m_scoreA = e.ScoreA;
            m_scoreB = e.ScoreB;


            m_sumGoals = e.ScoreA + e.ScoreB;

            DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), String.Format("goal canceled! new score is {0}. goalsum is {1}", e.Score, m_sumGoals));
            EventHandler<BFGoalSumChangedEventArgs> handler = GoalSumChangedEvent;
            if (handler != null)
                handler(this, new BFGoalSumChangedEventArgs(this.m_match, m_sumGoals));


            // Falls Toranzahl immer noch größer der Abschlussgrenze => starte Abschlusshandel
            if (m_sumGoals >= m_config.CloseTradeGoals)
            {
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal canceled! goal sum is still high enought for close trading");          
                // Falls Abschlusshandel schon läuft => raus
                if(SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal canceled! there's already a close trading running");
                    return;
                }
                
                // Falls watchdog schon läuft => raus
                if(SXThreadStateChecker.isStartedBackground(m_betWatchdogThread))
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal canceled! there's already a open bet watcher running");
                    return;
                }


                // Falls stop/loss schon läuft => raus
                if (SXThreadStateChecker.isStartedBackground(m_stopLossWatchdog))
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal canceled! stop/loss is running");
                    return;
                }
                if (State == SETSTATE.UNSETTELED)
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal canceled! starting close trading");
                    m_betWatcherThread = new Thread(betWatcher);
                    m_betWatcherThread.IsBackground = true;
                    m_betWatcherThread.Start();
                }
            }
            else
            {
                // Falls Abschlusshandel schon läuft => stornieren
                if(SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal canceled! stopping close trading");
                    m_betWatcherThread.Abort();
                }


                if(SXThreadStateChecker.isStartedBackground(m_stopLossWatchdog))
                {
                    DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "goal canceled! stopping stop/loss watcher");
                    m_stopLossWatchdog.Abort();
                }
                    //m_stopLossWatchdog.Abort();
            }
        }

        public void PlaytimeTick(object sender, PlaytimeTickEventArgs e)
        {
            if (m_playtime >= e.Tick)
                return;

            m_playtime = e.Tick;

            EventHandler<BFPlaytimeEventArgs> handler = PlaytimeEvent;
            if (handler != null)
            {
                handler(this, new BFPlaytimeEventArgs(this.m_match, e.Tick));
            }

            if (SXThreadStateChecker.isStartedBackground(m_stopLossWatchdog))
                return;

            if (e.Tick >= m_config.StopLossPlaytime && m_started)
            {
                DebugWriter.Instance.WriteMessage(String.Format("Strategy BackThe4 {0}", m_match), "playtime tick! starting stop/loss watcher");
                m_stopLossWatchdog = new Thread(stopLossWatchdog);
                m_stopLossWatchdog.IsBackground = true;
                m_stopLossWatchdog.Start();
            }
            
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion

        private void doDeactivation()
        {
            if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
            {
                DebugWriter.Instance.WriteMessage("BFBT4Strategy::doDeactivation", String.Format("{0}: Stopping Close-Trade", this.Match));
                m_betWatcherThread.Abort();
                m_betWatcherThread.Join();
            }
            if (SXThreadStateChecker.isStartedBackground(m_stopLossWatchdog))
            {
                DebugWriter.Instance.WriteMessage("BFBT4Strategy::doDeactivation", String.Format("{0}: Stopping Stop/Loss", this.Match));
                m_stopLossWatchdog.Join();
                m_stopLossWatchdog.Abort();
            }
        }

        private void doActivation()
        {
            // Abgeschlossen oder im Abschluss => alles i.O
            if (this.State != SETSTATE.UNSETTELED)
            {
                DebugWriter.Instance.WriteMessage("BFBT4Strategy::doActivation", String.Format("{0}: Trade is already settled. Leaving.", this.Match));
                return;
            }

            if (m_score == null && m_score2 == null)
            {
                DebugWriter.Instance.WriteMessage("BFBT4Strategy::doActivation", String.Format("{0}: No Livescores. Leaving", this.Match));
                return;
            }


            if (m_sumGoals >= MAXGOALS)
            {
                DebugWriter.Instance.WriteMessage("BFBT4Strategy::doActivation", String.Format("{0}: Already more or equals than 4 goals", this.Match));
                return;

            }

            // Falls Torsumme über Schwellwert => abschluss
            if (m_sumGoals >= m_config.CloseTradeGoals)
            {
                if (m_betWatcherThread != null)
                {
                    // Falls Wettabschlussthread schon läuft => alles i.O.
                    if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread) == true)
                    {
                        DebugWriter.Instance.WriteMessage("BFBT4Strategy::doActivation", String.Format("{0}: Close-Trade already running. Leaving", this.Match));
                        return;
                    }

                    //Ansonsten Wettabschlussthread starten
                    startPlaceBetTimer();
                }
            }


        }

        private void startPlaceBetTimer()
        {
            try
            {
                // Falls Strategie schon abgeschlossen oder im Abschluss => kein neuer Start nötig
                if (this.State == SETSTATE.SETTLED || this.State == SETSTATE.SETTLING)
                {
                    DebugWriter.Instance.WriteMessage("BFBT4Strategy::startPlaceBetTime", String.Format("{0}: Markted already settled. Don't start Close-Trade", this.Match));
                    return;
                }

                if (SXThreadStateChecker.isStartedBackground(m_betWatchdogThread))
                {
                    DebugWriter.Instance.WriteMessage("BFBT4Strategy::startPlaceBetTime", String.Format("{0}: There's already an Open-Bet-Watcher running. Don't start Close-Trade", this.Match));
                    return;
                }


                // Falls schon Marktbeobachter  und gleichstand=> dann kein Abschlussthread
                if (SXThreadStateChecker.isStartedBackground(m_stopLossWatchdog))
                {
                    DebugWriter.Instance.WriteMessage("BFBT4Strategy::startPlaceBetTime", String.Format("{0}: There's already a Stop/Loss running. Don't start Close-Trade", this.Match));
                    return;
                }
                

                // Falls BackWetterThread schon läuft => Thread beenden
                if (SXThreadStateChecker.isStartedBackground(m_betWatcherThread))
                {
                    DebugWriter.Instance.WriteMessage("BFBT4Strategy::startPlaceBetTime", String.Format("{0}: There's already a Close-Trade running. Restarting Close-Trade", this.Match));
                    m_betWatcherThread.Abort();
                    m_betWatcherThread.Join();
                }


                m_betWatcherThread = new Thread(this.betWatcher);
                m_betWatcherThread.IsBackground = true;

                m_betWatcherThread.Start();

                DebugWriter.Instance.WriteMessage("BFBT4Strategy::startPlaceBetTime", String.Format("{0}: Close-Trade Started", this.Match));
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
