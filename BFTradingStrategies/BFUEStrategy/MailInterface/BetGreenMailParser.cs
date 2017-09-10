using ImapX;
using ImapX.Collections;
using net.sxtrader.bftradingstrategies.bfuestrategy.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.MailInterface
{
    //TODO: ExceptionHandling!
    public class BetGreenMailParser
    {
        private const String TIPPMAILSUBJECT  = "Daily Trades";
        
        private const String NOLTDTIPPS       = "No LTD Trades";
       
        
        private const String TAGMATCH         = "Match:";
        private const String TAGLTD           = "LAY the DRAW";
        private const String TAGMAXODDS       = "Odds: Max Odds";
        private const String TAGSTOPLOSS      = "Stop loss:";
        private const String TAGBACK          = "(Back)";

        internal BetgreenLTDTippList parseMails(Message[] msgColl)
        {
            
            BetgreenLTDTippList theList = new BetgreenLTDTippList();
            IMailFormatParser parser = null;
            foreach (Message msg in msgColl)
            {
                if(!isTodaysTippMail(msg.Subject, out parser))
                {
                    continue;
                }

                if (parser == null)
                    continue;
                 String[] content = msg.Body.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                 List<String> contentList = content.ToList();

                 if (!parser.checkPrerequisite(content))
                     continue;


                //1. Schritt finde den Anfang der aktuellen Tipps in der Mail
                 if (!parser.jumpToTipps(ref contentList))
                     continue;
                 BetgreenLTDTipp theTipp = null;
                 while((theTipp = parser.extractTipps(contentList))!=null)
                    theList.Add(theTipp);
            }
            return theList;
        }

        private bool isTodaysTippMail(String subject, out IMailFormatParser parser)
        {
            parser = null;
            // Eine Tipp-Mail hat verschiedene Formate
            // z.B. Daily Trades
            // z.B. Friday 13th Trades
            // z.B. Thursday 12th September
            // z.B  Wednesday 11th September Trades
            // z.B  8th November Trades

            if (subject.Equals(TIPPMAILSUBJECT, StringComparison.InvariantCultureIgnoreCase))
            {
                parser = new StandardTippMailFormatParser();
                return true;
            }


            //Aktuelles lokales Datum holen
            DateTime dtsLocalNow = DateTime.Now;
            //Aktuelles Datum in UK-Zeit konvertieren
            String id = "GMT Standard Time";
            TimeZoneInfo tziUK;
            tziUK = TimeZoneInfo.FindSystemTimeZoneById(id);
            DateTime dtsLocalUK = TimeZoneInfo.ConvertTime(dtsLocalNow, TimeZoneInfo.Local, tziUK);   

            // September 13th Trades
            String expectedSubject = String.Format("{0} {1}{2} Trades", dtsLocalUK.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-GB")),dtsLocalUK.Day ,GetDaySuffix(dtsLocalUK.Day));

            if (expectedSubject.Equals(subject, StringComparison.InvariantCultureIgnoreCase))
            {
                parser = new NonStandardTippMailFormatParser();
                return true;
            }

            //Friday 13th Trades
            expectedSubject = String.Format("{0} {1}{2} Trades", 
                dtsLocalUK.ToString("dddd", CultureInfo.CreateSpecificCulture("en-GB")),
                dtsLocalUK.Day, GetDaySuffix(dtsLocalUK.Day));

            if (expectedSubject.Equals(subject, StringComparison.InvariantCultureIgnoreCase))
            {
                parser = new NonStandardTippMailFormatParser();
                return true;
            }
            //Thursday 12th September
            expectedSubject = String.Format("{0} {1}{2} {3}",
                dtsLocalUK.ToString("dddd", CultureInfo.CreateSpecificCulture("en-GB")),
                dtsLocalUK.Day, GetDaySuffix(dtsLocalUK.Day),
                dtsLocalUK.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-GB")));

            if (expectedSubject.Equals(subject, StringComparison.InvariantCultureIgnoreCase))
            {
                parser = new NonStandardTippMailFormatParser();
                return true;
            }


            //Wednesday 11th September Trades
            expectedSubject = String.Format("{0} {1}{2} {3} Trades",
               dtsLocalUK.ToString("dddd", CultureInfo.CreateSpecificCulture("en-GB")),
               dtsLocalUK.Day, GetDaySuffix(dtsLocalUK.Day),
               dtsLocalUK.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-GB")));

            if (expectedSubject.Equals(subject, StringComparison.InvariantCultureIgnoreCase))
            {
                parser = new NonStandardTippMailFormatParser();
                return true;
            }

            //8th November Trades
            expectedSubject = String.Format("{0}{1} {2} Trades",              
              dtsLocalUK.Day, GetDaySuffix(dtsLocalUK.Day),
              dtsLocalUK.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-GB")));
            if (expectedSubject.Equals(subject, StringComparison.InvariantCultureIgnoreCase))
            {
                parser = new NonStandardTippMailFormatParser();
                return true;
            }

            return false;
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

        private BetgreenLTDTippList extractTipps(List<String> tippList)
        {
            BetgreenLTDTippList theList = new BetgreenLTDTippList();
            BetgreenLTDTipp tipp = new BetgreenLTDTipp();
            bool parseState = true;
            bool isLTD = true;

            // Vorcheck: Keine Daten mehr?
            if (tippList == null || tippList.Count == 0)
            {
                return theList;
            }

            //Vorcheck: Kein Tipptag?
            if (tippList[0].Equals(NOLTDTIPPS))
                return theList;
            /*
            // Ein Tip fängt immer mit den Kick Off an, falls nicht, haben wir hier nichts mehr zu tun
            parseState = extractKickOff(tippList[0], ref tipp);

            if (parseState)
            {
                tippList.RemoveAt(0);
                parseState = extractLeague(tippList[0], ref tipp);
            }

            if (parseState)
            {
                tippList.RemoveAt(0);
            }
            */

            parseState = extractLeague(tippList[0], ref tipp);

            if (parseState)
            {
                tippList.RemoveAt(0);
                parseState = extractTeams(tippList[0], ref tipp);
            }

            if (parseState)
            {
                tippList.RemoveAt(0);
                isLTD = isLTDTipp(tippList[0]);
            }

            if (parseState)
            {
                tippList.RemoveAt(0);
                parseState = extractMaxOdds(tippList[0], ref tipp);
            }

            if (parseState)
            {
                tippList.RemoveAt(0);
                parseState = extractStoppLoss(tippList[0], ref tipp);
            }

            if (parseState)
            {
                tippList.RemoveAt(0);
                if (isLTD)
                    theList.Add(tipp);

                if (/*tippList[0].StartsWith(TAGKICKOFF, StringComparison.InvariantCultureIgnoreCase) ||
                    tippList[0].StartsWith(TAGLEAGUE, StringComparison.InvariantCultureIgnoreCase) ||*/
                    tippList[0].StartsWith(TAGMATCH, StringComparison.InvariantCultureIgnoreCase) ||
                    tippList[0].StartsWith(TAGLTD, StringComparison.InvariantCultureIgnoreCase) ||
                    tippList[0].StartsWith(TAGMAXODDS, StringComparison.InvariantCultureIgnoreCase) ||
                    tippList[0].StartsWith(TAGSTOPLOSS, StringComparison.InvariantCultureIgnoreCase))
                {
                    theList.AddRange(extractTipps(tippList));
                }
            }

            return theList;
        }

        private bool extractStoppLoss(String line, ref BetgreenLTDTipp theTipp)
        {
            if (!line.Trim().StartsWith(TAGSTOPLOSS, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            String stopLossLine = line.Remove(0, TAGSTOPLOSS.Length).Trim();
            stopLossLine = stopLossLine.Replace(TAGBACK, "").Trim();
            double stopLossOdds = 0.0;
            bool ret = Double.TryParse(stopLossLine, NumberStyles.Any, CultureInfo.InvariantCulture, out stopLossOdds);
            if (ret)
                theTipp.StoppLossOdds = stopLossOdds;
            return ret;
        }

        private bool extractMaxOdds(String line, ref BetgreenLTDTipp theTipp)
        {
            if (!line.Trim().StartsWith(TAGMAXODDS, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            String maxOddsLine = line.Remove(0, TAGMAXODDS.Length).Trim();
            double maxOdds = 0.0;
            bool ret = Double.TryParse(maxOddsLine,NumberStyles.Any, CultureInfo.InvariantCulture, out maxOdds);

            if (ret)
                theTipp.MaximumOdds = maxOdds;

            return ret;
        }

        private bool isLTDTipp(String line)
        {
            if (!line.Trim().StartsWith(TAGLTD, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private bool extractTeams(String line, ref BetgreenLTDTipp theTipp)
        {
            if (!line.Trim().StartsWith(TAGMATCH, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            String matchLine = line.Remove(0, TAGMATCH.Length).Trim();
            String[] teams = matchLine.Split(new string[] { " v " }, StringSplitOptions.RemoveEmptyEntries);
            if (teams == null || teams.Length < 2)
            {
                return false;
            }

            theTipp.TeamA = teams[0].Trim();
            theTipp.TeamB = teams[1].Trim();
            return true;
        }

        private bool extractLeague(String line, ref BetgreenLTDTipp theTipp)
        {
            /*
            if (!line.Trim().StartsWith(TAGLEAGUE, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
             * */
            return true;
        }

        private bool extractKickOff(String line, ref BetgreenLTDTipp theTipp)
        {
            /*
            if (!line.Trim().StartsWith(TAGKICKOFF, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            //Kickoff-Tag entfernen
            String workLine = line.Remove(0, TAGKICKOFF.Length);
            
            //Aktuelles lokales Datum holen
            DateTime dtsLocalNow = DateTime.Now;
            //Aktuelles Datum in UK-Zeit konvertieren
            String id = "GMT Standard Time";
            TimeZoneInfo tziUK;
            tziUK = TimeZoneInfo.FindSystemTimeZoneById(id);
            DateTime dtsLocalUK = TimeZoneInfo.ConvertTime(dtsLocalNow,TimeZoneInfo.Local, tziUK);                

            //Anstoßzeitpunkt in Britischer Zeit
            String strKickOffUK = String.Format("{0}.{1}.{2} {3}", dtsLocalUK.Day, dtsLocalUK.Month, dtsLocalUK.Year, workLine.Trim());
            DateTimeFormatInfo fi = new CultureInfo("en-GB", false).DateTimeFormat;
            DateTime dtsKickOffUK = DateTime.ParseExact(strKickOffUK, "dd.MM.yyyy hh:mm tt", fi);

            //Konvertieren in lokale Zeit
            DateTime dtsKickOffLocal = TimeZoneInfo.ConvertTime(dtsKickOffUK, tziUK, TimeZoneInfo.Local);

            theTipp.KickOff = dtsKickOffLocal;
            */
            return true;
             
        }
    }
}
