using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Dialogs;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.sxstatisticbase.GUI;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.ttr.GUI.Configuration;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder.Dialogs;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls
{
    public partial class ctlASCSConfigElementBack : UserControl
    {
        private TradeStarterConfigElement _element;
        private TRADETYPE _tradeType;


        public TRADETYPE TradeType
        {
            set { _tradeType = value; }
        }

        public TradeStarterConfigElement TSConfigElement
        {
            get 
            {
                _element.AlreadyTraded = chkCommonAlreadyTraded.Checked;
                _element.HiOdds = (double)spnIPOddsHigh.Value;
                _element.HiPlaytime = (int)spnIPPlaytimeHigh.Value;
                _element.HiVolume = (int)spnIPVolumeHigh.Value;
                _element.Inplay = chkInplay.Checked;
                _element.LoOdds = (double)spnIPOddsLow.Value;
                _element.LoPlaytime = (int)spnIPPlaytimeLow.Value;
                _element.LoVolume = (int)spnIPVolumeLow.Value;
                _element.Preplay = chkPreplay.Checked;
                _element.PreplayHiOdds = (double)spnPPOddsHi.Value;
                _element.PreplayHiVolume = (int)spnPPVolumeHigh.Value;
                _element.PreplayLoOdds = (double)spnPPOddsLo.Value;
                _element.PreplayLoVolume = (int)spnPPVolumeLow.Value;
                _element.RedCardWatch = chkIPRedCard.Checked;

                if (rbnIPNoRedCards.Checked)
                    _element.RedCardWatchState = TTRRedCardWatch.NOREDCARD;
                else if (rbnIPEqualRedCards.Checked)
                    _element.RedCardWatchState = TTRRedCardWatch.EQUALCARD;
                else if (rbnIPTeamAMoreRed.Checked)
                    _element.RedCardWatchState = TTRRedCardWatch.TEAMAMORE;
                else if (rbnIPTeamBMoreRed.Checked)
                    _element.RedCardWatchState = TTRRedCardWatch.TEAMBMORE;
                
                _element.SettledAllowed = rbnCommonSettledYes.Checked;
                _element.UnsettledAllowed = rbnCommonUnsettledYes.Checked;

                if (cbxTradeType.SelectedValue != null)
                {
                    _element.RequiredTrade = (TRADETYPE)Enum.Parse(typeof(TRADETYPE), cbxTradeType.SelectedValue.ToString());
                }

                if (cbxTradeState.SelectedValue != null)
                {
                    _element.RequiredTradeState = (TRADESTATE)Enum.Parse(typeof(TRADESTATE), cbxTradeState.SelectedValue.ToString());
                }

                foreach (Control ctl in pnlStatisticsRanges.Controls)
                {
                    if (ctl.GetType() == typeof(ctlStatisticRangePicker))
                    {
                        ctlStatisticRangePicker ctlStat = ctl as ctlStatisticRangePicker;
                        _element.addStatistic(ctlStat.StatisticRangeElement);
                    }
                }
                return _element; 
            }
            set
            {
                _element = value;

                if (_element != null)
                {
                    // Prerequisite

                    if (_element.RequiredTrade != null)
                    {
                        cbxTradeType.SelectedValue = _element.RequiredTrade;
                    }

                    if (_element.RequiredTradeState != null && _element.RequiredTradeState != TRADESTATE.UNASSIGNED)
                    {
                        cbxTradeState.SelectedValue = _element.RequiredTradeState;
                    }

                    //Preplay Werte
                    chkPreplay.Checked = _element.Preplay;
                    spnPPOddsLo.Value = (decimal)_element.PreplayLoOdds;
                    spnPPOddsHi.Value = (decimal)_element.PreplayHiOdds;
                    spnPPVolumeLow.Value = (decimal)_element.PreplayLoVolume;
                    spnPPVolumeHigh.Value = (decimal)_element.PreplayHiVolume;

                    //Inplay Werte
                    chkInplay.Checked = _element.Inplay;
                    spnIPPlaytimeLow.Value = (decimal)_element.LoPlaytime;
                    spnIPPlaytimeHigh.Value = (decimal)_element.HiPlaytime;
                    spnIPOddsLow.Value = (decimal)_element.LoOdds;
                    spnIPOddsHigh.Value = (decimal)_element.HiOdds;
                    spnIPVolumeLow.Value = (decimal)_element.LoVolume;
                    spnIPVolumeHigh.Value = (decimal)_element.HiVolume;
                    chkIPRedCard.Checked = _element.RedCardWatch;

                    // Torständer
                    buildScoreBtnText();

                    if (!chkIPRedCard.Checked)
                        rbnIPEqualRedCards.Enabled = rbnIPNoRedCards.Enabled = rbnIPTeamAMoreRed.Enabled = rbnIPTeamBMoreRed.Enabled = false;

                    switch (_element.RedCardWatchState)
                    {
                        case TTRRedCardWatch.NOREDCARD:
                            rbnIPNoRedCards.Checked = true;
                            break;
                        case TTRRedCardWatch.EQUALCARD:
                            rbnIPEqualRedCards.Checked = true;
                            break;
                        case TTRRedCardWatch.TEAMAMORE:
                            rbnIPTeamAMoreRed.Checked = true;
                            break;
                        case TTRRedCardWatch.TEAMBMORE:
                            rbnIPTeamBMoreRed.Checked = true;
                            break;
                    }


                    // Common Werte
                    //Statistiken
                    foreach (StatisticSelectionElement statistic in _element.Statistics)
                    {
                        ctlStatisticRangePicker statisticPicker = new ctlStatisticRangePicker();
                        statisticPicker.DeleteRangeEvent += new EventHandler<net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteRangeEventArgs>(ctl_DeleteRangeEvent);
                        statisticPicker.StatisticRangeElement = statistic;
                        statisticPicker.Dock = DockStyle.Top;
                        pnlStatisticsRanges.Controls.Add(statisticPicker);
                    }

                    rbnCommonSettledYes.Checked = _element.SettledAllowed;
                    rbnCommonSettledNo.Checked = !_element.SettledAllowed;
                    rbnCommonUnsettledYes.Checked = _element.UnsettledAllowed;
                    rbnCommonUnsettledNo.Checked = !_element.UnsettledAllowed;

                    chkCommonAlreadyTraded.Checked = _element.AlreadyTraded;

                    //Keine Torsummen bei Correct Score - Märkten, nur Spielständ.
                    _element.HiGoalSum = _element.LoGoalSum = -1;

                    //_element.addScore(TTRScores.ZEROZERO);
                }
            }
        }

        public ctlASCSConfigElementBack(TRADETYPE tradeType)
        {
            InitializeComponent();
            this.TradeType = tradeType;
            spnPPVolumeHigh.Value = 1000000m;
            
            TTRHelper.FillTradeStateComboBox(cbxTradeState);
            TTRHelper.FillTradeTypeComboBox(cbxTradeType);
        }
       
            
        public void checkValues(ref String msg)
        {
            msg = String.Empty;

            //Preplay Odds
            if (spnPPOddsLo.Value > spnPPOddsHi.Value)
            {
                msg = TradeTheReaction.strOddsHighLowMismatch;
                return;
            }

            if (spnPPVolumeLow.Value > spnPPVolumeHigh.Value)
            {
                msg = TradeTheReaction.strMarketVolumeHighLowMismatch;
                return;
            }

            if (spnIPPlaytimeLow.Value > spnIPPlaytimeHigh.Value)
            {
                msg = TradeTheReaction.strPlaytimeHighLowMismatch;
                return;
            }

            if (spnIPOddsLow.Value > spnIPOddsHigh.Value)
            {
                msg = TradeTheReaction.strOddsHighLowMismatch;
                return;
            }

            if (spnIPVolumeLow.Value > spnIPVolumeHigh.Value)
            {
                msg = TradeTheReaction.strMarketVolumeHighLowMismatch;
                return;
            }
        }

        private void btnScores_Click(object sender, EventArgs e)
        {
            using (frmScoresSelection dlg = new frmScoresSelection())
            {
                dlg.Scores = _element.Scores;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _element.clearScores();
                    foreach (TTRScores score in dlg.Scores)
                    {
                        _element.addScore(score);
                    }

                    // Text neu aufbauen
                    buildScoreBtnText();
                }
            }
        }

        private void buildScoreBtnText()
        {
            btnScores.Text = String.Empty;
            String strScores = String.Empty;

            foreach (TTRScores score in _element.Scores)
            {
                if (strScores != String.Empty)
                {
                    strScores += ", ";
                }
                switch (score)
                {
                    case TTRScores.ZEROZERO:
                        strScores += "[0 - 0]";
                        break;
                    case TTRScores.ZEROONE:
                        strScores += "[0 - 1]";
                        break;
                    case TTRScores.ZEROTWO:
                        strScores += "[0 - 2]";
                        break;
                    case TTRScores.ZEROTHREE:
                        strScores += "[0 - 3]";
                        break;
                    case TTRScores.ONEZERO:
                        strScores += "[1 - 0]";
                        break;
                    case TTRScores.ONEONE:
                        strScores += "[1 - 1]";
                        break;
                    case TTRScores.ONETWO:
                        strScores += "[1 - 2]";
                        break;
                    case TTRScores.ONETHREE:
                        strScores += "[1 - 3]";
                        break;
                    case TTRScores.TWOZERO:
                        strScores += "[2 - 0]";
                        break;
                    case TTRScores.TWOONE:
                        strScores += "[2 - 1]";
                        break;
                    case TTRScores.TWOTWO:
                        strScores += "[2 - 2]";
                        break;
                    case TTRScores.TWOTHREE:
                        strScores += "[2 - 3]";
                        break;
                    case TTRScores.THREEZERO:
                        strScores += "[3 - 0]";
                        break;
                    case TTRScores.THREEONE:
                        strScores += "[3 - 1]";
                        break;
                    case TTRScores.THREETWO:
                        strScores += "[3 - 2]";
                        break;
                    case TTRScores.THREETHREE:
                        strScores += "[3 - 3]";
                        break;
                    case TTRScores.OTHERS:
                        strScores += "[Others]";
                        break;
                }
            }
            btnScores.Text = String.Format("{0} {1}", TradeTheReaction.strScoresASBtn, strScores);
        }

        void ctl_DeleteRangeEvent(object sender, net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteRangeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                IAsyncResult result = this.BeginInvoke(new EventHandler<net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteRangeEventArgs>(ctl_DeleteRangeEvent), new object[] { sender, e });
                this.EndInvoke(result);
            }
            else
            {
                ctlStatisticRangePicker rangeStatisticPicker = sender as ctlStatisticRangePicker;
                if (rangeStatisticPicker != null)
                {                    
                    _element.removeStatistic(rangeStatisticPicker.StatisticRangeElement);
                }

                pnlStatisticsRanges.Controls.Remove(rangeStatisticPicker);
                rangeStatisticPicker.Dispose();
            }
        }

        private void spn_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown o = (NumericUpDown)sender;
            // Nächstes incr bestimmen
            o.Increment = SXALKom.Instance.getValidOddIncrement(o.Value);
            o.Value = SXALKom.Instance.validateOdd(o.Value);
        }

        private void btnCommonStatisticsRangeNew_Click(object sender, EventArgs e)
        {
            ctlStatisticRangePicker ctl = new ctlStatisticRangePicker();
            ctl.DeleteRangeEvent += new EventHandler<net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteRangeEventArgs>(ctl_DeleteRangeEvent);
            _element.addStatistic(ctl.StatisticRangeElement);
            ctl.Dock = DockStyle.Top;
            pnlStatisticsRanges.Controls.Add(ctl);
        }

        private void chkIPRedCard_CheckedChanged(object sender, EventArgs e)
        {
            rbnIPEqualRedCards.Enabled = rbnIPNoRedCards.Enabled = rbnIPTeamAMoreRed.Enabled = rbnIPTeamBMoreRed.Enabled = chkIPRedCard.Checked;
        }

        private void btnCommonTradeConfig_Click(object sender, EventArgs e)
        {
            using (frmTradeOutConfig dlg = new frmTradeOutConfig(_tradeType))
            {
                dlg.TradeOutCheckList = _element.TradeConfig.TradeOutRules;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _element.TradeConfig.TradeOutRules = dlg.TradeOutCheckList;
                }
            }
        }

        private void btnTradeInMoneyConfig_Click(object sender, EventArgs e)
        {
            using (frmTradeInMoneyConfig dlg = new frmTradeInMoneyConfig())
            {
                dlg.Configuration = _element.TradeConfig;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _element.TradeConfig = dlg.Configuration;
                }
            }
        }

        private void cbxTradeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                if (cbx.SelectedValue == null)
                {
                    cbxTradeState.Enabled = false;
                }
                else
                {
                    try
                    {
                        String str = cbx.SelectedValue.ToString();
                        TRADETYPE tradeType = (TRADETYPE)Enum.Parse(typeof(TRADETYPE), str);
                        if (tradeType == TRADETYPE.UNASSIGNED)
                        {
                            cbxTradeState.Enabled = false;
                        }
                        else
                        {
                            cbxTradeState.Enabled = true;
                        }
                    }
                    catch (ArgumentException)
                    {
                        cbx.SelectedIndex = -1;
                    }
                }
            }
        }
    }
}
