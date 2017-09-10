
using net.sxtrader.muk.interfaces;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using BetFairIF.com.betfair.api.exchange;
using System.Collections;
using BetFairIF.com.betfair.api;
using System;
using System.Linq;
using System.Xml.Linq;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Diagnostics;
using System.Collections.Generic;
namespace net.sxtrader.bftradingstrategies.betfairif
{
    public sealed partial class BetfairKom : ISXAL
    {
        #region ISXAL Member

        public bool login(string usr, string pwd)
        {
            this.usr = usr;
            this.pwd = pwd;
            return login();
            
        }
      
        SXALMarket ISXAL.translateHorseMarket(SXALMarket market)
        {
            try
            {
                //RACE
                if (_horseMarketMappingDoc == null)
                {
                    loadHorseRaceMapping();
                }

                var markets = from country in _horseMarketMappingDoc.Descendants("race")
                              where country.Parent.Attribute("id").Value.Trim() == market.Devision.Trim()
                              select new
                              {
                                  FullName = country.Attribute("full").Value.Trim(),
                                  Abbr = country.Attribute("abbr").Value.Trim()
                              };
                foreach (var m in markets)
                {
                    if (market.Match.StartsWith(m.Abbr))
                    {
                        market.Match = m.FullName;
                        break;
                    }
                    //Trace.WriteLine(m.ToString());
                }

                //Marketname
                if (!market.Name.Equals("To be Placed", StringComparison.InvariantCultureIgnoreCase) &&
                    !market.Name.Equals("Reversed FC", StringComparison.InvariantCultureIgnoreCase) && 
                    !market.Name.Equals("Without Fav(s)", StringComparison.InvariantCultureIgnoreCase) &&
                    !market.Name.Equals("Keep The Race?", StringComparison.InvariantCultureIgnoreCase) &&
                    (market.Name.Contains(" Hrd") || market.Name.Contains(" 2yo")|| market.Name.Contains(" 3yo") ||
                     market.Name.Contains(" 4yo") || market.Name.Contains(" Cup") || market.Name.Contains(" Oaks") ||
                     market.Name.Contains(" Stpl") || market.Name.Contains(" Stks") || market.Name.Contains(" Hcap") ||
                     market.Name.Contains(" Claim") || market.Name.Contains(" Listed") || market.Name.Contains(" Allw") ||
                     market.Name.Contains(" Mdn") || market.Name.Contains(" NHF") || market.Name.Contains(" INHF") ||
                     market.Name.Contains(" Plt") || market.Name.Contains(" Grp") || market.Name.Contains(" Grd") ||
                     market.Name.Contains(" PA") || market.Name.Contains(" CL") || market.Name.Contains(" 5yo")
                    ))
                {
                    market.Name = "Win Market";
                }

                if (market.Name.Equals("To be placed", StringComparison.InvariantCultureIgnoreCase))
                {
                    market.Name = "Place Market";
                }

                /*
                // Selection aufbauen
                Market bm = getMarket((int)market.Id);
                ArrayList alSel = new ArrayList();
                foreach (Runner r in bm.runners)
                {
                    SXALSelection s = new SXALSelection();
                    s.Id = r.selectionId;
                    s.Name = r.name;
                    s.IsNonStarter = false;
                    alSel.Add(s);
                }

                market.Selections = (SXALSelection[])alSel.ToArray(typeof(SXALSelection));
                 */
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return market;
        }

        SXALBet ISXAL.placeLayBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, bool isInplay)
        {
            Bet bet = null;
            if (money < getMinStakeForCurrency(_currency))
            {
                bet = placeLayBetBelowMin((int)marketId, (int)selectionId, asianId, price, money);
            }
            else
            {
                bet = placeLayBet((int)marketId, (int)selectionId, asianId, keepBet, price, money);
            }

            SXALBet sxalBet = null;
            if (bet != null)
            {
                sxalBet = new SXALBet();
                sxalBet.AsianLineId    = bet.asianLineId;
                sxalBet.AvgPrice       = bet.avgPrice;
                sxalBet.BetId          = bet.betId;
                switch (bet.betStatus)
                {
                    case BetStatusEnum.C:
                        sxalBet.BetStatus = SXALBetStatusEnum.C;
                        break;
                    case BetStatusEnum.L:
                        sxalBet.BetStatus = SXALBetStatusEnum.L;
                        break;
                    case BetStatusEnum.M:
                        sxalBet.BetStatus = SXALBetStatusEnum.M;
                        break;
                    case BetStatusEnum.MU:
                        sxalBet.BetStatus = SXALBetStatusEnum.MU;
                        break;
                    case BetStatusEnum.S:
                        sxalBet.BetStatus = SXALBetStatusEnum.S;
                        break;
                    case BetStatusEnum.U:
                        sxalBet.BetStatus = SXALBetStatusEnum.U;
                        break;
                    case BetStatusEnum.V:
                        sxalBet.BetStatus = SXALBetStatusEnum.V;
                        break;
                }
                
                sxalBet.BetType        = (SXALBetTypeEnum)bet.betType;
                sxalBet.CancelledDate  = bet.cancelledDate;
                sxalBet.FullMarketName = bet.fullMarketName;
                sxalBet.MarketId       = bet.marketId;
                sxalBet.MarketType     = (SXALMarketTypeEnum)bet.marketType;
                sxalBet.MatchedDate    = bet.matchedDate;
                sxalBet.MatchedSize    = bet.matchedSize;
                ArrayList alMatches    = new ArrayList();
                if (bet.matches != null)
                {
                    foreach (Match m in bet.matches)
                    {
                        SXALMatch sm = new SXALMatch();
                        sm.SizeMatched = m.sizeMatched;
                        alMatches.Add(sm);
                    }
                }
                sxalBet.Matches       = (SXALMatch[])alMatches.ToArray(typeof(SXALMatch));
                sxalBet.PlacedDate    = bet.placedDate;
                sxalBet.Price         = bet.price;
                sxalBet.ProfitAndLoss = bet.profitAndLoss;
                sxalBet.RemainingSize = bet.remainingSize;
                sxalBet.RequestedSize = bet.requestedSize;
                sxalBet.SelectionId   = bet.selectionId;
                sxalBet.SelectionName = bet.selectionName;
                sxalBet.SettledDate   = bet.settledDate;

                if (sxalBet.BetStatus == SXALBetStatusEnum.M && sxalBet.RemainingSize > 0)
                    sxalBet.BetStatus = SXALBetStatusEnum.MU;
            }
            return sxalBet;
        }

