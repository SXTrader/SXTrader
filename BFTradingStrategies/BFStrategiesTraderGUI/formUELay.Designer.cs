namespace BFStrategiesTraderGUI
{
    partial class formUELay
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
            this.lvwUE = new System.Windows.Forms.ListView();
            this.clhMatch = new System.Windows.Forms.ColumnHeader();
            this.clhScore = new System.Windows.Forms.ColumnHeader();
            this.clhRiskWin = new System.Windows.Forms.ColumnHeader();
            this.clhPlaytime = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lvwUE
            // 
            this.lvwUE.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhMatch,
            this.clhPlaytime,
            this.clhScore,
            this.clhRiskWin});
            this.lvwUE.FullRowSelect = true;
            this.lvwUE.GridLines = true;
            this.lvwUE.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwUE.Location = new System.Drawing.Point(2, 3);
            this.lvwUE.Name = "lvwUE";
            this.lvwUE.Size = new System.Drawing.Size(571, 315);
            this.lvwUE.TabIndex = 0;
            this.lvwUE.UseCompatibleStateImageBehavior = false;
            this.lvwUE.View = System.Windows.Forms.View.Details;
            // 
            // clhMatch
            // 
            this.clhMatch.Text = "Spiel";
            // 
            // clhScore
            // 
            this.clhScore.Text = "Spielstand";
            // 
            // clhRiskWin
            // 
            this.clhRiskWin.Text = "Gewinn/Verlust";
            // 
            // clhPlaytime
            // 
            this.clhPlaytime.Text = "Spielzeit";
            // 
            // formUELay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 318);
            this.Controls.Add(this.lvwUE);
            this.Name = "formUELay";
            this.Text = "formUELay";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvwUE;
        private System.Windows.Forms.ColumnHeader clhMatch;
        private System.Windows.Forms.ColumnHeader clhScore;
        private System.Windows.Forms.ColumnHeader clhRiskWin;
        private System.Windows.Forms.ColumnHeader clhPlaytime;
    }
}