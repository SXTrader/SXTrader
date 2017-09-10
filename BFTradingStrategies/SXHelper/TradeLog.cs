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
    public class TradeLog :IDisposable
    {
        private static volatile TradeLog instance;
        private static object _logLock = "tradelog";

        private bool _logEnabled = false;
        private bool _logBetAmounts = false;

        private System.Timers.Timer _cleanupTimer;

        private TradeLog() 
        { 
            //Alte Logs aufräumen
            //Überprüfe, ob log-Verzeichnis existiert.            
            String logPath = SXDirs.LogPath +  @"trades\";
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

                _cleanupTimer = new System.Timers.Timer(new TimeSpan(24, 0, 0).TotalMilliseconds);
                _cleanupTimer.Elapsed += _cleanupTimer_Elapsed;
                _cleanupTimer.Start();
                
            }
            catch { }
        }

        void _cleanupTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //Alte Logs aufräumen
                //Überprüfe, ob log-Verzeichnis existiert.            
                String logPath = SXDirs.LogPath + @"trades\";
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

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



        public static TradeLog Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_logLock)
                    {
                        if (instance == null)
                            instance = new TradeLog();
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

        public Boolean LogBetAmount
        {
            set
            {
                _logBetAmounts = value;
            }

            get
            {
                return _logBetAmounts;
            }
        }

        public void writeBetAmountLog(string match, string trademodul, string submodul, string message)
        {
            if (!_logBetAmounts)
                return;

            writeLog(match, trademodul, submodul, message);
        }

        public void writeLog(string match, string trademodul, string submodul, string message)
        {
            if (!_logEnabled)
                return;

            try
            {
                XDocument logDoc = null;
                //Name umformatieren.
                match = match.Replace('-', '_');
                match = match.Replace('.', '_');
                match = match.Replace(' ', '_');
                match = match.Replace(':', '_');

                trademodul = trademodul.Replace('-', '_');
                trademodul = trademodul.Replace('.', '_');
                trademodul = trademodul.Trim();
                trademodul = trademodul.Replace(' ', '_');
                trademodul = trademodul.Replace(':', '_');

                //_logLock = match;
                lock (_logLock)
                {

                    String logPath = SXDirs.LogPath +  @"trades\";
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

                    //Überprüfen, ob Trademodulknoten schon vorhanden
                    XElement trademoduleNode = root.Element(trademodul);
                    if (trademoduleNode == null)
                    {
                        trademoduleNode = new XElement(trademodul);
                        root.Add(trademoduleNode);
                    }

                    //Überprüfen, ob Submodulknoten schon vorhanden
                    XElement submodulNode = trademoduleNode.Element(submodul);
                    if (submodulNode == null)
                    {
                        submodulNode = new XElement(submodul);
                        trademoduleNode.Add(submodulNode);
                    }



                    //Nachricht protokollieren
                    XElement messageNode = new XElement("message", message);
                    messageNode.SetAttributeValue("dts", DateTime.Now.ToString());
                    submodulNode.Add(messageNode);

                    logDoc.Save(logFile);                   
                }
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteException(e);
            }
        }

        public void Dispose()
        {
            if (_cleanupTimer != null)
            {
                _cleanupTimer.Dispose();
            }
        }
    }
}
