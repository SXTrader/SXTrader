using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.betfairif;
using net.sxtrader.betdaqif;
using System.Windows.Forms;
using net.sxtrader.pinnacleif;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    /// <summary>
    /// Abstraktionsschicht für die Ansprache der Unterschiedlichen Wettbörsen
    /// </summary>
    public sealed class SXALKom : IBFTSCommon
    {
        /// <summary>
        /// Singleton Instanz
        /// </summary>
        private static volatile SXALKom _instance;
        /// <summary>
        /// Synchronisierter Zugriff auf die Singleton-Instanz
        /// </summary>
        private static Object _syncRoot = "syncRoot";
        private EXCHANGES _exchange = EXCHANGES.UNASSIGNED;
        private ISXAL _theExchange = null;
        public static event EventHandler<EventArgs> ShutdownRequest;

        private SXALKom() 
        {
            lock (_syncRoot)
            {
                using (frmLogin frm = new frmLogin())
                {
                    if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _exchange = frm.Exchange;
                        _theExchange = getExchange();
                        if (_theExchange != null)
                        {                            
                            try
                            {
                                _theExchange.login(frm.Usr, frm.Pwd);
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(exc.Message, "Error while Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                throw exc;
                            }
                        }
                    }
                    else
                    {
                        EventHandler<EventArgs> handler = ShutdownRequest;
                        if (handler != null)
                        {
                            handler(this, new EventArgs());                            
                        }
                        else
                        {
                            Environment.Exit(0);                            
                        }
                    }
                }

            }
        }

        private ISXAL getExchange()
        {
            ISXAL theExchange = null;

            switch (_exchange)
            {
                case EXCHANGES.BETFAIR:
                    theExchange = BetfairKom.Instance;
                    break;
                case EXCHANGES.BETDAQCOM:
                    BetdaqKom.Instance.IsUKExchange = false;
                    theExchange = BetdaqKom.Instance;
                    break;
                case EXCHANGES.BETDAQUK:
                    BetdaqKom.Instance.IsUKExchange = true;
                    theExchange = BetdaqKom.Instance;
                    break;
                case EXCHANGES.PINBET88:
                    theExchange = PinnacleKom.Instance;
                    break;
            }

            return theExchange;
        }

        static SXALKom()
        {
            //Dialog mit Login-Info-Abfrage
            // 1. Wettbörsentyp
            // 2. Nutzer + Pwd
        }

        public static SXALKom Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new SXALKom();
                    }
                }

                return _instance;
            }
        }


        public void login()
        {

        }

        public SXALSelection[] getSelections(SXALMarket market)
        {
            return _theExchange.getSelections(market);
        }

        public void translateHorseMarkets(ref SXALMarket[] marketsToTranslate)
        {
            for (int i = 0; i < marketsToTranslate.Length; i++)
            {
                marketsToTranslate[i] = _theExchange.translateHorseMarket(marketsToTranslate[i]);
            }
        }

        public String getCurrency()
        {
            return _theExchange.getCurrency();
        }

        public SXALBet placeLayBet(long marketId, long selectionId, double price, double money) 
        {
            //throw new NotImplementedException();
            return placeLayBet(marketId, selectionId, 0, false, price, money);
        }

        public SXALBet placeLayBet(long marketId, long selectionId, int asianId, double price, double money)
        {
            return placeLayBet(marketId, selectionId, asianId, false, price, money);
        }

        public SXALBet placeLayBet(SXALMarket market, long selectionId, int asianId, bool keepBet, double price, double money)
        {
            bool isInplay = false;
            
            if (market != null && market.StartDTS < DateTime.Now)
                isInplay = true;

            return _theExchange.placeLayBet(market.Id, selectionId, asianId, keepBet, price, money, isInplay);
        }

        public SXALBet placeLayBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money)
        {
            bool isInplay = false;
            SXALMarket m = SXALSoccerMarketManager.Instance.getMarketById(marketId,true);
            if (m != null && m.StartDTS < DateTime.Now)
                isInplay = true;

            return _theExchange.placeLayBet(marketId, selectionId, asianId, keepBet, price, money, isInplay);           
        }

     
        public SXALBet placeBackBet(long marketId, long selectionId, double price, double money)
        {
            return placeBackBet(marketId, selectionId, 0, false, price, money);
        }

        public SXALBet placeBackBet(long marketId, long selectionId, int asianId, double price, double money)
        {
            return placeBackBet(marketId, selectionId, asianId, false, price, money);
        }

        public SXALBet placeBackBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money)
        {
            bool isInplay = false;
            SXALMarket m = SXALSoccerMarketManager.Instance.getMarketById(marketId, true);
            if (m != null && m.StartDTS < DateTime.Now)
                isInplay = true;
            return _theExchange.placeBackBet(marketId, selectionId, asianId, keepBet, price, money, isInplay);
        }
      

        public bool cancelBet(long betId)
        {
            return _theExchange.cancelBet(betId);            
        }


        public long getSelectionId(SXALSelectionIdEnum selectionToGet, long marketId)
        {
            SXALMarket m = SXALSoccerMarketManager.Instance.getMarketById(marketId, true);

            if (m != null)
                return _theExchange.getSelectionId(selectionToGet, m);
            return -1;
        }

        public SXALSelectionIdEnum getReverseSelectionId(long selectionId, long marketId)
        {
            SXALMarket market = SXALSoccerMarketManager.Instance.getMarketById(marketId, true);
            if (market != null)
                return _theExchange.getReverseSelectionId(selectionId, market);

            throw new NotImplementedException();
        }

        public SXALMarketPrices getMarketPrices(long marketId)
        {
            return getMarketPrices(marketId, false);
        }

        public SXALMarketPrices getMarketPrices(long marketId, bool canThrowThrottleExceeded)
        {
            SXALMarketPrices prices = _theExchange.getMarketPrices(marketId, canThrowThrottleExceeded);
            if (prices != null)
            {
                prices.CurrencyCode = SXALBankrollManager.Instance.Currency;
            }
            return prices;
            
        }

        public void getAccounFounds(out double availBalance, out double currentBalance, out double creditLimit)
        {
            availBalance = currentBalance = creditLimit = 0.0;

            _theExchange.getAccounFounds(out availBalance, out currentBalance, out creditLimit);

            SXALBankrollManager.Instance.Currency = _theExchange.getCurrency();
        }

        public SXALBet getBetDetail(long betId)
        {
            return _theExchange.getBetDetail(betId);            
        }

        public SXALMUBet[] getBetsMU(long marketId)
        {
            return _theExchange.getBetsMU(marketId);            
        }

        public SXALMUBet[] getBetMU(long betId)
        {
            return _theExchange.getBetMU(betId);           
        }

        public SXALMarket[] getAllMarkets(int?[] eventids, DateTime fromDate, DateTime toDate)
        {
            return _theExchange.getAllMarkets(eventids, fromDate, toDate);
            
        }

        public SXALMUBet[] getBets()
        {
            return _theExchange.getBets(DateTime.MinValue);
        }

        public SXALMUBet[] getBets(DateTime dts)
        {
            return _theExchange.getBets(dts);
        }

        public SXALMarketLite getMarketInfo(long marketId)
        {
            return _theExchange.getMarketInfo(marketId);            
        }


        public SXALEventType[] loadEvents()
        {
            return _theExchange.loadEvents();
        }

        public decimal validateOdd(decimal odds)
        {
            return _theExchange.validateOdd(odds);
        }

        public decimal getValidOddIncrement(decimal odds)
        {
            return _theExchange.getValidOddIncrement(odds);
        }

        public double MinStake
        {
            get
            {
                return _theExchange.MinStake;
            }
        }

        public bool SupportsBelowMinStakeBetting { get { return _theExchange.SupportsBelowMinStakeBetting; } }

        public bool isUnder(long selectionId, String match)
        {
            return false;
            /*
            if (selectionId == (int)OVERUNDERSELCTIONIDS.UNDER05 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.UNDER15 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.UNDER25 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.UNDER35 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.UNDER45 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.UNDER55 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.UNDER65 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.UNDER75 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.UNDER85)
                return true;
            return false;
             * */
        }

        public bool isOver(long selectionId, String match)
        {
            return true;
            /*
            if (selectionId == (int)OVERUNDERSELCTIONIDS.OVER05 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.OVER15 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.OVER25 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.OVER35 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.OVER45 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.OVER55 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.OVER65 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.OVER75 ||
                selectionId == (int)OVERUNDERSELCTIONIDS.OVER85)
                return true;
            return false;
             */
        }

        public String getExchangeName()
        {
            return _theExchange.getExchangeName();
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;
        public event EventHandler<SXWMessageEventArgs> MessageEvent;
        #endregion
    }
}
