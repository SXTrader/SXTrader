using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.common.Configurations;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.eventargs;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.ttr.Helper;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls
{
    public class ScorelineTrade00 : ITrade
    {        
        private SXALBetCollection _back00;
        private SXALBetCollection _lay00;
        private IScore _liveticker;
        private TTRConfigurationRW _config;
        private String _match;
        private long _marketId;
        private String _teamA;
        private String _teamB;
        private uint _traderId;
        private TradeMoneyState _tradeState;
        private Thread _hedgeThread;
        private Thread _greenThread;
        private TRADETYPE _tradeType;
        private object _tradeLock = "tradeLock";
        private object _livertickerLock = "livertickerLock";
        private bool _disposed = false;

        public ScorelineTrade00(SXALBetCollection backBet, IScore liveticker,String match, TTRConfigurationRW config, TRADETYPE tradeType)
        {
            _match = match;
            _tradeLock = "tradeLock" + _match;
            _livertickerLock = "livetickerLock" + _match;
            _tradeType = TRADETYPE.SCORELINE00;
            _traderId = TradingIdsGetter.nextId();//++_tradeIdPool;

            log("Constructing a new Scoreline 0 - 0 Trade");
            if (backBet == null)
            {
                log("No Backbet given for Scoreline Trading");
                throw new NullReferenceException("No Backbet given for Scoreline Trading");
            }

            if (backBet.Bets.Count == 0)
            {
                log("No Backbet given for Scoreline Trading");
                throw new NullReferenceException("No Backbet given for Scoreline Trading");
            }
            //TODO: Überprüfen, ob Wette auch auf den 0 - 0 Markt


            if (liveticker == null)
            {
                log("No liveticker given for Scoreline Trading");
                throw new NullReferenceException("No liveticker given for Scoreline Trading");
            }



            _back00 = new SXALBetCollection();
            _lay00  = new SXALBetCollection();

            log(String.Format("Attaching {0} Back Bets to Scoreline", backBet.Bets.Count));
            foreach (SXALBet bet in backBet.Bets.Values)
            {                
                TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
                if (t != this.TradeType)
                    continue;
                _marketId = bet.MarketId;
                if (bet.BetType == SXALBetTypeEnum.B)
                    _back00.Bets.Add(bet.BetId, bet);
                else
                    _lay00.Bets.Add(bet.BetId, bet);
            }

            log("Attaching Liveticker to Scoreline");
            _liveticker = liveticker;
            if (_liveticker != null)
            {
                //TODO: Liveticker Events verbinden
                _liveticker.BackGoalEvent += new EventHandler<GoalBackEventArgs>(_liveticker_BackGoalEvent);
                _liveticker.GameEndedEvent += new EventHandler<net.sxtrader.bftradingstrategies.lsparserinterfaces.GameEndedEventArgs>(_liveticker_GameEndedEvent);
                _liveticker.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(_liveticker_PlaytimeTickEvent);
                _liveticker.RaiseGoalEvent += new EventHandler<GoalEventArgs>(_liveticker_RaiseGoalEvent);
                _liveticker.RedCardEvent += new EventHandler<RedCardEventArgs>(_liveticker_RedCardEvent);
            }
            log("Attaching Configuration to Scoreline");
            _config = config;

            _tradeType = tradeType;

            if (_config != null &&_config.TradeOutRules.TradeType == TRADETYPE.UNASSIGNED)
                _config.TradeOutRules.TradeType = this.TradeType;

            RunningState = new TradeRunningCreadedState(this);
            RunningState.StateChanged += new EventHandler<StateChangedEventArgs>(RunningState_StateChanged);
            TradeState = new TradeMoneyCreatedState(this);
            TradeState.StateChanged += new EventHandler<StateChangedEventArgs>(TradeState_StateChanged);

            _hedgeThread = new Thread(hedgeRunner);
            _hedgeThread.IsBackground = true;
            _greenThread = new Thread(greenRunner);
            _greenThread.IsBackground = true;



            log(String.Format("Running State was set to {0}", RunningState.ToString()));
            log(String.Format("Trade State was set to {0}", TradeState.ToString()));
        }

        private void _liveticker_RedCardEvent(object sender, RedCardEventArgs e)
        {
        }

        private void _liveticker_RaiseGoalEvent(object sender, GoalEventArgs e)
        {            
            lock (_livertickerLock)
            {
                log("Received a goal event. Stopping Scoreline Trade");
                if (SXThreadStateChecker.isStartedBackground(_hedgeThread))
                {
                    log("A Hedge Thread is running. Stopping it");
                    _hedgeThread.Abort();
                    _hedgeThread.Join();
                }
                else if (SXThreadStateChecker.isStartedBackground(_greenThread))
                {
                    log("A Green Thread is running. Stopping it");
                    _greenThread.Abort();
                    _greenThread.Join();
                }

                EventHandler<StopTimerEventArgs> stopTimer = this.StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(this.Match, this));

                EventHandler<GoalScoredEventArgs> handler = this.GoalScoredEvent;
                if(handler != null)
                    handler(this, new GoalScoredEventArgs(e.Team, e.ScoreA, e.ScoreB, this.Match));
            }
        }

        private void _liveticker_PlaytimeTickEvent(object sender, PlaytimeTickEventArgs e)
        {
            lock (_livertickerLock)
            {
                EventHandler<PlaytimeEventArgs> handler = PlaytimeEvent;
                if (handler != null)
                    handler(this, new PlaytimeEventArgs(this.Match, e.Tick));

                // Wenn es nicht 0 - 0 steht, so haben wir keine Aktien im Spiel
                if (_liveticker.getScoreState() != SCORESTATE.initdraw)
                    return;

                if (_config.NoTrade)
                {
                    log("No trading-Flag set. Leaving!");
                    return;
                }

                Type type = TradeState.GetType();
                if (type == typeof(TradeMoneyCreatedState) || type == typeof(TradeMoneyOpenState))
                {
                    if (!SXThreadStateChecker.isStartedBackground(_hedgeThread))
                    {
                        log(String.Format("Reveived a Playtime Event. Playtime is {0}. Start Hedging", e.Tick));
                        _hedgeThread = new Thread(hedgeRunner);
                        _hedgeThread.IsBackground = true;
                        _hedgeThread.Start();
                    }                   
                }
                else if (type == typeof(TradeMoneyHedgedState))
                {
                    if (!_config.OnlyHedge && !SXThreadStateChecker.isStartedBackground(_greenThread))
                    {
                        log(String.Format("Reveived a Playtime Event. Playtime is {0}. Start Greening", e.Tick));
                        _greenThread = new Thread(greenRunner);
                        _greenThread.IsBackground = true;
                        _greenThread.Start();
                    }
                }
            }
        }

        private void _liveticker_GameEndedEvent(object sender, net.sxtrader.bftradingstrategies.lsparserinterfaces.GameEndedEventArgs e)
        {
            lock (_livertickerLock)
            {
                //TODO: Unstoppable überprüfen;
                log("Game has ended");
                if (_hedgeThread != null && SXThreadStateChecker.isStartedBackground(_hedgeThread))
                {
                    log("Stop Hedging Thread");
                    _hedgeThread.Abort();
                    _hedgeThread.Join();
                }

                if (_greenThread != null && SXThreadStateChecker.isStartedBackground(_greenThread))
                {
                    log("Stop Greening Thread");
                    _greenThread.Abort();
                    _greenThread.Join();
                }

                EventHandler<net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs> handler = GameEndedEvent;
                if (handler != null)
                    handler(this, new net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs(this.Match, DateTime.Now, 0.0));
            }
        }

        private void _liveticker_BackGoalEvent(object sender, GoalBackEventArgs e)
        {
            lock (_livertickerLock)
            {
                //Nichts zu tun, da das Playtimeevent die Behandlung übernimmt

                EventHandler<GoalScoredEventArgs> handler = this.GoalScoredEvent;
                if (handler != null)
                    handler(this, new GoalScoredEventArgs(e.Team, e.ScoreA, e.ScoreB, this.Match));
            }
        }

        void TradeState_StateChanged(object sender, StateChangedEventArgs e)
        {
            if(e.NewState.ToString().Equals(e.OldState.ToString()))
                return;
            log(String.Format("Trade State has changed from {0} to {1}", e.OldState.ToString(), e.NewState.ToString()));
            Type type = e.NewState.GetType();
            this.TradeState = (TradeMoneyState)e.NewState;
            TradeState.StateChanged += new EventHandler<StateChangedEventArgs>(TradeState_StateChanged);
            // Nur starten, wenn folgende Kriterien erfüllt sind
            // 1. Es muss eine Livescoreanbindung existieren
            // 2. Der Spielstand muss 0 - 0 sein
            // 3. Die Begegnung muss gestartet sein
            if (this.Score != null && this.Score.getScoreState() == SCORESTATE.initdraw && this.Score.StartDTS < DateTime.Now)
            {
                if (type == typeof(TradeMoneyCreatedState))
                {
                }
                else if (type == typeof(TradeMoneyOpenState))
                {
                    if (!SXThreadStateChecker.isStartedBackground(_hedgeThread))
                    {
                        _hedgeThread = new Thread(hedgeRunner);
                        _hedgeThread.IsBackground = true;
                        _hedgeThread.Start();
                    }
                }
                else if (type == typeof(TradeMoneyHedgedState))
                {
                    
                    EventHandler<StopTimerEventArgs> stopTimerHandler = StopTimer;
                    if (stopTimerHandler != null)
                    {
                        stopTimerHandler(this, new StopTimerEventArgs(this._match, this));
                    }

                    if (!SXThreadStateChecker.isStartedBackground(_greenThread))
                    {
                        _greenThread = new Thread(greenRunner);
                        _greenThread.IsBackground = true;
                        _greenThread.Start();
                    }
                }
                else if (type == typeof(TradeMoneyGreenedState))
                {
                    if (SXThreadStateChecker.isStartedBackground(_hedgeThread))
                    {
                        _hedgeThread.Abort();
                        _hedgeThread.Join();
                    }
                    if (SXThreadStateChecker.isStartedBackground(_greenThread))
                    {
                        _greenThread.Abort();
                        _greenThread.Join();
                    }

                }
            }
            EventHandler<StateChangedEventArgs> handler = TradeStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        void RunningState_StateChanged(object sender, StateChangedEventArgs e)
        {
            // Nach aussen weiterreichen
            EventHandler<StateChangedEventArgs> handler = RunningStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private int getHedgePlaytimeSetting()
        {
            int playtime = 0;
            try
            {
                if (_config.UseHedgeSpecialPlayTime)
                {
                    SAConfigurationRW config = new SAConfigurationRW();

                    IHistoricDataService dataService = HistoricDataServiceFactory.getInstance(_liveticker.TeamAId, _liveticker.TeamBId,
                        _liveticker.TeamA, _liveticker.TeamB);

                    HistoricDataStatistic statistic = dataService.GetStatistic(_liveticker.TeamAId, _liveticker.TeamBId,
                        _liveticker.TeamA, _liveticker.TeamB, _liveticker.League, config.NoOfData, config.AgeOfData).Result;
                    switch (_config.HedgeSpecialPlayime)
                    {
                        case SPECIALPLAYTIME.EARLIESTGOAL:
                            playtime = statistic.Direct.EarlierstFirstGoal;
                            break;
                        case SPECIALPLAYTIME.LATESTGOAL:
                            playtime = statistic.Direct.LatestFirstGoal;
                            break;
                        case SPECIALPLAYTIME.AVERAGEGOAL:
                            playtime = (int)statistic.Direct.AvgFirstGoalMinute;
                            break;
                        default:
                            playtime = 10;
                            break;
                    }
                }
                else
                {
                    playtime = _config.HedgePlayime;
                }
            }
            catch (Exception)
            {
                playtime = _config.HedgePlayime;
            }
            return playtime;
        }

        private int getGreenPlaytimeSetting()
        {
            int playtime = 0;
            try
            {

                if (_config.UseGreenSpecialPlayTime)
                {
                    SAConfigurationRW config = new SAConfigurationRW();

                    IHistoricDataService dataService = HistoricDataServiceFactory.getInstance(_liveticker.TeamAId, _liveticker.TeamBId,
                        _liveticker.TeamA, _liveticker.TeamB);

                    HistoricDataStatistic statistic = dataService.GetStatistic(_liveticker.TeamAId, _liveticker.TeamBId,
                        _liveticker.TeamA, _liveticker.TeamB, _liveticker.League, config.NoOfData, config.AgeOfData).Result;
                    switch (_config.GreenSpecialPlayime)
                    {
                        case SPECIALPLAYTIME.EARLIESTGOAL:
                            playtime = statistic.Direct.EarlierstFirstGoal;
                            break;
                        case SPECIALPLAYTIME.LATESTGOAL:
                            playtime = statistic.Direct.LatestFirstGoal;
                            break;
                        case SPECIALPLAYTIME.AVERAGEGOAL:
                            playtime = (int)statistic.Direct.AvgFirstGoalMinute;
                            break;
                        default:
                            playtime = 10;
                            break;
                    }
                }
                else
                {
                    playtime = _config.GreenPlayime;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                playtime = _config.GreenPlayime;
            }
            return playtime;
        }

        private void hedgeRunner()
        {
            bool recheck = false;
            bool checkUnmatched = false;
            try
            {
                log("Start Hedging Thread");
                TimeSpan span = new TimeSpan(0, 0, _config.HedgeWaitTime);
                if (!_config.UseWaitTime)
                    span = new TimeSpan(0, 0, 1);
                Thread.Sleep(100);

                if (_config.UseWaitTime &&
                    _config.UseHedgeWaitTime)
                {
                    span = new TimeSpan(0, 0, _config.HedgeWaitTime);
                }

                bool playtimeRetry = false;
                SXALBet layBet = null;

                // Vor dem Starten der Funktion lesen wir mind. einmal alle Wetten neu
                recheckBets();

                //Falls es mind. eine offene Wette auf Lay-Seite gibt, dann gehen wir gleich in die Überprüfung
                if(this._lay00.OneBetUnmatched)
                {
                    checkUnmatched = true;
                }

                //  lock (_tradeLock)
                //  {
                while (true)
                {
                    EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                    if (_config.UseWaitTime && _config.UseHedgeWaitTime)
                    {
                        EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;
                        //EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                        if (setTimerHandler != null)
                        {
                            setTimerHandler(this, new SetTimerEventArgs(this._match, this, span));
                        }

                        log(String.Format("Wait {0} before attempt to hedge", span.TotalSeconds));
                        Thread.Sleep(span);
                        // Zukünftige Wiederholung
                        span = new TimeSpan(0, 0, 60);
                    }
                    else
                    {
                        try
                        {
                            //auf eine bestimmte Spielzeit warten
                            if (this.Score.Playtime < getHedgePlaytimeSetting() && playtimeRetry == false)
                            {
                                log(String.Format("Wait until playtime {0} before Hedging. Current Playtime is {1}", getHedgePlaytimeSetting(),
                                    this.Score.Playtime));
                                span = new TimeSpan(0, 0, 60);


                                EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;
                                //EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                                if (setTimerHandler != null)
                                {
                                    setTimerHandler(this, new SetTimerEventArgs(this._match, this, getHedgePlaytimeSetting().ToString()));
                                }

                                Thread.Sleep(span);
                                continue;
                            }
                            else if (playtimeRetry == true)
                            {
                                span = new TimeSpan(0, 0, 60);

                                log(String.Format("Wait {0} seconds before retrying", span.TotalSeconds));

                                EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;

                                if (setTimerHandler != null)
                                {
                                    setTimerHandler(this, new SetTimerEventArgs(this._match, this, span));
                                }

                                Thread.Sleep(span);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    //Bei Wiederholungen nur 60 Sekunden warten
                    span = new TimeSpan(0, 0, 60);


                    if (_config.NoTrade)
                    {
                        log(String.Format("No trading-Flag set. Rechecking in {0} sceconds", 60));
                        span = new TimeSpan(0, 0, 60);
                        //_stoppable = true;
                        //Thread.Sleep(span);
                        continue;
                    }

                    if (_back00.Bets.Count == 0)
                    {
                        log("No Back Bets. Retrying!");
                        continue;
                    }

                    if (recheck)
                    {
                        recheckBets();
                        recheck = false;
                    }

                    //Falls es eine mind. eine Nichterfüllte Wette gab => Neu überprüfen
                    if(checkUnmatched)
                    {
                        recheckBets();
                        if(!_lay00.OneBetUnmatched)
                        {
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    //Marktpreise holen
                    log("Read Market Prices/Odds");
                    SXALMarketPrices prices = null;
                    try
                    {
                        if (!getMarketPrices(out prices))
                        {
                            log("Can not retrieve market prices. Retrying");
                            continue;
                        }
                    }
                    catch (SXALMarketDoesNotExistException mdnee)
                    {
                        ExceptionWriter.Instance.WriteException(mdnee);
                        EventHandler<SXWMessageEventArgs> message = MessageEvent;
                        String msg = String.Format("Market {0} does not exist! ExpectionMessage {1}. Leaving Trade!", MarketId, mdnee.Message);
                        log(msg);
                        if (message != null)
                        {
                            message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - OverUnder"));
                        }

                        EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                        if (stopTimer != null)
                            stopTimer(this, new StopTimerEventArgs(this._match, this));

                        return;
                    }
                    catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
                    {
                        ExceptionWriter.Instance.WriteException(mnsnae);
                        EventHandler<SXWMessageEventArgs> message = MessageEvent;
                        String msg = String.Format("Market {0} neither suspended nor active! ExpectionMessage {1}. Leaving Trade!", MarketId, mnsnae.Message);
                        log(msg);
                        if (message != null)
                        {
                            message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - OverUnder"));
                        }

                        EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                        if (stopTimer != null)
                            stopTimer(this, new StopTimerEventArgs(this._match, this));

                        return;
                    }

                    // Bester Preis zum layen lesen
                    log("Get Best Odds for Laying");
                    double layOdds = 0.0;
                    if (!getBestLayOdds(prices, (int)SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, _marketId), out layOdds))
                    {
                        log("Can not retrieve Lay Odds. Retrying!");
                        continue;
                    }

                    //Prozentüberwachung, falls notwendig
                    if (_config.UseOddsPercentage)
                    {
                        double targetOdds = _back00.BetPrice * (_config.HedgePercentage * 0.01);
                        targetOdds = (double)SXALKom.Instance.validateOdd((decimal)targetOdds);
                        if (layOdds > targetOdds)
                        {
                            log(String.Format("The Lay Odds of {0} are larger than the minimum expected odds of {1}. Set Lay Odds to excpeted value", layOdds, targetOdds));
                            layOdds = targetOdds;
                        }
                    }

                    if (_config.CheckLayOdds && layOdds >= _back00.BetPrice)
                    {
                        log(String.Format("The Lay Odd {0} is larger than the Back Odd {1}. Repeating", layOdds, _back00.BetPrice));
                        continue;
                    }

                    

                    double money = _back00.BetSize - _lay00.BetSize;

                    logBetAmount(String.Format("Calculating money for Lay. Existing Back Bets {0} {3} - Existing Lay Bets {1} {3} = {2} {3}",
                        _back00.BetSize, _lay00.BetSize, money, SXALBankrollManager.Instance.Currency));

                    if (money <= 0.0)
                    {
                        EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                        if (stopTimer != null)
                            stopTimer(this, new StopTimerEventArgs(this._match, this));
                        return;
                    }
                    
                    if (_liveticker == null)
                    {
                        log("No Liveticker. Retrying!");
                        continue;
                    }

                    if (_liveticker.getScoreState() != SCORESTATE.initdraw)
                    {
                        EventHandler<StopTimerEventArgs> stopTimerHandler = StopTimer;
                        if (stopTimerHandler != null)
                        {
                            stopTimerHandler(this, new StopTimerEventArgs(this._match, this));
                        }
                        return;
                    }

                    try
                    {
                        try
                        {
                            if (!SXALKom.Instance.SupportsBelowMinStakeBetting && money < SXALKom.Instance.MinStake)
                                money = SXALKom.Instance.MinStake;

                            layBet = SXALKom.Instance.placeLayBet(_marketId, SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, _marketId), layOdds, money);
                        }
                        catch (SXALNoBetBelowMinAllowedException)
                        {
                            log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                            layBet = SXALKom.Instance.placeLayBet(_marketId, SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, _marketId), layOdds, SXALKom.Instance.MinStake);
                        }
                        catch (SXALInsufficientFundsException)
                        {
                            log("Not enough money on betting account. Retrying!");
                            recheck = true;
                            continue;
                        }
                    }
                    catch (SXALBetInProgressException bipe)
                    {
                        ExceptionWriter.Instance.WriteException(bipe);
                        log("Received a BetInProgressException. Reread Market");
                        layBet = checkBIP(bipe.BetId);
                    }

                    if (layBet == null)
                    {
                        //TODO: Markt nachlesen?
                        log("Lay Bet is null. Retrying!");
                        recheck = true;
                        continue;
                    }

                    //Wettstatus überprüfen
                    switch (layBet.BetStatus)
                    {
                        case SXALBetStatusEnum.M:
                            {
                                //TODO: Wette der Kollektion hinzufügen, Status setzen und Greening laufen lassen
                                //_lay00.Bets.Add(layBet.betId, layBet);
                                this.addBet(layBet, false);

                                break;
                            }
                        case SXALBetStatusEnum.MU:
                            //_lay00.Bets.Add(layBet.betId, layBet);
                            this.addBet(layBet, false);
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));
                            checkUnmatched = true;
                            continue;
                            /*
                            if (!tradeCancelBet(betsChangedEvent, ref layBet, ref playtimeRetry))
                            {
                                recheck = true;
                                continue;
                            }
                             */
                        case SXALBetStatusEnum.U:
                            this.addBet(layBet, false);
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));
                            checkUnmatched = true;
                            continue;
                            /*
                            if (!tradeCancelBet(betsChangedEvent, ref layBet, ref playtimeRetry))
                            {
                                recheck = true;
                                continue;
                            }
                             */
                        default:
                            log(String.Format("Bet Status is {0}. Repeating", layBet.BetStatus.ToString()));
                            continue;
                    }                    

                    return;

                }
            }
            catch (ThreadAbortException)
            {
                EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(_match, this));

                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                String msg = String.Format("Market {0} Match {1}! Hedge Stopped!", MarketId, Match);
                log(msg);
                if (message != null)
                {
                    message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - Scoreline 0 - 0"));
                }

            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                String msg = String.Format("Market {0} Match {1}! ExpectionMessage {2}. Leaving Trade!", MarketId, Match, exc.Message);
                log(msg);
                if (message != null)
                {
                    message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - Scoreline 0 - 0"));
                }

                EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(_match, this));

            }
            finally
            {
                //Status aktualisieren;
                this.TradeState.checkState();

                EventHandler<StopTimerEventArgs> stopTimer2 = StopTimer;
                if (stopTimer2 != null)
                    stopTimer2(this, new StopTimerEventArgs(this._match, this));
            }
        }

        private void greenRunner()
        {
            bool recheck = true;
            bool checkUnmatched = false;
            try
            {
                log("Start greening Thread");
                // Kein Greening erwünscht => raus
                if (_config.OnlyHedge)
                {
                    log("Leaving Greening Thread because only Hedging is configured");
                    return;
                }

                TimeSpan span = new TimeSpan(0, 0, _config.HedgeWaitTime);
                bool playtimeRetry = false;
                SXALBet layBet = null;

                if (!_config.UseWaitTime)
                    span = new TimeSpan(0, 0, 1);
                Thread.Sleep(100);


                if (_config.UseWaitTime &&
                    _config.UseHedgeWaitTime)
                {
                    span = new TimeSpan(0, 0, _config.GreenWaitTime);
                }


                //Vor dem Ersten Lauf überprüfen wir auf jeden Fall die Wetten
                recheckBets();

                //Falls es mind. eine offene Wette auf Lay-Seite gibt, dann gehen wir gleich in die Überprüfung
                if (this._lay00.OneBetUnmatched)
                {
                    checkUnmatched = true;
                }

                //lock (_tradeLock)
                //{


                while (true)
                {
                    EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                    if (_config.UseWaitTime && _config.UseGreenWaitTime)
                    {
                        EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;
                        //EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                        if (setTimerHandler != null)
                        {
                            setTimerHandler(this, new SetTimerEventArgs(this._match, this, span));
                        }

                        log(String.Format("Wait {0} before attempt to green", span.TotalSeconds));
                        Thread.Sleep(span);
                        // Zukünftige Wiederholung
                        span = new TimeSpan(0, 0, 60);
                    }
                    else
                    {
                        try
                        {
                            //auf eine bestimmte Spielzeit warten
                            if (this.Score.Playtime < getGreenPlaytimeSetting() && playtimeRetry == false)
                            {
                                log(String.Format("Wait until playtime {0} before Greening. Current Playtime is {1}", getGreenPlaytimeSetting(),
                                    this.Score.Playtime));
                                span = new TimeSpan(0, 0, 60);

                                EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;
                                //EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                                if (setTimerHandler != null)
                                {
                                    setTimerHandler(this, new SetTimerEventArgs(this._match, this, getGreenPlaytimeSetting().ToString()));
                                }

                                Thread.Sleep(span);
                                continue;
                            }
                            else if (playtimeRetry == true)
                            {
                                span = new TimeSpan(0, 0, 60);

                                log(String.Format("Wait {0} seconds before retrying", span.TotalSeconds));

                                EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;

                                if (setTimerHandler != null)
                                {
                                    setTimerHandler(this, new SetTimerEventArgs(this._match, this, span));
                                }

                                Thread.Sleep(span);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    if (_config.NoTrade)
                    {
                        log(String.Format("No trading-Flag set. Rechecking in {0} sceconds", 60));
                        span = new TimeSpan(0, 0, 60);
                        //_stoppable = true;
                        //Thread.Sleep(span);
                        continue;
                    }

                    if (_back00.Bets.Count == 0)
                    {
                        log("No Back Bets. Retrying!");
                        continue;
                    }

                    if(recheck)
                    {
                        recheckBets();
                        recheck = false;
                    }

                    //Falls es eine mind. eine Nichterfüllte Wette gab => Neu überprüfen
                    if (checkUnmatched)
                    {
                        recheckBets();
                        if (!_lay00.OneBetUnmatched)
                        {
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //Marktpreise holen
                    log("Read Market Prices/Odds");
                    SXALMarketPrices prices = null;
                    if (!getMarketPrices(out prices))
                    {
                        log("Can not retrieve market prices. Retrying");
                        continue;
                    }

                    // Bester Preis zum layen lesen
                    log("Get Best Odds for Laying");
                    double layOdds = 0.0;
                    if (!getBestLayOdds(prices, (int)SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, _marketId), out layOdds))
                    {
                        log("Can not retrieve Lay Odds. Retrying!");
                        continue;
                    }

                    if (_config.UseOddsPercentage)
                    {
                        double targetOdds = _back00.BetPrice * (_config.GreenPercentage * 0.01);
                        targetOdds = (double)SXALKom.Instance.validateOdd((decimal)targetOdds);
                        if (layOdds > targetOdds)
                        {
                            log(String.Format("The Lay Odds of {0} are larger than the minimum expected odds of {1}. Set Lay Odds to Expected Odds", layOdds, targetOdds));
                            layOdds = targetOdds;
                        }
                    }

                    if (!(layOdds < _back00.BetPrice))
                    {
                        log(String.Format("Lay Odds {0} are higher than Back Odds {0}. Retrying", layOdds, _back00.BetPrice));
                        continue;
                    }

                    //Prozentüberwachung, falls notwendig
                   

                    if (_liveticker == null)
                    {
                        log("No Liveticker. Retrying!");
                        continue;
                    }

                    if (_liveticker.getScoreState() != SCORESTATE.initdraw)
                    {
                        log(String.Format("Score is {0}. Leaving!", _liveticker.getScore()));
                        EventHandler<StopTimerEventArgs> stopTimerHandler = StopTimer;
                        if (stopTimerHandler != null)
                        {
                            stopTimerHandler(this, new StopTimerEventArgs(this._match, this));
                        }
                        return;
                    }

                    double money = ((_back00.BetSize * _back00.BetPrice - _back00.BetSize) - (_lay00.BetSize * _lay00.BetPrice - _lay00.BetSize)) / layOdds;

                    logBetAmount(String.Format("Calculating money for Lay. (Existing Lay Bets {1} {3} - Existing Back Bets {0} {3}) / Odds {4} = {2} {3}",
                        _back00.BetSize, _lay00.BetSize, money, SXALBankrollManager.Instance.Currency, layOdds));

                    if (money <= 0.0)
                    {
                        log(String.Format("Calculated money is {0}. Leaving!", money));
                        EventHandler<StopTimerEventArgs> stopTimerHandler = StopTimer;
                        if (stopTimerHandler != null)
                        {
                            stopTimerHandler(this, new StopTimerEventArgs(this._match, this));
                        }
                        return;
                    }

                    //double money = (_lay00.BetSize - _back00.BetSize) / layOdds;
                    

                    try
                    {
                        try
                        {
                            if (SXALKom.Instance.SupportsBelowMinStakeBetting == false && money < SXALKom.Instance.MinStake)
                                money = SXALKom.Instance.MinStake;

                            layBet = SXALKom.Instance.placeLayBet(_marketId, SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, _marketId), layOdds, money);
                        }
                        catch (SXALNoBetBelowMinAllowedException)
                        {
                            log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                            layBet = SXALKom.Instance.placeLayBet(_marketId, SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, _marketId), layOdds, SXALKom.Instance.MinStake);
                        }
                        catch (SXALInsufficientFundsException)
                        {
                            log("Not enough money on betting account. Retrying!");
                            recheck = true;
                            continue;
                        }
                    }
                    catch (SXALBetInProgressException bipe)
                    {
                        ExceptionWriter.Instance.WriteException(bipe);
                        log("Received a BetInProgressException. Reread Market");
                        layBet = checkBIP(bipe.BetId);
                    }

                    if (layBet == null)
                    {
                        //TODO: was tun?
                        log("Lay Bet is null. Retrying!");
                        recheck = true;
                        continue;
                    }

                    //Wettstatus überprüfen
                    switch (layBet.BetStatus)
                    {
                        case SXALBetStatusEnum.M:
                            //TODO: Wette der Kollektion hinzufügen, Status setzen und Greening laufen lassen
                            this.addBet(layBet, false);
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));
                            //_lay00.Bets.Add(layBet.betId, layBet);
                            break;
                        case SXALBetStatusEnum.MU:
                            //_lay00.Bets.Add(layBet.betId, layBet);
                            this.addBet(layBet, false);
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));
                            checkUnmatched = true;
                            continue;                            
                        case SXALBetStatusEnum.U:
                            this.addBet(layBet, false);
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));
                            checkUnmatched = true;
                            continue;
                        default:
                            log(String.Format("Bet Status is {0}. Repeating", layBet.BetStatus.ToString()));
                            recheck = true;
                            continue;
                    }

                    /*
                    //Status aktualisieren;
                    this.TradeState.checkState();


                    EventHandler<StopTimerEventArgs> stopTimerHandler2 = StopTimer;
                    if (stopTimerHandler2 != null)
                    {
                        stopTimerHandler2(this, new StopTimerEventArgs(this._match, this));
                    }*/
                    return;
                }
            }
            catch (ThreadAbortException)
            {
                EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(_match, this));

                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                String msg = String.Format("Market {0} Match {1}! Greening Stopped!", MarketId, Match);
                log(msg);
                if (message != null)
                {
                    message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - Scoreline 0 - 0"));
                }

            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                String msg = String.Format("Market {0} Match {1}! ExpectionMessage {2}. Leaving Trade!", MarketId, Match, exc.Message);
                log(msg);
                if (message != null)
                {
                    message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - Scoreline 0 - 0"));
                }

                EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(_match, this));
            }
            finally
            {
                //Status aktualisieren;
                this.TradeState.checkState();

                EventHandler<StopTimerEventArgs> stopTimer2 = StopTimer;
                if (stopTimer2 != null)
                    stopTimer2(this, new StopTimerEventArgs(this._match, this));
            }
        }

        /// <summary>
        /// Überprüfung einer Wette nach einer BetInProgressException
        /// </summary>
        /// <param name="betId"></param>
        /// <returns></returns>
        private SXALBet checkBIP(long betId)
        {
            log(String.Format("Rechecking bet {0} after a BetInProgress Exception", betId));
            SXALBet bet = null;
            TimeSpan span = new TimeSpan(0, 0, 10);
            while(true)
            {
                Thread.Sleep(span);
                try
                {
                    bet = SXALKom.Instance.getBetDetail(betId);
                    if (bet == null)
                        continue;

                    return bet;
                }
                catch (SXALBetInProgressException bipe)
                {
                    log(String.Format("Reveived a BetInProgress Exception for {0}", bipe.BetId));
                }
            }
        }

        private bool cancelBet(ref SXALBet bet)
        {
            bool bReturn = true;
            if (!SXALKom.Instance.cancelBet(bet.BetId))
            {                
                bReturn = false;
            }

            bet = SXALKom.Instance.getBetDetail(bet.BetId);

            return bReturn;
        }

        private bool tradeCancelBet(EventHandler<BetsChangedEventArgs> betsChangedEvent, ref SXALBet bet, ref bool retry)
        {
            if (!cancelBet(ref bet))
            {
                if (bet.BetStatus == SXALBetStatusEnum.M)
                {
                    addBet(bet, false);                        
                    retry = false;
                    return true;
                }
                else
                {
                    log(String.Format("Lay Bet Status is {0}. Retrying!", bet.BetStatus));
                    if (bet.BetStatus == SXALBetStatusEnum.MU)
                    {
                        addBet(bet, false);
                        
                    }
                    else
                    {
                        bet = null;
                    }
                   
                    retry = true;
                    return false;
                }

            }
            else
            {
                log(String.Format("Lay Bet Status {0}. Retrying!", bet.BetStatus));
                retry = true;
                return false;
            }
        }

        /// <summary>
        /// Gibt die beste zur Verfügung stehende Quote für eine Lay Wette zurück,
        /// sofern möglich.
        /// </summary>
        /// <param name="prices">Alle Marktpreise/Quoten</param>
        /// <param name="selectionId">Die Selection ID deren Quote gelesen werden sollen</param>
        /// <param name="odds">Die gefundene Quote</param>
        /// <returns>Indikator, ob Quote gelesen werden konnten oder nicht </returns>
        private bool getBestLayOdds(SXALMarketPrices prices, int selectionId, out double odds)
        {
            odds = 0.0;
            foreach (SXALRunnerPrices runnerPrices in prices.RunnerPrices)
            {
                if (runnerPrices.SelectionId != selectionId)
                    continue;

                if (runnerPrices.BestPricesToLay.Length == 0)
                {
                    log("There are no prices/odds for laying. Repeat");
                    return false;
                }

                odds = runnerPrices.BestPricesToLay[0].Price;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Liefert die aktuellen Marktpreise, sofern möglich zurück
        /// </summary>
        /// <param name="prices">Die Marktpreise</param>
        /// <returns>Indikator, ob Marktpreise gelesen werden konnten oder nicht (z.B. weil Markt suspendiert)</returns>
        private bool getMarketPrices(out SXALMarketPrices prices)
        {
            prices = null;
            prices = SXALKom.Instance.getMarketPrices(_marketId);
            if (prices == null)
            {
                log("Can't read market prices. Repeating");
                return false;
            }

            if (prices.MarketStatus != SXALMarketStatusEnum.ACTIVE)
            {
                log(String.Format("Market Status is not ACTIVE but {0}. Repeating", prices.MarketStatus.ToString()));
                return false;
            }
            return true;
        }

        private void splitMatchName(String match)
        {
            String[] sSeps = { " - ", " v " };

            String[] teams = match.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);
            /*if (teams.Length == 1)
                teams = market.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);
             */
            _teamA = teams[0].Trim();
            _teamB = teams[teams.GetLength(0) - 1].Trim();
        }

        public void log(string message)
        {
            try
            {
                TradeLog.Instance.writeLog(this._match, "Scoreline00", "Trader", String.Format("ID {0}: {1}", _traderId, message));
            }
            catch { }
        }

        public void logBetAmount(string message)
        {
            try
            {
                TradeLog.Instance.writeBetAmountLog(this._match, "Scoreline00", "Trader", String.Format("ID {0}: {1}", _traderId, message));
            }
            catch { }
        }

        #region ITrade Member

        public TradeRunningState RunningState
        {
            get;
            set;
        }

        public bool hasBet(long betId)
        {
            if (_back00.Bets.ContainsKey(betId) || _lay00.Bets.ContainsKey(betId))
            {
                return true;
            }
            return false;
        }

        public TradeMoneyState TradeState
        {
            get { return _tradeState; }
            set
            {
                if (_tradeState == null)
                    _tradeState = value;
                else
                {
                    if (_tradeState.GetType() == value.GetType())
                        return;
                    else
                    {
                        // Nachricht verbreiten
                        EventHandler<SXWMessageEventArgs> message = MessageEvent;
                        if (message != null)
                            message(this, new SXWMessageEventArgs(DateTime.Now, Match, String.Format(TradeTheReaction.strMsgTradeStateChanged, _tradeState.ToString(), value.ToString()), String.Format("{0}:{1}", TradeTheReaction.strName, TradeTypeName)));
                        _tradeState = value;
                    }
                }

                //Gleich nach dem setzen den Status überprüfen, um so den System die
                //Gelegenheit zu geben, sich automatisch weiterzusetzen;
                _tradeState.checkState();
            }
        }

        public SXALBetCollection Back
        {
            get { return _back00; }
        }

        public SXALBetCollection Lay
        {
            get { return _lay00; }
        }

        public event EventHandler<SetTimerEventArgs> SetTimer;

        public event EventHandler<StopTimerEventArgs> StopTimer;

        public event EventHandler<StateChangedEventArgs> RunningStateChanged;

        public event EventHandler<StateChangedEventArgs> TradeStateChanged;

        public event EventHandler<SXWMessageEventArgs> MessageEvent;

        public event EventHandler<GoalScoredEventArgs> GoalScoredEvent;

        public event EventHandler<PlaytimeEventArgs> PlaytimeEvent;

        public event EventHandler<net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs> GameEndedEvent;

        public event EventHandler<BetsChangedEventArgs> BetsChangedEvent;

        public IScore Score
        {
            get
            {
                return _liveticker;
            }
            set
            {
                if (_liveticker != null)
                {
                    _liveticker.BackGoalEvent -= _liveticker_BackGoalEvent;
                    _liveticker.GameEndedEvent -= _liveticker_GameEndedEvent;
                    _liveticker.PlaytimeTickEvent -= _liveticker_PlaytimeTickEvent;
                    _liveticker.RaiseGoalEvent -= _liveticker_RaiseGoalEvent;
                    _liveticker.RedCardEvent -= _liveticker_RedCardEvent;
                }
                _liveticker = value;
                if (_liveticker != null)
                {
                    _liveticker.BackGoalEvent += new EventHandler<GoalBackEventArgs>(_liveticker_BackGoalEvent);
                    _liveticker.GameEndedEvent+= new EventHandler<net.sxtrader.bftradingstrategies.lsparserinterfaces.GameEndedEventArgs>(_liveticker_GameEndedEvent);
                    _liveticker.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(_liveticker_PlaytimeTickEvent);
                    _liveticker.RaiseGoalEvent += new EventHandler<GoalEventArgs>(_liveticker_RaiseGoalEvent);
                    _liveticker.RedCardEvent += new EventHandler<RedCardEventArgs>(_liveticker_RedCardEvent);
                }
            }
        }

        public net.sxtrader.plugin.IConfiguration Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value as TTRConfigurationRW;
            }
        }

        public String Match
        {
            get { return _match; }
            /*set { _match = value; }*/
        }

        public String TeamA
        {
            get { return _teamA; }
        }

        public String TeamB
        {
            get { return _teamB; }
        }

        public long MarketId
        {
            get { return _marketId; }
        }

        public bool isSettled()
        {
            if (_config.OnlyHedge && this.TradeState.GetType() == typeof(TradeMoneyHedgedState))
                return true;
            else if (this.TradeState.GetType() == typeof(TradeMoneyGreenedState))
                return true;
            return false;
        }

        public void addBet(SXALBet bet, bool withTradeCheck)
        {
            try
            {
                if (bet == null)
                    return;

                TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
                if (t != TRADETYPE.SCORELINE00)
                    return;

                if (bet.BetType == SXALBetTypeEnum.B)
                {
                    if (!addBetInternal(_back00, bet))
                        return;
                }
                else
                {
                    if (!addBetInternal(_lay00, bet))
                        return;
                }
                EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                if (betsChangedEvent != null)
                    betsChangedEvent(this, new BetsChangedEventArgs(this));

                if (withTradeCheck)
                {
                    _tradeState.checkState();
                }
            }
            catch(Exception exc)
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

        public string TradeTypeName
        {
            get { return "Scoreline 0 - 0"; }
        }

        public uint TradeId
        {
            get { return _traderId; }
        }

        public void start()
        {
            TradeState.checkState();
        }


        public double getInitialStake()
        {
            return this.Back.BetSize;
        }

        public double getWinnings()
        {
            return this.Back.RiskWin - this.Lay.RiskWin;
        }




        public double getPLSnapshot()
        {
            double dMoney = 0.0;

            if (Score != null & Score.ScoreA + Score.ScoreB > 0)
            {
                // Kein 0 - 0 Lay hat gewonnen.
                dMoney = this.Lay.BetSize - this.Back.BetSize;
            }
            else if (Score != null)
            {
                // 0 - 0 Back hat gewonnen
                dMoney = (this.Back.BetSize * this.Back.BetPrice - this.Back.BetSize) - (this.Lay.BetSize * this.Lay.BetPrice - this.Lay.BetSize);
            }


            return dMoney;
        }

        #endregion

        #region ITrade Member


        public TRADETYPE TradeType
        {
            get { return _tradeType; }
        }

        #endregion

        #region ITrade Member


        public TRADEMODE TradeMode
        {
            get { return TRADEMODE.BACK; }
        }

        #endregion

        #region IDisposable Member

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!this._disposed)
                {
                    log("Disposing Trade");

                    if (_hedgeThread != null && SXThreadStateChecker.isStartedBackground(_hedgeThread))
                    {
                        log("Stop Hedging Thread");
                        _hedgeThread.Abort();
                        _hedgeThread.Join();
                        //_stoppable = true;
                    }

                    if (_greenThread != null && SXThreadStateChecker.isStartedBackground(_greenThread))
                    {
                        log("Stop Greening Thread");
                        _greenThread.Abort();
                        _greenThread.Join();
                        //_stoppable = true;
                    }

                    if (this.Score != null)
                    {
                        this.Score.DecreaseRef();
                        this.Score = null;
                    }

                    this._disposed = true;
                }
            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                if (message != null)
                    message(this, new SXWMessageEventArgs(DateTime.Now, this.Match, "There was an exception. Please check ExceptionsOutput.txt", this.TradeTypeName));
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        #endregion

        private void recheckBets()
        {
            try
            {
                //List der Wetten aufbauen
                SXALBetCollection tmp = new SXALBetCollection();
                foreach(SXALBet b in _back00.Bets.Values)
                {
                    tmp.Bets.Add(b.BetId, b);
                }

                foreach (SXALBet b in _lay00.Bets.Values)
                {
                    tmp.Bets.Add(b.BetId, b);
                }

                SXALMUBet[] muBets = SXALKom.Instance.getBetsMU(this.Back.MarketId);
                foreach (SXALMUBet muBet in muBets)
                {
                    /*
                    if (muBet.BetStatus == SXALBetStatusEnum.C || muBet.BetStatus == SXALBetStatusEnum.L ||
                        muBet.BetStatus == SXALBetStatusEnum.S || muBet.BetStatus == SXALBetStatusEnum.V)
                        continue;
                     */
                    SXALBet bet = SXALKom.Instance.getBetDetail(muBet.BetId);                    

                    if(tmp.Bets.ContainsKey(bet.BetId))
                    {
                        tmp.Bets.Remove(bet.BetId);
                    }

                    TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
                    if (t != TRADETYPE.SCORELINE00)
                        continue;

                    addBet(bet, false);

                }

                //Alle Wetten, die jetzt noch vorhandne sind, sind weder matched noch unmatched, also gesondert überprüfen.
                foreach(SXALBet b in tmp.Bets.Values)
                {
                    SXALBet bet = SXALKom.Instance.getBetDetail(b.BetId);

                    TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
                    if (t != TRADETYPE.SCORELINE00)
                        continue;

                    addBet(bet, false);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
