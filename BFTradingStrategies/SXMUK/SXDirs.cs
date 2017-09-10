using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace net.sxtrader.muk
{
    public static class SXDirs
    {
        private static string _sxApplPath;
        private static string _sxApplLocalStoragePath;
        private static string _sxApplConfigPath;
        private static string _sxUsrPath;
        private static string _sxUsrLogPath;
        private static string _sxUsrTemplatePath;        

        public static String ApplPath { get { return _sxApplPath; } }
        public static String LogPath { get { return _sxUsrLogPath; } }
        public static String TemplatePath { get { return _sxUsrTemplatePath; } }
        public static String LocalStoragePath { get { return _sxApplLocalStoragePath; } }
        public static String UsrPath { get { return _sxUsrPath; } }
        public static String CfgPath{get{return _sxApplConfigPath;}}


        static SXDirs()
        {
            try
            {
                // Verzeichnis für SXTrader-Zusatzdateien wie mapping, etc. Alles was eigentlich nicht vom
                // User direkt angefasst werden soll!
                String localApplPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                _sxApplPath = localApplPath + @"\SXTrader\";
                _sxApplLocalStoragePath = _sxApplPath + @"localDataStorage\";
                _sxApplConfigPath = _sxApplPath + @"configurations\";

                if (!Directory.Exists(_sxApplPath))
                {
                    Directory.CreateDirectory(_sxApplPath);
                }

                if (!Directory.Exists(_sxApplLocalStoragePath))
                {
                    Directory.CreateDirectory(_sxApplLocalStoragePath);
                }

                if (!Directory.Exists(_sxApplConfigPath))
                {
                    Directory.CreateDirectory(_sxApplConfigPath);
                }

                // Verzeichnis für Userdatein, wie logs oder templates
                String myDocDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                _sxUsrPath = myDocDir + @"\SXTrader\";
                _sxUsrLogPath = _sxUsrPath + @"logs\";
                _sxUsrTemplatePath = _sxUsrPath + @"templates\";

                if (!Directory.Exists(_sxUsrPath))
                {
                    Directory.CreateDirectory(_sxUsrPath);
                }

                if (!Directory.Exists(_sxUsrLogPath))
                {
                    Directory.CreateDirectory(_sxUsrLogPath);
                }

                if (!Directory.Exists(_sxUsrTemplatePath))
                {
                    Directory.CreateDirectory(_sxUsrTemplatePath);
                }

            }
            catch (Exception e)
            {
                /*
                ExceptionWriter.Instance.WriteMessage("MAIN", "SXTrader will be closed due to an Exception!");
                ExceptionWriter.Instance.WriteException(e);
                 */
            }
        }

    }
}
