using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BetFairIF.com.betfair.api;
using System.Diagnostics;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.betfairif
{
    public sealed class EventManager
    {
        private static volatile EventManager instance;
        private static Object syncRoot = new Object();

        private Thread m_eventUpdater;
        private SortedList<string,int> m_events;
        private LOADSTATE m_loadstate;

        private EventManager()
        {
            m_loadstate = LOADSTATE.UNLOADED;
            m_events = new SortedList<string,int>();
            m_eventUpdater = new Thread(this.eventUpdater);
            m_eventUpdater.IsBackground = true;
            m_eventUpdater.Start();
        }

        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new EventManager();
                    }
                }

                return instance;
            }
        }

        public int getEventIdByName(string name)
        {
            while (m_loadstate == LOADSTATE.UNLOADED)
                Thread.Sleep(1000);


            lock(syncRoot)
            {
                if(name == null)
                    return 0;
                try
                {
                     return m_events[name];
                }
                catch(KeyNotFoundException knfe)
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
                lock(syncRoot)
                {
                    m_loadstate = LOADSTATE.UNLOADED;
                    m_events.Clear();
                    try
                    {
                        EventType[] events = BetfairKom.Instance.loadEvents();
                        if (events == null)
                            continue;
                        foreach (EventType eventType in events)
                        {
                            if (eventType.name != null && eventType.id != 0)
                            {
                                m_events.Add(eventType.name, eventType.id);
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

