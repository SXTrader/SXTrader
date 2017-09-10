using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.livescoreparser;
using System.IO;
using System.Reflection;
using System.Xml;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk;
using net.sxtrader.bftradingstrategies.common;
using System.Collections;
using net.sxtrader.bftradingstrategies.bfuestrategy;
//using net.sxtrader.bftradingstrategies.ttr;
using net.sxtrader.bftradingstrategies.tippsters;
using net.sxtrader.bftradingstrategies.ttr;


namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    public partial class frmMain : Form, IPluginHost, IHistory
    {        
        private LiveScoreParser m_parser = LiveScoreParser.Instance;
        private LiveScore2Parser m_parser2 = LiveScore2Parser.Instance;
        private GenConfigurationRW m_config;
        private String m_newMessage;
        private String m_moneyInfo;
        private int m_errorNumber;
        private SortedList<int, string> m_errorMessages = new SortedList<int, string>();
        private Timer _timer;
        private Timer _gcTimer;
        private object syncRoot = "syncRoot";
        private object syncHistory = "syncHistory";
        private object syncMessage = "syncMessage";
        private object syncToolbar = "syncToolbar";
        private object syncToolbarLTCD = "syncToolbarLTCD";

        private delegate void AsyncMarketLoaderBuildUp();

        #region PluginHost
        void getPluginsFromTree(TreeNode theNode, ref ArrayList plugins)
        {
            foreach(TreeNode childNode in theNode.Nodes)
            {
                getPluginsFromTree(childNode, ref plugins);
            }

            if(theNode.Tag != null)
            {
                IPlugin plugin = theNode.Tag as IPlugin;
                if(plugin != null)
                {
                    plugins.Add(plugin);
                }
            }
        }
        #region IPluginHost Member
        public IPlugin[] PluginsArray 
        { 
            get
            {
                ArrayList plugins = new ArrayList();
                foreach (TreeNode node in tvwPlugins.Nodes)
                {
                    getPluginsFromTree(node, ref plugins);
                }
                return (IPlugin[]) plugins.ToArray(typeof(IPlugin));
            } 
        }

        public void ErrorMessage(int errorNumber, string message)
        {
            lock (syncRoot)
            {
                if (txtMessage.InvokeRequired)
                {
                    m_newMessage = errorNumber + " " + message + "\r\n";
                    m_errorNumber = errorNumber;

                    txtMessage.Invoke(new MethodInvoker(() =>
                    {
                        ErrorUpdate(this, new EventArgs());
                    }));
                }
                else
                {
                    // Wenn Nachricht schon vorhanden, keien weitere Aktion nötig
                    if (m_errorMessages.ContainsKey(errorNumber))
                        return;

                    txtMessage.AppendText(message + "\r\n");
                    txtMessage.Select(txtMessage.Text.LastIndexOf(message), message.Length);
                    txtMessage.SelectionColor = Color.Red;
                }
            }
        }

        public void AccountUpdate(double balance, double available, string currency)
        {
            lock (syncToolbar)
            {
                try
                {
                    StringBuilder sbMoneyInfo = new StringBuilder();

                    /*
                    if (!stbInfos.IsHandleCreated)
                        return;
                    */
                    if (stbInfos.InvokeRequired)
                    {
                        stbInfos.Invoke(new MethodInvoker(() =>
                            {
                                sbMoneyInfo.AppendFormat(Resources.strAccountStatement, balance, currency, available);
                                m_moneyInfo = sbMoneyInfo.ToString();
                                MoneyUpdate(this, new EventArgs());                                
                            }));                       
                    }
                    else
                    {
                        sbMoneyInfo.AppendFormat(Resources.strAccountStatement, balance, currency, available);
                        tssMoney.Text = sbMoneyInfo.ToString();
                        tssMoney.AutoSize = true;
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        public void Feedback(string Feedback, IPlugin Plugin)
        {
            lock (syncRoot)
            {
                try
                {
                    if (Feedback == "Shutdown")
                    {
                        Environment.Exit(0);
                    }

                    if (txtMessage.InvokeRequired)
                    {
                        txtMessage.Invoke(new MethodInvoker(() =>
                            {
                                m_newMessage = Feedback + "\r\n";
                                MessageUpdate(this, new EventArgs());
                            }));                        
                    }
                    else
                    {
                        if (Feedback != null && this.txtMessage != null)
                        {
                            this.txtMessage.Text = Feedback + "\r\n" + this.txtMessage.Text;
                        }
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        #endregion

        private void MoneyUpdate(object sender, EventArgs e)
        {
            try
            {                
                tssMoney.Text = m_moneyInfo;
                tssMoney.AutoSize = true;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void MessageUpdate(object sender, EventArgs e)
        {
            lock (syncMessage)
            {
                try
                {
                    txtMessage.Text = m_newMessage + txtMessage.Text;
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
                //txtMessage.AppendText(m_newMessage);
            }
            //this.txtMessage.Text += m_newMessage;
        }

        private void ErrorUpdate(object sender, EventArgs e)
        {
            lock (syncMessage)
            {
                try
                {
                    // Wenn Nachricht schon vorhanden, keien weitere Aktion nötig
                    if (m_errorMessages.ContainsKey(m_errorNumber))
                        return;

                    txtMessage.AppendText(m_newMessage);
                    if (txtMessage.Text.LastIndexOf(m_newMessage) > 0)
                    {
                        txtMessage.Select(txtMessage.Text.LastIndexOf(m_newMessage), m_newMessage.Length);
                        txtMessage.SelectionColor = Color.Red;
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }


        /// <summary>
        /// Unloads and Closes all AvailablePlugins
        /// </summary>
        public void ClosePlugins()
        {
            foreach(IPlugin plugin in this.PluginsArray)
            {
                ((IDisposable)plugin).Dispose();
            }
        }

        #endregion
        public frmMain()
        {
            
            InitializeComponent();


            frmDonation dialogDonation = new frmDonation();
            dialogDonation.ShowDialog();

            AsyncMarketLoaderBuildUp caller = new AsyncMarketLoaderBuildUp(this.loadMarketRunner);
            caller.BeginInvoke(null, null);

            loadMarketRunner();

            txtMessage.Dock = DockStyle.Bottom;
            pnlPlugins.Dock = DockStyle.Fill;

            doLanguage();

            m_config = new GenConfigurationRW();

            //Logs aktivieren/deaktivieren
            
            LiveTickerLog.Instance.LogEnabled = m_config.LogLiveticker;
            TradeLog.Instance.LogEnabled = m_config.LogTrades;
            TradeLog.Instance.LogBetAmount = m_config.LogBetAmounts;
            DebugWriter.Instance.WriteDebugInfo = m_config.WriteDebugFile;

            //BetWatchdog.Instance.WaitTime = m_config.BetfairCheck;
            m_parser.WaitTime = m_config.LivetickerCheck;
            m_parser.LiveScoreStateChangedEvent += new EventHandler<net.sxtrader.bftradingstrategies.lsparserinterfaces.LiveScoreStateEventArgs>(LiveScoreStateChangedEventHandler);
            m_parser.LiveScoreCheckCountDownEvent += new EventHandler<LiveScoreCheckCountDownEventArgs>(LiveScoreCheckCountDownEventHandler);
            m_parser2.WaitTime = m_config.LivetickerCheck;
            m_parser2.LiveScoreStateChangedEvent += new EventHandler<net.sxtrader.bftradingstrategies.lsparserinterfaces.LiveScoreStateEventArgs>(LiveScoreStateChangedEventHandler);
            m_parser2.LiveScoreCheckCountDownEvent += new EventHandler<LiveScoreCheckCountDownEventArgs>(LiveScoreCheckCountDownEventHandler);


            buildModuleTree();
            

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();

            dialogDonation.Dispose();
        }

        private void buildModuleTree()
        {
            PluginStatisticAnalyses statAnal = new PluginStatisticAnalyses();
            statAnal.Host = this;
            Object[] objects = { m_config.BetfairCheck, m_parser2, m_config.ConfirmFastBet };
            statAnal.Initialize(objects, m_parser);

            char[] seps = { '/' };
            String[] strFullQualified =statAnal.FullQualifiedName.Split(seps);
            TreeNode nodeCommon = new TreeNode(strFullQualified[0]);


            TreeNode nodeStats = new TreeNode(strFullQualified[1]);
            nodeStats.Tag = statAnal;
            nodeCommon.Nodes.Add(nodeStats);


            PluginLivescoreMapper lsMapper = new PluginLivescoreMapper();
            lsMapper.Host = this;
            lsMapper.Initialize(objects, m_parser);
            strFullQualified = lsMapper.FullQualifiedName.Split(seps);

            TreeNode nodeLSMapper = new TreeNode(strFullQualified[1]);
            nodeLSMapper.Tag = lsMapper;
            nodeCommon.Nodes.Add(nodeLSMapper);



            PluginLayDraw laythedraw = new PluginLayDraw();
            laythedraw.Host = this;
            laythedraw.Initialize(objects, m_parser);
            strFullQualified = laythedraw.FullQualifiedName.Split(seps);

            TreeNode nodeSoccer = new TreeNode(strFullQualified[0]);

            TreeNode nodeLTD = new TreeNode(strFullQualified[1]);
            nodeLTD.Tag = laythedraw;            
            nodeSoccer.Nodes.Add(nodeLTD);
            
            PluginTradeTheReaction tradethereaction = new PluginTradeTheReaction();
            tradethereaction.Host = this;
            tradethereaction.Initialize(objects, m_parser);
            strFullQualified = tradethereaction.FullQualifiedName.Split(seps);

            TreeNode nodeTTR = new TreeNode(strFullQualified[1]);
            nodeTTR.Tag = tradethereaction;
            nodeSoccer.Nodes.Add(nodeTTR);
            
            PluginTheLowLay thelowlay = new PluginTheLowLay();
            thelowlay.Host = this;
            thelowlay.Initialize(objects, m_parser);
            strFullQualified = thelowlay.FullQualifiedName.Split(seps);

            TreeNode nodeHorse = new TreeNode(strFullQualified[0]);

            TreeNode nodeTLL = new TreeNode(strFullQualified[1]);
            nodeTLL.Tag = thelowlay;
            nodeHorse.Nodes.Add(nodeTLL);

            PluginLayerOfProfit layerOfProfit = new PluginLayerOfProfit();
            layerOfProfit.Host = this;
            layerOfProfit.Initialize(objects, m_parser);
            strFullQualified = layerOfProfit.FullQualifiedName.Split(seps);

            TreeNode nodeLOP = new TreeNode(strFullQualified[1]);
            nodeLOP.Tag = layerOfProfit;
            nodeHorse.Nodes.Add(nodeLOP);

            this.tvwPlugins.Nodes.Add(nodeCommon);
            this.tvwPlugins.Nodes.Add(nodeSoccer);
            this.tvwPlugins.Nodes.Add(nodeHorse);
        }

        private void loadMarketRunner()
        {
            
            SXALSoccerMarketManager.Instance.MarketLoadEvent += new EventHandler<MarketLoadProgressEventArgs>(SXALSoccerMarketManager_MarketLoadEvent);
            SXALSoccerMarketManager.Instance.MarketUpdateCompletedEvent += Instance_MarketUpdateCompletedEvent;
            SXALSoccerMarketManager.Instance.isMarketInplay(0);
             
        }

        void Instance_MarketUpdateCompletedEvent(object sender, MarketUpdateCompletedEventArgs e)
        {
            try
            {
                if (stbInfos.InvokeRequired)
                {
                    stbInfos.Invoke(new MethodInvoker(() =>
                    {
                        tspbMarkets.Value = tspbMarkets.Maximum;
                    }));
                }
                else
                {
                    tspbMarkets.Value = tspbMarkets.Maximum;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void SXALSoccerMarketManager_MarketLoadEvent(object sender, MarketLoadProgressEventArgs e)
        {
            try
            {
                if (stbInfos.InvokeRequired)
                {
                    stbInfos.Invoke(new MethodInvoker(() =>
                    {
                        tspbMarkets.Maximum = e.OverallMarketCount;
                        tspbMarkets.Increment(e.CurrentMarketCount - tspbMarkets.Value);
                    }));
                }
                else
                {
                    tspbMarkets.Maximum = e.OverallMarketCount;
                    tspbMarkets.Increment(e.CurrentMarketCount - tspbMarkets.Value);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (stbInfos.InvokeRequired)
                {
                    stbInfos.Invoke(new MethodInvoker(() =>
                    {
                        // den Richtigen Liveticker finden
                        foreach (ToolStripItem item in stbInfos.Items)
                        {
                            LiveScoreCheckCountDownEventArgs tag = item.Tag as LiveScoreCheckCountDownEventArgs;
                            if (tag != null)
                            {
                                if (tag.Countdown > 0)
                                {
                                    tag = new LiveScoreCheckCountDownEventArgs(tag.Livescore, tag.Countdown - 1000);
                                    item.Tag = tag;
                                    item.Text = String.Format(Resources.strUpdateLiveticker, tag.Livescore, tag.Countdown / 1000);
                                }
                            }
                        }
                    }));
                }
                else
                {
                    lock (syncToolbarLTCD)
                    {
                        // den Richtigen Liveticker finden
                        foreach (ToolStripItem item in stbInfos.Items)
                        {
                            LiveScoreCheckCountDownEventArgs tag = item.Tag as LiveScoreCheckCountDownEventArgs;
                            if (tag != null)
                            {
                                if (tag.Countdown > 0)
                                {
                                    tag = new LiveScoreCheckCountDownEventArgs(tag.Livescore, tag.Countdown - 1000);
                                    item.Tag = tag;
                                    item.Text = String.Format(Resources.strUpdateLiveticker, tag.Livescore, tag.Countdown / 1000);
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

        void LiveScoreCheckCountDownEventHandler(object sender, LiveScoreCheckCountDownEventArgs e)
        {
            try
            {
                if (stbInfos.InvokeRequired)
                {
                    this.stbInfos.Invoke(new MethodInvoker(() =>
                    {
                        // den Richtigen Liveticker finden
                        foreach (ToolStripItem item in stbInfos.Items)
                        {
                            LiveScoreCheckCountDownEventArgs tag = item.Tag as LiveScoreCheckCountDownEventArgs;
                            if (tag != null && tag.Livescore.Equals(e.Livescore, StringComparison.InvariantCultureIgnoreCase))
                            {
                                item.Tag = e;
                                item.Text = String.Format(Resources.strUpdateLiveticker, e.Livescore, e.Countdown / 1000);
                            }
                        }
                    }));
                }
                else
                {
                    lock (syncToolbarLTCD)
                    {
                        // den Richtigen Liveticker finden
                        foreach (ToolStripItem item in stbInfos.Items)
                        {
                            LiveScoreCheckCountDownEventArgs tag = item.Tag as LiveScoreCheckCountDownEventArgs;
                            if (tag != null && tag.Livescore.Equals(e.Livescore, StringComparison.InvariantCultureIgnoreCase))
                            {
                                item.Tag = e;
                                item.Text = String.Format(Resources.strUpdateLiveticker, e.Livescore, e.Countdown / 1000);
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

        private void internalLiveScoreChanged(object sender, net.sxtrader.bftradingstrategies.lsparserinterfaces.LiveScoreStateEventArgs e)
        {
            ToolStripStatusLabel itemLiveScore = null;
            foreach (ToolStripItem item in stbInfos.Items)
            {
                if (item.Text == e.Livescore)
                {
                    itemLiveScore = (ToolStripStatusLabel)item;
                    break;
                }
            }

            if (itemLiveScore == null)
            {
                ToolStripSeparator sep = new ToolStripSeparator();
                stbInfos.Items.Add(sep);

                itemLiveScore = new ToolStripStatusLabel();
                itemLiveScore.AutoSize = true;
                itemLiveScore.DisplayStyle = ToolStripItemDisplayStyle.Text;
                itemLiveScore.BorderSides = ToolStripStatusLabelBorderSides.All;
                itemLiveScore.BorderStyle = Border3DStyle.Sunken;

                itemLiveScore.Text = e.Livescore;
                stbInfos.Items.Add(itemLiveScore);



                sep = new ToolStripSeparator();
                stbInfos.Items.Add(sep);

                ToolStripStatusLabel itemLiveScoreUpdate = new ToolStripStatusLabel();
                itemLiveScoreUpdate.AutoSize = true;
                itemLiveScoreUpdate.DisplayStyle = ToolStripItemDisplayStyle.Text;
                itemLiveScoreUpdate.BorderSides = ToolStripStatusLabelBorderSides.All;
                itemLiveScoreUpdate.BorderStyle = Border3DStyle.Sunken;

                itemLiveScoreUpdate.Text = String.Format(Resources.strUpdateLiveticker, e.Livescore, String.Empty);
                itemLiveScoreUpdate.Tag = new LiveScoreCheckCountDownEventArgs(e.Livescore, 0);
                stbInfos.Items.Add(itemLiveScoreUpdate);

            }

            if (e.State)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)(() => 
                    {
                        itemLiveScore.BorderStyle = Border3DStyle.Sunken;
                        itemLiveScore.BackColor = Color.LightGreen;
                    }));
                }
                else
                {
                    itemLiveScore.BorderStyle = Border3DStyle.Sunken;
                    itemLiveScore.BackColor = Color.LightGreen;
                }
            }
            else
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        itemLiveScore.BackColor = Color.Tomato;
                        itemLiveScore.BorderStyle = Border3DStyle.Raised;
                    }));
                }
                else
                {
                    itemLiveScore.BackColor = Color.Tomato;
                    itemLiveScore.BorderStyle = Border3DStyle.Raised;
                }
            }
        }

        void LiveScoreStateChangedEventHandler(object sender, net.sxtrader.bftradingstrategies.lsparserinterfaces.LiveScoreStateEventArgs e)
        {
            try
            {
                if (stbInfos.Created)
                {
                    stbInfos.Invoke(new MethodInvoker(() =>
                        {
                            internalLiveScoreChanged(sender, e);
                        }));
                }
                else
                {
                    internalLiveScoreChanged(sender, e);
                }
                
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void doLanguage()
        {
            mnuItemFile.Text = BFTSGUI.strFile;
            mnuFileExit.Text = BFTSGUI.strFileExit;
            mnuItemTools.Text = BFTSGUI.strTools;
            mnuToolsOptions.Text = BFTSGUI.strToolsOptions;
            donatToolStripMenuItem.Text = BFTSGUI.strDonation;
            contactToolStripMenuItem.Text = BFTSGUI.strContact;
            reportBugsToolStripMenuItem.Text = BFTSGUI.strContactBug;
            suggestionToolStripMenuItem.Text = BFTSGUI.strContactSuggestion;
            mnuItemHelp.Text = BFTSGUI.strHelp;
        }

        private void checkForUpdates()
        {
            String filename = "http://www.sxtrader.net/SXTraderVersion.xml";
            try
            {
                Type type = this.GetType();
                Assembly assembly = Assembly.GetAssembly(type);
                AssemblyName assemblyName = assembly.GetName();
                Version version = assemblyName.Version;

                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNode rootNode = doc.ChildNodes[1];
                XmlAttribute attr = rootNode.Attributes[0];
                // Neue Version?
                if (!version.ToString().Equals(attr.Value))
                {
                    String updateInfo = "There's a new version of SXTrader ready to download.\r\n" +
                                      "Your current version is {0}. The downloadable version is {1}.\r\n" +
                                      "If you choose 'Yes' below a browser window to the download location\r\n" +
                                      "will be opened.";
                    updateInfo = String.Format(updateInfo, version.ToString(), attr.Value);
                    DialogResult dr = MessageBox.Show(updateInfo, "New version of SXTrader available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dr == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(rootNode.InnerText);
                        Application.Exit();
                    }
                    else
                        Application.Exit();
                }
            }
            catch (Exception)
            {

            }
                   
        }

        #region IHistory Member

        public void Historize(string module, DateTime dts, string match, double winloose, bool test)
        {
            
            lock (syncHistory)
            {
                try
                {
                    // Öffene Historyfile
                    String strFile = String.Empty;
                    if (test)
                    {
                        strFile = SXDirs.ApplPath + @"\BFTSTestHistory.xml";
                    }
                    else
                    {
                        strFile = SXDirs.ApplPath + @"\BFTSHistory.xml";
                    }
                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.Load(strFile);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        //Datei nicht gefunden => erzeugen
                        XmlTextWriter writer = new XmlTextWriter(strFile, Encoding.UTF8);
                        writer.Formatting = Formatting.Indented;
                        writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                        writer.WriteStartElement("root");
                        writer.Close();
                        doc.Load(strFile);
                    }

                    XmlNode root = doc.DocumentElement;
                    XmlElement element = null;
                    element = doc.CreateElement("hentry");
                    element.SetAttribute("module", module);
                    element.SetAttribute("match", match);
                    element.SetAttribute("dts", dts.Ticks.ToString());
                    element.SetAttribute("money", winloose.ToString());
                    root.AppendChild(element);
                    doc.Save(strFile);
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
            
        }

        #endregion

        

        protected override void OnClosing(CancelEventArgs e)
        {
            DialogResult result = MessageBox.Show(Resources.strCloseQuestion, Resources.strCloseCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuToolsOptions_Click(object sender, EventArgs e)
        {
            //frmConfig config = new frmConfig();
            using (frmConfig config = new frmConfig())
            {
                config.Plugins = this.PluginsArray;
                config.ShowDialog();
            }
        }

        private void tvwPlugins_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {

                    tvwPlugins.Invoke(new MethodInvoker(() =>
                        {
                            IPlugin plugin = e.Node.Tag as IPlugin;
                            if(plugin != null)
                            {
                                this.pnlPlugins.Controls.Clear();
                                plugin.MainInterface.Dock = DockStyle.Fill;
                                plugin.MainInterface.Enabled = false;
                                plugin.MainInterface.Enabled = true;

                                //Finally, add the usercontrol to the panel... Tadah!                    
                                this.pnlPlugins.Controls.Add(plugin.MainInterface);
                            }
                        }));                   
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //BankrollManager.Instance.BalanceInfoUpdated += new EventHandler<BFBalanceUpdatedEventArgs>(Instance_BalanceInfoUpdated);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} - {1}", "SXTrader", SXAL.SXALKom.Instance.getExchangeName());
            this.Text = sb.ToString();

            _gcTimer = new Timer();
            _gcTimer.Interval = 6 * 60 * 60 * 1000;
            _gcTimer.Tick += _gcTimer_Tick;
            _gcTimer.Enabled = true;
            _gcTimer.Start();
        }

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                }

                if (_gcTimer != null)
                {
                    _gcTimer.Dispose();
                }

                ClosePlugins();
                
                LiveScore2Parser.Instance.Dispose();
                LiveScoreParser.Instance.Dispose();
                SXALSoccerMarketManager.Instance.Dispose();
                SXALHorseMarketManager.Instance.Dispose();
            }
            base.Dispose(disposing);
        }

        void _gcTimer_Tick(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void donatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=" + "TKJNJ54TU5ZAJ");
        }

        private void reportBugsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:bug@sxtrader.net");
        }

        private void suggestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:suggestion:sxtrader.net");
        }

        private void pnlPlugins_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void mnuItemHelp_Click(object sender, EventArgs e)
        {
           
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
           {
                 ntiSXTrader.Visible = true;
                 ntiSXTrader.BalloonTipText = "SXTrader";
                 ntiSXTrader.ShowBalloonTip(2);  //show balloon tip for 2 seconds
                 ntiSXTrader.Text = "SXTrader minimized to System Tray";
                 this.WindowState = FormWindowState.Minimized;
                 this.ShowInTaskbar = false;
           }

        }

        private void ntiSXTrader_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Maximized;
                ntiSXTrader.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void mnuItemHelpHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.sxtrader.net/index.php/7-sxtrader");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frm = new frmAbout();
            frm.ShowDialog();
        }
    }
}
