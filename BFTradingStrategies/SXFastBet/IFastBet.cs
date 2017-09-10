using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Windows.Forms;
using System.Drawing;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.SXFastBet
{
    public interface IFastBet
    {
        event EventHandler<IPSAddedEventArgs> IPSAdded;
        event EventHandler<IPSDeletedEventArgs> IPSDeleted;
        event EventHandler<IPSBetAddedEventArgs> IPSBetAdded;
        event EventHandler<LoadAutoTradeEventArgs> IPSLoadGUI;
        event EventHandler<UnloadAutoTradeEventArgs> IPSUnloadGUI;

        SXALMarket Market
        {
            get;
            set;
        }

        IScore LiveScore
        {
            set;
        }

        Boolean ConfirmFastBet { get; set; }

        Boolean HasMarketIPS(long marketId);

        Boolean HasMarketIPS(String match);

        Boolean HasMarketTrade(long marketId);

        Boolean HasMarketTrade(String match);

        Bitmap GetIPSBitmap();

        Bitmap GetTradeBitmap();

        int NoOfTeamARedCards(int marketId);

        int NoOfTeamBRedCards(int marketId);
    }

    public class IPSAddedEventArgs : EventArgs
    {
        private long _markedId;
        public long MarketID
        {
            get
            {
                return _markedId;
            }
        }

        public IPSAddedEventArgs(long marketid)
        {
            _markedId = marketid;
        }
    }

    public class IPSDeletedEventArgs : EventArgs
    {
        private long _markedId;
        public long MarketID
        {
            get
            {
                return _markedId;
            }
        }

        public IPSDeletedEventArgs(long marketid)
        {
            _markedId = marketid;
        }
    }

    public class IPSBetAddedEventArgs : EventArgs
    {
        private long _marketId;
        public long MarketId { get { return _marketId; } }
        public IPSBetAddedEventArgs(long marketId) { _marketId = marketId; }
    }

    public class LoadAutoTradeEventArgs : EventArgs
    {
        private Control _gui;

        public Control GUI { get { return _gui; } }
        public LoadAutoTradeEventArgs(Control gui) { _gui = gui; }
    }

    public class UnloadAutoTradeEventArgs : EventArgs
    {
        private Control _gui;

        public Control GUI { get { return _gui; } }
        public UnloadAutoTradeEventArgs(Control gui) { _gui = gui; }
    }
}
