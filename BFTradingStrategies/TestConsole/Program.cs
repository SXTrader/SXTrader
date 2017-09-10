using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using net.sxtrader.bftradingstrategies.LayThe4;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Collections;
//using net.sxtrader.bftradingstrategies.BFLTDScratch;

using System.IO;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;


using System.Threading;
//using net.sxtrader.bftradingstrategies.bfuestrategy.MailInterface;
//using net.sxtrader.bftradingstrategies.bfuestrategy.DataModel;


namespace TestConsole
{
   

    class Program
    {
        static void Main(string[] args)
        {
            DateTime dts = new DateTime(2014, 07, 24, 20, 15, 00);

            Console.WriteLine(dts.Subtract(DateTime.Now).TotalMinutes);
            /*
            LTDMail mail = new LTDMail();
            
            BetgreenLTDTippList theList = mail.getBetgreenLTDTipps("mail.sxtrader.net", 143, false, "donation@sxtrader.net", "pxZwF3g!hrq)");

            foreach (BetgreenLTDTipp t in theList)
            {
                Console.WriteLine(t.ToString());
            }

            
            Console.ReadLine();
             */
            //Console.WriteLine(response);
        }

        private static String doWebRequest(String url)
        {
            String strResponse = String.Empty;
            // Prepare web request...
            System.Net.HttpWebRequest myRequest =
                  (System.Net.HttpWebRequest)WebRequest.Create(url);

            
            myRequest.Headers.Add("Cookie", "__utma=157630119.845815891404136000.1232887443.1232887443.1232887443.1; __utma=124735641.1120100038319093200.1246803041.1246803041.1246803041.1; __utmz=124735641.1246803041.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)");
            myRequest.KeepAlive = true;

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
            return strResponse;

            //return null;
        }
    }
}
