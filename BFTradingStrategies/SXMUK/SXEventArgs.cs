using net.sxtrader.muk.enums;
using System;
using System.Globalization;
namespace net.sxtrader.muk.eventargs
{
    public class SXWMessageEventArgs : EventArgs
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

        public SXWMessageEventArgs(DateTime dts, String match, String message, String module)
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

    public class SXWNoIScoreAddedEventArgs : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public SXWNoIScoreAddedEventArgs(String match)
        {
            m_match = match;
        }
    }

    public class SXWIScoreAddedEventArgs : EventArgs
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

        public SXWIScoreAddedEventArgs(String match, long marketId)
        {
            m_state = LIVESCOREADDED.ALL;
            m_marketId = marketId;
            m_match = match;
        }

        public SXWIScoreAddedEventArgs(String match, LIVESCOREADDED state, long marketId)
        {
            m_state = state;
            m_match = match;
            m_marketId = marketId;
        }
    }




    public class SXWStopStopLossTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public SXWStopStopLossTimer(string match)
        {
            m_match = match;
        }
    }

    public class SXWStopOpenBetTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public SXWStopOpenBetTimer(string match)
        {
            m_match = match;
        }
    }

    public class SXWStopCloseTradeTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public SXWStopCloseTradeTimer(string match)
        {
            m_match = match;
        }
    }

    public class SXStopOpenBetTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public SXStopOpenBetTimer(string match)
        {
            m_match = match;
        }
    }

    public class SXStopStopLossTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public SXStopStopLossTimer(string match)
        {
            m_match = match;
        }
    }

    public class SXStopCloseTradeTimer : EventArgs
    {
        private String m_match;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public SXStopCloseTradeTimer(string match)
        {
            m_match = match;
        }
    }




    public class SXWSetStopLossTimer : EventArgs
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

        public SXWSetStopLossTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class SXWSetOpenBetTimer : EventArgs
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

        public SXWSetOpenBetTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class SXWSetCloseTradeTimer : EventArgs
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

        public SXWSetCloseTradeTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class SXSetStopLossTimer : EventArgs
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

        public SXSetStopLossTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class SXSetOpenBetTimer : EventArgs
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

        public SXSetOpenBetTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }

    public class SXSetCloseTradeTimer : EventArgs
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

        public SXSetCloseTradeTimer(string match, TimeSpan timer)
        {
            m_match = match;
            m_timer = timer;
        }
    }


    public class SXWGameEndedEventArgs : EventArgs
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

        public SXWGameEndedEventArgs(String match, DateTime dts, double winloose)
        {
            m_match = match;
            m_dts = dts;
            m_winloose = winloose;
        }
    }

    public class SXWManualTradeRemoveEventArgs : EventArgs
    {
        private String m_match;
        private DateTime m_dts;
        private long _marketId;

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
                return _marketId;
            }
        }

        public DateTime Dts
        {
            get
            {
                return m_dts;
            }
        }



        public SXWManualTradeRemoveEventArgs(String match, DateTime dts, long marketId)
        {
            m_match = match;
            m_dts = dts;
            _marketId = marketId;
        }
    }

    public class SXManualTradeRemoveEventArgs : EventArgs
    {
        private String m_match;
        private DateTime m_dts;
        private long _marketId;

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
                return _marketId;
            }
        }

        public DateTime Dts
        {
            get
            {
                return m_dts;
            }
        }



        public SXManualTradeRemoveEventArgs(String match, DateTime dts, long marketId)
        {
            m_match = match;
            m_dts = dts;
            _marketId = marketId;
        }
    }

    public class SXGameEndedEventArgs : EventArgs
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

        public SXGameEndedEventArgs(String match, DateTime dts, double winloose)
        {
            m_match = match;
            m_dts = dts;
            m_winloose = winloose;
        }
    }

    public class SXWPlaytimeEventArgs : EventArgs
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

        public SXWPlaytimeEventArgs(string match, int playtime)
        {
            m_match = match;
            m_playtime = playtime;
        }

    }

    public class SXPlaytimeEventArgs : EventArgs
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

        public SXPlaytimeEventArgs(string match, int playtime)
        {
            m_match = match;
            m_playtime = playtime;
        }

    }

    public class SXMessageEventArgs : EventArgs
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

        public SXMessageEventArgs(DateTime dts, String match, String message, String module)
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

    public class SXExceptionMessageEventArgs : EventArgs
    {
        private String m_message;
        private String m_component;
        private int m_actualMessageNumber;
        private static volatile int m_messageNumber = 1;

        public String Message
        {
            get
            {
                return m_message;
            }
        }

        public String Component
        {
            get
            {
                return m_component;
            }
        }

        public int MessageNumber
        {
            get
            {
                return m_actualMessageNumber;
            }
        }

        public SXExceptionMessageEventArgs(SXExceptionMessageEventArgs e)
        {
            m_message = e.Message;
            m_component = e.Component;
            m_actualMessageNumber = e.MessageNumber;
        }

        public SXExceptionMessageEventArgs(String component, String message)
        {
            m_message = message;
            m_component = component;
            m_actualMessageNumber = m_messageNumber++;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} - {1}: {2}", DateTime.Now.ToString(), m_component, m_message);
        }
    }

}