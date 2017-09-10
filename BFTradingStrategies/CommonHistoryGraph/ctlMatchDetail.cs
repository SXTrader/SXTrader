using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.common
{
    public partial class ctlMatchDetail : UserControl
    {
        
        public ctlMatchDetail()
        {
            InitializeComponent();
        }

        public LSHistoricMatch TheMatch
        {
            set
            {
                try
                {
                    gbxMatchDetail.Text = String.Format("{0} - {1} {2}", value.TeamA, value.TeamB, value.MatchDate.ToString("dd.MM.yyyy"));                    

                    lvwMatchDetail.Items.Clear();
                    
                    clhTeamA.Text = value.TeamA;
                    clhTeamB.Text = value.TeamB;

                    foreach (LSHistoricMatchEvent matchEvent in value.Events)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = matchEvent.EventMinute.ToString();
                        if (matchEvent.TeamId == value.TeamAId)
                        {
                            item.SubItems.Add(String.Format("{0}     {1}", matchEvent.EventType.ToString(), matchEvent.InfoEvent2));
                        }
                        else
                        {
                            item.SubItems.Add(String.Empty);
                        }

                        item.SubItems.Add(matchEvent.InfoEvent1);

                        if (matchEvent.TeamId == value.TeamBId)
                        {
                            item.SubItems.Add(String.Format("{0}     {1}", matchEvent.EventType.ToString(), matchEvent.InfoEvent2));
                        }
                        else
                        {
                            item.SubItems.Add(String.Empty);
                        }

                        lvwMatchDetail.Items.Add(item);
                    }

                    lvwMatchDetail.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    if (clhTeamA.Width < 60)
                        clhTeamA.Width = 60;
                    if (clhTeamB.Width < 60)
                        clhTeamB.Width = 60;
                    if (clhPlayTime.Width < 20)
                        clhPlayTime.Width = 20;
                    if (clhScore.Width < 20)
                        clhScore.Width = 20;
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }

            }

        }

    }
}
