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
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.common
{
    public partial class ctlHistoricDataView : UserControl
    {
        private HistoricMatchList _matchList;
        private SXALMarket _helperClass;

        public event EventHandler<ShowMatchDetailEventArgs> ShowMatchDetail;
        public event EventHandler<MoreStatisticsEventArgs> MoreStatisticsEvent;

        public ctlHistoricDataView()
        {
            InitializeComponent();
        }

        public String Titel { set { gbxTitel.Text = value; } }

        public HistoricMatchList MatchList
        {
            set { _matchList = value; buildData(); }
        }

        public SXALMarket NameInformations
        {
            set
            {
                _helperClass = value;
            }
        }

        private void buildData()
        {
            try
            {
                if (_matchList == null)
                    return;

                lvwHistoricData.Items.Clear();

                lblAvgFirstGoalTimeNumber.Text = _matchList.AvgFirstGoalMinute.ToString();
                lblAvgGoalNumber.Text = _matchList.AvgGoals.ToString();
                lblEarlierstFirstGoalTimeNumber.Text = _matchList.EarlierstFirstGoal.ToString();
                lblLatestFirstGoalTimeNumber.Text = _matchList.LatestFirstGoal.ToString();
                lblZeroToZeroNumber.Text = _matchList.ZeroZeroPercentage.ToString();

                foreach (LSHistoricMatch match in _matchList)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = match.Devision;
                    item.SubItems.Add(match.MatchDate.ToString("dd.MM.yyyy"));
                    if (_helperClass != null && _helperClass.Liveticker != null)
                    {                        
                        if (match.TeamAId == _helperClass.Liveticker.TeamAId)
                            item.SubItems.Add(_helperClass.TeamA);
                        else if (match.TeamAId == _helperClass.Liveticker.TeamBId)
                            item.SubItems.Add(_helperClass.TeamB);
                        else
                        {
                            if (match.TeamA != String.Empty)
                                item.SubItems.Add(match.TeamA);
                            else
                                item.SubItems.Add(match.TeamAId.ToString());
                        }

                    }
                    else
                    {
                        if (match.TeamA != String.Empty)
                            item.SubItems.Add(match.TeamA);
                        else
                            item.SubItems.Add(match.TeamAId.ToString());
                    }
                    String score = String.Format("{0} - {1} ({2})", match.ScoreA, match.ScoreB, match.HalftimeScore);
                    item.SubItems.Add(score);
                    if (_helperClass != null)
                    {
                        if (match.TeamBId == _helperClass.Liveticker.TeamBId)
                            item.SubItems.Add(_helperClass.TeamB);
                        else if (match.TeamBId == _helperClass.Liveticker.TeamAId)
                            item.SubItems.Add(_helperClass.TeamA);
                        else
                        {
                            if (match.TeamB != String.Empty)
                                item.SubItems.Add(match.TeamB);
                            else
                                item.SubItems.Add(match.TeamBId.ToString());
                        }

                    }
                    else
                    {
                        if (match.TeamB != String.Empty)
                            item.SubItems.Add(match.TeamB);
                        else
                            item.SubItems.Add(match.TeamBId.ToString());
                    }
                    item.Tag = match;
                    lvwHistoricData.Items.Add(item);
                }

                lvwHistoricData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void lvwHistoricData_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lvwHistoricData.InvokeRequired)
                {
                    IAsyncResult result = lvwHistoricData.BeginInvoke(new EventHandler<EventArgs>(lvwHistoricData_SelectedIndexChanged), new object[] { sender, e });
                    lvwHistoricData.EndInvoke(result);
                }
                else
                {
                    if (lvwHistoricData.SelectedItems.Count > 0)
                    {
                        LSHistoricMatch theMatch = (LSHistoricMatch)lvwHistoricData.SelectedItems[0].Tag;
                        if (theMatch != null)
                        {
                            EventHandler<ShowMatchDetailEventArgs> handler = ShowMatchDetail;
                            if (handler != null)
                            {
                                handler(this, new ShowMatchDetailEventArgs(theMatch));
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            try
            {
                EventHandler<MoreStatisticsEventArgs> handler = MoreStatisticsEvent;

                if (handler != null)
                {
                    handler(this, new MoreStatisticsEventArgs());
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }

    public class MoreStatisticsEventArgs : EventArgs { }

    public class ShowMatchDetailEventArgs : EventArgs
    {
        private LSHistoricMatch _match;

        public LSHistoricMatch TheMatch
        {
            get
            {
                return _match;
            }
        }

        public ShowMatchDetailEventArgs(LSHistoricMatch theMatch)
        {
            _match = theMatch;
        }
    }
}
