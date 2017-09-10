using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Collections;
using System.Threading;
using net.sxtrader.muk;
using System.Xml;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.sxhelper
{
    public class FastBetLog
    {
        private static volatile FastBetLog instance;
        private static object _logLock = "fastbetlog";
        

        private const long MAXLENGTH = 1048576L;

        private bool _logEnabled = false;
        private ConcurrentQueue<LogEntry> _queue;
        //private Queue _queue;
        private Thread _writerThread;

        private FastBetLog() 
        { 
            //Alte Logs aufräumen
            //Überprüfe, ob log-Verzeichnis existiert.
                       

            String logPath = SXDirs.LogPath + @"fastbet\";
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            try
            {
                foreach (String fileName in Directory.GetFiles(logPath))
                {
                    if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        FileInfo fi = new FileInfo(fileName);

                        if(fi.Length > MAXLENGTH)
                            File.Delete(fileName);
                    }
                }

                _queue = new ConcurrentQueue<LogEntry>();
                _writerThread = new Thread(writeLogRunner);
                _writerThread.IsBackground = true;
                _writerThread.Start();
            }
            catch { }
        }

        public static FastBetLog Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_logLock)
                    {
                        if (instance == null)
                            instance = new FastBetLog();
                    }
                }

                return instance;
            }
        }

        public Boolean LogEnabled
        {
            set
            {
                _logEnabled = value;
            }
            get
            {
                return _logEnabled;
            }
        }

        public void writeLogRunner()
        {
            while (true)
            {
                Thread.Sleep(5000);
                String logFile = String.Empty;
              
                try
                {
                    XDocument logDoc = null;
                    // Überprüfe, ob log-Verzeichnis existiert.                      

                    String logPath = SXDirs.LogPath + @"fastbet\";
                    if (!Directory.Exists(logPath))
                        Directory.CreateDirectory(logPath);

                    LogEntry entry = null;
                    while (_queue.TryDequeue(out entry ))
                    {                        
                        //Überprüfe ob Datei existiert und nicht zu alt ist.
                        logFile = logPath + entry.FastBet + ".xml";
                        if (File.Exists(logFile))
                        {
                            FileInfo fi = new FileInfo(logFile);
                            if (fi.Length > MAXLENGTH)
                                File.Delete(logFile);
                            else
                                logDoc = XDocument.Load(logFile);
                        }

                        // Falls Datei noch nicht vorhandne eine neue Erzeugen
                        if (logDoc == null)
                        {
                            logDoc = new XDocument();
                            logDoc.Add(new XElement("log"));
                            logDoc.Save(logFile);
                        }

                        // Rootknoten lesen
                        XElement root = logDoc.Root;

                        //Überprüfen, ob Tickerknoten schon vorhandne
                        XElement tickerNode = root.Element(entry.Module);
                        if (tickerNode == null)
                        {
                            tickerNode = new XElement(entry.Module);
                            root.Add(tickerNode);
                        }

                        //Nachricht protokollieren
                        XElement messageNode = new XElement("message", entry.Message);
                        messageNode.SetAttributeValue("dts", DateTime.Now.ToString());
                        tickerNode.Add(messageNode);

                        logDoc.Save(logFile);
                    }
                        
                }
                catch (XmlException)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(logFile))
                            File.Delete(logFile);
                    }
                    catch (Exception) { }
                }
                catch (Exception)
                {
                
                }
            }
        }
        public void writeQueueRunner(Object stateInfo)
        {
            try
            {
                LogEntry entry = (LogEntry)stateInfo;
                _queue.Enqueue(entry);
                
            }
            catch
            {
            }
        }
        public async  void writeLog(string fastbet, string module, string message)
        {
            try
            {
                if (!_logEnabled)
                    return;


                LogEntry entry = new LogEntry(fastbet, module, message);

                Task.Run(() => { writeQueueRunner(entry); });
                //ThreadPool.QueueUserWorkItem(new WaitCallback(writeQueueRunner), entry);
            }
            catch
            {
            }
                        
        }

        class LogEntry
        {
            public String FastBet { get; set; }
            public String Module { get; set; }
            public String Message { get; set; }

            public LogEntry(String fastBet, String module, String message)
            {
                FastBet = fastBet;
                Module = module;
                Message = message;
            }
        }
    }
}
