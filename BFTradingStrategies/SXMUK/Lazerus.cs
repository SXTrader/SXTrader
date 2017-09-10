using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace net.sxtrader.muk
{
    public class Lazerus
    {
        public enum ENTRYMODES { PREPAREMODE, EXISTMODE };
        private static volatile Lazerus _instance;
        private static Object _syncRoot = "syncRoot";

        private Lazerus() 
        {            
        }

        public static Lazerus Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new Lazerus();
                    }
                }
                return _instance;
            }
        }

        public LazerusList getEntriesByModuleName(String sportExchange, String module)
        {
            LazerusList list = new LazerusList();
            lock (_syncRoot)
            {
                //1. Lazerusdatei öffnen
                XDocument feed = loadLazerusFile();

                //2. Selektieren
                IEnumerable<XElement> entries = from el in feed.Root.Elements("entry")
                                                where (string)el.Attribute("sportexchange") == sportExchange
                                                  && (string)el.Attribute("module") == module
                                                select el;
                //3. Dateieintrag auf LazerusEntry mappen
                foreach (XElement entry in entries)
                {
                    LazerusEntry lazerusEntry = new LazerusEntry(entry);
                    list.Add(lazerusEntry);
                }
            }
            return list;
        }

        public void removeBetFromTrade(String sportExchange, String module, Lazerus.ENTRYMODES mode, long betId)
        {
            lock (_syncRoot)
            {
                XDocument feed = loadLazerusFile();

                IEnumerable<XElement> entries = from el in feed.Root.Elements("entry")
                                                where (string)el.Attribute("sportexchange") == sportExchange
                                                 && (string)el.Attribute("module") == module
                                                select el;
                foreach (XElement entry in entries)
                {
                    IEnumerable<XElement> trades = from el in entry.Elements("trade")
                                                   where (string)el.Attribute("mode") == mode.ToString()
                                                   select el;

                    foreach (XElement trade in trades)
                    {
                        IEnumerable<XElement> bets = from el in trade.Element("bets").Elements("bet")
                                                     where (long)el.Attribute("id") == betId
                                                     select el;
                        bets.Remove();
                    }
                }
                feed.Save(SXDirs.ApplPath + @"lazerus.xml");
            }
        }

        private XDocument loadLazerusFile()
        {
            XDocument feed = null;
            try
            {
                XDocument.Load(SXDirs.ApplPath + @"\lazerus.xml");
            }
            catch (FileNotFoundException)
            {
                feed = new XDocument(new XElement("root"));
                feed.Root.Add(new XAttribute("version", 1));
                feed.Save(SXDirs.ApplPath + @"lazerus.xml");
            }
            return feed;
        }
    }

    public class LazerusEntry
    {
        private List<LazerusNode> _existTrades;
        private List<LazerusNode> _prepareTrades;

        public LazerusNode[] ExistTrades
        {
            get
            {
                return _existTrades.ToArray();
            }
        }

        public LazerusNode[] PrepareTrades
        {
            get
            {
                return _prepareTrades.ToArray();
            }
        }

        internal LazerusEntry(XElement entryData)
        {
            _existTrades = new List<LazerusNode>();
            _prepareTrades = new List<LazerusNode>();

            IEnumerable<XElement> entries = from el in entryData.Elements("trade")
                                            where (string)el.Attribute("mode") == Lazerus.ENTRYMODES.EXISTMODE.ToString()
                                            select el;

            foreach (XElement el in entries)
            {
                LazerusNode node = new LazerusNode(el);
                _existTrades.Add(node);
            }            

            entries = from el in entryData.Elements("trade")
                        where (string)el.Attribute("mode") == Lazerus.ENTRYMODES.PREPAREMODE.ToString()
                      select el;

            foreach (XElement el in entries)
            {
                LazerusNode node = new LazerusNode(el);
                _prepareTrades.Add(node);
            }
        }


        public class LazerusNode
        {
            private XElement _settings;
            private List<long> _betIds;

            public XElement Settings
            {
                get
                {
                    return _settings;
                }
            }

            public long[] BetIds
            {
                get
                {
                    return _betIds.ToArray();
                }
            }

            internal LazerusNode(XElement nodeData)
            {
                _betIds = new List<long>();
                _settings = nodeData.Element("settings");

                foreach (XElement el in nodeData.Element("bets").Descendants())
                {
                    long betid = 0;
                    if (Int64.TryParse(el.Attribute("id").Value, out betid))
                    {
                        _betIds.Add(betid);
                    }
                }
            }
        }
    }

    public class LazerusList : List<LazerusEntry>
    {
    }
}
