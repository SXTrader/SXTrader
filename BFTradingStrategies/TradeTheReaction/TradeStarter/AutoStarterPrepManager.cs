using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.ttr.TradeStarter
{
    class ASPrepMgrBetAddedEventArgs : EventArgs
    {
        private long _marketId;
        public long MarketId { get { return _marketId; } }
        public ASPrepMgrBetAddedEventArgs(long marketId) { _marketId = marketId; }
    }
    class ASConfigSortedList : SortedList<String, AutoStarterPrepObj> { }
    class AutoStarterPrepMgr
    {
        private static volatile AutoStarterPrepMgr instance;
        private static Object syncRoot = new Object();

        private ASConfigSortedList _metaList = new ASConfigSortedList();        
        public event EventHandler<ASPrepMgrBetAddedEventArgs> BetAddedEvent;

        private AutoStarterPrepMgr()
        { }

        public static AutoStarterPrepMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AutoStarterPrepMgr();
                    }
                }

                return instance;
            }
        }

        public void addConfigList(String match, long marketId, TradeStarterConfigList list, IScore score, TTRWatcher watcher)
        {
            if (_metaList.ContainsKey(match))
            {
                if (list.Count == 0)
                    _metaList.Remove(match);
                else
                    _metaList[match].ConfigList = list;
            }
            else
            {
                if (list.Count != 0)
                {
                    //String match = SoccerMarketManager.Instance.getMatchById(marketId);
                    AutoStarterPrepObj asObj = new AutoStarterPrepObj(marketId,match, score, watcher);
                    asObj.BetAddedEvent += new EventHandler<AutoStarterBetAddedEventArgs>(asObj_BetAddedEvent);
                    asObj.ConfigList = list;
                    SXALMarket m = SXALSoccerMarketManager.Instance.getMarketById(marketId, false);

                    if (m != null)
                    {
                        m.MarketToBeRemoved += m_MarketToBeRemoved;
                    }
                    _metaList.Add(match, asObj);
                }
            }
        }

        void m_MarketToBeRemoved(object sender, SXALMarketRemoveEventArgs e)
        {
            try
            {
                AutoStarterPrepObj obj = _metaList[e.Market.Match];
                _metaList.Remove(e.Market.Match);
                if (obj != null)
                {
                    obj.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void asObj_BetAddedEvent(object sender, AutoStarterBetAddedEventArgs e)
        {            
            EventHandler<ASPrepMgrBetAddedEventArgs> handler = BetAddedEvent;
            if (handler != null)
            {
                handler(this, new ASPrepMgrBetAddedEventArgs(e.MarketId));
            }
            //throw new NotImplementedException();
        }

        public TradeStarterConfigList getConfigList(String match)
        {
            if (_metaList.ContainsKey(match))
                return _metaList[match].ConfigList;

            return null;
        }
    }
}
