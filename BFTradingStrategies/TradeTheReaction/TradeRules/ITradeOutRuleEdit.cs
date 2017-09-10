using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.ttr.Configuration;

namespace net.sxtrader.bftradingstrategies.ttr.TradeRules
{
    interface ITradeOutRuleEdit
    {
        event EventHandler<TTRTOElementSaveEventArgs> SaveTOElement;
        TTRTradeOutCheck TradeOutCheck
        {get;
         set;}
    }

    public class TTRTOElementSaveEventArgs : EventArgs
    {
        TTRTradeOutCheck _element;
        public TTRTradeOutCheck TradeOutElement
        {
            get
            {
                return _element;
            }
        }

        public TTRTOElementSaveEventArgs(TTRTradeOutCheck element)
        {
            _element = element;
        }
    }
}
