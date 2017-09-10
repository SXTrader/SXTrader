using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.muk.interfaces;
using net.sxtrader.betdaqif.betdaq;
using System.Collections;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using System.Threading;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;

namespace net.sxtrader.betdaqif
{
    public sealed partial class BetdaqKom : IBFTSCommon, IDisposable
    {
        private static volatile BetdaqKom _instance;
        private static Object syncRoot = "syncRootBetdaq";//new Object();

        private const string SECUREURL = "Http://api.betdaq{0}/v2.0/Secure/SecureService.asmx";
        private const string READONLYURL = "Http://api.betdaq{0}/v2.0/ReadOnlyService.asmx";
        private const String GBP = "GBP";
        private const double MINGBP = 0.5;
        private const String EUR = "EUR";
        private const double MINEUR = 0.5;        
        private const String USD = "USD";
        private const double MINUSD = 0.5;        
        private const String SGD = "SGD";
        private const double MINSGD = 1.0;

        //Fehlercode        
        private const int EVENTDOESNOTEXIST               = 5;
        private const int MARKETDOESNOTEXIST              = 8;
        private const int MARKETNOTACTIVE                 = 15;
        private const int MARKETNEITHERSUSPENDEDNORACTIVE = 16;
        private const int SELECTIONNOTACTIVE              = 17;
        private const int ORDERDOESNOTEXIST               = 21;
        private const int RESETHASOCCURED                 = 114;
        private const int MAXIMUMINPUTRECORDS             = 137;
        private const int INSUFFICIENTFOUND               = 241; 
        private const int PUNTERORDERMISMATCH             = 274;
        private const int DUPLICATEORDERSPECIFIED         = 299;
        private const int PUNTERISBLACKLISTED             = 406;

        //Wartezeit bei Blacklisting in ms
        private const int BLACKLISTWAITTIME = 30000;

        private SecureService _secureProxy = null;
        private ReadOnlyService _readOnlyProxy = null;
        private string _usr = String.Empty;
        private string _pwd = String.Empty;

        private GetOddsLadderResponseItem[] _ladder;

        private Properties.Settings _settings;
        private bool _isLoggedIn = false;
        private long _maxSequenceNo = -1;
        private bool _disposed = false;

        private String _currency = String.Empty;

        private enum ORDERACTIONTYPE  {
            Placed = 1,
            ExplicitlyUpdated = 2,
            Matched = 3,
            CancelledExplicitly = 4,
            CancelledByReset = 5,
            CancelledOnInRunning = 6,
            Expired = 7,
            MatchedPortionRepricedByR4 = 8,
            UnmatchedPortionRepricedByR4 = 9,
            UnmatchedPortionCancelledByWithdrawal = 10,
            Voided = 11,
            Settled = 12,
            Suspended = 13,
            Unsuspended = 14, 
            ExpiredByMatching = 15,
            Unsettled = 16,
            Unmatched = 17,
            MatchedPortionRepriced = 18,
            CreatedFromLightweightPrice = 19,
            CancelledOnComplete = 20,
        };


        private BetdaqKom() 
        {
            _settings = Properties.Settings.Default;
        }

