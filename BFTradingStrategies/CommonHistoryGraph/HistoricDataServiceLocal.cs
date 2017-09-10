using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.common
{
    public class HistoricDataServiceLocal : IHistoricDataService
    {
        #region IHistoricDataService Member

        public HistoricDataStatistic GetStatistic(ulong teamAId, ulong teamBId, string teamA, string teamB, string league)
        {
            return HistoricDataSerializer.Load(teamAId, teamBId);
        }

        #endregion
    }
}
