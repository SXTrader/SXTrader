using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using com.markus_heid.bftradingstrategies.livescoreparser;

namespace BFStrategiesTraderGUI
{
    public partial class formEntry : Form
    {
        public LiveScoreParser m_parser;
        public formEntry()
        {
            InitializeComponent();
            m_parser = LiveScoreParser.Instance;
        }

        private void btnUEStrategie_Click(object sender, EventArgs e)
        {
            formUELay form = formUELay.Instance;
            if (!form.IsInitialized)
                form.initUEWatcher(m_parser);
            form.Show();
        }
    }
}
