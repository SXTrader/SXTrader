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
        #region Over/Under Default
        /// <summary>
        /// Dieses Flag gibt an, ob nur gehedged, d. h. eine Lay Wette wird so platziert, das kein Verlust mehr entstehen kann,
        /// oder anschliessend auch gegreened, d.h. eine weiter Lay Wette wird so platziert, das der Gewinn in beiden 
        /// Fällen ungefähr gleich ist, werden soll
        /// </summary>
        public Boolean OverUnderDefaultOnlyHedge
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTONLYHEDGE)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(OUDFLTONLYHEDGE, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTONLYHEDGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Sollen die Lay Quote überprüft werden, ob sie geringer als die Back Quote ist?
        /// </summary>
        public Boolean OverUnderDefaultCheckLayOdds
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTCHECKLAYODDS)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(OUDFLTCHECKLAYODDS, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTCHECKLAYODDS)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Soll die Zeitüberwachung benutzt werden?
        /// </summary>
        public Boolean OverUnderDefaultUseWaitTime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTUSEWAITTIME)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(OUDFLTUSEWAITTIME, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTUSEWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Soll die Quotenprozentüberwachung benutzt werden?
        /// </summary>
        public Boolean OverUnderDefaultUseOddsPercentage
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTUSEODDSPERC)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(OUDFLTUSEODDSPERC, Boolean.FalseString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTUSEODDSPERC)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Soll nach einen gegebenen Zeitintervall das Hedging stattfinden, oder
        /// sobald eine definierte Spielminute erreicht wurde?
        /// </summary>
        public Boolean OverUnderDefaultUseHedgeWaittime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTUSEHEDGEMINUTES)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(OUDFLTUSEHEDGEMINUTES, Boolean.TrueString);
                return true;

            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTUSEHEDGEMINUTES)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Wartezeit in Sekunden, welche vergehen muss bevor versucht wird den Trade zu
        /// hedgen, d.h. Gegenwetten so zu setzen, dass kein Verlustrisiko mehr besteht.
        /// </summary>
        public int OverUnderDefaultHedgeWaitTime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTHEDGEWAITTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(OUDFLTHEDGEWAITTIME, "120");

                return 120;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTHEDGEWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Spielminute, welche erreicht werden muss bevor versucht wird den Trade zu
        /// hedgen, d.h. Gegenwetten so zu setzen, dass kein Verlustrisiko mehr besteht.
        /// </summary>
        public int OverUnderDefaultHedgePlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTHEDGEPLAYTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(OUDFLTHEDGEPLAYTIME, "45");

                return 45;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTHEDGEPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Soll nach einen gegebenen Zeitintervall das Greening stattfinden, oder
        /// sobald eine definierte Spielminute erreicht wurde?
        /// </summary>
        public Boolean OverUnderDefaultUseGreenWaittime
        {
            get
            {

                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTUSEGREENMINUTES)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                createConfigNode(OUDFLTUSEGREENMINUTES, Boolean.TrueString);
                return true;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTUSEGREENMINUTES)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Wartezeit in Sekunden, welche vergehen muss bevor versucht wird, den Trade 
        /// zu greenen, d.h. durch geschicktes platzieren von Gegenwetten eine Situation zu schaffen
        /// in der unabhängig von Ausgang des Ereignis ein konstanter Gewinn gefahren wird.
        /// </summary>
        public int OverUnderDefaultGreenWaitTime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTGREENWAITTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(OUDFLTGREENWAITTIME, "120");

                return 120;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTGREENWAITTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Spielminute, welche erreicht werden muss bevor versucht wird den Trade zu
        /// Greenen, d.h. durch geschicktes platzieren von Gegenwetten eine Situation zu schaffen
        /// in der unabhängig von Ausgang des Ereignis ein konstanter Gewinn gefahren wird.
        /// </summary>
        public int OverUnderDefaultGreenPlayime
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTGREENPLAYTIME)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(OUDFLTGREENPLAYTIME, "60");

                return 60;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTGREENPLAYTIME)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Prozentuale Abweichung von der Ursprungs-Back-Quote nach unten, die erfüllt sein muss, bovor
        /// gehedged wird
        /// </summary>
        public int OverUnderDefaultHedgePercentage
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTHEDGEPERCENTAGE)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(OUDFLTHEDGEPERCENTAGE, "70");

                return 70;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTHEDGEPERCENTAGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Prozentuale Abweichung von der Ursprungs-Back-Quote nach unten, die erfüllt sein muss, bovor
        /// gegreened wird
        /// </summary>
        public int OverUnderDefaultGreenPercentage
        {
            get
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTGREENPERCENTAGE)
                        return Int16.Parse(node.Attributes["value"].Value);
                }
                createConfigNode(OUDFLTGREENPERCENTAGE, "50");

                return 50;
            }

            set
            {
                XmlNode rootNode = _doc.FirstChild;
                if (rootNode.Name != "configuration")
                    rootNode = _doc.ChildNodes[1];

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == OUDFLTGREENPERCENTAGE)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }
        #endregion
    }
}
