using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.tippsters.Configration;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.tippsters.MailInterface;
using net.sxtrader.muk.Exceptions.MailInterface;

namespace net.sxtrader.bftradingstrategies.tippsters.GUI.Configuration
{
    public partial class ctlConfiguration : UserControl, IConfiguration
    {
        TLLConfigurationRW _config;
        public ctlConfiguration()
        {
            _config = new TLLConfigurationRW();
            InitializeComponent();
            try
            {
                txtMailHost.Text = _config.MailHost;
                spnIMAPPort.Value = (decimal)_config.MailPort;
                txtMailUser.Text = _config.MailUser;
                txtMailAccess.Text = _config.MailAccess;
                spnHours.Value = (decimal)_config.MailCheckInterval;
                cbxSSL.Checked = _config.UseSSL;

                spnMaxOdds.Value = (decimal)_config.MaximumLayOdd;
                spnStartMinute.Value = (decimal)_config.BeginTime;
                chkInRunning.Checked = _config.KeepInRunning;
                chkPlaceForMaxOdds.Checked = _config.PlaceForMax;
                chkNonRunner.Checked = _config.NonStarterObservation;
                spn1To5.Value = (decimal)_config.NonRunnerCount1;
                spn6To10.Value = (decimal)_config.NonRunnerCount2;
                spn11To15.Value = (decimal)_config.NonRunnerCount3;
                spn16To20.Value = (decimal)_config.NonRunnerCount4;
                spnMoreThan20.Value = (decimal)_config.NonRunnerCount5;
                chkDynamicBetting.Checked = _config.UseStepTrading;
                spnTicks.Value = (decimal)_config.StepTradingGap;
                chkLastBet.Checked = _config.UseFinalTime;
                spnLastTime.Value = (decimal)_config.FinalTime;

                lblCurrency.Text = SXALKom.Instance.getCurrency();
                spnTippAmount.Value = (decimal)_config.BetAmount;
                rbnAsBetAmount.Checked = _config.BetType;

                chkActive.Checked = _config.StrategyActive;

                lnkSubscription.Links.Add(0, lnkSubscription.Text.Length, "http://14ba1lae42i91s8eqqmwzekobs.hop.clickbank.net/?tid=SXTRADERCLIENT");
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void IConfiguration.save()
        {
            _config.MailHost = txtMailHost.Text;
            _config.MailPort = (int)spnIMAPPort.Value;
            _config.MailUser = txtMailUser.Text;
            _config.MailAccess = txtMailAccess.Text;
            _config.MailCheckInterval = (int)spnHours.Value;
            _config.UseSSL = cbxSSL.Checked;

            _config.MaximumLayOdd = (double)spnMaxOdds.Value;
            _config.BeginTime = (int)spnStartMinute.Value;
            _config.KeepInRunning = chkInRunning.Checked;
            _config.PlaceForMax = chkPlaceForMaxOdds.Checked;
            _config.NonStarterObservation = chkNonRunner.Checked;
            _config.NonRunnerCount1 = (int)spn1To5.Value;
            _config.NonRunnerCount2 = (int)spn6To10.Value;
            _config.NonRunnerCount3 = (int)spn11To15.Value;
            _config.NonRunnerCount4 = (int)spn16To20.Value;
            _config.NonRunnerCount5 = (int)spnMoreThan20.Value;
            _config.UseStepTrading = chkDynamicBetting.Checked;
            _config.StepTradingGap = (int)spnTicks.Value;
            _config.UseFinalTime = chkLastBet.Checked;
            _config.FinalTime = (int)spnLastTime.Value;

            _config.BetAmount = (double)spnTippAmount.Value;
            _config.BetType = rbnAsBetAmount.Checked;

            _config.StrategyActive = chkActive.Checked;

            _config.save();
        }

        string IConfiguration.getXML()
        {
            if (_config != null)
                return _config.ToString();

            return String.Empty;
            //throw new NotImplementedException();
        }

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                DebugWriter.Instance.WriteMessage("TLL - ctlConfiguration", "Disposing");
                if (_config != null)
                {
                    _config.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void lnkSubscription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                TLLMails.TestSettings(txtMailHost.Text, (int)spnIMAPPort.Value, cbxSSL.Checked, txtMailUser.Text, txtMailAccess.Text);
                MessageBox.Show(TheLowLay.strTestMailSuccess, TheLowLay.strTestMailSuccessCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (GenericImapException)
            {
                MessageBox.Show(TheLowLay.strDonnotConnect, TheLowLay.strTestMailFailedCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IMAPLogInFailedException)
            {
                MessageBox.Show(TheLowLay.strMailLoginFailed, TheLowLay.strTestMailFailedCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception Exc)
            {
                MessageBox.Show(Exc.Message, TheLowLay.strTestMailFailedCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
