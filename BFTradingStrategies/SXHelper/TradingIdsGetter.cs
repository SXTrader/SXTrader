using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.sxhelper
{
    public static class TradingIdsGetter
    {
        private static uint _tradeIdPool = 0;
        private static object _lock = "idGetterLock";

        public static uint nextId()
        {
            lock (_lock)
            {
                return ++_tradeIdPool;
            }
        }
    }
}
