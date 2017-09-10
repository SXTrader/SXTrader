using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase.GUI
{
    public partial class ctlStatisticRangePicker : UserControl
    {
        private StatisticSelectionElement _statisticSelectionElement;

        public event EventHandler<DeleteRangeEventArgs> DeleteRangeEvent;

        public StatisticSelectionElement StatisticRangeElement
        {
            get
            {
                return _statisticSelectionElement;
            }
            set
            {
                btnDelete.Enabled = false;
                _statisticSelectionElement = value;
                if (_statisticSelectionElement != null)
                {
                    btnDelete.Enabled = true;
                    decimal tmpValue = 000.00m;

                    tmpValue = (decimal)_statisticSelectionElement.HiRange;
                    spnHi.Value = (decimal)tmpValue;//_colorRangeElement.Hi;

                    tmpValue = (decimal)_statisticSelectionElement.LoRange;
                    spnLo.Value = (decimal)tmpValue;// _colorRangeElement.Lo;

                    if (spnHi.Value <= spnLo.Value)
                        spnHi.Value = spnLo.Value + (decimal)1;

                    cbxTeam.SelectedValue = _statisticSelectionElement.Team;
                    cbxHomeAway.SelectedValue = _statisticSelectionElement.HomeAway;
                    cbxStatistic.SelectedValue = _statisticSelectionElement.Statistic;

                }
            }
        }

        public ctlStatisticRangePicker()
        {
            InitializeComponent();
            buildTeamComboBox();
            buildHomeAwayComboBox();
            buildStatisticsComboBox();            
            _statisticSelectionElement = StatisticSelectionElement.createNew();
        }       

        private void buildStatisticsComboBox()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Selektion");
            DataColumn dcDisplay = new DataColumn("Display");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcDisplay);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strAvgFirstGoal;
            dr["Value"] = STATISTICTYPE.AVGFIRSTGOAL;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strAvgGoals;
            dr["Value"] = STATISTICTYPE.AVGGOALS;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strEarliestFirstGoal;
            dr["Value"] = STATISTICTYPE.EARLIESTFIRSTGOAL;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strLatestFirstGoal;
            dr["Value"] = STATISTICTYPE.LATESTFIRSTGOAL;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore00;
            dr["Value"] = STATISTICTYPE.SCORE00;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore01;
            dr["Value"] = STATISTICTYPE.SCORE01;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore02;
            dr["Value"] = STATISTICTYPE.SCORE02;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore03;
            dr["Value"] = STATISTICTYPE.SCORE03;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore10;
            dr["Value"] = STATISTICTYPE.SCORE10;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore11;
            dr["Value"] = STATISTICTYPE.SCORE11;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore12;
            dr["Value"] = STATISTICTYPE.SCORE12;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore13;
            dr["Value"] = STATISTICTYPE.SCORE13;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore20;
            dr["Value"] = STATISTICTYPE.SCORE20;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore21;
            dr["Value"] = STATISTICTYPE.SCORE21;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore22;
            dr["Value"] = STATISTICTYPE.SCORE22;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore23;
            dr["Value"] = STATISTICTYPE.SCORE23;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore30;
            dr["Value"] = STATISTICTYPE.SCORE30;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore31;
            dr["Value"] = STATISTICTYPE.SCORE31;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore32;
            dr["Value"] = STATISTICTYPE.SCORE32;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScore33;
            dr["Value"] = STATISTICTYPE.SCORE33;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strScoreOther;
            dr["Value"] = STATISTICTYPE.SCOREOTHER;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strWin;
            dr["Value"] = STATISTICTYPE.WIN;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strLoss;
            dr["Value"] = STATISTICTYPE.LOSS;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strDraw;
            dr["Value"] = STATISTICTYPE.DRAW;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strOver05;
            dr["Value"] = STATISTICTYPE.OVER05;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strOver15;
            dr["Value"] = STATISTICTYPE.OVER15;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strOver25;
            dr["Value"] = STATISTICTYPE.OVER25;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strOver35;
            dr["Value"] = STATISTICTYPE.OVER35;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strOver45;
            dr["Value"] = STATISTICTYPE.OVER45;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strNoOfData;
            dr["Value"] = STATISTICTYPE.NOOFDATA;
            dt.Rows.Add(dr);

            ds.Tables.Add(dt);

            cbxStatistic.SuspendLayout();
            cbxStatistic.DataSource = ds.Tables["Selektion"];
            cbxStatistic.DisplayMember = "Display";
            cbxStatistic.ValueMember = "Value";
            cbxStatistic.SelectedIndex = -1;
            cbxStatistic.ResumeLayout();
        }

        private void buildHomeAwayComboBox()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Selektion");
            DataColumn dcDisplay = new DataColumn("Display");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcDisplay);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strHomeAwayBoth;
            dr["Value"] = STATISTICHOMEAWAY.BOTH;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strHomeAwayHome;
            dr["Value"] = STATISTICHOMEAWAY.HOME;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Display"] = SXStatisticBase.strHomeAwayAway;
            dr["Value"] = STATISTICHOMEAWAY.AWAY;
            dt.Rows.Add(dr);

            ds.Tables.Add(dt);

            cbxHomeAway.SuspendLayout();
            cbxHomeAway.DataSource = ds.Tables["Selektion"];
            cbxHomeAway.DisplayMember = "Display";
            cbxHomeAway.ValueMember = "Value";
            cbxHomeAway.SelectedIndex = -1;
            cbxHomeAway.ResumeLayout();

        }

        private void buildTeamComboBox()
        {
            // Komboboxen füllen
            DataSet dsTeam = new DataSet();
            DataTable dtTeam = new DataTable("Selektion");
            DataColumn dcDisplay = new DataColumn("Display");
            DataColumn dcValue = new DataColumn("Value");

            dtTeam.Columns.Add(dcDisplay);
            dtTeam.Columns.Add(dcValue);

            DataRow drBoth = dtTeam.NewRow();
            drBoth["Display"] = SXStatisticBase.strTeamBoth;
            drBoth["Value"] = STATISTICTEAM.BOTH;

            dtTeam.Rows.Add(drBoth);


            DataRow drTeamA = dtTeam.NewRow();
            drTeamA["Display"] = SXStatisticBase.strTeamA;
            drTeamA["Value"] = STATISTICTEAM.TEAMA;

            dtTeam.Rows.Add(drTeamA);

            DataRow drTeamB = dtTeam.NewRow();
            drTeamB["Display"] = SXStatisticBase.strTeamB;
            drTeamB["Value"] = STATISTICTEAM.TEAMB;

            dtTeam.Rows.Add(drTeamB);


            dsTeam.Tables.Add(dtTeam);


            cbxTeam.SuspendLayout();
            cbxTeam.DataSource = dsTeam.Tables["Selektion"];
            cbxTeam.DisplayMember = "Display";
            cbxTeam.ValueMember = "Value";
            cbxTeam.SelectedIndex = -1;
            cbxTeam.ResumeLayout();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            EventHandler<DeleteRangeEventArgs> handler = DeleteRangeEvent;
            if (handler != null)
            {
                handler(this, new DeleteRangeEventArgs(_statisticSelectionElement));
            }
        }

        private void spnLo_ValueChanged(object sender, EventArgs e)
        {
            _statisticSelectionElement.LoRange = (double)spnLo.Value;
        }

        private void spnHi_ValueChanged(object sender, EventArgs e)
        {
            _statisticSelectionElement.HiRange = (double)spnHi.Value;
        }

        private void cbxTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(_statisticSelectionElement != null)
                    _statisticSelectionElement.Team = (STATISTICTEAM)Enum.Parse(typeof(STATISTICTEAM), cbxTeam.SelectedValue.ToString());
            }
            catch { }
        }

        private void cbxHomeAway_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_statisticSelectionElement != null)
                    _statisticSelectionElement.HomeAway = (STATISTICHOMEAWAY)Enum.Parse(typeof(STATISTICHOMEAWAY), cbxHomeAway.SelectedValue.ToString());
            }
            catch { }
        }

        private void cbxStatistic_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_statisticSelectionElement != null)
                    _statisticSelectionElement.Statistic = (STATISTICTYPE)Enum.Parse(typeof(STATISTICTYPE), cbxStatistic.SelectedValue.ToString());
            }
            catch { }
        }

        private void cbxTeam_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }
    }

    public class DeleteRangeEventArgs : EventArgs
    {
        private StatisticSelectionElement _range;
        public StatisticSelectionElement Range { get { return _range; } }

        public DeleteRangeEventArgs(StatisticSelectionElement range)
        {
            _range = range;
        }
    }
}
