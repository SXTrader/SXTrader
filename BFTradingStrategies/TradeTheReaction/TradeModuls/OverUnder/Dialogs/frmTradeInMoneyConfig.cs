using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder.Dialogs
{
    public partial class frmTradeInMoneyConfig : Form
    {
        private TTRConfigurationRW _config;

        public TTRConfigurationRW Configuration
        {
            set
            {
                _config = value;

                rbnFixedBetAmount.Checked            = _config.FastBetFixedAmount;
                rbnPercentBetAmount.Checked          = !_config.FastBetFixedAmount;
                spnFastBetFixedValue.Value           = (decimal)_config.FastBetFixedAmountValue;
                spnFastBetPercentValue.Value         = (decimal)_config.FastBetPercentAmountValue;
                rbnPercentageTotalAmount.Checked     = _config.FastBetPercentTotalAmount;
                rbnPercentageAvailibleAmount.Checked = !_config.FastBetPercentTotalAmount;
                rbnRelativeBetAmount.Checked         = _config.RelativeBetAmount;
                rbnBetAmountByTarget.Checked         = _config.FastBetCalculatedAmount;
                rbnFixedTargetAmount.Checked         = _config.FastBetTargetFixedAmount;
                rbnRelativeTargetAmount.Checked      = !_config.FastBetTargetFixedAmount;
                spnFixedTargetAmount.Value           = (decimal)_config.FastBetTargetFixedAmountValue;
                spnRelativeTargetAmount.Value        = (decimal)_config.FastBetTargetPercentAmountValue;
                rbnChangeOdds.Checked                = _config.UseBackLayTicks;
                spnTicks.Value                       = (decimal)_config.BackLayTicks;
                chkKeepUnmatched.Checked             = _config.KeepUnmatched;
                chkKeepInplay.Checked                = _config.KeepInplay;

                if (_config.RelativeTradeType != null)
                {
                    cbxRelativeTradeType.SelectedValue = _config.RelativeTradeType;
                }

                if (_config.RelativeBetType != null)
                {
                    cbxRelativeBetType.SelectedValue = _config.RelativeBetType;
                }

                spnRelativeBetSize.Value = _config.RelativeBetSize;                
            }
            get
            {
                _config.FastBetFixedAmount        = rbnFixedBetAmount.Checked;
                _config.FastBetFixedAmountValue   = (double)spnFastBetFixedValue.Value;
                _config.FastBetPercentAmountValue = (int)spnFastBetPercentValue.Value;
                _config.FastBetPercentTotalAmount = rbnPercentageTotalAmount.Checked;

                _config.RelativeBetAmount = rbnRelativeBetAmount.Checked;

                if (cbxRelativeTradeType.SelectedValue != null)
                {
                    String str = cbxRelativeTradeType.SelectedValue.ToString();
                    _config.RelativeTradeType = (TRADETYPE)Enum.Parse(typeof(TRADETYPE), str);
                }

                if (cbxRelativeBetType.SelectedValue != null)
                {
                    String str = cbxRelativeBetType.SelectedValue.ToString();
                    _config.RelativeBetType = (RELATIVEBETTINGTYPE)Enum.Parse(typeof(RELATIVEBETTINGTYPE), str);
                }

                _config.RelativeBetSize = (int)spnRelativeBetSize.Value;

                _config.FastBetCalculatedAmount         = rbnBetAmountByTarget.Checked;
                _config.FastBetTargetFixedAmount        = rbnFixedTargetAmount.Checked;                
                _config.FastBetTargetFixedAmountValue   = (double)spnFixedTargetAmount.Value;
                _config.FastBetTargetPercentAmountValue = (double)spnRelativeTargetAmount.Value;
                _config.UseBackLayTicks                 = rbnChangeOdds.Checked;
                _config.BackLayTicks                    = (int)spnTicks.Value;
                _config.KeepUnmatched                   = chkKeepUnmatched.Checked;
                _config.KeepInplay                      = chkKeepInplay.Checked;

                return _config;
            }
        }

        public frmTradeInMoneyConfig()
        {
            InitializeComponent();
            TTRHelper.FillTradeTypeComboBox(cbxRelativeTradeType);
            fillBetTypeComboBox();
        }
       
        private void fillBetTypeComboBox()
        {
            // Komboboxen füllen
            DataSet dsTTR = new DataSet();
            DataTable dtTTR = new DataTable("Selektion");
            DataColumn dcDisplay = new DataColumn("Display");
            DataColumn dcValue = new DataColumn("Value");

            dtTTR.Columns.Add(dcDisplay);
            dtTTR.Columns.Add(dcValue);

            DataRow drStake = dtTTR.NewRow();
            drStake["Display"] = TradeTheReaction.strStake;
            drStake["Value"] = RELATIVEBETTINGTYPE.STAKE;

            dtTTR.Rows.Add(drStake);

            DataRow drWinning = dtTTR.NewRow();
            drWinning["Display"] = TradeTheReaction.strWinning;
            drWinning["Value"] = RELATIVEBETTINGTYPE.WINNING;

            dtTTR.Rows.Add(drWinning);

            dsTTR.Tables.Add(dtTTR);

            cbxRelativeBetType.SuspendLayout();
            cbxRelativeBetType.DataSource = dsTTR.Tables["Selektion"];
            cbxRelativeBetType.DisplayMember = "Display";
            cbxRelativeBetType.ValueMember = "Value";
            cbxRelativeBetType.SelectedIndex = -1;
            cbxRelativeBetType.ResumeLayout();
        }

        private void chkKeepUnmatched_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                if (chk.Checked == false)
                    chkKeepInplay.Checked = false;
            }
        }        
    }
}
