using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Threading;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.muk.eventargs;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.ttr.Helper;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder
{
    public enum OUTYPE { OVER, UNDER, UNASSIGNED };
    public enum OUVALUE { ZEROFIVE = 1, ONEFIVE = 2, TWOFIVE = 3, THREEFIVE = 4, FOURFIVE=5, FIVEFIVE = 6, SIXFIVE =7, 
        SEVENFIVE = 8, EIGHTFIVE = 9, UNASSIGNED=0 };
    
    public class OverUnderTrade : ITrade
    {
        private enum TRADEOUTTYP { LAYSAME, BACKOPPOSITE, UNDECEIDED };

        private OUTYPE _ouType = OUTYPE.UNASSIGNED;
        private OUVALUE _ouValue = OUVALUE.UNASSIGNED;
        private TradeMoneyState _tradeState;
        private String _match;
        private uint _traderId;
        private IScore _liveticker;
        private SXALBetCollection _overBets;
        private SXALBetCollection _underBets;
        private long _marketId;
        private TTRConfigurationRW _config;
        private TTRTradeOutCheck _currentTradeOutSettings;
        private TRADEOUTTYP _tradeOutTyp = TRADEOUTTYP.UNDECEIDED;
        private TRADETYPE _tradeType;
        private bool _stoppable = true;
        private bool _disposed = false;
        private Thread _hedgeThread;
        private Thread _greenThread;

        private String _teamA;
        private String _teamB;

        private object _tradeLock = "tradeLock";
        private object _livertickerLock = "livertickerLock";

        public OUTYPE OverUnderType { get { return _ouType; } }
        public OUVALUE OverUnderValue { get { return _ouValue; } }

        public OverUnderTrade(SXALBetCollection bets, IScore liveticker,String match, TTRConfigurationRW config, OUTYPE ouType, OUVALUE ouValue, TRADETYPE tradeType)
        {
            _match = match;
            _tradeLock = "tradeLock" + _match;
            _livertickerLock = "livetickerLock" + _match;
            splitMatchName(_match);

            _traderId = TradingIdsGetter.nextId();

            _ouType = ouType;
            _ouValue = ouValue;

            _tradeType = tradeType;

            log(String.Format("Constructing a new {0} Trade",this.TradeTypeName));

            if (liveticker == null)
            {
                log("No liveticker given for Over/Under Trading");
                throw new NullReferenceException("No liveticker given for Over/Under Trading");
            }

            log("Attaching Liveticker to Over/Under");
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

            _overBets = new SXALBetCollection();
            _underBets = new SXALBetCollection();

            log(String.Format("Attaching {0} Bets to Over/Under", bets.Bets.Count));
            foreach (SXALBet bet in bets.Bets.Values)
            {               
                TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
                if (t != this.TradeType)
                    continue;
                if (SXALKom.Instance.isUnder(bet.SelectionId, this.Match))
                {
                    _marketId = bet.MarketId;
                    _underBets.Bets.Add(bet.BetId, bet);
                }

                if (SXALKom.Instance.isOver(bet.SelectionId, this.Match))
                {
                    _marketId = bet.MarketId;
                    _overBets.Bets.Add(bet.BetId, bet);
                }
            }

            RunningState = new TradeRunningCreadedState(this);
            RunningState.StateChanged += new EventHandler<StateChangedEventArgs>(RunningState_StateChanged);
            TradeState = new TradeMoneyCreatedState(this);
            TradeState.StateChanged += new EventHandler<StateChangedEventArgs>(TradeState_StateChanged);

            _hedgeThread = new Thread(hedgeRunner);
            _hedgeThread.IsBackground = true;
            _greenThread = new Thread(greenRunner);
            _greenThread.IsBackground = true;

            TradeState.checkState();

            _config = config;

            if (_config != null && _config.TradeOutRules.TradeType == TRADETYPE.UNASSIGNED)
                _config.TradeOutRules.TradeType = this.TradeType;

          
        }

        private void _liveticker_RedCardEvent(object sender, RedCardEventArgs e)
        {
            lock (_livertickerLock)
            {
                log("Received a Red Card Event. Reevaluating Trade Out Rules");
                tradeRuleCheck(TRADEOUTTRIGGER.REDCARD, false);
            }
        }

        private void _liveticker_RaiseGoalEvent(object sender, GoalEventArgs e)
        {
            lock (_livertickerLock)
            {
                log("Received a Goal Event. Reevaluating Trade Out Rules");
                tradeRuleCheck(TRADEOUTTRIGGER.GOAL, false);
            }

            EventHandler<GoalScoredEventArgs> handler = this.GoalScoredEvent;
            if (handler != null)
                handler(this, new GoalScoredEventArgs(e.Team, e.ScoreA, e.ScoreB, this.Match));
        }

        private void _liveticker_PlaytimeTickEvent(object sender, PlaytimeTickEventArgs e)
        {
            lock (_livertickerLock)
            {
                log("Received a Playtime Event. Reevaluating Trade Out Rules");
                tradeRuleCheck(TRADEOUTTRIGGER.PLAYTIME, false);

                EventHandler<PlaytimeEventArgs> handler = PlaytimeEvent;
                if (handler != null)
                    handler(this, new PlaytimeEventArgs(this.Match, e.Tick));
            }
        }

        private void _liveticker_GameEndedEvent(object sender, net.sxtrader.bftradingstrategies.lsparserinterfaces.GameEndedEventArgs e)
        {
            lock (_livertickerLock)
            {
                log("Game has ended");
                if (_hedgeThread != null && SXThreadStateChecker.isStartedBackground(_hedgeThread))
                {
                    log("Stop Hedging Thread");
                    _hedgeThread.Abort();
                    _hedgeThread.Join();
                    _stoppable = true;
                }

                if (_greenThread != null && SXThreadStateChecker.isStartedBackground(_greenThread))
                {
                    log("Stop Greening Thread");
                    _greenThread.Abort();
                    _greenThread.Join();
                    _stoppable = true;
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
                log("Received a Goal Back Event. Reevaluating Trade Out Rules");
                tradeRuleCheck(TRADEOUTTRIGGER.GOAL, false);

                EventHandler<GoalScoredEventArgs> handler = this.GoalScoredEvent;
                if (handler != null)
                    handler(this, new GoalScoredEventArgs(e.Team, e.ScoreA, e.ScoreB, this.Match));
            }
        }

        private void tradeRuleCheck(TRADEOUTTRIGGER trigger, bool init)
        {
             // Schaue, ob aktuelle Lauf noch stoppbar
            if (!_stoppable)
            {
                log("Current Trade is in an unstoppable state. Leaving");
                return;
            }

            if (trigger == TRADEOUTTRIGGER.PLAYTIME && _liveticker.Playtime >= 45 && _liveticker.Playtime <= 46)
            {
                log("Test");
            }

            // Als erstes Schau einmal, was für eine Regel gelten würde
            TTRTradeOutCheck tmp = _config.TradeOutRules.getTradeOutSettings(trigger, this.Score);

            if (tmp == null && trigger != TRADEOUTTRIGGER.PLAYTIME)
            {
                log("No Rule found. Stopping running TradeOuts and Leaving");
                if (SXThreadStateChecker.isStartedBackground(_hedgeThread))
                {
                    log("A Hedge Thread is running. Stopping it");
                    _hedgeThread.Abort();
                    _hedgeThread.Join();
                    _stoppable = true;
                }
                if (SXThreadStateChecker.isStartedBackground(_greenThread))
                {
                    log("A Green Thread is running. Stopping it");
                    _greenThread.Abort();
                    _greenThread.Join();
                    _stoppable = true;
                }
                return;
            }
            else if (tmp == null && trigger == TRADEOUTTRIGGER.PLAYTIME)
            {
                return;
            }
            else if (tmp.Order == 0 && trigger == TRADEOUTTRIGGER.PLAYTIME)
            {
                return;
            }

            if (init && tmp.Order == 0)
            {
                log("No default settings are allowed while initialization!");
                return;
            } 

            // Noch keine aktuelle Regel?
            if (_currentTradeOutSettings == null)
            {
                log(String.Format("Trade Out Rule will be set to Rule {0}", tmp.Id));
                _currentTradeOutSettings = tmp;
            }
            else
            {
                // Falls die Ids gleich sind => keine Änderungen nötig
                if (tmp.Id == _currentTradeOutSettings.Id)
                {
                    log("Trade Out Rule hasn't changed.");
                    return;
                }

                log(String.Format("Trade Out Rule will be changed from Rule {0} to Rule {1}", _currentTradeOutSettings.Id, tmp.Id));
                _currentTradeOutSettings = tmp;
            }

            // Schaue, ob aktuelle Lauf noch stoppbar
            if (!_stoppable)
            {
                log(String.Format("Current Trade is in an unstoppable state. Leaving"));
                return;
            }

            // Ansonsten alle aktuellen Läufe stoppen
            if (SXThreadStateChecker.isStartedBackground(_hedgeThread))
            {
                log("A Hedge Thread is running. Stopping it");
                _hedgeThread.Abort();
                _hedgeThread.Join();
                _stoppable = true;
            }
            if (SXThreadStateChecker.isStartedBackground(_greenThread))
            {
                log("A Green Thread is running. Stopping it");
                _greenThread.Abort();
                _greenThread.Join();
                _stoppable = true;
            }

            EventHandler<StopTimerEventArgs> stopTimer = this.StopTimer;
            if (stopTimer != null)
                stopTimer(this, new StopTimerEventArgs(this.Match, this));

            // Und wieder starten
            Type type = TradeState.GetType();

             if (type == typeof(TradeMoneyCreatedState) || type == typeof(TradeMoneyOpenState))
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
                if (!_currentTradeOutSettings.TradeOutSettings.OnlyHedge && !SXThreadStateChecker.isStartedBackground(_greenThread))
                {
                    _greenThread = new Thread(greenRunner);
                    _greenThread.IsBackground = true;
                    _greenThread.Start();
                }
            }

        }

        void TradeState_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.NewState.ToString().Equals(e.OldState.ToString()))
                return;

            log(String.Format("Trade State has changed from {0} to {1}", e.OldState.ToString(), e.NewState.ToString()));
            Type type = e.NewState.GetType();
            this.TradeState = (TradeMoneyState)e.NewState;
            TradeState.StateChanged += new EventHandler<StateChangedEventArgs>(TradeState_StateChanged);
            
            // Nur starten, wenn folgende Kriterien erfüllt sind
            // 1. Es muss eine Livescoreanbindung existieren
            // 2. Die Summe der Tore is weniger als die max. Anzahl für Trading
            // 3. Die Begegnung muss gestartet sein
            if (this.Score != null && this.Score.ScoreA + this.Score.ScoreB < (int)_ouValue && this.Score.StartDTS < DateTime.Now)
            {
                if (type == typeof(TradeMoneyCreatedState))
                {
                }
                else if (type == typeof(TradeMoneyOpenState))
                {
                    if (!SXThreadStateChecker.isStartedBackground(_hedgeThread) && _currentTradeOutSettings != null)
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

                    if (!SXThreadStateChecker.isStartedBackground(_greenThread) && _currentTradeOutSettings != null)
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

                        EventHandler<StopTimerEventArgs> stopTimerHandler = StopTimer;
                        if (stopTimerHandler != null)
                        {
                            stopTimerHandler(this, new StopTimerEventArgs(this._match, this));
                        }
                    }
                    if (SXThreadStateChecker.isStartedBackground(_greenThread))
                    {
                        _greenThread.Abort();
                        _greenThread.Join();

                        EventHandler<StopTimerEventArgs> stopTimerHandler = StopTimer;
                        if (stopTimerHandler != null)
                        {
                            stopTimerHandler(this, new StopTimerEventArgs(this._match, this));
                        }
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

        private void splitMatchName(String match)
        {
            String[] sSeps = { " - ", " v " };

            String[] teams = match.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);

            _teamA = teams[0].Trim();
            _teamB = teams[teams.GetLength(0) - 1].Trim();
        }

        private void hedgeRunner()
        {
            try
            {
                log("Start Hedging Thread");

                TimeSpan span = new TimeSpan(0, 0, 60);
                Boolean playtimeRetry = false;
                SXALBet layBet = null;


                //Bestimmte Zeit warten oder auf Spielminute?
                if (_currentTradeOutSettings.TradeOutSettings.UseWaitTime &&
                    _currentTradeOutSettings.TradeOutSettings.UseHedgeWaitTime)
                {
                    span = new TimeSpan(0, 0, _currentTradeOutSettings.TradeOutSettings.HedgeWaitTime);
                }
               

                while (true)
                {
                    _stoppable = true;
                    EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;

                    if (_currentTradeOutSettings.TradeOutSettings.UseWaitTime &&
                        _currentTradeOutSettings.TradeOutSettings.UseHedgeWaitTime)
                    {
                        log(String.Format("Wait {0} before attempt to hedge", span.TotalSeconds));
                        EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;

                        if (setTimerHandler != null)
                        {
                            setTimerHandler(this, new SetTimerEventArgs(this._match, this, span));
                        }

                        Thread.Sleep(span);
                        // Zukünftige Wiederholung
                        span = new TimeSpan(0, 0, 60);
                    }
                    else
                    {
                        //auf eine bestimmte Spielzeit warten
                        if (this.Score.Playtime < _currentTradeOutSettings.TradeOutSettings.HedgePlaytime && playtimeRetry == false)
                        {
                            log(String.Format("Wait until playtime {0} before hedging. Current Playtime is {1}", _currentTradeOutSettings.TradeOutSettings.HedgePlaytime,
                                this.Score.Playtime));
                            span = new TimeSpan(0, 0, 60);
                            _stoppable = true;

                            EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;

                            if (setTimerHandler != null)
                            {
                                setTimerHandler(this, new SetTimerEventArgs(this._match, this, _currentTradeOutSettings.TradeOutSettings.HedgePlaytime.ToString()));
                            }

                            Thread.Sleep(span);
                            continue;
                        }
                        else if (playtimeRetry == true)
                        {
                            span = new TimeSpan(0, 0, 60);
                            _stoppable = true;

                            log(String.Format("Wait {0} seconds before retrying", span.TotalSeconds));

                            EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;

                            if (setTimerHandler != null)
                            {
                                setTimerHandler(this, new SetTimerEventArgs(this._match, this, span));
                            }

                            Thread.Sleep(span);
                        }
                    }



                    if (_currentTradeOutSettings.TradeOutSettings.NoTrade)
                    {
                        log(String.Format("No trading-Flag set. Rechecking in {0} sceconds", 60));
                        span = new TimeSpan(0, 0, 60);
                        _stoppable = true;
                        playtimeRetry = true;
                        Thread.Sleep(span);
                        continue;
                    }

                    //TODO: Dynamisch je nachdem ob Over/Under
                    // Kein Trading mehr nötig
                    if (Score.ScoreA + Score.ScoreB >= (int)_ouValue)
                    {
                        log(String.Format("GoalSum {0} is larger than {1}. No Trading possible. Leave Hedging!", Score.ScoreA + Score.ScoreB, (int)_ouValue));
                        EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                        if (stopTimer != null)
                            stopTimer(this, new StopTimerEventArgs(this._match, this));
                        return;
                    }

                    // Wartezeit abgelaufen oder Spielzeit erreicht
                    _stoppable = false;


                    if (this.Back.Bets.Count == 0)
                    {
                        log("No Back Bets. Retrying!");
                        _stoppable = true;
                        playtimeRetry = true;
                        continue;
                    }

                    if (_liveticker == null)
                    {
                        log("No Liveticker. Retrying!");
                        _stoppable = true;
                        playtimeRetry = true;
                        continue;
                    }

                    if (layBet != null)
                    {
                        log("Rechecking an open LayBet");
                        SXALBetStatusEnum oldStatus = layBet.BetStatus;
                        layBet = SXAL.SXALKom.Instance.getBetDetail(layBet.BetId);
                        log(String.Format("Old Status was {0} and new status is {1}", oldStatus, layBet.BetStatus));
                        if (layBet.BetStatus == oldStatus && oldStatus != SXALBetStatusEnum.M)
                        {
                            playtimeRetry = true;
                            continue;
                        }

                        log("Status changed! Rechecking");
                        Lay.Bets[layBet.BetId] = layBet;

                        if (betsChangedEvent != null)
                            betsChangedEvent(this, new BetsChangedEventArgs(this));

                        _stoppable = false;

                        break;
                    }
                    

                    //TODO: Dynamische entscheidung, ob Lay Same oder Back Opposite
                    // Zu Testzwecken jetzt Trades immer Lay Opposite
                    //Marktpreise holen
                    log("Read Market Prices/Odds");
                    SXALMarketPrices prices = null;
                    double layOdds = 0.0;
                    try
                    {
                        if (!getMarketPrices(out prices, true))
                        {
                            log("Can not retrieve market prices. Retrying");
                            _stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }


                        log("Get Best Odds for Laying");

                        if (!getBestLayOdds(prices, Back.SelectionId, out layOdds))
                        {
                            log("Can not retrieve Lay Odds. Retrying!");
                            _stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }

                        if (_currentTradeOutSettings.TradeOutSettings.CheckLayOdds && layOdds >= Back.BetPrice)
                        {
                            log(String.Format("The Lay Odd {0} is larger than the Back Odd {1}. Repeating", layOdds, Back.BetPrice));
                            _stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }

                        //Prozentüberwachung, falls notwendig
                        if (_currentTradeOutSettings.TradeOutSettings.UseOddsPercentage)
                        {
                            double targetOdds = Back.BetPrice * (_currentTradeOutSettings.TradeOutSettings.HedgePercentage * 0.01);
                            targetOdds = (double)SXALKom.Instance.validateOdd((decimal)targetOdds);
                            if (layOdds > targetOdds)
                            {
                                log(String.Format("The Lay Odds of {0} are larger than the minimum expected odds of {1}. Don't hedge and repeat", layOdds, targetOdds));
                                _stoppable = true;
                                playtimeRetry = true;
                                continue;
                            }
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

                        _stoppable = true;

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

                        _stoppable = true;

                        return;
                    }
                    catch (SXALThrottleExceededException)
                    {
                        //Abfragelimit der Free API für Marktquoten und -preise wurde erreicht.
                        //Berechne Blind eine Hedgingquote ( 1/2 Einstiegsquote ) und versuche damit
                        // zu arbeiten.
                        double tmpOdds = Back.BetPrice - 1 / 2;
                        if (tmpOdds < 1) tmpOdds += 0.5;

                        layOdds = (double)SXALKom.Instance.validateOdd((decimal)tmpOdds);

                    }
                    double money = Back.BetSize - Lay.BetSize;

                    logBetAmount(String.Format("Calculating money for Lay. Existing Back Bets {0} {3} - Existing Lay Bets {1} {3} = {2} {3}",
                        Back.BetSize, Lay.BetSize, money, SXALBankrollManager.Instance.Currency));

                    if (money <= 0.0)
                    {
                        EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                        if (stopTimer != null)
                            stopTimer(this, new StopTimerEventArgs(this._match, this));
                        log("No Money needed to bet. Leaving!");
                        _stoppable = true;
                        return;
                    }

                    try
                    {
                        try
                        {
                            layBet = SXALKom.Instance.placeLayBet(_marketId, Back.SelectionId, layOdds, money);
                        }
                        catch (SXALNoBetBelowMinAllowedException)
                        {
                            log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                            layBet = SXALKom.Instance.placeLayBet(_marketId, Back.SelectionId, layOdds, SXALKom.Instance.MinStake);
                        }
                        catch (SXALInsufficientFundsException)
                        {
                            log("Not enough money on betting account. Retrying!");
                            _stoppable = true;
                            playtimeRetry = true;
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
                        _stoppable = true;
                        playtimeRetry = true;
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


                                if (betsChangedEvent != null)
                                    betsChangedEvent(this, new BetsChangedEventArgs(this));
                                _stoppable = true;
                                break;
                            }
                        case SXALBetStatusEnum.MU:
                            //_lay00.Bets.Add(layBet.betId, layBet);
                            this.addBet(layBet, false);
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));

                            if (!tradeCancelBet(betsChangedEvent, ref layBet, ref playtimeRetry))
                            {
                                continue;
                            }
                            break;                                                   
                        case SXALBetStatusEnum.U:
                            if (!tradeCancelBet(betsChangedEvent, ref layBet, ref playtimeRetry))
                            {
                                continue;
                            }
                            break;
                        default:
                            log(String.Format("Bet Status is {0}. Repeating", layBet.BetStatus.ToString()));
                            _stoppable = true;
                            playtimeRetry = true;
                            continue;
                    }

                    //Status aktualisieren;
                    this.TradeState.checkState();

                    EventHandler<StopTimerEventArgs> stopTimer2 = StopTimer;
                    if (stopTimer2 != null)
                        stopTimer2(this, new StopTimerEventArgs(this._match, this));

                    return;
                }
            }
            catch (ThreadAbortException)
            {
                EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(_match, this));

                _stoppable = true;

                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                String msg = String.Format("Market {0} Match {1}! Hedge stopped!", MarketId, Match);
                log(msg);
                if (message != null)
                {
                    message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - Over/Under"));
                }
            
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                String msg = String.Format("Market {0} Match {1}! ExpectionMessage {2}. Leaving Trade!", MarketId, Match, exc.Message);
                log(msg);
                if (message != null)
                {
                    message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - Over/Under"));
                }

                EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(_match, this));

                _stoppable = true;
            }
        }

        private void greenRunner()
        {
            try
            {
                log("Start Greening Thread");

                if (_currentTradeOutSettings.TradeOutSettings.OnlyHedge)
                {
                    log("Leaving Greening Thread because only Hedging is configured");
                    return;
                }

                TimeSpan span = new TimeSpan(0, 0, 60);
                bool playtimeRetry = false;
                SXALBet layBet = null;
                //Bestimmte Zeit warten oder auf Spielminute?
                if (_currentTradeOutSettings.TradeOutSettings.UseWaitTime &&
                    _currentTradeOutSettings.TradeOutSettings.UseGreenWaitTime)
                {
                    span = new TimeSpan(0, 0, _currentTradeOutSettings.TradeOutSettings.GreenWaitTime);
                }

                while (true)
                {
                    _stoppable = true;

                    EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                    if (_currentTradeOutSettings.TradeOutSettings.UseWaitTime &&
                        _currentTradeOutSettings.TradeOutSettings.UseGreenWaitTime)
                    {
                        EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;

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
                        //auf eine bestimmte Spielzeit warten
                        if (this.Score.Playtime < _currentTradeOutSettings.TradeOutSettings.GreenPlaytime && playtimeRetry == false)
                        {
                            log(String.Format("Wait until playtime {0} before greening. Current Playtime is {1}", _currentTradeOutSettings.TradeOutSettings.GreenPlaytime,
                                this.Score.Playtime));
                            span = new TimeSpan(0, 0, 60);
                            _stoppable = true;

                            EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;

                            if (setTimerHandler != null)
                            {
                                setTimerHandler(this, new SetTimerEventArgs(this._match, this, _currentTradeOutSettings.TradeOutSettings.GreenPlaytime.ToString()));
                            }

                            Thread.Sleep(span);
                            continue;
                        }
                        else if (playtimeRetry == true)
                        {
                            span = new TimeSpan(0, 0, 60);
                            _stoppable = true;

                            log(String.Format("Wait {0} seconds before retrying", span.TotalSeconds));

                            EventHandler<SetTimerEventArgs> setTimerHandler = SetTimer;

                            if (setTimerHandler != null)
                            {
                                setTimerHandler(this, new SetTimerEventArgs(this._match, this, span));
                            }

                            Thread.Sleep(span);
                        }
                    }

                    if (_currentTradeOutSettings.TradeOutSettings.NoTrade)
                    {
                        log(String.Format("No trading-Flag set. Rechecking in {0} sceconds", 60));
                        span = new TimeSpan(0, 0, 60);
                        _stoppable = true;
                        Thread.Sleep(span);
                        continue;
                    }

                    //TODO: Dynamisch je nachdem ob Over/Under
                    // Kein Trading mehr nötig
                    if (Score.ScoreA + Score.ScoreB >= (int)_ouValue)
                    {
                        log(String.Format("GoalSum {0} is larger than {1}. No Trading possible. Leave Hedging!", Score.ScoreA + Score.ScoreB, (int)_ouValue));
                        EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                        if (stopTimer != null)
                            stopTimer(this, new StopTimerEventArgs(this._match, this));
                        return;
                    }



                    // Wartezeit abgelaufen oder Spielzeit erreicht
                    _stoppable = false;


                    if (this.Back.Bets.Count == 0)
                    {
                        log("No Back Bets. Retrying!");
                        _stoppable = true;
                        playtimeRetry = true;
                        continue;
                    }

                    if (_liveticker == null)
                    {
                        log("No Liveticker. Retrying!");
                        _stoppable = true;
                        playtimeRetry = true;
                        continue;
                    }

                    if (layBet != null)
                    {
                        log("Rechecking an open LayBet");
                        SXALBetStatusEnum oldStatus = layBet.BetStatus;
                        layBet = SXAL.SXALKom.Instance.getBetDetail(layBet.BetId);
                        log(String.Format("Old Status was {0} and new status is {1}", oldStatus, layBet.BetStatus));
                        if (layBet.BetStatus == oldStatus && oldStatus != SXALBetStatusEnum.M)
                        {
                            playtimeRetry = true;
                            continue;
                        }

                        log("Status changed! Rechecking");
                        Lay.Bets[layBet.BetId] = layBet;

                        if (betsChangedEvent != null)
                            betsChangedEvent(this, new BetsChangedEventArgs(this));

                        _stoppable = false;
                        break;
                    }



                    //TODO: Dynamische entscheidung, ob Lay Same oder Back Opposite
                    // Zu Testzwecken jetzt Trades immer Lay Opposite
                    //Marktpreise holen
                    log("Read Market Prices/Odds");
                    SXALMarketPrices prices = null;
                    try
                    {
                        if (!getMarketPrices(out prices, false))
                        {
                            log("Can not retrieve market prices. Retrying");
                            _stoppable = true;
                            playtimeRetry = true;
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

                        _stoppable = true;

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

                        _stoppable = true;

                        return;
                    }

                    log("Get Best Odds for Laying");
                    double layOdds = 0.0;
                    if (!getBestLayOdds(prices, Back.SelectionId, out layOdds))
                    {
                        log("Can not retrieve Lay Odds. Retrying!");
                        _stoppable = true;
                        playtimeRetry = true;
                        continue;
                    }

                    if (_currentTradeOutSettings.TradeOutSettings.CheckLayOdds && layOdds >= Back.BetPrice)
                    {
                        log(String.Format("The Lay Odd {0} is larger than the Back Odd {1}. Repeating", layOdds, Back.BetPrice));
                        _stoppable = true;
                        playtimeRetry = true;
                        continue;
                    }

                    //Prozentüberwachung, falls notwendig
                    if (_currentTradeOutSettings.TradeOutSettings.UseOddsPercentage)
                    {
                        double targetOdds = Back.BetPrice * (_currentTradeOutSettings.TradeOutSettings.HedgePercentage * 0.01);
                        targetOdds = (double)SXALKom.Instance.validateOdd((decimal)targetOdds);
                        if (layOdds > targetOdds)
                        {
                            log(String.Format("The Lay Odds of {0} are larger than the minimum expected odds of {1}. Don't green and repeat", layOdds, targetOdds));
                            _stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }
                    }

                    //double money = Back.BetSize - Lay.BetSize;
                    double money = ((Back.BetSize * Back.BetPrice - Back.BetSize) - (Lay.BetSize * Lay.BetPrice - Lay.BetSize)) / layOdds;

                    logBetAmount(String.Format("Calculating money for Lay. Existing Back Bets {0} {3} - Existing Lay Bets {1} {3} = {2} {3}",
                        Back.BetSize, Lay.BetSize, money, SXALBankrollManager.Instance.Currency));

                    if (money <= 0.0)
                    {
                        EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                        if (stopTimer != null)
                            stopTimer(this, new StopTimerEventArgs(this._match, this));
                        log("No Money needed to bet. Leaving!");
                        _stoppable = true;
                        return;
                    }


                    try
                    {
                        try
                        {
                            layBet = SXALKom.Instance.placeLayBet(_marketId, Back.SelectionId, layOdds, money);
                        }
                        catch (SXALNoBetBelowMinAllowedException)
                        {
                            log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                            layBet = SXALKom.Instance.placeLayBet(_marketId, Back.SelectionId, layOdds, SXALKom.Instance.MinStake);
                        }
                        catch (SXALInsufficientFundsException)
                        {
                            log("Not enough money on betting account. Retrying!");
                            _stoppable = true;
                            playtimeRetry = true;
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
                        _stoppable = true;
                        playtimeRetry = true;
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


                                if (betsChangedEvent != null)
                                    betsChangedEvent(this, new BetsChangedEventArgs(this));
                                _stoppable = true;
                                break;
                            }
                        case SXALBetStatusEnum.MU:
                            //_lay00.Bets.Add(layBet.betId, layBet);
                            this.addBet(layBet, false);
                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(this));
                            if (!tradeCancelBet(betsChangedEvent, ref layBet, ref playtimeRetry))
                            {
                                continue;
                            }
                            break;
                        case SXALBetStatusEnum.U:
                            if (!tradeCancelBet(betsChangedEvent, ref layBet, ref playtimeRetry))
                            {
                                continue;
                            }
                            break;
                        default:
                            log(String.Format("Bet Status is {0}. Repeating", layBet.BetStatus.ToString()));
                            playtimeRetry = true;
                            continue;
                    }

                    //Status aktualisieren;
                    this.TradeState.checkState();

                    EventHandler<StopTimerEventArgs> stopTimer2 = StopTimer;
                    if (stopTimer2 != null)
                        stopTimer2(this, new StopTimerEventArgs(this._match, this));

                    return;
                }
            }
            catch (ThreadAbortException)
            {
                EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(_match, this));

                _stoppable = true;

                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                String msg = String.Format("Market {0} Match {1}! Greening stopped!", MarketId, Match);
                log(msg);
                if (message != null)
                {
                    message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - Over/Under"));
                }

            }
            catch (Exception exc)
            {
                EventHandler<SXWMessageEventArgs> message = MessageEvent;
                String msg = String.Format("Market {0} Match {1}! ExpectionMessage {2}. Leaving Trade!", MarketId, Match, exc.Message);
                log(msg);
                if (message != null)
                {
                    message(this, new SXWMessageEventArgs(DateTime.Now, Match, msg, "TradeTheReaction - Over/Under"));
                }

                EventHandler<StopTimerEventArgs> stopTimer = StopTimer;
                if (stopTimer != null)
                    stopTimer(this, new StopTimerEventArgs(_match, this));

                _stoppable = true;
            }
        }


        private bool cancelBet(ref SXALBet bet)
        {
            bool bReturn = true;
            if (!SXALKom.Instance.cancelBet(bet.BetId))
            {
                // Konnte nicht mehr stornieren.
                // Kurzen Moment warten und dan versuchen die Wette erneut zu lesen
                Thread.Sleep(5000);
                bReturn = false;
            }

            bet = SXALKom.Instance.getBetDetail(bet.BetId);

            return bReturn;
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

        /// <summary>
        /// Gibt die beste zur Verfügung stehende Quote für eine Lay Wette zurück,
        /// sofern möglich.
        /// </summary>
        /// <param name="prices">Alle Marktpreise/Quoten</param>
        /// <param name="selectionId">Die Selection ID deren Quote gelesen werden sollen</param>
        /// <param name="odds">Die gefundene Quote</param>
        /// <returns>Indikator, ob Quote gelesen werden konnten oder nicht </returns>
        private bool getBestLayOdds(SXALMarketPrices prices, long selectionId, out double odds)
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
        private bool getMarketPrices(out SXALMarketPrices prices, bool canThrowThrottleExceeded)
        {
            prices = null;
            prices = SXALKom.Instance.getMarketPrices(_marketId, canThrowThrottleExceeded);
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

        private bool tradeCancelBet(EventHandler<BetsChangedEventArgs> betsChangedEvent, ref SXALBet bet, ref bool retry)
        {
            if (!cancelBet(ref bet))
            {
                if (bet.BetStatus == SXALBetStatusEnum.M)
                {
                    Lay.Bets[bet.BetId] = bet;
                    if (betsChangedEvent != null)
                        betsChangedEvent(this, new BetsChangedEventArgs(this));
                    _stoppable = true;
                    retry = false;
                    return true;
                }
                else
                {
                    log(String.Format("Lay Bet Status is {0}. Retrying!", bet.BetStatus));
                    if (bet.BetStatus == SXALBetStatusEnum.MU)
                    {
                        Lay.Bets[bet.BetId] = bet;
                        if (betsChangedEvent != null)
                            betsChangedEvent(this, new BetsChangedEventArgs(this));

                        _stoppable = false;
                    }
                    else
                    {
                        bet = null;
                    }
                    _stoppable = true;
                    retry = true;
                    return false;
                }

            }
            else
            {
                log(String.Format("Lay Bet Status {0}. Retrying!", bet.BetStatus));
                _stoppable = true;
                retry  = true;
                return false;
            }
        }

        #region ITrade Member

        public event EventHandler<SetTimerEventArgs> SetTimer;

        public event EventHandler<StopTimerEventArgs> StopTimer;

        public event EventHandler<StateChangedEventArgs> RunningStateChanged;

        public event EventHandler<StateChangedEventArgs> TradeStateChanged;

        public event EventHandler<SXWMessageEventArgs> MessageEvent;

        public event EventHandler<GoalScoredEventArgs> GoalScoredEvent;

        public event EventHandler<PlaytimeEventArgs> PlaytimeEvent;

        public event EventHandler<net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs> GameEndedEvent;

        public event EventHandler<BetsChangedEventArgs> BetsChangedEvent;

        public net.sxtrader.bftradingstrategies.lsparserinterfaces.IScore Score
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
                    _liveticker.GameEndedEvent += new EventHandler<net.sxtrader.bftradingstrategies.lsparserinterfaces.GameEndedEventArgs>(_liveticker_GameEndedEvent);
                    _liveticker.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(_liveticker_PlaytimeTickEvent);
                    _liveticker.RaiseGoalEvent += new EventHandler<GoalEventArgs>(_liveticker_RaiseGoalEvent);
                    _liveticker.RedCardEvent += new EventHandler<RedCardEventArgs>(_liveticker_RedCardEvent);
                }
            }
        }

        public string Match
        {
            get { return _match; }
        }

        public string TeamA
        {
            get { return _teamA; }
        }

        public string TeamB
        {
            get { return _teamB; }
        }

        public string TradeTypeName
        {
            get 
            {
                String tradeTypeName = String.Empty;
                switch(_ouType)
                {
                    case OUTYPE.OVER:
                        tradeTypeName = "Over ";
                        break;
                    case OUTYPE.UNDER:
                        tradeTypeName = "Under ";
                        break;
                    default:
                        tradeTypeName = "OU Type Not Set Yet ";
                        break;
                }

                switch (_ouValue)
                {
                    case OUVALUE.ZEROFIVE:
                        tradeTypeName += " 0.5";
                        break;
                    case OUVALUE.ONEFIVE:
                        tradeTypeName += " 1.5";
                        break;
                    case OUVALUE.TWOFIVE:
                        tradeTypeName += " 2.5";
                        break;
                    case OUVALUE.THREEFIVE:
                        tradeTypeName += " 3.5";
                        break;
                   case OUVALUE.FOURFIVE:
                        tradeTypeName += " 4.5";
                        break;
                    case OUVALUE.FIVEFIVE:
                        tradeTypeName += " 5.5";
                        break;
                    case OUVALUE.SIXFIVE:
                        tradeTypeName += " 6.5";
                        break;
                    case OUVALUE.SEVENFIVE:
                        tradeTypeName += " 7.5";
                        break;
                    case OUVALUE.EIGHTFIVE:
                        tradeTypeName += " 8.5";
                        break;
                    default:
                        tradeTypeName = "OU Value Not Set Yet";
                        break;
                }

                return tradeTypeName;
            }
        }

        public long MarketId
        {
            get { return _marketId; }
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

        public TradeRunningState RunningState
        {
            get;
            set;            
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

        public uint TradeId
        {
            get { return _traderId; }
        }

        public SXALBetCollection Back
        {
            get 
            {
                if (_ouType == OUTYPE.OVER)
                    return _overBets;
                else return _underBets;
            }
        }

        public SXALBetCollection Lay
        {
            get 
            {
                if (_ouType == OUTYPE.OVER)
                    return _underBets;
                else return _overBets;
            }
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
            if (bet == null)
                return;            

            TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
            if (t != this.TradeType)
                return;

            try
            {
                switch (_ouType)
                {
                    case OUTYPE.OVER:
                        if (SXALKom.Instance.isOver(bet.SelectionId, this.Match) && bet.BetType == SXALBetTypeEnum.B)
                        {
                            if (!addBetInternal(_overBets, bet))
                                return;
                        }
                        else if (SXALKom.Instance.isOver(bet.SelectionId, this.Match) && bet.BetType == SXALBetTypeEnum.L &&
                            (_tradeOutTyp == TRADEOUTTYP.LAYSAME || _tradeOutTyp == TRADEOUTTYP.UNDECEIDED))
                        {
                            // Die Under beherbergen in diesen Fall die Lays auf das Over
                            if (!addBetInternal(_underBets, bet))
                                return;
                            _tradeOutTyp = TRADEOUTTYP.LAYSAME;
                        }
                        else if (SXALKom.Instance.isUnder(bet.SelectionId, this.Match) && bet.BetType == SXALBetTypeEnum.B &&
                            (_tradeOutTyp == TRADEOUTTYP.BACKOPPOSITE || _tradeOutTyp == TRADEOUTTYP.UNDECEIDED))
                        {
                            if (!addBetInternal(_underBets, bet))
                                return;
                            _tradeOutTyp = TRADEOUTTYP.BACKOPPOSITE;
                        }
                        else return;
                        break;
                    case OUTYPE.UNDER:
                        if (SXALKom.Instance.isUnder(bet.SelectionId, this.Match) && bet.BetType == SXALBetTypeEnum.B)
                        {
                            if (!addBetInternal(_underBets, bet))
                                return;
                        }
                        else if (SXALKom.Instance.isUnder(bet.SelectionId, this.Match) && bet.BetType == SXALBetTypeEnum.L &&
                            (_tradeOutTyp == TRADEOUTTYP.LAYSAME || _tradeOutTyp == TRADEOUTTYP.UNDECEIDED))
                        {
                            // Die Over beherben in diesen Fall die lays auf das Under
                            if (!addBetInternal(_overBets, bet))
                                return;
                            _tradeOutTyp = TRADEOUTTYP.LAYSAME;
                        }
                        else if (SXALKom.Instance.isOver(bet.SelectionId, this.Match) && bet.BetType == SXALBetTypeEnum.B &&
                            (_tradeOutTyp == TRADEOUTTYP.BACKOPPOSITE || _tradeOutTyp == TRADEOUTTYP.UNDECEIDED))
                        {
                            if (!addBetInternal(_overBets, bet))
                                return;
                            _tradeOutTyp = TRADEOUTTYP.BACKOPPOSITE;
                        }
                        break;
                }

                EventHandler<BetsChangedEventArgs> betsChangedEvent = BetsChangedEvent;
                if (betsChangedEvent != null)
                    betsChangedEvent(this, new BetsChangedEventArgs(this));

            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            if (withTradeCheck)
            {
                _tradeState.checkState();
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

        public void start()
        {
            if (Score.ScoreA + Score.ScoreB > 0)
            {
                tradeRuleCheck(TRADEOUTTRIGGER.GENERAL, true);
            }
            TradeState.checkState();
        }

        public bool hasBet(long betId)
        {
            if (_overBets.Bets.ContainsKey(betId) || _underBets.Bets.ContainsKey(betId))
            {
                return true;
            }
            return false;
        }

        #endregion

        private void log(string message)
        {
            try
            {
                TradeLog.Instance.writeLog(this._match, this.TradeTypeName, "Trader", String.Format("ID {0}: {1}", _traderId, message));
            }
            catch { }
        }

        private void logBetAmount(string message)
        {
            try
            {
                TradeLog.Instance.writeBetAmountLog(this._match, this.TradeTypeName, "Trader", String.Format("ID {0}: {1}", _traderId, message));
            }
            catch { }
        }

        #region ITrade Member


        public double getInitialStake()
        {
            double stake = 0.0;
            switch (_ouType)
            {
                case OUTYPE.OVER:
                    stake = _overBets.BetSize;
                    break;
                case OUTYPE.UNDER:
                    stake = _underBets.BetSize;
                    break;
            }
            return stake;
        }

        public double getWinnings()
        {
            double winnings = 0.0;
            switch (_ouType)
            {
                case OUTYPE.OVER:
                    switch (_tradeOutTyp)
                    {
                        case TRADEOUTTYP.UNDECEIDED:
                            winnings = _overBets.RiskWin;
                            break;
                        case TRADEOUTTYP.LAYSAME:
                            winnings = _overBets.RiskWin - _underBets.RiskWin;
                            break;
                        case TRADEOUTTYP.BACKOPPOSITE:
                            winnings = _overBets.RiskWin - _underBets.BetSize;
                            break;
                    }
                    break;
                case OUTYPE.UNDER:
                    switch (_tradeOutTyp)
                    {
                        case TRADEOUTTYP.UNDECEIDED:
                            winnings = _underBets.RiskWin;
                            break;
                        case TRADEOUTTYP.LAYSAME:
                            winnings = _underBets.RiskWin - _overBets.RiskWin;
                            break;
                        case TRADEOUTTYP.BACKOPPOSITE:
                            winnings = _underBets.RiskWin - _overBets.BetSize;
                            break;
                    }
                    break;
            }
            return winnings;

        }


        public double getPLSnapshot()
        {
            double dMoney = 0.0;

            if (Score != null && Score.ScoreA + Score.ScoreB >= (int)_ouValue)
            {
                // Over hat gewonnen
                switch (_ouType)
                {
                    case OUTYPE.OVER:
                        switch (_tradeOutTyp)
                        {
                            case TRADEOUTTYP.LAYSAME:
                                dMoney = (_overBets.BetSize * _overBets.BetPrice - _overBets.BetSize) - (_underBets.BetSize * _underBets.BetPrice - _underBets.BetSize);
                                break;
                            case TRADEOUTTYP.BACKOPPOSITE:
                                dMoney = (_overBets.BetSize * _overBets.BetPrice - _overBets.BetSize) - _underBets.BetSize;
                                break;
                            case TRADEOUTTYP.UNDECEIDED:
                                dMoney = (_overBets.BetSize * _overBets.BetPrice - _overBets.BetSize);
                                break;
                        }
                        break;
                    case OUTYPE.UNDER:
                        //Under noch implementieren
                        break;
                }
            }
            else if (Score != null)
            {
                switch (_ouType)
                {
                    case OUTYPE.OVER:
                        switch (_tradeOutTyp)
                        {
                            case TRADEOUTTYP.LAYSAME:
                                dMoney = _underBets.BetSize - _overBets.BetSize;
                                break;
                            case TRADEOUTTYP.BACKOPPOSITE:
                                dMoney = (_underBets.BetSize * _underBets.BetPrice - _underBets.BetSize) - _overBets.BetSize;
                                break;
                            case TRADEOUTTYP.UNDECEIDED:
                                dMoney = -_overBets.BetSize;
                                break;
                        }
                        break;
                    case OUTYPE.UNDER:
                        // Under noch implementieren
                        break;
                }
            }

            return dMoney;
        }
      
        public TRADETYPE TradeType
        {
            get { return _tradeType; }
        }
      

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
                        _stoppable = true;
                    }

                    if (_greenThread != null && SXThreadStateChecker.isStartedBackground(_greenThread))
                    {
                        log("Stop Greening Thread");
                        _greenThread.Abort();
                        _greenThread.Join();
                        _stoppable = true;
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
    }
}
