using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSCommonObjects;
using LSDataConnector;
using LSCConnector;

namespace LSParser
{
    public class LSParser
    {
        private static char[] SEPSTAGE1 = { ';' };
        private static char[] SEPSTAGE2 = { '=' };
        private static char[] SEPSTAGE3 = { ',' };
        private static String[] LINEBREAKSEP = { "\r\n" };

        private const String MATCHIDTAG = "d_wf_bh";
        private const String LIGAIDTAG = "d_wf_mn";
        private const String LIGANAMETAG = "d_wf_mn2";
        private const String MATCHDATETAG = "d_wf_tm";
        private const String TEAMAIDTAG = "d_wf_ai";
        private const String TEAMBIDTAG = "d_wf_bi";
        private const String TEAMANAMETAG = "d_wf_ta";
        private const String TEAMBNAMETAG = "d_wf_tb";
        private const String TEAMASCORETAG = "d_wf_Ascore";
        private const String TEAMBSCORETAG = "d_wf_Bscore";
        private const String HTSCORETAG = "d_wf_Hscore";

        private const String MATCHIDTAG2 = "th_bh";
        private const String LIGAIDTAG2 = "th_mn";
        private const String LIGANAMETAG2 = "th_mn2";
        private const String MATCHDATETAG2= "th_tm";
        private const String TEAMAIDTAG2 = "th_tai";
        private const String TEAMBIDTAG2 = "th_tbi";
        private const String TEAMANAMETAG2= "th_tan";
        private const String TEAMBNAMETAG2 = "th_tbn";
        private const String TEAMASCORETAG2 = "th_as";
        private const String TEAMBSCORETAG2 = "th_bs";
        private const String HTSCORETAG2 = "th_hs";

        private const string EVENTTYPETAG = "d_lx";
        private const string EVENTMINUTETAG = "d_tm";
        private const string EVENTSIDETAG = "d_sx";
        private const string EVENTSCORETAG = "d_bf";
        private const string EVENTPLAYERTAG = "d_pn";

        private LSDatabase _lsdb;
        private LSReader _lsrdr; 

        public LSParser(LSDatabase lsdb)
        {
            _lsdb = lsdb;
            _lsrdr = new LSReader();
        }



