using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.GUI.Configuration;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.ttr.TradeRules;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Dialogs;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.tradeinterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls
{
    public partial class ctlTradeOutRuleEditCSBack : ctlTradeOutRuleEditAbstract
    {
        public override TTRTradeOutCheck TradeOutCheck
        {
            get
            {
                String str = cbxTrigger.SelectedValue.ToString();
                _tradeOutCheck.Trigger = (TRADEOUTTRIGGER)Enum.Parse(typeof(TRADEOUTTRIGGER), str);
                _tradeOutCheck.Order = (int)spnCheckOrder.Value;

                //Überprüfungsregeln                
                getPlaytimeRule(chkCheckPlaytime.Checked, (int)spnPlaytimeHi.Value, (int)spnPlaytimeLo.Value);
                getRedCardRule(rbnRCEqual.Checked, rbnRCTeamA.Checked, rbnRCTeamB.Checked);

                // Korrekt Score besitzt kein GoalSum
                getGoalSumRule(false, 0, 0);                
                ScoreList scoreList = new ScoreList();
                if (btnScores.Tag != null)
                {
                    scoreList = btnScores.Tag as ScoreList;
                }
                getScoreRule(chkScore.Checked, scoreList);

                //Trade Out Settings
                getTradeOutSettings();

                return _tradeOutCheck;
            }

            set
            {
                _tradeOutCheck = value;
                if (_tradeOutCheck != null)
                {
                    cbxTrigger.SelectedValue = _tradeOutCheck.Trigger;
                    if (_tradeOutCheck.Order == 0)
                    {
                        _tradeOutCheck.Order = 10;
                    }
                    spnCheckOrder.Value = _tradeOutCheck.Order;

                    setGUIEnabled(true);
                   
                    //Überprüfungsregeln                    
                    setPlaytimeRule(chkCheckPlaytime, spnPlaytimeLo, spnPlaytimeHi);
                    setRedCardRule(rbnRCEqual, rbnRCTeamA, rbnRCTeamB, rbnNoRedCard);
                    setScoresRule(chkScore, btnScores);
                    buildScoreBtnText();

                    // Trade Out Settings
                    setTradeOutSettings();
                }
            }
        }

        public ctlTradeOutRuleEditCSBack(TRADETYPE tradeType) : base(tradeType)
        {
            
            InitializeComponent();
            fillTriggerCombobox(cbxTrigger);
            TTRHelper.FillSpecialPlaytimeCombobox(cbxSpecialGreenPT);
            TTRHelper.FillSpecialPlaytimeCombobox(cbxSpecialHedgePT);
            setGUIEnabled(false);
        }

        public ctlTradeOutRuleEditCSBack()
        {
            InitializeComponent();
            fillTriggerCombobox(cbxTrigger);
            TTRHelper.FillSpecialPlaytimeCombobox(cbxSpecialGreenPT);
            TTRHelper.FillSpecialPlaytimeCombobox(cbxSpecialHedgePT);
            setGUIEnabled(false);            
        }       

        private void setGUIEnabled(Boolean isEnabled)
        {
            cbxTrigger.Enabled = spnCheckOrder.Enabled = chkCheckPlaytime.Enabled = spnPlaytimeLo.Enabled = spnPlaytimeHi.Enabled =
                                    chkScore.Enabled = btnScores.Enabled = rbnNoRedCard.Enabled = rbnRCEqual.Enabled =
                                    rbnRCTeamA.Enabled = rbnRCTeamB.Enabled = chkOnlyHedge.Enabled = chkCheckLayOdds.Enabled = chkWaitTimes.Enabled =
                                    chkOddsPercentage.Enabled = chkUseHedgeWaittime.Enabled = chkUseGreenWaittime.Enabled = spnHedgeWaitTime.Enabled =
                                    spnHedgePlaytime.Enabled = spnHedgePercentage.Enabled = spnGreenWaitTime.Enabled = spnGreenPlaytime.Enabled =
                                    spnGreenPercentage.Enabled = chkNoTrade.Enabled = rbnFixedHedgePT.Enabled = spnHedgePlaytime.Enabled =
                                    rbnHedgeDynamic.Enabled = cbxSpecialHedgePT.Enabled = spnHedgeDelta.Enabled = rbnFixedGreenPT.Enabled =
                                    spnGreenPlaytime.Enabled = rbnGreenDynamic.Enabled = cbxSpecialGreenPT.Enabled = spnGreenDelta.Enabled =
                                    isEnabled; 
        }

        private void buildScoreBtnText()
        {
            btnScores.Text = String.Empty;
            String strScores = String.Empty;

            if (btnScores.Tag != null)
            {
                ScoreList scores = btnScores.Tag as ScoreList;
                if (scores != null)
                {
                    foreach (TTRScores score in scores)
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
                }
            }
            btnScores.Text = String.Format("{0} {1}", TradeTheReaction.strScoresASBtn, strScores);
        }

        protected override void getTradeOutSettings()
        {
            _tradeOutCheck.TradeOutSettings.NoTrade                         = chkNoTrade.Checked;
            _tradeOutCheck.TradeOutSettings.CSBackCheckLayOdds              = chkCheckLayOdds.Checked;
            _tradeOutCheck.TradeOutSettings.CSBackGreenPercentage           = (int)spnGreenPercentage.Value;
            _tradeOutCheck.TradeOutSettings.CSBackGreenPlaytime             = (int)spnGreenPlaytime.Value;
            _tradeOutCheck.TradeOutSettings.CSBackGreenSpecialPlayTime      = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), cbxSpecialGreenPT.SelectedValue.ToString());
            _tradeOutCheck.TradeOutSettings.CSBackGreenSpecialPlayTimeDelta = (int)spnGreenDelta.Value;
            _tradeOutCheck.TradeOutSettings.CSBackGreenWaittime             = (int)spnGreenWaitTime.Value;
            _tradeOutCheck.TradeOutSettings.CSBackHedgePercentage           = (int)spnHedgePercentage.Value;
            _tradeOutCheck.TradeOutSettings.CSBackHedgePlayTime             = (int)spnHedgePlaytime.Value;
            _tradeOutCheck.TradeOutSettings.CSBackHedgeSpecialPlayTime      = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), cbxSpecialHedgePT.SelectedValue.ToString());
            _tradeOutCheck.TradeOutSettings.CSBackHedgeSpecialPlayTimeDelta = (int)spnHedgeDelta.Value;
            _tradeOutCheck.TradeOutSettings.CSBackHedgeWaitTime             = (int)spnHedgeWaitTime.Value;
            _tradeOutCheck.TradeOutSettings.CSBackOnlyHedge                 = chkOnlyHedge.Checked;
            _tradeOutCheck.TradeOutSettings.CSBackUseGreenSpecialPlayTime   = rbnGreenDynamic.Checked;
            _tradeOutCheck.TradeOutSettings.CSBackUseGreenWaitTime          = chkUseGreenWaittime.Checked;
            _tradeOutCheck.TradeOutSettings.CSBackUseHedgeSpecialPlayTime   = rbnHedgeDynamic.Checked;
            _tradeOutCheck.TradeOutSettings.CSBackUseHedgeWaitTime          = chkUseHedgeWaittime.Checked;
            _tradeOutCheck.TradeOutSettings.CSBackUseOddsPercentage         = chkOddsPercentage.Checked;
            _tradeOutCheck.TradeOutSettings.CSBackUseWaitTime               = chkWaitTimes.Checked;

        }

        protected override void setTradeOutSettings()
        {
            chkNoTrade.Checked              = _tradeOutCheck.TradeOutSettings.NoTrade;
            chkCheckLayOdds.Checked         = _tradeOutCheck.TradeOutSettings.CSBackCheckLayOdds;
            spnGreenPercentage.Value        = _tradeOutCheck.TradeOutSettings.CSBackGreenPercentage;
            spnGreenPlaytime.Value          = _tradeOutCheck.TradeOutSettings.CSBackGreenPlaytime;
            cbxSpecialGreenPT.SelectedValue = _tradeOutCheck.TradeOutSettings.CSBackGreenSpecialPlayTime;
            spnGreenDelta.Value             = _tradeOutCheck.TradeOutSettings.CSBackGreenSpecialPlayTimeDelta;
            spnGreenWaitTime.Value          = _tradeOutCheck.TradeOutSettings.CSBackGreenWaittime;
            spnHedgePercentage.Value        = _tradeOutCheck.TradeOutSettings.CSBackHedgePercentage;
            spnHedgePlaytime.Value          = _tradeOutCheck.TradeOutSettings.CSBackHedgePlayTime;
            cbxSpecialHedgePT.SelectedValue = _tradeOutCheck.TradeOutSettings.CSBackHedgeSpecialPlayTime;
            spnHedgeDelta.Value             = _tradeOutCheck.TradeOutSettings.CSBackHedgeSpecialPlayTimeDelta;
            spnHedgeWaitTime.Value          = _tradeOutCheck.TradeOutSettings.CSBackHedgeWaitTime;
            chkOnlyHedge.Checked            = _tradeOutCheck.TradeOutSettings.CSBackOnlyHedge;
            rbnGreenDynamic.Checked         = _tradeOutCheck.TradeOutSettings.CSBackUseGreenSpecialPlayTime;
            chkUseGreenWaittime.Checked     = _tradeOutCheck.TradeOutSettings.CSBackUseGreenWaitTime;
            rbnHedgeDynamic.Checked         = _tradeOutCheck.TradeOutSettings.CSBackUseHedgeSpecialPlayTime;
            chkUseHedgeWaittime.Checked     = _tradeOutCheck.TradeOutSettings.CSBackUseHedgeWaitTime;
            chkOddsPercentage.Checked       = _tradeOutCheck.TradeOutSettings.CSBackUseOddsPercentage;
            chkWaitTimes.Checked            = _tradeOutCheck.TradeOutSettings.CSBackUseWaitTime;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.TradeOutCheck == null)
                return;
            
            OnSaveTOElement(new TTRTOElementSaveEventArgs(this.TradeOutCheck));
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            setGUIEnabled(true);

            TTRTradeOutCheck tmp = new TTRTradeOutCheck();
            tmp.Trigger = TRADEOUTTRIGGER.GENERAL;
            tmp.Order = 1;
            tmp.TradeOutSettings.TradeType = this._tradeType;
            this.TradeOutCheck = tmp;
        }

        private void gbxSettings_SizeChanged(object sender, EventArgs e)
        {
            gbxWaitTimes.Width = gbxSettings.Width - 10;
            gbxOddsPercentage.Width = gbxSettings.Width - 10;
            gbxPlaytime.Width = gbxSettings.Width - 10;
        }

        private void chkCheckPlaytime_CheckedChange(object sender, EventArgs e)
        {
            if (chkCheckPlaytime.Checked == true)
            {
                spnPlaytimeHi.Enabled = spnPlaytimeLo.Enabled = true;
            }
            else
            {
                spnPlaytimeHi.Enabled = spnPlaytimeLo.Enabled = false;
            }
        }

        private void chkScore_CheckedChanged(object sender, EventArgs e)
        {
            if (chkScore.Checked == true)
            {
                btnScores.Enabled = true;
            }
            else
            {
                btnScores.Enabled = false;
            }
        }

        private void btnScores_Click(object sender, EventArgs e)
        {
            using (frmScoresSelection dlg = new frmScoresSelection())
            {
                if (btnScores.Tag != null)
                {
                    ScoreList scoreList = btnScores.Tag as ScoreList;
                    if (scoreList != null)
                    {
                        dlg.Scores = scoreList.ToArray();
                    }
                }

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ScoreList scoreList = new ScoreList();
                    foreach (TTRScores score in dlg.Scores)
                    {
                        scoreList.Add(score);
                    }

                    btnScores.Tag = scoreList;

                    buildScoreBtnText();
                }
            }
        }
    }
}
