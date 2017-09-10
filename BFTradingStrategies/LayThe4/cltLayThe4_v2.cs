using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.betfairif;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Media;

namespace net.sxtrader.bftradingstrategies.LayThe4
{
    public partial class cltLayThe4_v2 : UserControl, IBFTSCommon
    {
        private IPluginHost m_host;
        private livescoreparser.LiveScoreParser m_parser;
        private livescoreparser.LiveScore2Parser m_parser2;
        private BFLT4Watcher m_watcher;
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


        public cltLayThe4_v2()
        {
            InitializeComponent();

            m_player = new SoundPlayer();

            clhLivescore1.HeaderText = LayThe4.strLivescore1;
            clhLivescore2.HeaderText = LayThe4.strLivescore2;

            clhLivescore1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            clhLivescore2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
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


        public void initHost(IPluginHost host)
        {
            DebugWriter.Instance.WriteMessage("GUI LayThe4", "initializing host");
            DebugWriter.Instance.WriteMessage("GUI LayThe4", "loading configuration");
            LT4ConfigurationRW reader = new LT4ConfigurationRW();
            m_host = host;
            m_host.Feedback(String.Format(LayThe4.strPluginLoaded, LayThe4.strModule), null);
            cbxActive.Checked = reader.StrategyActivated;
            cbxAutobetter.Checked = reader.AutomaticTrading;
            m_watcher.startListBuilder();
            BankrollManager.Instance.BalanceInfoUpdated += new EventHandler<BFBalanceUpdatedEventArgs>(Instance_BalanceInfoUpdated);
            m_watcher.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(m_watcher_ExceptionMessageEvent);

            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            m_timer.Start();
            /*
            m_watcher.startListBuilder();
            m_watcher.initRiskWin();
            cbxActive.Checked = reader.StrategyActivated;
             */
        }

        public void initWatcher(livescoreparser.LiveScoreParser parser, livescoreparser.LiveScore2Parser parser2)
        {
            m_parser = parser;
            m_parser2 = parser2;
            m_watcher = new BFLT4Watcher(parser, parser2);
            m_watcher.MessageEvent += new EventHandler<net.sxtrader.bftradingstrategies.betfairif.BFWMessageEventArgs>(MessageEventHandler);
            m_watcher.MatchAddedEvent += new EventHandler<MatchAddedEventArgs>(MatchAddedEventHandler);
            m_watcher.PlaytimeEvent += new EventHandler<BFWPlaytimeEventArgs>(PlaytimeEventHandler);
            m_watcher.GoalSumChangedEvent += new EventHandler<BFWGoalSumChangedEventArgs>(GoalSumChangedEventHandler);
            m_watcher.WinLooseChangedEvent += new EventHandler<BFWWinLooseChangedEventArgs>(WinLooseChangedEventHandler);
            m_watcher.IScoreAdded += new EventHandler<BFWIScoreAddedEventArgs>(m_watcher_IScoreAdded);
            m_watcher.NoIScoreAdded += new EventHandler<BFWNoIScoreAddedEventArgs>(m_watcher_NoIScoreAdded);
            m_watcher.SetCloseTradeTimer += new EventHandler<BFWSetCloseTradeTimer>(m_watcher_SetCloseTradeTimer);
            m_watcher.SetOpenBetTimer += new EventHandler<BFWSetOpenBetTimer>(m_watcher_SetOpenBetTimer);
            m_watcher.SetStopLossTimer += new EventHandler<BFWSetStopLossTimer>(m_watcher_SetStopLossTimer);
            m_watcher.StopCloseTradeTimer += new EventHandler<BFWStopCloseTradeTimer>(m_watcher_StopCloseTradeTimer);
            m_watcher.StopOpenBetTimer += new EventHandler<BFWStopOpenBetTimer>(m_watcher_StopOpenBetTimer);
            m_watcher.StopStopLossTimer += new EventHandler<BFWStopStopLossTimer>(m_watcher_StopStopLossTimer);
            /*
            m_watcher.GoalScoredEvent += new EventHandler<BFWGoalScoredEventArgs>(GoalScoredEventHandler);
            m_watcher.PlaytimeEvent += new EventHandler<BFWPlaytimeEventArgs>(PlaytimeEventHandler);
            m_watcher.RiskWinChangedEvent += new EventHandler<BFWRiskWinChangedEventArgs>(RiskWinChangedHandler);
            m_watcher.MessageEvent += new EventHandler<BFWMessageEventArgs>(MessageHandler);
             */
            m_watcher.GamenEndedEvent += new EventHandler<BFWGameEndedEventArgs>(GameEndedHandler);

        }

        void m_watcher_StopStopLossTimer(object sender, BFWStopStopLossTimer e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWStopStopLossTimer>(m_watcher_StopStopLossTimer), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXSLTIMER].Value = String.Empty;
                    }
                }
            }
        }

        void m_watcher_StopOpenBetTimer(object sender, BFWStopOpenBetTimer e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWStopOpenBetTimer>(m_watcher_StopOpenBetTimer), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXOBTIMER].Value = String.Empty;
                    }
                }
            }
        }

        void m_watcher_StopCloseTradeTimer(object sender, BFWStopCloseTradeTimer e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWStopCloseTradeTimer>(m_watcher_StopCloseTradeTimer), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXCTTIMER].Value = String.Empty;
                    }
                }
            }
        }

        void m_watcher_SetStopLossTimer(object sender, BFWSetStopLossTimer e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWSetStopLossTimer>(m_watcher_SetStopLossTimer), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXSLTIMER].Value = e.Timer.ToString();
                    }
                }
            }
        }

        void m_watcher_SetOpenBetTimer(object sender, BFWSetOpenBetTimer e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWSetOpenBetTimer>(m_watcher_SetOpenBetTimer), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXOBTIMER].Value = e.Timer.ToString();
                    }
                }
            }
        }

        void m_watcher_SetCloseTradeTimer(object sender, BFWSetCloseTradeTimer e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWSetCloseTradeTimer>(m_watcher_SetCloseTradeTimer), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXCTTIMER].Value = e.Timer.ToString();
                    }
                }
            }
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvLT4.Rows)
            {
                if (row.Cells[INDEXCTTIMER].Value != null && row.Cells[INDEXCTTIMER].Value.ToString() != String.Empty)
                {
                    try
                    {
                        String timer = row.Cells[INDEXCTTIMER].Value.ToString();
                        char[] seps = { ':' };
                        String[] tmps = timer.Split(seps);
                        TimeSpan span = new TimeSpan(Int16.Parse(tmps[0]), Int16.Parse(tmps[1]), Int16.Parse(tmps[2]));
                        TimeSpan second = new TimeSpan(0, 0, 1);
                        span = span.Subtract(second);
                        row.Cells[INDEXCTTIMER].Value = span.ToString();
                    }
                    catch (Exception)
                    {
                    }
                }

                if (row.Cells[INDEXOBTIMER].Value != null && row.Cells[INDEXOBTIMER].Value.ToString() != String.Empty)
                {
                    try
                    {
                        String timer = row.Cells[INDEXOBTIMER].Value.ToString();
                        char[] seps = { ':' };
                        String[] tmps = timer.Split(seps);
                        TimeSpan span = new TimeSpan(Int16.Parse(tmps[0]), Int16.Parse(tmps[1]), Int16.Parse(tmps[2]));
                        TimeSpan second = new TimeSpan(0, 0, 1);
                        span = span.Subtract(second);
                        row.Cells[INDEXOBTIMER].Value = span.ToString();
                    }
                    catch (Exception)
                    {
                    }
                }

                if (row.Cells[INDEXSLTIMER].Value != null && row.Cells[INDEXSLTIMER].Value.ToString() != String.Empty)
                {
                    try
                    {
                        String timer = row.Cells[INDEXSLTIMER].Value.ToString();
                        char[] seps = { ':' };
                        String[] tmps = timer.Split(seps);
                        TimeSpan span = new TimeSpan(Int16.Parse(tmps[0]), Int16.Parse(tmps[1]), Int16.Parse(tmps[2]));
                        TimeSpan second = new TimeSpan(0, 0, 1);
                        span = span.Subtract(second);
                        row.Cells[INDEXSLTIMER].Value = span.ToString();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        void m_watcher_ExceptionMessageEvent(object sender, SXExceptionMessageEventArgs e)
        {
            EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
            if (handler != null)
            {
                handler(this, new SXExceptionMessageEventArgs(e));
            }
        }

        private void Instance_BalanceInfoUpdated(object sender, BFBalanceUpdatedEventArgs e)
        {
            if (m_host != null)
                m_host.AccountUpdate(e.Balance, e.Availible, e.Currency);
        }


        private void m_watcher_IScoreAdded(object sender, BFWIScoreAddedEventArgs e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWIScoreAddedEventArgs>(m_watcher_IScoreAdded), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        if (e.State == LIVESCOREADDED.ALL)
                            row.DefaultCellStyle.BackColor = Color.White;
                        else if (e.State == LIVESCOREADDED.PARTLY)
                            row.DefaultCellStyle.BackColor = Color.LightGray;
                    }

                    // check Livescores
                    try
                    {
                        BFLT4Strategy strategy = (BFLT4Strategy)m_watcher.BetSet[e.MarketId];
                        if (strategy != null)
                        {
                            if (strategy.Score != null)
                            {
                                row.Cells[INDEXLS1].Value = String.Format("{0} - {1}", strategy.Score.TeamA, strategy.Score.TeamB);
                            }
                            else
                            {
                                row.Cells[INDEXLS1].Value = LayThe4.strNoLivescore;
                            }

                            if (strategy.Score2 != null)
                            {
                                row.Cells[INDEXLS2].Value = String.Format("{0} - {1}", strategy.Score2.TeamA, strategy.Score2.TeamB);
                            }
                            else
                            {
                                row.Cells[INDEXLS2].Value = LayThe4.strNoLivescore;
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

        private void m_watcher_NoIScoreAdded(object sender, BFWNoIScoreAddedEventArgs e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWNoIScoreAddedEventArgs>(m_watcher_NoIScoreAdded), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.DefaultCellStyle.BackColor = Color.SlateGray;
                    }
                }
            }
        }

        void WinLooseChangedEventHandler(object sender, BFWWinLooseChangedEventArgs e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWWinLooseChangedEventArgs>(WinLooseChangedEventHandler), new object[] { sender, e });
            }
            else
            {
                LT4ConfigurationRW config = new LT4ConfigurationRW();
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

                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXWINBACK].Value = e.BackGuV.ToString();
                        row.Cells[INDEXWINLAY].Value = e.LayGuV.ToString();
                    }
                }              
            }
        }

        void GoalSumChangedEventHandler(object sender, BFWGoalSumChangedEventArgs e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWGoalSumChangedEventArgs>(GoalSumChangedEventHandler), new object[] { sender, e });
            }
            else
            {
                LT4ConfigurationRW config = new LT4ConfigurationRW();
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

                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXSCORE].Value = e.GoalSum;
                    }
                }        
            }
        }

        void PlaytimeEventHandler(object sender, BFWPlaytimeEventArgs e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWPlaytimeEventArgs>(PlaytimeEventHandler), new object[] { sender, e });
            }
            else
            {
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        row.Cells[INDEXPLAYTIME].Value = e.Playtime;
                    }
                }
            }
        }

        void MatchAddedEventHandler(object sender, MatchAddedEventArgs e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<MatchAddedEventArgs>(MatchAddedEventHandler), new object[] { sender, e });
            }
            else
            {
                LT4ConfigurationRW config = new LT4ConfigurationRW();
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

                
                foreach (DataGridViewRow r in dgvLT4.Rows)
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

                // Summe Tore
                cell = new DataGridViewTextBoxCell();
                cell.Value = 0;
                row.Cells.Add(cell);

                // GuV Back-Win
                cell = new DataGridViewTextBoxCell();
                cell.Value = e.GuVBack;
                row.Cells.Add(cell);


                // GuV Back-Loss
                cell = new DataGridViewTextBoxCell();
                cell.Value = e.GuVLay;
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
                    BFLT4Strategy strategy = (BFLT4Strategy)m_watcher.BetSet[e.MarketId];
                    if (strategy != null)
                    {
                        if (strategy.Score != null)
                        {
                            row.Cells[INDEXLS1].Value = String.Format("{0} - {1}", strategy.Score.TeamA, strategy.Score.TeamB);
                        }
                        else
                        {
                            row.Cells[INDEXLS1].Value = LayThe4.strNoLivescore;
                        }

                        if (strategy.Score2 != null)
                        {
                            row.Cells[INDEXLS2].Value = String.Format("{0} - {1}", strategy.Score2.TeamA, strategy.Score2.TeamB);
                        }
                        else
                        {
                            row.Cells[INDEXLS2].Value = LayThe4.strNoLivescore;
                        }
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }

                //Insert Contextmenu
                ToolStripMenuItem tsiLivescore = new ToolStripMenuItem();
                tsiLivescore.Text = LayThe4.strAddLivescore;
                tsiLivescore.Click += new EventHandler(tsiLivescore_Click);

                ToolStripMenuItem tsiConfig = new ToolStripMenuItem();
                tsiConfig.Text = LayThe4.strLocalConfig;
                tsiConfig.Click += new EventHandler(tsiConfig_Click);

                ContextMenuStrip strip = new ContextMenuStrip();

                row.ContextMenuStrip = strip;
                row.ContextMenuStrip.Items.Add(tsiLivescore);
                row.ContextMenuStrip.Items.Add(tsiConfig);

                if (e.WithLivescore == LIVESCOREADDED.PARTLY)
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                else if (e.WithLivescore == LIVESCOREADDED.NONE)
                    row.DefaultCellStyle.BackColor = Color.SlateGray;

                /*
                if (!e.WithLivescore)
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                */
                dgvLT4.Rows.Add(row);
            }
        }

        void tsiConfig_Click(object sender, EventArgs e)
        {
            //Lese passendes Match
            String match = dgvLT4[0, m_mouseLocation.RowIndex].Value.ToString();
            LT4ConfigurationRW config = m_watcher.getConfiguration(match);
            if (config != null)
            {
                frmLocalConfig frmConfig = new frmLocalConfig();
                frmConfig.Configuration = config;
                DialogResult result = frmConfig.ShowDialog();
                if (result == DialogResult.OK)
                {
                    m_watcher.setConfiguration(match, frmConfig.Configuration);
                }
            }
        }

        void tsiLivescore_Click(object sender, EventArgs e)
        {
            String match = dgvLT4.Rows[m_mouseLocation.RowIndex].Cells[0].Value.ToString();

            if (!m_watcher.hasLiveScore1(match))
            {
                net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd dlg = new net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd();
                dlg.IsLiveScore2 = false;
                dlg.Match = match;
                DialogResult result = dlg.ShowDialog();

                if (result == DialogResult.OK)
                {
                    match = dgvLT4.Rows[m_mouseLocation.RowIndex].Cells[0].Value.ToString();
                    m_watcher.manualBFLSLink(match, dlg.Livescore);
                    dgvLT4.Rows[m_mouseLocation.RowIndex].DefaultCellStyle.BackColor = Color.White;

                    dgvLT4[INDEXWINBACK, m_mouseLocation.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLT4.Rows[m_mouseLocation.RowIndex].Cells[INDEXWINBACK].Value.ToString()));
                    dgvLT4[INDEXWINLAY, m_mouseLocation.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLT4.Rows[m_mouseLocation.RowIndex].Cells[INDEXWINLAY].Value.ToString()));

                }
            }

            if (!m_watcher.hasLiveScore2(match))
            {
                net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd dlg = new net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd();
                dlg.IsLiveScore2 = true;
                dlg.Match = match;
                DialogResult result = dlg.ShowDialog();

                if (result == DialogResult.OK)
                {
                    match = dgvLT4.Rows[m_mouseLocation.RowIndex].Cells[0].Value.ToString();
                    m_watcher.manualBFLSLink2(match, dlg.Livescore);
                    dgvLT4.Rows[m_mouseLocation.RowIndex].DefaultCellStyle.BackColor = Color.White;

                    dgvLT4[INDEXWINBACK, m_mouseLocation.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLT4.Rows[m_mouseLocation.RowIndex].Cells[INDEXWINBACK].Value.ToString()));
                    dgvLT4[INDEXWINLAY, m_mouseLocation.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLT4.Rows[m_mouseLocation.RowIndex].Cells[INDEXWINLAY].Value.ToString()));
                }
            }

        }

        void MessageEventHandler(object sender, net.sxtrader.bftradingstrategies.betfairif.BFWMessageEventArgs e)
        {
            if (m_host != null)
                m_host.Feedback(e.ToString(), null);
        }

        void GameEndedHandler(object sender, BFWGameEndedEventArgs e)
        {
            if (dgvLT4.InvokeRequired)
            {
                dgvLT4.Invoke(new EventHandler<BFWGameEndedEventArgs>(GameEndedHandler), new object[] { sender, e });
            }
            else
            {
                LT4ConfigurationRW config = new LT4ConfigurationRW();
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
                    history.Historize(LayThe4.strModule, e.Dts, e.Match, e.WinLoose, true);
                }

                // Spiel aus der Übersicht entfernen
                foreach (DataGridViewRow row in dgvLT4.Rows)
                {
                    if (row.Cells[INDEXMATCH].Value.ToString().Equals(e.Match))
                    {
                        dgvLT4.Rows.Remove(row);
                    }
                }
            }
        }

        private void dgvLT4_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == INDEXWINBACK || e.ColumnIndex == INDEXWINLAY) && e.RowIndex >= 0)
            {
                dgvLT4[e.ColumnIndex, e.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLT4.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()));
            }
        }

        private void dgvLT4_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dgvLT4[INDEXWINBACK, e.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLT4.Rows[e.RowIndex].Cells[3].Value.ToString()));
            dgvLT4[INDEXWINLAY, e.RowIndex].Style.BackColor = this.colorFinder(Double.Parse(dgvLT4.Rows[e.RowIndex].Cells[4].Value.ToString()));
            dgvLT4[INDEXCONFIG, e.RowIndex].Value = "C";
            dgvLT4[INDEXCONFIG, e.RowIndex].Style.BackColor = SystemColors.ButtonFace;
            /*
            dgvLT4[6, e.RowIndex].Value = "M";
            dgvLT4[6, e.RowIndex].Style.BackColor = SystemColors.ButtonFace;
             */
        }

        private void dgvLT4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == INDEXCONFIG)
            {
                //Lese passendes Match
                String match = dgvLT4[INDEXMATCH, e.RowIndex].Value.ToString();
                LT4ConfigurationRW config = m_watcher.getConfiguration(match);
                if (config != null)
                {
                    frmLocalConfig frmConfig = new frmLocalConfig();
                    frmConfig.Configuration = config;
                    DialogResult result = frmConfig.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        m_watcher.setConfiguration(match, frmConfig.Configuration);
                    }
                }
            }
        }

        private void dgvLT4_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            m_mouseLocation = e;
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion
    }
}
