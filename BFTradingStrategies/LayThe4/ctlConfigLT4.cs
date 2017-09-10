using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;

namespace net.sxtrader.bftradingstrategies.LayThe4
{
    public partial class ctlConfigLT4 : UserControl, IConfiguration
    {
        LT4ConfigurationRW m_config;

        public ctlConfigLT4()
        {
            InitializeComponent();            
            doLanguage();
            m_config = new LT4ConfigurationRW();
            tbpAutomatic.Hide();
            loadConfig();
            tbcConfig.TabPages.Remove(tbpAutomatic);
        }

        public ctlConfigLT4(LT4ConfigurationRW config)
        {
            InitializeComponent();
            tbpAutomatic.Hide();
            doLanguage();
            m_config = new LT4ConfigurationRW(config);
            loadConfig();
            tbcConfig.TabPages.Remove(tbpAutomatic);
        }

        public LT4ConfigurationRW Configuration
        {
            get
            {
                m_config.AutomaticTrading = cbxAutoTrade.Checked;
                m_config.MinOdds = (double)spnLay4MinOdds.Value;
                m_config.MaxOdds = (double)spnLay4OddsMax.Value;
                m_config.PercentBank = (double)spnPercentBank.Value;
                m_config.MinAmount = (double)spnAutoBetMinimum.Value;
                m_config.MaxAmount = (double)spnAutobetMaximum.Value;
                m_config.PlaceLower = rbnBetLower.Checked;
                m_config.MarketVolume = (int)spnMarketVolume.Value;
                m_config.StrategyActivated = cbxDefaultActivation.Checked;
                m_config.StartPlaytime = (int)spnExitStartMinute.Value;
                m_config.NoProfitWait = (int)spnSecondsWaitNoWin.Value;
                m_config.ProfitPercent = (double)spnProfit.Value;
                m_config.ProfitWait = (int)spnProfitWaitSeconds.Value;
                m_config.Goals = (int)spnGoals.Value;
                m_config.ExitSeconds = (int)spnCloseSeconds.Value;
                m_config.NoProfit = rbnNoProfitWait.Checked;
                m_config.Profit = rbnProfitWait.Checked;
                m_config.TradeStart = (int)spnTradStart.Value;
                m_config.ActivePassive = rbnActive.Checked;

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
            cbxAutoTrade.Checked = m_config.AutomaticTrading;
            spnLay4MinOdds.Value = (decimal)m_config.MinOdds;
            spnLay4OddsMax.Value = (decimal)m_config.MaxOdds;
            spnPercentBank.Value = (decimal)m_config.PercentBank;
            spnAutoBetMinimum.Value = (decimal)m_config.MinAmount;
            spnAutobetMaximum.Value = (decimal)m_config.MaxAmount;
            if (m_config.PlaceLower)
                rbnBetLower.Checked = true;
            else
                rbnDontPlace.Checked = true;
            spnMarketVolume.Value = m_config.MarketVolume;
            cbxDefaultActivation.Checked = m_config.StrategyActivated;
            spnExitStartMinute.Value = m_config.StartPlaytime;
            spnSecondsWaitNoWin.Value = m_config.NoProfitWait;
            spnProfit.Value = (decimal)m_config.ProfitPercent;
            spnProfitWaitSeconds.Value = m_config.ProfitWait;
            spnGoals.Value = m_config.Goals;
            spnCloseSeconds.Value = m_config.ExitSeconds;
            if (m_config.ActivePassive)
                rbnActive.Checked = true;
            else
                rbnPassive.Checked = true;

            if (m_config.NoProfit)
                rbnNoProfitWait.Checked = true;
            else
                rbnCloseNoWin.Checked = true;
            if (m_config.Profit)
                rbnProfitWait.Checked = true;
            else
                rbnCloseTradeProfit.Checked = true;
            spnTradStart.Value = m_config.TradeStart;

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
            // General
            gbxConfig.Text = LayThe4.strConfigLT4;
            tbpAutomatic.Text = LayThe4.strAutoTrade;
            tbpStrategy.Text = LayThe4.strStrategyContrl;

            // Automatic Trading Flag
            cbxAutoTrade.Text = LayThe4.strAutoTradeQ;

            // Quotenintervall für Automatic Trading
            lblAutoOdds.Text = LayThe4.strAutoOdds;
            lblAnd.Text = LayThe4.strAnd;
            spnLay4MinOdds.Left = lblAutoOdds.Right + 5;
            lblAnd.Left = spnLay4MinOdds.Right + 5;
            spnLay4OddsMax.Left = lblAnd.Right + 5;

            // Betrag für Automatic Trading
            lblPlace.Text = LayThe4.strPlace;
            lblOfBank.Text = LayThe4.strOfBank;
            lblMinimum.Text = LayThe4.strMinimum;
            lblAutobetMaximum.Text = LayThe4.strMaximum;
            spnPercentBank.Left = lblPlace.Right + 5;
            lblOfBank.Left = spnPercentBank.Right + 5;
            lblMinimum.Left = lblOfBank.Right + 5;
            spnAutoBetMinimum.Left = lblMinimum.Right + 5;
            lblAutobetMaximum.Left = spnAutoBetMinimum.Right + 5;
            spnAutobetMaximum.Left = lblAutobetMaximum.Right + 5;

            //Verfügbarer Betrag zu gering
            gbxTooLow.Text = LayThe4.strTooLow;

            rbnBetLower.Text = LayThe4.strBetLower;
            rbnDontPlace.Text = LayThe4.strDontPlace;

            //Marktvolumen
            lblMarkets.Text = LayThe4.strMarkets;
            spnMarketVolume.Left = lblMarkets.Right + 5;


            //Default activation
            cbxDefaultActivation.Text = LayThe4.strDefaultActivation;

            //Ausstiegsüberwachung Aktivierungszeit
            lblStartExitWatch.Text = LayThe4.strStartExitWatch;
            lblMinutesOfPlaytime.Text = LayThe4.strMinutesOfPlaytime;

            spnExitStartMinute.Left = lblStartExitWatch.Right + 5;
            lblMinutesOfPlaytime.Left = spnExitStartMinute.Right + 5;

            //Kein Gewinn
            gbxNoWin.Text = LayThe4.strNoProfit;

            rbnNoProfitWait.Text = LayThe4.strWait;
            lblSecondsNoWin.Text = LayThe4.strSeconds;

            spnSecondsWaitNoWin.Left = rbnNoProfitWait.Right + 5;
            lblSecondsNoWin.Left = spnSecondsWaitNoWin.Right + 5;

            rbnCloseNoWin.Text = LayThe4.strCloseTrade;

            //Gewinn
            gbxWin.Text = LayThe4.strProfit;
            rbnProfitWait.Text = LayThe4.strProfitLower;
            lblProfitWait.Text = LayThe4.strProfitWait;
            lblProfitMinutes.Text = LayThe4.strSeconds;

            spnProfit.Left = rbnProfitWait.Right + 5;
            lblProfitWait.Left = spnProfit.Right + 5;
            spnProfitWaitSeconds.Left = lblProfitWait.Right + 5;
            lblProfitMinutes.Left = spnProfitWaitSeconds.Right + 5;

            //Notausstiegsparametriesierung
            lblAfter.Text = LayThe4.strAfter;
            lblGoalWait.Text = LayThe4.strGoalWait;
            lblSecondsCloseTrade.Text = LayThe4.strSecondsCloseTrades;

            spnGoals.Left = lblAfter.Right + 5;
            lblGoalWait.Left = spnGoals.Right + 5;
            spnCloseSeconds.Left = lblGoalWait.Right + 5;
            lblSecondsCloseTrade.Left = spnCloseSeconds.Right + 5;
            
            // automatische Abschluss Laywette Zeitpunkt
            lblTradeStart.Text = LayThe4.strTradeStart;
            lblMarketInplay.Text = LayThe4.strSecondsMarketInplay;

            spnTradStart.Left = lblTradeStart.Right + 5;
            lblMarketInplay.Left = spnTradStart.Right + 5;

            rbnActive.Text = LayThe4.strActive;
            rbnPassive.Text = LayThe4.strPassive;

            ////////////////////////////////////////////////////
            //// Sound
            ///////////////////////////////////////////////////
            cbxPlaySounds.Text = LayThe4.strPlaySoundsText;

            //Konfiguration Töne Spiel hinzugefügt
            gbxMatchAdded.Text = LayThe4.strMatchAdded;
            cbxPlayMatchAdded.Text = LayThe4.strPlay;
            btnMatchAdded.Text = LayThe4.strBrowse;

            //KOnfiguration Töne Spiel beendet
            gbxGameEnded.Text = LayThe4.strGameEnded;
            cbxPlayGameEnded.Text = LayThe4.strPlay;
            btnGameEnded.Text = LayThe4.strBrowse;

            //Konfiguration Töne Spielstand geändert
            gbxScoreChanged.Text = LayThe4.strScoreChanged;
            cbxPlayScoreChanged.Text = LayThe4.strPlay;
            btnScoreChanged.Text = LayThe4.strBrowse;

            //Konfiguration Töne Trading geändert
            gbxTradingChanged.Text = LayThe4.strTradingChanged;
            cbxPlayTradingChanged.Text = LayThe4.strPlay;
            btnTradingChanged.Text = LayThe4.strBrowse;
        }

        #region IConfiguration Member

        public void save()
        {
            m_config.AutomaticTrading = cbxAutoTrade.Checked;
            m_config.MinOdds = (double)spnLay4MinOdds.Value;
            m_config.MaxOdds = (double)spnLay4OddsMax.Value;
            m_config.PercentBank = (double)spnPercentBank.Value;
            m_config.MinAmount = (double)spnAutoBetMinimum.Value;
            m_config.MaxAmount = (double)spnAutobetMaximum.Value;
            m_config.PlaceLower = rbnBetLower.Checked;
            m_config.MarketVolume = (int)spnMarketVolume.Value;
            m_config.StrategyActivated = cbxDefaultActivation.Checked;
            m_config.StartPlaytime = (int)spnExitStartMinute.Value;
            m_config.NoProfitWait = (int)spnSecondsWaitNoWin.Value;
            m_config.ProfitPercent = (double)spnProfit.Value;
            m_config.ProfitWait = (int)spnProfitWaitSeconds.Value;
            m_config.Goals = (int)spnGoals.Value;
            m_config.ExitSeconds = (int)spnCloseSeconds.Value;
            m_config.NoProfit = rbnNoProfitWait.Checked;
            m_config.Profit = rbnProfitWait.Checked;
            m_config.TradeStart = (int)spnTradStart.Value;
            m_config.ActivePassive = rbnActive.Checked;

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

        public string getXML()
        {
            return String.Empty;
        }

        #endregion
    }
}
