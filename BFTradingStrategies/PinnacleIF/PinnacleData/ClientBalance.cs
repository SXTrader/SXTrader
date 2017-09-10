using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.pinnacleif.pinnacledata
{
    class ClientBalance
    {
        [JsonProperty(PropertyName = "availableBalance")]
        public decimal AvailableBalance { get; set; }

        [JsonProperty(PropertyName = "outstandingTransactions")]
        public decimal OutstandingTransactions { get; set; }

        [JsonProperty(PropertyName = "givenCredit")]
        public decimal GivenCredit { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
    }
}
