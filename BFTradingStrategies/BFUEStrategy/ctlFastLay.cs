using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXFastBet;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using System.Threading;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{

    public partial class ctlFastLay : UserControl, IFastBet
    {
        private SXALMarket _market = null;
        private SXALMarketWatcher _marketWatcher = null;
        private BFUEWatcher _watcher = null;
        private SXFastBetPrepManager _prepMgr = null;
        private IPTraderPrepManager _iptMgr = null;
        private double _odds = 0.0;
        private IScore _livescore = null;

        #region IFastBet Member

        public event EventHandler<IPSAddedEventArgs> IPSAdded;

        public event EventHandler<IPSDeletedEventArgs> IPSDeleted;

        public event EventHandler<IPSBetAddedEventArgs> IPSBetAdded;

        #endregion

        private void log(String message)
        {
            FastBetLog.Instance.writeLog("LayTheDraw", "FBEntryGui", message);
        }

        public ctlFastLay()
        {
            log("Constructing Fast Bet GUI");
            InitializeComponent();
            doLanguage();
            log("Linking Preplay Manger");
            _prepMgr = new SXFastBetPrepManager();
            _prepMgr.CancelPrepBet += new EventHandler<PrepMgrCancelBetEventArgs>(_prepMgr_CancelPrepBet);
            _prepMgr.NewPrepBet += new EventHandler<PrepMgrNewBetEventArgs>(_prepMgr_NewPrepBet);
            _prepMgr.PrepBetError += new EventHandler<PrepMgrBetErrorEventArgs>(_prepMgr_PrepBetError);
            _prepMgr.PreperationCompleted += new EventHandler<PrepMgrPrepCompletedEvenArgs>(_prepMgr_PreperationCompleted);

            log("Linking Inplay Manager");
            _iptMgr = new IPTraderPrepManager();

            _iptMgr.BetAddedEvent += new EventHandler<IPTraderPrepMgrBetAddedEventArgs>(_iptMgr_BetAddedEvent);
        }

        void _iptMgr_BetAddedEvent(object sender, IPTraderPrepMgrBetAddedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log(String.Format("Received an Bet Added for {0} {1} and Revoke required", e.MarketId, _market.Match));
                    IAsyncResult result = this.BeginInvoke(new EventHandler<IPTraderPrepMgrBetAddedEventArgs>(_iptMgr_BetAddedEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log(String.Format("Received a Bet Added for {0} {1} and calling IPS Bet Added", e.MarketId, _market.Match));
                    OnIPSBetAdded(new IPSBetAddedEventArgs(e.MarketId));
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public IScore LiveScore
        {
            set
            {
                log("Linking Liveticker");
                _livescore = value;
            }
        }

        private void doLanguage()
        {
            btnTargetOdds.Left = btnFastBet.Right + 15;
        }

        void _prepMgr_PreperationCompleted(object sender, PrepMgrPrepCompletedEvenArgs e)
        {
            try
            {
                if (btnFastBet.InvokeRequired)
                {
                    log(String.Format("Reveived an Preperation Completed for {0} {1} and Invoke required", e.Bet.MarketId, _market.Match));
                    IAsyncResult result = lblBackNumber.BeginInvoke(new EventHandler<PrepMgrPrepCompletedEvenArgs>(_prepMgr_PreperationCompleted), new object[] { sender, e });
                    lblBackNumber.EndInvoke(result);
                }
                else
                {
                    if (e.Bet != null && e.Bet.MarketId == _market.Id)
                    {
                        log(String.Format("Reveived a Preperation Completed for {0} {1} and Enabling GUI elements", e.Bet.MarketId, _market.Match));
                        btnFastBet.Enabled = true;
                        btnTargetOdds.Enabled = true;
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
           // throw new NotImplementedException();
        }

        void _prepMgr_PrepBetError(object sender, PrepMgrBetErrorEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _prepMgr_NewPrepBet(object sender, PrepMgrNewBetEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log(String.Format("Reveived an New Preplay Bet for {0} {1} and Invoke Required", e.Bet.MarketId, _market.Match));
                    IAsyncResult result = this.BeginInvoke(new EventHandler<PrepMgrNewBetEventArgs>(_prepMgr_NewPrepBet), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log(String.Format("Reveived a New Preplay Bet for {0} {1}. Set Watcher and call IPS Bet Added", e.Bet.MarketId, _market.Match));
                    _watcher.setNewBet(e.Bet);
                    OnIPSBetAdded(new IPSBetAddedEventArgs(e.Bet.MarketId));
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            //throw new NotImplementedException();
        }

        void _prepMgr_CancelPrepBet(object sender, PrepMgrCancelBetEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log(String.Format("Reveived an Preperation Canceled for {0} {1} and Invoke required", e.Bet.MarketId, _market.Match));
                    IAsyncResult result = this.BeginInvoke(new EventHandler<PrepMgrCancelBetEventArgs>(_prepMgr_CancelPrepBet), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    if (e.Bet.MarketId == _market.Id)
                    {
                        log(String.Format("Reveived a Preperation Canceled for {0} {1} and Enabling GUI elements", e.Bet.MarketId, _market.Match));
                        btnFastBet.Enabled = true;
                        btnTargetOdds.Enabled = true;
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        
        public BFUEWatcher DrawWatcher
        {
            set
            {
                if (value != null)
                {
                    log("Linking Watcher");
                    _watcher = value;
                    _watcher.RiskWinChangedEvent += new EventHandler<BFWRiskWinChangedEventArgs>(_watcher_RiskWinChangedEvent);
                }
            }
        }

        void _watcher_RiskWinChangedEvent(object sender, BFWRiskWinChangedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log("Reveived an Risk Win Changed and Invoke Required");
                    IAsyncResult result = this.BeginInvoke(new EventHandler<BFWRiskWinChangedEventArgs>(_watcher_RiskWinChangedEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    if (_market == null)
                        return;
                    if (lblMarketVolumeNumber.InvokeRequired)
                    {
                        log("Reveived an Risk Win Changed and Invoke Required for Market Volume Number");
                        IAsyncResult result = lblBackNumber.BeginInvoke(new EventHandler<BFWRiskWinChangedEventArgs>(_watcher_RiskWinChangedEvent), new object[] { sender, e });
                        lblBackNumber.EndInvoke(result);
                    }
                    else
                    {
                        if (e.Match == _market.Match)
                        {
                            log(String.Format("Reveived an Risk Win Changed for {0} {1}. Updating information", _market.Id, e.Match));
                            lblBackNumber.Text = e.BackGuV.ToString() + SXALBankrollManager.Instance.Currency;
                            if (e.BackGuV < 0)
                                lblBackNumber.BackColor = Color.Red;
                            else if (e.BackGuV > 0)
                                lblBackNumber.BackColor = Color.Green;
                            else
                                lblBackNumber.BackColor = SystemColors.Control;

                            lblLayNumber.Text = e.LayGuV.ToString() + SXALBankrollManager.Instance.Currency;

                            if (e.LayGuV < 0)
                                lblLayNumber.BackColor = Color.Red;
                            else if (e.LayGuV > 0)
                                lblLayNumber.BackColor = Color.Green;
                            else
                                lblLayNumber.BackColor = SystemColors.Control;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public bool ConfirmFastBet { get; set; }

        public SXALMarket Market
        {
            get
            {
                return _market;
            }

            set
            {
                try
                {
                    if (_market != value)
                    {

                        groupBox1.Text = value.Match;
                        lblMarketVolumeNumber.Text = "000000.00";
                        lblBackNumber.Text = "000000.00";
                        lblLayNumber.Text = "000000.00";
                        lblBackNumber.BackColor = SystemColors.Control;
                        lblLayNumber.BackColor = SystemColors.Control;
                        btnFastBet.Text = "No Odds";
                        // Match Odds Markt holen
                        if (!value.IsMatchOdds)
                        {
                            foreach (SXALMarket market in SXALSoccerMarketManager.Instance.InPlayMarkets.Values)
                            {
                                if (market.Match == value.Match)
                                {
                                    if (market.IsMatchOdds)
                                    {
                                        _market = market;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            _market = value;
                        }
                        log(String.Format("Setting Market to {0}", _market.Match));
                        // Alten Marktbeobachter anhalten
                        if (_marketWatcher != null)
                        {
                            log("Removing Market Price Update Listner");
                            _marketWatcher.MarketPriceUpdate -= _marketWatcher_MarketPriceUpdate;
                            _marketWatcher.Stop();
                        }
                        log("Connecting Market Price Update Listener");
                        _marketWatcher = new SXALMarketWatcher(_market.Id);
                        _marketWatcher.MarketPriceUpdate += new EventHandler<SXALMarketWatcherPricesUpdate>(_marketWatcher_MarketPriceUpdate);

                        // UE-Watcher aufsetzen, falls vorhanden
                        if (_watcher != null)
                        {
                            log(String.Format("Requesting Watcher for Information for Match {0}", _market.Match));
                            try
                            {
                                if (_watcher.BetSet.ContainsKey(_market.Id))
                                {
                                    BFUEStrategy strategy = (BFUEStrategy)_watcher.BetSet[_market.Id];
                                    if (strategy != null)
                                    {
                                        double backWin = strategy.Back.RiskWin - strategy.Lay.RiskWin;
                                        double backLost = strategy.Lay.BetSize - strategy.Back.BetSize;
                                        lblBackNumber.Text = /*strategy.Back.RiskWin.ToString()*/ backWin + SXALBankrollManager.Instance.Currency;
                                        if (backWin < 0)
                                            lblBackNumber.BackColor = Color.Red;
                                        else if (backWin > 0)
                                            lblBackNumber.BackColor = Color.Green;
                                        else
                                            lblBackNumber.BackColor = SystemColors.Control;

                                        lblLayNumber.Text = /*strategy.Lay.RiskWin.ToString()*/ backLost + SXALBankrollManager.Instance.Currency;
                                        if (backLost < 0)
                                            lblLayNumber.BackColor = Color.Red;
                                        else if (backLost > 0)
                                            lblLayNumber.BackColor = Color.Green;
                                        else
                                            lblLayNumber.BackColor = SystemColors.Control;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }

                        // Fast Bet Button initializieren
                        if (_prepMgr.isPrepInProgress(_market.Id))
                        {
                            log("Disabling Preplay Starter Buttons");
                            btnFastBet.Enabled = false;
                            btnTargetOdds.Enabled = false;
                        }
                        else
                        {
                            log("Enabling Preplay Starter Buttons");
                            btnFastBet.Enabled = true;
                            btnTargetOdds.Enabled = true;
                        }
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }

            }
        }

        public Boolean HasMarketIPS(long marketId)
        {
            try
            {
                if (_market == null)
                {
                    //log(String.Format("No SXALMarket for Id {0}. Returning false", marketId));
                    _market = SXALSoccerMarketManager.Instance.getMarketById(marketId, false);
                    if(_market == null)
                        return false;
                }
                log(String.Format("Check whether Market {0} {1} has IPS running", marketId, _market.Match));

                bool bRet = false;
                if (_iptMgr != null)
                {
                    if (_iptMgr.getConfigList(marketId) != null)
                        bRet = true;
                }
                return bRet;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

            return false;
        }

        void _marketWatcher_MarketPriceUpdate(object sender, SXALMarketWatcherPricesUpdate e)
        {
            try
            {
                if (btnFastBet.InvokeRequired)
                {
                    try
                    {
                        log(String.Format("Reveived Market Price Update for {0} {1} and Button Fast Bet Invoke Required", e.Market.Id, e.Market.Match));
                        IAsyncResult result = btnFastBet.BeginInvoke(new EventHandler<SXALMarketWatcherPricesUpdate>(_marketWatcher_MarketPriceUpdate), new object[] { sender, e });
                        btnFastBet.EndInvoke(result);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                }
                else
                {
                    if (!lblMarketVolumeNumber.InvokeRequired)
                    {
                        log(String.Format("Reveived Market Price update for {0} {1}. Updateing Informations", e.Market.Id, e.Market.Match));
                        if (e.MarketPrices.RunnerPrices.Length < 3)
                            return;
                        lblMarketVolumeNumber.Text = e.MarketPrices.RunnerPrices[2].TotalAmountMatched + " " + e.MarketPrices.CurrencyCode;
                        try
                        {
                            log(String.Format("Market Price update {0} {1} reading Configuration", e.Market.Id, e.Market.Match));
                            LTDConfigurationRW config = new LTDConfigurationRW();
                            SXFastBetSettings settings = new SXFastBetSettings();
                            settings.CancelUnmatchedFlag = config.FastBetUnmatchedCancel;
                            settings.FixedAmountFlag = config.FastBetFixedAmount;
                            settings.FixedAmountValue = config.FastBetFixedAmountValue;
                            settings.PercentAmounValue = config.FastBetPercentAmountValue;
                            settings.TotalAmountFlag = config.FastBetPercentTotalAmount;
                            settings.UnmatchedWaitSeconds = config.FastBetUnmatchedWaitSeconds;

                            try
                            {
                                log(String.Format("Market Price Update {0} {1}. Calculating money for Display", e.Market.Id, e.Market.Match));

                                String str = "No Odds";

                                if (e.MarketPrices.RunnerPrices.Length == 3 && e.MarketPrices.RunnerPrices[2].BestPricesToLay.Length > 0)
                                {
                                    str = SXFastBetMoneyGetter.getMoney(settings).ToString() + " " + SXALBankrollManager.Instance.Currency + " @ " +
                                        e.MarketPrices.RunnerPrices[2].BestPricesToLay[0].Price.ToString();
                                }
                                btnFastBet.Text = str;
                                 
                            }
                            catch (SXFastBetInsufficentFoundsExcpetion)
                            {
                                btnFastBet.Text = "0" + SXALBankrollManager.Instance.Currency + " @ " + e.MarketPrices.RunnerPrices[2].BestPricesToLay[0].Price.ToString();
                            }
                            btnFastBet.AutoSize = true;

                            _odds = e.MarketPrices.RunnerPrices[2].BestPricesToLay[0].Price;
                        }
                        catch
                        {
                            btnFastBet.Text = "No Odds";
                        }
                    }
                    else
                    {
                        try
                        {
                            log(String.Format("Reveived Market Price Update for {0} {1} and Label Market Volume Number Invoke Required", e.Market.Id, e.Market.Match));
                            IAsyncResult result = lblMarketVolumeNumber.BeginInvoke(new EventHandler<SXALMarketWatcherPricesUpdate>(_marketWatcher_MarketPriceUpdate), new object[] { sender, e });
                            lblMarketVolumeNumber.EndInvoke(result);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void lblBackNumber_Resize(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log("Reveived Resize for BackNumber and Invoke Required");
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(lblBackNumber_Resize), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log("Reveived Resize for BackNumber. Calculating new Size");
                    lblSlash.Left = lblBackNumber.Right + 2;
                    lblLay.Left = lblSlash.Right + 2;
                    lblLayNumber.Left = lblLay.Right + 2;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnFastBet_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log("Fast Bet Button was pressed and Invoke Required");
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(btnFastBet_Click), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log("Fast Bet Button was pressed. Executing");
                    bool bConfirmFastBet = true;
                    //TODO: Abfrage Bestätigung


                    log("Fast Bet Button Pressed: Get Configuration");
                    LTDConfigurationRW config = new LTDConfigurationRW();
                    SXFastBetSettings settings = new SXFastBetSettings();
                    settings.CancelUnmatchedFlag = config.FastBetUnmatchedCancel;
                    settings.FixedAmountFlag = config.FastBetFixedAmount;
                    settings.FixedAmountValue = config.FastBetFixedAmountValue;
                    settings.PercentAmounValue = config.FastBetPercentAmountValue;
                    settings.TotalAmountFlag = config.FastBetPercentTotalAmount;
                    settings.UnmatchedWaitSeconds = config.FastBetUnmatchedWaitSeconds;

                    if (ConfirmFastBet)
                    {
                        log("Fast Bet Button Pressed: Confirmation required");
                        if (MessageBox.Show(String.Format(LayTheDraw.strConfirmFastBet, SXFastBetMoneyGetter.getMoney(settings).ToString(), SXALBankrollManager.Instance.Currency, _odds), "Confirm Fast Bet", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            bConfirmFastBet = false;

                    }

                    if (bConfirmFastBet)
                    {
                        log("Fast Bet Button Pressed: Calling Preperation Manager");
                        _prepMgr.startPreperation(_market.Id, _odds, settings);
                        btnFastBet.Enabled = false;
                        btnTargetOdds.Enabled = false;
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnDrpDwnFastBet_Click(object sender, EventArgs e)
        {
        }

        private void btnDrpDwnFastBet_ButtonClick(object sender, EventArgs e)
        {            
        }

        private void btnTargetOdds_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log("Target Odds Button was pressed and Invoke Required");
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(btnTargetOdds_Click), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log("Target Odds Button was pressed. Executing");
                    if (_livescore != null && _livescore.isRunning())
                    {
                        log(String.Format("Target Odds Button Pressed: Match {0} is already running. No execution possible. Leaving", _livescore.BetfairMatch));
                        MessageBox.Show(LayTheDraw.strNoMorePreplayTargetBet, LayTheDraw.strPreplayTargetError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (_livescore != null && _livescore.Ended)
                    {
                        log(String.Format("Target Odds Button Pressed: Match {0} has already ended. No execution possible. Leaving", _livescore.BetfairMatch));
                        MessageBox.Show(LayTheDraw.strTargetBetMatchEnded, LayTheDraw.strPreplayTargetError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    log("Target Odds Button Pressed: Calling Target Odds Dialog");
                    using (frmTargetOdds dlgTargetOdds = new frmTargetOdds())
                    {
                        dlgTargetOdds.Odds = _odds;
                        DialogResult result = dlgTargetOdds.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            log("Target Odds Button Pressed: Calling Preperation Manager");
                            _prepMgr.startPreperation(_market.Id, dlgTargetOdds.Odds, dlgTargetOdds.FastBetSettings);
                            btnFastBet.Enabled = false;
                            btnTargetOdds.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnIPTrader_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log("Inplay Starter Button was pressed and Invoke Required");
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(btnIPTrader_Click), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log("Inplay Starter Button was pressed. Executing");
                    log("Inplay Starter Button pressed: Calling Inplay Starter Dialog");
                    using (frmIPTradeConfig dlg = new frmIPTradeConfig())
                    {
                        dlg.Text = _market.Match;
                        log("Linking Dialog Closed");
                        dlg.CloseIPTConfigDialog += new EventHandler<BFUEIPTraderDialogCloseEventArgs>(CloseIPTConfigDialog);
                        log(String.Format("Getting List of existing Rules for Market {0}", _market.Match));
                        BFUEFBIPTraderConfigList list = _iptMgr.getConfigList(_market.Id);
                        if (list != null)
                            dlg.ConfigList = list;
                        DialogResult result = dlg.ShowDialog();
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void CloseIPTConfigDialog(object sender, BFUEIPTraderDialogCloseEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<BFUEIPTraderDialogCloseEventArgs>(CloseIPTConfigDialog), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log("Inplay Starter Dialog was closed. Executing");
                    _iptMgr.addConfigList(_market.Id, e.ConfigList, _livescore, _watcher);
                    if (e.ConfigList.Count > 0)
                    {
                        log("Inplay Starter Dialog was Closed: Calling IPS Added");
                        OnIPSAdded(new IPSAddedEventArgs(_market.Id));
                    }
                    else
                    {
                        log("Inplay Starter Dialog was Cloased: Calling IPS Deleted");
                        OnIPSDeleted(new IPSDeletedEventArgs(_market.Id));
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        protected virtual void OnIPSAdded(IPSAddedEventArgs e)
        {
            try
            {
                if (IPSAdded != null)
                {
                    log("Received an On IPS Added. Calling IPS Added");
                    IPSAdded(this, e);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        protected virtual void OnIPSDeleted(IPSDeletedEventArgs e)
        {
            try
            {
                if (IPSDeleted != null)
                {
                    log("Received an On IPS Deleted. Calling IPS Deleted");
                    IPSDeleted(this, e);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        protected virtual void OnIPSBetAdded(IPSBetAddedEventArgs e)
        {
            try
            {
                if (IPSBetAdded != null)
                {
                    log("Received an On IPS Bet Added. Calling IPS Bet Added");
                    IPSBetAdded(this, e);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public bool HasMarketTrade(long marketId)
        {
            try
            {
                if (_market == null)
                {
                    log(String.Format("No SXALMarket for Id {0}. Returning false", marketId));
                    return false;
                }
                log(String.Format("Check whether Market {0} {1} has Trades running", marketId, _market.Match));
                if (_watcher.BetSet.ContainsKey(marketId) && _watcher.BetSet[marketId] != null)
                    return true;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return false;
        }

        public int NoOfTeamARedCards(int marketId)
        {
            try
            {
                log(String.Format("Get number of Red Cards Team A for Match {0} {1}", marketId, _market.Match));
                BFUEStrategy strategy = (BFUEStrategy)_watcher.BetSet[marketId];

                if (strategy != null)
                    return strategy.Score.RedA;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return 0;


        }

        public int NoOfTeamBRedCards(int marketId)
        {
            try
            {
                log(String.Format("Get number of Red Cards Team B for Match {0} {1}", marketId, _market.Match));
                BFUEStrategy strategy = (BFUEStrategy)_watcher.BetSet[marketId];

                if (strategy != null)
                    return strategy.Score.RedB;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return 0;
        }

        #region IFastBet Member


        public event EventHandler<LoadAutoTradeEventArgs> IPSLoadGUI;

        public event EventHandler<UnloadAutoTradeEventArgs> IPSUnloadGUI;       

        public bool HasMarketIPS(string match)
        {
            return false;
        }

        public bool HasMarketTrade(string match)
        {
            return false;
        }

        public Bitmap GetIPSBitmap()
        {
            try
            {
                if (_market != null)
                    log(String.Format("Get IPS Bitmap for Market {0}", _market.Match));
                else
                    log("Get IPS Bitmap but Market is null");
                return Resourcen.Resourcen.IPS.ToBitmap();
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return null;
        }

        public Bitmap GetTradeBitmap()
        {
            try
            {
                if (_market != null)
                    log(String.Format("Get Trade Bitmap for Market {0}", _market.Match));
                else
                    log("Get Trade Bitmap but Market is null");
                return Resourcen.Resourcen.LTD.ToBitmap();
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return null;
        }

        #endregion
    }

    public class SXFastBetMatchAlreadyStartedExcecption : Exception
    {
    }
}
