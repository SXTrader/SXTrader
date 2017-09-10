using net.sxtrader.bftradingstrategies.bfuestrategy.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.MailInterface
{
    class NonStandardTippMailFormatParser : IMailFormatParser
    {
        private const String TAGKICKOFF = "Kick off:";
        private const String TAGLEAGUE = "League:";
        private const String TAGMATCH = "Match:";
        private const String TAGLTD = "LAY the DRAW";
        private const String TAGODDS = "Odds:";
        private const String TAGSTOPPLOSS = "Stop loss:";
        private const String TAGBACK = "(Back)";

        public bool checkPrerequisite(string[] mail)
        {
            return true;
        }


        public bool jumpToTipps(ref List<string> mail)
        {
            for (int i = 0; i < mail.Count; i++)
            {
                if (!mail[i].Trim().StartsWith(TAGKICKOFF, StringComparison.InvariantCultureIgnoreCase))
                {
                    mail.RemoveAt(i--);
                    continue;
                }

                return true;
            }

            return false;
        }

        public DataModel.BetgreenLTDTipp extractTipps(List<string> mail)
        {
            /*
             * Kick off: 11.50pm
               League: Brazilian Cup (Brazil)
               Match: Atletico PR v Flamengo
               LAY the DRAW
               Odds: 3.65 (Max Odds 4.2)
               Stop loss: 2.0 (Back)
             */
            //BetgreenLTDTippList theList = new BetgreenLTDTippList();
            BetgreenLTDTipp theTipp = new BetgreenLTDTipp();
            //1. Kickoff
            for (int i = 0; i < mail.Count; i++)
            {
                // Keine Tipp Element => entfernen
                if (!mail[i].Trim().StartsWith(TAGKICKOFF, StringComparison.InvariantCultureIgnoreCase) &&
                    !mail[i].Trim().StartsWith(TAGLEAGUE, StringComparison.InvariantCultureIgnoreCase) &&
                    !mail[i].Trim().StartsWith(TAGMATCH, StringComparison.InvariantCultureIgnoreCase) &&
                    !mail[i].Trim().StartsWith(TAGLTD, StringComparison.InvariantCultureIgnoreCase) &&
                    !mail[i].Trim().StartsWith(TAGODDS, StringComparison.InvariantCultureIgnoreCase) &&
                    !mail[i].Trim().StartsWith(TAGSTOPPLOSS, StringComparison.InvariantCultureIgnoreCase))
                {
                    mail.RemoveAt(i--);
                    continue;
                }

                if (mail[i].Trim().StartsWith(TAGKICKOFF, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!extractKickOff(mail[i], ref theTipp))
                    {
                        mail.RemoveAt(i--);
                        return null;
                    }
                    mail.RemoveAt(i--);
                   
                }
                else if (mail[i].Trim().StartsWith(TAGLEAGUE, StringComparison.InvariantCultureIgnoreCase))
                {
                    mail.RemoveAt(i--);
                }
                else if (mail[i].Trim().StartsWith(TAGMATCH, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!extractTeams(mail[i], ref theTipp))
                    {
                        mail.RemoveAt(i--);
                        return null;
                    }

                    mail.RemoveAt(i--);
                }
                else if (mail[i].Trim().StartsWith(TAGLTD, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!isLTDTipp(mail[i]))
                    {
                        mail.RemoveAt(i--);
                        return null;
                    }

                    mail.RemoveAt(i--);
                }
                else if (mail[i].Trim().StartsWith(TAGODDS, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!extractOdds(mail[i], ref theTipp))
                    {
                        mail.RemoveAt(i--);
                        return null;
                    }

                    mail.RemoveAt(i--);
                }
                else if (mail[i].Trim().StartsWith(TAGSTOPPLOSS, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!extractStoppLoss(mail[i], ref theTipp))
                    {
                        mail.RemoveAt(i--);
                        return null;
                    }

                    mail.RemoveAt(i--);
                }

                if (isTippComplete(theTipp))
                    return theTipp;                
            }
                                              
            return null;
        }


        private bool isTippComplete(BetgreenLTDTipp theTipp)
        {
            bool bRet = false;
            if (!theTipp.KickOff.Equals(DateTime.MinValue) &&
                theTipp.MaximumOdds != 0.0 &&
                theTipp.RecommendedOdds != 0.0 &&
                theTipp.StoppLossOdds != 0.0 &&
                !String.IsNullOrEmpty(theTipp.TeamA) &&
                !String.IsNullOrEmpty(theTipp.TeamB))
            {
                bRet = true;
            }

            return bRet;
        }

        private bool extractKickOff(String line, ref BetgreenLTDTipp theTipp)
        {
            
            if (!line.Trim().StartsWith(TAGKICKOFF, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            //Falls dieses Tipp-Element schon mal gesetzt, dann liegt wohl ein Fehler vor
            if (!theTipp.KickOff.Equals(DateTime.MinValue))
            {
                //TODO: Logging
                return false;
            }

            //Kickoff-Tag entfernen
            String workLine = line.Remove(0, TAGKICKOFF.Length).Trim();
            
            //Aktuelles lokales Datum holen
            DateTime dtsLocalNow = DateTime.Now;
            //Aktuelles Datum in UK-Zeit konvertieren
            String id = "GMT Standard Time";
            TimeZoneInfo tziUK;
            tziUK = TimeZoneInfo.FindSystemTimeZoneById(id);
            DateTime dtsLocalUK = TimeZoneInfo.ConvertTime(dtsLocalNow,TimeZoneInfo.Local, tziUK);                

            //Anstoßzeitpunkt in Britischer Zeit
            if (workLine.Length == 6)
                workLine = "0" + workLine;
            String strKickOffUK = String.Format("{0}.{1}.{2} {3}", dtsLocalUK.Day, dtsLocalUK.Month, dtsLocalUK.Year, workLine.Trim());
            DateTimeFormatInfo fi = new CultureInfo("en-GB", false).DateTimeFormat;
            DateTime dtsKickOffUK = DateTime.ParseExact(strKickOffUK, "dd.MM.yyyy hh.mmtt", fi);

            //Konvertieren in lokale Zeit
            DateTime dtsKickOffLocal = TimeZoneInfo.ConvertTime(dtsKickOffUK, tziUK, TimeZoneInfo.Local);

            theTipp.KickOff = dtsKickOffLocal;
            
            return true;

        }

        private bool extractTeams(String line, ref BetgreenLTDTipp theTipp)
        {
            if (!line.Trim().StartsWith(TAGMATCH, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }


            //Falls dieses Team-Elemente schon mal gesetzt, dann liegt wohl ein Fehler vor
            if (!String.IsNullOrEmpty(theTipp.TeamA) || !String.IsNullOrEmpty(theTipp.TeamB))
            {
                //TODO: Logging
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
        private bool extractOdds(String line, ref BetgreenLTDTipp theTipp)
        {
            if (!line.Trim().StartsWith(TAGODDS, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            //Falls dieses Team-Elemente schon mal gesetzt, dann liegt wohl ein Fehler vor
            if (theTipp.MaximumOdds != 0.0)
            {
                //TODO: Logging
                return false;
            }

            String oddsLine = line.Remove(0, TAGODDS.Length).Trim();

            String[] splits = oddsLine.Split(new string[] { "(Max Odds" }, StringSplitOptions.RemoveEmptyEntries);

            // Wir brauchen 2 Elemente
            if (splits.Length != 2)
            {
                //TODO: Logging
                return false;
            }

            double recOdds = 0.0;
            bool ret = Double.TryParse(splits[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out recOdds);
            if (ret)
                theTipp.RecommendedOdds = recOdds;
            else
            {
                //TODO: Logging
                return false;
            }

            String maxOddsString = splits[1].Substring(0, splits[1].Length - 1);
            double maxOdds = 0.0;
            ret = Double.TryParse(maxOddsString, NumberStyles.Any, CultureInfo.InvariantCulture, out maxOdds);

            if (ret)
                theTipp.MaximumOdds = maxOdds;

            return ret;            
        }

        private bool extractStoppLoss(String line, ref BetgreenLTDTipp theTipp)
        {
            if (!line.Trim().StartsWith(TAGSTOPPLOSS, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            //Falls Stopp/Loss-Element schon mal gesetzt, dann liegt wohl ein Fehler vor
            if (theTipp.StoppLossOdds != 0.0)
            {
                //TODO: Logging
                return false;
            }

            String stopLossLine = line.Remove(0, TAGSTOPPLOSS.Length).Trim();
            stopLossLine = stopLossLine.Replace(TAGBACK, "").Trim();
            double stopLossOdds = 0.0;
            bool ret = Double.TryParse(stopLossLine, NumberStyles.Any, CultureInfo.InvariantCulture, out stopLossOdds);
            if (ret)
                theTipp.StoppLossOdds = stopLossOdds;
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
    }
}
