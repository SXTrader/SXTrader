using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    class GenConfigurationRW
    {
        private XmlDocument m_doc = null;
        private String m_file = String.Empty;

        public GenConfigurationRW()
        {
            m_file = SXDirs.CfgPath + @"\Configuration.xml";
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
            }
        }

        private void createBasicConfiguration()
        {            
            // Intervall Abfrage Liveticker
            createConfigNode("LivetickerCheck", "30");

            // Intervall Abfrage Betfair auf Wetten
            createConfigNode("BetfairCheck", "600");  
          
            //Debug-Informationen
            createConfigNode("WriteDebugFile", Boolean.FalseString);

            // Fast Bets bestätigen
            createConfigNode("ConfirmFastBet", Boolean.TrueString);

            //Liveticker loggen
            createConfigNode("LogLiveticker", Boolean.FalseString);

            //Trades loggen
            createConfigNode("LogTrades", Boolean.FalseString);
            createConfigNode("LogBetAmounts", Boolean.FalseString);
            m_doc.Save(m_file);



        }

        public void save()
        {
            m_doc.Save(m_file);
        }

        private void createConfigNode(String name, String value)
        {
            XmlNode rootNode = m_doc.ChildNodes[1];
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

        public int LivetickerCheck
        {
            get
            {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == "LivetickerCheck")
                            return Int16.Parse(node.Attributes["value"].Value);
                    }
                    createConfigNode("LivetickerCheck", "30");

                    return 30;
            }

            set
            {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == "LivetickerCheck")
                            node.Attributes["value"].Value = value.ToString();
                    }
            }
        }

        public int BetfairCheck
        {
            get
            {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == "BetfairCheck")
                            return Int16.Parse(node.Attributes["value"].Value);
                    }
                    createConfigNode("BetfairCheck", "600");

                    return 600;
            }

            set
            {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == "BetfairCheck")
                            node.Attributes["value"].Value = value.ToString();
                    }
            }
        }

        public Boolean WriteDebugFile
        {
            get
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "WriteDebugFile")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }
                createConfigNode("WriteDebugFile", Boolean.FalseString);

                return false;
            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "WriteDebugFile")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean ConfirmFastBet
        {
            get
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ConfirmFastBet")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }
                createConfigNode("ConfirmFastBet", Boolean.FalseString);

                return false;
            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ConfirmFastBet")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean LogLiveticker
        {
            get
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "LogLiveticker")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }
                createConfigNode("LogLiveticker", Boolean.FalseString);

                return false;
            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "LogLiveticker")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean LogTrades
        {
            get
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "LogTrades")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }
                createConfigNode("LogTrades", Boolean.FalseString);

                return false;
            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "LogTrades")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean LogBetAmounts
        {
            get
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "LogBetAmounts")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }
                createConfigNode("LogBetAmounts", Boolean.FalseString);

                return false;
            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "LogBetAmounts")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
    }
}
