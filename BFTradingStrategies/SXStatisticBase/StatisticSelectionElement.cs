using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase
{
    /// <summary>
    /// Klasse um Ranges über eine Statistik zu definiere
    /// </summary>
    public class StatisticSelectionElement
    {
        private static int _Static_maxCounter = 0;
        private int _objNumber;

        public STATISTICTEAM Team { get; set; }
        public STATISTICHOMEAWAY HomeAway { get; set; }
        public STATISTICTYPE Statistic { get; set; }
        public double LoRange { get; set; }
        public double HiRange { get; set; }

        public int RangeNumber { get { return _objNumber; } }

        public static StatisticSelectionElement createNew()
        {
            return new StatisticSelectionElement();
        }

        private StatisticSelectionElement()
        {
            _objNumber = ++_Static_maxCounter;            
            LoRange= HiRange = 0.0;
        }

        public override string ToString()
        {
            String msg = String.Empty;

            switch (Team)
            {
                case STATISTICTEAM.BOTH:
                    msg += SXStatisticBase.strTeamBoth;
                    break;
                case STATISTICTEAM.TEAMA:
                    msg += SXStatisticBase.strTeamA;
                    break;
                case STATISTICTEAM.TEAMB:
                    msg += SXStatisticBase.strTeamB;
                    break;
            }

            msg += " ";

            switch (HomeAway)
            {
                case STATISTICHOMEAWAY.BOTH:
                    msg += SXStatisticBase.strHomeAwayBoth;
                    break;
                case STATISTICHOMEAWAY.AWAY:
                    msg += SXStatisticBase.strHomeAwayAway;
                    break;
                case STATISTICHOMEAWAY.HOME:
                    msg += SXStatisticBase.strHomeAwayHome;
                    break;
            }

            msg += " ";

            switch (Statistic)
            {
                case STATISTICTYPE.AVGFIRSTGOAL:
                    msg += SXStatisticBase.strAvgFirstGoal;
                    break;
                case STATISTICTYPE.AVGGOALS:
                    msg += SXStatisticBase.strAvgGoals;
                    break;
                case STATISTICTYPE.DRAW:
                    msg += SXStatisticBase.strDraw;
                    break;
                case STATISTICTYPE.EARLIESTFIRSTGOAL:
                    msg += SXStatisticBase.strEarliestFirstGoal;
                    break;
                case STATISTICTYPE.LATESTFIRSTGOAL:
                    msg += SXStatisticBase.strLatestFirstGoal;
                    break;
                case STATISTICTYPE.LOSS:
                    msg += SXStatisticBase.strLoss;
                    break;
                case STATISTICTYPE.OVER05:
                    msg += SXStatisticBase.strOver05;
                    break;
                case STATISTICTYPE.OVER15:
                    msg += SXStatisticBase.strOver15;
                    break;
                case STATISTICTYPE.OVER25:
                    msg += SXStatisticBase.strOver25;
                    break;
                case STATISTICTYPE.OVER35:
                    msg += SXStatisticBase.strOver35;
                    break;
                case STATISTICTYPE.OVER45:
                    msg += SXStatisticBase.strOver45;
                    break;
                case STATISTICTYPE.SCORE00:
                    msg += SXStatisticBase.strScore00;
                    break;
                case STATISTICTYPE.SCORE01:
                    msg += SXStatisticBase.strScore01;
                    break;
                case STATISTICTYPE.SCORE02:
                    msg += SXStatisticBase.strScore02;
                    break;
                case STATISTICTYPE.SCORE03:
                    msg += SXStatisticBase.strScore03;
                    break;
                case STATISTICTYPE.SCORE10:
                    msg += SXStatisticBase.strScore10;
                    break;
                case STATISTICTYPE.SCORE11:
                    msg += SXStatisticBase.strScore11;
                    break;
                case STATISTICTYPE.SCORE12:
                    msg += SXStatisticBase.strScore12;
                    break;
                case STATISTICTYPE.SCORE13:
                    msg += SXStatisticBase.strScore13;
                    break;
                case STATISTICTYPE.SCORE20:
                    msg += SXStatisticBase.strScore20;
                    break;
                case STATISTICTYPE.SCORE21:
                    msg += SXStatisticBase.strScore21;
                    break;
                case STATISTICTYPE.SCORE22:
                    msg += SXStatisticBase.strScore22;
                    break;
                case STATISTICTYPE.SCORE23:
                    msg += SXStatisticBase.strScore23;
                    break;
                case STATISTICTYPE.SCORE30:
                    msg += SXStatisticBase.strScore30;
                    break;
                case STATISTICTYPE.SCORE31:
                    msg += SXStatisticBase.strScore31;
                    break;
                case STATISTICTYPE.SCORE32:
                    msg += SXStatisticBase.strScore32;
                    break;
                case STATISTICTYPE.SCORE33:
                    msg += SXStatisticBase.strScore33;
                    break;
                case STATISTICTYPE.SCOREOTHER:
                    msg += SXStatisticBase.strScoreOther;
                    break;
                case STATISTICTYPE.WIN:
                    msg += SXStatisticBase.strWin;
                    break;
                case STATISTICTYPE.NOOFDATA:
                    msg += SXStatisticBase.strNoOfData;
                    break;
            }

            msg += " ";

            msg += "between " + LoRange.ToString() + " and " + HiRange.ToString() + ". ";

            return msg;
        }
    }

    public class StatisticSelectionList : List<StatisticSelectionElement>
    {}
}
