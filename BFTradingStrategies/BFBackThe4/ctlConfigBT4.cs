using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;

namespace net.sxtrader.bftradingstrategies.BackThe4
{
    public partial class ctlConfigBT4 : UserControl, IConfiguration
    {
        BT4ConfigurationRW m_config;


        public ctlConfigBT4()
        {
            InitializeComponent();
            doLanguage();
            m_config = new BT4ConfigurationRW();
            loadConfig();
        }

        public ctlConfigBT4(BT4ConfigurationRW config)
        {
            InitializeComponent();
            doLanguage();
            //m_config = config;
            m_config = new BT4ConfigurationRW(config);
            loadConfig();
        }

        public BT4ConfigurationRW Configuration
        {
            get
            {
                bool value = false;
                m_config.StrategyActivated = (bool)cbxDefaultActivation.Checked;
                m_config.CloseTradeGoals = (int)spnGoals.Value;
                m_config.CloseTradeWaitSeconds = (int)spnWaitSeconds.Value;
                value = cbxNoProfit.Checked;
                m_config.CloseTradeLoss = value;//(bool)cbxNoProfit.Checked;
                m_config.CloseTradeProfit = (double)spnProfit.Value;
                m_config.StopLossPlaytime = (int)spnMinutesStopLoose.Value;
                m_config.StopLossInitFactor = (double)spnOddsTimes.Value;
                m_config.StopLossMax = (double)spnOdds.Value;
                m_config.TradeWatchdogMinutes = (int)spnRestartMinutes.Value;

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

                return m_config;
            }
        }

        private void loadConfig()
        {            
            cbxDefaultActivation.Checked = m_config.StrategyActivated;
            spnGoals.Value = (decimal)m_config.CloseTradeGoals;
            spnWaitSeconds.Value = (decimal)m_config.CloseTradeWaitSeconds;
            cbxNoProfit.Checked = m_config.CloseTradeLoss;
            spnProfit.Value = (decimal)m_config.CloseTradeProfit;
            spnMinutesStopLoose.Value = (decimal)m_config.StopLossPlaytime;
            spnOddsTimes.Value = (decimal)m_config.StopLossInitFactor;
            spnOdds.Value = (decimal)m_config.StopLossMax;
            spnRestartMinutes.Value = (decimal)m_config.TradeWatchdogMinutes;

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

        }

        public Boolean HideSoundTab
        {
            set
            {
                if (value == true)
                {
                    tbpSound.Hide();
                    
                    tbcConfig.TabPages.Remove(tbpSound);
                }
            }
        }

        private void doLanguage()
        {
            gbxConfig.Text = BackThe4.strConfigBT4;
            tbpStrategy.Text = BackThe4.strStrategyControl;

            cbxDefaultActivation.Text = BackThe4.strDefaultActivation;

            gbxGoalBehavior.Text = BackThe4.strCloseTradeBehavior;

            lblStartCloseTradeAfter.Text = BackThe4.strStartClose;
            spnGoals.Left = lblStartCloseTradeAfter.Right + 5;

            lblGoals.Text = BackThe4.strGoals;
            lblGoals.Left = spnGoals.Right + 5;

            lblWait.Text = BackThe4.strWait;
            spnWaitSeconds.Left = lblWait.Right + 5;

            lblBeforeStart.Text = BackThe4.strSecondsBefore;
            lblBeforeStart.Left = spnWaitSeconds.Right + 5;

            cbxNoProfit.Text = BackThe4.strNoProfit;

            lblProfitLower.Text = BackThe4.strProfitLower;
            spnProfit.Left = lblProfitLower.Right + 5;

            lblDontClose.Text = BackThe4.strDontClose;
            lblDontClose.Left = spnProfit.Right + 5;

            gbxStopLoss.Text = BackThe4.strStopLossBehavior;

            lblStopLoose.Text = BackThe4.strStartStopLoss;
            spnMinutesStopLoose.Left = lblStopLoose.Right + 5;
            lblMinutes.Text = BackThe4.strMinutesOfPlaytime;
            lblMinutes.Left = spnMinutesStopLoose.Right + 5;

            lblStopLooseOdds.Text = BackThe4.strStopLossOdds;
            spnOddsTimes.Left = lblStopLooseOdds.Right + 5;
            lblTimes.Text = BackThe4.strTimes;
            lblTimes.Left = spnOddsTimes.Right + 5;

            lblLowerThan.Text = BackThe4.strLowerThan;
            spnOdds.Left = lblLowerThan.Right + 5;
            lblEndBackOdds.Text = BackThe4.strEndBackOdds;
            lblEndBackOdds.Left = spnOdds.Right + 5;

            lblRestartTrade.Text = BackThe4.strRestartTrade;
            spnRestartMinutes.Left = lblRestartTrade.Right + 5;
            lblMinutesTrade.Text = BackThe4.strIsntCompleted;
            lblMinutesTrade.Left = spnRestartMinutes.Right + 5;

            ////////////////////////////////////////////////////
            //// Sound
            ///////////////////////////////////////////////////
            cbxPlaySounds.Text = BackThe4.strPlaySoundsText;

            //Konfiguration Töne Spiel hinzugefügt
            gbxMatchAdded.Text = BackThe4.strMatchAdded;
            cbxPlayMatchAdded.Text = BackThe4.strPlay;
            btnMatchAdded.Text = BackThe4.strBrowse;

            //KOnfiguration Töne Spiel beendet
            gbxGameEnded.Text = BackThe4.strGameEnded;
            cbxPlayGameEnded.Text = BackThe4.strPlay;
            btnGameEnded.Text = BackThe4.strBrowse;

            //Konfiguration Töne Spielstand geändert
            gbxScoreChanged.Text = BackThe4.strScoreChanged;
            cbxPlayScoreChanged.Text = BackThe4.strPlay;
            btnScoreChanged.Text = BackThe4.strBrowse;

            //Konfiguration Töne Trading geändert
            gbxTradingChanged.Text = BackThe4.strTradingChanged;
            cbxPlayTradingChanged.Text = BackThe4.strPlay;
            btnTradingChanged.Text = BackThe4.strBrowse;
        }
        #region IConfiguration Member

        public void save()
        {
            m_config.StrategyActivated = cbxDefaultActivation.Checked;
            m_config.CloseTradeGoals = (int)spnGoals.Value;
            m_config.CloseTradeWaitSeconds = (int)spnWaitSeconds.Value;
            m_config.CloseTradeLoss = cbxNoProfit.Checked;
            m_config.CloseTradeProfit = (double)spnProfit.Value;
            m_config.StopLossPlaytime = (int)spnMinutesStopLoose.Value;
            m_config.StopLossInitFactor = (double)spnOddsTimes.Value;
            m_config.StopLossMax = (double)spnOdds.Value;
            m_config.TradeWatchdogMinutes = (int)spnRestartMinutes.Value;

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

            m_config.save();            
        }

        #endregion

        private void btnMatchAdded_Click(object sender, EventArgs e)
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

        private void btnGameEnded_Click(object sender, EventArgs e)
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

        private void btnScoreChanged_Click(object sender, EventArgs e)
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

        private void btnTradingChanged_Click(object sender, EventArgs e)
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        #region IConfiguration Member


        public string getXML()
        {
            return String.Empty;
        }

        #endregion
    }
}
