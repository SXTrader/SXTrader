using net.sxtrader.bftradingstrategies.bfuestrategy.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.MailInterface
{
    class StandardTippMailFormatParser : IMailFormatParser
    {
        private const String TIPPSECTIONSTART = "Today's Trades";
        private const String TAGKICKOFF = "Kick off:";
        private const String TAGLEAGUE = "League:";


        public bool checkPrerequisite(string[] mail)
        {
            // Mindestens 2 Zeilen müssen vorhanden sein
            if (mail.Length < 2)
                return false;
            // Im Standardformat ist immer die zweite Zeile der Email das Datum der aktuellen Tipps
            String dateLine = mail[1];

            //Aktuelles lokales Datum holen
            DateTime dtsLocalNow = DateTime.Now;
            //Aktuelles Datum in UK-Zeit konvertieren
            String id = "GMT Standard Time";
            TimeZoneInfo tziUK;
            tziUK = TimeZoneInfo.FindSystemTimeZoneById(id);
            DateTime dtsLocalUK = TimeZoneInfo.ConvertTime(dtsLocalNow, TimeZoneInfo.Local, tziUK);

            //13th  September  Trades
            String expectedDateLine = String.Format("{1}{2} {0} Trades", dtsLocalUK.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-GB")), dtsLocalUK.Day, GetDaySuffix(dtsLocalUK.Day));

            if (!dateLine.Trim().Equals(expectedDateLine, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        public bool jumpToTipps(ref List<String> mail)
        {
            for (int i = 0; i < mail.Count; i++)
            {
                if (!mail[i].Trim().Equals(TIPPSECTIONSTART, StringComparison.InvariantCultureIgnoreCase))
                {
                    mail.RemoveAt(i--);
                    continue;
                }

                return true;
            }

            return false;
        }

        public BetgreenLTDTipp extractTipps(List<string> mail)
        {
            //BetgreenLTDTippList theList =new BetgreenLTDTippList();
            BetgreenLTDTipp theTipp = new BetgreenLTDTipp();
            //1. Kickoff
            for (int i = 0; i < mail.Count; i++)
            {
                if (!mail[i].Trim().Equals(TAGKICKOFF, StringComparison.InvariantCultureIgnoreCase))
                {
                    mail.RemoveAt(i--);
                    continue;
                }

                //Aktuelles lokales Datum holen
                DateTime dtsLocalNow = DateTime.Now;
                //Aktuelles Datum in UK-Zeit konvertieren
                String id = "GMT Standard Time";
                TimeZoneInfo tziUK;
                tziUK = TimeZoneInfo.FindSystemTimeZoneById(id);
                DateTime dtsLocalUK = TimeZoneInfo.ConvertTime(dtsLocalNow, TimeZoneInfo.Local, tziUK);

                //Anstoßzeitpunkt in Britischer Zeit
                String strKickOffUK = String.Format("{0}.{1}.{2} {3}", dtsLocalUK.Day, dtsLocalUK.Month, dtsLocalUK.Year, mail[i].Trim());
                DateTimeFormatInfo fi = new CultureInfo("en-GB", false).DateTimeFormat;
                DateTime dtsKickOffUK = DateTime.ParseExact(strKickOffUK, "dd.MM.yyyy hh:mm tt", fi);

                //Konvertieren in lokale Zeit
                DateTime dtsKickOffLocal = TimeZoneInfo.ConvertTime(dtsKickOffUK, tziUK, TimeZoneInfo.Local);

                theTipp.KickOff = dtsKickOffLocal;
                mail.RemoveAt(i);
                break;
            }

            /*
            if (mail.Count == 0)
                return theList;
            */

            return theTipp;
            //2. Liga
            if (!mail[0].Trim().StartsWith(TAGLEAGUE, StringComparison.InvariantCultureIgnoreCase))
            {

            }
            else
            {
                mail.RemoveAt(0);
            }

            return null;

        }

        private bool extractLeague(ref List<String> mail)
        {
            if (!mail[0].Trim().StartsWith(TAGLEAGUE, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            else
            {
                mail.RemoveAt(0);
                return true;
            }
        }

        private string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

    }
}
