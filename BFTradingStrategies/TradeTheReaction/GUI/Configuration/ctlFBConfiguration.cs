using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    public partial class ctlFBConfiguration : UserControl
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
        }
        

        public ctlFBConfiguration()
        {
            InitializeComponent();

            TTRHelper.FillTradeTypeComboBox(cbxRelativeTradeType);
            fillBetTypeComboBox();

            doLanguage();

        }

        public void getConfig(ref TTRConfigurationRW config)
        {
            config.FastBetFixedAmount        = rbnFixedBetAmount.Checked;
            config.FastBetFixedAmountValue   = (double)spnFastBetFixedValue.Value;
            config.FastBetPercentAmountValue = (int)spnFastBetPercentValue.Value;
            config.FastBetPercentTotalAmount = rbnPercentageTotalAmount.Checked;

            _config.RelativeBetAmount        = rbnRelativeBetAmount.Checked;

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

        }

        private void doLanguage()
        {            
            lblFastBetCurrency.Text = SXALBankrollManager.Instance.Currency;            
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

    }
}
