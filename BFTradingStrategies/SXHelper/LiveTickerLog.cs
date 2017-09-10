using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.sxhelper
{
    public class LiveTickerLog
    {
        private static volatile LiveTickerLog instance;
        private static object _logLock = "livetickerlog";

        private bool _logEnabled = false;

        private LiveTickerLog() 
        { 
            //Alte Logs aufräumen
            //Überprüfe, ob log-Verzeichnis existiert.           

            String logPath = SXDirs.LogPath + @"liveticker\";
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            try
            {
                foreach (String fileName in Directory.GetFiles(logPath))
                {
                    if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTime.Now.Subtract(File.GetCreationTime(fileName)).Days >= 2)
                            //if (File.GetCreationTime(fileName).Subtract(DateTime.Now).Days > 2)
                            File.Delete(fileName);
                    }
                }
            }
            catch { }
        }

        public static LiveTickerLog Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_logLock)
                    {
                        if (instance == null)
                            instance = new LiveTickerLog();
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

        public void writeLog(string match, string ticker, string message)
        {
            if (!_logEnabled)
                return;

            try
            {               
                XDocument logDoc = null;
                //Name umformatieren.
                match = match.Replace('-', '_');
                match = match.Replace('.', '_');
                match = match.Replace('/', '_');
                

                //_logLock = match;
                lock (_logLock)
                {
                    // Überprüfe, ob log-Verzeichnis existiert.                  
                    String logPath = SXDirs.LogPath + @"liveticker\";
                    if (!Directory.Exists(logPath))
                        Directory.CreateDirectory(logPath);

                    //Überprüfe ob Datei existiert und nicht zu alt ist.
                    String logFile = logPath + match + ".xml";
                    if (File.Exists(logFile))
                    {
                        if (File.GetLastWriteTime(logFile).Subtract(DateTime.Now).Days > 2)
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
                    XElement tickerNode = root.Element(ticker);
                    if (tickerNode == null)
                    {
                        tickerNode = new XElement(ticker);
                        root.Add(tickerNode);
                    }

                    //Nachricht protokollieren
                    XElement messageNode = new XElement("message", message);
                    messageNode.SetAttributeValue("dts", DateTime.Now.ToString());
                    tickerNode.Add(messageNode);

                    logDoc.Save(logFile);                   
                }
            }
            catch (Exception)
            {
                //ExceptionWriter.Instance.WriteException(e);
            }
        }
    }
}
