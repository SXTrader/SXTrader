using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using BetFairIF.com.betfair.api;
using BetFairIF.com.betfair.api.exchange;
using System.Timers;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using System.Globalization;
using System.Xml.Linq;
using net.sxtrader.muk;
using System.Threading.Tasks;


namespace net.sxtrader.bftradingstrategies.betfairif
{
    public sealed partial class BetfairKom : IBFTSCommon, IDisposable
    {
        private static volatile BetfairKom instance;
        private static Object syncRoot = new Object();
        private static Object placeBetLock = "placeBetLock";

        private string usr = String.Empty;
        private string pwd = String.Empty;
        private string _currency = String.Empty;
        private XDocument _horseMarketMappingDoc;
        private bool _disposed = false;

        public const int DRAWSELECTIONID = 58805;
        public const int UNDER25SELECTIONID = 47972;
        public const int OVER25SELECTIONID = 47973;

        public enum SCORESELECTIONIDS
        {
            ZEROTOZERO = 1, ZEROTOONE = 4, ZEROTOTWO = 9, ZEROTOTHREE = 16,
            ONETOZERO = 2, ONETOONE = 3, ONETOTWO = 8, ONETOTHREE = 15,
            TWOTOZERO = 5, TWOTOONE = 6, TWOTOTWO = 7, TWOTOTHREE = 14,
            THREETOZERO = 10, THREETOONE = 11, THREETOTWO = 12, THREETOTHREE = 13,
            OTHERS = 4506345
        };

        public enum OVERUNDERSELCTIONIDS
        {
            UNDER05 = 5851482, OVER05 = 5851483,
            UNDER15 = 1221385, OVER15 = 1221386,
            UNDER25 = 47972,   OVER25 = 47973,
            UNDER35 = 1222344, OVER35 = 1222345,
            UNDER45 = 1222347, OVER45 = 1222348,
            UNDER55 = 1485567, OVER55 = 1485568,
            UNDER65 = 2542448, OVER65 = 2542449,
            UNDER75 = 1485572, OVER75 = 1485573,
            UNDER85 = 2407528, OVER85 = 2407529
        }

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

        /// <summary>
        /// Der Betfair-Session-String. Er ist für ein Login immer eindeutig und muss bei
        /// jeder Abfrage mitgegeben werden. Sollte der Sessionstring der Antwort sich unterscheiden
        /// so muss bei jeder zukünftigen Abfrage der neue Sessionstring benutzt werden.
        /// </summary>
        private string _session;

        /// <summary>
        /// Timer für den Hearbeat an den Betfairserver, wenn 20 Minuten keine aktion erfolgt ist.
        /// </summary>
        private System.Timers.Timer _heartbeat;


        private BetfairKom() 
        { 
            // Lade Mapping Abkürzung Rennbahn zu Rennbahn
            loadHorseRaceMapping();
        }

        public static event EventHandler<EventArgs> ShutdownRequest;

