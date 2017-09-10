using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.common.Statistics
{
    public partial class ctlCommonStats : UserControl
    {
        private HistoricMatchList _matchList;
        public HistoricMatchList MatchList { get { return _matchList; }
            set 
            {
                try
                {
                    _matchList = value;
                    if (_matchList != null)
                    {
                        lblAvgGoalNumber.Text = _matchList.AvgGoals.ToString();
                        lblAvg1stGoalNumber.Text = _matchList.AvgFirstGoalMinute.ToString();
                        lblEarlist1stGoalNumber.Text = _matchList.EarlierstFirstGoal.ToString();
                        lblLatest1stGoalNumber.Text = _matchList.LatestFirstGoal.ToString();
                    }
                    else
                    {
                        lblAvg1stGoalNumber.Text = "n.a.";
                        lblAvgGoalNumber.Text = "n.a";
                        lblEarlist1stGoalNumber.Text = "n.a";
                        lblLatest1stGoalNumber.Text = "n.a";
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        public ctlCommonStats()
        {
            InitializeComponent();
        }
        
    }
}
