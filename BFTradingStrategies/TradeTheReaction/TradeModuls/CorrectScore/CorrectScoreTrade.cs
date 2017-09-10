using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Threading;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.eventargs;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore
{
    

    partial class CorrectScoreTrade : ITrade
    {
        

        private String    _match;
        private long       _marketId;
        private String    _teamA;
        private String    _teamB;

        private SXALBetCollection _backBets;
        private SXALBetCollection _layBets;
        
        private uint               _traderId;
        private IScore             _liveticker;
        private TradeMode          _tradeMode;
        private TradeMoneyState    _tradeState;
        private TTRConfigurationRW _config;
        private Thread             _hedgeThread;
        private Thread             _greenThread;
        private TTRTradeOutCheck   _currentTradeOutSettings;
        private bool _stoppable = true;
        private bool _disposed = false;

        private object _livertickerLock = "livertickerLock";

        public TRADETYPE TradeType
        {
            get;
            private set;
        }

        public CorrectScoreTrade(SXALBetCollection bets, IScore liveticker, String match, TTRConfigurationRW config, TRADETYPE tradeType)
        {
            this.TradeType = tradeType;

            _match = match;

            _livertickerLock = "livetickerLock" + _match;
            splitMatchName(_match);

            _traderId = TradingIdsGetter.nextId();

            log(String.Format("Constructing a new {0} Trade", this.TradeTypeName));

            if (liveticker == null)
            {
                log("No liveticker given for Correct Score Trading");
                throw new NullReferenceException("No liveticker given for Correct Score Trading");
            }

            log("Attaching Liveticker to Correct Score");
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

            _backBets = new SXALBetCollection();
            _layBets = new SXALBetCollection();

            log(String.Format("Attaching {0} Bets to Corrrect Score", bets.Bets.Count));
            foreach (SXALBet bet in bets.Bets.Values)
            {
                
                TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
                if (t != this.TradeType)
                    continue;

                if (bet.BetType == SXALBetTypeEnum.B)
                {
                    _marketId = bet.MarketId;
                    _backBets.Bets.Add(bet.BetId, bet);
                }

                if (bet.BetType == SXALBetTypeEnum.L)
                {
                    _marketId = bet.MarketId;
                    _layBets.Bets.Add(bet.BetId, bet);
                }
            }

            getInner();

            _hedgeThread = new Thread(_tradeMode.hedgeRunner);
            _hedgeThread.IsBackground = true;
            _greenThread = new Thread(_tradeMode.greenRunner);
            _greenThread.IsBackground = true;

            RunningState = new TradeRunningCreadedState(this);
            RunningState.StateChanged += new EventHandler<StateChangedEventArgs>(RunningState_StateChanged);
            TradeState = new TradeMoneyCreatedState(this);
            TradeState.StateChanged += new EventHandler<StateChangedEventArgs>(TradeState_StateChanged);
            TradeState.checkState();

            _config = config;

            if (_config != null &&_config.TradeOutRules.TradeType == TRADETYPE.UNASSIGNED)
                _config.TradeOutRules.TradeType = this.TradeType;

            

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
                String tradeTypeName = TradeTheReaction.strCorrectScore;

                switch (this.TradeType)
                {
                    case TRADETYPE.SCORELINE01BACK:
                        tradeTypeName += " 0 - 1 Back";
                        break;
                    case TRADETYPE.SCORELINE01LAY:
                        tradeTypeName += " 0 - 1 Lay";
                        break;
                    case TRADETYPE.SCORELINE02BACK:
                        tradeTypeName += " 0 - 2 Back";
                        break;
                    case TRADETYPE.SCORELINE02LAY:
                        tradeTypeName += " 0 - 2 Lay";
                        break;
                    case TRADETYPE.SCORELINE03BACK:
                        tradeTypeName += " 0 - 3 Back";
                        break;
                    case TRADETYPE.SCORELINE03LAY:
                        tradeTypeName += " 0 - 3 Lay";
                        break;
                    case TRADETYPE.SCORELINE10BACK:
                        tradeTypeName += " 1 - 0 Back";
                        break;
                    case TRADETYPE.SCORELINE10LAY:
                        tradeTypeName += " 1 - 0 Lay";
                        break;
                    case TRADETYPE.SCORELINE11BACK:
                        tradeTypeName += " 1 - 1 Back";
                        break;
                    case TRADETYPE.SCORELINE11LAY:
                        tradeTypeName += " 1 - 1 Lay";
                        break;
                    case TRADETYPE.SCORELINE12BACK:
                        tradeTypeName += " 1 - 2 Back";
                        break;
                    case TRADETYPE.SCORELINE12LAY:
                        tradeTypeName += " 1 - 2 Lay ";
                        break;
                    case TRADETYPE.SCORELINE13BACK:
                        tradeTypeName += " 1 - 3 Back";
                        break;
                    case TRADETYPE.SCORELINE13LAY:
                        tradeTypeName += " 1 - 3 Lay";
                        break;
                    case TRADETYPE.SCORELINE20BACK:
                        tradeTypeName += " 2 - 0 Back";
                        break;
                    case TRADETYPE.SCORELINE20LAY:
                        tradeTypeName += " 2 - 0 Lay";
                        break;
                    case TRADETYPE.SCORELINE21BACK:
                        tradeTypeName += " 2 - 1 Back";
                        break;
                    case TRADETYPE.SCORELINE21LAY:
                        tradeTypeName += " 2 - 1 Lay";
                        break;
                    case TRADETYPE.SCORELINE22BACK:
                        tradeTypeName += " 2 - 2 Back";
                        break;
                    case TRADETYPE.SCORELINE22LAY:
                        tradeTypeName += " 2 - 2 Lay";
                        break;
                    case TRADETYPE.SCORELINE23BACK:
                        tradeTypeName += " 2 - 3 Back";
                        break;
                    case TRADETYPE.SCORELINE23LAY:
                        tradeTypeName += " 2 - 3 Lay";
                        break;
                    case TRADETYPE.SCORELINE30BACK:
                        tradeTypeName += " 3 - 0 Back";
                        break;
                    case TRADETYPE.SCORELINE30LAY:
                        tradeTypeName += " 3 - 0 Lay";
                        break;
                    case TRADETYPE.SCORELINE31BACK:
                        tradeTypeName += " 3 - 1 Back";
                        break;
                    case TRADETYPE.SCORELINE31LAY:
                        tradeTypeName += " 3 - 1 Lay";
                        break;
                    case TRADETYPE.SCORELINE32BACK:
                        tradeTypeName += " 3 - 2 Back";
                        break;
                    case TRADETYPE.SCORELINE32LAY:
                        tradeTypeName += " 3 - 2 Lay";
                        break;
                    case TRADETYPE.SCORELINE33BACK:
                        tradeTypeName += " 3 - 3 Back";
                        break;
                    case TRADETYPE.SCORELINE33LAY:
                        tradeTypeName += " 3 - 3 Lay";
                        break;
                    case TRADETYPE.SCORELINEOTHERBACK:
                        tradeTypeName += " Other Back";
                        break;
                    case TRADETYPE.SCORELINEOTHERLAY:
                        tradeTypeName += " Other Lay";
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
            get { return _backBets; }
        }

        public SXALBetCollection Lay
        {
            get { return _layBets; }
        }

        public void addBet(SXALBet bet, bool withTradeCheck)
        {
            if (bet == null)
                return;            

            TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
            if (t != this.TradeType && t != TTRHelper.GetReverseTradeType(this.TradeType))
                return;

            try
            {
                if (bet.BetType == SXALBetTypeEnum.B)
                {
                    if (!addBetInternal(_backBets, bet))
                        return;
                }
                else
                {
                    if (!addBetInternal(_layBets, bet))
                        return;
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

        public double getInitialStake()
        {
            return _tradeMode.getInitialStake();
        }

        public double getWinnings()
        {
            return _tradeMode.getWinnings();
        }

        public double getPLSnapshot()
        {
            return _tradeMode.getPLSnapshot();
        }

        public bool isSettled()
        {
            if (_config.OnlyHedge && this.TradeState.GetType() == typeof(TradeMoneyHedgedState))
                return true;
            else if (this.TradeState.GetType() == typeof(TradeMoneyGreenedState))
                return true;
            return false;
        }

        public void start()
        {
            //throw new NotImplementedException();
        }

        public bool hasBet(long betId)
        {
            if(_backBets.Bets.ContainsKey(betId) || _layBets.Bets.ContainsKey(betId))
            {
                return true;
            }
            return false;
        }

        #endregion

        private void splitMatchName(String match)
        {
            String[] sSeps = { " - ", " v " };

            String[] teams = match.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);

            _teamA = teams[0].Trim();
            _teamB = teams[teams.GetLength(0) - 1].Trim();
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
        /// Überprüfung einer Wette nach einer BetInProgressException
        /// </summary>
        /// <param name="betId"></param>
        /// <returns></returns>
        private SXALBet checkBIP(long betId)
        {
            log(String.Format("Rechecking bet {0} after a BetInProgress Exception", betId));
            SXALBet bet = null;
            TimeSpan span = new TimeSpan(0, 0, 10);
            while (true)
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
                // Konnte nicht mehr stornieren.
                // Kurzen Moment warten und dan versuchen die Wette erneut zu lesen
                Thread.Sleep(5000);
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
                    _stoppable = true;
                    retry = false;
                    return true;
                }
                else
                {
                    log(String.Format("Lay Bet Status is {0}. Retrying!", bet.BetStatus));
                    if (bet.BetStatus == SXALBetStatusEnum.MU)
                    {
                        addBet(bet, false);

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
                retry = true;
                return false;
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

        private void _liveticker_PlaytimeTickEvent(object sender, PlaytimeTickEventArgs e)
        {
            lock (_livertickerLock)
            {
                //log("Received a Playtime Event. Reevaluating Trade Out Rules");
                tradeRuleCheck(TRADEOUTTRIGGER.PLAYTIME, false);

                EventHandler<PlaytimeEventArgs> handler = PlaytimeEvent;
                if (handler != null)
                    handler(this, new PlaytimeEventArgs(this.Match, e.Tick));
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

        private void _liveticker_RedCardEvent(object sender, RedCardEventArgs e)
        {
            lock (_livertickerLock)
            {
                log("Received a Red Card Event. Reevaluating Trade Out Rules");
                tradeRuleCheck(TRADEOUTTRIGGER.REDCARD, false);
            }
        }

        private void TradeState_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.OldState.ToString().Equals(e.NewState.ToString()))
                return;

            log(String.Format("Trade State has changed from {0} to {1}", e.OldState.ToString(), e.NewState.ToString()));
            Type type = e.NewState.GetType();
            this.TradeState = (TradeMoneyState)e.NewState;
            TradeState.StateChanged += new EventHandler<StateChangedEventArgs>(TradeState_StateChanged);

            
            // Nur starten, wenn folgende Kriterien erfüllt sind
            // 1. Es muss eine Livescoreanbindung existieren
            // 2. Der Torstand muss noch einen Trade zulassen
            // 3. Die Begegnung muss gestartet sein
            //Sonderfall Other
            bool bContinue = false;
            if (this.TradeType == TRADETYPE.SCORELINEOTHERBACK ||
                this.TradeType == TRADETYPE.SCORELINEOTHERLAY)
            {
                if (this.Score.ScoreA <= 3 && this.Score.ScoreB <= 3)
                    bContinue = true;
            }
            else
            {
                if (this.Score.ScoreA <= CSTradeTypeToScoresList.GetScoreA(this.TradeType) &&
                this.Score.ScoreB <= CSTradeTypeToScoresList.GetScoreB(this.TradeType))
                    bContinue = true;
            }


            if( this.Score != null && bContinue &&
                this.Score.StartDTS < DateTime.Now)
            {
                if (type == typeof(TradeMoneyCreatedState))
                {
                }
                else if (type == typeof(TradeMoneyOpenState))
                {
                    if (!SXThreadStateChecker.isStartedBackground(_hedgeThread) && _currentTradeOutSettings != null)
                    {
                        _hedgeThread = new Thread(_tradeMode.hedgeRunner);
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
                        _greenThread = new Thread(_tradeMode.greenRunner);
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

        private void RunningState_StateChanged(object sender, StateChangedEventArgs e)
        {
            // Nach aussen weiterreichen
            EventHandler<StateChangedEventArgs> handler = RunningStateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void getInner()
        {
            switch (this.TradeType)
            {
                case TRADETYPE.SCORELINE01BACK:
                case TRADETYPE.SCORELINE02BACK:
                case TRADETYPE.SCORELINE03BACK:
                case TRADETYPE.SCORELINE10BACK:
                case TRADETYPE.SCORELINE11BACK:
                case TRADETYPE.SCORELINE12BACK:
                case TRADETYPE.SCORELINE13BACK:
                case TRADETYPE.SCORELINE20BACK:
                case TRADETYPE.SCORELINE21BACK:
                case TRADETYPE.SCORELINE22BACK:
                case TRADETYPE.SCORELINE23BACK:
                case TRADETYPE.SCORELINE30BACK:
                case TRADETYPE.SCORELINE31BACK:
                case TRADETYPE.SCORELINE32BACK:
                case TRADETYPE.SCORELINE33BACK:
                case TRADETYPE.SCORELINEOTHERBACK:
                    _tradeMode = new BackMode(this);
                    break;
                case TRADETYPE.SCORELINE01LAY:
                case TRADETYPE.SCORELINE02LAY:
                case TRADETYPE.SCORELINE03LAY:
                case TRADETYPE.SCORELINE10LAY:
                case TRADETYPE.SCORELINE11LAY:
                case TRADETYPE.SCORELINE12LAY:
                case TRADETYPE.SCORELINE13LAY:
                case TRADETYPE.SCORELINE20LAY:
                case TRADETYPE.SCORELINE21LAY:
                case TRADETYPE.SCORELINE22LAY:
                case TRADETYPE.SCORELINE23LAY:
                case TRADETYPE.SCORELINE30LAY:
                case TRADETYPE.SCORELINE31LAY:
                case TRADETYPE.SCORELINE32LAY:
                case TRADETYPE.SCORELINE33LAY:
                    _tradeMode = new LayMode(this);
                    break;
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

            // Als erstes Schau einmal, was für eine Regel gelten würde

            if (_config == null)
                return;

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

                _tradeMode.cancelOpenBets();                
                _currentTradeOutSettings = null;
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
                
                _tradeMode.cancelOpenBets();
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
                    _hedgeThread = new Thread(_tradeMode.hedgeRunner);
                    _hedgeThread.IsBackground = true;
                    _hedgeThread.Start();
                }
            }
            else if (type == typeof(TradeMoneyHedgedState))
            {
                if (!_currentTradeOutSettings.TradeOutSettings.OnlyHedge && !SXThreadStateChecker.isStartedBackground(_greenThread))
                {
                    _greenThread = new Thread(_tradeMode.greenRunner);
                    _greenThread.IsBackground = true;
                    _greenThread.Start();
                }
            }

        }

        private void recheckBets()
        {
            try
            {
                //List der Wetten aufbauen
                SXALBetCollection tmp = new SXALBetCollection();
                foreach (SXALBet b in _backBets.Bets.Values)
                {
                    tmp.Bets.Add(b.BetId, b);
                }

                foreach (SXALBet b in _layBets.Bets.Values)
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
                    

                    if (tmp.Bets.ContainsKey(bet.BetId))
                    {
                        tmp.Bets.Remove(bet.BetId);
                    }

                    TRADETYPE t = TTRHelper.GetTradeTypeByBetAndSelection(bet);
                    if (t != this.TradeType && t != TTRHelper.GetReverseTradeType(this.TradeType))
                        continue;

                    addBet(bet, false);

                }

                //Alle Wetten, die jetzt noch vorhandne sind, sind weder matched noch unmatched, also gesondert überprüfen.
                foreach (SXALBet b in tmp.Bets.Values)
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


        public TRADEMODE TradeMode
        {
            get { return _tradeMode.TradeModeEnum; }
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

    abstract internal class TradeMode
    {
        internal abstract void hedgeRunner();
        internal abstract void greenRunner();
        internal abstract double getInitialStake();
        internal abstract double getWinnings();
        internal abstract double getPLSnapshot();
        /// <summary>
        /// Storniert alle offene Gegenwetten
        /// </summary>
        internal abstract void cancelOpenBets();
        internal abstract TRADEMODE TradeModeEnum { get; }

    }
}
