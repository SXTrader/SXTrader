using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    public sealed class SXALBalanceUpdatedEventArgs : EventArgs
    {
        private double m_balance;
        private double m_availible;
        private string m_currency;
        private object m_lock = "_lock";

        public String Currency
        {
            get
            {
                return m_currency;
            }
        }

        public Double Balance
        {
            get
            {
                lock (m_lock)
                {
                    return m_balance;
                }
            }
        }

        public Double Availible
        {
            get
            {
                lock (m_lock)
                {
                    return m_availible;
                }
            }
        }

        public SXALBalanceUpdatedEventArgs(double balance, double availible, string currency)
        {
            lock (m_lock)
            {
                m_balance = balance;
                m_availible = availible;
                m_currency = currency;
            }
        }

    }

    public sealed class SXALBankrollManager
    {
        private static volatile SXALBankrollManager instance;
        private static Object syncRoot = new Object();

        private Thread m_bankrollUpdater;
        private LOADSTATE m_loadstate;
        private double m_availBalance;
        private double m_balance;
        private double m_creditLimit;
        private String m_currency;
        private object lockRoot = "lockRoot";

        public event EventHandler<SXALBalanceUpdatedEventArgs> BalanceInfoUpdated;

      
        private SXALBankrollManager()
        {
            m_loadstate = LOADSTATE.UNLOADED;
            
            m_bankrollUpdater = new Thread(this.bankrollUpdater);
            m_bankrollUpdater.IsBackground = true;
            m_bankrollUpdater.Start();
        }

        public static SXALBankrollManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SXALBankrollManager();
                    }
                }

                return instance;
            }
        }

        public String Currency
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                {
                    Thread.Sleep(100);
                }
                
                return m_currency;
            }
            set
            {
                m_currency = value;
            }
        }

        public double MinStake
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                {
                    Thread.Sleep(100);
                }

                return SXALKom.Instance.MinStake;
            }
        }

        public double AvailibleBalance
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                {
                    Thread.Sleep(100);
                }

                return m_availBalance;

            }
        }

        public double TotalBalance
        {
            get
            {
                while (m_loadstate == LOADSTATE.UNLOADED)
                {
                    Thread.Sleep(100);
                }
                lock (lockRoot)
                {
                    return m_balance;
                }
            }
        }

        public void placedLayBet(double stake, double odds)
        {
            lock (lockRoot)
            {
                lock (syncRoot)
                {
                    while (m_loadstate == LOADSTATE.UNLOADED)
                    {
                        Thread.Sleep(100);
                    }

                    m_availBalance -= (stake * odds - stake);
                    m_availBalance = Math.Round(m_availBalance, 2);

                }
            }
        }

        public double getMoneyForLay(double percent, double odds)
        {
            lock (lockRoot)
            {
                lock (syncRoot)
                {
                    while (m_loadstate == LOADSTATE.UNLOADED)
                    {
                        Thread.Sleep(100);
                    }
                    double stake = m_balance * percent / 100;
                    if (stake * odds - stake > m_availBalance)
                        stake = 0;
                    else if (stake < this.MinStake)
                        stake = this.MinStake;

                    return Math.Round(stake);
                }
            }
        }

        private void bankrollUpdater()
        {
            TimeSpan timeToWait = new TimeSpan(0, 0, 0);
            while (true)
            {
                try
                {
                    Thread.Sleep(timeToWait);
                    lock (lockRoot)
                    {
                        lock (syncRoot)
                        {
                            SXALKom.Instance.getAccounFounds(out m_availBalance, out m_balance, out m_creditLimit);

                            EventHandler<SXALBalanceUpdatedEventArgs> handler = BalanceInfoUpdated;
                            if (handler != null)
                                handler(this, new SXALBalanceUpdatedEventArgs(m_balance, m_availBalance, m_currency));
                            m_loadstate = LOADSTATE.LOADED;
                        }
                    }
                    timeToWait = new TimeSpan(0, 1, 0);
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
            //TODO: Alles: Acountabfrage usw.
        }

        public void localSubtract(double amount)
        {
            lock (lockRoot)
            {
                m_availBalance = m_availBalance - amount;
            }
        }
    }
}
