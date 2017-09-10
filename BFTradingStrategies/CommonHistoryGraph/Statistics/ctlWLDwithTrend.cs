using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using net.sxtrader.bftradingstrategies.common.Configurations;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.sxstatisticbase;

namespace net.sxtrader.bftradingstrategies.common.Statistics
{
    public partial class ctlWLDwithTrend : UserControl
    {
        public ctlWLDwithTrend()
        {
            InitializeComponent();
        }

        public HistoricWLDData TrendDate
        {
            set
            {
                try
                {
                    SAConfigurationRW config = new SAConfigurationRW();

                    while(crtWLD.Series["Default"].Points.Count > 0)
                    {
                        crtWLD.Series["Default"].Points[0].Dispose();
                        crtWLD.Series["Default"].Points.RemoveAt(0);
                    }
                    crtWLD.Series["Default"].Points.Add(new DataPoint(1, Math.Round(value.WinPercent, 2)));
                    crtWLD.Series["Default"].Points[0].AxisLabel = HistoryGraph.strWin;
                    crtWLD.Series["Default"].Points[0].Color = getColor(value.WinPercent, config.WLDWinRangeColors);//Color.Cyan;
                    crtWLD.Series["Default"].Points.Add(new DataPoint(2, Math.Round(value.LossPercent, 2)));
                    crtWLD.Series["Default"].Points[1].AxisLabel = HistoryGraph.strLoss;
                    crtWLD.Series["Default"].Points[1].Color = getColor(value.LossPercent, config.WLDLossRangeColors);//Color.Orange;
                    crtWLD.Series["Default"].Points.Add(new DataPoint(3, Math.Round(value.DrawPercent, 2)));
                    crtWLD.Series["Default"].Points[2].AxisLabel = HistoryGraph.strDraw;
                    crtWLD.Series["Default"].Points[2].Color = getColor(value.DrawPercent, config.WLDDrawRangeColors);

                    crtWLD.Invalidate();

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
                        if (value.Trend[i] == HistoricWLDData.EQUAL)
                            lbl.BackColor = config.WLDTrendColorDraw;//Color.Yellow;
                        else if (value.Trend[i] == HistoricWLDData.NEGATIV)
                            lbl.BackColor = config.WLDTrendColorLoss;//Color.Red;
                        else if (value.Trend[i] == HistoricWLDData.POSITV)
                            lbl.BackColor = config.WLDTrendColorWin;//Color.Green;
                        else if (value.Trend[i] == HistoricWLDData.ZERO)
                            lbl.BackColor = config.WLDTrendColorZero;//Color.Black;

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
