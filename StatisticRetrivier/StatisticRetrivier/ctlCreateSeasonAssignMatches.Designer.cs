namespace StatisticRetrivier
{
    partial class ctlCreateSeasonAssignMatches
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
            this.gbxSeason = new System.Windows.Forms.GroupBox();
            this.btnCreateSpreadSheet = new System.Windows.Forms.Button();
            this.pnlAssignedMatches = new System.Windows.Forms.Panel();
            this.lvwAssignedMatches = new System.Windows.Forms.ListView();
            this.clhDateAssigned = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhMatchAssigned = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlMoveButtons = new System.Windows.Forms.Panel();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnRemoveSelection = new System.Windows.Forms.Button();
            this.btnMoveAll = new System.Windows.Forms.Button();
            this.btnMoveSelection = new System.Windows.Forms.Button();
            this.pnlUnassignedMatches = new System.Windows.Forms.Panel();
            this.lvwUnassignedMatches = new System.Windows.Forms.ListView();
            this.clhDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhMatch = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlMatchDays = new System.Windows.Forms.Panel();
            this.clbMatchDays = new System.Windows.Forms.CheckedListBox();
            this.clhResult = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhResultAssigned = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbxSeason.SuspendLayout();
            this.pnlAssignedMatches.SuspendLayout();
            this.pnlMoveButtons.SuspendLayout();
            this.pnlUnassignedMatches.SuspendLayout();
            this.pnlMatchDays.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxSeason
            // 
            this.gbxSeason.Controls.Add(this.btnCreateSpreadSheet);
            this.gbxSeason.Controls.Add(this.pnlAssignedMatches);
            this.gbxSeason.Controls.Add(this.pnlMoveButtons);
            this.gbxSeason.Controls.Add(this.pnlUnassignedMatches);
            this.gbxSeason.Controls.Add(this.pnlMatchDays);
            this.gbxSeason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxSeason.Location = new System.Drawing.Point(0, 0);
            this.gbxSeason.Name = "gbxSeason";
            this.gbxSeason.Size = new System.Drawing.Size(1275, 447);
            this.gbxSeason.TabIndex = 0;
            this.gbxSeason.TabStop = false;
            this.gbxSeason.Text = "Saison-Name";
            // 
            // btnCreateSpreadSheet
            // 
            this.btnCreateSpreadSheet.AutoSize = true;
            this.btnCreateSpreadSheet.Location = new System.Drawing.Point(1151, 19);
            this.btnCreateSpreadSheet.Name = "btnCreateSpreadSheet";
            this.btnCreateSpreadSheet.Size = new System.Drawing.Size(124, 23);
            this.btnCreateSpreadSheet.TabIndex = 4;
            this.btnCreateSpreadSheet.Text = "Spreadsheet erzeugen";
            this.btnCreateSpreadSheet.UseVisualStyleBackColor = true;
            this.btnCreateSpreadSheet.Click += new System.EventHandler(this.btnCreateSpreadSheet_Click);
            // 
            // pnlAssignedMatches
            // 
            this.pnlAssignedMatches.Controls.Add(this.lvwAssignedMatches);
            this.pnlAssignedMatches.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlAssignedMatches.Location = new System.Drawing.Point(758, 16);
            this.pnlAssignedMatches.Name = "pnlAssignedMatches";
            this.pnlAssignedMatches.Size = new System.Drawing.Size(390, 428);
            this.pnlAssignedMatches.TabIndex = 3;
            // 
            // lvwAssignedMatches
            // 
            this.lvwAssignedMatches.CheckBoxes = true;
            this.lvwAssignedMatches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhDateAssigned,
            this.clhMatchAssigned,
            this.clhResultAssigned});
            this.lvwAssignedMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwAssignedMatches.FullRowSelect = true;
            this.lvwAssignedMatches.GridLines = true;
            this.lvwAssignedMatches.Location = new System.Drawing.Point(0, 0);
            this.lvwAssignedMatches.Name = "lvwAssignedMatches";
            this.lvwAssignedMatches.Size = new System.Drawing.Size(390, 428);
            this.lvwAssignedMatches.TabIndex = 0;
            this.lvwAssignedMatches.UseCompatibleStateImageBehavior = false;
            this.lvwAssignedMatches.View = System.Windows.Forms.View.Details;
            // 
            // clhDateAssigned
            // 
            this.clhDateAssigned.Text = "Datum";
            // 
            // clhMatchAssigned
            // 
            this.clhMatchAssigned.Text = "Begegnung";
            // 
            // pnlMoveButtons
            // 
            this.pnlMoveButtons.Controls.Add(this.btnRemoveAll);
            this.pnlMoveButtons.Controls.Add(this.btnRemoveSelection);
            this.pnlMoveButtons.Controls.Add(this.btnMoveAll);
            this.pnlMoveButtons.Controls.Add(this.btnMoveSelection);
            this.pnlMoveButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMoveButtons.Location = new System.Drawing.Point(647, 16);
            this.pnlMoveButtons.Name = "pnlMoveButtons";
            this.pnlMoveButtons.Size = new System.Drawing.Size(111, 428);
            this.pnlMoveButtons.TabIndex = 2;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(18, 232);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveAll.TabIndex = 3;
            this.btnRemoveAll.Text = "<<";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnRemoveSelection
            // 
            this.btnRemoveSelection.Location = new System.Drawing.Point(18, 203);
            this.btnRemoveSelection.Name = "btnRemoveSelection";
            this.btnRemoveSelection.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveSelection.TabIndex = 2;
            this.btnRemoveSelection.Text = "<";
            this.btnRemoveSelection.UseVisualStyleBackColor = true;
            this.btnRemoveSelection.Click += new System.EventHandler(this.btnRemoveSelection_Click);
            // 
            // btnMoveAll
            // 
            this.btnMoveAll.Location = new System.Drawing.Point(18, 136);
            this.btnMoveAll.Name = "btnMoveAll";
            this.btnMoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnMoveAll.TabIndex = 1;
            this.btnMoveAll.Text = ">>";
            this.btnMoveAll.UseVisualStyleBackColor = true;
            this.btnMoveAll.Click += new System.EventHandler(this.btnMoveAll_Click);
            // 
            // btnMoveSelection
            // 
            this.btnMoveSelection.Location = new System.Drawing.Point(18, 107);
            this.btnMoveSelection.Name = "btnMoveSelection";
            this.btnMoveSelection.Size = new System.Drawing.Size(75, 23);
            this.btnMoveSelection.TabIndex = 0;
            this.btnMoveSelection.Text = ">";
            this.btnMoveSelection.UseVisualStyleBackColor = true;
            this.btnMoveSelection.Click += new System.EventHandler(this.btnMoveSelection_Click);
            // 
            // pnlUnassignedMatches
            // 
            this.pnlUnassignedMatches.Controls.Add(this.lvwUnassignedMatches);
            this.pnlUnassignedMatches.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlUnassignedMatches.Location = new System.Drawing.Point(257, 16);
            this.pnlUnassignedMatches.Name = "pnlUnassignedMatches";
            this.pnlUnassignedMatches.Size = new System.Drawing.Size(390, 428);
            this.pnlUnassignedMatches.TabIndex = 1;
            // 
            // lvwUnassignedMatches
            // 
            this.lvwUnassignedMatches.CheckBoxes = true;
            this.lvwUnassignedMatches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhDate,
            this.clhMatch,
            this.clhResult});
            this.lvwUnassignedMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwUnassignedMatches.FullRowSelect = true;
            this.lvwUnassignedMatches.GridLines = true;
            this.lvwUnassignedMatches.Location = new System.Drawing.Point(0, 0);
            this.lvwUnassignedMatches.Name = "lvwUnassignedMatches";
            this.lvwUnassignedMatches.Size = new System.Drawing.Size(390, 428);
            this.lvwUnassignedMatches.TabIndex = 0;
            this.lvwUnassignedMatches.UseCompatibleStateImageBehavior = false;
            this.lvwUnassignedMatches.View = System.Windows.Forms.View.Details;
            this.lvwUnassignedMatches.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvwUnassignedMatches_ItemChecked);
            // 
            // clhDate
            // 
            this.clhDate.Text = "Datum";
            // 
            // clhMatch
            // 
            this.clhMatch.Text = "Begegnung";
            // 
            // pnlMatchDays
            // 
            this.pnlMatchDays.Controls.Add(this.clbMatchDays);
            this.pnlMatchDays.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMatchDays.Location = new System.Drawing.Point(3, 16);
            this.pnlMatchDays.Name = "pnlMatchDays";
            this.pnlMatchDays.Size = new System.Drawing.Size(254, 428);
            this.pnlMatchDays.TabIndex = 0;
            // 
            // clbMatchDays
            // 
            this.clbMatchDays.CheckOnClick = true;
            this.clbMatchDays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbMatchDays.FormattingEnabled = true;
            this.clbMatchDays.Location = new System.Drawing.Point(0, 0);
            this.clbMatchDays.Name = "clbMatchDays";
            this.clbMatchDays.Size = new System.Drawing.Size(254, 428);
            this.clbMatchDays.TabIndex = 0;
            this.clbMatchDays.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbMatchDays_ItemCheck);
            // 
            // clhResult
            // 
            this.clhResult.Text = "Ergebnis";
            // 
            // clhResultAssigned
            // 
            this.clhResultAssigned.Text = "Ergebnis";
            // 
            // ctlCreateSeasonAssignMatches
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxSeason);
            this.Name = "ctlCreateSeasonAssignMatches";
            this.Size = new System.Drawing.Size(1275, 447);
            this.gbxSeason.ResumeLayout(false);
            this.gbxSeason.PerformLayout();
            this.pnlAssignedMatches.ResumeLayout(false);
            this.pnlMoveButtons.ResumeLayout(false);
            this.pnlUnassignedMatches.ResumeLayout(false);
            this.pnlMatchDays.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxSeason;
        private System.Windows.Forms.Panel pnlMatchDays;
        private System.Windows.Forms.CheckedListBox clbMatchDays;
        private System.Windows.Forms.Panel pnlUnassignedMatches;
        private System.Windows.Forms.ListView lvwUnassignedMatches;
        private System.Windows.Forms.ColumnHeader clhDate;
        private System.Windows.Forms.ColumnHeader clhMatch;
        private System.Windows.Forms.Panel pnlMoveButtons;
        private System.Windows.Forms.Button btnMoveAll;
        private System.Windows.Forms.Button btnMoveSelection;
        private System.Windows.Forms.Button btnRemoveSelection;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Panel pnlAssignedMatches;
        private System.Windows.Forms.ListView lvwAssignedMatches;
        private System.Windows.Forms.ColumnHeader clhDateAssigned;
        private System.Windows.Forms.ColumnHeader clhMatchAssigned;
        private System.Windows.Forms.Button btnCreateSpreadSheet;
        private System.Windows.Forms.ColumnHeader clhResultAssigned;
        private System.Windows.Forms.ColumnHeader clhResult;
    }
}
