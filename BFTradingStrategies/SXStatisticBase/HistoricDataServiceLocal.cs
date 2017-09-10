using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase
{
    public class HistoricDataServiceLocal : IHistoricDataService
    {
        #region IHistoricDataService Member

        public async Task<HistoricDataStatistic> GetStatistic(ulong teamAId, ulong teamBId, string teamA, string teamB, string league, int noOfData, int ageOfData)
        {
            return HistoricDataSerializer.Load(teamAId, teamBId);
        }

        #endregion
    }
}
