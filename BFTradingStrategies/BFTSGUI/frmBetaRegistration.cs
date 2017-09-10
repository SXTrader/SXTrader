using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    public partial class frmBetaRegistration : Form
    {
        private String m_strFile = SXDirs.ApplPath + @"\email.dat";
        private Boolean m_ok = false;
        
        public frmBetaRegistration()
        {
            InitializeComponent();
            doLanguage();
            if(readFile())
                btnOK_Click(this, new EventArgs());
        }

        public Boolean OK
        {
            get
            {
                return m_ok;
            }
        }

        private Boolean readFile()
        {
            if (File.Exists(m_strFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(m_strFile);
                XmlElement root = doc.DocumentElement;
                txtEmail.Text = root.ChildNodes[0].InnerText;
                return true;
            }
            return false;
        }

        private void writeFile()
        {
            if (File.Exists(m_strFile))
                File.Delete(m_strFile);

            XmlTextWriter writer = new XmlTextWriter(m_strFile, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            writer.WriteStartElement("email");
            writer.Close();

            XmlDocument doc = new XmlDocument();
            doc.Load(m_strFile);
            XmlNode node = doc.ChildNodes[1];
            node.InnerText = txtEmail.Text;
            doc.Save(m_strFile);
        }

        private void doLanguage()
        {
            lblBetaRegistration.Text = BFTSGUI.strBetaAddress;
            this.Text = BFTSGUI.strBetaDialog;
            chkRemember.Text = BFTSGUI.strRemember;
            btnOK.Text = BFTSGUI.strBtnOK;
            btnCancel.Text = BFTSGUI.strBtnCancel;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(-1);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtEmail.Text == String.Empty)
                    return;
                String filename = "http://www.sxtrader.net/" + txtEmail.Text.Replace("@", "at") + ".xml";

                XmlDocument doc = new XmlDocument();

                doc.Load(filename);
                m_ok = true;
                if (chkRemember.Checked)
                    writeFile();
                Close();
            }
            catch
            {
                m_ok = false;
            }
        }
    }
}
