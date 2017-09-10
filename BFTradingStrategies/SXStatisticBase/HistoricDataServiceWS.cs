using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using net.sxtrader.bftradingstrategies.common.SXTraderWS;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.Windows.Forms;
using System.Collections;
using System.Threading.Tasks;


namespace net.sxtrader.bftradingstrategies.sxstatisticbase
{
    public enum SCORES { ZEROZERO, ZEROONE, ZEROTWO, ZEROTHREE, ONEZERO, ONEONE, ONETWO, ONETHREE, 
                  TWOZERO, TWOONE, TWOTWO, TWOTHREE, THREEZERO, THREEONE, THREETWO, THREETHREE, OTHERS};

    class Score
    {
        private int _scoreA;
        private int _scoreB;

        public int ScoreA
        {
            get { return _scoreA; }
        }

        public int ScoreB
        {
            get { return _scoreB; }
        }

        public Score(int scoreA, int ScoreB)
        {
            _scoreA = scoreA;
            _scoreB = ScoreB;
        }

    }

    public class HistoricDataServiceWS : IHistoricDataService
    {

        private const int MAXMESSAGESIZE = 1048576;
        #region IHistoricDataService Member

        public async Task<HistoricDataStatistic> GetStatistic(ulong teamAId, ulong teamBId, string teamA, string teamB, string league, int noOfData, int ageOfData)
        {
            //TODO: Sobald neuer Statistikserver online, wieder umstellen
            return null;
            /*
            try
            {
                System.ServiceModel.Channels.Binding binding = new BasicHttpBinding(
                    BasicHttpSecurityMode.None);
                ((BasicHttpBinding)binding).MaxReceivedMessageSize = MAXMESSAGESIZE;
                //((BasicHttpBinding)binding).TransferMode = TransferMode.Buffered;

                //Binding binding = new WSHttpBinding(SecurityMode.None);
                //((WSHttpBinding)binding).Security = SecurityBindingElement.CreateAnonymousForCertificateBindingElement();
                EndpointAddress endpointAddress = new
       EndpointAddress("");

                Service1SoapClient wsClient = new Service1SoapClient(binding, endpointAddress);


                LSHistoricDataStatistic statistic = wsClient.GetStatistics(teamAId, teamBId);
                HistoricDataStatistic clientStatistic = new HistoricDataStatistic();
                HistoricMatchList direct = new HistoricMatchList();
                direct.AddRange(statistic.Direct);
                HistoricMatchList teamAMatchList = new HistoricMatchList();
                teamAMatchList.AddRange(statistic.TeamA);
                HistoricMatchList teamBMatchList = new HistoricMatchList();
                teamBMatchList.AddRange(statistic.TeamB);
                clientStatistic.Direct = direct;
                clientStatistic.TeamA = teamAMatchList;
                clientStatistic.TeamB = teamBMatchList;

                // Serialize
                HistoricDataSerializer.Save(clientStatistic, teamAId, teamBId);

                return clientStatistic;
            }
            catch (CommunicationException)
            {
                MessageBox.Show(HistoryGraph.strStatisticServerNA, HistoryGraph.strStatisticAnalyses, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
             * */
        }

        #endregion
            
    }

    [Serializable]
    public class HistoricDataStatistic
    {
        private HistoricMatchList _direct;
        private HistoricMatchList _teamA;
        private HistoricMatchList _teamB;

        public ulong TeamAId { get; set; }
        public ulong TeamBId { get; set; }
        public String TeamAName { get; set; }
        public String TeamBName { get; set; }

        
        public HistoricMatchList Direct { get { return _direct; } set { _direct = value; } }
        
        public HistoricMatchList TeamA { get { return _teamA; } set { _teamA = value; } }
        
        public HistoricMatchList TeamB { get { return _teamB; } set { _teamB = value; } }
    }

    [Serializable]
    public class HistoricMatchList : List<LSHistoricMatch> 
    {
        private static Dictionary<SCORES, Score> _staticScores;

