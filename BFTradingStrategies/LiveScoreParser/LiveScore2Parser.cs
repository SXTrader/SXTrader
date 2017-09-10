using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Net;
using System.IO;
using System.Threading;
using System.Xml;
using System.Diagnostics;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Globalization;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using System.Collections;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    public class LiveScore2Parser : IBFTSCommon, ILSParser, IDisposable
    {
        private static volatile LiveScore2Parser instance;
        private static Object syncRoot = new Object();
        private static Object synchUpdate = "Update";
        private LiveScore2SortedList m_scoreList = null;
        //
        //private Thread m_updateThread;
        private System.Timers.Timer _updateTimer;
        private System.Timers.Timer _cleanerTimer;
        private int m_waitTime = 5000;
        private bool _disposed = false;

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion

        #region ILSParser Member

        public event EventHandler<LiveScoreStateEventArgs> LiveScoreStateChangedEvent;
        public event EventHandler<LiveScoreCheckCountDownEventArgs> LiveScoreCheckCountDownEvent;
        public event EventHandler<LiveScoreAddedEventArgs> LiveScoreAddedEvent;
        public event EventHandler<LiveScoreRemovedEventArgs> LiveScoreRemovedEvent;

        public IScore linkSportExchange(String teamAName, String teamBName)
        {            
            return injectBetfair(teamAName, teamBName);
        }

        #endregion

        public static LiveScore2Parser Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new LiveScore2Parser();
                    }
                }

                return instance;
            }
        }

        public SortedList<int, IScore> LiveList
        {
            get
            {
                return m_scoreList;
            }
        }

        public int WaitTime
        {
            get
            {
                return m_waitTime;
            }
            set
            {
                m_waitTime = value * 1000;

                if (_updateTimer.Enabled)
                    _updateTimer.Stop();

                _updateTimer.Interval = m_waitTime;
                _updateTimer.Start();
            }
        }

         private LiveScore2Parser()
        {
            initialize();
             
            _cleanerTimer = new System.Timers.Timer(new TimeSpan(0, 30, 0).TotalMilliseconds);
            _cleanerTimer.Elapsed += new System.Timers.ElapsedEventHandler(_cleanerTimer_Elapsed);
            _cleanerTimer.Start();

             // Standardwert ist 10 Sekunden.
            _updateTimer = new System.Timers.Timer(10000);
            _updateTimer.AutoReset = false;
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Start();
        }

         private void _updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
         {
             lock (synchUpdate)
             {
                 updateThread();

                 EventHandler<LiveScoreCheckCountDownEventArgs> countDownHandler = LiveScoreCheckCountDownEvent;
                 if (countDownHandler != null)
                 {
                     countDownHandler(this, new LiveScoreCheckCountDownEventArgs("Livescore 2", m_waitTime));
                 }

                 _updateTimer.Interval = m_waitTime;
                 _updateTimer.Start();
             }
         }

         private void _cleanerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
         {
             try
             {
                 ArrayList ids = new ArrayList();
                 foreach (KeyValuePair<int, IScore> keyValue in m_scoreList)
                 {
                     IScore score = keyValue.Value;
                     //if (!score.Ended)
                     //    continue;

                     if (DateTime.Now.Subtract(score.StartDTS).TotalHours > 3)
                     {
                         ids.Add(keyValue.Key);
                        
                     }
                 }

                 foreach (int id in ids)
                 {
                     IScore score = m_scoreList[id];
                     DebugWriter.Instance.WriteMessage("Liveticker 2", String.Format("Remove match {0}", score.getLiveMatch()));
                     m_scoreList.Remove(id);

                     EventHandler<LiveScoreRemovedEventArgs> handlerRemoved = LiveScoreRemovedEvent;
                     if (handlerRemoved != null)
                     {
                         handlerRemoved(this, new LiveScoreRemovedEventArgs(score));
                     }
                 }
             }
             catch (Exception exc)
             {
                 ExceptionWriter.Instance.WriteException(exc);
             }
         }

         private IScore injectBetfair(String strBFTeamA, String strBFTeamB)
         {
             try
             {
                 return m_scoreList.injectBetfair(strBFTeamA, strBFTeamB);
             }
             catch (NullReferenceException nre)
             {
                 ExceptionWriter.Instance.WriteException(nre);
             }
             return null;
         }
         private async void initialize()
         {
             while (true)
             {
                 try
                 {
                     
                     DateTime dts = new DateTime(1970, 1, 1);

                     TimeSpan span = DateTime.Now.Subtract(dts);
                     String url = String.Format(CultureInfo.InvariantCulture, "http://futbol24.com/f24/u/liveNow.xml?Los={0}", span.TotalSeconds);
                     String response = String.Empty;

                     response = await SXTools.doWebRequest(url);

                     if (m_scoreList != null)
                     {
                         m_scoreList.Clear();
                         m_scoreList = null;
                     }
                     m_scoreList = new LiveScore2SortedList();

                     SortedList<int, IScore> myList = LiveScore2SortedList.buildList(response);

                     if (myList != null)
                     {
                         foreach (int id in myList.Keys)
                         {
                             if (myList[id].Ended)
                                 continue;

                             if (!m_scoreList.ContainsKey(id))
                             {
                                 m_scoreList.Add(id, myList[id]);
                             }
                             /*
                             EventHandler<LiveScoreAddedEventArgs> handlerAdded = LiveScoreAddedEvent;

                             Trace.WriteLine(String.Format("Liveticker 2: Retreived new ticker for match {0} - {1}", ticker.TeamA, ticker.TeamB));

                             if (handlerAdded != null)
                             {
                                 handlerAdded(this, new LiveScoreAddedEventArgs(ticker));
                             }
                              */
                         }
                     }

                     EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                     if (handler != null)
                     {
                         handler(this, new LiveScoreStateEventArgs("Livescore 2", true));
                     }

                     return;
                 }
                 catch (Exception exc)
                 {
                     EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                     if (handler != null)
                     {
                         handler(this, new LiveScoreStateEventArgs("Livescore 2", false));
                     }

                     ExceptionWriter.Instance.WriteException(exc);
                     return;
                     //Thread.Sleep(1000);
                 }
             }
         }

         private async void updateThread()
         {
            SortedList<int, IScore> localScoreList = null;
             try
             {
                 //mhe 09.11.2011. Neue Art der Abfrage nach Updates
                 DateTime dts = new DateTime(1970, 1, 1);
                 TimeSpan span = DateTime.Now.Subtract(dts);
                 String url = String.Format(CultureInfo.InvariantCulture, "http://futbol24.com/f24/u/liveNow.xml?Los={0}", span.TotalSeconds);
                 String response = String.Empty;

                 response = await SXTools.doWebRequest(url);

                 localScoreList = LiveScore2SortedList.buildList(response);

                 if (localScoreList != null)
                 {
                     foreach (int scoreId in localScoreList.Keys)
                     {
                         if (m_scoreList.ContainsKey(scoreId))
                         {
                             LiveScore2 localScore = (LiveScore2)localScoreList[scoreId];

                             // Es macht keinen Sinn Liveticker zu aktualisieren, die beendet sind.
                             if (localScore.Ended && m_scoreList[scoreId].Ended)
                                 continue;
                             (m_scoreList[scoreId] as LiveScore2).updateLivescore(
                                 localScore.getLivestate(), localScore.ScoreA, localScore.ScoreB, localScore.RedA, localScore.RedB, localScore.StartDTS);
                         }
                         else
                         {
                             // Es macht keinen Sinn Spiele hinzuzufügen, die beendet sind
                             if (localScoreList[scoreId].Ended)
                                 continue;

                             if (DateTime.Now.Subtract(localScoreList[scoreId].StartDTS).TotalHours > 3)
                             {
                                 //Diese Begegnungen sind in der Regel schon beendet, deswegen verzichten wir darauf diese aufzunehmen
                                 continue;
                             }

                             IScore score = localScoreList[scoreId];

                             m_scoreList.Add(scoreId, score);
                             DebugWriter.Instance.WriteMessage("Liveticker 2", String.Format("Liveticker 2: Retreived new ticker for match {0} - {1}", localScoreList[scoreId].TeamA, localScoreList[scoreId].TeamB));
                             DateTime dtsStart = DateTime.Now;
                             EventHandler<LiveScoreAddedEventArgs> handlerAdded = LiveScoreAddedEvent;
                             if (handlerAdded != null)
                             {
                                 handlerAdded(this, new LiveScoreAddedEventArgs(localScoreList[scoreId]));
                             }
                             DateTime dtsEnd = DateTime.Now;
                             DebugWriter.Instance.WriteMessage("Liveticker 2", String.Format("Time need to broadcast new ticker for match {0} - {1}: {2}", localScoreList[scoreId].TeamA, localScoreList[scoreId].TeamB, dtsEnd.Subtract(dtsStart)));
                         }
                     }
                 }
                 EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                 if (handler != null)
                 {
                     handler(this, new LiveScoreStateEventArgs("Livescore 2", true));
                 }

             }
             catch (XmlException xe)
             {

                 EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                 if (handler != null)
                 {
                     handler(this, new LiveScoreStateEventArgs("Livescore 2", false));
                 }

                 ExceptionWriter.Instance.WriteException(xe);
             }
             catch (Exception exc)
             {
                 EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                 if (handler != null)
                 {
                     handler(this, new LiveScoreStateEventArgs("Livescore 2", false));
                 }

                 ExceptionWriter.Instance.WriteException(exc);
             }
             finally
             {
                 if(localScoreList != null)
                 {
                     localScoreList.Clear();
                     localScoreList = null;
                 }
             }
         }

        /*
         private static String doWebRequest(String url)
         {
             String strResponse = String.Empty;
             // Prepare web request...
             System.Net.HttpWebRequest myRequest =
                   (System.Net.HttpWebRequest)WebRequest.Create(url);

             // Assign the response object of 'HttpWebRequest' to a 'HttpWebResponse' variable.
             HttpWebResponse myHttpWebResponse = (HttpWebResponse)myRequest.GetResponse();

             // Display the contents of the page to the console.
             Stream streamResponse = myHttpWebResponse.GetResponseStream();

             // Get stream object
             StreamReader streamRead = new StreamReader(streamResponse);

             Char[] readBuffer = new Char[8192];

             // Read from buffer
             int count = streamRead.Read(readBuffer, 0, 8192);

             while (count > 0)
             {
                 // get string
                 String resultData = new String(readBuffer, 0, count);

                 // Write the data
                 strResponse = strResponse + resultData;

                 // Read from buffer
                 count = streamRead.Read(readBuffer, 0, 8192);
             }
             // Release the response object resources.
             streamRead.Close();
             streamResponse.Close();

             // Close response
             myHttpWebResponse.Close();
             return strResponse;

             //return null;
         }
         * */

         public static void writeLocalXml(String match, IScore score)
         {

             String strFile = SXDirs.ApplPath + @"\BFLSLocalMapping2.xml"; ;

             String[] seps = { " - " };
             String[] teams = match.Split(seps, StringSplitOptions.RemoveEmptyEntries);
             XmlDocument doc = new XmlDocument();
             try
             {
                 doc.Load(strFile);
             }
             catch (System.IO.FileNotFoundException)
             {
                 //Datei nicht gefunden => erzeugen
                 XmlTextWriter writer = new XmlTextWriter(strFile, Encoding.UTF8);
                 writer.Formatting = Formatting.Indented;
                 writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                 writer.WriteStartElement("root");
                 writer.Close();
                 doc.Load(strFile);
             }

             writeWithLinq(teams[0], score.TeamA, strFile);
             writeWithLinq(teams[1], score.TeamB, strFile);
         }

         private static void writeWithLinq(String teamBetfair, String teamLivescore, String strFile)
         {
             XDocument feed = XDocument.Load(strFile);
             bool exists = false;

             var mappings = from map in feed.Descendants("Map")
                            where map.Attribute("betfair").Value == teamBetfair &&
                                  map.Attribute("livescore").Value == teamLivescore
                            select new
                            {
                                Betfair = map.Attribute("betfair").Value,
                                GamblerWiki = map.Attribute("livescore").Value
                            };
             foreach (var map in mappings)
             {
                 exists = true;
             }
             //Falls nichts vorhanden => neuen Eintrag anlegen
             if (!exists)
             {
                 XElement element = new XElement("Map");
                 element.SetAttributeValue("livescore", teamLivescore.Trim());
                 element.SetAttributeValue("betfair", teamBetfair.Trim());
                 feed.Element("root").Add(element);
                 feed.Save(strFile);
             }
         }

         public void Dispose()
         {
             Dispose(true);
         }

         protected virtual void Dispose(bool disposing)
         {
            if (!_disposed)
            {
                DebugWriter.Instance.WriteMessage("Livescore2Parser", "Disposing");
                if (disposing)
                {
                    if(_cleanerTimer != null)
                    {
                        _cleanerTimer.Stop();
                        _cleanerTimer.Dispose();
                    }

                    if(_updateTimer != null)
                    {
                        _updateTimer.Stop();
                        _updateTimer.Dispose();
                    }
                }

                if (m_scoreList != null)
                {
                    m_scoreList.Clear();
                }

                _disposed = true;
            }
         }


         public event EventHandler<SXWMessageEventArgs> MessageEvent;
    }
}
