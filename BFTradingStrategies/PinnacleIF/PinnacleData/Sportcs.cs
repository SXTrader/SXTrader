using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.pinnacleif.PinnacleData
{
    class Sports
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "hasOfferings")]
        public bool HasOfferings { get; set; }
        [JsonProperty(PropertyName = "leagueSpecialsCount")]
        public int LeagueSpecialCount { get; set; }
        [JsonProperty(PropertyName = "eventSpecialsCount")]
        public int EventSpecialsCount { get; set; }
        [JsonProperty(PropertyName = "eventCount")]
        public int EventCount { get; set; }
    }
}

