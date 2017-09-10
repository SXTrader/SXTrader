using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.pinnacleif.PinnacleData
{
    enum LIVE_STATUS { NO_LIVE = 0, LIVE = 1, GOING_LIVE=2}
    enum EVENT_STATUS
    {
        O,  // This is the starting status of a game. It means that the lines are open for betting.
        I,  // This status indicates that one or more lines have a red circle (a lower maximum bet amount).
        H   // This status indicates that the lines are temporarily unavailable for betting.
    }

    public enum PARLAYRESTRICTION
    {
        AVAILABLE  = 0,
        UNAVAILABLE = 1,
        ONLYONELEG = 2
    }

    class Fixtures
    {
        [JsonProperty(PropertyName = "sportId")]
        public int SportId {get;set;}
        [JsonProperty(PropertyName ="last")]
        public long Last { get; set; }
        [JsonProperty(PropertyName ="league")]
        public IList<FixtureLeague> League { get; set; }
    }

    class FixtureLeague
    {
        [JsonProperty(PropertyName ="id")]
        public long Id { get; set; }
        [JsonProperty(PropertyName ="events")]
        public IList<FixtureEvent> Events { get; set; }
    }

    class FixtureEvent
    {
        [JsonProperty(PropertyName ="id")]
        public long Id { get; set; }
        [JsonProperty(PropertyName ="starts")]
        public DateTime Starts { get; set; }
        [JsonProperty(PropertyName ="home")]
        public string Home { get; set; }
        [JsonProperty(PropertyName ="away")]
        public string Away { get; set; }
        [JsonProperty(PropertyName = "liveStatus", DefaultValueHandling = DefaultValueHandling.Ignore )]
        public LIVE_STATUS LiveSatus { get; set; }
        [JsonProperty(PropertyName = "status")]
        public EVENT_STATUS Status { get; set; }
        [JsonProperty(PropertyName ="parlayRestriction")]
        public PARLAYRESTRICTION ParlayRestriction { get; set; }
        [JsonProperty(PropertyName ="homePitcher", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HomePitcher { get; set; } 
        [JsonProperty(PropertyName ="awayPitcher", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AwayPitcher { get; set; }
    }
}
