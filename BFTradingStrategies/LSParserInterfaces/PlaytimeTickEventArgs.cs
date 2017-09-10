using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.lsparserinterfaces
{
    public class PlaytimeTickEventArgs : EventArgs
    {
        private String m_match;
        private int m_tick;

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public int Tick
        {
            get
            {
                return m_tick;
            }
        }

        public PlaytimeTickEventArgs(string match, int tick)
        {
            m_match = match;
            m_tick = tick;
        }
    }
}
