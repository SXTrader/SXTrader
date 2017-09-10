using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.common
{
    public partial class ctlLivescores : UserControl
    {
        XDocument _feedLS1 = null;
        XDocument _feedLS2 = null;
        String _pathLocalLS1 = SXDirs.ApplPath + @"\BFLSLocalMapping.xml";
        String _pathLocalLS2 = SXDirs.ApplPath + @"\BFLSLocalMapping2.xml";
        private DataGridViewCellEventArgs _mouseLocation;
        
        public ctlLivescores()
        {
            InitializeComponent();
            _feedLS1 = loadFeed(_pathLocalLS1);
            _feedLS2 = loadFeed(_pathLocalLS2);
            
            if (_feedLS1 != null)
            {
                buildGrid(_feedLS1, dgvLivescore1);
            }

            if (_feedLS2 != null)
            {
                buildGrid(_feedLS2, dgvLivescore2);
            }
        }

        private void buildGrid(XDocument feed, DataGridView grid)
        {
            try
            {
                if (feed == null)
                    return;

                //foreach (DataGridViewRow r in grid.Rows)
                while(grid.Rows.Count > 0)
                {
                    DataGridViewRow r = grid.Rows[0];
                    
                    grid.Rows.Remove(r);
                    r.Dispose();
                }
                grid.Tag = feed;

                var mappingTeam = from map in feed.Descendants("Map")
                                  select new
                                  {
                                      Betfair = map.Attribute("betfair").Value.Trim(),
                                      GamblerWiki = map.Attribute("livescore").Value.Trim()
                                  };
                foreach (var mapping in mappingTeam)
                {
                    // Betfair
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = mapping.Betfair;
                    row.Cells.Add(cell);

                    // Livescore
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = mapping.GamblerWiki;
                    row.Cells.Add(cell);

                    ToolStripMenuItem tsiDelLS = new ToolStripMenuItem();
                    tsiDelLS.Text = HistoryGraph.strDelLSMapping;
                    tsiDelLS.Click += new EventHandler(tsiDelLS_Click);

                    ContextMenuStrip strip = new ContextMenuStrip();

                    strip.Tag = grid;
                    row.ContextMenuStrip = strip;
                    row.ContextMenuStrip.Items.Add(tsiDelLS);

                    grid.Rows.Add(row);
                }
                grid.Sort(grid.Columns[0], ListSortDirection.Ascending);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void buildGrindWithText(XDocument feed, DataGridView grid, String text)
        {
            try
            {
                if (feed == null)
                    return;

                //foreach (DataGridViewRow r in grid.Rows)
                while(grid.Rows.Count > 0)
                {
                    DataGridViewRow r = grid.Rows[0];
                    grid.Rows.Remove(r);
                    r.Dispose();
                }
                grid.Tag = feed;
                var mappingTeam = from map in feed.Descendants("Map")
                                  select new
                                  {
                                      Betfair = map.Attribute("betfair").Value.Trim(),
                                      GamblerWiki = map.Attribute("livescore").Value.Trim()
                                  };
                foreach (var mapping in mappingTeam)
                {
                    if (mapping.Betfair.StartsWith(text))
                    {
                        // Betfair
                        DataGridViewRow row = new DataGridViewRow();
                        DataGridViewCell cell = new DataGridViewTextBoxCell();
                        cell.Value = mapping.Betfair;
                        row.Cells.Add(cell);

                        // Livescore
                        cell = new DataGridViewTextBoxCell();
                        cell.Value = mapping.GamblerWiki;
                        row.Cells.Add(cell);

                        ToolStripMenuItem tsiDelLS = new ToolStripMenuItem();
                        tsiDelLS.Text = HistoryGraph.strDelLSMapping;
                        tsiDelLS.Click += new EventHandler(tsiDelLS_Click);

                        ContextMenuStrip strip = new ContextMenuStrip();
                        strip.Tag = grid;

                        row.ContextMenuStrip = strip;
                        row.ContextMenuStrip.Items.Add(tsiDelLS);
                        
                        grid.Rows.Add(row);
                    }
                }
                grid.Sort(grid.Columns[0], ListSortDirection.Ascending);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private XDocument loadFeed(XmlReader reader)
        {
            try
            {
                return XDocument.Load(reader);
            }
            catch (FileNotFoundException fnfe)
            {
                
                return null;
            }
        }

        private XDocument loadFeed(String path)
        {
            try
            {
                return XDocument.Load(path, LoadOptions.SetBaseUri);
            }
            catch (FileNotFoundException fnfe)
            {

                return null;
            }
        }

        private void ctlLivescores_SizeChanged(object sender, EventArgs e)
        {

        }

        private void ctlLivescores_Resize(object sender, EventArgs e)
        {
            try
            {
                dgvLivescore1.Location = new Point(lblBetfairName.Left, lblBetfairName.Bottom + 5);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void ctlLivescores_Enter(object sender, EventArgs e)
        {
            
        }

        void tsiDelLS_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridView grid = (DataGridView)((ToolStripMenuItem)sender).Owner.Tag;
                XDocument feed = (XDocument)grid.Tag;
                DataGridViewRow row = grid.Rows[_mouseLocation.RowIndex];

                if (row == null)
                    return;
                String strMapping = String.Format("{0} <-> {1}", row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());

                //DataGridViewRow row =             
                DialogResult result = MessageBox.Show(String.Format(HistoryGraph.strDelLSConfirm, strMapping), HistoryGraph.strDelLSCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;


                XmlDocument d = new XmlDocument();

                d.Load(feed.CreateReader());

                String strXPathBetfair = String.Format("/root/Map[@betfair='{0}' and @livescore='{1}']", row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
                XmlNode t = d.SelectSingleNode(strXPathBetfair);

                t.ParentNode.RemoveChild(t);                

                d.Save(feed.BaseUri.Replace("file:///", String.Empty));

                feed = loadFeed(feed.BaseUri);

                buildGrid(feed, grid);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void txtBetfairName_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBetfairName.Text.Trim() != String.Empty)
                {
                    buildGrindWithText(_feedLS1, dgvLivescore1, txtBetfairName.Text);
                }
                else
                {
                    buildGrid(_feedLS1, dgvLivescore1);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void txtBetfair2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBetfair2.Text.Trim() != String.Empty)
                {
                    buildGrindWithText(_feedLS2, dgvLivescore2, txtBetfair2.Text);
                }
                else
                {
                    buildGrid(_feedLS2, dgvLivescore2);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            _mouseLocation = e;
        }
    }
}
