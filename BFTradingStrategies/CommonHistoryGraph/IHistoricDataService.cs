using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxstatisticbase;

namespace net.sxtrader.bftradingstrategies.common
{
    public class NoHistoricDataException : Exception { }

    interface IHistoricDataService
    {
        HistoricDataStatistic GetStatistic(ulong teamAId, ulong teamBId, string teamA, string teamB, string league);
    }
}
