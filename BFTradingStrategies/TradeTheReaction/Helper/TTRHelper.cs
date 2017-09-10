using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using System.Xml.Linq;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Globalization;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.Helper
{
    public class CSTradeTypeToScores
    {
        public TRADETYPE TradeType
        {
            get;
            private set;
        }

        public int ScoreA
        {
            get;
            private set;
        }

        public int ScoreB
        {
            get;
            private set;
        }

        internal CSTradeTypeToScores(TRADETYPE tradeType, int scoreA, int scoreB)
        {
            this.TradeType = tradeType;
            this.ScoreA    = scoreA;
            this.ScoreB    = scoreB;
        }
    }

    public static class CSTradeTypeToScoresList
    {
        private static SortedList<TRADETYPE, CSTradeTypeToScores> _list;
        static CSTradeTypeToScoresList()
        {
            _list = new SortedList<TRADETYPE, CSTradeTypeToScores>();
            _list.Add(TRADETYPE.SCORELINE01BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE01BACK, 0, 1));
            _list.Add(TRADETYPE.SCORELINE01LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE01LAY, 0, 1));
            _list.Add(TRADETYPE.SCORELINE02BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE02BACK, 0, 2));
            _list.Add(TRADETYPE.SCORELINE02LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE02LAY, 0, 2));
            _list.Add(TRADETYPE.SCORELINE03BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE03BACK, 0, 3));
            _list.Add(TRADETYPE.SCORELINE03LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE03LAY, 0, 3));
            _list.Add(TRADETYPE.SCORELINE10BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE10BACK, 1, 0));
            _list.Add(TRADETYPE.SCORELINE10LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE10LAY, 1, 0));
            _list.Add(TRADETYPE.SCORELINE11BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE11BACK, 1, 1));
            _list.Add(TRADETYPE.SCORELINE11LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE11LAY, 1, 1));
            _list.Add(TRADETYPE.SCORELINE12BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE12BACK, 1, 2));
            _list.Add(TRADETYPE.SCORELINE12LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE12LAY, 1, 2));
            _list.Add(TRADETYPE.SCORELINE13BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE13BACK, 1, 3));
            _list.Add(TRADETYPE.SCORELINE13LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE13LAY, 1, 3));
            _list.Add(TRADETYPE.SCORELINE20BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE20BACK, 2, 0));
            _list.Add(TRADETYPE.SCORELINE20LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE20LAY, 2, 0));
            _list.Add(TRADETYPE.SCORELINE21BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE21BACK, 2, 1));
            _list.Add(TRADETYPE.SCORELINE21LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE21LAY, 2, 1));
            _list.Add(TRADETYPE.SCORELINE22BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE22BACK, 2, 2));
            _list.Add(TRADETYPE.SCORELINE22LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE22LAY, 2, 2));
            _list.Add(TRADETYPE.SCORELINE23BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE23BACK, 2, 3));
            _list.Add(TRADETYPE.SCORELINE23LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE23LAY, 2, 3));
            _list.Add(TRADETYPE.SCORELINE30BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE30BACK, 3, 0));
            _list.Add(TRADETYPE.SCORELINE30LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE30LAY, 3, 0));
            _list.Add(TRADETYPE.SCORELINE31BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE31BACK, 3, 1));
            _list.Add(TRADETYPE.SCORELINE31LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE31LAY, 3, 1));
            _list.Add(TRADETYPE.SCORELINE32BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE32BACK, 3, 2));
            _list.Add(TRADETYPE.SCORELINE32LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE32LAY, 3, 2));
            _list.Add(TRADETYPE.SCORELINE33BACK, new CSTradeTypeToScores(TRADETYPE.SCORELINE33BACK, 3, 3));
            _list.Add(TRADETYPE.SCORELINE33LAY, new CSTradeTypeToScores(TRADETYPE.SCORELINE33LAY, 3, 3));
        }

        public static int GetScoreA(TRADETYPE tradeType)
        {
            return _list[tradeType].ScoreA;
        }

        public static int GetScoreB(TRADETYPE tradeType)
        {
            return _list[tradeType].ScoreB;
        }
    }

    public static class TTRHelper
    {

        public static TradeStarterConfigList OpenTemplate(string templateFile)
        {
            TradeStarterConfigList list = new TradeStarterConfigList();
            int templateVersion = 0;
            XDocument doc = null;
            try
            {
                doc = XDocument.Load(templateFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(TradeTheReaction.strASTmpltOpenFileOpenError, templateFile), TradeTheReaction.strASTmpltOpenErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExceptionWriter.Instance.WriteException(e);
                return null;
            }

            XElement rootElement = (XElement)doc.FirstNode;
            XAttribute fbAttr = rootElement.Attribute("fbtype");
            if (fbAttr == null)
            {
                MessageBox.Show(TradeTheReaction.strASTmpltOpenNoType, TradeTheReaction.strASTmpltOpenErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (fbAttr.Value != "TTR")
            {
                MessageBox.Show(TradeTheReaction.strASTmpltOpenWrongType, TradeTheReaction.strASTmpltOpenErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
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
                list.Clear();

                foreach (XElement rule in rootElement.Elements())
                {
                    TradeStarterConfigElement configElement = TradeStarterConfigElement.createNew();

                    //TradeType
                    if (rule.Attribute("tradetype") != null)
                    {
                        configElement.TradeType = (TRADETYPE)Enum.Parse(typeof(TRADETYPE), rule.Attribute("tradetype").Value);
                    }

                    ////////////////////////////////////////////////////////////
                    // PREREQUISITE
                    ////////////////////////////////////////////////////////////
                    XElement prerequisiteTradeElement = rule.Element("prerequisitetrade");
                    if (prerequisiteTradeElement != null)
                    {
                        if (rule.Attribute("tradetype") != null)
                        {
                            configElement.RequiredTrade = (TRADETYPE)Enum.Parse(typeof(TRADETYPE), prerequisiteTradeElement.Attribute("tradetype").Value);
                        }
                    }

                    XElement prerequisiteTradeStateElement = rule.Element("prerequisitetradestate");
                    if (prerequisiteTradeStateElement != null)
                    {
                        if (prerequisiteTradeStateElement.Attribute("state") != null)
                        {
                            configElement.RequiredTradeState = (TRADESTATE)Enum.Parse(typeof(TRADESTATE), prerequisiteTradeStateElement.Attribute("state").Value);
                        }
                    }

                    ////////////////////////////////////////////////////////////
                    // PREPLAY
                    ////////////////////////////////////////////////////////////
                    XElement preplayCheckElement = rule.Element("preplaycheck");
                    if (preplayCheckElement != null)
                    {
                        if (preplayCheckElement.Attribute("preplay") != null)
                        {
                            bool b = false;
                            if (Boolean.TryParse(preplayCheckElement.Attribute("preplay").Value, out b))
                                configElement.Preplay = b;
                        }
                    }

                    XElement preplayOddsElement = rule.Element("preplayodd");
                    if (preplayOddsElement != null)
                    {
                        if (preplayOddsElement.Attribute("lo") != null)
                        {
                            double d = 0.0;
                            NumberStyles styles = NumberStyles.Float;
                            if (Double.TryParse(preplayOddsElement.Attribute("lo").Value,styles, CultureInfo.InvariantCulture, out d))
                                configElement.PreplayLoOdds = d;
                        }

                        if (preplayOddsElement.Attribute("hi") != null)
                        {
                            double d = 0.0;
                            NumberStyles styles = NumberStyles.Float;
                            if (Double.TryParse(preplayOddsElement.Attribute("hi").Value,styles, CultureInfo.InvariantCulture, out d))
                                configElement.PreplayHiOdds = d;
                        }
                    }

                    XElement preplayMarketVolume = rule.Element("preplaymarketvolume");
                    if (preplayMarketVolume != null)
                    {
                        if (preplayMarketVolume.Attribute("lo") != null)
                        {                            
                            long l = 0;
                            if (long.TryParse(preplayMarketVolume.Attribute("lo").Value, out l))
                            {
                                if (templateVersion > 1)
                                {
                                    l = (long)(l * SXALBankrollManager.Instance.MinStake);
                                }
                                configElement.PreplayLoVolume = l;
                            }
                        }

                        if (preplayMarketVolume.Attribute("hi") != null)
                        {
                            long l = 0;
                            {

                                if (long.TryParse(preplayMarketVolume.Attribute("hi").Value, out l))
                                {
                                    if (templateVersion > 1)
                                    {
                                        l = (long)(l * SXALBankrollManager.Instance.MinStake);
                                    }
                                    configElement.PreplayHiVolume = l;
                                }
                            }
                        }
                    }

                    ////////////////////////////////////////////////////////////
                    // INPLAY
                    ////////////////////////////////////////////////////////////
                    XElement inplayCheckElement = rule.Element("inplaycheck");
                    if (inplayCheckElement != null)
                    {
                        if (inplayCheckElement.Attribute("inplay") != null)
                        {
                            bool b = false;
                            if (Boolean.TryParse(inplayCheckElement.Attribute("inplay").Value, out b))
                                configElement.Inplay = b;
                        }
                    }

                    XElement inplayPlaytimeElement = rule.Element("playtime");
                    if (inplayPlaytimeElement != null)
                    {
                        if (inplayPlaytimeElement.Attribute("lo") != null)
                        {
                            int i = 0;
                            if (Int32.TryParse(inplayPlaytimeElement.Attribute("lo").Value, out i))
                                configElement.LoPlaytime = i;
                        }

                        if (inplayPlaytimeElement.Attribute("hi") != null)
                        {
                            int i = 0;
                            if (Int32.TryParse(inplayPlaytimeElement.Attribute("hi").Value, out i))
                                configElement.HiPlaytime = i;
                        }
                    }

                    XElement inplayOddsElement = rule.Element("odd");
                    if (inplayOddsElement != null)
                    {
                        if (inplayOddsElement.Attribute("lo") != null)
                        {
                            double d = 0.0;
                            NumberStyles styles = NumberStyles.Float;
                            if (double.TryParse(inplayOddsElement.Attribute("lo").Value,styles,CultureInfo.InvariantCulture, out d))
                                configElement.LoOdds = d;
                        }

                        if (inplayOddsElement.Attribute("hi") != null)
                        {
                            double d = 0.0;
                            NumberStyles styles = NumberStyles.Float;
                            if (double.TryParse(inplayOddsElement.Attribute("hi").Value, styles, CultureInfo.InvariantCulture, out d))
                                configElement.HiOdds = d;
                        }
                    }

                    //Torstände
                    XElement scoresElement = rule.Element("scores");
                    if (scoresElement != null)
                    {
                        foreach (XElement scoreElement in scoresElement.Elements())
                        {
                            if (scoreElement.Attribute("score") != null)
                            {
                                TTRScores score = (TTRScores)Enum.Parse(typeof(TTRScores), scoreElement.Attribute("score").Value);
                                configElement.addScore(score);
                            }
                        }
                    }

                    //Torsumme
                    XElement goalSumelement = rule.Element("goalsum");
                    if (goalSumelement != null)
                    {
                        if (goalSumelement.Attribute("lo") != null)
                        {
                            int i = 0;
                            if (Int32.TryParse(goalSumelement.Attribute("lo").Value, out i))
                                configElement.LoGoalSum = i;
                        }

                        if (goalSumelement.Attribute("hi") != null)
                        {
                            int i = 0;
                            if (Int32.TryParse(goalSumelement.Attribute("hi").Value, out i))
                                configElement.HiGoalSum = i;
                        }
                    }

                    //Marktvolumen
                    XElement inplayMarketVolumeElement = rule.Element("marketvolume");
                    if (inplayMarketVolumeElement != null)
                    {
                        if (inplayMarketVolumeElement.Attribute("lo") != null)
                        {
                            long l = 0;
                            if (long.TryParse(inplayMarketVolumeElement.Attribute("lo").Value, out l))
                            {
                                if (templateVersion > 1)
                                {
                                    l = (long)(l * SXALBankrollManager.Instance.MinStake);
                                }
                                configElement.LoVolume = l;
                            }
                        }

                        if (inplayMarketVolumeElement.Attribute("hi") != null)
                        {
                            long l = 0;
                            if (long.TryParse(inplayMarketVolumeElement.Attribute("hi").Value, out l))
                            {
                                if (templateVersion > 1)
                                {
                                    l = (long)(l * SXALBankrollManager.Instance.MinStake);
                                }
                                configElement.HiVolume = l;
                            }
                        }
                    }

                    // Rote Karte überwachen
                    XElement inplayRedCardWatchElement = rule.Element("redCardWatch");
                    if (inplayRedCardWatchElement != null)
                    {
                        if (inplayRedCardWatchElement.Attribute("watch") != null)
                        {
                            bool b = false;
                            if (Boolean.TryParse(inplayRedCardWatchElement.Attribute("watch").Value, out b))
                                configElement.RedCardWatch = b;
                        }
                    }

                    // Art der Rote Karten Überwachung
                    XElement inplayRedCardWatchStateElement = rule.Element("redCardWatchState");
                    if (inplayRedCardWatchStateElement != null)
                    {
                        if (inplayRedCardWatchStateElement.Attribute("state") != null)
                        {
                            TTRRedCardWatch redcardwatch = (TTRRedCardWatch)Enum.Parse(typeof(TTRRedCardWatch), inplayRedCardWatchStateElement.Attribute("state").Value);
                            configElement.RedCardWatchState = redcardwatch;
                        }
                    }

                    ////////////////////////////////////////////////////////////
                    // ALLGEMEIN
                    ////////////////////////////////////////////////////////////

                    //Abgeschlossener Trade
                    XElement commonSettledTrade = rule.Element("settledAllowed");
                    if (commonSettledTrade != null)
                    {
                        if (commonSettledTrade.Attribute("allowed") != null)
                        {
                            bool b = false;
                            if (Boolean.TryParse(commonSettledTrade.Attribute("allowed").Value, out b))
                                configElement.SettledAllowed = b;
                        }
                    }

                    // Nicht Abgeschlossener Trade
                    XElement commonUnsettledTrade = rule.Element("unsettledAllowed");
                    if (commonUnsettledTrade != null)
                    {
                        if (commonUnsettledTrade.Attribute("allowed") != null)
                        {
                            bool b = false;
                            if (Boolean.TryParse(commonUnsettledTrade.Attribute("allowed").Value, out b))
                                configElement.UnsettledAllowed = b;
                        }
                    }

                    //Config
                    // Allgemeine TTR-Konfiguration für Regel
                    XElement configurationElement = rule.Element("configuration");

                    TTRConfigurationRW tradeConfig = new TTRConfigurationRW(configurationElement.ToString(), templateVersion);
                    configElement.TradeConfig = tradeConfig;


                    //Statistiken
                    XElement commonStatisticsElement = rule.Element("statistics");
                    if (commonStatisticsElement != null)
                    {
                        foreach (XElement statRuleElment in commonStatisticsElement.Elements())
                        {
                            StatisticSelectionElement statSelElem = StatisticSelectionElement.createNew();
                            if (statRuleElment.Attribute("teamtype") != null)
                            {
                                statSelElem.Team = (STATISTICTEAM)Enum.Parse(typeof(STATISTICTEAM), statRuleElment.Attribute("teamtype").Value);
                                
                            }

                            if (statRuleElment.Attribute("homeawaytype") != null)
                            {
                                statSelElem.HomeAway = (STATISTICHOMEAWAY)Enum.Parse(typeof(STATISTICHOMEAWAY), statRuleElment.Attribute("homeawaytype").Value);
                            }

                            if (statRuleElment.Attribute("stattype") != null)
                            {
                                statSelElem.Statistic = (STATISTICTYPE)Enum.Parse(typeof(STATISTICTYPE), statRuleElment.Attribute("stattype").Value);
                            }

                            if (statRuleElment.Attribute("lo") != null)
                            {
                                NumberStyles style = NumberStyles.Float;
                                //CultureInfo culture = CultureInfo.InvariantCulture;
                                double d = 0.0;
                                if (double.TryParse(statRuleElment.Attribute("lo").Value, style,CultureInfo.InvariantCulture, out d))
                                    statSelElem.LoRange = d;
                                
                            }

                            if (statRuleElment.Attribute("hi") != null)
                            {
                                double d = 0.0;
                                NumberStyles style = NumberStyles.Float;
                                //CultureInfo culture = CultureInfo.InvariantCulture;
                                if (double.TryParse(statRuleElment.Attribute("hi").Value,style,CultureInfo.InvariantCulture, out d))
                                    statSelElem.HiRange = d;
                            }

                            configElement.addStatistic(statSelElem);
                        }
                    }

                    ////////////////////////////////////////////////////////////////
                    // Dynamische Trade Out Regeln
                    ////////////////////////////////////////////////////////////////
                    XElement tradeOutRulesElement = rule.Element("tradeoutrules");
                    try
                    {
                        if (tradeOutRulesElement != null)
                        {
                            foreach (XElement ruleElement in tradeOutRulesElement.Elements())
                            {
                                TTRTradeOutCheck tradeOutCheck = new TTRTradeOutCheck();
                                if (ruleElement.Attribute("checkOrder") != null)
                                {
                                    tradeOutCheck.Order = Int32.Parse(ruleElement.Attribute("checkOrder").Value);
                                }

                                if (ruleElement.Attribute("checkTrigger") != null)
                                {
                                    tradeOutCheck.Trigger = (TRADEOUTTRIGGER)Enum.Parse(typeof(TRADEOUTTRIGGER), ruleElement.Attribute("checkTrigger").Value);
                                }

                                if (ruleElement.Element("tradeoutsettings") != null)
                                {
                                    #region Over/Under
                                    XElement settingsElement = ruleElement.Element("tradeoutsettings");

                                    TTRTradeOutSettings tradeOutSettings = new TTRTradeOutSettings();

                                    if (settingsElement.Element("notrade") != null)
                                    {
                                        tradeOutSettings.NoTrade = Boolean.Parse(settingsElement.Element("notrade").Value);
                                    }

                                    if (settingsElement.Element("checklayodds") != null)
                                    {
                                        tradeOutSettings.CheckLayOdds = Boolean.Parse(settingsElement.Element("checklayodds").Value);
                                    }

                                    if (settingsElement.Element("greenpercentage") != null)
                                    {
                                        tradeOutSettings.GreenPercentage = Int32.Parse(settingsElement.Element("greenpercentage").Value);
                                    }

                                    if (settingsElement.Element("greenplaytime") != null)
                                    {
                                        tradeOutSettings.GreenPlaytime = Int32.Parse(settingsElement.Element("greenplaytime").Value);
                                    }

                                    if (settingsElement.Element("greenwaittime") != null)
                                    {
                                        tradeOutSettings.GreenWaitTime = Int32.Parse(settingsElement.Element("greenwaittime").Value);
                                    }

                                    if (settingsElement.Element("hedgepercentage") != null)
                                    {
                                        tradeOutSettings.HedgePercentage = Int32.Parse(settingsElement.Element("hedgepercentage").Value);
                                    }

                                    if (settingsElement.Element("hedgeplaytime") != null)
                                    {
                                        tradeOutSettings.HedgePlaytime = Int32.Parse(settingsElement.Element("hedgeplaytime").Value);
                                    }

                                    if (settingsElement.Element("hedgewaittime") != null)
                                    {
                                        tradeOutSettings.HedgeWaitTime = Int32.Parse(settingsElement.Element("hedgewaittime").Value);
                                    }

                                    if (settingsElement.Element("onlyhedge") != null)
                                    {
                                        tradeOutSettings.OnlyHedge = Boolean.Parse(settingsElement.Element("onlyhedge").Value);
                                    }

                                    if (settingsElement.Element("usegreenminutes") != null)
                                    {
                                        tradeOutSettings.UseGreenWaitTime = Boolean.Parse(settingsElement.Element("usegreenminutes").Value);
                                    }

                                    if (settingsElement.Element("usehedgewaittime") != null)
                                    {
                                        tradeOutSettings.UseHedgeWaitTime = Boolean.Parse(settingsElement.Element("usehedgewaittime").Value);
                                    }

                                    if (settingsElement.Element("useoddspercentage") != null)
                                    {
                                        tradeOutSettings.UseOddsPercentage = Boolean.Parse(settingsElement.Element("useoddspercentage").Value);
                                    }

                                    if (settingsElement.Element("usewaittime") != null)
                                    {
                                        tradeOutSettings.UseWaitTime = Boolean.Parse(settingsElement.Element("usewaittime").Value);
                                    }
                                    #endregion

                                    #region Correct Score

                                    if (settingsElement.Element("csbchecklayodds") != null)
                                    {
                                        tradeOutSettings.CSBackCheckLayOdds = Boolean.Parse(settingsElement.Element("csbchecklayodds").Value);
                                    }


                                    if (settingsElement.Element("csbgreenpercentage") != null)
                                    {
                                        tradeOutSettings.CSBackGreenPercentage = Int32.Parse(settingsElement.Element("csbgreenpercentage").Value, CultureInfo.InvariantCulture);
                                    }

                                    if (settingsElement.Element("csbgreenplaytime") != null)
                                    {
                                        tradeOutSettings.CSBackGreenPlaytime = Int32.Parse(settingsElement.Element("csbgreenplaytime").Value, CultureInfo.InvariantCulture);
                                    }

                                    if (settingsElement.Element("csbgreenspecialplaytime") != null)
                                    {
                                        tradeOutSettings.CSBackGreenSpecialPlayTime = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), settingsElement.Element("csbgreenspecialplaytime").Value);
                                    }

                                    if (settingsElement.Element("csbgreespecialplaytimedelta") != null)
                                    {
                                        tradeOutSettings.CSBackGreenSpecialPlayTimeDelta = Int32.Parse(settingsElement.Element("csbgreenspecialplaytimedelta").Value, CultureInfo.InvariantCulture);
                                    }

                                    if (settingsElement.Element("csbgreenwaittime") != null)
                                    {
                                        tradeOutSettings.CSBackGreenWaittime = Int32.Parse(settingsElement.Element("csbgreenwaittime").Value, CultureInfo.InvariantCulture);
                                    }

                                    if (settingsElement.Element("csbhedgepercentage") != null)
                                    {
                                        tradeOutSettings.CSBackHedgePercentage = Int32.Parse(settingsElement.Element("csbhedgepercentage").Value, CultureInfo.InvariantCulture);
                                    }

                                    if (settingsElement.Element("csbhedgeplaytime") != null)
                                    {
                                        tradeOutSettings.CSBackHedgePlayTime = Int32.Parse(settingsElement.Element("csbhedgeplaytime").Value, CultureInfo.InvariantCulture);
                                    }

                                    if (settingsElement.Element("csbhedgespecialplaytime") != null)
                                    {
                                        tradeOutSettings.CSBackHedgeSpecialPlayTime = (SPECIALPLAYTIME)Enum.Parse(typeof(SPECIALPLAYTIME), settingsElement.Element("csbhedgespecialplaytime").Value);
                                    }

                                    if (settingsElement.Element("csbhedgespecialplaytimedelta") != null)
                                    {
                                        tradeOutSettings.CSBackHedgeSpecialPlayTimeDelta = Int32.Parse(settingsElement.Element("csbhedgespecialplaytimedelta").Value, CultureInfo.InvariantCulture);
                                    }

                                    if (settingsElement.Element("csbhedgewaittime") != null)
                                    {
                                        tradeOutSettings.CSBackHedgeWaitTime = Int32.Parse(settingsElement.Element("csbhedgewaittime").Value, CultureInfo.InvariantCulture);
                                    }

                                    if (settingsElement.Element("csbonlyhedge") != null)
                                    {
                                        tradeOutSettings.CSBackOnlyHedge = Boolean.Parse(settingsElement.Element("csbonlyhedge").Value);
                                    }

                                    if (settingsElement.Element("csbusegreenspecialplaytime") != null)
                                    {
                                        tradeOutSettings.CSBackUseGreenSpecialPlayTime = Boolean.Parse(settingsElement.Element("csbusegreenspecialplaytime").Value);
                                    }

                                    if (settingsElement.Element("csbusegreenwaittime") != null)
                                    {
                                        tradeOutSettings.CSBackUseGreenWaitTime = Boolean.Parse(settingsElement.Element("csbusegreenwaittime").Value);
                                    }

                                    if (settingsElement.Element("csbusehedgespecialplaytime") != null)
                                    {
                                        tradeOutSettings.CSBackUseHedgeSpecialPlayTime = Boolean.Parse(settingsElement.Element("csbusehedgespecialplaytime").Value);
                                    }

                                    if (settingsElement.Element("csbusehedgewaittime") != null)
                                    {
                                        tradeOutSettings.CSBackUseHedgeWaitTime = Boolean.Parse(settingsElement.Element("csbusehedgewaittime").Value);
                                    }

                                    if (settingsElement.Element("csbuseoddspercentage") != null)
                                    {
                                        tradeOutSettings.CSBackUseOddsPercentage = Boolean.Parse(settingsElement.Element("csbuseoddspercentage").Value);
                                    }

                                    if (settingsElement.Element("csbusewaittime") != null)
                                    {
                                        tradeOutSettings.CSBackUseWaitTime = Boolean.Parse(settingsElement.Element("csbusewaittime").Value);
                                    }                                    
                                    #endregion
                                    
                                    tradeOutSettings.TradeType = configElement.TradeType;

                                    tradeOutCheck.TradeOutSettings = tradeOutSettings;
                                }
                                /////////////////////////////////////////////////////////////////////////////////
                                // Überprüfungsregeln
                                /////////////////////////////////////////////////////////////////////////////////

                                if (ruleElement.Element("checkrules") != null)
                                {
                                    foreach (XElement checkElement in ruleElement.Element("checkrules").Elements())
                                    {
                                        TTRTradeOutRuleFragment fragment = TTRTradeOutRuleFragment.createFromXml(checkElement.ToString());
                                        if (fragment != null)
                                        {
                                            tradeOutCheck.Rules.Add(fragment);
                                        }
                                    }
                                }

                                configElement.TradeConfig.TradeOutRules.Add(tradeOutCheck.Order, tradeOutCheck);
                            }

                        }
                    }
                    catch (ArgumentNullException ane)
                    {
                        ExceptionWriter.Instance.WriteException(ane);
                    }
                    catch (FormatException fe)
                    {
                        ExceptionWriter.Instance.WriteException(fe);
                    }
                    catch (OverflowException oe)
                    {
                        ExceptionWriter.Instance.WriteException(oe);
                    }
                    list.Add(configElement);


                }

            }
            catch (Exception e)
            {
                MessageBox.Show("There was an unexpected error while opening file", "Error while loading Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExceptionWriter.Instance.WriteException(e);
                return null;
            }
            return list;
        }

    
    


        public static TRADEMODE IsTradeTypeBackMode(TRADETYPE tradeType)
        {
            switch (tradeType)
            {
                case TRADETYPE.SCORELINE00:
                case TRADETYPE.SCORELINE01BACK:
                case TRADETYPE.SCORELINE02BACK:
                case TRADETYPE.SCORELINE03BACK:
                case TRADETYPE.SCORELINE10BACK:
                case TRADETYPE.SCORELINE11BACK:
                case TRADETYPE.SCORELINE12BACK:
                case TRADETYPE.SCORELINE13BACK:
                case TRADETYPE.SCORELINE20BACK:
                case TRADETYPE.SCORELINE21BACK:
                case TRADETYPE.SCORELINE22BACK:
                case TRADETYPE.SCORELINE23BACK:
                case TRADETYPE.SCORELINE30BACK:
                case TRADETYPE.SCORELINE31BACK:
                case TRADETYPE.SCORELINE32BACK:
                case TRADETYPE.SCORELINE33BACK:
                case TRADETYPE.SCORELINEOTHERBACK:
                case TRADETYPE.OVER05:
                case TRADETYPE.OVER15:
                case TRADETYPE.OVER25:
                case TRADETYPE.OVER35:
                case TRADETYPE.OVER45:
                case TRADETYPE.OVER55:
                case TRADETYPE.OVER65:
                case TRADETYPE.OVER75:
                case TRADETYPE.OVER85:
                    return TRADEMODE.BACK;
            }
            return TRADEMODE.LAY;
        }

        public static TRADETYPE GetReverseTradeType(TRADETYPE t)
        {
            switch(t)
            {
                case TRADETYPE.SCORELINE01BACK:
                    return TRADETYPE.SCORELINE01LAY;
                case TRADETYPE.SCORELINE01LAY:
                    return TRADETYPE.SCORELINE01BACK;
                case TRADETYPE.SCORELINE02BACK:
                    return TRADETYPE.SCORELINE02LAY;
                case TRADETYPE.SCORELINE02LAY:
                    return TRADETYPE.SCORELINE02BACK;
                case TRADETYPE.SCORELINE03BACK:
                    return TRADETYPE.SCORELINE03LAY;
                case TRADETYPE.SCORELINE03LAY:
                    return TRADETYPE.SCORELINE03BACK;
                case TRADETYPE.SCORELINE10BACK:
                    return TRADETYPE.SCORELINE10LAY;
                case TRADETYPE.SCORELINE10LAY:
                    return TRADETYPE.SCORELINE10BACK;
                case TRADETYPE.SCORELINE11BACK:
                    return TRADETYPE.SCORELINE11LAY;
                case TRADETYPE.SCORELINE11LAY:
                    return TRADETYPE.SCORELINE11BACK;
                case TRADETYPE.SCORELINE12BACK:
                    return TRADETYPE.SCORELINE12LAY;
                case TRADETYPE.SCORELINE12LAY:
                    return TRADETYPE.SCORELINE12BACK;
                case TRADETYPE.SCORELINE13BACK:
                    return TRADETYPE.SCORELINE13LAY;
                case TRADETYPE.SCORELINE13LAY:
                    return TRADETYPE.SCORELINE13BACK;
                case TRADETYPE.SCORELINE20BACK:
                    return TRADETYPE.SCORELINE20LAY;
                case TRADETYPE.SCORELINE20LAY:
                    return TRADETYPE.SCORELINE20BACK;
                case TRADETYPE.SCORELINE21BACK:
                    return TRADETYPE.SCORELINE21LAY;
                case TRADETYPE.SCORELINE21LAY:
                    return TRADETYPE.SCORELINE21BACK;
                case TRADETYPE.SCORELINE22BACK:
                    return TRADETYPE.SCORELINE22LAY;
                case TRADETYPE.SCORELINE22LAY:
                    return TRADETYPE.SCORELINE22BACK;
                case TRADETYPE.SCORELINE23BACK:
                    return TRADETYPE.SCORELINE23LAY;
                case TRADETYPE.SCORELINE23LAY:
                    return TRADETYPE.SCORELINE23BACK;
                case TRADETYPE.SCORELINE30BACK:
                    return TRADETYPE.SCORELINE30LAY;
                case TRADETYPE.SCORELINE30LAY:
                    return TRADETYPE.SCORELINE30BACK;
                case TRADETYPE.SCORELINE31BACK:
                    return TRADETYPE.SCORELINE31LAY;
                case TRADETYPE.SCORELINE31LAY:
                    return TRADETYPE.SCORELINE31BACK;
                case TRADETYPE.SCORELINE32BACK:
                    return TRADETYPE.SCORELINE32LAY;
                case TRADETYPE.SCORELINE32LAY:
                    return TRADETYPE.SCORELINE32BACK;
                case TRADETYPE.SCORELINE33BACK:
                    return TRADETYPE.SCORELINE33LAY;
                case TRADETYPE.SCORELINE33LAY:
                    return TRADETYPE.SCORELINE33BACK;
                case TRADETYPE.SCORELINEOTHERBACK:
                    return TRADETYPE.SCORELINEOTHERLAY;
                case TRADETYPE.SCORELINEOTHERLAY:
                    return TRADETYPE.SCORELINEOTHERBACK;

            }
            return TRADETYPE.UNASSIGNED;
        }

        public static TRADETYPE GetTradeTypeByBetAndSelection(SXALBet bet)
        {
            SXALSelectionIdEnum selectionId = SXALKom.Instance.getReverseSelectionId(bet.SelectionId, bet.MarketId);
            switch (selectionId)
            {
                case SXALSelectionIdEnum.OVER05:
                    return TRADETYPE.OVER05;
                case SXALSelectionIdEnum.OVER15:
                    return TRADETYPE.OVER15;
                case SXALSelectionIdEnum.OVER25:
                    return TRADETYPE.OVER25;
                case SXALSelectionIdEnum.OVER35:
                    return TRADETYPE.OVER35;
                case SXALSelectionIdEnum.OVER45:
                    return TRADETYPE.OVER45;
                case SXALSelectionIdEnum.OVER55:
                    return TRADETYPE.OVER55;
                case SXALSelectionIdEnum.OVER65:
                    return TRADETYPE.OVER65;
                case SXALSelectionIdEnum.OVER75:
                    return TRADETYPE.OVER75;
                case SXALSelectionIdEnum.OVER85:
                    return TRADETYPE.OVER85;
                case SXALSelectionIdEnum.CSZEROZERO:
                    return TRADETYPE.SCORELINE00;
                case SXALSelectionIdEnum.CSZEROONE:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE01BACK;
                    else return TRADETYPE.SCORELINE01LAY;
                case SXALSelectionIdEnum.CSZEROTWO:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE02BACK;
                    else return TRADETYPE.SCORELINE02LAY;
                case SXALSelectionIdEnum.CSZEROTHREE:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE03BACK;
                    else return TRADETYPE.SCORELINE03LAY;
                case SXALSelectionIdEnum.CSONEZERO:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE10BACK;
                    else return TRADETYPE.SCORELINE10LAY;
                case SXALSelectionIdEnum.CSONEONE:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE11BACK;
                    else return TRADETYPE.SCORELINE11LAY;
                case SXALSelectionIdEnum.CSONETWO:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE12BACK;
                    else return TRADETYPE.SCORELINE12LAY;
                case SXALSelectionIdEnum.CSONETHREE:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE13BACK;
                    else return TRADETYPE.SCORELINE30LAY;
                case SXALSelectionIdEnum.CSTWOZERO:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE20BACK;
                    else return TRADETYPE.SCORELINE20LAY;
                case SXALSelectionIdEnum.CSTWOONE:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE21BACK;
                    else return TRADETYPE.SCORELINE21LAY;
                case SXALSelectionIdEnum.CSTWOTWO:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE22BACK;
                    else return TRADETYPE.SCORELINE22LAY;
                case SXALSelectionIdEnum.CSTWOTHREE:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE23BACK;
                    else return TRADETYPE.SCORELINE23LAY;
                case SXALSelectionIdEnum.CSTHREEZERO:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE30BACK;
                    else return TRADETYPE.SCORELINE30LAY;
                case SXALSelectionIdEnum.CSTHREEONE:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE31BACK;
                    else return TRADETYPE.SCORELINE31LAY;
                case SXALSelectionIdEnum.CSTHREETWO:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE32BACK;
                    else return TRADETYPE.SCORELINE32LAY;
                case SXALSelectionIdEnum.CSTHREETHREE:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINE33BACK;
                    else return TRADETYPE.SCORELINE33LAY;
                case SXALSelectionIdEnum.CSOTHER:
                    if (bet.BetType == SXALBetTypeEnum.B)
                        return TRADETYPE.SCORELINEOTHERBACK;
                    else return TRADETYPE.SCORELINEOTHERLAY;

            }
            return TRADETYPE.UNASSIGNED;
        }
        public static long GetSelectionIdByTradeType(TRADETYPE tradeType, long marketId)
        {
            switch (tradeType)
            {
                case TRADETYPE.SCORELINE00:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, marketId);
                case TRADETYPE.OVER05:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER05, marketId);
                case TRADETYPE.OVER15:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER15, marketId);
                case TRADETYPE.OVER25:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER25, marketId);
                case TRADETYPE.OVER35:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER35, marketId);
                case TRADETYPE.OVER45:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER45, marketId);
                case TRADETYPE.OVER55:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER55, marketId);
                case TRADETYPE.OVER65:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER65, marketId);
                case TRADETYPE.OVER75:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER75, marketId);
                case TRADETYPE.OVER85:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.OVER85, marketId);
                case TRADETYPE.SCORELINE01BACK:
                case TRADETYPE.SCORELINE01LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROONE, marketId);
                case TRADETYPE.SCORELINE02BACK:
                case TRADETYPE.SCORELINE02LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROTWO, marketId);
                case TRADETYPE.SCORELINE03BACK:
                case TRADETYPE.SCORELINE03LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROTHREE, marketId);
                case TRADETYPE.SCORELINE10BACK:
                case TRADETYPE.SCORELINE10LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSONEZERO, marketId);
                case TRADETYPE.SCORELINE11BACK:
                case TRADETYPE.SCORELINE11LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSONEONE, marketId);
                case TRADETYPE.SCORELINE12BACK:
                case TRADETYPE.SCORELINE12LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSONETWO, marketId);
                case TRADETYPE.SCORELINE13BACK:
                case TRADETYPE.SCORELINE13LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSONETHREE, marketId);
                case TRADETYPE.SCORELINE20BACK:
                case TRADETYPE.SCORELINE20LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSTWOZERO, marketId);
                case TRADETYPE.SCORELINE21BACK:
                case TRADETYPE.SCORELINE21LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSTWOONE, marketId);
                case TRADETYPE.SCORELINE22BACK:
                case TRADETYPE.SCORELINE22LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSTWOTWO, marketId);
                case TRADETYPE.SCORELINE23BACK:
                case TRADETYPE.SCORELINE23LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSTWOTHREE, marketId);
                case TRADETYPE.SCORELINE30BACK:
                case TRADETYPE.SCORELINE30LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSTHREEZERO, marketId);
                case TRADETYPE.SCORELINE31BACK:
                case TRADETYPE.SCORELINE31LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSTHREEONE, marketId);
                case TRADETYPE.SCORELINE32BACK:
                case TRADETYPE.SCORELINE32LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSTHREETWO, marketId);
                case TRADETYPE.SCORELINE33BACK:
                case TRADETYPE.SCORELINE33LAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSTHREETHREE, marketId);
                case TRADETYPE.SCORELINEOTHERBACK:
                case TRADETYPE.SCORELINEOTHERLAY:
                    return SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSOTHER, marketId);
            }
            return 0;
        }

        public static SXALMarket GetMarketByTradeType(TRADETYPE tradeType, String match)
        {
            switch (tradeType)
            {
                case TRADETYPE.SCORELINE00:
                    return SXALSoccerMarketManager.Instance.getCSMarketByMatch(match);
                case TRADETYPE.OVER05:
                    return SXALSoccerMarketManager.Instance.getOU05MarketByMatch(match);
                case TRADETYPE.OVER15:
                    return SXALSoccerMarketManager.Instance.getOU15MarketByMatch(match);
                case TRADETYPE.OVER25:
                    return SXALSoccerMarketManager.Instance.getOU25MarketByMatch(match);
                case TRADETYPE.OVER35:
                    return SXALSoccerMarketManager.Instance.getOU35MarketByMatch(match);
                case TRADETYPE.OVER45:
                    return SXALSoccerMarketManager.Instance.getOU45MarketByMatch(match);
                case TRADETYPE.OVER55:
                    return SXALSoccerMarketManager.Instance.getOU55MarketByMatch(match);
                case TRADETYPE.OVER65:
                    return SXALSoccerMarketManager.Instance.getOU65MarketByMatch(match);
                case TRADETYPE.OVER75:
                    return SXALSoccerMarketManager.Instance.getOU75MarketByMatch(match);
                case TRADETYPE.OVER85:
                    return SXALSoccerMarketManager.Instance.getOU85MarketByMatch(match);
                case TRADETYPE.SCORELINE01BACK:
                case TRADETYPE.SCORELINE01LAY:
                case TRADETYPE.SCORELINE02BACK:
                case TRADETYPE.SCORELINE02LAY:
                case TRADETYPE.SCORELINE03BACK:
                case TRADETYPE.SCORELINE03LAY:
                case TRADETYPE.SCORELINE10BACK:
                case TRADETYPE.SCORELINE10LAY:
                case TRADETYPE.SCORELINE11BACK:
                case TRADETYPE.SCORELINE11LAY:
                case TRADETYPE.SCORELINE12BACK:
                case TRADETYPE.SCORELINE12LAY:
                case TRADETYPE.SCORELINE13BACK:
                case TRADETYPE.SCORELINE13LAY:
                case TRADETYPE.SCORELINE20BACK:
                case TRADETYPE.SCORELINE20LAY:
                case TRADETYPE.SCORELINE21BACK:
                case TRADETYPE.SCORELINE21LAY:
                case TRADETYPE.SCORELINE22BACK:
                case TRADETYPE.SCORELINE22LAY:
                case TRADETYPE.SCORELINE23BACK:
                case TRADETYPE.SCORELINE23LAY:
                case TRADETYPE.SCORELINE30BACK:
                case TRADETYPE.SCORELINE30LAY:
                case TRADETYPE.SCORELINE31BACK:
                case TRADETYPE.SCORELINE31LAY:
                case TRADETYPE.SCORELINE32BACK:
                case TRADETYPE.SCORELINE32LAY:
                case TRADETYPE.SCORELINE33BACK:
                case TRADETYPE.SCORELINE33LAY:
                case TRADETYPE.SCORELINEOTHERBACK:
                case TRADETYPE.SCORELINEOTHERLAY:
                    return SXALSoccerMarketManager.Instance.getCSMarketByMatch(match);
            }
            return null;
        }

        public static void FillSpecialPlaytimeCombobox(ComboBox cbx)
        {
            DataSet dsSpecial = new DataSet();
            DataTable dtSpecial = new DataTable("Selektion");
            DataColumn dcDisplay = new DataColumn("Display");
            DataColumn dcValue = new DataColumn("Value");

            dtSpecial.Columns.Add(dcDisplay);
            dtSpecial.Columns.Add(dcValue);

            DataRow drUnassigned = dtSpecial.NewRow();
            drUnassigned["Display"] = TradeTheReaction.strUnassigned;
            drUnassigned["Value"] = SPECIALPLAYTIME.UNASSIGNED;
            dtSpecial.Rows.Add(drUnassigned);

            DataRow drEarliest = dtSpecial.NewRow();
            drEarliest["Display"] = TradeTheReaction.strEarliestGoal;
            drEarliest["Value"] = SPECIALPLAYTIME.EARLIESTGOAL;

            dtSpecial.Rows.Add(drEarliest);

            DataRow drAverage = dtSpecial.NewRow();
            drAverage["Display"] = TradeTheReaction.strAverageFirstGoal;
            drAverage["Value"] = SPECIALPLAYTIME.AVERAGEGOAL;

            dtSpecial.Rows.Add(drAverage);

            DataRow drLatest = dtSpecial.NewRow();
            drLatest["Display"] = TradeTheReaction.strLatestFirstGoal;
            drLatest["Value"] = SPECIALPLAYTIME.LATESTGOAL;

            dtSpecial.Rows.Add(drLatest);

            dsSpecial.Tables.Add(dtSpecial);

            cbx.SuspendLayout();
            cbx.DataSource = dsSpecial.Tables["Selektion"];
            cbx.DisplayMember = "Display";
            cbx.ValueMember = "Value";
            cbx.SelectedIndex = 0;
            cbx.ResumeLayout();
        }

        public static void FillTradeStateComboBox(ComboBox cbx)
        {
            if (cbx == null)
                return;

            try
            {
                cbx.Items.Clear();                
            }
            catch
            {
            }

            // Komboboxen füllen
            DataSet dsTTR = new DataSet();
            DataTable dtTTR = new DataTable("Selektion");
            DataColumn dcDisplay = new DataColumn("Display");
            DataColumn dcValue = new DataColumn("Value");

            dtTTR.Columns.Add(dcDisplay);
            dtTTR.Columns.Add(dcValue);

            DataRow drOpened = dtTTR.NewRow();
            drOpened["Display"] = TradeTheReaction.strOpened;
            drOpened["Value"] = TRADESTATE.OPENED;

            dtTTR.Rows.Add(drOpened);

            DataRow drHedged = dtTTR.NewRow();
            drHedged["Display"] = TradeTheReaction.strHedged;
            drHedged["Value"] = TRADESTATE.HEDGED;

            dtTTR.Rows.Add(drHedged);

            DataRow drGreened = dtTTR.NewRow();
            drGreened["Display"] = TradeTheReaction.strGreened;
            drGreened["Value"] = TRADESTATE.GREENED;

            dtTTR.Rows.Add(drGreened);

            DataRow drNoMatter = dtTTR.NewRow();
            drNoMatter["Display"] = TradeTheReaction.strNoMatter;
            drNoMatter["Value"] = TRADESTATE.NOMATTER;

            dtTTR.Rows.Add(drNoMatter);

            

            dsTTR.Tables.Add(dtTTR);

            cbx.SuspendLayout();
            cbx.DataSource = dsTTR.Tables["Selektion"];
            cbx.DisplayMember = "Display";
            cbx.ValueMember = "Value";
            cbx.SelectedIndex = -1;
            cbx.ResumeLayout();
        }

        public static void FillTradeTypeComboBox(ComboBox cbx)
        {
            if (cbx == null)
                return;
            try
            {
                cbx.Items.Clear();
            }
            catch { }

            // Komboboxen füllen
            DataSet dsTTR = new DataSet();
            DataTable dtTTR = new DataTable("Selektion");
            DataColumn dcDisplay = new DataColumn("Display");
            DataColumn dcValue = new DataColumn("Value");

            dtTTR.Columns.Add(dcDisplay);
            dtTTR.Columns.Add(dcValue);



            /* Für Version 2.2 erst einmal auskommentiert
            fillRow(dtTTR, TRADETYPE.OVER05, TradeTheReaction.strOver05);
            fillRow(dtTTR, TRADETYPE.OVER15, TradeTheReaction.strOver15);
            fillRow(dtTTR, TRADETYPE.OVER25, TradeTheReaction.strOver25);
            fillRow(dtTTR, TRADETYPE.OVER35, TradeTheReaction.strOver35);
            fillRow(dtTTR, TRADETYPE.OVER45, TradeTheReaction.strOver45);
            fillRow(dtTTR, TRADETYPE.OVER55, TradeTheReaction.strOver55);
            fillRow(dtTTR, TRADETYPE.OVER65, TradeTheReaction.strOver65);
            fillRow(dtTTR, TRADETYPE.OVER75, TradeTheReaction.strOver75);
            fillRow(dtTTR, TRADETYPE.OVER85, TradeTheReaction.strOver85);
            */

            fillRow(dtTTR, TRADETYPE.SCORELINE00, TradeTheReaction.strScoreLine00);
            fillRow(dtTTR, TRADETYPE.SCORELINE01BACK, TradeTheReaction.strScoreline01Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE01LAY, TradeTheReaction.strScoreline01Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE02BACK, TradeTheReaction.strScoreline02Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE02LAY, TradeTheReaction.strScoreline02Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE03BACK, TradeTheReaction.strScoreline03Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE03LAY, TradeTheReaction.strScoreline03Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE10BACK, TradeTheReaction.strScoreline10Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE10LAY, TradeTheReaction.strScoreline10Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE11BACK, TradeTheReaction.strScoreline11Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE11LAY, TradeTheReaction.strScoreline11Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE12BACK, TradeTheReaction.strScoreline12Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE12LAY, TradeTheReaction.strScoreline12Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE13BACK, TradeTheReaction.strScoreline13Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE13LAY, TradeTheReaction.strScoreline13Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE20BACK, TradeTheReaction.strScoreline20Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE20LAY, TradeTheReaction.strScoreline20Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE21BACK, TradeTheReaction.strScoreline21Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE21LAY, TradeTheReaction.strScoreline21Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE22BACK, TradeTheReaction.strScoreline22Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE22LAY, TradeTheReaction.strScoreline22Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE23BACK, TradeTheReaction.strScoreline23Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE23LAY, TradeTheReaction.strScoreline23Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE30BACK, TradeTheReaction.strScoreline30Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE30LAY, TradeTheReaction.strScoreline30Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE31BACK, TradeTheReaction.strScoreline31Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE31LAY, TradeTheReaction.strScoreline31Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE32BACK, TradeTheReaction.strScoreline32Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE32LAY, TradeTheReaction.strScoreline32Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINE33BACK, TradeTheReaction.strScoreline33Back);
            fillRow(dtTTR, TRADETYPE.SCORELINE33LAY, TradeTheReaction.strScoreline33Lay);
            fillRow(dtTTR, TRADETYPE.SCORELINEOTHERBACK, TradeTheReaction.strScorelineOthersBack);
            fillRow(dtTTR, TRADETYPE.SCORELINEOTHERLAY, TradeTheReaction.strScorelineOtherLay);

            fillRow(dtTTR, TRADETYPE.UNASSIGNED, TradeTheReaction.strUnassigned);
           
            dsTTR.Tables.Add(dtTTR);

            cbx.SuspendLayout();
            cbx.DataSource = dsTTR.Tables["Selektion"];
            cbx.DisplayMember = "Display";
            cbx.ValueMember = "Value";
            cbx.SelectedIndex = -1;
            cbx.ResumeLayout();
        }

        private static void fillRow(DataTable dt, TRADETYPE tradeType, String text)
        {
            DataRow dr = dt.NewRow();
            dr["Display"] = text;
            dr["Value"] = tradeType;

            dt.Rows.Add(dr);
        }

    }
}
