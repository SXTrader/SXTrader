using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    public class SXALMarketWatcherPricesUpdate : EventArgs
    {
        private SXALMarketPrices _marketPrices;
        private SXALMarket _market;

        public SXALMarketPrices MarketPrices
        {
            get { return _marketPrices; }
        }

        public SXALMarket Market
        {
            get { return _market; }
        }

        public SXALMarketWatcherPricesUpdate(SXALMarket market, SXALMarketPrices marketPrices)
        {
            _market = market;
            _marketPrices = marketPrices;
        }

    }
    public class SXALMarketWatcher
    {
        private SXALMarket _market = null;
        private Thread m_marketUpdater;

        public event EventHandler<SXALMarketWatcherPricesUpdate> MarketPriceUpdate;


        public SXALMarketWatcher(long marketId)
        {
            _market = SXALSoccerMarketManager.Instance.getMarketById(marketId, true);
            m_marketUpdater = new Thread(this.marketUpdater);
            m_marketUpdater.IsBackground = true;
            m_marketUpdater.Start();
        }


        public void Stop()
        {
            if (m_marketUpdater != null)
            {
                try
                {
                    m_marketUpdater.Abort();
                }
                catch
                {
                }

            }
        }

        private void marketUpdater()
        {
            try
            {
                while (true)
                {
                    if (_market == null)
                        return;
                    //Marktpreise laden
                    SXALMarketPrices prices = SXALKom.Instance.getMarketPrices(_market.Id);

                    if (prices != null)
                    {
                        EventHandler<SXALMarketWatcherPricesUpdate> marketWatcherPricesUpdate = MarketPriceUpdate;
                        if (marketWatcherPricesUpdate != null)
                        {
                            //TODO: Allgemeine Lösung finden
                            marketWatcherPricesUpdate(this, new SXALMarketWatcherPricesUpdate(_market, prices));
                        }
                    }

                    //Warte gegebene Zeit
                    TimeSpan waitSpan = new TimeSpan(0, 0, 20);
                    Thread.Sleep(waitSpan);
                }
            }
            catch (SXALMarketDoesNotExistException mdnee)
            {
                ExceptionWriter.Instance.WriteException(mdnee);
            }
            catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
            {
                ExceptionWriter.Instance.WriteException(mnsnae);
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
