using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.BackThe4
{
    public class BT4ConfigurationRW
    {
        private XmlDocument m_doc = null;
        private String m_file = String.Empty;

        public BT4ConfigurationRW(BT4ConfigurationRW config)
        {
            m_doc = new XmlDocument();
            m_doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><configuration></configuration>");

            /*XmlElement configurationElement = m_doc.CreateElement("configuration");
            m_doc.AppendChild(configurationElement);
             */

            // Strategie automatisch aktiviert?
            createConfigNode("StrategyActivated", config.StrategyActivated.ToString());

            // Anzahl der zu erzielenden Tore bevor Trade abgeschlossen wird
            createConfigNode("CloseTradeGoals", config.CloseTradeGoals.ToString());

            // Anzahl Sekunden bevor Abschlusshandel getätigt wird
            createConfigNode("CloseTradeWaitSeconds", config.CloseTradeWaitSeconds.ToString());

            // Trade auch mit Verlust?
            createConfigNode("CloseTradeLoss", config.CloseTradeLoss.ToString());

            // Soviel Prozent Gewinn wird erwartet
            createConfigNode("CloseTradeProfit", config.CloseTradeProfit.ToString());

            // Spielminute in der die Stop/Loose-Überwachung gestartet werden soll
            createConfigNode("StopLossPlaytime", config.StopLossPlaytime.ToString());

            // x-faches der Anfangsquote, ab der Stop-Loss gehandelt werden soll
            createConfigNode("StopLossInitFactor", config.StopLossInitFactor.ToString());

            // Maximalquote bei der ein Stop-Loss durchgeführt werden soll
            createConfigNode("StopLossMax", config.StopLossMax.ToString());

            // Anzahl der Minuten nachdem ein nicht abgeschlossender Handel erneut getätigt wird
            createConfigNode("TradeWatchdogMinutes", config.TradeWatchdogMinutes.ToString());

            //////////////////////////////////////////////
            // Sound
            //////////////////////////////////////////////
            // Sounds generell
            createConfigNode(BackThe4.strPlaySounds, config.PlaySounds.ToString());

            // Ton Spiel hinzugefügt
            createConfigNode(BackThe4.strPlayMatchAddedSound, config.PlayMatchAdded.ToString());
            // Datei Spiel hinzugefügt
            createConfigNode(BackThe4.strFileMatchAddedSound, config.FileMatchAdded);

            // Ton Spiel beendet
            createConfigNode(BackThe4.strPlayGameEndedSound, config.PlayGameEnded.ToString());
            // Datei Spiel beendet
            createConfigNode(BackThe4.strFileGameEndedSound, config.FileGameEnded);

            // Ton Spielstand geändert
            createConfigNode(BackThe4.strPlayScoreChangedSound, config.PlayScoreChanged.ToString());
            // Datei Spielstand geändert
            createConfigNode(BackThe4.strFileScoreChangedSound, config.FileScoreChanged);

            // Ton Trading geändert
            createConfigNode(BackThe4.strPlayTradingChangedSound, config.PlayTradingChanged.ToString());
            // Datei Trading geändert
            createConfigNode(BackThe4.strFileTradingChangedSound, config.FileTradingChanged); 
        }

        public BT4ConfigurationRW()
        {
            m_file = Application.StartupPath + @"\Plugins\BackThe4\Configuration.xml";
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
            // Strategie automatisch aktiviert?
            createConfigNode("StrategyActivated", Boolean.FalseString);

            // Anzahl der zu erzielenden Tore bevor Trade abgeschlossen wird
            createConfigNode("CloseTradeGoals", "1");

            // Anzahl Sekunden bevor Abschlusshandel getätigt wird
            createConfigNode("CloseTradeWaitSeconds", "150");

            // Trade auch mit Verlust?
            createConfigNode("CloseTradeLoss", Boolean.FalseString);

            // Soviel Prozent Gewinn wird erwartet
            createConfigNode("CloseTradeProfit", "0");

            // Spielminute in der die Stop/Loose-Überwachung gestartet werden soll
            createConfigNode("StopLossPlaytime", "30");

            // x-faches der Anfangsquote, ab der Stop-Loss gehandelt werden soll
            createConfigNode("StopLossInitFactor", "2");

            // Maximalquote bei der ein Stop-Loss durchgeführt werden soll
            createConfigNode("StopLossMax", "10");

            // Anzahl der Minuten nachdem ein nicht abgeschlossender Handel erneut getätigt wird
            createConfigNode("TradeWatchdogMinutes", "3");

            //////////////////////////////////////////////////////////////////
            ////    SOUND
            /////////////////////////////////////////////////////////////////
            // Generell Töne Abspielen?
            createConfigNode(BackThe4.strPlaySounds, Boolean.FalseString);

            // Ton Spiel hinzugefügt
            createConfigNode(BackThe4.strPlayMatchAddedSound, Boolean.FalseString);
            // Datei Spiel hinzugefügt
            createConfigNode(BackThe4.strFileMatchAddedSound, String.Empty);

            // Ton Spiel beendet
            createConfigNode(BackThe4.strPlayGameEndedSound, Boolean.FalseString);
            // Datei Spiel beendet
            createConfigNode(BackThe4.strFileGameEndedSound, String.Empty);

            // Ton Spielstandsänderung
            createConfigNode(BackThe4.strPlayScoreChangedSound, Boolean.FalseString);
            // Datei Spiestandsänderung
            createConfigNode(BackThe4.strFileScoreChangedSound, String.Empty);

            // Ton Tradingänderung
            createConfigNode(BackThe4.strPlayTradingChangedSound, Boolean.FalseString);
            // Datei Tradingänderung
            createConfigNode(BackThe4.strFileTradingChangedSound, String.Empty);

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
            //attributeName.Value = BackThe4.strCheckServerBetsSeconds;
            XmlAttribute attributeValue = m_doc.CreateAttribute("value");
            attributeValue.Value = value;
            //attributeValue.Value = "600";
            configurationElement.Attributes.Append(attributeName);
            configurationElement.Attributes.Append(attributeValue);
            rootNode.AppendChild(configurationElement);
        }


        public Boolean PlaySounds
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlaySounds)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(BackThe4.strPlaySounds, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlaySounds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }


        public Boolean PlayMatchAdded
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlayMatchAddedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(BackThe4.strPlayMatchAddedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlayMatchAddedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }


        public String FileMatchAdded
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strFileMatchAddedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(BackThe4.strFileMatchAddedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strFileMatchAddedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean PlayGameEnded
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlayGameEndedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(BackThe4.strPlayGameEndedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlayGameEndedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public String FileGameEnded
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strFileGameEndedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(BackThe4.strFileGameEndedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strFileGameEndedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean PlayScoreChanged
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlayScoreChangedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(BackThe4.strPlayScoreChangedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlayScoreChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public String FileScoreChanged
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strFileScoreChangedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(BackThe4.strFileScoreChangedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strFileScoreChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean PlayTradingChanged
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlayTradingChangedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(BackThe4.strPlayTradingChangedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strPlayTradingChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public String FileTradingChanged
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strFileTradingChangedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(BackThe4.strFileTradingChangedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BackThe4.strFileTradingChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean StrategyActivated
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StrategyActivated")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("StrategyActivated", Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StrategyActivated")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int CloseTradeGoals
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CloseTradeGoals")
                        return Int32.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("CloseTradeGoals", "1");
                return 1;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CloseTradeGoals")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int CloseTradeWaitSeconds
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CloseTradeWaitSeconds")
                        return Int32.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("CloseTradeWaitSeconds", "150");
                return 150;
            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CloseTradeWaitSeconds")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean CloseTradeLoss
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CloseTradeLoss")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("CloseTradeLoss", Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CloseTradeLoss")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Double CloseTradeProfit
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CloseTradeProfit")
                        return Double.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("CloseTradeProfit", "0");
                return 0;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CloseTradeProfit")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int StopLossPlaytime
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StopLossPlaytime")
                        return Int32.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("StopLossPlaytime", "30");
                return 30;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StopLossPlaytime")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Double StopLossInitFactor
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StopLossInitFactor")
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 2.0;
                createConfigNode("StopLossInitFactor", value.ToString());
                return value;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StopLossInitFactor")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Double StopLossMax
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StopLossMax")
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 10.0;
                createConfigNode("StopLossMax", value.ToString());
                return value;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StopLossMax")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int TradeWatchdogMinutes
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "TradeWatchdogMinutes")
                        return Int32.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("TradeWatchdogMinutes", "3");
                return 3;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "TradeWatchdogMinutes")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
    }
}
