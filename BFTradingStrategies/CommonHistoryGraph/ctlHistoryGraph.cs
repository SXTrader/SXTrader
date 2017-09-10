using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;


namespace net.sxtrader.bftradingstrategies.common
{
    public partial class ctlHistoryGraph : UserControl
    {
        private delegate void buildCheckBoxes2();
        private buildCheckBoxes2 myBuildInvoke;
        private XDocument m_feedMapper = null;
        private SortedList<string, CheckBox> m_checkboxes = new SortedList<string, CheckBox>();
        private Thread m_updateThread;

        public ctlHistoryGraph()
        {
            myBuildInvoke = new buildCheckBoxes2(buildCheckBoxes);
            InitializeComponent();
            loadXml();
            this.buildCheckBoxes();


            // Set X axis automatic fitting style
            crtHistogramm.Titles.Add(HistoryGraph.strHistogramm);
            crtHistogramm.ChartAreas["Default"].AxisX.IsLabelAutoFit = true;
            crtHistogramm.ChartAreas["Default"].AxisY.IsLabelAutoFit = true;
            crtHistogramm.ChartAreas["Default"].AxisX.LabelAutoFitStyle = 
                LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.WordWrap;
            crtHistogramm.ChartAreas["Default"].AxisY.LabelAutoFitStyle =
                LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.WordWrap;
            
            crtHistogramm.ChartAreas["Default"].AxisX.Title =  HistoryGraph.strDate;
            crtHistogramm.ChartAreas["Default"].AxisY.Title = HistoryGraph.strMoney;

            // Disable end labels for the X axis
            crtHistogramm.ChartAreas["Default"].AxisX.LabelStyle.IsEndLabelVisible = false;

            // Set X axis labels format
            crtHistogramm.ChartAreas["Default"].AxisX.LabelStyle.Format = "D";

            // Set Y axis labels format
            crtHistogramm.ChartAreas["Default"].AxisY.LabelStyle.Format = "C0";


            //Scrollbar
            // Set scrollbar size
            crtHistogramm.ChartAreas["Default"].AxisX.ScrollBar.Size = 10;

            // Show small scroll buttons only
            //crtHistogramm.ChartAreas["Default"].AxisX.ScrollBar.Bu = ScrollBarButtonStyle.SmallScroll;

            // Scrollbars position
            crtHistogramm.ChartAreas["Default"].AxisX.ScrollBar.IsPositionedInside = true;

            // Change scrollbar colors
            crtHistogramm.ChartAreas["Default"].AxisX.ScrollBar.BackColor = Color.LightGray;
            crtHistogramm.ChartAreas["Default"].AxisX.ScrollBar.ButtonColor = Color.Gray;
            crtHistogramm.ChartAreas["Default"].AxisX.ScrollBar.LineColor = Color.Black;

            // Set scrollbar size
            crtHistogramm.ChartAreas["Default"].AxisY.ScrollBar.Size = 10;

            // Show small scroll buttons only
            //crtHistogramm.ChartAreas["Default"].AxisX.ScrollBar.Bu = ScrollBarButtonStyle.SmallScroll;

            // Scrollbars position
            crtHistogramm.ChartAreas["Default"].AxisY.ScrollBar.IsPositionedInside = true;

            // Change scrollbar colors
            crtHistogramm.ChartAreas["Default"].AxisY.ScrollBar.BackColor = Color.LightGray;
            crtHistogramm.ChartAreas["Default"].AxisY.ScrollBar.ButtonColor = Color.Gray;
            crtHistogramm.ChartAreas["Default"].AxisY.ScrollBar.LineColor = Color.Black;

            m_updateThread = new Thread(update);
            m_updateThread.IsBackground = true;
            m_updateThread.Start();
           
            //crtHistogramm.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
        }

        private void update()
        {
            while (true)
            {
                TimeSpan span = new TimeSpan(0, 3, 0);
                Thread.Sleep(span);

                loadXml();
                buildCheckBoxes();
            }
        }

        private void loadXml()
        {
            string filePath = SXDirs.ApplPath + @"\BFTSHistory.xml";
            try
            {
                m_feedMapper = XDocument.Load(filePath);
            }
            catch (System.IO.FileNotFoundException)
            {
            }
        }

