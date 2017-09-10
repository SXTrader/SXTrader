using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatisticRetrivier
{
    public partial class ctlCreateSaisonLoadData : UserControl
    {
        private EventHandler<CreateSeasonStepTwoEventArgs> _createSeasonStepTwo;

        public event EventHandler<CreateSeasonStepTwoEventArgs> CreateSeasonStepTwo
        {
            add
            {
                if (_createSeasonStepTwo == null || !_createSeasonStepTwo.GetInvocationList().Contains(value))
                    _createSeasonStepTwo += value;
            }
            remove
            {
                _createSeasonStepTwo -= value;
            }
        }

        public ctlCreateSaisonLoadData()
        {
            InitializeComponent();
        }

        private void ctlCreateSaisonLoadData_Load(object sender, EventArgs e)
        {
            Task t = getLeaguesAsync();
        }

        private async Task getLeaguesAsync()
        {
            cbxLeague.Enabled = dtpFrom.Enabled = dtpTo.Enabled = false;
            
            String leaguesString = await Helpers.DoWebRequest("http://www.sxtrader.net/LSHistoryDB/getAllDevisions.php");

            String[] leaguesSplit = leaguesString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            List<String> listLeagues = new List<string>(leaguesSplit.Length);
            foreach (String s in leaguesSplit)
            {
                listLeagues.Add(s);
            }

            BindingSource bs = new BindingSource();
            bs.DataSource = listLeagues;
            cbxLeague.DataSource = bs;
            cbxLeague.Enabled = dtpFrom.Enabled = dtpTo.Enabled = true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_createSeasonStepTwo != null)
                _createSeasonStepTwo(this, new CreateSeasonStepTwoEventArgs(cbxLeague.SelectedItem.ToString(), dtpFrom.Value, dtpTo.Value));
        }
    }

    public class CreateSeasonStepTwoEventArgs : EventArgs
    {
        private String _league;
        private DateTime _from;
        private DateTime _to;

        public String League
        {
            get
            {
                return _league;
            }
        }

        public DateTime From
        {
            get
            {
                return _from;
            }
        }

        public DateTime To
        {
            get
            {
                return _to;
            }
        }

        public CreateSeasonStepTwoEventArgs(String league, DateTime from, DateTime to)
        {
            _league = league;
            _from = from;
            _to = to;
        }
    }
}
