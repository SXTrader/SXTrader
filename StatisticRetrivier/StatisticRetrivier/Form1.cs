using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace StatisticRetrivier
{
    public partial class Form1 : Form
    {
        private String _league;
        private DateTime _from;
        private DateTime _to;
        private String _season;
        private decimal _matchDays;
        public Form1()
        {
            InitializeComponent();

           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void mniCreateSeason_Click(object sender, EventArgs e)
        {
            ctlCreateSaisonLoadData ctl = new ctlCreateSaisonLoadData();
            ctl.CreateSeasonStepTwo += ctl_CreateSeasonStepTwo;
            ctl.Dock = DockStyle.Fill;
            ctl.Visible = true;
            pnlControls.Controls.Add(ctl);
        }

        void ctl_CreateSeasonStepTwo(object sender, CreateSeasonStepTwoEventArgs e)
        {
            _league = e.League;
            _from = e.From;
            _to = e.To;

            pnlControls.Controls.Clear();

            ctlCreateSeasonDefineSeason ctl = new ctlCreateSeasonDefineSeason();
            ctl.CreateSeasonStepThree += ctl_CreateSeasonStepThree;
            ctl.Dock = DockStyle.Fill;
            ctl.Visible = true;
            pnlControls.Controls.Add(ctl);
        }

        void ctl_CreateSeasonStepThree(object sender, CreateSeasonStepThreeEventArgs e)
        {
            _season = e.Season;
            _matchDays = e.MatchDays;
            pnlControls.Controls.Clear();

            ctlCreateSeasonAssignMatches ctl = new ctlCreateSeasonAssignMatches(_matchDays, _league,_season, _from, _to);
            ctl.Dock = DockStyle.Fill;
            ctl.Visible = true;
            pnlControls.Controls.Add(ctl);
        }

        
    }
}
