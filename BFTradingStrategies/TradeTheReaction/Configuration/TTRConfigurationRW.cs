using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.plugin;
using System.Windows.Forms;
using System.Xml;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Xml.Linq;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Globalization;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.ttr.Configuration
{
    public enum TRADEOUTTRIGGER {GOAL, REDCARD, GENERAL, PLAYTIME}
    public enum SPECIALPLAYTIME { UNASSIGNED = -1, EARLIESTGOAL, AVERAGEGOAL, LATESTGOAL }

    public partial class TTRConfigurationRW : IConfiguration
    {
        internal XmlDocument _doc = null;       
        private String _file = String.Empty;

        private const String PREPLAYSTARTPOINT     = "genpreplaystartpoint";
        private const String NOTRADE               = "gennotrade";
        private const String CURRENCY              = "currency";

        #region Konstanten Scoreline
        private const String CHECKLAYODDS          = "sl00checklayodds";
        private const String ONLYHEDGE             = "sl00onlyhedge";
        private const String USEODDSPERCENTAGE     = "sl00useoddspercentage";
        private const String HEDGEPERCENTAGE       = "sl00hedgepercentage";
        private const String GREENPERCENTAGE       = "sl00greenpercentage";

        private const String USEWAITTIME           = "sl00usewaittime";

        private const String USEHEDGEWAITTIME      = "sl00usehedgewaittime"; // Neu mit Version 1.6.4
        private const String HEDGEWAITTIME         = "sl00hedgewaittime";
        private const String HEDGEPLAYTIME         = "sl00hedgeplaytime";  // Neu mit Version 1.6.4
        private const String USEHEDGESPECIALPT     = "sl00usehedgespecialplaytime"; // Neu mit Version 1.6.4
        private const String HEDGESPECIALPLAYTIME  = "sl00hedgespecialplaytime"; // Neu mit Version 1.6.4
        private const String HEDGESPECIALPTDELTA   = "sl00hedgespecialplaytimedelta"; // Neu im Version 1.6.4

        private const String USEGREENWAITTIME      = "sl00usegreenwaititme"; // Neu mit Version 1.6.4
        private const String GREENWAITTIME         = "sl00greenwaittime";
        private const String GREENPLAYTIME         = "sl00greenplaytime"; // Neu mit Version 1.6.4
        private const String USEGREENSPECIALPT     = "sl00usegreenspecialplaytime"; // Neu mit Version 1.6.4
        private const String GREENSPECIALPLAYTIME  = "sl00greenspecialplaytime"; // Neu mit Version 1.6.4
        private const String GREENSPECIALPTDELTA   = "sl00greenspecialplaytimedelta"; // Neu im Version 1.6.4
        #endregion
        private const String FIXEDFASTBETAMOUNT            = "fixedfastbetamount";
        private const String RELATIVEFASTBETAMOUNT         = "relativefastbetamount";
        private const String CALCULATEDFASTBETAMOUNT       = "calculatedfastbetamount";
        private const String RELATIVETRADETYPE             = "relativetradetype";       
        private const String RELATIVEBETTYPE               = "relativebettype";
        private const String RELATIVEBETSIZE               = "relativebetsize";
        private const String FIXEDFASTBETMONEY             = "fixedfastbetmoney";
        private const String PERCFASTBETMONEY              = "percentfastbetmoney";
        private const String TOTALMONEYFASTBET             = "totalmoneyfastbet";
        private const String CALCULATEDFIXEDMONEYFASTBET   = "calculatedfixedmoneyfastbet";
        private const String CALCULATEDFIXEDFASTBETMONEY   = "calculatedfixedfastbetmoney";
        private const String CALCULATEDPERCENTFASTBETMONEY = "calculatedpercentfastbetmoney";
        private const String USEBACKLAYTICKS               = "usebacklayticks";
        private const String BACKLAYTICKS                  = "backlayticks";
        private const String KEEPUNMATCHED                 = "keepunmatched";
        private const String KEEPINPLAY                    = "keepinplay";

        #region Konstanten Default Over/Under
        //////////////////////////////////////////////////////////////////////// 
        /// Default Config Keys für Over/Under
        ////////////////////////////////////////////////////////////////////////
        private const String OUDFLTONLYHEDGE       = "oudefaultonlyhedge";
        private const String OUDFLTCHECKLAYODDS    = "oudefaultchecklayodds";
        private const String OUDFLTUSEWAITTIME     = "oudefaultusewaittime";
        private const String OUDFLTUSEODDSPERC     = "oudefaultuseoddspercentage";
        /// <summary>
        /// Gibt an, ob eine absolute Zeitdauer, z.B. 10 Minute gewartet werden soll
        /// oder bis zu einer definierten Spielzeit z.B. 60 Spielminute
        /// </summary>
        private const String OUDFLTUSEHEDGEMINUTES = "oudefaultusehedgeminutes";
        private const String OUDFLTHEDGEWAITTIME   = "oudefaulthedgewaittime";
        private const String OUDFLTHEDGEPLAYTIME   = "oudefaulthedgeplaytime";
        /// <summary>
        /// Gibt an, ob eine absolute Zeitdauer, z.B. 10 Minute gewartet werden soll
        /// oder bis zu einer definierten Spielzeit z.B. 60 Spielminute
        /// </summary>
        private const String OUDFLTUSEGREENMINUTES = "ouddefaultusegreenminutes";
        private const String OUDFLTGREENWAITTIME   = "oudefaultgreenwaittime";
        private const String OUDFLTGREENPLAYTIME   = "oudefaultgreenplaytime";
        private const String OUDFLTHEDGEPERCENTAGE = "oudefaulthedgepercentage";
        private const String OUDFLTGREENPERCENTAGE = "oudefaultgreenpercentage";
        #endregion

        #region Konstanten Correct Score
        #region Back Correct Score Trade
        private const String CSBCHECKLAYODDS         = "csbchecklayodds";
        private const String CSBONLYHEDGE            = "csbonlyhedge";
        private const String CSBUSEODDSPERCENTAGE    = "csbuseoddspercentage";
        private const String CSBHEDGEPERCENTAGE      = "csbhedgepercentage";
        private const String CSBGREENPERCENTAGE      = "csbgreenpercentage";

        private const String CSBUSEWAITTIME          = "csbusewaittime";

        private const String CSBUSEHEDGEWAITTIME     = "csbusehedgewaittime"; 
        private const String CSBHEDGEWAITTIME        = "csbhedgewaittime";
        private const String CSBHEDGEPLAYTIME        = "csbhedgeplaytime"; 
        private const String CSBUSEHEDGESPECIALPT    = "csbusehedgespecialplaytime";
        private const String CSBHEDGESPECIALPLAYTIME = "csbhedgespecialplaytime"; 
        private const String CSBHEDGESPECIALPTDELTA  = "csbhedgespecialplaytimedelta";

        private const String CSBUSEGREENWAITTIME     = "csbusegreenwaititme";
        private const String CSBGREENWAITTIME        = "csbgreenwaittime";
        private const String CSBGREENPLAYTIME        = "csbgreenplaytime";
        private const String CSBUSEGREENSPECIALPT    = "csbusegreenspecialplaytime";
        private const String CSBGREENSPECIALPLAYTIME = "csbgreenspecialplaytime";
        private const String CSBGREENSPECIALPTDELTA  = "csbgreenspecialplaytimedelta";
        #endregion

        #region Lay Correct Score Trade
        #endregion
        #endregion

        private const int CONFIGVERSION = 2;
        private TTRTradeOutCheckSortedList _tradeOutCheckList = new TTRTradeOutCheckSortedList();

        public TTRConfigurationRW(string xml, int version)
        {
            if (_doc == null)
                _doc = new XmlDocument();

            _doc.LoadXml(xml);

            if (version < CONFIGVERSION)
                upgradeConfig();
        }

        public TTRConfigurationRW()
        {
            _file = SXDirs.CfgPath + @"\TTRConfiguration.xml";
            _doc = new XmlDocument();
            try
            {
                _doc.Load(_file);

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

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
                XmlTextWriter writer = new XmlTextWriter(_file, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartElement("configuration");
                writer.Close();
                _doc.Load(_file);
                createBasicConfiguration();
            }
        }

        private void upgradeConfig()
        {
            this.FastBetFixedAmountValue = this.FastBetFixedAmountValue / SXALBankrollManager.Instance.MinStake;
        }

        private void createBasicConfiguration()
        {
            #region General
            createConfigNode(PREPLAYSTARTPOINT, "60");
            createConfigNode(NOTRADE, Boolean.FalseString);
            createConfigNode(CURRENCY, SXALBankrollManager.Instance.Currency);
            #endregion
            #region Scoreline 0-0           
            createConfigNode(HEDGEWAITTIME, "600");
            createConfigNode(GREENWAITTIME, "600");
            createConfigNode(CHECKLAYODDS, Boolean.TrueString);
            createConfigNode(ONLYHEDGE, Boolean.TrueString);
            createConfigNode(USEWAITTIME, Boolean.TrueString);

            createConfigNode(USEHEDGEWAITTIME, Boolean.TrueString);
            createConfigNode(HEDGEPLAYTIME, "10");
            createConfigNode(USEHEDGESPECIALPT, Boolean.FalseString);
            createConfigNode(HEDGESPECIALPLAYTIME, SPECIALPLAYTIME.UNASSIGNED.ToString());
            createConfigNode(HEDGESPECIALPTDELTA, "0");

            createConfigNode(USEGREENWAITTIME, Boolean.TrueString);
            createConfigNode(GREENPLAYTIME, "10");
            createConfigNode(USEGREENSPECIALPT, Boolean.FalseString);
            createConfigNode(GREENSPECIALPLAYTIME, SPECIALPLAYTIME.UNASSIGNED.ToString());
            createConfigNode(GREENSPECIALPTDELTA, "0");

            createConfigNode(USEODDSPERCENTAGE, Boolean.FalseString);
            createConfigNode(HEDGEPERCENTAGE, "70");
            createConfigNode(GREENPERCENTAGE, "50");
            #endregion
            #region OverUnder
            createConfigNode(OUDFLTHEDGEWAITTIME, "120");
            createConfigNode(OUDFLTGREENWAITTIME, "120");
            createConfigNode(OUDFLTCHECKLAYODDS, Boolean.TrueString);
            createConfigNode(OUDFLTONLYHEDGE, Boolean.TrueString);
            createConfigNode(OUDFLTUSEWAITTIME, Boolean.TrueString);
            createConfigNode(OUDFLTUSEODDSPERC, Boolean.FalseString);
            createConfigNode(OUDFLTHEDGEPERCENTAGE, "70");
            createConfigNode(OUDFLTGREENPERCENTAGE, "50");
            createConfigNode(OUDFLTUSEHEDGEMINUTES, Boolean.TrueString);
            createConfigNode(OUDFLTHEDGEWAITTIME, "10");
            createConfigNode(OUDFLTHEDGEPLAYTIME, "45");
            createConfigNode(OUDFLTUSEGREENMINUTES, Boolean.TrueString);
            createConfigNode(OUDFLTGREENWAITTIME, "10");
            createConfigNode(OUDFLTGREENPLAYTIME, "60");
            #endregion
            #region Correct Score
            #region Back Mode
            createConfigNode(CSBHEDGEWAITTIME, "600");
            createConfigNode(CSBGREENWAITTIME, "600");
            createConfigNode(CSBCHECKLAYODDS, Boolean.TrueString);
            createConfigNode(CSBONLYHEDGE, Boolean.TrueString);
            createConfigNode(CSBUSEWAITTIME, Boolean.TrueString);

            createConfigNode(CSBUSEHEDGEWAITTIME, Boolean.TrueString);
            createConfigNode(CSBHEDGEPLAYTIME, "10");
            createConfigNode(CSBUSEHEDGESPECIALPT, Boolean.FalseString);
            createConfigNode(CSBHEDGESPECIALPLAYTIME, SPECIALPLAYTIME.UNASSIGNED.ToString());
            createConfigNode(CSBHEDGESPECIALPTDELTA, "0");

            createConfigNode(CSBUSEGREENWAITTIME, Boolean.TrueString);
            createConfigNode(CSBGREENPLAYTIME, "10");
            createConfigNode(CSBUSEGREENSPECIALPT, Boolean.FalseString);
            createConfigNode(CSBGREENSPECIALPLAYTIME, SPECIALPLAYTIME.UNASSIGNED.ToString());
            createConfigNode(CSBGREENSPECIALPTDELTA, "0");

            createConfigNode(CSBUSEODDSPERCENTAGE, Boolean.FalseString);
            createConfigNode(CSBHEDGEPERCENTAGE, "70");
            createConfigNode(CSBGREENPERCENTAGE, "50");           
            #endregion
            #endregion
            #region Fast Bet
            //////////////////////////////////////////////////////////
            // Fast Bet
            /////////////////////////////////////////////////////////
            // Wert ist relativ zu einen anderen Betrag eines Trades
            createConfigNode(RELATIVEFASTBETAMOUNT, Boolean.FalseString);

            // Art des relativen Trades
            createConfigNode(RELATIVETRADETYPE, TRADETYPE.UNASSIGNED.ToString());            

            // Art der Einsatzberechnung bei eine relativen Trade
            createConfigNode(RELATIVEBETTYPE, RELATIVEBETTINGTYPE.UNASSIGNED.ToString());

            // Höhe des relativen Einsatzes in %
            createConfigNode(RELATIVEBETSIZE, "0");

            // Wert des fest zu setztenden Betrages
            createConfigNode(FIXEDFASTBETMONEY, "1" /*Defaultsetzbetrag ist immer ein Mindestbetrag*/);

            // Prozentuale Angabe des relativ zu setzenden Wertes
            createConfigNode(PERCFASTBETMONEY, "2");

            // Art der prozentuale angabe (Gesamtkontostand od. verfügbarer Kontostand
            createConfigNode(TOTALMONEYFASTBET, Boolean.TrueString);
            /*
            // Schalter, ob nicht erfüllte Wettern storniert werden sollen
            createConfigNode(LayTheDraw.strFastBetUnmatchedCancel, Boolean.FalseString);

            // Wartezeit in Sekunden, falls nicht erfüllte Wettern NICHT storniert werden sollen
            createConfigNode(LayTheDraw.strFastBetUnmatchedWaitSeconds, "60");
            */
            // Schalter, ob fester Betrag oder relativer Betrag zu setzen ist             
            createConfigNode(FIXEDFASTBETAMOUNT, Boolean.TrueString);

            // Schalter, ob Einsatzhöhe relative zum angegebenen Risiko/Gewinn berechnet werden soll
            createConfigNode(CALCULATEDFASTBETAMOUNT, Boolean.FalseString);

            // Schalter, ob Risiko/Gewinn fix oder Anteil des Gesamtkontostands ist.
            createConfigNode(CALCULATEDFIXEDMONEYFASTBET, Boolean.TrueString);

            // Fixer Zielbetrag anhand dem die Einsatzhöhe berechnet werden soll
            createConfigNode(CALCULATEDFIXEDFASTBETMONEY, SXALBankrollManager.Instance.MinStake.ToString(CultureInfo.InvariantCulture));

            // Relativer Zielbetrag anhand dem die Einsatzhöhe berechnet werden soll
            createConfigNode(CALCULATEDPERCENTFASTBETMONEY, "2");

            // Soll nicht die aktuelle beste Lay/Back Quote genommen werden, sondern eine, die sich von in Ticks davon unterscheidet?
            createConfigNode(USEBACKLAYTICKS, Boolean.FalseString);

            //Anzahl der Ticks in der sich die zu setztende Quote unterscheidet
            createConfigNode(BACKLAYTICKS, "1");

            // Offene Wetten werden nicht storniert vom Starter
            createConfigNode(KEEPUNMATCHED, Boolean.FalseString);

            // Offene Wetten bleiben bei Begegnungsstart erhalten
            createConfigNode(KEEPINPLAY, Boolean.FalseString);
            #endregion
        }

        private void createConfigNode(String name, String value)
        {
            XmlNode rootNode = _doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = _doc.ChildNodes[1];
            XmlElement configurationElement = _doc.CreateElement("configitem");
            XmlAttribute attributeName = _doc.CreateAttribute("name");
            attributeName.Value = name;
            XmlAttribute attributeValue = _doc.CreateAttribute("value");
            attributeValue.Value = value;
            configurationElement.Attributes.Append(attributeName);
            configurationElement.Attributes.Append(attributeValue);
            rootNode.AppendChild(configurationElement);
        }

        private void createConfigNode2(String nodeName, String name, String value)
        {
            XmlNode rootNode = _doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = _doc.ChildNodes[1];
            XmlElement nodeElment = rootNode[nodeName];
            if (nodeElment == null)
            {
                nodeElment = _doc.CreateElement(nodeName);
                rootNode.AppendChild(nodeElment);
            }

            XmlElement configurationElement = _doc.CreateElement("configitem");
            XmlAttribute attributeName = _doc.CreateAttribute("name");
            attributeName.Value = name;
            XmlAttribute attributeValue = _doc.CreateAttribute("value");
            attributeValue.Value = value;
            configurationElement.Attributes.Append(attributeName);
            configurationElement.Attributes.Append(attributeValue);
            //rootNode.AppendChild(configurationElement);
            nodeElment.AppendChild(configurationElement);
        }

        private void createConfigNode3(String upperNodeName, String subNodeName, String name, String value)
        {
            XmlNode rootNode = _doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = _doc.ChildNodes[1];
            XmlElement nodeUpperElment = rootNode[upperNodeName];
            if (nodeUpperElment == null)
            {
                nodeUpperElment = _doc.CreateElement(upperNodeName);
                rootNode.AppendChild(nodeUpperElment);
            }

            XmlElement nodeSubElement = nodeUpperElment[subNodeName];
            if (nodeSubElement == null)
            {
                nodeSubElement = _doc.CreateElement(subNodeName);
                nodeUpperElment.AppendChild(nodeSubElement);
            }

            XmlElement configurationElement = _doc.CreateElement("configitem");
            XmlAttribute attributeName = _doc.CreateAttribute("name");
            attributeName.Value = name;
            XmlAttribute attributeValue = _doc.CreateAttribute("value");
            attributeValue.Value = value;
            configurationElement.Attributes.Append(attributeName);
            configurationElement.Attributes.Append(attributeValue);
            //rootNode.AppendChild(configurationElement);
            nodeSubElement.AppendChild(configurationElement);
        }

        /// <summary>
        /// Flag, ob getradet werden soll oder nicht. Falls Wert 'TRUE', dann wird kein
        /// hedging, greening oder sonstwie gearteter Trade durchgeführt.
        /// </summary>
        public Boolean NoTrade
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NOTRADE) 
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(NOTRADE, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NOTRADE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public String Currency
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CURRENCY)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(CURRENCY, SXALBankrollManager.Instance.Currency);
                return SXALBankrollManager.Instance.Currency;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CURRENCY)
                        node.Attributes["value"].Value = SXALBankrollManager.Instance.Currency;
;
                }
            }
        }

        /// <summary>
        /// Einstellbare Minutenanzahl an der die Regelüberwachung vor Spielstart 
        /// begonnen werden soll
        /// </summary>
        public int PreplayStartPoint
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == PREPLAYSTARTPOINT)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(PREPLAYSTARTPOINT, "60");

                return 60;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == PREPLAYSTARTPOINT)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        #region Dynamic Trade Out checks
        internal TTRTradeOutCheckSortedList TradeOutRules
        {
            get { return _tradeOutCheckList; }
            set { _tradeOutCheckList = value; }
        }

        #endregion

      


       

        #region ScoreLine 0 - 0
        /// <summary>
        /// Soll die Quotenprozentüberwachung benutzt werden?
        /// </summary>
        public Boolean UseOddsPercentage
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEODDSPERCENTAGE)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USEODDSPERCENTAGE, Boolean.FalseString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEODDSPERCENTAGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Prozentuale Abweichung von der Ursprungs-Back-Quote nach unten, die erfüllt sein muss, bovor
        /// gehedged wird
        /// </summary>
        public int HedgePercentage
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGEPERCENTAGE)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(HEDGEPERCENTAGE, "70");

                return 70;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGEPERCENTAGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Prozentuale Abweichung von der Ursprungs-Back-Quote nach unten, die erfüllt sein muss, bovor
        /// gegreened wird
        /// </summary>
        public int GreenPercentage
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENPERCENTAGE)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(GREENPERCENTAGE, "50");

                return 50;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENPERCENTAGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Das Hedging wird mittels Countdown gesteuert
        /// </summary>
        public Boolean UseHedgeWaitTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEHEDGEWAITTIME)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USEHEDGEWAITTIME, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEHEDGEWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Spielminute, welche erreicht werden muss bevor versucht wird den Trade zu
        /// hedgen, d.h. Gegenwetten so zu setzen, dass kein Verlustrisiko mehr besteht.
        /// </summary>
        public int HedgePlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGEPLAYTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(HEDGEPLAYTIME, "10");

                return 10;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGEPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Das Hedging wird mittels einen speziellen statistischen Wert gesteuert.
        /// </summary>
        public Boolean UseHedgeSpecialPlayTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEHEDGESPECIALPT)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USEHEDGESPECIALPT, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEHEDGESPECIALPT)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Statistischer Wert, aus dem die Spielminute gelesen wird bevor versucht wird den Trade zu
        /// hedgen, d.h. Gegenwetten so zu setzen, dass kein Verlustrisiko mehr besteht.
        /// </summary>
        public SPECIALPLAYTIME HedgeSpecialPlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGESPECIALPLAYTIME)
                        return (SPECIALPLAYTIME) Enum.Parse(typeof(SPECIALPLAYTIME), node.Attributes["value"].Value);
                }
                createConfigNode(HEDGESPECIALPLAYTIME, SPECIALPLAYTIME.UNASSIGNED.ToString());

                return SPECIALPLAYTIME.UNASSIGNED;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGESPECIALPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Delta-Spielminute zur Special Hedging-time
        /// </summary>
        public int HedgeSpecialPlayimeDelta
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGESPECIALPTDELTA)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(HEDGESPECIALPTDELTA, "0");

                return 0;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGESPECIALPTDELTA)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }


        /// <summary>
        /// Soll die Zeitüberwachung benutzt werden?
        /// </summary>
        public Boolean UseWaitTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEWAITTIME)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USEWAITTIME, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Wartezeit in Sekunden, welche vergehen muss bevor versucht wird den Trade zu
        /// hedgen, d.h. Gegenwetten so zu setzen, dass kein Verlustrisiko mehr besteht.
        /// </summary>
        public int HedgeWaitTime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGEWAITTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(HEDGEWAITTIME, "600");

                return 600;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HEDGEWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Das Greening wird mittels Countdown gesteuert
        /// </summary>
        public Boolean UseGreenWaitTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEGREENWAITTIME)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USEGREENWAITTIME, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEGREENWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Spielminute, welche erreicht werden muss bevor versucht wird den Trade zu
        /// greenen, d.h. Gegenwetten so zu setzen, unabhängig vom Ausgang der gleiche 
        /// Gewinn/Verlust entsteht.
        /// </summary>
        public int GreenPlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENPLAYTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(GREENPLAYTIME, "20");

                return 20;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Das Greening wird mittels einen speziellen statistischen Wert gesteuert.
        /// </summary>
        public Boolean UseGreenSpecialPlayTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEGREENSPECIALPT)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USEGREENSPECIALPT, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEGREENSPECIALPT)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Statistischer Wert, aus dem die Spielminute gelesen wird bevor versucht wird den Trade zu
        /// greenen, d.h. Gegenwetten so zu setzen, dass, unabhängig vom Endergebnisse, der gleiche 
        /// Gewinn/Verlust entsteht.
        /// </summary>
        public SPECIALPLAYTIME GreenSpecialPlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENSPECIALPLAYTIME)
                        return (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), node.Attributes["value"].Value);
                }
                createConfigNode(GREENSPECIALPLAYTIME, SPECIALPLAYTIME.UNASSIGNED.ToString());

                return SPECIALPLAYTIME.UNASSIGNED;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENSPECIALPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Delta-Spielminute zur Special Greening-Spielminute
        /// </summary>
        public int GreenSpecialPlayimeDelta
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENSPECIALPTDELTA)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(GREENSPECIALPTDELTA, "0");

                return 0;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENSPECIALPTDELTA)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Wartezeit in Sekunden, welche vergehen muss bevor versucht wird, den Trade 
        /// zu greenen, d.h. durch geschicktes platzieren von Gegenwetten eine Situation zu schaffen
        /// in der unabhängig von Ausgang des Ereignis ein konstanter Gewinn gefahren wird.
        /// </summary>
        public int GreenWaitTime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENWAITTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(GREENWAITTIME, "600");

                return 600;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == GREENWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Sollen die Lay Quote überprüft werden, ob sie geringer als die Back Quote ist?
        /// </summary>
        public Boolean CheckLayOdds
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CHECKLAYODDS)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CHECKLAYODDS, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CHECKLAYODDS)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }


        /// <summary>
        /// Dieses Flag gibt an, ob nur gehedged, d. h. eine Lay Wette wird so platziert, das kein Verlust mehr entstehen kann,
        /// oder anschliessend auch gegreened, d.h. eine weiter Lay Wette wird so platziert, das der Gewinn in beiden 
        /// Fällen ungefähr gleich ist, werden soll
        /// </summary>
        public Boolean OnlyHedge
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == ONLYHEDGE)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(ONLYHEDGE, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == ONLYHEDGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
        #endregion

        #region Fast Lay
        public int RelativeBetSize
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == RELATIVEBETSIZE)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(RELATIVEBETSIZE, "0");

                return 0;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == RELATIVEBETSIZE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public RELATIVEBETTINGTYPE RelativeBetType
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == RELATIVEBETTYPE)
                        return (RELATIVEBETTINGTYPE)Enum.Parse(typeof(RELATIVEBETTINGTYPE), node.Attributes["value"].Value);//Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(RELATIVEBETTYPE, RELATIVEBETTINGTYPE.UNASSIGNED.ToString());
                return RELATIVEBETTINGTYPE.UNASSIGNED;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == RELATIVEBETTYPE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public TRADETYPE RelativeTradeType
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == RELATIVETRADETYPE)
                        return (TRADETYPE)Enum.Parse(typeof(TRADETYPE), node.Attributes["value"].Value);//Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(RELATIVETRADETYPE, TRADETYPE.UNASSIGNED.ToString());
                return TRADETYPE.UNASSIGNED;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == RELATIVETRADETYPE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean RelativeBetAmount
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == RELATIVEFASTBETAMOUNT)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(RELATIVEFASTBETAMOUNT, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == RELATIVEFASTBETAMOUNT)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean FastBetFixedAmount
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == FIXEDFASTBETAMOUNT)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(FIXEDFASTBETAMOUNT, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == FIXEDFASTBETAMOUNT)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public Boolean FastBetCalculatedAmount
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CALCULATEDFASTBETAMOUNT)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CALCULATEDFASTBETAMOUNT, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CALCULATEDFASTBETAMOUNT)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public double FastBetTargetFixedAmountValue
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CALCULATEDFIXEDFASTBETMONEY)
                    {
                        double d = 0.0;
                        if (Double.TryParse(node.Attributes["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                            return d * SXALBankrollManager.Instance.MinStake; ;
                    }
                }
                double value = 1.0;
                createConfigNode(CALCULATEDFIXEDFASTBETMONEY, value.ToString(CultureInfo.InvariantCulture));
                return value;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CALCULATEDFIXEDFASTBETMONEY)
                        node.Attributes["value"].Value = (value / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public double FastBetTargetPercentAmountValue
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CALCULATEDPERCENTFASTBETMONEY)
                    {
                        double d = 0.0;
                        if (Double.TryParse(node.Attributes["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                            return d ;
                    }
                }
                double value = 2.0;
                createConfigNode(CALCULATEDPERCENTFASTBETMONEY, value.ToString(CultureInfo.InvariantCulture));
                return value;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CALCULATEDPERCENTFASTBETMONEY)
                        node.Attributes["value"].Value = value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public double FastBetFixedAmountValue
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == FIXEDFASTBETMONEY)
                    {
                        double d = 0.0;
                        if(Double.TryParse(node.Attributes["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                            return d * SXALBankrollManager.Instance.MinStake; ;
                    }
                }
                double value = 1.0;
                createConfigNode(FIXEDFASTBETMONEY, value.ToString(CultureInfo.InvariantCulture));
                return value;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == FIXEDFASTBETMONEY)
                        node.Attributes["value"].Value = (value / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public int FastBetPercentAmountValue
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == PERCFASTBETMONEY)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(PERCFASTBETMONEY, "2");

                return 2;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value ==PERCFASTBETMONEY)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Flag gibt an, ob für die Berechnung des Einsatzes der Totale Geldbetrag (true) oder 
        /// der verfügbare Geldbetrag (true) auf dem Wettkonto verwendet werden soll
        /// </summary>
        public Boolean FastBetPercentTotalAmount
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == TOTALMONEYFASTBET)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(TOTALMONEYFASTBET, Boolean.TrueString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == TOTALMONEYFASTBET)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }


        /// <summary>
        /// Flag gibt an, ob das Risiko/Gewinn anhand derer sich die Einsatzhöhe berechnen soll
        /// ein fixer Wert ist oder sich relativ zum Kontostand bezieht.
        /// </summary>
        public Boolean FastBetTargetFixedAmount
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CALCULATEDFIXEDMONEYFASTBET)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CALCULATEDFIXEDMONEYFASTBET, Boolean.TrueString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CALCULATEDFIXEDMONEYFASTBET)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Anzahl der Ticks um die sich die Quote in der zu setztenden Wette von den aktuell gelesenen Quote untescheiden soll
        /// Ein Tick wird an entsprechender Stelle in die absolute Quotenschrittweite umgerechnet
        /// </summary>
        public int BackLayTicks
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BACKLAYTICKS)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(BACKLAYTICKS, "1");

                return 1;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BACKLAYTICKS)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// In der Regel werden offene Wetten von Betfair storniert, wenn eine Begegnung InPlay geht
        /// Ist dieser Schalter gesetzt, so wird Betfair mitgeteilt, dass die Wette offen bleiben soll
        /// </summary>
        public Boolean KeepInplay
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == KEEPINPLAY)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(KEEPINPLAY, Boolean.FalseString);
                return false;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == KEEPINPLAY)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// In der Regel wird vom Tradestarter nicht erfüllte Wetten gleich wieder storniert.
        /// Falls dieser Schalter gesetzt wurde, so bleiben diese Wetten offen und es wird ein
        /// Trade daraus konstruiert.
        /// </summary>
        public Boolean KeepUnmatched
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == KEEPUNMATCHED)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(KEEPUNMATCHED, Boolean.FalseString);
                return false;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == KEEPUNMATCHED)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }


        /// <summary>
        /// Gibt an, ob für die Quoten anhand von Ticks vor dem setzen errechnet werden, oder direkt die angegebenen Lay
        /// und Back Quoten errechnet werden
        /// Im Falle einer Lay Wette ergibt sich: Lay-Quote = Back-Quote + Ticks
        /// Im Falle einer Back Wette ergibt sich: Back-Quote = Lay-Quote - Ticks
        /// </summary>
        public Boolean UseBackLayTicks
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value ==USEBACKLAYTICKS)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USEBACKLAYTICKS, Boolean.FalseString);
                return false;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEBACKLAYTICKS)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /*
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
*/
        #endregion

        #region IConfiguration Member

        public void save()
        {
            XmlNode rootNode = _doc.FirstChild;
            if (rootNode.Name != "configuration")
                rootNode = _doc.ChildNodes[1];

            XmlAttribute attrVersion = rootNode.Attributes["configversion"];
            if (attrVersion == null)
            {
                attrVersion = _doc.CreateAttribute("configversion");
                attrVersion.Value = CONFIGVERSION.ToString(CultureInfo.InvariantCulture);
                rootNode.Attributes.Append(attrVersion);
            }
            else
            {
                attrVersion.Value = CONFIGVERSION.ToString(CultureInfo.InvariantCulture);
            }

            _doc.Save(_file);
        }
        public string getXML()
        {
            return _doc.InnerXml;
        }

        #endregion
    }

    public abstract class TTRTradeOutRuleFragment
    {
        public abstract bool check(IScore score);

        public abstract String toXml();
        
        public abstract void fromXml(string xml);

        public static TTRTradeOutRuleFragment createFromXml(string xml)
        {
            TTRTradeOutRuleFragment fragment = null;
            try
            {
                XElement element = XElement.Parse(xml);
                if (element != null)
                {
                    if (element.Attribute("checktype") != null)
                    {
                        Type ruleType = Type.GetType(element.Attribute("checktype").Value);
                        if (typeof(TTRTradeOutGoalSumRule) == ruleType)
                        {
                            fragment = new TTRTradeOutGoalSumRule();
                        }
                        else if (typeof(TTRTradeOutPlayTimeRule) == ruleType)
                        {
                            fragment = new TTRTradeOutPlayTimeRule();
                        }
                        else if (typeof(TTRTradeOutRCEqual) == ruleType)
                        {
                            fragment = new TTRTradeOutRCEqual();
                        }
                        else if (typeof(TTRTradeOutRCTeamAMore) == ruleType)
                        {
                            fragment = new TTRTradeOutRCTeamAMore();
                        }
                        else if (typeof(TTRTradeOutRCTeamBMore) == ruleType)
                        {
                            fragment = new TTRTradeOutRCTeamBMore();
                        }
                        else if (typeof(TTRTradeOutScoreRule) == ruleType)
                        {
                            fragment = new TTRTradeOutScoreRule();
                        }

                        if (fragment != null)
                        {
                            fragment.fromXml(xml);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteException(e);
            }
            return fragment;
        }

    }

    public class TTRTradeOutScoreRule : TTRTradeOutRuleFragment
    {
        public ScoreList Scores { get; set; }

        public override bool check(IScore score)
        {
            if (score == null)
                return false;
            foreach (TTRScores scores in Scores)
            {
                if (score.getScore().Equals(ScoreList.getScoreString(scores)))
                    return true;

                //
                if (scores == TTRScores.OTHERS && (score.ScoreA > 3 || score.ScoreB > 3))
                    return true;
            }

            return false;

        }


        public override string ToString()
        {
            String strScores = String.Empty;
            foreach (TTRScores score in Scores)
            {
                if (strScores.Length > 0)
                {
                    strScores += ", ";
                }
                switch (score)
                {
                    case TTRScores.ZEROZERO:
                        strScores += "[0 - 0]";
                        break;
                    case TTRScores.ZEROONE:
                        strScores += "[0 - 1]";
                        break;
                    case TTRScores.ZEROTWO:
                        strScores += "[0 - 2]";
                        break;
                    case TTRScores.ZEROTHREE:
                        strScores += "[0 - 3]";
                        break;
                    case TTRScores.ONEZERO:
                        strScores += "[1 - 0]";
                        break;
                    case TTRScores.ONEONE:
                        strScores += "[1 - 1]";
                        break;
                    case TTRScores.ONETWO:
                        strScores += "[1 - 2]";
                        break;
                    case TTRScores.ONETHREE:
                        strScores += "[1 - 3]";
                        break;
                    case TTRScores.TWOZERO:
                        strScores += "[2 - 0]";
                        break;
                    case TTRScores.TWOONE:
                        strScores += "[2 - 1]";
                        break;
                    case TTRScores.TWOTWO:
                        strScores += "[2 - 2]";
                        break;
                    case TTRScores.TWOTHREE:
                        strScores += "[2 - 3]";
                        break;
                    case TTRScores.THREEZERO:
                        strScores += "[3 - 0]";
                        break;
                    case TTRScores.THREEONE:
                        strScores += "[3 - 1]";
                        break;
                    case TTRScores.THREETWO:
                        strScores += "[3 - 2]";
                        break;
                    case TTRScores.THREETHREE:
                        strScores += "[3 - 3]";
                        break;
                    case TTRScores.OTHERS:
                        strScores += "[Others]";
                        break;
                }
            }

            return String.Format("{0} {1}", TradeTheReaction.strScoresASBtn, strScores);
        }

        public override string toXml()
        {
            XElement element = new XElement("checkRule");
            element.SetAttributeValue("checktype", this.GetType().ToString());
            foreach (TTRScores scores in Scores)
            {
                XElement scoreElement = new XElement("score");
                scoreElement.SetValue(scores.ToString());
                element.Add(scoreElement);
            }
            return element.ToString();
        }

        public override void fromXml(string xml)
        {
            XElement element = XElement.Parse(xml);
            foreach (XElement childElement in element.Nodes().OfType<XElement>())
            {
                TTRScores scores = (TTRScores)Enum.Parse(typeof(TTRScores), childElement.Value);
                if (Scores == null)
                    Scores = new ScoreList();
                Scores.Add(scores);
            }
        }
    }

    public class TTRTradeOutGoalSumRule : TTRTradeOutRuleFragment
    {
        public int Lo { get; set; }
        public int Hi { get; set; }
        public override bool check(IScore score)
        {
            if (score == null)
                return false;
            if (score.ScoreA + score.ScoreB < Lo || score.ScoreA + score.ScoreB > Hi)
                return false;

            return true;
        }

        public override string toXml()
        {
            XElement element = new XElement("checkRule");
            element.SetAttributeValue("checktype", this.GetType().ToString());
            element.SetAttributeValue("lo", this.Lo.ToString(CultureInfo.InvariantCulture));
            element.SetAttributeValue("hi", this.Hi.ToString(CultureInfo.InvariantCulture));
            return element.ToString();
        }

        public override void fromXml(string xml)
        {
            XElement element = XElement.Parse(xml);
            if (element.Attribute("lo") != null)
            {
                this.Lo = Int32.Parse(element.Attribute("lo").Value,CultureInfo.InvariantCulture);
            }

            if (element.Attribute("hi") != null)
            {
                this.Hi = Int32.Parse(element.Attribute("hi").Value, CultureInfo.InvariantCulture);
            }
        }

        public override string ToString()
        {
            String msg = String.Format("{0} {1} {2} {3} {4}", TradeTheReaction.strGoalSum, TradeTheReaction.strBetween, this.Lo,
                TradeTheReaction.strAnd, this.Hi);
            return msg;
        }
    }

    public class TTRTradeOutPlayTimeRule : TTRTradeOutRuleFragment
    {
        public int Lo { get; set; }
        public int Hi { get; set; }
        public override bool check(IScore score)
        {

            if (score == null)
                return false;
            if (score.Playtime < Lo || score.Playtime > Hi)
                return false;
            return true;
        }

        public override string toXml()
        {
            XElement element = new XElement("checkRule");
            element.SetAttributeValue("checktype", this.GetType().ToString());
            element.SetAttributeValue("lo", this.Lo.ToString(CultureInfo.InvariantCulture));
            element.SetAttributeValue("hi", this.Hi.ToString(CultureInfo.InvariantCulture));
            return element.ToString();
        }

        public override void fromXml(string xml)
        {
            XElement element = XElement.Parse(xml);
            if (element.Attribute("lo") != null)
            {
                this.Lo = Int32.Parse(element.Attribute("lo").Value, CultureInfo.InvariantCulture);
            }

            if (element.Attribute("hi") != null)
            {
                this.Hi = Int32.Parse(element.Attribute("hi").Value, CultureInfo.InvariantCulture);
            }
        }

        public override string ToString()
        {
            String msg = String.Format("{0} {1} {2} {3} {4}", TradeTheReaction.strPlaytime, TradeTheReaction.strBetween, this.Lo,
                TradeTheReaction.strAnd, this.Hi);
            return msg;
        }
    }

    public class TTRTradeOutRCTeamAMore : TTRTradeOutRuleFragment
    {
        public override bool check(IScore score)
        {
            if (score.RedA <= score.RedB)
                return false;
            return true;
        }

        public override string toXml()
        {
            XElement element = new XElement("checkRule");
            element.SetAttributeValue("checktype", this.GetType().ToString());
            return element.ToString();
        }

        public override void fromXml(string xml)
        {
        }

        public override string ToString()
        {
            String msg = String.Format("{0}",TradeTheReaction.strTAMoreRCR);
            return msg;
        }
    }

    public class TTRTradeOutRCTeamBMore : TTRTradeOutRuleFragment
    {
        public override bool check(IScore score)
        {
            if (score.RedA >= score.RedB)
                return false;
            return true;
        }

        public override string toXml()
        {
            XElement element = new XElement("checkRule");
            element.SetAttributeValue("checktype", this.GetType().ToString());
            return element.ToString();
        }

        public override void fromXml(string xml)
        {
        }

        public override string ToString()
        {
            String msg = String.Format("{0}", TradeTheReaction.strTBMoreRCR);
            return msg;
        }
    }

    public class TTRTradeOutRCEqual : TTRTradeOutRuleFragment
    {
        public override bool check(IScore score)
        {
            if (score.RedA == 0 && score.RedB == 0)
                return false;
            if (score.RedA != score.RedB)
                return false;
            return true;
        }

        public override string toXml()
        {
            XElement element = new XElement("checkRule");
            element.SetAttributeValue("checktype", this.GetType().ToString());
            return element.ToString();
        }

        public override void fromXml(string xml)
        {
        }

        public override string ToString()
        {
            String msg = String.Format("{0}", TradeTheReaction.strEqualRCR);
            return msg;
        }
    }

    public class TTRTradeOutRulesList : List<TTRTradeOutRuleFragment> { }

    public class TTRTradeOutSettings
    {
        #region Over/Under
        public Boolean OnlyHedge { get; set; }
        public Boolean CheckLayOdds { get; set; }
        public Boolean UseWaitTime { get; set; }
        public Boolean UseOddsPercentage { get; set; }
        public Boolean UseHedgeWaitTime { get; set; }
        public Boolean UseGreenWaitTime { get; set; }
        public Boolean NoTrade { get; set; }
        public int HedgeWaitTime { get; set; }
        public int HedgePlaytime { get; set; }
        public int GreenWaitTime { get; set; }
        public int GreenPlaytime { get; set; }
        public int HedgePercentage { get; set; }
        public int GreenPercentage { get; set; }
        #endregion

        #region Correct Score
        #region Back Mode        
        public Boolean CSBackCheckLayOdds { get; set; }
        public Boolean CSBackOnlyHedge { get; set; }
        
        public Boolean CSBackUseOddsPercentage { get; set; }
        public int CSBackHedgePercentage { get; set; }
        public int CSBackGreenPercentage { get; set; }

        public Boolean CSBackUseWaitTime { get; set; }

        public Boolean CSBackUseHedgeWaitTime { get; set; }
        public int CSBackHedgeWaitTime { get; set; }
        public int CSBackHedgePlayTime { get; set; }
        public Boolean CSBackUseHedgeSpecialPlayTime { get; set; }
        public SPECIALPLAYTIME CSBackHedgeSpecialPlayTime { get; set; }
        public int CSBackHedgeSpecialPlayTimeDelta { get; set; }

        public Boolean CSBackUseGreenWaitTime { get; set; }
        public int CSBackGreenWaittime { get; set; }
        public int CSBackGreenPlaytime { get; set; }
        public Boolean CSBackUseGreenSpecialPlayTime { get; set; }
        public SPECIALPLAYTIME CSBackGreenSpecialPlayTime { get; set; }
        public int CSBackGreenSpecialPlayTimeDelta { get; set; }
        #endregion
        #endregion

        public TRADETYPE TradeType { get; set; }

        public TTRTradeOutSettings()
        {
            TTRConfigurationRW config = new TTRConfigurationRW();
            #region Over/Under
            this.OnlyHedge = config.OverUnderDefaultOnlyHedge;
            this.CheckLayOdds = config.OverUnderDefaultCheckLayOdds;
            this.UseWaitTime = config.OverUnderDefaultUseWaitTime;
            this.UseOddsPercentage = config.OverUnderDefaultUseOddsPercentage;
            this.UseHedgeWaitTime = config.OverUnderDefaultUseHedgeWaittime;
            this.UseGreenWaitTime = config.OverUnderDefaultUseGreenWaittime;
            this.NoTrade = false;
            this.HedgeWaitTime = config.OverUnderDefaultHedgeWaitTime;
            this.HedgePlaytime = config.OverUnderDefaultHedgePlayime;
            this.GreenWaitTime = config.OverUnderDefaultGreenWaitTime;
            this.GreenPlaytime = config.OverUnderDefaultGreenPlayime;
            this.HedgePercentage = config.OverUnderDefaultHedgePercentage;
            this.GreenPercentage = config.OverUnderDefaultGreenPercentage;
            #endregion

            #region Correct Score
            #region Back Mode
            this.CSBackCheckLayOdds              = config.CSBackDefaultCheckLayOdds;
            this.CSBackGreenPercentage           = config.CSBackDefaultGreenPercentage;
            this.CSBackGreenPlaytime             = config.CSBackDefaultGreenPlayime;
            this.CSBackGreenSpecialPlayTime      = config.CSBackDefaultGreenSpecialPlayime;
            this.CSBackGreenSpecialPlayTimeDelta = config.CSBackDefaultGreenSpecialPlayimeDelta;
            this.CSBackGreenWaittime             = config.CSBackDefaultGreenWaitTime;
            this.CSBackHedgePercentage           = config.CSBackDefaultHedgePercentage;
            this.CSBackHedgePlayTime             = config.CSBackDefaultHedgePlayime;
            this.CSBackHedgeSpecialPlayTime      = config.CSBackDefaultHedgeSpecialPlayime;
            this.CSBackHedgeSpecialPlayTimeDelta = config.CSBackDefaultHedgeSpecialPlayimeDelta;
            this.CSBackHedgeWaitTime             = config.CSBackDefaultHedgeWaitTime;
            this.CSBackOnlyHedge                 = config.CSBackDefaultOnlyHedge;
            this.CSBackUseGreenSpecialPlayTime   = config.CSBackDefaultUseGreenSpecialPlayTime;
            this.CSBackUseGreenWaitTime          = config.CSBackDefaultUseGreenWaitTime;
            this.CSBackUseHedgeSpecialPlayTime   = config.CSBackDefaultUseHedgeSpecialPlayTime;
            this.CSBackUseHedgeWaitTime          = config.CSBackDefaultUseHedgeWaitTime;
            this.CSBackUseOddsPercentage         = config.CSBackDefaultUseOddsPercentage;
            this.CSBackUseWaitTime               = config.CSBackDefaultUseWaitTime;
            #endregion
            #endregion

        }

        public override string ToString()
        {
            String msg = TradeTheReaction.strTradeOutSetting + "\r\n";

            if (this.NoTrade)
            {
                msg += TradeTheReaction.strNoTrading;
            }
            else
            {
                switch(this.TradeType)
                {
                    case TRADETYPE.OVER05:
                    case TRADETYPE.OVER15:
                    case TRADETYPE.OVER25:
                    case TRADETYPE.OVER35:
                    case TRADETYPE.OVER45:
                    case TRADETYPE.OVER55:
                    case TRADETYPE.OVER65:
                    case TRADETYPE.OVER75:
                    case TRADETYPE.OVER85:
                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strHedgeOnly, this.OnlyHedge.ToString());
                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strCheckLayOdds, this.CheckLayOdds.ToString());
                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strUseWaitTime, this.UseWaitTime.ToString());
                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strUseOddsPercentage, this.UseOddsPercentage.ToString());
                        msg += "\r\n";

                        if (this.UseWaitTime)
                        {
                            msg += String.Format("{0}: {1}. ", TradeTheReaction.strUseHedgeWaitTime, this.UseHedgeWaitTime.ToString());
                            msg += String.Format("{0}: {1}. ", TradeTheReaction.strUseGreenMinutes, this.UseGreenWaitTime.ToString());
                            msg += "\r\n";
                        }

                        if (this.UseWaitTime && this.UseHedgeWaitTime)
                        {
                            msg += String.Format("{0}: {1} {2}. ", TradeTheReaction.strHedgeWaitTime, this.HedgeWaitTime, TradeTheReaction.strSeconds);
                            msg += "\r\n";
                        }
                        else
                        {
                            msg += String.Format("{0}: {1}. ", TradeTheReaction.strHedgePlaytime, this.HedgePlaytime);
                            msg += "\r\n";
                        }

                        
                        if (!this.OnlyHedge)
                        {
                            if (this.UseWaitTime && this.UseGreenWaitTime)
                            {
                                msg += String.Format("{0}: {1} {2}. ", TradeTheReaction.strGreenWaittime, this.GreenWaitTime, TradeTheReaction.strSeconds);
                                msg += "\r\n";
                            }
                            else
                            {
                                msg += String.Format("{0}: {1}. ", TradeTheReaction.strGreenPlaytime, this.GreenPlaytime);
                                msg += "\r\n";
                            }
                        }
                        if (this.UseOddsPercentage)
                        {
                            msg += String.Format("{0}: {1} {2}. ", TradeTheReaction.strHedgePercentage, this.HedgePercentage, TradeTheReaction.strPercent);
                            if (!this.OnlyHedge)
                            {
                                msg += String.Format("{0}: {1} {2}. ", TradeTheReaction.strGreenPercentage, this.GreenPercentage, TradeTheReaction.strPercent);
                            }
                        }
                        break;
                    case TRADETYPE.SCORELINE01BACK:
                    case TRADETYPE.SCORELINE02BACK:
                    case TRADETYPE.SCORELINE03BACK:
                    case TRADETYPE.SCORELINE10BACK:
                    case TRADETYPE.SCORELINE11BACK:
                    case TRADETYPE.SCORELINE12BACK:
                    case TRADETYPE.SCORELINE13BACK:
                    case TRADETYPE.SCORELINE20BACK:
                    case TRADETYPE.SCORELINE21BACK:
                    case TRADETYPE.SCORELINE22BACK:
                    case TRADETYPE.SCORELINE23BACK:
                    case TRADETYPE.SCORELINE30BACK:
                    case TRADETYPE.SCORELINE31BACK:
                    case TRADETYPE.SCORELINE32BACK:
                    case TRADETYPE.SCORELINE33BACK:
                    case TRADETYPE.SCORELINEOTHERBACK:
                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strHedgeOnly, this.CSBackOnlyHedge.ToString());
                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strCheckLayOdds, this.CSBackCheckLayOdds.ToString());
                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strUseWaitTime, this.CSBackUseWaitTime.ToString());
                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strUseOddsPercentage, this.CSBackUseOddsPercentage.ToString());
                        msg += "\r\n";

                        if (this.CSBackUseWaitTime)
                        {
                            msg += String.Format("{0}: {1}. ", TradeTheReaction.strUseHedgeWaitTime, this.CSBackUseHedgeWaitTime.ToString());
                            msg += String.Format("{0}: {1}. ", TradeTheReaction.strUseGreenMinutes, this.CSBackUseGreenWaitTime.ToString());
                            msg += "\r\n";
                        }

                        if (this.CSBackUseWaitTime && this.CSBackUseHedgeWaitTime)
                        {
                            msg += String.Format("{0}: {1} {2}. ", TradeTheReaction.strHedgeWaitTime, this.CSBackHedgeWaitTime, TradeTheReaction.strSeconds);
                            msg += "\r\n";
                        }
                        else
                        {
                            if (this.CSBackUseHedgeSpecialPlayTime)
                            {
                                msg += String.Format("{0}: {1}. ", TradeTheReaction.strHedgeSpecialTime, this.CSBackHedgeSpecialPlayTime.ToString());
                                if(this.CSBackHedgeSpecialPlayTimeDelta != 0)
                                {
                                    msg += String.Format("{0}: {1}. ", TradeTheReaction.strDelta, this.CSBackHedgeSpecialPlayTimeDelta.ToString());
                                }
                            }
                            else
                            {
                                msg += String.Format("{0}: {1}. ", TradeTheReaction.strHedgePlaytime, this.CSBackHedgePlayTime);
                            }
                            msg += "\r\n";
                        }


                        if (!this.CSBackOnlyHedge)
                        {
                            if (this.CSBackUseWaitTime && this.CSBackUseGreenWaitTime)
                            {
                                msg += String.Format("{0}: {1} {2}. ", TradeTheReaction.strGreenWaittime, this.CSBackGreenWaittime, TradeTheReaction.strSeconds);
                                msg += "\r\n";
                            }
                            else
                            {
                                if (this.CSBackUseGreenSpecialPlayTime)
                                {
                                    msg += String.Format("{0}: {1}. ", TradeTheReaction.strGreenSpecialTime, this.CSBackGreenSpecialPlayTime.ToString());
                                    if (this.CSBackHedgeSpecialPlayTimeDelta != 0)
                                    {
                                        msg += String.Format("{0}: {1}. ", TradeTheReaction.strDelta, this.CSBackGreenSpecialPlayTimeDelta.ToString());
                                    }
                                }
                                else
                                {
                                    msg += String.Format("{0}: {1}. ", TradeTheReaction.strGreenPlaytime, this.CSBackGreenPlaytime);
                                }
                                msg += "\r\n";
                            }
                        }
                        if (this.UseOddsPercentage)
                        {
                            msg += String.Format("{0}: {1} {2}. ", TradeTheReaction.strHedgePercentage, this.CSBackHedgePercentage, TradeTheReaction.strPercent);
                            if (!this.CSBackOnlyHedge)
                            {
                                msg += String.Format("{0}: {1} {2}. ", TradeTheReaction.strGreenPercentage, this.CSBackGreenPercentage, TradeTheReaction.strPercent);
                            }
                        }
                        break;
                    case TRADETYPE.SCORELINE01LAY:
                    case TRADETYPE.SCORELINE02LAY:
                    case TRADETYPE.SCORELINE03LAY:
                    case TRADETYPE.SCORELINE10LAY:
                    case TRADETYPE.SCORELINE11LAY:
                    case TRADETYPE.SCORELINE12LAY:
                    case TRADETYPE.SCORELINE13LAY:
                    case TRADETYPE.SCORELINE20LAY:
                    case TRADETYPE.SCORELINE21LAY:
                    case TRADETYPE.SCORELINE22LAY:
                    case TRADETYPE.SCORELINE23LAY:
                    case TRADETYPE.SCORELINE30LAY:
                    case TRADETYPE.SCORELINE31LAY:
                    case TRADETYPE.SCORELINE32LAY:
                    case TRADETYPE.SCORELINE33LAY:
                    case TRADETYPE.SCORELINEOTHERLAY:
                        msg += "To be defined";
                        break;
                    default:
                        msg = "Unknown Trading Type!";
                        break;
                }
            }
            return msg;
        }
    }

    public class TTRTradeOutCheck
    {
        private static int _static_idPool = 0;
        private TRADEOUTTRIGGER _trigger;
        private TTRTradeOutSettings _settings;
        private TTRTradeOutRulesList _ruleList;
        private int _checkOrder;
        private int _ruleId;
        private object _lock = "_ttrTradeOutCheckLock";

        public TRADEOUTTRIGGER Trigger { get { return _trigger; } set { _trigger = value; } }
        public TTRTradeOutSettings TradeOutSettings { get { return _settings; } set { _settings = value;} }
        public TTRTradeOutRulesList Rules { get { return _ruleList; } }
        public int Order { get { return _checkOrder; } set { _checkOrder = value; } }
        public int Id { get { return _ruleId; } }

        public TTRTradeOutCheck()
        {
            lock (_lock)
            {
                _ruleId = ++_static_idPool;
            }
            _ruleList = new TTRTradeOutRulesList();
            _settings = new TTRTradeOutSettings();
        }

        public override string ToString()
        {
            String msg = String.Format("{0}: {1}. ", TradeTheReaction.strRuleId, _ruleId);
            msg += String.Format("{0}: {1}\r\n", TradeTheReaction.strOrder, _checkOrder);
            msg += String.Format("{0}: {1}\r\n", TradeTheReaction.strTrigger, _trigger == TRADEOUTTRIGGER.GENERAL ? 
                    TradeTheReaction.strTriggerGeneral :
                        _trigger == TRADEOUTTRIGGER.GOAL ? 
                            TradeTheReaction.strTriggerGoalScored : 
                                _trigger == TRADEOUTTRIGGER.REDCARD ? 
                                    TradeTheReaction.strTriggerRedCard :
                                        _trigger == TRADEOUTTRIGGER.PLAYTIME ?
                                            TradeTheReaction.strPlaytime :
                                                "undefined");
            msg += TradeTheReaction.strTradeOutRule + "\r\n";
            foreach (TTRTradeOutRuleFragment fragment in _ruleList)
            {
                msg += fragment.ToString();
            }

            msg += "\r\n\r\n";

            msg += _settings.ToString();

            return msg;
        }
    }

    public class TTRTradeOutCheckSortedList : SortedList<int, TTRTradeOutCheck> 
    {
        TRADETYPE _tradeType = TRADETYPE.UNASSIGNED;
        public TRADETYPE TradeType
        {
            get
            {
                if (this.Count == 0)
                    return _tradeType;//TRADETYPE.UNASSIGNED;
                else
                    return this.Values[0].TradeOutSettings.TradeType;
            }
            set
            {
                _tradeType = value;
                foreach (TTRTradeOutCheck tradeOutcheck in this.Values)
                {
                    tradeOutcheck.TradeOutSettings.TradeType = value;
                }
            }
        }

        public TTRTradeOutCheck getTradeOutSettings(TRADEOUTTRIGGER trigger, IScore liverticker)
        {
            // Falls überhaupt keine Benutzerdefinierte Regeln existieren => Nimm die Default-Einstellungen
            if (this.Count == 0)
                return new TTRTradeOutCheck();

            TTRTradeOutCheckSortedList tmpList = new TTRTradeOutCheckSortedList();
            foreach (TTRTradeOutCheck tradeOutcheck in this.Values)
            {
                if (tradeOutcheck.Trigger == trigger || tradeOutcheck.Trigger == TRADEOUTTRIGGER.GENERAL)
                {
                    bool bRule = true;
                    foreach (TTRTradeOutRuleFragment fragment in tradeOutcheck.Rules)
                    {
                        if (!fragment.check(liverticker))
                        {
                            bRule = false;
                            break;
                        }
                    }
                    if (bRule)
                    {
                        tmpList.Add(tradeOutcheck.Order, tradeOutcheck);
                    }
                }
            }

            if(tmpList.Count != 0)
                return tmpList.ElementAt(0).Value;

            return null;
        }
    }


}