        SXALBet ISXAL.placeBackBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, bool isInplay)
        {
            Bet bet = null;
            if (money < getMinStakeForCurrency(_currency))
            {
                bet = placeBackBetBelowMin((int)marketId, (int)selectionId, asianId, price, money);
            }
            else
            {
                bet = placeBackBet((int)marketId, (int)selectionId, asianId, keepBet, price, money);
            }

            SXALBet sxalBet = null;
            if (bet != null)
            {
                sxalBet = new SXALBet();
                sxalBet.AsianLineId = bet.asianLineId;
                sxalBet.AvgPrice = bet.avgPrice;
                sxalBet.BetId = bet.betId;
                switch (bet.betStatus)
                {
                    case BetStatusEnum.C:
                        sxalBet.BetStatus = SXALBetStatusEnum.C;
                        break;
                    case BetStatusEnum.L:
                        sxalBet.BetStatus = SXALBetStatusEnum.L;
                        break;
                    case BetStatusEnum.M:
                        sxalBet.BetStatus = SXALBetStatusEnum.M;
                        break;
                    case BetStatusEnum.MU:
                        sxalBet.BetStatus = SXALBetStatusEnum.MU;
                        break;
                    case BetStatusEnum.S:
                        sxalBet.BetStatus = SXALBetStatusEnum.S;
                        break;
                    case BetStatusEnum.U:
                        sxalBet.BetStatus = SXALBetStatusEnum.U;
                        break;
                    case BetStatusEnum.V:
                        sxalBet.BetStatus = SXALBetStatusEnum.V;
                        break;
                }
                sxalBet.BetType = (SXALBetTypeEnum)bet.betType;
                sxalBet.CancelledDate = bet.cancelledDate;
                sxalBet.FullMarketName = bet.fullMarketName;
                sxalBet.MarketId = bet.marketId;
                sxalBet.MarketType = (SXALMarketTypeEnum)bet.marketType;
                sxalBet.MatchedDate = bet.matchedDate;
                sxalBet.MatchedSize = bet.matchedSize;
                ArrayList alMatches = new ArrayList();
                if (bet.matches != null)
                {
                    foreach (Match m in bet.matches)
                    {
                        SXALMatch sm = new SXALMatch();
                        sm.SizeMatched = m.sizeMatched;
                        alMatches.Add(sm);
                    }
                }
                sxalBet.Matches = (SXALMatch[])alMatches.ToArray(typeof(SXALMatch));
                sxalBet.PlacedDate = bet.placedDate;
                sxalBet.Price = bet.price;
                sxalBet.ProfitAndLoss = bet.profitAndLoss;
                sxalBet.RemainingSize = bet.remainingSize;
                sxalBet.RequestedSize = bet.requestedSize;
                sxalBet.SelectionId = bet.selectionId;
                sxalBet.SelectionName = bet.selectionName;
                sxalBet.SettledDate = bet.settledDate;

                if (sxalBet.BetStatus == SXALBetStatusEnum.M && sxalBet.RemainingSize > 0)
                    sxalBet.BetStatus = SXALBetStatusEnum.MU;
            }
            return sxalBet;
        }

