using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.betdaqif.betdaq;
using System.Collections;
using System;
namespace net.sxtrader.betdaqif
{
    public sealed partial class BetdaqKom : ISXAL
    {
        #region ISXAL Member

        private SXALEventType[] _events;

        bool ISXAL.login(string usr, string pwd)
        {
            _usr = usr;
            _pwd = pwd;
            _isLoggedIn = login();
            return _isLoggedIn;
        }

        SXALMarket ISXAL.translateHorseMarket(SXALMarket market)
        {
            market.Match = market.Match.Trim();

            if (market.Match.Equals("Newton Abbot"))
            {
                market.Match = "Newton";
            }

            if (market.Match.Equals("Epsom Downs"))
            {
                market.Match = "Epsom";
            }
/*
            if (market.Match.Equals("Kempton"))
            {
                market.Match = "Kempton";
            }
*/
            foreach (SXALSelection s in market.Selections)
            {
                s.Name = translateHorseName(s.Name.Trim());
            }
            return market;
        }

        SXALBet ISXAL.placeLayBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, bool isInplay)
        {           
            if (money < this.MinStake)
                throw new SXALNoBetBelowMinAllowedException("Below MinStake Betting not supported"); //TODO: Spezielle Exception 
            return placeBet(marketId, selectionId, asianId, keepBet, price, money, SXALBetTypeEnum.L, isInplay);            
        }

