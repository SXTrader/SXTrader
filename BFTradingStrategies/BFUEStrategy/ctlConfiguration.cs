using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.controls
{
    public partial class ctlConfiguration : UserControl, IConfiguration
    {
        LTDConfigurationRW m_config;
        public ctlConfiguration()
        {
            InitializeComponent();
            doLanguage();

            m_config = new LTDConfigurationRW();
            spnPlaytime.Value = m_config.ExitWatchActivationPlaytime;
            spnOdds.Value = (decimal)m_config.ExitCloseOdds;
            spnSeconds.Value = m_config.CheckExitOddsSeconds;
            spnGoalWaitSeconds.Value = m_config.GoalScoredCloseTradeSeconds;
            spnTicks.Value = (decimal)m_config.DontCloseOdds;
            spnTradeCloseRetry.Value = m_config.DontCloseRetrySeconds;
            spnTradeCancelSeconds.Value = m_config.UnmatchedWaitSeconds;
            spnDontCloseDraw.Value = (decimal)m_config.DontCloseDrawPlaytime;
            cbxActivated.Checked = m_config.StrategyActivated;
            chkScoreline00.Checked = m_config.Check00StoppLoss;
            spnSL00SLPercentage.Value = (decimal)m_config.Win00Percentage;
            if (m_config.No00StoppLoss)
                rbnSL00NoStopLoss.Checked = true;
            else
                rbnSL00StopLoss.Checked = true;
            spnSL00SLBetPercent.Value = (decimal)m_config.StoppLoss00BetPercentage;
            spnNoStoppLossOdds.Value = (decimal)m_config.NoStoppLossOdds;
            spnDontCloseDrawOdds.Value = (decimal)m_config.DontCloseTradeDrawOdds;
           

            /////////////////////////////////////////////////
            //// Töne
            /////////////////////////////////////////////////
            cbxPlaySounds.Checked = m_config.PlaySounds;
            cbxPlayMatchAdded.Checked = m_config.PlayMatchAdded;
            txtMatchAdded.Text = m_config.FileMatchAdded;
            cbxPlayGameEnded.Checked = m_config.PlayGameEnded;
            txtGameEnded.Text = m_config.FileGameEnded;
            cbxPlayScoreChanged.Checked = m_config.PlayScoreChanged;
            txtScoreChanged.Text = m_config.FileScoreChanged;
            cbxPlayTradingChanged.Checked = m_config.PlayTradingChanged;
            txtTradingChanged.Text = m_config.FileTradingChanged;

            ///////////////////////////////////////////////////////////
            //// Fast Lay
            ///////////////////////////////////////////////////////////
            if (m_config.FastBetFixedAmount)
                rbnFixedBetAmount.Checked = true;
            else
                rbnPercentBetAmount.Checked = true;

            spnFastBetFixedValue.Value = (decimal)m_config.FastBetFixedAmountValue;
            spnFastBetPercentValue.Value = (decimal)m_config.FastBetPercentAmountValue;
            if (m_config.FastBetPercentTotalAmount)
                rbnPercentageTotalAmount.Checked = true;
            else
                rbnPercentageAvailibleAmount.Checked = true;


            if (m_config.FastBetUnmatchedCancel)
                rbnFastBetCancelUnmatched.Checked = true;
            else
                rbnFastBetUnmatchedWait.Checked = true;

            spnFastBetUnmatchedWaitSeconds.Value = (decimal)m_config.FastBetUnmatchedWaitSeconds;
        }

        public LTDConfigurationRW Configuration
        {
            get
            {
                m_config.ExitWatchActivationPlaytime = (int)spnPlaytime.Value;
                m_config.ExitCloseOdds = (double)spnOdds.Value;
                m_config.CheckExitOddsSeconds = (int)spnSeconds.Value;
                m_config.GoalScoredCloseTradeSeconds = (int)spnGoalWaitSeconds.Value;
                m_config.DontCloseOdds = (double)spnTicks.Value;
                m_config.DontCloseRetrySeconds = (int)spnTradeCloseRetry.Value;
                m_config.UnmatchedWaitSeconds = (int)spnTradeCancelSeconds.Value;
                m_config.DontCloseDrawPlaytime = (int)spnDontCloseDraw.Value;
                m_config.StrategyActivated = cbxActivated.Checked;
                m_config.Check00StoppLoss = chkScoreline00.Checked;
                m_config.Win00Percentage = (int)spnSL00SLPercentage.Value;
                m_config.No00StoppLoss = rbnSL00NoStopLoss.Checked;
                m_config.StoppLoss00BetPercentage = (int)spnSL00SLBetPercent.Value;
                m_config.NoStoppLossOdds = (double)spnNoStoppLossOdds.Value;
                m_config.DontCloseTradeDrawOdds = (double)spnDontCloseDrawOdds.Value;

                //////////////////////////////////////////////////
                //// Töne
                //////////////////////////////////////////////////
                m_config.PlaySounds = cbxPlaySounds.Checked;
                m_config.PlayMatchAdded = cbxPlayMatchAdded.Checked;
                m_config.FileMatchAdded = txtMatchAdded.Text;
                m_config.PlayGameEnded = cbxPlayGameEnded.Checked;
                m_config.FileGameEnded = txtGameEnded.Text;
                m_config.PlayScoreChanged = cbxPlayScoreChanged.Checked;
                m_config.FileScoreChanged = txtScoreChanged.Text;
                m_config.PlayTradingChanged = cbxPlayTradingChanged.Checked;
                m_config.FileTradingChanged = txtTradingChanged.Text;

                ///////////////////////////////////////////////////////////
                //// Fast Bet
                //////////////////////////////////////////////////////////
                m_config.FastBetFixedAmount = rbnFixedBetAmount.Checked;
                m_config.FastBetFixedAmountValue = (double)spnFastBetFixedValue.Value;
                m_config.FastBetPercentAmountValue = (int)spnFastBetPercentValue.Value;
                m_config.FastBetPercentTotalAmount = rbnPercentageTotalAmount.Checked;

                m_config.FastBetUnmatchedCancel = rbnFastBetCancelUnmatched.Checked;
                m_config.FastBetUnmatchedWaitSeconds = (int)spnFastBetUnmatchedWaitSeconds.Value;
                return m_config;
            }
        }

        public ctlConfiguration(LTDConfigurationRW config)
        {
            InitializeComponent();
            doLanguage();
            //m_config = config;
            m_config = new LTDConfigurationRW(config);
            spnPlaytime.Value = m_config.ExitWatchActivationPlaytime;
            spnOdds.Value = (decimal)m_config.ExitCloseOdds;
            spnSeconds.Value = m_config.CheckExitOddsSeconds;
            spnGoalWaitSeconds.Value = m_config.GoalScoredCloseTradeSeconds;
            spnTicks.Value = (decimal)m_config.DontCloseOdds;
            spnTradeCloseRetry.Value = m_config.DontCloseRetrySeconds;
            spnTradeCancelSeconds.Value = m_config.UnmatchedWaitSeconds;
            spnDontCloseDraw.Value = (decimal)m_config.DontCloseDrawPlaytime;
            cbxActivated.Checked = m_config.StrategyActivated;
            chkScoreline00.Checked = m_config.Check00StoppLoss;
            spnSL00SLPercentage.Value = (decimal)m_config.Win00Percentage;
            if (m_config.No00StoppLoss)
                rbnSL00NoStopLoss.Checked = true;
            else
                rbnSL00StopLoss.Checked = true;
            spnSL00SLBetPercent.Value = (decimal)m_config.StoppLoss00BetPercentage;
            spnNoStoppLossOdds.Value = (decimal)m_config.NoStoppLossOdds;
            spnDontCloseDrawOdds.Value = (decimal)m_config.DontCloseTradeDrawOdds;

            /////////////////////////////////////////////////
            //// Töne
            /////////////////////////////////////////////////
            cbxPlaySounds.Checked = m_config.PlaySounds;
            cbxPlayMatchAdded.Checked = m_config.PlayMatchAdded;
            txtMatchAdded.Text = m_config.FileMatchAdded;
            cbxPlayGameEnded.Checked = m_config.PlayGameEnded;
            txtGameEnded.Text = m_config.FileGameEnded;
            cbxPlayScoreChanged.Checked = m_config.PlayScoreChanged;
            txtScoreChanged.Text = m_config.FileScoreChanged;
            cbxPlayTradingChanged.Checked = m_config.PlayTradingChanged;
            txtTradingChanged.Text = m_config.FileTradingChanged;

            ///////////////////////////////////////////////////////////
            //// Fast Lay
            ///////////////////////////////////////////////////////////
            if (m_config.FastBetFixedAmount)
                rbnFixedBetAmount.Checked = true;
            else
                rbnPercentBetAmount.Checked = true;

            spnFastBetFixedValue.Value = (decimal)m_config.FastBetFixedAmountValue;
            spnFastBetPercentValue.Value = (decimal)m_config.FastBetPercentAmountValue;
            if (m_config.FastBetPercentTotalAmount)
                rbnPercentageTotalAmount.Checked = true;
            else
                rbnPercentageAvailibleAmount.Checked = true;


            if (m_config.FastBetUnmatchedCancel)
                rbnFastBetCancelUnmatched.Checked = true;
            else
                rbnFastBetUnmatchedWait.Checked = true;

            spnFastBetUnmatchedWaitSeconds.Value = (decimal)m_config.FastBetUnmatchedWaitSeconds;
            


        }

        public Boolean HideSoundTab
        {
            set
            {
                if (value == true)
                {
                    tbpSound.Hide();
                    tbcConfiguration.TabPages.Remove(tbpSound);
                    tbpSound.Dispose();
                }
            }
        }

        private void doLanguage()
        {
            lblFastBetCurrency.Text = SXALBankrollManager.Instance.Currency;           
        }

        #region IConfiguration Member

        public void save()
        {
            m_config.ExitWatchActivationPlaytime = (int)spnPlaytime.Value;
            m_config.ExitCloseOdds = (double)spnOdds.Value;
            m_config.CheckExitOddsSeconds = (int)spnSeconds.Value;
            m_config.GoalScoredCloseTradeSeconds = (int)spnGoalWaitSeconds.Value;
            m_config.DontCloseOdds = (double)spnTicks.Value;
            m_config.DontCloseRetrySeconds = (int)spnTradeCloseRetry.Value;
            m_config.UnmatchedWaitSeconds = (int)spnTradeCancelSeconds.Value;
            m_config.DontCloseDrawPlaytime = (int)spnDontCloseDraw.Value;
            m_config.StrategyActivated = cbxActivated.Checked;
            m_config.Check00StoppLoss = chkScoreline00.Checked;
            m_config.Win00Percentage = (int)spnSL00SLPercentage.Value;
            m_config.No00StoppLoss = rbnSL00NoStopLoss.Checked;
            m_config.StoppLoss00BetPercentage = (int)spnSL00SLBetPercent.Value;
            m_config.NoStoppLossOdds = (double)spnNoStoppLossOdds.Value;
            m_config.DontCloseTradeDrawOdds = (double)spnDontCloseDrawOdds.Value;

            //////////////////////////////////////////////////
            //// Töne
            //////////////////////////////////////////////////
            m_config.PlaySounds = cbxPlaySounds.Checked;
            m_config.PlayMatchAdded = cbxPlayMatchAdded.Checked;
            m_config.FileMatchAdded = txtMatchAdded.Text;
            m_config.PlayGameEnded = cbxPlayGameEnded.Checked;
            m_config.FileGameEnded = txtGameEnded.Text;
            m_config.PlayScoreChanged = cbxPlayScoreChanged.Checked;
            m_config.FileScoreChanged = txtScoreChanged.Text;
            m_config.PlayTradingChanged = cbxPlayTradingChanged.Checked;
            m_config.FileTradingChanged = txtTradingChanged.Text;

            ///////////////////////////////////////////////////////////
            //// Fast Bet
            //////////////////////////////////////////////////////////
            m_config.FastBetFixedAmount = rbnFixedBetAmount.Checked;
            m_config.FastBetFixedAmountValue = (double)spnFastBetFixedValue.Value;
            m_config.FastBetPercentAmountValue= (int)spnFastBetPercentValue.Value;
            m_config.FastBetPercentTotalAmount = rbnPercentageTotalAmount.Checked;

            m_config.FastBetUnmatchedCancel = rbnFastBetCancelUnmatched.Checked;
            m_config.FastBetUnmatchedWaitSeconds = (int)spnFastBetUnmatchedWaitSeconds.Value;

            m_config.save();
        }

        #endregion

        private void cbxPlayMatchAdded_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void btnMatchAdded_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a new OpenFileDialog.
                OpenFileDialog dlg = new OpenFileDialog();

                // Make sure the dialog checks for existence of the 
                // selected file.
                dlg.CheckFileExists = true;

                // Allow selection of .wav files only.
                dlg.Filter = "WAV files (*.wav)|*.wav";
                dlg.DefaultExt = ".wav";

                // Activate the file selection dialog.
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file's path from the dialog.
                    txtMatchAdded.Text = dlg.FileName;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnGameEnded_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a new OpenFileDialog.
                OpenFileDialog dlg = new OpenFileDialog();

                // Make sure the dialog checks for existence of the 
                // selected file.
                dlg.CheckFileExists = true;

                // Allow selection of .wav files only.
                dlg.Filter = "WAV files (*.wav)|*.wav";
                dlg.DefaultExt = ".wav";

                // Activate the file selection dialog.
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file's path from the dialog.
                    txtGameEnded.Text = dlg.FileName;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnScoreChanged_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a new OpenFileDialog.
                OpenFileDialog dlg = new OpenFileDialog();

                // Make sure the dialog checks for existence of the 
                // selected file.
                dlg.CheckFileExists = true;

                // Allow selection of .wav files only.
                dlg.Filter = "WAV files (*.wav)|*.wav";
                dlg.DefaultExt = ".wav";

                // Activate the file selection dialog.
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file's path from the dialog.
                    txtScoreChanged.Text = dlg.FileName;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnTradingChanged_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a new OpenFileDialog.
                OpenFileDialog dlg = new OpenFileDialog();

                // Make sure the dialog checks for existence of the 
                // selected file.
                dlg.CheckFileExists = true;

                // Allow selection of .wav files only.
                dlg.Filter = "WAV files (*.wav)|*.wav";
                dlg.DefaultExt = ".wav";

                // Activate the file selection dialog.
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file's path from the dialog.
                    txtTradingChanged.Text = dlg.FileName;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void spnNoStoppLossOdds_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spnNoStoppLossOdds.InvokeRequired)
                {
                    IAsyncResult result = spnNoStoppLossOdds.BeginInvoke(new EventHandler<EventArgs>(spnNoStoppLossOdds_ValueChanged), new object[] { sender, e });
                    spnNoStoppLossOdds.EndInvoke(result);
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


        private void spnDontCloseDrawOdds_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spnDontCloseDrawOdds.InvokeRequired)
                {
                    IAsyncResult result = spnDontCloseDrawOdds.BeginInvoke(new EventHandler<EventArgs>(spnDontCloseDrawOdds_ValueChanged), new object[] { sender, e });
                    spnDontCloseDrawOdds.EndInvoke(result);
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

        #region IConfiguration Member


        public string getXML()
        {
            return String.Empty;
        }

        #endregion

       

        private void lblDonCloseOddsDraw_Click(object sender, EventArgs e)
        {

        }

       
    }
}
