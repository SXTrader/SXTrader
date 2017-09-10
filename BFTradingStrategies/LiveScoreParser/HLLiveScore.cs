using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Globalization;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    /// <summary>
    /// Zusammenfassung der einzelnen Livescores in einen HighLevel-Livescore
    /// </summary>
    public class HLLiveScore : IScore
    {
        private IScore _score1; // erster Livescore
        private IScore _score2; // zweiter Livescore
        /*
        private String _teamA;
        private String _teamB;
        private ulong _idTeamA;
        private ulong _idTeamB;
         * */
        private int _scoreA;
        private int _scoreB;
        private int _redA;
        private int _redB;
        private int _playtime;
        private string _xchangeMatch;
        private DateTime _startTime = DateTime.MaxValue;

        private object _lockPlaytime = new object();
        private object _lockEnded = new object();
        private object _lockGoal = new object();
        private object _lockRedCard = new object();
        private object _lockLivetickerConnect = new object();

        public event EventHandler<LiveScoreRemovedEventArgs> LiveScoreRemovedEvent;
        public event EventHandler<EventArgs> LiveScoreAddedEvent;

        public delegate void AsyncMethodCaller(bool connecteLiveticker2);

        public IScore Score1
        {
            get
            {
                return _score1;
            }
            set
            {
                log("Setting new Liveticker 1");
                Boolean bScoreChanged = false;
                Boolean bRedCardsChanged = false;
                if (_score1 != null)
                {
                    _score1.DecreaseRef();
                    _score1 = null;
                }

                _score1 = value;
                if (_score1 != null)
                {

                    _lockEnded = "ended" + _score1.getLiveMatch();
                    _lockGoal = "goal" + _score1.getLiveMatch();
                    _lockPlaytime = "playtime" + _score1.getLiveMatch(); ;
                    _lockRedCard = "redcard"  +_score1.getLiveMatch();

                    _score1.RaiseGoalEvent += new EventHandler<GoalEventArgs>(RaiseGoalEventHandler);
                    _score1.BackGoalEvent += new EventHandler<GoalBackEventArgs>(BackGoalEventHandler);
                    _score1.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEventHandler);
                    _score1.GameEndedEvent += new EventHandler<GameEndedEventArgs>(GameEndedEventHandler);
                    _score1.RedCardEvent += new EventHandler<RedCardEventArgs>(RedCardEventHandler);
                    _score1.IncreaseRef();

                    if (_scoreA != _score1.ScoreA)
                    {
                        bScoreChanged = true;
                        _scoreA = _score1.ScoreA;
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} scores has changed while setting Liveticker 1. New Score: {1}", this.TeamA, this.getScore()));
                    }

                    if (_scoreB != _score1.ScoreB)
                    {
                        bScoreChanged = true;
                        _scoreB = _score1.ScoreB;
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} scores has changed while setting Liveticker 1. New Score: {1}", this.TeamB, this.getScore()));
                    }

                    if (_redA != _score1.RedA)
                    {
                        bRedCardsChanged = true;
                        _redA = _score1.RedA;
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} Red Cards has changed while setting Liveticker 1. New Red Cards: {1}", this.TeamA, this.RedA));
                    }

                    if (_redB != _score1.RedB)
                    {
                        bRedCardsChanged = true;
                        _redB = _score1.RedB;
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} Red Cards has changed while setting Liveticker 1. New Red Cards: {1}", this.TeamB, this.RedB));
                    }

                    _startTime = _score1.StartDTS;

                    if (bScoreChanged)
                    {
                        // Falls Stand ungleich 0 - 0 event auslösen
                        if (getScoreState() != SCORESTATE.initdraw)
                        {
                            EventHandler<GoalEventArgs> handler = RaiseGoalEvent;
                            if (handler != null)
                            {
                                log("Notifying Listerners: New Score while setting Liveticker 1");
                                handler(this, new GoalEventArgs(String.Empty, _scoreA, _scoreB));
                            }
                        }
                    }

                    if (bRedCardsChanged)
                    {
                        EventHandler<RedCardEventArgs> handler = RedCardEvent;
                        if (handler != null)
                        {
                            log("Notifying Listerners: New Red Cards while setting Liveticker 1");
                            handler(this, new RedCardEventArgs(String.Empty, _redA, _redB));
                        }
                    }
                }
                else
                {
                    log("Liveticker 1 was deleted");
                }
            }
        }



        public IScore Score2
        {
            get
            {
                return _score2;
            }
            set
            {
                log("Setting new Liveticker 2");
                Boolean bScoreChanged = false;
                Boolean bRedCardsChanged = false;
                if (_score2 != null)
                {
                    _score2.DecreaseRef();
                    _score2 = null;
                }

                _score2 = value;
                if (_score2 != null)
                {
                    if (_score1 == null)
                    {
                        _lockEnded = "ended" + _score2.getLiveMatch();
                        _lockGoal = "goal"  +_score2.getLiveMatch();
                        _lockPlaytime = "playtime" + _score2.getLiveMatch(); ;
                        _lockRedCard = "redcard"  +_score2.getLiveMatch();
                    }
                    _score2.RaiseGoalEvent += new EventHandler<GoalEventArgs>(RaiseGoalEventHandler);
                    _score2.BackGoalEvent += new EventHandler<GoalBackEventArgs>(BackGoalEventHandler);
                    _score2.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEventHandler);
                    _score2.GameEndedEvent += new EventHandler<GameEndedEventArgs>(GameEndedEventHandler);
                    _score2.RedCardEvent += new EventHandler<RedCardEventArgs>(RedCardEventHandler);
                    _score2.IncreaseRef();

                    if (_scoreA != _score2.ScoreA)
                    {
                        bScoreChanged = true;
                        _scoreA = _score2.ScoreA;
                        log(String.Format(CultureInfo.InvariantCulture,"Team {0} Score has changed while setting Liveticker 2. New Score: {1}", this.TeamA, this.getScore()));
                    }

                    if (_scoreB != _score2.ScoreB)
                    {
                        bScoreChanged = true;
                        _scoreB = _score2.ScoreB;
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} Score has changed while setting Liveticker 2. New Score: {1}", this.TeamB, this.getScore()));
                    }

                    if (_redA != _score2.RedA)
                    {
                        bRedCardsChanged = true;
                        _redA = _score2.RedA;
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} Red Cards has changed while setting Liveticker 2. New Red Cards: {1}", this.TeamA, this.RedA));
                    }

                    if (_redB != _score2.RedB)
                    {
                        bRedCardsChanged = true;
                        _redB = _score2.RedB;
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} Red Cards has changed while setting Liveticker 2. New Red Cards: {1}", this.TeamB, this.RedB));
                    }

                    if (bScoreChanged)
                    {
                        // Falls Stand ungleich 0 - 0 event auslösen
                        if (getScoreState() != SCORESTATE.initdraw)
                        {
                            EventHandler<GoalEventArgs> handler = RaiseGoalEvent;
                            if (handler != null)
                            {
                                log("Notifying Listerners: New Score while setting Liveticker 2");
                                handler(this, new GoalEventArgs(String.Empty, _scoreA, _scoreB));
                            }
                        }
                    }

                    if (bRedCardsChanged)
                    {
                        EventHandler<RedCardEventArgs> handler = RedCardEvent;
                        if (handler != null)
                        {
                            log("Notifying Listerners: New Red Cards while setting Liveticker 2");
                            handler(this, new RedCardEventArgs(String.Empty, _redA, _redB));
                        }
                    }
                }
                else
                {
                    log("Liveticker 2 was deleted");
                }
            }
        }

        public HLLiveScore(IScore score1, IScore score2)
        {
            LiveScoreParser.Instance.LiveScoreRemovedEvent += new EventHandler<LiveScoreRemovedEventArgs>(Liveticker1_LiveScoreRemovedEvent);
            LiveScore2Parser.Instance.LiveScoreRemovedEvent += new EventHandler<LiveScoreRemovedEventArgs>(Liveticker2_LiveScoreRemovedEvent);

            LiveScoreParser.Instance.LiveScoreAddedEvent += Instance_LiveScoreAddedEvent;
            LiveScore2Parser.Instance.LiveScoreAddedEvent += Instance_LiveScore2AddedEvent;

            _score1 = score1;
            _score2 = score2;

            if (_score1 != null)
            {
                _lockEnded = "ended" + _score1.getLiveMatch();
                _lockGoal = "goal"  +_score1.getLiveMatch();
                _lockPlaytime = "playtime" + _score1.getLiveMatch(); ;
                _lockRedCard = "redcard" +_score1.getLiveMatch();

                _score1.RaiseGoalEvent += new EventHandler<GoalEventArgs>(RaiseGoalEventHandler);
                _score1.BackGoalEvent += new EventHandler<GoalBackEventArgs>(BackGoalEventHandler);
                _score1.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEventHandler);
                _score1.GameEndedEvent += new EventHandler<GameEndedEventArgs>(GameEndedEventHandler);
                _score1.RedCardEvent += new EventHandler<RedCardEventArgs>(RedCardEventHandler);
                _score1.IncreaseRef();

                // Spielstand setzen
                _scoreA = _score1.ScoreA;
                _scoreB = _score1.ScoreB;

                // Rote Karten
                _redA = _score1.RedA;
                _redB = _score1.RedB;

                _startTime = _score1.StartDTS;
            }

            if (_score2 != null)
            {
                if (_score1 == null)
                {
                    _lockEnded = "ended" + _score2.getLiveMatch();
                    _lockGoal = "goal"  +_score2.getLiveMatch();
                    _lockPlaytime = "playtime" + _score2.getLiveMatch(); ;
                    _lockRedCard = "redcard"  +_score2.getLiveMatch();
                }

                _score2.RaiseGoalEvent += new EventHandler<GoalEventArgs>(RaiseGoalEventHandler);
                _score2.BackGoalEvent += new EventHandler<GoalBackEventArgs>(BackGoalEventHandler);
                _score2.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(PlaytimeTickEventHandler);
                _score2.GameEndedEvent += new EventHandler<GameEndedEventArgs>(GameEndedEventHandler);
                _score2.RedCardEvent += new EventHandler<RedCardEventArgs>(RedCardEventHandler);
                _score2.IncreaseRef();

                if (_score1 == null)
                {
                    // Spielstand setzen
                    if (_scoreA != _score2.ScoreA)
                    {
                        _scoreA = _score2.ScoreA;
                    }

                    if (_scoreB != _score2.ScoreB)
                    {
                        _scoreB = _score2.ScoreB;
                    }

                    //Torstand setzen
                    if (_redA != _score2.RedA)
                        _redA = _score2.RedA;

                    if (_redB != _score2.RedB)
                        _redB = _score2.RedB;
                }
                 
            }

            // Falls Stand ungleich 0 - 0 event auslösen
            if (getScoreState() != SCORESTATE.initdraw)
            {
                EventHandler<GoalEventArgs> handler = RaiseGoalEvent;
                if (handler != null)
                    handler(this, new GoalEventArgs(String.Empty, _scoreA, _scoreB));
            }

            if (_redA != 0 || _redB != 0)
            {
                EventHandler<RedCardEventArgs> handler = RedCardEvent;
                if (handler != null)
                    handler(this, new RedCardEventArgs(String.Empty, _redA, _redB));
            }

            _lockPlaytime = "lockPlaytime" + this.getLiveMatch();
            _lockRedCard = "lockRedCard" + this.getLiveMatch();
            _lockGoal = "lockGoal" + this.getLiveMatch();
            _lockEnded = "lockEnded" + this.getLiveMatch();
        }

        private void Instance_LiveScore2AddedEvent(object sender, LiveScoreAddedEventArgs e)
        {
                //Falls schon ein Liveticker verbunden wurde, ist es nicht mehr nötig erneut zu prüfen.
                if (this._score2 != null)
                    return;

                this.connectLivetickerWorker(true);
                //AsyncMethodCaller caller = new AsyncMethodCaller(this.connectLivetickerWorker);
                //caller.BeginInvoke(true, new AsyncCallback(CallbackMethod), null);

                //connectLivetickerWorker(true);
                //ThreadPool.QueueUserWorkItem(this.connectLivetickerWorker, true);
        }

        private void Instance_LiveScoreAddedEvent(object sender, LiveScoreAddedEventArgs e)
        {
            
            //Falls schon ein Liveticker verbunden wurde, ist es nicht mehr nötig erneut zu prüfen.
            if (this._score1 != null)
                return;

            this.connectLivetickerWorker(false);

            //AsyncMethodCaller caller = new AsyncMethodCaller(this.connectLivetickerWorker);
            //caller.BeginInvoke(false, new AsyncCallback(CallbackMethod), null);
                //connectLivetickerWorker(false);
                //ThreadPool.QueueUserWorkItem(this.connectLivetickerWorker, false);
            
        }

        static void CallbackMethod(IAsyncResult ar)
        {
            AsyncResult result = (AsyncResult)ar;
            AsyncMethodCaller caller = (AsyncMethodCaller)result.AsyncDelegate;
            
            ar.AsyncWaitHandle.Close();

            caller.EndInvoke(ar);
        }

        private void connectLivetickerWorker(bool connectLT2)
        {            
            if (String.IsNullOrEmpty(this.BetfairMatch))
                return;

            String[] teams = this.BetfairMatch.Split(new string[] {" - "}, StringSplitOptions.RemoveEmptyEntries);

            if (teams.Length < 2)
            {
                DebugWriter.Instance.WriteMessage("HLLiveScore", String.Format("Can't resolve team names for match {0}", this.BetfairMatch));
                return;
            }

            DebugWriter.Instance.WriteMessage("HLLiveticker", String.Format("HLLiveticker: Retreived new ticker for  sport exchange match {0} - {1}", teams[0], teams[1]));
            DateTime dtsStart = DateTime.Now;
            lock (_lockLivetickerConnect)
            {
                if (connectLT2)
                {
                    if (_score2 != null)
                        return;
                    IScore score2 = LiveScore2Parser.Instance.linkSportExchange(teams[0], teams[1]);
                    if (score2 != null)
                    {
                        DebugWriter.Instance.WriteMessage("HLLiveScore", String.Format("Found match for Liveticker 2. Now linking Match {0} to Liveticker 2 {1} Start Date {2}", this.BetfairMatch, score2.getLiveMatch(), score2.StartDTS));
                        this.Score2 = score2;

                        EventHandler<EventArgs> ltAdded = LiveScoreAddedEvent;
                        if (ltAdded != null)
                        {
                            ltAdded(this, new EventArgs());
                        }
                    }
                }
                else
                {
                    if (_score1 != null)
                        return;
                    IScore score1 = LiveScoreParser.Instance.linkSportExchange(teams[0], teams[1]);
                    if (score1 != null)
                    {
                        DebugWriter.Instance.WriteMessage("HLLiveScore", String.Format("Found match for Liveticker 1. Now linking Match {0} to Liveticker 1 {1} Start Date {2}", this.BetfairMatch, score1.getLiveMatch(), score1.StartDTS));
                        this.Score1 = score1;

                        EventHandler<EventArgs> ltAdded = LiveScoreAddedEvent;
                        if (ltAdded != null)
                        {
                            ltAdded(this, new EventArgs());
                        }
                    }
                }
            }
            DateTime dtsEnd = DateTime.Now;

            DebugWriter.Instance.WriteMessage("HLLiveticker", String.Format("Time need to connect broadcast new HL-ticker for sport exchange match {0} - {1}: {2}", teams[0], teams[1], dtsEnd.Subtract(dtsStart))); 
        }

        public Boolean IsScore1Connected()
        {
            return _score1 == null ? false : true;
        }

        public Boolean IsScore2Connected()
        {
            return _score2 == null ? false : true;
        }

        #region EventHandler

        private void Liveticker2_LiveScoreRemovedEvent(object sender, LiveScoreRemovedEventArgs e)
        {
            if (e.Liveticker == this.Score2)
            {
                log("Liveticker 2 has been removed");
                this.Score2.Dispose();
                this.Score2 = null;
                if (this.Score1 == null && this.Score2 == null)
                {
                    EventHandler<LiveScoreRemovedEventArgs> handlerRemoved = LiveScoreRemovedEvent;
                    if (handlerRemoved != null)
                    {
                        log("Notifying Listerners: Livescores have been removed completely");
                        handlerRemoved(this, new LiveScoreRemovedEventArgs(this));
                    }
                }
            }
        }

        private void Liveticker1_LiveScoreRemovedEvent(object sender, LiveScoreRemovedEventArgs e)
        {
            if (e.Liveticker == this.Score1)
            {
                log("Liveticker 1 has been removed");
                if (this.Score1 != null)
                {
                    this.Score1.Dispose();
                    this.Score1 = null;
                }
                if (this.Score1 == null && this.Score2 == null)
                {
                    EventHandler<LiveScoreRemovedEventArgs> handlerRemoved = LiveScoreRemovedEvent;
                    if (handlerRemoved != null)
                    {
                        log("Notifying Listerners: Livescores have been removed completely");
                        handlerRemoved(this, new LiveScoreRemovedEventArgs(this));
                    }
                }
            }
        }

        private void RedCardEventHandler(object sender, RedCardEventArgs e)
        {
            lock (_lockRedCard)
            {
                try
                {
                    if (_redA == e.RedA && _redB == e.RedB)
                        return;
                    int redAOld = _redA;
                    int redBOld = _redB;
                    _redA = e.RedA;
                    _redB = e.RedB;

                    log("Receiving New Red Card from subordinated Livescore.");
                    EventHandler<RedCardEventArgs> handler = RedCardEvent;
                    if (handler != null)
                    {
                        if (_score1 != null)
                        {
                            if (_redA < redAOld)
                            {
                                log(String.Format(CultureInfo.InvariantCulture, "Team {0} Red Card: Old Red Card {0}. New Red Card {1}", redAOld, this.RedA));
                                log("Notifying Listerners: Red Card");
                                handler(this, new RedCardEventArgs(_score1.TeamA, _redA, _redB));
                            }
                            else
                            {
                                log(String.Format(CultureInfo.InvariantCulture, "Team {0} Red Card: Old Red Card {0}. New Red Card {1}", redBOld, this.RedB));
                                log("Notifying Listerners: Red Card");
                                handler(this, new RedCardEventArgs(_score1.TeamB, _redA, _redB));
                            }
                        }
                        else
                        {
                            log("Notifying Listerners: Red Card. Unspecified");
                            handler(this, e);
                        }
                    }

                }
                catch (NullReferenceException nre)
                {
                    ExceptionWriter.Instance.WriteException(nre);
                }
            }
        }

        private void GameEndedEventHandler(object sender, GameEndedEventArgs e)
        {
            lock (_lockEnded)
            {
                try
                {
                    if (_score1 != null)
                    {
                        if (String.IsNullOrEmpty(_score1.TeamA))
                            return;
                    }

                    log("Receiving Game has Ended from subordinated Liveticker");
                    EventHandler<GameEndedEventArgs> handler = GameEndedEvent;
                    if (handler != null)
                    {
                        if (_score1 != null)
                        {
                            log("Notifying Listerners: Game has ended");
                            handler(this, new GameEndedEventArgs(_score1.TeamA, _scoreA, _scoreB));
                        }
                        else
                        {
                            log("Notifying Listerners: Game has ended. Unspecified");
                            handler(this, e);
                        }
                    }
                }
                catch (NullReferenceException nre)
                {
                    ExceptionWriter.Instance.WriteException(nre);
                }
            }
            
        }

        private void PlaytimeTickEventHandler(object sender, PlaytimeTickEventArgs e)
        {
            lock (_lockPlaytime)
            {
                try
                {
                    log("Receiving new playtime from subordinated Liveticker");
                    if (e.Tick > _playtime)
                    {
                        int oldPlaytime = _playtime;

                        if (_score1 != null && _score2 != null)
                            if (_score1.isRunning() && _score2.isRunning())
                            {
                                if (_score1.MatchState != _score2.MatchState)
                                {
                                    if (_score1.MatchState == LIVESTATES.Second)
                                        _playtime = _score1.Playtime;
                                    else if (_score2.MatchState == LIVESTATES.Second)
                                        _playtime = _score2.Playtime;
                                    else
                                        _playtime = _score1.Playtime < _score2.Playtime ? _score1.Playtime : _score2.Playtime;

                                }
                                else
                                {
                                    _playtime = _score1.Playtime < _score2.Playtime ? _score1.Playtime : _score2.Playtime;
                                }
                            }
                            else
                                _playtime = e.Tick;
                        else
                            _playtime = e.Tick;
                        //log(String.Format("Old Playtime {0}. New Playtime {1}", _playtime, e.Tick));
                        if (oldPlaytime >= _playtime)
                            return;

                        log(String.Format(CultureInfo.InvariantCulture, "Playtime: {0}", _playtime));

                        EventHandler<PlaytimeTickEventArgs> handler = PlaytimeTickEvent;
                        if (handler != null)
                        {
                            if (_score1 != null)
                            {
                                log("Notifying Listerners: New Playtime");
                                handler(this, new PlaytimeTickEventArgs(String.Format(CultureInfo.InvariantCulture, "Match {0} - {1}", _score1.TeamA, _score1.TeamB), _playtime));
                            }
                            else
                            {
                                log("Notifying Listerners: New Playtime. Unspecified");
                                handler(this, e);
                            }
                        }
                    }
                    else
                    {
                        log(String.Format(CultureInfo.InvariantCulture, "Playtime hasn't changed. Playtime is {0}", _playtime));
                        return;
                    }
                }
                catch (NullReferenceException nre)
                {
                    ExceptionWriter.Instance.WriteException(nre);
                }
            }
        }

        private void BackGoalEventHandler(object sender, GoalBackEventArgs e)
        {
            lock (_lockGoal)
            {
                try
                {
                    log("Receiving Goal canceled from subordinated Liveticker");
                    if (_scoreA == e.ScoreA && _scoreB == e.ScoreB)
                    {
                        log("No changement: Goal canceled");
                        return;
                    }
                    int scoreAOld = _scoreA;
                    int scoreBOld = _scoreB;
                    _scoreA = e.ScoreA;
                    _scoreB = e.ScoreB;
                    log(String.Format(CultureInfo.InvariantCulture, "Goal canceled. Old Score {0} - {1}. New Score {2}", scoreAOld, scoreBOld, this.getScore()));
                    EventHandler<GoalBackEventArgs> handler = BackGoalEvent;
                    if (handler != null)
                    {
                        if (_score1 != null)
                        {
                            log("Notifying Listerners: Goal canceled");
                            if (_scoreA < scoreAOld)
                                handler(this, new GoalBackEventArgs(_score1.TeamA, _scoreA, _scoreB));
                            else
                                handler(this, new GoalBackEventArgs(_score1.TeamB, _scoreA, _scoreB));
                        }
                        else
                        {
                            log("Notifying Listerners: Goal canceled. Unspecified");
                            handler(this, e);
                        }
                    }
                }
                catch (NullReferenceException nre)
                {
                    ExceptionWriter.Instance.WriteException(nre);
                }
            }
        }

        private void RaiseGoalEventHandler(object sender, GoalEventArgs e)
        {
            lock (_lockGoal)
            {
                try
                {
                    log("Receiving New Score from subordinated liveticker");
                    if (_scoreA == e.ScoreA && _scoreB == e.ScoreB)
                    {
                        log("No changement: New Score");
                        return;
                    }


                    int scoreAOld = _scoreA;
                    int scoreBOld = _scoreB;


                    if (e.ScoreA > _scoreA)
                    {
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} scored a goal. Old Score is {1}. New Score is now {2}", this.TeamA, this.getScore(), e.Score));
                    }
                    else if (e.ScoreB > _scoreB)
                    {
                        log(String.Format(CultureInfo.InvariantCulture, "Team {0} scored a goal. Old Score is {1}. New Score is now {2}", this.TeamB, this.getScore(), e.Score));
                    }
                    else
                    {
                        log(String.Format(CultureInfo.InvariantCulture, "Reported Score {0} is lower than actual Score {1} and therefore invalid. Leaving", this.getScore(), e.Score));
                    }

                    _scoreA = e.ScoreA;
                    _scoreB = e.ScoreB;






                    EventHandler<GoalEventArgs> handler = RaiseGoalEvent;
                    if (handler != null)
                    {
                        if (_score1 != null)
                        {
                            log("Notifying Listerners: New Score");
                            if (_scoreA > scoreAOld)
                                handler(this, new GoalEventArgs(_score1.TeamA, _scoreA, _scoreB));
                            else
                                handler(this, new GoalEventArgs(_score1.TeamB, _scoreA, _scoreB));
                        }
                        else
                        {
                            log("Notifying Listerners: New Score. Unspecified");
                            handler(this, e);
                        }
                    }
                }
                catch (NullReferenceException nre)
                {
                    ExceptionWriter.Instance.WriteException(nre);
                }
            }
        }
        #endregion
        #region IScore Member

        public event EventHandler<GoalEventArgs> RaiseGoalEvent;

        public event EventHandler<GoalBackEventArgs> BackGoalEvent;

        public event EventHandler<PlaytimeTickEventArgs> PlaytimeTickEvent;

        public event EventHandler<GameEndedEventArgs> GameEndedEvent;

        public int Playtime
        {
            get
            {
                return _playtime;
            }
        }


        public bool Ended
        {
            get {
                bool bEnded = false;
                if (_score1 != null && _score2 != null)
                {
                    bEnded = _score1.Ended || _score2.Ended;
                }
                else if (_score1 != null)
                {
                    if (_score1.Ended)
                        bEnded = true;
                }
                else if (_score2 != null)
                {
                    if (_score2.Ended)
                        bEnded = true;
                }
                return bEnded;
            }
        }

        public string TeamA
        {
            get {
                if (_score1 != null)
                    return _score1.TeamA;
                if (_score2 != null)
                    return _score2.TeamA;
                return String.Empty; 
            }
        }

        public string TeamB
        {
            get
            {
                if (_score1 != null)
                    return _score1.TeamB;
                if (_score2 != null)
                    return _score2.TeamB;
                return String.Empty;
            }
        }

        public ulong TeamAId
        {
            get {
                if (_score1 != null)
                    return _score1.TeamAId;
                if (_score2 != null)
                    return _score2.TeamAId;
                return 0;
            }
        }

        public ulong TeamBId
        {
            get
            {
                if (_score1 != null)
                    return _score1.TeamBId;
                if (_score2 != null)
                    return _score2.TeamBId;
                return 0;
            }
        }

        public string League
        {
            get {
                if (_score1 != null)
                    return _score1.League;
                return String.Empty;
            }
        }

        public int ScoreA
        {
            get { return _scoreA; }
        }

        public int ScoreB
        {
            get { return _scoreB; }
        }

        public DateTime StartDTS
        {
            get { return _startTime; }
        }

        public String BetfairMatch 
        { 
            get 
            { 
                if (_score1 != null) 
                    return _score1.BetfairMatch; 
                if (_score2 != null) 
                    return _score2.BetfairMatch; 
                return _xchangeMatch; 
            }
            set 
            { 
                if (_score1 != null) 
                    _score1.BetfairMatch = value; 
                if (_score2 != null) 
                    _score2.BetfairMatch = value;
                _xchangeMatch = value;

                if(!String.IsNullOrEmpty(_xchangeMatch))
                    _lockLivetickerConnect = "livetickerconnect" + _xchangeMatch;
            }
        }

        public string getScore()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} - {1}", _scoreA, _scoreB);
            return sb.ToString();
        }

        public string getLiveMatch()
        {
            return String.Format(CultureInfo.InvariantCulture,"{0} - {1}", this.TeamA, this.TeamB);
        }

        public IScore IncreaseRef()
        {
            return this;
        }

        public void DecreaseRef()
        {
            if (_score1 != null)
                _score1.DecreaseRef();
            if (_score2 != null)
                _score2.DecreaseRef();
        }

        public SCORESTATE getScoreState()
        {
            if (_scoreA == 0 && _scoreB == 0)
                return SCORESTATE.initdraw;
            else if (_scoreA == _scoreB)
                return SCORESTATE.draw;
            else
                return SCORESTATE.undraw;
        }

        public bool isRunning()
        {
            if (_score1 != null && _score2 != null)
                return _score1.isRunning() || _score2.isRunning();
            if (_score1 != null)
                return _score1.isRunning();
            if (_score2 != null)
                return _score2.isRunning();

            return false;
        }
        #endregion

        #region IScore Member


        public event EventHandler<RedCardEventArgs> RedCardEvent;

        public int RedA
        {
            get { return _redA; }
        }

        public int RedB
        {
            get { return _redB; }
        }

        public LIVESTATES MatchState
        {
            get
            {
                if (_score1 != null)
                    return _score1.MatchState;
                else
                    return _score2.MatchState;
                   
            }
        }

        #endregion

        private void log(String message)
        {
            if (BetfairMatch != null)
                LiveTickerLog.Instance.writeLog(this.BetfairMatch, "livetickerCombined", message);
        }

        public void Dispose()
        {
            if (_score1 != null)
            {
                _score1.Dispose();
            }

            if (_score2 != null)
            {
                _score2.Dispose();
            }
        }
    }
}
