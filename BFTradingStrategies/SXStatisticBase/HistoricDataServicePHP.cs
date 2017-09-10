using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Collections;
using System.Windows.Forms;
using System.Threading.Tasks;
//using net.sxtrader.bftradingstrategies.common.Configurations;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase
{
    class HistoricDataServicePHP : IHistoricDataService
    {
        #region IHistoricDataService Member

        public async Task<HistoricDataStatistic> GetStatistic(ulong teamAId, ulong teamBId, string teamA, string teamB, string league, int noOfData, int ageOfData)
        {

            try
            {
                //SAConfigurationRW config = new SAConfigurationRW(); 
                HistoricDataStatistic clientStatistic = new HistoricDataStatistic();
                clientStatistic.TeamAId = teamAId;
                clientStatistic.TeamBId = teamBId;
                clientStatistic.TeamAName = teamA;
                clientStatistic.TeamBName = teamB;
                String url = String.Empty;

                
                StringBuilder sbAgeOfData = new StringBuilder();
                if (ageOfData == 0)
                {
                    sbAgeOfData.Append("0");
                }
                else
                {
                    DateTime dtsAge = DateTime.Now.AddMonths(ageOfData * (-1));
                    sbAgeOfData.AppendFormat("{0}-{1}-{2}", dtsAge.Year, dtsAge.Month, dtsAge.Day);
                }

                if (league == string.Empty)
                {
                    url = String.Format("http://www.sxtrader.net/LSHistoryDB/getStatistics.php?teamAId={0}&teamBId={1}&count={2}&age={3}", teamAId, teamBId, noOfData,sbAgeOfData.ToString());
                }
                else
                {
                    url = String.Format("http://www.sxtrader.net/LSHistoryDB/getStatistics.php?teamAId={0}&teamBId={1}&league={2}&count={3}&age={4}", teamAId, teamBId,league, noOfData,sbAgeOfData.ToString());
                }
                String response = await  SXTools.doWebRequest(url);

                Console.WriteLine("HistoricDataServicePHP.DoWebRequest beendet");

                response = response.Trim();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);


                XmlNodeList directList = doc.GetElementsByTagName("directMatches");
                HistoricMatchList direct = buildHistoricList(directList[0]);

                XmlNodeList teamAList = doc.GetElementsByTagName("teamAMatches");
                HistoricMatchList teamAMatchList = buildHistoricList(teamAList[0]);

                XmlNodeList teamBList = doc.GetElementsByTagName("teamBMatches");
                HistoricMatchList teamBMatchList = buildHistoricList(teamBList[0]);

                clientStatistic.Direct = direct;
                clientStatistic.TeamA = teamAMatchList;
                clientStatistic.TeamB = teamBMatchList;
                // Serialize
                HistoricDataSerializer.Save(clientStatistic, teamAId, teamBId);
                return clientStatistic;                               
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteException(e);
            }
            return null;
        }

        #endregion

        private LSHistoricMatchEvent[] buildMatchEventList(XmlNode eventsNode)
        {
            ArrayList events = new ArrayList();
            try
            {
                if (eventsNode != null)
                {
                    foreach (XmlNode node in eventsNode.ChildNodes)
                    {
                        LSHistoricMatchEvent matchEvent = new LSHistoricMatchEvent();
                        matchEvent.MatchId = UInt64.Parse(node.Attributes["matchId"].Value);
                        matchEvent.TeamId = UInt64.Parse(node.Attributes["teamId"].Value);
                        matchEvent.EventType = (MATCHEVENTTYPE)UInt16.Parse(node.Attributes["eventType"].Value);
                        matchEvent.InfoEvent1 = node.Attributes["infoEvent1"].Value;
                        matchEvent.InfoEvent2 = node.Attributes["infoEvent2"].Value;
                        matchEvent.EventMinute = Int16.Parse(node.Attributes["eventMinute"].Value);
                        events.Add(matchEvent);
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteException(e);
            }
            return (LSHistoricMatchEvent[])events.ToArray(typeof(LSHistoricMatchEvent));
        }

        private int getFirstGoalTime(LSHistoricMatchEvent[] events)
        {
            int goalMinute = -1;
            foreach (LSHistoricMatchEvent matchEvent in events)
            {
                if (matchEvent.EventType == MATCHEVENTTYPE.GOAL ||
                    matchEvent.EventType == MATCHEVENTTYPE.PENALTY ||
                    matchEvent.EventType == MATCHEVENTTYPE.OWNGOAL)
                {
                    goalMinute = matchEvent.EventMinute;
                    break;
                }
            }
            return goalMinute;
        }

        private HistoricMatchList buildHistoricList(XmlNode matchesNode)
        {
            try
            {
                if (matchesNode != null)
                {
                    HistoricMatchList direct = new HistoricMatchList();
                    foreach (XmlNode node in matchesNode.ChildNodes)
                    {
                        char[] split = { '-' };


                        LSHistoricMatch match = new LSHistoricMatch();
                        match.MatchId = UInt64.Parse(node.Attributes["matchId"].Value);
                        try
                        {
                            String[] dateElements = node.Attributes["matchDate"].Value.Split(split);
                            match.MatchDate = new DateTime(Int16.Parse(dateElements[0]), Int16.Parse(dateElements[1]), Int16.Parse(dateElements[2]));
                        }
                        catch (Exception e)
                        {
                            ExceptionWriter.Instance.WriteException(e);                            
                            match.MatchDate = DateTime.MinValue;
                        }
                        match.TeamAId = UInt64.Parse(node.Attributes["teamAId"].Value);
                        match.TeamA = node.Attributes["teamA"].Value;
                        match.TeamBId = UInt64.Parse(node.Attributes["teamBId"].Value);
                        match.TeamB = node.Attributes["teamB"].Value;
                        match.Devision = node.Attributes["devision"].Value;
                        match.ScoreA = UInt32.Parse(node.Attributes["scoreA"].Value);
                        match.ScoreB = UInt32.Parse(node.Attributes["scoreB"].Value);
                        match.HalftimeScore = node.Attributes["htScore"].Value;
                        
                        XmlNode eventsNode = node.FirstChild;
                        match.Events = buildMatchEventList(eventsNode);
                        match.FirstGoalMinute = getFirstGoalTime(match.Events);
                        direct.Add(match);
                    }
                    
                    return direct;
                }
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteException(e);
            }

            return null;
        }

        /*
        private static async Task<String> doWebRequest(String url)
        {
            
            String strResponse = String.Empty;
            // Prepare web request...
            System.Net.HttpWebRequest myRequest =
                  (System.Net.HttpWebRequest)WebRequest.Create(url);

            myRequest.Timeout = 60000;

            myRequest.UserAgent = String.Format("SXTrader {0}", Application.ProductVersion);
           
            // Assign the response object of 'HttpWebRequest' to a 'HttpWebResponse' variable.

            using (WebResponse myHttpWebResponse = await myRequest.GetResponseAsync())
            {

                // Display the contents of the page to the console.
                Stream streamResponse = myHttpWebResponse.GetResponseStream();

                // Get stream object
                StreamReader streamRead = new StreamReader(streamResponse);

                //Char[] readBuffer = new Char[8192];



                // Read from buffer
                strResponse = streamRead.ReadToEnd();

                // Release the response object resources.
                streamRead.Close();
                streamResponse.Close();

                // Close response
                myHttpWebResponse.Close();
            }
            
            return strResponse;

            //return null;
        }
         * */
    }
}
