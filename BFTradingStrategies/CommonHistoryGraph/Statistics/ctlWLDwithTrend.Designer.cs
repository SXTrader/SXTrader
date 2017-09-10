namespace net.sxtrader.bftradingstrategies.common.Statistics
{
    partial class ctlWLDwithTrend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlWLDwithTrend));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.crtWLD = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlTrend = new System.Windows.Forms.Panel();
            this.lblTrend = new System.Windows.Forms.Label();
            this.pnlTrendDisplay = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.crtWLD)).BeginInit();
            this.pnlTrend.SuspendLayout();
            this.SuspendLayout();
            // 
            // crtWLD
            // 
            this.crtWLD.AccessibleDescription = null;
            this.crtWLD.AccessibleName = null;
            resources.ApplyResources(this.crtWLD, "crtWLD");
            this.crtWLD.BackColor = System.Drawing.SystemColors.Control;
            this.crtWLD.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
            this.crtWLD.BorderlineWidth = 2;
            this.crtWLD.BorderSkin.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.ScrollBar.Enabled = false;
            chartArea1.AxisX.ScrollBar.LineColor = System.Drawing.Color.Black;
            chartArea1.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY.ScrollBar.BackColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.ScrollBar.LineColor = System.Drawing.Color.Black;
            chartArea1.BackColor = System.Drawing.Color.OldLace;
            chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea1.BackSecondaryColor = System.Drawing.Color.White;
            chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.Name = "Default";
            chartArea1.ShadowColor = System.Drawing.Color.Transparent;
            this.crtWLD.ChartAreas.Add(chartArea1);
            this.crtWLD.Name = "crtWLD";
            series1.ChartArea = "Default";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            series1.Name = "Default";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.crtWLD.Series.Add(series1);
            // 
            // pnlTrend
            // 
            this.pnlTrend.AccessibleDescription = null;
            this.pnlTrend.AccessibleName = null;
            resources.ApplyResources(this.pnlTrend, "pnlTrend");
            this.pnlTrend.BackgroundImage = null;
            this.pnlTrend.Controls.Add(this.lblTrend);
            this.pnlTrend.Controls.Add(this.pnlTrendDisplay);
            this.pnlTrend.Font = null;
            this.pnlTrend.Name = "pnlTrend";
            // 
            // lblTrend
            // 
            this.lblTrend.AccessibleDescription = null;
            this.lblTrend.AccessibleName = null;
            resources.ApplyResources(this.lblTrend, "lblTrend");
            this.lblTrend.Font = null;
            this.lblTrend.Name = "lblTrend";
            // 
            // pnlTrendDisplay
            // 
            this.pnlTrendDisplay.AccessibleDescription = null;
            this.pnlTrendDisplay.AccessibleName = null;
            resources.ApplyResources(this.pnlTrendDisplay, "pnlTrendDisplay");
            this.pnlTrendDisplay.BackgroundImage = null;
            this.pnlTrendDisplay.Font = null;
            this.pnlTrendDisplay.Name = "pnlTrendDisplay";
            // 
            // ctlWLDwithTrend
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.pnlTrend);
            this.Controls.Add(this.crtWLD);
            this.Font = null;
            this.Name = "ctlWLDwithTrend";
            ((System.ComponentModel.ISupportInitialize)(this.crtWLD)).EndInit();
            this.pnlTrend.ResumeLayout(false);
            this.pnlTrend.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart crtWLD;
        private System.Windows.Forms.Panel pnlTrend;
        private System.Windows.Forms.Panel pnlTrendDisplay;
        private System.Windows.Forms.Label lblTrend;


    }
}
