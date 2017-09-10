using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Xml;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    enum MATCHSTATE { FIRSTHALF = 2, HT = 3, SECONDHALF = 4, ENDED = 5 };
    class LiveScore2 : IScore
    {
        private int _matchId;
        private int _leagueId;
        private int _leagueIdParent;
        private int _sid;
        private int _kid;
        private int _lnr;
        private DateTime _startTime;
        private int _c1;
        private int _c2;
        private int _sma;
        private int _h0;
        private int _h1;
        private int _h2;
        private int _h3;
        private int _g0;
        private int _g1;
        private int _g2;
        private int _g3;
        private ulong _hid;
        private ulong _gid;
        private int _scoreA = 0;
        private int _scoreB = 0;
        private String _teamA;
        private String _teamB;
        private String _hrc; //Heimmannschaft Rote Karte String: Spielzeit + Name Spieler
        private int _hrci; //Heimmannschaft Rote Kart Zähler
        private String _grc; //Gastmannschaft Rote Karte String: Spielzeit + Name Spieler
        private int _grci; //Gastmannschaft Rote Karte Zähler
        private String _mstan;
        private String _hstan;
        private String _gstan;
        private String _golI;
        private String _l;
        private String _a;
        private String _league;

        private int m_iRefCounter;
        
        private int m_playtime;
        private bool m_stopPlaytime = false;
        private bool _pulsconnected = false;
        private LIVESTATES m_isStart = LIVESTATES.Eighth;

        private object _lockThreadStarter = "lockThreadStarter";

        #region IScore Member

        public event EventHandler<GoalEventArgs> RaiseGoalEvent;

        public event EventHandler<GoalBackEventArgs> BackGoalEvent;

        public event EventHandler<PlaytimeTickEventArgs> PlaytimeTickEvent;

        public event EventHandler<GameEndedEventArgs> GameEndedEvent;

        public bool Ended
        {
            get
            {
                return this.MatchState == LIVESTATES.End;
            }
            
        }

        public string TeamA
        {
            get { return _teamA; }
        }

        public string TeamB
        {
            get { return _teamB; }
        }

        public ulong TeamAId { get { return _hid;  } }
        public ulong TeamBId { get { return _gid; } }

        public string League
        {
            get { return _league; }
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

        public String BetfairMatch { get; set; }

        public string getScore()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} - {1}", _scoreA, _scoreB);
            return sb.ToString();
        }

        public string getLiveMatch()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} - {1}", this.TeamA, this.TeamB);
        }

        public IScore IncreaseRef()
        {
            m_iRefCounter++;
            lock (_lockThreadStarter)
            {
                // Playtimethreadüberprüfung               
            }

            
            return this;
        }

        public void DecreaseRef()
        {
            m_iRefCounter--;
            if (m_iRefCounter < 0)
                m_iRefCounter = 0;

            // Falls kein Subscriber mehr vorhanden, dann Playtimethread stoppen
            if (m_iRefCounter == 0)
            {               
            }

            
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
            if (m_isStart == LIVESTATES.First || m_isStart == LIVESTATES.HT || m_isStart == LIVESTATES.Second)
                return true;
            return false;
        }

        public int Playtime { get { return m_playtime; } }

        public LIVESTATES MatchState
        {
            get
            {
                switch ((MATCHSTATE)_sid)
                {
                    case MATCHSTATE.FIRSTHALF:
                        return LIVESTATES.First;
                    case MATCHSTATE.HT:
                        return LIVESTATES.HT;
                    case MATCHSTATE.SECONDHALF:
                        return LIVESTATES.Second;
                    case MATCHSTATE.ENDED:
                        return LIVESTATES.End;
                    default:
                        return LIVESTATES.Eigthteenth;
                }
            }
        }

        #endregion

        public int ID
        {
            get
            {
                return _matchId;
            }
        }

        public LiveScore2(XmlNode node)
        {
            XmlAttribute attribute = node.Attributes["MId"];
            _matchId = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _matchId = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["LId"];
            _leagueId = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _leagueId = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["_LId"];
            _leagueIdParent = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _leagueIdParent = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["SId"];
            _sid = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _sid = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["KId"];
            _kid = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _kid = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["LNr"];
            _lnr = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _lnr = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["C0"];
            int startTimeSeconds = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                startTimeSeconds = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
                DateTime dts1970 = new DateTime(1970, 1, 1);
                TimeSpan spanToAdd = new TimeSpan(0, 0, startTimeSeconds);
                DateTime dtsStartUtc = dts1970.Add(spanToAdd);
                TimeZoneInfo tziLocal = TimeZoneInfo.Local;
                _startTime = TimeZoneInfo.ConvertTimeFromUtc(dtsStartUtc, tziLocal);                
            }

            attribute = node.Attributes["C1"];
            _c1 = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _c1 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["C2"];
            _c2 = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _c2 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
                if(_sid == (int)MATCHSTATE.SECONDHALF)
                    _startTime = _startTime.AddSeconds(_c2);
            }

            attribute = node.Attributes["SMA"];
            _sma = -1;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _sma = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["H0"];
            _h0 = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _h0 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }
            _scoreA = _h0;

            attribute = node.Attributes["H1"];
            _h1 = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _h1 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["H2"];
            _h2 = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _h2 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["H3"];
            _h3 = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _h3 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["G0"];
            _g0 = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _g0 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }
            _scoreB = _g0;

            attribute = node.Attributes["G1"];
            _g1 = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _g1 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["G2"];
            _g2 = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _g2 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["G3"];
            _g3 = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _g3 = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["HId"];
            _hid = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _hid = UInt32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["GId"];
            _gid = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _gid = UInt32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["HN"];
            _teamA = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _teamA = attribute.Value;
            }

            attribute = node.Attributes["GN"];
            _teamB = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _teamB = attribute.Value;
            }

            attribute = node.Attributes["HRC"];
            _hrc = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _hrc = attribute.Value;
            }

            attribute = node.Attributes["HRCi"];
            _hrci = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _hrci = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["GRC"];
            _grc = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _grc = attribute.Value;
            }

            attribute = node.Attributes["GRCi"];
            _grci = 0;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _grci = Int32.Parse(attribute.Value, CultureInfo.InvariantCulture);
            }

            attribute = node.Attributes["MStan"];
            _mstan = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _mstan = attribute.Value;
            }

            attribute = node.Attributes["HStan"];
            _hstan = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _hstan = attribute.Value;
            }

            attribute = node.Attributes["GStan"];
            _gstan = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _gstan = attribute.Value;
            }

            attribute = node.Attributes["GolI"];
            _golI = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _golI = attribute.Value;
            }

            attribute = node.Attributes["L"];
            _l = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _l = attribute.Value;
            }

            attribute = node.Attributes["A"];
            _a = String.Empty;
            if (attribute != null && attribute.Value.Length != 0)
            {
                _a = attribute.Value;
            }

            //mhe 09.11.11
            //Neue Logik für Tor Alternative über Z-Knoten auch möglich?
            attribute = node.Attributes["S1"];
            if(attribute != null && attribute.Value.Length != 0)
            {
                String[] scoreStrings = attribute.Value.Split(new char[] {'-'});
                try
                {
                    if (!Int32.TryParse(scoreStrings[0], out _scoreA))
                        _scoreA = 0;
                    if (!Int32.TryParse(scoreStrings[1], out _scoreB))
                        _scoreB = 0;
                }
                catch
                {
                    //Alles i.O., wenn Score nicht nummerisch, dann beachten wir ihn nicht.
                    ;
                }
            }
            
            _lockThreadStarter = "lockThreadStarter" + this.ID;
        }

        void Instance_Pulse(object sender, EventArgs e)
        {
            if (this.isRunning())
            {
                log("Received Pulse - Update Playtime");
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.updatePlaytime));
            }
        }

        /*
        public void updateLivescore(XmlDocument document)
        {
            log(String.Format("Updateing Livescore. Updatestring {0}", document.InnerXml));
            XmlElement elem = document.DocumentElement;

           

            XmlAttribute attribute = elem.Attributes["SId"];
            if (attribute != null)
            {
                updateMatchState(attribute.Value);
            }

            attribute = elem.Attributes["H0"];
            if (attribute != null)
            {
                updateHomeScore(attribute.Value);
            }

            attribute = elem.Attributes["G0"];
            if (attribute != null)
            {
                updateGuestScore(attribute.Value);
            }

            attribute = elem.Attributes["HRCi"];
            if (attribute != null)
            {
                updateHomeRedCard(attribute.Value);
            }

            attribute = elem.Attributes["GRCi"];
            if (attribute != null)
            {
                updateGuestRedCard(attribute.Value);
            }

            //mhe 09.11.11
            //Neue Logik für Tor Alternative über Z-Knoten auch möglich?
            attribute = elem.Attributes["S1"];
            if (!String.IsNullOrEmpty(attribute.Value))
            {
                String[] scoreStrings = attribute.Value.Split(new char[] { '-' });
                try
                {
                    updateHomeScore(scoreStrings[0]);
                    updateGuestScore(scoreStrings[1]);
                }
                catch
                {
                    //Alles i.O., wenn Score nicht nummerisch, dann beachten wir ihn nicht.
                    ;
                }
            }
        }
        */

        public void updateLivescore(MATCHSTATE livestate, int live_a, int live_b, int red_a, int red_b, DateTime startTime)
        {
            //log("Updateing Livescore");
            if (_startTime != startTime)
            {
                log(String.Format(CultureInfo.InvariantCulture, "Starttime has changed. Old Starttime: {0}. New Starttime: {1}", _startTime, startTime));
                _startTime = startTime;
            }

            if (_sid != (int)livestate)
            {
                updateMatchState(livestate);
            }

            // Tor für Team a gefallen oder Zurückgenommen
            if (live_a != _scoreA)
            {
                updateHomeScore(live_a.ToString(CultureInfo.InvariantCulture));               

            }            

            // Tor für Team b gefallen oder zurück genommen
            if (live_b != _scoreB)
            {
                updateGuestScore(live_b.ToString(CultureInfo.InvariantCulture));

            }            

            // Rote Karte für Team A
            if (red_a != _hrci)
            {
                updateHomeRedCard(red_a.ToString(CultureInfo.InvariantCulture));
            }

            if (red_b != _grci)
            {
                updateGuestRedCard(red_b.ToString(CultureInfo.InvariantCulture));
            }
        }

        private void updateGuestRedCard(String value)
        {
            if (value.Length == 0)
                return;
            int redB = Int32.Parse(value, CultureInfo.InvariantCulture);
            if (redB != _grci)
            {
                _grci = redB;
                log(String.Format(CultureInfo.InvariantCulture, "Team {0} Red Card: {1}", this.TeamB, this.RedB));
                EventHandler<RedCardEventArgs> handler = RedCardEvent;
                if (handler != null)
                {
                    log("Notifying Listerners: Red Card");
                    handler(this, new RedCardEventArgs(_teamB, _hrci, _grci));
                }
            }
        }

        private void updateHomeRedCard(String value)
        {
            if (value.Length == 0)
                return;
            int redA = Int32.Parse(value, CultureInfo.InvariantCulture);
            if (redA != _hrci)
            {
                _hrci = redA;
                log(String.Format(CultureInfo.InvariantCulture, "Team {0} Red Card: {1}", this.TeamA, this.RedA));
                EventHandler<RedCardEventArgs> handler = RedCardEvent;
                if (handler != null)
                {
                    log("Notifying Listerners: Red Card");
                    handler(this, new RedCardEventArgs(_teamA, _hrci, _grci));
                }
            }
        }

        private void updateGuestScore(String value)
        {
            if (String.IsNullOrEmpty(value))
                return;
            int scoreB = Int32.Parse(value, CultureInfo.InvariantCulture);
            if (scoreB > _scoreB)
            {
                _scoreB = scoreB;
                log(String.Format(CultureInfo.InvariantCulture, "Team {0} scored a goal. Score is now {1}", this.TeamB, this.getScore()));
                EventHandler<GoalEventArgs> handler = RaiseGoalEvent;
                if (handler != null)
                {
                    log("Notifying Listerners: New Score");
                    handler(this, new GoalEventArgs(_teamB, _scoreA, _scoreB));
                }
            }
            else if (scoreB < _scoreB)
            {
                _scoreB = scoreB;
                log(String.Format(CultureInfo.InvariantCulture, "Team {0} goal was canceled. New Score {1}", this.TeamB, this.getScore()));
                EventHandler<GoalBackEventArgs> handler = BackGoalEvent;
                if (handler != null)
                {
                    log("Notifying Listerners: Goal canceled");
                    handler(this, new GoalBackEventArgs(_teamB, _scoreA, _scoreB));
                }
            }

        }

        private void updateHomeScore(String value)
        {
            if (value.Length == 0)
                return;
            int scoreA = Int32.Parse(value, CultureInfo.InvariantCulture);
            if (scoreA > _scoreA)
            {                
                _scoreA = scoreA;
                log(String.Format(CultureInfo.InvariantCulture, "Team {0} scored a goal. Score is now {1}", this.TeamA, this.getScore()));
                EventHandler<GoalEventArgs> handler = RaiseGoalEvent;
                if (handler != null)
                {
                    log("Notifying Listerners: New Score");
                    handler(this, new GoalEventArgs(_teamA, _scoreA, _scoreB));
                }
            }
            else if (scoreA < _scoreA)
            {
                _scoreA = scoreA;
                log(String.Format(CultureInfo.InvariantCulture, "Team {0} goal was canceled. New Score {1}", this.TeamA, this.getScore()));
                EventHandler<GoalBackEventArgs> handler = BackGoalEvent;
                if (handler != null)
                {
                    log("Notifying Listerners: Goal canceled");
                    handler(this, new GoalBackEventArgs(_teamA, _scoreA, _scoreB));
                }
            }

        }

        private void updateMatchState(MATCHSTATE value)
        {
            MATCHSTATE oldState = (MATCHSTATE)_sid;
            log(String.Format(CultureInfo.InvariantCulture, "Update Gamestate. Old state is {0}. New State is {1}", _sid, value));
            _sid = (int)value;
            if (oldState == MATCHSTATE.HT && value == MATCHSTATE.SECONDHALF)
                _startTime = DateTime.Now;
            updateTick();
        }


        /*
        private void updateMatchState(String value)
        {
            if (String.IsNullOrEmpty(value))
                return;
            if (_sid == Int32.Parse(value, CultureInfo.InvariantCulture))
                return;

            log(String.Format(CultureInfo.InvariantCulture, "Update Gamestate. Old state is {0}. New State is {1}", _sid, value));

            _sid = Int32.Parse(value);
            updateTick();
        }
        */

        private void updateTick()
        {
            if (_sid == (int)MATCHSTATE.FIRSTHALF)
            {
                m_isStart = LIVESTATES.First;
                // Nur wenn schon Subscriber vorhanden sind
                if (m_iRefCounter > 0)
                {
                    lock (_lockThreadStarter)
                    {
                        if (!_pulsconnected)
                        {
                            SXMinutePulse.Instance.Pulse += Instance_Pulse;
                            _pulsconnected = true;
                        }
                        m_stopPlaytime = false;
                    }
                }
            }
            else if (_sid == (int)MATCHSTATE.HT)
            {
                m_isStart = LIVESTATES.HT;
                lock (_lockThreadStarter)
                {
                    if (!_pulsconnected)
                    {
                        SXMinutePulse.Instance.Pulse += Instance_Pulse;
                        _pulsconnected = true;
                    }
                }
            }
            else if (_sid == (int)MATCHSTATE.SECONDHALF)
            {
                m_isStart = LIVESTATES.Second;
                // Nur wenn Subscriber vorhanden
                if (m_iRefCounter > 0)
                {
                    lock (_lockThreadStarter)
                    {
                        if (!_pulsconnected)
                        {
                            SXMinutePulse.Instance.Pulse += Instance_Pulse;
                            _pulsconnected = true;
                        }
                        m_stopPlaytime = false;
                    }
                }
            }
            else if (_sid == (int)MATCHSTATE.ENDED)
            {
                m_isStart = LIVESTATES.End;

                if (_pulsconnected)
                {
                    SXMinutePulse.Instance.Pulse -= Instance_Pulse;
                    _pulsconnected = false;
                }

                log("Game has ended");
                EventHandler<GameEndedEventArgs> handler = GameEndedEvent;
                if (handler != null)
                {
                    log("Notifying Listeners: Game has Ended");
                    handler(this, new GameEndedEventArgs(_teamA, _scoreA, _scoreB));
                }
            }
            else
            {
                m_isStart = LIVESTATES.Eighth;
                lock (_lockThreadStarter)
                {
                    if (_pulsconnected)
                    {
                        SXMinutePulse.Instance.Pulse -= Instance_Pulse;
                        _pulsconnected = false;
                    }
                    m_stopPlaytime = true;
                }
            }

            
               
        }

        private void doTick()
        {

            int bssj = _sid == (int)MATCHSTATE.HT ? 46 : 0;

            if (_sid == (int)MATCHSTATE.FIRSTHALF || _sid == (int)MATCHSTATE.SECONDHALF)
            {
                if (_sid == (int)MATCHSTATE.FIRSTHALF)
                {
                    bssj = Convert.ToInt32(DateTime.Now.Subtract(_startTime).TotalMinutes);
                    if (bssj < 0)
                        bssj = 0;
                    if (bssj > 45)
                        bssj = 45;
                }
                else if (_sid == (int)MATCHSTATE.SECONDHALF)
                {
                    bssj = Convert.ToInt32(DateTime.Now.Subtract(_startTime).TotalMinutes) + 45;// -15;
                    if (bssj < 46)
                        bssj = 46;
                    if (bssj > 90)
                        bssj = 90;

                }
                else if (_sid == (int)MATCHSTATE.ENDED)
                    bssj = 90;
                else if (_sid == (int)MATCHSTATE.HT)
                    bssj = 45;

                m_playtime = bssj;
                /*
                                m_playtimeThread = new Thread(this.updatePlaytime);
                                m_playtimeThread.IsBackground = true;
                                m_playtimeThread.Start();
                 */
            }
        }

        private void updatePlaytime(Object stateInfo)
        {
            log("Update Playtime");

            doTick();

            log(String.Format(CultureInfo.InvariantCulture, "New Playtime is {0}", m_playtime));
            //m_playtime += 1;
            EventHandler<PlaytimeTickEventArgs> handler = PlaytimeTickEvent;
            if (handler != null)
            {
                log("Notifying Listerners: New Playtime");
                handler(this, new PlaytimeTickEventArgs(String.Format(CultureInfo.InvariantCulture, "Match {0} - {1}", _teamA, _teamB), m_playtime));
            }
            
        }

        #region IScore Member


        public event EventHandler<RedCardEventArgs> RedCardEvent;

        public int RedA
        {
            get { return _hrci; }
        }

        public int RedB
        {
            get { return _grci; }
        }

        #endregion

        public MATCHSTATE getLivestate()
        {
            return (MATCHSTATE)_sid;
        }

        private void log(String message)
        {
            if (BetfairMatch != null)
                LiveTickerLog.Instance.writeLog(this.BetfairMatch, "liveticker2", message);
        }

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
