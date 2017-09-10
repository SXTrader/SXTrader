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
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Globalization;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk;


namespace net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj
{
    public partial class frmIPTradeConfig : Form
    {
        private BFUEFBIPTraderConfigList _list;
        public event EventHandler<BFUEIPTraderDialogCloseEventArgs> CloseIPTConfigDialog;
        public frmIPTradeConfig()
        {
            InitializeComponent();
            _list = new BFUEFBIPTraderConfigList();
        }


        public BFUEFBIPTraderConfigList ConfigList
        {
            set
            {
                try
                {
                    _list = value;
                    // Bisherigen Konfigurationsbildschirm zurücksetzen
                    ctlIPTElementEdit1.IPTConfigElement = null;

                    foreach (Control ctrl in pnlIPDescs2.Controls)
                    {
                        pnlIPDescs2.Controls.Remove(ctrl);
                        ctrl.Dispose();
                    }
                    pnlIPDescs2.AutoScroll = true;

                    // Neue Liste aufbauen
                    foreach (BFUEFBIPTraderConfigElement element in _list)
                    {
                        ctlIPTElement newElement = new ctlIPTElement(element);
                        newElement.EditIPTConfigElement += new EventHandler<BFUEIPTElementEditEventArgs>(EditIPTConfigElement);
                        newElement.DeleteIPTConfigElement += new EventHandler<BFUEIPTElementDeleteEventArgs>(DeleteIPTConfigElement);
                        newElement.BorderStyle = BorderStyle.FixedSingle;
                        newElement.Dock = DockStyle.Top;
                        pnlIPDescs2.Controls.Add(newElement);
                    }
                    pnlIPDescs2.AutoScroll = true;
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }
        private void ctlIPTElementEdit1_SaveIPTConfigElement(object sender, BFUEIPTElementSaveEventArgs e)
        {
            try
            {
                if (ctlIPTElementEdit1.InvokeRequired)
                {
                    IAsyncResult result = ctlIPTElementEdit1.BeginInvoke(new EventHandler<BFUEIPTElementSaveEventArgs>(ctlIPTElementEdit1_SaveIPTConfigElement), new object[] { sender, e });
                    ctlIPTElementEdit1.EndInvoke(result);
                }
                else
                {
                    if (_list != null)
                    {
                        if (!_list.Contains(e.ConfigElement))
                        {
                            _list.Add(e.ConfigElement);
                            ctlIPTElement newElement = new ctlIPTElement(e.ConfigElement);
                            newElement.EditIPTConfigElement += new EventHandler<BFUEIPTElementEditEventArgs>(EditIPTConfigElement);
                            newElement.DeleteIPTConfigElement += new EventHandler<BFUEIPTElementDeleteEventArgs>(DeleteIPTConfigElement);
                            newElement.BorderStyle = BorderStyle.FixedSingle;
                            newElement.Dock = DockStyle.Top;
                            pnlIPDescs2.Controls.Add(newElement);
                            pnlIPDescs2.AutoScroll = true;
                        }
                        else
                        {
                            int index = _list.IndexOf(e.ConfigElement);
                            _list[index] = e.ConfigElement;
                            foreach (Control ctrl in pnlIPDescs2.Controls)
                            {
                                ctlIPTElement element = ctrl as ctlIPTElement;
                                if (element != null && element.IPTConfigElement.ElementNumber == e.ConfigElement.ElementNumber)
                                {
                                    element.IPTConfigElement = e.ConfigElement;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void DeleteIPTConfigElement(object sender, BFUEIPTElementDeleteEventArgs e)
        {
            try
            {
                _list.Remove(e.ConfigElement);
                foreach (Control ctrl in pnlIPDescs2.Controls)
                {
                    ctlIPTElement element = ctrl as ctlIPTElement;
                    if (element != null && element.IPTConfigElement.ElementNumber == e.ConfigElement.ElementNumber)
                    {
                        pnlIPDescs2.Controls.Remove(ctrl);
                        ctrl.Dispose();
                        return;
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

        }

        void EditIPTConfigElement(object sender, BFUEIPTElementEditEventArgs e)
        {
            try
            {
                if (ctlIPTElementEdit1.InvokeRequired)
                {
                    IAsyncResult result = ctlIPTElementEdit1.BeginInvoke(new EventHandler<BFUEIPTElementEditEventArgs>(EditIPTConfigElement), new object[] { sender, e });
                    ctlIPTElementEdit1.EndInvoke(result);
                }
                else
                {
                    ctlIPTElementEdit1.IPTConfigElement = e.ConfigElement;
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
                if (btnOK.InvokeRequired)
                {
                    IAsyncResult result = btnOK.BeginInvoke(new EventHandler<EventArgs>(btnOK_Click), new object[] { sender, e });
                    btnOK.EndInvoke(result);
                }
                else
                {
                    EventHandler<BFUEIPTraderDialogCloseEventArgs> closeDialogHandler = CloseIPTConfigDialog;
                    if (closeDialogHandler != null)
                        closeDialogHandler(this, new BFUEIPTraderDialogCloseEventArgs(_list));
                    this.Close();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSaveTemplate.InvokeRequired)
                {
                    IAsyncResult result = btnSaveTemplate.BeginInvoke(new EventHandler<EventArgs>(btnSaveTemplate_Click), new object[] { sender, e });
                    btnSaveTemplate.EndInvoke(result);
                }
                else
                {
                    String templatePath = SXDirs.TemplatePath + @"LTD";
                    if (!Directory.Exists(templatePath))
                        Directory.CreateDirectory(templatePath);

                    if (_list.Count == 0)
                    {
                        MessageBox.Show(LayTheDraw.strCantCreateTemplate, LayTheDraw.strErrorSaveTemplate, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    sfdTemplate.InitialDirectory = templatePath;
                    sfdTemplate.Filter = "SXTrader Template files (*.sxtempl)|*.sxtempl";
                    sfdTemplate.RestoreDirectory = true;
                    sfdTemplate.OverwritePrompt = true;
                    if (sfdTemplate.ShowDialog() == DialogResult.OK)
                    {
                        // Hier als Template speichern
                        saveTemplate(sfdTemplate.FileName);
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnOpenTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnOpenTemplate.InvokeRequired)
                {
                    IAsyncResult result = btnOpenTemplate.BeginInvoke(new EventHandler<EventArgs>(btnOpenTemplate_Click), new object[] { sender, e });
                    btnOpenTemplate.EndInvoke(result);
                }
                else
                {
                    String templatePath = SXDirs.TemplatePath + @"LTD";
                    if (!Directory.Exists(templatePath))
                        Directory.CreateDirectory(templatePath);

                    ofdTemplate.InitialDirectory = templatePath;
                    ofdTemplate.Filter = "SXTrader Template files (*.sxtempl)|*.sxtempl";
                    ofdTemplate.RestoreDirectory = true;
                    if (ofdTemplate.ShowDialog() == DialogResult.OK)
                    {
                        //Aus Template aufbauen
                        openTemplate(ofdTemplate.FileName);
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void openTemplate(string templateFile)
        {
            XDocument doc = null;
            int templateVersion = 0;
            try
            {
                doc = XDocument.Load(templateFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(LayTheDraw.strOpenFileError, templateFile), LayTheDraw.strTemplateLoadeError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExceptionWriter.Instance.WriteException(e);
                return;
            }

            XElement rootElement = (XElement)doc.FirstNode;
            XAttribute fbAttr = rootElement.Attribute("fbtype");
            if (fbAttr == null)
            {
                MessageBox.Show(LayTheDraw.strTemplateErrorNoId, LayTheDraw.strTemplateLoadeError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (fbAttr.Value != "LTD")
            {
                MessageBox.Show(LayTheDraw.strTemplateErrorNoLTD, LayTheDraw.strTemplateLoadeError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            XAttribute verAttr = rootElement.Attribute("templateVersion");
            if (verAttr == null)
            {
                templateVersion = 0;
            }
            else
            {
                templateVersion = Int32.Parse(verAttr.Value, CultureInfo.InvariantCulture);
            }

            try
            {
                _list.Clear();

                foreach (XElement rule in rootElement.Elements())
                {
                    BFUEFBIPTraderConfigElement configElement = BFUEFBIPTraderConfigElement.createNew();

                    //Spielzeit
                    XElement playtimeElement = rule.Element("playtime");
                    configElement.LoPlaytime = Int16.Parse(playtimeElement.Attribute("lo").Value, CultureInfo.InvariantCulture);
                    configElement.HiPlayTime = Int16.Parse(playtimeElement.Attribute("hi").Value, CultureInfo.InvariantCulture);

                    //Torstände
                    XElement scoresElement = rule.Element("scores");
                    foreach (XElement scoreElement in scoresElement.Elements())
                    {
                        IPTScores score = (IPTScores)Enum.Parse(typeof(IPTScores), scoreElement.Attribute("score").Value);
                        configElement.addScore(score);
                    }

                    //Quoten
                    XElement oddElement = rule.Element("odd");
                    configElement.LoOdds = Double.Parse(oddElement.Attribute("lo").Value,CultureInfo.InvariantCulture);
                    configElement.HiOdds = Double.Parse(oddElement.Attribute("hi").Value, CultureInfo.InvariantCulture);

                    //Marktvolumen
                    XElement marketElement = rule.Element("marketvolume");
                    if (templateVersion > 1)
                    {
                        long l = 0;
                        if (long.TryParse(marketElement.Attribute("lo").Value, out l))
                        {
                            l = (long)(l * SXALBankrollManager.Instance.MinStake);
                        }
                        configElement.LoVolume = l;
                    }
                    else
                    {
                        configElement.LoVolume = Int32.Parse(marketElement.Attribute("lo").Value, CultureInfo.InvariantCulture);
                    }

                    if (templateVersion > 1)
                    {
                        long l = 0;
                        if (long.TryParse(marketElement.Attribute("hi").Value, out l))
                        {
                            l = (long)(l * SXALBankrollManager.Instance.MinStake);
                        }
                        configElement.HiVolume = l;
                    }
                    else
                    {
                        configElement.HiVolume = Int32.Parse(marketElement.Attribute("hi").Value, CultureInfo.InvariantCulture);
                    }

                    //Abgeschlossener Markt
                    XElement settledElement = rule.Element("settledAllowed");
                    configElement.SettledAllowed = Boolean.Parse(settledElement.Attribute("allowed").Value);

                    // offener Markte
                    XElement unsettledElement = rule.Element("unsettledAllowed");
                    configElement.UnsettledAllowed = Boolean.Parse(unsettledElement.Attribute("allowed").Value);

                    // Rote Karten werden beobachted?
                    XElement redCardWatch = rule.Element("redCardWatch");
                    if (redCardWatch != null)
                    {
                        configElement.RedCardWatch = Boolean.Parse(redCardWatch.Attribute("watch").Value);
                    }

                    //Auf  welche Art spielen Rote Karten eine Rolle?
                    XElement redCardWatchState = rule.Element("redCardWatchState");
                    if (redCardWatchState != null)
                    {
                        IPTRedCardWatch watchState = (IPTRedCardWatch)Enum.Parse(typeof(IPTRedCardWatch), redCardWatchState.Attribute("state").Value);
                        configElement.RedCardWatchState = watchState;
                    }

                    // Allgemeine LTD-Konfiguration für Regel
                    XElement configurationElement = rule.Element("configuration");

                    LTDConfigurationRW tradeConfig = new LTDConfigurationRW(configurationElement.ToString(), templateVersion);
                    configElement.TradeConfig = tradeConfig;

                    _list.Add(configElement);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(LayTheDraw.strTemplateErrorUnexpected, LayTheDraw.strTemplateLoadeError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExceptionWriter.Instance.WriteException(e);
                return;
            }
            EventHandler<BFUEIPTraderDialogCloseEventArgs> closeDialogHandler = CloseIPTConfigDialog;
            if (closeDialogHandler != null)
                closeDialogHandler(this, new BFUEIPTraderDialogCloseEventArgs(_list));
            this.Close();
        }

        private void saveTemplate(string templateFile)
        {
            // ggf. existierende Datei löschen
            if (File.Exists(templateFile))
            {
                try
                {
                    File.Delete(templateFile);
                }
                catch
                {
                    MessageBox.Show(LayTheDraw.strTemplateErrorDelete, LayTheDraw.strErrorSaveTemplate	
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                // Datei anlegen
                //Datei nicht gefunden => erzeugen
                XmlTextWriter writer = new XmlTextWriter(templateFile, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartElement("rules");
                writer.Close();

                XDocument feed = XDocument.Load(templateFile);
                XElement rootElement = (XElement)feed.FirstNode;
                rootElement.SetAttributeValue("fbtype", "LTD");
                rootElement.SetAttributeValue("templateVersion", "2");
                // Die einzelnen Regeln abspeichern
                foreach (BFUEFBIPTraderConfigElement configElement in _list)
                {
                    XElement ruleElement = new XElement("rule");
                    //Spielzeit
                    XElement playtimeElement = new XElement("playtime");
                    playtimeElement.SetAttributeValue("lo", configElement.LoPlaytime.ToString(CultureInfo.InvariantCulture));
                    playtimeElement.SetAttributeValue("hi", configElement.HiPlayTime.ToString(CultureInfo.InvariantCulture));
                    ruleElement.Add(playtimeElement);

                    //Scores
                    XElement scoresElement = new XElement("scores");
                    foreach (IPTScores score in configElement.Scores)
                    {
                        XElement scoreElement = new XElement("score");
                        scoreElement.SetAttributeValue("score", score.ToString());
                        scoresElement.Add(scoreElement);
                    }
                    ruleElement.Add(scoresElement);

                    //Quote
                    XElement oddElement = new XElement("odd");
                    oddElement.SetAttributeValue("lo", configElement.LoOdds.ToString(CultureInfo.InvariantCulture));
                    oddElement.SetAttributeValue("hi", configElement.HiOdds.ToString(CultureInfo.InvariantCulture));
                    ruleElement.Add(oddElement);

                    //Marktvolumen
                    XElement marketElement = new XElement("marketvolume");
                    marketElement.SetAttributeValue("lo", (configElement.LoVolume / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture));
                    marketElement.SetAttributeValue("hi", (configElement.HiVolume / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture));
                    ruleElement.Add(marketElement);

                    //Abgeschlossener Markt erlaubt
                    XElement settledElement = new XElement("settledAllowed");
                    settledElement.SetAttributeValue("allowed", configElement.SettledAllowed);
                    ruleElement.Add(settledElement);

                    // Rote Karte spielen eine Rolle?
                    XElement redCardWatch = new XElement("redCardWatch");
                    redCardWatch.SetAttributeValue("watch", configElement.RedCardWatch);
                    ruleElement.Add(redCardWatch);

                    //Welches Verhalten bei der Überwachung von Roten Karten?
                    XElement redCardWatchState = new XElement("redCardWatchState");
                    redCardWatchState.SetAttributeValue("state", configElement.RedCardWatchState);
                    ruleElement.Add(redCardWatchState);

                    //Nicht abgeschlossener Markt erlaubt
                    XElement unsettledElement = new XElement("unsettledAllowed");
                    unsettledElement.SetAttributeValue("allowed", configElement.UnsettledAllowed);
                    ruleElement.Add(unsettledElement);

                    //Trading-Konfiguration               
                    TextReader tr = new StringReader(configElement.TradeConfig.getXML());
                    XDocument docConfig = XDocument.Load(tr);
                    ruleElement.Add(docConfig.FirstNode);                
                    /*
                    XElement configurationElement = new XElement("config");
                    XElement generalElement = new XElement("general");
                    configurationElement.Add(generalElement);

                    XElement fastbetElement = new XElement("fastbet");
                    configurationElement.Add(fastbetElement);


                    ruleElement.Add(configurationElement);
                     */

                    rootElement.Add(ruleElement);

                }

                feed.Save(templateFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(LayTheDraw.strTemplateSaveErrorUnexpected, LayTheDraw.strErrorSaveTemplate, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExceptionWriter.Instance.WriteException(e);
                return;
            }
            MessageBox.Show(String.Format(LayTheDraw.strTemplateSaveSuccess, templateFile), LayTheDraw.strTemplateSave, 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


    }
}
