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
using System.Xml.Linq;
using System.Net;

namespace LSBFMapperHelper
{
    public partial class Form1 : Form
    {
        private string FILENAME;
        private string m_filePath;
        private string m_filePath2;
        private string m_filePathGlobal1;
        private string m_filePathGlobal2;
        private string m_pathDateXml;
        private string m_pathDateXml2;
        private XmlDocument m_doc = null;
        private XmlDocument m_docChange = null;
        private XDocument m_feedMapper;
        private XDocument m_feedMapper2;
        private XDocument m_globalMapper1;
        private XDocument m_globalMapper2;

        public Form1()
        {
            FILENAME  = Application.StartupPath + @"\FilePath.xml";
            m_doc = new XmlDocument();
            InitializeComponent();
            // Falls Datei mit Pfad zur lokalen Mappingdatei noch nicht
            // existiert, dann legen einen an.
            if (!File.Exists(FILENAME))
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "XML-Datei(*.xml)|*.xml";
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                }

                m_filePath = dlg.FileName;                
                try
                {
                    m_doc.Load(FILENAME);
                }
                catch (System.IO.FileNotFoundException)
                {
                    //Datei nicht gefunden => erzeugen
                    XmlTextWriter writer = new XmlTextWriter(FILENAME, Encoding.UTF8);
                    writer.Formatting = Formatting.Indented;
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    writer.WriteStartElement("MapperFile");
                    writer.Close();
                    m_doc.Load(FILENAME);
                    XmlNode node = m_doc.ChildNodes[1];
                    node.InnerText = m_filePath;
                    m_doc.Save(FILENAME);
                }

            }
            else
            {
                m_doc.Load(FILENAME);
            }

            String str = m_doc.ChildNodes[1].ChildNodes[0].InnerText;
            m_feedMapper = XDocument.Load(str);

            str = m_doc.ChildNodes[1].ChildNodes[1].InnerText;
            m_feedMapper2 = XDocument.Load(str);

            m_filePathGlobal1 = m_doc.ChildNodes[1].ChildNodes[2].InnerText;
            m_globalMapper1 = XDocument.Load(m_filePathGlobal1);

            m_filePathGlobal2 = m_doc.ChildNodes[1].ChildNodes[3].InnerText;
            m_globalMapper2 = XDocument.Load(m_filePathGlobal2);

            /*
            String str = m_doc.ChildNodes[1].InnerText;
            char[] c = {';'};
            String[] files = str.Split(c);
            m_filePath = files[0];
            m_feedMapper = XDocument.Load(m_filePath);

            m_filePath2 = files[1];
            m_feedMapper2 = XDocument.Load(m_filePath2);
            */
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            bool exists = false;
            bool global1changed = false;
            bool global2changed = false;
            int i = 0;
            foreach (var mappings in m_feedMapper.Descendants("Map"))
            {
                exists = false;
                var map2 = from map in m_globalMapper1.Descendants("Map")
                           where map.Attribute("betfair").Value == mappings.Attribute("betfair").Value &&
                                 map.Attribute("livescore").Value == mappings.Attribute("livescore").Value
                           select new
                               {
                                   Betfair = map.Attribute("betfair").Value,
                                   LiveScore = map.Attribute("livescore").Value
                               };

                foreach (var v in map2)
                {
                    Console.WriteLine(v.ToString());
                    exists = true;
                }

                if (exists == false)
                {
                    XElement element = new XElement("Map");
                    element.SetAttributeValue("livescore", mappings.Attribute("livescore").Value);
                    element.SetAttributeValue("betfair", mappings.Attribute("betfair").Value);
                    m_globalMapper1.Element("root").Add(element);
                    m_globalMapper1.Save(m_filePathGlobal1);
                    global1changed = true;
                }

                Console.Write(i++);
                Console.WriteLine(mappings.ToString());
            }

            if (global1changed)
            {
                upload("BFLSMapping.xml", "mappingchangedate.xml", m_filePathGlobal1);
            }

            i = 0;

            foreach (var mappings in m_feedMapper2.Descendants("Map"))
            {
                exists = false;
                var map2 = from map in m_globalMapper2.Descendants("Map")
                           where map.Attribute("betfair").Value == mappings.Attribute("betfair").Value &&
                                 map.Attribute("livescore").Value == mappings.Attribute("livescore").Value
                           select new
                           {
                               Betfair = map.Attribute("betfair").Value,
                               LiveScore = map.Attribute("livescore").Value
                           };

                foreach (var v in map2)
                {
                    Console.WriteLine(v.ToString());
                    exists = true;
                }

                if (exists == false)
                {
                    XElement element = new XElement("Map");
                    element.SetAttributeValue("livescore", mappings.Attribute("livescore").Value);
                    element.SetAttributeValue("betfair", mappings.Attribute("betfair").Value);
                    m_globalMapper2.Element("root").Add(element);
                    m_globalMapper2.Save(m_filePathGlobal2);
                    global2changed = true;
                }

                Console.Write(i++);
                Console.WriteLine(mappings.ToString());
            }

