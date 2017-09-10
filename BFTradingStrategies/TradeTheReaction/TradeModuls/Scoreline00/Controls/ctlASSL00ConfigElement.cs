using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.sxstatisticbase.GUI;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.ttr.GUI.Configuration;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    public partial class ctlSL00ConfigElement : UserControl
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

                _element.clearScores();
                _element.addScore(TTRScores.ZEROZERO);

                _element.SettledAllowed = rbnCommonSettledYes.Checked;
                _element.UnsettledAllowed = rbnCommonUnsettledYes.Checked;

                foreach (Control ctl in pnlStatisticsRanges.Controls)
                {
                    if (ctl.GetType() == typeof(ctlStatisticRangePicker))
                    {
                        ctlStatisticRangePicker ctlStat = ctl as ctlStatisticRangePicker;
                        _element.addStatistic(ctlStat.StatisticRangeElement);
                    }
                }

                // Für Scoreline 0 - 0 nicht benötigte Felder löschen /auf initial setzen
                _element.LoGoalSum = _element.HiGoalSum = -1;
                _element.TradeConfig.RelativeBetAmount = false;
                _element.TradeConfig.RelativeBetSize = 0;
                _element.TradeConfig.RelativeBetType = RELATIVEBETTINGTYPE.UNASSIGNED;
                _element.TradeConfig.RelativeTradeType = TRADETYPE.UNASSIGNED;

                return _element; 
            }
            set
            {
                _element = value;
                
                if (_element != null)
                {
                    //Preplay Werte
                    chkPreplay.Checked    = _element.Preplay;
                    spnPPOddsLo.Value     = (decimal)_element.PreplayLoOdds;
                    spnPPOddsHi.Value     = (decimal)_element.PreplayHiOdds;
                    spnPPVolumeLow.Value  = (decimal)_element.PreplayLoVolume;
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

                    //Einziger Erlaubte Spielstände bei Scoreline 0 - 0 ist das 0 - 0
                    _element.clearScores();
                    _element.addScore(TTRScores.ZEROZERO);

                    _element.LoGoalSum = _element.HiGoalSum = -1;
                }
            }
        }

        public ctlSL00ConfigElement(TRADETYPE tradeType)
        {
            InitializeComponent();
            this.TradeType = tradeType;
            spnPPVolumeHigh.Value = 1000000m;
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

        private void spn_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown o = (NumericUpDown)sender;
            // Nächstes incr bestimmen
            o.Increment = SXALKom.Instance.getValidOddIncrement(o.Value);
            o.Value = SXALKom.Instance.validateOdd(o.Value);
        }

        private void btnStatisticsRangeNew_Click(object sender, EventArgs e)
        {
            ctlStatisticRangePicker ctl = new ctlStatisticRangePicker();
            ctl.DeleteRangeEvent += new EventHandler<net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteRangeEventArgs>(ctl_DeleteRangeEvent);
            _element.addStatistic(ctl.StatisticRangeElement);
            ctl.Dock = DockStyle.Top;
            pnlStatisticsRanges.Controls.Add(ctl);
            //ctlRangeColorPicker rangeColorPicker = new ctlRangeColorPicker();
            //rangeColorPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeWLDWinPicker_DeleteRangeEvent);
            //_rangeWLDWin.Add(rangeColorPicker.ColorRangeElement);
            //rangeColorPicker.Dock = DockStyle.Top;
            //pnlWLDWinRanges.Controls.Add(rangeColorPicker);
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

        private void chkIPRedCard_CheckedChanged(object sender, EventArgs e)
        {
            rbnIPEqualRedCards.Enabled = rbnIPNoRedCards.Enabled = rbnIPTeamAMoreRed.Enabled = rbnIPTeamBMoreRed.Enabled = chkIPRedCard.Checked;
        }

        private void btnCommonTradeConfig_Click(object sender, EventArgs e)
        {
            using (frmLocalConfig dlg = new frmLocalConfig())
            {
                dlg.Configuration = _element.TradeConfig;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _element.TradeConfig = dlg.Configuration;
                }
            }
        }
    }
}
