using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using MySql.Data.MySqlClient;
using System.Collections;
using System.Data;
using LSCommonObjects;
using System.ServiceModel;
using System.Net;
using System.IO;
using System.Xml;
using System.Threading;

namespace LSDataConnector
{
    public class LSDatabase
    {
        private const int MAXMESSAGESIZE = 1048576;
        //private MySqlConnection _conn;
        private LSDB.LSDB1SoapClient _client = null;
        private MatchReminder _matchReminder = null;
        public LSDatabase()
        {
           // _matchReminder = new MatchReminder();
            /*
            System.ServiceModel.Channels.Binding binding = new BasicHttpBinding(
                   BasicHttpSecurityMode.None);
                ((BasicHttpBinding)binding).MaxReceivedMessageSize = MAXMESSAGESIZE;
                //((BasicHttpBinding)binding).TransferMode = TransferMode.Buffered;

                //Binding binding = new WSHttpBinding(SecurityMode.None);
                //((WSHttpBinding)binding).Security = SecurityBindingElement.CreateAnonymousForCertificateBindingElement();
                EndpointAddress endpointAddress = new
         EndpointAddress("http://markusheid.vpscustomer.com/LSDB.asmx");
            _client = new LSDataConnector.LSDB.LSDB1SoapClient(binding,endpointAddress);
             */
            /*
            try
            {
                _conn = new MySqlConnection();
                String connectionString = "server=localhost;uid=LSDBUser;pwd=wohnheim;database=LSHistoryDB;";
                _conn.ConnectionString = connectionString;
                _conn.Open();
             }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            */
        }

        public void bulkInsertMatchData(LiveScoreMatchDataList theList)
        {
            //Masseneinfügen von Daten, falls noch nicht vorhanden
            foreach (LiveScoreMatchData matchData in theList.Values)
            {
                //existiert Team A?
                if (!TeamExists(matchData.TeamAId))
                {
                    InsertTeam(matchData.TeamAId);
                }

                // existiert Team A Name?
                SortedList<String, ulong> teamNameList = GetTeamNames(matchData.TeamAId);
                if (!teamNameList.Keys.Contains(matchData.TeamA))
                {
                    InsertTeamName(matchData.TeamAId, matchData.TeamA);
                }

                // existiert Team B?
                if (!TeamExists(matchData.TeamBId))
                {
                    InsertTeam(matchData.TeamBId);
                }

                // existiert Team B Name?
                teamNameList = GetTeamNames(matchData.TeamBId);
                if (!teamNameList.Keys.Contains(matchData.TeamB))
                {
                    InsertTeamName(matchData.TeamBId, matchData.TeamB);
                }

                // existiert Liga?
                if (!DevisionExist(matchData.Devision))
                {
                    InsertDevision(matchData.Devision, String.Empty);
                }

                // Existiert Begegnung?
                if (!MatchExist(matchData.MatchId))
                {
                    InsertMatch(matchData);

                    foreach (MatchEvent matchEvent in matchData.MatchEvents)
                    {
                        InsertMatchEvent(matchEvent);
                    }

                }
            }
        }

        public SortedList<String,ulong> GetTeamNames(ulong teamId)
        {

            while (true)
            {
                ArrayList result = new ArrayList();
                SortedList<String, ulong> resultList = new SortedList<string, ulong>();
                try
                {

                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/getTeamNames.php?teamId={0}", teamId);
                    String response = doWebRequest(url);
                    XmlDocument doc = new XmlDocument();
                    if (response == String.Empty)
                    {
                        Thread.Sleep(2000);
                        continue;
                    }
                    doc.LoadXml(response);
                    //XmlNode node = doc.GetElementById("teamNames");
                    XmlNodeList nodeList = doc.GetElementsByTagName("teamNames");
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        foreach (XmlNode childNode in nodeList[0].ChildNodes)
                        {
                            String teamName = childNode.Attributes["name"].Value;
                            result.Add(teamName);
                        }
                    }

                    foreach (String str in result)
                    {
                        resultList.Add(str, teamId);
                    }
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch!");
                    continue;
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
                return resultList;
            }
            /*
            SortedList<String, ulong> resultList = new SortedList<string, ulong>();

            String sql = "SELECT * FROM teamnames where idTeam=@teamId";

            MySqlCommand cmd = new MySqlCommand(sql, _conn);
            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@teamId";
            param.Value = teamId;
            cmd.Parameters.Add(param);
            MySqlDataReader rdr = cmd.ExecuteReader();


            while(rdr.Read())
            {
                ulong id = UInt32.Parse(rdr["idTeam"].ToString());
                String name = (String)rdr["teamName"];

                resultList.Add(name, id);
            }

            rdr.Close();
            return resultList;
             */
        }

