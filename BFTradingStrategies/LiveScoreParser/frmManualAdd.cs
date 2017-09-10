using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Diagnostics;
//using BFUEStrategy;


namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    public partial class frmManualAdd : Form
    {
        private LiveScoreParser m_parser = LiveScoreParser.Instance;
        private LiveScore2Parser m_parser2 = LiveScore2Parser.Instance;
        private bool m_isLiveScore2 = false;
        private String m_match = String.Empty;
        private static DateTime m_dtsFrom = DateTime.Now.Subtract(new TimeSpan(2, 0, 0));
        private static DateTime m_dtsTo = DateTime.Now.AddHours(2);
        //private ctlLayDraw_v2 m_ctlLayDraw;

        public frmManualAdd()
        {           
            InitializeComponent();
            lvwLivescores.Columns[0].Width = lvwLivescores.Width;

            doLanguageInitialization();

            //m_dtsFrom = DateTime.Now.Subtract(new TimeSpan(2, 0, 0));
            //m_dtsTo = DateTime.Now.AddHours(2);

            dtpFrom.Value = m_dtsFrom;
            dtpTo.Value = m_dtsTo;
        }

        public String Match
        {
            set
            {
                m_match = value.Trim();
            }
            get
            {
                return m_match;
            }
        }

        public bool IsLiveScore2
        {
            set
            {
                m_isLiveScore2 = value;
                doLanguageInitialization();
            }
            get
            {
                return m_isLiveScore2;
            }
        }

        public IScore Livescore
        {
            get
            {
                return (IScore)this.lvwLivescores.SelectedItems[0].Tag;
            }
        }

        private void doLanguageInitialization()
        {
            //btnCancel.Text = LSResources.strCancel;
            //btnConnect.Text = LSResources.strConnect;

            //lblSelectionRange.Text = LSResources.strSelectionRange;
            //lblFrom.Text = LSResources.strFrom;
            //lblTo.Text = LSResources.strTo;

            if (!m_isLiveScore2)
                this.Text = LSResources.strTitle;
            else
                this.Text = LSResources.strTitel2;
            //this.lvwLivescores.Columns[0].Text = LSResources.strLivescoreMatches;
            //this.lvwLivescores.Columns[1].Text = LSResources.strStartTime;            
        }

        private void buildLiveScoreList()
        {
            lvwLivescores.Items.Clear();

            int iCounter = 0;
            if (!m_isLiveScore2)
                iCounter = m_parser.LiveList.Count;
            else
                iCounter = m_parser2.LiveList.Count;

            for (int i = 0; i < iCounter; i++)
            {

                KeyValuePair<int, IScore> keyValue;
                if (!m_isLiveScore2)
                    keyValue = m_parser.LiveList.ElementAt(i);
                else
                    keyValue = m_parser2.LiveList.ElementAt(i);

                IScore score = (IScore)keyValue.Value;
                if (score.Ended == true)
                {
                    

                    continue;
                }
                /*
                TimeSpan span = dtsNow.Subtract(score.StartDTS);
                if (span.TotalHours > 2 || span.TotalHours < -2)
                    continue;
                 */
                if (score.StartDTS < m_dtsFrom || score.StartDTS > m_dtsTo)
                    continue;


                ListViewItem item = new ListViewItem(score.getLiveMatch());
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, score.StartDTS.ToString()));
                item.Tag = score;
                lvwLivescores.Items.Add(item);
            }


            lvwLivescores.Sort();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (this.lvwLivescores.SelectedItems.Count == 0)
            {
                MessageBox.Show(LSResources.strManualConnectionAdvise, "Information",MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ((IScore)this.lvwLivescores.SelectedItems[0].Tag).BetfairMatch = this.Match;
                this.Close();
            }
        }

        private void frmManualAdd_Shown(object sender, EventArgs e)
        {
            txtBFMatch.Text = m_match;
            buildLiveScoreList();
           // this.txtBFMatch.Text = m_ctlLayDraw.UnaddedMatches[0].Match;
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            m_dtsFrom = dtpFrom.Value;
            buildLiveScoreList();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            m_dtsTo = dtpTo.Value;
            buildLiveScoreList();
        }
    }
}
