using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using net.sxtrader.plugin;

namespace net.sxtrader.bftradingstrategies.ttr.Configuration
{
    public partial class TTRConfigurationRW : IConfiguration
    {
        #region Correct Score
        #region Back Mode
        /// <summary>
        /// Soll die Quotenprozentüberwachung benutzt werden?
        /// </summary>
        public Boolean CSBackDefaultUseOddsPercentage
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEODDSPERCENTAGE)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CSBUSEODDSPERCENTAGE, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEODDSPERCENTAGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Prozentuale Abweichung von der Ursprungs-Back-Quote nach unten, die erfüllt sein muss, bovor
        /// gehedged wird
        /// </summary>
        public int CSBackDefaultHedgePercentage
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGEPERCENTAGE)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(CSBHEDGEPERCENTAGE, "70");

                return 70;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGEPERCENTAGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Prozentuale Abweichung von der Ursprungs-Back-Quote nach unten, die erfüllt sein muss, bovor
        /// gegreened wird
        /// </summary>
        public int CSBackDefaultGreenPercentage
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENPERCENTAGE)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(CSBGREENPERCENTAGE, "50");

                return 50;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENPERCENTAGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Das Hedging wird mittels Countdown gesteuert
        /// </summary>
        public Boolean CSBackDefaultUseHedgeWaitTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEHEDGEWAITTIME)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CSBUSEHEDGEWAITTIME, Boolean.TrueString);
                return true;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEHEDGEWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Spielminute, welche erreicht werden muss bevor versucht wird den Trade zu
        /// hedgen, d.h. Gegenwetten so zu setzen, dass kein Verlustrisiko mehr besteht.
        /// </summary>
        public int CSBackDefaultHedgePlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGEPLAYTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(CSBHEDGEPLAYTIME, "10");

                return 10;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGEPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Das Hedging wird mittels einen speziellen statistischen Wert gesteuert.
        /// </summary>
        public Boolean CSBackDefaultUseHedgeSpecialPlayTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEHEDGESPECIALPT)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CSBUSEHEDGESPECIALPT, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEHEDGESPECIALPT)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Statistischer Wert, aus dem die Spielminute gelesen wird bevor versucht wird den Trade zu
        /// hedgen, d.h. Gegenwetten so zu setzen, dass kein Verlustrisiko mehr besteht.
        /// </summary>
        public SPECIALPLAYTIME CSBackDefaultHedgeSpecialPlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGESPECIALPLAYTIME)
                        return (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), node.Attributes["value"].Value);
                }
                createConfigNode(CSBHEDGESPECIALPLAYTIME, SPECIALPLAYTIME.UNASSIGNED.ToString());

                return SPECIALPLAYTIME.UNASSIGNED;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGESPECIALPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Delta-Spielminute zur Special Hedging-time
        /// </summary>
        public int CSBackDefaultHedgeSpecialPlayimeDelta
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGESPECIALPTDELTA)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(CSBHEDGESPECIALPTDELTA, "0");

                return 0;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGESPECIALPTDELTA)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Soll die Zeitüberwachung benutzt werden?
        /// </summary>
        public Boolean CSBackDefaultUseWaitTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEWAITTIME)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CSBUSEWAITTIME, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Wartezeit in Sekunden, welche vergehen muss bevor versucht wird den Trade zu
        /// hedgen, d.h. Gegenwetten so zu setzen, dass kein Verlustrisiko mehr besteht.
        /// </summary>
        public int CSBackDefaultHedgeWaitTime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGEWAITTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(CSBHEDGEWAITTIME, "600");

                return 600;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBHEDGEWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Das Greening wird mittels Countdown gesteuert
        /// </summary>
        public Boolean CSBackDefaultUseGreenWaitTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEGREENWAITTIME)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CSBUSEGREENWAITTIME, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEGREENWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Spielminute, welche erreicht werden muss bevor versucht wird den Trade zu
        /// greenen, d.h. Gegenwetten so zu setzen, unabhängig vom Ausgang der gleiche 
        /// Gewinn/Verlust entsteht.
        /// </summary>
        public int CSBackDefaultGreenPlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENPLAYTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(CSBGREENPLAYTIME, "20");

                return 20;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Das Greening wird mittels einen speziellen statistischen Wert gesteuert.
        /// </summary>
        public Boolean CSBackDefaultUseGreenSpecialPlayTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEGREENSPECIALPT)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CSBUSEGREENSPECIALPT, Boolean.FalseString);
                return false;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBUSEGREENSPECIALPT)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Statistischer Wert, aus dem die Spielminute gelesen wird bevor versucht wird den Trade zu
        /// greenen, d.h. Gegenwetten so zu setzen, dass, unabhängig vom Endergebnisse, der gleiche 
        /// Gewinn/Verlust entsteht.
        /// </summary>
        public SPECIALPLAYTIME CSBackDefaultGreenSpecialPlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENSPECIALPLAYTIME)
                        return (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), node.Attributes["value"].Value);
                }
                createConfigNode(CSBGREENSPECIALPLAYTIME, SPECIALPLAYTIME.UNASSIGNED.ToString());

                return SPECIALPLAYTIME.UNASSIGNED;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENSPECIALPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Delta-Spielminute zur Special Greening-Spielminute
        /// </summary>
        public int CSBackDefaultGreenSpecialPlayimeDelta
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENSPECIALPTDELTA)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(CSBGREENSPECIALPTDELTA, "0");

                return 0;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENSPECIALPTDELTA)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Wartezeit in Sekunden, welche vergehen muss bevor versucht wird, den Trade 
        /// zu greenen, d.h. durch geschicktes platzieren von Gegenwetten eine Situation zu schaffen
        /// in der unabhängig von Ausgang des Ereignis ein konstanter Gewinn gefahren wird.
        /// </summary>
        public int CSBackDefaultGreenWaitTime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENWAITTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(CSBGREENWAITTIME, "600");

                return 600;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBGREENWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Sollen die Lay Quote überprüft werden, ob sie geringer als die Back Quote ist?
        /// </summary>
        public Boolean CSBackDefaultCheckLayOdds
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBCHECKLAYODDS)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CSBCHECKLAYODDS, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBCHECKLAYODDS)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Dieses Flag gibt an, ob nur gehedged, d. h. eine Lay Wette wird so platziert, das kein Verlust mehr entstehen kann,
        /// oder anschliessend auch gegreened, d.h. eine weiter Lay Wette wird so platziert, das der Gewinn in beiden 
        /// Fällen ungefähr gleich ist, werden soll
        /// </summary>
        public Boolean CSBackDefaultOnlyHedge
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBONLYHEDGE)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(CSBONLYHEDGE, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == CSBONLYHEDGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        #endregion
        #endregion
    }
}
