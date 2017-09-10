using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.betfairif
{
    public enum LOADSTATE
    {
        UNLOADED = 1,
        LOADED = 2,
        ERROR = 3
    };

    public enum LIVESCOREADDED { ALL = 1, PARTLY = 2, NONE = 3 };

    public class BFStopCloseTradeTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public BFStopCloseTradeTimer(string match)
        {
            m_match = match;
        }
    }

    public class BFWStopCloseTradeTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public BFWStopCloseTradeTimer(string match)
        {
            m_match = match;
        }
    }

    public class BFStopOpenBetTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public BFStopOpenBetTimer(string match)
        {
            m_match = match;
        }
    }

    public class BFWStopOpenBetTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public BFWStopOpenBetTimer(string match)
        {
            m_match = match;
        }
    }

    public class BFStopStopLossTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public BFStopStopLossTimer(string match)
        {
            m_match = match;
        }
    }

    public class BFWStopStopLossTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public BFWStopStopLossTimer(string match)
        {
            m_match = match;
        }
    }

    public class BFSetCloseTradeTimer : EventArgs
    {
        private String m_match;
        TimeSpan m_timer;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public TimeSpan Timer
        {
            get
            {
                return m_timer;
            }
        }

        public BFSetCloseTradeTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class BFWSetCloseTradeTimer : EventArgs
    {
        private String m_match;
        TimeSpan m_timer;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public TimeSpan Timer
        {
            get
            {
                return m_timer;
            }
        }

        public BFWSetCloseTradeTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class BFSetOpenBetTimer : EventArgs
    {
        private String m_match;
        TimeSpan m_timer;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public TimeSpan Timer
        {
            get
            {
                return m_timer;
            }
        }

        public BFSetOpenBetTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class BFWSetOpenBetTimer : EventArgs
    {
        private String m_match;
        TimeSpan m_timer;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public TimeSpan Timer
        {
            get
            {
                return m_timer;
            }
        }

        public BFWSetOpenBetTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class BFSetStopLossTimer : EventArgs
    {
        private String m_match;
        TimeSpan m_timer;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public TimeSpan Timer
        {
            get
            {
                return m_timer;
            }
        }

        public BFSetStopLossTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class BFWSetStopLossTimer : EventArgs
    {
        private String m_match;
        TimeSpan m_timer;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public TimeSpan Timer
        {
            get
            {
                return m_timer;
            }
        }

        public BFWSetStopLossTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class BFWWinLooseChangedEventArgs : EventArgs
    {
        private String m_match;
        private double m_backGuV;
        private double m_layGuV;

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

        public BFWWinLooseChangedEventArgs(String match, double backGuV, double layGuV)
        {
            m_match = match;
            m_backGuV = backGuV;
            m_layGuV = layGuV;
        }
    }

    public class BFWNoIScoreAddedEventArgs : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public BFWNoIScoreAddedEventArgs(String match)
        {
            m_match = match;
        }
    }

    public class BFWIScoreAddedEventArgs : EventArgs
    {
        private String m_match;
        private int m_marketId;
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

        public int MarketId
        {
            get
            {
                return m_marketId;
            }
        }

        public BFWIScoreAddedEventArgs(String match, int marketId)
        {
            m_state = LIVESCOREADDED.ALL;
            m_marketId = marketId;
            m_match = match;
        }

        public BFWIScoreAddedEventArgs(String match, LIVESCOREADDED state, int marketId)
        {
            m_state = state;
            m_match = match;
            m_marketId = marketId;
        }
    }

    public class BFWGoalSumChangedEventArgs : EventArgs
    {
        private String m_match;
        private int m_goalSum;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public int GoalSum
        {
            get
            {
                return m_goalSum;
            }
        }

        public BFWGoalSumChangedEventArgs(string match, int goalsum)
        {
            m_match = match;
            m_goalSum = goalsum;
        }
    }

    public class MatchAddedEventArgs : EventArgs
    {
        private string m_match;
        private string m_GuVBack;
        private string m_GuVLay;
        private string m_sumGoals;
        private int m_marketId;
        private LIVESCOREADDED m_withLivescore;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public int MarketId
        {
            get
            {
                return m_marketId;
            }
        }

        public String GuVLay
        {
            get
            {
                return m_GuVLay;
            }
        }

        public String GuVBack
        {
            get
            {
                return m_GuVBack;
            }
        }

        public String SumGoals
        {
            get
            {
                return m_sumGoals;
            }
        }

        public LIVESCOREADDED WithLivescore
        {
            get
            {
                return m_withLivescore;
            }
        }

        public MatchAddedEventArgs(String match, String sumGoals, String guVBack, String guVLay, LIVESCOREADDED state, int marketId)
        {
            m_match = match;
            m_marketId = marketId;
            m_sumGoals = sumGoals;
            m_GuVLay = guVLay;
            m_GuVBack = guVBack;
            m_withLivescore = state;
        }

        /*
        public MatchAddedEventArgs(String match, String sumGoals, String guVBack, String guVLay)
        {
            m_match = match;
            m_sumGoals = sumGoals;
            m_GuVLay = guVLay;
            m_GuVBack = guVBack;            
        }
         */
    }



    public class BFWinLooseChangedEventArgs : EventArgs
    {
        private String m_match;
        private double m_backGuV;
        private double m_layGuV;

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

        public BFWinLooseChangedEventArgs(String match, double backGuV, double layGuV)
        {
            m_match = match;
            m_backGuV = Math.Round(backGuV, 2);
            m_layGuV = Math.Round(layGuV, 2);
        }
    }

    public class BFGoalSumChangedEventArgs : EventArgs
    {
        private String m_match;
        private int m_goalSum;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public int GoalSum
        {
            get
            {
                return m_goalSum;
            }
        }

        public BFGoalSumChangedEventArgs(string match, int goalsum)
        {
            m_match = match;
            m_goalSum = goalsum;
        }
    }

    public class BFMessageEventArgs : EventArgs
    {
        private string m_match;
        private string m_message;
        private DateTime m_dts;
        private string m_module;

        public String Match
        {
            get
            {
                return m_match;
            }

        }

        public String Message
        {
            get
            {
                return m_message;
            }
        }

        public DateTime MessageDTS
        {
            get
            {
                return m_dts;
            }
        }

        public String Module
        {
            get
            {
                return m_module;
            }
        }

        public BFMessageEventArgs(DateTime dts, String match, String message, String module)
        {
            m_dts = dts;
            m_match = match;
            m_message = message;
            m_module = module;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}: {2} - {3}", m_dts.ToString(), m_module, m_match, m_message);
        }
    }

    public class BFWPlaytimeEventArgs : EventArgs
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

        public BFWPlaytimeEventArgs(string match, int playtime)
        {
            m_match = match;
            m_playtime = playtime;
        }

    }

    public class BFPlaytimeEventArgs : EventArgs
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

        public BFPlaytimeEventArgs(string match, int playtime)
        {
            m_match = match;
            m_playtime = playtime;
        }

    }

    public class BFGameEndedEventArgs : EventArgs
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

        public BFGameEndedEventArgs(String match, DateTime dts, double winloose)
        {
            m_match = match;
            m_dts = dts;
            m_winloose = winloose;
        }
    }

    public class BFWGameEndedEventArgs : EventArgs
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

        public BFWGameEndedEventArgs(String match, DateTime dts, double winloose)
        {
            m_match = match;
            m_dts = dts;
            m_winloose = winloose;
        }
    }

    public class BFWMessageEventArgs : EventArgs
    {
        private string m_match;
        private string m_message;
        private string m_module;
        private DateTime m_dts;

        public String Match
        {
            get
            {
                return m_match;
            }

        }

        public String Message
        {
            get
            {
                return m_message;
            }
        }

        public DateTime MessageDTS
        {
            get
            {
                return m_dts;
            }
        }

        public String Module
        {
            get
            {
                return m_module;
            }
        }

        public BFWMessageEventArgs(DateTime dts, String match, String message, String module)
        {
            m_dts = dts;
            m_match = match;
            m_message = message;
            m_module = module;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}: {2} - {3}", m_dts.ToString(), m_module, m_match, m_message);
        }
    }
}
