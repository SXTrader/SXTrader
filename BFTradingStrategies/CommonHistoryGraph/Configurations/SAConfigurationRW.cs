using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.common.Configurations
{
    public class SAConfigurationRW
    {
        private XmlDocument m_doc = null;
        private String m_file = String.Empty;

        public SAConfigurationRW()
        {
            m_file = SXDirs.CfgPath + @"\commonconfiguration.xml"; 
            m_doc = new XmlDocument();
            try
            {
                m_doc.Load(m_file);
            }
            catch (System.IO.FileNotFoundException)
            {
                //Datei nicht gefunden => erzeugen
                XmlTextWriter writer = new XmlTextWriter(m_file, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartElement("configuration");
                writer.Close();
                m_doc.Load(m_file);
                createBasicConfiguration();
                save();
            }
        }

        private void createBasicConfiguration()
        {
            //Anzahl der zu lesenden Datensätze
            createConfigNode(HistoryGraph.strNoOfData, "30");

            // Maximales Alter der zu lesenden Datensätze in Monaten
            createConfigNode(HistoryGraph.strCfgKeyAgeOfData, "0");

            //Farbkodierung Ergebnis-Matrix
            createScoreMatrixDefault();

            //Farbkodierung Win/Loss/Draw + Trend
            createWLDDefault();

            //Farbkodierung für Over/Under
            createOUDefault();

            // Farbliche Markierung der Spielauswahl
            createGameSelectionColorsDefault();
        }

       

        private void createColorRange(String configname, double from, double to, Color color, XmlNode rootNode)
        {
            // Farbranges für Prozentanzeige
            XmlElement configurationRanges = m_doc.CreateElement("configrange");
            XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
            attributeRangeType.Value = configname;
            configurationRanges.Attributes.Append(attributeRangeType);

            XmlElement rangeElement = m_doc.CreateElement("range");
            XmlAttribute attributeFrom = m_doc.CreateAttribute("from");
            attributeFrom.Value = from.ToString();
            rangeElement.Attributes.Append(attributeFrom);
            XmlAttribute attributeTo = m_doc.CreateAttribute("to");           
            attributeTo.Value = to.ToString();
            rangeElement.Attributes.Append(attributeTo);
            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
            attributeColor.Value = color.ToArgb().ToString();
            rangeElement.Attributes.Append(attributeColor);
            configurationRanges.AppendChild(rangeElement);

            rootNode.AppendChild(configurationRanges);
        }

        private void createGameSelectionColorsDefault()
        {
            XmlNode rootNode = m_doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = m_doc.ChildNodes[1];

            XmlElement configurationRanges = m_doc.CreateElement("configrange");
            XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");

            attributeRangeType.Value = HistoryGraph.strStatColors;
            configurationRanges.Attributes.Append(attributeRangeType);

            rootNode.AppendChild(configurationRanges);
        }

        private void createOUDefault()
        {
            XmlNode rootNode = m_doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = m_doc.ChildNodes[1];

            // Farben für Trend
            createConfigNode(HistoryGraph.strColorOUTrendOver, Color.Green.ToArgb().ToString());
            createConfigNode(HistoryGraph.strColorOUTrendUnder, Color.Red.ToArgb().ToString());

            // Farbranges für Prozentanzeige
            createColorRange(HistoryGraph.strOUOverPercentColor, 0.0, 100.0, Color.Orange, rootNode);
            createColorRange(HistoryGraph.strOUUnderPercentColor, 0.0, 100.0, Color.Cyan, rootNode);   
        }

        private void createWLDDefault()
        {            
            XmlNode rootNode = m_doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = m_doc.ChildNodes[1];

            // Farben für Trend
            createConfigNode(HistoryGraph.strColorWLDTrendDraw, Color.Blue.ToArgb().ToString());
            createConfigNode(HistoryGraph.strColorWLDTrendLoss, Color.Red.ToArgb().ToString());
            createConfigNode(HistoryGraph.strColorWLDTrendWin, Color.Green.ToArgb().ToString());
            createConfigNode(HistoryGraph.strColorWLDTrendZero, Color.Black.ToArgb().ToString());          

            // Farbranges für Prozentanzeige
            createColorRange(HistoryGraph.strWLDWinPercentColor, 0.0, 100.0, Color.Orange, rootNode);
            createColorRange(HistoryGraph.strWLDLossPercentColor, 0.0, 100.0, Color.Cyan, rootNode);
            createColorRange(HistoryGraph.strWLDDrawPercentColor, 0.0, 100.0, Color.Blue, rootNode);
        }

        private void createScoreMatrixDefault()
        {
            double value = 0.0;
            XmlNode rootNode = m_doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = m_doc.ChildNodes[1];

            XmlElement configurationRanges = m_doc.CreateElement("configrange");
            XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
            attributeRangeType.Value = HistoryGraph.strScoreColorPercent;
            configurationRanges.Attributes.Append(attributeRangeType);

            XmlElement rangeElement = m_doc.CreateElement("range");
            XmlAttribute attributeFrom = m_doc.CreateAttribute("from");
            attributeFrom.Value = value.ToString();
            rangeElement.Attributes.Append(attributeFrom);
            XmlAttribute attributeTo = m_doc.CreateAttribute("to");
            value = 33.0;
            attributeTo.Value = value.ToString();
            rangeElement.Attributes.Append(attributeTo);
            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
            attributeColor.Value = Color.Red.ToArgb().ToString();
            rangeElement.Attributes.Append(attributeColor);
            configurationRanges.AppendChild(rangeElement);

            rangeElement = m_doc.CreateElement("range");
            attributeFrom = m_doc.CreateAttribute("from");
            attributeFrom.Value = value.ToString();
            rangeElement.Attributes.Append(attributeFrom);
            attributeTo = m_doc.CreateAttribute("to");
            value = 66.0;
            attributeTo.Value = value.ToString();
            rangeElement.Attributes.Append(attributeTo);
            attributeColor = m_doc.CreateAttribute("color");
            attributeColor.Value = Color.Yellow.ToArgb().ToString();
            rangeElement.Attributes.Append(attributeColor);
            configurationRanges.AppendChild(rangeElement);

            rangeElement = m_doc.CreateElement("range");
            attributeFrom = m_doc.CreateAttribute("from");
            attributeFrom.Value = value.ToString();
            rangeElement.Attributes.Append(attributeFrom);
            attributeTo = m_doc.CreateAttribute("to");
            value = 100.0;
            attributeTo.Value = value.ToString();
            rangeElement.Attributes.Append(attributeTo);
            attributeColor = m_doc.CreateAttribute("color");
            attributeColor.Value = Color.Green.ToArgb().ToString();
            rangeElement.Attributes.Append(attributeColor);
            configurationRanges.AppendChild(rangeElement);

            rootNode.AppendChild(configurationRanges);
        }

        private void createConfigNode(String name, String value)
        {
            XmlNode rootNode = m_doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = m_doc.ChildNodes[1];
            XmlElement configurationElement = m_doc.CreateElement("configitem");
            XmlAttribute attributeName = m_doc.CreateAttribute("name");
            attributeName.Value = name;
            //attributeName.Value = LayTheDraw.strCheckServerBetsSeconds;
            XmlAttribute attributeValue = m_doc.CreateAttribute("value");
            attributeValue.Value = value;
            //attributeValue.Value = "600";
            configurationElement.Attributes.Append(attributeName);
            configurationElement.Attributes.Append(attributeValue);
            rootNode.AppendChild(configurationElement);
        }

        public void save()
        {
            m_doc.Save(m_file);
        }

        public Color OUTrendColorOver
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorOUTrendOver)
                    {
                        int color = Int32.Parse(node.Attributes["value"].Value);
                        return Color.FromArgb(color);
                    }
                }
                createConfigNode(HistoryGraph.strColorOUTrendOver, Color.Green.ToArgb().ToString());

                return Color.Green;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorOUTrendOver)
                        node.Attributes["value"].Value = value.ToArgb().ToString();
                }
            }
        }

        public Color OUTrendColorUnder
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorOUTrendUnder)
                    {
                        int color = Int32.Parse(node.Attributes["value"].Value);
                        return Color.FromArgb(color);
                    }
                }
                createConfigNode(HistoryGraph.strColorOUTrendUnder, Color.Red.ToArgb().ToString());

                return Color.Red;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorOUTrendUnder)
                        node.Attributes["value"].Value = value.ToArgb().ToString();
                }
            }
        }

        public Color WLDTrendColorWin
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorWLDTrendWin)
                    {
                        int color = Int32.Parse(node.Attributes["value"].Value);
                        return Color.FromArgb(color);
                    }
                }
                createConfigNode(HistoryGraph.strColorWLDTrendWin, Color.Green.ToArgb().ToString());

                return Color.Green;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorWLDTrendWin)
                        node.Attributes["value"].Value = value.ToArgb().ToString();
                }
            }
        }

        public Color WLDTrendColorLoss
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorWLDTrendLoss)
                    {
                        int color = Int32.Parse(node.Attributes["value"].Value);
                        return Color.FromArgb(color);
                    }
                }
                createConfigNode(HistoryGraph.strColorWLDTrendLoss, Color.Red.ToArgb().ToString());

                return Color.Red;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorWLDTrendLoss)
                        node.Attributes["value"].Value = value.ToArgb().ToString();
                }
            }
        }

        public Color WLDTrendColorDraw
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorWLDTrendDraw)
                    {
                        int color = Int32.Parse(node.Attributes["value"].Value);
                        return Color.FromArgb(color);
                    }
                }
                createConfigNode(HistoryGraph.strColorWLDTrendDraw, Color.Yellow.ToArgb().ToString());

                return Color.Yellow;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorWLDTrendDraw)
                        node.Attributes["value"].Value = value.ToArgb().ToString();
                }
            }
        }

        public Color WLDTrendColorZero
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorWLDTrendZero)
                    {
                        int color = Int32.Parse(node.Attributes["value"].Value);
                        return Color.FromArgb(color);
                    }
                }
                createConfigNode(HistoryGraph.strColorWLDTrendZero, Color.Black.ToArgb().ToString());

                return Color.Black;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strColorWLDTrendZero)
                        node.Attributes["value"].Value = value.ToArgb().ToString();
                }
            }
        }

        public RangeColorList UnderRangeColors
        {
            get
            {
                bool bFound = false;
                RangeColorList list = new RangeColorList();

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strOUUnderPercentColor)
                    {
                        bFound = true;
                        foreach (XmlNode nodeColorRange in node.ChildNodes)
                        {

                            double from = Double.Parse(nodeColorRange.Attributes["from"].Value);
                            double to = Double.Parse(nodeColorRange.Attributes["to"].Value);
                            int color = Int32.Parse(nodeColorRange.Attributes["color"].Value);

                            RangeColorElement rangeColor = RangeColorElement.createNew();
                            rangeColor.Lo = from;
                            rangeColor.Hi = to;
                            rangeColor.Color = color;

                            list.Add(rangeColor);
                        }
                    }
                }

                if (!bFound)
                    createColorRange(HistoryGraph.strOUUnderPercentColor, 0.0, 100.0, Color.Cyan, rootNode);
                return list;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strOUUnderPercentColor)
                    {
                        // Alle Alten Werte löschen
                        node.RemoveAll();
                        XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
                        attributeRangeType.Value = HistoryGraph.strOUUnderPercentColor;
                        node.Attributes.Append(attributeRangeType);

                        foreach (RangeColorElement rangeColor in value)
                        {
                            if (rangeColor.Lo == 0.0 && rangeColor.Hi == 0.0)
                                continue;
                            XmlElement rangeElement = m_doc.CreateElement("range");
                            XmlAttribute attributeFrom = m_doc.CreateAttribute("from");
                            attributeFrom.Value = rangeColor.Lo.ToString();
                            rangeElement.Attributes.Append(attributeFrom);
                            XmlAttribute attributeTo = m_doc.CreateAttribute("to");

                            attributeTo.Value = rangeColor.Hi.ToString();
                            rangeElement.Attributes.Append(attributeTo);
                            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
                            attributeColor.Value = rangeColor.Color.ToString();
                            rangeElement.Attributes.Append(attributeColor);
                            node.AppendChild(rangeElement);
                        }
                    }
                }
            }
        }

        public RangeColorList OverRangeColors
        {
            get
            {
                bool bFound = false;
                RangeColorList list = new RangeColorList();

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strOUOverPercentColor)
                    {
                        bFound = true;
                        foreach (XmlNode nodeColorRange in node.ChildNodes)
                        {

                            double from = Double.Parse(nodeColorRange.Attributes["from"].Value);
                            double to = Double.Parse(nodeColorRange.Attributes["to"].Value);
                            int color = Int32.Parse(nodeColorRange.Attributes["color"].Value);

                            RangeColorElement rangeColor = RangeColorElement.createNew();
                            rangeColor.Lo = from;
                            rangeColor.Hi = to;
                            rangeColor.Color = color;

                            list.Add(rangeColor);
                        }
                    }
                }

                if (!bFound)
                    createColorRange(HistoryGraph.strOUOverPercentColor, 0.0, 100.0, Color.Orange, rootNode);
                return list;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strOUOverPercentColor)
                    {
                        // Alle Alten Werte löschen
                        node.RemoveAll();
                        XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
                        attributeRangeType.Value = HistoryGraph.strOUOverPercentColor;
                        node.Attributes.Append(attributeRangeType);

                        foreach (RangeColorElement rangeColor in value)
                        {
                            if (rangeColor.Lo == 0.0 && rangeColor.Hi == 0.0)
                                continue;
                            XmlElement rangeElement = m_doc.CreateElement("range");
                            XmlAttribute attributeFrom = m_doc.CreateAttribute("from");
                            attributeFrom.Value = rangeColor.Lo.ToString();
                            rangeElement.Attributes.Append(attributeFrom);
                            XmlAttribute attributeTo = m_doc.CreateAttribute("to");

                            attributeTo.Value = rangeColor.Hi.ToString();
                            rangeElement.Attributes.Append(attributeTo);
                            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
                            attributeColor.Value = rangeColor.Color.ToString();
                            rangeElement.Attributes.Append(attributeColor);
                            node.AppendChild(rangeElement);
                        }
                    }
                }
            }
        }

        public RangeColorList WLDWinRangeColors
        {
            get
            {
                bool bFound = false;
                RangeColorList list = new RangeColorList();

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strWLDWinPercentColor)
                    {
                        bFound = true;
                        foreach (XmlNode nodeColorRange in node.ChildNodes)
                        {

                            double from = Double.Parse(nodeColorRange.Attributes["from"].Value);
                            double to = Double.Parse(nodeColorRange.Attributes["to"].Value);
                            int color = Int32.Parse(nodeColorRange.Attributes["color"].Value);

                            RangeColorElement rangeColor = RangeColorElement.createNew();
                            rangeColor.Lo = from;
                            rangeColor.Hi = to;
                            rangeColor.Color = color;

                            list.Add(rangeColor);
                        }
                    }
                }

                if(!bFound)
                    createColorRange(HistoryGraph.strWLDWinPercentColor, 0.0, 100.0, Color.Orange, rootNode);
                return list;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strWLDWinPercentColor)
                    {
                        // Alle Alten Werte löschen
                        node.RemoveAll();
                        XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
                        attributeRangeType.Value = HistoryGraph.strWLDWinPercentColor;
                        node.Attributes.Append(attributeRangeType);

                        foreach (RangeColorElement rangeColor in value)
                        {
                            if (rangeColor.Lo == 0.0 && rangeColor.Hi == 0.0)
                                continue;
                            XmlElement rangeElement = m_doc.CreateElement("range");
                            XmlAttribute attributeFrom = m_doc.CreateAttribute("from");
                            attributeFrom.Value = rangeColor.Lo.ToString();
                            rangeElement.Attributes.Append(attributeFrom);
                            XmlAttribute attributeTo = m_doc.CreateAttribute("to");

                            attributeTo.Value = rangeColor.Hi.ToString();
                            rangeElement.Attributes.Append(attributeTo);
                            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
                            attributeColor.Value = rangeColor.Color.ToString();
                            rangeElement.Attributes.Append(attributeColor);
                            node.AppendChild(rangeElement);
                        }
                    }
                }
            }
        }

        public RangeColorList WLDLossRangeColors
        {
            get
            {
                bool bFound = false;
                RangeColorList list = new RangeColorList();

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strWLDLossPercentColor)
                    {
                        bFound = true;
                        foreach (XmlNode nodeColorRange in node.ChildNodes)
                        {
                            double from = Double.Parse(nodeColorRange.Attributes["from"].Value);
                            double to = Double.Parse(nodeColorRange.Attributes["to"].Value);
                            int color = Int32.Parse(nodeColorRange.Attributes["color"].Value);

                            RangeColorElement rangeColor = RangeColorElement.createNew();
                            rangeColor.Lo = from;
                            rangeColor.Hi = to;
                            rangeColor.Color = color;

                            list.Add(rangeColor);
                        }
                    }
                }

                if(!bFound)
                    createColorRange(HistoryGraph.strWLDLossPercentColor, 0.0, 100.0, Color.Cyan, rootNode);
                return list;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strWLDLossPercentColor)
                    {
                        // Alle Alten Werte löschen
                        node.RemoveAll();
                        XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
                        attributeRangeType.Value = HistoryGraph.strWLDLossPercentColor;
                        node.Attributes.Append(attributeRangeType);

                        foreach (RangeColorElement rangeColor in value)
                        {
                            if (rangeColor.Lo == 0.0 && rangeColor.Hi == 0.0)
                                continue;
                            XmlElement rangeElement = m_doc.CreateElement("range");
                            XmlAttribute attributeFrom = m_doc.CreateAttribute("from");
                            attributeFrom.Value = rangeColor.Lo.ToString();
                            rangeElement.Attributes.Append(attributeFrom);
                            XmlAttribute attributeTo = m_doc.CreateAttribute("to");

                            attributeTo.Value = rangeColor.Hi.ToString();
                            rangeElement.Attributes.Append(attributeTo);
                            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
                            attributeColor.Value = rangeColor.Color.ToString();
                            rangeElement.Attributes.Append(attributeColor);
                            node.AppendChild(rangeElement);
                        }
                    }
                }
            }
        }

        public RangeColorList WLDDrawRangeColors
        {
            get
            {
                bool bFound = false;
                RangeColorList list = new RangeColorList();

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strWLDDrawPercentColor)
                    {
                        bFound = true;
                        foreach (XmlNode nodeColorRange in node.ChildNodes)
                        {
                            double from = Double.Parse(nodeColorRange.Attributes["from"].Value);
                            double to = Double.Parse(nodeColorRange.Attributes["to"].Value);
                            int color = Int32.Parse(nodeColorRange.Attributes["color"].Value);

                            RangeColorElement rangeColor = RangeColorElement.createNew();
                            rangeColor.Lo = from;
                            rangeColor.Hi = to;
                            rangeColor.Color = color;

                            list.Add(rangeColor);
                        }
                    }
                }
                if(!bFound)
                    createColorRange(HistoryGraph.strWLDDrawPercentColor, 0.0, 100.0, Color.Blue, rootNode);
                return list;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strWLDDrawPercentColor)
                    {
                        // Alle Alten Werte löschen
                        node.RemoveAll();
                        XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
                        attributeRangeType.Value = HistoryGraph.strWLDDrawPercentColor;
                        node.Attributes.Append(attributeRangeType);

                        foreach (RangeColorElement rangeColor in value)
                        {
                            if (rangeColor.Lo == 0.0 && rangeColor.Hi == 0.0)
                                continue;
                            XmlElement rangeElement = m_doc.CreateElement("range");
                            XmlAttribute attributeFrom = m_doc.CreateAttribute("from");
                            attributeFrom.Value = rangeColor.Lo.ToString();
                            rangeElement.Attributes.Append(attributeFrom);
                            XmlAttribute attributeTo = m_doc.CreateAttribute("to");

                            attributeTo.Value = rangeColor.Hi.ToString();
                            rangeElement.Attributes.Append(attributeTo);
                            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
                            attributeColor.Value = rangeColor.Color.ToString();
                            rangeElement.Attributes.Append(attributeColor);
                            node.AppendChild(rangeElement);
                        }
                    }
                }
            }
        }

        public StatisticsColorSelectionSortedList GameSelectionColors
        {
            get
            {
                StatisticsColorSelectionSortedList theList = new StatisticsColorSelectionSortedList();
                bool bFound = false;
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strStatColors)
                    {
                        bFound = true;
                        foreach (XmlNode nodeStatColor in node.ChildNodes)
                        {
                            if (nodeStatColor.Attributes["name"].Value == HistoryGraph.strStatColor)
                            {
                                StatisticsColorSelectionElement statColElem = StatisticsColorSelectionElement.createNew();
                                statColElem.StatisticColor = Color.FromArgb(Int32.Parse(nodeStatColor.Attributes["color"].Value));
                                try
                                {
                                    statColElem.SortNumber = Int32.Parse(nodeStatColor.Attributes["sort"].Value);
                                }
                                catch (NullReferenceException)
                                {
                                    statColElem.SortNumber = statColElem.RangeNumber;
                                }

                                foreach (XmlNode nodeStatistic in nodeStatColor.ChildNodes)
                                {
                                    StatisticSelectionElement statElem = StatisticSelectionElement.createNew();
                                    statElem.Team = (STATISTICTEAM)Enum.Parse(typeof(STATISTICTEAM), nodeStatistic.Attributes["team"].Value);
                                    statElem.HomeAway = (STATISTICHOMEAWAY)Enum.Parse(typeof(STATISTICHOMEAWAY), nodeStatistic.Attributes["homeaway"].Value);
                                    statElem.Statistic = (STATISTICTYPE)Enum.Parse(typeof(STATISTICTYPE),nodeStatistic.Attributes["statistic"].Value);
                                    statElem.LoRange = Double.Parse(nodeStatistic.Attributes["lo"].Value);
                                    statElem.HiRange = Double.Parse(nodeStatistic.Attributes["hi"].Value);                                    
                                    statColElem.Statistics.Add(statElem);
                                }

                                try
                                {
                                    theList.Add(statColElem.SortNumber, statColElem);
                                }
                                catch (ArgumentException)
                                {
                                    theList.Add(theList.Keys.Last() + 1, statColElem);
                                }
                            }
                        }
                    }
                }

                if (!bFound)
                    createGameSelectionColorsDefault();

                return theList;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strStatColors)
                    {
                        node.RemoveAll();
                        XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
                        attributeRangeType.Value = HistoryGraph.strStatColors;
                        node.Attributes.Append(attributeRangeType);

                        foreach (StatisticsColorSelectionElement statColElem in value.Values)
                        {
                            //Cleanup
                            foreach (StatisticSelectionElement statElem in statColElem.Statistics)
                            {
                                if (statElem.HiRange == 0 && statElem.LoRange == 0)
                                    statColElem.Statistics.Remove(statElem);
                            }

                            if (statColElem.Statistics.Count == 0)
                                continue;

                            XmlElement statColorElement = m_doc.CreateElement("configrange");
                            XmlAttribute attributeName = m_doc.CreateAttribute("name");
                            attributeName.Value = HistoryGraph.strStatColor;
                            statColorElement.Attributes.Append(attributeName);

                            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
                            attributeColor.Value = statColElem.StatisticColor.ToArgb().ToString();
                            statColorElement.Attributes.Append(attributeColor);

                            XmlAttribute attributeSort = m_doc.CreateAttribute("sort");
                            attributeSort.Value = statColElem.SortNumber.ToString();
                            statColorElement.Attributes.Append(attributeSort);

                            foreach (StatisticSelectionElement statElem in statColElem.Statistics)
                            {
                                XmlElement statXmlElem = m_doc.CreateElement("range");

                                XmlAttribute attributeTeam = m_doc.CreateAttribute("team");
                                attributeTeam.Value = statElem.Team.ToString();
                                statXmlElem.Attributes.Append(attributeTeam);

                                XmlAttribute attributeHA = m_doc.CreateAttribute("homeaway");
                                attributeHA.Value = statElem.HomeAway.ToString();
                                statXmlElem.Attributes.Append(attributeHA);

                                XmlAttribute attributeStat = m_doc.CreateAttribute("statistic");
                                attributeStat.Value = statElem.Statistic.ToString();
                                statXmlElem.Attributes.Append(attributeStat);

                                XmlAttribute attributeLo = m_doc.CreateAttribute("lo");
                                attributeLo.Value = statElem.LoRange.ToString();
                                statXmlElem.Attributes.Append(attributeLo);

                                XmlAttribute attributeHi = m_doc.CreateAttribute("hi");
                                attributeHi.Value = statElem.HiRange.ToString();
                                statXmlElem.Attributes.Append(attributeHi);

                                statColorElement.AppendChild(statXmlElem);

                            }

                            node.AppendChild(statColorElement);
                        }
                    }
                }
            }
        }

        public RangeColorList RangeColors
        {
            get
            {
                RangeColorList list = new RangeColorList();

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strScoreColorPercent)
                    {
                        foreach (XmlNode nodeColorRange in node.ChildNodes)
                        {
                            double from = Double.Parse(nodeColorRange.Attributes["from"].Value);
                            double to = Double.Parse(nodeColorRange.Attributes["to"].Value);
                            int color = Int32.Parse(nodeColorRange.Attributes["color"].Value);

                            RangeColorElement rangeColor = RangeColorElement.createNew();
                            rangeColor.Lo = from;
                            rangeColor.Hi = to;
                            rangeColor.Color = color;

                            list.Add(rangeColor);
                        }
                    }
                }

                return list;
            }
            set 
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strScoreColorPercent)
                    {
                        // Alle Alten Werte löschen
                        node.RemoveAll();
                        XmlAttribute attributeRangeType = m_doc.CreateAttribute("name");
                        attributeRangeType.Value = HistoryGraph.strScoreColorPercent;
                        node.Attributes.Append(attributeRangeType);

                        foreach (RangeColorElement rangeColor in value)
                        {
                            if (rangeColor.Lo == 0.0 && rangeColor.Hi == 0.0)
                                continue;
                            XmlElement rangeElement = m_doc.CreateElement("range");
                            XmlAttribute attributeFrom = m_doc.CreateAttribute("from");
                            attributeFrom.Value = rangeColor.Lo.ToString();
                            rangeElement.Attributes.Append(attributeFrom);
                            XmlAttribute attributeTo = m_doc.CreateAttribute("to");
                            
                            attributeTo.Value =rangeColor.Hi.ToString();
                            rangeElement.Attributes.Append(attributeTo);
                            XmlAttribute attributeColor = m_doc.CreateAttribute("color");
                            attributeColor.Value = rangeColor.Color.ToString();
                            rangeElement.Attributes.Append(attributeColor);
                            node.AppendChild(rangeElement);
                        }
                    }
                }
            }
        }

        public int NoOfData
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strNoOfData)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(HistoryGraph.strNoOfData, "30");

                return 30;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strNoOfData)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int AgeOfData
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strCfgKeyAgeOfData)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(HistoryGraph.strCfgKeyAgeOfData, "0");

                return 0;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HistoryGraph.strCfgKeyAgeOfData)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
    }
}
