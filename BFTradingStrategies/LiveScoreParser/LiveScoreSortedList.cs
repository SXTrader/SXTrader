using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Xml.XPath;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Net;
using System.Globalization;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    public class LivetickerList : SortedList<int, IScore>, IBFTSCommon 
    {
        private DateTime _lastChangeRequest = DateTime.MinValue;

        //TODO: Logic für Inject-Betfair überarbeit, z.B. über ID, start Namensgleichheit gehen

        /// <summary>
        /// Verbindet eine Betfair-Begegnung mit einen Liveticker, sofern möglich
        /// </summary>
        /// <param name="strBFTeamA"></param>
        /// <param name="strBFTeamB"></param>
        /// <returns></returns>
        public IScore injectBetfair(String strBFTeamA, String strBFTeamB)
        {
            XmlDocument m_docLocalChangeDate = new XmlDocument();
            XmlDocument m_docNetChangeDate = new XmlDocument();
            XmlDocument doc = new XmlDocument();
            DateTime dtsStart = DateTime.Now;
            try
            {                
                String uri = String.Empty;
                bool bSave = false;
                String teamA = String.Empty;
                String teamB = String.Empty;

                try
                {
                    if (DateTime.Now.Subtract(_lastChangeRequest).TotalHours < 2.0)
                    {
                        uri = SXDirs.ApplPath + @"\BFLSMapping.xml";
                    }
                    else
                    {
                        _lastChangeRequest = DateTime.Now;
                        //jetzt Datei aus dem Netz lesen
                        m_docNetChangeDate.Load("http://www.sxtrader.net/mappingchangedate.xml");
                        //jetzt Lokale Datumsdatei lesen
                        m_docLocalChangeDate.Load(SXDirs.ApplPath + @"\mappingchangedate.xml");
                        //Änderungsdatum der lokalen Datei
                        DateTime dtsLocal = new DateTime(long.Parse(m_docLocalChangeDate.ChildNodes[1].InnerText, CultureInfo.InvariantCulture));


                        DateTime dtsNet = new DateTime(long.Parse(m_docNetChangeDate.ChildNodes[1].InnerText, CultureInfo.InvariantCulture));

                        // Falls Datei aus dem Netz aktueller ist => Mappingdatei nachlesen, ansonsten alte Versionb beibehalten
                        if (dtsNet.Ticks > dtsLocal.Ticks)
                        {
                            uri = "http://www.sxtrader.net/BFLSMapping.xml";
                            bSave = true;
                        }
                        else
                        {
                            uri = SXDirs.ApplPath + @"\BFLSMapping.xml";
                        }

                    }

                }
                catch (System.IO.FileNotFoundException)
                {
                    // keine Lokale Änderungsdelta vorhanden
                    // Also alles aus dem Netz lesen
                    uri = "http://www.sxtrader.net/BFLSMapping.xml";
                    bSave = true;
                }
                catch (WebException)
                {
                    uri = "http://www.sxtrader.net/BFLSMapping.xml";
                    bSave = true;
                }

                //XPathDocument xpathDoc = new XPathDocument(uri);
                if (bSave)
                {
                    // Mapping speichern
                    doc = new XmlDocument();
                    doc.Load(uri);
                    doc.Save(SXDirs.ApplPath + @"\BFLSMapping.xml");

                    //Datumsdatei speichern
                    m_docNetChangeDate.Save(SXDirs.ApplPath + @"\mappingchangedate.xml");
                }
                try
                {
                    IScore score = injectBetfairIntern(strBFTeamA, strBFTeamB, uri);
                    if (score == null)
                    {
                        uri = SXDirs.ApplPath + @"\BFLSLocalMapping.xml";
                        score = injectBetfairIntern(strBFTeamA, strBFTeamB, uri);
                    }
                    return score;
                }
                catch (NullReferenceException nre)
                {
                    ExceptionWriter.Instance.WriteException(nre);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            finally
            {                
                m_docLocalChangeDate = null;
                m_docNetChangeDate = null;
                doc = null;
                DateTime dtsEnd = DateTime.Now;
                DebugWriter.Instance.WriteMessage("Liveticker 1", String.Format("Time need to search mapping for match {0} - {1}: {2}", strBFTeamA, strBFTeamB, dtsEnd.Subtract(dtsStart))); 
            }

            return null;
        }

        private IScore injectBetfairIntern(String strBFTeamA, String strBFTeamB, String uri)
        {
            XDocument feed = null;
            try
            {
                try
                {
                    feed = XDocument.Load(uri);
                }
                catch
                {
                    return null;
                }

                var mappingTeamA = from map in feed.Descendants("Map")
                                   where map.Attribute("betfair").Value.Trim() == strBFTeamA.Trim()
                                   select new
                                   {
                                       Betfair = map.Attribute("betfair").Value.Trim(),
                                       GamblerWiki = map.Attribute("livescore").Value.Trim()
                                   };

                var mappingTeamB = from map in feed.Descendants("Map")
                                   where map.Attribute("betfair").Value.Trim() == strBFTeamB.Trim()
                                   select new
                                   {
                                       Betfair = map.Attribute("betfair").Value.Trim(),
                                       GamblerWiki = map.Attribute("livescore").Value.Trim()
                                   };
                try
                {
                    foreach (var mapA in mappingTeamA)
                    {
                        foreach (var mapB in mappingTeamB)
                        {
                            //IDictionaryEnumerator enumLivescore = (IDictionaryEnumerator)this.GetEnumerator();
                            //while (enumLivescore.MoveNext() == true)
                            for (int i = 0; i < this.Count; i++ )
                            {
                                IScore score = this.Values[i];//(LiveScore)enumLivescore.Value;
                                try
                                {
                                    if (score.TeamA.Equals(mapA.GamblerWiki.Trim()) && score.TeamB.Equals(mapB.GamblerWiki.Trim()))
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
                        }

                    }
                }
                catch (NullReferenceException nre)
                {
                    ExceptionWriter.Instance.WriteException(nre);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            finally
            {
                if (feed != null)
                {
                    feed = null;
                }
            }
            return null;
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion


        public event EventHandler<SXWMessageEventArgs> MessageEvent;
    }
 
 }

