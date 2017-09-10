using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.tradeinterfaces
{
    public interface IOverviewGUI
    {
        ITrade Trade { set; get; }
    }
}
