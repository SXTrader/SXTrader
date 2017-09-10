using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Runtime.InteropServices;

namespace StatisticRetrivier
{
    public partial class ctlCreateSeasonAssignMatches : UserControl
    {

        private SortedList<decimal, List<MatchData>> _matchDayMatches;
        private String _league;
        private String _seasonName;

        public ctlCreateSeasonAssignMatches(decimal matchDays, String league, String seasonName, DateTime from, DateTime to)
        {
            InitializeComponent();
            _matchDayMatches = new SortedList<decimal, List<MatchData>>((int)matchDays);
            clbMatchDays.DisplayMember = "Name";
            clbMatchDays.ValueMember = "Value";

            buildMatchDays(matchDays);
            Task t = getMatchesAsync(league, from, to);

            _league = league;
            _seasonName = seasonName.Replace('/', '_');
        }

        private void buildMatchDays(decimal matchDays)
        {
            for (decimal d = 1; d <= matchDays; d++)
            {
                MatchDayListItem item = new MatchDayListItem(String.Format("{0}. Spieltag", d), d);                
                clbMatchDays.Items.Add(item);                
            }
        }

        private async Task getMatchesAsync(String league, DateTime from, DateTime to)
        {
            clbMatchDays.Enabled = lvwUnassignedMatches.Enabled= btnMoveSelection.Enabled=btnMoveAll.Enabled = btnRemoveSelection.Enabled =
                btnRemoveAll.Enabled = false; ;
           String url = String.Format("http://www.sxtrader.net/LSHistoryDB/getMatchesByLeague.php?league={0}&from={1}&to={2}", league, from.ToString("yyyy-MM-dd"),to.ToString("yyyy-MM-dd"));
           String xmlString = await Helpers.DoWebRequest(url);
           
           XDocument xDoc = XDocument.Parse(xmlString);

            List<MatchData> match= (from matches in xDoc.Descendants("match")
                       select new MatchData
                       {
                           MatchId = matches.Attribute("matchId").Value.Trim(),
                           MatchDate = matches.Attribute("matchDate").Value.Trim(),
                           TeamAId = matches.Attribute("teamAId").Value.Trim(),
                           TeamBId = matches.Attribute("teamBId").Value.Trim(),
                           TeamAName = matches.Attribute("teamA").Value.Trim(),
                           TeamBName = matches.Attribute("teamB") .Value.Trim(),
                           ScoreA = Int32.Parse(matches.Attribute("scoreA").Value.Trim()),
                           ScoreB = Int32.Parse(matches.Attribute("scoreB").Value.Trim()),
                           HalftimeScore = matches.Attribute("htScore").Value.Trim()

                       }).ToList();

            foreach (var m in match)
            {
                ListViewItem lvi = new ListViewItem(new[] { m.MatchDate, String.Format("{0} - {1}", m.TeamAName, m.TeamBName), String.Format("{0} - {1}", m.ScoreA, m.ScoreB) });
                lvi.Tag = m;
                lvwUnassignedMatches.Items.Add(lvi);
                
            }

            lvwUnassignedMatches.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            clbMatchDays.Enabled = true;
        }

       

        private void clbMatchDays_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Ensure that we are checking an item
            if (e.NewValue != CheckState.Checked)
            {
                btnMoveSelection.Enabled = btnMoveAll.Enabled = btnRemoveSelection.Enabled = btnRemoveAll.Enabled
                    = lvwUnassignedMatches.Enabled = lvwAssignedMatches.Enabled = false;
                lvwAssignedMatches.Items.Clear();
                return;
            }

            // Get the items that are selected
            CheckedListBox.CheckedIndexCollection selectedItems = this.clbMatchDays.CheckedIndices;

            // Check that we have at least 1 item selected
            if (selectedItems.Count > 0)
            {
                // Uncheck the other item
                this.clbMatchDays.SetItemChecked(selectedItems[0], false);
                
            }

            lvwUnassignedMatches.Enabled = btnMoveSelection.Enabled = btnMoveAll.Enabled = btnRemoveSelection.Enabled = btnRemoveAll.Enabled = true;
            lvwAssignedMatches.Items.Clear();
            lvwAssignedMatches.Enabled = false;
            if (clbMatchDays.SelectedItem == null)
                return;
            MatchDayListItem mdli = clbMatchDays.SelectedItem as MatchDayListItem;

