using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Timers;
using System.Text;

namespace LiveScoreCrawlerGen2
{
    class CurrentGamesLoadedEventArgs : EventArgs
    {
        public List<LiveScoreData> LiveScores { get; private set; }

        public CurrentGamesLoadedEventArgs(List<LiveScoreData> liveScores)
        {
            LiveScores = liveScores;
        }
    }

    class MatchDataLoadedEventArgs : EventArgs
    {
        public List<LiveScoreMatchData> MatchData { get; private set; }

        public MatchDataLoadedEventArgs(List<LiveScoreMatchData> matchData)
        {
            MatchData = matchData;
        }
    }
    class StatisticCrawler
    {
        CancellationTokenSource _wtoken;
        private ConcurrentDictionary<long, LiveScoreMatchData> pureMatchesDictionary;

        private const string WLIDTAG = ";WLID";
        private const string SDTTAG = "sDt[";
        private const string SDTENDTAG = "]=";
        private const string HISTORYTAG = "var gamehistory =";
        private const string HISTORYTEAMTAG = "var gameTeamHistory =";
        private const string VARORDTAG = "var ORD =";

        private delegate void CurrentGamesLoadedHandler(object sender, CurrentGamesLoadedEventArgs e);
        private delegate void MatchDataLoadedHandler(object sender, MatchDataLoadedEventArgs e);

        private event CurrentGamesLoadedHandler CurrentGamesLoaded;
        private event MatchDataLoadedHandler MatchDataLoaded;

        public StatisticCrawler()
        {
            _wtoken = new CancellationTokenSource();
            pureMatchesDictionary = new ConcurrentDictionary<long, LiveScoreMatchData>();
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            CurrentGamesLoaded += StatisticCrawler_CurrentGamesLoaded;
            MatchDataLoaded += StatisticCrawler_MatchDataLoaded;
        }

        private void StatisticCrawler_MatchDataLoaded(object sender, MatchDataLoadedEventArgs e)
        {

            //  Task.Run(new Action(() =>
            //{

            List<long> idList = new List<long>();

            foreach (LiveScoreMatchData m in e.MatchData)
              {
                  if(!pureMatchesDictionary.ContainsKey(m.MatchId))
                  {
                      pureMatchesDictionary.TryAdd(m.MatchId, m);
                  }
              }
            //}));

            //Test PHP
            idList.AddRange(pureMatchesDictionary.Keys);

            var json = JsonConvert.SerializeObject(idList);
            json = "matchIdList=" + json;
            byte[] byteArray = Encoding.ASCII.GetBytes(json);



            WebRequest request = WebRequest.Create("http://www.sxtrader.net/LSAdminIF/matchExistBulk.php");
            request.Method = "POST";
            request.ContentLength = byteArray.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();
            if (stream == null)
            {
                //Fehler!
                return;
            }

            StreamReader reader = new StreamReader(stream);

            string responseText = reader.ReadToEnd();
            stream.Close();
            reader.Close();

            var unknownMatchIds = JsonConvert.DeserializeObject<List<long>>(responseText);

            const String GOALDATAURL = "http://data.7m.cn/goaldata/en/{0}.js";

            foreach(long id in unknownMatchIds)
            {
                request = WebRequest.Create(/*"http://data.7m.cn/goaldata/en/1638813.shtml"*/ String.Format(GOALDATAURL, id)); 

                response = request.GetResponse() as HttpWebResponse;

                stream = response.GetResponseStream();
                if (stream == null)
                {
                    //Fehler!
                    return;
                }

                reader = new StreamReader(stream);

                responseText = reader.ReadToEnd();
            }


    }

        private void StatisticCrawler_CurrentGamesLoaded(object sender, CurrentGamesLoadedEventArgs e)
        {

            Task.Run(new Action(() =>
            {
                loadHistoryData(e.LiveScores);
            }));
        }

        public void StartWork()
        {
#pragma warning disable CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist
            PeriodicMarketCheckAsync(TimeSpan.Zero, new TimeSpan(24, 0, 0), _wtoken.Token);
#pragma warning restore CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine("Exception");
        }


