namespace StatisticRetrivier
{
    partial class ctlCreateSeasonDefineSeason
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
            this.lblSeasonName = new System.Windows.Forms.Label();
            this.cbxSeason = new System.Windows.Forms.ComboBox();
            this.lblMatchDays = new System.Windows.Forms.Label();
            this.spnMatchdays = new System.Windows.Forms.NumericUpDown();
            this.btnNext = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.spnMatchdays)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSeasonName
            // 
            this.lblSeasonName.AutoSize = true;
            this.lblSeasonName.Location = new System.Drawing.Point(4, 0);
            this.lblSeasonName.Name = "lblSeasonName";
            this.lblSeasonName.Size = new System.Drawing.Size(42, 13);
            this.lblSeasonName.TabIndex = 0;
            this.lblSeasonName.Text = "Saison:";
            // 
            // cbxSeason
            // 
            this.cbxSeason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSeason.FormattingEnabled = true;
            this.cbxSeason.Items.AddRange(new object[] {
            "2000/2001",
            "2001/2002",
            "2002/2003",
            "2003/2004",
            "2004/2005",
            "2005/2006",
            "2006/2007",
            "2007/2008",
            "2008/2009",
            "2010/2011",
            "2011/2012",
            "2012/2013",
            "2013/2014"});
            this.cbxSeason.Location = new System.Drawing.Point(7, 17);
            this.cbxSeason.Name = "cbxSeason";
            this.cbxSeason.Size = new System.Drawing.Size(121, 21);
            this.cbxSeason.TabIndex = 1;
            // 
            // lblMatchDays
            // 
            this.lblMatchDays.AutoSize = true;
            this.lblMatchDays.Location = new System.Drawing.Point(7, 67);
            this.lblMatchDays.Name = "lblMatchDays";
            this.lblMatchDays.Size = new System.Drawing.Size(86, 13);
            this.lblMatchDays.TabIndex = 2;
            this.lblMatchDays.Text = "Anzahl Spieltage";
            // 
            // spnMatchdays
            // 
            this.spnMatchdays.Location = new System.Drawing.Point(7, 84);
            this.spnMatchdays.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.spnMatchdays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnMatchdays.Name = "spnMatchdays";
            this.spnMatchdays.Size = new System.Drawing.Size(120, 20);
            this.spnMatchdays.TabIndex = 3;
            this.spnMatchdays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(160, 155);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // ctlCreateSeasonDefineSeason
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.spnMatchdays);
            this.Controls.Add(this.lblMatchDays);
            this.Controls.Add(this.cbxSeason);
            this.Controls.Add(this.lblSeasonName);
            this.Name = "ctlCreateSeasonDefineSeason";
            this.Size = new System.Drawing.Size(257, 182);
            ((System.ComponentModel.ISupportInitialize)(this.spnMatchdays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSeasonName;
        private System.Windows.Forms.ComboBox cbxSeason;
        private System.Windows.Forms.Label lblMatchDays;
        private System.Windows.Forms.NumericUpDown spnMatchdays;
        private System.Windows.Forms.Button btnNext;
    }
}