            if (mdli != null && _matchDayMatches.ContainsKey(mdli.Value))
            {
                foreach (MatchData m in _matchDayMatches[mdli.Value])
                {
                    ListViewItem lvi = new ListViewItem(new[] { m.MatchDate, String.Format("{0} - {1}", m.TeamAName, m.TeamBName), String.Format("{0} - {1}", m.ScoreA, m.ScoreB) });
                    lvi.Tag = m;
                    lvwAssignedMatches.Items.Add(lvi);
                }

                lvwAssignedMatches.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                if (lvwAssignedMatches.Items.Count > 0)
                    lvwAssignedMatches.Enabled = true;

                lvwAssignedMatches.Sorting = SortOrder.Ascending;
                lvwAssignedMatches.Sort();
            }

        }
        private void btnRemoveSelection_Click(object sender, EventArgs e)
        {
            List<MatchData> listData = new List<MatchData>(lvwAssignedMatches.CheckedItems.Count);
            for (int i = 0; i < lvwAssignedMatches.CheckedItems.Count; i++)
            {
                Console.WriteLine(lvwAssignedMatches.CheckedItems[i]);
                listData.Add(lvwAssignedMatches.CheckedItems[i].Tag as MatchData);
                lvwAssignedMatches.Items.Remove(lvwAssignedMatches.CheckedItems[i--]);
            }
            MatchDayListItem mdli = clbMatchDays.CheckedItems[0] as MatchDayListItem;
            unassignToMatchDay(mdli.Value, listData);
        }

        private void unassignToMatchDay(decimal matchday, List<MatchData> matchData)
        {
            if (!_matchDayMatches.ContainsKey(matchday))
                return;
            foreach (MatchData m in matchData)
            {
                if (!_matchDayMatches[matchday].Contains(m))
                    continue;
                _matchDayMatches[matchday].Remove(m);

                ListViewItem lvi = new ListViewItem(new[] { m.MatchDate, String.Format("{0} - {1}", m.TeamAName, m.TeamBName), String.Format("{0} - {1}", m.ScoreA, m.ScoreB) });
                lvi.Tag = m;
                lvwUnassignedMatches.Items.Add(lvi);
            }

            lvwUnassignedMatches.Sorting = SortOrder.Ascending;
            lvwUnassignedMatches.Sort();
            
        }

        private void btnMoveSelection_Click(object sender, EventArgs e)
        {
            List<MatchData> listData = new List<MatchData>(lvwUnassignedMatches.CheckedItems.Count);
            for (int i = 0; i < lvwUnassignedMatches.CheckedItems.Count; i++)
            {
                Console.WriteLine(lvwUnassignedMatches.CheckedItems[i]);
                listData.Add(lvwUnassignedMatches.CheckedItems[i].Tag as MatchData);
                lvwUnassignedMatches.Items.Remove(lvwUnassignedMatches.CheckedItems[i--]);                
            }
            MatchDayListItem mdli = clbMatchDays.CheckedItems[0] as MatchDayListItem;
            assignToMatchDay(mdli.Value, listData);

            lvwUnassignedMatches.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void assignToMatchDay(decimal matchday, List<MatchData> matchData)
        {
            
            if (!_matchDayMatches.ContainsKey(matchday))
            {
                _matchDayMatches.Add(matchday, matchData);
            }
            else
            {
                _matchDayMatches[matchday].AddRange(matchData);
            }

            lvwAssignedMatches.Items.Clear();

            foreach (MatchData m in _matchDayMatches[matchday])
            {
                ListViewItem lvi = new ListViewItem(new[] { m.MatchDate, String.Format("{0} - {1}", m.TeamAName, m.TeamBName), String.Format("{0} - {1}", m.ScoreA, m.ScoreB) });
                lvi.Tag = m;
                lvwAssignedMatches.Items.Add(lvi);
            }

            lvwAssignedMatches.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            if(lvwAssignedMatches.Items.Count > 0)
                lvwAssignedMatches.Enabled = true;

            lvwAssignedMatches.Sorting = SortOrder.Ascending;            
            lvwAssignedMatches.Sort();
        }

        private void lvwUnassignedMatches_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (lvwUnassignedMatches.CheckedItems.Count > 0)
            {
                btnMoveSelection.Enabled = btnMoveAll.Enabled = true;
            }
            else
            {
                btnMoveSelection.Enabled = btnMoveAll.Enabled = false;
            }
        }

