using net.sxtrader.bftradingstrategies.bfuestrategy;
using net.sxtrader.bftradingstrategies.bfuestrategy.LayTheDraw2.Exceptions;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;
using net.sxtrader.muk.eventargs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.LayTheDraw2
{
    #region EventArgs-Klassen
    public class BFRiskWinChangedEventArgs : EventArgs
    {
        private string _match;
        private double _backGuV;
        private double _layGuV;

        public String Match
        {
            get
            {
                return _match;
            }
        }

        public double BackGuV
        {
            get
            {
                return _backGuV;
            }
        }

        public double LayGuV
        {
            get
            {
                return _layGuV;
            }
        }

        public BFRiskWinChangedEventArgs(string match, double backGuV, double layGuV)
        {
            _match = match;
            _backGuV = backGuV;
            _layGuV = layGuV;
        }
    }

    public class BFGoalScoredEventArgs : EventArgs
    {

        private int _scoreA;
        private int _scoreB;
        private String _team;

        public SCORESTATE ScoreState
        {
            get
            {
                if (_scoreA == 0 && _scoreB == 0)
                    return SCORESTATE.initdraw;
                else if (_scoreA == _scoreB)
                    return SCORESTATE.draw;
                else
                    return SCORESTATE.undraw;

            }
        }

        public String Team
        {
            get
            {
                return _team;
            }
        }

        public int ScoreA
        {
            get
            {
                return _scoreA;
            }
        }

        public int ScoreB
        {
            get
            {
                return _scoreB;
            }
        }

        public int GoalSum
        {
            get
            {
                return this.ScoreA + this.ScoreB;
            }
        }

        public String Score
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} - {1}", _scoreA, _scoreB);
                return sb.ToString();
            }
        }

        public BFGoalScoredEventArgs(string team, int scoreA, int scoreB)
        {
            _team = team;
            _scoreA = scoreA;
            _scoreB = scoreB;
        }
    }
    #endregion
    public class LayTheDraw
    {
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
        #endregion

        private Task _taskCloseTrade;
        private CancellationTokenSource _ctsCloseTrade = new CancellationTokenSource();
        private bool _closeTradeUnstoppable = false;
        private Task _taskStopLoss;

        private HLLiveScore _score;
        private LTDConfigurationRW _config;

        private int _scoreA;
        private int _scoreB;
        private String _teamA = String.Empty;
        private String _teamB = String.Empty;
        private int _tradeId;
        private SXALBetCollection _betBack = new SXALBetCollection();
        private SXALBetCollection _betLay = new SXALBetCollection();

        /// <summary>
        /// Falls aus irgendeinen Grund es notwendig sein sollte, alles Wetten zu einen Markt 
        /// erneut zu lesen, so wird dieses Kennzeichnen gesetzt.
        /// </summary>
        private bool _dirtyFlag;

        #region Konstanten
        private const double LOWTOLERANCE = -0.1;
        private const double HIGHTOLERANCE = 0.1;
        #endregion

        #region Attribute

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

                logBetAmount(String.Format("Evaluating Trade state. The Difference between Profit/Loss Undraw and Profit/Loss Draw is {0}", diff));

                SETSTATE state = SETSTATE.UNSETTELED;
                if (LOWTOLERANCE < diff && diff < HIGHTOLERANCE)
                {
                    state = SETSTATE.SETTLED;
                    log("Trade state is settled because diff is within tolerance");
                }
                else
                {
                    //if (laybackwin > backlaywin)
                    if (drawPL > undrawPL)
                    {
                        state = SETSTATE.SETTLED;
                        log("Trade state is settled because the difference of Winning Back and Risk of Lay is greater than the"
                            + "difference of  Size of Lay and Size of Back");
                    }
                    else
                    {
                        /*
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

        public SXALBetCollection Back
        {
            get
            {
                return _betBack;
            }
            set
            {
                _betBack = value;
            }
        }

        public SXALBetCollection Lay
        {
            get
            {
                return _betLay;
            }
            set
            {
                _betLay = value;
            }
        }

        public LTDConfigurationRW Configuration
        {
            get
            {
                return _config;
            }
            set
            {
                LTDConfigurationRW oldConfig = _config;
                _config = value;

                if (oldConfig != null && _config != null)
                {
                    if (oldConfig.StrategyActivated == true && _config.StrategyActivated == false)
                        ;//doDeactivation();
                    else if (oldConfig.StrategyActivated == false && _config.StrategyActivated == true)
                        ;// doActivation();
                }
            }
        }

        public String TeamA { get { return _teamA; } }
        public String TeamB { get { return _teamB; } }
        public String Match 
        {
            get
            {
                return String.Format("{0} - {1}", this.TeamA, this.TeamB);
            }
        }


        public HLLiveScore Score
        {
            get { return _score; }
            set 
            {
                bool scoreChanged = false;

                //bisherige Ereignishandler entfernen
                removeScoreEventHandlers();

                _score = value;
                if (_score != null)
                {
                    log("Connect a combined liveticker");
                    log(String.Format("Combined Liveticker. Liveticker 1 {0}. Liveticker 2 {1}", _score.IsScore1Connected(),
                        _score.IsScore2Connected()));

                    if (_score.ScoreA != _scoreA)
                    {
                        log(String.Format("Connect a Combined Liveticker. Score Team A has changed from {0} to {1}. Total Score is now {2}",
                            _scoreA, _score.ScoreA, this.Score.getScore()));
                        _scoreA = _score.ScoreA;
                        scoreChanged = true;
                    }

                    if (_score.ScoreB != _scoreB)
                    {
                        log(String.Format("Connect a Combined Liveticker. Score Team B has changed from {0} to {1}. Total Score is now {2}",
                            _scoreB, _score.ScoreB, this.Score.getScore()));
                        _scoreB = _score.ScoreB;
                        scoreChanged = true;
                    }

                    // Ereignishandler setzen
                    setScoreEventHandlers();


                    if (scoreChanged)
                    {
                        // Hier Tradeout initieren
                    }
                }
                else
                {
                    log("Combined Liveticker was deleted");
                }
            }

        }

        
        #endregion

        private void removeScoreEventHandlers()
        {
            if (_score != null)
            {
                _score.BackGoalEvent -= _score_BackGoalEvent;
                _score.GameEndedEvent -= _score_GameEndedEvent;
                _score.LiveScoreAddedEvent -= _score_LiveScoreAddedEvent;
                _score.LiveScoreRemovedEvent -= _score_LiveScoreRemovedEvent;
                _score.PlaytimeTickEvent -= _score_PlaytimeTickEvent;
                _score.RaiseGoalEvent -= _score_RaiseGoalEvent;
                _score.RedCardEvent -= _score_RedCardEvent;
            }
        }

        private void setScoreEventHandlers()
        {
            if (_score != null)
            {
                _score.BackGoalEvent += _score_BackGoalEvent;
                _score.GameEndedEvent += _score_GameEndedEvent;
                _score.LiveScoreAddedEvent += _score_LiveScoreAddedEvent;
                _score.LiveScoreRemovedEvent += _score_LiveScoreRemovedEvent;
                _score.PlaytimeTickEvent += _score_PlaytimeTickEvent;
                _score.RaiseGoalEvent += _score_RaiseGoalEvent;
                _score.RedCardEvent += _score_RedCardEvent;
            }

        }

        public void CancelCloseTrade()
        {
            _ctsCloseTrade.Cancel();
        }

        /// <summary>
        /// Kopf des Prozesses des austradens. Hier werden die globalen Faktoren überprüft und auf Ablauf der Wartezeit gewartet.
        /// Folgende Elemente werden durchgeführt:
        /// <list type="number">
        /// <item>Ist eine Austradezeit definiert? Falls nicht erfolgt kein austraden</item>
        /// <item>Wurde vom Aufrufer ein Abbruch des austradens verlangt?</item>
        /// <item>Informieren der Subscriber über die Countdownzeit</item>
        /// <item>Informiere der Subscriber über Abbruch des Countdowns</item>
        /// <item>Abwarten der Wartezeit</item>
        /// </list>
        /// </summary>
        /// <param name="ct"></param>
        private async Task closeTradeHead(TimeSpan span, CancellationToken ct)
        {
            //Überprüfe, ob Task storniert wurde
            if (ct.IsCancellationRequested)
            {
                //Informiere Subscriber darüber, dass der Countdown angehalten wurde
                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                if (stopHandler != null)
                {
                    log("Close Trade: Inform Listeners: Stop Close Trade Countdown");
                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                }
                ct.ThrowIfCancellationRequested();
            }
            //Erneutes Überprüfen vor dem Abwarten des Zeitraumes.
            if (_config.GoalScoredCloseTradeSeconds == -1)
            {
                log("No Close Trade after goal configured! Leaving");
                return;
            }

            //Informiere die Subscriber über die Dauer des Wartens
            _closeTradeUnstoppable = false;
            EventHandler<SXSetCloseTradeTimer> openBetHandler = SetCloseTradeTimer;
            if (openBetHandler != null)
            {
                log("Close Trade: Notifying Listeners: Start Close Trade Countdown");
                openBetHandler(this, new SXSetCloseTradeTimer(this.Match, span));
            }

            //TODO: Überprüfe, ob Strategie aktiv geschaltet

            log(String.Format("Close Trade: Wait for {0} seconds", span.TotalSeconds));

            try
            {
                await Task.Delay(span, ct);
            }
            catch (TaskCanceledException)
            {
            }
            //Überprüfe, ob Task storniert wurde
            if (ct.IsCancellationRequested)
            {
                //Informiere Subscriber darüber, dass der Countdown angehalten wurde
                EventHandler<SXStopCloseTradeTimer> stopHandler = StopCloseTradeTimer;
                if (stopHandler != null)
                {
                    log("Close Trade: Inform Listeners: Stop Close Trade Countdown");
                    stopHandler(this, new SXStopCloseTradeTimer(this.Match));
                }
                ct.ThrowIfCancellationRequested();
            }

            _closeTradeUnstoppable = true;

            //Jetzt befinden wir uns in der Abschlußschleife und die Wartezeit ist nun nicht mehr 
            //die Initiale, wie nach Empfangen des Tores definiert, sondern die angegebenen Wiederholungszeit pro Schleife
            span = new TimeSpan(0, 0, _config.DontCloseRetrySeconds);
        }

        private async Task closeTrade(CancellationToken ct)
        {
            // Were we already canceled?
            ct.ThrowIfCancellationRequested();
            
            
            log("Starting a Close Trade");
            // Überprüfe im Vorfeld, ob überhaupt ein Tradeabschluß nach einen Tor gewünscht ist.
            // Dies ist notwendig, da ansonsten die Erstellung der abzuwartenten Zeitspanne wegen
            // des Wertes -1 auf einen Fehler läuft.
            if (_config.GoalScoredCloseTradeSeconds == -1)
            {
                log("No Close Trade after goal configured! Leaving");
                return;
            }

            TimeSpan span = new TimeSpan(0, 0, _config.GoalScoredCloseTradeSeconds);
            while (true)
            {
                await closeTradeHead(span, ct);   
                
                //Falls keine Lay-Wetten vorhanden sind, kann es kein Lay The Draw sein
                if(this.Lay.Bets.Count == 0)
                {
                    //Ausnahme auslösen
                    throw new NoLTDTradeException(String.Format("Match {0} is not an Lay The Draw Trade", this.Match));
                }
                //Alle Wetten nachladen
                SXALMUBet[] bets = SXALKom.Instance.getBetsMU(this.Lay.MarketId);
                if (bets != null && bets.Length > 0)
                    updateBets(bets);

                //Status des Trades nochmal überprüfen

            }


        }

        void _score_RedCardEvent(object sender, RedCardEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async void _score_RaiseGoalEvent(object sender, GoalEventArgs e)
        {
            try
            {
                await closeTrade(_ctsCloseTrade.Token);
            }
            catch (OperationCanceledException)
            {
                log("Close Trade was successfully cancelled");
            }
            /*
            // 1. Überprüfe, bereits ein Task mit einen Close-Trade läuft.
            if (_taskCloseTrade != null && _taskCloseTrade.Status != TaskStatus.Canceled && _taskCloseTrade.Status != TaskStatus.Faulted
                && _taskCloseTrade.Status != TaskStatus.RanToCompletion)
            {
                //Noch stornierbar?
                

            }
            */

            //throw new NotImplementedException();
        }

        void _score_PlaytimeTickEvent(object sender, PlaytimeTickEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _score_LiveScoreRemovedEvent(object sender, LiveScoreRemovedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _score_LiveScoreAddedEvent(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void _score_GameEndedEvent(object sender, GameEndedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _score_BackGoalEvent(object sender, GoalBackEventArgs e)
        {
            throw new NotImplementedException();
        }


        public LayTheDraw()
        {
           
        }

        private void log(string message)
        {
            try
            {
                TradeLog.Instance.writeLog(this.Match, "LayTheDraw", "Trader", String.Format("ID {0}: {1}", _tradeId, message));
            }
            catch { }
        }

        public void logBetAmount(string message)
        {
            try
            {
                TradeLog.Instance.writeBetAmountLog(this.Match, "LayTheDraw", "Trader", String.Format("ID {0}: {1}", _tradeId, message));
            }
            catch { }
        }

        private void updateBets(SXALMUBet[] bets)
        {
            foreach(SXALMUBet bet in bets)
            {
                if(bet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, this.Lay.MarketId))
                {
                    log(String.Format("Bet {0} is not a Draw-Bet. Ignoring", bet.BetId));
                    continue;
                }
                SXALBetCollection tempColl = null;
                //Falls Back-Wette Sammlung der Backwetten heranziehen, ansonsten die der LAy-Wetten
                if(bet.BetType == SXALBetTypeEnum.B)
                {
                    tempColl = this.Back;
                }
                else
                {
                    tempColl = this.Lay;
                }

                //Ist die Wette noch nicht bekannt?
                if (!tempColl.Bets.ContainsKey(bet.BetId))
                {
                    SXALBet betDetail = SXALKom.Instance.getBetDetail(bet.BetId);
                    if (betDetail != null)
                    {
                        tempColl.Bets.Add(betDetail.BetId, betDetail);
                    }
                    //Benachrichtigungne auslösen
                    EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                    if (handler != null)
                    {
                        double backWin = Back.RiskWin - Lay.RiskWin;
                        double backLost = Lay.BetSize - Back.BetSize;
                        handler(this, new BFRiskWinChangedEventArgs(this.Match, Math.Round(backWin, 2), Math.Round(backLost, 2)));
                    }
                }
                else
                {
                    SXALBet betDetail = SXALKom.Instance.getBetDetail(bet.BetId);
                    //Hat sich am Status oder Größe der Wette geändert?                    
                    if(betDetail.BetStatus != this.Back.Bets[bet.BetId].BetStatus ||
                        betDetail.MatchedSize != this.Back.Bets[bet.BetId].MatchedSize ||
                        betDetail.RemainingSize != this.Back.Bets[bet.BetId].RemainingSize)
                    {
                        this.Back.Bets[bet.BetId] = betDetail;
                        //Benachrichtigungne auslösen
                        EventHandler<BFRiskWinChangedEventArgs> handler = RiskWinChangedEvent;
                        if (handler != null)
                        {
                            double backWin = Back.RiskWin - Lay.RiskWin;
                            double backLost = Lay.BetSize - Back.BetSize;
                            handler(this, new BFRiskWinChangedEventArgs(this.Match, Math.Round(backWin, 2), Math.Round(backLost, 2)));
                        }
                    }
                }
            }
        }

    }
}
