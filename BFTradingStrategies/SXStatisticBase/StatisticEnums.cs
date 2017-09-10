using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase
{
    public enum STATISTICTEAM { BOTH, TEAMA, TEAMB };
    public enum STATISTICHOMEAWAY { BOTH, HOME, AWAY };
    public enum STATISTICTYPE
    {
        AVGFIRSTGOAL, AVGGOALS, EARLIESTFIRSTGOAL, LATESTFIRSTGOAL, SCORE00, SCORE01, SCORE02, SCORE03,
        SCORE10, SCORE11, SCORE12, SCORE13, SCORE20, SCORE21, SCORE22, SCORE23, SCORE30, SCORE31, SCORE32, SCORE33, SCOREOTHER,
        WIN, LOSS, DRAW, OVER05, OVER15, OVER25, OVER35, OVER45, NOOFDATA
    };

    public enum STATISTICTREND { POSITIVE, NEGATIVE, NEUTRAL, ZEROTOZERO };
    /*
    public class StatisticEnums
    {
    }
     * */
}
