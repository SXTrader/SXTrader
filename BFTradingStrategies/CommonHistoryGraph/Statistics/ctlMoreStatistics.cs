using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.common.Configurations;

namespace net.sxtrader.bftradingstrategies.common.Statistics
{
    public partial class ctlMoreStatistics : UserControl
    {
        private enum SELECTIONOPTION { BOTH, HOME, AWAY };

        private HistoricDataStatistic _dataStatistic;
        private IScore _livescore;
        private bool _noDataDisplay = false;

        public IScore Livescore
        {
            get { return _livescore; }
            set
            {
                try
                {
                    _noDataDisplay = true;
                    _livescore = value;
                    if (_livescore != null)
                    {
                        buildSelektorComboBox(cbxSelectorDirect, _livescore.TeamA);
                        buildSelektorComboBox(cbxSelectorA, _livescore.TeamA);
                        buildSelektorComboBox(cbxSelectorB, _livescore.TeamB);
                    }
                    _noDataDisplay = false;
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        public event EventHandler<LeaveStatisticsEventArgs> LeaveStatisticsEvent;
        public ctlMoreStatistics()
        {
            InitializeComponent();
            _dataStatistic = null;
        }


        private void buildSelektorComboBox(ComboBox cbx, String teamName)
        {
            try
            {
                // Komboboxen füllen
                DataSet dsDirect = new DataSet();
                DataTable dtDirect = new DataTable("Selektion");
                DataColumn dcDisplay = new DataColumn("Display");
                DataColumn dcValue = new DataColumn("Value");

                dtDirect.Columns.Add(dcDisplay);
                dtDirect.Columns.Add(dcValue);

                DataRow drDirectBoth = dtDirect.NewRow();
                drDirectBoth["Display"] = HistoryGraph.strHomeAway;
                drDirectBoth["Value"] = SELECTIONOPTION.BOTH;

                dtDirect.Rows.Add(drDirectBoth);

                DataRow drDirectHome = dtDirect.NewRow();
                drDirectHome["Display"] = String.Format(HistoryGraph.strFormatHome, teamName);
                drDirectHome["Value"] = SELECTIONOPTION.HOME;

                dtDirect.Rows.Add(drDirectHome);

                DataRow drDirectAway = dtDirect.NewRow();
                drDirectAway["Display"] = String.Format(HistoryGraph.strFormatAway, teamName);
                drDirectAway["Value"] = SELECTIONOPTION.AWAY;

                dtDirect.Rows.Add(drDirectAway);

                dsDirect.Tables.Add(dtDirect);

                cbx.SuspendLayout();
                cbx.DataSource = dsDirect.Tables["Selektion"];
                cbx.DisplayMember = "Display";
                cbx.ValueMember = "Value";
                cbx.SelectedIndex = -1;
                cbx.ResumeLayout();
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
        
        public async void loadStatistics()
        {
            try
            {
                if (Livescore == null || Livescore.TeamAId == 0 || Livescore.TeamBId == 0 || Livescore.TeamA == null ||
                    Livescore.TeamA == String.Empty || Livescore.TeamB == null || Livescore.TeamB == String.Empty)
                {
                    return;
                }
                IHistoricDataService historicService = HistoricDataServiceFactory.getInstance(this.Livescore.TeamAId, this.Livescore.TeamBId,
                    this.Livescore.TeamA, this.Livescore.TeamB);
                SAConfigurationRW config = new SAConfigurationRW();
                try
                {
                    _dataStatistic = await historicService.GetStatistic(this.Livescore.TeamAId, this.Livescore.TeamBId,
                        this.Livescore.TeamA, this.Livescore.TeamB, this.Livescore.League, config.NoOfData, config.AgeOfData);
                }
                catch (NoHistoricDataException)
                {
                    historicService = HistoricDataServiceFactory.getInstance(this.Livescore.TeamAId, this.Livescore.TeamBId,
                        this.Livescore.TeamA, this.Livescore.TeamB);
                    try
                    {
                        _dataStatistic = historicService.GetStatistic(this.Livescore.TeamAId, this.Livescore.TeamBId,
                            this.Livescore.TeamA, this.Livescore.TeamB, this.Livescore.League, config.NoOfData, config.AgeOfData).Result;
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                }


                if (_dataStatistic != null)
                {
                    cbxSelectorDirect.SelectedIndex = 0;
                    cbxSelectorA.SelectedIndex = 0;
                    cbxSelectorB.SelectedIndex = 0;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                EventHandler<LeaveStatisticsEventArgs> handler = LeaveStatisticsEvent;
                if (handler != null)
                {
                    handler(this, new LeaveStatisticsEventArgs());
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void ctlMoreStatistics_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(ctlMoreStatistics_SizeChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    pnlDirect.Width = pnlTeamA.Width = pnlTeamB.Width = this.Width / 3;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void cbxSelectorDirect_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cbxSelectorDirect_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbxSelectorDirect.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(cbxSelectorDirect_SelectedValueChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    if (_noDataDisplay)
                        return;
                    HistoricMatchList localMatchList = new HistoricMatchList();
                    ComboBox cbx = (sender as ComboBox);
                    if (cbx == null)
                        return;

                    if (cbx.SelectedValue.ToString() == SELECTIONOPTION.BOTH.ToString())
                    {
                        localMatchList = _dataStatistic.Direct;
                    }
                    else if (cbx.SelectedValue.ToString() == SELECTIONOPTION.AWAY.ToString())
                    {
                        foreach (LSHistoricMatch match in _dataStatistic.Direct)
                        {
                            if (match.TeamBId != Livescore.TeamAId)
                                continue;
                            localMatchList.Add(match);
                        }
                    }
                    else if (cbx.SelectedValue.ToString() == SELECTIONOPTION.HOME.ToString())
                    {
                        foreach (LSHistoricMatch match in _dataStatistic.Direct)
                        {
                            if (match.TeamAId != Livescore.TeamAId)
                                continue;
                            localMatchList.Add(match);

                        }
                    }

                    ctlMatrixDirect.MatchList = localMatchList;
                    ctlMatrixDirect.TeamAId = _dataStatistic.TeamAId;
                    ctlMatrixDirect.TeamAName = _dataStatistic.TeamAName;
                    ctlMatrixDirect.TeamBId = _dataStatistic.TeamBId;
                    ctlMatrixDirect.TeamBName = _dataStatistic.TeamBName;
                    ctlMatrixDirect.loadMatrix();

                    ctlWLDDirect.TrendDate = localMatchList.getWLD(_dataStatistic.TeamAId);
                    ctlCommonDirect.MatchList = localMatchList;

                    ctlOverUnder05Direct.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ZEROPOINTFIVE);
                    ctlOverUnder15Direct.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ONEPOINTFIVE);
                    ctlOverUnder25Direct.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.TWOPOINTFIVE);
                    ctlOverUnder35Direct.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.THREEPOINTFIVE);
                    ctlOverUnder45Direct.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.FOURPOINTFIVE);

                    gbxScoreMatrixDirect.Text = String.Format("{0} (n = {1})", "Direct", localMatchList.Count);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void cbxSelectorA_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbxSelectorA.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(cbxSelectorA_SelectedValueChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    if (_noDataDisplay)
                        return;
                    HistoricMatchList localMatchList = new HistoricMatchList();

                    ComboBox cbx = (sender as ComboBox);
                    if (cbx == null)
                        return;

                    if (cbx.SelectedValue.ToString() == SELECTIONOPTION.BOTH.ToString())
                    {
                        localMatchList = _dataStatistic.TeamA;
                    }
                    else if (cbx.SelectedValue.ToString() == SELECTIONOPTION.AWAY.ToString())
                    {
                        foreach (LSHistoricMatch match in _dataStatistic.TeamA)
                        {
                            if (match.TeamBId != Livescore.TeamAId)
                                continue;
                            localMatchList.Add(match);
                        }
                    }
                    else if (cbx.SelectedValue.ToString() == SELECTIONOPTION.HOME.ToString())
                    {
                        foreach (LSHistoricMatch match in _dataStatistic.TeamA)
                        {
                            if (match.TeamAId != Livescore.TeamAId)
                                continue;
                            localMatchList.Add(match);

                        }
                    }

                    ctlMatrixTeamA.MatchList = localMatchList;
                    ctlMatrixTeamA.TeamAId = _dataStatistic.TeamAId;
                    ctlMatrixTeamA.TeamAName = _dataStatistic.TeamAName;
                    ctlMatrixTeamA.loadMatrix();

                    ctlOverUnder05A.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ZEROPOINTFIVE);
                    ctlOverUnder15A.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ONEPOINTFIVE);
                    ctlOverUnder25A.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.TWOPOINTFIVE);
                    ctlOverUnder35A.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.THREEPOINTFIVE);
                    ctlOverUnder45A.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.FOURPOINTFIVE);

                    ctlWLDA.TrendDate = localMatchList.getWLD(_dataStatistic.TeamAId);
                    ctlCommonA.MatchList = localMatchList;
                    gbxScoreMatrixTeamA.Text = String.Format("{0} (n = {1})", _dataStatistic.TeamAName, localMatchList.Count);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void cbxSelectorB_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbxSelectorB.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(cbxSelectorB_SelectedValueChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    if (_noDataDisplay)
                        return;
                    HistoricMatchList localMatchList = new HistoricMatchList();

                    ComboBox cbx = (sender as ComboBox);
                    if (cbx == null)
                        return;

                    if (cbx.SelectedValue.ToString() == SELECTIONOPTION.BOTH.ToString())
                    {
                        localMatchList = _dataStatistic.TeamB;
                    }
                    else if (cbx.SelectedValue.ToString() == SELECTIONOPTION.AWAY.ToString())
                    {
                        foreach (LSHistoricMatch match in _dataStatistic.TeamB)
                        {
                            if (match.TeamBId != Livescore.TeamBId)
                                continue;
                            localMatchList.Add(match);
                        }
                    }
                    else if (cbx.SelectedValue.ToString() == SELECTIONOPTION.HOME.ToString())
                    {
                        foreach (LSHistoricMatch match in _dataStatistic.TeamB)
                        {
                            if (match.TeamAId != Livescore.TeamBId)
                                continue;
                            localMatchList.Add(match);

                        }
                    }

                    ctlMatrixTeamB.MatchList = localMatchList;
                    ctlMatrixTeamB.TeamAId = _dataStatistic.TeamBId;
                    ctlMatrixTeamB.TeamAName = _dataStatistic.TeamBName;
                    ctlMatrixTeamB.loadMatrix();

                    ctlOverUnder05B.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ZEROPOINTFIVE);
                    ctlOverUnder15B.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ONEPOINTFIVE);
                    ctlOverUnder25B.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.TWOPOINTFIVE);
                    ctlOverUnder35B.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.THREEPOINTFIVE);
                    ctlOverUnder45B.TrendData = localMatchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.FOURPOINTFIVE);

                    ctlA.TrendDate = localMatchList.getWLD(_dataStatistic.TeamBId);
                    ctlCommonB.MatchList = localMatchList;
                    gbxScoreMatrixTeamB.Text = String.Format("{0} (n = {1})", _dataStatistic.TeamBName, localMatchList.Count);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }

    public class LeaveStatisticsEventArgs : EventArgs { }
}