        public static BetfairKom Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BetfairKom();
                    }
                }

                return instance;
            }
        }

        public void logout()
        {
            lock (syncRoot)
            {
                try
                {
                    //string session = SessionTokenManager.GetSessionToken();
                    if (_session != null)
                    {                                                
                        BetFairIF.com.betfair.api.APIRequestHeader header = new BetFairIF.com.betfair.api.APIRequestHeader();
                        header.sessionToken = _session;
                        //LogoutReq req = new LogoutReq();
                        BetFairIF.com.betfair.api.LogoutReq req = new LogoutReq();                        

                        req.header = header;
                        BFGlobalService bfService = new BFGlobalService();                        
                        
                        LogoutResp resp = bfService.logout(req);
                        

                        if(resp.header.errorCode != BetFairIF.com.betfair.api.APIErrorEnum.OK)
                        //if (resp.header.errorCode != BetFairIF.com.betfair.api.APIErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::logout");
                        }
                    }
                }
                catch (NoSessionException)
                {
                    // Alles i.O, wir wollten sowieso ausloggen
                }
                catch (ThrottleExceededException)
                {
                    // Alles i.O.
                }
            }
        }

        

        public double getMinStakeForCurrency(string currency)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }

                        GetCurrenciesV2Req req = new GetCurrenciesV2Req();
                        req.header = new BetFairIF.com.betfair.api.APIRequestHeader();
                        req.header.sessionToken = _session;

                        GetCurrenciesV2Resp resp = new BFGlobalService().getAllCurrenciesV2(req);

                        setHeartbeat();

                        /*
                        if (resp.header.errorCode == BetFairIF.com.betfair.api.APIErrorEnum.EXCEEDED_THROTTLE)
                        {

                            SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                            continue;
                        }
                        */
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getMInStakeForCurrency");
                        }

                        CurrencyV2[] currencyItems = resp.currencyItems;

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        foreach (CurrencyV2 item in currencyItems)
                        {
                            if (item.currencyCode == currency)
                                return (double)item.minimumStake;
                        }
                    }
                    catch (NoSessionException)
                    {
                        // Session ist abgelaufen => Token löschen und nochmal
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
               
                }
                
            }
        }

        public void getAccounFounds(out double availBalance, out double currentBalance, out double creditLimit)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }

                        GetAccountFundsReq req = new GetAccountFundsReq();
                        req.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        req.header.sessionToken = _session;

                        GetAccountFundsResp resp = new BFExchangeService().getAccountFunds(req);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        if (resp.errorCode == GetAccountFundsErrorEnum.EXPOSURE_CALC_IN_PROGRESS)
                        {
                            //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                            continue;
                        }

                        /*
                        if (resp.header.errorCode == BetFairIF.com.betfair.api.exchange.APIErrorEnum.EXCEEDED_THROTTLE)
                        {
                            
                            //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                            continue;
                        }
                         */
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getAccountFounds");
                        }
                        availBalance = resp.availBalance;
                        currentBalance = resp.balance;
                        creditLimit = resp.creditLimit;
                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        return;
                    }
                    catch (NoSessionException)
                    {
                        // Session ist abgelaufen => Token löschen und nochmal
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                }
            }
        }

        public String getCompleteMarketPrices(int marketId)
        {
            while(true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }

                        BFGlobalService bfGlobal = new BFGlobalService();
                        GetCompleteMarketPricesCompressedReq req = new GetCompleteMarketPricesCompressedReq();
                        req.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        req.header.sessionToken = _session;

                        req.marketId = marketId;

                        GetCompleteMarketPricesCompressedResp resp = new BFExchangeService().getCompleteMarketPricesCompressed(req);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        /*
                        if (resp.header.errorCode == BetFairIF.com.betfair.api.exchange.APIErrorEnum.EXCEEDED_THROTTLE)
                        {

                            //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                            continue;
                        }
                         * */
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getCompleteMarketPrices");
                            //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                            return null;
                        }

                        if (resp.errorCode != GetCompleteMarketPricesErrorEnum.OK)
                        {
                            GCMPErrorResolver(resp.errorCode, "BetfairKom:getCompleteMarketPrices");
                            return null;
                        }


                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        return resp.completeMarketPrices;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }
            }
        }

        public void GCMPErrorResolver(GetCompleteMarketPricesErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case GetCompleteMarketPricesErrorEnum.EVENT_CLOSED:
                    msg = String.Format("Error {0}: The market has closed.", error.ToString());
                    break;
                case GetCompleteMarketPricesErrorEnum.EVENT_INACTIVE:
                    msg = String.Format("Error {0}: The market is inactive.", error.ToString());
                    break;
                case GetCompleteMarketPricesErrorEnum.EVENT_SUSPENDED:
                    msg = String.Format("Error {0}: The market is suspended.", error.ToString());
                    break;
                case GetCompleteMarketPricesErrorEnum.INVALID_CURRENCY:
                    msg = String.Format("Error {0}: Currency code not a valid 3 letter ISO 4217 currency abbreviation.", error.ToString());                    
                    break;
                case GetCompleteMarketPricesErrorEnum.INVALID_MARKET:
                    msg = String.Format("Market ID is not a valid market id. " +
                            "Check that you have sent your service request to the correct exchange server " +
                            "(the Australian exchange server cannot see markets on the UK exchange server, and vice versa)."
                            , error.ToString());                    
                    break;
                case GetCompleteMarketPricesErrorEnum.MARKET_TYPE_NOT_SUPPORTED:
                    msg = String.Format("Error {0}: The MarketID supplied refers to a market that is not supported by the API.", error.ToString());                    
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }
            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);            
        }

        public  Market getMarket(int marketId)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }

                        
                        GetMarketReq req = new GetMarketReq();
                        req.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        req.header.sessionToken = _session;
                        req.locale = "en";
                        req.marketId = marketId;

                        GetMarketResp resp = new BFExchangeService().getMarket(req);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getAllMarkets");
                            return null;
                        }
                        if (resp.errorCode != GetMarketErrorEnum.OK)
                        {
                            GMErrorResolver(resp.errorCode, "BetfairKom::getAllMarkets");
                        }
                        
                        return resp.market;

                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (InvalidOperationException ioe)
                    {
                        ExceptionWriter.Instance.WriteException(ioe);
                        Thread.Sleep(1000);
                        continue;
                    }
                }
            }
        }

        public String getAllMarkets(int?[] eventids, DateTime fromDate, DateTime toDate)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }

                        BFGlobalService bfGlobal = new BFGlobalService();
                        GetAllMarketsReq req = new GetAllMarketsReq();
                        req.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        req.header.sessionToken = _session;
                        req.locale = "en";
                        req.eventTypeIds = eventids;
                        req.fromDate = fromDate;
                        req.toDate = toDate;

                        GetAllMarketsResp resp = new BFExchangeService().getAllMarkets(req);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getAllMarkets");
                            return null;
                        }
                        if (resp.errorCode != GetAllMarketsErrorEnum.OK)
                        {
                            GAMErrorResolver(resp.errorCode, "BetfairKom::getAllMarkets");
                        }
                        return resp.marketData;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (InvalidOperationException ioe)
                    {
                        ExceptionWriter.Instance.WriteException(ioe);
                        Thread.Sleep(1000);
                        continue;
                    }
                }
            }
        }

        private void GMErrorResolver(GetMarketErrorEnum error, string caller)
        {
            String ms = String.Empty;
            switch (error)
            {
                case GetMarketErrorEnum.API_ERROR:
                    break;
                case GetMarketErrorEnum.INVALID_LOCALE_DEFAULTING_TO_ENGLISH:
                    break;
                case GetMarketErrorEnum.INVALID_MARKET:
                    break;
                case GetMarketErrorEnum.MARKET_TYPE_NOT_SUPPORTED:
                    break;
                default:
                    break;
            }

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                //handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            //ExceptionWriter.Instance.WriteMessage(caller, msg);
        }

        private void GAMErrorResolver(GetAllMarketsErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case GetAllMarketsErrorEnum.INVALID_COUNTRY_CODE:
                    msg = String.Format("Error {0}: The country code submitted does not match a known country code.", error.ToString());
                    break;
                case GetAllMarketsErrorEnum.INVALID_EVENT_TYPE_ID:
                    msg = String.Format("Error {0}: The eventTypeId does not match any known eventTypes.", error.ToString());
                    break;
                case GetAllMarketsErrorEnum.INVALID_LOCALE:
                    msg = String.Format("Error {0}: The locale specified does not exist.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);
        }


        public EventType[] loadEvents()
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }

                        BFGlobalService bfGlobal = new BFGlobalService();
                        GetEventTypesReq req = new GetEventTypesReq();
                        req.header = new BetFairIF.com.betfair.api.APIRequestHeader();
                        req.header.sessionToken = _session;
                        req.locale = "en";

                        GetEventTypesResp resp = bfGlobal.getAllEventTypes(req);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.APIErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::loadEvents");
                            return null;
                        }
                        if (resp.errorCode != GetEventsErrorEnum.OK)
                        {
                            LEErrorResolver(resp.errorCode, "BetfairKom::loadEvents");
                        }
                        return resp.eventTypeItems;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }
            }
        }

        private void LEErrorResolver(GetEventsErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case GetEventsErrorEnum.INVALID_EVENT_ID:
                    msg = String.Format("Error {0}: Not used.", error.ToString());
                    break;
                case GetEventsErrorEnum.INVALID_LOCALE_DEFAULTING_TO_ENGLISH:
                    msg = String.Format("Error {0}: The locale string was not recognized. Returned results are in English.", error.ToString());
                    break;
                case GetEventsErrorEnum.NO_RESULTS:
                    msg = String.Format("Error {0}: No data available to return.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);
        }

        public bool cancelBet(long betId)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        // get a session or login
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }
                        BFGlobalService bfGlobal = new BFGlobalService();
                        CancelBetsReq cancelBetsReq = new CancelBetsReq();
                        cancelBetsReq.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        cancelBetsReq.header.sessionToken = _session;
                        CancelBets cancelBets = new CancelBets();
                        cancelBets.betId = betId;
                        cancelBetsReq.bets = new CancelBets[1];
                        cancelBetsReq.bets[0] = cancelBets;
                        CancelBetsResp resp = new BFExchangeService().cancelBets(cancelBetsReq);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::cancelBet");
                            return false;
                        }
                        if (resp.errorCode == CancelBetsErrorEnum.OK)
                        {
                            if (resp.betResults[0].success == false)
                            {
                                // Fehlgeschlagen 

                            }
                            else
                            {

                            }

                            return resp.betResults[0].success;
                        }
                        else
                        {
                            if (resp.errorCode == CancelBetsErrorEnum.BET_IN_PROGRESS ||
                                resp.errorCode == CancelBetsErrorEnum.MARKET_STATUS_INVALID)
                            {
                                

                            }
                            else
                            {
                                CBErrorResolver(resp.errorCode, "BetfairKomm:cancelBet");
                            }
                        }
                        return false;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }
            }
        }

        private void CBErrorResolver(CancelBetsErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case CancelBetsErrorEnum.BBM_DAEMON_NOT_AVAILABLE:
                    msg = String.Format("Error {0}: The exchange encountered a critical error and was not able to cancel your bet. " +
                            "Your bet was not cancelled.", error.ToString());
                    break;
                case CancelBetsErrorEnum.INVALID_MARKET_ID:
                    msg = String.Format("Error {0}: The bets were not all from the same market.", error.ToString());
                    break;
                case CancelBetsErrorEnum.INVALID_NUMER_OF_CANCELLATIONS:
                    msg = String.Format("Error {0}: Number of bets < 1 or > 40.", error.ToString());
                    break;
                case CancelBetsErrorEnum.MARKET_IDS_DONT_MATCH:
                    msg = String.Format("Error {0}: Bet ID does not exist.", error.ToString());
                    break;
                case CancelBetsErrorEnum.MARKET_TYPE_NOT_SUPPORTED:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Invalid Market type.", error.ToString());
                    break;
            }

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);
        }




        public Bet placeLayBetBelowMin(int marketId, int selectionId, int asianId, bool keepBet, double price, double money)
        {
            lock (placeBetLock)
            {
                long betId = 0;
                Bet myBet = null;
                #region Pseudewette zu Quote 1.01
                while (true)
                {
                    lock (syncRoot)
                    {
                        betId = 0;
                        // 1. Wette absetzen zur Quote 1.01 absetzen
                        try
                        {
                            // Get a session, or login 
                            // string session = SessionTokenManager.GetSessionToken();
                            if (_session == null || _session == String.Empty)
                            {
                                login();
                                //session = SessionTokenManager.GetSessionToken();
                            }
                            BFGlobalService bfGlobal = new BFGlobalService();
                            PlaceBetsReq placeBetsReq = new PlaceBetsReq();
                            placeBetsReq.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                            placeBetsReq.header.sessionToken = _session;
                            PlaceBets bet = new PlaceBets();
                            bet.marketId = marketId;
                            bet.selectionId = selectionId;
                            bet.betType = BetTypeEnum.L;
                            /////////////////////////////////////////////////////////////////////////////////////////////
                            // Place lowest allowed Lay-price
                            bet.price = 1.01;
                            /////////////////////////////////////////////////////////////////////////////////////////////
                            // Place Minimum allowed Money
                            bet.size = getMinStakeForCurrency(getCurrency());//BankrollManager.Instance.MinStake;
                            ////////////////////////////////////////////////////////////////////////////////////////////
                            bet.asianLineId = asianId;
                            bet.bspLiability = 0.0;
                            bet.betCategoryType = BetCategoryTypeEnum.E;
                            if (keepBet)
                                bet.betPersistenceType = BetPersistenceTypeEnum.IP;
                            else
                                bet.betPersistenceType = BetPersistenceTypeEnum.NONE;
                            placeBetsReq.bets = new PlaceBets[1];
                            placeBetsReq.bets[0] = bet;

                            //BFExchangeService bfService = new BFExchangeService();

                            PlaceBetsResp placeBetsResp = new BFExchangeService().placeBets(placeBetsReq);

                            setHeartbeat();

                            if (_session != placeBetsResp.header.sessionToken)
                                _session = placeBetsResp.header.sessionToken;

                            if (placeBetsResp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK)
                            {
                                APIErrorResolver(placeBetsResp.header.errorCode, "BetfairKom::placeLayBetBelowMin");
                            }

                            if (placeBetsResp.errorCode != PlaceBetsErrorEnum.OK)
                            {
                                try
                                {
                                    PBErrorResolver(placeBetsResp.errorCode, "BetfairKomm::placeLayBetBelowMin");
                                }
                                catch (BetInProgressException)
                                {
                                    if (placeBetsResp.betResults.Length > 0)
                                    {
                                        throw new BetInProgressException(placeBetsResp.betResults[0].betId);
                                    }
                                }
                                return null;
                            }

                            if (placeBetsResp.betResults[0].betId == 0)
                            {
                                
                                throw new Exception();
                            }

                            betId = placeBetsResp.betResults[0].betId;

                            //Wette überprüfen:
                            myBet = getBetDetail(betId);
                            if (myBet.betStatus == BetStatusEnum.M)
                            {
                                // Wette wurde bereits zur Dummy-Quote angenommen =>super und raus
                                return myBet;
                            }

                            // Pseudoendlosschleife verlassen
                            break;

                        }
                        catch (NoSessionException)
                        {
                            continue;
                        }
                        catch (ThrottleExceededException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                }
                #endregion

                #region Endgültige Wette zur Zielquote
                // Zweiter Teil: Wette aktualisieren
                while (true)
                {
                    lock (syncRoot)
                    {
                        try
                        {
                            // Get a session, or login 
                            // string session = SessionTokenManager.GetSessionToken();
                            if (_session == null || _session == String.Empty)
                            {
                                login();
                                //session = SessionTokenManager.GetSessionToken();
                            }

                            double newMoney = getMinStakeForCurrency(getCurrency()) + money;
                            myBet = updateBet(betId, getMinStakeForCurrency(getCurrency()), 1.01, Math.Round(newMoney, 2), 1.01);

                            if (myBet == null)
                                return null;

                            if (myBet.betId == 0)
                                return null;

                            // Wenn Wette irgendwie angenommen wurde => raus
                            if (myBet.matchedSize > 0)
                                return myBet;
                            else
                            {
                                // Cancel old Bet
                                if (cancelBet(betId))
                                {
                                    //Update price
                                    myBet = updateBet(myBet.betId, Math.Round(money, 2), 1.01, Math.Round(money, 2), price);
                                    if (myBet == null)
                                        return null;
                                    if (myBet.matchedSize > 0)
                                    {
                                        
                                        return myBet;
                                    }
                                    else
                                    {
                                        
                                        return myBet;
                                    }
                                    //return BetfairKom.Instance.getBetDetail(localBet.betId);
                                }
                                else
                                {
                                    return getBetDetail(myBet.betId);
                                }
                            }
                        }
                        catch (NoSessionException)
                        {
                            continue;
                        }
                        catch (ThrottleExceededException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                }
                #endregion
                /*lock (syncRoot)
                {
                    if (myBet != null && myBet.betId != 0)
                        return getBetDetail(myBet.betId);
                    else return null;
                }*/
            }
        }

        public Bet placeLayBetBelowMin(int marketId, int selectionId, int asianId, double price, double money)
        {
            return placeLayBetBelowMin(marketId, selectionId, asianId, false, price, money);
        }


        public Bet placeLayBet(int marketId, int selectionId, int asianId, bool keepBet, double price, double money)
        {
            lock (placeBetLock)
            {
                while (true)
                {
                    lock (syncRoot)
                    {
                        try
                        {
                            // Get a session, or login 
                            //string session = SessionTokenManager.GetSessionToken();
                            if (_session == null || _session == String.Empty)
                            {
                                login();
                                //session = SessionTokenManager.GetSessionToken();
                            }
                            BFGlobalService bfGlobal = new BFGlobalService();
                            PlaceBetsReq placeBetsReq = new PlaceBetsReq();
                            placeBetsReq.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                            placeBetsReq.header.sessionToken = _session;
                            PlaceBets bet = new PlaceBets();
                            bet.marketId = marketId;
                            bet.selectionId = selectionId;
                            bet.betType = BetTypeEnum.L;
                            bet.price = price;
                            bet.size = money;
                            bet.asianLineId = asianId;
                            bet.bspLiability = 0.0;
                            bet.betCategoryType = BetCategoryTypeEnum.E;
                            if (keepBet)
                                bet.betPersistenceType = BetPersistenceTypeEnum.IP;
                            else
                                bet.betPersistenceType = BetPersistenceTypeEnum.NONE;
                            placeBetsReq.bets = new PlaceBets[1];
                            placeBetsReq.bets[0] = bet;

                            //BFExchangeService bfService = new BFExchangeService();

                            PlaceBetsResp placeBetsResp = new BFExchangeService().placeBets(placeBetsReq);

                            setHeartbeat();

                            if (_session != placeBetsResp.header.sessionToken)
                                _session = placeBetsResp.header.sessionToken;

                            if (placeBetsResp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK)
                            {
                                APIErrorResolver(placeBetsResp.header.errorCode, "BetfairKom::placeLayBet");
                                return null;
                            }

                            if (placeBetsResp.errorCode != PlaceBetsErrorEnum.OK)
                            {
                                try
                                {
                                    PBErrorResolver(placeBetsResp.errorCode, "BetfairKomm::placeLayBet");
                                }
                                catch (BetInProgressException)
                                {
                                    if (placeBetsResp.betResults.Length > 0)
                                    {
                                        throw new BetInProgressException(placeBetsResp.betResults[0].betId);
                                    }
                                    else
                                        throw new BetInProgressException();
                                }
                                return null;
                            }

                            if (placeBetsResp.betResults[0].betId == 0)
                                return null;
                            else
                                return getBetDetail(placeBetsResp.betResults[0].betId);
                        }
                        catch (NoSessionException)
                        {
                            _session = String.Empty;
                            continue;
                        }
                        catch (ThrottleExceededException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                }
            }
        }

        public Bet placeLayBet(int marketId, int selectionId,int asianId, double price, double money)
        {
            return placeLayBet(marketId, selectionId, asianId, false, price, money);
        }



        public Bet placeLayBet(int marketId, int selectionId, double price, double money)
        {
            return placeLayBet(marketId, selectionId, 0, false, price, money);
        }

        public Bet placeBackBetBelowMin(int marketId, int selectionId, int asianId, bool keepBet, double price, double money)
        {
            lock (placeBetLock)
            {
                long betId = 0;
                Bet myBet = null;
                #region Pseudewette zu Quote 1000
                while (true)
                {
                    lock (syncRoot)
                    {
                        betId = 0;
                        // 1. Wette absetzen zur Quote 1.01 absetzen
                        try
                        {
                            // Get a session, or login 
                            // string session = SessionTokenManager.GetSessionToken();
                            if (_session == null || _session == String.Empty)
                            {
                                login();
                                //session = SessionTokenManager.GetSessionToken();
                            }
                            BFGlobalService bfGlobal = new BFGlobalService();
                            PlaceBetsReq placeBetsReq = new PlaceBetsReq();
                            placeBetsReq.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                            placeBetsReq.header.sessionToken = _session;
                            PlaceBets bet = new PlaceBets();
                            bet.marketId = marketId;
                            bet.selectionId = selectionId;
                            bet.betType = BetTypeEnum.B;
                            /////////////////////////////////////////////////////////////////////////////////////////////
                            // Place lowest allowed Lay-price
                            bet.price = 1000;
                            /////////////////////////////////////////////////////////////////////////////////////////////
                            // Place Minimum allowed Money
                            bet.size = getMinStakeForCurrency(getCurrency());//BankrollManager.Instance.MinStake;
                            ////////////////////////////////////////////////////////////////////////////////////////////
                            bet.asianLineId = asianId;
                            bet.bspLiability = 0.0;
                            bet.betCategoryType = BetCategoryTypeEnum.E;
                            if (keepBet)
                                bet.betPersistenceType = BetPersistenceTypeEnum.IP;
                            else
                                bet.betPersistenceType = BetPersistenceTypeEnum.NONE;
                            placeBetsReq.bets = new PlaceBets[1];
                            placeBetsReq.bets[0] = bet;

                            //BFExchangeService bfService = new BFExchangeService();

                            PlaceBetsResp placeBetsResp = new BFExchangeService().placeBets(placeBetsReq);

                            setHeartbeat();

                            if (_session != placeBetsResp.header.sessionToken)
                                _session = placeBetsResp.header.sessionToken;

                            if (placeBetsResp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK)
                            {
                                APIErrorResolver(placeBetsResp.header.errorCode, "BetfairKom::placeBackBetBelowMin");
                            }

                            if (placeBetsResp.errorCode != PlaceBetsErrorEnum.OK)
                            {
                                try
                                {
                                    PBErrorResolver(placeBetsResp.errorCode, "BetfairKomm::placeBackBetBelowMin");
                                }
                                catch (BetInProgressException)
                                {
                                    if (placeBetsResp.betResults.Length > 0)
                                    {
                                        throw new BetInProgressException(placeBetsResp.betResults[0].betId);
                                    }
                                    else
                                        throw new BetInProgressException();
                                }
                                return null;
                            }

                            if (placeBetsResp.betResults[0].betId == 0)
                            {
                                
                                throw new Exception();
                            }

                            betId = placeBetsResp.betResults[0].betId;

                            //Wette überprüfen:
                            myBet = getBetDetail(betId);
                            if (myBet.betStatus == BetStatusEnum.M)
                            {
                                // Wette wurde bereits zur Dummy-Quote angenommen =>super und raus
                                return myBet;
                            }

                            // Pseudoendlosschleife verlassen
                            break;

                        }
                        catch (NoSessionException)
                        {
                            continue;
                        }
                        catch (ThrottleExceededException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        catch (Exception e)
                        {
                            ExceptionWriter.Instance.WriteException(e);
                            return null;
                        }
                    }
                }
                #endregion
                #region Endgültige Wette zur Zielquote
                // Zweiter Teil: Wette aktualisieren
                while (true)
                {
                    lock (syncRoot)
                    {
                        try
                        {
                            // Get a session, or login 
                            // string session = SessionTokenManager.GetSessionToken();
                            if (_session == null || _session == String.Empty)
                            {
                                login();
                                //session = SessionTokenManager.GetSessionToken();
                            }

                            double newMoney = getMinStakeForCurrency(getCurrency()) + money;
                            myBet = updateBet(betId, getMinStakeForCurrency(getCurrency()), 1000, Math.Round(newMoney, 2), 1000);

                            if (myBet == null)
                                return null;

                            if (myBet.betId == 0)
                                return null;

                            // Wenn Wette irgendwie angenommen wurde => raus
                            if (myBet.matchedSize > 0)
                                break;
                            else
                            {
                                // Cancel old Bet
                                if (cancelBet(betId))
                                {
                                    //Update price
                                    myBet = updateBet(myBet.betId, Math.Round(money, 2), 1000, Math.Round(money, 2), price);
                                    if (myBet == null)
                                        return null;
                                    if (myBet.matchedSize > 0)
                                    {
                                        
                                        return myBet;
                                    }
                                    else
                                    {
                                        
                                        return myBet;
                                    }
                                    //return BetfairKom.Instance.getBetDetail(localBet.betId);
                                }
                                else
                                {
                                    return getBetDetail(myBet.betId);
                                }
                            }
                        }
                        catch (NoSessionException)
                        {
                            continue;
                        }
                        catch (ThrottleExceededException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                }
                #endregion
                lock (syncRoot)
                {
                    if (myBet != null && myBet.betId != 0)
                        return getBetDetail(myBet.betId);
                    else return null;
                }
            }
        }

        public Bet placeBackBetBelowMin(int marketId, int selectionId, int asianId, double price, double money)
        {
            return placeBackBetBelowMin(marketId, selectionId, asianId, false, price, money);
        }

        public Bet placeBackBet(int marketId, int selectionId, int asianId, bool keepBet, double price, double money)
        {
            lock (placeBetLock)
            {
                while (true)
                {
                    lock (syncRoot)
                    {
                        try
                        {
                            // Get a session, or login 
                            //string session = SessionTokenManager.GetSessionToken();
                            if (_session == null || _session == String.Empty)
                            {
                                login();
                                //session = SessionTokenManager.GetSessionToken();
                            }
                            BFGlobalService bfGlobal = new BFGlobalService();
                            PlaceBetsReq placeBetsReq = new PlaceBetsReq();
                            placeBetsReq.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                            placeBetsReq.header.sessionToken = _session;
                            PlaceBets bet = new PlaceBets();
                            bet.marketId = marketId;
                            bet.selectionId = selectionId;
                            bet.betType = BetTypeEnum.B;
                            bet.price = price;
                            bet.size = money;
                            bet.asianLineId = asianId;
                            bet.bspLiability = 0.0;
                            bet.betCategoryType = BetCategoryTypeEnum.E;
                            if (keepBet)
                                bet.betPersistenceType = BetPersistenceTypeEnum.IP;
                            else
                                bet.betPersistenceType = BetPersistenceTypeEnum.NONE;
                            placeBetsReq.bets = new PlaceBets[1];
                            placeBetsReq.bets[0] = bet;

                            //BFExchangeService bfService = new BFExchangeService();

                            PlaceBetsResp placeBetsResp = new BFExchangeService().placeBets(placeBetsReq);

                            setHeartbeat();

                            if (_session != placeBetsResp.header.sessionToken)
                                _session = placeBetsResp.header.sessionToken;

                            if (placeBetsResp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK)
                            {
                                APIErrorResolver(placeBetsResp.header.errorCode, "BetfairKom::placeBackBet");
                                return null;
                            }

                            if (placeBetsResp.errorCode != PlaceBetsErrorEnum.OK)
                            {
                                try
                                {
                                    PBErrorResolver(placeBetsResp.errorCode, "BetfairKomm::placeBackBet");
                                }
                                catch (BetInProgressException)
                                {
                                    if (placeBetsResp.betResults.Length > 0)
                                    {
                                        throw new BetInProgressException(placeBetsResp.betResults[0].betId);
                                    }
                                    else
                                        throw new BetInProgressException();
                                }
                                return null;
                            }

                            if (placeBetsResp.betResults[0].betId == 0)
                                return null;
                            else
                                return getBetDetail(placeBetsResp.betResults[0].betId);
                        }
                        catch (NoSessionException)
                        {
                            _session = String.Empty;
                        }
                        catch (ThrottleExceededException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                }
            }
        }

        public Bet placeBackBet(int marketId, int selectionId,int asianId, double price, double money)
        {
            return placeBackBet(marketId, selectionId, asianId, false, price, money);
        }


        public Bet[] placeBets(PlaceBets[] betsToPlace)
        {
            lock (placeBetLock)
            {
                while (true)
                {
                    lock (syncRoot)
                    {
                        ArrayList bets = new ArrayList();
                        try
                        {
                            if (_session == null || _session == String.Empty)
                            {
                                login();
                                //session = SessionTokenManager.GetSessionToken();
                            }

                            BFGlobalService bfGlobal = new BFGlobalService();
                            PlaceBetsReq placeBetsReq = new PlaceBetsReq();
                            placeBetsReq.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                            placeBetsReq.header.sessionToken = _session;

                            placeBetsReq.bets = betsToPlace;

                            PlaceBetsResp placeBetsResp = new BFExchangeService().placeBets(placeBetsReq);

                            setHeartbeat();

                            if (_session != placeBetsResp.header.sessionToken)
                                _session = placeBetsResp.header.sessionToken;

                            if (placeBetsResp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK)
                            {
                                APIErrorResolver(placeBetsResp.header.errorCode, "BetfairKom::placeBackBet");
                                return null;
                            }

                            if (placeBetsResp.errorCode != PlaceBetsErrorEnum.OK)
                            {
                                try
                                {
                                    PBErrorResolver(placeBetsResp.errorCode, "BetfairKomm::placeBackBet");
                                }
                                catch (BetInProgressException)
                                {
                                    if (placeBetsResp.betResults.Length > 0)
                                    {
                                        throw new BetInProgressException(placeBetsResp.betResults[0].betId);
                                    }
                                    else
                                        throw new BetInProgressException();
                                }
                                return null;
                            }

                            foreach (PlaceBetsResult result in placeBetsResp.betResults)
                            {
                                Bet bet = getBetDetail(result.betId);
                                bets.Add(bet);
                            }

                            return (Bet[])bets.ToArray(typeof(Bet));

                        }
                        catch (NoSessionException)
                        {
                            _session = String.Empty;
                        }
                        catch (ThrottleExceededException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                }
            }
        }

        public Bet placeBackBet(int marketId, int selectionId, double price, double money)
        {
            return placeBackBet(marketId, selectionId, 0, false, price, money);
        }

        private void PBErrorResolver(PlaceBetsErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case PlaceBetsErrorEnum.ACCOUNT_CLOSED:
                    msg = String.Format("Error {0}: The user’s account has been closed. Please contact Betfair.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.ACCOUNT_IS_EXCLUDED:
                    msg = String.Format("Error {0}: The account has been locked due to self-exclusion.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.ACCOUNT_SUSPENDED:
                    msg = String.Format("Error {0}: Account has been suspended. One reason you might receive this message is that you have " +
                            "attempted to place a bet on the Australian exchange server but your Australian wallet " +
                            "is suspended because Betfair have not yet received confirmation of your name and address.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.AUTHORISATION_PENDING:
                    msg = String.Format("Error {0}: Account is pending authorisation.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.BACK_LAY_COMBINATION:
                    msg = String.Format("Error {0}: Bets contains a Back and a Lay on the same runner.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.BBM_DAEMON_NOT_AVAILABLE:
                    msg = String.Format("Error {0}: The exchange encountered a critical error and was not able to " +
                            "match your bet. Your bet was not placed.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.BET_IN_PROGRESS:
                    msg = String.Format("Error {0}: The exchange has not completed matching the bet before the bet matching time " +
                            "expired. You should use GetMUBets to determine if the bet was placed successfully.", error.ToString());
                    throw new BetInProgressException();
                    //break;
                case PlaceBetsErrorEnum.BETWEEN_1_AND_60_BETS_REQUIRED:
                    msg = String.Format("Error {0}: Number of BetPlacement less than 1 or greater than 60.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.BSP_BETTING_NOT_SUPPORTED:
                    msg = String.Format("Error {0}: You have requested a BSP bet, " +
                            "but BSP betting is not supported for this market.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.DIFFERING_MARKETS:
                    msg = String.Format("Error {0}: All bets not all for the same market.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.EVENT_CLOSED:
                    msg = String.Format("Error {0}: Market has already closed.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.EVENT_INACTIVE:
                    msg = String.Format("Error {0}: Market is not active.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.EVENT_SUSPENDED:
                    msg = String.Format("Error {0}: Market is suspended.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.FROM_COUNTRY_FORBIDDEN:
                    msg = String.Format("Error {0}: Bet origin from a restricted country.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.INTERNAL_ERROR:
                    msg = String.Format("Error {0}: Internal error occurred.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.INVALID_MARKET:
                    msg = String.Format("Error {0}: MarketID doesn't exist. Check that you have sent your service request " +
                            "to the correct exchange server (the Australian exchange server cannot see markets on the UK " +
                            "exchange server, and vice versa).", error.ToString());
                    break;
                case PlaceBetsErrorEnum.MARKET_TYPE_NOT_SUPPORTED:
                    msg = String.Format("Error {0}: Market Type is invalid or does not exist.", error.ToString());
                    break;
                case PlaceBetsErrorEnum.SITE_UPGRADE:
                    msg = String.Format("Error {0}: Site is currently being upgraded.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }
            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);
        }


        public Bet updateBet(long betId, double oldMoney, double oldPrice, double newMoney, double newPrice)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }

                        UpdateBetsReq updateReq = new UpdateBetsReq();
                        updateReq.header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        updateReq.header.sessionToken = _session;
                        UpdateBets updateBet = new UpdateBets();
                        updateBet.betId = betId;
                        updateBet.newPrice = newPrice;
                        updateBet.newSize = newMoney;
                        updateBet.oldPrice = oldPrice;
                        updateBet.oldSize = oldMoney;
                        updateReq.bets = new UpdateBets[1];
                        updateReq.bets[0] = updateBet;

                        UpdateBetsResp resp = new BFExchangeService().updateBets(updateReq);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK)
                        {
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::updateEvents");
                            return null;
                        }
                        if (resp.errorCode == UpdateBetsErrorEnum.OK)
                        {

                            // Details holen
                            long newBetId = (long)resp.betResults[0].newBetId;
                            return getBetDetail(newBetId);
                        }
                        else if (resp.errorCode == UpdateBetsErrorEnum.BET_IN_PROGRESS ||
                                resp.errorCode == UpdateBetsErrorEnum.INVALID_MARKET_ID)
                        {
                            long newBetId = (long)resp.betResults[0].newBetId;
                            return BetfairKom.Instance.getBetDetail(newBetId);
                        }
                        else
                        {
                            UBErrorResolver(resp.errorCode, "BetfairKom::UpdateBet");
                        }

                        return null;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                        continue;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }


            }
        }

        private void UBErrorResolver(UpdateBetsErrorEnum error, string caller)
        {
                       String msg = String.Empty;
            switch (error)
            {
                case UpdateBetsErrorEnum.ACCOUNT_CLOSED:
                    msg = String.Format("Error {0}: Invalid Market type.", error.ToString());
                    break;
                case UpdateBetsErrorEnum.ACCOUNT_PENDING:
                    msg = String.Format("Error {0}: The user’s account is pending authorisation.", error.ToString());
                    break;
                case UpdateBetsErrorEnum.ACCOUNT_SUSPENDED:
                    msg = String.Format("Error {0}: Bet could not be cancelled. This may be because the user’s local wallet is "+
                            "suspended (for example, because Betfair have not yet received confirmation of your name and " + 
                            "address). Part of the process of updating a bet involves cancellation of the original bet. " +
                            "However, if between the original bet and the UpdateBets request the local wallet that was used " +
                            "to fund the bet has been suspended, then the original bet will not be cancelled and the bet " +
                            "update will therefore not be successful.", error.ToString());
                    break;
                case UpdateBetsErrorEnum.BBM_DAEMON_NOT_AVAILABLE:
                    msg = String.Format("Error {0}: The exchange encountered a critical error and was not able to match your bet. " +
                            "Your bet was not placed.", error.ToString());
                    break;
                case UpdateBetsErrorEnum.FROM_COUNTRY_FORBIDDEN:
                    msg = String.Format("Error {0}: Update request from restricted country.", error.ToString());
                    break;
                case UpdateBetsErrorEnum.INVALID_NUMBER_OF_BETS:
                    msg = String.Format("Error {0}: Number of bets not between 0 and 15.", error.ToString());
                    break;
                case UpdateBetsErrorEnum.MARKET_TYPE_NOT_SUPPORTED:
                    msg = String.Format("Error {0}: The MarketID supplied refers to a market that is not supported by the API.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);
        }

        public MarketPrices getMarketPrices(int marketId)
        {
            return getMarketPrices(marketId, false);
        }

        public MarketPrices getMarketPrices(int marketId, bool canThrowThrottleExceeded)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    //string session = SessionTokenManager.GetSessionToken();
                    if (_session == null || _session == String.Empty)
                    {
                        login();
                        //session = SessionTokenManager.GetSessionToken();
                    }

                    BFExchangeService bfService = new BFExchangeService();
                    BetFairIF.com.betfair.api.exchange.APIRequestHeader header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                    GetMarketPricesReq marketPricesReq = new GetMarketPricesReq();
                    header.sessionToken = _session;
                    marketPricesReq.header = header;
                    marketPricesReq.marketId = marketId;
                    try
                    {
                        //TODO: Exceptions Abfangen (z.b InvalidOperationExceptions. NULL Wert zurück geben und Globale Fehlernachricht schicken
                        GetMarketPricesResp marketPricesResp = bfService.getMarketPrices(marketPricesReq);

                        setHeartbeat();

                        if (_session != marketPricesResp.header.sessionToken)
                            _session = marketPricesResp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(marketPricesResp.header.sessionToken);

                        if (marketPricesResp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK || marketPricesResp.errorCode != GetMarketPricesErrorEnum.OK)
                        {
                            APIErrorResolver(marketPricesResp.header.errorCode, "BetfairKom::getMarketPrices");
                            return null;
                        }

                        if (marketPricesResp.errorCode != GetMarketPricesErrorEnum.OK)
                        {
                            GMPErrorResolver(marketPricesResp.errorCode, "BetfairKom::getMarketPrices");
                            return null;
                        }
                        
                        return marketPricesResp.marketPrices;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException tee)
                    {
                        if (canThrowThrottleExceeded)
                        {
                            throw tee;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }

        private void GMPErrorResolver(GetMarketPricesErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case GetMarketPricesErrorEnum.INVALID_CURRENCY:
                    msg = String.Format("Error {0}: Currency code not a valid 3 letter ISO 4217 currency abbreviation.", error.ToString());
                    break;
                case GetMarketPricesErrorEnum.INVALID_MARKET:
                    msg = String.Format("Error {0}: Market Id is not valid. Check that you have sent your service request " +
                            "to the correct exchange server (the Australian exchange server cannot see markets on the UK " +
                            " exchange server, and vice versa).", error.ToString());
                    break;
                case GetMarketPricesErrorEnum.MARKET_TYPE_NOT_SUPPORTED:
                    msg = String.Format("Error {0}: The MarketID supplied refers to a market that is not supported by the API. " +
                            "Currently, this includes Line and Range markets.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);
        }

        public MarketLite getMarketInfo(int marketId)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }

                        BFExchangeService bfService = new BFExchangeService();
                        GetMarketInfoReq marketReq = new GetMarketInfoReq();
                        BetFairIF.com.betfair.api.exchange.APIRequestHeader header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        header.sessionToken = _session;
                        marketReq.header = header;
                        marketReq.marketId = marketId;
                        GetMarketInfoResp marketResp = bfService.getMarketInfo(marketReq);

                        setHeartbeat();

                        if (_session != marketResp.header.sessionToken)
                            _session = marketResp.header.sessionToken;

                        if (marketResp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK)
                        {
                            APIErrorResolver(marketResp.header.errorCode, "BetfairKom::getMarketInfo");
                            return null;
                        }
                        if (marketResp.errorCode != GetMarketErrorEnum.OK)
                        {
                            GMIErrorResolver(marketResp.errorCode, "BetfairKom::getMarketInfo");
                            return null;
                        }
                        return marketResp.marketLite;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                        continue;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (Exception e)
                    {
                        ExceptionWriter.Instance.WriteException(e);
                        return null;
                    }
                }
            }
        }

        private void GMIErrorResolver(GetMarketErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case GetMarketErrorEnum.INVALID_LOCALE_DEFAULTING_TO_ENGLISH:
                    msg = String.Format("Error {0}: The locale string was not recognized. Returned results are in English.", error.ToString());
                    break;
                case GetMarketErrorEnum.INVALID_MARKET:
                    msg = String.Format("Error {0}: Invalid Market ID supplied. Make sure you have sent the request to the " +
                            "correct exchange server. Check that you have sent your service request to the correct exchange " +
                            "server (the Australian exchange server cannot see markets on the UK exchange server, " +
                            "and vice versa).", error.ToString());
                    break;
                case GetMarketErrorEnum.MARKET_TYPE_NOT_SUPPORTED:
                    msg = String.Format("Error {0}: The MarketID supplied identifies a market of a type that is not " +
                            "supported by the API.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }

            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);
        }

        public Bet getBetDetail(long betId)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }
                        BetFairIF.com.betfair.api.exchange.APIRequestHeader header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        header.sessionToken = _session;
                        BFExchangeService bfService = new BFExchangeService();

                        // Details holen
                        GetBetReq betReq = new GetBetReq();

                        betReq.header = header;
                        betReq.betId = betId;
                        betReq.locale = "en";
                        //betReq.locale = "en_GB";

                        GetBetResp betResp = bfService.getBet(betReq);

                        setHeartbeat();

                        if (_session != betResp.header.sessionToken)
                            _session = betResp.header.sessionToken;

                        if (betResp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            APIErrorResolver(betResp.header.errorCode, "BetfairKom::getBetDetail");
                            return null;
                        }

                        if (betResp.errorCode != GetBetErrorEnum.OK)
                        {
                            GBDErrorResolver(betResp.errorCode, "BetfairKom::getBetDetail");
                            return null;
                        }

                        return betResp.bet;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                        continue;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (Exception e)
                    {
                        ExceptionWriter.Instance.WriteException(e);
                        return null;
                    }
                }
            }
        }

        private void GBDErrorResolver(GetBetErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case GetBetErrorEnum.BET_ID_INVALID:
                    msg = String.Format("Error {0}: Bet Id is invalid or does not exist. Make sure you have sent your request " +
                            "to the correct exchange server. The getBet service only searches on the exchange server it is " +
                            "sent to for the bet that you specify.", error.ToString());
                    break;
                case GetBetErrorEnum.INVALID_LOCALE_DEFAULTING_TO_ENGLISH:
                    msg = String.Format("Error {0}: The locale string was not recognized. Returned results are in English.", error.ToString());
                    break;
                case GetBetErrorEnum.MARKET_TYPE_NOT_SUPPORTED:
                    msg = String.Format("Error {0}: Market Type is invalid or does not exist", error.ToString());
                    break;
                case GetBetErrorEnum.NO_RESULTS:
                    msg = String.Format("Error {0}: No results.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }
            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg); 
        }

        public MUBet[] getBets(DateTime dts)
        {
            while (true)
            {
                lock (syncRoot)
                {
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }
                        BFExchangeService bfService = new BFExchangeService();
                        GetMUBetsReq req = new GetMUBetsReq();
                        //GetCurrentBetsReq req = new GetCurrentBetsReq();
                        BetFairIF.com.betfair.api.exchange.APIRequestHeader header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        header.sessionToken = _session;
                        req.header = header;
                        req.orderBy = BetsOrderByEnum.BET_ID;
                        // Nur gültige Wetten holen
                        req.betStatus = BetStatusEnum.M;
                        req.recordCount = 200;
                        req.startRecord = 0;
                        req.matchedSince = dts;                        

                        req.sortOrder = SortOrderEnum.ASC;

                        //GetCurrentBetsResp resp = bfService.getCurrentBets(req);
                        
                        GetMUBetsResp resp = bfService.getMUBets(req);
                        
                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getBets");
                            return resp.bets;
                        }

                        if (resp.errorCode == GetMUBetsErrorEnum.NO_RESULTS)
                        {
                            
                            return null;
                        }

                        if (resp.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            GBErrorResolver(resp.errorCode, "BetfairKom::getBets");
                        }

                        
                        return resp.bets;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (WebException webExc)
                    {
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs("BetfairKom::getBets", webExc.ToString()));
                        }
                        ExceptionWriter.Instance.WriteException(webExc);
                        return null;
                    }
                }
            }
        }

        public MUBet[] getBetMU(long betId)
        {
            
            while (true)
            {
                lock (syncRoot)
                {
                    
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }
                        BFExchangeService bfService = new BFExchangeService();
                        GetMUBetsReq req = new GetMUBetsReq();
                        //GetCurrentBetsReq req = new GetCurrentBetsReq();
                        BetFairIF.com.betfair.api.exchange.APIRequestHeader header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        header.sessionToken = _session;
                        req.header = header;
                        req.orderBy = BetsOrderByEnum.BET_ID;
                        // Nur gültige Wetten holen
                        req.betStatus = BetStatusEnum.MU;
                        req.betIds = new long[1];
                        req.betIds[0] = betId;
                        req.recordCount = 200;
                        req.startRecord = 0;
                        /*
                        req.locale = "en";
                        req.detailed = true;
                        req.noTotalRecordCount = false;
                        req.startRecord = 0;
                         */
                        req.sortOrder = SortOrderEnum.ASC;

                        //GetCurrentBetsResp resp = bfService.getCurrentBets(req);
                        
                        GetMUBetsResp resp = bfService.getMUBets(req);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getBetMU");
                            return resp.bets;
                        }
                        if (resp.errorCode == GetMUBetsErrorEnum.NO_RESULTS)
                        {

                            return null;
                        }

                        if (resp.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            GBErrorResolver(resp.errorCode, "BetfairKom::getBetMU");
                        }
                        
                        return resp.bets;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (WebException webExc)
                    {
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs("BetfairKom::getBetMU", webExc.ToString()));
                        }
                        ExceptionWriter.Instance.WriteException(webExc);
                        return null;
                    }
                }
            }
        }

        public MUBet[] getBetsMU(int marketId)
        {
            
            while (true)
            {
                lock (syncRoot)
                {
                    
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || _session == String.Empty)
                        {
                            
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }
                        BFExchangeService bfService = new BFExchangeService();
                        GetMUBetsReq req = new GetMUBetsReq();
                        //GetCurrentBetsReq req = new GetCurrentBetsReq();
                        BetFairIF.com.betfair.api.exchange.APIRequestHeader header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        header.sessionToken = _session;
                        req.header = header;
                        req.orderBy = BetsOrderByEnum.BET_ID;
                        // Nur gültige Wetten holen
                        req.betStatus = BetStatusEnum.MU;
                        
                        req.marketId = marketId;
                        req.recordCount = 200;
                        req.startRecord = 0;
                        /*
                        req.locale = "en";
                        req.detailed = true;
                        req.noTotalRecordCount = false;
                        req.startRecord = 0;
                         */
                        req.sortOrder = SortOrderEnum.ASC;

                        //GetCurrentBetsResp resp = bfService.getCurrentBets(req);
                        
                        GetMUBetsResp resp = bfService.getMUBets(req);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getBetsMU");
                            return resp.bets;
                        }
                        if (resp.errorCode == GetMUBetsErrorEnum.NO_RESULTS)
                        {
                            
                            return null;
                        }

                        if (resp.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            GBErrorResolver(resp.errorCode, "BetfairKom::getBetsMU");
                        }
                        
                        return resp.bets;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (WebException webExc)
                    {
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs("BetfairKom::getBetsMU", webExc.ToString()));
                        }
                        ExceptionWriter.Instance.WriteException(webExc);
                        return null;
                    }
                }
            }
        }

        public MUBet[] getBets()
        {
            
            while (true)
            {
                lock (syncRoot)
                {
                    
                    try
                    {
                        //string session = SessionTokenManager.GetSessionToken();
                        if (_session == null || String.IsNullOrEmpty(_session))
                        {
                            
                            login();
                            //session = SessionTokenManager.GetSessionToken();
                        }
                        BFExchangeService bfService = new BFExchangeService();
                        GetMUBetsReq req = new GetMUBetsReq();
                        //GetCurrentBetsReq req = new GetCurrentBetsReq();
                        BetFairIF.com.betfair.api.exchange.APIRequestHeader header = new BetFairIF.com.betfair.api.exchange.APIRequestHeader();
                        header.sessionToken = _session;
                        req.header = header;
                        req.orderBy = BetsOrderByEnum.BET_ID;
                        // Nur gültige Wetten holen
                        req.betStatus = BetStatusEnum.M;
                        req.recordCount = 200;
                        req.startRecord = 0;
                        /*
                        req.locale = "en";
                        req.detailed = true;
                        req.noTotalRecordCount = false;
                        req.startRecord = 0;
                         */
                        req.sortOrder = SortOrderEnum.ASC;

                        //GetCurrentBetsResp resp = bfService.getCurrentBets(req);
                        
                        GetMUBetsResp resp = bfService.getMUBets(req);

                        setHeartbeat();

                        if (_session != resp.header.sessionToken)
                            _session = resp.header.sessionToken;

                        //SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                        if (resp.header.errorCode != BetFairIF.com.betfair.api.exchange.APIErrorEnum.OK) // || resp.header.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            
                            APIErrorResolver(resp.header.errorCode, "BetfairKom::getBets");
                            return resp.bets;
                        }
                        if (resp.errorCode == GetMUBetsErrorEnum.NO_RESULTS)
                        {
                            
                            return null;
                        }

                        if (resp.errorCode != GetMUBetsErrorEnum.OK)
                        {
                            GBErrorResolver(resp.errorCode, "BetfairKom::getBets");
                        }
                        
                        return resp.bets;
                    }
                    catch (NoSessionException)
                    {
                        _session = String.Empty;
                    }
                    catch (ThrottleExceededException)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (WebException webExc)
                    {
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs("BetfairKom::getBets", webExc.ToString()));
                        }
                        ExceptionWriter.Instance.WriteException(webExc);
                        return null;
                    }
                }
            }
        }

        private void GBErrorResolver(GetMUBetsErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case GetMUBetsErrorEnum.INVALID_BET_STATUS:
                    msg = String.Format("Error {0}: Invalid bet status. The bet status must be M, U, or MU.", error.ToString());
                    break;
                case GetMUBetsErrorEnum.INVALID_MARKET_ID:
                    msg = String.Format("Error {0}: Market ID is negative or does not exist. Check that you have sent your " +
                            "service request to the correct exchange server (the Australian exchange server cannot see " +
                            "markets on the UK exchange server, and vice versa).", error.ToString());
                    break;
                case GetMUBetsErrorEnum.INVALID_ORDER_BY_FOR_STATUS:
                    msg = String.Format("Error {0}: Ordering is not NONE and\r\n" +
                            "Bet Status is M (matched) and Ordering is neither MATCHED_DATE or PLACED_DATE\r\n" +
                            "Bet Status is U (unmatched) and ordering isn't PLACED_DATE\r\n" +
                            "Bet Status is MU (matched and unmatched) and Ordering is neither MATCHED_DATE or PLACED_DATE."
                            , error.ToString());
                    break;
                case GetMUBetsErrorEnum.INVALID_RECORD_COUNT:
                    msg = String.Format("Error {0}: Record Count is negative.", error.ToString());
                    break;
                case GetMUBetsErrorEnum.INVALID_START_RECORD:
                    msg = String.Format("Error {0}: Start record is not supplied or is not greater than or equal to 1.", error.ToString());
                    break;
                case GetMUBetsErrorEnum.TOO_MANY_BETS_REQUESTED:
                    msg = String.Format("Error {0}: You submitted an array of betIds for more than 200 bets.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }
            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg); 
        }

        public bool login()
        {
            while (true)
            {
                
                lock (syncRoot)
                {
                    
                    do
                    {
                        LoginReq req = new LoginReq();
                        String strFile = SXDirs.ApplPath + @"\BFTS.dat";
                        // Versuche die Datei mit Logindaten zu finden.
                        if (usr == String.Empty && pwd == String.Empty)
                        {
                            if (!File.Exists(strFile))
                            {
                                frmLogin dialog = new frmLogin();
                                DialogResult result = dialog.ShowDialog();
                                if (result != DialogResult.OK)
                                {
                                    EventHandler<EventArgs> handler = ShutdownRequest;
                                    if (handler != null)
                                    {
                                        handler(this, new EventArgs());
                                       
                                        return false;
                                    }
                                    else
                                    {
                                        Environment.Exit(0);
                                        return false;
                                    }
                                }
                                else
                                {
                                    usr = dialog.User;
                                    pwd = dialog.Pwd;

                                    if (dialog.Remember)
                                    {
                                        createFile();
                                    }
                                }
                            }
                            else
                            {
                                readFile();
                            }
                        }

                        req.username = usr;//"lichtbringer666";
                        req.password = pwd; //"acheron666";
                        req.productId = 82;
                        BetFairIF.com.betfair.api.BFGlobalService bfService = new BetFairIF.com.betfair.api.BFGlobalService();
                        try
                        {
                            
                            LoginResp resp = bfService.login(req);

                            setHeartbeat();

                            if (_session != resp.header.sessionToken)
                                _session = resp.header.sessionToken;

                            /*
                            if (resp.errorCode == LoginErrorEnum.INVALID_USERNAME_OR_PASSWORD)
                            {
                                usr = String.Empty;
                                pwd = String.Empty;
                                File.Delete(strFile);
                                continue;
                            }
                            */
                            /*
                            if (resp.header.errorCode == BetFairIF.com.betfair.api.APIErrorEnum.EXCEEDED_THROTTLE)
                            {

                                continue;
                            }
                             * */

                            if (resp.header.errorCode != BetFairIF.com.betfair.api.APIErrorEnum.OK)//|| resp.errorCode != LoginErrorEnum.OK)
                            {
                                
                                String msg = APIErrorResolver(resp.header.errorCode, "BetfairKom::login");
                                MessageBox.Show(msg, "Error Logging into Betfair", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                usr = String.Empty;
                                pwd = String.Empty;
                            }
                            else
                            {
                                if (resp.errorCode == LoginErrorEnum.OK_MESSAGES)
                                {
                                    loginErrorResolver(resp.errorCode, "BetfairKom::login");
                                }
                                if (resp.errorCode != LoginErrorEnum.OK)
                                {
                                    loginErrorResolver(resp.errorCode, "BetfairKom::login");
                                    usr = String.Empty;
                                    pwd = String.Empty;
                                    File.Delete(strFile);
                                    continue;
                                }
                                
                                _currency = resp.currency;
                                // SessionTokenManager.ReturnSessionToken(resp.header.sessionToken);
                                SXTools.doWebRequest(String.Format("http://www.sxtrader.net/StatIF/sportexchange.php?sx=BF&fb={0}", FingerPrint.Value()));
                                return true;
                            }
                        }
                        catch (ThrottleExceededException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        catch (WebException exc)
                        {
                            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                            if (handler != null)
                            {
                                handler(this, new SXExceptionMessageEventArgs("BetfairKom::login", exc.ToString()));
                            }
                            ExceptionWriter.Instance.WriteException(exc);
                            
                        }
                        catch (InvalidOperationException excIOE)
                        {
                            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                            if (handler != null)
                            {
                                handler(this, new SXExceptionMessageEventArgs("BetfairKom::login", excIOE.ToString()));
                            }
                            ExceptionWriter.Instance.WriteException(excIOE);


                            Thread.Sleep(1000);
                            continue;
                        }
                        catch (IOException excIO)
                        {
                            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                            if (handler != null)
                            {
                                handler(this, new SXExceptionMessageEventArgs("BetfairKom::login", excIO.ToString()));
                            }
                            ExceptionWriter.Instance.WriteException(excIO);


                            Thread.Sleep(1000);
                            continue;
                        }
                    } while (true);
                }
            }
        }


        /// <summary>
        /// Initialisiert das Heartbeatintervall, welches sich nach einger gegebenen Zeit am Server meldet, um
        /// anzuzeigen, dass der client noch aktiv ist.
        /// </summary>
        private void setHeartbeat()
        {
            if (this._heartbeat != null)
            {
                this._heartbeat.Stop();
                this._heartbeat.Close();
            }

            this._heartbeat = new System.Timers.Timer();
            this._heartbeat.Elapsed += new ElapsedEventHandler(_heartbeat_Elapsed);
            TimeSpan span = new TimeSpan(0, 15, 0);
            this._heartbeat.Interval = span.TotalMilliseconds;
            this._heartbeat.AutoReset = true;
            this._heartbeat.Start();
        }


        private void _heartbeat_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (syncRoot)
            {
                
                KeepAliveReq request = new KeepAliveReq();
                request.header = new BetFairIF.com.betfair.api.APIRequestHeader();
                request.header.sessionToken = _session;
                BetFairIF.com.betfair.api.BFGlobalService bfService = new BetFairIF.com.betfair.api.BFGlobalService();
                KeepAliveResp response = bfService.keepAlive(request);  //_myClient.keepAlive(request);

                // Sessiontoken überprüfen
                if (response.header.sessionToken != _session)
                    _session = response.header.sessionToken;

                // Falls Fehlermeldung zurück diese Auswerten
                if (response.header.errorCode != BetFairIF.com.betfair.api.APIErrorEnum.OK)
                {
                    //TODO: Was auch immer zu tun ist.
                }

            }

        }

        private void loginErrorResolver(LoginErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case LoginErrorEnum.ACCOUNT_CLOSED:
                    msg = String.Format("Error {0}: Not used.", error.ToString());
                    break;
                case LoginErrorEnum.ACCOUNT_SUSPENDED:
                    msg = String.Format("Error {0}: Account suspended.", error.ToString());
                    break;
                case LoginErrorEnum.FAILED_MESSAGE:
                    msg = String.Format("Error {0}: The user cannot login until they acknowledge a message from Betfair.", error.ToString());
                    break;
                case LoginErrorEnum.INVALID_LOCATION:
                    msg = String.Format("Error {0}: Invalid LocationID.", error.ToString());
                    break;
                case LoginErrorEnum.INVALID_PRODUCT:
                    msg = String.Format("Error {0}: Invalid productID entered.", error.ToString());
                    break;
                case LoginErrorEnum.INVALID_VENDOR_SOFTWARE_ID:
                    msg = String.Format("Error {0}: Invalid vendorSoftwareId supplied.", error.ToString());
                    break;
                case LoginErrorEnum.LOGIN_FAILED_ACCOUNT_LOCKED:
                    msg = String.Format("Error {0}: Not used.", error.ToString());
                    break;
                case LoginErrorEnum.LOGIN_REQUIRE_TERMS_AND_CONDITIONS_ACCEPTANCE:
                    msg = String.Format("Error {0}: Not used.", error.ToString());
                    break;
                case LoginErrorEnum.LOGIN_RESTRICTED_LOCATION:
                    msg = String.Format("Error {0}: Login origin from a restricted country.", error.ToString());
                    break;
                case LoginErrorEnum.LOGIN_UNAUTHORIZED:
                    msg = String.Format("Error {0}: User has not been permissioned to use API login.", error.ToString());
                    break;
                case LoginErrorEnum.OK_MESSAGES:
                    msg = String.Format("Error {0}: There are additional messages on your account. " 
                            + "Please log in to the web site to view them.", error.ToString());
                    break;
                case LoginErrorEnum.POKER_T_AND_C_ACCEPTANCE_REQUIRED:
                    msg = String.Format("Error {0}: Account locked, Please login to the Betfair Poker website and "
                            +"assent to the terms and conditions.", error.ToString());
                    break;
                case LoginErrorEnum.T_AND_C_ACCEPTANCE_REQUIRED:
                    msg = String.Format("Error {0}: There are new Terms and Conditions." +
                            "Continued usage of the Betfair API and/or website will be considered acceptance of the new terms.", error.ToString());
                    break;
                case LoginErrorEnum.USER_NOT_ACCOUNT_OWNER:
                    msg = String.Format("Error {0}: The specified account is not a trading account and therefore cannot be " +
                            "used for API access.", error.ToString());
                    break;
                default:
                    msg = String.Format("Error {0}: Unknown Error.", error.ToString());
                    break;
            }

            
            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);

            MessageBox.Show(msg, "Error during Login", MessageBoxButtons.OK, MessageBoxIcon.Error);

            EventHandler<EventArgs> shutdown = ShutdownRequest;
            if (shutdown != null)
            {
                shutdown(this, new EventArgs());
                return;
            }

        }

        private void readFile()
        {
            try
            {
                String strFile = SXDirs.ApplPath + @"\BFTS.dat";
                if (!File.Exists(strFile))
                {
                    pwd = String.Empty;
                    usr = String.Empty;
                    return;
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(strFile);
                XmlElement root = doc.DocumentElement;
                usr = root.ChildNodes[0].InnerText;
            }
            catch /*(Exception exc)*/
            {
                pwd = String.Empty;
                usr = String.Empty;
            }
        }

        private void createFile()
        {
            String strFile = SXDirs.ApplPath + @"\BFTS.dat";
            if(File.Exists(strFile))
            {
                File.Delete(strFile);
            }

            //Datei nicht gefunden => erzeugen
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("node1");
            XmlElement ele2 = doc.CreateElement("node2");
            ((XmlNode)ele2).InnerText = usr;
            element.AppendChild(ele2);
            XmlElement ele3 = doc.CreateElement("node3");
            ((XmlNode)ele3).InnerText = pwd;
            element.AppendChild(ele3);
            doc.AppendChild(element);
            doc.Save(strFile);
                        
// TODO: Ersetzten durch Verschlüsselte Datei
            doc.Load(strFile);
        }

        private String APIErrorResolver(BetFairIF.com.betfair.api.APIErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case BetFairIF.com.betfair.api.APIErrorEnum.EXCEEDED_THROTTLE:
                    msg = String.Format("Error {0}: This means that the maximum count of request for the free api has been exceeded\n\r" +
                                               "Normally this is not an error. The call will be delayed.", error.ToString());
                    throw new ThrottleExceededException();
                   
                case BetFairIF.com.betfair.api.APIErrorEnum.INTERNAL_ERROR:
                    msg = String.Format("Error {0}: There was an unspecified error on the Betfair-Server. Please check the Status\n\r" +
                                            "Betfair at their Website www.betfair.com.", error.ToString() );
                    break;
                case BetFairIF.com.betfair.api.APIErrorEnum.USER_NOT_SUBSCRIBED_TO_PRODUCT:
                    msg = String.Format("Error {0}: User not subscribed to specified product.", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.APIErrorEnum.SUBSCRIPTION_INACTIVE_OR_SUSPENDED:
                    msg = String.Format("Error {0}: User subscription to product is Inactive or Suspended", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.APIErrorEnum.VENDOR_SOFTWARE_INACTIVE:
                    msg = String.Format("Error {0}: Vendor Software is Inactive.", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.APIErrorEnum.VENDOR_SOFTWARE_INVALID:
                    msg = String.Format("Error {0}: Specified VendorSoftwareId is invalid or does not exist", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.APIErrorEnum.SERVICE_NOT_AVAILABLE_IN_PRODUCT:
                    msg = String.Format("Error {0}: User attempting to access service which is not present in their subscription", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.APIErrorEnum.NO_SESSION:
                    msg = String.Format("Error {0}: Session Token does not exist or has expired", error.ToString());
                    throw new NoSessionException();                    
                case BetFairIF.com.betfair.api.APIErrorEnum.TOO_MANY_REQUESTS:
                    msg = String.Format("Error {0}: For each session/login/unique sessionToken the user can only make one call at a time.\r\n If two requests are sent simultaneously, requests will receive this message until previous requests are processed.\r\n If you want to send multiple requests, you must call login and use a second sessionToken sequence.", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.APIErrorEnum.PRODUCT_REQUIRES_FUNDED_ACCOUNT:
                    msg = String.Format("Error {0}: You cannot use this product unless your Betfair account is active.\r\n An active account is one in which your Account Statement page on Betfair.com contains at least one transaction.\r\n Transactions expire after three month." +
                                            "Transactions include depositing funds and placing a bet that is settled.", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.APIErrorEnum.SERVICE_NOT_AVAILABLE_FOR_LOGIN_STATUS:
                    msg = String.Format("Error {0}: You have logged in, but access to services is restricted to retrieveLIMBMessage,submitLIMBMessage, and logout.\r\n  This may be because there is a login message that requires attention.", error.ToString());
                    break;

            }            

            

            EventHandler<SXExceptionMessageEventArgs> handler =  ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
                return msg;
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);

            return msg;
        }

        private void APIErrorResolver(BetFairIF.com.betfair.api.exchange.APIErrorEnum error, string caller)
        {
            String msg = String.Empty;
            switch (error)
            {
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.EXCEEDED_THROTTLE:
                    msg = String.Format("Error {0}: This means that the maximum count of request for the free api has been exceeded\n\r" +
                                               "Normally this is not an error. The call will be delayed.", error.ToString());
                    throw new ThrottleExceededException();                    
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.INTERNAL_ERROR:
                    msg = String.Format("Error {0}: There was an unspecified error on the Betfair-Server. Please check the Status\n\r" +
                                            "Betfair at their Website www.betfair.com.", error.ToString() );
                    break;
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.USER_NOT_SUBSCRIBED_TO_PRODUCT:
                    msg = String.Format("Error {0}: User not subscribed to specified product.", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.SUBSCRIPTION_INACTIVE_OR_SUSPENDED:
                    msg = String.Format("Error {0}: User subscription to product is Inactive or Suspended", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.VENDOR_SOFTWARE_INACTIVE:
                    msg = String.Format("Error {0}: Vendor Software is Inactive.", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.VENDOR_SOFTWARE_INVALID:                                                                                                                                                                          
                    msg = String.Format("Error {0}: Specified VendorSoftwareId is invalid or does not exist", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.SERVICE_NOT_AVAILABLE_IN_PRODUCT:
                    msg = String.Format("Error {0}: User attempting to access service which is not present in their subscription", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.NO_SESSION:
                    msg = String.Format("Error {0}: Session Token does not exist or has expired", error.ToString());
                    throw new NoSessionException();
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.TOO_MANY_REQUESTS:
                    msg = String.Format("Error {0}: For each session/login/unique sessionToken the user can only make one call at a time.\r\n If two requests are sent simultaneously, requests will receive this message until previous requests are processed.\r\n If you want to send multiple requests, you must call login and use a second sessionToken sequence.", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.PRODUCT_REQUIRES_FUNDED_ACCOUNT:
                    msg = String.Format("Error {0}: You cannot use this product unless your Betfair account is active.\r\n An active account is one in which your Account Statement page on Betfair.com contains at least one transaction.\r\n Transactions expire after three month." +
                                            "Transactions include depositing funds and placing a bet that is settled.", error.ToString());
                    break;
                case BetFairIF.com.betfair.api.exchange.APIErrorEnum.SERVICE_NOT_AVAILABLE_FOR_LOGIN_STATUS:
                    msg = String.Format("Error {0}: You have logged in, but access to services is restricted to retrieveLIMBMessage,submitLIMBMessage, and logout.\r\n  This may be because there is a login message that requires attention.", error.ToString());
                    break;

            }

            

            EventHandler<SXExceptionMessageEventArgs> handler =  ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(caller, msg));
            }

            ExceptionWriter.Instance.WriteMessage(caller, msg);
        }

        /// <summary>
        /// Die Bezeichnung der Rennbahnen wird von Betfair immer in Abkürzungen angegeben.
        /// Zur leichteren Verständnis und zur Vereinheitlichung ist es notwendig diese Abkürzungen
        /// in volle Namen umzuwandeln.
        /// </summary>
        private void loadHorseRaceMapping()
        {
            XmlDocument localChangeDateDoc = new XmlDocument();
            XmlDocument remoteChangeDateDoc = new XmlDocument();
            try
            {
                remoteChangeDateDoc.Load("http://www.sxtrader.net/bfhorsemappingchangedate.xml");
                localChangeDateDoc.Load(SXDirs.ApplPath + @"\bfhorsemappingchangedate.xml");
                
                //Änderungsdatum der lokalen Datei
                DateTime dtsLocal = new DateTime(long.Parse(localChangeDateDoc.ChildNodes[1].InnerText, CultureInfo.InvariantCulture));


                DateTime dtsNet = new DateTime(long.Parse(remoteChangeDateDoc.ChildNodes[1].InnerText, CultureInfo.InvariantCulture));
                // Falls Datei aus dem Netz aktueller ist => Mappingdatei nachlesen, ansonsten alte Versionb beibehalten
                if (dtsNet.Ticks > dtsLocal.Ticks)
                {
                    //Version im Netz ist neuer => Herunterladen
                    downloadHorseMapping(remoteChangeDateDoc);
                }
            }
            catch (IOException)
            {
                downloadHorseMapping(remoteChangeDateDoc);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                return;
            }

            try
            {
                _horseMarketMappingDoc = XDocument.Load(SXDirs.ApplPath + @"\BFHorseMarketTranslation.xml");
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                _horseMarketMappingDoc = null;
                return;
            }
        }

        private void downloadHorseMapping(XmlDocument remoteDateChangeDoc)
        {
            XmlDocument mappingDoc = new XmlDocument();
            mappingDoc.Load(@"http://www.sxtrader.net/BFHorseMarketsTranslation.xml");
            mappingDoc.Save(SXDirs.ApplPath + @"\BFHorseMarketTranslation.xml");

            //Datumsdatei speichern
            remoteDateChangeDoc.Save(SXDirs.ApplPath + @"\bfhorsemappingchangedate.xml");
        }

        private string translateHorseName(String horse)
        {
            if (char.IsNumber(horse[0]))
            {
                int index = horse.IndexOf(' ');
                if (index >= 0)
                {
                    return horse.Substring(index).Trim();
                }
            }
            return horse;
        }
      
        public static bool isUnder(int selectionId)
        {
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
        }

        public static bool isOver(int selectionId)
        {
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
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;
        public event EventHandler<SXWMessageEventArgs> MessageEvent;

        #endregion

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_heartbeat != null)
                    {
                        _heartbeat.Dispose();
                    }
                }
                _disposed = true;
            }
        }        
    }

    public class NoSessionException : Exception { }

    public class ThrottleExceededException : Exception { }

    public class BetInProgressException : Exception 
    {
        long _betId;
        public long BetId { get { return _betId; } }

        public BetInProgressException()
        {
            _betId = 0;
        }

        public BetInProgressException(long betId)
        {
            _betId = betId;
        }
    }
   
}
