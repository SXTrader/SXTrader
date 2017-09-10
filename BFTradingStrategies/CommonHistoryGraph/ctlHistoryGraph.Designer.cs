namespace net.sxtrader.bftradingstrategies.common
{
    partial class ctlHistoryGraph
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pnlCheckboxes = new System.Windows.Forms.Panel();
            this.crtHistogramm = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.crtHistogramm)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCheckboxes
            // 
            this.pnlCheckboxes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCheckboxes.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlCheckboxes.Location = new System.Drawing.Point(0, 0);
            this.pnlCheckboxes.Name = "pnlCheckboxes";
            this.pnlCheckboxes.Size = new System.Drawing.Size(200, 328);
            this.pnlCheckboxes.TabIndex = 0;
            this.pnlCheckboxes.LocationChanged += new System.EventHandler(this.pnlCheckboxes_LocationChanged);
            // 
            // crtHistogramm
            // 
            this.crtHistogramm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
            this.crtHistogramm.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.crtHistogramm.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
            this.crtHistogramm.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.crtHistogramm.BorderlineWidth = 2;
            this.crtHistogramm.BorderSkin.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
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
            chartArea1.CursorX.Position = 1;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.CursorY.Position = 1;
            chartArea1.Name = "Default";
            chartArea1.ShadowColor = System.Drawing.Color.Transparent;
            this.crtHistogramm.ChartAreas.Add(chartArea1);
            this.crtHistogramm.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            legend1.IsTextAutoFit = false;
            legend1.Name = " Default";
            this.crtHistogramm.Legends.Add(legend1);
            this.crtHistogramm.Location = new System.Drawing.Point(200, 0);
            this.crtHistogramm.Name = "crtHistogramm";
            series1.ChartArea = "Default";
            series1.Legend = " Default";
            series1.Name = "Series1";
            this.crtHistogramm.Series.Add(series1);
            this.crtHistogramm.Size = new System.Drawing.Size(416, 328);
            this.crtHistogramm.TabIndex = 1;
            this.crtHistogramm.Text = "Histogramm";
            this.crtHistogramm.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(this.crtHistogramm_GetToolTipText);
            // 
            // ctlHistoryGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.crtHistogramm);
            this.Controls.Add(this.pnlCheckboxes);
            this.Name = "ctlHistoryGraph";
            this.Size = new System.Drawing.Size(616, 328);
            this.Load += new System.EventHandler(this.ctlHistoryGraph_Load);
            ((System.ComponentModel.ISupportInitialize)(this.crtHistogramm)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCheckboxes;
        private System.Windows.Forms.DataVisualization.Charting.Chart crtHistogramm;
    }
}
