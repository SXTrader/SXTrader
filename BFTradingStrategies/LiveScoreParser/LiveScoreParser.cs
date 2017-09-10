using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Windows.Forms;
using System.Xml.XPath;
using System.Xml.Linq;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Collections;
using System.Globalization;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using System.Security;
using net.sxtrader.muk;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    public class LiveScoreParser : IBFTSCommon, ILSParser, IDisposable
    {
        private static volatile LiveScoreParser _instance;
        private static object _syncRoot = "livescoreParserLock";
        private object _syncUpdate = "livescoreUpdateLock";

        private int _flagnum = 0;
        private LivetickerList _livetickers;
        private int _waitTime;

        private Thread _buildRunner;
        System.Timers.Timer _updateTimer;
        private System.Timers.Timer _cleanerRunner;
        private bool _disposed = false;


        public static LiveScoreParser Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new LiveScoreParser();
                    }
                }

                return _instance;
            }
        }

        public int WaitTime
        {
            get
            {
                return _waitTime;
            }
            set
            {
                _waitTime = value;// *1000;
                if (_updateTimer != null)
                {
                    _updateTimer.Stop();
                    if (_waitTime != null)
                        _updateTimer.Interval = _waitTime * 1000;
                    else
                        _updateTimer.Interval = 30000;
                    _updateTimer.Start();
                }
            }
        }

        public LivetickerList LiveList
        {
            get
            {
                return _livetickers;
            }
        }

        private IScore injectBetfair(String strBFTeamA, String strBFTeamB)
        {
            return _livetickers.injectBetfair(strBFTeamA, strBFTeamB);
        }

        public static void WriteLocalXml(String match, IScore score)
        {

            String strFile = SXDirs.ApplPath + @"\BFLSLocalMapping.xml"; ;

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

            WriteWithLinq(teams[0], score.TeamA, strFile);
            WriteWithLinq(teams[1], score.TeamB, strFile);
        }

        private LiveScoreParser()
        {
            initialize();
        }

        private void initialize()
        {
            _livetickers = new LivetickerList();

            _buildRunner = new Thread(this.buildRunner);
            _buildRunner.IsBackground = true;
            _buildRunner.Start();
        }

        private static void WriteWithLinq(String teamBetfair, String teamLivescore, String strFile)
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
        
        /// <summary>
        /// Erstellt Liste mit Livetickern
        /// </summary>
        private async void buildRunner()
        {
            bool bFirstRun = true;
            
            while (true)
            {
                TimeSpan rerun = new TimeSpan(0, 30, 0);
                try
                {
                    
                    //String response = DoWebRequest("http://en.7m.cn/datafile/fen.js");
                    Task<String> t = SXTools.doWebRequest("http://ctc.7m.cn/datafile/fen.js");
                    t.Wait();
                    String response = t.Result;
                    //String response2 = DoWebRequest("http://en.7m.cn/datafile/csxl.js");
                    String response2 = await SXTools.doWebRequest("http://ctc.7m.cn/datafile/csxl.js");
                    //response = response.Replace("http://", String.Empty);
                    //response2 = response2.Replace("http://", String.Empty);
                    String remain = String.Empty;
                    int[] ids = LivescoreParserInternal.String1IdParser(response, out remain);

                    Segment1List segment1List = LivescoreParserInternal.String1DataParser(remain, ids);

                    String data2Tmp = LivescoreParserInternal.RemoveString2Unneccesary(response2);

                    _flagnum = LivescoreParserInternal.String2GetFlagnum(data2Tmp, out data2Tmp);
                    Segment2List segment2List = LivescoreParserInternal.String2DataParser(data2Tmp, ids);

                    foreach (int id in ids)
                    {
                        if (!segment1List.ContainsKey(id) || !segment2List.ContainsKey(id))
                        {
                            
                            continue;
                        }


                        if (!_livetickers.ContainsKey(id))
                        {
                            LiveScore ticker = new LiveScore(segment1List[id], segment2List[id]);

                            //Beendete Trades sind für uns uninteressant
                            if (ticker.Ended)
                                continue;

                            _livetickers.Add(id, ticker);

                            DebugWriter.Instance.WriteMessage("Liveticker 1", String.Format("Liveticker 1: Retreived new ticker for match {0} - {1}", ticker.TeamA, ticker.TeamB));

                            DateTime dtsStart = DateTime.Now;
                            EventHandler<LiveScoreAddedEventArgs> handlerAdded = LiveScoreAddedEvent;
                            if (handlerAdded != null)
                            {
                                handlerAdded(this, new LiveScoreAddedEventArgs(ticker));
                            }
                            DateTime dtsEnd = DateTime.Now;

                            DebugWriter.Instance.WriteMessage("Liveticker 1", String.Format("Time need to broadcast new ticker for match {0} - {1}: {2}", ticker.TeamA, ticker.TeamB, dtsEnd.Subtract(dtsStart)));
                        }
                    }

                    EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                    if (handler != null)
                    {
                        handler(this, new LiveScoreStateEventArgs("Livescore 1", true));
                    }

                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                    EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                    if (handler != null)
                    {
                        handler(this, new LiveScoreStateEventArgs("Livescore 1", false));
                    }
                    rerun = new TimeSpan(0, 1, 0);
                }

                if (bFirstRun)
                {
                    /*
                    _updateRunner = new Thread(this.updateRunner);
                    _updateRunner.IsBackground = true;
                    _updateRunner.Start();

                     */
                    _updateTimer = new System.Timers.Timer();
                    _updateTimer.Interval = 30000;
                    _updateTimer.AutoReset = false;
                    _updateTimer.Elapsed += _updateTimer_Elapsed;
                    _updateTimer.Start();

                    _cleanerRunner = new System.Timers.Timer(new TimeSpan(0, 30, 0).TotalMilliseconds);
                    _cleanerRunner.Elapsed += new System.Timers.ElapsedEventHandler(_cleanerRunner_Elapsed);
                    _cleanerRunner.Start();
                }

                Thread.Sleep(rerun);
            }
        }

        private void _updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateRunner();
            if (_waitTime == 0)
                _waitTime = 10;
            _updateTimer.Interval = _waitTime * 1000;

            TimeSpan span = new TimeSpan(0, 0, /*30*/_waitTime);
            EventHandler<LiveScoreCheckCountDownEventArgs> countDownHandler = LiveScoreCheckCountDownEvent;
            if (countDownHandler != null)
            {
                countDownHandler(this, new LiveScoreCheckCountDownEventArgs("Livescore 1", (int)span.TotalMilliseconds));
            }

            _updateTimer.Start();
        }

        /// <summary>
        /// Räumt die Liste der Liveticker auf
        /// </summary>
        private void _cleanerRunner_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {                    
                LivetickerList tmpList = new LivetickerList();

                foreach (KeyValuePair<int, IScore> keyValue in _livetickers)
                {
                    IScore score = keyValue.Value;
                    if (!score.Ended)
                        continue;

                    if (DateTime.Now.Subtract(score.StartDTS).TotalHours > 4)
                    {

                        tmpList.Add(keyValue.Key, keyValue.Value);
                    }
                }

                foreach (KeyValuePair<int, IScore> keyValue in tmpList)
                {
                    DebugWriter.Instance.WriteMessage("Liveticker 1", String.Format("Liveticker 1: Remove Ticker for match {0} - {1}", keyValue.Value.TeamA, keyValue.Value.TeamB));
                    _livetickers.Remove(keyValue.Key);


                    EventHandler<LiveScoreRemovedEventArgs> handlerRemoved = LiveScoreRemovedEvent;
                    if (handlerRemoved != null)
                    {
                        handlerRemoved(this, new LiveScoreRemovedEventArgs(keyValue.Value));
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        /// <summary>
        /// Aktualisiert die Liste der Spiele
        /// </summary>
        private async void updateRunner()
        {

            lock(_syncUpdate)
            {
                try
                {
                   
                    //Thread.Sleep(span);
                    Random rnd = new Random(1);
                    double number = rnd.NextDouble();
                    //String updateUrl = String.Format(CultureInfo.InvariantCulture, "http://en.7m.cn/livedata/sxl.js?c={0}", number.ToString(CultureInfo.InvariantCulture));
                    String updateUrl = String.Format(CultureInfo.InvariantCulture, "http://ctc.7m.cn/livedata/sxl.js?c={0}", number.ToString(CultureInfo.InvariantCulture));
                    String response =  SXTools.doWebRequest(/*"http://free.7m.cn:10001/livedata/sxl.js"*/updateUrl).Result;
                    
                    response = response.Replace("http://", String.Empty);
                    String[] splits = response.Split(new char[] { ':' });

                    //Flagnum extrahieren
                    splits[splits.Length - 1] = splits[splits.Length - 1].Replace("};", String.Empty);
                    int tmpFlagnum = 0;
                    if (Int32.TryParse(splits[splits.Length - 1], out tmpFlagnum))
                    {
                        if (tmpFlagnum - 1 > _flagnum)
                        {
                            // Aufholen
                            catchupFlagnum(ref tmpFlagnum);
                        }


                        if (tmpFlagnum == _flagnum + 1)
                        {
                            splits[splits.Length - 2] = LivescoreParserInternal.RemoveUpdateStringUnneccesary(splits[splits.Length - 2]);
                            if (splits[splits.Length - 2].Length == 0)
                                return;

                            String[] updateStrings = LivescoreParserInternal.BuildUpdateDataStrings(splits[splits.Length - 2]);
                            foreach (String str in updateStrings)
                            {
                                if (str.Trim().Length == 0)
                                    continue;
                                LivetickerSegment2 segment2 = LivescoreParserInternal.Segment2UpdateBuilder(str);

                                if (_livetickers.ContainsKey(segment2.ID))
                                {                                    
                                    (_livetickers[segment2.ID] as LiveScore).update(segment2);
                                }
                            }
                            _flagnum = tmpFlagnum;
                        }
                        // Ansonsten hat sich nichts geändert
                    }
                    EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                    if (handler != null)
                    {
                        handler(this, new LiveScoreStateEventArgs("Livescore 1", true));
                    }

                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                    EventHandler<LiveScoreStateEventArgs> handler = LiveScoreStateChangedEvent;
                    if (handler != null)
                    {
                        handler(this, new LiveScoreStateEventArgs("Livescore 1", false));
                    }
                }
            }
            
        }

        private void catchupFlagnum(ref int flagnum)
        {
            while (flagnum > _flagnum + 1)
            {
                

                int tmpFlagNum = _flagnum + 1;
                //_flagnum++;
                Random rnd = new Random(1);
                double number = rnd.NextDouble();
                String updateUrl = String.Format(CultureInfo.InvariantCulture, "http://en1.7m.cn/livedata/sxl_{1}.js?c={0}", number.ToString(CultureInfo.InvariantCulture), tmpFlagNum);
                String response = SXTools.doWebRequest(/*"http://free.7m.cn:10001/livedata/sxl.js"*/updateUrl).Result;
                if (response.Length == 0)
                {
                    _flagnum = tmpFlagNum;
                    continue;
                }
                //ACHTUNG: Splitted auch links innerhalb des streams. (http://);
                response = response.Replace("http://", String.Empty);
                String[] splits = response.Split(new char[] { ':' });
                splits[splits.Length - 2] = LivescoreParserInternal.RemoveUpdateStringUnneccesary(splits[splits.Length - 2]);
                if (splits[splits.Length - 2].Length == 0)
                {
                    _flagnum = tmpFlagNum;
                    continue;
                }

                String[] updateStrings = LivescoreParserInternal.BuildUpdateDataStrings(splits[splits.Length - 2]);
                _flagnum = tmpFlagNum;
                foreach (String str in updateStrings)
                {
                    if (str.Trim().Length == 0)
                        continue;
                    LivetickerSegment2 segment2 = LivescoreParserInternal.Segment2UpdateBuilder(str);

                    if (_livetickers.ContainsKey(segment2.ID))
                    {
                        (_livetickers[segment2.ID] as LiveScore).update(segment2);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        /*
        private static String DoWebRequest(String url)
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

        */
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

        private static class LivescoreParserInternal
        {
            private const String VARORD = ";var ORD = ";
            private const String VARSDT = "sDt[{0}]=[";
            private const String VARSDT2 = "sDt2[{0}]=[";
            private const String ARRAYCLOSE = "];";

            private const String VARSPLIT = "var SplitLive";// = 2; var GetFlagNum = 73352;
            private const String VARFLAGNUM = "var GetFlagNum =";

            private const String UPDATESTART = "[";
            private const String UPDATEEND = "],fn";

            private const String UPDATESTRINGSTART = "[";
            private const String UPDATESTRINGEND = "],";
            private const String UPDATESTRINGEND2 = "]";

            public static int[] String1IdParser(String tickerData1, out String remainData)
            {
                // Im kompletten Datenstring das Array mit den Spiel-Ids finden
                int ordpos = tickerData1.IndexOf(VARORD, StringComparison.OrdinalIgnoreCase);
                if (ordpos == -1)
                    throw new FormatException("Keine OrdPos");

                String posStrings = tickerData1.Substring(ordpos);

                // Identifiziere des Spiel-Id-Arrays entfernen
                posStrings = posStrings.Remove(0, VARORD.Length);

                // Array-Klammern entfernen
                posStrings = posStrings.Replace("[", String.Empty);
                posStrings = posStrings.Replace("]", String.Empty);

                // Abschliessendes Semicolon entfernen
                posStrings = posStrings.Replace(";", String.Empty);

                String[] strIds = posStrings.Split(new char[] { ',' });

                int[] ids = new int[strIds.Length];

                for (int i = 0; i < strIds.Length; i++)
                {
                    if (!Int32.TryParse(strIds[i], out ids[i]))
                    {
                        throw new FormatException();
                    }
                }

                remainData = tickerData1.Substring(0, ordpos);

                return ids;
            }

            public static Segment1List String1DataParser(String tickerData1, int[] ids)
            {
                
                Segment1List list = new Segment1List();
                
                foreach (int id in ids)
                {
                    String sdt = String.Format(CultureInfo.InvariantCulture,VARSDT, id);
                    int sdtstartpos = tickerData1.IndexOf(sdt, StringComparison.OrdinalIgnoreCase);
                    if (sdtstartpos == -1)
                        continue;

                    int sdtendpos = tickerData1.IndexOf(ARRAYCLOSE, sdtstartpos,StringComparison.OrdinalIgnoreCase);
                    if (sdtendpos == -1)
                        continue;

                    String sdtData = tickerData1.Substring(sdtstartpos, sdtendpos + ARRAYCLOSE.Length - sdtstartpos);
                    tickerData1 = tickerData1.Replace(sdtData, String.Empty);
                    sdtData = sdtData.Replace(sdt, String.Empty);
                    sdtData = sdtData.Replace(ARRAYCLOSE, String.Empty);

                    String[] arrData = sdtData.Split(new char[] { ',' });

                    //ggf. Hochkomma am anfang und Ende entfernen
                    arrData[0] = removeApostrophe(arrData[0]);
                    arrData[2] = removeApostrophe(arrData[2]);
                    arrData[3] = removeApostrophe(arrData[3]);

                    try
                    {
                        LivetickerSegment1 segment = new LivetickerSegment1(id, arrData[0], UInt64.Parse(arrData[9], CultureInfo.InvariantCulture),
                            UInt64.Parse(arrData[10], CultureInfo.InvariantCulture), arrData[2], arrData[3]);
                        list.Add(id, segment);
                    }
                    catch (Exception e)
                    {
                        ExceptionWriter.Instance.WriteException(e);
                        continue;
                    }
                }



                return list;
            }

            public static Segment2List String2DataParser(String tickerData2, int[] ids)
            {
                Segment2List list = new Segment2List();
                //String[] data2Splits = tickerData2.Split(new char[] { ';' });
                foreach (int id in ids)
                {
                    String sdt2 = String.Format(CultureInfo.InvariantCulture,VARSDT2, id);
                    int sdtstartpos = tickerData2.IndexOf(sdt2, StringComparison.OrdinalIgnoreCase);
                    if (sdtstartpos == -1)
                        continue;

                    int sdtendpos = tickerData2.IndexOf(ARRAYCLOSE, sdtstartpos, StringComparison.OrdinalIgnoreCase);
                    if (sdtendpos == -1)
                        continue;

                    String sdt2Data = tickerData2.Substring(sdtstartpos, sdtendpos + ARRAYCLOSE.Length - sdtstartpos);
                    tickerData2 = tickerData2.Replace(sdt2Data, String.Empty);
                    sdt2Data = sdt2Data.Replace(sdt2, String.Empty);
                    sdt2Data = sdt2Data.Replace(ARRAYCLOSE, String.Empty);
                    list.Add(id, segment2Builder(sdt2Data, id));

                }
                return list;
            }

            public static LivetickerSegment2 Segment2UpdateBuilder(String segmentData)
            {
                // ID extrahieren
                int commaPos = segmentData.IndexOf(',');
                if (commaPos == -1)
                    throw new FormatException("Keine ID in UpdateString");

                int id = 0;
                if (!Int32.TryParse(segmentData.Substring(0, commaPos), out id))
                    throw new FormatException("Kann Id aus UpdateString nicht erzeugen");

                segmentData = segmentData.Remove(0, commaPos + 1);
                return segment2Builder(segmentData, id);

            }

            private static LivetickerSegment2 segment2Builder(String segmentData, int id)
            {

                // SpielStatus
                int commaPos = segmentData.IndexOf(',');
                String gamestate = segmentData.Substring(0, commaPos);
                segmentData = segmentData.Remove(0, commaPos + 1);
                // Spielstand Team A
                commaPos = segmentData.IndexOf(',');
                String scoreA = segmentData.Substring(0, commaPos);
                segmentData = segmentData.Remove(0, commaPos + 1);
                // Spielstand Team B
                commaPos = segmentData.IndexOf(',');
                String scoreB = segmentData.Substring(0, commaPos);
                segmentData = segmentData.Remove(0, commaPos + 1);
                // Rote Karte Team A
                commaPos = segmentData.IndexOf(',');
                String redA = segmentData.Substring(0, commaPos);
                segmentData = segmentData.Remove(0, commaPos + 1);
                // Rote Karte Team B
                commaPos = segmentData.IndexOf(',');
                String redB = segmentData.Substring(0, commaPos);
                segmentData = segmentData.Remove(0, commaPos + 1);
                // Startzeit Halbzeit (muss nicht gefüllt sein
                String halfTimeDTS = String.Empty;
                if (segmentData.StartsWith("''", StringComparison.OrdinalIgnoreCase))
                {
                    commaPos = segmentData.IndexOf(',');
                    segmentData = segmentData.Remove(0, commaPos + "''".Length);
                }
                else
                {
                    commaPos = segmentData.IndexOf("',", StringComparison.OrdinalIgnoreCase);
                    halfTimeDTS = segmentData.Substring(0, commaPos);
                    segmentData = segmentData.Remove(0, commaPos + "',".Length);
                }

                // Halbzeitsstand
                commaPos = segmentData.IndexOf(',');
                String halfTimeScore = segmentData.Substring(0, commaPos);
                if (halfTimeScore.Equals("''", StringComparison.OrdinalIgnoreCase))
                    halfTimeScore = String.Empty;
                else
                    halfTimeScore = halfTimeScore.Replace("'", String.Empty);
                segmentData = segmentData.Remove(0, commaPos + 1);

                //Sonstiges
                if (segmentData.StartsWith("''",StringComparison.OrdinalIgnoreCase))
                {
                    commaPos = segmentData.IndexOf(',');
                    segmentData = segmentData.Remove(0, commaPos + "''".Length);
                }
                else
                {
                    commaPos = segmentData.IndexOf("',", StringComparison.OrdinalIgnoreCase);
                    segmentData = segmentData.Remove(0, commaPos + "',".Length);
                }

                // Startzeit
                String startTimeDTS = String.Empty;
                if (segmentData.StartsWith("''", StringComparison.OrdinalIgnoreCase))
                {
                    commaPos = segmentData.IndexOf(',');
                    segmentData = segmentData.Remove(0, commaPos + "''".Length);
                }
                else
                {
                    commaPos = segmentData.IndexOf("',", StringComparison.OrdinalIgnoreCase);
                    startTimeDTS = segmentData.Substring(0, commaPos);
                    segmentData = segmentData.Remove(0, commaPos + "',".Length);
                }

                return new LivetickerSegment2(id, gamestate, scoreA, scoreB, redA, redB, halfTimeDTS, halfTimeScore, startTimeDTS);

            }

            public static int String2GetFlagnum(String tickerData2, out String remain)
            {
                String tmp = String.Empty;

                int splitpos = tickerData2.IndexOf(VARFLAGNUM, StringComparison.OrdinalIgnoreCase);
                if (splitpos == -1)
                    throw new FormatException("Keine SplitPos");

                int splitendpos = tickerData2.IndexOf(";", splitpos, StringComparison.OrdinalIgnoreCase);
                if (splitendpos == -1)
                    throw new FormatException("Keine SplitEndPos");

                tmp = tickerData2.Substring(splitpos, splitendpos + 1 - splitpos);

                remain = tickerData2.Replace(tmp, String.Empty);

                tmp = tmp.Replace(VARFLAGNUM, String.Empty);
                tmp = tmp.Replace(";", String.Empty);

                return Int32.Parse(tmp, CultureInfo.InvariantCulture);
            }

            public static String[] BuildUpdateDataStrings(String updateString)
            {
                ArrayList updateStrings = new ArrayList();
                updateString = updateString.Trim();
                while (updateString.Length > 0)
                {
                    int startPos = updateString.IndexOf(UPDATESTRINGSTART, StringComparison.OrdinalIgnoreCase);
                    if (startPos == -1)
                        throw new FormatException("Keine Updatestring StartPos");

                    int endPos = updateString.IndexOf(UPDATESTRINGEND, StringComparison.OrdinalIgnoreCase);
                    int endPosLength = 0;
                    if (endPos == -1)
                    {
                        endPos = updateString.Length - 1;//updateString.IndexOf(UPDATESTRINGEND2);
                        if (endPos == -1)
                            throw new FormatException("Keine Updatestring EndPos");
                        else
                            endPosLength = UPDATESTRINGEND2.Length;
                    }
                    else
                    {
                        bool b = false;
                        while (updateString.Substring(endPos + UPDATESTRINGEND.Length, 1) != UPDATESTRINGSTART)
                        {
                            endPos = updateString.IndexOf(UPDATESTRINGEND, endPos + 1, StringComparison.OrdinalIgnoreCase);
                            if (endPos == -1)
                            {
                                endPos = updateString.Length - 1;
                                b = true;
                                break;
                            }
                        }
                        if (b)
                            endPosLength = UPDATESTRINGEND2.Length;
                        else
                            endPosLength = UPDATESTRINGEND.Length;
                    }


                    String tmp = updateString.Substring(startPos + UPDATESTRINGSTART.Length, endPos - startPos - endPosLength);
                    updateStrings.Add(tmp);
                    updateString = updateString.Remove(startPos, endPos + endPosLength - startPos);
                }

                return (String[])updateStrings.ToArray(typeof(string));
            }

            public static String RemoveUpdateStringUnneccesary(String updateString)
            {
                String tmp = String.Empty;

                int startpos = updateString.IndexOf(UPDATESTART, StringComparison.OrdinalIgnoreCase);
                if (startpos == -1)
                    throw new FormatException("Keine Update StartPos");

                int endpos = updateString.IndexOf(UPDATEEND, StringComparison.OrdinalIgnoreCase);
                if (endpos == -1)
                    throw new FormatException("Keine Update EndPos");

                tmp = updateString.Substring(startpos + UPDATESTART.Length, endpos - startpos - UPDATESTART.Length);

                return tmp;
            }

            public static String RemoveString2Unneccesary(String tickerData2)
            {
                String tmp = String.Empty;
                int splitpos = tickerData2.IndexOf(VARSPLIT, StringComparison.OrdinalIgnoreCase);
                if (splitpos == -1)
                    throw new FormatException("Keine SplitPos");

                int splitendpos = tickerData2.IndexOf(";", splitpos, StringComparison.OrdinalIgnoreCase);
                if (splitendpos == -1)
                    throw new FormatException("Keine SplitEndPos");

                tmp = tickerData2.Remove(splitpos, splitendpos + 1 - splitpos);

                return tmp;
            }

            private static String removeApostrophe(String value)
            {
                String tmp = value;
                if (tmp.StartsWith("'", StringComparison.OrdinalIgnoreCase))
                {
                    tmp = tmp.Substring(1);
                }

                if (tmp.EndsWith("'", StringComparison.OrdinalIgnoreCase))
                {
                    tmp = tmp.Substring(0, tmp.Length - 1);
                }
                return tmp;
            }
        }


        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;
        public event EventHandler<SXWMessageEventArgs> MessageEvent;
        #endregion

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DebugWriter.Instance.WriteMessage("LivescoreParser", "Disposing");
                if (disposing)
                {
                    if (_cleanerRunner != null)
                    {
                        _cleanerRunner.Stop();
                        _cleanerRunner.Dispose();
                    }

                    if (_updateTimer != null)
                    {
                        _updateTimer.Stop();
                        _updateTimer.Dispose();
                    }

                    if (_buildRunner != null)
                    {
                        try
                        {
                            _buildRunner.Abort();                            
                        }
                        catch (SecurityException)
                        {
                        }
                        catch (ThreadStateException)
                        {
                        }
                    }

                    if (_livetickers != null)
                    {
                        _livetickers.Clear();
                    }
                }

                _disposed = true;
            }
        }


       
    }
}
