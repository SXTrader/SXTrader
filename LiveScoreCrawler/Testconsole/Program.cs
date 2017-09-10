using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSCConnector;
using LSParser;
using LSCommonObjects;
using LSDataConnector;
using System.Threading;

namespace Testconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                LSDatabase lsdb = new LSDatabase();
                int counter = 0;
                //lsdb.InsertTeam(1);

                LSReader reader = new LSReader();
                String data = reader.readData();
                LSParser.LSParser parser = new LSParser.LSParser(lsdb);

                LiveScoreMatchDataList currentGamesList = parser.parseInitialData(data);
                LiveScoreMatchDataList historicGamesList = new LiveScoreMatchDataList();
                Console.WriteLine(String.Format("Anzahl der aktuellen Begegnungen {0}", currentGamesList.Count));
                foreach (LiveScoreMatchData matchData in currentGamesList.Values)
                {
                    Console.WriteLine(String.Format("\tIteration {5} of {6}\r\n\tMatch {0}\r\n\t\t Team A {1} Id {2}\r\n\t\t Team B {3} Id {4}", matchData.MatchId, matchData.TeamA, matchData.TeamAId, matchData.TeamB, matchData.TeamBId, ++counter, currentGamesList.Count));

                    String directEncounterData = reader.getLastDirectEncounter(matchData.MatchId);
                    LiveScoreMatchDataList tmpList = parser.parseDirectEncounterData(directEncounterData);
                    LiveScoreMatchDataList anotherList = new LiveScoreMatchDataList();
                    foreach (KeyValuePair<ulong, LiveScoreMatchData> keyValue in tmpList)
                    {
                        if (!historicGamesList.ContainsKey(keyValue.Key))
                        {
                            historicGamesList.Add(keyValue.Key, keyValue.Value);
                            anotherList.Add(keyValue.Key, keyValue.Value);
                        }
                    }
                    String latestEncounterTeamA = reader.getLatestEncounterForTeam(matchData.TeamAId);
                    tmpList = parser.parseHistoricEncounterData(latestEncounterTeamA);
                    foreach (KeyValuePair<ulong, LiveScoreMatchData> keyValue in tmpList)
                    {
                        if (!historicGamesList.ContainsKey(keyValue.Key))
                        {
                            historicGamesList.Add(keyValue.Key, keyValue.Value);
                            anotherList.Add(keyValue.Key, keyValue.Value);
                        }
                    }

                    String latestEncounterTeamB = reader.getLatestEncounterForTeam(matchData.TeamBId);
                    tmpList = parser.parseHistoricEncounterData(latestEncounterTeamB);
                    foreach (KeyValuePair<ulong, LiveScoreMatchData> keyValue in tmpList)
                    {
                        if (!historicGamesList.ContainsKey(keyValue.Key))
                        {
                            historicGamesList.Add(keyValue.Key, keyValue.Value);
                            anotherList.Add(keyValue.Key, keyValue.Value);
                        }
                    }


                    lsdb.bulkInsertMatchData(anotherList);
                    Console.WriteLine(String.Format("Historische Daten gelesen. Anzahl insgesamt: {0}\r\n\r\n", historicGamesList.Count));
                }

                //Console.ReadLine();
                Thread.Sleep(new TimeSpan(6, 0, 0));
            }
        }
    }
}