        public MatchEventList parseMatchEvents(ulong matchId, ulong teamAId, ulong teamBId, String theData)
        {
            MatchEventList resultList = new MatchEventList();
            /*
            var d_pi = [0,0,0,0,0,0];
            var d_pn = ['','','','','',''];
            var d_lx = [0,0,0,4,0,0];
            var d_tm = [15,45,69,77,84,90];
            var d_sx = [0,0,0,-1,-1,0];
            var d_bf = ['1-0','2-0','3-0','','3-1','4-1'];
            var d_mc = '0542B0';
            var d_grade = 4;
            var d_mi = 728;
            var d_mn = 'Korea National League Cup';
            var d_ai = 5890;
            var d_bi = 5921;
            var d_ta = 'Busan Transpor Tation';
            var d_tb = 'Gangneung';
            var d_upi = [];
            var d_upn = [];
            var d_dpi = [];
            var d_dpn = [];
            var d_stm = [];
            var d_ssx = [];

            */
            // Feld d_lx gibt art des Ereignis an 0 = TOR;1=Elfmetertor;2=Eigentor?; 3=Gelbe Karte; 4 = Rote Karte; 5 = Gelb/Rot?
            // Feld d_sx gibt an welches Team das Ereignis hatte: 0 = TeamA; -1 = TeamB
            // Feld d_pn gibt den Spielernamen an, auf den sich das Ereignis bezieht
            // Feld d_pi gibt die Spieler-ID an
            // Felder d_ai und d_bi sind die Team-Ids
            String strMatchEvents = String.Empty;
            String strMatchMinutes = String.Empty;
            String strEventSide = String.Empty;
            String strScore = String.Empty;
            String strPlayer = String.Empty;

            //Erster Schritt des Parsings: Einzelne Datenblöcke separieren.
            String[] stage1SplitArray = theData.Split(LINEBREAKSEP, StringSplitOptions.RemoveEmptyEntries);

            //Zweiter Schritt: unnötige Tags entfernen und benötigte Arrays ausfiltern
            for (int i = 0; i < stage1SplitArray.Length; i++)
            {
                if (stage1SplitArray[i].Trim().StartsWith("var", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (stage1SplitArray[i].Trim().Length > 3)
                        stage1SplitArray[i] = stage1SplitArray[i].Trim().Remove(0, 4);
                    else
                        continue;
                }

                if (stage1SplitArray[i].Trim().EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
                {
                    stage1SplitArray[i] = stage1SplitArray[i].Trim().Remove(stage1SplitArray[i].Length - 1, 1);
                }

                if (stage1SplitArray[i].Trim() == String.Empty)
                    continue;

                if (stage1SplitArray[i].Trim().StartsWith(EVENTTYPETAG, StringComparison.InvariantCultureIgnoreCase))
                {
                    strMatchEvents = stringRemover(EVENTTYPETAG, stage1SplitArray[i]);
                }

                if (stage1SplitArray[i].Trim().StartsWith(EVENTMINUTETAG, StringComparison.InvariantCultureIgnoreCase))
                {
                    strMatchMinutes = stringRemover(EVENTMINUTETAG, stage1SplitArray[i]);
                }

                if (stage1SplitArray[i].Trim().StartsWith(EVENTSIDETAG, StringComparison.InvariantCultureIgnoreCase))
                {
                    strEventSide = stringRemover(EVENTSIDETAG, stage1SplitArray[i]);
                }

                if (stage1SplitArray[i].Trim().StartsWith(EVENTSCORETAG, StringComparison.InvariantCultureIgnoreCase))
                {
                    strScore = stringRemover(EVENTSCORETAG, stage1SplitArray[i]);
                }

                if (stage1SplitArray[i].Trim().StartsWith(EVENTPLAYERTAG, StringComparison.InvariantCultureIgnoreCase))
                {
                    strPlayer = stringRemover(EVENTPLAYERTAG, stage1SplitArray[i]);
                }
            }
            resultList = internalMatchEventBuilder(matchId, teamAId, teamBId, strMatchEvents, strMatchMinutes, strEventSide, strScore, strPlayer);
            return resultList;
        }

        public LiveScoreMatchDataList parseHistoricEncounterData(String theData)
        {
            String strMatchIds = String.Empty;
            String strLigaIds = String.Empty;
            String strLigaNames = String.Empty;
            String strMatchDates = String.Empty;
            String strTeamAIds = String.Empty;
            String strTeamBIds = String.Empty;
            String strTeamANames = String.Empty;
            String strTeamBNames = String.Empty;
            String strTeamAScores = String.Empty;
            String strTeamBScores = String.Empty;
            String strHTScores = String.Empty;
            LiveScoreMatchDataList sortedList = new LiveScoreMatchDataList();

            //Erster Schritt des Parsings: Einzelne Datenblöcke separieren.
            String[] stage1SplitArray = theData.Split(LINEBREAKSEP, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                //Zweiter Schritt: unnötige Tags entfernen und benötigte Arrays ausfiltern
                for (int i = 0; i < stage1SplitArray.Length; i++)
                {
                    if (stage1SplitArray[i].Trim().StartsWith("var", StringComparison.InvariantCultureIgnoreCase))
                    {
                        stage1SplitArray[i] = stage1SplitArray[i].Trim().Remove(0, 4);
                    }

                    if (stage1SplitArray[i].Trim().EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
                    {
                        stage1SplitArray[i] = stage1SplitArray[i].Trim().Remove(stage1SplitArray[i].Length - 1, 1);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(MATCHIDTAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strMatchIds = stringRemover(MATCHIDTAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(LIGAIDTAG2, StringComparison.InvariantCultureIgnoreCase) && !stage1SplitArray[i].Trim().StartsWith(LIGANAMETAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strLigaIds = stringRemover(LIGAIDTAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(LIGANAMETAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strLigaNames = stringRemover(LIGANAMETAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(MATCHDATETAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strMatchDates = stringRemover(MATCHDATETAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMAIDTAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamAIds = stringRemover(TEAMAIDTAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMBIDTAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamBIds = stringRemover(TEAMBIDTAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMANAMETAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamANames = stringRemover(TEAMANAMETAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMBNAMETAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamBNames = stringRemover(TEAMBNAMETAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMASCORETAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamAScores = stringRemover(TEAMASCORETAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMBSCORETAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamBScores = stringRemover(TEAMBSCORETAG2, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(HTSCORETAG2, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strHTScores = stringRemover(HTSCORETAG2, stage1SplitArray[i]);
                    }


                }

                if (!(strMatchIds == String.Empty || strLigaIds == String.Empty || strLigaNames == String.Empty ||
                    strMatchDates == String.Empty || strTeamAIds == String.Empty || strTeamBIds == String.Empty ||
                    strTeamANames == String.Empty || strTeamBNames == String.Empty || strTeamAScores == String.Empty ||
                    strTeamBScores == String.Empty || strHTScores == String.Empty))
                {
                    sortedList = internalMatchDataBuilder(strMatchIds, strLigaIds, strLigaNames, strMatchDates, strTeamAIds, strTeamBIds, strTeamANames, strTeamBNames,
                        strTeamAScores, strTeamBScores, strHTScores);
                }

            }
            catch (Exception)
            {
                //...
            }
            return sortedList;
        }

        public LiveScoreMatchDataList parseDirectEncounterData(String theData)
        {
            String strMatchIds = String.Empty;
            String strLigaIds = String.Empty;
            String strLigaNames = String.Empty;
            String strMatchDates = String.Empty;
            String strTeamAIds = String.Empty;
            String strTeamBIds = String.Empty;
            String strTeamANames = String.Empty;
            String strTeamBNames = String.Empty;
            String strTeamAScores = String.Empty;
            String strTeamBScores = String.Empty;
            String strHTScores = String.Empty;

            LiveScoreMatchDataList sortedList = new LiveScoreMatchDataList();

            //Erster Schritt des Parsings: Einzelne Datenblöcke separieren.
            String[] stage1SplitArray = theData.Split(LINEBREAKSEP, StringSplitOptions.RemoveEmptyEntries);

           
            //Zweiter Schritt: unnötige Tags entfernen und benötigte Arrays ausfiltern
            for (int i = 0; i < stage1SplitArray.Length; i++)
            {
                try
                {
                    if (stage1SplitArray[i].Trim().StartsWith("var", StringComparison.InvariantCultureIgnoreCase))
                    {
                        stage1SplitArray[i] = stage1SplitArray[i].Trim().Remove(0, 4);
                    }

                    if (stage1SplitArray[i].Trim().EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
                    {
                        stage1SplitArray[i] = stage1SplitArray[i].Trim().Remove(stage1SplitArray[i].Length - 1, 1);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(MATCHIDTAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strMatchIds = stringRemover(MATCHIDTAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(LIGAIDTAG, StringComparison.InvariantCultureIgnoreCase) && !stage1SplitArray[i].Trim().StartsWith(LIGANAMETAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strLigaIds = stringRemover(LIGAIDTAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(LIGANAMETAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strLigaNames = stringRemover(LIGANAMETAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(MATCHDATETAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strMatchDates = stringRemover(MATCHDATETAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMAIDTAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamAIds = stringRemover(TEAMAIDTAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMBIDTAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamBIds = stringRemover(TEAMBIDTAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMANAMETAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamANames = stringRemover(TEAMANAMETAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMBNAMETAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamBNames = stringRemover(TEAMBNAMETAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMASCORETAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamAScores = stringRemover(TEAMASCORETAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(TEAMBSCORETAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strTeamBScores = stringRemover(TEAMBSCORETAG, stage1SplitArray[i]);
                    }

                    if (stage1SplitArray[i].Trim().StartsWith(HTSCORETAG, StringComparison.InvariantCultureIgnoreCase))
                    {
                        strHTScores = stringRemover(HTSCORETAG, stage1SplitArray[i]);
                    }

                }
                catch (ArgumentOutOfRangeException aoore)
                {
                    continue;
                }
            }

            if (!(strMatchIds == String.Empty || strLigaIds == String.Empty || strLigaNames == String.Empty ||
                strMatchDates == String.Empty || strTeamAIds == String.Empty || strTeamBIds == String.Empty ||
                strTeamANames == String.Empty || strTeamBNames == String.Empty || strTeamAScores == String.Empty ||
                strTeamBScores == String.Empty || strHTScores == String.Empty))
            {
                sortedList = internalMatchDataBuilder(strMatchIds, strLigaIds, strLigaNames, strMatchDates, strTeamAIds, strTeamBIds, strTeamANames, strTeamBNames,
                    strTeamAScores, strTeamBScores, strHTScores);
            }
            return sortedList;
        }

        public LiveScoreMatchDataList parseInitialData(String theData)
        {
            LiveScoreMatchDataList sortedList = new LiveScoreMatchDataList();
            // Erste Stufe des Parsings: Einzelne Datenblöcke separieren
            String[] stage1SplitArray = theData.Trim().Split(SEPSTAGE1);
            foreach (String stage1String in stage1SplitArray)
            {
                // Zweite Stufe: Deklaration von Dateninhalt trennen
                String[] stage2SplitArray = stage1String.Trim().Split(SEPSTAGE2);
                for (int i = 0; i < stage2SplitArray.Length; i += 2)
                {
                    String strDeclartion = stage2SplitArray[i].Trim();
                    if (strDeclartion == String.Empty)
                        continue;
                    if (!strDeclartion.Trim().StartsWith("sDt"))
                        continue;
                    //unnötiges aus der Deklaration entfernen
                    strDeclartion = strDeclartion.Trim();
                    //"sDt[" und "]" entfernen
                    strDeclartion = strDeclartion.Replace("sDt[", String.Empty).Trim();
                    strDeclartion = strDeclartion.Replace("]", String.Empty).Trim();
                    // MatchId konvertieren
                    ulong matchId = UInt32.Parse(strDeclartion);


                    //Datenteil auswerten
                    String strData = stage2SplitArray[i + 1];
                    strData = strData.Replace("[", String.Empty);
                    strData = strData.Replace("]", String.Empty);
                    //Dritte Stufe: Datenfelder Vereinzeln
                    String[] stage3SplitArray = strData.Split(SEPSTAGE3);

                    // Vierte Stufe: Benötigte Daten lesen und daraus ein Objekt generiren
                    try
                    {
                        String devision = stage3SplitArray[0].Trim();
                        String teamA = stage3SplitArray[2].Trim();
                        String teamB = stage3SplitArray[3].Trim();
                        ulong teamAId = UInt32.Parse(stage3SplitArray[9].Trim());
                        ulong teamBId = UInt32.Parse(stage3SplitArray[10].Trim());

                        // Nur hinzufügen, wenn Begegnung noch nicht vorhanden
                        if (!_lsdb.MatchExist(matchId))
                        {
                            LiveScoreMatchData matchData = new LiveScoreMatchData(teamA, teamAId, teamB, teamBId, devision, matchId);

                            sortedList.Add(matchData.MatchId, matchData);
                        }
                    }
                    catch
                    {
                        continue;
                    }

                }

            }
            return sortedList;
        }

        private String stringRemover(String string1, String string2)
        {
            String strReturn =string2.Trim();

            strReturn = strReturn.Remove(0, string1.Length).Trim();
            if (strReturn.Trim() != String.Empty)
            {
                if (strReturn[0] == '=') strReturn = strReturn.Remove(0, 1).Trim();
                if (strReturn.Length > 0 && strReturn[0] == '[') strReturn = strReturn.Remove(0, 1).Trim();
                if (strReturn.Length > 0 && strReturn[strReturn.Length - 1] == ']') strReturn = strReturn.Remove(strReturn.Length - 1, 1).Trim();
            }
            return strReturn;
        }

        private MatchEventList internalMatchEventBuilder(ulong matchId, ulong teamAId, ulong teamBId, String matchEvents, String eventMinutes,
            String eventSides, String eventScores, String eventPlayers)
        {
            MatchEventList resultList = new MatchEventList();
            // Erster Schritt Arrays aufbauen
            String[] arrEvents = matchEvents.Trim().Split(SEPSTAGE3);
            String[] arrEventMinutes = eventMinutes.Trim().Split(SEPSTAGE3);
            String[] arrEventSides = eventSides.Trim().Split(SEPSTAGE3);
            String[] arrEventScores = eventScores.Trim().Split(SEPSTAGE3);
            String[] arrEventPlayers = eventPlayers.Trim().Split(SEPSTAGE3);

            for (int i = 0; i < arrEvents.Length; i++)
            {
                if (arrEvents[i].Trim() == string.Empty)
                    continue;
                //Daten aufbauen
                MATCHEVENTTYPE eventType = (MATCHEVENTTYPE)UInt16.Parse(arrEvents[i]);
                try
                {
                    uint eventMinute = UInt16.Parse(arrEventMinutes[i]);
                    int eventSide = Int16.Parse(arrEventSides[i]);
                    ulong eventTeam = eventSide == 0 ? teamAId : teamBId;
                    String eventScore = arrEventScores[i];
                    String eventPlayer = arrEventPlayers[i];

                    //Daten säubern
                    if (eventScore.StartsWith("'")) eventScore = eventScore.Remove(0, 1);
                    if (eventScore.EndsWith("'")) eventScore = eventScore.Remove(eventScore.Length - 1, 1);

                    if (eventPlayer.StartsWith("'")) eventPlayer = eventPlayer.Remove(0, 1);
                    if (eventPlayer.EndsWith("'")) eventPlayer = eventPlayer.Remove(eventPlayer.Length - 1, 1);

                    MatchEvent matchEvent = new MatchEvent(matchId, eventTeam, eventType, eventMinute, eventScore, eventPlayer);
                    resultList.Add(matchEvent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return resultList;
        }

        private LiveScoreMatchDataList internalMatchDataBuilder(String matchIds, String leagueIds, String leagueNames, String matchDates,
            String teamAIds, String teamBIds, String teamANames, String teamBNames, String teamAScores, String teamBScores, String htScores )
        {
            LiveScoreMatchDataList resultList = new LiveScoreMatchDataList();
            // Erster Schritt Arrays aufbauen
            String[] arrMatchIds = matchIds.Split(SEPSTAGE3);
            String[] arrLeagueIds = leagueIds.Split(SEPSTAGE3);
            String[] arrLeagueNames = leagueNames.Split(SEPSTAGE3);
            String[] arrMatchDates = matchDates.Split(SEPSTAGE3);
            String[] arrTeamAIds = teamAIds.Split(SEPSTAGE3);
            String[] arrTeamBIds = teamBIds.Split(SEPSTAGE3);
            String[] arrTeamANames = teamANames.Split(SEPSTAGE3);
            String[] arrTeamBNames = teamBNames.Split(SEPSTAGE3);
            String[] arrTeamAScores = teamAScores.Split(SEPSTAGE3);
            String[] arrTeamBScores = teamBScores.Split(SEPSTAGE3);
            String[] arrHTScores = htScores.Split(SEPSTAGE3);

            // Zweiter Schritt: Alle Begegnungen iterieren
            for (int i = 0; i < arrMatchIds.Length; i++)
            {
                try
                {
                    //Daten aufbauen
                    ulong matchId = UInt32.Parse(arrMatchIds[i]);
                    String leagueId = arrLeagueIds[i];
                    String leagueName = arrLeagueNames[i];
                    String matchDate = arrMatchDates[i];
                    ulong teamAId = UInt32.Parse(arrTeamAIds[i]);
                    ulong teamBId = UInt32.Parse(arrTeamBIds[i]);
                    String teamAName = arrTeamANames[i];
                    String teamBName = arrTeamBNames[i];
                    uint teamAScore = UInt32.Parse(arrTeamAScores[i]);
                    uint teamBScore = UInt32.Parse(arrTeamBScores[i]);
                    String htScore = arrHTScores[i];


                    //Daten säubern
                    if (leagueId.StartsWith("'")) leagueId = leagueId.Remove(0, 1);
                    if (leagueId.EndsWith("'")) leagueId = leagueId.Remove(leagueId.Length - 1, 1);

                    if (leagueName.StartsWith("'")) leagueName = leagueName.Remove(0, 1);
                    if (leagueName.EndsWith("'")) leagueName = leagueName.Remove(leagueName.Length - 1, 1);

                    if (matchDate.StartsWith("'")) matchDate = matchDate.Remove(0, 1);
                    if (matchDate.EndsWith("'")) matchDate = matchDate.Remove(matchDate.Length - 1, 1);

                    if (teamAName.StartsWith("'")) teamAName = teamAName.Remove(0, 1);
                    if (teamAName.EndsWith("'")) teamAName = teamAName.Remove(teamAName.Length - 1, 1);

                    if (teamBName.StartsWith("'")) teamBName = teamBName.Remove(0, 1);
                    if (teamBName.EndsWith("'")) teamBName = teamBName.Remove(teamBName.Length - 1, 1);

                    if (htScore.StartsWith("'")) htScore = htScore.Remove(0, 1);
                    if (htScore.EndsWith("'")) htScore = htScore.Remove(htScore.Length - 1, 1);

                    // Objekt erzeugen, falls Objekt noch nicht in der Datenbank
                    if (!_lsdb.MatchExist(matchId))
                    {
                        LiveScoreMatchData matchData = new LiveScoreMatchData(teamAName, teamAId, teamBName, teamBId, leagueId, matchId, matchDate);
                        matchData.ScoreTeamA = teamAScore;
                        matchData.ScoreTeamB = teamBScore;
                        matchData.HalftimeScore = htScore;

                        // MatchEvents
                        String matchEvents = _lsrdr.getGoaldData(matchId);
                        matchData.MatchEvents = this.parseMatchEvents(matchId, teamAId, teamBId, matchEvents);

                        resultList.Add(matchData.MatchId, matchData);
                    }
                    else
                    { //TODO: Datenvalidät prüfen
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }




            return resultList;
        }
    }
}