        static HistoricMatchList()
        {
            _staticScores = new Dictionary<SCORES, Score>(16);
            _staticScores.Add(SCORES.ZEROZERO, new Score(0, 0));
            _staticScores.Add(SCORES.ZEROONE, new Score(0, 1));
            _staticScores.Add(SCORES.ZEROTWO, new Score(0, 2));
            _staticScores.Add(SCORES.ZEROTHREE, new Score(0, 3));
            _staticScores.Add(SCORES.ONEZERO, new Score(1, 0));
            _staticScores.Add(SCORES.ONEONE, new Score(1, 1));
            _staticScores.Add(SCORES.ONETWO, new Score(1, 2));
            _staticScores.Add(SCORES.ONETHREE, new Score(1, 3));
            _staticScores.Add(SCORES.TWOZERO, new Score(2, 0));
            _staticScores.Add(SCORES.TWOONE, new Score(2, 1));
            _staticScores.Add(SCORES.TWOTWO, new Score(2, 2));
            _staticScores.Add(SCORES.TWOTHREE, new Score(2, 3));
            _staticScores.Add(SCORES.THREEZERO, new Score(3, 0));
            _staticScores.Add(SCORES.THREEONE, new Score(3, 1));
            _staticScores.Add(SCORES.THREETWO, new Score(3, 2));
            _staticScores.Add(SCORES.THREETHREE, new Score(3, 3));           
        }

        public int NoOfData
        {
            get
            {
                return this.Count;
            }
        }

        public double ZeroZeroPercentage
        {
            get
            {
                if (this.Count == 0)
                    return 0.0;

                int iCounter = 0;
                foreach (LSHistoricMatch match in this)
                {
                    if (match.ScoreA == 0 && match.ScoreB == 0)
                        iCounter++;
                }

                double dResult = ((double)iCounter / this.Count) * 100;
                return Math.Round(dResult, 2);
            }
        }

        public int LatestFirstGoal
        {
            get
            {
                int latestGoalTime = -1;
                foreach (LSHistoricMatch match in this)
                {
                    foreach (LSHistoricMatchEvent matchEvent in match.Events)
                    {
                        if ((matchEvent.EventType == MATCHEVENTTYPE.GOAL || matchEvent.EventType == MATCHEVENTTYPE.OWNGOAL || matchEvent.EventType == MATCHEVENTTYPE.PENALTY)
                            && (matchEvent.InfoEvent1 == "0-1" || matchEvent.InfoEvent1 == "1-0"))
                        {
                            if (matchEvent.EventMinute >= 0 && matchEvent.EventMinute > latestGoalTime)
                                latestGoalTime = matchEvent.EventMinute;
                        }
                    }
                }

                return latestGoalTime;
            }
        }

        public int EarlierstFirstGoal
        {
            get
            {
                int earliestGoalTime = 99999;
                foreach (LSHistoricMatch match in this)
                {
                    foreach (LSHistoricMatchEvent matchEvent in match.Events)
                    {
                        if (matchEvent.EventType == MATCHEVENTTYPE.GOAL || matchEvent.EventType == MATCHEVENTTYPE.OWNGOAL || matchEvent.EventType == MATCHEVENTTYPE.PENALTY)
                        {
                            if (matchEvent.EventMinute >= 0 && matchEvent.EventMinute < earliestGoalTime)
                                earliestGoalTime = matchEvent.EventMinute;
                        }
                    }
                }

                return earliestGoalTime;
            }
        }

        public double AvgGoals
        {
            get
            {
                double avgGoals = 0.0;
                int iCounter = 0;
                foreach (LSHistoricMatch match in this)
                {
                    iCounter++;
                    avgGoals += match.ScoreA + match.ScoreB;
                }


                avgGoals = avgGoals / iCounter;
                return Math.Round(avgGoals, 2);
            }
        }

        public double AvgFirstGoalMinute
        {
            get
            {
                double avgMinute = 0.0;
                int iCounter = 0;
                foreach (LSHistoricMatch match in this)
                {
                    if (match.FirstGoalMinute >= 0)
                    {
                        iCounter++;
                        avgMinute += match.FirstGoalMinute;
                    }
                }

                if (iCounter > 0)
                {
                    avgMinute = avgMinute / iCounter;

                }
                return Math.Round(avgMinute, 0);
            }
        }


        public double getScorePercentage(SCORES score, ulong teamId)
        {
            double counter = 0;
            foreach (LSHistoricMatch match in this)
            {
                if (match.TeamAId == teamId)
                {
                    if (match.ScoreA == _staticScores[score].ScoreA && match.ScoreB == _staticScores[score].ScoreB)
                        counter++;
                }
                else
                {
                    if (match.ScoreB == _staticScores[score].ScoreA && match.ScoreA == _staticScores[score].ScoreB)
                        counter++;
                }
            }
            return (counter/this.Count)*100;
        }

