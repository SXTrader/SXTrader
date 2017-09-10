using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Diagnostics;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Globalization;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    public class LiveScore : IScore
    {
        private LivetickerSegment1 _segment1;
        private LivetickerSegment2 _segment2;
        private bool _pulsconnected = false;
        private int _refCounter;

        private int _playtime;        


        public LiveScore(LivetickerSegment1 segment1, LivetickerSegment2 segment2)
        {
            _segment1 = segment1;
            _segment2 = segment2;
            _segment2.GameStateChangedEvent += new EventHandler<EventArgs>(_segment2_GameStateChangedEvent);            
        }

        private void Instance_Pulse(object sender, EventArgs e)
        {
            if (this.isRunning())
            {
                log("Received Pulse - Update Playtime");
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.playtimeRunner));
            }
        }

        void _segment2_GameStateChangedEvent(object sender, EventArgs e)
        {
            switch(_segment2.GameState)
            {
                case LIVESTATES.First:
                    if (_refCounter > 0)
                    {
                        if (!_pulsconnected)
                        {
                            SXMinutePulse.Instance.Pulse += new EventHandler<EventArgs>(Instance_Pulse);
                            _pulsconnected = true;
                        }
                        log("Notifying Listerners: Game has started");
                        playtimeRunner(null);
                    }
                    break;
                case LIVESTATES.Second:
                    if (_refCounter > 0)
                    {
                        if (!_pulsconnected)
                        {
                            SXMinutePulse.Instance.Pulse += new EventHandler<EventArgs>(Instance_Pulse);
                            _pulsconnected = true;
                        }
                        log("Notifying Listerners: Game Second Half has started");
                        playtimeRunner(null);
                    }
                    break;
                case LIVESTATES.End:
                    SXMinutePulse.Instance.Pulse -= new EventHandler<EventArgs>(Instance_Pulse);
                    _pulsconnected = false;
                    EventHandler<GameEndedEventArgs> gameEndedHandler = GameEndedEvent;
                    if(gameEndedHandler != null)
                    {
                        log("Notifying Listerners: Game has Ended");
                        gameEndedHandler(this, new GameEndedEventArgs(this.TeamA, this.ScoreA, this.ScoreB));
                    }

                    break;
            }
        }

        public void update(LivetickerSegment2 segment2)
        {
            UPDATEEVENT[] events = _segment2.update(segment2);

            foreach (UPDATEEVENT singleEvent in events)
            {
                switch (singleEvent)
                {
                    case UPDATEEVENT.GOALTEAMA:
                        EventHandler<GoalEventArgs> goalHandlerA = RaiseGoalEvent;
                        if (goalHandlerA != null)
                        {
                            log(String.Format("Team A ({0}) has scored a goal. Score is now {1} - {2}", this.TeamA, this.ScoreA, this.ScoreB));
                            log("Notifying Listerners: New Score");
                            goalHandlerA(this, new GoalEventArgs(this.TeamA, this.ScoreA, this.ScoreB));
                        }
                        break;
                    case UPDATEEVENT.GOALTEAMB:
                        EventHandler<GoalEventArgs> goalHandlerB = RaiseGoalEvent;
                        if (goalHandlerB != null)
                        {
                            log(String.Format("Team B ({0}) has scored a goal. Score is now {1} - {2}", this.TeamB, this.ScoreA, this.ScoreB));
                            log("Notifying Listerners: New Score");
                            goalHandlerB(this, new GoalEventArgs(this.TeamB, this.ScoreA, this.ScoreB));
                        }
                        break;
                    case UPDATEEVENT.GOALBACKA:
                        EventHandler<GoalBackEventArgs> goalBackHandlerA = BackGoalEvent;
                        if (goalBackHandlerA != null)
                        {
                            log(String.Format("Team A ({0}) Goal canceled. Score is now {1} - {2}", this.TeamA, this.ScoreA, this.ScoreB));
                            log("Notifying Listerners: Goal canceled");
                            goalBackHandlerA(this, new GoalBackEventArgs(this.TeamA, this.ScoreA, this.ScoreB));
                        }
                        break;
                    case UPDATEEVENT.GOALBACKB:
                        EventHandler<GoalBackEventArgs> goalBackHandlerB = BackGoalEvent;
                        if (goalBackHandlerB != null)
                        {
                            log(String.Format("Team B ({0}) Goal canceled. Score is now {1} - {2}", this.TeamB, this.ScoreA, this.ScoreB));
                            log("Notifying Listerners: Goal canceled");
                            goalBackHandlerB(this, new GoalBackEventArgs(this.TeamB, this.ScoreA, this.ScoreB));
                        }
                        break;
                    case UPDATEEVENT.REDCARDA:
                        EventHandler<RedCardEventArgs> redCardHandlerA = RedCardEvent;
                        if(redCardHandlerA != null)
                        {
                            log(String.Format("Team A ({0}) Red Card. Red Card Count: Team A {1} Team B {2}", this.TeamA, this.RedA, this.RedB));
                            log("Notifying Listerners: Red Card");
                            redCardHandlerA(this, new RedCardEventArgs(this.TeamA, this.RedA, this.RedB));
                        }
                        break;
                    case UPDATEEVENT.REDCARDB:
                        EventHandler<RedCardEventArgs> redCardHandlerB = RedCardEvent;
                        if (redCardHandlerB != null)
                        {
                            log(String.Format("Team B ({0}) Red Card. Red Card Count: Team A {1} Team B {2}", this.TeamB, this.RedA, this.RedB));
                            log("Notifying Listerners: Red Card");
                            redCardHandlerB(this, new RedCardEventArgs(this.TeamB, this.RedA, this.RedB));
                        }
                        break;
                }
            }
        }

        private void playtimeRunner(Object stateInfo)
        {                        
            if (_segment2.GameState == LIVESTATES.First)
            {
                _playtime = Convert.ToInt32(DateTime.Now.Subtract(this.StartDTS).TotalMinutes);
                if (_playtime < 0)
                    _playtime = 0;
                if (_playtime > 45)
                    _playtime = 45;
            }
            else if(_segment2.GameState == LIVESTATES.Second)
            {
                _playtime = Convert.ToInt32(DateTime.Now.Subtract(this.StartDTS).TotalMinutes) + 45;
                if (_playtime < 46)
                    _playtime = 46;
                if (_playtime > 90)
                    _playtime = 90;

            }

            //log(String.Format(CultureInfo.InvariantCulture,"New Playtime is {0}", _playtime));
            
            EventHandler<PlaytimeTickEventArgs> handler = PlaytimeTickEvent;
            if (handler != null)
            {
                //log("Notifying Listerners: New Playtime");
                handler(this, new PlaytimeTickEventArgs(String.Format(CultureInfo.InvariantCulture,"Match {0} - {1}", this.TeamA, this.TeamB), _playtime));
            }
        }

        private void log(String message)
        {
            if (BetfairMatch != null)
                LiveTickerLog.Instance.writeLog(this.BetfairMatch, "liveticker1", message);
        }

        #region IScore Member

        public event EventHandler<GoalEventArgs> RaiseGoalEvent;

        public event EventHandler<GoalBackEventArgs> BackGoalEvent;

        public event EventHandler<PlaytimeTickEventArgs> PlaytimeTickEvent;

        public event EventHandler<GameEndedEventArgs> GameEndedEvent;

        public event EventHandler<RedCardEventArgs> RedCardEvent;

        public bool Ended
        {
            get
            {
                if (_segment2 != null)
                {
                    if (_segment2.GameState == LIVESTATES.End)
                        return true;
                }
                return false;
            }
        }

        public string TeamA
        {
            get { if (_segment1 != null) return _segment1.TeamA; return String.Empty; }
        }

        public string TeamB
        {
            get { if (_segment1 != null) return _segment1.TeamB; return String.Empty; }
        }

        public ulong TeamAId
        {
            get { if (_segment1 != null) return _segment1.TeamAId; return 0; }
        }

        public ulong TeamBId
        {
            get { if (_segment1 != null) return _segment1.TeamBId; return 0; }
        }

        public string League
        {
            get { if (_segment1 != null) return _segment1.League; return String.Empty; }
        }

        public int Playtime
        {
            get { return _playtime; }
        }

        public string BetfairMatch
        {
            get;

            set;
            
        }

        public int ScoreA
        {
            get { if (_segment2 != null) return _segment2.ScoreA; return -1; }
        }

        public int ScoreB
        {
            get { if (_segment2 != null) return _segment2.ScoreB; return -1; }
        }

        public int RedA
        {
            get { if (_segment2 != null) return _segment2.RedA; return 0; }
        }

        public int RedB
        {
            get { if (_segment2 != null) return _segment2.RedB; return 0; }
        }

        public DateTime StartDTS
        {
            get { if (_segment2 != null) return _segment2.StartDTS; return DateTime.MaxValue; }
        }

        public LIVESTATES MatchState
        {
            get
            {
                return _segment2.GameState;
            }
        }

        public bool isRunning()
        {
            if (_segment2 != null)
            {
                if (_segment2.GameState == LIVESTATES.First ||
                    _segment2.GameState == LIVESTATES.HT ||
                    _segment2.GameState == LIVESTATES.Second)
                    return true;
            }

            return false;
        }

        public string getScore()
        {
            if (_segment2 != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} - {1}", _segment2.ScoreA, _segment2.ScoreB);
                return sb.ToString();
            }
            return String.Empty;
        }

        public string getLiveMatch()
        {
            if(_segment1 != null)
                return String.Format(CultureInfo.InvariantCulture,"{0} - {1}", this.TeamA, this.TeamB);
            return String.Empty;
        }

        public IScore IncreaseRef()
        {
            ++_refCounter;
            if (_refCounter > 0)
            {
                if (!_pulsconnected)
                {
                    SXMinutePulse.Instance.Pulse += new EventHandler<EventArgs>(Instance_Pulse);
                    _pulsconnected = true;
                }
            }
            return this;
        }

        public void DecreaseRef()
        {
            --_refCounter;
            if (_refCounter < 0) _refCounter = 0;
            if (_refCounter == 0)
            {
                if (_pulsconnected)
                {
                    SXMinutePulse.Instance.Pulse -= new EventHandler<EventArgs>(Instance_Pulse);
                    _pulsconnected = false;
                }
            }
        }

        public SCORESTATE getScoreState()
        {
            if (_segment2 != null)
            {
                if (this.ScoreA == 0 && this.ScoreB == 0)
                    return SCORESTATE.initdraw;
                else if (this.ScoreA == this.ScoreB)
                    return SCORESTATE.draw;
                else
                    return SCORESTATE.undraw;
            }

            return SCORESTATE.initdraw;
        }

        #endregion

        public void Dispose()
        {
            if (_pulsconnected)
            {
                SXMinutePulse.Instance.Pulse -= new EventHandler<EventArgs>(Instance_Pulse);
                _pulsconnected = false;
            }
        }
    }


}
