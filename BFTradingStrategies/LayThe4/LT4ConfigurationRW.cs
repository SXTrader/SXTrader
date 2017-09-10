using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace net.sxtrader.bftradingstrategies.LayThe4
{
    public class LT4ConfigurationRW
    {
        private XmlDocument m_doc = null;
        private String m_file = String.Empty;

        public LT4ConfigurationRW(LT4ConfigurationRW config)
        {

            m_doc = new XmlDocument();
            m_doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><configuration></configuration>");

            /*XmlElement configurationElement = m_doc.CreateElement("configuration");
            m_doc.AppendChild(configurationElement);
             */
            // Automatisches Trading
            createConfigNode("AutomaticTrading", config.AutomaticTrading.ToString());

            // Minimale Quote für automatisches Trading            
            createConfigNode("MinOdds", config.MinOdds.ToString());

            // Maximale Quote für automatisches Trading
            createConfigNode("MaxOdds", config.MaxOdds.ToString());

            // Prozent der Bank das eingesetzt werden soll
            createConfigNode("PercentBank", config.PercentBank.ToString());

            // Minimaler Einsatz
            createConfigNode("MinAmount", config.MinAmount.ToString());

            // Maximaler Einsatz
            createConfigNode("MaxAmount", config.MaxAmount.ToString());

            // Soll geringer Betrag gesetzt werden?
            createConfigNode("PlaceLower", config.PlaceLower.ToString());

            // Volumen Markt
            createConfigNode("MarketVolume", config.MarketVolume.ToString());

            // Strategie automatisch aktiviert?
            createConfigNode("StrategyActivated", config.StrategyActivated.ToString());

            // Startzeit für Ausstiegsüberwachung
            createConfigNode("StartPlaytime", config.StartPlaytime.ToString());

            // Wartezeit, falls kein Gewinn
            createConfigNode("NoProfitWait", config.NoProfitWait.ToString());

            // Erwarte Prozent an Profit
            createConfigNode("ProfitPercent", config.ProfitPercent.ToString());

            // Wartezeit, falls erwartete Prozentsatz an Profit nicht vorhandne
            createConfigNode("ProfitWait", config.ProfitWait.ToString());

            // Anzahl der Tore für Notausstieg
            createConfigNode("Goals", config.Goals.ToString());

            // Wartezeit Notausstieg
            createConfigNode("ExitSeconds", config.ExitSeconds.ToString());

            // Verhalten bei keinen Gewinn
            createConfigNode("NoProfit", config.NoProfit.ToString());

            // Verhalten in Gewinnfall
            createConfigNode("Profit", config.Profit.ToString());

            // Aktives oder Passives Tradingverhalten
            createConfigNode("ActivePassive", config.ActivePassive.ToString());

            //////////////////////////////////////////////
            // Sound
            //////////////////////////////////////////////
            // Sounds generell
            createConfigNode(LayThe4.strPlaySounds, config.PlaySounds.ToString());

            // Ton Spiel hinzugefügt
            createConfigNode(LayThe4.strPlayMatchAddedSound, config.PlayMatchAdded.ToString());
            // Datei Spiel hinzugefügt
            createConfigNode(LayThe4.strFileMatchAddedSound, config.FileMatchAdded);

            // Ton Spiel beendet
            createConfigNode(LayThe4.strPlayGameEndedSound, config.PlayGameEnded.ToString());
            // Datei Spiel beendet
            createConfigNode(LayThe4.strFileGameEndedSound, config.FileGameEnded);

            // Ton Spielstand geändert
            createConfigNode(LayThe4.strPlayScoreChangedSound, config.PlayScoreChanged.ToString());
            // Datei Spielstand geändert
            createConfigNode(LayThe4.strFileScoreChangedSound, config.FileScoreChanged);

            // Ton Trading geändert
            createConfigNode(LayThe4.strPlayTradingChangedSound, config.PlayTradingChanged.ToString());
            // Datei Trading geändert
            createConfigNode(LayThe4.strFileTradingChangedSound, config.FileTradingChanged); 
        }

        public LT4ConfigurationRW()
        {
            m_file = Application.StartupPath + @"\Plugins\LayThe4\Configuration.xml";
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
            double value = 0.0;
            // Automatisches Trading
            createConfigNode("AutomaticTrading", Boolean.FalseString);

            // Minimale Quote für automatisches Trading
            value = 1.5;
            createConfigNode("MinOdds", value.ToString());

            // Maximale Quote für automatisches Trading
            value = 4.5;
            createConfigNode("MaxOdds", value.ToString());

            // Prozent der Bank das eingesetzt werden soll
            value = 5.0;
            createConfigNode("PercentBank", value.ToString());

            // Minimaler Einsatz
            value = 6.0;
            createConfigNode("MinAmount", value.ToString());

            // Maximaler Einsatz
            value = 15.0;
            createConfigNode("MaxAmount", value.ToString());

            // Soll geringer Betrag gesetzt werden?
            createConfigNode("PlaceLower", Boolean.TrueString);

            // Volumen Markt
            createConfigNode("MarketVolume", "1000");

            // Strategie automatisch aktiviert?
            createConfigNode("StrategyActivated", Boolean.FalseString);

            // Startzeit für Ausstiegsüberwachung
            createConfigNode("StartPlaytime", "10");

            // Wartezeit, falls kein Gewinn
            createConfigNode("NoProfitWait", "1");

            // Erwarte Prozent an Profit
            value = 10.0;
            createConfigNode("ProfitPercent", value.ToString());

            // Wartezeit, falls erwartete Prozentsatz an Profit nicht vorhandne
            createConfigNode("ProfitWait", "1");

            // Anzahl der Tore für Notausstieg
            createConfigNode("Goals", "1");

            // Wartezeit Notausstieg
            createConfigNode("ExitSeconds", "120");

            // Verhalten bei keinen Gewinn
            createConfigNode("NoProfit", Boolean.TrueString);

            // Verhalten in Gewinnfall
            createConfigNode("Profit", Boolean.FalseString);

            // Aktives oder Passives Tradingverhalten
            createConfigNode("ActivePassive", Boolean.TrueString);

            //////////////////////////////////////////////////////////////////
            ////    SOUND
            /////////////////////////////////////////////////////////////////
            // Generell Töne Abspielen?
            createConfigNode(LayThe4.strPlaySounds, Boolean.FalseString);

            // Ton Spiel hinzugefügt
            createConfigNode(LayThe4.strPlayMatchAddedSound, Boolean.FalseString);
            // Datei Spiel hinzugefügt
            createConfigNode(LayThe4.strFileMatchAddedSound, String.Empty);

            // Ton Spiel beendet
            createConfigNode(LayThe4.strPlayGameEndedSound, Boolean.FalseString);
            // Datei Spiel beendet
            createConfigNode(LayThe4.strFileGameEndedSound, String.Empty);

            // Ton Spielstandsänderung
            createConfigNode(LayThe4.strPlayScoreChangedSound, Boolean.FalseString);
            // Datei Spiestandsänderung
            createConfigNode(LayThe4.strFileScoreChangedSound, String.Empty);

            // Ton Tradingänderung
            createConfigNode(LayThe4.strPlayTradingChangedSound, Boolean.FalseString);
            // Datei Tradingänderung
            createConfigNode(LayThe4.strFileTradingChangedSound, String.Empty);

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

        public Boolean PlaySounds
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strPlaySounds)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayThe4.strPlaySounds, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strPlaySounds)
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
                    if (node.Attributes["name"].Value == LayThe4.strPlayMatchAddedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayThe4.strPlayMatchAddedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strPlayMatchAddedSound)
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
                    if (node.Attributes["name"].Value == LayThe4.strFileMatchAddedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(LayThe4.strFileMatchAddedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strFileMatchAddedSound)
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
                    if (node.Attributes["name"].Value == LayThe4.strPlayGameEndedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayThe4.strPlayGameEndedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strPlayGameEndedSound)
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
                    if (node.Attributes["name"].Value == LayThe4.strFileGameEndedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(LayThe4.strFileGameEndedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strFileGameEndedSound)
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
                    if (node.Attributes["name"].Value == LayThe4.strPlayScoreChangedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayThe4.strPlayScoreChangedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strPlayScoreChangedSound)
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
                    if (node.Attributes["name"].Value == LayThe4.strFileScoreChangedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(LayThe4.strFileScoreChangedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strFileScoreChangedSound)
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
                    if (node.Attributes["name"].Value == LayThe4.strPlayTradingChangedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayThe4.strPlayTradingChangedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strPlayTradingChangedSound)
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
                    if (node.Attributes["name"].Value == LayThe4.strFileTradingChangedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(LayThe4.strFileTradingChangedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayThe4.strFileTradingChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean Profit
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "Profit")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }
                
                createConfigNode("Profit", Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "Profit")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean NoProfit
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "NoProfit")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }
                
                createConfigNode("NoProfit", Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "NoProfit")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int ExitSeconds
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ExitSeconds")
                        return Int32.Parse(node.Attributes["value"].Value);
                }
                
                createConfigNode("ExitSeconds", "120");
                return 120;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ExistSeconds")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int Goals
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "Goals")
                        return Int32.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("Goals", "1");
                return 1;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "Goals")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public void save()
        {
            m_doc.Save(m_file);
        }

        public int CheckServerBetSeconds
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CheckServerBetSeconds")
                        return Int32.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("CheckServerBetSeconds", "600");
                return 600;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "CheckServerBetSeconds")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int TradeStart
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "TradeStart")
                        return Int32.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("TradeStart", "90");
                return 90;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "TradeStart")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int ProfitWait
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ProfitWait")
                        return Int32.Parse(node.Attributes["value"].Value);
                }
                
                createConfigNode("ProfitWait", "1");
                return 1;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ProfitWait")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Double ProfitPercent
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ProfitPercent")
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 10.0;
                createConfigNode("ProfitPercent", value.ToString());
                return value;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ProfitPercent")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int NoProfitWait
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "NoProfitWait")
                        return Int32.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("NoProfitWait", "1");
                return 1;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "NoProfitWait")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int StartPlaytime
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StartPlaytime")
                        return Int32.Parse(node.Attributes["value"].Value);
                }
                
                createConfigNode("StartPlaytime", "10");
                return 10;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "StartPlaytime")
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

        public int MarketVolume
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MarketVolume")
                        return Int32.Parse(node.Attributes["value"].Value);
                }
                
                createConfigNode("MarketVolume", "1000");
                return 1000;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MarketVolume")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean PlaceLower
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "PlaceLower")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }
                
                createConfigNode("PlaceLower", Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "PlaceLower")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Double MaxAmount
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MaxAmount")
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 15.0;
                createConfigNode("MaxAmount", value.ToString());
                return value;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MaxAmount")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Double MinAmount
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MinAmount")
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 6.0;
                createConfigNode("MinAmount", value.ToString());
                return value;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MinAmount")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }


        public Double PercentBank
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "PercentBank")
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 5.0;
                createConfigNode("PercentBank", value.ToString());
                return value;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "PercentBank")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Double MaxOdds
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MaxOdds")
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 4.5;
                createConfigNode("MaxOdds", value.ToString());
                return value;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MaxOdds")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Double MinOdds
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MinOdds")
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 1.5;
                createConfigNode("MinOdds", value.ToString());
                return value;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "MinOdds")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean ActivePassive
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ActivePassive")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("ActivePassive", Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "ActivePassive")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean AutomaticTrading
        {
            get
            {

                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "AutomaticTrading")
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode("AutomaticTrading", Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.DocumentElement;
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == "AutomaticTrading")
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
    }
}
