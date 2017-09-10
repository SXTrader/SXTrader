using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.betfairif
{
    public class BFBalanceUpdatedEventArgs : EventArgs
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

        public BFBalanceUpdatedEventArgs(double balance, double availible, string currency)
        {
            lock (m_lock)
            {
                m_balance = balance;
                m_availible = availible;
                m_currency = currency;
            }
        }

    }

    public sealed class BankrollManager
    {
        private static volatile BankrollManager instance;
        private static Object syncRoot = new Object();

        private Thread m_bankrollUpdater;
        private LOADSTATE m_loadstate;
        private double m_availBalance;
        private double m_balance;
        private double m_creditLimit;
        private String m_currency;
        private object lockRoot = "lockRoot";

        public event EventHandler<BFBalanceUpdatedEventArgs> BalanceInfoUpdated;

        private const String GBP = "GBP";
        private const int MINGBP = 2;
        private const String EUR = "EUR";
        private const int MINEUR = 2;
        private const String AUD = "AUD";
        private const int MINAUD = 5;
        private const String USD = "USD";
        private const int MINUSD = 4;
        private const String CAD = "CAD";
        private const int MINCAD = 6;
        private const String SGD = "SGD";
        private const int MINSGD = 6;
        private const String HKD = "HKD";
        private const int MINHKD = 25;
        private const String NOK = "NOK";
        private const int MINNOK = 30;
        private const String DKK = "DKK";
        private const int MINDKK = 30;
        private const String SEK = "SEK";
        private const int MINSEK = 30;

        private BankrollManager()
        {
            m_loadstate = LOADSTATE.UNLOADED;
            
            m_bankrollUpdater = new Thread(this.bankrollUpdater);
            m_bankrollUpdater.IsBackground = true;
            m_bankrollUpdater.Start();
        }

        public static BankrollManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BankrollManager();
                    }
                }

                return instance;
            }
        }

        public String Currency
        {
            get
            {
                while (m_loadstate != LOADSTATE.LOADED)
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
                while (m_loadstate != LOADSTATE.LOADED)
                {
                    Thread.Sleep(100);
                }

                double minStake = 0.0;
                //TODO: MinStake abhängig von Währung
                switch (m_currency)
                {
                    case GBP:
                        minStake = MINGBP;
                        break;
                    case EUR:
                        minStake = MINEUR;
                        break;
                    case AUD:
                        minStake = MINAUD;
                        break;
                    case USD:
                        minStake = MINUSD;
                        break;
                    case CAD:
                        minStake = MINCAD;
                        break;
                    case SGD:
                        minStake = MINSGD;
                        break;
                    case HKD:
                        minStake = MINHKD;
                        break;
                    case NOK:
                        minStake = MINNOK;
                        break;
                    case DKK:
                        minStake = MINDKK;
                        break;
                    case SEK:
                        minStake = MINSEK;
                        break;
                    default:
                        throw new Exception(String.Format("Unknow currency {0}. No minimum bet amount known.", m_currency));
                }
                return minStake;//BetfairKom.Instance.getMinStakeForCurrency(m_currency);
            }
        }

        public double AvailibleBalance
        {
            get
            {
                while (m_loadstate != LOADSTATE.LOADED)
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
                while (m_loadstate != LOADSTATE.LOADED)
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
                    while (m_loadstate != LOADSTATE.LOADED)
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
                    while (m_loadstate != LOADSTATE.LOADED)
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
                            BetfairKom.Instance.getAccounFounds(out m_availBalance, out m_balance, out m_creditLimit);

                            EventHandler<BFBalanceUpdatedEventArgs> handler = BalanceInfoUpdated;
                            if (handler != null)
                                handler(this, new BFBalanceUpdatedEventArgs(m_balance, m_availBalance, m_currency));
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
