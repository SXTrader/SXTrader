using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase.GUI
{
    public partial class ctlStatisticRangeColorPicker : UserControl
    {
        //private StatisticSelectionList _statistics;
        private StatisticsColorSelectionElement _element;
        public event EventHandler<DeleteColoredRangeEventArgs> DeleteStatColorRangeEvent;

        public StatisticsColorSelectionElement StatColor
        {
            get
            {
                return _element;
            }
            set
            {
                _element = value;
                if (_element != null)
                {
                    //btnStatColor.SelectedColor = _element.StatisticColor;
                    btnStatColor2.Color = _element.StatisticColor;
                    try
                    {
                        spnSortNo.Value = _element.SortNumber;
                    }
                    catch (Exception)
                    {
                        spnSortNo.Value = 1;
                    }
                    //lblNumber.Text = _element.SortNumber.ToString();
                    foreach (StatisticSelectionElement statElem in _element.Statistics)
                    {
                        ctlStatisticRangePicker ctl = new ctlStatisticRangePicker();
                        ctl.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(ctl_DeleteRangeEvent);
                        ctl.StatisticRangeElement = statElem;
                        ctl.Dock = DockStyle.Top;
                        pnlStatisticsRange.Controls.Add(ctl);
                    }
                }
            }
        }

        public ctlStatisticRangeColorPicker()
        {
            InitializeComponent();
            _element = StatisticsColorSelectionElement.createNew();            
        }        

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ctlStatisticRangePicker ctl = new ctlStatisticRangePicker();
                ctl.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(ctl_DeleteRangeEvent);
                _element.Statistics.Add(ctl.StatisticRangeElement);
                ctl.Dock = DockStyle.Top;
                pnlStatisticsRange.Controls.Add(ctl);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void ctl_DeleteRangeEvent(object sender, DeleteRangeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteRangeEventArgs>(ctl_DeleteRangeEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    ctlStatisticRangePicker rangeStatisticPicker = sender as ctlStatisticRangePicker;
                    if (rangeStatisticPicker != null)
                    {
                        _element.Statistics.Remove(rangeStatisticPicker.StatisticRangeElement);
                    }

                    pnlStatisticsRange.Controls.Remove(rangeStatisticPicker);
                    rangeStatisticPicker.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                EventHandler<DeleteColoredRangeEventArgs> handler = DeleteStatColorRangeEvent;
                if (handler != null)
                {
                    handler(this, new DeleteColoredRangeEventArgs(_element));
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        /*
        private void btnStatColor_SelectedColorChanged(object sender, ComponentFactory.Krypton.Toolkit.ColorEventArgs e)
        {
            _element.StatisticColor = e.Color;
        }
        */
        private void spnSortNo_ValueChanged(object sender, EventArgs e)
        {
            _element.SortNumber = (int)spnSortNo.Value;
        }

        private void btnStatColor2_Changed(object sender, EventArgs e)
        {
            _element.StatisticColor = btnStatColor2.Color;
        }
    }

    public class DeleteColoredRangeEventArgs : EventArgs
    {
        private StatisticsColorSelectionElement _range;
        public StatisticsColorSelectionElement Range { get { return _range; } }

        public DeleteColoredRangeEventArgs(StatisticsColorSelectionElement range)
        {
            _range = range;
        }
    }
}
