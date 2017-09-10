namespace net.sxtrader.bftradingstrategies.common
{
    partial class ctlLivescores
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlLivescores));
            this.tbcLivescores = new System.Windows.Forms.TabControl();
            this.tbpLivescore1 = new System.Windows.Forms.TabPage();
            this.dgvLivescore1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblBetfairName = new System.Windows.Forms.Label();
            this.txtBetfairName = new System.Windows.Forms.TextBox();
            this.tbpLivescore2 = new System.Windows.Forms.TabPage();
            this.dgvLivescore2 = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtBetfair2 = new System.Windows.Forms.TextBox();
            this.lblBetfair2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.clhBetfair = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhLivescore1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhBetfair2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clhLivescore2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbcLivescores.SuspendLayout();
            this.tbpLivescore1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLivescore1)).BeginInit();
            this.panel1.SuspendLayout();
            this.tbpLivescore2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLivescore2)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcLivescores
            // 
            this.tbcLivescores.AccessibleDescription = null;
            this.tbcLivescores.AccessibleName = null;
            resources.ApplyResources(this.tbcLivescores, "tbcLivescores");
            this.tbcLivescores.BackgroundImage = null;
            this.tbcLivescores.Controls.Add(this.tbpLivescore1);
            this.tbcLivescores.Controls.Add(this.tbpLivescore2);
            this.tbcLivescores.Font = null;
            this.tbcLivescores.Name = "tbcLivescores";
            this.tbcLivescores.SelectedIndex = 0;
            // 
            // tbpLivescore1
            // 
            this.tbpLivescore1.AccessibleDescription = null;
            this.tbpLivescore1.AccessibleName = null;
            resources.ApplyResources(this.tbpLivescore1, "tbpLivescore1");
            this.tbpLivescore1.BackgroundImage = null;
            this.tbpLivescore1.Controls.Add(this.dgvLivescore1);
            this.tbpLivescore1.Controls.Add(this.panel1);
            this.tbpLivescore1.Font = null;
            this.tbpLivescore1.Name = "tbpLivescore1";
            this.tbpLivescore1.UseVisualStyleBackColor = true;
            // 
            // dgvLivescore1
            // 
            this.dgvLivescore1.AccessibleDescription = null;
            this.dgvLivescore1.AccessibleName = null;
            this.dgvLivescore1.AllowUserToAddRows = false;
            this.dgvLivescore1.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.dgvLivescore1, "dgvLivescore1");
            this.dgvLivescore1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLivescore1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvLivescore1.BackgroundImage = null;
            this.dgvLivescore1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLivescore1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clhBetfair,
            this.clhLivescore1});
            this.dgvLivescore1.Font = null;
            this.dgvLivescore1.MultiSelect = false;
            this.dgvLivescore1.Name = "dgvLivescore1";
            this.dgvLivescore1.ReadOnly = true;
            this.dgvLivescore1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLivescore1.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellMouseEnter);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.lblBetfairName);
            this.panel1.Controls.Add(this.txtBetfairName);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // lblBetfairName
            // 
            this.lblBetfairName.AccessibleDescription = null;
            this.lblBetfairName.AccessibleName = null;
            resources.ApplyResources(this.lblBetfairName, "lblBetfairName");
            this.lblBetfairName.Font = null;
            this.lblBetfairName.Name = "lblBetfairName";
            // 
            // txtBetfairName
            // 
            this.txtBetfairName.AccessibleDescription = null;
            this.txtBetfairName.AccessibleName = null;
            resources.ApplyResources(this.txtBetfairName, "txtBetfairName");
            this.txtBetfairName.BackgroundImage = null;
            this.txtBetfairName.Font = null;
            this.txtBetfairName.Name = "txtBetfairName";
            this.txtBetfairName.TextChanged += new System.EventHandler(this.txtBetfairName_TextChanged);
            // 
            // tbpLivescore2
            // 
            this.tbpLivescore2.AccessibleDescription = null;
            this.tbpLivescore2.AccessibleName = null;
            resources.ApplyResources(this.tbpLivescore2, "tbpLivescore2");
            this.tbpLivescore2.BackgroundImage = null;
            this.tbpLivescore2.Controls.Add(this.dgvLivescore2);
            this.tbpLivescore2.Controls.Add(this.panel2);
            this.tbpLivescore2.Font = null;
            this.tbpLivescore2.Name = "tbpLivescore2";
            this.tbpLivescore2.UseVisualStyleBackColor = true;
            // 
            // dgvLivescore2
            // 
            this.dgvLivescore2.AccessibleDescription = null;
            this.dgvLivescore2.AccessibleName = null;
            this.dgvLivescore2.AllowUserToAddRows = false;
            resources.ApplyResources(this.dgvLivescore2, "dgvLivescore2");
            this.dgvLivescore2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLivescore2.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvLivescore2.BackgroundImage = null;
            this.dgvLivescore2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLivescore2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clhBetfair2,
            this.clhLivescore2});
            this.dgvLivescore2.Font = null;
            this.dgvLivescore2.MultiSelect = false;
            this.dgvLivescore2.Name = "dgvLivescore2";
            this.dgvLivescore2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLivescore2.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellMouseEnter);
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Controls.Add(this.txtBetfair2);
            this.panel2.Controls.Add(this.lblBetfair2);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // txtBetfair2
            // 
            this.txtBetfair2.AccessibleDescription = null;
            this.txtBetfair2.AccessibleName = null;
            resources.ApplyResources(this.txtBetfair2, "txtBetfair2");
            this.txtBetfair2.BackgroundImage = null;
            this.txtBetfair2.Font = null;
            this.txtBetfair2.Name = "txtBetfair2";
            this.txtBetfair2.TextChanged += new System.EventHandler(this.txtBetfair2_TextChanged);
            // 
            // lblBetfair2
            // 
            this.lblBetfair2.AccessibleDescription = null;
            this.lblBetfair2.AccessibleName = null;
            resources.ApplyResources(this.lblBetfair2, "lblBetfair2");
            this.lblBetfair2.Font = null;
            this.lblBetfair2.Name = "lblBetfair2";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // textBox1
            // 
            this.textBox1.AccessibleDescription = null;
            this.textBox1.AccessibleName = null;
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.BackgroundImage = null;
            this.textBox1.Font = null;
            this.textBox1.Name = "textBox1";
            // 
            // clhBetfair
            // 
            resources.ApplyResources(this.clhBetfair, "clhBetfair");
            this.clhBetfair.Name = "clhBetfair";
            this.clhBetfair.ReadOnly = true;
            // 
            // clhLivescore1
            // 
            resources.ApplyResources(this.clhLivescore1, "clhLivescore1");
            this.clhLivescore1.Name = "clhLivescore1";
            this.clhLivescore1.ReadOnly = true;
            // 
            // clhBetfair2
            // 
            resources.ApplyResources(this.clhBetfair2, "clhBetfair2");
            this.clhBetfair2.Name = "clhBetfair2";
            this.clhBetfair2.ReadOnly = true;
            // 
            // clhLivescore2
            // 
            resources.ApplyResources(this.clhLivescore2, "clhLivescore2");
            this.clhLivescore2.Name = "clhLivescore2";
            this.clhLivescore2.ReadOnly = true;
            // 
            // ctlLivescores
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.tbcLivescores);
            this.Font = null;
            this.Name = "ctlLivescores";
            this.Resize += new System.EventHandler(this.ctlLivescores_Resize);
            this.Enter += new System.EventHandler(this.ctlLivescores_Enter);
            this.SizeChanged += new System.EventHandler(this.ctlLivescores_SizeChanged);
            this.tbcLivescores.ResumeLayout(false);
            this.tbpLivescore1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLivescore1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tbpLivescore2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLivescore2)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcLivescores;
        private System.Windows.Forms.TabPage tbpLivescore1;
        private System.Windows.Forms.TabPage tbpLivescore2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dgvLivescore1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblBetfairName;
        private System.Windows.Forms.TextBox txtBetfairName;
        private System.Windows.Forms.DataGridView dgvLivescore2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtBetfair2;
        private System.Windows.Forms.Label lblBetfair2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhBetfair;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhLivescore1;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhBetfair2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clhLivescore2;
    }
}
