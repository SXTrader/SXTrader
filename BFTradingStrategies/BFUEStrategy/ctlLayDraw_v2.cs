using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.bfuestrategy.controls;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Media;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using net.sxtrader.muk.enums;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    public partial class ctlLayDraw_v2 : UserControl, IBFTSCommon
    {
        private BFUEWatcher m_watcher;
        private IPluginHost m_host;
        private livescoreparser.LiveScoreParser m_parser;
        private livescoreparser.LiveScore2Parser m_parser2;
        private List<BFUEStrategy> m_lstNotFound;
        private DataGridViewCellEventArgs m_mouseLocation;
        private Timer m_timer;
        private SoundPlayer m_player;

        private const int INDEXMATCH = 0;
        private const int INDEXPLAYTIME = 1;
        private const int INDEXSCORE = 2;
        private const int INDEXWINBACK = 3;
        private const int INDEXWINLAY = 4;
        private const int INDEXCTTIMER = 5;
        private const int INDEXOBTIMER = 6;
        private const int INDEXSLTIMER = 7;
        private const int INDEXCONFIG = 8;
        private const int INDEXLS1 = 9;
        private const int INDEXLS2 = 10;

        public ctlLayDraw_v2()
        {
            InitializeComponent();
            m_player = new SoundPlayer();
            m_lstNotFound = new List<BFUEStrategy>();
            
            BFUEWatcher.MatchAddedEvent += new EventHandler<MatchAddedEventArgs>(MatchAddedEventHandler);
            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            m_timer.Start();

            

            dgvLTD.CellToolTipTextNeeded += new DataGridViewCellToolTipTextNeededEventHandler(dgvLTD_CellToolTipTextNeeded);
//            BFUEWatcher.MatchNotFoundEvent += new EventHandler<BFWMatchNotFoundEventArgs>(MatchNotFoundEventHandler);
        }

        void dgvLTD_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                e.ToolTipText = "Dies\r\nist\r\nein\r\ntooltip";
                //ttpInfo.ToolTipTitle = "Info " + dgvLTD[0, e.RowIndex].Value.ToString();
                //ttpInfo.Show("Dies\r\nist\r\nein\r\ntooltip", this.dgvLTD);
            }
            
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<EventArgs>(m_timer_Tick), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXCTTIMER].Value != null && row.Cells[INDEXCTTIMER].Value.ToString() != String.Empty)
                        {

                            String timer = row.Cells[INDEXCTTIMER].Value.ToString();
                            char[] seps = { ':' };
                            String[] tmps = timer.Split(seps);
                            TimeSpan span = new TimeSpan(Int16.Parse(tmps[0]), Int16.Parse(tmps[1]), Int16.Parse(tmps[2]));
                            TimeSpan second = new TimeSpan(0, 0, 1);
                            span = span.Subtract(second);
                            row.Cells[INDEXCTTIMER].Value = span.ToString();
                        }

                        if (row.Cells[INDEXOBTIMER].Value != null && row.Cells[INDEXOBTIMER].Value.ToString() != String.Empty)
                        {
                            String timer = row.Cells[INDEXOBTIMER].Value.ToString();
                            char[] seps = { ':' };
                            String[] tmps = timer.Split(seps);
                            TimeSpan span = new TimeSpan(Int16.Parse(tmps[0]), Int16.Parse(tmps[1]), Int16.Parse(tmps[2]));
                            TimeSpan second = new TimeSpan(0, 0, 1);
                            span = span.Subtract(second);
                            row.Cells[INDEXOBTIMER].Value = span.ToString();
                        }

                        if (row.Cells[INDEXSLTIMER].Value != null && row.Cells[INDEXSLTIMER].Value.ToString() != String.Empty)
                        {
                            String timer = row.Cells[INDEXSLTIMER].Value.ToString();
                            char[] seps = { ':' };
                            String[] tmps = timer.Split(seps);
                            TimeSpan span = new TimeSpan(Int16.Parse(tmps[0]), Int16.Parse(tmps[1]), Int16.Parse(tmps[2]));
                            TimeSpan second = new TimeSpan(0, 0, 1);
                            span = span.Subtract(second);
                            row.Cells[INDEXSLTIMER].Value = span.ToString();
                        }
                    }
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
                LTDConfigurationReader reader = new LTDConfigurationReader();
                m_host = host;
                m_host.Feedback(String.Format(LayTheDraw.strPluginLoaded, LayTheDraw.strModule), null);

                m_watcher.startListBuilder();
                m_watcher.initRiskWin();
                cbxActive.Checked = reader.StrategyActivated;

                SXALBankrollManager.Instance.BalanceInfoUpdated += new EventHandler<SXALBalanceUpdatedEventArgs>(Instance_BalanceInfoUpdated);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void Instance_BalanceInfoUpdated(object sender, SXALBalanceUpdatedEventArgs e)
        {
            try
            {
                if (m_host != null)
                    m_host.AccountUpdate(e.Balance, e.Availible, e.Currency);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public void initWatcher(livescoreparser.LiveScoreParser parser, livescoreparser.LiveScore2Parser parser2)
        {
            try
            {
                m_parser = parser;
                m_parser2 = parser2;
                m_watcher = BFUEWatcher.getInstance(parser, parser2);
                m_watcher.GoalScoredEvent += new EventHandler<BFWGoalScoredEventArgs>(GoalScoredEventHandler);
                m_watcher.PlaytimeEvent += new EventHandler<SXWPlaytimeEventArgs>(PlaytimeEventHandler);
                m_watcher.RiskWinChangedEvent += new EventHandler<BFWRiskWinChangedEventArgs>(RiskWinChangedHandler);
                m_watcher.MessageEvent += new EventHandler<SXWMessageEventArgs>(MessageHandler);
                m_watcher.GamenEndedEvent += new EventHandler<SXWGameEndedEventArgs>(GameEndedHandler);
                m_watcher.ManualTradeRemove += m_watcher_ManualTradeRemove;
                m_watcher.IScoreAdded += new EventHandler<SXWIScoreAddedEventArgs>(m_watcher_IScoreAdded);
                m_watcher.NoIScoreAdded += new EventHandler<SXWNoIScoreAddedEventArgs>(m_watcher_NoIScoreAdded);
                m_watcher.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(m_watcher_ExceptionMessageEvent);
                m_watcher.SetCloseTradeTimer += new EventHandler<SXWSetCloseTradeTimer>(m_watcher_SetCloseTradeTimer);
                m_watcher.SetOpenBetTimer += new EventHandler<SXWSetOpenBetTimer>(m_watcher_SetOpenBetTimer);
                m_watcher.SetStopLossTimer += new EventHandler<SXWSetStopLossTimer>(m_watcher_SetStopLossTimer);
                m_watcher.StopCloseTradeTimer += new EventHandler<SXWStopCloseTradeTimer>(m_watcher_StopCloseTradeTimer);
                m_watcher.StopOpenBetTimer += new EventHandler<SXWStopOpenBetTimer>(m_watcher_StopOpenBetTimer);
                m_watcher.StopStopLossTimer += new EventHandler<SXWStopStopLossTimer>(m_watcher_StopStopLossTimer);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void m_watcher_ManualTradeRemove(object sender, SXWManualTradeRemoveEventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWManualTradeRemoveEventArgs>(m_watcher_ManualTradeRemove), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    LTDConfigurationRW config = new LTDConfigurationRW();
                    if (config.PlaySounds && config.PlayGameEnded && config.FileGameEnded != String.Empty)
                    {
                        try
                        {
                            m_player.SoundLocation = config.FileGameEnded;
                            m_player.Load();
                            m_player.Play();
                        }
                        catch (Exception exc)
                        {
                            ExceptionWriter.Instance.WriteException(exc);
                        }
                    }

                    // Spiel aus der Übersicht entfernen
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            dgvLTD.Rows.Remove(row);
                            row.Dispose();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }            
        }

        void m_watcher_StopStopLossTimer(object sender, SXWStopStopLossTimer e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWStopStopLossTimer>(m_watcher_StopStopLossTimer), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.Cells[INDEXSLTIMER].Value = String.Empty;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void m_watcher_StopOpenBetTimer(object sender, SXWStopOpenBetTimer e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWStopOpenBetTimer>(m_watcher_StopOpenBetTimer), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.Cells[INDEXOBTIMER].Value = String.Empty;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void m_watcher_StopCloseTradeTimer(object sender, SXWStopCloseTradeTimer e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWStopCloseTradeTimer>(m_watcher_StopCloseTradeTimer), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.Cells[INDEXCTTIMER].Value = String.Empty;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void m_watcher_SetStopLossTimer(object sender, SXWSetStopLossTimer e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWSetStopLossTimer>(m_watcher_SetStopLossTimer), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.Cells[INDEXSLTIMER].Value = e.Timer.ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void m_watcher_SetOpenBetTimer(object sender, SXWSetOpenBetTimer e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWSetOpenBetTimer>(m_watcher_SetOpenBetTimer), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.Cells[INDEXOBTIMER].Value = e.Timer.ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void m_watcher_SetCloseTradeTimer(object sender, SXWSetCloseTradeTimer e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWSetCloseTradeTimer>(m_watcher_SetCloseTradeTimer), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.Cells[INDEXCTTIMER].Value = e.Timer.ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void m_watcher_ExceptionMessageEvent(object sender, SXExceptionMessageEventArgs e)
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

        void m_watcher_IScoreAdded(object sender, SXWIScoreAddedEventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWIScoreAddedEventArgs>(m_watcher_IScoreAdded), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            if (e.State == LIVESCOREADDED.ALL)
                                row.DefaultCellStyle.BackColor = Color.White;
                            else if (e.State == LIVESCOREADDED.PARTLY)
                                row.DefaultCellStyle.BackColor = Color.LightGray;
                            // check Livescores
                            try
                            {
                                BFUEStrategy strategy = (BFUEStrategy)m_watcher.BetSet[e.MarketId];
                                if (strategy != null)
                                {
                                    if (((HLLiveScore)strategy.Score).IsScore1Connected())
                                    {
                                        row.Cells[INDEXLS1].Value = String.Format("{0} - {1}", ((HLLiveScore)strategy.Score).Score1.TeamA, ((HLLiveScore)strategy.Score).Score1.TeamB);
                                    }
                                    else
                                    {
                                        row.Cells[INDEXLS1].Value = LayTheDraw.strNoLivescore;
                                    }

                                    if (((HLLiveScore)strategy.Score).IsScore2Connected())
                                    {
                                        row.Cells[INDEXLS2].Value = String.Format("{0} - {1}", ((HLLiveScore)strategy.Score).Score2.TeamA, ((HLLiveScore)strategy.Score).Score2.TeamB);
                                    }
                                    else
                                    {
                                        row.Cells[INDEXLS2].Value = LayTheDraw.strNoLivescore;
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
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void m_watcher_NoIScoreAdded(object sender, SXWNoIScoreAddedEventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWNoIScoreAddedEventArgs>(m_watcher_NoIScoreAdded), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.DefaultCellStyle.BackColor = Color.SlateGray;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        public BFUEWatcher Watcher
        {
            get
            {
                return m_watcher;
            }
        }

        public List<BFUEStrategy> UnaddedMatches
        {
            get
            {
                return m_lstNotFound;
            }
        }

        public livescoreparser.LiveScoreParser Parser
        {
            get
            {
                return m_parser;
            }
        }

        public livescoreparser.LiveScore2Parser Parser2
        {
            get
            {
                return m_parser2;
            }
        }

        private void cbxActive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                m_watcher.Active = cbxActive.Checked;
                if (cbxActive.Checked == true)
                {
                    m_host.Feedback(String.Format(LayTheDraw.strPluginActivated, LayTheDraw.strModule), null);
                }
                else
                {
                    m_host.Feedback(String.Format(LayTheDraw.strPluginDeactivated, LayTheDraw.strModule), null);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void GameEndedHandler(object sender, SXWGameEndedEventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWGameEndedEventArgs>(GameEndedHandler), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    LTDConfigurationRW config = new LTDConfigurationRW();
                    if (config.PlaySounds && config.PlayGameEnded && config.FileGameEnded != String.Empty)
                    {
                        try
                        {
                            m_player.SoundLocation = config.FileGameEnded;
                            m_player.Load();
                            m_player.Play();
                        }
                        catch (Exception exc)
                        {
                            ExceptionWriter.Instance.WriteException(exc);
                        }
                    }

                    if (m_host != null)
                    {
                        IHistory history = (IHistory)m_host;
                        history.Historize(LayTheDraw.strModule, e.Dts, e.Match, e.WinLoose, false);
                    }

                    // Spiel aus der Übersicht entfernen
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            dgvLTD.Rows.Remove(row);
                            row.Dispose();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void MessageHandler(Object sender, SXWMessageEventArgs e)
        {
            try
            {
                if (m_host != null)
                    m_host.Feedback(e.ToString(), null);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void RiskWinChangedHandler(Object sender, BFWRiskWinChangedEventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<BFWRiskWinChangedEventArgs>(RiskWinChangedHandler), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    LTDConfigurationRW config = new LTDConfigurationRW();
                    if (config.PlaySounds && config.PlayTradingChanged && config.FileTradingChanged != String.Empty)
                    {
                        try
                        {
                            m_player.SoundLocation = config.FileTradingChanged;
                            m_player.Load();
                            m_player.Play();
                        }
                        catch (Exception exc)
                        {
                            ExceptionWriter.Instance.WriteException(exc);
                        }
                    }
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.Cells[INDEXWINBACK].Value = e.BackGuV.ToString();
                            row.Cells[INDEXWINLAY].Value = e.LayGuV.ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void PlaytimeEventHandler(Object sender, SXWPlaytimeEventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<SXWPlaytimeEventArgs>(PlaytimeEventHandler), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                        {
                            row.Cells[INDEXPLAYTIME].Value = e.Playtime;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void GoalScoredEventHandler(Object sender, BFWGoalScoredEventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<BFWGoalScoredEventArgs>(GoalScoredEventHandler), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    LTDConfigurationRW config = new LTDConfigurationRW();
                    if (config.PlaySounds && config.PlayScoreChanged && config.FileScoreChanged != String.Empty)
                    {
                        try
                        {
                            m_player.SoundLocation = config.FileScoreChanged;
                            m_player.Load();
                            m_player.Play();
                        }
                        catch (Exception exc)
                        {
                            ExceptionWriter.Instance.WriteException(exc);
                        }
                    }

                    foreach (DataGridViewRow row in dgvLTD.Rows)
                    {
                        if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Team))
                        {
                            row.Cells[INDEXSCORE].Value = e.Score;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void MatchAddedEventHandler(Object sender, MatchAddedEventArgs e)
        {
            try
            {
                if (dgvLTD.InvokeRequired)
                {
                    IAsyncResult result = dgvLTD.BeginInvoke(new EventHandler<MatchAddedEventArgs>(MatchAddedEventHandler), new object[] { sender, e });
                    dgvLTD.EndInvoke(result);
                }
                else
                {
                    LTDConfigurationRW config = new LTDConfigurationRW();
                    if (config.PlaySounds && config.PlayMatchAdded && config.FileMatchAdded != String.Empty)
                    {
                        try
                        {
                            m_player.SoundLocation = config.FileMatchAdded;
                            m_player.Load();
                            m_player.Play();
                        }
                        catch (Exception exc)
                        {
                            ExceptionWriter.Instance.WriteException(exc);
                        }
                    }


                    foreach (DataGridViewRow r in dgvLTD.Rows)
                    {
                        String tmp = (String)r.Cells[0].Value;
                        if (tmp == e.Match)
                            return;
                    }

                    // Match
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = e.Match;
                    row.Cells.Add(cell);

                    // Spielzeit
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = 0;
                    row.Cells.Add(cell);

                    // Spielstand
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = e.Score;
                    row.Cells.Add(cell);

                    // GuV Back-Win
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = e.BackGuV;
                    row.Cells.Add(cell);

                    // GuV Lay-Win
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = e.LayGuV;
                    row.Cells.Add(cell);

                    // Close Trade Time
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = String.Empty;
                    row.Cells.Add(cell);

                    // Open Bet Time
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = String.Empty;
                    row.Cells.Add(cell);

                    // Stop Loss Time
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = String.Empty;
                    row.Cells.Add(cell);

                    // Lokale Konfiguration
                    cell = new DataGridViewButtonCell();
                    row.Cells.Add(cell);

                    /*
                    // Manueller Abschluss
                    cell = new DataGridViewButtonCell();
                    row.Cells.Add(cell);
                     */

                    // Livescore 1
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = String.Empty;
                    row.Cells.Add(cell);

                    // Livescore 2
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = String.Empty;
                    row.Cells.Add(cell);

                    // check Livescores
                    try
                    {
                        BFUEStrategy strategy = (BFUEStrategy)m_watcher.BetSet[e.MarketId];
                        if (strategy != null)
                        {
                            if (((HLLiveScore)strategy.Score).IsScore1Connected())
                            {
                                row.Cells[INDEXLS1].Value = String.Format("{0} - {1}", ((HLLiveScore)strategy.Score).Score1.TeamA, ((HLLiveScore)strategy.Score).Score1.TeamB);
                            }
                            else
                            {
                                row.Cells[INDEXLS1].Value = LayTheDraw.strNoLivescore;
                            }

                            if (((HLLiveScore)strategy.Score).IsScore2Connected())
                            {
                                row.Cells[INDEXLS2].Value = String.Format("{0} - {1}", ((HLLiveScore)strategy.Score).Score2.TeamA, ((HLLiveScore)strategy.Score).Score2.TeamB);
                            }
                            else
                            {
                                row.Cells[INDEXLS2].Value = LayTheDraw.strNoLivescore;
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                    //Insert Contextmenu
                    ToolStripMenuItem tsiLivescore = new ToolStripMenuItem();
                    tsiLivescore.Text = LayTheDraw.strAddLivescore;
                    tsiLivescore.Click += new EventHandler(tsiLivescore_Click);


                    ToolStripSeparator tsiSeparator = new ToolStripSeparator();
                    tsiSeparator.Text = "-";

                    ToolStripMenuItem tsiDisCLivescore1 = new ToolStripMenuItem();
                    tsiDisCLivescore1.Text = String.Format(LayTheDraw.strDisCLivescore, "1");
                    tsiDisCLivescore1.Click += new EventHandler(tsiDisCLivescore1_Click);

                    ToolStripMenuItem tsiDisCLivescore2 = new ToolStripMenuItem();
                    tsiDisCLivescore2.Text = String.Format(LayTheDraw.strDisCLivescore, "2");
                    tsiDisCLivescore2.Click += new EventHandler(tsiDisCLivescore2_Click);


                    ToolStripMenuItem tsiConfig = new ToolStripMenuItem();
                    tsiConfig.Text = LayTheDraw.strLocalConfig;
                    tsiConfig.Click += new EventHandler(tsiConfig_Click);

                    ContextMenuStrip strip = new ContextMenuStrip();

                    row.ContextMenuStrip = strip;
                    row.ContextMenuStrip.Items.Add(tsiLivescore);
                    row.ContextMenuStrip.Items.Add(tsiDisCLivescore1);
                    row.ContextMenuStrip.Items.Add(tsiDisCLivescore2);
                    row.ContextMenuStrip.Items.Add(tsiSeparator);
                    row.ContextMenuStrip.Items.Add(tsiConfig);

                    if (e.WithLivescore == LIVESCOREADDED.PARTLY)
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                    else if (e.WithLivescore == LIVESCOREADDED.NONE)
                        row.DefaultCellStyle.BackColor = Color.SlateGray;

                    dgvLTD.Rows.Add(row);

                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void tsiDisCLivescore2_Click(object sender, EventArgs e)
        {
            try
            {
                //Lese passendes Match
                String match = dgvLTD[INDEXMATCH, m_mouseLocation.RowIndex].Value.ToString();
                this.Watcher.disconnectLS2(match);
                dgvLTD[INDEXLS2, m_mouseLocation.RowIndex].Value = String.Empty;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

        }

        void tsiDisCLivescore1_Click(object sender, EventArgs e)
        {
            try
            {
                //Lese passendes Match
                String match = dgvLTD[INDEXMATCH, m_mouseLocation.RowIndex].Value.ToString();
                this.Watcher.disconnectLS1(match);
                dgvLTD[INDEXLS1, m_mouseLocation.RowIndex].Value = String.Empty;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void tsiConfig_Click(object sender, EventArgs e)
        {
            try
            {
                //Lese passendes Match
                String match = dgvLTD[INDEXMATCH, m_mouseLocation.RowIndex].Value.ToString();
                LTDConfigurationRW config = m_watcher.getConfiguration(match);
                if (config != null)
                {
                    using (frmLocalConfig frmConfig = new frmLocalConfig())
                    {
                        frmConfig.Configuration = config;
                        DialogResult result = frmConfig.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            m_watcher.setConfiguration(match, frmConfig.Configuration);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void tsiLivescore_Click(object sender, EventArgs e)
        {
            try
            {
                String match = dgvLTD.Rows[m_mouseLocation.RowIndex].Cells[0].Value.ToString();

                if (!m_watcher.hasLiveScore1(match))
                {
                    net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd dlg = new net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd();
                    dlg.IsLiveScore2 = false;
                    dlg.Match = match;
                    DialogResult result = dlg.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        match = dgvLTD.Rows[m_mouseLocation.RowIndex].Cells[0].Value.ToString();
                        m_watcher.manualBFLSLink(match, dlg.Livescore);
                        dgvLTD.Rows[m_mouseLocation.RowIndex].DefaultCellStyle.BackColor = Color.White;

                        dgvLTD[INDEXWINBACK, m_mouseLocation.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLTD.Rows[m_mouseLocation.RowIndex].Cells[INDEXWINBACK].Value.ToString()));
                        dgvLTD[INDEXWINLAY, m_mouseLocation.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLTD.Rows[m_mouseLocation.RowIndex].Cells[INDEXWINLAY].Value.ToString()));

                    }
                    dlg.Dispose();
                }

                if (!m_watcher.hasLiveScore2(match))
                {
                    net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd dlg = new net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd();
                    dlg.IsLiveScore2 = true;
                    dlg.Match = match;
                    DialogResult result = dlg.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        match = dgvLTD.Rows[m_mouseLocation.RowIndex].Cells[0].Value.ToString();
                        m_watcher.manualBFLSLink2(match, dlg.Livescore);
                        dgvLTD.Rows[m_mouseLocation.RowIndex].DefaultCellStyle.BackColor = Color.White;

                        dgvLTD[INDEXWINBACK, m_mouseLocation.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLTD.Rows[m_mouseLocation.RowIndex].Cells[INDEXWINBACK].Value.ToString()));
                        dgvLTD[INDEXWINLAY, m_mouseLocation.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLTD.Rows[m_mouseLocation.RowIndex].Cells[INDEXWINLAY].Value.ToString()));

                    }
                    dlg.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

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

        private void dgvLTD_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((e.ColumnIndex == INDEXWINBACK | e.ColumnIndex == INDEXWINLAY) && e.RowIndex >= 0)
                {
                    dgvLTD[e.ColumnIndex, e.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLTD.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()));
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void dgvLTD_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                dgvLTD[INDEXWINBACK, e.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLTD.Rows[e.RowIndex].Cells[INDEXWINBACK].Value.ToString()));
                dgvLTD[INDEXWINLAY, e.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLTD.Rows[e.RowIndex].Cells[INDEXWINLAY].Value.ToString()));
                dgvLTD[INDEXCONFIG, e.RowIndex].Value = "C";
                dgvLTD[INDEXCONFIG, e.RowIndex].Style.BackColor = SystemColors.ButtonFace;

            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void dgvLTD_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == INDEXCONFIG)
                {
                    //Lese passendes Match
                    String match = dgvLTD[0, e.RowIndex].Value.ToString();
                    LTDConfigurationRW config = m_watcher.getConfiguration(match);
                    if (config != null)
                    {
                        using (frmLocalConfig frmConfig = new frmLocalConfig())
                        {
                            frmConfig.Configuration = config;
                            DialogResult result = frmConfig.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                m_watcher.setConfiguration(match, frmConfig.Configuration);
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

        private void dgvLTD_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                m_mouseLocation = e;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;
        public event EventHandler<SXWMessageEventArgs> MessageEvent;
        #endregion

        private void btnCheckTrades_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnCheckTrades.InvokeRequired)
                {
                    IAsyncResult result = btnCheckTrades.BeginInvoke(new EventHandler<EventArgs>(btnCheckTrades_Click), new object[] { sender, e });
                    btnCheckTrades.EndInvoke(result);
                }
                else
                {
                    try
                    {
                        if (m_watcher != null)
                        {
                            m_watcher.checkForTrades();
                        }
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                        if (handler != null)
                        {
                            handler(this, new SXExceptionMessageEventArgs(LayTheDraw.strName, exc.ToString()));
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
