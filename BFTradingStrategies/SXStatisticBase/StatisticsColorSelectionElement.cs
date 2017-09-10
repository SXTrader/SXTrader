using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase
{
    public class StatisticsColorSelectionElement
    {
        private static int _Static_maxCounter = 0;
        private int _objNumber;
        private int _sortNo;

        private StatisticSelectionList _statistics;

        public int RangeNumber { get { return _objNumber; } }
        public int SortNumber {get{return _sortNo;}set{_sortNo = value;}}


        public StatisticSelectionList Statistics
        {
            get{return _statistics;}
            set { _statistics = value; }
        }

        public Color StatisticColor
        {
            get;
            set;
        }

        public static StatisticsColorSelectionElement createNew()
        {
            return new StatisticsColorSelectionElement();
        }

        private StatisticsColorSelectionElement()
        {
            _objNumber = ++_Static_maxCounter;
            _statistics = new StatisticSelectionList();
            _sortNo = 1;
        }
    }

    public class StatisticsColorSelectionList : List<StatisticsColorSelectionElement>
    { }
    public class StatisticsColorSelectionSortedList : SortedList<int, StatisticsColorSelectionElement>
    {
    }
}
