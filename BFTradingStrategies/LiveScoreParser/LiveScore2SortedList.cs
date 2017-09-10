using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Xml;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;
using System.Globalization;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    class LiveScore2SortedList  : SortedList<int, IScore>, IBFTSCommon
    {        
        private static String m_updateXMLFile = String.Empty;
        private DateTime _lastChangeRequest = DateTime.MinValue;
      /*
        private LiveScore2SortedList(SortedList list)
        {
            this. = list;
            
        }
       
        public void clear()
        {
            if (m_list != null)
            {
                m_list.Clear();
            }
        }

        public bool removeLiveticker(int scoreId)
        {
            try
            {
                m_list.Remove(scoreId);
                return true;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

            return false;
        }

        public bool addNewLiveticker(int scoreId, IScore livescore)
        {
            try
            {
                if(!m_list.ContainsKey(scoreId))
                    m_list.Add(scoreId, livescore);
                return true;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return false;            
        }
        */
        public IScore injectBetfair(String strBFTeamA, String strBFTeamB)
        {
            XmlDocument doc = new XmlDocument();
            String uri = String.Empty;
            bool bSave = false;
            String teamA = String.Empty;
            String teamB = String.Empty;


            XmlDocument m_docLocalChangeDate = new XmlDocument();
            XmlDocument m_docNetChangeDate = new XmlDocument();
            DateTime dtsStart = DateTime.Now;
            try
            {
                if(DateTime.Now.Subtract(_lastChangeRequest).TotalHours < 2.0)
                {
                    uri = SXDirs.ApplPath + @"\BFLSMapping2.xml";
                }
                else
                {
                    _lastChangeRequest = DateTime.Now;
                    //jetzt Datei aus dem Netz lesen
                    m_docNetChangeDate.Load("http://www.sxtrader.net/mappingchangedate2.xml");
                    //jetzt Lokale Datumsdatei lesen
                    m_docLocalChangeDate.Load(SXDirs.ApplPath + @"\mappingchangedate2.xml");
                    //Änderungsdatum der lokalen Datei
                    DateTime dtsLocal = new DateTime(long.Parse(m_docLocalChangeDate.ChildNodes[1].InnerText, CultureInfo.InvariantCulture));


                    DateTime dtsNet = new DateTime(long.Parse(m_docNetChangeDate.ChildNodes[1].InnerText, CultureInfo.InvariantCulture));

                    // Falls Datei aus dem Netz aktueller ist => Mappingdatei nachlesen, ansonsten alte Versionb beibehalten
                    if (dtsNet.Ticks > dtsLocal.Ticks)
                    {
                        uri = "http://www.sxtrader.net/BFLSMapping2.xml";                        
                        bSave = true;
                    }
                    else
                    {
                        uri = SXDirs.ApplPath + @"\BFLSMapping2.xml";
                    }

                }

            }
            catch (System.IO.FileNotFoundException)
            {
                // keine Lokale Änderungsdelta vorhanden
                // Also alles aus dem Netz lesen
                uri = "http://www.sxtrader.net/BFLSMapping2.xml";
                bSave = true;
            }
            catch (Exception)
            {
                uri = "http://www.sxtrader.net/BFLSMapping2.xml";
                bSave = true;
            }

            try
            {
                //XPathDocument xpathDoc = new XPathDocument(uri);
                if (bSave)
                {
                    // Mapping speichern
                    doc = new XmlDocument();
                    doc.Load(uri);
                    doc.Save(SXDirs.ApplPath + @"\BFLSMapping2.xml");

                    //Datumsdatei speichern
                    m_docNetChangeDate.Save(SXDirs.ApplPath + @"\mappingchangedate2.xml");
                }

                IScore score = injectBetfairIntern(strBFTeamA, strBFTeamB, uri);
                if (score == null)
                {
                    uri = SXDirs.ApplPath + @"\BFLSLocalMapping2.xml";
                    score = injectBetfairIntern(strBFTeamA, strBFTeamB, uri);
                }
                return score;
            }
            catch (NullReferenceException nre)
            {
                ExceptionWriter.Instance.WriteException(nre);
            }
            finally
            {
                m_docLocalChangeDate = null;
                m_docNetChangeDate = null;
                doc = null;
                DateTime dtsEnd = DateTime.Now;
                DebugWriter.Instance.WriteMessage("Liveticker 2", String.Format("Time need to search mapping for match {0} - {1}: {2}", strBFTeamA, strBFTeamB, dtsEnd.Subtract(dtsStart))); 
            }
            return null;
        }

        private IScore injectBetfairIntern(String strBFTeamA, String strBFTeamB, String uri)
        {
            XDocument feed = null;
            try
            {                
              
                feed = XDocument.Load(uri);
              
              

                var mappingTeamA = from map in feed.Descendants("Map")
                                   where map.Attribute("betfair").Value.Trim() == strBFTeamA.Trim()
                                   select new
                                   {
                                       Betfair = map.Attribute("betfair").Value.Trim(),
                                       Futbol24 = map.Attribute("livescore").Value.Trim()
                                   };

                var mappingTeamB = from map in feed.Descendants("Map")
                                   where map.Attribute("betfair").Value.Trim() == strBFTeamB.Trim()
                                   select new
                                   {
                                       Betfair = map.Attribute("betfair").Value.Trim(),
                                       Futbol24 = map.Attribute("livescore").Value.Trim()
                                   };

                foreach (var mapA in mappingTeamA)
                {
                    foreach (var mapB in mappingTeamB)
                    {
                       // IDictionaryEnumerator enumLivescore = (IDictionaryEnumerator)this.GetEnumerator();

                        for (int i = 0; i < this.Count;i++ )
                            //while (enumLivescore.MoveNext() == true)
                        {
                            IScore score = this.Values[i];//(LiveScore2)enumLivescore.Value;
                            try
                            {
                                if (score.TeamA.Equals(mapA.Futbol24.Trim()) && score.TeamB.Equals(mapB.Futbol24.Trim()))
                                {

                                    score.BetfairMatch = String.Format(CultureInfo.InvariantCulture, "{0} - {1}", strBFTeamA, strBFTeamB);
                                    return score.IncreaseRef();
                                }
                            }
                            catch (Exception exc)
                            {
                                ExceptionWriter.Instance.WriteException(exc);
                            }
                        }
                        /*
                        for (int i = 0; i < m_list.Count; i++)
                        {
                            LiveScore2 score = (LiveScore2)m_list.GetByIndex(i);
                            if (score.TeamA.Equals(mapA.Futbol24.Trim()) && score.TeamB.Equals(mapB.Futbol24.Trim()))
                            {

                                score.BetfairMatch = String.Format(CultureInfo.InvariantCulture, "{0} - {1}", strBFTeamA, strBFTeamB);
                                return score.IncreaseRef();
                            }
                        }
                         */
                    }

                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            finally
            {
                if (feed != null)
                    feed = null;
            }
            return null;
        }

        /*
        public void updateLiveScore(XmlDocument document)
        {
            XmlElement elem = document.DocumentElement;            

            XmlNodeList nodeList = elem.GetElementsByTagName("Mecze");

            if (nodeList.Count == 1)
            {
                XmlNode nodeMatches = nodeList[0];
                foreach (XmlNode node in nodeMatches.ChildNodes)
                {

                    XmlAttribute attribute = node.Attributes["MId"];
                    if (attribute != null)
                    {                        
                        LiveScore2 objLiveScore = (LiveScore2)m_list[Int32.Parse(attribute.Value,CultureInfo.InvariantCulture)];
                        if (objLiveScore != null)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(node.OuterXml);
                            objLiveScore.updateLivescore(doc);
                        }
                    }
                    
                }
            }
         

            
        }*/

        public static SortedList<int, IScore> buildList(String xmlString)
        {
            SortedList<int, IScore> list = new SortedList<int,IScore>();
            //LiveScore2SortedList objList = null;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            XmlElement elem = doc.DocumentElement;
            m_updateXMLFile = elem.Attributes["PlikXml"].Value;

            XmlNodeList nodeList = elem.GetElementsByTagName("Mecze");

            if (nodeList.Count == 1)
            {
                XmlNode nodeMatches = nodeList[0];
                foreach (XmlNode node in nodeMatches.ChildNodes)
                {
                    

                    LiveScore2 livescore = new LiveScore2(node);   
                    if(!livescore.Ended)
                        list.Add(livescore.ID, livescore);  
                }
            }
            //objList = new LiveScore2SortedList(list);
            
            return list;
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion


        public event EventHandler<SXWMessageEventArgs> MessageEvent;
    }
}
