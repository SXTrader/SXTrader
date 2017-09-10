using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.lsparserinterfaces
{
    public class RedCardEventArgs : EventArgs
    {
        private int _ra;
        private int _rb;
        private String _team;


        public String Team
        {
            get
            {
                return _team;
            }
        }

        public int RedA
        {
            get
            {
                return _ra;
            }
        }

        public int RedB
        {
            get
            {
                return _rb;
            }
        }

        public RedCardEventArgs(string team, int ra, int rb)
        {
            this._team = team;
            this._ra = ra;
            this._rb = rb;
        }

    }
}
