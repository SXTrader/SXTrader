using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.common.Configurations;
using System.Windows.Forms.DataVisualization.Charting;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.sxstatisticbase;

namespace net.sxtrader.bftradingstrategies.common.Statistics
{
    public partial class ctlOverUnderWithTrend : UserControl
    {
        public ctlOverUnderWithTrend()
        {
            InitializeComponent();
        }

        public HistoricOverUnderData TrendData
        {
            set
            {
                try
                {
                    SAConfigurationRW config = new SAConfigurationRW();

                    while(crtOU.Series["Default"].Points.Count>0)
                    {
                        crtOU.Series["Default"].Points[0].Dispose();
                        crtOU.Series["Default"].Points.RemoveAt(0);
                    }
                    crtOU.Series["Default"].Points.Add(new DataPoint(1, Math.Round(value.OverPercent, 2)));
                    crtOU.Series["Default"].Points[0].AxisLabel = HistoryGraph.strOver;
                    crtOU.Series["Default"].Points[0].Color = getColor(value.OverPercent, config.OverRangeColors);//Color.Cyan;
                    crtOU.Series["Default"].Points.Add(new DataPoint(2, Math.Round(100.0 - value.OverPercent, 2)));
                    crtOU.Series["Default"].Points[1].AxisLabel = HistoryGraph.strUnder;
                    crtOU.Series["Default"].Points[1].Color = getColor(100.0 - value.OverPercent, config.UnderRangeColors);//Color.Orange;               

                    crtOU.Invalidate();
                    
                    while(this.pnlTrendDisplay.Controls.Count > 0)
                    {
                        this.pnlTrendDisplay.Controls[0].Dispose();
                    }
                    for (int i = 0; i < value.Trend.Length; i++)
                    {
                        //TODO: Wert über Customizing festlegen
                        if (i >= 10)
                            break;

                        Label lbl = new Label();
                        lbl.BorderStyle = BorderStyle.FixedSingle;

                        lbl.Size = new Size(32, 32);
                        if (value.Trend[i] == HistoricOverUnderData.NEGATIV)
                            lbl.BackColor = config.OUTrendColorUnder;//Color.Red;
                        else if (value.Trend[i] == HistoricOverUnderData.POSITV)
                            lbl.BackColor = config.OUTrendColorOver;//Color.Green;                   
                        if (pnlTrendDisplay.Controls.Count == 0)
                            lbl.Left = 2;
                        else
                            lbl.Left = pnlTrendDisplay.Controls[pnlTrendDisplay.Controls.Count - 1].Right + 2;
                        //lbl.Dock = DockStyle.Left;
                        pnlTrendDisplay.Controls.Add(lbl);
                    }

                    if (value.Trend.Length > 10)
                    {
                        lblTrend.Text = String.Format(HistoryGraph.strTrendLabelFormat, 10);
                    }
                    else
                    {
                        lblTrend.Text = String.Format(HistoryGraph.strTrendLabelFormat, value.Trend.Length);
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private Color getColor(double value, RangeColorList colorList)
        {
            Color color = SystemColors.Control;
            foreach (RangeColorElement colorElement in colorList)
            {
                if (value >= colorElement.Lo && value <= colorElement.Hi)
                {
                    color = Color.FromArgb(colorElement.Color);
                    break;
                }
            }
            return color;
        }
    }
}
