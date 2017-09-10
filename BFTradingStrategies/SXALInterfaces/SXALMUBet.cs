using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    public class SXALMUBet
    {
        public long BetId { get; set; }
        public long SelectionId { get; set; }
        public long MarketId { get; set; }
        public int AsianLineId { get; set; }

        public SXALBetStatusEnum BetStatus { get; set; }

        public double Size {get;set;}

        public SXALBetTypeEnum BetType { get; set; }

        public DateTime MatchedDate { get; set; }
    }
}
