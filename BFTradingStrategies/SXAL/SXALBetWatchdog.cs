using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.muk.interfaces;
using System.Threading;
using net.sxtrader.muk.eventargs;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    public class SXALBetsUpdatedEventArgs : EventArgs
    {
    }

    public class SXALBetWatchdog : IBFTSCommon
    {
        private static volatile SXALBetWatchdog instance;
        private static Object syncRoot = new Object();


        private Thread m_eventUpdater;
        private int m_waitTime = 600;
        private DateTime m_dtsLastUpdate;
        private SXALMUBet[] m_bets;
        private LOADSTATE m_loadstate;

        public event EventHandler<SXALBetsUpdatedEventArgs> BetsUpdated;

        private SXALBetWatchdog()
        {
            m_loadstate = LOADSTATE.UNLOADED;
            m_dtsLastUpdate = DateTime.MinValue;
            m_eventUpdater = new Thread(this.betLoader);
            m_eventUpdater.IsBackground = true;
            m_eventUpdater.Start();            
        }

        public static SXALBetWatchdog Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SXALBetWatchdog();
                    }
                }

                return instance;
            }
        }

        public int WaitTime
        {
            get
            {
                return m_waitTime;
            }
            set
            {
                m_waitTime = value;
            }
        }

        public SXALMUBet[] Bets
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                {
                    Thread.Sleep(100);
                }

                return m_bets;
            }
        }

        private void betLoader()
        {
            while (true)
            {
                SXALKom sxalKom = SXALKom.Instance;

                //DebugWriter.Instance.WriteMessage("BetWatchdog", String.Format("looking for bets newer than {0}", m_dtsLastUpdate.ToString()));
                try
                {
                    //MUBet[] bets = betfairKom.getBets();
                    SXALMUBet[] bets = null;

                    bets = sxalKom.getBets(m_dtsLastUpdate);
                    if (bets != null && bets.Length != 0)
                    {
                        m_bets = bets;
                        EventHandler<SXALBetsUpdatedEventArgs> handler = BetsUpdated;
                        if (handler != null)
                            handler(this, new SXALBetsUpdatedEventArgs());
                    }

                    

                    if (bets != null)
                    {
                        //m_dtsLastUpdate = DateTime.Now;
                    }
                    m_loadstate = LOADSTATE.LOADED;
                    TimeSpan span = new TimeSpan(0, 0, m_waitTime);
                    Thread.Sleep(span);
                }
                catch (Exception exc)
                {
                    EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                    if (handler != null)
                    {
                        handler(this, new SXExceptionMessageEventArgs("BetWatchdog::betLoader", exc.ToString()));
                    }

                    ExceptionWriter.Instance.WriteException(exc);
                    Thread.Sleep(1000);
                    continue;
                    //TODO: Fehler nach aussen geben
                }
            }
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;
        public event EventHandler<SXWMessageEventArgs> MessageEvent;
        #endregion
    }
}
