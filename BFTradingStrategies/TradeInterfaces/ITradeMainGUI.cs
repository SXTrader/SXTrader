using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.plugin;

namespace net.sxtrader.bftradingstrategies.tradeinterfaces
{
    public interface ITradeMainGUI
    {
        IWatcher Watcher { get; }
        void initWatcher(LiveScoreParser parser, LiveScore2Parser parser2);
        void initHost(IPluginHost host);
    }
}
