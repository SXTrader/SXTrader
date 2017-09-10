using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    public class SXALBet
    {
        public long BetId { get; set; }
        public long SelectionId { get; set; }
        public string SelectionName { get; set; }
        public int AsianLineId { get; set; }
        public long MarketId { get; set; }

        public SXALMarketTypeEnum MarketType { get; set; }
        public string FullMarketName { get; set; }

        public SXALBetStatusEnum BetStatus { get; set; }
        public SXALBetTypeEnum BetType { get; set; }

        public double MatchedSize { get; set; }
        public double RemainingSize { get; set; }
        public double RequestedSize { get; set; }
        public SXALMatch[] Matches { get; set; }
        public DateTime MatchedDate { get; set; }
        public DateTime PlacedDate { get; set; }
        public DateTime CancelledDate { get; set; }
        public DateTime SettledDate { get; set; }

        public double ProfitAndLoss { get; set; }

        public double AvgPrice { get; set; }
        public double Price { get; set; }
       
    }

    public class SXALSortedBetList : SortedList<long, SXALBet> { }
}
