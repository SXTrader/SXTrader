using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.TradeRules;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.tradeinterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    public partial class ctlTradeOutRuleEditAbstract : UserControl, ITradeOutRuleEdit
    {
        protected TTRTradeOutCheck _tradeOutCheck;
        protected TRADETYPE _tradeType;

        public ctlTradeOutRuleEditAbstract()
        {
            InitializeComponent();
        }

        public ctlTradeOutRuleEditAbstract(TRADETYPE tradeType)
        {
            _tradeType = tradeType;
            InitializeComponent();
        }

        #region ITradeOutRuleEdit Member

        public event EventHandler<TTRTOElementSaveEventArgs> SaveTOElement;

        public virtual net.sxtrader.bftradingstrategies.ttr.Configuration.TTRTradeOutCheck TradeOutCheck
        {
            get
            {
                return null;//throw new NotImplementedException();
            }
            set
            {
                
                //throw new NotImplementedException();
            }
        }
        #endregion

        protected void getScoreRule(Boolean isChecked, ScoreList scoreList)
        {
            if (isChecked == false)
            {
                int iCounter = 0;
                while (iCounter < _tradeOutCheck.Rules.Count)
                {
                    TTRTradeOutRuleFragment fragment = _tradeOutCheck.Rules[iCounter];
                    if (fragment.GetType() == typeof(TTRTradeOutScoreRule))
                    {
                        _tradeOutCheck.Rules.Remove(fragment);                        
                        break;
                    }
                    else iCounter++;
                }
            }
            else
            {
                bool bFound = false;
                foreach (TTRTradeOutRuleFragment fragment in _tradeOutCheck.Rules)
                {

                    if (fragment.GetType() == typeof(TTRTradeOutScoreRule))
                    {
                        TTRTradeOutScoreRule tmp = fragment as TTRTradeOutScoreRule;
                        if (tmp != null)
                        {
                            tmp.Scores = scoreList;
                            bFound = true;
                            break;
                        }
                    }
                }
                if (!bFound)
                {
                    TTRTradeOutScoreRule tmp = new TTRTradeOutScoreRule();
                    tmp.Scores = scoreList;
                    _tradeOutCheck.Rules.Add(tmp);
                }
            }

        }

        protected void getPlaytimeRule(Boolean isChecked,int hi, int lo)
        {
            if (isChecked== false)
            {
                int iCounter = 0;
                while (iCounter < _tradeOutCheck.Rules.Count)
                {
                    TTRTradeOutRuleFragment fragment = _tradeOutCheck.Rules[iCounter];
                    if (fragment.GetType() == typeof(TTRTradeOutPlayTimeRule))
                    {
                        _tradeOutCheck.Rules.Remove(fragment);
                        break;
                    }
                    else iCounter++;
                }
            }
            else
            {
                bool bFound = false;
                foreach (TTRTradeOutRuleFragment fragment in _tradeOutCheck.Rules)
                {

                    if (fragment.GetType() == typeof(TTRTradeOutPlayTimeRule))
                    {
                        TTRTradeOutPlayTimeRule tmp = fragment as TTRTradeOutPlayTimeRule;
                        if (tmp != null)
                        {
                            tmp.Hi = hi;
                            tmp.Lo = lo;
                            bFound = true;
                            break;
                        }
                    }
                }
                if (!bFound)
                {
                    TTRTradeOutPlayTimeRule tmp = new TTRTradeOutPlayTimeRule();
                    tmp.Hi = hi;
                    tmp.Lo = lo;
                    _tradeOutCheck.Rules.Add(tmp);
                }
            }
        }

        protected void getGoalSumRule(Boolean isChecked, int hi, int lo)
        {
            if (isChecked == false)
            {
                int iCounter = 0;
                while (iCounter < _tradeOutCheck.Rules.Count)
                {
                    TTRTradeOutRuleFragment fragment = _tradeOutCheck.Rules[iCounter];
                    if (fragment.GetType() == typeof(TTRTradeOutGoalSumRule))
                    {
                        _tradeOutCheck.Rules.Remove(fragment);
                        break;
                    }
                    else iCounter++;
                }
            }
            else
            {
                bool bFound = false;
                foreach (TTRTradeOutRuleFragment fragment in _tradeOutCheck.Rules)
                {

                    if (fragment.GetType() == typeof(TTRTradeOutGoalSumRule))
                    {
                        TTRTradeOutGoalSumRule tmp = fragment as TTRTradeOutGoalSumRule;
                        if (tmp != null)
                        {
                            tmp.Hi = hi;
                            tmp.Lo = lo;
                            bFound = true;
                            break;
                        }
                    }
                }
                if (!bFound)
                {
                    TTRTradeOutGoalSumRule tmp = new TTRTradeOutGoalSumRule();
                    tmp.Hi = hi;
                    tmp.Lo = lo;
                    _tradeOutCheck.Rules.Add(tmp);
                }
            }
        }

        protected void getRedCardRule(Boolean isRCEqual, Boolean isRCTeamA, Boolean isRCTeamB)
        {
            //Löschen und danach neu setzen
            int iCounter = 0;
            while (iCounter < _tradeOutCheck.Rules.Count)
            {
                TTRTradeOutRuleFragment fragment = _tradeOutCheck.Rules[iCounter];

                if (fragment.GetType() == typeof(TTRTradeOutRCEqual)
                    || fragment.GetType() == typeof(TTRTradeOutRCTeamAMore)
                    || fragment.GetType() == typeof(TTRTradeOutRCTeamBMore))
                {
                    _tradeOutCheck.Rules.Remove(fragment);
                }
                else iCounter++;
            }

            if (isRCEqual == true)
            {
                TTRTradeOutRCEqual tmp = new TTRTradeOutRCEqual();
                _tradeOutCheck.Rules.Add(tmp);
            }
            else if (isRCTeamA == true)
            {
                TTRTradeOutRCTeamAMore tmp = new TTRTradeOutRCTeamAMore();
                _tradeOutCheck.Rules.Add(tmp);
            }
            else if (isRCTeamB == true)
            {
                TTRTradeOutRCTeamBMore tmp = new TTRTradeOutRCTeamBMore();
                _tradeOutCheck.Rules.Add(tmp);
            }

        }

        protected void setGoalSumRule(CheckBox chkCheckGoalSum, NumericUpDown spnGoalSumLo, NumericUpDown spnGoalSumHi)
        {
            chkCheckGoalSum.Checked = false;
            foreach (TTRTradeOutRuleFragment fragment in _tradeOutCheck.Rules)
            {
                if (fragment.GetType() == typeof(TTRTradeOutGoalSumRule))
                {
                    TTRTradeOutGoalSumRule tmp = fragment as TTRTradeOutGoalSumRule;
                    if (tmp != null)
                    {
                        chkCheckGoalSum.Checked = true;
                        spnGoalSumLo.Value = tmp.Lo;
                        spnGoalSumHi.Value = tmp.Hi;
                        break;
                    }
                }
            }
        }

        protected void setScoresRule(CheckBox chkCheckScore, Button btnScores)
        {
            ScoreList scoreList = new ScoreList();
            btnScores.Tag = scoreList;
            foreach (TTRTradeOutRuleFragment fragment in _tradeOutCheck.Rules)
            {
                if(fragment.GetType() == typeof(TTRTradeOutScoreRule))
                {
                    TTRTradeOutScoreRule tmp = fragment as TTRTradeOutScoreRule;
                    if (tmp != null)
                    {
                        chkCheckScore.Checked = true;
                        btnScores.Tag = tmp.Scores;
                        break;
                    }
                }
            }
        }

        protected void setPlaytimeRule(CheckBox chkCheckPlaytime, NumericUpDown spnPlaytimeLo, NumericUpDown spnPlaytimeHi)
        {
            foreach (TTRTradeOutRuleFragment fragment in _tradeOutCheck.Rules)
            {
                if (fragment.GetType() == typeof(TTRTradeOutPlayTimeRule))
                {
                    TTRTradeOutPlayTimeRule tmp = fragment as TTRTradeOutPlayTimeRule;
                    if (tmp != null)
                    {
                        chkCheckPlaytime.Checked = true;
                        spnPlaytimeLo.Value = tmp.Lo;
                        spnPlaytimeHi.Value = tmp.Hi;
                        break;
                    }
                }
            }
        }

        protected void setRedCardRule(RadioButton rbnRCEqual, RadioButton rbnRCTeamA, RadioButton rbnRCTeamB, RadioButton rbnNoRedCard)
        {
            foreach (TTRTradeOutRuleFragment fragment in _tradeOutCheck.Rules)
            {
                if (fragment.GetType() == typeof(TTRTradeOutRCEqual))
                {
                    rbnRCEqual.Checked = true;
                }
                else if (fragment.GetType() == typeof(TTRTradeOutRCTeamAMore))
                {
                    rbnRCTeamA.Checked = true;
                }
                else if (fragment.GetType() == typeof(TTRTradeOutRCTeamBMore))
                {
                    rbnRCTeamB.Checked = true;
                }
            }

            if (rbnRCEqual.Checked == false && rbnRCTeamA.Checked == false && rbnRCTeamB.Checked == false)
            {
                rbnNoRedCard.Checked = true;
            }
        }

        protected void fillTriggerCombobox(ComboBox cbxTrigger)
        {
            DataSet dsTTR = new DataSet();
            DataTable dtTTR = new DataTable("Selektion");
            DataColumn dcDisplay = new DataColumn("Display");
            DataColumn dcValue = new DataColumn("Value");

            dtTTR.Columns.Add(dcDisplay);
            dtTTR.Columns.Add(dcValue);

            DataRow drGeneral = dtTTR.NewRow();
            drGeneral["Display"] = TradeTheReaction.strGeneral;
            drGeneral["Value"] = TRADEOUTTRIGGER.GENERAL;

            dtTTR.Rows.Add(drGeneral);

            DataRow drGoal = dtTTR.NewRow();
            drGoal["Display"] = TradeTheReaction.strGoal;
            drGoal["Value"] = TRADEOUTTRIGGER.GOAL;

            dtTTR.Rows.Add(drGoal);

            DataRow drRedCard = dtTTR.NewRow();
            drRedCard["Display"] = TradeTheReaction.strRedCard;
            drRedCard["Value"] = TRADEOUTTRIGGER.REDCARD;

            dtTTR.Rows.Add(drRedCard);

            DataRow drPlaytime = dtTTR.NewRow();
            drRedCard["Display"] = TradeTheReaction.strPlaytime;
            drRedCard["Value"] = TRADEOUTTRIGGER.PLAYTIME;

            dtTTR.Rows.Add(drPlaytime);

            dsTTR.Tables.Add(dtTTR);

            cbxTrigger.SuspendLayout();
            cbxTrigger.DataSource = dsTTR.Tables["Selektion"];
            cbxTrigger.DisplayMember = "Display";
            cbxTrigger.ValueMember = "Value";
            cbxTrigger.SelectedIndex = 0;
            cbxTrigger.ResumeLayout();
        }


        protected void OnSaveTOElement(TTRTOElementSaveEventArgs e)
        {
            try
            {
                /*
                EventHandler<TTRTOElementSaveEventArgs> handler = SaveTOElement;
                if (handler != null)
                {
                    handler(this, e);
                }
                */

                if (SaveTOElement != null)
                {
                    //log(String.Format("Calling IPSAdded for Market {0}", e.MarketID));
                    SaveTOElement(this, e);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        protected virtual void getTradeOutSettings(){}
        protected virtual void setTradeOutSettings(){}
    }
}
