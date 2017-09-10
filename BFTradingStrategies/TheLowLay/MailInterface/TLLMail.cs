using ImapX;
using ImapX.Collections;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.tippsters.Configration;
using net.sxtrader.bftradingstrategies.tippsters.DataModel;
using net.sxtrader.muk.Exceptions.MailInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.tippsters.MailInterface
{
    public class TLLMails : IDisposable
    {
        TLLConfigurationRW _config;
        private bool _disposed = false;

        //IsolatedStorageFile _iso;
        public TLLMails()
        {
            _config = new TLLConfigurationRW();

            //_iso = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
            //    IsolatedStorageScope.Assembly, null, null);

        }

        public static void TestSettings(String host, int port, bool useSSL, string mailUser, string mailAccess )
        {
            try
            {
                ImapClient imapClient = new ImapClient(host, port, useSSL);
                imapClient.IsDebug = true;
                imapClient.Connect();
                if (!imapClient.Login(mailUser, mailAccess))
                    throw new IMAPLogInFailedException();
            }
            catch (Exception ie)
            {
                throw new GenericImapException(ie.Message, ie);
            }
        }

        public async Task<TLLTipp> getLOPTipp()
        {
            try
            {
                ImapClient imapclient = new ImapClient(_config.MailHost, _config.MailPort, _config.UseSSL);
                TLLTipp tipp = new TLLTipp();
                if (imapclient.Connect())
                {
                    if (imapclient.Login(_config.MailUser, _config.MailAccess))
                    {
                        FolderCollection foldercoll = imapclient.Folders;
                        String dts = DateTime.Now.Subtract(new TimeSpan(23, 59, 59)).ToString("d-MMM-yyyy", CultureInfo.InvariantCulture);
                        String searchQuery = String.Format("FROM \"{0}\" SINCE {1}", "thelowlay@gmail.com", dts);
                        //MessageCollection msgcoll = foldercoll["INBOX"].Search(searchQuery, true);
                        Message[] msgcoll = foldercoll["INBOX"].Search(searchQuery);
                        if (msgcoll.Count() == 0)
                        {
                            tipp.NoTippFound = true;
                            return tipp;
                        }


                        tipp = parseMails(msgcoll);
                        if (tipp != null)
                            translateTipp(tipp);
                        imapclient.Disconnect();
                    }
                    else
                    {
                        throw new IMAPLogInFailedException();
                    }

                }
                else
                {
                    throw new NoIMAPConnectionException();
                }
                return tipp;
            }
            catch (Exception ie)
            {
                ExceptionWriter.Instance.WriteException(ie);
                throw new GenericImapException(ie.Message, ie);
            }
        }


        private void translateTipp(TLLTipp tipp)
        {
            if (String.IsNullOrEmpty(tipp.Race))
                return;

            if (tipp.Race.Equals("Epsom Downs", StringComparison.InvariantCultureIgnoreCase))
                tipp.Race = "Epsom";
        }


        private TLLTipp parseMails(Message[] msgcoll)
        {
            ArrayList tipps = new ArrayList(msgcoll.Count());
            foreach (Message msg in msgcoll)
            {
                if (msg.Subject.Contains("No Selection") || msg.Subject.Contains("No LowLay Today"))
                {
                    TLLTipp tipp = new TLLTipp();
                    tipp.NoSelectionDay = true;
                    tipps.Add(tipp);
                    continue;
                }
                else if (!msg.Subject.Contains("Today’s LowLay") && !msg.Subject.Contains("Todays LowLay"))
                {
                    continue;
                }

                // Wir haben einen Tipp gefunden 
                // Inhalt parsen
                // TODO: Performantere Methode des parsens finden
                
                String[] content = msg.Body.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (content.Length == 0)
                    continue;

                int lineCounter = 0;
                foreach (String line in content)
                {
                    /*
                    if (!(line.Contains("Time") && line.Contains("Meeting") && line.Contains("Lay")))
                    {
                        lineCounter++;
                        continue;
                    }
                     * */
                    if (String.IsNullOrEmpty(line))
                    {
                        lineCounter++;
                        continue;
                    }

                    //Nächste Zeile Enthält die Daten
                    String dataLine = content[lineCounter + 1];
                    if (dataLine.Contains("No Selection"))
                    {
                        TLLTipp tipp = new TLLTipp();
                        tipp.NoSelectionDay = true;
                        tipps.Add(tipp);
                        break;
                    }

                    String[] tippData = dataLine.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                    if (tippData.Length != 4)
                        break;

                    try
                    {
                        TLLTipp foundTipp = new TLLTipp();

                        // Ersten String ist Datum + Renne
                        //String strDts = tippData[0].Substring(0, tippData[0].Trim().IndexOf(' '));
                        DateTime dts = msg.Date ?? DateTime.Now;
                        foundTipp.EventDate = convertTime(dts, tippData[0]);
                        foundTipp.Race = tippData[1];//.Substring(tippData[0].Trim().IndexOf(' ') + 1);//tippData[1].Trim();
                        foundTipp.Horse = tippData[2].Trim();

                        String maxOdds = tippData[3].Replace("Max", "").Trim();
                        foundTipp.MaxOdds = Double.Parse(maxOdds, CultureInfo.InvariantCulture);

                        tipps.Add(foundTipp);
                    }
                    catch (NoEventTimeException nete)
                    {
                        DebugWriter.Instance.WriteMessage("TLLMail", "Could not retrieve Event Time. Skipping Tipp");
                        ExceptionWriter.Instance.WriteException(nete);
                    }
                    catch (Exception exc)
                    {
                        DebugWriter.Instance.WriteMessage("TLLMail", "Received Unknown Exception. Skipping Tipp");
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                    break;
                }

            }
            if (tipps.Count == 0)
            {
                TLLTipp noEmailTipp = new TLLTipp();
                noEmailTipp.NoTippFound = true;
                return noEmailTipp;
            }

            return (TLLTipp)tipps[tipps.Count - 1];
        }

        private DateTime convertTime(DateTime mailDTS, String eventTime)
        {
            //Mailzeit ist immer LocalTimeZone und muss in GMT Standard umgewandelt werden

            //DateTime localTime = new DateTime(mailDTS.Year, mailDTS.Month, mailDTS.Day,
            String id = "GMT Standard Time";
            //String id = "Indochina Time";
            TimeZoneInfo tziUK;
            tziUK = TimeZoneInfo.FindSystemTimeZoneById(id);

            TimeZoneInfo tziLocal = TimeZoneInfo.Local;

            DateTime dtsMailGmt = TimeZoneInfo.ConvertTime(mailDTS, tziLocal, tziUK);

            //Jetzt aus dem String eventTime die Stunde und Minute extrahieren
            String[] timeFragments = eventTime.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (timeFragments.Length != 2)
                throw new NoEventTimeException();

            try
            {
                int eventGMTHour = Int32.Parse(timeFragments[0]) + 12; // In 24-Stunden-Format
                if (eventGMTHour == 24)
                    eventGMTHour = 12;
                int eventGMTMinute = Int32.Parse(timeFragments[1]);

                //Event Datum in GMT konstruieren
                DateTime dtsEvent = new DateTime(dtsMailGmt.Year, dtsMailGmt.Month, dtsMailGmt.Day, eventGMTHour, eventGMTMinute, 0);
                //Und in lokale Zeit konvertieren
                dtsEvent = TimeZoneInfo.ConvertTime(dtsEvent, tziUK, tziLocal);
                return dtsEvent;
            }
            catch (ArgumentNullException ane)
            {
                ExceptionWriter.Instance.WriteException(ane);
                throw new NoEventTimeException();
            }
            catch (FormatException fe)
            {
                ExceptionWriter.Instance.WriteException(fe);
                throw new NoEventTimeException();
            }
            catch (OverflowException oe)
            {
                ExceptionWriter.Instance.WriteException(oe);
                throw new NoEventTimeException();
            }

        }

        #region IDisposable Member
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DebugWriter.Instance.WriteMessage("TLLMails", "Disposing");
                if (disposing)
                {
                    if (_config != null)
                    {
                        _config.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }


}
