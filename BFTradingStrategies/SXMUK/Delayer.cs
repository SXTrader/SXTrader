using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.muk
{
    public static class Delayer
    {

        public static int delay(int lobound, int hibound)
        {
            Random rnd = new Random();
            return rnd.Next(lobound, hibound);
        }

    }
}
