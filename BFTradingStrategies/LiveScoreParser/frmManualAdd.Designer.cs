namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    partial class frmManualAdd
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmManualAdd));
            this.lblBFMatch = new System.Windows.Forms.Label();
            this.txtBFMatch = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvwLivescores = new System.Windows.Forms.ListView();
            this.clhLivescore = new System.Windows.Forms.ColumnHeader();
            this.clhStartTime = new System.Windows.Forms.ColumnHeader();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblSelectionRange = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBFMatch
            // 
            resources.ApplyResources(this.lblBFMatch, "lblBFMatch");
            this.lblBFMatch.Name = "lblBFMatch";
            // 
            // txtBFMatch
            // 
            resources.ApplyResources(this.txtBFMatch, "txtBFMatch");
            this.txtBFMatch.Name = "txtBFMatch";
            this.txtBFMatch.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvwLivescores);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lvwLivescores
            // 
            this.lvwLivescores.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhLivescore,
            this.clhStartTime});
            resources.ApplyResources(this.lvwLivescores, "lvwLivescores");
            this.lvwLivescores.FullRowSelect = true;
            this.lvwLivescores.GridLines = true;
            this.lvwLivescores.MultiSelect = false;
            this.lvwLivescores.Name = "lvwLivescores";
            this.lvwLivescores.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwLivescores.UseCompatibleStateImageBehavior = false;
            this.lvwLivescores.View = System.Windows.Forms.View.Details;
            // 
            // clhLivescore
            // 
            resources.ApplyResources(this.clhLivescore, "clhLivescore");
            // 
            // clhStartTime
            // 
            resources.ApplyResources(this.clhStartTime, "clhStartTime");
            // 
            // btnConnect
            // 
            this.btnConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnConnect, "btnConnect");
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // dtpFrom
            // 
            resources.ApplyResources(this.dtpFrom, "dtpFrom");
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.ValueChanged += new System.EventHandler(this.dtpFrom_ValueChanged);
            // 
            // dtpTo
            // 
            resources.ApplyResources(this.dtpTo, "dtpTo");
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.ValueChanged += new System.EventHandler(this.dtpTo_ValueChanged);
            // 
            // lblSelectionRange
            // 
            resources.ApplyResources(this.lblSelectionRange, "lblSelectionRange");
            this.lblSelectionRange.Name = "lblSelectionRange";
            // 
            // lblFrom
            // 
            resources.ApplyResources(this.lblFrom, "lblFrom");
            this.lblFrom.Name = "lblFrom";
            // 
            // lblTo
            // 
            resources.ApplyResources(this.lblTo, "lblTo");
            this.lblTo.Name = "lblTo";
            // 
            // frmManualAdd
            // 
            this.AcceptButton = this.btnConnect;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.lblFrom);
            this.Controls.Add(this.lblSelectionRange);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.dtpFrom);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtBFMatch);
            this.Controls.Add(this.lblBFMatch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmManualAdd";
            this.Shown += new System.EventHandler(this.frmManualAdd_Shown);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBFMatch;
        private System.Windows.Forms.TextBox txtBFMatch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lvwLivescores;
        private System.Windows.Forms.ColumnHeader clhLivescore;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColumnHeader clhStartTime;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblSelectionRange;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
    }
}