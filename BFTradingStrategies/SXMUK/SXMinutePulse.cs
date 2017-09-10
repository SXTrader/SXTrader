using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

namespace net.sxtrader.muk
{
    public class SX5MinutePulse : IDisposable
    {
        private static volatile SX5MinutePulse instance;
        private System.Timers.Timer _minuteTimer;
        public event EventHandler<EventArgs> Pulse;
        private static Object syncRoot = "syncRoot5Pulse";
        private bool _disposed = false;

        public static SX5MinutePulse Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SX5MinutePulse();
                    }
                }

                return instance;
            }
        }

        private SX5MinutePulse()
        {
            _minuteTimer = new System.Timers.Timer(300000);
            _minuteTimer.Elapsed += new ElapsedEventHandler(_minuteTimer_Elapsed);
            _minuteTimer.Start();
        }

        private void _minuteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.signalPulse));
        }

        private void signalPulse(Object stateInfo)
        {
            EventHandler<EventArgs> pulse = Pulse;
            if (pulse != null)
            {
                pulse(this, new EventArgs());
            }
        }

        #region IDisposable Member
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_minuteTimer != null)
                    {
                        _minuteTimer.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }

    public class SXMinutePulse : IDisposable
    {
        private static volatile SXMinutePulse instance;
        private System.Timers.Timer _minuteTimer;
        public event EventHandler<EventArgs> Pulse;
        private static Object syncRoot = "syncRootPulse";
        private bool _disposed = false;

        public static SXMinutePulse Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SXMinutePulse();
                    }
                }

                return instance;
            }
        }

        private SXMinutePulse()
        {
            _minuteTimer = new System.Timers.Timer(60000);
            _minuteTimer.Elapsed += new ElapsedEventHandler(_minuteTimer_Elapsed);
            _minuteTimer.Start();
        }

        private void _minuteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.signalPulse));
        }

        private void signalPulse(Object stateInfo)
        {
            EventHandler<EventArgs> pulse = Pulse;
            if (pulse != null)
            {
                pulse(this, new EventArgs());
            }
        }

        #region IDisposable Member
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {               
                if (disposing)
                {
                    if (_minuteTimer != null)
                    {
                        _minuteTimer.Dispose();                        
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
