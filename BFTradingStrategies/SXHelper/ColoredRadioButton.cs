using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;

namespace net.sxtrader.bftradingstrategies.sxhelper
{
    public class ColoredRadioButton : RadioButton
    {
        private List<Color> _colors = new List<Color>();
        private int _heightWithColors = 0;

        public void addColor(Color color)
        {
            _colors.Add(color);
            _heightWithColors += 10;
            
        }

        public void clearColors()
        {
            _heightWithColors = 0;
            _colors.Clear();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            
            _heightWithColors += this.Height;
            
            this.Height = _heightWithColors;
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            
            base.OnInvalidated(e);            
            int i = 0;
            foreach (Color c in _colors)
            {
                System.Drawing.Pen myPen;
                myPen = new System.Drawing.Pen(c, 5);

                System.Drawing.Graphics formGraphics = this.CreateGraphics();
                formGraphics.DrawLine(myPen, 0, i * 5, this.Width, i * 5);
                myPen.Dispose();
                formGraphics.Dispose();
                i++;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            int i = 0;
            foreach (Color c in _colors)
            {
                System.Drawing.Pen myPen;
                myPen = new System.Drawing.Pen(c, 5);

                System.Drawing.Graphics formGraphics = this.CreateGraphics();
                formGraphics.DrawLine(myPen, 0, i * 5, this.Width, i * 5);
                myPen.Dispose();
                formGraphics.Dispose();
                i++;
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            
            base.OnPaint(pevent);
            int i = 0;
            foreach (Color c in _colors)
            {
                System.Drawing.Pen myPen;
                myPen = new System.Drawing.Pen(c,5);
                
                System.Drawing.Graphics formGraphics = this.CreateGraphics();
                formGraphics.DrawLine(myPen, 0, i * 5, this.Width, i * 5);
                myPen.Dispose();
                formGraphics.Dispose();
                i++;
            }
        }

    }
}
