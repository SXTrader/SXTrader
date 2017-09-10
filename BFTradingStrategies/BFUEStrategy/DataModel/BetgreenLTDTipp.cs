using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.DataModel
{
    public class BetgreenLTDTippList : List<BetgreenLTDTipp> { }
    public class BetgreenLTDTipp
    {
        public DateTime KickOff { get; set; }
        public String TeamA { get; set; }
        public String TeamB { get; set; }
        public double RecommendedOdds { get; set; }
        public double MaximumOdds { get; set; }
        public double StoppLossOdds { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Match {0} - {1}\r\n", TeamA, TeamB);
            sb.AppendFormat("Kickoff {0}\r\n", KickOff);
            sb.AppendFormat("Recommended Odds {0}\r\n", RecommendedOdds);
            sb.AppendFormat("Maximum Odds {0}\r\n", MaximumOdds);
            sb.AppendFormat("Stopp/Loss {0}\r\n", StoppLossOdds);
            return sb.ToString();
        }
    }
}