        private void buildCheckBoxes()
        {
            if (pnlCheckboxes.InvokeRequired)
            {
                IAsyncResult result = pnlCheckboxes.BeginInvoke(myBuildInvoke);
                pnlCheckboxes.EndInvoke(result);
            }
            else
            {
                SortedList<string, bool>states = new SortedList<string, bool>();

                //foreach (CheckBox cbx in m_checkboxes.Values)
                while(m_checkboxes.Values.Count > 0)
                {
                    CheckBox cbx = m_checkboxes.Values[0];
                    states.Add(cbx.Name, cbx.Checked);
                    if (pnlCheckboxes.Controls.ContainsKey(cbx.Name))
                    {
                        pnlCheckboxes.Controls.RemoveByKey(cbx.Name);
                    }
                    cbx.Dispose();
                    m_checkboxes.RemoveAt(0);
                }

                m_checkboxes = new SortedList<string, CheckBox>();

                // CheckBox All hinzufügen und aktivieren
                CheckBox checkBox = new CheckBox();
                checkBox.Name = HistoryGraph.strAll;
                checkBox.Text = HistoryGraph.strAll;
                checkBox.Visible = true;
                checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                pnlCheckboxes.Controls.Add(checkBox);
                checkBox.Dock = DockStyle.Top;
                m_checkboxes.Add(HistoryGraph.strAll, checkBox);
                checkBox.Checked = true;

                // Nur falls Historydatei existiert
                if (m_feedMapper != null)
                {
                    var histories = from hentry in m_feedMapper.Descendants("hentry")
                                    select new
                                    {
                                        Module = hentry.Attribute("module").Value,
                                        Match = hentry.Attribute("match").Value,
                                        Dts = hentry.Attribute("dts").Value,
                                        Money = hentry.Attribute("money").Value
                                    };
                    foreach (var history in histories)
                    {
                        try
                        {
                            if (m_checkboxes.ContainsKey(history.Module))
                                checkBox = m_checkboxes[history.Module];
                            else
                            {
                                checkBox = new CheckBox();
                                checkBox.Name = history.Module;
                                checkBox.Text = history.Module;
                                checkBox.Visible = true;
                                checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                                pnlCheckboxes.Controls.Add(checkBox);
                                checkBox.Dock = DockStyle.Top;
                                m_checkboxes.Add(history.Module, checkBox);
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            checkBox = new CheckBox();
                            checkBox.Name = history.Module;
                            checkBox.Text = history.Module;
                            checkBox.Visible = true;
                            checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                            pnlCheckboxes.Controls.Add(checkBox);
                            checkBox.Dock = DockStyle.Top;
                            m_checkboxes.Add(history.Module, checkBox);
                        }
                    }
                }

                foreach (CheckBox cbx in pnlCheckboxes.Controls)
                {
                    try
                    {
                        if(states.ContainsKey(cbx.Name))
                            cbx.Checked = states[cbx.Name];                        
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

        }

        void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_feedMapper == null)
                return;

            //foreach (Series s in crtHistogramm.Series)
            while(crtHistogramm.Series.Count > 0)
            {
                Series s = crtHistogramm.Series[0];
                crtHistogramm.Series.Remove(s);
                s.Dispose();
            }

            crtHistogramm.Text = HistoryGraph.strHistogramm;
            foreach (CheckBox checkbox in m_checkboxes.Values)
            {
                double money = 0.0;
                DateTime dts;

                if (!checkbox.Checked)
                    continue;

                if (checkbox.Text == HistoryGraph.strAll)
                {
                    Series series = new Series(HistoryGraph.strAll);
                    series.XAxisType = AxisType.Primary;
                    series.YAxisType = AxisType.Primary;
                    var histories = from hentry in m_feedMapper.Descendants("hentry")
                                    select new
                                    {
                                        Module = hentry.Attribute("module").Value,
                                        Match = hentry.Attribute("match").Value,
                                        Dts = hentry.Attribute("dts").Value,
                                        Money = hentry.Attribute("money").Value
                                    };
                    foreach (var history in histories)
                    {
                        money += Double.Parse(history.Money);
                        dts = new DateTime(long.Parse(history.Dts));
                        series.Points.AddXY(dts, money);
                    }

                    series.ChartType = SeriesChartType.Line;
                    series.XValueType = ChartValueType.DateTime;
                    series.YValueType = ChartValueType.Double;
                    series.MarkerSize = 2;
                    series.MarkerStyle = MarkerStyle.Circle;
                    series.BorderWidth = 3;
                    series.ShadowColor = Color.Black;
                    series.ShadowOffset = 2;


                    // Set series point labels format
                    series.LabelFormat = "P3";                                        
                    
                    crtHistogramm.Series.Add(series);                    
                }
                else
                {
                    Series series = new Series(checkbox.Text);
                    series.XAxisType = AxisType.Primary;
                    series.YAxisType = AxisType.Primary;
                    var histories = from hentry in m_feedMapper.Descendants("hentry")
                                    where hentry.Attribute("module").Value == checkbox.Text 
                                    select new
                                    {
                                        Module = hentry.Attribute("module").Value,
                                        Match = hentry.Attribute("match").Value,
                                        Dts = hentry.Attribute("dts").Value,
                                        Money = hentry.Attribute("money").Value
                                    };
                    foreach (var history in histories)
                    {
                        money += Double.Parse(history.Money);
                        dts = new DateTime(long.Parse(history.Dts));
                        series.Points.AddXY(dts, money);
                    }

                    series.ChartType = SeriesChartType.Line;
                    series.XValueType = ChartValueType.DateTime;
                    series.YValueType = ChartValueType.Double;
                    series.MarkerSize = 2;
                    series.MarkerStyle = MarkerStyle.Circle;
                    series.BorderWidth = 3;
                    series.ShadowColor = Color.Black;
                    series.ShadowOffset = 2;
                    crtHistogramm.Series.Add(series);   
                }
            }

            crtHistogramm.Invalidate();
            //throw new NotImplementedException();
        }

        private void buildGraph()
        {

        }

        private void buildData()
        {
        }

        private void ctlHistoryGraph_Load(object sender, EventArgs e)
        {
            loadXml();
            buildGraph();
            buildData();
            // refresh chart
            
        }

        private void pnlCheckboxes_LocationChanged(object sender, EventArgs e)
        {
            loadXml();
            buildGraph();
            buildData();
            // refresh chart

        }

        private void crtHistogramm_GetToolTipText(object sender, ToolTipEventArgs e)
        {
             // Check selected chart element and set tooltip text
            switch (e.HitTestResult.ChartElementType)
            {
                case ChartElementType.Axis:
                    e.Text = e.HitTestResult.Axis.Name;
                    break;
                case ChartElementType.ScrollBarLargeDecrement:
                    e.Text = "A scrollbar large decrement button";
                    break;
                case ChartElementType.ScrollBarLargeIncrement:
                    e.Text = "A scrollbar large increment button";
                    break;
                case ChartElementType.ScrollBarSmallDecrement:
                    e.Text = "A scrollbar small decrement button";
                    break;
                case ChartElementType.ScrollBarSmallIncrement:
                    e.Text = "A scrollbar small increment button";
                    break;
                case ChartElementType.ScrollBarThumbTracker:
                    e.Text = "A scrollbar tracking thumb";
                    break;
                case ChartElementType.ScrollBarZoomReset:
                    e.Text = "The ZoomReset button of a scrollbar";
                    break;
                case ChartElementType.DataPoint:
                    e.Text = "Value " + e.HitTestResult.Series.Points[e.HitTestResult.PointIndex].YValues[0];
                    //e.Text = "Data Point " + e.HitTestResult.PointIndex.ToString();
                    break;
                case ChartElementType.Gridlines:
                    e.Text = "Grid Lines";
                    break;
                case ChartElementType.LegendArea:
                    e.Text = "Legend Area";
                    break;
                case ChartElementType.LegendItem:
                    e.Text = "Legend Item";
                    break;
                case ChartElementType.PlottingArea:
                    e.Text = "Plotting Area";
                    break;
                case ChartElementType.StripLines:
                    e.Text = "Strip Lines";
                    break;
                case ChartElementType.TickMarks:
                    e.Text = "Tick Marks";
                    break;
                case ChartElementType.Title:
                    e.Text = "Title";
                    break;
            }
        }
    }
}
