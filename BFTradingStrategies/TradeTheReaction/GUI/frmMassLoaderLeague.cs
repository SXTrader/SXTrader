using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.muk;
using System.IO;

namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
   
    public partial class frmMassLoaderLeague : Form
    {
        private MassLoaderLeagueInfos _infos;

        public MassLoaderLeagueInfos Infos { get { return _infos; } }

        public frmMassLoaderLeague()
        {
            InitializeComponent();

            _infos = new MassLoaderLeagueInfos();

            clbLeagues.Items.Clear();

            foreach (String league in SXLeagues.Instance.Keys)
            {
                clbLeagues.Items.Add(league);
            }
        }

        private void btnLoadDialog_Click(object sender, EventArgs e)
        {
            try
            {
                String templatePath =SXDirs.TemplatePath + @"\TTR";
                if (!Directory.Exists(templatePath))
                    Directory.CreateDirectory(templatePath);

                ofdTemplate.InitialDirectory = templatePath;
                ofdTemplate.Filter = "SXTrader Template files (*.sxtempl)|*.sxtempl";
                ofdTemplate.RestoreDirectory = true;
                if (ofdTemplate.ShowDialog() == DialogResult.OK)
                {
                    //Aus Template aufbauen
                    _infos.TemplatePath = ofdTemplate.FileName;
                    txtTemplate.Text = _infos.TemplatePath;
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
            //foreach (Object obj in clbLeagues.CheckedItems)
            for(int i = 0; i < clbLeagues.CheckedItems.Count; i++)
            {
                String league = clbLeagues.CheckedItems[i] as String;
                if (league != null)
                    _infos.addLeagueToList(league);
            }
        }
    }


    public class MassLoaderLeagueInfos
    {
        private IList<String> _list;
        private String _templatePath;

        public String TemplatePath
        {
            get { return _templatePath; }
            set { _templatePath = value; }
        }

        public void addLeagueToList(String league)
        {
            if (!_list.Contains(league))
                _list.Add(league);
        }

        public String[] getSelectedLeagues()
        {
            return _list.ToArray<String>();
        }

        public MassLoaderLeagueInfos()
        {
            _list = new List<String>();
        }
    }
}
