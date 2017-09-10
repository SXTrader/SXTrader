using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.Scoreline00.Controls
{
    public partial class ctlSL00Overview : UserControl, IOverviewGUI
    {
        public ITrade _trade;
        public ctlSL00Overview()
        {
            InitializeComponent();
        }        

        #region IOverviewGUI Member

        public ITrade Trade
        {
            get
            {
                return _trade;
            }
            set
            {
                _trade = value;
                if (_trade != null)
                {                    
                    lblTradeStateValue.Text = _trade.TradeState.ToString();
                    lblAvgBackOddsValue.Text = _trade.Back.BetPrice.ToString();
                    lblAvgBackSizeValue.Text = _trade.Back.BetSize.ToString() + " " + SXALBankrollManager.Instance.Currency;
                    lblAvgLayOddsValue.Text = _trade.Lay.BetPrice.ToString();
                    lblAvgLaySizeValue.Text = _trade.Lay.BetSize.ToString() + " " + SXALBankrollManager.Instance.Currency;
                    _trade.TradeStateChanged += new EventHandler<StateChangedEventArgs>(_trade_TradeStateChanged);
                    _trade.BetsChangedEvent += new EventHandler<BetsChangedEventArgs>(_trade_BetsChangedEvent);
                }
            }
        }

        private void _trade_BetsChangedEvent(object sender, BetsChangedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<BetsChangedEventArgs>(_trade_BetsChangedEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    lblAvgBackOddsValue.Text = e.Trade.Back.BetPrice.ToString();
                    lblAvgBackSizeValue.Text = e.Trade.Back.BetSize.ToString() + " " + SXALBankrollManager.Instance.Currency;
                    lblAvgLayOddsValue.Text = e.Trade.Lay.BetPrice.ToString();
                    lblAvgLaySizeValue.Text = e.Trade.Lay.BetSize.ToString() + " " + SXALBankrollManager.Instance.Currency;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _trade_TradeStateChanged(object sender, StateChangedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<StateChangedEventArgs>(_trade_TradeStateChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    lblTradeStateValue.Text = e.NewState.ToString();//_trade.TradeState.ToString();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        #endregion
    }
}
