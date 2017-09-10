using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj
{
    public partial class ctlIPTElementEdit : UserControl
    {
        BFUEFBIPTraderConfigElement _configElement;

        public event EventHandler<BFUEIPTElementSaveEventArgs> SaveIPTConfigElement;

        public ctlIPTElementEdit()
        {
            InitializeComponent();            
            spnOddsHigh.Enabled = spnOddsLow.Enabled = spnPlaytimeHigh.Enabled = spnPlaytimeLow.Enabled = false;
            spnVolumeLow.Enabled = spnVolumeHigh.Enabled = false;
            chkOneOne.Enabled = chkThreeThree.Enabled = chkTwoTwo.Enabled = chkZeroZero.Enabled = false;
            rbnSettledNo.Enabled = rbnSettledYes.Enabled = rbnUnsettledNo.Enabled = rbnUnsettledYes.Enabled = false;
            btnTradeConfig.Enabled = false;
            chkAlreadyTraded.Enabled = false;
            cbxRedCard.Enabled = false;
            rbnNoRedCards.Enabled = rbnEqualRedCards.Enabled = rbnTeamAMoreRed.Enabled = rbnTeamBMoreRed.Enabled = false;
            btnSave.Enabled = btnCancel.Enabled = false;
        }

        public ctlIPTElementEdit(BFUEFBIPTraderConfigElement element)
        {
            InitializeComponent();
            IPTConfigElement = element;
        }

        public BFUEFBIPTraderConfigElement IPTConfigElement
        {
            get
            {
                return _configElement;
            }
            set
            {
                try
                {
                    spnPlaytimeLow.Value = (decimal)0;
                    spnPlaytimeHigh.Value = (decimal)0;
                    spnVolumeLow.Value = 0;
                    spnVolumeHigh.Value = 10000000;
                    chkZeroZero.Checked = chkOneOne.Checked = chkTwoTwo.Checked = chkThreeThree.Checked = false;
                    spnOddsLow.Value = spnOddsHigh.Value = (decimal)1.01;
                    rbnSettledNo.Checked = false;
                    rbnUnsettledYes.Checked = false;
                    chkAlreadyTraded.Checked = false;

                    _configElement = value;
                    if (_configElement != null)
                    {
                        spnOddsHigh.Enabled = spnOddsLow.Enabled = spnPlaytimeHigh.Enabled = spnPlaytimeLow.Enabled = true;
                        spnVolumeLow.Enabled = spnVolumeHigh.Enabled = true;
                        chkOneOne.Enabled = chkThreeThree.Enabled = chkTwoTwo.Enabled = chkZeroZero.Enabled = true;
                        rbnSettledNo.Enabled = rbnSettledYes.Enabled = rbnUnsettledNo.Enabled = rbnUnsettledYes.Enabled = true;
                        btnSave.Enabled = btnCancel.Enabled = btnTradeConfig.Enabled = true;
                        chkAlreadyTraded.Enabled = true;
                        cbxRedCard.Enabled = true;

                        spnPlaytimeLow.Value = _configElement.LoPlaytime;
                        spnPlaytimeHigh.Value = _configElement.HiPlayTime;

                        foreach (IPTScores score in _configElement.Scores)
                        {
                            switch (score)
                            {
                                case IPTScores.ZEROZERO:
                                    chkZeroZero.Checked = true;
                                    break;
                                case IPTScores.ONEONE:
                                    chkOneOne.Checked = true;
                                    break;
                                case IPTScores.TWOTWO:
                                    chkTwoTwo.Checked = true;
                                    break;
                                case IPTScores.THREETHREE:
                                    chkThreeThree.Checked = true;
                                    break;
                                default:
                                    throw new Exception();
                            }
                        }

                        spnOddsLow.Value = (decimal)_configElement.LoOdds;
                        spnOddsHigh.Value = (decimal)_configElement.HiOdds;

                        spnVolumeLow.Value = (decimal)_configElement.LoVolume;
                        spnVolumeHigh.Value = (decimal)_configElement.HiVolume;

                        rbnSettledYes.Checked = _configElement.SettledAllowed;
                        rbnSettledNo.Checked = !_configElement.SettledAllowed;
                        rbnUnsettledYes.Checked = _configElement.UnsettledAllowed;
                        rbnUnsettledNo.Checked = !_configElement.UnsettledAllowed;

                        chkAlreadyTraded.Checked = _configElement.AlreadyTraded;

                        cbxRedCard.Checked = _configElement.RedCardWatch;

                        if (cbxRedCard.Checked)
                            rbnNoRedCards.Enabled = rbnEqualRedCards.Enabled = rbnTeamAMoreRed.Enabled = rbnTeamBMoreRed.Enabled = true;
                        switch (_configElement.RedCardWatchState)
                        {
                            case IPTRedCardWatch.NOREDCARD:
                                {
                                    rbnNoRedCards.Checked = true;
                                    break;
                                }
                            case IPTRedCardWatch.EQUALCARD:
                                {
                                    rbnEqualRedCards.Checked = true;
                                    break;
                                }
                            case IPTRedCardWatch.TEAMAMORE:
                                {
                                    rbnTeamAMoreRed.Checked = true;
                                    break;
                                }
                            case IPTRedCardWatch.TEAMBMORE:
                                {
                                    rbnTeamBMoreRed.Checked = true;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        spnOddsHigh.Enabled = spnOddsLow.Enabled = spnPlaytimeHigh.Enabled = spnPlaytimeLow.Enabled = false;
                        spnVolumeLow.Enabled = spnVolumeHigh.Enabled = false;
                        chkOneOne.Enabled = chkThreeThree.Enabled = chkTwoTwo.Enabled = chkZeroZero.Enabled = false;
                        rbnSettledNo.Enabled = rbnSettledYes.Enabled = rbnUnsettledNo.Enabled = rbnUnsettledYes.Enabled = false;
                        btnSave.Enabled = btnCancel.Enabled = btnTradeConfig.Enabled = false;
                        chkAlreadyTraded.Enabled = false;
                        cbxRedCard.Enabled = false;
                        rbnNoRedCards.Enabled = rbnEqualRedCards.Enabled = rbnTeamAMoreRed.Enabled = rbnTeamBMoreRed.Enabled = false;
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private bool checkValues(ref String msg)
        {
            msg = String.Empty;
            if (spnPlaytimeHigh.Value < spnPlaytimeLow.Value)
            {
                msg = LayTheDraw.strPlayTimeHighLowMismatch;
            }

            if (spnOddsHigh.Value < spnOddsLow.Value)
            {
                if (msg != String.Empty)
                    msg += "\r\n";
                msg += LayTheDraw.strOddsHighLowMismatch;
            }

            if (spnVolumeHigh.Value < spnVolumeLow.Value)
            {
                if (msg != String.Empty)
                    msg += "\r\n";
                msg += LayTheDraw.strMarketVolumeHighLowMismatch;
            }

            if (msg != String.Empty)
                return false;
            else
                return true;
        }

        private void spnOddsLow_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spnOddsLow.InvokeRequired)
                {
                    IAsyncResult result = spnOddsLow.BeginInvoke(new EventHandler<EventArgs>(spnOddsLow_ValueChanged), new object[] { sender, e });
                    spnOddsLow.EndInvoke(result);
                }
                else
                {
                    NumericUpDown o = (NumericUpDown)sender;
                    // Nächstes incr bestimmen
                    o.Increment = SXALKom.Instance.getValidOddIncrement(o.Value);
                    o.Value = SXALKom.Instance.validateOdd(o.Value);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void spnOddsHigh_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spnOddsHigh.InvokeRequired)
                {
                    IAsyncResult result = spnOddsHigh.BeginInvoke(new EventHandler<EventArgs>(spnOddsHigh_ValueChanged), new object[] { sender, e });
                    spnOddsHigh.EndInvoke(result);
                }
                else
                {
                    NumericUpDown o = (NumericUpDown)sender;
                    // Nächstes incr bestimmen
                    o.Increment = SXALKom.Instance.getValidOddIncrement(o.Value);
                    o.Value = SXALKom.Instance.validateOdd(o.Value);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnNew.InvokeRequired)
                {
                    IAsyncResult result = btnNew.BeginInvoke(new EventHandler<EventArgs>(btnNew_Click), new object[] { sender, e });
                    btnNew.EndInvoke(result);
                }
                else
                {
                    this.IPTConfigElement = null;
                    BFUEFBIPTraderConfigElement newElement = BFUEFBIPTraderConfigElement.createNew();
                    this.IPTConfigElement = newElement;
                    spnOddsHigh.Enabled = spnOddsLow.Enabled = spnPlaytimeHigh.Enabled = spnPlaytimeLow.Enabled = true;
                    spnVolumeLow.Enabled = spnVolumeHigh.Enabled = true;
                    chkOneOne.Enabled = chkThreeThree.Enabled = chkTwoTwo.Enabled = chkZeroZero.Enabled = true;
                    rbnSettledNo.Enabled = rbnSettledYes.Enabled = rbnUnsettledNo.Enabled = rbnUnsettledYes.Enabled = true;
                    btnSave.Enabled = btnCancel.Enabled = btnTradeConfig.Enabled = true;
                    chkAlreadyTraded.Enabled = true;
                    cbxRedCard.Enabled = true;
                    if (cbxRedCard.Checked)
                        rbnNoRedCards.Enabled = rbnEqualRedCards.Enabled = rbnTeamAMoreRed.Enabled = rbnTeamBMoreRed.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.InvokeRequired)
                {
                    IAsyncResult result = btnSave.BeginInvoke(new EventHandler<EventArgs>(btnSave_Click), new object[] { sender, e });
                    btnSave.EndInvoke(result);
                }
                else
                {
                    if (_configElement != null)
                    {
                        String msg = String.Empty;
                        if (!checkValues(ref msg))
                        {
                            MessageBox.Show(msg, "Invalid Values", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _configElement.LoPlaytime = (int)spnPlaytimeLow.Value;
                        _configElement.HiPlayTime = (int)spnPlaytimeHigh.Value;

                        if (chkZeroZero.Checked)
                        {
                            _configElement.addScore(IPTScores.ZEROZERO);
                        }
                        else
                        {
                            _configElement.removeScore(IPTScores.ZEROZERO);
                        }

                        if (chkOneOne.Checked)
                        {
                            _configElement.addScore(IPTScores.ONEONE);
                        }
                        else
                        {
                            _configElement.removeScore(IPTScores.ONEONE);
                        }

                        if (chkTwoTwo.Checked)
                        {
                            _configElement.addScore(IPTScores.TWOTWO);
                        }
                        else
                        {
                            _configElement.removeScore(IPTScores.TWOTWO);
                        }

                        if (chkThreeThree.Checked)
                        {
                            _configElement.addScore(IPTScores.THREETHREE);
                        }
                        else
                        {
                            _configElement.removeScore(IPTScores.THREETHREE);
                        }

                        _configElement.LoOdds = (double)spnOddsLow.Value;
                        _configElement.HiOdds = (double)spnOddsHigh.Value;

                        _configElement.LoVolume = (long)spnVolumeLow.Value;
                        _configElement.HiVolume = (long)spnVolumeHigh.Value;

                        _configElement.SettledAllowed = rbnSettledYes.Checked;
                        _configElement.UnsettledAllowed = rbnUnsettledYes.Checked;

                        _configElement.AlreadyTraded = chkAlreadyTraded.Checked;

                        _configElement.RedCardWatch = cbxRedCard.Checked;

                        if (_configElement.RedCardWatch)
                        {
                            if (rbnNoRedCards.Checked)
                                _configElement.RedCardWatchState = IPTRedCardWatch.NOREDCARD;
                            else if (rbnEqualRedCards.Checked)
                                _configElement.RedCardWatchState = IPTRedCardWatch.EQUALCARD;
                            else if (rbnTeamAMoreRed.Checked)
                                _configElement.RedCardWatchState = IPTRedCardWatch.TEAMAMORE;
                            else if (rbnTeamBMoreRed.Checked)
                                _configElement.RedCardWatchState = IPTRedCardWatch.TEAMBMORE;
                            else
                                _configElement.RedCardWatchState = IPTRedCardWatch.UNASSIGNED;
                        }

                        EventHandler<BFUEIPTElementSaveEventArgs> saveHandler = SaveIPTConfigElement;
                        if (saveHandler != null)
                        {
                            saveHandler(this, new BFUEIPTElementSaveEventArgs(this.IPTConfigElement));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnCancel.InvokeRequired)
                {
                    IAsyncResult result = btnCancel.BeginInvoke(new EventHandler<EventArgs>(btnCancel_Click), new object[] { sender, e });
                    btnCancel.EndInvoke(result);
                }
                else
                {
                    this.IPTConfigElement = null;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnTradeConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnTradeConfig.InvokeRequired)
                {
                    IAsyncResult result = btnTradeConfig.BeginInvoke(new EventHandler<EventArgs>(btnTradeConfig_Click), new object[] { sender, e });
                    btnTradeConfig.EndInvoke(result);
                }
                else
                {
                    using (frmLocalConfig dlg = new frmLocalConfig())
                    {
                        dlg.Configuration = _configElement.TradeConfig;
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            _configElement.TradeConfig = dlg.Configuration;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void cbxRedCard_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbxRedCard.InvokeRequired)
                {
                    IAsyncResult result = cbxRedCard.BeginInvoke(new EventHandler<EventArgs>(cbxRedCard_CheckedChanged), new object[] { sender, e });
                    cbxRedCard.EndInvoke(result);
                }
                else
                {
                    rbnNoRedCards.Enabled = rbnEqualRedCards.Enabled = rbnTeamAMoreRed.Enabled = rbnTeamBMoreRed.Enabled = cbxRedCard.Checked;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