        private void btnMoveAll_Click(object sender, EventArgs e)
        {
            List<MatchData> listData = new List<MatchData>(lvwUnassignedMatches.Items.Count);
            for (int i = 0; i < lvwUnassignedMatches.Items.Count; i++)
            {
                Console.WriteLine(lvwUnassignedMatches.Items[i]);
                listData.Add(lvwUnassignedMatches.Items[i].Tag as MatchData);
                lvwUnassignedMatches.Items.Remove(lvwUnassignedMatches.Items[i--]);
            }
            MatchDayListItem mdli = clbMatchDays.CheckedItems[0] as MatchDayListItem;
            assignToMatchDay(mdli.Value, listData);
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            List<MatchData> listData = new List<MatchData>(lvwAssignedMatches.Items.Count);
            for (int i = 0; i < lvwAssignedMatches.Items.Count; i++)
            {
                Console.WriteLine(lvwAssignedMatches.Items[i]);
                listData.Add(lvwAssignedMatches.Items[i].Tag as MatchData);
                lvwAssignedMatches.Items.Remove(lvwAssignedMatches.Items[i--]);
            }
            MatchDayListItem mdli = clbMatchDays.CheckedItems[0] as MatchDayListItem;
            unassignToMatchDay(mdli.Value, listData);
        }

        private void btnCreateSpreadSheet_Click(object sender, EventArgs e)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            
            using (ExcelWriter excelWriter = new ExcelWriter(_matchDayMatches, _league, _seasonName))
            {
                excelWriter.createSpreadsheet(cts.Token);
            }
            
