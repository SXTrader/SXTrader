using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.tippsters.DataModel
{
    public class TLLTipp
    {
        public DateTime EventDate { get; set; }
        public String Race { get; set; }
        public String Horse { get; set; }
        public double MaxOdds { get; set; }
        public Boolean NoSelectionDay { get; set; }
        public Boolean NoTippFound { get; set; }
    }
}
