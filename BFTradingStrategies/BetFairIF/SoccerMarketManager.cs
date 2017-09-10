using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.betfairif
{
    public sealed class SoccerMarketManager
    {
        private static volatile SoccerMarketManager instance;
        private static Object syncRoot = new Object();

        private Thread m_marketUpdater;
        private SortedList<long, BFMarket> m_listMarket;
        private LOADSTATE m_loadstate;

        private SoccerMarketManager()
        {
            m_loadstate = LOADSTATE.UNLOADED;
            m_listMarket = new SortedList<long, BFMarket>();
            m_marketUpdater = new Thread(this.marketUpdater);
            m_marketUpdater.IsBackground = true;
            m_marketUpdater.Start();
            
        }

        public static SoccerMarketManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SoccerMarketManager();
                    }
                }

                return instance;
            }
        }


        /// <summary>
        /// Die Liste aller Märkte, welche nicht InPlay gehen
        /// </summary>
        public SortedList<int, BFMarket> RegularMarkets
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(1000);
                SortedList<int, BFMarket> regularmarkets = new SortedList<int, BFMarket>();
                foreach (BFMarket market in m_listMarket.Values)
                {
                    try
                    {
                        if (!market.InPlayMarket)
                            regularmarkets.Add(market.Id, market);
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                }
                return regularmarkets;
            }
        }

        public SortedList<int, BFMarket> InPlayMarkets
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(1000);
                SortedList<int, BFMarket> inplaymarkets = new SortedList<int, BFMarket>();
                try
                {
                    foreach (BFMarket market in m_listMarket.Values)
                    {
                        if (market.InPlayMarket)
                            inplaymarkets.Add(market.Id, market);
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
                return inplaymarkets;
            }

        }

        public bool isCorrectScoreMarket(int marketid)
        {
            while (m_loadstate == LOADSTATE.UNLOADED)
                Thread.Sleep(1000);
            try
            {
                if (m_listMarket.ContainsKey(marketid))
                    return m_listMarket[marketid].IsScoreMarket;
                else
                    return false;
            }
            catch (KeyNotFoundException knfe)
            {
                ExceptionWriter.Instance.WriteException(knfe);
                return false;
            }
        }

        public bool isMarketTotalGoals(int marketid)
        {
            while (m_loadstate == LOADSTATE.UNLOADED)
                Thread.Sleep(1000);
            try
            {
                if (m_listMarket.ContainsKey(marketid))
                    return m_listMarket[marketid].IsTotalGoals;
                else 
                    return false;
            }
            catch (KeyNotFoundException knfe)
            {
                ExceptionWriter.Instance.WriteException(knfe);
                return false;
            }
        }

        public bool isMarketMatchOdds(int marketid)
        {
            while (m_loadstate == LOADSTATE.UNLOADED)
                Thread.Sleep(1000);
            try
            {
                if (m_listMarket.ContainsKey(marketid))
                    return m_listMarket[marketid].IsMatchOdds;
                else
                    return false;
            }
            catch (KeyNotFoundException knfe)
            {
                ExceptionWriter.Instance.WriteException(knfe);
                return false;
            }
        }

        public bool isMarketInplay(int marketid)
        {
            while (m_loadstate == LOADSTATE.UNLOADED)
                Thread.Sleep(1000);
            try
            {
                if (m_listMarket.ContainsKey(marketid))
                    return m_listMarket[marketid].InPlayMarket;
                else
                    return false;
            }
            catch (KeyNotFoundException knfe)
            {
                ExceptionWriter.Instance.WriteException(knfe);
                return false;
            }
        }

        /// <summary>
        /// Liefert den Correct Score Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getCSMarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsScoreMarket)
                    return market;
            }

            return null;
        }

        public BFMarket getWLDMarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsMatchOdds)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 2.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU25MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder25)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 0.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU05MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder05)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 1.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU15MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder15)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 3.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU35MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder35)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 4.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU45MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder45)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 5.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU55MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder55)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 6.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU65MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder65)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 7.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU75MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder75)
                    return market;
            }

            return null;
        }

        /// <summary>
        /// Liefert den Over/Under 8.5 Markt für eine
        /// gegebenen Begegnung zurück.
        /// </summary>
        /// <param name="match">Name der Begegnung</param>
        /// <returns>Den Markt oder null, wenn nicht gefunden</returns>
        public BFMarket getOU85MarketByMatch(string match)
        {
            foreach (BFMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder85)
                    return market;
            }

            return null;
        }

        public BFMarket getMarketById(long marketId)
        {
            if(m_listMarket.ContainsKey(marketId))
                return m_listMarket[marketId];
            return null;
        }

        public String getMatchById(int marketId)
        {
            try
            {
                if(m_listMarket.ContainsKey(marketId))
                    return m_listMarket[marketId].Match;
                return null;
            }
            catch
            {
                return null;
            }
        }

        private void marketUpdater()
        {           
            TimeSpan timeToWait = new TimeSpan();
            char[] cSeps = { ':' };

            while (true)
            {
                Thread.Sleep(timeToWait);
                DateTime dtsBegin = DateTime.Now.AddHours(-2.0);
                DateTime dtsEnd = DateTime.Now.AddHours(26.0);
                lock (syncRoot)
                {
                    try
                    {
                        m_loadstate = LOADSTATE.UNLOADED;
                        m_listMarket.Clear();
                        int soccerEventId = EventManager.Instance.getEventIdByName("Soccer");
                        int?[] events = { soccerEventId };
                        String plainMarketString = BetfairKom.Instance.getAllMarkets(events, dtsBegin, dtsEnd);
                        if (plainMarketString != null)
                        {
                            String[] marketStrings = plainMarketString.Split(cSeps);
                            foreach (String str in marketStrings)
                            {
                                if (str == null || str == String.Empty)
                                    continue;

                                BFMarket market = new BFMarket(str);
                                try
                                {
                                    m_listMarket.Add(market.Id, market);
                                }
                                catch (ArgumentException ae)
                                {
                                    
                                    ExceptionWriter.Instance.WriteException(ae);
                                    continue;
                                }

                            }
                            
                            m_loadstate = LOADSTATE.LOADED;
                        }
                    }
                    catch (Exception exc)
                    {
                        m_loadstate = LOADSTATE.LOADED;
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                }
                timeToWait = new TimeSpan(2, 0, 0);
            }
        }
    }
}
