namespace net.sxtrader.bftradingstrategies.common
{
    partial class ctlMatchDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlMatchDetail));
            this.gbxMatchDetail = new System.Windows.Forms.GroupBox();
            this.lvwMatchDetail = new System.Windows.Forms.ListView();
            this.clhPlayTime = new System.Windows.Forms.ColumnHeader();
            this.clhTeamA = new System.Windows.Forms.ColumnHeader();
            this.clhScore = new System.Windows.Forms.ColumnHeader();
            this.clhTeamB = new System.Windows.Forms.ColumnHeader();
            this.gbxMatchDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxMatchDetail
            // 
            this.gbxMatchDetail.AccessibleDescription = null;
            this.gbxMatchDetail.AccessibleName = null;
            resources.ApplyResources(this.gbxMatchDetail, "gbxMatchDetail");
            this.gbxMatchDetail.BackgroundImage = null;
            this.gbxMatchDetail.Controls.Add(this.lvwMatchDetail);
            this.gbxMatchDetail.Font = null;
            this.gbxMatchDetail.Name = "gbxMatchDetail";
            this.gbxMatchDetail.TabStop = false;
            // 
            // lvwMatchDetail
            // 
            this.lvwMatchDetail.AccessibleDescription = null;
            this.lvwMatchDetail.AccessibleName = null;
            resources.ApplyResources(this.lvwMatchDetail, "lvwMatchDetail");
            this.lvwMatchDetail.BackgroundImage = null;
            this.lvwMatchDetail.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhPlayTime,
            this.clhTeamA,
            this.clhScore,
            this.clhTeamB});
            this.lvwMatchDetail.Font = null;
            this.lvwMatchDetail.FullRowSelect = true;
            this.lvwMatchDetail.GridLines = true;
            this.lvwMatchDetail.MultiSelect = false;
            this.lvwMatchDetail.Name = "lvwMatchDetail";
            this.lvwMatchDetail.UseCompatibleStateImageBehavior = false;
            this.lvwMatchDetail.View = System.Windows.Forms.View.Details;
            // 
            // clhPlayTime
            // 
            resources.ApplyResources(this.clhPlayTime, "clhPlayTime");
            // 
            // clhTeamA
            // 
            resources.ApplyResources(this.clhTeamA, "clhTeamA");
            // 
            // clhScore
            // 
            resources.ApplyResources(this.clhScore, "clhScore");
            // 
            // clhTeamB
            // 
            resources.ApplyResources(this.clhTeamB, "clhTeamB");
            // 
            // ctlMatchDetail
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.gbxMatchDetail);
            this.Font = null;
            this.Name = "ctlMatchDetail";
            this.gbxMatchDetail.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxMatchDetail;
        private System.Windows.Forms.ListView lvwMatchDetail;
        private System.Windows.Forms.ColumnHeader clhPlayTime;
        private System.Windows.Forms.ColumnHeader clhTeamA;
        private System.Windows.Forms.ColumnHeader clhScore;
        private System.Windows.Forms.ColumnHeader clhTeamB;
    }
}
