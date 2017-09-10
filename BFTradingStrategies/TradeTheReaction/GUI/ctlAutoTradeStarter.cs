using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using System.Globalization;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    public partial class ctlAutoTradeStarter : UserControl
    {
        public event EventHandler<TTRASTraderDialogCloseEventArgs> CloseConfigDialog;

        private TradeStarterConfigList _list;

        public ctlAutoTradeStarter()
        {
            InitializeComponent();
            _list = new TradeStarterConfigList();
        }


        public TradeStarterConfigList ConfigList
        {
            set
            {
                try
                {
                    _list = value;
                    // Bisherigen Konfigurationsbildschirm zurücksetzen
                    ctlConfigElement.TSConfigElement = null;
                    while(this.pnlConfigDisplay.Controls.Count > 0)
                    {
                        this.pnlConfigDisplay.Controls[0].Dispose();
                    }
                    pnlConfigDisplay.AutoScroll = true;

                    // Neue Liste aufbauen
                    foreach (TradeStarterConfigElement element in _list)
                    {
                        ctlASConfigElementDisplay newDisplay = new ctlASConfigElementDisplay(element);
                        newDisplay.EditASConfigElement += new EventHandler<TTRASElementEditEventArgs>(EditASConfigElement);
                        newDisplay.DeleteASConfigElement += new EventHandler<TTRASElementDeleteEventArgs>(DeleteASConfigElement);
                        newDisplay.BorderStyle = BorderStyle.FixedSingle;
                        newDisplay.Dock = DockStyle.Top;
                        pnlConfigDisplay.Controls.Add(newDisplay);
                    }
                    pnlConfigDisplay.AutoScroll = true;
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        void DeleteASConfigElement(object sender, TTRASElementDeleteEventArgs e)
        {
            try
            {
                _list.Remove(e.ConfigElement);
                foreach (Control ctrl in pnlConfigDisplay.Controls)
                {
                    ctlASConfigElementDisplay element = ctrl as ctlASConfigElementDisplay;

                    if (element != null && element.ASConfigElement.ElementNumber == e.ConfigElement.ElementNumber)
                    {
                        pnlConfigDisplay.Controls.Remove(ctrl);
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

        void EditASConfigElement(object sender, TTRASElementEditEventArgs e)
        {
            ctlConfigElement.TSConfigElement = e.ConfigElement;
            
        }

        private void pnlButtons_SizeChanged(object sender, EventArgs e)
        {
            btnClose.Left = pnlButtons.Right - 5 - btnClose.Width;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                EventHandler<TTRASTraderDialogCloseEventArgs> handler = CloseConfigDialog;
                if (handler != null)
                    handler(this, new TTRASTraderDialogCloseEventArgs(_list));
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void ctlAutoTradeStarter_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<EventArgs>(ctlAutoTradeStarter_SizeChanged), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    this.pnlConfigEdit.Width = (int)(this.Width * 0.33);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void ctlConfigElement_SaveASConfigElement(object sender, net.sxtrader.bftradingstrategies.ttr.TradeStarter.TTRASElementSaveEventArgs e)
        {
            try
            {
                if (_list != null)
                {
                    if (!_list.Contains(e.ConfigElement))
                    {
                        _list.Add(e.ConfigElement);
                        ctlASConfigElementDisplay newElement = new ctlASConfigElementDisplay(e.ConfigElement);
                        newElement.EditASConfigElement += new EventHandler<TTRASElementEditEventArgs>(EditASConfigElement);
                        newElement.DeleteASConfigElement += new EventHandler<TTRASElementDeleteEventArgs>(DeleteASConfigElement);
                        newElement.BorderStyle = BorderStyle.FixedSingle;
                        newElement.Dock = DockStyle.Top;
                        pnlConfigDisplay.Controls.Add(newElement);
                        pnlConfigDisplay.AutoScroll = true;
                    }
                    else
                    {
                        int index = _list.IndexOf(e.ConfigElement);
                        _list[index] = e.ConfigElement;
                        foreach (Control ctrl in pnlConfigDisplay.Controls)
                        {
                            ctlASConfigElementDisplay element = ctrl as ctlASConfigElementDisplay;
                            if (element != null && element.ASConfigElement.ElementNumber == e.ConfigElement.ElementNumber)
                            {
                                element.ASConfigElement = e.ConfigElement;
                                return;
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

        private void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                String templatePath =  SXDirs.TemplatePath + @"\TTR";
                if (!Directory.Exists(templatePath))
                    Directory.CreateDirectory(templatePath);

                if (_list.Count == 0)
                {
                    MessageBox.Show("A Template File can't be created\r\nif there are no rules defined", "Error saving Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
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
                    MessageBox.Show("There was an error while deleting the existing template file", "Error saving Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Datei anlegen
            //Datei nicht gefunden => erzeugen
            XmlTextWriter writer = new XmlTextWriter(templateFile, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            writer.WriteStartElement("rules");
            writer.Close();

            XDocument feed = XDocument.Load(templateFile);
            XElement rootElement = (XElement)feed.FirstNode;
            rootElement.SetAttributeValue("fbtype", "TTR");
            rootElement.SetAttributeValue("templateVersion", "2");

            // Die einzelnen Regeln abspeichern
            foreach (TradeStarterConfigElement element in _list)
            {
                XElement ruleElement = new XElement("rule");
                //Tradetype
                ruleElement.SetAttributeValue("tradetype", element.TradeType);                
                ////////////////////////////////////////////////////////////////////
                /// Prerequisite
                ///////////////////////////////////////////////////////////////////
                // Benötigte Anderer Trader
                XElement prerequisiteTradeElement = new XElement("prerequisitetrade");
                prerequisiteTradeElement.SetAttributeValue("tradetype", element.RequiredTrade);
                ruleElement.Add(prerequisiteTradeElement);

                // Benötigter Status des andren Trades
                XElement prerequisiteTradeStateElement = new XElement("prerequisitetradestate");
                prerequisiteTradeStateElement.SetAttributeValue("state", element.RequiredTradeState);
                ruleElement.Add(prerequisiteTradeStateElement);

                ////////////////////////////////////////////////////////////////////
                //// Preplay
                ////////////////////////////////////////////////////////////////////
                //Preplay Rule Check
                XElement preplayElement = new XElement("preplaycheck");
                preplayElement.SetAttributeValue("preplay", element.Preplay);
                ruleElement.Add(preplayElement);

                //Preplay Odds
                XElement preplayOddElement = new XElement("preplayodd");
                preplayOddElement.SetAttributeValue("lo", element.PreplayLoOdds.ToString(CultureInfo.InvariantCulture));
                preplayOddElement.SetAttributeValue("hi", element.PreplayHiOdds.ToString(CultureInfo.InvariantCulture));
                ruleElement.Add(preplayOddElement);

                // Preplay Market Volume
                XElement preplayMarketElement = new XElement("preplaymarketvolume");
                preplayMarketElement.SetAttributeValue("lo", (element.PreplayLoVolume / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture) );
                preplayMarketElement.SetAttributeValue("hi", (element.PreplayHiVolume / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture));
                ruleElement.Add(preplayMarketElement);

                ////////////////////////////////////////////////////////////////////
                //// Inplay
                ////////////////////////////////////////////////////////////////////
                //Inplay Rule Check
                XElement inplayElement = new XElement("inplaycheck");
                inplayElement.SetAttributeValue("inplay", element.Inplay);
                ruleElement.Add(inplayElement);

                //Spielzeit
                XElement playtimeElement = new XElement("playtime");
                playtimeElement.SetAttributeValue("lo", element.LoPlaytime);
                playtimeElement.SetAttributeValue("hi", element.HiPlaytime);
                ruleElement.Add(playtimeElement);

                //Scores
                XElement scoresElement = new XElement("scores");
                foreach (TTRScores score in element.Scores)
                {
                    XElement scoreElement = new XElement("score");
                    scoreElement.SetAttributeValue("score", score.ToString());
                    scoresElement.Add(scoreElement);
                }
                ruleElement.Add(scoresElement);

                //GoalSum
                XElement goalSumElement = new XElement("goalsum");
                goalSumElement.SetAttributeValue("lo", element.LoGoalSum);
                goalSumElement.SetAttributeValue("hi", element.HiGoalSum);
                ruleElement.Add(goalSumElement);

                //Quote
                XElement oddElement = new XElement("odd");
                oddElement.SetAttributeValue("lo", element.LoOdds.ToString(CultureInfo.InvariantCulture));
                oddElement.SetAttributeValue("hi", element.HiOdds.ToString(CultureInfo.InvariantCulture));
                ruleElement.Add(oddElement);

                //Marktvolumen
                XElement marketElement = new XElement("marketvolume");
                marketElement.SetAttributeValue("lo", (element.LoVolume / SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture) );
                marketElement.SetAttributeValue("hi", (element.HiVolume/ SXALBankrollManager.Instance.MinStake).ToString(CultureInfo.InvariantCulture) );
                ruleElement.Add(marketElement);

                // Rote Karte spielen eine Rolle?
                XElement redCardWatch = new XElement("redCardWatch");
                redCardWatch.SetAttributeValue("watch", element.RedCardWatch);
                ruleElement.Add(redCardWatch);

                //Welches Verhalten bei der Überwachung von Roten Karten?
                XElement redCardWatchState = new XElement("redCardWatchState");
                redCardWatchState.SetAttributeValue("state", element.RedCardWatchState);
                ruleElement.Add(redCardWatchState);

                ////////////////////////////////////////////////////////////////////
                // Allgemein
                ////////////////////////////////////////////////////////////////////

                //Abgeschlossener Markt erlaubt
                XElement settledElement = new XElement("settledAllowed");
                settledElement.SetAttributeValue("allowed", element.SettledAllowed);
                ruleElement.Add(settledElement);

                //Nicht abgeschlossener Markt erlaubt
                XElement unsettledElement = new XElement("unsettledAllowed");
                unsettledElement.SetAttributeValue("allowed", element.UnsettledAllowed);
                ruleElement.Add(unsettledElement);

                //Trading-Konfiguration               
                TextReader tr = new StringReader(element.TradeConfig.getXML());
                XDocument docConfig = XDocument.Load(tr);
                ruleElement.Add(docConfig.FirstNode);

                // Statistik
                XElement statisticsElement = new XElement("statistics");
                foreach (StatisticSelectionElement statConfigElement in element.Statistics)
                {
                    XElement statElement = new XElement("statrule");
                    statElement.SetAttributeValue("teamtype", statConfigElement.Team);
                    statElement.SetAttributeValue("homeawaytype", statConfigElement.HomeAway);
                    statElement.SetAttributeValue("stattype", statConfigElement.Statistic);                    
                    statElement.SetAttributeValue("lo", statConfigElement.LoRange.ToString(CultureInfo.InvariantCulture));
                    statElement.SetAttributeValue("hi", statConfigElement.HiRange.ToString(CultureInfo.InvariantCulture));
                    statisticsElement.Add(statElement);
                }
                ruleElement.Add(statisticsElement);

                ////////////////////////////////////////////////////////////////
                // Dynamische Trade Out Regeln
                ////////////////////////////////////////////////////////////////
                XElement tradeOutRules = new XElement("tradeoutrules");
                foreach (TTRTradeOutCheck tradeOutCheck in element.TradeConfig.TradeOutRules.Values)
                {
                    XElement rule = new XElement("rule");
                    //////////////////////////////////////////////////////////////////////////
                    // Reichenfolge und Auslöser
                    /////////////////////////////////////////////////////////////////////////
                    rule.SetAttributeValue("checkOrder", tradeOutCheck.Order.ToString());
                    rule.SetAttributeValue("checkTrigger", tradeOutCheck.Trigger.ToString());


                    ///////////////////////////////////////////////////////////////////
                    // Einstellungen für das Austraden
                    //////////////////////////////////////////////////////////////////
                    XElement tradeOutSettings = new XElement("tradeoutsettings");  
                    #region Over/Under
                  
                    XElement tradeOutSettingsNoTrade = new XElement("notrade");
                    tradeOutSettingsNoTrade.Value = tradeOutCheck.TradeOutSettings.NoTrade.ToString();
                    tradeOutSettings.Add(tradeOutSettingsNoTrade);

                    XElement tradeOutSettingsCheckLayOdds = new XElement("checklayodds");
                    tradeOutSettingsCheckLayOdds.Value = tradeOutCheck.TradeOutSettings.CheckLayOdds.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCheckLayOdds);

                    XElement tradeOutSettingsGreenPerc = new XElement("greenpercentage");
                    tradeOutSettingsGreenPerc.Value = tradeOutCheck.TradeOutSettings.GreenPercentage.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsGreenPerc);

                    XElement tradeOutSettingsGreenPlaytime = new XElement("greenplaytime");
                    tradeOutSettingsGreenPlaytime.Value = tradeOutCheck.TradeOutSettings.GreenPlaytime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsGreenPlaytime);

                    XElement tradeOutSettingsGreenWaittime = new XElement("greenwaittime");
                    tradeOutSettingsGreenWaittime.Value = tradeOutCheck.TradeOutSettings.GreenWaitTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsGreenWaittime);

                    XElement tradeOutSettingsHedgePerc = new XElement("hedgepercentage");
                    tradeOutSettingsHedgePerc.Value = tradeOutCheck.TradeOutSettings.HedgePercentage.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsHedgePerc);

                    XElement tradeOutSettingsHedgePlaytime = new XElement("hedgeplaytime");
                    tradeOutSettingsHedgePlaytime.Value = tradeOutCheck.TradeOutSettings.HedgePlaytime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsHedgePlaytime);

                    XElement tradeOutSettingsHedgeWaittime = new XElement("hedgewaittime");
                    tradeOutSettingsHedgeWaittime.Value = tradeOutCheck.TradeOutSettings.HedgeWaitTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsHedgeWaittime);

                    XElement tradeOutSettingsOnlyHedge = new XElement("onlyhedge");
                    tradeOutSettingsOnlyHedge.Value = tradeOutCheck.TradeOutSettings.OnlyHedge.ToString();
                    tradeOutSettings.Add(tradeOutSettingsOnlyHedge);

                    XElement tradeOutSettingsUseGreenMinutes = new XElement("usegreenminutes");
                    tradeOutSettingsUseGreenMinutes.Value = tradeOutCheck.TradeOutSettings.UseGreenWaitTime.ToString();
                    tradeOutSettings.Add(tradeOutSettingsUseGreenMinutes);

                    XElement tradeOutSettingsUseHedgeWaittime = new XElement("usehedgewaittime");
                    tradeOutSettingsUseHedgeWaittime.Value = tradeOutCheck.TradeOutSettings.UseHedgeWaitTime.ToString();
                    tradeOutSettings.Add(tradeOutSettingsUseHedgeWaittime);

                    XElement tradeOutSettingsUseOddsPerc = new XElement("useoddspercentage");
                    tradeOutSettingsUseOddsPerc.Value = tradeOutCheck.TradeOutSettings.UseOddsPercentage.ToString();
                    tradeOutSettings.Add(tradeOutSettingsUseOddsPerc);

                    XElement tradeOutSettingsUseWaittime = new XElement("usewaittime");
                    tradeOutSettingsUseWaittime.Value = tradeOutCheck.TradeOutSettings.UseWaitTime.ToString();
                    tradeOutSettings.Add(tradeOutSettingsUseWaittime);
                    #endregion

                    #region Correct Score
                    XElement tradeOutSettingsCSBCheckLayOdds = new XElement("csbchecklayodds");
                    tradeOutSettingsCSBCheckLayOdds.Value = tradeOutCheck.TradeOutSettings.CSBackCheckLayOdds.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBCheckLayOdds);

                    XElement tradeOutSettingsCSBGreenPercentage = new XElement("csbgreenpercentage");
                    tradeOutSettingsCSBGreenPercentage.Value = tradeOutCheck.TradeOutSettings.CSBackGreenPercentage.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBGreenPercentage);

                    XElement tradeOutSettingsCSBGreenPlaytime = new XElement("csbgreenplaytime");
                    tradeOutSettingsCSBGreenPlaytime.Value = tradeOutCheck.TradeOutSettings.CSBackGreenPlaytime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBGreenPlaytime);

                    XElement tradeOutSettingsCSBGreenSpecialPlaytime = new XElement("csbgreenspecialplaytime");
                    tradeOutSettingsCSBGreenSpecialPlaytime.Value =tradeOutCheck.TradeOutSettings.CSBackGreenSpecialPlayTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBGreenSpecialPlaytime);

                    XElement tradeOutSettingsCSBGreenSpecialPlaytimeDelta = new XElement("csbgreenspecialplaytimedelta");
                    tradeOutSettingsCSBGreenSpecialPlaytimeDelta.Value = tradeOutCheck.TradeOutSettings.CSBackGreenSpecialPlayTimeDelta.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBGreenSpecialPlaytimeDelta);

                    XElement tradeOutSettingsCSBGreenWaittime = new XElement("csbgreenwaittime");
                    tradeOutSettingsCSBGreenWaittime.Value = tradeOutCheck.TradeOutSettings.CSBackGreenWaittime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBGreenWaittime);

                    XElement tradeOutSettingsCSBHedgePercentage = new XElement("csbhedgepercentage");
                    tradeOutSettingsCSBHedgePercentage.Value = tradeOutCheck.TradeOutSettings.CSBackHedgePercentage.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBHedgePercentage);

                    XElement tradeOutSettingsCSBHedgePlaytime = new XElement("csbhedgeplaytime");
                    tradeOutSettingsCSBHedgePlaytime.Value = tradeOutCheck.TradeOutSettings.CSBackHedgePlayTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBHedgePlaytime);

                    XElement tradeOutSettingsCSBHedgeSpecialPlaytime = new XElement("csbhedgespecialplaytime");
                    tradeOutSettingsCSBHedgeSpecialPlaytime.Value = tradeOutCheck.TradeOutSettings.CSBackHedgeSpecialPlayTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBHedgeSpecialPlaytime);

                    XElement tradeOutSettingsCSBHedgeSpecialPlaytimeDelta = new XElement("csbhedgespecialplaytimedelta");
                    tradeOutSettingsCSBHedgeSpecialPlaytimeDelta.Value = tradeOutCheck.TradeOutSettings.CSBackHedgeSpecialPlayTimeDelta.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBHedgeSpecialPlaytimeDelta);

                    XElement tradeOutSettingsCSBHedgeWaittime = new XElement("csbhedgewaittime");
                    tradeOutSettingsCSBHedgeWaittime.Value = tradeOutCheck.TradeOutSettings.CSBackHedgeWaitTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBHedgeWaittime);

                    XElement tradeOutSettingsCSBOnlyHedge = new XElement("csbonlyhedge");
                    tradeOutSettingsCSBOnlyHedge.Value = tradeOutCheck.TradeOutSettings.CSBackOnlyHedge.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBOnlyHedge);

                    XElement tradeOutSettingsCSBUseGreenSpecialPlaytime = new XElement("csbusegreenspecialplaytime");
                    tradeOutSettingsCSBUseGreenSpecialPlaytime.Value = tradeOutCheck.TradeOutSettings.CSBackUseGreenSpecialPlayTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBUseGreenSpecialPlaytime);

                    XElement tradeOutSettingsCSBUseGreenWaitTime = new XElement("csbusegreenwaittime");
                    tradeOutSettingsCSBUseGreenWaitTime.Value = tradeOutCheck.TradeOutSettings.CSBackUseGreenWaitTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBUseGreenWaitTime);


                    XElement tradeOutSettingsCSBUseHedgeSpecialPlaytime = new XElement("csbusehedgespecialplaytime");
                    tradeOutSettingsCSBUseHedgeSpecialPlaytime.Value = tradeOutCheck.TradeOutSettings.CSBackUseHedgeSpecialPlayTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBUseHedgeSpecialPlaytime);

                    XElement tradeOutSettingsCSBUseHedgeWaittime = new XElement("csbusehedgewaittime");
                    tradeOutSettingsCSBUseHedgeWaittime.Value = tradeOutCheck.TradeOutSettings.CSBackUseHedgeWaitTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBUseHedgeWaittime);

                    XElement tradeOutSettingsCSBUseOddsPercentage = new XElement("csbuseoddspercentage");
                    tradeOutSettingsCSBUseOddsPercentage.Value = tradeOutCheck.TradeOutSettings.CSBackUseOddsPercentage.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBUseOddsPercentage);

                    XElement tradeOutSettingsCSBUseWaitTime = new XElement("csbusewaittime");
                    tradeOutSettingsCSBUseWaitTime.Value = tradeOutCheck.TradeOutSettings.CSBackUseWaitTime.ToString(CultureInfo.InvariantCulture);
                    tradeOutSettings.Add(tradeOutSettingsCSBUseWaitTime);

                    #endregion
                    rule.Add(tradeOutSettings);


                    /////////////////////////////////////////////////////////////////////////////////
                    // Überprüfungsregeln
                    /////////////////////////////////////////////////////////////////////////////////
                    XElement checkRules = new XElement("checkrules");
                    foreach (TTRTradeOutRuleFragment fragment in tradeOutCheck.Rules)
                    {
                        XElement checkRule = XElement.Parse(fragment.toXml());
                        checkRules.Add(checkRule);
                    }

                    rule.Add(checkRules);

                    tradeOutRules.Add(rule);                    
                }
                ruleElement.Add(tradeOutRules);

                rootElement.Add(ruleElement);

            }

            
            feed.Save(templateFile);

            MessageBox.Show(String.Format(TradeTheReaction.strASTmpltSaveSuccMessage, templateFile), TradeTheReaction.strASTmpltSaveTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnOpenTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                String templatePath = SXDirs.TemplatePath   + @"\TTR";
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
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void openTemplate(string templateFile)
        {
            _list = TTRHelper.OpenTemplate(templateFile);
            if (_list != null)
            {
                EventHandler<TTRASTraderDialogCloseEventArgs> handler = CloseConfigDialog;
                if (handler != null)
                    handler(this, new TTRASTraderDialogCloseEventArgs(_list));
            }
        }


    }

    public class TTRASTraderDialogCloseEventArgs : EventArgs
    {
        TradeStarterConfigList _list;
        public TradeStarterConfigList ConfigList
        {
            get
            {
                return _list;
            }
        }

        public TTRASTraderDialogCloseEventArgs(TradeStarterConfigList list)
        {
            _list = list;
        }
    }
}
