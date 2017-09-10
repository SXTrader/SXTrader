using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.common
{
    public class HistoricDataSerializer
    {
        const int VERSION = 1;
        public const String SERIALIZEFILEFORMATSTRING = @"{2}\localDataStorage\{0}_{1}.bin";

        static HistoricDataSerializer()
        {
            //Alte Stats aufräumen
            //Überprüfe, ob Stats-Verzeichnis existiert.
            String statPath = Application.StartupPath + @"\localDataStorage\";
            if (!Directory.Exists(statPath))
                Directory.CreateDirectory(statPath);
           
            foreach (String fileName in Directory.GetFiles(statPath))
            {
                if (fileName.EndsWith(".bin"))
                {
                    if (DateTime.Now.Subtract(File.GetCreationTime(fileName)).Days >= 2)
                    {
                        try
                        {
                            File.Delete(fileName);
                        }
                        catch { }
                    }
                }
            }
        }

        public static void Save(HistoricDataStatistic statisticData, ulong idTeamA, ulong idTeamB)
        {
            Stream stream = null;
            try
            {                
                String fileName = String.Format(SERIALIZEFILEFORMATSTRING, idTeamA, idTeamB, Application.StartupPath);
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, VERSION);
                formatter.Serialize(stream, statisticData);
            }
            catch
            {
                // do nothing, just ignore any possible errors
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
        }

        public static HistoricDataStatistic Load(ulong idTeamA, ulong idTeamB)
        {
            Stream stream = null;
            HistoricDataStatistic statisticData = null;
            String fileName = String.Format(SERIALIZEFILEFORMATSTRING, idTeamA, idTeamB, Application.StartupPath);
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                int version = (int)formatter.Deserialize(stream);
                Debug.Assert(version == VERSION);
                statisticData = (HistoricDataStatistic)formatter.Deserialize(stream);
            }
            catch(Exception exc)
            {
                if (null != stream)
                    stream.Close();
                Trace.WriteLine(exc.Message);
                File.Delete(fileName);
                throw new NoHistoricDataException();
            }
            finally
            {
                if (null != stream)
                    stream.Close();
            }
            return statisticData;
        }


    }
}
