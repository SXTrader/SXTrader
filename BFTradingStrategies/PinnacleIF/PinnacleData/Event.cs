using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.pinnacleif.PinnacleData
{
    class Event
    {
        [JsonProperty(PropertyName = "sports")]
        public List<Sports> Sports { get; set; }
    }
}
