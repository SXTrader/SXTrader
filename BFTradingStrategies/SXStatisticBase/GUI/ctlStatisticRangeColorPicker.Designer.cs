namespace net.sxtrader.bftradingstrategies.sxstatisticbase.GUI
{
    partial class ctlStatisticRangeColorPicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlStatisticRangeColorPicker));
            this.pnlColorButton = new System.Windows.Forms.Panel();
            this.btnStatColor2 = new net.sxtrader.muk.ColorButton();
            this.lblCheckOrder = new System.Windows.Forms.Label();//ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.spnSortNo = new System.Windows.Forms.NumericUpDown();
            this.pnlButton = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.pnlStatisticsRange = new System.Windows.Forms.Panel();
            this.pnlColorButton.SuspendLayout();
            this.pnlButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlColorButton
            // 
            this.pnlColorButton.Controls.Add(this.btnStatColor2);
            this.pnlColorButton.Controls.Add(this.lblCheckOrder);
            this.pnlColorButton.Controls.Add(this.spnSortNo);
            resources.ApplyResources(this.pnlColorButton, "pnlColorButton");
            this.pnlColorButton.Name = "pnlColorButton";
            // 
            // btnStatColor2
            // 
            this.btnStatColor2.Automatic = "Automatic";
            this.btnStatColor2.Color = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnStatColor2, "btnStatColor2");
            this.btnStatColor2.MoreColors = "More Colors...";
            this.btnStatColor2.Name = "btnStatColor2";
            this.btnStatColor2.UseVisualStyleBackColor = true;
            this.btnStatColor2.Changed += new System.EventHandler(this.btnStatColor2_Changed);
            // 
            // lblCheckOrder
            // 
            //this.lblCheckOrder.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            resources.ApplyResources(this.lblCheckOrder, "lblCheckOrder");
            this.lblCheckOrder.Name = "lblCheckOrder";
            //this.lblCheckOrder.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
            this.lblCheckOrder.Text = "Check Order:";
            //this.lblCheckOrder.Values.ExtraText = resources.GetString("lblCheckOrder.Values.ExtraText");
            //this.lblCheckOrder.Values.Image = ((System.Drawing.Image)(resources.GetObject("lblCheckOrder.Values.Image")));
            //this.lblCheckOrder.Values.Text = resources.GetString("lblCheckOrder.Values.Text");
            // 
            // spnSortNo
            // 
            //this.spnSortNo.InputControlStyle = ComponentFactory.Krypton.Toolkit.InputControlStyle.Standalone;
            resources.ApplyResources(this.spnSortNo, "spnSortNo");
            this.spnSortNo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnSortNo.Name = "spnSortNo";
            //this.spnSortNo.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem;
            //this.spnSortNo.UpDownButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.InputControl;
            this.spnSortNo.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnSortNo.ValueChanged += new System.EventHandler(this.spnSortNo_ValueChanged);
            // 
            // pnlButton
            // 
            this.pnlButton.Controls.Add(this.btnDelete);
            this.pnlButton.Controls.Add(this.btnNew);
            resources.ApplyResources(this.pnlButton, "pnlButton");
            this.pnlButton.Name = "pnlButton";
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            resources.ApplyResources(this.btnNew, "btnNew");
            this.btnNew.Name = "btnNew";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // pnlStatisticsRange
            // 
            resources.ApplyResources(this.pnlStatisticsRange, "pnlStatisticsRange");
            this.pnlStatisticsRange.Name = "pnlStatisticsRange";
            // 
            // ctlStatisticRangeColorPicker
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlStatisticsRange);
            this.Controls.Add(this.pnlButton);
            this.Controls.Add(this.pnlColorButton);
            this.Name = "ctlStatisticRangeColorPicker";
            this.pnlColorButton.ResumeLayout(false);
            this.pnlColorButton.PerformLayout();
            this.pnlButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlColorButton;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Panel pnlStatisticsRange;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.NumericUpDown spnSortNo;
        private System.Windows.Forms.Label lblCheckOrder;
        private muk.ColorButton btnStatColor2;
    }
}
