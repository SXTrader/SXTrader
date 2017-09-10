using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Reflection;
using System.Xml;
using System.IO;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                if (!checkForUpdates())
                {                    
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmMain());
                }
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteMessage("MAIN", "SXTrader will be closed due to an Exception!");
                ExceptionWriter.Instance.WriteException(e);
            }
        }



        private static bool checkForUpdates()
        {
            String filename = "http://www.sxtrader.net/SXTraderVersion2.x.xml";
            try
            {
                /*
                Type type = GetType();
                Assembly assembly = Assembly.GetAssembly(type);
                AssemblyName assemblyName = assembly.GetName();
                
                Version version = assemblyName.Version;
                 */
                
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNode rootNode = doc.ChildNodes[1];
                XmlAttribute attr = rootNode.Attributes[0];
                // Neue Version?
                if (!Application.ProductVersion.Equals(attr.Value))
                {
                    DialogResult dr = DialogResult.None;
                    XmlAttribute attr2 = rootNode.Attributes["infoUrl"];
                    if (attr2 != null)
                    {
                        frmUpdateInfo dlgUpdateInfo = new frmUpdateInfo();
                        dlgUpdateInfo.UpdateInfo = attr2.Value;
                        dr = dlgUpdateInfo.ShowDialog();
                    }
                    else
                    {
                        String updateInfo = "There's a new version of SXTrader ready to download.\r\n" +
                                          "Your current version is {0}. The downloadable version is {1}.\r\n" +
                                          "If you choose 'Yes' below a browser window to the download location\r\n" +
                                          "will be opened.";
                        updateInfo = String.Format(updateInfo, Application.ProductVersion, attr.Value);
                        dr = MessageBox.Show(updateInfo, "New version of SXTrader available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    }
                    if (dr == DialogResult.Yes || dr == DialogResult.OK)
                    {
                        System.Diagnostics.Process.Start(rootNode.InnerText);
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (System.Net.WebException)
            {
                MessageBox.Show("The SXTrader-Server seems to be unavailable. Please try again lager", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);                
            }
            catch (Exception)
            {

            }

            return false;
        }
    }
}
