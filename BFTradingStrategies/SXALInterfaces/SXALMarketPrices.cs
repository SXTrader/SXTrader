using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    public class SXALMarketPrices
    {
        public long MarketId { get; set; }
        public SXALMarketStatusEnum MarketStatus { get; set; }

        public string CurrencyCode { get; set; }

        public float MarketBaseRate { get; set; }

        public SXALRunnerPrices[] RunnerPrices { get; set; }
    }
}
