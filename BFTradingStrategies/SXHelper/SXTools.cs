using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.sxhelper
{  
    public static class  SXTools
    {

        public static async Task<String> doWebRequest(String url)
        {
            String strResponse = String.Empty;
            // Prepare web request...
            System.Net.HttpWebRequest myRequest =
                  (System.Net.HttpWebRequest)WebRequest.Create(url);

            myRequest.Timeout = 60000;

            myRequest.UserAgent = String.Format("SXTrader {0}", Application.ProductVersion);

            // Assign the response object of 'HttpWebRequest' to a 'HttpWebResponse' variable.
            //HttpWebResponse myHttpWebResponse = (HttpWebResponse)myRequest.GetResponse();
            using (WebResponse myHttpWebResponse = await myRequest.GetResponseAsync())
            {
                // Display the contents of the page to the console.
                Stream streamResponse = myHttpWebResponse.GetResponseStream();

                // Get stream object
                StreamReader streamRead = new StreamReader(streamResponse);
                strResponse = streamRead.ReadToEnd();
                /*
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
                 **/
                // Release the response object resources.
                streamRead.Close();
                streamResponse.Close();

                // Close response
                myHttpWebResponse.Close();
            }
            return strResponse;

            //return null;
        }
        /*
        public static decimal getValidOddIncrement(decimal odds)
        {
            decimal incr =(decimal) 0.01;
            if (odds >= 100)
            {
                // Erlaubte Werte sind 10er Schritte
                incr = 10;
            }
            else if (odds >= 50 && odds < 100)
            {
                // Erlaubte Werte sind 5er Schritte
                incr = 5;
            }
            else if (odds >= 30 && odds < 50)
            {
                // Erlaubte Werte sind 2er Schritte
                incr = 2;
            }
            else if (odds >= 20 && odds <= 30)
            {
                //Erlaubte Werte sind 1er Schritte
                incr = 1;
            }
            else if (odds >= 10 && odds <= 20)
            {
                //Erlaubte Werte sind 0,5er Schritte
                incr = (decimal)0.5;
            }
            else if (odds >= 6 && odds < 10)
            {
                //Erlaubte Werte sind 0,2er Schritte
                incr = (decimal)0.2;
            }
            else if (odds >= 4 && odds < 6)
            {
                //Erlaubte Werte sind 0,1er Schritte
                incr = (decimal)0.1;
            }
            else if (odds >= 3 && odds < 4)
            {
                //Erlaubte Werte sind 0,05er Schritte
                incr = (decimal)0.05;
            }
            else if (odds >= 2 && odds < 3)
            {
                //Erlaubte Werte sind 0,02er Schritte
                incr = (decimal)0.02;
            }
            return incr;
        }
        public static decimal validateOdd(decimal odds)
        {
            if (odds >= 100)
            {
                // Erlaubte Werte sind 10er Schritte
                if (odds % 10 != 0)
                {
                    int result = (int)((odds / 10) * 10);
                    odds = result;
                }
            }
            else if (odds >= 50 && odds < 100)
            {
                // Erlaubte Werte sind 5er Schritte
                if (odds % 5 != 0)
                {
                    int result = (int)((odds / 5) * 5);
                    odds = result;
                }
            }
            else if (odds >= 30 && odds < 50)
            {
                // Erlaubte Werte sind 2er Schritte
                if (odds % 2 != 0)
                {
                    int result = (int)((odds / 2) * 2);
                    odds = result;
                }
            }
            else if (odds >= 20 && odds <= 30)
            {
                //Erlaubte Werte sind 1er Schritte
                int result = (int)((odds / 1) * 1);
                odds = result;
            }
            else if (odds >= 10 && odds <= 20)
            {
                //Erlaubte Werte sind 0,5er Schritte
                int result =(int)(odds * 100);
                if (result % 50 != 0)
                {
                    result = (result / 50) * 50;
                    odds = (decimal)result / 100;
                }
            }
            else if (odds >= 6 && odds < 10)
            {
                //Erlaubte Werte sind 0,2er Schritte
                int result = (int)(odds * 100);
                if (result % 20 != 0)
                {
                    result = (result / 20) * 20;
                    odds = (decimal)result / 100;
                }                
            }
            else if (odds >= 4 && odds < 6)
            {
                //Erlaubte Werte sind 0,1er Schritte
                int result = (int)(odds * 100);
                if (result % 10 != 0)
                {
                    result = (result / 10) * 10;
                    odds = (decimal)result / 100;
                }
            }
            else if (odds >= 3 && odds < 4)
            {
                //Erlaubte Werte sind 0,05er Schritte
                int result = (int)(odds * 1000);
                if (result % 50 != 0)
                {
                    result = (result / 50) * 50;
                    odds = (decimal)result / 1000;
                }
            }
            else if (odds >= 2 && odds < 3)
            {
                //Erlaubte Werte sind 0,02er Schritte
                int result = (int)(odds * 1000);
                if (result % 20 != 0)
                {
                    result = (result / 20) * 20;
                    odds = (decimal)result / 1000;
                }
            }
            return (decimal)odds;
        }
        */

    }

    public static class SXThreadStateChecker
    {
        public static bool isStartedBackground(Thread t)
        {
            bool ret = false;
            if (t.ThreadState == (System.Threading.ThreadState.Background | System.Threading.ThreadState.Running)
                    || t.ThreadState == (System.Threading.ThreadState.Background | System.Threading.ThreadState.WaitSleepJoin))
            {
                ret = true;
            }

            return ret;
        }
    }

   
}
