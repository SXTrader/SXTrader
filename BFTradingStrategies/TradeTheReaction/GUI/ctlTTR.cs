using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls;
using net.sxtrader.bftradingstrategies.ttr.GUI.Configuration;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.enums;

namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    public partial class ctlTTR : UserControl, ITradeMainGUI
    {

        private TTRWatcher _watcher;
        private IPluginHost _host;
        private Timer _timer;

        private const int TIMERINDEX  = 5;
        private const int WIN1INDEX   = 6;
        private const int WIN2INDEX   = 7;
        private const int CONFIGINDEX = 8;

        public ctlTTR()
        {
            InitializeComponent();

            dgvTrades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<EventArgs>(_timer_Tick), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvTrades.Rows)
                    {
                        if (row.Cells[TIMERINDEX].Value != null && row.Cells[TIMERINDEX].Value.ToString() != String.Empty)
                        {
                            try
                            {
                                String timer = row.Cells[TIMERINDEX].Value.ToString();
                                char[] seps = { ':' };
                                String[] tmps = timer.Split(seps);
                                TimeSpan span = new TimeSpan(Int16.Parse(tmps[0]), Int16.Parse(tmps[1]), Int16.Parse(tmps[2]));
                                TimeSpan second = new TimeSpan(0, 0, 1);
                                span = span.Subtract(second);
                                row.Cells[TIMERINDEX].Value = span.ToString();
                            }
                            catch (Exception)
                            {
                            }
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        #region ITradeMainGUI Member

        public IWatcher Watcher
        {
            get { return _watcher; }
        }

        public void initWatcher(net.sxtrader.bftradingstrategies.livescoreparser.LiveScoreParser parser, net.sxtrader.bftradingstrategies.livescoreparser.LiveScore2Parser parser2)
        {
            _watcher = new TTRWatcher(parser, parser2);
            _watcher.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(_watcher_ExceptionMessageEvent);
            _watcher.GamenEndedEvent += new EventHandler<GameEndedEventArgs>(_watcher_GamenEndedEvent);
            _watcher.GoalScoredEvent += new EventHandler<GoalScoredEventArgs>(_watcher_GoalScoredEvent);
            _watcher.IScoreAdded += new EventHandler<IScoreAddedEventArgs>(_watcher_IScoreAdded);
            _watcher.MessageEvent += new EventHandler<SXWMessageEventArgs>(_watcher_MessageEvent);
            _watcher.NoIScoreAdded += new EventHandler<NoIScoreAddedEventArgs>(_watcher_NoIScoreAdded);
            _watcher.PlaytimeEvent += new EventHandler<PlaytimeEventArgs>(_watcher_PlaytimeEvent);
            _watcher.RunningStateChanged += new EventHandler<StateChangedEventArgs>(_watcher_RunningStateChanged);
            _watcher.SetTimer += new EventHandler<SetTimerEventArgs>(_watcher_SetTimer);
            _watcher.StopTimer += new EventHandler<StopTimerEventArgs>(_watcher_StopTimer);
            _watcher.TradeAddedEvent += new EventHandler<TradeAddedEventArgs>(_watcher_TradeAddedEvent);
            _watcher.TradeStateChanged += new EventHandler<StateChangedEventArgs>(_watcher_TradeStateChanged);
            _watcher.BetsChangedEvent += new EventHandler<BetsChangedEventArgs>(_watcher_BetsChangedEvent);

            _watcher.initializeTradeList();
        }

        void _watcher_BetsChangedEvent(object sender, BetsChangedEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<BetsChangedEventArgs>(_watcher_BetsChangedEvent), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    if (e == null)
                        return;

                    if (e.Trade == null)
                        return;

                    lock (e.Trade.MarketId.ToString())
                    {
                        foreach (DataGridViewRow r in dgvTrades.Rows)
                        {
                            String tmp = (String)r.Cells[0].Value;
                            String tmp2 = (String)r.Cells[1].Value;
                            if (tmp == e.Trade.Match && tmp2 == e.Trade.TradeTypeName)
                            {
                                // Geld
                                // Geld 1: Gewinn wenn Event eintritt: Gewinn Back minus Verlust Lay
                                double moneyWin1 = (e.Trade.Back.BetPrice * e.Trade.Back.BetSize - e.Trade.Back.BetSize) - (e.Trade.Lay.BetPrice * e.Trade.Lay.BetSize - e.Trade.Lay.BetSize);
                                // Geld 2: Gewinn wenn Event nicht eintritt: Erlös Lay - Einsatz Back
                                double moneyWin2 = e.Trade.Lay.BetSize - e.Trade.Back.BetSize;

                                r.Cells[WIN1INDEX].Style.BackColor = colorFinder(moneyWin1);
                                r.Cells[WIN2INDEX].Style.BackColor = colorFinder(moneyWin2);

                                r.Cells[WIN1INDEX].Value = Math.Round(moneyWin1, 2).ToString();
                                r.Cells[WIN2INDEX].Value = Math.Round(moneyWin2, 2).ToString();

                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void Instance_BalanceInfoUpdated(object sender, SXALBalanceUpdatedEventArgs e)
        {
            
            if (_host != null)
                _host.AccountUpdate(e.Balance, e.Availible, e.Currency);
             
        }

        private void _watcher_TradeStateChanged(object sender, StateChangedEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<StateChangedEventArgs>(_watcher_TradeStateChanged), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    if (e == null)
                    {
                        
                        return;
                    }
                    if (e.NewState == null)
                    {
                        
                        return;
                    }

                    if (e.NewState.Trade == null)
                    {
                        
                        return;
                    }
                    lock (e.NewState.Trade.Match)
                    {
                        foreach (DataGridViewRow r in dgvTrades.Rows)
                        {
                            String tmp = (String)r.Cells[0].Value;
                            String tmp2 = (String)r.Cells[1].Value;
                            if (tmp == e.NewState.Trade.Match && tmp2 == e.NewState.Trade.TradeTypeName)
                            {
                                r.Cells[4].Value = e.NewState.ToString();

                                // Geld
                                // Geld 1: Gewinn wenn Event eintritt: Gewinn Back minus Verlust Lay
                                double moneyWin1 = (e.NewState.Trade.Back.BetPrice * e.NewState.Trade.Back.BetSize - e.NewState.Trade.Back.BetSize) - (e.NewState.Trade.Lay.BetPrice * e.NewState.Trade.Lay.BetSize - e.NewState.Trade.Lay.BetSize);
                                // Geld 2: Gewinn wenn Event nicht eintritt: Erlös Lay - Einsatz Back
                                double moneyWin2 = e.NewState.Trade.Lay.BetSize - e.NewState.Trade.Back.BetSize;

                                r.Cells[WIN1INDEX].Style.BackColor = colorFinder(moneyWin1);
                                r.Cells[WIN2INDEX].Style.BackColor = colorFinder(moneyWin2);

                                r.Cells[WIN1INDEX].Value = Math.Round(moneyWin1, 2).ToString();
                                r.Cells[WIN2INDEX].Value = Math.Round(moneyWin2, 2).ToString();

                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _watcher_TradeAddedEvent(object sender, TradeAddedEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<TradeAddedEventArgs>(_watcher_TradeAddedEvent), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    if (e.Trade == null)
                        return;
                    lock (e.Trade.Match)
                    {
                        //TODO: Sounds


                        foreach (DataGridViewRow r in dgvTrades.Rows)
                        {
                            String tmp = (String)r.Cells[0].Value;
                            if (tmp == e.Trade.Match)
                                if (e.Trade.TradeTypeName.Equals((String)r.Cells[1].Value, StringComparison.InvariantCultureIgnoreCase))
                                    return;
                        }

                        // Match
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCell cell = new DataGridViewTextBoxCell();
                        cell.Value = e.Trade.Match;
                        row.Cells.Add(cell);


                        //Trade Typ
                        cell = new DataGridViewTextBoxCell();
                        cell.Value = e.Trade.TradeTypeName;
                        row.Cells.Add(cell);

                        // Spielzeit
                        cell = new DataGridViewTextBoxCell();
                        if (e.Trade.Score != null && e.Trade.Score.StartDTS > DateTime.Now)
                        {
                            cell.Value = e.Trade.Score.StartDTS.ToString();
                        }
                        else if (e.Trade.Score != null)
                        {
                            cell.Value = e.Trade.Score.Playtime;
                        }
                        else
                        {
                            cell.Value = String.Empty;
                        }
                        row.Cells.Add(cell);

                        //Spielstand
                        cell = new DataGridViewTextBoxCell();
                        if (e.Trade.Score != null)
                            cell.Value = e.Trade.Score.getScore();
                        else
                            cell.Value = String.Empty;
                        row.Cells.Add(cell);

                        // Trade Status
                        cell = new DataGridViewTextBoxCell();
                        cell.Value = e.Trade.TradeState.ToString();
                        row.Cells.Add(cell);


                        // Timer
                        cell = new DataGridViewTextBoxCell();
                        cell.Value = String.Empty;
                        row.Cells.Add(cell);

                        if (e.WithLivescore == LIVESCOREADDED.PARTLY)
                            row.DefaultCellStyle.BackColor = Color.LightGray;
                        else if (e.WithLivescore == LIVESCOREADDED.NONE)
                            row.DefaultCellStyle.BackColor = Color.SlateGray;

                        // Geld
                        // Geld 1: Gewinn wenn Event eintritt: Gewinn Back minus Verlust Lay
                        double moneyWin1 = (e.Trade.Back.BetPrice * e.Trade.Back.BetSize - e.Trade.Back.BetSize) - (e.Trade.Lay.BetPrice * e.Trade.Lay.BetSize - e.Trade.Lay.BetSize);
                        // Geld 2: Gewinn wenn Event nicht eintritt: Erlös Lay - Einsatz Back
                        double moneyWin2 = e.Trade.Lay.BetSize - e.Trade.Back.BetSize;
                        cell = new DataGridViewTextBoxCell();
                        cell.Value = moneyWin1.ToString();
                        cell.Style.BackColor = colorFinder(moneyWin1);
                        row.Cells.Add(cell);

                        cell = new DataGridViewTextBoxCell();
                        cell.Value = moneyWin2.ToString();
                        cell.Style.BackColor = colorFinder(moneyWin2);
                        row.Cells.Add(cell);


                        // Lokale Konfiguration
                        cell = new DataGridViewButtonCell();
                        cell.Value = TradeTheReaction.strOptions;
                        row.Cells.Add(cell);

                        row.Tag = e.Trade;

                        dgvTrades.Rows.Add(row);
                        try
                        {
                            dgvTrades.Sort(dgvTrades.Columns[0], ListSortDirection.Ascending);
                        }
                        catch (Exception exc)
                        {
                            ExceptionWriter.Instance.WriteException(exc);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            //throw new NotImplementedException();
        }

        private void _watcher_StopTimer(object sender, StopTimerEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<StopTimerEventArgs>(_watcher_StopTimer), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvTrades.Rows)
                    {
                        if (row.Cells[0].Value.ToString().Equals(e.Match) &&
                            row.Cells[1].Value.ToString().Equals(e.Trade.TradeTypeName))
                        {
                            row.Cells[TIMERINDEX].Value = String.Empty;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _watcher_SetTimer(object sender, SetTimerEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<SetTimerEventArgs>(_watcher_SetTimer), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvTrades.Rows)
                    {
                        if (row.Cells[0].Value.ToString().Equals(e.Match) &&
                            row.Cells[1].Value.ToString().Equals(e.Trade.TradeTypeName))
                        {
                            row.Cells[TIMERINDEX].Value = e.Timer.ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _watcher_RunningStateChanged(object sender, StateChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void _watcher_PlaytimeEvent(object sender, PlaytimeEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<PlaytimeEventArgs>(_watcher_PlaytimeEvent), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    if (e == null)
                        return;
                    lock (e.Match)
                    {
                        foreach (DataGridViewRow r in dgvTrades.Rows)
                        {
                            String tmp = (String)r.Cells[0].Value;
                            if (tmp == e.Match)
                            {
                                r.Cells[2].Value = e.Playtime;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _watcher_NoIScoreAdded(object sender, NoIScoreAddedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void _watcher_MessageEvent(object sender, SXWMessageEventArgs e)
        {
            if (_host != null)
                _host.Feedback(e.ToString(), null);
        }

        private void _watcher_IScoreAdded(object sender, IScoreAddedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void _watcher_GoalScoredEvent(object sender, GoalScoredEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<GoalScoredEventArgs>(_watcher_GoalScoredEvent), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    if (e == null)
                        return;
                    lock (e.Match)
                    {
                        foreach (DataGridViewRow r in dgvTrades.Rows)
                        {
                            String tmp = (String)r.Cells[0].Value;
                            if (tmp == e.Match)
                            {
                                r.Cells[3].Value = e.Score;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _watcher_GamenEndedEvent(object sender, GameEndedEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<GameEndedEventArgs>(_watcher_GamenEndedEvent), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow r in dgvTrades.Rows)
                    {
                        String tmp = (String)r.Cells[0].Value;
                        if (tmp == e.Match)
                        {
                            dgvTrades.Rows.Remove(r);
                            r.Dispose();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _watcher_ExceptionMessageEvent(object sender, SXExceptionMessageEventArgs e)
        {
            //throw new NotImplementedException();
        }



        void ITradeMainGUI.initHost(IPluginHost host)
        {
            _host = host;


            _host.Feedback(String.Format(TradeTheReaction.strPluginLoaded, TradeTheReaction.strName), null);

            //_watcher.startListBuilder();
            //m_watcher.initRiskWin();
            //cbxActive.Checked = reader.StrategyActivated;

            SXALBankrollManager.Instance.BalanceInfoUpdated += new EventHandler<SXALBalanceUpdatedEventArgs>(Instance_BalanceInfoUpdated);
        }

        #endregion

        private Color colorFinder(double money)
        {
            Color color = Color.Black;

            if (money < 0)
                color = Color.Red;
            else if (money > 0)
                color = Color.LightGreen;
            else
                color = Color.White;

            return color;
        }

        private void dgvTrades_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<EventArgs>(dgvTrades_SelectionChanged), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                        if (dgvTrades.SelectedRows.Count == 0)
                            return;


                        DataGridViewRow row = dgvTrades.SelectedRows[0];

                        while(flpDetailView.Controls.Count > 0)
                        {
                            flpDetailView.Controls[0].Dispose();
                        }

                        ctlTTRTotalOverview ctl = new ctlTTRTotalOverview(_watcher);
                        ctl.MatchName = row.Cells[0].Value.ToString();
                        flpDetailView.Controls.Add(ctl);


                        Control[] arrCtrl = _watcher.getOverviewFragments(row.Cells[0].Value.ToString());

                        flpDetailView.Controls.AddRange(arrCtrl);                    
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void dgvTrades_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvTrades.InvokeRequired)
                {
                    IAsyncResult result = dgvTrades.BeginInvoke(new EventHandler<DataGridViewCellEventArgs>(dgvTrades_CellContentClick), new object[] { sender, e });
                    dgvTrades.EndInvoke(result);
                }
                else
                {
                    if (e.ColumnIndex == CONFIGINDEX)
                    {
                        DataGridViewRow row = dgvTrades.Rows[e.RowIndex];
                        ITrade trade = row.Tag as ITrade;
                        if (trade != null)
                        {
                            if (trade.GetType() == typeof(ScorelineTrade00))
                            {
                                using (frmLocalConfig dlg = new frmLocalConfig())
                                {
                                    dlg.Configuration = (TTRConfigurationRW)trade.Config;
                                    if (dlg.ShowDialog() == DialogResult.OK)
                                        trade.Config = dlg.Configuration;
                                }
                            }
                            else if (trade.GetType() == typeof(OverUnderTrade))
                            {
                                using (frmTradeOutConfig dlg = new frmTradeOutConfig())
                                {
                                    dlg.setTradeOutCheckList(trade.Config);
                                    if (dlg.ShowDialog() == DialogResult.OK)
                                    {
                                        IConfiguration config = trade.Config;
                                        dlg.getTradeOutCheckList(ref config);
                                        trade.Config = config;
                                    }
                                }
                            }
                            else if (trade.GetType() == typeof(CorrectScoreTrade))
                            {
                                using (frmTradeOutConfig dlg = new frmTradeOutConfig())
                                {
                                    dlg.setTradeOutCheckList(trade.Config);
                                    if (dlg.ShowDialog() == DialogResult.OK)
                                    {
                                        IConfiguration config = trade.Config;
                                        dlg.getTradeOutCheckList(ref config);
                                        trade.Config = config;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }        
    }
}