        private async Task PeriodicMarketCheckAsync(TimeSpan dueTime,
                              TimeSpan interval,
                              CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {

                doWorkAsync(token);
                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }


        private void doWorkAsync(CancellationToken cancellationToken)
        {                                 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ctc.live.7m.cn/datafile/fen.js");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            pureMatchesDictionary.Clear();
            if(response.StatusCode != HttpStatusCode.OK)
            {
                //Fehler!
                return;
            }

            Stream stream = response.GetResponseStream();
            if(stream == null)
            {
                //Fehler!
                return;
            }

            StreamReader reader = new StreamReader(stream);

            string responseText = reader.ReadToEnd();

            reduceByTag(ref responseText, WLIDTAG);
            reduceByTag(ref responseText, VARORDTAG);
            

            Dictionary<long, String> unformattedData = new Dictionary<long, string>();

            unformattedData = buildUnformattedData(responseText);
            buildFormattedData(unformattedData);

            reader.Close();
            stream.Close();
            response.Close();
            
        }

        private void loadHistoryData(List<LiveScoreData> data)
        {

            List<LiveScoreMatchData> listLoadedMatches = new List<LiveScoreMatchData>(); ;


            const string URLDIRECTHISTORY = "http://analyse.7m.cn/{0}/data/gamehistory_en.js"; //Parameter ist die Begegnungsid
            const string URLTEAMHISTORY = "http://analyse.7m.cn/{0}/data/gameteamhistory_en.js";//Parameter ist die Begegnungsid
            
            foreach (var d in data)
            {
                listLoadedMatches.Clear();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(URLDIRECTHISTORY, d.bh));
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //Fehler!
                    return;
                }

                Stream stream = response.GetResponseStream();
                if (stream == null)
                {
                    //Fehler!
                    return;
                }

                StreamReader reader = new StreamReader(stream);

                string responseText = reader.ReadToEnd();

                responseText = responseText.Replace(HISTORYTAG, "");
                if(responseText.EndsWith(";"))
                {
                    responseText = responseText.Remove(responseText.Length - 1, 1);
                }

                var obj = JsonConvert.DeserializeObject<History<League>>(responseText);


                reader.Close();
                stream.Close();

                response.Close();



                request = (HttpWebRequest)WebRequest.Create(String.Format(URLTEAMHISTORY, d.bh));
                response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //Fehler!
                    return;
                }

                stream = response.GetResponseStream();
                if (stream == null)
                {
                    //Fehler!
                    return;
                }

                reader = new StreamReader(stream);

                responseText = reader.ReadToEnd();

                responseText = responseText.Replace(HISTORYTEAMTAG, "");
                if (responseText.EndsWith(";"))
                {
                    responseText = responseText.Remove(responseText.Length - 1, 1);
                }

                var obj2 = JsonConvert.DeserializeObject<TeamHistory<League>>(responseText);

                reader.Close();
                stream.Close();

                response.Close();

                //Livescoredaten aufbauen
                if(obj.HistoryMatch != null)
                {
                    for(int i = 0; i < obj.HistoryMatch.MatchId.Count; i++)
                    {
                        long teamAId = obj.HistoryMatch.TeamAId[i];
                        long teamBId = obj.HistoryMatch.TeamBId[i];
                        long matchId = obj.HistoryMatch.MatchId[i];
                        long leagueId = obj.HistoryMatch.LeagueId[i];
                        int teamAScore = obj.HistoryMatch.TeamAScore[i];
                        int teamBScore = obj.HistoryMatch.TeamBScore[i];
                        string htScore = obj.HistoryMatch.HalfTimeScore[i];
                        string league = obj.League.Where(x => x.Key == leagueId.ToString()).FirstOrDefault().Value.Name;
                        string teamAName = obj.Team.Where(x => x.Key == teamAId.ToString()).FirstOrDefault().Value;
                        string teamBName = obj.Team.Where(x => x.Key == teamBId.ToString()).FirstOrDefault().Value;
                        string matchDate = obj.HistoryMatch.MatchDate[i];
                        //string league = obj.League.Values.W
                        
                        LiveScoreMatchData matchData = new LiveScoreMatchData(teamAName, teamAId, teamBName, teamBId, league, matchId, matchDate);
                        matchData.ScoreTeamA = teamAScore;
                        matchData.ScoreTeamB = teamBScore;
                        matchData.HalftimeScore = htScore;
                        listLoadedMatches.Add(matchData);
                        
                    }
                }

