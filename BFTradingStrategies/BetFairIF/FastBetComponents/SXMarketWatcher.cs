using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.betfairif;
using System.Threading;
using BetFairIF.com.betfair.api.exchange;

namespace BetFairIF.FastBetComponents
{
    public class SXMarketWatcherPricesUpdate : EventArgs
    {
        private MarketPrices _marketPrices;        
        private BFMarket _market;

        public MarketPrices MarketPrices
        {
            get { return _marketPrices; }
        }

        public BFMarket Market
        {
            get { return _market; }
        }

        public SXMarketWatcherPricesUpdate(BFMarket market, MarketPrices marketPrices)
        {
            _market = market;
            _marketPrices = marketPrices;
        }

    }
    public class SXMarketWatcher
    {
        private BFMarket _market = null;
        private Thread m_marketUpdater;

        public event EventHandler<SXMarketWatcherPricesUpdate> MarketPriceUpdate;


        public SXMarketWatcher(int marketId)
        {
            _market = SoccerMarketManager.Instance.getMarketById(marketId);
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
                    MarketPrices prices = BetfairKom.Instance.getMarketPrices(_market.Id);

                    EventHandler<SXMarketWatcherPricesUpdate> marketWatcherPricesUpdate = MarketPriceUpdate;
                    if (marketWatcherPricesUpdate != null)
                    {
                        //TODO: Allgemeine Lösung finden
                        marketWatcherPricesUpdate(this, new SXMarketWatcherPricesUpdate(_market, prices));
                    }

                    //Warte gegebene Zeit
                    TimeSpan waitSpan = new TimeSpan(0, 0, 20);
                    Thread.Sleep(waitSpan);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }
    }
}
