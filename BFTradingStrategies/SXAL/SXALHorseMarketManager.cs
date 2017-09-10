using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using System.Timers;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Threading;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    public sealed class SXALHorseMarketManager : IDisposable
    {
        /*
         * MATCH == RACE
         * NAME = MARKET (TO BE PLACED etc)
         */

        private delegate void AsyncMethodCaller();

        private static volatile SXALHorseMarketManager instance;
        private static Object syncRoot = new Object();
        private LOADSTATE _loadstate;
        private SortedList<long, SXALMarket> _listMarket;
        private bool _disposed = false;

        private System.Timers.Timer _timer;

        private SXALHorseMarketManager()
        {
            _listMarket = new SortedList<long, SXALMarket>();
            _timer = new System.Timers.Timer(new TimeSpan(2,0,0).TotalMilliseconds);;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();

            _loadstate = LOADSTATE.UNLOADED;          
            
            AsyncMethodCaller caller = new AsyncMethodCaller(this.marketUpdater);
            IAsyncResult ar = caller.BeginInvoke(null, null);
            /*
            if (!ar.AsyncWaitHandle.WaitOne(60000))
            {
                DebugWriter.Instance.WriteMessage("SXAL", "Constructor SXALHorseMarketManager received Timeout");
            }
            else
                caller.EndInvoke(ar);
             */
            
        }


        public static SXALHorseMarketManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SXALHorseMarketManager();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Alle Pferdemärkte in unstrukturierte Reihenfolge
        /// </summary>
        public SortedList<long, SXALMarket> AllMarketsChaotic
        {
            get
            {
                while (this._loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(100);
                return _listMarket;
            }
        }

        /// <summary>
        /// Liefert alle Win-Markets zurück
        /// </summary>
        public SortedList<long, SXALMarket> AllWinMarkets
        {
            get
            {
                while (this._loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(100);
                SortedList<long, SXALMarket> list = new SortedList<long, SXALMarket>();
                foreach (SXALMarket m in _listMarket.Values)
                {
                    if (m.Name.Equals("Win Market", StringComparison.InvariantCultureIgnoreCase))
                        list.Add(m.Id, m);
                }

                return list;
            }
        }

        public SortedList<long, SXALMarket> AllPlacedMarkets
        {
            get
            {
                while (this._loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(100);
                SortedList<long, SXALMarket> list = new SortedList<long, SXALMarket>();
                foreach (SXALMarket m in _listMarket.Values)
                {
                    if (m.Name.Equals("Place Market", StringComparison.InvariantCultureIgnoreCase))
                        list.Add(m.Id, m);
                }

                return list;
            }
        }

        public SortedList<long, SXALMarket> AllEachwayMarkets
        {
            get
            {
                while (this._loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(100);
                SortedList<long, SXALMarket> list = new SortedList<long, SXALMarket>();
                foreach (SXALMarket m in _listMarket.Values)
                {
                    if (m.Name.Contains("Each Way "))
                        list.Add(m.Id, m);
                }

                return list;
            }
        }

        public SortedList<long, SXALMarket> AllUnspecifiedMarkets
        {
            get
            {
                while (this._loadstate == LOADSTATE.UNLOADED)
                    Thread.Sleep(100);
                SortedList<long, SXALMarket> list = new SortedList<long, SXALMarket>();
                foreach (SXALMarket m in _listMarket.Values)
                {
                    if (!m.Name.Contains("Each Way ") &&
                        !m.Name.Equals("Place market", StringComparison.InvariantCultureIgnoreCase) &&
                        !m.Name.Equals("Win Market", StringComparison.InvariantCultureIgnoreCase))
                        list.Add(m.Id, m);
                }

                return list;
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {            
            AsyncMethodCaller caller = new AsyncMethodCaller(this.marketUpdater);
            IAsyncResult ar = caller.BeginInvoke(null, null);
            if (!ar.AsyncWaitHandle.WaitOne(60000))
            {
                DebugWriter.Instance.WriteMessage("SXAL", "SXALHorseMarketManager/_timer_Elapsed received Timeout");
            }
            else
                caller.EndInvoke(ar);            
        }

        private void marketUpdater()
        {
            lock (syncRoot)
            {
                DateTime dtsBegin = DateTime.Now.AddHours(-2.0);
                DateTime dtsEnd = DateTime.Now.AddHours(26.0);

                try
                {
                    _loadstate = LOADSTATE.UNLOADED;                    
                    long horseEventId = SXALEventManager.Instance.getEventIdByName("Horse Racing");
                    int?[] events = { (int)horseEventId };
                    DebugWriter.Instance.WriteMessage("SXALHorseMarketManager", String.Format("Read new Horse Markets. Event ID {0}. StartDate {1}. EndDate {2}", (int)horseEventId, dtsBegin, dtsEnd));
                    SXALMarket[] markets = SXALKom.Instance.getAllMarkets(events, dtsBegin, dtsEnd);
                    SXALKom.Instance.translateHorseMarkets(ref markets);
                    if (markets != null)
                    {
                        foreach (SXALMarket m in markets)
                        {
                            if (_listMarket.ContainsKey(m.Id))
                                _listMarket[m.Id] = m;
                            else
                                _listMarket.Add(m.Id,m);  
                        }
                    }

                    _loadstate = LOADSTATE.LOADED;
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
                    _loadstate = LOADSTATE.LOADED;
                }
                catch (Exception exc)
                {                    
                    _loadstate = LOADSTATE.LOADED;
                    ExceptionWriter.Instance.WriteException(exc);
                }
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
                DebugWriter.Instance.WriteMessage("SXALHorseMarketManager", "Disposing");
                if (disposing)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    if (_listMarket != null)
                    {
                        _listMarket.Clear();
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }
}