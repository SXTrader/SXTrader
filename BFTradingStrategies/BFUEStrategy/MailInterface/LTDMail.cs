using ImapX;
using ImapX.Collections;
using net.sxtrader.bftradingstrategies.bfuestrategy.DataModel;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk.Exceptions.MailInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.MailInterface
{
    public class LTDMail
    {
        public BetgreenLTDTippList getBetgreenLTDTipps(String host, int port, bool useSSL, string mailUser, string mailAccess)
        {
            BetgreenLTDTippList theList = new BetgreenLTDTippList();
            try
            {
                ImapClient imapclient = new ImapClient(host, port, useSSL);
                if (imapclient.Connect())
                {
                    if (imapclient.Login(mailUser, mailAccess))
                    {
                        FolderCollection foldercoll = imapclient.Folders;
                        String dts = DateTime.Now.Subtract(new TimeSpan(23, 59, 59)).ToString("d-MMM-yyyy", CultureInfo.InvariantCulture);
                        String searchQuery = String.Format("FROM \"{0}\" SINCE {1}", "betgreen@mail.com", dts);
                        Message[] msgcoll = foldercoll["INBOX"].Search(searchQuery);
                        

                        BetGreenMailParser parser = new BetGreenMailParser();
                        theList = parser.parseMails(msgcoll);
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

                return theList;
            }
            catch (Exception ie)
            {
                ExceptionWriter.Instance.WriteException(ie);
                return theList;
            }

        }
    }
}
