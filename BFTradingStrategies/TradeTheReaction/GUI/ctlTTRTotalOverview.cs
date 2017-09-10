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
using System.Globalization;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    public partial class ctlTTRTotalOverview : UserControl
    {
        private String _matchName;
        private TTRWatcher _watcher;
        private List<Label> _labelList = new List<Label>();
        private List<Label> _moneyList = new List<Label>();
        private System.Timers.Timer _timer;
        
        private object _buildLock = "_buildLock";

        public String MatchName
        {
            get { return _matchName; }
            set { _matchName = value; buildView(); }
        }

        public ctlTTRTotalOverview(TTRWatcher watcher)
        {
            InitializeComponent();
            _watcher = watcher;            

            _timer = new System.Timers.Timer(60000);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
            
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result =  this.BeginInvoke(new EventHandler<System.Timers.ElapsedEventArgs>(_timer_Elapsed), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    buildView();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }


        private void buildView()
        {
            lock (_buildLock)
            {
                double guv = 0.0;
                while (this.pnlPL.Controls.Count > 0)
                {
                    this.pnlPL.Controls[0].Dispose();
                }

                foreach (Label lbl in _labelList)
                {
                    if (lbl.Tag != null)
                    {
                        ITrade trade = lbl.Tag as ITrade;
                        if (trade != null)
                        {
                            trade.BetsChangedEvent -= new EventHandler<BetsChangedEventArgs>(trade_BetsChangedEvent);
                            trade.TradeStateChanged -= new EventHandler<StateChangedEventArgs>(trade_TradeStateChanged);
                            trade.GoalScoredEvent -= new EventHandler<GoalScoredEventArgs>(trade_GoalScoredEvent);
                        }
                    }
                }

                /*
                foreach (Label l in _labelList)
                {
                    _labelList.Remove(l);
                    l.Dispose();
                }

                foreach (Label l in _moneyList)
                {
                    _moneyList.Remove(l);
                    l.Dispose();
                }
                */

                //_labelList.Clear();
                //_moneyList.Clear();
                while (_labelList.Count > 0)
                {
                    _labelList[0].Dispose();
                    _labelList.RemoveAt(0);
                }

                while (_moneyList.Count > 0)
                {
                    _moneyList[0].Dispose();
                    _moneyList.RemoveAt(0);
                }


                if (_matchName != null && _matchName != String.Empty)
                {
                    Label lblMatch = new Label();
                    lblMatch.AutoSize = true;
                    lblMatch.Text = _matchName;
                    lblMatch.Left = 5;
                    lblMatch.Top = 5;
                    pnlPL.Controls.Add(lblMatch);
                    _labelList.Add(lblMatch);

                    if (!_watcher.ContainsKey(_matchName))
                        return;
                    TradeCollection tradeColection = _watcher[_matchName];
                    foreach (ITrade trade in tradeColection.Values)
                    {
                        trade.BetsChangedEvent += new EventHandler<BetsChangedEventArgs>(trade_BetsChangedEvent);
                        trade.TradeStateChanged += new EventHandler<StateChangedEventArgs>(trade_TradeStateChanged);
                        trade.GoalScoredEvent += new EventHandler<GoalScoredEventArgs>(trade_GoalScoredEvent);


                        Label lblTradeName = new Label();
                        lblTradeName.Text = trade.TradeTypeName;
                        lblTradeName.Left = 5;
                        lblTradeName.Top = _labelList.Last().Bottom + 10;
                        lblTradeName.Visible = true;
                        lblTradeName.AutoSize = true;
                        lblTradeName.Tag = trade;
                        pnlPL.Controls.Add(lblTradeName);
                        _labelList.Add(lblTradeName);


                        Label lblMoney = new Label();
                        lblMoney.Text = Math.Round(trade.getPLSnapshot(),2) + " " + SXALBankrollManager.Instance.Currency;
                        guv += trade.getPLSnapshot();
                        lblMoney.Left = lblTradeName.Right + 5;
                        lblMoney.Top = lblTradeName.Top;
                        lblMoney.AutoSize = true;
                        lblMoney.Visible = true;
                        pnlPL.Controls.Add(lblMoney);
                        _moneyList.Add(lblMoney);
                    }

                    Label lblTotal = new Label();
                    lblTotal.AutoSize = true;
                    lblTotal.Text = TradeTheReaction.strTotal;
                    lblTotal.Left = 5;
                    lblTotal.Top = _labelList.Last().Bottom + 10; ;
                    pnlPL.Controls.Add(lblTotal);
                    _labelList.Add(lblTotal);

                    Label lblTotalMoney = new Label();
                    lblTotalMoney.AutoSize = true;
                    lblTotalMoney.Text = Math.Round(guv,2).ToString(CultureInfo.CurrentCulture) + " " + SXALBankrollManager.Instance.Currency;
                    if (guv > 0)
                        lblTotalMoney.BackColor = Color.Green;
                    else if (guv < 0)
                        lblTotalMoney.BackColor = Color.Red;

                    lblTotalMoney.Left = _labelList[_labelList.Count - 1].Right + 5; 
                    lblTotalMoney.Top = lblTotal.Top;
                    pnlPL.Controls.Add(lblTotalMoney);
                    _moneyList.Add(lblTotalMoney);
                }
            }
        }

        void trade_GoalScoredEvent(object sender, GoalScoredEventArgs e)
        {
            try
            {

                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<GoalScoredEventArgs>(trade_GoalScoredEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    buildView();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void trade_TradeStateChanged(object sender, StateChangedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<StateChangedEventArgs>(trade_TradeStateChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    buildView();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void trade_BetsChangedEvent(object sender, BetsChangedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<BetsChangedEventArgs>(trade_BetsChangedEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    buildView();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
