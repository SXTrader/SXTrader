using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.muk
{
    public class SXLeagues : SortedList<String,String>
    {
        private static volatile SXLeagues instance;
        private static Object syncRoot = "syncRoot";

        private SXLeagues() : base() { }
        private SXLeagues(int capacity) : base(capacity) { }
        private SXLeagues(IComparer<string> comparer) : base(comparer) { }
        private SXLeagues(IDictionary<string,string> dictionary) : base(dictionary) { }
        private SXLeagues(int capacity, IComparer<string> comparer) : base(capacity, comparer) { }
        private SXLeagues(IDictionary<string,string> dictionary, IComparer<string> comparer) : base(dictionary, comparer) { }


        public static SXLeagues Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SXLeagues();
                    }
                }

                return instance;
            }
        }
    }
}
