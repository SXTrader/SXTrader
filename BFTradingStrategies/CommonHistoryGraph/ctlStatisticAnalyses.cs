using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.plugin;

using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXFastBet;
using System.Threading;
using net.sxtrader.bftradingstrategies.common.Statistics;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.common.Configurations;
using net.sxtrader.muk;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using System.Runtime.Remoting.Messaging;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.common
{
    public partial class ctlStatisticAnalyses : UserControl
    {
        private SortedList<String, String> _buttonList = new SortedList<String,String>();
        private IPluginHost _pluginHost;


        private bool _newButtonInserted = false;
        private bool _firstLoad = true;
        private object _buttonListLock = "buildButtonList";
        private object _ipsUpdateLock = "ipsUpdateLock";
        private ConcurrentDictionary<String, SXALMarket> _marketLivetickerUpdate;
        private System.Timers.Timer _timerMarketLivetickerUpdate;
        

        public ctlStatisticAnalyses()
        {
            InitializeComponent();

            _marketLivetickerUpdate = new  ConcurrentDictionary<String, SXALMarket>() ;

            SXALSoccerMarketManager.Instance.MarketAddedEvent += new EventHandler<MarketAddedEventArgs>(SXALSoccerMarketManager_MarketAddedEvent);
            SXALSoccerMarketManager.Instance.MarketUpdateStartedEvent += new EventHandler<MarketUpdateStartedEventArgs>(SXALSoccerMarketManager_MarketUpdateStartedEvent);
            SXALSoccerMarketManager.Instance.MarketUpdateCompletedEvent += new EventHandler<MarketUpdateCompletedEventArgs>(SXALSoccerMarketManager_MarketUpdateCompletedEvent);

            pnlSplit.BackColor = SystemColors.Control;
            pnlSplit.Panel1.BackColor = SystemColors.Control;
            pnlSplitStatsTop.Panel1.BackColor = SystemColors.Control;           
            pnlSplitStatsTop.Panel2.BackColor = SystemColors.Control;
            pnlSplitStats.Panel2.BackColor = SystemColors.Control;
            pnlMatchDetail.BackColor = SystemColors.Control;
            pnlMatchDetail.BorderStyle = BorderStyle.FixedSingle;
            pnlBets.BackColor = SystemColors.Control;
            pnlBets.BorderStyle = BorderStyle.FixedSingle;
            lblBetModuls.Visible = false;

            _timerMarketLivetickerUpdate = new System.Timers.Timer(new TimeSpan(0,10,0).TotalMilliseconds);
            _timerMarketLivetickerUpdate.Elapsed += _timerMarketLivetickerUpdate_Elapsed;
            _timerMarketLivetickerUpdate.Start();

            //initialList();
        }
        
        async void SXALSoccerMarketManager_MarketUpdateCompletedEvent(object sender, MarketUpdateCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine("SXALSoccerMarketManager_MarketUpdateCompletedEvent");

                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() =>
                        {
                            sortButtons();
                            colorButtons();
                            loadTradeInfoBitmaps();
                        }));
                }
                else
                {
                    await Task.Run(() => sortButtons());
                    await Task.Run(() => colorButtons());
                    await Task.Run(() => loadTradeInfoBitmaps());
                }
            }
            catch(Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            finally
            {
                Console.WriteLine("SXALSoccerMarketManager_MarketUpdateCompletedEvent beendet");
            }

        }

        void SXALSoccerMarketManager_MarketUpdateStartedEvent(object sender, MarketUpdateStartedEventArgs e)
        {
        }

        async void SXALSoccerMarketManager_MarketAddedEvent(object sender, MarketAddedEventArgs e)
        {
            //Momentan wollen wir keine Unmanaged Markets
            if (e.Market.IsUnmanaged) return;
            //Für die ButtonListe nehmen wir exemplarisch nur den MatchOdds-Markt.
            if (!e.Market.IsMatchOdds) return;

            String strKey = e.Market.StartDTS.ToString("dd.MM.yyyy HH:mm") +
                       " " + e.Market.Match;

            Console.WriteLine("MarketAddedEvent + " + strKey);

            if (_buttonList.ContainsKey(strKey))
                return;


            await addButtons(e.Market);

        }

        //private delegate void delegateControlsAdd(Control ValueType);
        //private delegate void delegateControlsSetChildIndex(Control child, int newIndex);
        //private delegate void delegateAddButton(SXALMarket e);
        //private delegate void delegateIPSUpdate();
        //private delegate void delegateUpdateButtonImages(RadioButton rbn, List<Bitmap> bitmaps);
        //private delegate void delegateRadioButtonPanelAdd(RadioButton rbn, int index);
        //private delegate void delegateLivetickerButtonUpdate(RadioButton rbn, SXALMarket e, String strKey);


        private void Market_MarketLivetickerAdded(object sender, SXALMarketLivetickerAddedEventArgs e)
        {
            try
            {
                if (!e.NoUpdateWait)
                {
                    if (_marketLivetickerUpdate.ContainsKey(e.Market.Match))
                    {
                        DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Liveticker Update Dictionary already contains match {0}", e.Market.Match));
                        return;
                    }

                    DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Insert match {0} to Liveticker Update Dictionary", e.Market.Match));
                    if (!_marketLivetickerUpdate.TryAdd(e.Market.Match, e.Market))
                    {
                        DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Failed to insert match {0} to Liveticker Update Dictionary", e.Market.Match));
                        return;
                    }
                }
                else
                {
                    DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Directly update match {0} for liveticker added", e.Market.Match));
                    marketLivetickerAddedRunner(e.Market);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

        }

        private void _timerMarketLivetickerUpdate_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DebugWriter.Instance.WriteMessage("Statistics & Analyses", "Timer Market Liveticker Update: Elapsed");

            DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Timer Market Liveticker Update: Number of Entries {0}", _marketLivetickerUpdate.Count));

            foreach (String key in _marketLivetickerUpdate.Keys)
            {
                SXALMarket market = null;
                DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Timer Market Liveticker Update: Process entry {0}", key));
                if (!_marketLivetickerUpdate.TryRemove(key, out market))
                {
                    DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Timer Market Liveticker Update: Failed to retrieve entry {0}", key));
                    continue;
                }

                marketLivetickerAddedRunner(market);
            }
            
        }

        private void marketLivetickerAddedRunner(SXALMarket e)
        {
            if (lblButtons.InvokeRequired)
            {
                lblButtons.Invoke(new MethodInvoker(() =>
                {
                    marketLivetickerAddedRunner(e);
                }));
            }
            else
            {
                String strKey = e.StartDTS.ToString("dd.MM.yyyy HH:mm") +
                                " " + e.Match;

                lblButtons.Visible = false;
                bool exist = false;

                if (!_buttonList.ContainsKey(strKey))
                    exist = true;

                if (exist) return;


                RadioButton rbnMatch = (RadioButton)this.pnlSplit.Panel1.Controls[_buttonList[strKey]];
                DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Update Entry {0} directly", strKey));
                livetickerButtonUpdate(rbnMatch, e, strKey);
            }
        }

        private async void livetickerButtonUpdate(RadioButton rbnMatch, SXALMarket e, String strKey)
        {
            /*
             * if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() =>
                        {
                            sortButtons();
                            colorButtons();
                            loadTradeInfoBitmaps();                            
                        }));
                }
                else
                {
                    await Task.Run(() => sortButtons());
                    await Task.Run(() => colorButtons());
                    await Task.Run(() => loadTradeInfoBitmaps());
                }
             */
            try
            {
                if (rbnMatch.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() =>
                    { 
                        livetickerButtonUpdate(rbnMatch, e, strKey); 
                    }));
                    /*
                    DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Update of Entry {0} need Invoke", strKey));
                    delegateLivetickerButtonUpdate livetickerButtonUpdate = new delegateLivetickerButtonUpdate(this.livetickerButtonUpdate);
                    IAsyncResult result = rbnMatch.BeginInvoke(livetickerButtonUpdate, new object[] { rbnMatch, e, strKey });

                    if (!result.AsyncWaitHandle.WaitOne(60000))
                    {
                        DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Update of Entry {0} Async-Timeout after 60 seconds", strKey));
                    }
                    result.AsyncWaitHandle.Close();
                    rbnMatch.EndInvoke(result);
                     */
                }
                else
                {

                    DebugWriter.Instance.WriteMessage("Statistcs & Analyses", String.Format("Retrived Notification of  New Liveticker to Sport Exchange Match {0}. Updating Buttonlist", e.Match));

                    if (e.Liveticker != null && ((e.Liveticker as HLLiveScore).IsScore1Connected()
                        || (e.Liveticker as HLLiveScore).IsScore2Connected()))
                    {

                        rbnMatch.Text = strKey + " " + e.Liveticker.League;
                        rbnMatch.Enabled = true;
                        /*
                        e.Liveticker.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(score_PlaytimeTickEvent);
                        e.Liveticker.RaiseGoalEvent += new EventHandler<GoalEventArgs>(score_RaiseGoalEvent);
                        e.Liveticker.BackGoalEvent += new EventHandler<GoalBackEventArgs>(score_BackGoalEvent);
                        e.Liveticker.GameEndedEvent += new EventHandler<GameEndedEventArgs>(score_GameEndedEvent);
                        e.Liveticker.RedCardEvent += new EventHandler<RedCardEventArgs>(score_RedCardEvent);
                         */
                        HLLiveScore hlScore = e.Liveticker as HLLiveScore;
                        if (hlScore != null && (!hlScore.IsScore1Connected() || !hlScore.IsScore2Connected()))
                        {
                            rbnMatch.BackColor = Color.LightGray;
                        }
                        else if (hlScore != null && hlScore.IsScore1Connected() && hlScore.IsScore2Connected())
                        {
                            rbnMatch.BackColor = SystemColors.ButtonFace;
                        }

                        if (hlScore.IsScore1Connected())
                        {
                            
                            Color color = Color.Transparent;
                            ButtonColorStatHelper helper = new ButtonColorStatHelper();
                            Tuple<bool, Color> t = await helper.buttonStatColor(hlScore.TeamAId, hlScore.TeamBId, hlScore.TeamA, hlScore.TeamB, hlScore.League);
                            if (t.Item1)
                            {
                                rbnMatch.BackColor = t.Item2;
                            }
                            
                            if (!SXLeagues.Instance.ContainsKey(hlScore.League))
                            {
                                SXLeagues.Instance.Add(hlScore.League, hlScore.League);
                            }
                             
                        }


                        if (DateTime.Now > e.Liveticker.StartDTS)
                        {
                            if (e.Liveticker.Ended)
                            {
                                rbnMatch.BackColor = Color.Red;
                            }
                            else
                            {
                                rbnMatch.BackColor = Color.LightGreen;
                            }

                            rbnMatch.Text = strKey + " " + e.Liveticker.League + "  /  " + e.Liveticker.getScore();
                        }
                    }
                    else
                    {
                        rbnMatch.BackColor = Color.SlateGray;

                    }

                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            finally
            {
                
            }

        }

        private async Task addButtons(SXALMarket e)
        {
            try
            {
                DebugWriter.Instance.WriteMessage("Statistics & Analyses", "Add Button");

                //Momentan wollen wir keine Unmanaged Markets
                if (e.IsUnmanaged) return;
                //Für die ButtonListe nehmen wir exemplarisch nur den MatchOdds-Markt.
                if (!e.IsMatchOdds) return;

                String strKey = e.StartDTS.ToString("dd.MM.yyyy HH:mm") +
                    " " + e.Match;

                DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Key of Button is {0}", strKey));

                if (lblButtons.Created || lblButtons.InvokeRequired)
                {
                    lblButtons.Invoke((MethodInvoker)(() =>
                    {
                        lblButtons.Visible = false;
                    }));
                }
                else
                    lblButtons.Visible = false;

                
                if (_buttonList.ContainsKey(strKey))
                {
                    DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Button with key {0} alread exists. Leaving", strKey));
                    return;
                }

                DebugWriter.Instance.WriteMessage("Statistics & Analyses", "Actually Build Button");
                e.MarketLivetickerAdded += new EventHandler<SXALMarketLivetickerAddedEventArgs>(Market_MarketLivetickerAdded);

                RadioButton rbnMatch = new RadioButton();

                rbnMatch.Name = "rbn" + e.Id.ToString();
                rbnMatch.Appearance = Appearance.Button;
                rbnMatch.AutoSize = true;
                rbnMatch.Text = strKey;
                rbnMatch.Dock = DockStyle.Top;

                e.MarketToBeRemoved += new EventHandler<SXALMarketRemoveEventArgs>(e_MarketToBeRemoved);

                DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Button {0} created", rbnMatch.Name));

                //TODO: Livetickergedöns
                linkLivetickerToNewButton(e.Liveticker, rbnMatch);
                rbnMatch.Tag = e;

                // Mausevent für Kontextmenü
                rbnMatch.MouseClick += new MouseEventHandler(rbnMatch_MouseClick);
                rbnMatch.CheckedChanged += new EventHandler(rbnMatch_CheckedChanged);


                _buttonList.Add(strKey, rbnMatch.Name);
                int iButtonIndex = _buttonList.IndexOfKey(strKey);
                DebugWriter.Instance.WriteMessage("Statistics & Analyses", "Add Button to Panel");


                if (this.pnlSplit.Panel1.InvokeRequired)
                {
                    DebugWriter.Instance.WriteMessage("Statistics & Analyses", "With Invoke");

                    //this.pnlSplit.Invoke()
                    this.pnlSplit.Invoke(new MethodInvoker(() => 
                    {
                        radioButtonPannelAdd(rbnMatch, iButtonIndex);
                    }));
                }
                else
                    radioButtonPannelAdd(rbnMatch, iButtonIndex);

            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void linkLivetickerToNewButton(IScore score, RadioButton rbnMatch)
        {
            if (score != null)
            {
                DebugWriter.Instance.WriteMessage("Statistics & Analyses", "Link Liveticker to Button");
                rbnMatch.Text = rbnMatch.Text + " " + score.League;
                rbnMatch.Enabled = true;
                score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(score_PlaytimeTickEvent);
                score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(score_RaiseGoalEvent);
                score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(score_BackGoalEvent);
                score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(score_GameEndedEvent);
                score.RedCardEvent += new EventHandler<RedCardEventArgs>(score_RedCardEvent);
                HLLiveScore hlScore = score as HLLiveScore;
                //hlScore.LiveScoreAddedEvent += ctlStatisticAnalyses_LiveScoreAddedEvent;
                if (hlScore != null && hlScore.Score1 != null && hlScore.Score2 != null)
                {
                    /*
                    Color color = Color.Transparent;

                    
                    ButtonColorStatHelper helper = new ButtonColorStatHelper();
                    Tuple<bool, Color> t = await helper.buttonStatColor(hlScore.TeamAId, hlScore.TeamBId, hlScore.TeamA, hlScore.TeamB, hlScore.League);
                    if (t.Item1)
                    {
                        rbnMatch.BackColor = color;
                    }
                    */
                    if (!SXLeagues.Instance.ContainsKey(hlScore.League))
                    {
                        SXLeagues.Instance.Add(hlScore.League, hlScore.League);
                    }
                }
                else if (hlScore != null && (hlScore.Score1 != null || hlScore.Score2 != null))
                {
                    rbnMatch.BackColor = Color.LightGray;

                    if (hlScore != null && hlScore.Score1 != null)
                    {
                        /*
                        Color color = Color.Transparent;

                        ButtonColorStatHelper helper = new ButtonColorStatHelper();
                        Tuple<bool, Color> t = await helper.buttonStatColor(hlScore.TeamAId, hlScore.TeamBId, hlScore.TeamA, hlScore.TeamB, hlScore.League);
                        if (t.Item1)
                        {
                            rbnMatch.BackColor = color;
                        }
                        */
                        if (!SXLeagues.Instance.ContainsKey(hlScore.League))
                        {
                            SXLeagues.Instance.Add(hlScore.League, hlScore.League);
                        }
                    }
                }
                else if (hlScore != null && hlScore.Score1 == null && hlScore.Score2 == null)
                {
                    rbnMatch.BackColor = Color.SlateGray;
                }



                if (DateTime.Now > score.StartDTS)
                {
                    if (score.Ended)
                    {
                        rbnMatch.BackColor = Color.Red;
                    }
                    else
                    {
                        rbnMatch.BackColor = Color.LightGreen;
                    }

                    rbnMatch.Text = rbnMatch.Text + "  /  " + score.getScore();
                }
            }
            else
            {
                rbnMatch.BackColor = Color.SlateGray;
            }
        }

      
        async void ctlStatisticAnalyses_LiveScoreAddedEvent(object sender, EventArgs e)
        {
            try
            {
                SXALMarket m = null;
                foreach (String btnName in _buttonList.Values)
                {
                    RadioButton btn = (RadioButton)this.pnlSplit.Panel1.Controls[btnName];
                    //Momentan wollen wir keine Unmanaged Markets
                    if ((btn.Tag as SXALMarket).IsUnmanaged) continue;
                    //Für die ButtonListe nehmen wir exemplarisch nur den MatchOdds-Markt.
                    if (!(btn.Tag as SXALMarket).IsMatchOdds) continue;
                    if ((btn.Tag as SXALMarket).Liveticker == (sender as IScore))
                    {
                        m = btn.Tag as SXALMarket;
                        break;
                    }
                }

                if (m == null)
                    return;

                DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("New Liveticker for Match {0}", m.Match));
                await Task.Run(() => { marketLivetickerAddedRunner(m); });
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void radioButtonPannelAdd(RadioButton rbn, int buttonIndex)
        {
            try
            {
                DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Add Button {0} to Panel", rbn.Name));
                this.pnlSplit.Panel1.Controls.Add(rbn);

                this.pnlSplit.Panel1.Controls.SetChildIndex(rbn, _buttonList.Count - buttonIndex + 1);
                _newButtonInserted = true;
            }
            catch (ArgumentException)
            {
                DebugWriter.Instance.WriteMessage("Statistics & Analyses", "Received an ArgumentExcpetion try the Delegate way");
                //delegateControlsAdd d = new delegateControlsAdd(this.pnlSplit.Panel1.Controls.Add);
                //delegateControlsSetChildIndex sci = new delegateControlsSetChildIndex(this.pnlSplit.Panel1.Controls.SetChildIndex);
                //IAsyncResult result = this.pnlSplit.Panel1.BeginInvoke(d, new object[] { rbn });
                //this.pnlSplit.Panel1.EndInvoke(result);

                this.pnlSplit.Panel1.Invoke(new MethodInvoker(() => { this.pnlSplit.Panel1.Controls.Add(rbn); }));

                int newIndex = _buttonList.Count - buttonIndex + 1;
                this.pnlSplit.Panel1.Invoke(new MethodInvoker(() => { this.pnlSplit.Panel1.Controls.SetChildIndex(rbn, newIndex); }));
                //result = this.pnlSplit.Panel1.BeginInvoke(sci, new object[] { rbn, newIndex });
                //this.pnlSplit.Panel1.EndInvoke(result);

                //this.pnlSplit.Panel1.Controls.SetChildIndex(rbnMatch, _buttonList.Count - iButtonIndex + 1);
                _newButtonInserted = true;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void e_MarketToBeRemoved(object sender, SXALMarketRemoveEventArgs e)
        {
            DebugWriter.Instance.WriteMessage("Statistics & Analyes", String.Format("Market {0} has been removed. Cleaning up Button List", e.Market.Match));
            if (this.InvokeRequired)
            {
                /*
                IAsyncResult ar = this.BeginInvoke(new EventHandler<SXALMarketRemoveEventArgs>(e_MarketToBeRemoved), new object[] { sender, e });
                if (ar.AsyncWaitHandle.WaitOne(60000))
                {
                    DebugWriter.Instance.WriteMessage("Statistics & Analyses", "e_MarketToBeRemoved received timeout");
                }
                else
                    this.EndInvoke(ar);                
                 */
                IAsyncResult result = this.BeginInvoke(new EventHandler<SXALMarketRemoveEventArgs>(e_MarketToBeRemoved), new object[] { sender, e });
                this.EndInvoke(result);
            }
            else
            {
                //lock (_buttonListLock)
                //{
                    try
                    {
                        String rbnToBeRemoved = String.Empty;
                        String keyToBeRemoved = String.Empty;
                        foreach (KeyValuePair<String, String> kvp in _buttonList)
                        {

                            SXALMarket m = this.pnlSplit.Panel1.Controls[kvp.Value].Tag as SXALMarket;//kvp.Value.Tag as SXALMarket;
                            if (m != null && m.Id == e.Market.Id)
                            {
                                rbnToBeRemoved = kvp.Value;
                                keyToBeRemoved = kvp.Key;
                                break;
                            }
                        }

                        if (rbnToBeRemoved != null)
                        {
                            this.pnlSplit.SuspendLayout();
                            _buttonList.Remove(keyToBeRemoved);                            
                            this.pnlSplit.Panel1.Controls.RemoveByKey(rbnToBeRemoved);
                            //rbnToBeRemoved.Dispose();
                            //rbnToBeRemoved = null;
                            this.pnlSplit.ResumeLayout(true);
                        }

                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                //}
            }
        }
        
        void rbnMatch_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<ButtonListAddButtonEventArgs>(rbnMatch_CheckedChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    RadioButton btn = sender as RadioButton;
                    if (btn != null)
                    {
                        if (btn.Checked)
                        {
                            Font boldFont = new Font(btn.Font.FontFamily, btn.Font.Size, FontStyle.Bold);
                            btn.Font = boldFont;
                        }
                        else
                        {
                            Font regularFont = new Font(btn.Font.FontFamily, btn.Font.Size, FontStyle.Regular);
                            btn.Font = regularFont;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }



        private async void loadTradeInfoBitmaps()
        {
            foreach (String btnName in _buttonList.Values.Reverse())
            {
                await Task.Run(() => loadTradeInfoBitmap(this.pnlSplit.Panel1.Controls[btnName] as RadioButton));
            }
        }

        private void loadTradeInfoBitmap(RadioButton rbnMatch)
        {
            try
            {
                SXALMarket m = rbnMatch.Tag as SXALMarket;
                if (m == null)
                    return;

                IScore score = m.Liveticker;

                if (score == null)
                    return;

                List<Bitmap> bitmapList = new List<Bitmap>();
                foreach (Control ctrl in pnlBets.Controls)
                {
                    try
                    {
                        IFastBet fastBet = ctrl as IFastBet;
                        if (fastBet == null)
                            continue;

                        if (score != null && !score.Ended)
                        {
                            Bitmap tmpBitmap = null;
                            if ((tmpBitmap = buildInfoImages2(fastBet, m.Id)) != null)
                                bitmapList.Add(tmpBitmap);
                            //buildInfoImages(rbnMatch, fastBet, e.Market.Id);
                        }
                    }
                    catch
                    {
                        // Nichts zu machen
                    }
                }

                Bitmap tmpRCBitmap = null;
                if ((tmpRCBitmap = buildRedCardImages(rbnMatch)) != null)
                    bitmapList.Add(tmpRCBitmap);

                DebugWriter.Instance.WriteMessage("Statistics & Analyses", "Update view of Information icons");
                updateButtonImages(rbnMatch, bitmapList);
            }
            catch(Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private async void colorButtons()
        {
            foreach (String btnName in _buttonList.Values.Reverse())
            {
                await colorButton(this.pnlSplit.Panel1.Controls[btnName] as RadioButton);
            }
        }

        private async Task colorButton(RadioButton btn)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            SXALMarket m = btn.Tag as SXALMarket;
            if (m == null)
                return;
             HLLiveScore hlScore = m.Liveticker as HLLiveScore;
                //hlScore.LiveScoreAddedEvent += ctlStatisticAnalyses_LiveScoreAddedEvent;
             if (hlScore != null && hlScore.Score1 != null)
             {

                 Color color = Color.Transparent;


                 ButtonColorStatHelper helper = new ButtonColorStatHelper();
                 Tuple<bool, Color> t = await helper.buttonStatColor(hlScore.TeamAId, hlScore.TeamBId, hlScore.TeamA, hlScore.TeamB, hlScore.League);
                 if (t.Item1)
                 {
                     if (btn.InvokeRequired)
                     {
                         btn.Invoke((MethodInvoker)(() =>
                         {
                             btn.BackColor = t.Item2;
                         }
                          ));
                        
                     }
                     else
                         btn.BackColor = t.Item2;
                 }
             }            
        }

        //Initialer Aufbau der Begegnungsliste
        private async Task  initialList()
        {
            try
            {

                //SortedList<long, SXALMarket> inplayMarkets = SXALSoccerMarketManager.Instance.InPlayMarkets;

                foreach (SXALMarket market in SXALSoccerMarketManager.Instance.InPlayMarkets.Values)
                {
                    if (market.IsUnmanaged) continue;

                    //Nur Match Odds Markets
                    if (!market.IsMatchOdds) continue;

                    await addButtons(market);

                }


                sortButtons();
                colorButtons();
                loadTradeInfoBitmaps();

                
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            _firstLoad = false;
        }

        /// <summary>
        /// Sortiere die Buttons in Reihenfolge des Startdatums
        /// </summary>
        private void sortButtons()
        {
            if (!_newButtonInserted)
                return;

            foreach (String btnName in _buttonList.Values.Reverse())
            {
                RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;
                if (!this.pnlSplit.Panel1.Controls.Contains(btn))
                    this.pnlSplit.Panel1.Controls.Add(btn);

                int indexOld = this.pnlSplit.Panel1.Controls.GetChildIndex(btn);
                int indexNew = _buttonList.IndexOfValue(btnName);
                if ((_buttonList.Count - indexOld) == indexNew)
                    continue;
                this.pnlSplit.Panel1.Controls.SetChildIndex(btn, _buttonList.Count - indexNew);
            }

            _newButtonInserted = false;

            if (_firstLoad)
            {
                _firstLoad = false;
                //this.pnlSplit.Panel1.ResumeLayout();
            }
        }

        //TODO: Zu entfernen
        void _buttonBuilderHelper_CompleteButtonListEvent(object sender, EventArgs e)
        {
            try
            {
                if (lblButtons.InvokeRequired)
                {
                    try
                    {
                        IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(_buttonBuilderHelper_CompleteButtonListEvent), new object[] { sender, e });
                        this.EndInvoke(result);
                    }
                    catch (ArgumentException ae)
                    {
                        ExceptionWriter.Instance.WriteException(ae);
                    }
                }
                else
                {
                    //lock (_buttonListLock)
                    //{
                        if (!_newButtonInserted)
                            return;

                        foreach (String btnName in _buttonList.Values.Reverse())
                        {

                            RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;

                            if (!this.pnlSplit.Panel1.Controls.Contains(btn))
                                this.pnlSplit.Panel1.Controls.Add(btn);

                            int indexOld = this.pnlSplit.Panel1.Controls.GetChildIndex(btn);
                            int indexNew = _buttonList.IndexOfValue(btnName);
                            if ((_buttonList.Count - indexOld) == indexNew)
                                continue;
                            this.pnlSplit.Panel1.Controls.SetChildIndex(btn, _buttonList.Count - indexNew);
                        }                        
                        _newButtonInserted = false;
                        if (_firstLoad)
                        {                            
                            _firstLoad = false;
                            //this.pnlSplit.Panel1.ResumeLayout();
                        }
                    //}
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void _buttonBuilderHelper_StartButtonListEvent(object sender, EventArgs e)
        {
            /*
            if (_firstLoad)
            {
                lblButtons.Text = "Initial Load!\r\nPlease wait until markets are loaded.";
                this.pnlSplit.Panel1.SuspendLayout();
            }
             * */
        }

        public IPluginHost PluginHost
        {
            set
            {
                try
                {
                    _pluginHost = value;
                    IPlugin[] plugins = _pluginHost.PluginsArray;
                    foreach (IPlugin plugin in plugins)
                    {
                        UserControl ctl = plugin.FastBetInterface;
                        if (ctl != null)
                        {
                            
                            if (!pnlBets.Controls.Contains(ctl))
                            {
                                IFastBet fastBet = ctl as IFastBet;
                                if (fastBet != null)
                                {
                                    fastBet.IPSAdded += new EventHandler<IPSAddedEventArgs>(ctlStatisticAnalyses_IPSAdded);
                                    fastBet.IPSDeleted += new EventHandler<IPSDeletedEventArgs>(ctlStatisticAnalyses_IPSDeleted);
                                    fastBet.IPSBetAdded += new EventHandler<IPSBetAddedEventArgs>(ctlStatisticAnalyses_IPSBetAdded);
                                    fastBet.IPSLoadGUI += new EventHandler<LoadAutoTradeEventArgs>(ctlStatisticAnalyses_IPSLoadGUI);
                                    fastBet.IPSUnloadGUI += new EventHandler<UnloadAutoTradeEventArgs>(ctlStatisticAnalyses_IPSUnloadGUI);
                                    ctl.Dock = DockStyle.Left;
                                    pnlBets.Controls.Add(ctl);
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

        void ctlStatisticAnalyses_IPSUnloadGUI(object sender, UnloadAutoTradeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<UnloadAutoTradeEventArgs>(ctlStatisticAnalyses_IPSUnloadGUI), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    this.Controls.Remove(e.GUI);
                    e.GUI.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void ctlStatisticAnalyses_IPSLoadGUI(object sender, LoadAutoTradeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<LoadAutoTradeEventArgs>(ctlStatisticAnalyses_IPSLoadGUI), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    e.GUI.Dock = DockStyle.Fill;
                    this.Controls.Add(e.GUI);
                    e.GUI.BringToFront();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void ctlStatisticAnalyses_IPSBetAdded(object sender, IPSBetAddedEventArgs e)
        {
            try
            {               
                ThreadPool.QueueUserWorkItem(new WaitCallback(ipsUpdaterRunner), e.MarketId);
                //delegateIPSUpdate ipsUpdate = new delegateIPSUpdate(this.ipsUpdaterRunner);
                //this.BeginInvoke(ipsUpdate);

            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void ctlStatisticAnalyses_IPSDeleted(object sender, IPSDeletedEventArgs e)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ipsUpdaterRunner), e.MarketID);
                //delegateIPSUpdate ipsUpdate = new delegateIPSUpdate(this.ipsUpdaterRunner);
                //this.BeginInvoke(ipsUpdate);

            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

       private void ctlStatisticAnalyses_IPSAdded(object sender, IPSAddedEventArgs e)
        {
            try
            {                               
                ThreadPool.QueueUserWorkItem(new WaitCallback(ipsUpdaterRunner), e.MarketID);
                //delegateIPSUpdate ipsUpdate = new delegateIPSUpdate(this.ipsUpdaterRunner);
                //this.BeginInvoke(ipsUpdate);               
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

       private void ipsUpdaterRunner(object stateInfo)
       {
           try
           {
               if (stateInfo != null)
               {
                    long marketId = (long)stateInfo;
                    SXALMarket market = SXALSoccerMarketManager.Instance.getMarketById(marketId, false);
                    if (market == null)
                       return;

                    String key = market.StartDTS.ToString("dd.MM.yyyy HH:mm") + " " +
                                    market.Match;
                    if (!_buttonList.ContainsKey(key))
                       return;
                    RadioButton btn = this.pnlSplit.Panel1.Controls[_buttonList[key]] as RadioButton;

                    List<Bitmap> bitmapList = new List<Bitmap>();
                    foreach (Control ctrl in pnlBets.Controls)
                    {
                        try
                        {
                            IFastBet fastBet = ctrl as IFastBet;
                            if (fastBet == null)
                                continue;

                            if (market.Liveticker != null && !market.Liveticker.Ended)
                            {
                                Bitmap tmpBitmap = null;
                                if ((tmpBitmap = buildInfoImages2(fastBet, market.Id)) != null)
                                    bitmapList.Add(tmpBitmap);
                                //buildInfoImages(rbnMatch, fastBet, e.Market.Id);
                            }
                        }
                    catch
                    {
                        // Nichts zu machen
                    }
                }

                    Bitmap tmpRCBitmap = null;
                    if ((tmpRCBitmap = buildRedCardImages(btn)) != null)
                        bitmapList.Add(tmpRCBitmap);


                    updateButtonImages(btn, bitmapList);
                   
               }
               else
               {
                   int i = 0;
                   foreach (String btnName in _buttonList.Values)
                   {
                       RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;
                       SXALMarket market = (SXALMarket)btn.Tag;
                       List<Bitmap> bitmapList = new List<Bitmap>();
                       foreach (Control ctrl in pnlBets.Controls)
                       {
                           try
                           {
                               IFastBet fastBet = ctrl as IFastBet;
                               if (fastBet == null)
                                   continue;

                               if (market.Liveticker != null && !market.Liveticker.Ended)
                               {
                                   Bitmap tmpBitmap = null;
                                   if ((tmpBitmap = buildInfoImages2(fastBet, market.Id)) != null)
                                       bitmapList.Add(tmpBitmap);
                                   //buildInfoImages(rbnMatch, fastBet, e.Market.Id);
                               }
                           }
                           catch
                           {
                               // Nichts zu machen
                           }
                       }

                       Bitmap tmpRCBitmap = null;
                       if ((tmpRCBitmap = buildRedCardImages(btn)) != null)
                           bitmapList.Add(tmpRCBitmap);


                       updateButtonImages(btn, bitmapList);
                   }
               }
           }
           catch (Exception exc)
           {
               ExceptionWriter.Instance.WriteException(exc);
           }
       }
          
                
        private void score_RedCardEvent(object sender, RedCardEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<RedCardEventArgs>(score_RedCardEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    //lock (_ipsUpdateLock)
                    //{
                        IScore score = (IScore)sender;
                        foreach (String btnName in _buttonList.Values)
                        {
                            RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;
                            SXALMarket market = (SXALMarket)btn.Tag;
                            try
                            {
                                if (market.Liveticker == score)
                                {
                                    List<Bitmap> bitmapList = new List<Bitmap>();
                                    foreach (Control ctrl in pnlBets.Controls)
                                    {
                                        try
                                        {
                                            IFastBet fastBet = ctrl as IFastBet;
                                            if (fastBet == null)
                                                continue;

                                            if (market.Liveticker != null && !market.Liveticker.Ended)
                                            {
                                                Bitmap tmpBitmap = null;
                                                if ((tmpBitmap = buildInfoImages2(fastBet, market.Id)) != null)
                                                    bitmapList.Add(tmpBitmap);
                                            }
                                        }
                                        catch
                                        {
                                            // Nichts zu machen
                                        }
                                    }

                                    Bitmap tmpRCBitmap = null;
                                    if ((tmpRCBitmap = buildRedCardImages(btn)) != null)
                                        bitmapList.Add(tmpRCBitmap);


                                    updateButtonImages(btn, bitmapList);

                                    return;
                                }
                            }
                            catch (KeyNotFoundException knfe)
                            {
                                DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Could not find an entry in HLList for match {0}", ((IScore)sender).BetfairMatch));
                                ExceptionWriter.Instance.WriteException(knfe);
                            }
                        }
                    }

                //}
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        async void rbnMatch_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    /*
                    IAsyncResult ar = this.BeginInvoke(new EventHandler<MouseEventArgs>(rbnMatch_MouseClick), new object[] { sender, e });
                    if (!ar.AsyncWaitHandle.WaitOne(60000))
                    {
                        DebugWriter.Instance.WriteMessage("Statistics & Analyses", "rbnMatch_MouseClick received timeout");
                    }
                    else
                        this.EndInvoke(ar);
                     */

                    IAsyncResult result = this.BeginInvoke(new EventHandler<MouseEventArgs>(rbnMatch_MouseClick), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    if (_firstLoad)
                    {
                        MessageBox.Show("Initial Load!\r\nPlease wait until markets are loaded.", "Initial Load", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    RadioButton rbn = sender as RadioButton;
                    if (rbn == null)
                        return; 

                    SXALMarket market = rbn.Tag as SXALMarket;

                    if (market == null)
                        return;

                    //Falls kein Livescore vorhanden
                    if (e.Button == MouseButtons.Left &&
                        (market.Liveticker == null || !(market.Liveticker as HLLiveScore).IsScore1Connected() || !(market.Liveticker as HLLiveScore).IsScore2Connected()))
                    {

                        market.doManualLivetickerConnection();
                        
                    }

                    rbn = (RadioButton)sender;
                    market = rbn.Tag as SXALMarket;
                    if(market == null)
                        return;

                    HLLiveScore livescore =  market.Liveticker as HLLiveScore;
                    if (e.Button == MouseButtons.Left && livescore != null && livescore.IsScore1Connected())
                    {
                        //TODO: Statistische Daten holen               
                        // Wenn schon geladen, dann weiter
                        if (market == pnlSplitRight.Panel1.Tag)
                            return;

                        pnlSplitRight.Panel1.Tag = market;
                        // Holen Datenservice
                        //TODO: Asynchron
                        await loadStats(market, livescore, rbn);
                        
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private async Task loadStats(SXALMarket market, IScore livescore, RadioButton rbn)
        {
            IHistoricDataService historicService = HistoricDataServiceFactory.getInstance(livescore.TeamAId, livescore.TeamBId, market.TeamA, market.TeamB);
            SAConfigurationRW config = new SAConfigurationRW();
            HistoricDataStatistic dataStatistic = null;
            try
            {
                dataStatistic = await historicService.GetStatistic(livescore.TeamAId, livescore.TeamBId,
                    livescore.TeamA, livescore.TeamB, livescore.League, config.NoOfData, config.AgeOfData);
            }
            catch (NoHistoricDataException)
            {
                historicService = HistoricDataServiceFactory.getInstance(livescore.TeamAId, livescore.TeamBId, market.TeamA, market.TeamB);
                try
                {                   
                    dataStatistic = historicService.GetStatistic(livescore.TeamAId, livescore.TeamBId,
                        livescore.TeamA, livescore.TeamB, livescore.League, config.NoOfData, config.AgeOfData).Result;
                    
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }


            if (dataStatistic != null)
            {
                //GUI aufbauen.
                while (this.pnlSplitStatsTop.Panel1.Controls.Count > 0)
                {
                    this.pnlSplitStatsTop.Panel1.Controls[0].Dispose();
                }
                ctlHistoricDataView directView = new ctlHistoricDataView();
                directView.Titel = HistoryGraph.strDirectConfrontations;
                directView.NameInformations = market;
                directView.MatchList = dataStatistic.Direct;
                directView.Dock = DockStyle.Fill;
                directView.ShowMatchDetail += new EventHandler<ShowMatchDetailEventArgs>(showMatchDetailHandler);
                directView.MoreStatisticsEvent += new EventHandler<MoreStatisticsEventArgs>(moreStatisticsEventHandler);
                pnlSplitStatsTop.Panel1.Controls.Add(directView);


                while (this.pnlSplitStatsTop.Panel2.Controls.Count > 0)
                {
                    this.pnlSplitStatsTop.Panel2.Controls[0].Dispose();
                }
                ctlHistoricDataView teamAView = new ctlHistoricDataView();
                teamAView.Titel = String.Format(HistoryGraph.strTeamHistDataFormat, livescore.TeamA);
                teamAView.NameInformations = market;
                teamAView.MatchList = dataStatistic.TeamA;
                teamAView.Dock = DockStyle.Fill;
                teamAView.ShowMatchDetail += new EventHandler<ShowMatchDetailEventArgs>(showMatchDetailHandler);
                teamAView.MoreStatisticsEvent += new EventHandler<MoreStatisticsEventArgs>(moreStatisticsEventHandler);
                pnlSplitStatsTop.Panel2.Controls.Add(teamAView);

                while (this.pnlSplitStats.Panel2.Controls.Count > 0)
                {
                    this.pnlSplitStats.Panel2.Controls[0].Dispose();
                }
                ctlHistoricDataView teamBView = new ctlHistoricDataView();
                teamBView.Titel = String.Format(HistoryGraph.strTeamHistDataFormat, livescore.TeamB);
                teamBView.NameInformations = market;
                teamBView.MatchList = dataStatistic.TeamB;
                teamBView.Dock = DockStyle.Fill;
                teamBView.ShowMatchDetail += new EventHandler<ShowMatchDetailEventArgs>(showMatchDetailHandler);
                teamBView.MoreStatisticsEvent += new EventHandler<MoreStatisticsEventArgs>(moreStatisticsEventHandler);
                pnlSplitStats.Panel2.Controls.Add(teamBView);

                while (pnlMatchDetail.Controls.Count > 0)
                {
                    pnlMatchDetail.Controls[0].Dispose();
                }
            }
            //Fast Bet Componenten Aktualisieren

            List<Bitmap> bitmapList = new List<Bitmap>();
            foreach (Control ctrl in pnlBets.Controls)
            {
                try
                {
                    IFastBet fastBet = ctrl as IFastBet;
                    if (fastBet == null)
                        continue;

                    fastBet.Market = market;
                    fastBet.LiveScore = livescore;

                    if (livescore != null && !livescore.Ended)
                    {
                        Bitmap tmpBitmap = null;
                        if ((tmpBitmap = buildInfoImages2(fastBet, market.Id)) != null)
                            bitmapList.Add(tmpBitmap);
                        //buildInfoImages(rbnMatch, fastBet, e.Market.Id);
                    }
                }
                catch
                {
                    // Nichts zu machen
                }
            }

            Bitmap tmpRCBitmap = null;
            if ((tmpRCBitmap = buildRedCardImages(rbn)) != null)
                bitmapList.Add(tmpRCBitmap);


            updateButtonImages(rbn, bitmapList);
            return;
        }

        void moreStatisticsEventHandler(object sender, MoreStatisticsEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<MoreStatisticsEventArgs>(moreStatisticsEventHandler), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    Statistics.ctlMoreStatistics ctl = new net.sxtrader.bftradingstrategies.common.Statistics.ctlMoreStatistics();
                    ctl.LeaveStatisticsEvent += new EventHandler<net.sxtrader.bftradingstrategies.common.Statistics.LeaveStatisticsEventArgs>(leaveStatisticsEventHandler);
                    ctl.Dock = DockStyle.Fill;

                    // Aktuell gewähltes Spiel ausfindig machen
                    foreach (String btnName in _buttonList.Values)
                    {
                        RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;
                        if (btn.Checked)
                        {
                            // Wir haben das gewählte Match gefunden; Nun die Daten lesen.
                            try
                            {
                                SXALMarket market = (SXALMarket)btn.Tag;
                                if (market != null)
                                {
                                    ctl.Livescore = market.Liveticker;
                                    ctl.loadStatistics();
                                }
                                break;
                            }
                            catch (Exception exc)
                            {
                                ExceptionWriter.Instance.WriteException(exc);
                                return;
                            }

                        }
                    }

                    //pnlStatistics.Controls.Add(ctl);
                    this.Controls.Add(ctl);
                    ctl.BringToFront();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void leaveStatisticsEventHandler(object sender, net.sxtrader.bftradingstrategies.common.Statistics.LeaveStatisticsEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<LeaveStatisticsEventArgs>(leaveStatisticsEventHandler), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    Statistics.ctlMoreStatistics ctl = (Statistics.ctlMoreStatistics)sender;
                    this.Controls.Remove(ctl);
                    ctl.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void showMatchDetailHandler(object sender, ShowMatchDetailEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<ShowMatchDetailEventArgs>(showMatchDetailHandler), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    while(pnlMatchDetail.Controls.Count > 0)
                    {
                        pnlMatchDetail.Controls[0].Dispose();
                    }

                    ctlMatchDetail matchDetailView = new ctlMatchDetail();
                    matchDetailView.TheMatch = e.TheMatch;
                    matchDetailView.Dock = DockStyle.Fill;
                    pnlMatchDetail.Controls.Add(matchDetailView);
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
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(tsiLivescore_Click), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd dlg = new net.sxtrader.bftradingstrategies.livescoreparser.frmManualAdd();
                    dlg.IsLiveScore2 = false;
                    dlg.Match = String.Empty;
                    DialogResult result = dlg.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        RadioButton rbn = (RadioButton)((ToolStripMenuItem)sender).Tag;
                        rbn.Tag = dlg.Livescore;
                        rbn.BackColor = SystemColors.Control;
                        rbn.Enabled = true;
                    }

                    dlg.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        /*
        private IScore linkLiveScore(SXALMarket market)
        {
            try
            {
                LiveScoreParser parser = LiveScoreParser.Instance;
                String[] sepMatch = { " - " };
                String[] teams = market.Match.Split(sepMatch, StringSplitOptions.None);//SoccerMarketManager.Instance.getMatchById(m_id).Split(sepMatch,StringSplitOptions.None);
                IScore score = parser.linkSportExchange(teams[0], teams[1]);

                LiveScore2Parser parser2 = LiveScore2Parser.Instance;
                IScore score2 = parser2.linkSportExchange(teams[0], teams[1]);

                HLLiveScore hlScore = new HLLiveScore(score, score2);
                return hlScore;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return null;
        }
         */

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlStatistics_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(pnlStatistics_SizeChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    //pnlDirect.Height = pnlTeamA.Height = pnlTeamB.Height = pnlSplitRight.Panel1.Height / 3;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private async void ctlStatisticAnalyses_Load(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    initialList();
                }));
            }
            else
            {
                await initialList();
                /*
                MessageBox.Show("Hello Trader, \r\n the Statistic Database is down for maintenance.\r\nIt will be back approx. mid of August 2011.\r\n" +
                    "Sorry for the inconvenience.", "Statistic Database down", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 * */
            }
        }

        private void ctlStatisticAnalyses_DockChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(ctlStatisticAnalyses_DockChanged), new object[] { sender, e });
                this.EndInvoke(result);
            }
            else
            {
            }
        }

        private void ctlStatisticAnalyses_EnabledChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(ctlStatisticAnalyses_EnabledChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    // Wenn Control aktiviert wird, so wird die Buttonlist neu aufgebaut.
                    if (((ctlStatisticAnalyses)sender).Enabled == true)
                    {
                        //_buttonBuilderHelper.start();
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void score_PlaytimeTickEvent(object sender, PlaytimeTickEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        IAsyncResult result = this.BeginInvoke(new EventHandler<PlaytimeTickEventArgs>(score_PlaytimeTickEvent), new object[] { sender, e });
                        this.EndInvoke(result);
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                }
                else
                {
                    IScore score = (IScore)sender;
                    foreach (String btnName in _buttonList.Values)
                    {
                        try
                        {
                            RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;
                            SXALMarket market = (SXALMarket)btn.Tag;
                            if (market.Liveticker != null && market.Liveticker == score)
                            {
                                if (!score.Ended && btn.BackColor != Color.LightGreen)
                                {
                                    btn.BackColor = Color.LightGreen;
                                }
                                if (score.Ended)
                                {
                                    btn.BackColor = Color.Red;
                                }
                                //tagClass.Market.StartDTS.ToShortDateString()

                                String strBtnText = market.StartDTS.ToString("dd.MM.yyyy HH:mm") + " " +
                                    market.Match + " " + market.Liveticker.League + "  /  " + market.Liveticker.getScore() +
                                    "  /  '" + e.Tick.ToString();

                                if (btn.Image != null)
                                {
                                    btn.SuspendLayout();
                                    btn.Text = "\r\n\r\n\r\n\r\n" + strBtnText;
                                    btn.ResumeLayout();
                                }
                                else
                                {
                                    btn.Text = strBtnText;
                                }
                                return;
                            }
                        }
                        catch (KeyNotFoundException knfe)
                        {
                            DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Could not find an entry in HLList for match {0}", e.Match));
                            ExceptionWriter.Instance.WriteException(knfe);
                        }
                    }
                }
            }           
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void score_RaiseGoalEvent(object sender, GoalEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<GoalEventArgs>(score_RaiseGoalEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    IScore score = (IScore)sender;
                    foreach (String btnName in _buttonList.Values)
                    {
                        RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;
                        SXALMarket market = (SXALMarket)btn.Tag;
                        try
                        {
                            if ( market.Liveticker != null && market.Liveticker == score)
                            {
                                String strBtnText = market.StartDTS.ToString("dd.MM.yyyy HH:mm") + " " +
                                    market.Match + " " + market.Liveticker.League + "  /  " + market.Liveticker.getScore();

                                if (btn.Image != null)
                                {
                                    btn.SuspendLayout();
                                    btn.Text = "\r\n\r\n\r\n\r\n" + strBtnText;
                                    btn.ResumeLayout();
                                }
                                else
                                {
                                    btn.Text = strBtnText;
                                }
                                return;
                            }
                        }
                        catch (KeyNotFoundException knfe)
                        {
                            DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Could not find an entry in HLList for match {0}", ((IScore)sender).BetfairMatch));
                            ExceptionWriter.Instance.WriteException(knfe);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void score_BackGoalEvent(object sender, GoalBackEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<GoalBackEventArgs>(score_BackGoalEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    IScore score = (IScore)sender;
                    foreach (String btnName in _buttonList.Values)
                    {
                        try
                        {
                            RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;
                            SXALMarket market = (SXALMarket)btn.Tag;
                            if (market.Liveticker != null && market.Liveticker == score)
                            {

                                String strBtnText = market.StartDTS.ToString("dd.MM.yyyy HH:mm") + " " +
                                    market.Match + " " + market.Liveticker.League + "  /  " + market.Liveticker.getScore();

                                if (btn.Image != null)
                                {
                                    btn.SuspendLayout();
                                    btn.Text = "\r\n\r\n\r\n\r\n" + strBtnText;
                                    btn.ResumeLayout();
                                }
                                else
                                {
                                    btn.Text = strBtnText;
                                }
                                return;
                            }
                        }
                        catch (KeyNotFoundException knfe)
                        {
                            DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Could not find an entry in HLList for match {0}", ((IScore)sender).BetfairMatch ));
                            ExceptionWriter.Instance.WriteException(knfe);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void score_GameEndedEvent(object sender, GameEndedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<GameEndedEventArgs>(score_GameEndedEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    IScore score = (IScore)sender;
                    foreach (String btnName in _buttonList.Values)
                    {
                        RadioButton btn = this.pnlSplit.Panel1.Controls[btnName] as RadioButton;
                        SXALMarket market = (SXALMarket)btn.Tag;
                        try
                        {
                            if (market != null && market.Liveticker == score)
                            {
                                btn.SuspendLayout();
                                btn.BackColor = Color.Red;
                                btn.Image = null;
                                btn.Text = btn.Text.Trim();
                                btn.ResumeLayout();
                                return;
                            }
                        }
                        catch (KeyNotFoundException knfe)
                        {
                            DebugWriter.Instance.WriteMessage("Statistics & Analyses", String.Format("Could not find an entry in HLList for match {0}", ((IScore)sender).BetfairMatch));
                            ExceptionWriter.Instance.WriteException(knfe);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
        
        private void updateButtonImages(RadioButton btn, List<Bitmap> bitmaps)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke(new MethodInvoker(() => { updateButtonImages(btn, bitmaps); }));
                //IAsyncResult result = btn.BeginInvoke(new delegateUpdateButtonImages(updateButtonImages), new object[] { btn, bitmaps });
                //btn.EndInvoke(result);
            }
            else
            {
                try
                {
                    btn.SuspendLayout();
                    if (btn.Image != null)
                        btn.Image.Dispose();
                    btn.Image = null;
                    btn.ImageAlign = ContentAlignment.TopLeft;


                    System.Drawing.Bitmap finalImage = null;

                    int width = 0;
                    int height = 0;

                    //create a bitmap to hold the combined image
                    if (bitmaps.Count > 0)
                    {
                        foreach (Bitmap image in bitmaps)
                        {
                            if (image.Height > height)
                                height = image.Height;

                            width += image.Width;

                        }
                        finalImage = new System.Drawing.Bitmap(width, height);

                        //get a graphics object from the image so we can draw on it
                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                        {
                            //set background color

                            //go through each image and draw it on the final image
                            int offset = 0;
                            foreach (System.Drawing.Bitmap image in bitmaps)
                            {
                                g.DrawImage(image,
                                  new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                                offset += image.Width;
                            }
                        }
                        btn.Image = finalImage;
                        btn.Text = btn.Text.Trim();
                        //TODO: dynamische Höhenberechnung
                        btn.Text = "\r\n\r\n\r\n\r\n" + btn.Text;


                        //clean up memory                    
                        while (bitmaps.Count > 0)
                        {
                            bitmaps[0].Dispose();
                            bitmaps.RemoveAt(0);
                        }
                    }
                    btn.ResumeLayout();
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private Bitmap buildInfoImages2(IFastBet fastBet, long marketId)
        {
            try
            {
                List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
                System.Drawing.Bitmap finalImage = null;

                int width = 0;
                int height = 0;

                if (fastBet.HasMarketIPS(marketId))
                {
                    Bitmap bitmap = fastBet.GetIPSBitmap();//Resourcen.Resourcen.IPS.ToBitmap();
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                if (fastBet.HasMarketTrade(marketId))
                {
                    Bitmap bitmap = fastBet.GetTradeBitmap();//Resourcen.Resourcen.LTD.ToBitmap();
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                if (images.Count > 0)
                {
                    finalImage = new System.Drawing.Bitmap(width, height);

                    //get a graphics object from the image so we can draw on it
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                    {
                        //set background color

                        //go through each image and draw it on the final image
                        int offset = 0;
                        foreach (System.Drawing.Bitmap image in images)
                        {
                            g.DrawImage(image,
                              new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                            offset += image.Width;
                        }
                    }
                }

                return finalImage;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

            return null;
        }

        private Bitmap buildRedCardImages(RadioButton btn)
        {
            try
            {
                List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
                System.Drawing.Bitmap finalImage = null;

                SXALMarket market = (SXALMarket)btn.Tag;
                int width = 0;
                int height = 0;

                if (market != null && market.Liveticker != null && !market.Liveticker.Ended)
                {
                    int redA = market.Liveticker.RedA;
                    Bitmap bitmapRedA = null;
                    switch (redA)
                    {
                        case 0:
                            {
                                break;
                            }
                        case 1:
                            {
                                bitmapRedA = Resourcen.Resourcen.RedCardA1.ToBitmap();
                                break;
                            }
                        case 2:
                            {
                                bitmapRedA = Resourcen.Resourcen.RedCardA2.ToBitmap();
                                break;
                            }
                        case 3:
                            {
                                bitmapRedA = Resourcen.Resourcen.RedCardA3.ToBitmap();
                                break;
                            }
                        default:
                            {
                                bitmapRedA = Resourcen.Resourcen.RedCardA3Plus.ToBitmap();
                                break;
                            }
                    }

                    if (bitmapRedA != null)
                    {
                        width += bitmapRedA.Width;
                        height = bitmapRedA.Height > height ? bitmapRedA.Height : height;

                        images.Add(bitmapRedA);
                    }

                    int redB = market.Liveticker.RedB;
                    Bitmap bitmapRedB = null;
                    switch (redB)
                    {
                        case 0:
                            {
                                break;
                            }
                        case 1:
                            {
                                bitmapRedB = Resourcen.Resourcen.RedCardB1.ToBitmap();
                                break;
                            }
                        case 2:
                            {
                                bitmapRedB = Resourcen.Resourcen.RedCardB2.ToBitmap();
                                break;
                            }
                        case 3:
                            {
                                bitmapRedB = Resourcen.Resourcen.RedCardB3.ToBitmap();
                                break;
                            }
                        default:
                            {
                                bitmapRedB = Resourcen.Resourcen.RedCardB3Plus.ToBitmap();
                                break;
                            }
                    }

                    if (bitmapRedB != null)
                    {
                        width += bitmapRedB.Width;
                        height = bitmapRedB.Height > height ? bitmapRedB.Height : height;

                        images.Add(bitmapRedB);
                    }
                }
                //create a bitmap to hold the combined image
                if (images.Count > 0)
                {
                    finalImage = new System.Drawing.Bitmap(width, height);

                    //get a graphics object from the image so we can draw on it
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                    {
                        //set background color

                        //go through each image and draw it on the final image
                        int offset = 0;
                        foreach (System.Drawing.Bitmap image in images)
                        {
                            g.DrawImage(image,
                              new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                            offset += image.Width;
                        }
                    }
                }

                return finalImage;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return null;
        }

        private void buildInfoImages(RadioButton btn, IFastBet fastBet, int marketId)
        {
            try
            {
                btn.SuspendLayout();
                if (btn.Image != null)
                    btn.Image.Dispose();
                btn.Image = null;
                btn.ImageAlign = ContentAlignment.TopLeft;

                //read all images into memory
                List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
                System.Drawing.Bitmap finalImage = null;

                SXALMarket market = (SXALMarket)btn.Tag;//new SAButtonHelperClass();

                int width = 0;
                int height = 0;

                if (fastBet.HasMarketIPS(marketId))
                {
                    Bitmap bitmap = fastBet.GetIPSBitmap();//Resourcen.Resourcen.IPS.ToBitmap();
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                if (fastBet.HasMarketTrade(marketId))
                {
                    Bitmap bitmap = fastBet.GetTradeBitmap();//Resourcen.Resourcen.LTD.ToBitmap();
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                if (market != null && market.Liveticker != null && !market.Liveticker.Ended)
                {
                    int redA = market.Liveticker.RedA;
                    Bitmap bitmapRedA = null;
                    switch (redA)
                    {
                        case 0:
                            {
                                break;
                            }
                        case 1:
                            {
                                bitmapRedA = Resourcen.Resourcen.RedCardA1.ToBitmap();
                                break;
                            }
                        case 2:
                            {
                                bitmapRedA = Resourcen.Resourcen.RedCardA2.ToBitmap();
                                break;
                            }
                        case 3:
                            {
                                bitmapRedA = Resourcen.Resourcen.RedCardA3.ToBitmap();
                                break;
                            }
                        default:
                            {
                                bitmapRedA = Resourcen.Resourcen.RedCardA3Plus.ToBitmap();
                                break;
                            }
                    }

                    if (bitmapRedA != null)
                    {
                        width += bitmapRedA.Width;
                        height = bitmapRedA.Height > height ? bitmapRedA.Height : height;

                        images.Add(bitmapRedA);
                    }

                    int redB = market.Liveticker.RedB;
                    Bitmap bitmapRedB = null;
                    switch (redB)
                    {
                        case 0:
                            {
                                break;
                            }
                        case 1:
                            {
                                bitmapRedB = Resourcen.Resourcen.RedCardB1.ToBitmap();
                                break;
                            }
                        case 2:
                            {
                                bitmapRedB = Resourcen.Resourcen.RedCardB2.ToBitmap();
                                break;
                            }
                        case 3:
                            {
                                bitmapRedB = Resourcen.Resourcen.RedCardB3.ToBitmap();
                                break;
                            }
                        default:
                            {
                                bitmapRedB = Resourcen.Resourcen.RedCardB3Plus.ToBitmap();
                                break;
                            }
                    }

                    if (bitmapRedB != null)
                    {
                        width += bitmapRedB.Width;
                        height = bitmapRedB.Height > height ? bitmapRedB.Height : height;

                        images.Add(bitmapRedB);
                    }
                }
                //create a bitmap to hold the combined image
                if (images.Count > 0)
                {
                    finalImage = new System.Drawing.Bitmap(width, height);

                    //get a graphics object from the image so we can draw on it
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                    {
                        //set background color

                        //go through each image and draw it on the final image
                        int offset = 0;
                        foreach (System.Drawing.Bitmap image in images)
                        {
                            g.DrawImage(image,
                              new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                            offset += image.Width;
                        }
                    }
                    btn.Image = finalImage;
                    btn.Text = btn.Text.Trim();
                    //TODO: dynamische Höhenberechnung
                    btn.Text = "\r\n\r\n\r\n\r\n" + btn.Text;


                    //clean up memory
                    //foreach (System.Drawing.Bitmap image in images)
                    while (images.Count > 0)
                    {
                        images[0].Dispose();
                    }
                }
                btn.ResumeLayout();
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

        }

        static void CallbackMethod(IAsyncResult ar)
        {
            /*
            AsyncResult result = (AsyncResult)ar;
            delegateAddButton caller = (delegateAddButton)result.AsyncDelegate;
            ar.AsyncWaitHandle.Close();
            caller.EndInvoke(ar);
             */
        }

        static void CallbackMethod2(IAsyncResult ar)
        {
            /*
            AsyncResult result = (AsyncResult)ar;
            delegateLivetickerButtonUpdate caller = (delegateLivetickerButtonUpdate)result.AsyncDelegate;
            ar.AsyncWaitHandle.Close();
            caller.EndInvoke(ar);
             */
            
        }

        static void CallbackMethod3(IAsyncResult ar)
        {
            /*
            AsyncResult result = (AsyncResult)ar;
            delegateRadioButtonPanelAdd caller = (delegateRadioButtonPanelAdd)result.AsyncDelegate;            
            ar.AsyncWaitHandle.Close();
            caller.EndInvoke(ar);
             */
        }

        private void pnlSplitStats_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
        
    }

    class ButtonListAddButtonEventArgs : EventArgs
    {
        private SXALMarket _inplayMarket;

        public SXALMarket Market { get { return _inplayMarket; } }

        public ButtonListAddButtonEventArgs(SXALMarket inplayMarket)
        {
            _inplayMarket = inplayMarket;
        }
    }

    class ButtonBuilderHelper
    {
        public event EventHandler<EventArgs> StartButtonListEvent;
        public event EventHandler<EventArgs> CompleteButtonListEvent;
        public event EventHandler<ButtonListAddButtonEventArgs> AddButtonEvent;

        private Thread _buttonBuilderThread;

        public ButtonBuilderHelper()
        {
            _buttonBuilderThread = new Thread(buttonBuilderRunner);
            _buttonBuilderThread.IsBackground = true;
        }

        public void start()
        {
            try
            {
                if (SXThreadStateChecker.isStartedBackground(_buttonBuilderThread))
                    return;

                _buttonBuilderThread = new Thread(buttonBuilderRunner);
                _buttonBuilderThread.IsBackground = true;
                _buttonBuilderThread.Start();
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void buttonBuilderRunner()
        {
            try
            {
                EventHandler<EventArgs> startButtonList = StartButtonListEvent;
                if (startButtonList != null)
                {
                    startButtonList(this, new EventArgs());
                }

                SortedList<long, SXALMarket> inplayMarkets = SXALSoccerMarketManager.Instance.InPlayMarkets;

                foreach (SXALMarket market in inplayMarkets.Values)
                {
                    if (market.IsUnmanaged) continue;

                    //Nur Match Odds Markets
                    if (!market.IsMatchOdds) continue;

                    EventHandler<ButtonListAddButtonEventArgs> addButton = AddButtonEvent;
                    if (addButton != null)
                    {
                        addButton(this, new ButtonListAddButtonEventArgs(market));
                    }
                    Thread.Sleep(50);
                }


                EventHandler<EventArgs> completeButtonList = CompleteButtonListEvent;
                if (completeButtonList != null)
                {
                    completeButtonList(this, new EventArgs());
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }

    class ButtonColorStatHelper
    {
        private SAConfigurationRW _config;

        public ButtonColorStatHelper()
        {
            _config = new SAConfigurationRW();
        }
        public async Task<Tuple<bool,Color>> buttonStatColor(ulong teamAId, ulong teamBId, String teamA, String teamB, String league)
        {

            bool bRet = false;
            Color statColor = Color.Transparent;
            try
            {
                if (_config.GameSelectionColors == null || _config.GameSelectionColors.Count == 0)
                    return new Tuple<bool,Color>(false, statColor);

                IHistoricDataService histData = HistoricDataServiceFactory.getInstance(teamAId, teamBId, teamA, teamB);
                if (histData != null)
                {
                    HistoricDataStatistic stat = await histData.GetStatistic(teamAId, teamBId, teamA, teamB, league, _config.NoOfData, _config.AgeOfData);

                    foreach (StatisticsColorSelectionElement element in _config.GameSelectionColors.Values)
                    {
                        foreach (StatisticSelectionElement statElement in element.Statistics)
                        {
                            ulong teamId;
                            // Geeignete Spielliste laden
                            HistoricMatchList matchList = getInitialMatchList(stat, statElement.Team, out teamId);

                            //Es konnte keine Spielliste geladen werden => weiter
                            if (matchList == null)
                                continue;

                            if (matchList.Count == 0)
                            {
                                bRet = false;
                                break;
                            }

                            //Home away oder beides?
                            matchList = selectHomeAway(matchList, statElement.HomeAway, teamId);

                            bRet = isInStatRange(statElement.Statistic, statElement.LoRange, statElement.HiRange, matchList, teamId);
                            if (!bRet)
                                break;
                        }

                        if (bRet)
                        {
                            statColor = element.StatisticColor;
                            Console.WriteLine(String.Format("Gewählte Farbe ist {0}", statColor.Name));
                            break;
                        }
                    }
                }
                else
                    return new Tuple<bool,Color>(false, statColor);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return new Tuple<bool,Color>( bRet, statColor);
        }

        private bool isInStatRange(STATISTICTYPE statType, double lo, double hi, HistoricMatchList matchList, ulong teamId)
        {
            bool bRet = false;
            try
            {
                switch (statType)
                {
                    case STATISTICTYPE.AVGFIRSTGOAL:
                        bRet = matchList.AvgFirstGoalMinute >= lo && matchList.AvgFirstGoalMinute <= hi ? true : false;
                        break;
                    case STATISTICTYPE.AVGGOALS:
                        bRet = matchList.AvgGoals >= lo && matchList.AvgGoals <= hi ? true : false;
                        break;
                    case STATISTICTYPE.DRAW:
                        bRet = matchList.getWLD(teamId).DrawPercent >= lo && matchList.getWLD(teamId).DrawPercent <= hi ? true : false;
                        break;
                    case STATISTICTYPE.EARLIESTFIRSTGOAL:
                        bRet = matchList.EarlierstFirstGoal >= lo && matchList.EarlierstFirstGoal <= hi ? true : false;
                        break;
                    case STATISTICTYPE.LATESTFIRSTGOAL:
                        bRet = matchList.LatestFirstGoal >= lo && matchList.LatestFirstGoal <= hi ? true : false;
                        break;
                    case STATISTICTYPE.LOSS:
                        bRet = matchList.getWLD(teamId).LossPercent >= lo && matchList.getWLD(teamId).LossPercent <= hi ? true : false;
                        break;
                    case STATISTICTYPE.OVER05:
                        bRet = matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ZEROPOINTFIVE).OverPercent >= lo &&
                            matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ZEROPOINTFIVE).OverPercent <= hi ? true : false;
                        break;
                    case STATISTICTYPE.OVER15:
                        bRet = matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ONEPOINTFIVE).OverPercent >= lo &&
                            matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ONEPOINTFIVE).OverPercent <= hi ? true : false;
                        break;
                    case STATISTICTYPE.OVER25:
                        bRet = matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.TWOPOINTFIVE).OverPercent >= lo &&
                            matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.TWOPOINTFIVE).OverPercent <= hi ? true : false;
                        break;
                    case STATISTICTYPE.OVER35:
                        bRet = matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.THREEPOINTFIVE).OverPercent >= lo &&
                            matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.THREEPOINTFIVE).OverPercent <= hi ? true : false;
                        break;
                    case STATISTICTYPE.OVER45:
                        bRet = matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.FOURPOINTFIVE).OverPercent >= lo &&
                            matchList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.FOURPOINTFIVE).OverPercent <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE00:
                        bRet = matchList.getScorePercentage(SCORES.ZEROZERO, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.ZEROZERO, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE01:
                        bRet = matchList.getScorePercentage(SCORES.ZEROONE, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.ZEROONE, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE02:
                        bRet = matchList.getScorePercentage(SCORES.ZEROTWO, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.ZEROTWO, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE03:
                        bRet = matchList.getScorePercentage(SCORES.ZEROTHREE, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.ZEROTHREE, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE10:
                        bRet = matchList.getScorePercentage(SCORES.ONEZERO, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.ONEZERO, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE11:
                        bRet = matchList.getScorePercentage(SCORES.ONEONE, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.ONEONE, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE12:
                        bRet = matchList.getScorePercentage(SCORES.ONETWO, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.ONETWO, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE13:
                        bRet = matchList.getScorePercentage(SCORES.ONETHREE, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.ONETHREE, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE20:
                        bRet = matchList.getScorePercentage(SCORES.TWOZERO, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.TWOZERO, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE21:
                        bRet = matchList.getScorePercentage(SCORES.TWOONE, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.TWOONE, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE22:
                        bRet = matchList.getScorePercentage(SCORES.TWOTWO, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.TWOTWO, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE23:
                        bRet = matchList.getScorePercentage(SCORES.TWOTHREE, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.TWOTHREE, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE30:
                        bRet = matchList.getScorePercentage(SCORES.THREEZERO, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.THREEZERO, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE31:
                        bRet = matchList.getScorePercentage(SCORES.THREEONE, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.THREEONE, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE32:
                        bRet = matchList.getScorePercentage(SCORES.THREETWO, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.THREETWO, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCORE33:
                        bRet = matchList.getScorePercentage(SCORES.THREETHREE, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.THREETHREE, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.SCOREOTHER:
                        bRet = matchList.getScorePercentage(SCORES.OTHERS, teamId) >= lo
                            && matchList.getScorePercentage(SCORES.OTHERS, teamId) <= hi ? true : false;
                        break;
                    case STATISTICTYPE.WIN:
                        bRet = matchList.getWLD(teamId).WinPercent >= lo && matchList.getWLD(teamId).WinPercent <= hi ? true : false;
                        break;
                    case STATISTICTYPE.NOOFDATA:
                        bRet = matchList.NoOfData >= lo && matchList.NoOfData <= hi ? true : false;
                        break;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return bRet;
        }

        private HistoricMatchList getInitialMatchList(HistoricDataStatistic statistics, STATISTICTEAM selection, out ulong teamId)
        {

            HistoricMatchList theList = null;
            teamId = 0;
            try
            {
                switch (selection)
                {
                    case STATISTICTEAM.BOTH:
                        if (statistics != null)
                        {
                            theList = statistics.Direct;
                            teamId = statistics.TeamAId;
                        }
                        break;
                    case STATISTICTEAM.TEAMA:
                        if (statistics != null)
                        {
                            theList = statistics.TeamA;
                            teamId = statistics.TeamAId;
                        }
                        break;
                    case STATISTICTEAM.TEAMB:
                        if (statistics != null)
                        {
                            theList = statistics.TeamB;
                            teamId = statistics.TeamBId;
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return theList;
        }

        private HistoricMatchList selectHomeAway(HistoricMatchList matchList, STATISTICHOMEAWAY homeAway, ulong teamId)
        {
            HistoricMatchList theList = new HistoricMatchList();
            try
            {
                switch (homeAway)
                {
                    case STATISTICHOMEAWAY.BOTH:
                        theList = matchList;
                        break;
                    case STATISTICHOMEAWAY.HOME:
                        foreach (LSHistoricMatch match in matchList)
                        {
                            if (match.TeamAId == teamId)
                                theList.Add(match);
                        }
                        break;
                    case STATISTICHOMEAWAY.AWAY:
                        foreach (LSHistoricMatch match in matchList)
                        {
                            if (match.TeamBId == teamId)
                                theList.Add(match);
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return theList;
        }        
    }
}
