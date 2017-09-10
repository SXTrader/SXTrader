using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj
{
    class IPTraderPrepMgrBetAddedEventArgs : EventArgs
    {
        private long _marketId;
        public long MarketId { get { return _marketId; } }
        public IPTraderPrepMgrBetAddedEventArgs(long marketId) { _marketId = marketId; }
    }
    class IPTraderConfigSortedList : SortedList<long, IPTraderPrepManagerObj> { }
    class IPTraderPrepManager
    {
        private IPTraderConfigSortedList _metaList = new IPTraderConfigSortedList();

        public event EventHandler<IPTraderPrepMgrBetAddedEventArgs> BetAddedEvent;

        public IPTraderPrepManager()
        { }

        public void addConfigList(long marketId, BFUEFBIPTraderConfigList list, IScore score, BFUEWatcher watcher)
        {
            try
            {
                if (_metaList.ContainsKey(marketId))
                {
                    if (list.Count == 0)
                        _metaList.Remove(marketId);
                    else
                        _metaList[marketId].ConfigList = list;
                }
                else
                {
                    if (list.Count != 0)
                    {
                        IPTraderPrepManagerObj iptObj = new IPTraderPrepManagerObj(marketId, score, watcher);
                        iptObj.BetAddedEvent += new EventHandler<IPTraderPrepBetAddedEventArgs>(iptObj_BetAddedEvent);
                        iptObj.ConfigList = list;
                        _metaList.Add(marketId, iptObj);
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void iptObj_BetAddedEvent(object sender, IPTraderPrepBetAddedEventArgs e)
        {
            EventHandler<IPTraderPrepMgrBetAddedEventArgs> handler = BetAddedEvent;
            if (handler != null)
            {
                handler(this, new IPTraderPrepMgrBetAddedEventArgs(e.MarketId));
            }
            //throw new NotImplementedException();
        }

        public BFUEFBIPTraderConfigList getConfigList(long marketId)
        {
            try
            {
                if (_metaList.ContainsKey(marketId))
                    return _metaList[marketId].ConfigList;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return null;
        }
    }
}
