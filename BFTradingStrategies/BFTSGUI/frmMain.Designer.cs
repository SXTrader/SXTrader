namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    partial class frmMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.donatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportBugsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.suggestionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemHelpHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.stbInfos = new System.Windows.Forms.StatusStrip();
            this.tssMoney = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspbMarkets = new System.Windows.Forms.ToolStripProgressBar();
            this.tsslMarketsLoad = new System.Windows.Forms.ToolStripStatusLabel();
            this.tvwPlugins = new System.Windows.Forms.TreeView();
            this.txtMessage = new System.Windows.Forms.RichTextBox();
            this.pnlPlugins = new System.Windows.Forms.Panel();
            this.ntiSXTrader = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuMain.SuspendLayout();
            this.stbInfos.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemFile,
            this.mnuItemTools,
            this.donatToolStripMenuItem,
            this.contactToolStripMenuItem,
            this.mnuItemHelp});
            resources.ApplyResources(this.mnuMain, "mnuMain");
            this.mnuMain.Name = "mnuMain";
            // 
            // mnuItemFile
            // 
            this.mnuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileExit});
            this.mnuItemFile.Name = "mnuItemFile";
            resources.ApplyResources(this.mnuItemFile, "mnuItemFile");
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            resources.ApplyResources(this.mnuFileExit, "mnuFileExit");
            this.mnuFileExit.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // mnuItemTools
            // 
            this.mnuItemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsOptions});
            this.mnuItemTools.Name = "mnuItemTools";
            resources.ApplyResources(this.mnuItemTools, "mnuItemTools");
            // 
            // mnuToolsOptions
            // 
            this.mnuToolsOptions.Name = "mnuToolsOptions";
            resources.ApplyResources(this.mnuToolsOptions, "mnuToolsOptions");
            this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
            // 
            // donatToolStripMenuItem
            // 
            this.donatToolStripMenuItem.Name = "donatToolStripMenuItem";
            resources.ApplyResources(this.donatToolStripMenuItem, "donatToolStripMenuItem");
            this.donatToolStripMenuItem.Click += new System.EventHandler(this.donatToolStripMenuItem_Click);
            // 
            // contactToolStripMenuItem
            // 
            this.contactToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportBugsToolStripMenuItem,
            this.suggestionToolStripMenuItem});
            this.contactToolStripMenuItem.Name = "contactToolStripMenuItem";
            resources.ApplyResources(this.contactToolStripMenuItem, "contactToolStripMenuItem");
            // 
            // reportBugsToolStripMenuItem
            // 
            this.reportBugsToolStripMenuItem.Name = "reportBugsToolStripMenuItem";
            resources.ApplyResources(this.reportBugsToolStripMenuItem, "reportBugsToolStripMenuItem");
            this.reportBugsToolStripMenuItem.Click += new System.EventHandler(this.reportBugsToolStripMenuItem_Click);
            // 
            // suggestionToolStripMenuItem
            // 
            this.suggestionToolStripMenuItem.Name = "suggestionToolStripMenuItem";
            resources.ApplyResources(this.suggestionToolStripMenuItem, "suggestionToolStripMenuItem");
            this.suggestionToolStripMenuItem.Click += new System.EventHandler(this.suggestionToolStripMenuItem_Click);
            // 
            // mnuItemHelp
            // 
            this.mnuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.mnuItemHelpHelp});
            this.mnuItemHelp.Name = "mnuItemHelp";
            resources.ApplyResources(this.mnuItemHelp, "mnuItemHelp");
            this.mnuItemHelp.Click += new System.EventHandler(this.mnuItemHelp_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // mnuItemHelpHelp
            // 
            this.mnuItemHelpHelp.Name = "mnuItemHelpHelp";
            resources.ApplyResources(this.mnuItemHelpHelp, "mnuItemHelpHelp");
            this.mnuItemHelpHelp.Click += new System.EventHandler(this.mnuItemHelpHelp_Click);
            // 
            // stbInfos
            // 
            this.stbInfos.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssMoney,
            this.tspbMarkets,
            this.tsslMarketsLoad});
            resources.ApplyResources(this.stbInfos, "stbInfos");
            this.stbInfos.Name = "stbInfos";
            // 
            // tssMoney
            // 
            this.tssMoney.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssMoney.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.tssMoney.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tssMoney.Name = "tssMoney";
            resources.ApplyResources(this.tssMoney, "tssMoney");
            // 
            // tspbMarkets
            // 
            this.tspbMarkets.Name = "tspbMarkets";
            resources.ApplyResources(this.tspbMarkets, "tspbMarkets");
            // 
            // tsslMarketsLoad
            // 
            this.tsslMarketsLoad.Name = "tsslMarketsLoad";
            resources.ApplyResources(this.tsslMarketsLoad, "tsslMarketsLoad");
            // 
            // tvwPlugins
            // 
            resources.ApplyResources(this.tvwPlugins, "tvwPlugins");
            this.tvwPlugins.Name = "tvwPlugins";
            this.tvwPlugins.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwPlugins_AfterSelect);
            // 
            // txtMessage
            // 
            resources.ApplyResources(this.txtMessage, "txtMessage");
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            // 
            // pnlPlugins
            // 
            resources.ApplyResources(this.pnlPlugins, "pnlPlugins");
            this.pnlPlugins.Name = "pnlPlugins";
            this.pnlPlugins.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPlugins_Paint);
            // 
            // ntiSXTrader
            // 
            resources.ApplyResources(this.ntiSXTrader, "ntiSXTrader");
            this.ntiSXTrader.DoubleClick += new System.EventHandler(this.ntiSXTrader_DoubleClick);
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlPlugins);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.tvwPlugins);
            this.Controls.Add(this.stbInfos);
            this.Controls.Add(this.mnuMain);
            this.MainMenuStrip = this.mnuMain;
            this.Name = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.stbInfos.ResumeLayout(false);
            this.stbInfos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.StatusStrip stbInfos;
        private System.Windows.Forms.ToolStripMenuItem mnuItemFile;
        private System.Windows.Forms.TreeView tvwPlugins;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuItemTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOptions;
        private System.Windows.Forms.ToolStripStatusLabel tssMoney;
        private System.Windows.Forms.RichTextBox txtMessage;
        private System.Windows.Forms.Panel pnlPlugins;
        private System.Windows.Forms.ToolStripMenuItem donatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contactToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportBugsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem suggestionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuItemHelp;
        private System.Windows.Forms.NotifyIcon ntiSXTrader;
        private System.Windows.Forms.ToolStripProgressBar tspbMarkets;
        private System.Windows.Forms.ToolStripStatusLabel tsslMarketsLoad;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuItemHelpHelp;
    }
}

