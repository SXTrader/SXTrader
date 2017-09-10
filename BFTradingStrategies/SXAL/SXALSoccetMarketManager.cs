using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using System.Windows.Forms;
using System.Diagnostics;
using net.sxtrader.bftradingstrategies.livescoreparser;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    public sealed class SXALSoccerMarketManager : IDisposable
    {
        private static volatile SXALSoccerMarketManager instance;
        private static Object syncRoot = new Object();

        private Thread m_marketUpdater;
        private System.Timers.Timer _timerCleanUp;
        private SortedList<long, SXALMarket> m_listMarket;
        private LOADSTATE m_loadstate;
        private bool _initialLoad = true;
        private bool _disposed = false;

        public event EventHandler<MarketLoadProgressEventArgs> MarketLoadEvent;
        public event EventHandler<MarketAddedEventArgs> MarketAddedEvent;
        public event EventHandler<MarketUpdateStartedEventArgs> MarketUpdateStartedEvent;
        public event EventHandler<MarketUpdateCompletedEventArgs> MarketUpdateCompletedEvent;

        private SXALSoccerMarketManager()
        {
            m_loadstate = LOADSTATE.UNLOADED;
            m_listMarket = new SortedList<long, SXALMarket>();
            m_marketUpdater = new Thread(this.marketUpdater);
            m_marketUpdater.IsBackground = true;
            m_marketUpdater.Start();

            //Timer für das Aufräumen beendeter Märkte
            _timerCleanUp = new System.Timers.Timer(new TimeSpan(0, 30, 0).TotalMilliseconds);
            _timerCleanUp.Elapsed += new System.Timers.ElapsedEventHandler(_timerCleanUp_Elapsed);
            _timerCleanUp.Start();

           
            //Debug.WriteLine("Instanz von SoccerMarketManager wurde erzeugt!");
        }

        public static SXALSoccerMarketManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SXALSoccerMarketManager();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Alle Märkte, sowohl InPlay als auch nicht InPlay
        /// </summary>
        public SortedList<long, SXALMarket> AllMarkets
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(1000);

                return m_listMarket;
            }
        }

        /// <summary>
        /// Die Liste aller Märkte, welche nicht InPlay gehen
        /// </summary>
        public SortedList<long, SXALMarket> RegularMarkets
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(1000);
                SortedList<long, SXALMarket> regularmarkets = new SortedList<long, SXALMarket>();
                foreach (SXALMarket market in m_listMarket.Values)
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

        public SortedList<long, SXALMarket> InPlayMarkets
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(1000);
                SortedList<long, SXALMarket> inplaymarkets = new SortedList<long, SXALMarket>();
                try
                {
                    foreach (SXALMarket market in m_listMarket.Values)
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

        public bool isCorrectScoreMarket(long marketid)
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

        public bool isMarketMatchOdds(long marketid)
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

        public bool isMarketInplay(long marketid)
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
        public SXALMarket getCSMarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsScoreMarket)
                    return market;
            }

            return null;
        }

        public SXALMarket getWLDMarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU25MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU05MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU15MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU35MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU45MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU55MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU65MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU75MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
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
        public SXALMarket getOU85MarketByMatch(string match)
        {
            foreach (SXALMarket market in m_listMarket.Values)
            {
                if (market.Match == match && market.IsOverUnder85)
                    return market;
            }

            return null;
        }

        public SXALMarket getMarketById(long marketId, bool checkLoadState)
        {
            if (checkLoadState)
            {
                while (this.m_loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(100);
            }
            if (m_listMarket.ContainsKey(marketId))
                return m_listMarket[marketId];
            return null;
        }

        public String getMatchById(long marketId)
        {
            try
            {
                if (m_listMarket.ContainsKey(marketId))
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
                        if (_initialLoad)
                            m_loadstate = LOADSTATE.UNLOADED;
                        else
                            m_loadstate = LOADSTATE.UPDATEING;
                        //m_listMarket.Clear();
                        long soccerEventId = SXALEventManager.Instance.getEventIdByName("Soccer");
                        int?[] events = { (int)soccerEventId };
                        DebugWriter.Instance.WriteMessage("SXALScooerMarketManager", String.Format("Read new Soccer Markets. Event ID {0}. StartDate {1}. EndDate {2}", (int)soccerEventId, dtsBegin, dtsEnd));
                        SXALMarket[] markets = SXALKom.Instance.getAllMarkets(events, dtsBegin, dtsEnd);
                        int i = 0;
                        int deduction = 0;
                        if (markets != null)
                        {

                            EventHandler<MarketUpdateStartedEventArgs> marketUpdateStarted = MarketUpdateStartedEvent;
                            if (marketUpdateStarted != null)
                            {
                                marketUpdateStarted(this, new MarketUpdateStartedEventArgs());
                            }

                            foreach (SXALMarket m in markets)
                            {
                                //Uns interessieren nur Inplay-Märkte
                                if (!m.InPlayMarket)
                                {
                                    deduction++;
                                    //Trace.WriteLine(String.Format("SXALSoccerMarketManager: Market {0} Match {1} is not an Inplay Market. Don't add", m.Name, m.Match));
                                    continue;
                                }

                                try
                                {
                                    if (!m_listMarket.ContainsKey(m.Id))
                                    {
                                        DateTime startDts = DateTime.Now;
                                        //Schaue, ob schon ein Markt für diese Begegnung existiert.
                                        //Falls Ja nimm dessen Liveticker zur initializierung.
                                        foreach (SXALMarket m2 in m_listMarket.Values)
                                        {
                                            if (m2.Match.Equals(m.Match, StringComparison.InvariantCultureIgnoreCase))
                                            {                                                
                                                break;
                                            }
                                        }
                                        m.initializeLiveticker();
                                        m_listMarket.Add(m.Id, m);
                                        m.MarketToBeRemoved += new EventHandler<SXALMarketRemoveEventArgs>(m_MarketToBeRemoved);

                                        if (_initialLoad)
                                        {
                                            EventHandler<MarketLoadProgressEventArgs> marketLoadProgress = MarketLoadEvent;
                                            if (marketLoadProgress != null)
                                            {
                                                marketLoadProgress(this, new MarketLoadProgressEventArgs(markets.Length-deduction, i));
                                            }
                                        }

                                        EventHandler<MarketAddedEventArgs> marketAdded = MarketAddedEvent;
                                        if (marketAdded != null)
                                        {
                                            marketAdded(this, new MarketAddedEventArgs(m));
                                        }
                                        DateTime endDts = DateTime.Now;
                                        //Trace.Write(String.Format("{3} - {2} Add Market {0} for Match {1}\r\n", m.Name, m.Match, ++i, endDts - startDts));

                                    }
                                }
                                catch (ArgumentException ae)
                                {
                                    ExceptionWriter.Instance.WriteException(ae);
                                }
                            }

                            _initialLoad = false;
                        }

                        m_loadstate = LOADSTATE.LOADED;
                    }
                    catch (SXALEventDoesNotExistException ednee)
                    {
                        MessageBox.Show(String.Format("An Event does not Exist. Geting Error Message {0}. SXTrader is terminating", ednee.Message), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ExceptionWriter.Instance.WriteException(ednee);
                        Environment.Exit(-1);
                    }
                    catch (SXALMaxInputRecordExceedException miree)
                    {
                        ExceptionWriter.Instance.WriteException(miree);
                        m_loadstate = LOADSTATE.LOADED;
                    }
                    catch (Exception exc)
                    {
                        m_loadstate = LOADSTATE.LOADED;
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                    finally
                    {
                        EventHandler<MarketUpdateCompletedEventArgs> marketUpdateCompleted = MarketUpdateCompletedEvent;
                        if (marketUpdateCompleted != null)
                        {
                            marketUpdateCompleted(this, new MarketUpdateCompletedEventArgs());
                        }
                    }
                }


                timeToWait = new TimeSpan(0, 30, 0);
            }
        }

        private void m_MarketToBeRemoved(object sender, SXALMarketRemoveEventArgs e)
        {
            try
            {
                /*
                while (m_loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(1000);
                */
                m_listMarket.Remove(e.Market.Id);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _timerCleanUp_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(1000);

                List<SXALMarket> marketsToBeRemoved = new List<SXALMarket>();

                foreach (SXALMarket m in m_listMarket.Values)
                {
                    if (DateTime.Now.Subtract(m.StartDTS).TotalHours > 5)
                    {
                        marketsToBeRemoved.Add(m);
                    }
                }

                foreach (SXALMarket m in marketsToBeRemoved)
                {
                    m.forceRemoveSignal();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        #region IDisposable Member
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DebugWriter.Instance.WriteMessage("SXALSoccerMarketManager", "Disposing");
                if (disposing)
                {
                    if (_timerCleanUp != null)
                    {
                        _timerCleanUp.Dispose();
                        _timerCleanUp = null;
                    }

                    if (m_listMarket != null)
                    {
                        m_listMarket.Clear();
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }

    public class MarketUpdateStartedEventArgs : EventArgs { }

    public class MarketUpdateCompletedEventArgs : EventArgs { }

    public class MarketAddedEventArgs : EventArgs
    {
        SXALMarket _market;

        public SXALMarket Market
        {
            get
            {
                return _market;
            }
        }

        public MarketAddedEventArgs(SXALMarket market)
        {
            _market = market;
        }
    }

    public class MarketLoadProgressEventArgs : EventArgs
    {
        private int _overallMarket;
        private int _currentMarket;

        public int OverallMarketCount
        {
            get
            {
                return _overallMarket;
            }
        }

        public int CurrentMarketCount
        {
            get
            {
                return _currentMarket;
            }
        }

        public MarketLoadProgressEventArgs(int overallMarketCount, int currentMarketCount)
        {
            _overallMarket = overallMarketCount;
            _currentMarket = currentMarketCount;
        }

    }
}
