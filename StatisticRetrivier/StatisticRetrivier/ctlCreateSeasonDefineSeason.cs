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
    public partial class ctlCreateSeasonDefineSeason : UserControl
    {
        private EventHandler<CreateSeasonStepThreeEventArgs> _createSeasonStepThree;

        public event EventHandler<CreateSeasonStepThreeEventArgs> CreateSeasonStepThree
        {
            add
            {
                if (_createSeasonStepThree == null || !_createSeasonStepThree.GetInvocationList().Contains(value))
                    _createSeasonStepThree += value;
            }
            remove
            {
                _createSeasonStepThree -= value;
            }
        }

        public ctlCreateSeasonDefineSeason()
        {
            InitializeComponent();
            cbxSeason.SelectedIndex = 0;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_createSeasonStepThree != null)
                _createSeasonStepThree(this, new CreateSeasonStepThreeEventArgs(cbxSeason.Text, spnMatchdays.Value));
        }
    }

    public class CreateSeasonStepThreeEventArgs
    {
        private decimal _matchDays;
        private String _season;

        public decimal MatchDays
        {
            get
            {
                return _matchDays;
            }
        }

        public String Season
        {
            get
            {
                return _season;
            }
        }

        public CreateSeasonStepThreeEventArgs(String season, decimal matchdays)
        {
            _season = season;
            _matchDays = matchdays;
        }
    }
}
