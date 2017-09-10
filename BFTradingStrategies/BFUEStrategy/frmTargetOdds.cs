using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.SXFastBet;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    public partial class frmTargetOdds : Form
    {

        public Double Odds
        {
            set
            {
                spnOdds.Value = (decimal)value;
            }
            get
            {
                return (Double)spnOdds.Value;
            }
        }


        public SXFastBetSettings FastBetSettings
        {
            get
            {
                
                SXFastBetSettings settings = new SXFastBetSettings();
                settings.FixedAmountFlag = rbnFixedBetAmount.Checked;
                settings.PercentAmounValue = (int)spnFastBetPercentValue.Value;
                settings.FixedAmountValue = (double)spnFastBetFixedValue.Value;
                settings.TotalAmountFlag = rbnPercentageTotalAmount.Checked;
                settings.CancelUnmatchedFlag = false;
                settings.UnmatchedWaitSeconds = 90;
                return settings;
                //settings.FastBetFixedAmount = rbnFixedBetAmount.Checked;
                //m_config.FastBetFixedAmountValue = (double)spnFastBetFixedValue.Value;
                //m_config.FastBetPercentAmountValue = (int)spnFastBetPercentValue.Value;
                //m_config.FastBetPercentTotalAmount = rbnPercentageTotalAmount.Checked;
                /*
                settings.CancelUnmatchedFlag = false;
                settings.FixedAmountFlag = rbnFixedBetAmount.Checked;
                settings.FixedAmountValue = (double)spnFastBetFixedValue.Value;
                settings.PercentAmounValue = (int)spnFastBetPercentValue.Value;
                //settings.TotalAmountFlag = rbn;
                settings.UnmatchedWaitSeconds = config.FastBetUnmatchedWaitSeconds;
                 */
            }
        }
        public frmTargetOdds()
        {
            InitializeComponent();
            doLanguage();

            LTDConfigurationRW config = new LTDConfigurationRW();

            if (config.FastBetFixedAmount)
                rbnFixedBetAmount.Checked = true;
            else
                rbnPercentBetAmount.Checked = true;

            spnFastBetFixedValue.Value = (decimal)config.FastBetFixedAmountValue;
            spnFastBetPercentValue.Value = (decimal)config.FastBetPercentAmountValue;
            if (config.FastBetPercentTotalAmount)
                rbnPercentageTotalAmount.Checked = true;
            else
                rbnPercentageAvailibleAmount.Checked = true;


            //spnFastBetFixedValue.Minimum = (decimal) SXALBankrollManager.Instance.MinStake;

        }

        private void doLanguage()
        {   
            lblFastBetCurrency.Text = SXALBankrollManager.Instance.Currency;                        
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void spnOdds_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown o = (NumericUpDown)sender;
            // Nächstes incr bestimmen
            o.Increment = SXALKom.Instance.getValidOddIncrement(o.Value);
            o.Value = SXALKom.Instance.validateOdd(o.Value);
        }
    }

}
