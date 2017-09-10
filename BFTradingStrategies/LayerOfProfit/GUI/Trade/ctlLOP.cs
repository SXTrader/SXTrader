using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.plugin;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;


namespace net.sxtrader.bftradingstrategies.tippsters.GUI.Trade
{
    public partial class ctlLOP : UserControl, IBFTSCommon
    {
        private IPluginHost _host;

        public ctlLOP()
        {
            InitializeComponent();
            LOPWatcher.Instance.NoSelectionDayEvent += Instance_NoSelectionDayEvent;
            LOPWatcher.Instance.NoTippFoundEvent += Instance_NoTippFoundEvent;
            LOPWatcher.Instance.TippPlacedEvent += Instance_TippPlacedEvent;
            LOPWatcher.Instance.ExceptionMessageEvent += Instance_ExceptionMessageEvent;
            LOPWatcher.Instance.TippSettledEvent += Instance_TippSettledEvent;
            LOPWatcher.Instance.TippUpdateEvent += Instance_TippUpdateEvent;
            LOPWatcher.Instance.MessageEvent += Instance_MessageEvent;

            clearFields();

            LOPWatcher.Instance.triggerCheck();
        }

        void Instance_MessageEvent(object sender, muk.eventargs.SXWMessageEventArgs e)
        {
            try
            {
                if (_host != null)
                    _host.Feedback(e.ToString(), null);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void Instance_TippUpdateEvent(object sender, LOPTippAddedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {                    
                    IAsyncResult result = this.BeginInvoke(new EventHandler<LOPTippAddedEventArgs>(Instance_TippUpdateEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    // Nur wenn es der aktuelle Trade ist
                    if (lblRaceName.Text.Equals(e.Race, StringComparison.InvariantCultureIgnoreCase) && lblHorseName.Text.Equals(e.Horse, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Nur infos die ein Update benötigen
                        lblMatchedSizeValue.Text = e.PlacedAmount.ToString() + " " + e.Currency;
                        lblUnmatchedSizeValue.Text = e.OpenAmount.ToString() + " " + e.Currency;
                        lblCurrentRiskValue.Text = e.Risk.ToString() + " " + e.Currency;
                        lblAverageOddsValue.Text = e.AvgOdds.ToString();
                        lblPotentialRiskValue.Text = e.PotentialRisk.ToString() + " " + e.Currency;
                        lblTradingStateValue.Text = e.TradeState.ToString();

                        if (e.TradeState == tippsters.Trade.LOPTrade.TRADESTATE.COMPLETED)
                        {
                            lblTradingStateValue.ForeColor = Color.LimeGreen;
                        }
                        else if (e.TradeState == tippsters.Trade.LOPTrade.TRADESTATE.NONRUNNER)
                        {
                            lblTradingStateValue.ForeColor = Color.Red;
                        }
                        else if (e.TradeState == tippsters.Trade.LOPTrade.TRADESTATE.NOTTRADING)
                        {
                            lblTradingStateValue.ForeColor = Color.Red;
                        }
                        else
                        {
                            lblTradingStateValue.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void Instance_TippSettledEvent(object sender, LOPSettledEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<LOPSettledEventArgs>(Instance_TippSettledEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    if (lblRaceName.Text.Equals(e.Race, StringComparison.InvariantCultureIgnoreCase) && lblHorseName.Text.Equals(e.Horse, StringComparison.InvariantCultureIgnoreCase))
                    {
                        lblTradingStateValue.Text = e.TradeState.ToString();
                        lblTradingStateValue.ForeColor = Color.LimeGreen;
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void Instance_ExceptionMessageEvent(object sender, muk.eventargs.SXExceptionMessageEventArgs e)
        {
            try
            {
                EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                if (handler != null)
                {
                    handler(this, new SXExceptionMessageEventArgs(e));
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void Instance_TippPlacedEvent(object sender, LOPTippAddedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<LOPTippAddedEventArgs>(Instance_TippPlacedEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    tlpValues.Visible = true;
                    lblLayerOfProfit.Text = LayerOfProfit.strName + " " + DateTime.Now.ToShortDateString();
                    lblLayerOfProfit.ForeColor = Color.Green;
                    lblRaceName.Text = e.Race;
                    lblRaceDateValue.Text = e.EventDate.ToString();
                    lblHorseName.Text = e.Horse;
                    lblMatchedSizeValue.Text = e.PlacedAmount.ToString() + " " + e.Currency;
                    lblUnmatchedSizeValue.Text = e.OpenAmount.ToString() + " " + e.Currency;
                    lblCurrentRiskValue.Text = e.Risk.ToString() + " " + e.Currency;
                    lblAverageOddsValue.Text = e.AvgOdds.ToString();
                    lblPotentialRiskValue.Text = e.PotentialRisk.ToString() + " " + e.Currency;
                    lblTradingStateValue.Text = e.TradeState.ToString();

                    lblTradingStateValue.ForeColor = Color.Black;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void Instance_NoTippFoundEvent(object sender, LOPNoTippFoundEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<LOPNoTippFoundEventArgs>(Instance_NoTippFoundEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    lblLayerOfProfit.Text = LayerOfProfit.strName + " " + DateTime.Now.ToShortDateString() + " " + LayerOfProfit.strNoTippFound;
                    lblLayerOfProfit.ForeColor = Color.Red;
                    clearFields();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void Instance_NoSelectionDayEvent(object sender, LOPNoSelectionDayEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<LOPNoSelectionDayEventArgs>(Instance_NoSelectionDayEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    lblLayerOfProfit.Text = LayerOfProfit.strName + " " + DateTime.Now.ToShortDateString() + " " + LayerOfProfit.strNoSelectionDay;
                    lblLayerOfProfit.ForeColor = Color.Red;
                    clearFields();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public void initHost(IPluginHost host)
        {
            try
            {
                _host = host;
                _host.Feedback(String.Format(LayerOfProfit.strPluginLoaded, LayerOfProfit.strName), null);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void clearFields()
        {
            lblRaceName.Text = String.Empty;
            lblRaceDateValue.Text = String.Empty;
            lblHorseName.Text = String.Empty;
            lblMatchedSizeValue.Text = String.Empty;
            lblUnmatchedSizeValue.Text = String.Empty;
            lblCurrentRiskValue.Text = String.Empty;
            lblAverageOddsValue.Text = String.Empty;
            lblPotentialRiskValue.Text = String.Empty;
            lblTradingStateValue.Text = String.Empty;
        }


        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        public event EventHandler<SXWMessageEventArgs> MessageEvent;
    }
}