        public HistoricOverUnderData getOverUnder(HistoricOverUnderData.OVERUNDERTYPES overUnderType)
        {
            
            HistoricOverUnderData ouData = new HistoricOverUnderData(overUnderType);
            if (this.Count == 0)
            {
                ouData.OverPercent = 0.0;
                return ouData;
            }

            double ouValue = HistoricOverUnderData.OVERUNDERVALUES[(int)overUnderType];
            double overCount = 0.0;
            foreach (LSHistoricMatch match in this)
            {
                //Tore der Mannschaften addieren und mit dem Over/Under-Wert überprüfen
                double goals = match.ScoreA + match.ScoreB;
                if (goals > ouValue)
                {
                    //Over
                    overCount++;
                    ouData.addTrendValue(HistoricOverUnderData.POSITV);
                }
                else
                {
                    ouData.addTrendValue(HistoricOverUnderData.NEGATIV);
                }
            }

            ouData.OverPercent = (overCount / this.Count) * 100;
            return ouData;
        }

        public HistoricWLDData getWLD(ulong teamId)
        {
            HistoricWLDData wldData = new HistoricWLDData();
            if (this.Count == 0)
            {
                wldData.WinPercent = 0.0;
                wldData.LossPercent = 0.0;
                wldData.DrawPercent = 0.0;

                return wldData;
            }

            double winCount  = 0.0;
            double lossCount = 0.0;
            double drawCount = 0.0;

            foreach (LSHistoricMatch match in this)
            {
                if (match.ScoreA == 0 && match.ScoreB == 0)
                {
                    drawCount++;
                    wldData.addTrendValue(HistoricWLDData.ZERO);
                }
                else if (match.ScoreA == match.ScoreB)
                {
                    drawCount++;
                    wldData.addTrendValue(HistoricWLDData.EQUAL);
                }
                else if (match.ScoreA > match.ScoreB && match.TeamAId == teamId)
                {
                    winCount++;
                    wldData.addTrendValue(HistoricWLDData.POSITV);
                }
                else if (match.ScoreA > match.ScoreB && match.TeamAId != teamId)
                {
                    lossCount++;
                    wldData.addTrendValue(HistoricWLDData.NEGATIV);
                }
                else if (match.ScoreB > match.ScoreA && match.TeamBId == teamId)
                {
                    winCount++;
                    wldData.addTrendValue(HistoricWLDData.POSITV);
                }
                else if (match.ScoreB > match.ScoreA && match.TeamBId != teamId)
                {
                    lossCount++;
                    wldData.addTrendValue(HistoricWLDData.NEGATIV);
                }                

            }

            wldData.WinPercent = (winCount / this.Count) * 100;
            wldData.LossPercent = (lossCount / this.Count) * 100;
            wldData.DrawPercent = (drawCount / this.Count) * 100;


            return wldData;
        }
    }

    public class HistoricOverUnderData
    {
        public enum OVERUNDERTYPES { ZEROPOINTFIVE, ONEPOINTFIVE, TWOPOINTFIVE, THREEPOINTFIVE, FOURPOINTFIVE, FIVEPOINTFIVE, SIXPOINTFIVE, OTHERS }
        public static readonly double[] OVERUNDERVALUES = { 0.5, 1.5, 2.5, 3.5, 4.5, 5.5, 6.5, 7.5 };
        public const char POSITV = '+';
        public const char NEGATIV = '-';

        private ArrayList _trend;
        private OVERUNDERTYPES _overUnderType;

        public HistoricOverUnderData(OVERUNDERTYPES overUnderType)
        {
            _trend = new ArrayList();
            _overUnderType = overUnderType;
        }

        public void addTrendValue(char value)
        {
            _trend.Add(value);
        }

        public double OverPercent { get; set; }
        
        /// <summary>
        /// Kein extr Attribute für Under, da Under immer 100 - Over ist.
        /// </summary>
        public char[] Trend { get { return (char[])_trend.ToArray(typeof(char)); } }
        public OVERUNDERTYPES OverUnderType { get { return _overUnderType; } }

    }

    public class HistoricWLDData
    {
        public const char POSITV = '+';
        public const char NEGATIV = '-';
        public const char EQUAL = '=';
        public const char ZERO = '0';
        private ArrayList _trend;
        public HistoricWLDData()
        {
            _trend = new ArrayList();
        }

        public void addTrendValue(char value)
        {
            _trend.Add(value);
        }

        public double WinPercent { get; set; }
        public double LossPercent { get; set; }
        public double DrawPercent { get; set; }
        
        public char[] Trend { get { return (char[])_trend.ToArray(typeof(char)); } }
    }
}
