using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;

namespace net.sxtrader.bftradingstrategies.ttr.GUI
{
    public partial class ctlASConfigElementDisplay : UserControl
    {
        TradeStarterConfigElement _configElement;

        public event EventHandler<TTRASElementEditEventArgs> EditASConfigElement;
        public event EventHandler<TTRASElementDeleteEventArgs> DeleteASConfigElement;

        public ctlASConfigElementDisplay()
        {
            InitializeComponent();            
        }

        public ctlASConfigElementDisplay(TradeStarterConfigElement element)
        {
            InitializeComponent();
            this.ASConfigElement = element;
        }

        public TradeStarterConfigElement ASConfigElement
        {
            get
            {
                return _configElement;
            }
            set
            {
                lblDescription.Text = String.Empty;
                _configElement = value;
                if (_configElement != null)
                {
                    lblDescription.Text = _configElement.ToString();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            EventHandler<TTRASElementDeleteEventArgs> deleteHandler = DeleteASConfigElement;
            if (deleteHandler != null)
            {
                deleteHandler(this, new TTRASElementDeleteEventArgs(this.ASConfigElement));
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EventHandler<TTRASElementEditEventArgs> editHandler = EditASConfigElement;
            if (editHandler != null)
                editHandler(this, new TTRASElementEditEventArgs(this.ASConfigElement));
        }

        private void lblDescription_SizeChanged(object sender, EventArgs e)
        {
            pnlDescription.Height = lblDescription.Height + 2;
        }

        private void pnlDescription_SizeChanged(object sender, EventArgs e)
        {
            this.Height = pnlDescription.Height + pnlButtons.Height + 2; 
        }
    }

    public class TTRASElementDeleteEventArgs : EventArgs
    {
        TradeStarterConfigElement _element;
        public TradeStarterConfigElement ConfigElement
        {
            get
            {
                return _element;
            }
        }

        public TTRASElementDeleteEventArgs(TradeStarterConfigElement element)
        {
            _element = element;
        }
    }

    public class TTRASElementEditEventArgs : EventArgs
    {
        TradeStarterConfigElement _element;
        public TradeStarterConfigElement ConfigElement
        {
            get
            {
                return _element;
            }
        }

        public TTRASElementEditEventArgs(TradeStarterConfigElement element)
        {
            _element = element;
        }
    }
}