            if (global2changed)
            {
                upload("BFLSMapping2.xml", "mappingchangedate2.xml", m_filePathGlobal2);
            }
            if (txtBetfair.Text == String.Empty || txtGamblerWiki.Text == String.Empty)
            {
                return;
            }

        }

        private void upload(String mapperFile, String dateFile, String globalFile)
        {
            int iPos = globalFile.LastIndexOf('\\');
            m_pathDateXml = globalFile.Substring(0, iPos + 1) + dateFile/*"mappingchangedate.xml"*/;
            XmlNode node;

            m_docChange = new XmlDocument();
            try
            {
                m_docChange.Load(m_pathDateXml);
            }
            catch (System.IO.FileNotFoundException)
            {
                //Datei nicht gefunden => erzeugen
                XmlTextWriter writer = new XmlTextWriter(m_pathDateXml, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartElement("ChangeTime");
                writer.Close();
                m_docChange.Load(m_pathDateXml);
                node = m_doc.ChildNodes[1];
                node.InnerText = File.GetLastWriteTime(m_filePath).ToString(); ;
                m_docChange.Save(m_pathDateXml);
            }

            node = m_docChange.ChildNodes[1];

            node.InnerText = File.GetLastWriteTime(globalFile).Ticks.ToString(); ;
            m_docChange.Save(m_pathDateXml);

            SaveOnFtP(mapperFile, dateFile,globalFile);

            /*
            iPos = m_filePath.LastIndexOf('\\');
            m_pathDateXml2 = m_filePath.Substring(0, iPos + 1) + "mappingchangedate2.xml";

            m_docChange = new XmlDocument();
            try
            {
                m_docChange.Load(m_pathDateXml2);
            }
            catch (System.IO.FileNotFoundException)
            {
                //Datei nicht gefunden => erzeugen
                XmlTextWriter writer = new XmlTextWriter(m_pathDateXml2, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartElement("ChangeTime");
                writer.Close();
                m_docChange.Load(m_pathDateXml2);
                node = m_doc.ChildNodes[1];
                node.InnerText = File.GetLastWriteTime(m_filePath2).ToString(); ;
                m_docChange.Save(m_pathDateXml2);
            }

            node = m_docChange.ChildNodes[1];

            node.InnerText = File.GetLastWriteTime(m_filePath2).Ticks.ToString(); ;
            m_docChange.Save(m_pathDateXml2);
            */
            //SaveOnFtP();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            int iPos = m_filePath.LastIndexOf('\\');
            m_pathDateXml = m_filePath.Substring(0,iPos+1) + "mappingchangedate.xml";
            XmlNode node;

            m_docChange = new XmlDocument();
            try
            {
                m_docChange.Load(m_pathDateXml);
            }
            catch (System.IO.FileNotFoundException)
            {
                //Datei nicht gefunden => erzeugen
                XmlTextWriter writer = new XmlTextWriter(m_pathDateXml, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartElement("ChangeTime");
                writer.Close();
                m_docChange.Load(m_pathDateXml);
                node = m_doc.ChildNodes[1];
                node.InnerText = File.GetLastWriteTime(m_filePath).ToString(); ;
                m_docChange.Save(m_pathDateXml);
            }

            node = m_docChange.ChildNodes[1];

            node.InnerText = File.GetLastWriteTime(m_filePath).Ticks.ToString(); ;
            m_docChange.Save(m_pathDateXml);

            iPos = m_filePath.LastIndexOf('\\');
            m_pathDateXml2 = m_filePath.Substring(0, iPos + 1) + "mappingchangedate2.xml";

            m_docChange = new XmlDocument();
            try
            {
                m_docChange.Load(m_pathDateXml2);
            }
            catch (System.IO.FileNotFoundException)
            {
                //Datei nicht gefunden => erzeugen
                XmlTextWriter writer = new XmlTextWriter(m_pathDateXml2, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartElement("ChangeTime");
                writer.Close();
                m_docChange.Load(m_pathDateXml2);
                node = m_doc.ChildNodes[1];
                node.InnerText = File.GetLastWriteTime(m_filePath2).ToString(); ;
                m_docChange.Save(m_pathDateXml2);
            }

            node = m_docChange.ChildNodes[1];

            node.InnerText = File.GetLastWriteTime(m_filePath2).Ticks.ToString(); ;
            m_docChange.Save(m_pathDateXml2);

            //SaveOnFtP();

        }

        private void SaveOnFtP(String mapperFile, String dateFile, String globalFile)
        {
            String fullMapperFile = String.Format("ftp://ftp.sxtrader.net/public_html/{0}", mapperFile);

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(fullMapperFile);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("sxtrade1", "02AKGOX9");
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            FileStream stream = File.OpenRead(globalFile);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
            request.UsePassive = false;
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();

            String fullDateFile = String.Format("ftp://ftp.sxtrader.net/public_html/{0}", dateFile);
            request = (FtpWebRequest)FtpWebRequest.Create(fullDateFile);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("sxtrade1", "02AKGOX9");
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            stream = File.OpenRead(m_pathDateXml);
            buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
            request.UsePassive = false;
            reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();

            /*

            request = (FtpWebRequest)FtpWebRequest.Create("ftp://ftp.markus-heid.com/public_html/BFLSMapping2.xml");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("kgwfnzdk", "dfJW34BGeS");
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            stream = File.OpenRead(m_filePath2);
            buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();

            request = (FtpWebRequest)FtpWebRequest.Create("ftp://ftp.markus-heid.com/public_html/mappingchangedate2.xml");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("kgwfnzdk", "dfJW34BGeS");
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            stream = File.OpenRead(m_pathDateXml2);
            buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();
             * */
        }
        
    }
}
