using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    public sealed class SXALEventManager
    {
        private static volatile SXALEventManager instance;
        private static Object syncRoot = new Object();

        private Thread m_eventUpdater;
        private SortedList<string, long> m_events;
        private LOADSTATE m_loadstate;

        private SXALEventManager()
        {
            m_loadstate = LOADSTATE.UNLOADED;
            m_events = new SortedList<string, long>();
            m_eventUpdater = new Thread(this.eventUpdater);
            m_eventUpdater.IsBackground = true;
            m_eventUpdater.Start();
        }

        public static SXALEventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SXALEventManager();
                    }
                }

                return instance;
            }
        }

        public long getEventIdByName(string name)
        {
            while (m_loadstate == LOADSTATE.UNLOADED)
                Thread.Sleep(1000);


            lock (syncRoot)
            {
                if (name == null)
                    return 0;
                try
                {
                    return m_events[name];
                }
                catch (KeyNotFoundException knfe)
                {
                    ExceptionWriter.Instance.WriteException(knfe);
                    return 0;
                }
            }
        }

        private void eventUpdater()
        {
            TimeSpan timeToWait = new TimeSpan();//dtsEnd.Subtract(DateTime.Now);            

            while (true)
            {
                Thread.Sleep(timeToWait);
                lock (syncRoot)
                {
                    m_loadstate = LOADSTATE.UNLOADED;
                    m_events.Clear();
                    try
                    {
                        SXALEventType[] events = SXALKom.Instance.loadEvents();
                        if (events == null)
                            continue;
                        foreach (SXALEventType eventType in events)
                        {
                            if (eventType.Name != null && eventType.Id != 0)
                            {
                                m_events.Add(eventType.Name, eventType.Id);
                            }
                        }                        
                        m_loadstate = LOADSTATE.LOADED;
                    }
                    catch (Exception e)
                    {
                        ExceptionWriter.Instance.WriteException(e);                        
                        timeToWait = new TimeSpan(0, 0, 10);
                        continue;
                    }
                }
                timeToWait = new TimeSpan(24, 0, 0);
            }
        }
    }
}