        public void InsertMatchEvent(MatchEvent matchEvent)
        {
            while (true)
            {
                try
                {
                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/insertMatchEvent.php?teamId={0}&matchId={1}&eventType={2}&eventMinute={3}&infoEvent1={4}&infoEvent2={5}",
                        matchEvent.TeamId, matchEvent.MatchId, (int)matchEvent.EventType, matchEvent.EventMinute, matchEvent.InfoEvent1, matchEvent.InfoEvent2);
                    String response = doWebRequest(url);
                    if (response != String.Empty)
                        Console.WriteLine(response);
                    //_client.InsertMatchEvent(matchEvent.MatchId, matchEvent.TeamId, (int)matchEvent.EventType, (int)matchEvent.EventMinute, matchEvent.InfoEvent1, matchEvent.InfoEvent2);
                    return;
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch");
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
            }

            /*
            String sql = "INSERT INTO matchevent(matchId, teamId, typeEvent, eventMinute, infoEvent1, infoEvent2, version) " +
                "VALUES( @matchId, @teamId, @eventType, @eventMinute, @infoEvent1, @infoEvent2, @versionId)";


            MySqlCommand cmd = new MySqlCommand(sql, _conn);

            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@matchId";
            param.Value = matchEvent.MatchId;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@teamId";
            param.Value = matchEvent.TeamId;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@eventType";
            param.Value = (int)matchEvent.EventType;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@eventMinute";
            param.Value = (int)matchEvent.EventMinute;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@infoEvent1";
            param.Value = matchEvent.InfoEvent1;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@infoEvent2";
            param.Value = matchEvent.InfoEvent2;
            cmd.Parameters.Add(param);
            param = new MySqlParameter();
            param.ParameterName = "@versionId";
            param.Value = 1;
            cmd.Parameters.Add(param);

            int i = cmd.ExecuteNonQuery();
             * */
        }

        public void InsertMatch(LiveScoreMatchData matchData)
        {
            while(true)
            {
                try
                {
                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/insertMatch.php?matchId={0}&matchDate={1}&teamAId={2}&teamBId={3}&scoreTeamA={4}&scoreTeamB={5}&halftimeScore={6}&devisionId={7}",
                        matchData.MatchId, matchData.MatchDate, matchData.TeamAId, matchData.TeamBId, matchData.ScoreTeamA, matchData.ScoreTeamB, matchData.HalftimeScore, matchData.Devision);
                    String response = doWebRequest(url);
                    if (response != String.Empty)
                        Console.WriteLine(response);

                    if(response == String.Empty)
                        _matchReminder.insertMatchReminder(matchData.MatchId);
                    //_client.InsertMatch(matchData.MatchId, matchData.MatchDate, matchData.TeamAId, matchData.TeamBId, matchData.Devision, matchData.ScoreTeamA, matchData.ScoreTeamB, matchData.HalftimeScore);
                    return;
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch");
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
            }
            /*
            String sql = "INSERT INTO lshistorydb.match VALUES( @matchId, @matchDate, @teamAId, @teamBId, @devisionId, @scoreTeamA, @scoreTeamB, @htScore, @versionId)";

            MySqlCommand cmd = new MySqlCommand(sql, _conn);

            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@matchId";
            param.Value = matchData.MatchId;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@matchDate";
            param.Value = matchData.MatchDate;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@teamAId";
            param.Value = matchData.TeamAId;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@teamBId";
            param.Value = matchData.TeamBId;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@devisionId";
            param.Value = matchData.Devision;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@scoreTeamA";
            param.Value = matchData.ScoreTeamA;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@scoreTeamB";
            param.Value = matchData.ScoreTeamB;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@htScore";
            param.Value = matchData.HalftimeScore;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@versionId";
            param.Value = 1;
            cmd.Parameters.Add(param);

            int i = cmd.ExecuteNonQuery();
            */
        }

        public void InsertDevision(String devisionId, String devisionName)
        {
            while(true)
            {
                try
                {
                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/insertDevision.php?devisionId={0}&devisionName={1}",
                        devisionId, devisionName);
                    String response = doWebRequest(url);
                    //_client.InsertDevision(devisionId, devisionName);
                    return;
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch");
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
            }
            /*
            String sql = "INSERT INTO devision VALUES(@devisionid, @devisionName, @versionId)";

            MySqlCommand cmd = new MySqlCommand(sql, _conn);

            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@devisionId";
            param.Value = devisionId;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@devisionName";
            param.Value = devisionName;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@versionId";
            param.Value = 1;
            cmd.Parameters.Add(param);

            int i = cmd.ExecuteNonQuery();
             * */

        }

        public void InsertTeamName(ulong teamId, String teamName)
        {
            while(true)
            {
                try
                {
                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/insertTeamName.php?teamId={0}&teamName={1}",
                        teamId, teamName);
                    String response = doWebRequest(url);
                    if (response != String.Empty)
                        Console.WriteLine(response);
                    //_client.InsertTeamName(teamId, teamName);
                    return;
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch");
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
            }
            /*
            String sql = "INSERT INTO teamnames VALUES(@teamid, @teamName, @versionId)";

            MySqlCommand cmd = new MySqlCommand(sql, _conn);

            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@teamId";
            param.Value = teamId;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@teamName";
            param.Value = teamName;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@versionId";
            param.Value = 1;
            cmd.Parameters.Add(param);
            try
            {
                int i = cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                // nichts tun.
            }
             * */
        }