        SXALSelection[] ISXAL.getSelections(SXALMarket market)
        {
            //ArrayList alSel = new ArrayList();
            SortedList<String, SXALSelection> listSelections = new SortedList<String, SXALSelection>();
            //Noch keine Selektionen vorhanden? 
            if (market.Selections == null)
            {
                Market bfMarket = getMarket((int)market.Id);

                ArrayList alSelection = new ArrayList();
                foreach (Runner r in bfMarket.runners)
                {
                    SXALSelection s = new SXALSelection();
                    s.Id = r.selectionId;
                    s.Name = translateHorseName(r.name.Trim());
                    s.IsNonStarter = false;
                    alSelection.Add(s);
                }

                market.Selections = (SXALSelection[])alSelection.ToArray(typeof(SXALSelection));
            }

            MarketPrices marketPrices = getMarketPrices((int)market.Id);

            if (marketPrices != null)
            {
                // Nichtstarter ermitteln.
                String[] removedRunnersSplit = marketPrices.removedRunners.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String rrs in removedRunnersSplit)
                {
                    String[] dataSplit = rrs.Split(new char[] { ',' });
                    SXALSelection s = new SXALSelection();
                    s.Name = translateHorseName(dataSplit[0].Trim());
                    s.IsNonStarter = true;
                    listSelections.Add(s.Name, s);                    
                }

                /*
                foreach (RunnerPrices r in marketPrices.runnerPrices)
                {
                    SXALSelection s = new SXALSelection();
                    s.Id = r.selectionId;
                    s.IsNonStarter = false;
                    alSel.Add(s);

                }
                 */
            }

            ArrayList list = new ArrayList(market.Selections.Length);
            foreach (SXALSelection s in market.Selections)
            {
                list.Add(s);
                if (listSelections.ContainsKey(s.Name))
                {
                    s.IsNonStarter = true;
                    listSelections.Remove(s.Name);
                }
            }


            if (listSelections.Count > 0)
            {
                foreach (SXALSelection s in listSelections.Values)
                {
                    list.Add(s);
                }
                market.Selections = (SXALSelection[])list.ToArray(typeof(SXALSelection));
            }


