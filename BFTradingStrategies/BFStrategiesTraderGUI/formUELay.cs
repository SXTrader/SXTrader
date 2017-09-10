using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using com.markus_heid.bftradingstrategies.bfuestrategy;
using com.markus_heid.bftradingstrategies.livescoreparser;
using System.Diagnostics;

namespace BFStrategiesTraderGUI
{
    public partial class formUELay : Form
    {
        private static volatile formUELay instance;
        private static Object syncRoot = new Object();


        private bool m_initialized = false;
        private BFUEWatcher m_watcher;


        public static formUELay Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new formUELay();
                    }
                }

                return instance;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return m_initialized;
            }
        }

        public void initUEWatcher(LiveScoreParser parser)
        {
            m_watcher = new BFUEWatcher(parser);

            m_watcher.GoalScoredEvent += new EventHandler<BFWGoalScoredEventArgs>(GoalScoredEventHandler);
            m_watcher.PlaytimeEvent += new EventHandler<BFWPlaytimeEventArgs>(PlaytimeEventHandler);
            m_watcher.RiskWinChangedEvent += new EventHandler<BFWRiskWinChangedEventArgs>(RiskWinChangedHandler);
            m_initialized = true;
            m_watcher.initRiskWin();
        }

        private formUELay()
        {
            BFUEWatcher.MatchAddedEvent += new EventHandler<MatchAddedEventArgs>(MatchAddedEventHandler);            
            InitializeComponent();            
        }


        private Color colorFinder(String riskWin)
        {
            char[] cSeps = {'/'};
            Color color = Color.Black;
            double dRisk, dWin;
            String[] money = riskWin.Split(cSeps);
            if (money.Length == 0)
                color = Color.Red;
            else if (money.Length==1)
            {
                dWin = Double.Parse(money[0]);
                if (dWin < 0)
                    color = Color.Red;
                else
                    color = Color.LightGreen;
            }
            else if(money.Length == 2)
            {
                dRisk = Double.Parse(money[0]);
                dWin = Double.Parse(money[1]);
                if (dRisk < 0 && dWin < 0)
                    color = Color.Red;
                else if (dRisk > 0 && dWin > 0)
                    color = Color.LightGreen;
                else
                    color = Color.Orange;
            }
            return color;
        }

        #region EventHandler

        private void RiskWinChangedHandler(Object sender, BFWRiskWinChangedEventArgs e)
        {
            if (lvwUE.InvokeRequired)
            {
                lvwUE.Invoke(new EventHandler<BFWRiskWinChangedEventArgs>(RiskWinChangedHandler), new object[] { sender, e });
            }
            else
            {


                foreach (ListViewItem item in lvwUE.Items)
                {
                    if (item.Text.Equals(e.Match))
                    {
                        lvwUE.BeginUpdate();
                        item.SubItems[3].Text = e.RiskWin;
                        item.SubItems[3].BackColor = this.colorFinder(e.RiskWin);
                        lvwUE.EndUpdate();
                    }
                }
            }
        }

        private void PlaytimeEventHandler(Object sender, BFWPlaytimeEventArgs e)
        {
            if (lvwUE.InvokeRequired)
            {
                lvwUE.Invoke(new EventHandler<BFWPlaytimeEventArgs>(PlaytimeEventHandler), new object[] { sender, e });
            }
            else
            {


                foreach (ListViewItem item in lvwUE.Items)
                {
                    if (item.Text.Equals(e.Match))
                    {
                        lvwUE.BeginUpdate();
                        item.SubItems[1].Text = e.Playtime.ToString();
                        lvwUE.EndUpdate();
                    }
                }
            }
        }

        private void GoalScoredEventHandler(Object sender, BFWGoalScoredEventArgs e)
        {
            if (lvwUE.InvokeRequired)
            {
                lvwUE.Invoke(new EventHandler<BFWGoalScoredEventArgs>(GoalScoredEventHandler), new object[] { sender, e });
            }
            else
            {
                

                foreach (ListViewItem item in lvwUE.Items)
                {
                    if (item.Text.Equals(e.Team))
                    {
                        lvwUE.BeginUpdate();
                        item.SubItems[2].Text = e.Score;
                        lvwUE.EndUpdate();
                    }
                }                
            }
        }

        private void MatchAddedEventHandler(Object sender, MatchAddedEventArgs e)
        {
            if (lvwUE.InvokeRequired)
            {
                lvwUE.Invoke(new EventHandler<MatchAddedEventArgs>(MatchAddedEventHandler), new object[] { sender, e });
            }
            else
            {
                lvwUE.BeginUpdate();
                ListViewItem item = new ListViewItem(e.Match);
                item.UseItemStyleForSubItems = false;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item,"0"));
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, e.Score));
                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, e.RiskWin);
                subItem.BackColor = colorFinder(e.RiskWin);
                item.SubItems.Add(subItem);
                lvwUE.Items.Add(item);
                lvwUE.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);                
                lvwUE.EndUpdate();
                Debug.WriteLine(item.Text);
            }
        }

        #endregion
    }
}
