using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;
using net.sxtrader.plugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace net.sxtrader.bftradingstrategies.tippsters.Configration
{
    public class TLLConfigurationRW : IConfiguration, IDisposable
    {
        internal XmlDocument _doc = null;
        private readonly byte[] entropy = new byte[] { 0x01, 0x22, 0x47, 0x77, 0x04, 0x22, 0x42 };

        private const int CONFIGVERSION = 2;

        private const String MAILHOST = "mailhost";
        private const String HOSTPORT = "hostport";
        private const String MAILUSER = "mailuser";
        private const String MAILACCESS = "mailaccess";
        private const String MAILCHECKINTERVAL = "mailcheckinterval";
        private const String USESSL = "usessl";

        private const String MAXIMUMLAYODD = "maximumlayodd";
        private const String KEEPINRUNNING = "keepinrunning";
        private const String PLACEMAX = "placemax";
        private const String NROBSERVATION = "nonrunnerobservation";
        private const String NRCOUNT1 = "nonrunnercount1";
        private const String NRCOUNT2 = "nonrunnercount2";
        private const String NRCOUNT3 = "nonrunnercount3";
        private const String NRCOUNT4 = "nonrunnercount4";
        private const String NRCOUNT5 = "nonrunnercount5";
        private const String USESTEPTRADING = "usesteptrading";
        private const String STEPTRADINGGAP = "steptradingstep";
        private const String USEFINALTIME = "usefinaltime";
        private const String FINALTIME = "finaltime";
        private const String BEGINTIME = "begintime";

        private const String BETAMOUNT = "betamount";
        private const String BETTYPE = "bettype";

        private const String STRATEGYACTIVE = "strategyactive";

        public const int FIELD1SIZE = 5;
        public const int FIELD2SIZE = 10;
        public const int FIELD3SIZE = 15;
        public const int FIELD4SIZE = 20;

        private const int MINMAILCHECKINTERVAL = 5;

        private String _file = String.Empty;
        private bool _disposed = false;


        public TLLConfigurationRW(XElement settings)
        {
            int version = 99;
            IEnumerable<XElement> entries = from el in settings.Elements("configuration")
                                            select el;
            foreach (XElement entry in entries)
            {
                if (Int32.TryParse(entry.Attribute("configversion").Value, out version))
                    break;
            }

            if (_doc == null)
                _doc = new XmlDocument();

            _doc.LoadXml(settings.Element("configuration").ToString());

            if (version < CONFIGVERSION)
                upgradeConfig();

        }

        public TLLConfigurationRW(string xml, int version)
        {
            if (_doc == null)
                _doc = new XmlDocument();

            _doc.LoadXml(xml);

            if (version < CONFIGVERSION)
                upgradeConfig();
        }

        public TLLConfigurationRW()
        {
            _file = SXDirs.CfgPath + @"\TTLConfiguration.dat";
            _doc = new XmlDocument();

            try
            {

                byte[] configData = File.ReadAllBytes(_file);
                byte[] nextConfigData = ProtectedData.Unprotect(configData, entropy, DataProtectionScope.CurrentUser);
                String configDataString = Encoding.Unicode.GetString(nextConfigData);
                //_doc.Load(_file);
                _doc.LoadXml(configDataString);

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

                byte[] sensitiveData = Encoding.Unicode.GetBytes(_doc.InnerXml);
                byte[] protectedData = ProtectedData.Protect(sensitiveData, entropy, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(_file, protectedData);

                byte[] configData = File.ReadAllBytes(_file);
                byte[] nextConfigData = ProtectedData.Unprotect(configData, entropy, DataProtectionScope.CurrentUser);
                String configDataString = Encoding.Unicode.GetString(nextConfigData);

                _doc.LoadXml(configDataString);
                //_doc.Load(_file);
                createBasicConfiguration();
            }
        }

        #region MailSettings
        public String MailHost
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAILHOST)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(MAILHOST, String.Empty);
                return String.Empty;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAILHOST)
                    {
                        node.Attributes["value"].Value = value;
                        break;
                    }
                }
            }
        }

        public String MailUser
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAILUSER)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(MAILUSER, String.Empty);
                return String.Empty;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAILUSER)
                    {
                        node.Attributes["value"].Value = value;
                        break;
                    }
                }
            }
        }

        public String MailAccess
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAILACCESS)
                        return node.Attributes["value"].Value;
                }

                createConfigNode(MAILACCESS, String.Empty);
                return String.Empty;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAILACCESS)
                    {
                        node.Attributes["value"].Value = value;
                        break;
                    }
                }
            }
        }

        public int MailCheckInterval
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAILCHECKINTERVAL)
                    {
                        if (Int16.Parse(node.Attributes["value"].Value) >= MINMAILCHECKINTERVAL)
                            return Int16.Parse(node.Attributes["value"].Value);
                        else return MINMAILCHECKINTERVAL;
                    }
                }
                createConfigNode(HOSTPORT,MINMAILCHECKINTERVAL.ToString());

                return MINMAILCHECKINTERVAL;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAILCHECKINTERVAL)
                    {
                        node.Attributes["value"].Value = value.ToString();
                        break;
                    }
                }
            }
        }

        public int MailPort
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HOSTPORT)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(HOSTPORT, "143");

                return 143;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == HOSTPORT)
                    {
                        node.Attributes["value"].Value = value.ToString();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Soll die Anmeldung an dem IMAP-Server via SSL erfolgen
        /// </summary>
        public Boolean UseSSL
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USESSL)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USESSL, Boolean.FalseString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USESSL)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        #endregion

        #region TradeSettings
        
        /// <summary>
        /// Maximale LayQuote für die ein Layer Of Profit Tip platziert werden darf.
        /// </summary>
        public double MaximumLayOdd
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAXIMUMLAYODD)
                    {
                        return Double.Parse(node.Attributes["value"].Value, CultureInfo.InvariantCulture);
                    }
                }
                double value = 3.0;
                createConfigNode(MAXIMUMLAYODD, value.ToString(CultureInfo.InvariantCulture));
                return 3.0;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == MAXIMUMLAYODD)
                        node.Attributes["value"].Value = value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        
        /// <summary>
        /// Falls eine Wette noch offen ist, storniere sie nicht automatisch, sobald
        /// dass Rennen startet.
        /// </summary>
        public Boolean KeepInRunning
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == KEEPINRUNNING)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(KEEPINRUNNING, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == KEEPINRUNNING)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Sollte die aktuelle Lay-Quote größer als die Maximalquote sein, so platziere einfach
        /// eine offene Lay-Wette für die Maximalquote
        /// </summary>
        public Boolean PlaceForMax
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == PLACEMAX)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(PLACEMAX, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == PLACEMAX)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Soll die Anzahl der Nichtstarter in der Entscheidung, ob ein Tipp platziert werden soll,
        /// berücksichtige werden?
        /// </summary>
        public Boolean NonStarterObservation
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NROBSERVATION)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(NROBSERVATION, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NROBSERVATION)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Anzahl der Nichtstarter bei einen Starterfeld von 1 bis 5.
        /// </summary>
        public int NonRunnerCount1
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT1)
                    {
                        return Int32.Parse(node.Attributes["value"].Value);
                    }
                }
                int value = 0;
                createConfigNode(NRCOUNT1, value.ToString());
                return 0;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT1)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Anzahl der Nichtstarter bei einen Starterfeld von 6 bis 10.
        /// </summary>
        public int NonRunnerCount2
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT2)
                    {
                        return Int32.Parse(node.Attributes["value"].Value);
                    }
                }
                int value = 0;
                createConfigNode(NRCOUNT2, value.ToString());
                return 0;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT2)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Anzahl der Nichtstarter bei einen Starterfeld von 11 bis 15.
        /// </summary>
        public int NonRunnerCount3
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT3)
                    {
                        return Int32.Parse(node.Attributes["value"].Value);
                    }
                }
                int value = 0;
                createConfigNode(NRCOUNT3, value.ToString());
                return 0;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT3)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Anzahl der Nichtstarter bei einen Starterfeld von 16 bis 20.
        /// </summary>
        public int NonRunnerCount4
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT4)
                    {
                        return Int32.Parse(node.Attributes["value"].Value);
                    }
                }
                int value = 0;
                createConfigNode(NRCOUNT4, value.ToString());
                return 0;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT4)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Anzahl der Nichtstarter bei einen Starterfeld größer als 20.
        /// </summary>
        public int NonRunnerCount5
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT5)
                    {
                        return Int32.Parse(node.Attributes["value"].Value);
                    }
                }
                int value = 0;
                createConfigNode(NRCOUNT5, value.ToString());
                return 0;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == NRCOUNT5)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Falls mind. eine Quote unter Maximum und es gibt eine Lücke zwischen Back und Lay, dann 
        /// benutzer das Schrittweise Traden (Back-Quote + 1 Tick);
        /// </summary>
        public Boolean UseStepTrading
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USESTEPTRADING)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USESTEPTRADING, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USESTEPTRADING)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int StepTradingGap
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == STEPTRADINGGAP)
                    {
                        return Int32.Parse(node.Attributes["value"].Value);
                    }
                }
                int value = 1;
                createConfigNode(STEPTRADINGGAP, value.ToString());
                return 1;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == STEPTRADINGGAP)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gibt an ob es einen letztmöglichen Zeitpunkt vor dem Start des Rennens gibt, 
        /// an dem das Schrittweise Traden abgebrochen wird und auf jeden Fall eine 
        /// Lay-Wette zur aktuellen Lay-Quote bzw. zur Maximumquote platziert wird.
        /// </summary>
        public Boolean UseFinalTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEFINALTIME)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(USEFINALTIME, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == USEFINALTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Bestimmt den Zeitpunkt in Minuten vor Start des Rennens, an dem das Schrittweise Traden
        /// unterbrochen wird.
        /// </summary>
        public int FinalTime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == FINALTIME)
                    {
                        return Int32.Parse(node.Attributes["value"].Value);
                    }
                }
                int value = 5;
                createConfigNode(FINALTIME, value.ToString());
                return value;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == FINALTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Bestimmt den Zeitpunkt in Minuten vor Start des Rennens an dem das Layer Of Profit Modul beginnt zu
        /// versuchen den Tipp umzusetzen.
        /// </summary>
        public int BeginTime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BEGINTIME)
                    {
                        return Int32.Parse(node.Attributes["value"].Value);
                    }
                }
                int value = 120;
                createConfigNode(BEGINTIME, value.ToString());
                return value;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BEGINTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
        #endregion

        #region Bankroll & Money Management Settings

        /// <summary>
        /// Definiert, ob der Geldbetrag als zu setzend (wert = true) oder als erlaubtes Verlustrisiko zu sehen ist (wert = false)
        /// </summary>
        public Boolean BetType
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BETTYPE)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(BETTYPE, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BETTYPE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gibt den zu setztenden Geldbetrag an
        /// </summary>
        public double BetAmount
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BETAMOUNT)
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
                createConfigNode(BETAMOUNT, value.ToString(CultureInfo.InvariantCulture));
                return value;
            }
            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == BETAMOUNT)
                        node.Attributes["value"].Value = (value / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        #endregion

        #region Global Settings
        public Boolean StrategyActive
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == STRATEGYACTIVE)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(STRATEGYACTIVE, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == STRATEGYACTIVE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
        #endregion

        private void createBasicConfiguration()
        {
            ////////////////////////////////////////////////////////////////////
            //
            //     Mail Settings
            //
            ////////////////////////////////////////////////////////////////////
            createConfigNode(MAILHOST, String.Empty);

            createConfigNode(HOSTPORT, "143");

            createConfigNode(MAILUSER, String.Empty);
            //encrypt(MAILUSER);

            createConfigNode(MAILACCESS, String.Empty);
            //encrypt(MAILACCESS);

            createConfigNode(MAILCHECKINTERVAL, MINMAILCHECKINTERVAL.ToString());

            createConfigNode(USESSL, Boolean.FalseString);

            ////////////////////////////////////////////////////////////////////
            //
            //     Trade Settings
            //
            ////////////////////////////////////////////////////////////////////

            createConfigNode(MAXIMUMLAYODD, "3.0");

            createConfigNode(KEEPINRUNNING, Boolean.TrueString);

            createConfigNode(PLACEMAX, Boolean.TrueString);

            createConfigNode(NROBSERVATION, Boolean.FalseString);

            createConfigNode(NRCOUNT1, "0");

            createConfigNode(NRCOUNT2, "0");

            createConfigNode(NRCOUNT3, "0");

            createConfigNode(NRCOUNT4, "0");

            createConfigNode(NRCOUNT5, "0");

            createConfigNode(USESTEPTRADING, Boolean.TrueString);

            createConfigNode(STEPTRADINGGAP, "1");

            createConfigNode(USEFINALTIME, Boolean.TrueString);

            ////////////////////////////////////////////////////////////////////
            //
            //     Bankroll & Money Management Settings
            //
            ////////////////////////////////////////////////////////////////////

            createConfigNode(BETAMOUNT, SXALBankrollManager.Instance.MinStake.ToString(CultureInfo.InvariantCulture));

            createConfigNode(BETTYPE, Boolean.TrueString);

            ////////////////////////////////////////////////////////////////////
            //
            //    Global Settings
            //
            ////////////////////////////////////////////////////////////////////

            createConfigNode(STRATEGYACTIVE, Boolean.FalseString);

            save();
            //_doc.Save(_file);   
        }

        private void upgradeConfig()
        {
        }


        private void createConfigNode(String name, String value)
        {
            XmlNode rootNode = _doc.ChildNodes[1];
            XmlElement configurationElement = _doc.CreateElement("configitem");
            XmlAttribute attributeName = _doc.CreateAttribute("name");
            attributeName.Value = name;

            XmlAttribute attributeValue = _doc.CreateAttribute("value");
            attributeValue.Value = value;

            configurationElement.Attributes.Append(attributeName);
            configurationElement.Attributes.Append(attributeValue);
            rootNode.AppendChild(configurationElement);
        }



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

            byte[] sensitiveData = Encoding.Unicode.GetBytes(_doc.InnerXml);
            byte[] protectedData = ProtectedData.Protect(sensitiveData, entropy, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(_file, protectedData);

            //_doc.Save(_file);
        }

        public string getXML()
        {
            return _doc.InnerXml;
        }
        #endregion

        #region IDisposable Member
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DebugWriter.Instance.WriteMessage("TLLConfigurationRW", "Disposing");
                if (disposing)
                {
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
