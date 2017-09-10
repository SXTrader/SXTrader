using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    public partial class frmConfig : Form
    {
        private TreeNode nodeRoot;
        private IPlugin[] m_colPlugins = null;

        public IPlugin[] Plugins
        {
            get
            {
                return m_colPlugins;
            }
            set
            {
                m_colPlugins = value;
            }
        }
        
        public frmConfig()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = BFTSGUI.strConfiguration;
                this.btnOK.Text = BFTSGUI.strBtnOK;
                this.btnApply.Text = BFTSGUI.strBtnApply;
                this.btnCancel.Text = BFTSGUI.strBtnCancel;


                nodeRoot = new TreeNode(BFTSGUI.strConfiguration);
                TreeNode node = new TreeNode(BFTSGUI.strGeneral);
                ctlConfig ctlConfig = new ctlConfig();
                node.Tag = ctlConfig;
                nodeRoot.Nodes.Add(node);
                tvwConfigurations.Nodes.Add(nodeRoot);

                //Add each plugin to the treeview
                
                foreach (IPlugin pluginOn in this.Plugins)
                {
                    if (pluginOn.ConfigurationInterface != null)
                    {
                        TreeNode newNode = new TreeNode(pluginOn.Name);
                        newNode.Tag = pluginOn.ConfigurationInterface;
                        this.nodeRoot.Nodes.Add(newNode);
                        newNode = null;
                    }
                }
                
                tvwConfigurations.ExpandAll();

                foreach (Control c in this.pnlConfig.Controls)
                {
                    this.pnlConfig.Controls.Remove(c);
                }


                ctlConfig.Dock = DockStyle.Fill;
                ctlConfig.Width = pnlConfig.Width;
                ctlConfig.Height = pnlConfig.Height;
                this.pnlConfig.Controls.Add(ctlConfig);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tvwConfigurations_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                foreach (Control c in this.pnlConfig.Controls)
                {
                    this.pnlConfig.Controls.Remove(c);
                   // c.Dispose();
                }

                UserControl control = (UserControl)e.Node.Tag;


                if (control != null)
                {
                    control.Width = pnlConfig.Width;
                    control.Height = pnlConfig.Height;
                    this.pnlConfig.Controls.Add(control);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.pnlConfig.Controls.Count > 0)
                {
                    IConfiguration config = (IConfiguration)this.pnlConfig.Controls[0];
                    config.save();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.pnlConfig.Controls.Count > 0)
                {
                    IConfiguration config = (IConfiguration)this.pnlConfig.Controls[0];
                    config.save();
                }
                this.Close();
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
