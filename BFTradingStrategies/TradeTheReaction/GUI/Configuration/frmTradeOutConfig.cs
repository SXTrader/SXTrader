using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.ttr.TradeRules;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.Scoreline00.Controls;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    public partial class frmTradeOutConfig : Form
    {
        private TTRTradeOutCheckSortedList _theList = new TTRTradeOutCheckSortedList();
        private TRADETYPE _tradeType = TRADETYPE.UNASSIGNED;
        private ctlTradeOutRuleEditAbstract _ctl;

        public TTRTradeOutCheckSortedList TradeOutCheckList
        {
            get
            {
                return _theList;
            }
            set
            {
                _theList = value;
                //Bisherigen Configurationsbildschirm zurücksetzen
                _ctl.TradeOutCheck = null;
                
                while(this.pnlDisplayRules.Controls.Count > 0)
                {
                    this.pnlDisplayRules.Controls[0].Dispose();
                }
                pnlDisplayRules.AutoScroll = true;

                if (_theList != null)
                {

                    // Edit Control neu bestimmen
                    if (_theList.TradeType != TRADETYPE.UNASSIGNED)
                    {
                        getTradeOutRuleEdit(_theList.TradeType);
                    }
                    else
                    {
                        getTradeOutRuleEdit(_tradeType);
                    }

                    foreach (TTRTradeOutCheck tradeOutCheck in _theList.Values)
                    {
                        ctlTradeOutRuleDisplay ctlDisplay = new ctlTradeOutRuleDisplay(tradeOutCheck);
                        ctlDisplay.EditTOConfigElement += new EventHandler<TTRTOElementEditEventArgs>(ctlDisplay_EditTOConfigElement);
                        ctlDisplay.DeleteTOConfigElement += new EventHandler<TTRTOElementDeleteEventArgs>(ctlDisplay_DeleteTOConfigElement);
                        ctlDisplay.BorderStyle = BorderStyle.FixedSingle;
                        ctlDisplay.Dock = DockStyle.Top;
                        pnlDisplayRules.Controls.Add(ctlDisplay);
                    }
                    pnlDisplayRules.AutoScroll = true;
                }
            }
        }

        public void setTradeOutCheckList(IConfiguration config)
        {
            TTRConfigurationRW ttrConfig = config as TTRConfigurationRW;
            getTradeOutRuleEdit(ttrConfig.TradeOutRules.TradeType);
            if (ttrConfig != null)
            {
                this.TradeOutCheckList = ttrConfig.TradeOutRules;
            }
        }

        public void getTradeOutCheckList(ref IConfiguration config)
        {
            TTRConfigurationRW ttrConfig = config as TTRConfigurationRW;
            if (ttrConfig != null)
            {
                ttrConfig.TradeOutRules = this.TradeOutCheckList;
            }
        }
        void ctlDisplay_DeleteTOConfigElement(object sender, TTRTOElementDeleteEventArgs e)
        {
            try
            {
                for (int i = 0; i < _theList.Keys.Count; i++)
                {
                    if (_theList[_theList.Keys[i]].Id == e.TradeOutElement.Id)
                    {
                        _theList.RemoveAt(i);
                        break;
                    }
                }

                foreach (Control ctrl in pnlDisplayRules.Controls)
                {
                    ctlTradeOutRuleDisplay element = ctrl as ctlTradeOutRuleDisplay;

                    if (element != null && element.TradeOutElement.Id == e.TradeOutElement.Id)
                    {
                        pnlDisplayRules.Controls.Remove(ctrl);
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

        void ctlDisplay_EditTOConfigElement(object sender, TTRTOElementEditEventArgs e)
        {
            getTradeOutRuleEdit(e.TradeOutElement.TradeOutSettings.TradeType);
            _ctl.TradeOutCheck = e.TradeOutElement;
        }

        public frmTradeOutConfig()
        {
            InitializeComponent();
            _ctl = new ctlTradeOutRuleEditAbstract();
            _ctl.SaveTOElement += new EventHandler<TTRTOElementSaveEventArgs>(ctl_SaveTOElement);
            pnlConfigRule.Controls.Add(_ctl);            
        }

        public frmTradeOutConfig(TRADETYPE tradeType)
        {
            InitializeComponent();
            _tradeType = tradeType;
            getTradeOutRuleEdit(_tradeType);//new ctlTradeOutRuleEditAbstract();
            //_ctl.SaveTOElement += new EventHandler<TTRTOElementSaveEventArgs>(ctl_SaveTOElement);
            pnlConfigRule.Controls.Add(_ctl);            
        }

        private void getTradeOutRuleEdit(TRADETYPE tradeType)
        {
            if (_ctl != null)
            {
                pnlConfigRule.Controls.Remove(_ctl);
                _ctl.SaveTOElement -= ctl_SaveTOElement;
                _ctl.Dispose();
                _ctl = null;
            }

            switch (tradeType)
            {
                case TRADETYPE.OVER05:
                case TRADETYPE.OVER15:
                case TRADETYPE.OVER25:
                case TRADETYPE.OVER35:
                case TRADETYPE.OVER45:
                case TRADETYPE.OVER55:
                case TRADETYPE.OVER65:
                case TRADETYPE.OVER75:
                case TRADETYPE.OVER85:
                    _ctl = new ctlTradeOutRuleEditOver(tradeType);
                    _ctl.SaveTOElement += new EventHandler<TTRTOElementSaveEventArgs>(ctl_SaveTOElement);
                    pnlConfigRule.Controls.Add(_ctl);
                    break;
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
                    _ctl = new ctlTradeOutRuleEditCSBack(tradeType);
                    _ctl.SaveTOElement += new EventHandler<TTRTOElementSaveEventArgs>(ctl_SaveTOElement);
                    pnlConfigRule.Controls.Add(_ctl);
                    break;
                case TRADETYPE.SCORELINE01LAY:
                case TRADETYPE.SCORELINE02LAY:
                case TRADETYPE.SCORELINE03LAY:
                case TRADETYPE.SCORELINE10LAY:
                case TRADETYPE.SCORELINE11LAY:
                case TRADETYPE.SCORELINE12LAY:
                case TRADETYPE.SCORELINE13LAY:
                case TRADETYPE.SCORELINE20LAY:
                case TRADETYPE.SCORELINE21LAY:
                case TRADETYPE.SCORELINE22LAY:
                case TRADETYPE.SCORELINE23LAY:
                case TRADETYPE.SCORELINE30LAY:
                case TRADETYPE.SCORELINE31LAY:
                case TRADETYPE.SCORELINE32LAY:
                case TRADETYPE.SCORELINE33LAY:
                case TRADETYPE.SCORELINEOTHERLAY:
                    _ctl = new ctlTradeOutRuleEditCSLay(tradeType);
                    _ctl.SaveTOElement += new EventHandler<TTRTOElementSaveEventArgs>(ctl_SaveTOElement);
                    pnlConfigRule.Controls.Add(_ctl);
                    break;
                default:
                    throw new Exception(String.Format("Unknown TradeType {0}", tradeType));
            }
        }

        void ctl_SaveTOElement(object sender, TTRTOElementSaveEventArgs e)
        {
            try
            {
                if (_theList == null)
                    _theList = new TTRTradeOutCheckSortedList();

                if (_theList.ContainsKey(e.TradeOutElement.Order))
                {
                    // Das gleiche Element nur aktualisiert?
                    if (_theList[e.TradeOutElement.Order].Id == e.TradeOutElement.Id)
                    {
                        _theList[e.TradeOutElement.Order] = e.TradeOutElement;
                    }
                    else
                    {
                        // Kollision
                        TTRTradeOutCheck tmp = _theList[e.TradeOutElement.Order];
                        tmp.Order = _theList.Keys.Last() + 1;
                        _theList.Add(tmp.Order, tmp);

                        ctlTradeOutRuleDisplay ctlDisplay = new ctlTradeOutRuleDisplay(tmp);
                        ctlDisplay.EditTOConfigElement += new EventHandler<TTRTOElementEditEventArgs>(ctlDisplay_EditTOConfigElement);
                        ctlDisplay.DeleteTOConfigElement += new EventHandler<TTRTOElementDeleteEventArgs>(ctlDisplay_DeleteTOConfigElement);
                        ctlDisplay.BorderStyle = BorderStyle.FixedSingle;
                        ctlDisplay.Dock = DockStyle.Top;
                        pnlDisplayRules.Controls.Add(ctlDisplay);

                        _theList[e.TradeOutElement.Order] = e.TradeOutElement;

                        foreach (Control ctrl in pnlDisplayRules.Controls)
                        {
                            ctlTradeOutRuleDisplay element = ctrl as ctlTradeOutRuleDisplay;
                            if (element != null && element.TradeOutElement.Id == tmp.Id)
                            {
                                element.TradeOutElement = e.TradeOutElement;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    //Hat sich die Order-Id geändert (aber es gibt kein Element mit der gleichen Order-Id?
                    int iCounter = 0;
                    while (iCounter < _theList.Count)
                    {
                        if (_theList.Values[iCounter] == e.TradeOutElement)
                        {
                            foreach (Control ctrl in pnlDisplayRules.Controls)
                            {
                                ctlTradeOutRuleDisplay element = ctrl as ctlTradeOutRuleDisplay;

                                if (element != null && element.TradeOutElement.Id == e.TradeOutElement.Id)
                                {
                                    pnlDisplayRules.Controls.Remove(ctrl);
                                    ctrl.Dispose();
                                    break;
                                }
                            }

                            _theList.RemoveAt(iCounter);
                            break;
                        }
                        else iCounter++;
                    }


                    _theList.Add(e.TradeOutElement.Order, e.TradeOutElement);
                    ctlTradeOutRuleDisplay ctlDisplay = new ctlTradeOutRuleDisplay(e.TradeOutElement);
                    ctlDisplay.EditTOConfigElement += new EventHandler<TTRTOElementEditEventArgs>(ctlDisplay_EditTOConfigElement);
                    ctlDisplay.DeleteTOConfigElement += new EventHandler<TTRTOElementDeleteEventArgs>(ctlDisplay_DeleteTOConfigElement);
                    ctlDisplay.BorderStyle = BorderStyle.FixedSingle;
                    ctlDisplay.Dock = DockStyle.Top;
                    pnlDisplayRules.Controls.Add(ctlDisplay);

                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
