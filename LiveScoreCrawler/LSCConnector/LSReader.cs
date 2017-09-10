using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace LSCConnector
{
    public class LSReader
    {
        private const String DATAURL = "http://ctc.live.7m.cn/datafile/fen.js"; //"http://en1.7m.cn/datafile/fen.js";
        private const String DIRECTENCOUNTERURL = "http://data2.7m.cn/analyse/en/{0}.js"; //Parameter ist die Begegnungsid
        private const String LATESTENCOUNTRURL = "http://data2.7m.cn/team_data/{0}/en/history_dt.js"; //Parameter ist die Mannschaftsid
        private const String GOALDATAURL = "http://data2.7m.cn:13000/goaldata/en/{0}.js"; //Parameter is die Begegnungsid

        // Feld d_lx gibt art des Ereignis an 0 = TOR;1=Elfmetertor;2=Eigentor?; 3=Gelbe Karte; 4 = Rote Karte; 5 = Gelb/Rot?
        // Feld d_sx gibt an welches Team das Ereignis hatte: 0 = TeamA; -1 = TeamB
        // Feld d_pn gibt den Spielernamen an, auf den sich das Ereignis bezieht
        // Feld d_pi gibt die Spieler-ID an
        // Felder d_ai und d_bi sind die Team-Ids

        public String getGoaldData(ulong matchId)
        {
            return doWebRequest(String.Format(GOALDATAURL, matchId));
        }

        public String getLatestEncounterForTeam(ulong teamId)
        {
            return doWebRequest(String.Format(LATESTENCOUNTRURL, teamId));
        }

        public String getLastDirectEncounter(ulong currentMatchId)
        {
            return doWebRequest(String.Format(DIRECTENCOUNTERURL, currentMatchId));
        }

        public String readData()
        {
            return doWebRequest(DATAURL);
        }

        private static String doWebRequest(String url)
        {
            String strResponse = String.Empty;
            try
            {
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
            }
            catch (Exception ex)
            {

            }
            return strResponse;

            //return null;
        }
    }
}
