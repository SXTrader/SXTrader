using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.SXFastBet;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.muk;
using System.Threading;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    public partial class ctlFastBet : UserControl, IFastBet
    {
        private SXALMarket _bfMarket;
        private IScore _liveticker;
        //private SXMarketWatcher _marketWatcher;
        private TTRWatcher _ttrWatcher;
        //private TradeStarterConfigList _configList;
        private AutoStarterPrepMgr _asPrepMgr;

        public TTRWatcher Watcher
        {
            get { return _ttrWatcher; }
            set
            {
                if (value != null)
                {
                    log("Setting Watcher");
                    _ttrWatcher = value;
                }
            }
        }


        public ctlFastBet()
        {
            FastBetLog.Instance.LogEnabled = true;
            InitializeComponent();
            log("Constructing Fast Bet GUI");
            log("Creating AutoStarterPrepMgr");
            _asPrepMgr = AutoStarterPrepMgr.Instance;//new AutoStarterPrepMgr();
            log("Connecting BetAddedEvent of AutoStarterPrepMgr");
            _asPrepMgr.BetAddedEvent += new EventHandler<ASPrepMgrBetAddedEventArgs>(_asPrepMgr_BetAddedEvent);

            //TODO: Wieder ausklammern
            
        }

        void _asPrepMgr_BetAddedEvent(object sender, ASPrepMgrBetAddedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<ASPrepMgrBetAddedEventArgs>(_asPrepMgr_BetAddedEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log(String.Format("Received a Bet Added Event for market {0}. Calling OnIPSBetAdded", e.MarketId));
                    OnIPSBetAdded(new IPSBetAddedEventArgs(e.MarketId));
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
                    log(String.Format("Calling IPSAdded for Market {0}", e.MarketID));
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
                    log(String.Format("Calling IPSDeleted for Market {0}", e.MarketID));
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
                    log(String.Format("Calling IPSBetAdded for Market {0}", e.MarketId));
                    IPSBetAdded(this, e);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        #region IFastBet Member

        public event EventHandler<IPSAddedEventArgs> IPSAdded;

        public event EventHandler<IPSDeletedEventArgs> IPSDeleted;

        public event EventHandler<IPSBetAddedEventArgs> IPSBetAdded;

        public SXALMarket Market
        {
            get
            {
                return _bfMarket;//throw new NotImplementedException();
            }
            set
            {
                
                _bfMarket = value;
                if (_bfMarket != null)
                {
                    log(String.Format("Setting Market to {0}", _bfMarket.Match));
                }
                else
                {
                    log("Deleting Market");
                }
            }
        }

        public net.sxtrader.bftradingstrategies.lsparserinterfaces.IScore LiveScore
        {
            set 
            { 
                _liveticker = value;
                if (_liveticker != null)
                {
                    log(String.Format("Set liveticker to {0}", _liveticker.BetfairMatch));
                }
                else
                {
                    log("Deleting Liveticker");
                }
            }
        }

        public bool ConfirmFastBet
        {
            get;
            set;            
        }

        public bool HasMarketIPS(long marketId)
        {
            try
            {
                SXALMarket m = SXALSoccerMarketManager.Instance.getMarketById(marketId, false);
                if (m == null)
                    return false;

                return HasMarketIPS(m.Match);
                /*
                if (_bfMarket == null)
                {
                    log(String.Format("No SXALMarket for Id {0}. Returning false", marketId));
                    return false;
                }
                else if (_bfMarket.Id == marketId)
                {
                    log(String.Format("Check whether Market {0} {1} has IPS running", marketId, _bfMarket.Match));
                    return HasMarketIPS(_bfMarket.Match);
                }
            

                return false;
                 * */
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);                
            }
            return false;
        }

        public bool HasMarketTrade(long marketId)
        {
            if (_bfMarket == null)
            {
                log(String.Format("No SXALMarket for Id {0}. Returning false", marketId));
                return false;
            }
            else if (_bfMarket.Id == marketId)
            {
                log(String.Format("Check whether Market {0} {1} has Trades running", marketId, _bfMarket.Match));
                return HasMarketTrade(_bfMarket.Match);
            }

            return false;
        }

        public int NoOfTeamARedCards(int marketId)
        {
            log(String.Format("Get number of Red Cards Team A for Match {0} {1}", marketId, _bfMarket.Match));
            if (_liveticker != null)
                return _liveticker.RedA;
            return 0;//throw new NotImplementedException();
        }

        public int NoOfTeamBRedCards(int marketId)
        {
            log(String.Format("Get number of Red Cards Team B for Match {0} {1}", marketId, _bfMarket.Match));
            if (_liveticker != null)
                return _liveticker.RedB;
            return 0; // throw new NotImplementedException();
        }


        public bool HasMarketIPS(string match)
        {
            log(String.Format("Check whether Market {0} has IPS running", match));
            bool bRet = false;
            if (_asPrepMgr != null)
            {
                log("Look in AutoStarterPrepMgr for IPS");
                if (_asPrepMgr.getConfigList(match) != null)
                    bRet = true;
            }
            log(String.Format("Market has IPS: {0}", bRet.ToString()));
            return bRet;
        }

        public bool HasMarketTrade(string match)
        {
            log(String.Format("Check whether Market {0} has Trades running", match));
            if (_ttrWatcher.ContainsKey(match))
            {
                log(String.Format("Market {0} has trades", match));
                return true;
            }

            log(String.Format("Market {0} has no trades", match));
            return false;
        }

        public Bitmap GetIPSBitmap()
        {
            if (_bfMarket != null)
                log(String.Format("Get IPS Bitmap for Market {0}", _bfMarket.Match));
            else
                log("Get IPS Bitmap but Market is null");
            return Resourcen.Resourcen.TTRStarter.ToBitmap();            
        }

        public Bitmap GetTradeBitmap()
        {
            if (_bfMarket != null)
                log(String.Format("Get Trade Bitmap for Market {0}", _bfMarket.Match));
            else
                log("Get Trade Bitmap but Market is null");
            return Resourcen.Resourcen.TTRTrade.ToBitmap();
        }


        public event EventHandler<LoadAutoTradeEventArgs> IPSLoadGUI;

        public event EventHandler<UnloadAutoTradeEventArgs> IPSUnloadGUI;

        #endregion

        private void btnStarter_Click(object sender, EventArgs e)
        {

        }

        private void btnStarter_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (btnStarter.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(btnStarter_Click_1), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    if (_bfMarket == null)
                    {
                        MessageBox.Show(TradeTheReaction.strNoMatchSelected, TradeTheReaction.strError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    ctlAutoTradeStarter ctl = new ctlAutoTradeStarter();
                    TradeStarterConfigList list = _asPrepMgr.getConfigList(_bfMarket.Match);
                    if (list != null)
                        ctl.ConfigList = list;

                    //ctl.ConfigList = _configList;
                    ctl.CloseConfigDialog += new EventHandler<TTRASTraderDialogCloseEventArgs>(ctl_CloseConfigDialog);
                    //ctl.CloseConfigDialog += new EventHandler(ctl_CloseConfigDialog);
                    ctl.Dock = DockStyle.Fill;
                    LoadAutoTradeEventArgs latea = new LoadAutoTradeEventArgs(ctl);
                    EventHandler<LoadAutoTradeEventArgs> handler = IPSLoadGUI;
                    if (handler != null)
                    {
                        handler(this, latea);
                    }

                    //this.Parent.Parent.Parent.Parent.Controls.Add(ctl);
                    //ctl.BringToFront();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void ctl_CloseConfigDialog(object sender, TTRASTraderDialogCloseEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    log(String.Format("Received and CloseConfigDialog and Invoke Requiered Market {0}", _bfMarket.Match));
                    IAsyncResult result = this.BeginInvoke(new EventHandler<TTRASTraderDialogCloseEventArgs>(ctl_CloseConfigDialog), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    log(String.Format("Received and CloseConfigDialog Market {0}", _bfMarket.Match));
                    log(String.Format("Length of AutoStarterRulesList is {0}. Market {1}", e.ConfigList.Count, _bfMarket.Match));
                    log(String.Format("Updateing AutoStarterPrepMgr with new AutoStarterRules for Market {0}", _bfMarket.Match));
                    _asPrepMgr.addConfigList(_bfMarket.Match, _bfMarket.Id, e.ConfigList, _liveticker, _ttrWatcher);
                    if (e.ConfigList.Count > 0)
                    {
                        log(String.Format("Calling OnIPSAdded for Market {0}", _bfMarket.Match));
                        OnIPSAdded(new IPSAddedEventArgs(_bfMarket.Id));
                    }
                    else
                    {
                        log(String.Format("Calling OnIPSDeleted for Market {0}", _bfMarket.Match));
                        OnIPSDeleted(new IPSDeletedEventArgs(_bfMarket.Id));
                    }
                    //
                    EventHandler<UnloadAutoTradeEventArgs> handler = IPSUnloadGUI;
                    if (handler != null)
                    {
                        log(String.Format("Notifying Listereners IPS GUI unloaded for Market {0}", _bfMarket.Match));
                        handler(this, new UnloadAutoTradeEventArgs((Control)sender));
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnMassLoadeLeague_Click(object sender, EventArgs e)
        {
            using (frmMassLoaderLeague dlg = new frmMassLoaderLeague())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        MassLoaderLeagueInfos infos = dlg.Infos;
                        if (infos.TemplatePath.Length == 0)
                        {
                            MessageBox.Show("No Template specified", "Error while Mass Loading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }


                        foreach (String league in infos.getSelectedLeagues())
                        {
                            log(String.Format("Mass loading matches of league {0}", league));
                            foreach(KeyValuePair<String, HLLiveScore> kvp in HLList.Instance)
//                            foreach (SXMatchLivescoreLink mlLink in SXMatchLivescoreLinkList.Instance.Values)
                            {
                                //if (mlLink.HLScore.IsScore1Connected() && mlLink.HLScore.League == league)
                                if (kvp.Value.IsScore1Connected() && kvp.Value.League == league)
                                {
                                    TradeStarterConfigList list = TTRHelper.OpenTemplate(infos.TemplatePath);
                                    log(String.Format("Length of AutoStarterRulesList is {0}. Market {1}", list.Count, kvp.Key));
                                    log(String.Format("Mass Loading AutoStarterPrepMgr with new AutoStarterRules for Market {0}", kvp.Key));
                                    SXALMarket m = SXALSoccerMarketManager.Instance.getWLDMarketByMatch(kvp.Key);                                    
                                    if (m == null)
                                    {
                                        log(String.Format("Could not find a Match Odds Market for match {0}",kvp.Key));
                                        continue;
                                    }
                                    //_asPrepMgr.addConfigList(mlLink.Match, mlLink.MarketId, list, mlLink.HLScore, _ttrWatcher);
                                    _asPrepMgr.addConfigList(kvp.Key, m.Id, list,kvp.Value, _ttrWatcher);
                                    if (list.Count > 0)
                                    {
                                        log(String.Format("Calling OnIPSAdded for Market {0}", m.Match));
                                        OnIPSAdded(new IPSAddedEventArgs(m.Id));
                                    }
                                    else
                                    {
                                        log(String.Format("Calling OnIPSDeleted for Market {0}",kvp.Key));
                                        OnIPSDeleted(new IPSDeletedEventArgs(m.Id));
                                    }
                                    //
                                    //EventHandler<UnloadAutoTradeEventArgs> handler = IPSUnloadGUI;
                                    //if (handler != null)
                                    //{
                                    //    log(String.Format("Notifying Listereners IPS GUI unloaded for Market {0}", mlLink.Match));
                                    //    handler(this, new UnloadAutoTradeEventArgs((Control)sender));
                                    //}
                                    //Thread.Sleep(20);
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Error while loading Template", "Mass Loader By League", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                }
            }
        }

        private void log(String message)
        {            
            FastBetLog.Instance.writeLog("TradeTheReaction", "FBEntryGui", message);
        }
    }
}
