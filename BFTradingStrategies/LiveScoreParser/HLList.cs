using net.sxtrader.bftradingstrategies.lsparserinterfaces;
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
    public class HLList : SortedList<String, HLLiveScore>
    {
        private static volatile HLList instance;
        private static Object syncRoot = new Object();

        private HLList()
        {
        }
        
        public static HLList Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new HLList();
                    }
                }

                return instance;
            }
        }

        public void Add(String key, HLLiveScore value)
        {
            DebugWriter.Instance.WriteMessage("HLList", String.Format("Add match {0}", key));
            base.Add(key, value);
        }

        public bool ContainsValue(IScore ticker)
        {
            HLLiveScore hlScore = ticker as HLLiveScore;
            if (hlScore != null)
            {
                return base.ContainsValue(hlScore);
            }
            return false;
        }

        public int IndexOfValue(IScore ticker)
        {
            HLLiveScore hlScore = ticker as HLLiveScore;
            if (hlScore != null)
            {
                return base.IndexOfValue(hlScore);
            }
            return -1;
        }
    }
     
}
