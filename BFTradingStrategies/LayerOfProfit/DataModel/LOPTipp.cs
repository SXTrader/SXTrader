using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.tippsters.LOP.DataModel
{
    public class LOPTipp
    {
        public DateTime EventDate { get; set; }
        public String Race { get; set; }
        public String Horse { get; set; }
        public Boolean NoSelectionDay { get; set; }
        public Boolean NoTippFound { get; set; }
    }
}
