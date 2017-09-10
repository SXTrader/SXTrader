using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.sxhelper
{
    public partial class ctlRangeColorPicker : UserControl
    {

        private RangeColorElement _colorRangeElement;

        public event EventHandler<DeleteRangeEventArgs> DeleteRangeEvent;

        public RangeColorElement ColorRangeElement
        {
            get
            {
                return _colorRangeElement;
            }
            set
            {
                btnDelete.Enabled = false;
                _colorRangeElement = value;
                if (_colorRangeElement != null)
                {
                    btnDelete.Enabled = true;
                    decimal tmpValue = 000.00m;

                    tmpValue = (decimal)_colorRangeElement.Hi;
                    spnHi.Value = (decimal)tmpValue;//_colorRangeElement.Hi;

                    cbtColor.Color = Color.FromArgb(_colorRangeElement.Color);
                    tmpValue = (decimal)_colorRangeElement.Lo;
                    spnLo.Value = (decimal)tmpValue;// _colorRangeElement.Lo;

                    if (spnHi.Value <= spnLo.Value)
                        spnHi.Value = spnLo.Value + (decimal)0.01;
                }
            }
        }

        public ctlRangeColorPicker()
        {
            InitializeComponent();
            _colorRangeElement = RangeColorElement.createNew();
            btnDelete.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            EventHandler<DeleteRangeEventArgs> handler = DeleteRangeEvent;
            if (handler != null)
            {
                handler(this, new DeleteRangeEventArgs(_colorRangeElement));
            }
        }

        /*
        private void cbtColor_SelectedColorChanged(object sender, ComponentFactory.Krypton.Toolkit.ColorEventArgs e)
        {
            _colorRangeElement.Color = e.Color.ToArgb();
        }
         */

        private void spnLo_ValueChanged(object sender, EventArgs e)
        {
            if (spnLo.Value > spnHi.Value)
                spnLo.Value = spnHi.Value;

            _colorRangeElement.Lo = (double)spnLo.Value;
        }

        private void spnHi_ValueChanged(object sender, EventArgs e)
        {
            if (spnLo.Value > spnHi.Value)
                spnLo.Value = spnHi.Value;

            _colorRangeElement.Hi = (double)spnHi.Value;
        }

        private void cbtColor_Changed(object sender, EventArgs e)
        {
            _colorRangeElement.Color = cbtColor.Color.ToArgb();
        }
    }

    public class DeleteRangeEventArgs : EventArgs
    {
        private  RangeColorElement _range;
        public RangeColorElement Range { get { return _range; } }

        public DeleteRangeEventArgs(RangeColorElement range)
        {
            _range = range;
        }
    }

    public class RangeColorElement
    {
        private static int _Static_maxCounter = 0;
        private int _objNumber;

        private Double _lo;
        private Double _hi;
        private int _color;

        public Double Lo { get { return _lo; } set { _lo = value; } }
        public Double Hi { get { return _hi; } set { _hi = value; } }
        public int Color { get { return _color; } set { _color = value; } }
        
        public int RangeNumber { get { return _objNumber; } }

        public static RangeColorElement createNew()
        {
            return new RangeColorElement();
        }

        private RangeColorElement()
        {
            _objNumber = ++_Static_maxCounter;
            _color = SystemColors.Control.ToArgb();
            _lo = _hi = 0.0;
        }
    }

    public class RangeColorList : List<RangeColorElement> { }
}
