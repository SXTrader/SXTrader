using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.lsparserinterfaces
{
    public class GoalBackEventArgs : EventArgs
    {

        private int m_la;
        private int m_lb;
        private String m_team;

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

        public String Score
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} - {1}", m_la, m_lb);
                return sb.ToString();
            }
        }

        public GoalBackEventArgs(string team, int la, int lb)
        {
            m_team = team;
            m_la = la;
            m_lb = lb;
        }
    }
}
