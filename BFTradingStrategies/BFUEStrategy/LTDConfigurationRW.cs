using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Globalization;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    public class LTDConfigurationRW
    {
        private XmlDocument m_doc = null;
        private String m_file = String.Empty;

        private const int CONFIGVERSION = 2;

         public LTDConfigurationRW(LTDConfigurationRW config)
        {
            m_doc = new XmlDocument();
            m_doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><configuration></configuration>");


            // Intervall Abfrage Betfair auf Wetten
            createConfigNode(LayTheDraw.strCheckServerBetsSeconds, config.CheckServerBetsSeconds.ToString());

            // Flag Strategie standardmäßig aktiviert
            createConfigNode(LayTheDraw.strStrategyActivated, config.StrategyActivated.ToString());

            // Intervall Wartezeit unmatched Wetten
            createConfigNode(LayTheDraw.strUnmatchedWaitSeconds, config.UnmatchedWaitSeconds.ToString());

            // Intervall Wartezeit erneuter Versuch Tradeabschluss
            createConfigNode(LayTheDraw.strDontCloseRetrySeconds, config.DontCloseRetrySeconds.ToString());

            // Ticks maximale Abweichung für Wettabschluss wenn Stand ungleich
            // Unentschieden
            createConfigNode(LayTheDraw.strDontCloseOdds, config.DontCloseOdds.ToString());

            // Intervall Wartezeit in Sekund bis Abschluss Trade
            // nach Tor
            createConfigNode(LayTheDraw.strGoalScoredCloseTradeSeconds, config.GoalScoredCloseTradeSeconds.ToString());

            // Intervall Wartezeit für Quotenabfrage für Notausstieg
            createConfigNode(LayTheDraw.strCheckExitOddsSeconds, config.CheckExitOddsSeconds.ToString());

            // Notausstiegsquote
            createConfigNode(LayTheDraw.strExitCloseOdds, config.ExitCloseOdds.ToString());

            // Spielzeit, ab den der Notausstieg überprüft wird
            createConfigNode(LayTheDraw.strExitWatchActivationPlaytime, config.ExitWatchActivationPlaytime.ToString());         
   
             //Untere Quotengrenze, ab der kein Stopp/Loss mehr getätigt wird.
            createConfigNode(LayTheDraw.strNoStoppLossOdds, config.NoStoppLossOdds.ToString());

             //Absolute Untergrenze der Quote unter die ein regulärer Trade-Out im Falle eines Unentschieden nicht durchgerführt wird
            createConfigNode(LayTheDraw.strNoCloseTradeDrawOdds, config.DontCloseTradeDrawOdds.ToString());

            // Maximale Spielzeit bis zu der kein Ausstieg im Falle eines Unentschieden erfolgen soll
            createConfigNode(LayTheDraw.strDontCloseDrawPlaytime, config.DontCloseDrawPlaytime.ToString());
    
             // Überprüfe auf 0 - 0 Scoreline
            createConfigNode(LayTheDraw.strCheck00StoppLoss, config.Check00StoppLoss.ToString());

             // Prozentuale Wert in Relation zum Potentiellen Verlust auf den Draw Markt, den der 0 - 0 Scoreline
             // als Gewinn auf das 0 - 0 vorweisen muss, damit das Scoreline 0 - 0 beim Stopp/Loss Draw beachtung findet
            createConfigNode(LayTheDraw.str00WinPercentage, config.Win00Percentage.ToString());

             // Falls das Scoreline beachtet wird, wie soll sich der Stopp/Loss Draw verhalten? Kein Stopp/Loss (true) oder Stopp/Loss 
             // zu einen eingestellten Prozentwert (false)
            createConfigNode(LayTheDraw.str00NoStoppLoss, config.No00StoppLoss.ToString());

             // Falls ein Stopp/Loss zu einen relativen Prozentsatz gewünscht ist, ist dies der Prozentsatz der zieht
            createConfigNode(LayTheDraw.str00StopLossPercentage, config.StoppLoss00BetPercentage.ToString());

             //////////////////////////////////////////////
             // Sound
             //////////////////////////////////////////////
             // Sounds generell
             createConfigNode(LayTheDraw.strPlaySounds, config.PlaySounds.ToString());

             // Ton Spiel hinzugefügt
             createConfigNode(LayTheDraw.strPlayMatchAddedSound, config.PlayMatchAdded.ToString());
             // Datei Spiel hinzugefügt
             createConfigNode(LayTheDraw.strFileMatchAddedSound, config.FileMatchAdded);

             // Ton Spiel beendet
             createConfigNode(LayTheDraw.strPlayGameEndedSound, config.PlayGameEnded.ToString());
             // Datei Spiel beendet
             createConfigNode(LayTheDraw.strFileGameEndedSound, config.FileGameEnded);

             // Ton Spielstand geändert
             createConfigNode(LayTheDraw.strPlayScoreChangedSound, config.PlayScoreChanged.ToString());
             // Datei Spielstand geändert
             createConfigNode(LayTheDraw.strFileScoreChangedSound, config.FileScoreChanged);

             // Ton Trading geändert
             createConfigNode(LayTheDraw.strPlayTradingChangedSound, config.PlayTradingChanged.ToString());
             // Datei Trading geändert
             createConfigNode(LayTheDraw.strFileTradingChangedSound, config.FileTradingChanged);   
          

             //////////////////////////////////////////////////////////
             // Fast Lay 
             /////////////////////////////////////////////////////////
             // Wert des fest zu setztenden Betrages
             createConfigNode(LayTheDraw.strFastBetFixedAmountValue, config.FastBetFixedAmountValue.ToString());

             // Prozentuale Angabe des relativ zu setzenden Wertes
             createConfigNode(LayTheDraw.strFastBetPercentAmountValue, config.FastBetPercentAmountValue.ToString());

             // Art der prozentuale angabe (Gesamtkontostand od. verfügbarer Kontostand
             createConfigNode(LayTheDraw.strFastBetPercentTotalAmount, config.FastBetPercentTotalAmount.ToString());

             // Schalter, ob nicht erfüllte Wettern storniert werden sollen
             createConfigNode(LayTheDraw.strFastBetUnmatchedCancel, config.FastBetUnmatchedCancel.ToString());

             // Wartezeit in Sekunden, falls nicht erfüllte Wettern NICHT storniert werden sollen
             createConfigNode(LayTheDraw.strFastBetUnmatchedWaitSeconds, config.FastBetUnmatchedWaitSeconds.ToString());

             // Schalter, ob fester Betrag oder relativer Betrag zu setzen ist
             createConfigNode(LayTheDraw.strFastLayFixedAmount, config.FastBetFixedAmount.ToString());

             XmlNode rootNode = m_doc.FirstChild;
             if (rootNode.Name != "configuration")
                 rootNode = m_doc.ChildNodes[1];

             XmlAttribute attrVersion = rootNode.Attributes["configversion"];
             if (attrVersion == null)
             {
                 upgradeConfig();
             }
             else
             {
                 int configversion = 0;
                 try
                 {
                     Int32.TryParse(attrVersion.Value, out configversion);
                     if (configversion < CONFIGVERSION)
                         upgradeConfig();
                 }
                 catch (ArgumentException)
                 {
                     upgradeConfig();
                 }
             }


        }

         public LTDConfigurationRW(String xml, int version)
         {
             if (m_doc == null)
                 m_doc = new XmlDocument();
             m_doc.LoadXml(xml);

             if (version < CONFIGVERSION)
                 upgradeConfig();
         }

        public LTDConfigurationRW()
        {
            m_file = SXDirs.CfgPath + @"\LTDConfiguration.xml"; 
            m_doc = new XmlDocument();
            try
            {
                m_doc.Load(m_file);

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                XmlAttribute attrVersion = rootNode.Attributes["configversion"];
                if (attrVersion == null)
                {
                    upgradeConfig();
                }
                else
                {
                    int configversion = 0;
                    try
                    {
                        Int32.TryParse(attrVersion.Value, out configversion);
                        if (configversion < CONFIGVERSION)
                            upgradeConfig();
                    }
                    catch (ArgumentException)
                    {
                        upgradeConfig();
                    }
                }

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

        private void upgradeConfig()
        {
            this.FastBetFixedAmountValue = this.FastBetFixedAmountValue / SXALBankrollManager.Instance.MinStake;
        }

        private void createBasicConfiguration()
        {
            double value = 0.0;
            // Intervall Abfrage Betfair auf Wetten
            createConfigNode(LayTheDraw.strCheckServerBetsSeconds, "240");

            // Flag Strategie standardmäßig aktiviert
            createConfigNode(LayTheDraw.strStrategyActivated, Boolean.FalseString);

            // Intervall Wartezeit unmatched Wetten
            createConfigNode(LayTheDraw.strUnmatchedWaitSeconds, "180");

            // Intervall Wartezeit erneuter Versuch Tradeabschluss
            createConfigNode(LayTheDraw.strDontCloseRetrySeconds, "60");

            // Ticks maximale Abweichung für Wettabschluss wenn Stand ungleich
            // Unentschieden
            value = 0.5;
            createConfigNode(LayTheDraw.strDontCloseOdds, value.ToString());

            // Intervall Wartezeit in Sekund bis Abschluss Trade
            // nach Tor
            createConfigNode(LayTheDraw.strGoalScoredCloseTradeSeconds, "150");

            // Intervall Wartezeit für Quotenabfrage für Notausstieg
            createConfigNode(LayTheDraw.strCheckExitOddsSeconds, "30");

            // Notausstiegsquote
            value = 50;
            createConfigNode(LayTheDraw.strExitCloseOdds, value.ToString());

            //Untere Quotengrenze, ab der kein Stopp/Loss mehr getätigt wird.
            createConfigNode(LayTheDraw.strNoStoppLossOdds, "1.5");

            //Absolute Untergrenze der Quote unter die ein regulärer Trade-Out im Falle eines Unentschieden nicht durchgerführt wird
            createConfigNode(LayTheDraw.strNoCloseTradeDrawOdds,"2.0");

            // Spielzeit, ab den der Notausstieg überprüft wird
            createConfigNode(LayTheDraw.strExitWatchActivationPlaytime, "80");

            // Maximale Spielzeit bis zu der kein Ausstieg im Falle eines Unentschieden erfolgen soll
            createConfigNode(LayTheDraw.strDontCloseDrawPlaytime, "45" );

            // Überprüfe auf 0 - 0 Scoreline
            createConfigNode(LayTheDraw.strCheck00StoppLoss, Boolean.FalseString);

            // Prozentuale Wert in Relation zum Potentiellen Verlust auf den Draw Markt, den der 0 - 0 Scoreline
            // als Gewinn auf das 0 - 0 vorweisen muss, damit das Scoreline 0 - 0 beim Stopp/Loss Draw beachtung findet
            createConfigNode(LayTheDraw.str00WinPercentage, "50");

            // Falls das Scoreline beachtet wird, wie soll sich der Stopp/Loss Draw verhalten? Kein Stopp/Loss (true) oder Stopp/Loss 
            // zu einen eingestellten Prozentwert (false)
            createConfigNode(LayTheDraw.str00NoStoppLoss, Boolean.TrueString);

            // Falls ein Stopp/Loss zu einen relativen Prozentsatz gewünscht ist, ist dies der Prozentsatz der zieht
            createConfigNode(LayTheDraw.str00StopLossPercentage, "50");


            //////////////////////////////////////////////////////////////////
            ////    SOUND
            /////////////////////////////////////////////////////////////////
            // Generell Töne Abspielen?
            createConfigNode(LayTheDraw.strPlaySounds, Boolean.FalseString);

            // Ton Spiel hinzugefügt
            createConfigNode(LayTheDraw.strPlayMatchAddedSound, Boolean.FalseString);
            // Datei Spiel hinzugefügt
            createConfigNode(LayTheDraw.strFileMatchAddedSound, String.Empty);

            // Ton Spiel beendet
            createConfigNode(LayTheDraw.strPlayGameEndedSound, Boolean.FalseString);
            // Datei Spiel beendet
            createConfigNode(LayTheDraw.strFileGameEndedSound, String.Empty);

            // Ton Spielstandsänderung
            createConfigNode(LayTheDraw.strPlayScoreChangedSound, Boolean.FalseString);
            // Datei Spiestandsänderung
            createConfigNode(LayTheDraw.strFileScoreChangedSound, String.Empty);

            // Ton Tradingänderung
            createConfigNode(LayTheDraw.strPlayTradingChangedSound, Boolean.FalseString);
            // Datei Tradingänderung
            createConfigNode(LayTheDraw.strFileTradingChangedSound, String.Empty);


            //////////////////////////////////////////////////////////
            // Fast Lay 
            /////////////////////////////////////////////////////////
            // Wert des fest zu setztenden Betrages
            createConfigNode(LayTheDraw.strFastBetFixedAmountValue,  SXALBankrollManager.Instance.MinStake.ToString());

            // Prozentuale Angabe des relativ zu setzenden Wertes
            createConfigNode(LayTheDraw.strFastBetPercentAmountValue,"2");

            // Art der prozentuale angabe (Gesamtkontostand od. verfügbarer Kontostand
            createConfigNode(LayTheDraw.strFastBetPercentTotalAmount, Boolean.TrueString);

            // Schalter, ob nicht erfüllte Wettern storniert werden sollen
            createConfigNode(LayTheDraw.strFastBetUnmatchedCancel, Boolean.FalseString);

            // Wartezeit in Sekunden, falls nicht erfüllte Wettern NICHT storniert werden sollen
            createConfigNode(LayTheDraw.strFastBetUnmatchedWaitSeconds, "60");

            // Schalter, ob fester Betrag oder relativer Betrag zu setzen ist
            createConfigNode(LayTheDraw.strFastLayFixedAmount, Boolean.TrueString);

            m_doc.Save(m_file);
        }

        public void save()
        {
            XmlNode rootNode = m_doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = m_doc.ChildNodes[1];

            XmlAttribute attrVersion = rootNode.Attributes["configversion"];
            if (attrVersion == null)
            {
                attrVersion = m_doc.CreateAttribute("configversion");
                attrVersion.Value = CONFIGVERSION.ToString(CultureInfo.InvariantCulture);
                rootNode.Attributes.Append(attrVersion);
            }
            else
            {
                attrVersion.Value = CONFIGVERSION.ToString(CultureInfo.InvariantCulture);
            }

            m_doc.Save(m_file);
        }

        public String getXML()
        {
            return m_doc.InnerXml;
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
        #region Fast Lay
        public Boolean FastBetFixedAmount
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastLayFixedAmount)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strFastLayFixedAmount, Boolean.TrueString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastLayFixedAmount)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public double FastBetFixedAmountValue
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetFixedAmountValue) 
                    {
                        double d = 0.0;
                        try
                        {
                            if (Double.TryParse(node.Attributes["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                                return d * SXALBankrollManager.Instance.MinStake; ;
                        }
                        catch
                        {
                            return SXALBankrollManager.Instance.MinStake;
                        }
                    }
                }
                double value = 1.0;
                createConfigNode(LayTheDraw.strFastBetFixedAmountValue, value.ToString());
                return value;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetFixedAmountValue)
                        node.Attributes["value"].Value = (value / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public int FastBetPercentAmountValue
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetPercentAmountValue)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.strFastBetPercentAmountValue, "2");

                return 2;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetPercentAmountValue)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean FastBetPercentTotalAmount
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetPercentTotalAmount)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strFastBetPercentTotalAmount, Boolean.TrueString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetPercentTotalAmount)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean FastBetUnmatchedCancel
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetUnmatchedCancel)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strFastBetUnmatchedCancel, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetUnmatchedCancel)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int FastBetUnmatchedWaitSeconds
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetUnmatchedWaitSeconds)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.strFastBetUnmatchedWaitSeconds, "60");

                return 60;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetUnmatchedWaitSeconds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        #endregion

        #region Sounds
        public Boolean PlaySounds
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlaySounds)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strPlaySounds, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlaySounds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }


        public Boolean PlayMatchAdded
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlayMatchAddedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strPlayMatchAddedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlayMatchAddedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public String FileMatchAdded
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFileMatchAddedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(LayTheDraw.strFileMatchAddedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFileMatchAddedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean PlayGameEnded
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlayGameEndedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strPlayGameEndedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlayGameEndedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public String FileGameEnded
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFileGameEndedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(LayTheDraw.strFileGameEndedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFileGameEndedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean PlayScoreChanged
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlayScoreChangedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strPlayScoreChangedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlayScoreChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public String FileScoreChanged
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFileScoreChangedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(LayTheDraw.strFileScoreChangedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFileScoreChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean PlayTradingChanged
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlayTradingChangedSound)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strPlayTradingChangedSound, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strPlayTradingChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public String FileTradingChanged
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFileTradingChangedSound)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(LayTheDraw.strFileTradingChangedSound, String.Empty);
                return String.Empty;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFileTradingChangedSound)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
        #endregion

        #region Strategie
        public int DontCloseDrawPlaytime
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strDontCloseDrawPlaytime)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.strCheckServerBetsSeconds, "45");

                return 45;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strDontCloseDrawPlaytime)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int CheckServerBetsSeconds
        {
            get
            {
                    
                XmlNode rootNode = m_doc.FirstChild;
                if(rootNode.Name!="configuration")
                    rootNode = m_doc.ChildNodes[1];

                 foreach (XmlNode node in rootNode.ChildNodes)
                 {
                      if (node.Attributes["name"].Value == LayTheDraw.strCheckServerBetsSeconds)
                         return Int16.Parse(node.Attributes["value"].Value);
                 }
                 createConfigNode(LayTheDraw.strCheckServerBetsSeconds, "240");

                 return 240;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strCheckServerBetsSeconds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean StrategyActivated
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strStrategyActivated)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strStrategyActivated, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strStrategyActivated)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int UnmatchedWaitSeconds
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strUnmatchedWaitSeconds)
                        return Int16.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strUnmatchedWaitSeconds, "180");
                return 180;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strUnmatchedWaitSeconds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int DontCloseRetrySeconds
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strDontCloseRetrySeconds)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.strDontCloseRetrySeconds, "60");
                return 60;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strDontCloseRetrySeconds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public double DontCloseOdds
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strDontCloseOdds)
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 0.5;
                createConfigNode(LayTheDraw.strDontCloseOdds, value.ToString());
                return 0.5;               
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strDontCloseOdds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public double DontCloseTradeDrawOdds
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strNoCloseTradeDrawOdds)
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 2.0;
                createConfigNode(LayTheDraw.strNoCloseTradeDrawOdds, value.ToString());
                return 2.0;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strNoCloseTradeDrawOdds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public double NoStoppLossOdds
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strNoStoppLossOdds)
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = 1.5;
                createConfigNode(LayTheDraw.strNoStoppLossOdds, value.ToString());
                return 1.5;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strNoStoppLossOdds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int GoalScoredCloseTradeSeconds
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strGoalScoredCloseTradeSeconds)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.strGoalScoredCloseTradeSeconds, "150"); 
                return 30;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strGoalScoredCloseTradeSeconds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int CheckExitOddsSeconds
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strCheckExitOddsSeconds)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.strCheckExitOddsSeconds, "30");
                return 30;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strCheckExitOddsSeconds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public double ExitCloseOdds
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strExitCloseOdds)
                    {
                        return Double.Parse(node.Attributes["value"].Value);
                    }
                }
                double value = 50;
                createConfigNode(LayTheDraw.strExitCloseOdds, value.ToString() );
                return 50;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strExitCloseOdds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int ExitWatchActivationPlaytime
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strExitWatchActivationPlaytime)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.strExitWatchActivationPlaytime, "80");
                return 80;
            }
            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strExitWatchActivationPlaytime)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Soll vor eine Stopp/Loss überprüft werden, ob ein 0 - 0 Scoreline existiert
        /// </summary>
        public bool Check00StoppLoss
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strCheck00StoppLoss)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.strCheck00StoppLoss, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strCheck00StoppLoss)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Wie hoch in % des potentiellen Verlust des Draw Trades muss der 0 - 0 Scoreline Gewinn auf
        /// das 0 - 0 sein, damit das Scoreline beim Stopp/Loss beachtet wird?
        /// </summary>
        public int Win00Percentage
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.str00WinPercentage)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.str00WinPercentage, "50");
                return 50;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.str00WinPercentage)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Falls das Scoreline beachtet wird zeigt dieses Flag an:
        /// TRUE: Kein Stopp Loss
        /// FALSE: Ein Stopp/Loss mit dem eingestellten wert
        /// </summary>
        public bool No00StoppLoss
        {
            get
            {

                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.str00NoStoppLoss)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(LayTheDraw.str00NoStoppLoss, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.str00NoStoppLoss)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Prozentualler Wert den im Verhältnis zum Gesamtverlust der durch einen Stopp/Loss abgedeckt werden soll
        /// </summary>
        public int StoppLoss00BetPercentage
        {
            get
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.str00StopLossPercentage)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(LayTheDraw.str00StopLossPercentage, "50");
                return 50;
            }

            set
            {
                XmlNode rootNode = m_doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = m_doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.str00StopLossPercentage)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
        #endregion

    }
}
