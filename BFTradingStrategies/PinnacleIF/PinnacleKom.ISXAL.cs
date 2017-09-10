using net.sxtrader.bftradingstrategies.SXALInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.pinnacleif
{ 
    public sealed partial class PinnacleKom : ISXAL
    {
        public double MinStake
        {
            get
            {
                return 5.0;
            }
        }

        public bool SupportsBelowMinStakeBetting
        {
            get
            {
                return false;
            }
        }

        public bool cancelBet(long betId)
        {
            throw new NotImplementedException();
        }

        public void getAccounFounds(out double availBalance, out double currentBalance, out double creditLimit)
        {
            string currency = string.Empty;
            internalGetAccounFounds(out availBalance, out currentBalance, out creditLimit, out currency);
        }

        public SXALMarket[] getAllMarkets(int?[] eventids, DateTime fromDate, DateTime toDate)
        {
            return internalGetAllMarkets(eventids, fromDate, toDate);
        }

        public SXALBet getBetDetail(long betId)
        {
            throw new NotImplementedException();
        }

        public SXALMUBet[] getBetMU(long betId)
        {
            throw new NotImplementedException();
        }

        public SXALMUBet[] getBets(DateTime dts)
        {
            return internalGetBets(dts);
        }

        public SXALMUBet[] getBetsMU(long marketId)
        {
            throw new NotImplementedException();
        }

        public string getCurrency()
        {
            string currency = string.Empty;
            double availBalance, currentBalance, creditLimit;
            availBalance = currentBalance = creditLimit = 0;
            internalGetAccounFounds(out availBalance, out currentBalance, out creditLimit, out currency);
            return currency;
        }

        public string getExchangeName()
        {
            return "Pinnacle";
        }

        public SXALMarketLite getMarketInfo(long marketId)
        {
            throw new NotImplementedException();
        }

        public SXALMarketPrices getMarketPrices(long marketId, bool canThrowThrottleExceeded)
        {
            throw new NotImplementedException();
        }

        public SXALSelectionIdEnum getReverseSelectionId(long selectionId, SXALMarket m)
        {
            throw new NotImplementedException();
        }

        public long getSelectionId(SXALSelectionIdEnum selectionToGet, SXALMarket m)
        {
            throw new NotImplementedException();
        }

        public long getSelectionIdByName(string name, SXALMarket m)
        {
            throw new NotImplementedException();
        }

        public SXALSelection[] getSelections(SXALMarket market)
        {
            throw new NotImplementedException();
        }

        public decimal getValidOddIncrement(decimal odds)
        {
            throw new NotImplementedException();
        }

        public SXALEventType[] loadEvents()
        {
            return internalLoadEvents();
        }

        public bool login(string usr, string pwd)
        {
            _usr = usr;
            _pwd = pwd;
            return login();
        }

        public SXALBet placeBackBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, bool isInplay)
        {
            throw new NotImplementedException();
        }

        public SXALBet placeLayBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, bool isInplay)
        {
            throw new NotImplementedException();
        }

        public SXALMarket translateHorseMarket(SXALMarket market)
        {
            throw new NotImplementedException();
        }

        public decimal validateOdd(decimal odds)
        {
            throw new NotImplementedException();
        }
    }
}
