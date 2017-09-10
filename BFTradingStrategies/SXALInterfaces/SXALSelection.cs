using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    public class SXALSelection
    {
        public SXALSelection()
        {
            IsNonStarter = false;
        }

        public bool IsNonStarter { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
