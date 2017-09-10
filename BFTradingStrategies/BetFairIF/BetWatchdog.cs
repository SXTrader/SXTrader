using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using BetFairIF.com.betfair.api.exchange;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;

namespace net.sxtrader.bftradingstrategies.betfairif
{
    public class BetsUpdatedEventArgs : EventArgs
    {
    }

    public class BetWatchdog : IBFTSCommon
    {
        private static volatile BetWatchdog instance;
        private static Object syncRoot = new Object();


        private Thread m_eventUpdater;
        private int m_waitTime = 600;
        private DateTime m_dtsLastUpdate;
        private MUBet[] m_bets;
        private LOADSTATE m_loadstate;

        public event EventHandler<BetsUpdatedEventArgs>BetsUpdated;

        private BetWatchdog() 
        {
            m_loadstate = LOADSTATE.UNLOADED;
            m_dtsLastUpdate = DateTime.MinValue;
            m_eventUpdater = new Thread(this.betLoader);
            m_eventUpdater.IsBackground = true;
            m_eventUpdater.Start();


        }

        public static BetWatchdog Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BetWatchdog();
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

        public MUBet[] Bets
        {
            get
            {
                while (m_loadstate != LOADSTATE.LOADED)
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
                BetfairKom betfairKom = BetfairKom.Instance;

                
                try
                {
                    //MUBet[] bets = betfairKom.getBets();
                    MUBet[] bets = null;

                    bets = betfairKom.getBets(m_dtsLastUpdate);
                    if (bets != null)
                    {
                        m_bets = bets;
                        EventHandler<BetsUpdatedEventArgs> handler = BetsUpdated;
                        if (handler != null)
                            handler(this, new BetsUpdatedEventArgs());
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

    

        event EventHandler<SXWMessageEventArgs> MessageEvent;

        #endregion


        event EventHandler<SXWMessageEventArgs> IBFTSCommon.MessageEvent
        {
            add
            {
                lock (MessageEvent)
                {
                    MessageEvent += value;
                }
            }
            remove
            {
                lock (MessageEvent)
                {
                    MessageEvent -= value;
                }
            }
        }
    }
}