        SXALBet ISXAL.placeBackBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, bool isInplay)
        {                 
            if (money < this.MinStake)
                throw new SXALNoBetBelowMinAllowedException("Below MinStake Betting not supported"); //TODO: Spezielle Exception 
            return placeBet(marketId, selectionId, asianId, keepBet, price, money, SXALBetTypeEnum.B, isInplay);            
        }

        bool ISXAL.cancelBet(long betId)
        {
            return cancelOrder(betId);
        }

        SXALSelection[] ISXAL.getSelections(SXALMarket market)
        {
            ArrayList alSel = new ArrayList();

            MarketType marketType = getMarketInfo(market.Id);

            if (marketType.Selections != null)
            {
                foreach (SelectionType s in marketType.Selections)
                {
                    SXALSelection selection = new SXALSelection();
                    selection.Id = s.Id;
                    selection.Name = translateHorseName(s.Name.Trim());
                    if (s.Status == 4 || s.Status == 5 || s.Status == 9)
                        selection.IsNonStarter = true;

                    alSel.Add(selection);
                }
            }

            return (SXALSelection[])alSel.ToArray(typeof(SXALSelection)); ;
        }

        SXALMarketPrices ISXAL.getMarketPrices(long marketId, bool canThrowThrottleExceeded)
        {
            ArrayList alMp = new ArrayList();
            SXALMarketPrices mp = new SXALMarketPrices();
            MarketTypeWithPrices[] prices = internalGetMarketPrices(marketId);

            if (prices == null)
                return null;

            if (prices.Length < 1)
                return null;
            mp.MarketId = prices[0].Id;
            
            switch (prices[0].Status)
            {
                case 1:
                    mp.MarketStatus = SXALMarketStatusEnum.INACTIVE;
                    break;
                case 2:
                    mp.MarketStatus = SXALMarketStatusEnum.ACTIVE;
                    break;
                case 3:
                    mp.MarketStatus = SXALMarketStatusEnum.SUSPENDED;
                    break;
                case 4:
                    mp.MarketStatus = SXALMarketStatusEnum.CLOSED;
                    break;
                case 6:
                    mp.MarketStatus = SXALMarketStatusEnum.CLOSED;
                    break;

            }


            ArrayList alRp = new ArrayList();
            if (prices[0].Selections != null)
            {
                foreach (SelectionTypeWithPrices sp in prices[0].Selections)
                {

                    SXALRunnerPrices runnerPrices = new SXALRunnerPrices();
                    runnerPrices.SelectionId = sp.Id;
                    runnerPrices.SelectionName = sp.Name;                    

                    runnerPrices.TotalAmountMatched = (double)(sp.MatchedSelectionAgainstStake + sp.MatchedSelectionForStake);
                    ArrayList alBp = new ArrayList();
                    if (sp.ForSidePrices != null)
                    {
                        foreach (PricesType bp in sp.ForSidePrices)
                        {
                            SXALPrice price = new SXALPrice();
                            price.Price = (double)bp.Price;
                            price.Stake = (double)bp.Stake;
                            alBp.Add(price);
                        }
                    }
                    runnerPrices.BestPricesToBack = (SXALPrice[])alBp.ToArray(typeof(SXALPrice));



                    ArrayList alLp = new ArrayList();
                    if (sp.AgainstSidePrices != null)
                    {
                        foreach (PricesType lp in sp.AgainstSidePrices)
                        {
                            SXALPrice price = new SXALPrice();
                            price.Price = (double)lp.Price;
                            price.Stake = (double)lp.Stake;
                            alLp.Add(price);
                        }
                    }

                    runnerPrices.BestPricesToLay = (SXALPrice[])alLp.ToArray(typeof(SXALPrice));

                    alRp.Add(runnerPrices);
                }
            }
            mp.RunnerPrices = (SXALRunnerPrices[])alRp.ToArray(typeof(SXALRunnerPrices));
            return mp;

        }

        void ISXAL.getAccounFounds(out double availBalance, out double currentBalance, out double creditLimit)
        {
            internalGetAccounFounds(out availBalance, out currentBalance, out creditLimit);
        }

        string ISXAL.getCurrency()
        {
            return _currency;
        }

        SXALBet ISXAL.getBetDetail(long betId)
        {
            return getOrderDetail(betId);
        }

        SXALMUBet[] ISXAL.getBetsMU(long marketId)
        {
            SXALMUBet[] mus = this.getBets(DateTime.MinValue);

            ArrayList alMu = new ArrayList();

            if (mus != null)
            {
                foreach (SXALMUBet m in mus)
                {
                    if (m.MarketId == marketId)
                    {
                        alMu.Add(m);
                    }
                }
            }

            return (SXALMUBet[])alMu.ToArray(typeof(SXALMUBet));
        }

        SXALMUBet[] ISXAL.getBetMU(long betId)
        {
            ArrayList alMu = new ArrayList();
            SXALBet bet = getOrderDetail(betId);
            if (bet != null)
            {
                SXALMUBet mu = new SXALMUBet();
                mu.AsianLineId = bet.AsianLineId;
                mu.BetId = bet.BetId;
                mu.BetStatus = bet.BetStatus;
                mu.BetType = bet.BetType;
                mu.MarketId = bet.MarketId;
                mu.MatchedDate = bet.MatchedDate;
                mu.SelectionId = bet.SelectionId;
                mu.Size = bet.MatchedSize;
                alMu.Add(mu);
            }

            return (SXALMUBet[])alMu.ToArray(typeof(SXALMUBet));
        }

        public SXALMUBet[] getBets(System.DateTime dts)
        {
            Order[] orders = listBootstrapOrders(out _maxSequenceNo);
            ArrayList alMu = new ArrayList();

            foreach (Order o in orders)
            {
                if (o.IssuedAt < dts)
                    continue;
                SXALMUBet b = new SXALMUBet();
                b.AsianLineId = 0;
                b.BetId = o.Id;
                
                switch (o.Status)
                {
                    case 1:
                        b.BetStatus = SXALBetStatusEnum.U;
                        if (o.MatchedStake > 0 && o.UnmatchedStake > 0)
                            b.BetStatus = SXALBetStatusEnum.MU;
                        break;
                    case 2:
                        b.BetStatus = SXALBetStatusEnum.M;
                        break;
                    case 3:
                        b.BetStatus = SXALBetStatusEnum.C;
                        break;
                    case 4:
                        b.BetStatus = SXALBetStatusEnum.S;
                        break;
                    case 5:
                        b.BetStatus = SXALBetStatusEnum.V;
                        break;
                }


                switch (o.Polarity)
                {
                    case 1:
                        b.BetType = SXALBetTypeEnum.B;
                        break;
                    case 2:
                        b.BetType = SXALBetTypeEnum.L;
                        break;
                }


                b.MarketId = o.MarketId;
                b.MatchedDate = o.IssuedAt;
                b.SelectionId = o.SelectionId;
                b.Size = (double)o.MatchedStake;

                alMu.Add(b);
            }

            return (SXALMUBet[])alMu.ToArray(typeof(SXALMUBet));
        }

        SXALMarketLite ISXAL.getMarketInfo(long marketId)
        {
            SXALMarketLite m = new SXALMarketLite();

            MarketType t = getMarketInfo(marketId);

            if (t == null)
                return null;

            switch (t.Status)
            {
                case 1:
                    m.MarketStatus = SXALMarketStatusEnum.INACTIVE;
                    break;
                case 2:
                    m.MarketStatus = SXALMarketStatusEnum.ACTIVE;
                    break;
                case 3:
                    m.MarketStatus = SXALMarketStatusEnum.SUSPENDED;
                    break;
                case 4:
                    m.MarketStatus = SXALMarketStatusEnum.COMPLETED;
                    break;
                case 6:
                    m.MarketStatus = SXALMarketStatusEnum.SETTLED;
                    break;

            }

            return m;
        }

        SXALMarket[] ISXAL.getAllMarkets(int?[] eventids, System.DateTime fromDate, System.DateTime toDate)
        {
            long[] lEventIds = new long[eventids.Length];
            for (int i = 0; i < eventids.Length; i++)
            {
                lEventIds[i] = (long)eventids[i];
            }

            return  getEventSubtreeNoSelections(lEventIds, fromDate, toDate);
          
        }

        SXALEventType[] ISXAL.loadEvents()
        {
            EventClassifierType[] events =  getTopLevelEvents();

            ArrayList alEvents = new ArrayList();
            foreach (EventClassifierType ect in events)
            {
                SXALEventType set = new SXALEventType();

                set.Id = ect.Id;
                set.Name = ect.Name;

                alEvents.Add(set);
            }
            _events = (SXALEventType[])alEvents.ToArray(typeof(SXALEventType));
            return (SXALEventType[])alEvents.ToArray(typeof(SXALEventType));          
        }

        bool ISXAL.SupportsBelowMinStakeBetting { get { return false; } }

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
                    case USD:
                        minStake = MINUSD;
                        break;                    
                    case SGD:
                        minStake = MINSGD;
                        break;                    
                    default:
                        throw new Exception(String.Format("Unknow currency {0}. No minimum bet amount known.", _currency));
                }
                return minStake;
            }
        }


        String ISXAL.getExchangeName()
        {
            return "Betdaq";
        }


        long ISXAL.getSelectionIdByName(String name, SXALMarket m)
        {
            foreach (SXALSelection s in m.Selections)
            {
                if (s.Name.Equals(name.Trim(), StringComparison.CurrentCultureIgnoreCase))
                    return s.Id;
            }
            return -1;
        }

        long ISXAL.getSelectionId(SXALSelectionIdEnum selectionToGet, SXALMarket m)
        {                         
            switch (selectionToGet)
            {
                case SXALSelectionIdEnum.DRAW:
                    foreach(SXALSelection s in m.Selections)
                    {
                        if (s.Name.Equals("Draw", StringComparison.CurrentCultureIgnoreCase))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER05:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Over") && s.Name.Contains("0.5"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER15:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Over") && s.Name.Contains("1.5"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER25:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if(s.Name.Contains("Over") && s.Name.Contains("2.5"))                        
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER35:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Over") && s.Name.Contains("3.5"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER45:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Over") && s.Name.Contains("4.5"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER55:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Over") && s.Name.Contains("5.5"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER65:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Over") && s.Name.Contains("6.5"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER75:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Over") && s.Name.Contains("7.5"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.OVER85:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Over") && s.Name.Contains("8.5"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSZEROZERO:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith("Draw") && s.Name.Contains("0-0"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSZEROONE:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("1-0"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSZEROTWO:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("2-0"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSZEROTHREE:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("3-0"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSONEZERO:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("1-0"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSONEONE:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith("Draw") && s.Name.Contains("1-1"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSONETWO:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("2-1"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSONETHREE:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("3-1"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSTWOZERO:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("2-0"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSTWOONE:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("2-1"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSTWOTWO:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith("Draw") && s.Name.Contains("2-2"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSTWOTHREE:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("3-2"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSTHREEZERO:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("3-0"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSTHREEONE:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("3-1"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSTHREETWO:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("3-2"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSTHREETHREE:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.StartsWith("Draw") && s.Name.Contains("3-3"))
                            return s.Id;
                    }
                    break;
                case SXALSelectionIdEnum.CSOTHER:
                    foreach (SXALSelection s in m.Selections)
                    {
                        if (s.Name.Contains("Other"))
                            return s.Id;
                    }
                    break;

            }
            return -1;
        }

        SXALSelectionIdEnum ISXAL.getReverseSelectionId(long selectionId, SXALMarket m)
        {
            if (m.IsMatchOdds)
                return SXALSelectionIdEnum.DRAW;
            else if (m.IsOverUnder05)
                return SXALSelectionIdEnum.OVER05;
            else if (m.IsOverUnder15)
                return SXALSelectionIdEnum.OVER15;
            else if (m.IsOverUnder25)
                return SXALSelectionIdEnum.OVER25;
            else if (m.IsOverUnder35)
                return SXALSelectionIdEnum.OVER35;
            else if (m.IsOverUnder45)
                return SXALSelectionIdEnum.OVER45;
            else if (m.IsOverUnder55)
                return SXALSelectionIdEnum.OVER55;
            else if (m.IsOverUnder65)
                return SXALSelectionIdEnum.OVER65;
            else if (m.IsOverUnder75)
                return SXALSelectionIdEnum.OVER75;
            else if (m.IsOverUnder85)
                return SXALSelectionIdEnum.OVER85;
            else if (m.IsScoreMarket)
            {
                foreach (SXALSelection s in m.Selections)
                {
                    if (selectionId != s.Id)
                        continue;
                    if (s.Name.StartsWith("Draw") && s.Name.Contains("0-0"))
                        return SXALSelectionIdEnum.CSZEROZERO;
                    if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("1-0"))
                        return SXALSelectionIdEnum.CSZEROONE;
                    if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("2-0"))
                        return SXALSelectionIdEnum.CSZEROTWO;
                    if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("3-0"))
                        return SXALSelectionIdEnum.CSZEROTHREE;
                    if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("1-0"))
                        return SXALSelectionIdEnum.CSONEZERO;
                    if (s.Name.StartsWith("Draw") && s.Name.Contains("1-1"))
                        return SXALSelectionIdEnum.CSONEONE;
                    if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("2-1"))
                        return SXALSelectionIdEnum.CSONETWO;
                    if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("3-1"))
                        return SXALSelectionIdEnum.CSONETHREE;
                    if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("2-0"))
                        return SXALSelectionIdEnum.CSTWOZERO;
                    if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("2-1"))
                        return SXALSelectionIdEnum.CSTWOONE;
                    if (s.Name.StartsWith("Draw") && s.Name.Contains("2-2"))
                        return SXALSelectionIdEnum.CSTWOTWO;
                    if (s.Name.StartsWith(m.TeamB) && s.Name.Contains("3-2"))
                        return SXALSelectionIdEnum.CSTWOTHREE;
                    if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("3-0"))
                        return SXALSelectionIdEnum.CSTHREEZERO;
                    if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("3-1"))
                        return SXALSelectionIdEnum.CSTHREEONE;
                    if (s.Name.StartsWith(m.TeamA) && s.Name.Contains("3-2"))
                        return SXALSelectionIdEnum.CSTHREETWO;
                    if (s.Name.StartsWith("Draw") && s.Name.Contains("3-3"))
                        return SXALSelectionIdEnum.CSTHREETHREE;
                    if (s.Name.Contains("Other"))
                        return SXALSelectionIdEnum.CSOTHER;
                }
            }
            throw new NotImplementedException();
        }

        decimal ISXAL.validateOdd(decimal odds)
        {
            //Immer laden?
            if(_ladder == null)
                loadOddsLadder();
            if (_ladder != null)
            {
                // 1. Über Intervall finden
                // 2. Falls in keinen intervall vorhanden
                //    nähsten Wert findeun und diesen zurück geben
                return validateOdd(odds, 0, _ladder.Length -1);
                
            }
            throw new NotImplementedException();
        }

        decimal ISXAL.getValidOddIncrement(decimal odds)
        {
            //Immer laden?
            if(_ladder == null)
                loadOddsLadder();
            if (_ladder != null)
            {
                return getValidOddIncrement(odds, 0, _ladder.Length - 1);
            }
            throw new NotImplementedException();
        }

        

        #endregion
    }
}