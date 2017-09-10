
using System;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Text;
using net.sxtrader.muk.enums;
namespace net.sxtrader.bftradingstrategies.tradeinterfaces
{
    public class GoalScoredEventArgs : EventArgs
    {

        private int m_la;
        private int m_lb;
        private String m_team;
        private String m_match;

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

        public String Match
        {
            get { return m_match; }
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

        public GoalScoredEventArgs(string team, int la, int lb, String match)
        {
            m_team = team;
            m_la = la;
            m_lb = lb;
            m_match = match;
        }
    }

    public class PlaytimeEventArgs : EventArgs
    {
        private string m_match;
        private int m_playtime;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public int Playtime
        {
            get
            {
                return m_playtime;
            }
        }

        public PlaytimeEventArgs(string match, int playtime)
        {
            m_match = match;
            m_playtime = playtime;
        }

    }

    public class GameEndedEventArgs : EventArgs
    {
        private String m_match;
        private DateTime m_dts;
        private double m_winloose;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public DateTime Dts
        {
            get
            {
                return m_dts;
            }
        }

        public double WinLoose
        {
            get
            {
                return m_winloose;
            }
        }

        public GameEndedEventArgs(String match, DateTime dts, double winloose)
        {
            m_match = match;
            m_dts = dts;
            m_winloose = winloose;
        }
    }

    public class StateChangedEventArgs : EventArgs
    {
        private TradeStateAbstract _newState;
        private TradeStateAbstract _oldState;

        public TradeStateAbstract NewState
        {
            get { return _newState; }
        }

        public TradeStateAbstract OldState
        {
            get { return _oldState; }
        }

        public StateChangedEventArgs(TradeStateAbstract newState, TradeStateAbstract oldState)
        {
            _newState = newState;
            _oldState = oldState;
        }
    }

    public class BetsChangedEventArgs : EventArgs
    {
        private ITrade _trade;
        public ITrade Trade {get{return _trade;}}

        public BetsChangedEventArgs(ITrade trade)
        {
            _trade = trade;
        }
    }

    public class NoIScoreAddedEventArgs : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public NoIScoreAddedEventArgs(String match)
        {
            m_match = match;
        }
    }

    public class IScoreAddedEventArgs : EventArgs
    {
        private String m_match;
        private long m_marketId;
        private LIVESCOREADDED m_state;

        public LIVESCOREADDED State
        {
            get
            {
                return m_state;
            }
        }

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public long MarketId
        {
            get
            {
                return m_marketId;
            }
        }

        public IScoreAddedEventArgs(String match, long marketId)
        {
            m_state = LIVESCOREADDED.ALL;
            m_marketId = marketId;
            m_match = match;
        }

        public IScoreAddedEventArgs(String match, LIVESCOREADDED state, long marketId)
        {
            m_state = state;
            m_match = match;
            m_marketId = marketId;
        }
    }

    public class TradeAddedEventArgs : EventArgs
    {
        /*
        private string m_match;
        private int m_marketId;
        //private string m_money;
        private double m_backGuV;
        private double m_layGuV;
        private string m_score;
         */
        private LIVESCOREADDED _withLivescore;
        private ITrade _trade;
        /*
        public int MarketId
        {
            get
            {
                return m_marketId;
            }
        }

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

        public String Score
        {
            get
            {
                return m_score;
            }
        }

        */
        public LIVESCOREADDED WithLivescore
        {
            get
            {
                return _withLivescore;
            }
        }

        public ITrade Trade
        {
            get { return _trade; }
        }

        public TradeAddedEventArgs(/*String teamA, String teamB, String score, double backGuV, double layGuV*/ ITrade trade, LIVESCOREADDED state/*, int marketId*/)
        {
            /*
            m_match = String.Format("{0} - {1}", teamA, teamB);
            m_marketId = marketId;
            m_score = score;
            m_backGuV = backGuV;
            m_layGuV = layGuV;
             * */
            _trade = trade;
            _withLivescore = state;
        }
    }


    public class SetTimerEventArgs : EventArgs
    {
        private String m_match;
        private ITrade _trade;

        private TimeSpan m_timer;
        private String m_playtime;
        private Boolean m_flagPlaytime;


        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public Boolean UsePlaytime
        {
            get { return m_flagPlaytime; }
        }


        public TimeSpan Time
        {
            get
            {
                if (m_flagPlaytime) throw new Exception("Fixed Playtime not a TimeSpan.");
                return m_timer;
            }
        }

        public /*TimeSpan*/ String Timer
        {
            get
            {
                if (m_flagPlaytime)
                    return m_playtime;
                else
                    return m_timer.ToString();
            }
        }

        public ITrade Trade { get { return _trade; } }

        public SetTimerEventArgs(string match, ITrade trade, string playtime)
        {
            m_match = match;
            _trade = trade;
            m_flagPlaytime = true;
            if (!playtime.StartsWith("PT"))
                m_playtime = "PT " + playtime;
            else
                m_playtime = playtime;
        }

        public SetTimerEventArgs(string match, ITrade trade, TimeSpan timer)
        {
            m_match = match;
            _trade = trade;
            m_timer = timer;
            m_flagPlaytime = false;
        }
    }

    public class StopTimerEventArgs : EventArgs
    {
        private String m_match;
        private ITrade _trade;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public ITrade Trade { get { return _trade; } }

        public StopTimerEventArgs(string match, ITrade trade)
        {
            m_match = match;
            _trade = trade;
        }
    }
}