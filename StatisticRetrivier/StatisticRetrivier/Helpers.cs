using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StatisticRetrivier
{
    class Helpers
    {
        public static async Task<String> DoWebRequest(string url)
        {
            String strResponse = String.Empty;
            HttpWebRequest req =  (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)await req.GetResponseAsync();

            
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
        }
    }
}
