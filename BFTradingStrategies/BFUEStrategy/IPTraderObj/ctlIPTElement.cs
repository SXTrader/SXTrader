using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj
{
    public partial class ctlIPTElement : UserControl
    {
        BFUEFBIPTraderConfigElement _configElement;

        public event EventHandler<BFUEIPTElementEditEventArgs> EditIPTConfigElement;
        public event EventHandler<BFUEIPTElementDeleteEventArgs> DeleteIPTConfigElement;

        public ctlIPTElement()
        {
            InitializeComponent();            
        }

        public ctlIPTElement(BFUEFBIPTraderConfigElement element)
        {
            InitializeComponent();
            this.IPTConfigElement = element;
        }

        public BFUEFBIPTraderConfigElement IPTConfigElement
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EventHandler<BFUEIPTElementEditEventArgs> editHandler = EditIPTConfigElement;
            if (editHandler != null)
                editHandler(this, new BFUEIPTElementEditEventArgs(this.IPTConfigElement));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            EventHandler<BFUEIPTElementDeleteEventArgs> deleteHandler = DeleteIPTConfigElement;
            if (deleteHandler != null)
            {
                deleteHandler(this, new BFUEIPTElementDeleteEventArgs(this.IPTConfigElement));
            }
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
}