        public static BetdaqKom Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new BetdaqKom();
                    }
                }

                return _instance;
            }
        }


        public bool IsUKExchange
        {
            get;
            set;
        }

        private bool login()
        {
            lock (syncRoot)
            {
                if (!_isLoggedIn)
                {

                    String urlFragment = String.Empty;

                    if (IsUKExchange)
                        urlFragment = ".co.uk";
                    else
                        urlFragment = ".com";

                    _secureProxy = new SecureService();
                    _secureProxy.Url = String.Format(SECUREURL, urlFragment);

                    ExternalApiHeader header = new ExternalApiHeader();
                    header.version = 2;
                    header.username = _usr;
                    header.password = _pwd;
                    header.languageCode = "en";

                    _secureProxy.ExternalApiHeaderValue = header;

                    _readOnlyProxy = new ReadOnlyService();
                    _readOnlyProxy.Url = String.Format(READONLYURL, urlFragment);

                    _readOnlyProxy.ExternalApiHeaderValue = header;

                    double a, b, c;
                    try
                    {
                        internalGetAccounFounds(out a, out b, out c);
                        //_settings["BetdaqServer"] = urlFragment;
                    }
                    catch (BDInvalidPasswordOrUserException bipoue)
                    {                        
                        throw new Exception(bipoue.Message, bipoue);
                    }

                    //Aufruf http://www.sxtrader.net/StatIF/sportexchange.php?sx=BD
                    SXTools.doWebRequest(String.Format("http://www.sxtrader.net/StatIF/sportexchange.php?sx=BD&fb={0}", FingerPrint.Value()));
                }
            }
            return true;
        }

        public void loadOddsLadder()
        {
            lock (syncRoot)
            {
                while (true)
                {
                    try
                    {
                        GetOddsLadderRequest req = new GetOddsLadderRequest();
                        req.PriceFormat = 1;
                        req.PriceFormatSpecified = true;

                        GetOddsLadderResponse resp = _readOnlyProxy.GetOddsLadder(req);

                        if (resp.ReturnStatus.Code != 0)
                        {
                            ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);
                        }
                        _ladder = resp.Ladder;

                        return;
                    }
                    catch (BDPunterIsBlacklisted)
                    {
                        //Abfragelimit wurde erreicht => Abwarten
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                    catch (WebException exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                }
            }
        }

        public EventClassifierType[] getTopLevelEvents()
        {
            lock (syncRoot)
            {
                while (true)
                {
                    try
                    {
                        ListTopLevelEventsRequest req = new ListTopLevelEventsRequest();


                        ListTopLevelEventsResponse resp =
                            _readOnlyProxy.ListTopLevelEvents(req);

                        if (resp.ReturnStatus.Code != 0)
                        {
                            ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);
                        }

                        return resp.EventClassifiers;
                    }
                    catch (BDPunterIsBlacklisted)
                    {
                        //Abfragelimit wurde erreicht => Abwarten
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                    catch (WebException exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                }
                
            }
        }

        public SXALMarket[] getEventSubtreeNoSelections(long[] eventIds, DateTime fromDate, DateTime toDate)
        {
            lock (syncRoot)
            {
                while (true)
                {
                    try
                    {
                        GetEventSubTreeWithSelectionsRequest req =
                            new GetEventSubTreeWithSelectionsRequest();

                        ArrayList alMt = new ArrayList();

                        req.EventClassifierIds = eventIds;
                        req.WantPlayMarkets = true;

                        GetEventSubTreeWithSelectionsResponse resp = _readOnlyProxy.GetEventSubTreeWithSelections(req);


                        if (resp.ReturnStatus.Code != 0)
                        {
                            ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);
                        }

                        buildMarketInfo(resp.EventClassifiers,null, eventIds[0], fromDate, toDate, ref alMt);

                        return (SXALMarket[])alMt.ToArray(typeof(SXALMarket));
                    }
                    catch (BDPunterIsBlacklisted)
                    {
                        Thread.Sleep(BLACKLISTWAITTIME);
                        continue;
                    }
                    catch (WebException exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                }
            }
        }

        private void buildMarketInfo(EventClassifierType[] ects, EventClassifierType parent, long eventId, DateTime fromDate, DateTime toDate, ref ArrayList theList)
        {
            long horseRacingId = -1;

            if(ects == null)
            {
                return;
            }

            foreach (EventClassifierType ect in ects)
            {
                if (ect.Markets == null)
                {
                    buildMarketInfo(ect.EventClassifiers,ect,eventId, fromDate, toDate, ref theList);
                }
                else
                {
                    foreach (MarketType m in ect.Markets)
                    {
                        if (m.StartTime < fromDate)
                            continue;

                        if (m.StartTime > toDate)
                            continue;

                        SXALMarket sm = new SXALMarket();

                        sm.Id = m.Id;
                        //sm.Devision = m.
                        sm.InPlayMarket = m.IsInRunningAllowed;
                        
                        foreach(SXALEventType evt in _events)
                        {
                            if(evt.Name == "Horse Racing")
                            {
                                horseRacingId = evt.Id;
                            }
                        }


                        if (parent != null && eventId == horseRacingId && ect.Name != parent.Name)
                        {
                            sm.Match = formatMatchName(parent.Name);
                        }
                        else
                        {
                            sm.Match = formatMatchName(ect.Name);                           
                        }

                        sm.Name = formatMarketName(m.Name);
                        sm.StartDTS = m.StartTime;
                        ArrayList alS = new ArrayList();
                        if (m.Selections != null)
                        {
                            foreach (SelectionType s in m.Selections)
                            {
                                SXALSelection ss = new SXALSelection();
                                ss.Id = s.Id;
                                ss.Name = s.Name;
                                if (s.Status == 4 || s.Status == 5 || s.Status == 9)
                                    ss.IsNonStarter = true;

                                alS.Add(ss);
                            }
                        }

                        sm.Selections = (SXALSelection[])alS.ToArray(typeof(SXALSelection));
                        theList.Add(sm);
                    }
                }

            }
        }

        private String formatMarketName(String unformatedMarketName)
        {
            String result = unformatedMarketName;
            if (unformatedMarketName.Equals("Under/Over - Goals (0.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 0.5 Goals";
            }
            else if (unformatedMarketName.Equals("Under/Over - Goals (1.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 1.5 Goals";
            }
            else if (unformatedMarketName.Equals("Under/Over - Goals (2.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 2.5 Goals";
            }
            else if (unformatedMarketName.Equals("Under/Over - Goals (3.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 3.5 Goals";
            }
            else if (unformatedMarketName.Equals("Under/Over - Goals (4.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 4.5 Goals";
            }
            else if (unformatedMarketName.Equals("Under/Over - Goals (5.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 5.5 Goals";
            }
            else if (unformatedMarketName.Equals("Under/Over - Goals (6.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 6.5 Goals";
            }
            else if (unformatedMarketName.Equals("Under/Over - Goals (7.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 7.5 Goals";
            }
            else if (unformatedMarketName.Equals("Under/Over - Goals (8.5)", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "Over/Under 8.5 Goals";
            }
            else if (unformatedMarketName.Contains("Correct"))
            {
                return result;
            }

            return result;
        }

        private String formatMatchName(String unformatedName)
        {
            String result = unformatedName;

            int iStart = 0;

            //Klammerinfos entfernen
            iStart = result.IndexOf('(');
            while (iStart > -1)
            {
                int iEnd = result.IndexOf(')');
                if (iEnd > -1)
                    result = result.Remove(iStart, iEnd - iStart +1);

                iStart = result.IndexOf('(');
            }

            result = result.Replace(" v ", " - ");
            
            // Beginnt mit Uhrzeit?
            if (result[2] == ':')
                result = result.Remove(0, 5);


            return result;
        }

        public Order[] listBootstrapOrders(out long maxSequenceNo)
        {
            lock (syncRoot)
            {
                while (true)
                {
                    try
                    {
                        ListBootstrapOrdersRequest req = new ListBootstrapOrdersRequest();
                        ListBootstrapOrdersResponse resp = null;
                        ArrayList alOrders = new ArrayList();
                        long lastSeqNoReceived = -1;

                        do
                        {
                            req.SequenceNumber = lastSeqNoReceived;                            
                            resp = _secureProxy.ListBootstrapOrders(req);

                            if (resp.ReturnStatus.Code != 0)
                                ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);

                            if (resp.Orders == null || resp.Orders.Length == 0)
                                break;

                            alOrders.AddRange(resp.Orders);
                            lastSeqNoReceived = resp.Orders[resp.Orders.Length - 1].SequenceNumber;
                        } while (lastSeqNoReceived < resp.MaximumSequenceNumber);

                        maxSequenceNo = lastSeqNoReceived;
                        return (Order[])alOrders.ToArray(typeof(Order));
                    }
                    catch (BDPunterIsBlacklisted)
                    {
                        Thread.Sleep(BLACKLISTWAITTIME);
                        continue;
                    }
                    catch (WebException exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                }
            }
        }

        public MarketTypeWithPrices[] internalGetMarketPrices(long marketId)
        {
            lock (syncRoot)
            {
                while (true)
                {
                    GetPricesRequest req = new GetPricesRequest();
                    long[] marketIds = new long[1];
                    marketIds[0] = marketId;

                    req.MarketIds = marketIds;
                    req.NumberAgainstPricesRequired = 3;
                    req.NumberForPricesRequired = 3;
                    req.ThresholdAmount = 0;
                    req.WantMarketMatchedAmountSpecified = true;
                    req.WantMarketMatchedAmount = true;
                    req.WantSelectionMatchedDetailsSpecified = true;
                    req.WantSelectionMatchedDetails = true;
                    req.WantSelectionsMatchedAmounts = true;
                    req.WantSelectionsMatchedAmountsSpecified = true;

                    try
                    {
                        GetPricesResponse resp = _readOnlyProxy.GetPrices(req);


                        if (resp.ReturnStatus.Code != 0)
                        {
                            throw new Exception(resp.ReturnStatus.Description);
                        }

                        
                        return resp.MarketPrices;
                    }
                    catch (WebException)
                    {
                        Thread.Sleep(BLACKLISTWAITTIME);
                        continue;
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                }
            }
        }

        public void internalGetAccounFounds(out double availBalance, out double currentBalance, out double creditLimit)
        {
            lock (syncRoot)
            {
                while (true)
                {
                    try
                    {
                        availBalance = currentBalance = creditLimit = 0.0;
                        GetAccountBalancesRequest req = new GetAccountBalancesRequest();

                        GetAccountBalancesResponse resp = _secureProxy.GetAccountBalances(req);                        

                        if (resp.ReturnStatus.Code != 0)
                        {
                            ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);
                        }

                        availBalance = (double)resp.AvailableFunds;
                        currentBalance = (double)resp.Balance;
                        creditLimit = (double)resp.Credit;

                        _currency = resp.Currency;
                        return;
                    }
                    catch (BDPunterIsBlacklisted)
                    {
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                    catch (WebException exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                }
            }
        }

        public bool cancelOrder(long orderId)
        {
            lock (syncRoot)
            {
                while (true)
                {
                    try
                    {
                        long[] ids = new long[1];
                        CancelOrdersRequest req = new CancelOrdersRequest();

                        ids[0] = orderId;
                        req.OrderHandle = ids;

                        CancelOrdersResponse resp = _secureProxy.CancelOrders(req);

                        if (resp.ReturnStatus.Code != 0)
                        {
                            ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);
                        }

                        if (resp.Orders != null)
                        {
                            foreach (CancelOrdersResponseItem ropi in resp.Orders)
                            {
                                if (ropi.OrderHandle == orderId && ropi.cancelledForSideStakeSpecified && ropi.cancelledForSideStake > 0)
                                    return true;
                            }
                        }
                        return false;
                    }
                    catch (BDPunterIsBlacklisted)
                    {
                        Thread.Sleep(BLACKLISTWAITTIME);
                        continue;
                    }
                    catch (WebException exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                }
            }
        }

        public SXALBet placeBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, SXALBetTypeEnum betType, bool isInplay)
        {
            lock (syncRoot)
            {
                MarketType type = getMarketInfo(marketId);
                
                PlaceOrdersNoReceiptRequest req = new PlaceOrdersNoReceiptRequest();
                req.WantAllOrNothingBehaviour = false;

                SimpleOrderRequest soq = new SimpleOrderRequest();
                if (betType == SXALBetTypeEnum.B)
                    soq.Polarity = 1;
                else
                    soq.Polarity = 2;
                soq.SelectionId = selectionId;
                soq.Price = (decimal)Math.Round(price, 2);
                soq.Stake = (decimal)Math.Round(money,2);
                soq.CancelOnInRunning = false;
                if (isInplay)
                {
                    soq.CancelIfSelectionReset = false;
                }
                else
                {
                    soq.CancelIfSelectionReset = !keepBet;
                }
                soq.PunterReferenceNumber = 42;
                soq.ExpectedWithdrawalSequenceNumber = (byte)type.WithdrawalSequenceNumber;

                foreach (SelectionType s in type.Selections)
                {
                    if (s.Id == selectionId)
                    {
                        soq.ExpectedSelectionResetCount = (byte)s.ResetCount;
                    }
                }
                

                req.Orders = new SimpleOrderRequest[1];
                req.Orders[0] = soq;
                

                PlaceOrdersNoReceiptResponse resp =
                    _secureProxy.PlaceOrdersNoReceipt(req);

                if (resp.ReturnStatus.Code != 0)
                {
                    ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);
                }

                Thread.Sleep(5000);

                return getOrderDetail(resp.OrderHandles[0]);
            }
        }

        public MarketType getMarketInfo(long marketId)
        {
            lock (syncRoot)
            {
                while (true)
                {
                    try
                    {
                        GetMarketInformationRequest req = new GetMarketInformationRequest();
                        long[] ids = new long[1];
                        ids[0] = marketId;
                        req.MarketIds = ids;

                        GetMarketInformationResponse resp = _readOnlyProxy.GetMarketInformation(req);

                        if (resp.ReturnStatus.Code != 0)
                        {
                            ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);
                        }

                        if (resp.Markets == null || resp.Markets.Length == 0)
                            return null;
                        else
                            return resp.Markets[0];
                    }
                    catch (BDPunterIsBlacklisted)
                    {
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                    catch (WebException exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        Thread.Sleep(BLACKLISTWAITTIME);
                    }
                }
            }
        }

        public SXALBet getOrderDetail(long orderId)
        {
            while (true)
            {
                try
                {
                    GetOrderDetailsRequest req = new GetOrderDetailsRequest();
                    req.OrderId = orderId;
                    req.OrderIdSpecified = true;

                    GetOrderDetailsResponse resp = _secureProxy.GetOrderDetails(req);

                    if (resp.ReturnStatus.Code != 0)
                    {
                        ErrorCodeToException(resp.ReturnStatus.Code, resp.ReturnStatus.Description);
                    }


                    _maxSequenceNo = resp.SequenceNumber;

                    SXALBet b = new SXALBet();
                    b.AvgPrice = (double)resp.AveragePrice;
                    b.BetId = orderId;

                    if (resp.AuditLog != null)
                    {
                        ArrayList alMatches = new ArrayList();
                        foreach (AuditLogItem logItem in resp.AuditLog)
                        {
                            if (logItem.OrderActionType == (int)ORDERACTIONTYPE.Matched)
                            {
                                SXALMatch m = new SXALMatch();
                                m.SizeMatched = (double)logItem.MatchedOrderInformation.MatchedStake;
                                alMatches.Add(m);
                            }
                            else
                            {
                                Trace.WriteLine(String.Format("OrderActionType is {0}", logItem.OrderActionType));
                                Trace.WriteLine(String.Format("Average Price is {0}", logItem.AveragePrice));
                                if (logItem.CommissionInformation != null)
                                {
                                    Trace.WriteLine("Commission Information is not null");
                                    Trace.WriteLine(String.Format("\tGross Settlement Amount {0}", logItem.CommissionInformation.GrossSettlementAmount));
                                    Trace.WriteLine(String.Format("\tOrder Commission is {0}", logItem.CommissionInformation.OrderCommission));
                                }

                                if (logItem.MatchedOrderInformation != null)
                                {
                                    Trace.WriteLine("Matched Order Information is not null");
                                }
                            }
                        }

                        b.Matches = (SXALMatch[])alMatches.ToArray(typeof(SXALMatch));
                    }


                    switch (resp.OrderStatus)
                    {
                        case 1:
                            b.BetStatus = SXALBetStatusEnum.U;
                            if (resp.AveragePriceSpecified && resp.AveragePrice != 0)
                                b.BetStatus = SXALBetStatusEnum.MU;
                            break;
                        case 2:
                            b.BetStatus = SXALBetStatusEnum.M;
                            break;
                        case 3:
                            b.BetStatus = SXALBetStatusEnum.C;
                            if (resp.AveragePriceSpecified && resp.AveragePrice != 0)
                                b.BetStatus = SXALBetStatusEnum.M;
                            break;
                        case 4:
                            b.BetStatus = SXALBetStatusEnum.S;
                            break;
                        case 5:
                            b.BetStatus = SXALBetStatusEnum.V;
                            break;
                    }

                    switch (resp.Polarity)
                    {
                        case 1:
                            b.BetType = SXALBetTypeEnum.B;
                            break;
                        case 2:
                            b.BetType = SXALBetTypeEnum.L;
                            break;
                    }

                    b.CancelledDate = resp.ExpiresAt;
                    b.MarketId = resp.MarketId;
                    b.MarketType = mapMarketType(resp.MarketType);
                    b.MatchedDate = resp.MatchingTimeStamp;
                    if (b.Matches != null && b.Matches.Length > 0)
                    {
                        foreach (SXALMatch m in b.Matches)
                        {
                            b.MatchedSize += m.SizeMatched;
                        }
                    }
                    else
                    {
                        b.MatchedSize = (double)(resp.RequestedStake - resp.UnmatchedStake);
                    }
                    b.PlacedDate = resp.IssuedAt;
                    b.Price = (double)resp.RequestedPrice;
                    //b.ProfitAndLoss = resp.OrderSettlementInformation.
                    b.RemainingSize = (double)resp.UnmatchedStake;
                    b.RequestedSize = (double)resp.RequestedStake;
                    b.SelectionId = resp.SelectionId;
                    if (resp.OrderSettlementInformation != null)
                    {
                        b.SettledDate = resp.OrderSettlementInformation.MarketSettledDate;
                    }

                    return b;
                }
                catch (BDPunterIsBlacklisted)
                {
                    Thread.Sleep(BLACKLISTWAITTIME);
                }
                catch (WebException wex)
                {
                    ExceptionWriter.Instance.WriteException(wex);
                    throw new SXExcecutionFailedException(String.Format("Cannot execute getOrderDetail for order {0}", orderId), wex);
                }
            }
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

        private decimal validateOdd(decimal odds, int start, int end)
        {
            for (int i = 0; i < _ladder.Length; i++)
            {
                if (_ladder[i].price == odds)
                {
                    return _ladder[i].price;
                }
                if (_ladder[i].price > odds)
                {

                    return _ladder[i].price;
                }
            }

            return 0;
            /*
            if (start == end)
                return _ladder[start].price;

            if (_ladder[start].price == odds)
            {
                return _ladder[start].price;
            }
            else if (_ladder[end].price == odds)
            {
                return _ladder[end].price;
            }
            else if (_ladder[start].price < odds && _ladder[end / 2].price > odds)
            {
                return validateOdd(odds, start, end / 2);
            }
            else
            {
                return validateOdd(odds, end / 2, end);
            }
             */
        }

        private decimal getValidOddIncrement(decimal odds, int start, int end)
        {

            for (int i = 0; i < _ladder.Length; i++)
            {
                if (_ladder[i].price == odds)
                {
                    if (i == 0)
                    {
                        return _ladder[i + 1].price - _ladder[i].price;
                    }
                    else
                    {
                        return _ladder[i].price - _ladder[i - 1].price;
                    }
                }
                if (_ladder[i].price > odds)
                {
                    if (i == 0)
                    {
                        return _ladder[i + 1].price - _ladder[i].price;
                    }
                    else if (i == 1)
                    {
                        return _ladder[i].price - _ladder[i-1].price;
                    }
                    else
                    {
                        return _ladder[i - 1].price - _ladder[i - 2].price;
                    }
                }
            }

            return 0;

            /*
            if (start == end)
            {
                // Randbedingung oberes ende
                if (end == _ladder.Length - 1)
                {
                    return _ladder[end].price - _ladder[end - 1].price;
                }
                else 
                {
                    return _ladder[end + 1].price - _ladder[end].price;
                }                
            }


            if (_ladder[start].price == odds)
            {
                return _ladder[start + 1].price - _ladder[start].price;
            }
            else if (_ladder[end].price == odds)
            {
                return _ladder[end].price - _ladder[end-1].price;
            }
            else if (_ladder[start].price < odds && _ladder[end / 2].price > odds)
            {
                return getValidOddIncrement(odds, start, end / 2);
            } 
            else
            {
                return getValidOddIncrement(odds, end / 2, end);
            }
             * */
        }

        private SXALMarketTypeEnum mapMarketType(byte marketType)
        {
            switch (marketType)
            {
                case 3:
                    return SXALMarketTypeEnum.O;
                case 10:
                    return SXALMarketTypeEnum.A;
                case 4:
                    return SXALMarketTypeEnum.R;
            }

            return SXALMarketTypeEnum.NOT_APPLICABLE;
        }

        private void ErrorCodeToException(int code, string description)
        {
            //Ungültiges Passwort oder Benutzer
            if (code == 405 || code == 1000)
            {
                throw new BDInvalidPasswordOrUserException(description);
            }

            switch (code)
            {
                case EVENTDOESNOTEXIST:
                    {
                        throw new SXALEventDoesNotExistException(description);
                    }
                case MAXIMUMINPUTRECORDS:
                    {
                        throw new SXALMaxInputRecordExceedException(description);
                    }
                case MARKETDOESNOTEXIST:
                    {
                        throw new SXALMarketDoesNotExistException(description);
                    }
                case MARKETNOTACTIVE:
                    {
                        throw new SXALMarketNotActiveException(description);
                    }
                case MARKETNEITHERSUSPENDEDNORACTIVE:
                    {
                        throw new SXALMarketNeitherSuspendedNorActiveException(description);
                    }
                case ORDERDOESNOTEXIST:
                    {
                        throw new SXALBetDoesNotExistException(description);
                    }
                case PUNTERORDERMISMATCH:
                    {
                        throw new SXALPunterBetMismatchException(description);
                    }
                case RESETHASOCCURED:
                    {
                        throw new SXALResetOccurredException(description);
                    }
                case SELECTIONNOTACTIVE:
                    {
                        throw new SXALSelectionNotActiveException(description);
                    }
                case DUPLICATEORDERSPECIFIED:
                    {
                        throw new SXALDuplicateBetSpecifiedException(description);
                    }
                default:
                    {
                        throw new Exception(String.Format("Unknown Error Code {0} with Message {1}", code, description));
                    }
            }
        }

        #region IBFTSCommon Member

        public event EventHandler<net.sxtrader.muk.eventargs.SXExceptionMessageEventArgs> ExceptionMessageEvent;
        public event EventHandler<muk.eventargs.SXWMessageEventArgs> MessageEvent;
        #endregion

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                //DebugWriter.Instance.WriteMessage("BetdaqKom", "Disposing");
                if (disposing)
                {
                    if (_readOnlyProxy != null)
                    {
                        _readOnlyProxy.Dispose();
                    }

                    if (_secureProxy != null)
                    {
                        _secureProxy.Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }

    class BDInvalidPasswordOrUserException : Exception {
        public BDInvalidPasswordOrUserException() : base() {  }
        public BDInvalidPasswordOrUserException(String message) : base(message) { }
    }

    class BDPunterIsBlacklisted : Exception
    {
        public BDPunterIsBlacklisted() : base() { }
    }

    
}