                if(obj2.A != null)
                {
                    for(int i = 0; i <obj2.A.All.History.MatchId.Count; i++)
                    {

                        long teamAId = obj2.A.All.History.TeamAId[i];
                        long teamBId = obj2.A.All.History.TeamBId[i];
                        long matchId = obj2.A.All.History.MatchId[i];
                        long leagueId = obj2.A.All.History.LeagueId[i];
                        int teamAScore = obj2.A.All.History.TeamAScore[i];
                        int teamBScore = obj2.A.All.History.TeamBScore[i];
                        string htScore = obj2.A.All.History.HalfTimeScore[i];
                        string league = obj2.League.Where(x => x.Key == leagueId.ToString()).FirstOrDefault().Value.Name;
                        string teamAName = obj2.Team.Where(x => x.Key == teamAId.ToString()).FirstOrDefault().Value;
                        string teamBName = obj2.Team.Where(x => x.Key == teamBId.ToString()).FirstOrDefault().Value;
                        string matchDate = obj2.A.All.History.MatchDate[i];
                        //string league = obj.League.Values.W

                        LiveScoreMatchData matchData = new LiveScoreMatchData(teamAName, teamAId, teamBName, teamBId, league, matchId, matchDate);
                        matchData.ScoreTeamA = teamAScore;
                        matchData.ScoreTeamB = teamBScore;
                        matchData.HalftimeScore = htScore;
                        listLoadedMatches.Add(matchData);
                    }
                }

                if (obj2.B != null)
                {
                    for (int i = 0; i < obj2.B.All.History.MatchId.Count; i++)
                    {

                        long teamAId = obj2.B.All.History.TeamAId[i];
                        long teamBId = obj2.B.All.History.TeamBId[i];
                        long matchId = obj2.B.All.History.MatchId[i];
                        long leagueId = obj2.B.All.History.LeagueId[i];
                        int teamAScore = obj2.B.All.History.TeamAScore[i];
                        int teamBScore = obj2.B.All.History.TeamBScore[i];
                        string htScore = obj2.B.All.History.HalfTimeScore[i];
                        string league = obj2.League.Where(x => x.Key == leagueId.ToString()).FirstOrDefault().Value.Name;
                        string teamAName = obj2.Team.Where(x => x.Key == teamAId.ToString()).FirstOrDefault().Value;
                        string teamBName = obj2.Team.Where(x => x.Key == teamBId.ToString()).FirstOrDefault().Value;
                        string matchDate = obj2.B.All.History.MatchDate[i];
                        //string league = obj.League.Values.W

                        LiveScoreMatchData matchData = new LiveScoreMatchData(teamAName, teamAId, teamBName, teamBId, league, matchId, matchDate);
                        matchData.ScoreTeamA = teamAScore;
                        matchData.ScoreTeamB = teamBScore;
                        matchData.HalftimeScore = htScore;
                        listLoadedMatches.Add(matchData);
                    }
                }

                MatchDataLoaded?.Invoke(this, new MatchDataLoadedEventArgs(listLoadedMatches));