            return market.Selections;
        }

        SXALMarketPrices ISXAL.getMarketPrices(long marketId, bool canThrowThrottleExceeded)
        {
            MarketPrices marketPrices = getMarketPrices((int)marketId, canThrowThrottleExceeded);
            SXALMarketPrices sxalMarketPrices = null;
            if (marketPrices != null)
            {
                sxalMarketPrices = new SXALMarketPrices();
                sxalMarketPrices.CurrencyCode = marketPrices.currencyCode;
                sxalMarketPrices.MarketBaseRate = marketPrices.marketBaseRate;
                sxalMarketPrices.MarketId = marketPrices.marketId;

                sxalMarketPrices.MarketStatus = (SXALMarketStatusEnum)marketPrices.marketStatus;
                ArrayList alRunnerPrices = new ArrayList();
                if (marketPrices.runnerPrices != null)
                {
                    foreach (RunnerPrices r in marketPrices.runnerPrices)
                    {
                        SXALRunnerPrices sr = new SXALRunnerPrices();
                        sr.AsianLineId = (int)r.asianLineId;

                        ArrayList alBackPrices = new ArrayList();
                        if (r.bestPricesToBack != null)
                        {
                            foreach (Price bp in r.bestPricesToBack)
                            {
                                SXALPrice p = new SXALPrice();
                                p.Price = bp.price;
                                p.Stake = bp.amountAvailable;
                                alBackPrices.Add(p);
                            }
                        }
                        sr.BestPricesToBack = (SXALPrice[])alBackPrices.ToArray(typeof(SXALPrice));

                        ArrayList alLayPrices = new ArrayList();
                        if (r.bestPricesToLay != null)
                        {
                            foreach (Price lp in r.bestPricesToLay)
                            {
                                SXALPrice p = new SXALPrice();
                                p.Price = lp.price;
                                p.Stake = lp.amountAvailable;
                                alLayPrices.Add(p);
                            }
                        }
                        sr.BestPricesToLay = (SXALPrice[])alLayPrices.ToArray(typeof(SXALPrice));


                        sr.SelectionId = r.selectionId;
                        sr.TotalAmountMatched = r.totalAmountMatched;

                        alRunnerPrices.Add(sr);
                    }
                    sxalMarketPrices.RunnerPrices = (SXALRunnerPrices[])alRunnerPrices.ToArray(typeof(SXALRunnerPrices));
                }
            }
            return sxalMarketPrices;
        }

        SXALBet ISXAL.getBetDetail(long betId)
        {
            Bet bet = getBetDetail(betId);

            SXALBet sxalBet = null;
            if (bet != null)
            {
                sxalBet = new SXALBet();
                sxalBet.AsianLineId = bet.asianLineId;
                sxalBet.AvgPrice = bet.avgPrice;
                sxalBet.BetId = bet.betId;
                switch (bet.betStatus)
                {
                    case BetStatusEnum.C:
                        sxalBet.BetStatus = SXALBetStatusEnum.C;
                        break;
                    case BetStatusEnum.L:
                        sxalBet.BetStatus = SXALBetStatusEnum.L;
                        break;
                    case BetStatusEnum.M:
                        sxalBet.BetStatus = SXALBetStatusEnum.M;
                        break;
                    case BetStatusEnum.MU:
                        sxalBet.BetStatus = SXALBetStatusEnum.MU;
                        break;
                    case BetStatusEnum.S:
                        sxalBet.BetStatus = SXALBetStatusEnum.S;
                        break;
                    case BetStatusEnum.U:
                        sxalBet.BetStatus = SXALBetStatusEnum.U;
                        break;
                    case BetStatusEnum.V:
                        sxalBet.BetStatus = SXALBetStatusEnum.V;
                        break;
                }

                
                sxalBet.BetType = (SXALBetTypeEnum)bet.betType;
                sxalBet.CancelledDate = bet.cancelledDate;
                sxalBet.FullMarketName = bet.fullMarketName;
                sxalBet.MarketId = bet.marketId;
                sxalBet.MarketType = (SXALMarketTypeEnum)bet.marketType;
                sxalBet.MatchedDate = bet.matchedDate;
                sxalBet.MatchedSize = bet.matchedSize;
                ArrayList alMatches = new ArrayList();
                if (bet.matches != null)
                {
                    foreach (Match m in bet.matches)
                    {
                        SXALMatch sm = new SXALMatch();
                        sm.SizeMatched = m.sizeMatched;
                        alMatches.Add(sm);
                    }
                }
                sxalBet.Matches = (SXALMatch[])alMatches.ToArray(typeof(SXALMatch));
                sxalBet.PlacedDate = bet.placedDate;
                sxalBet.Price = bet.price;
                sxalBet.ProfitAndLoss = bet.profitAndLoss;
                sxalBet.RemainingSize = bet.remainingSize;
                sxalBet.RequestedSize = bet.requestedSize;
                sxalBet.SelectionId = bet.selectionId;
                sxalBet.SelectionName = bet.selectionName;
                sxalBet.SettledDate = bet.settledDate;

                if (sxalBet.BetStatus == SXALBetStatusEnum.M && sxalBet.RemainingSize > 0)
                    sxalBet.BetStatus = SXALBetStatusEnum.MU;
            }
            return sxalBet;
        }

        SXALMUBet[] ISXAL.getBetsMU(long marketId)
        {
            MUBet[] muBets = getBetsMU((int)marketId);

            ArrayList alMu = new ArrayList();

            if (muBets != null)
            {
                foreach (MUBet b in muBets)
                {
                    SXALMUBet sb = new SXALMUBet();
                    sb.BetId       = b.betId;
                    sb.AsianLineId = b.asianLineId;
                    sb.BetStatus   = (SXALBetStatusEnum)b.betStatus;
                    sb.BetType     = (SXALBetTypeEnum)b.betType;
                    sb.MarketId    = b.marketId;
                    sb.MatchedDate = b.matchedDate;
                    sb.SelectionId = b.selectionId;
                    sb.Size        = b.size;

                    alMu.Add(sb);
                }
            }

            return (SXALMUBet[])alMu.ToArray(typeof(SXALMUBet));
        }

        SXALMUBet[] ISXAL.getBetMU(long betId)
        {
            MUBet[] muBets = getBetMU(betId);
            ArrayList alMu = new ArrayList();

            if (muBets != null)
            {
                foreach (MUBet b in muBets)
                {
                    SXALMUBet sb = new SXALMUBet();
                    sb.BetId = b.betId;
                    sb.AsianLineId = b.asianLineId;
                    sb.BetStatus = (SXALBetStatusEnum)b.betStatus;
                    sb.BetType = (SXALBetTypeEnum)b.betType;
                    sb.MarketId = b.marketId;
                    sb.MatchedDate = b.matchedDate;
                    sb.SelectionId = b.selectionId;
                    sb.Size = b.size;

                    alMu.Add(sb);
                }
            }

            return (SXALMUBet[])alMu.ToArray(typeof(SXALMUBet));
        }

        SXALMUBet[] ISXAL.getBets(System.DateTime dts)
        {
            MUBet[] muBets = getBets(dts);

            ArrayList alMu = new ArrayList();

            if (muBets != null)
            {
                foreach (MUBet b in muBets)
                {
                    SXALMUBet sb = new SXALMUBet();
                    sb.BetId = b.betId;
                    sb.AsianLineId = b.asianLineId;
                    sb.BetStatus = (SXALBetStatusEnum)b.betStatus;
                    sb.BetType = (SXALBetTypeEnum)b.betType;
                    sb.MarketId = b.marketId;
                    sb.MatchedDate = b.matchedDate;
                    sb.SelectionId = b.selectionId;
                    sb.Size = b.size;

                    alMu.Add(sb);
                }
            }

            return (SXALMUBet[])alMu.ToArray(typeof(SXALMUBet));
        }

        SXALMarketLite ISXAL.getMarketInfo(long marketId)
        {
            MarketLite marketLite = getMarketInfo((int)marketId);
            SXALMarketLite sxalMarketLite = null;

            if (marketLite != null)
            {
                sxalMarketLite = new SXALMarketLite();                
                sxalMarketLite.MarketStatus = (SXALMarketStatusEnum)marketLite.marketStatus;                
            }

            return sxalMarketLite;
        }

        SXALEventType[] ISXAL.loadEvents()
        {
            EventType[] eventTypes = loadEvents();
            ArrayList alEventType = new ArrayList();

            if (eventTypes != null)
            {
                foreach (EventType e in eventTypes)
                {
                    SXALEventType se = new SXALEventType();
                    se.Id   = e.id;
                    se.Name = e.name;

                    alEventType.Add(se);
                }
            }

            return (SXALEventType[])alEventType.ToArray(typeof(SXALEventType));
        }

        public string getCurrency()
        {
            return _currency;
        }

        bool ISXAL.SupportsBelowMinStakeBetting { get { return true; } }

        public double MinStake
        {
            get 
            {
                double minStake = 0.0;
                
                switch (_currency)
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
                        throw new Exception(String.Format("Unknow currency {0}. No minimum bet amount known.", _currency));
                }
                return minStake;
            }
        }

        SXALMarket[] ISXAL.getAllMarkets(int?[] eventids, DateTime fromDate, DateTime toDate)
        {
            ArrayList alMt = new ArrayList();
            String result = getAllMarkets(eventids, fromDate, toDate);

            if (result != null)
            {
                String[] splittedUpResult = result.Split(new char[] { ':' });

                foreach (String str in splittedUpResult)
                {
                    if (str == String.Empty)
                        continue;

                    String[] marketDatas = str.Split(new char[] {'~'});

                    if (marketDatas.Length < 2)
                        throw new FormatException("Invalid MarketData from GetAllMarkets");


                    SXALMarket m = new SXALMarket();

                    try
                    {
                        m.Id = Int32.Parse(marketDatas[0]);
                    }
                    catch (Exception e)
                    {
                        ExceptionWriter.Instance.WriteException(e);
                        continue;
                    }

                    m.Name = marketDatas[1];

                    /*
                    if (m.Name.Equals("Total Goals", StringComparison.OrdinalIgnoreCase) && marketDatas.Length >7)
                    {
                        //string asianId = marketDatas[7];
                    }
                    */


                    if (marketDatas.Length < 6)
                        continue;
                    m.Match = splitFullMarketName(marketDatas[5]);
                    m.Devision = splitDevision(marketDatas[5]);

                    DateTime startDts = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    startDts = startDts.AddMilliseconds(Double.Parse(marketDatas[4]));
                    m.StartDTS = startDts.ToLocalTime();
                    
                    if (marketDatas.Length < 16)
                    {
                        m.InPlayMarket = false;
                        continue;
                    }

                    if (marketDatas[15] == "Y")
                    {
                        m.InPlayMarket = true;
                    }
                    else
                    {
                        m.InPlayMarket = false;
                    }


                    alMt.Add(m);
                }
            }

            return (SXALMarket[])alMt.ToArray(typeof(SXALMarket));
        }

        String ISXAL.getExchangeName()
        {
            return "Betfair";
        }


        long ISXAL.getSelectionIdByName(String name, SXALMarket m)
        {
            //Marktinformationen Nachladen
            if (m.Selections == null)
            {
                Market market = getMarket((int)m.Id);
                
                if (market != null)
                {
                    ArrayList alS = new ArrayList();
                    foreach (Runner r in market.runners)
                    {
                        SXALSelection s = new SXALSelection();
                        s.Id = r.selectionId;
                        s.Name = r.name;       
                        
                        alS.Add(s);
                    }
                    m.Selections = (SXALSelection[])alS.ToArray(typeof(SXALSelection));
                }
            }

            foreach (SXALSelection s in m.Selections)
            {
                if (s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return s.Id;
            }

            return -1;
        }

        long ISXAL.getSelectionId(SXALSelectionIdEnum selectionToGet, SXALMarket m)
        {
            switch (selectionToGet)
            {
                case SXALSelectionIdEnum.DRAW:
                    return DRAWSELECTIONID;
                case SXALSelectionIdEnum.OVER05:
                    return (long) OVERUNDERSELCTIONIDS.OVER05;
                case SXALSelectionIdEnum.OVER15:
                    return (long)OVERUNDERSELCTIONIDS.OVER15;
                case SXALSelectionIdEnum.OVER25:
                    return (long)OVERUNDERSELCTIONIDS.OVER25;
                case SXALSelectionIdEnum.OVER35:
                    return (long)OVERUNDERSELCTIONIDS.OVER35;
                case SXALSelectionIdEnum.OVER45:
                    return (long)OVERUNDERSELCTIONIDS.OVER45;
                case SXALSelectionIdEnum.OVER55:
                    return (long)OVERUNDERSELCTIONIDS.OVER55;
                case SXALSelectionIdEnum.OVER65:
                    return (long)OVERUNDERSELCTIONIDS.OVER65;
                case SXALSelectionIdEnum.OVER75:
                    return (long)OVERUNDERSELCTIONIDS.OVER75;
                case SXALSelectionIdEnum.OVER85:
                    return (long)OVERUNDERSELCTIONIDS.OVER85;
                case SXALSelectionIdEnum.CSZEROZERO:
                    return (long)SCORESELECTIONIDS.ZEROTOZERO;
                case SXALSelectionIdEnum.CSZEROONE:
                    return (long)SCORESELECTIONIDS.ZEROTOONE;
                case SXALSelectionIdEnum.CSZEROTWO:
                    return (long)SCORESELECTIONIDS.ZEROTOTWO;
                case SXALSelectionIdEnum.CSZEROTHREE:
                    return (long)SCORESELECTIONIDS.ZEROTOTHREE;
                case SXALSelectionIdEnum.CSONEZERO:
                    return (long)SCORESELECTIONIDS.ONETOZERO;
                case SXALSelectionIdEnum.CSONEONE:
                    return (long)SCORESELECTIONIDS.ONETOONE;
                case SXALSelectionIdEnum.CSONETWO:
                    return (long)SCORESELECTIONIDS.ONETOTWO;
                case SXALSelectionIdEnum.CSONETHREE:
                    return (long)SCORESELECTIONIDS.ONETOTHREE;
                case SXALSelectionIdEnum.CSTWOZERO:
                    return (long)SCORESELECTIONIDS.TWOTOZERO;
                case SXALSelectionIdEnum.CSTWOONE:
                    return (long)SCORESELECTIONIDS.TWOTOONE;
                case SXALSelectionIdEnum.CSTWOTWO:
                    return (long)SCORESELECTIONIDS.TWOTOTWO;
                case SXALSelectionIdEnum.CSTWOTHREE:
                    return (long)SCORESELECTIONIDS.TWOTOTHREE;
                case SXALSelectionIdEnum.CSTHREEZERO:
                    return (long)SCORESELECTIONIDS.THREETOZERO;
                case SXALSelectionIdEnum.CSTHREEONE:
                    return (long)SCORESELECTIONIDS.THREETOONE;
                case SXALSelectionIdEnum.CSTHREETWO:
                    return (long)SCORESELECTIONIDS.THREETOTWO;
                case SXALSelectionIdEnum.CSTHREETHREE:
                    return (long)SCORESELECTIONIDS.THREETOTHREE;
                case SXALSelectionIdEnum.CSOTHER:
                    return (long)SCORESELECTIONIDS.OTHERS;
            }

            return -1;            
        }

        SXALSelectionIdEnum ISXAL.getReverseSelectionId(long selectionId, SXALMarket m)
        {
            try
            {
                if (selectionId == DRAWSELECTIONID)
                    return SXALSelectionIdEnum.DRAW;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER05)
                    return SXALSelectionIdEnum.OVER05;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER15)
                    return SXALSelectionIdEnum.OVER15;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER25)
                    return SXALSelectionIdEnum.OVER25;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER35)
                    return SXALSelectionIdEnum.OVER35;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER45)
                    return SXALSelectionIdEnum.OVER45;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER55)
                    return SXALSelectionIdEnum.OVER55;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER65)
                    return SXALSelectionIdEnum.OVER65;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER75)
                    return SXALSelectionIdEnum.OVER75;
                if (selectionId == (long)OVERUNDERSELCTIONIDS.OVER85)
                    return SXALSelectionIdEnum.OVER85;
                if (selectionId == (long)SCORESELECTIONIDS.ZEROTOZERO)
                    return SXALSelectionIdEnum.CSZEROZERO;
                if (selectionId == (long)SCORESELECTIONIDS.ZEROTOONE)
                    return SXALSelectionIdEnum.CSZEROONE;
                if (selectionId == (long)SCORESELECTIONIDS.ZEROTOTWO)
                    return SXALSelectionIdEnum.CSZEROTWO;
                if (selectionId == (long)SCORESELECTIONIDS.ZEROTOTHREE)
                    return SXALSelectionIdEnum.CSZEROTHREE;
                if (selectionId == (long)SCORESELECTIONIDS.ONETOZERO)
                    return SXALSelectionIdEnum.CSONEZERO;
                if (selectionId == (long)SCORESELECTIONIDS.ONETOONE)
                    return SXALSelectionIdEnum.CSONEONE;
                if (selectionId == (long)SCORESELECTIONIDS.ONETOTWO)
                    return SXALSelectionIdEnum.CSONETWO;
                if (selectionId == (long)SCORESELECTIONIDS.ONETOTHREE)
                    return SXALSelectionIdEnum.CSONETHREE;
                if (selectionId == (long)SCORESELECTIONIDS.TWOTOZERO)
                    return SXALSelectionIdEnum.CSTWOZERO;
                if (selectionId == (long)SCORESELECTIONIDS.TWOTOONE)
                    return SXALSelectionIdEnum.CSTWOONE;
                if (selectionId == (long)SCORESELECTIONIDS.TWOTOTWO)
                    return SXALSelectionIdEnum.CSTWOTWO;
                if (selectionId == (long)SCORESELECTIONIDS.TWOTOTHREE)
                    return SXALSelectionIdEnum.CSTWOTHREE;
                if (selectionId == (long)SCORESELECTIONIDS.THREETOZERO)
                    return SXALSelectionIdEnum.CSTHREEZERO;
                if (selectionId == (long)SCORESELECTIONIDS.THREETOONE)
                    return SXALSelectionIdEnum.CSTHREEONE;
                if (selectionId == (long)SCORESELECTIONIDS.THREETOTWO)
                    return SXALSelectionIdEnum.CSTHREETWO;
                if (selectionId == (long)SCORESELECTIONIDS.THREETOTHREE)
                    return SXALSelectionIdEnum.CSTHREETHREE;
                if (selectionId == (long)SCORESELECTIONIDS.OTHERS)
                    return SXALSelectionIdEnum.CSOTHER;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

            return SXALSelectionIdEnum.UNKNOWN;
        }

        decimal ISXAL.validateOdd(decimal odds)
        {
            if (odds >= 100)
            {
                // Erlaubte Werte sind 10er Schritte
                if (odds % 10 != 0)
                {
                    int result = (int)((odds / 10) * 10);
                    odds = result;
                }
            }
            else if (odds >= 50 && odds < 100)
            {
                // Erlaubte Werte sind 5er Schritte
                if (odds % 5 != 0)
                {
                    int result = (int)((odds / 5) * 5);
                    odds = result;
                }
            }
            else if (odds >= 30 && odds < 50)
            {
                // Erlaubte Werte sind 2er Schritte
                if (odds % 2 != 0)
                {
                    int result = (int)((odds / 2) * 2);
                    odds = result;
                }
            }
            else if (odds >= 20 && odds <= 30)
            {
                //Erlaubte Werte sind 1er Schritte
                int result = (int)((odds / 1) * 1);
                odds = result;
            }
            else if (odds >= 10 && odds <= 20)
            {
                //Erlaubte Werte sind 0,5er Schritte
                int result = (int)(odds * 100);
                if (result % 50 != 0)
                {
                    result = (result / 50) * 50;
                    odds = (decimal)result / 100;
                }
            }
            else if (odds >= 6 && odds < 10)
            {
                //Erlaubte Werte sind 0,2er Schritte
                int result = (int)(odds * 100);
                if (result % 20 != 0)
                {
                    result = (result / 20) * 20;
                    odds = (decimal)result / 100;
                }
            }
            else if (odds >= 4 && odds < 6)
            {
                //Erlaubte Werte sind 0,1er Schritte
                int result = (int)(odds * 100);
                if (result % 10 != 0)
                {
                    result = (result / 10) * 10;
                    odds = (decimal)result / 100;
                }
            }
            else if (odds >= 3 && odds < 4)
            {
                //Erlaubte Werte sind 0,05er Schritte
                int result = (int)(odds * 1000);
                if (result % 50 != 0)
                {
                    result = (result / 50) * 50;
                    odds = (decimal)result / 1000;
                }
            }
            else if (odds >= 2 && odds < 3)
            {
                //Erlaubte Werte sind 0,02er Schritte
                int result = (int)(odds * 1000);
                if (result % 20 != 0)
                {
                    result = (result / 20) * 20;
                    odds = (decimal)result / 1000;
                }
            }
            return (decimal)odds;
        }

        decimal ISXAL.getValidOddIncrement(decimal odds)
        {
            decimal incr = (decimal)0.01;
            if (odds >= 100)
            {
                // Erlaubte Werte sind 10er Schritte
                incr = 10;
            }
            else if (odds >= 50 && odds < 100)
            {
                // Erlaubte Werte sind 5er Schritte
                incr = 5;
            }
            else if (odds >= 30 && odds < 50)
            {
                // Erlaubte Werte sind 2er Schritte
                incr = 2;
            }
            else if (odds >= 20 && odds <= 30)
            {
                //Erlaubte Werte sind 1er Schritte
                incr = 1;
            }
            else if (odds >= 10 && odds <= 20)
            {
                //Erlaubte Werte sind 0,5er Schritte
                incr = (decimal)0.5;
            }
            else if (odds >= 6 && odds < 10)
            {
                //Erlaubte Werte sind 0,2er Schritte
                incr = (decimal)0.2;
            }
            else if (odds >= 4 && odds < 6)
            {
                //Erlaubte Werte sind 0,1er Schritte
                incr = (decimal)0.1;
            }
            else if (odds >= 3 && odds < 4)
            {
                //Erlaubte Werte sind 0,05er Schritte
                incr = (decimal)0.05;
            }
            else if (odds >= 2 && odds < 3)
            {
                //Erlaubte Werte sind 0,02er Schritte
                incr = (decimal)0.02;
            }
            return incr;
        }

        #endregion

        private String splitFullMarketName(String fullmarketname)
        {
            try
            {
                char[] cSeps = { '\\' };
                String[] splittedMarketName = fullmarketname.Split(cSeps);
                splittedMarketName[splittedMarketName.Length - 1] = splittedMarketName[splittedMarketName.Length - 1].Replace(" v ", " - ");
                return splittedMarketName[splittedMarketName.Length - 1];
            }
            catch (IndexOutOfRangeException ioore)
            {
                ExceptionWriter.Instance.WriteException(ioore);
                throw ioore;
            }
        }

        private static String splitDevision(String fullmarketname)
        {
            try
            {
                char[] cSeps = { '\\' };
                String[] splittedMarketName = fullmarketname.Split(cSeps);
                splittedMarketName[splittedMarketName.Length - 1] = splittedMarketName[splittedMarketName.Length - 1].Replace(" v ", " - ");

                return splittedMarketName[2];
            }
            catch (IndexOutOfRangeException ioore)
            {
                ExceptionWriter.Instance.WriteException(ioore);
                throw ioore;
            }
        }

       
    }
}