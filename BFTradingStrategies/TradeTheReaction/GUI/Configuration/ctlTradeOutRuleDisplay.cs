using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.Configuration;

namespace net.sxtrader.bftradingstrategies.ttr.GUI.Configuration
{
    internal partial class ctlTradeOutRuleDisplay : UserControl
    {
        private TTRTradeOutCheck _tradeOutElement;

        public event EventHandler<TTRTOElementEditEventArgs> EditTOConfigElement;
        public event EventHandler<TTRTOElementDeleteEventArgs> DeleteTOConfigElement;

        public ctlTradeOutRuleDisplay()
        {
            InitializeComponent();            
        }

        public ctlTradeOutRuleDisplay(TTRTradeOutCheck tradeOutElement)
        {
            InitializeComponent();            
            if (tradeOutElement != null)
            {
                this.TradeOutElement = tradeOutElement;
            }

        }

        public TTRTradeOutCheck TradeOutElement
        {
            get
            {
                return _tradeOutElement;
            }
            set
            {
                lblDescription.Text = String.Empty;
                _tradeOutElement = value;
                if (_tradeOutElement != null)
                {
                    lblDescription.Text = _tradeOutElement.ToString();
                }
            }
        }
        

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EventHandler<TTRTOElementEditEventArgs> editHandler = EditTOConfigElement;
            if (editHandler != null)
                editHandler(this, new TTRTOElementEditEventArgs(this.TradeOutElement));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            EventHandler<TTRTOElementDeleteEventArgs> deleteHandler = DeleteTOConfigElement;
            if (deleteHandler != null)
            {
                deleteHandler(this, new TTRTOElementDeleteEventArgs(this.TradeOutElement));
            }
        }

        private void pnlDescription_SizeChanged(object sender, EventArgs e)
        {
            this.Height = pnlDescription.Height + pnlButtons.Height + 2;
        }

        private void lblDescription_SizeChanged(object sender, EventArgs e)
        {
            pnlDescription.Height = lblDescription.Height + 2;
        }
    }

    internal class TTRTOElementDeleteEventArgs : EventArgs
    {
        TTRTradeOutCheck _element;
        public TTRTradeOutCheck TradeOutElement
        {
            get
            {
                return _element;
            }
        }

        public TTRTOElementDeleteEventArgs(TTRTradeOutCheck element)
        {
            _element = element;
        }
    }

    internal class TTRTOElementEditEventArgs : EventArgs
    {
        TTRTradeOutCheck _element;
        public TTRTradeOutCheck TradeOutElement
        {
            get
            {
                return _element;
            }
        }

        public TTRTOElementEditEventArgs(TTRTradeOutCheck element)
        {
            _element = element;
        }
    }
}