                Thread.Sleep(5000);
            }
        }

        private void buildFormattedData(Dictionary<long,String>dictionary)
        {
            List<LiveScoreData> liveScoreDatas = new List<LiveScoreData>();
            foreach(var v in dictionary)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<List<String>>(v.Value);

                    LiveScoreData lsData = new LiveScoreData(v.Key, obj);
                    liveScoreDatas.Add(lsData);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            CurrentGamesLoaded?.Invoke(this, new CurrentGamesLoadedEventArgs(liveScoreDatas));
            
            return;
        }

        private Dictionary<long,String> buildUnformattedData(string content)
        {
            //http://ctc.live.7m.cn/datafile/sxl.js?nocache=1476639759"
            //view-source:http://live2.7msport.com/pk_live_f.aspx?encode=en&view=all&match=&ordtype=
            // Wr(i, sDt[i][0], sDt[i][1], sDt[i][2], sDt[i][3], sDt[i][4], sDt[i][5], sDt[i][6], sDt[i][7], sDt[i][8], sDt[i][9], sDt[i][10], sDt[i][11], 
            //sDt[i][12], sDt[i][13], sDt[i][14], sDt[i][15], sDt2[i][0], sDt2[i][1], sDt2[i][2], sDt2[i][3], sDt2[i][4], sDt2[i][5], sDt2[i][6], 
            //sDt2[i][7], sDt2[i][8], sDt2[i][9], sDt2[i][10], sDt2[i][11], sDt2[i][12], sDt2[i][13], sDt[i][17], sDt[i][18]);

            //Wr(bh, gl, cr, live_a, live_b, memo, weather, wd, pm1, pm2, abh, bbh, isDisplay, glbh, bir, r_bh, r_bir, isstart, la, lb, ra, rb, difftime, bc, resume, sj, sx, rq, sw1, sw2, sy, euro1x2, pk2)


            int startIndex = content.IndexOf(SDTTAG);
            Dictionary<long, string> dictionary = new Dictionary<long, string>();
            while(startIndex  != -1)
            {
                string str = String.Empty;
                int endIndex = content.IndexOf(SDTTAG, startIndex + 4);
                if (endIndex == -1)
                {
                    str = content.Substring(startIndex);                    
                    startIndex = endIndex;
                }
                else
                {
                    str = content.Substring(startIndex, endIndex - startIndex);                   
                    startIndex = endIndex;                    
                }

                //remove the Tag
                str = str.Trim().Remove(0, SDTTAG.Length);
                int startIndexOfEndTag = str.IndexOf(SDTENDTAG);
                if (startIndexOfEndTag == -1)
                    continue;

                long id = 0;
                if(Int64.TryParse(str.Substring(0, startIndexOfEndTag), out id))
                {
                    string unformattedData = str.Substring(startIndexOfEndTag + SDTENDTAG.Length);
                    unformattedData = unformattedData.TrimEnd(new char[] { ';' });
                    dictionary.Add(id, unformattedData);
                }

            }

            return dictionary;
        }

        private void reduceByTag(ref string content, string tag)
        {
            int firstIndex = content.IndexOf(tag);
            if(firstIndex > -1)
            {
                content = content.Substring(0, firstIndex);
            }
        }

    }

    class LiveScoreData
    {
        public LiveScoreData(long matchId, List<string> dataList)
        {
            bh = matchId;
            gl = dataList[0];
            cr = dataList[1];
            live_a = dataList[2];
            live_b = dataList[3];
            memo = dataList[4];
            weather = dataList[5];
            wd = dataList[6];
            pm1 = dataList[7];
            pm2 = dataList[8];
            abh = dataList[9];
            bbh = dataList[10];
            isDisplay = dataList[11];
            glbh = dataList[12];
            bir = dataList[13];
            r_bh = dataList[14];
            x1 = dataList[15];
            x2 = dataList[16];
            x3 = dataList[17];
            x4 = dataList[18];
            x5 = dataList[19];
            x6 = dataList[20];
        }

       [JsonIgnore]
        public long bh { get; set; }
        
        [JsonProperty(Order=0, NullValueHandling = NullValueHandling.Ignore)  ]        
        public String gl { get; set; }
        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public String cr { get; set; }
        [JsonProperty(Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public String live_a { get; set; }
        [JsonProperty(Order = 3, NullValueHandling = NullValueHandling.Ignore)]
        public String live_b { get; set; }
        [JsonProperty(Order = 4, NullValueHandling = NullValueHandling.Ignore)]
        public String memo { get; set; }
        [JsonProperty(Order = 5, NullValueHandling = NullValueHandling.Ignore)]
        public String weather { get; set; }
        [JsonProperty(Order = 6, NullValueHandling = NullValueHandling.Ignore)]
        public String wd { get; set; }
        [JsonProperty(Order = 7, NullValueHandling = NullValueHandling.Ignore)]
        public String pm1 { get; set; }
        [JsonProperty(Order = 8, NullValueHandling = NullValueHandling.Ignore)]
        public String pm2 { get; set; }
        [JsonProperty(Order = 9, NullValueHandling = NullValueHandling.Ignore)]
        public String abh { get; set; }
        [JsonProperty(Order = 10, NullValueHandling = NullValueHandling.Ignore)]
        public String bbh { get; set; }
        [JsonProperty(Order = 11, NullValueHandling = NullValueHandling.Ignore)]
        public String isDisplay { get; set; }
        [JsonProperty(Order = 12, NullValueHandling = NullValueHandling.Ignore)]
        public String glbh { get; set; }
        [JsonProperty(Order = 13, NullValueHandling = NullValueHandling.Ignore)]
        public String bir { get; set; }
        [JsonProperty(Order = 14, NullValueHandling = NullValueHandling.Ignore)]
        public String r_bh { get; set; } //r_bir, isstart, la, lb, ra, rb, difftime, bc, resume, sj, sx, rq, sw1, sw2, sy, euro1x2, pk2)

        [JsonProperty(Order = 15, NullValueHandling = NullValueHandling.Ignore)]
        public String x1 { get; set; }

        [JsonProperty(Order = 16, NullValueHandling = NullValueHandling.Ignore)]
        public String x2 { get; set; }

        [JsonProperty(Order = 17, NullValueHandling = NullValueHandling.Ignore)]
        public String x3 { get; set; }

        [JsonProperty(Order = 18, NullValueHandling = NullValueHandling.Ignore)]
        public String x4 { get; set; }

        [JsonProperty(Order = 19, NullValueHandling = NullValueHandling.Ignore)]
        public String x5 { get; set; }

        [JsonProperty(Order = 20, NullValueHandling = NullValueHandling.Ignore)]
        public String x6 { get; set; }
        
    }
}


class TeamHistory<T> where T : new()
{
    [JsonProperty("A")]
    public Team<T> A { get; set; }
    [JsonProperty("B")]
    public Team<T> B { get; set; }

    [JsonProperty("team")]
    public Dictionary<string, string> Team { get; set; }
    [JsonProperty("note")]
    public Dictionary<string, string> Note { get; set; }

    [JsonProperty("match")]
    public Dictionary<string, League> League { get; set; }
}

class History<T> where T : new()
{
    public HistoryMatch<T> HistoryMatch { get; set; }
    [JsonProperty("match")]
    public Dictionary<string, T> League { get; set; }
    [JsonProperty("team")]
    public Dictionary<string, string> Team { get; set; }
    [JsonProperty("note")]
    public Note Note { get; set; }
}

class Team<T> where T: new()
{
    [JsonProperty("all")]
    public TeamHistoryMatch<T> All { get; set; }

    [JsonProperty("home", NullValueHandling = NullValueHandling.Ignore)]
    public TeamHistoryMatch<T> Home { get; set; }

    [JsonProperty("away", NullValueHandling = NullValueHandling.Ignore)]
    public TeamHistoryMatch<T> Away { get; set; }
}

class TeamHistoryMatch<T> where T: new()
{
    [JsonProperty("history")]
    public HistoryMatch<T> History { get; set; }
    [JsonProperty("match")]
    public IList<long> Note { get; set; }
}

class HistoryMatch<T> where T: new()
{
    [JsonProperty("id")]
    public IList<long> MatchId { get; set; }
    [JsonProperty("mid")]
    public IList<long> LeagueId { get; set; }
    [JsonProperty("aid")]
    public IList<long> TeamAId { get; set; }
    [JsonProperty("bid")]
    public IList<long> TeamBId { get; set; }
    [JsonProperty("date")]
    public IList<string> MatchDate { get; set; }
    [JsonProperty("liveA")]
    public IList<int> TeamAScore { get; set; }
    [JsonProperty("liveB")]
    public IList<int> TeamBScore { get; set; }
    [JsonProperty("redA")]
    public IList<long> TeamARedCard { get; set; }
    [JsonProperty("redB")]
    public IList<long> TeamBRedCard { get; set; }
    [JsonProperty("bc")]
    public IList<string> HalfTimeScore { get; set; }
    [JsonProperty("ng")]
    public IList<long> Ng { get; set; }
    [JsonProperty("rq")]
    public IList<string> Rq { get; set; }
    [JsonProperty("rql")]
    public IList<long> Rql { get; set; }
    [JsonProperty("worl")]
    public IList<long> Worl { get; set; }
    [JsonProperty("oworl")]
    public List<long> Oworl { get; set; }
}

class League
{
    [JsonProperty("n")]
    public string Name { get; set; }
    [JsonProperty("c")]
    public string Color { get; set; }
}

class Team
{
    public Dictionary<string, string> Values { get; set; }
}

class Note
{
    public Dictionary<string, string> Values { get; set; }
}


/*
 * {
"historymatch":
{
 "id":[1686313,1446466],
 "mid":[1365,166],
 "aid":[24719,24699],
 "bid":[24699,24719],
 "date":["19/12/16","19/11/15"],
 "liveA":[0,1],
 "liveB":[0,0],
 "redA":[0,0],
 "redB":[0,0],
 "bc":["0-0","0-0"],
 "ng":[0,0],
 "rq":["1.25","-3.0"],
 "rql":[0,0],
 "worl":[1,0],
 "oworl":[3,3]},
 "match":
 {
  "1365":
  {
   "n":"IND U18",
   "c":"FFC144"
  },
  "166":
  {
   "n":"INT CF",
   "c":"5691D8"
  }
 },
 "team":
 {
  "24719":"Rangdajied United(U18)",
  "24699":"Royal Wahingdoh(U18)"
 },
"note":{}
}

 */
