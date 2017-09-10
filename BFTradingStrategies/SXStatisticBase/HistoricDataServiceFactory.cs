using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase
{
    public class HistoricDataServiceFactory
    {
        private static bool _assemblyLoaded = false;
        public static IHistoricDataService getInstance(ulong teamAId, ulong teamBId, String teamA, String teamB)
        {
            // Bei ersten Aufruf wird auf jedenfall über das Web abgefragt, damit die Assemblies für die spätere Deserialisierung 
            // geladen werden.
            if (_assemblyLoaded == false)
            {
                _assemblyLoaded = true;
                return new HistoricDataServicePHP();
            }

            String localDataStoragePath = SXDirs.LocalStoragePath;
            if (!Directory.Exists(localDataStoragePath))
            {
                Directory.CreateDirectory(localDataStoragePath);
                return new HistoricDataServicePHP();
            }

            String[] files = Directory.GetFiles(localDataStoragePath);
            foreach (String fileName in files)
            {
                if (File.GetCreationTime(fileName).Subtract(DateTime.Now).Days > 1)
                    File.Delete(fileName);
            }

            String localDataStorageName = localDataStoragePath + teamAId.ToString() + "_" + teamBId.ToString() + ".bin";

            if (!File.Exists(localDataStorageName))
            {
                return new HistoricDataServicePHP();
            }

            if (File.GetCreationTime(localDataStorageName).Subtract(DateTime.Now).Days > 7)
            {
                File.Delete(localDataStorageName);
                return new HistoricDataServicePHP();
            }

            return new HistoricDataServiceLocal();
            //return null;
        }
    }
}
