using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.pinnacleif.PinnacleData
{
    class Odds
    {
        [JsonProperty(PropertyName ="sportId")]
        public int SportId { get; set; }
        [JsonProperty(PropertyName ="last")]
        public long Last { get; set; }
        [JsonProperty(PropertyName ="leagues")]
        public IList<OddsLeague> Leagues { get; set; }
    }

    class OddsLeague
    {
        [JsonProperty(PropertyName ="id")]
        public long LeagueId { get; set; }
        [JsonProperty(PropertyName ="events")]
        public IList<OddsEvent> Events { get; set; }
    }

    class OddsEvent
    {
        [JsonProperty(PropertyName ="id")]
        public long EventId { get; set; }
        [JsonProperty(PropertyName ="periods")]
        public IList<Period> Periods { get; set; }
        [JsonProperty(PropertyName ="awayScore", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int AwayScore { get; set; }
        [JsonProperty(PropertyName ="homeScore", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int HomeScore { get; set; }
        [JsonProperty(PropertyName ="awayRedCards", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int AwayRedCards { get; set; }
        [JsonProperty(PropertyName ="homeRedCards", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int HomeRedCards { get; set; }
    }

    class Period
    {
        [JsonProperty(PropertyName ="@lineId")]
        public long LineId { get; set; }
        [JsonProperty(PropertyName ="number")]
        public int Number { get; set; }
        [JsonProperty(PropertyName ="cutoff")]
        public DateTime CutOff { get; set; }
        [JsonProperty(PropertyName ="spreads", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<Spread> Spreads { get; set; }
        [JsonProperty(PropertyName ="totals", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<TeamPoints> Totals { get; set; }
        [JsonProperty(PropertyName ="moneyLine", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public MoneyLine MoneyLine { get; set; }
        [JsonProperty(PropertyName ="teamTotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TeamTotalPoints TeamTotal { get; set; }
        [JsonProperty(PropertyName ="maxSpread", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public decimal MaxSpread { get; set; }
        [JsonProperty(PropertyName ="maxTotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public decimal MaxTotal { get; set; }
        [JsonProperty(PropertyName ="maxMoneyLine", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public decimal MaxMoneyLine { get; set; }
        [JsonProperty(PropertyName ="maxTeamTotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public decimal MaxTeamTotal { get; set; }
    }

    class Spread
    {
        [JsonProperty(PropertyName ="altLineId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long AltLineId { get; set; }
        [JsonProperty(PropertyName ="hdp")]
        public decimal Hdp { get; set; }
        [JsonProperty(PropertyName ="away")]
        public decimal Away { get; set; }
        [JsonProperty(PropertyName ="home")]
        public decimal Home { get; set; }
    }

    class MoneyLine
    {
        [JsonProperty(PropertyName = "away")]
        public decimal Away { get; set; }
        [JsonProperty(PropertyName ="home")]
        public decimal Home { get; set; }
        [JsonProperty(PropertyName ="draw", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public decimal Draw { get; set; }
    }

    class TeamTotalPoints
    {
        [JsonProperty(PropertyName ="away", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TeamPoints Away { get; set; }
        [JsonProperty(PropertyName ="home", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TeamPoints Home { get; set; }
    }

    class TeamPoints
    {
        [JsonProperty(PropertyName ="altLineId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long AltLineId { get; set; }
        [JsonProperty(PropertyName ="points")]
        public decimal Points { get; set; }
        [JsonProperty(PropertyName ="over")]
        public decimal Over { get; set; }
        [JsonProperty(PropertyName ="under")]
        public decimal Under { get; set; }
    }
}
