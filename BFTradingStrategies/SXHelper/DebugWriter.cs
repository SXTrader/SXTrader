using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.sxhelper
{
    public class DebugWriter
    {
        private static volatile DebugWriter instance;
        private static Object syncRoot = new Object();

        private bool m_writeDebug = false;

        private const long MAXLENGTH = 1048576L;
        private const string FILENAME = "DebugOutput.txt";        
        private static String DEBUGFILE = SXDirs.UsrPath + FILENAME;


        public static DebugWriter Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DebugWriter();
                    }
                }

                return instance;
            }
        }

        public Boolean WriteDebugInfo
        {
            set
            {
                m_writeDebug = value;
            }
            get
            {
                return m_writeDebug;
            }
        }

        public void WriteMessage(String component, String message)
        {
            try
            {
                lock (syncRoot)
                {
                    // Nur schreiben, wenn Debug auch eingeschaltet ist
                    if (!m_writeDebug)
                        return;
                    FileInfo fileInfo = new FileInfo(DEBUGFILE);
                    FileStream fs;
                    StreamWriter streamWriter;
                    if (fileInfo.Exists)
                    {
                        if (fileInfo.Length > MAXLENGTH)
                        {
                            try
                            {
                                fileInfo.Delete();
                            }
                            catch { }
                        }
                    }
                    fs = new FileStream(DEBUGFILE, FileMode.Append);
                    streamWriter = new StreamWriter(fs);

                    streamWriter.WriteLine("{0} - {1}: {2}", DateTime.Now.ToString(), component, message);
                    streamWriter.Flush();
                    streamWriter.Close();
                    fs.Close();
                }
            }
            catch (Exception)
            {
                // Da LowLevel-Funktion und für Funktion der SW nicht nötig=> Darf keine Exception werfen
                
            }
        }
    }
}
