using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.GUI;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder.Controls;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.tradeinterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.TradeStarter
{
    public partial class ctlASConfigElement : UserControl
    {
        private TradeStarterConfigElement _element;

        public event EventHandler<TTRASElementSaveEventArgs> SaveASConfigElement;

        public TradeStarterConfigElement TSConfigElement
        {
            get { return _element; }
            set
            {
                //TODO: Sub-GUI laden und Elemente setzen
                _element = value;

                if (_element != null)
                {
                    cbxTradeType.SelectedValue = _element.TradeType;
                }
            }
        }

        public ctlASConfigElement()
        {
            InitializeComponent();
            //fillTradeTypeComboBox();
            TTRHelper.FillTradeTypeComboBox(cbxTradeType);            

            cbxTradeType.Enabled = false;
        }
        
       
        private bool checkValues(ref String msg)
        {
            msg = String.Empty;

            if(pnlASElementConfig.Controls.Count >0)
            {
                Control ctl = pnlASElementConfig.Controls[0];
                if (ctl.GetType() == typeof(ctlSL00ConfigElement))
                {
                    (ctl as ctlSL00ConfigElement).checkValues(ref msg);
                }
            }
            if (msg == String.Empty)
                return true;
            else return false;
        }

        private void ctlASConfigElement_SizeChanged(object sender, EventArgs e)
        {
            btnCancel.Left = this.Right - 5 - btnCancel.Width;
            btnSave.Left = btnCancel.Left - 5 - btnSave.Width;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.TSConfigElement = null;
            this.TSConfigElement = TradeStarterConfigElement.createNew();

            cbxTradeType.Enabled = true;
            cbxTradeType.SelectedIndex = -1;

            while(this.pnlASElementConfig.Controls.Count > 0)
            {
                this.pnlASElementConfig.Controls[0].Dispose();
            }
        }

        private void cbxTradeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (pnlASElementConfig == null)
                    return;

                while(this.pnlASElementConfig.Controls.Count > 0)
                {
                    this.pnlASElementConfig.Controls[0].Dispose();
                }
                ComboBox cbx = sender as ComboBox;
                if (cbx != null && cbx.SelectedValue != null && _element != null)
                {
                    String str = cbx.SelectedValue.ToString();
                    TRADETYPE tradeType = (TRADETYPE)Enum.Parse(typeof(TRADETYPE), str);
                    UserControl ctl = (UserControl)loadSubConfig(tradeType, _element);
                    if (ctl != null)
                    {

                        //ctl.Dock = DockStyle.Fill;
                        //ctl.AutoScroll = true;
                        pnlASElementConfig.Height = ctl.Height;
                        //pnlASElementConfig.AutoScroll = true;                    
                        pnlASElementConfig.Controls.Add(ctl);
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private Control loadSubConfig(TRADETYPE tradeType, TradeStarterConfigElement element)
        {
            try
            {
                switch (tradeType)
                {
                    case TRADETYPE.SCORELINE00:
                        {
                            ctlSL00ConfigElement ctl = new ctlSL00ConfigElement(tradeType);
                            ctl.TSConfigElement = element;
                            return ctl;
                        }
                    case TRADETYPE.OVER05:
                    case TRADETYPE.OVER15:
                    case TRADETYPE.OVER25:
                    case TRADETYPE.OVER35:
                    case TRADETYPE.OVER45:
                    case TRADETYPE.OVER55:
                    case TRADETYPE.OVER65:
                    case TRADETYPE.OVER75:
                    case TRADETYPE.OVER85:
                        {
                            ctlASOUConfigElement ctl = new ctlASOUConfigElement(tradeType);
                            ctl.TSConfigElement = element;
                            return ctl;
                        }   
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
                        {
                            ctlASCSConfigElementBack ctl = new ctlASCSConfigElementBack(tradeType);
                            ctl.TSConfigElement = element;
                            return ctl;
                        }
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
                        {
                            ctlASCSConfigElementLay ctl = new ctlASCSConfigElementLay(tradeType);
                            ctl.TSConfigElement = element;
                            return ctl;
                        }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_element != null)
                {
                    String msg = String.Empty;
                    if (!checkValues(ref msg))
                    {
                        MessageBox.Show(msg, "Invalid Values", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (cbxTradeType.SelectedIndex != -1)
                    {
                        _element.TradeType = (TRADETYPE)Enum.Parse(typeof(TRADETYPE), cbxTradeType.SelectedValue.ToString());//(TRADETYPE)cbxTradeType.SelectedValue;
                        switch (_element.TradeType)
                        {
                            case TRADETYPE.SCORELINE00:
                                {
                                    ctlSL00ConfigElement ctl = pnlASElementConfig.Controls[0] as ctlSL00ConfigElement;
                                    _element = ctl.TSConfigElement;
                                    break;
                                }
                            case TRADETYPE.OVER05:
                            case TRADETYPE.OVER15:
                            case TRADETYPE.OVER25:
                            case TRADETYPE.OVER35:
                            case TRADETYPE.OVER45:
                            case TRADETYPE.OVER55:
                            case TRADETYPE.OVER65:
                            case TRADETYPE.OVER75:
                            case TRADETYPE.OVER85:
                                {
                                    ctlASOUConfigElement ctl = pnlASElementConfig.Controls[0] as ctlASOUConfigElement;
                                    _element = ctl.TSConfigElement;
                                    break;
                                }
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
                                {
                                    ctlASCSConfigElementBack ctl = pnlASElementConfig.Controls[0] as ctlASCSConfigElementBack;
                                    _element = ctl.TSConfigElement;
                                    break;
                                }
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
                                {
                                    ctlASCSConfigElementLay ctl = pnlASElementConfig.Controls[0] as ctlASCSConfigElementLay;
                                    _element = ctl.TSConfigElement;
                                    break;
                                }
                        }
                        _element.TradeType = (TRADETYPE)Enum.Parse(typeof(TRADETYPE), cbxTradeType.SelectedValue.ToString());
                    }

                    EventHandler<TTRASElementSaveEventArgs> saveHandler = SaveASConfigElement;
                    if (saveHandler != null)
                    {
                        saveHandler(this, new TTRASElementSaveEventArgs(this.TSConfigElement));
                    }

                }              
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }       
    }

    public class TTRASElementSaveEventArgs : EventArgs
    {
        TradeStarterConfigElement _element;
        public TradeStarterConfigElement ConfigElement
        {
            get
            {
                return _element;
            }
        }

        public TTRASElementSaveEventArgs(TradeStarterConfigElement element)
        {
            _element = element;
        }
    }
}
