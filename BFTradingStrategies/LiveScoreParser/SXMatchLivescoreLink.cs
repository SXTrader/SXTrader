using net.sxtrader.bftradingstrategies.sxhelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    /// <summary>
    /// Klasse verwalten zu einen Wettbörsenbegegnung (Key) den Highlevelticker (Value). Die Zuordnung wird von SoccerMarketManager vorgenommen.
    /// </summary>
    /// 
    /*
    public class SXMatchLivescoreLinkList : SortedList<String, SXMatchLivescoreLink>
    {
        private static volatile SXMatchLivescoreLinkList instance;
        private static Object syncRoot = "syncRoot";

        private SXMatchLivescoreLinkList() : base() { }
        private SXMatchLivescoreLinkList(int capacity) : base(capacity) { }
        private SXMatchLivescoreLinkList(IComparer<string> comparer) : base(comparer) { }
        private SXMatchLivescoreLinkList(IDictionary<string, SXMatchLivescoreLink> dictionary) : base(dictionary) { }
        private SXMatchLivescoreLinkList(int capacity, IComparer<string> comparer) : base(capacity, comparer) { }
        private SXMatchLivescoreLinkList(IDictionary<string, SXMatchLivescoreLink> dictionary, IComparer<string> comparer) : base(dictionary, comparer) { }


        public static SXMatchLivescoreLinkList Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SXMatchLivescoreLinkList();
                    }
                }

                return instance;
            }
        }

        public void Add(String key, SXMatchLivescoreLink value)
        {
            DebugWriter.Instance.WriteMessage("SXMatchLivescoreLinkList", String.Format("Add match {0}", key));
            base.Add(key, value);
        }

    }

    public class SXMatchLivescoreLink
    {
        private HLLiveScore _liveticker;
        private String _match;
        private long _marketId;

        public HLLiveScore HLScore { get { return _liveticker; } }
        public String Match { get { return _match; } }
        public long MarketId { get { return _marketId; } }

        public SXMatchLivescoreLink(String match, long marketId, HLLiveScore liverticker)
        {
            _match = match;
            _marketId = marketId;
            _liveticker = liverticker;
        }

    }
     */
}
