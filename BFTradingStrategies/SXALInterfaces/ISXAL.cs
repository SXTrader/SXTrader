using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    public interface ISXAL
    {
        double MinStake { get; }
        bool SupportsBelowMinStakeBetting { get; }

        bool login(string usr, string pwd);

        SXALBet placeLayBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, bool isInplay);
        SXALBet placeBackBet(long marketId, long selectionId, int asianId, bool keepBet, double price, double money, bool isInplay);
        bool cancelBet(long betId);

        SXALMarketPrices getMarketPrices(long marketId, bool canThrowThrottleExceeded);

        void getAccounFounds(out double availBalance, out double currentBalance, out double creditLimit);

        SXALBet getBetDetail(long betId);
        SXALMUBet[] getBetsMU(long marketId);
        SXALMUBet[] getBetMU(long betId);
        SXALMUBet[] getBets(DateTime dts);
        SXALMarketLite getMarketInfo(long marketId);
        SXALMarket[] getAllMarkets(int?[] eventids, DateTime fromDate, DateTime toDate);
        SXALSelection[] getSelections(SXALMarket market);
        String getCurrency();              

        SXALEventType[] loadEvents();

        String getExchangeName();
        long getSelectionId(SXALSelectionIdEnum selectionToGet, SXALMarket m);
        long getSelectionIdByName(String name, SXALMarket m);
        SXALSelectionIdEnum getReverseSelectionId(long selectionId, SXALMarket m);
        SXALMarket translateHorseMarket(SXALMarket market);

        //Berechnung der gültigen Quoten ist nun Wettbörsenabhängig
        decimal getValidOddIncrement(decimal odds);
        decimal validateOdd(decimal odds);
    }

    public class SXExcecutionFailedException : Exception
    {
        public SXExcecutionFailedException() : base() { }
        public SXExcecutionFailedException(string message) : base(message) { }
        public SXExcecutionFailedException(string message, Exception innerException) : base(message, innerException) { }
        public SXExcecutionFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