        public void InsertTeam(ulong teamId)
        {
            while(true)
            {
                try
                {
                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/insertTeam.php?teamId={0}",teamId);
                    String response = doWebRequest(url);
                    //_client.InsertTeam(teamId);
                    return;
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch");
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
            }
            /*
            String sql = "INSERT INTO team VALUES(@teamId, @versionId)";

            MySqlCommand cmd = new MySqlCommand(sql, _conn);

            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@teamId";
            param.Value = teamId;
            cmd.Parameters.Add(param);

            param = new MySqlParameter();
            param.ParameterName = "@versionId";
            param.Value = 1;
            cmd.Parameters.Add(param);

            int i = cmd.ExecuteNonQuery();
            */
        }

        private static String doWebRequest(String url)
        {
            String strResponse = String.Empty;
            try
            {
                // Prepare web request...
                System.Net.HttpWebRequest myRequest =
                      (System.Net.HttpWebRequest)WebRequest.Create(url);
                myRequest.UserAgent = "SXTrader Livescore Crawler";
                // Assign the response object of 'HttpWebRequest' to a 'HttpWebResponse' variable.
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myRequest.GetResponse();

                // Display the contents of the page to the console.
                Stream streamResponse = myHttpWebResponse.GetResponseStream();

                // Get stream object
                StreamReader streamRead = new StreamReader(streamResponse);

                Char[] readBuffer = new Char[8192];

                // Read from buffer
                int count = streamRead.Read(readBuffer, 0, 8192);

                while (count > 0)
                {
                    // get string
                    String resultData = new String(readBuffer, 0, count);

                    // Write the data
                    strResponse = strResponse + resultData;

                    // Read from buffer
                    count = streamRead.Read(readBuffer, 0, 8192);
                }
                // Release the response object resources.
                streamRead.Close();
                streamResponse.Close();

                // Close response
                myHttpWebResponse.Close();
            }
            catch (Exception ex)
            {

            }
            return strResponse;

            //return null;
        }

        public bool TeamExists(ulong teamId)
        {
            while(true)
            {
                try
                {
                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/teamExist.php?teamId={0}", teamId);
                    String response = doWebRequest(url);
                    return Boolean.Parse(response);
                    //return _client.TeamExists(teamId);
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch");
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
                catch (FormatException)
                {
                    Console.WriteLine(String.Format("There was an format exception for teamId {0}. retrying", teamId));
                    continue;
                }
            }
            /*
            string sql = "SELECT count(*) FROM team WHERE idTeam=@teamId";
            MySqlCommand cmd = new MySqlCommand(sql, _conn);


            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@teamId";
            param.Value = teamId;
            cmd.Parameters.Add(param);

            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            int i = rdr.GetInt16(0);
            rdr.Close();

            if (i > 0)
                return true;
            else
                return false;
             */
        }

        public bool MatchExist(ulong matchId)
        {
            if (_matchReminder.isMatchReminded(matchId))
                return true;

            while(true)
            {
                try
                {
                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/matchExist.php?matchId={0}", matchId);
                    String response = doWebRequest(url);
                    if (response == null || response == String.Empty)
                        continue;
                    if (Boolean.Parse(response) == true)
                    {
                        _matchReminder.insertMatchReminder(matchId);
                    }
                    return Boolean.Parse(response);
                    //return _client.MatchExist(matchId);
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch");
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
            }
            /*
            string sql = "SELECT count(*) from lshistorydb.match where idMatch=@matchId";
            MySqlCommand cmd = new MySqlCommand(sql, _conn);

            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@matchId";
            param.Value = matchId;
            cmd.Parameters.Add(param);
            MySqlDataReader rdr = cmd.ExecuteReader();            
            rdr.Read();
            int i = rdr.GetInt16(0);
            rdr.Close();
            if(i>0)
                return true;
            else
                return false;
             */
        }

        public bool DevisionExist(String devisionId)
        {
            while(true)
            {
                try
                {
                    String url = String.Format("http://www.sxtrader.net/LSAdminIF/devisionExist.php?devisionId={0}", devisionId);
                    String response = doWebRequest(url);
                    return Boolean.Parse(response);
                    //return _client.DevisionExist(devisionId);
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Eine WebException! Erneuter Versuch");
                }
                catch (TimeoutException tex)
                {
                    Console.WriteLine("Eine TimeoutException! Erneuter Versuch!");
                    continue;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Eine FormatException! Erneuter Versuch!");
                    continue;
                }
            }
            /*
            string sql = "SELECT count(*) FROM devision WHERE idDevision=@devisionId";
            MySqlCommand cmd = new MySqlCommand(sql, _conn);

            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@devisionId";
            param.Value = devisionId;
            cmd.Parameters.Add(param);
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            int i = rdr.GetInt16(0);
            rdr.Close();

            if (i > 0)
                return true;
            else
                return false;
             * */
        }
    }
}
