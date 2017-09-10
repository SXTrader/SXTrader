using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    class LTDConfigurationReader
    {
        private XmlDocument m_doc = null;        

        public LTDConfigurationReader()
        {
            String strFile = SXDirs.CfgPath + @"\LTDConfiguration.xml"; 
            m_doc = new XmlDocument();
            try
            {
                m_doc.Load(strFile);
            }
            catch (System.IO.FileNotFoundException)
            {
                m_doc = null; 
            }
        }

        #region Fast Lay
        public Boolean FastBetFixedAmount
        {
            get
            {

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastLayFixedAmount)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
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

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetFixedAmountValue)
                        return Double.Parse(node.Attributes["value"].Value);
                }
                double value = SXALBankrollManager.Instance.MinStake;
                return value;
            }
            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetFixedAmountValue)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        public int FastBetPercentAmountValue
        {
            get
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetPercentAmountValue)
                        return Int16.Parse(node.Attributes["value"].Value);
                }

                return 2;
            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
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

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetPercentTotalAmount)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
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

                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetUnmatchedCancel)
                        return Boolean.Parse(node.Attributes["value"].Value);
                }

                return false;

            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
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
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetUnmatchedWaitSeconds)
                        return Int16.Parse(node.Attributes["value"].Value);
                }

                return 60;
            }

            set
            {
                XmlNode rootNode = m_doc.ChildNodes[1];
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (node.Attributes["name"].Value == LayTheDraw.strFastBetUnmatchedWaitSeconds)
                        node.Attributes["value"].Value = value.ToString();
                }
            }
        }

        #endregion

        public int CheckServerBetsSeconds
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return 600;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strCheckServerBetsSeconds)
                            return Int16.Parse(node.Attributes["value"].Value);
                    }
                    return 600;
                }
                catch (Exception)
                {
                    return 600;
                }
            }
        }

        public Boolean StrategyActivated
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return false;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strStrategyActivated)
                            return Boolean.Parse(node.Attributes["value"].Value);
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public int UnmatchedWaitSeconds
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return 600;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strUnmatchedWaitSeconds)
                            return Int16.Parse(node.Attributes["value"].Value);
                    }
                    return 600;
                }
                catch (Exception)
                {
                    return 600;
                }
            }
        }

        public int DontCloseRetrySeconds
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return 60;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strDontCloseRetrySeconds)
                            return Int16.Parse(node.Attributes["value"].Value);
                    }
                    return 60;
                }
                catch (Exception)
                {
                    return 60;
                }
            }
        }

        public double DontCloseOdds
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return 0.5;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strDontCloseOdds)
                            return Double.Parse(node.Attributes["value"].Value);
                    }
                    return 0.5;
                }
                catch (Exception)
                {
                    return 0.5;
                }
            }
        }

        public int GoalScoredCloseTradeSeconds
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return 30;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strGoalScoredCloseTradeSeconds)
                            return Int16.Parse(node.Attributes["value"].Value);
                    }
                    return 30;
                }
                catch (Exception)
                {
                    return 30;
                }
            }
        }

        public int CheckExitOddsSeconds
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return 30;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strCheckExitOddsSeconds)
                            return Int16.Parse(node.Attributes["value"].Value);
                    }
                    return 30;
                }
                catch (Exception)
                {
                    return 30;
                }
            }
        }

        public double ExitCloseOdds
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return 1.8;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strExitCloseOdds)                            
                            return Double.Parse(node.Attributes["value"].Value);
                    }
                    return 1.8;
                }
                catch (Exception)
                {
                    return 1.8;
                }
            }
        }

        public int ExitWatchActivationPlaytime
        {
            get
            {
                if (m_doc == null)
                {
                    //Defaultwert zurückgeben
                    return 80;
                }

                try
                {
                    XmlNode rootNode = m_doc.ChildNodes[1];
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Attributes["name"].Value == LayTheDraw.strExitWatchActivationPlaytime)
                            return Int16.Parse(node.Attributes["value"].Value);
                    }
                    return 80;
                }
                catch(Exception)
                {
                    return 80;
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

                return false;

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
                return 50;
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
                return true;

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
                return 50;
            }
        }

    }
}