            //
           
        }

        private class ExcelWriter : IDisposable
        {
            private SortedList<decimal, List<MatchData>> _matchDayMatches;
            private String _league;
            private String _seasonName;

            Microsoft.Office.Interop.Excel.Application _app;

            public ExcelWriter(SortedList<decimal, List<MatchData>> matchDayMatches, String league, String seasonName)
            {
                _matchDayMatches = matchDayMatches;
                _league = league;
                _seasonName = seasonName;
                _app = new Microsoft.Office.Interop.Excel.Application();
                if (_app == null)
                {
                    throw new Exception("Konnte Excel nicht instanzieren");                    
                }
            }

            ~ExcelWriter()
            {
                Dispose(false);
            }

            public async Task createSpreadsheet(CancellationToken ct)
            {
                Workbook workbook = _app.Workbooks.Add(1);
                Worksheet sheet = (Worksheet)workbook.Worksheets[1];
                sheet.Name = _league + " " + _seasonName;
                List<Task<StatisticMatchData>> tasks = new List<Task<StatisticMatchData>>();
                //Überlegen, ob die ganze Berechnung zu parallelisieren ist.
                foreach (decimal matchday in _matchDayMatches.Keys)
                {
                    //Hole die zugeordnete Begegnungen 
                    //List<MatchData> matchDatas = _matchDayMatches[matchday];
                    
                    foreach (MatchData matchData in _matchDayMatches[matchday])
                    {
                        tasks.Add(calculateStrength(matchday, matchData, ct));
                    }                   
                   
                }

                //Berrechnungne asynchron ausführen und auf ergebnis warten
                Task<StatisticMatchData[]> allTask = Task.WhenAll(tasks);
                StatisticMatchData[] results = await allTask;
                //Ergebnisse nach Spieltag gruppieren.
                SortedList<decimal, List<StatisticMatchData>> matchDayStatistics = new SortedList<decimal,List<StatisticMatchData>>();
                for (int i = 0; i < results.Count(); i++)
                {
                    if (!matchDayStatistics.ContainsKey(results[i].MatchDay))
                    {
                        List<StatisticMatchData> list = new List<StatisticMatchData>();
                        matchDayStatistics.Add(results[i].MatchDay, list);
                    }

                    matchDayStatistics[results[i].MatchDay].Add(results[i]);
                    //sheet.Cells[i+1, 2] = String.Format("Spieltag {0}", results[i].MatchDay);
                    //workSheet_range = sheet.get_Range(cell1, cell2);
                    //workSheet_range.Merge(mergeColumns);
                }

                int headRow = 1;
                int startColumn = 2;
                foreach (decimal matchday in matchDayStatistics.Keys)
                {
                    if (matchday % 2 == 0)
                    {
                        startColumn = 12;
                    }
                    else
                    {
                        startColumn = 2;
                    }

                    //Kopf erzeugen
                    sheet.Cells[headRow, startColumn] = String.Format("Spieltag {0}", matchday);
                    var range = sheet.get_Range(String.Format("B{0}", headRow), String.Format("C{0}", headRow));
                    range.Merge();
                    sheet.Cells[headRow, startColumn + 2] = "Heim";
                    sheet.Cells[headRow, startColumn + 3] = "X";
                    sheet.Cells[headRow, startColumn + 4] = "Gast";
                    sheet.Cells[headRow, startColumn + 5] = "Ergebnis";

                    int dataRow = headRow + 1;
                    foreach (StatisticMatchData stat in matchDayStatistics[matchday])
                    {
                        sheet.Cells[dataRow, startColumn] = stat.MatchData.TeamAName;
                        sheet.Cells[dataRow, startColumn + 1] = stat.MatchData.TeamBName;
                        sheet.Cells[dataRow, startColumn + 2] = Math.Round(stat.PercentageHome,2);
                        sheet.Cells[dataRow, startColumn + 3] = Math.Round(stat.PercentageDraw,2);
                        sheet.Cells[dataRow, startColumn + 4] = Math.Round(stat.PercentageAway,2);

                        sheet.Cells[dataRow, startColumn + 5].NumberFormat = "@";
                        sheet.Cells[dataRow, startColumn + 5] = String.Format("{0} - {1}", stat.MatchData.ScoreA, stat.MatchData.ScoreB);
                        dataRow++;
                    }

                    if (matchday % 2 == 0)
                    {
                        headRow += matchDayStatistics[matchday].Count() + 2;
                    }
                }

                               
                _app.ActiveWorkbook.SaveAs(String.Format(@".\{0}_{1}.xlsx", _league, _seasonName));
            }

            private async Task<StatisticMatchData> calculateStrength(decimal matchDay, MatchData matchData, CancellationToken ct)
            {                
                Console.WriteLine("Berechne Statistiken für {0} - {1} bis Spieltag {2}", matchData.TeamAName, matchData.TeamBName, matchDay);
                StatisticMatchData statistics = new StatisticMatchData();
                statistics.MatchData = matchData;
                statistics.MatchDay = matchDay;
                //für die ersten 4 Spieltage werden keine Statistiken erzeugt,
                if (matchDay == 1 || matchDay == 2 || matchDay == 3 || matchDay == 4)
                {
                    return statistics;

                }

                calculateHomeAwayStrenght(ref statistics);

                Console.WriteLine("{0} - {1}: Heimstärke = {2};UE-Stärke = {3}; Auswärtsstärke = {4}", statistics.MatchData.TeamAName, statistics.MatchData.TeamBName,
                    statistics.StrengthHome, statistics.StrengthDraw, statistics.StrengthAway);

                calculateHomeAwayOverallStrenght(ref statistics);
                Console.WriteLine("{0} - {1}: Absolute Heimstärke = {2};Absolute UE-Stärke = {3}; Absolute Auswärtsstärke = {4}", statistics.MatchData.TeamAName, statistics.MatchData.TeamBName,
                    statistics.OverallStrengthHome, statistics.OverallStrengthDraw, statistics.OverallStrengthAway);

                Console.WriteLine("{0} - {1}: % Heim = {2:0.00};% UE = {3:0.00}; % Auswärts = {4:0.00}. Summe = {5:0.00}", statistics.MatchData.TeamAName, statistics.MatchData.TeamBName,
                    statistics.PercentageHome * 100, statistics.PercentageDraw * 100, statistics.PercentageAway * 100, (statistics.PercentageHome + statistics.PercentageDraw + statistics.PercentageAway) * 100);
                
                return statistics ;
            }

            private void calculateHomeAwayOverallStrenght(ref StatisticMatchData statisticData)
            {
                /*
                Heim-Gesamt: (Gesamtsiege(Heimteam)+Gesamtniederlagen(Gastteam) / (Gesamtspiele(Heimteam) + Gesamtspiele(Gastteam)
 
                Gesamt X : (Gesamt X (Heim) + Gesamt X (Gast) / (Gesamtspiele (Heimteam) + Gesamtspiele (Gastteam)

                Gast-Gesamt: (Gesamtsiege (Gastteam) + Gesamtniederlagen (Heimteam) / (Gesamtspiele (Gastteam) + Gesamtspiele Heimteam)
                 */

                int countTeamAOverallMatches = 0;
                int countTeamBOverallMatches = 0;
                int countTeamAWonMatches = 0;
                int countTeamALostMatches = 0;
                int countTeamBWonMatches = 0;
                int countTeamBLostMatches = 0;
                int countDrawMatches = 0;
                String teamA = statisticData.MatchData.TeamAId;
                String teamB = statisticData.MatchData.TeamBId;

                for (decimal d = statisticData.MatchDay - 1; d > 0; d--)
                {
                    List<MatchData> listMatchData = _matchDayMatches[d];

                    var listTeamAMatches = from l in listMatchData
                                           where l.TeamAId == teamA || l.TeamBId == teamA
                                           select l;

                    var listTeamBMatches = from l in listMatchData
                                           where l.TeamAId == teamB || l.TeamBId == teamB
                                           select l;


                    countTeamAOverallMatches += listTeamAMatches.Count();
                    countTeamBOverallMatches += listTeamBMatches.Count();

                    foreach (var p in listTeamAMatches)
                    {
                        if (p.TeamAId == teamA && p.ScoreA > p.ScoreB)
                            countTeamAWonMatches++;
                        if (p.TeamBId == teamA && p.ScoreB > p.ScoreA)
                            countTeamAWonMatches++;

                        if (p.TeamAId == teamA && p.ScoreA < p.ScoreB)
                            countTeamALostMatches++;
                        if (p.TeamBId == teamA && p.ScoreB < p.ScoreA)
                            countTeamALostMatches++;

                        if (p.TeamAId == teamA && p.ScoreA == p.ScoreB)
                            countDrawMatches++;
                        if (p.TeamBId == teamA && p.ScoreB == p.ScoreA)
                            countDrawMatches++;
                    }

                    foreach (var p in listTeamBMatches)
                    {
                        if (p.TeamAId == teamB && p.ScoreA > p.ScoreB)
                            countTeamBWonMatches++;
                        if (p.TeamBId == teamB && p.ScoreB > p.ScoreA)
                            countTeamBWonMatches++;

                        if (p.TeamAId == teamB && p.ScoreA < p.ScoreB)
                            countTeamBLostMatches++;
                        if (p.TeamBId == teamB && p.ScoreB < p.ScoreA)
                            countTeamBLostMatches++;

                        if (p.TeamAId == teamB && p.TeamBId != teamA && p.ScoreA == p.ScoreB) //Unentschieden in den direkten Begegnungen zwischen TeamA und Team B 
                                                                                              //wurden schon bei der Auswertung der Liste von Team A aufgenommen
                            countDrawMatches++;
                        if (p.TeamBId == teamB && p.TeamAId != teamA && p.ScoreB == p.ScoreA)
                            countDrawMatches++;
                    }                    
                }

                statisticData.OverallStrengthHome = (double)(countTeamAWonMatches + countTeamBLostMatches) / (countTeamAOverallMatches + countTeamBOverallMatches);
                statisticData.OverallStrengthDraw = (double)(countDrawMatches)/(countTeamAOverallMatches + countTeamBOverallMatches);
                statisticData.OverallStrengthAway = (double)(countTeamBWonMatches + countTeamALostMatches) / (countTeamAOverallMatches + countTeamBOverallMatches); 
            }

            private void calculateHomeAwayStrenght(ref StatisticMatchData statisticData)
            {
                int countHomeMatchesTeamA = 0;
                int countHomeWinsTeamA = 0;
                int countAwayMatchesTeamA = 0;
                int countAwayLossTeamA = 0;
                int countAwayMatchesTeamB = 0;
                int countAwayWinsTeamB = 0;
                int countHomeMatchesTeamB = 0;
                int countHomeLossTeamB = 0;
                int countDrawMatches = 0;
                int countDraws = 0;
                String teamA = statisticData.MatchData.TeamAId;
                String teamB = statisticData.MatchData.TeamBId;

                

                // Wahrscheinlichkeit für Heim/Draw/Gast rückwirkend basierend auf Daten exklusive aktuellen Spieltag
                for (decimal d = statisticData.MatchDay-1; d > 0; d--)
                {
                    List<MatchData> listMatchData = _matchDayMatches[d];
                    #region Datenextraktion zur Berechnung der Heimstärke der Heimmannschaft
                    //Zählen der Heimsiege und der Heimspiele in Total
                    
                    var listHomeMatches = from l in listMatchData
                                          where l.TeamAId == teamA
                                          select l;

                    countHomeMatchesTeamA += listHomeMatches.Count();

                    foreach (var p in listHomeMatches)
                    {
                        if (p.ScoreA > p.ScoreB)
                            countHomeWinsTeamA++;
                    }


                    //Zählen der Auswärtsniederlagen und der Auswärtsspiele in Total
                    var listAwayMatches = from l in listMatchData
                                          where l.TeamBId.Equals(teamA, StringComparison.InvariantCultureIgnoreCase)
                                          select l;

                    countAwayMatchesTeamA += listAwayMatches.Count();

                    foreach (var p in listAwayMatches)
                    {
                        if (p.ScoreB < p.ScoreA)
                            countAwayLossTeamA++;
                    }
                    #endregion

                    #region Datenextraktion zur Berechnung der Unentschiedenstärke für beide Mannschaften

                    var listDrawMatches = from l in listAwayMatches
                                          where l.TeamAId == teamA || l.TeamBId == teamA || l.TeamAId == teamB || l.TeamBId == teamB
                                          select l;

                    countDrawMatches += listDrawMatches.Count();

                    foreach (var p in listDrawMatches)
                    {
                        if (p.ScoreA == p.ScoreB)
                            countDraws++;
                    }
                    #endregion

                    #region Datenextraktion zu Berechnung der Auswärtsstärke der Gastmannschaft
                    //Auswärtsstärke: (Auswärtssiege+Heimniederlagen) / ( Auswärtsspiele+Heimspiele)
                    //Zählen der Auswärtssiege und der Auswärtsspiele in Total
                    
                    listAwayMatches = from l in listMatchData
                                      where l.TeamBId == teamB
                                      select l;

                    countAwayMatchesTeamB += listAwayMatches.Count();
                    foreach (var p in listAwayMatches)
                    {
                        if (p.ScoreA < p.ScoreB)
                            countAwayWinsTeamB++;
                    }

                    //Zählen der Heimlniederlagen und der Heimspiel der Gastmannschaft in Total

                    listHomeMatches = from l in listMatchData
                                      where l.TeamAId == teamB
                                      select l;

                    countHomeMatchesTeamB += listHomeMatches.Count();

                    foreach (var p in listHomeMatches)
                    {
                        if (p.ScoreA < p.ScoreB)
                            countHomeLossTeamB++;
                    }
                    #endregion

                }

                statisticData.StrengthHome = (double)(countHomeWinsTeamA + countAwayLossTeamA) / (countHomeMatchesTeamA + countAwayMatchesTeamA);
                statisticData.StrengthDraw =((double)countDraws/(double)countDrawMatches);
                statisticData.StrengthAway = (double)(countAwayWinsTeamB + countHomeLossTeamB) / (countAwayMatchesTeamB + countHomeMatchesTeamB);
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(Boolean disposing)
            {
                try
                {
                    if (_app != null)
                    {
                        _app.Quit();
                        Marshal.ReleaseComObject(_app);
                        _app = null;
                    }
                }
                finally
                {
                }
            }
        }

        private class StatisticMatchData
        {                       
            public MatchData MatchData { get; set; }
            public decimal MatchDay { get; set; }
            public double StrengthHome { get; set; }
            public double StrengthDraw { get; set; }
            public double StrengthAway { get; set; }
            public double OverallStrengthHome { get; set; }
            public double OverallStrengthDraw { get; set; }
            public double OverallStrengthAway { get; set; }
            public double PercentageHome
            {
                get
                {
                    return ((StrengthAway + OverallStrengthHome) / 2) * 100;
                }
            }
            public double PercentageDraw
            {
                get
                {
                    return ((StrengthDraw + OverallStrengthDraw) / 2) * 100;
                }
            }

            public double PercentageAway
            {
                get
                {
                    return ((StrengthAway + OverallStrengthAway) / 2) * 100;
                }
            }
        }

        class MatchData
        {
            public String MatchId{get;set;}
            public String MatchDate{get;set;}
            public String TeamAId{get;set;}
            public String TeamBId{get;set;}
            public String TeamAName{get;set;}
            public String TeamBName { get; set; }
            public int ScoreA { get; set; }
            public int ScoreB { get; set; }
            public String HalftimeScore { get; set; }
        }

        class MatchDayListItem
        {
            private String _name;
            private Decimal _value;

            public String Name
            {
                get { return _name; }
            }

            public Decimal Value
            {
                get { return _value; }
            }

            

            public MatchDayListItem(String name, Decimal value)
            {
                _name = name;
                _value = value;
            }
        }                
    }
}
