using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.sxhelper
{
    public class ExceptionWriter : IDisposable
    {
        private static volatile ExceptionWriter instance;
        private static Object syncRoot = new Object();
        private const String FILENAME = "ExceptionOutput.txt";
        private static String EXCEPTIONFILE = SXDirs.UsrPath + FILENAME;

        private FileStream m_fs;
        StreamWriter m_streamWriter;
        private const long MAXLENGTH = 1048576L;

        public static ExceptionWriter Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ExceptionWriter();
                    }
                }

                return instance;
            }
        }

        private ExceptionWriter()
        {
            /*
            if (File.Exists(EXCEPTIONFILE))
            {
                FileInfo fileInfo = new FileInfo(EXCEPTIONFILE);
                if (fileInfo.Length > MAXLENGTH)
                {
                    try
                    {
                        File.Delete(EXCEPTIONFILE);
                    }
                    catch { }
                }
            }
            m_fs = new FileStream(EXCEPTIONFILE, FileMode.Append);
            m_streamWriter= new StreamWriter(m_fs); 
             */
        }

        public void WriteException(Exception e)
        {
            try
            {
                lock (syncRoot)
                {
                    if (File.Exists(EXCEPTIONFILE))
                    {
                        FileInfo fileInfo = new FileInfo(EXCEPTIONFILE);
                        if (fileInfo.Length > MAXLENGTH)
                        {
                            try
                            {
                                File.Delete(EXCEPTIONFILE);
                            }
                            catch { }
                        }
                    }
                    m_fs = new FileStream(EXCEPTIONFILE, FileMode.Append);
                    m_streamWriter = new StreamWriter(m_fs); 

                    if (m_streamWriter == null)
                        return;

                    m_streamWriter.WriteLine("{0}: Received Exception {1} to protocoll", DateTime.Now.ToString(), e.GetType().ToString());
                    foreach (DictionaryEntry de in e.Data)
                    {
                        m_streamWriter.WriteLine("DataItem: {0}", de.ToString());
                    }

                    m_streamWriter.WriteLine("Message: {0}", e.Message);
                    m_streamWriter.WriteLine("Source: {0}", e.Source);
                    m_streamWriter.WriteLine("Stacktrace: {0}", e.StackTrace);
                    m_streamWriter.WriteLine("Targetsite: {0}", e.TargetSite.ToString());
                    m_streamWriter.WriteLine();
                    m_streamWriter.WriteLine();
                    m_streamWriter.Flush();
                    m_streamWriter.Close();

                    m_fs.Close();
                }
            }
            catch (Exception)
            {
                // Da LowLevel-Funktion und für Funktion der SW nicht nötig=> Darf keine Exception werfen
                
            }
        }

        public void WriteMessage(String component, String message)
        {
            try
            {
                lock (syncRoot)
                {
                    if (File.Exists(EXCEPTIONFILE))
                    {
                        FileInfo fileInfo = new FileInfo(EXCEPTIONFILE);
                        if (fileInfo.Length > MAXLENGTH)
                        {
                            try
                            {
                                File.Delete(EXCEPTIONFILE);
                            }
                            catch { }
                        }
                    }
                    m_fs = new FileStream(EXCEPTIONFILE, FileMode.Append);
                    m_streamWriter = new StreamWriter(m_fs); 

                    if (m_streamWriter == null)
                        return;

                    m_streamWriter.WriteLine("{0}: Received message from component {1} to protocoll", DateTime.Now.ToString(), component);
                    m_streamWriter.WriteLine("Message: {0}", message);

                    m_streamWriter.WriteLine();
                    m_streamWriter.WriteLine();
                    m_streamWriter.Flush();
                    m_streamWriter.Close();

                    m_fs.Close();
                }
            }
            catch (Exception)
            {
                // Da LowLevel-Funktion und für Funktion der SW nicht nötig=> Darf keine Exception werfen
                
            }
        }

        public void Write(String component, String message, String stacktrace)
        {
            try
            {
                lock (syncRoot)
                {
                    if (m_streamWriter == null)
                        return;

                    m_streamWriter.WriteLine("{0}: Received message from component {1} to protocoll", DateTime.Now.ToString(), component);
                    m_streamWriter.WriteLine("Message: {0}", message);
                    m_streamWriter.WriteLine("StackTrace: {0}", stacktrace);

                    m_streamWriter.WriteLine();
                    m_streamWriter.WriteLine();
                    m_streamWriter.Flush();
                }
            }
            catch (Exception)
            {
                // Da LowLevel-Funktion und für Funktion der SW nicht nötig=> Darf keine Exception werfen
                
            }
        }

        #region IDisposable Member

        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);

            //throw new NotImplementedException();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_fs != null)
                    m_fs.Dispose();

                if (m_streamWriter != null)
                    m_streamWriter.Dispose();
            }
        }
        #endregion
    }
}
