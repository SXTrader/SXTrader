using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.common.Configurations;
using net.sxtrader.bftradingstrategies.sxhelper;


namespace net.sxtrader.bftradingstrategies.common
{
    public partial class ctlConfiguration : UserControl, IConfiguration
    {
        private SAConfigurationRW m_config;
        private RangeColorList _rangeColorList;
        private RangeColorList _rangeWLDWin;
        private RangeColorList _rangeWLDLoss;
        private RangeColorList _rangeWLDDraw;
        private RangeColorList _rangeOUOver;
        private RangeColorList _rangeOUUnder;
        private net.sxtrader.bftradingstrategies.sxstatisticbase.StatisticsColorSelectionSortedList _rangeStatColor;
        public ctlConfiguration()
        {
            try
            {
                InitializeComponent();
                m_config = new SAConfigurationRW();

                spnHistoricData.Value = m_config.NoOfData;
                spnAgeOfData.Value = m_config.AgeOfData;

                _rangeColorList = m_config.RangeColors;

                foreach (RangeColorElement element in _rangeColorList)
                {
                    ctlRangeColorPicker rangeColorPicker = new ctlRangeColorPicker();
                    rangeColorPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeColorPicker_DeleteRangeEvent);
                    rangeColorPicker.ColorRangeElement = element;
                    rangeColorPicker.Dock = DockStyle.Top;
                    pnlColorRanges.Controls.Add(rangeColorPicker);
                }


                // Win/Loss/Draw
                cbtWLDTrendWin.Color = m_config.WLDTrendColorWin;                
                cbtWLDTrendLoss.Color = m_config.WLDTrendColorLoss;
                cbtWLDTrendDraw.Color = m_config.WLDTrendColorDraw;
                cbtWLDTrendZero.Color = m_config.WLDTrendColorZero;

                _rangeWLDWin = m_config.WLDWinRangeColors;
                foreach (RangeColorElement element in _rangeWLDWin)
                {
                    ctlRangeColorPicker rangeWLDWinPicker = new ctlRangeColorPicker();
                    rangeWLDWinPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeWLDWinPicker_DeleteRangeEvent);
                    rangeWLDWinPicker.ColorRangeElement = element;
                    rangeWLDWinPicker.Dock = DockStyle.Top;
                    pnlWLDWinRanges.Controls.Add(rangeWLDWinPicker);
                }

                _rangeWLDLoss = m_config.WLDLossRangeColors;
                foreach (RangeColorElement element in _rangeWLDLoss)
                {
                    ctlRangeColorPicker rangeWLDLossPicker = new ctlRangeColorPicker();
                    rangeWLDLossPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeWLDLossPicker_DeleteRangeEvent);
                    rangeWLDLossPicker.ColorRangeElement = element;
                    rangeWLDLossPicker.Dock = DockStyle.Top;
                    pnlWLDLossRanges.Controls.Add(rangeWLDLossPicker);
                }

                _rangeWLDDraw = m_config.WLDDrawRangeColors;
                foreach (RangeColorElement element in _rangeWLDDraw)
                {
                    ctlRangeColorPicker rangeWLDDrawPicker = new ctlRangeColorPicker();
                    rangeWLDDrawPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeWLDDrawPicker_DeleteRangeEvent);
                    rangeWLDDrawPicker.ColorRangeElement = element;
                    rangeWLDDrawPicker.Dock = DockStyle.Top;
                    pnlWLDDrawRanges.Controls.Add(rangeWLDDrawPicker);
                }

                //Over/Under
                cbtOUTrendOver.Color = m_config.OUTrendColorOver;
                cbtOUTrendUnder.Color = m_config.OUTrendColorUnder;

                _rangeOUOver = m_config.OverRangeColors;
                foreach (RangeColorElement element in _rangeOUOver)
                {
                    ctlRangeColorPicker rangeOUOverPicker = new ctlRangeColorPicker();
                    rangeOUOverPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeOUOverPicker_DeleteRangeEvent);
                    rangeOUOverPicker.ColorRangeElement = element;
                    rangeOUOverPicker.Dock = DockStyle.Top;
                    pnlOUOverRanges.Controls.Add(rangeOUOverPicker);
                }

                _rangeOUUnder = m_config.UnderRangeColors;
                foreach (RangeColorElement element in _rangeOUUnder)
                {
                    ctlRangeColorPicker rangeOUUnderPicker = new ctlRangeColorPicker();
                    rangeOUUnderPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeOUUnderPicker_DeleteRangeEvent);
                    rangeOUUnderPicker.ColorRangeElement = element;
                    rangeOUUnderPicker.Dock = DockStyle.Top;
                    pnlOUUnderRanges.Controls.Add(rangeOUUnderPicker);
                }

                _rangeStatColor = m_config.GameSelectionColors;
                foreach (net.sxtrader.bftradingstrategies.sxstatisticbase.StatisticsColorSelectionElement element in _rangeStatColor.Values)
                {
                    net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.ctlStatisticRangeColorPicker ctl = new net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.ctlStatisticRangeColorPicker();
                    ctl.DeleteStatColorRangeEvent += new EventHandler<net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteColoredRangeEventArgs>(ctl_DeleteStatColorRangeEvent);
                    ctl.Dock = DockStyle.Top;
                    ctl.StatColor = element;

                    pnlStatColors.Controls.Add(ctl);
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void rangeOUUnderPicker_DeleteRangeEvent(object sender, DeleteRangeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result =  this.BeginInvoke(new EventHandler<DeleteRangeEventArgs>(rangeOUUnderPicker_DeleteRangeEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    ctlRangeColorPicker rangeColorPicker = sender as ctlRangeColorPicker;
                    if (rangeColorPicker != null)
                    {
                        //In der Liste suchen und entfernen
                        foreach (RangeColorElement element in _rangeOUUnder)
                        {
                            if (element.RangeNumber == rangeColorPicker.ColorRangeElement.RangeNumber)
                            {
                                _rangeOUUnder.Remove(element);                                
                                break;
                            }
                        }
                    }

                    pnlWLDWinRanges.Controls.Remove(rangeColorPicker);
                    rangeColorPicker.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void rangeOUOverPicker_DeleteRangeEvent(object sender, DeleteRangeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<DeleteRangeEventArgs>(rangeOUOverPicker_DeleteRangeEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    ctlRangeColorPicker rangeColorPicker = sender as ctlRangeColorPicker;
                    if (rangeColorPicker != null)
                    {
                        //In der Liste suchen und entfernen
                        foreach (RangeColorElement element in _rangeOUOver)
                        {
                            if (element.RangeNumber == rangeColorPicker.ColorRangeElement.RangeNumber)
                            {
                                _rangeOUOver.Remove(element);
                                break;
                            }
                        }
                    }

                    pnlWLDWinRanges.Controls.Remove(rangeColorPicker);
                    rangeColorPicker.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void rangeWLDWinPicker_DeleteRangeEvent(object sender, DeleteRangeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<DeleteRangeEventArgs>(rangeWLDWinPicker_DeleteRangeEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    ctlRangeColorPicker rangeColorPicker = sender as ctlRangeColorPicker;
                    if (rangeColorPicker != null)
                    {
                        //In der Liste suchen und entfernen
                        foreach (RangeColorElement element in _rangeWLDWin)
                        {
                            if (element.RangeNumber == rangeColorPicker.ColorRangeElement.RangeNumber)
                            {
                                _rangeWLDWin.Remove(element);
                                break;
                            }
                        }
                    }

                    pnlWLDWinRanges.Controls.Remove(rangeColorPicker);
                    rangeColorPicker.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void rangeWLDLossPicker_DeleteRangeEvent(object sender, DeleteRangeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<DeleteRangeEventArgs>(rangeWLDLossPicker_DeleteRangeEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    ctlRangeColorPicker rangeColorPicker = sender as ctlRangeColorPicker;
                    if (rangeColorPicker != null)
                    {
                        //In der Liste suchen und entfernen
                        foreach (RangeColorElement element in _rangeWLDLoss)
                        {
                            if (element.RangeNumber == rangeColorPicker.ColorRangeElement.RangeNumber)
                            {
                                _rangeWLDLoss.Remove(element);
                                break;
                            }
                        }
                    }

                    pnlWLDLossRanges.Controls.Remove(rangeColorPicker);
                    rangeColorPicker.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void rangeWLDDrawPicker_DeleteRangeEvent(object sender, DeleteRangeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<DeleteRangeEventArgs>(rangeWLDDrawPicker_DeleteRangeEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    ctlRangeColorPicker rangeColorPicker = sender as ctlRangeColorPicker;
                    if (rangeColorPicker != null)
                    {
                        //In der Liste suchen und entfernen
                        foreach (RangeColorElement element in _rangeWLDWin)
                        {
                            if (element.RangeNumber == rangeColorPicker.ColorRangeElement.RangeNumber)
                            {
                                _rangeWLDDraw.Remove(element);
                                break;
                            }
                        }
                    }

                    pnlWLDDrawRanges.Controls.Remove(rangeColorPicker);
                    rangeColorPicker.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void rangeColorPicker_DeleteRangeEvent(object sender, DeleteRangeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<DeleteRangeEventArgs>(rangeColorPicker_DeleteRangeEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    ctlRangeColorPicker rangeColorPicker = sender as ctlRangeColorPicker;
                    if (rangeColorPicker != null)
                    {
                        //In der Liste suchen und entfernen
                        foreach (RangeColorElement element in _rangeColorList)
                        {
                            if (element.RangeNumber == rangeColorPicker.ColorRangeElement.RangeNumber)
                            {
                                _rangeColorList.Remove(element);
                                break;
                            }
                        }
                    }

                    pnlColorRanges.Controls.Remove(rangeColorPicker);
                    rangeColorPicker.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            //throw new NotImplementedException();
        }       

        #region IConfiguration Member

        public void save()
        {
            m_config.NoOfData = (int)spnHistoricData.Value;
            m_config.AgeOfData = (int)spnAgeOfData.Value;

            m_config.OUTrendColorOver = cbtOUTrendOver.Color;
            m_config.OUTrendColorUnder = cbtOUTrendUnder.Color;
            m_config.OverRangeColors = _rangeOUOver;
            m_config.UnderRangeColors = _rangeOUUnder;

            m_config.WLDTrendColorWin = cbtWLDTrendWin.Color;
            m_config.WLDTrendColorLoss = cbtWLDTrendLoss.Color;
            m_config.WLDTrendColorDraw = cbtWLDTrendDraw.Color;
            m_config.WLDTrendColorZero = cbtWLDTrendZero.Color;

            m_config.RangeColors = _rangeColorList;
            m_config.WLDWinRangeColors = _rangeWLDWin;
            m_config.WLDLossRangeColors = _rangeWLDLoss;
            m_config.WLDDrawRangeColors = _rangeWLDDraw;

            m_config.GameSelectionColors = _rangeStatColor;

            m_config.save();
        }

        #endregion

        private void btnNewRange_Click(object sender, EventArgs e)
        {
            try
            {
                ctlRangeColorPicker rangeColorPicker = new ctlRangeColorPicker();
                rangeColorPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeColorPicker_DeleteRangeEvent);
                _rangeColorList.Add(rangeColorPicker.ColorRangeElement);
                rangeColorPicker.Dock = DockStyle.Top;
                pnlColorRanges.Controls.Add(rangeColorPicker);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnWLDWinRangeNew_Click(object sender, EventArgs e)
        {
            try
            {
                ctlRangeColorPicker rangeColorPicker = new ctlRangeColorPicker();
                rangeColorPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeWLDWinPicker_DeleteRangeEvent);
                _rangeWLDWin.Add(rangeColorPicker.ColorRangeElement);
                rangeColorPicker.Dock = DockStyle.Top;
                pnlWLDWinRanges.Controls.Add(rangeColorPicker);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnWLDLossRangeNew_Click(object sender, EventArgs e)
        {
            try
            {
                ctlRangeColorPicker rangeColorPicker = new ctlRangeColorPicker();
                rangeColorPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeWLDLossPicker_DeleteRangeEvent);
                _rangeWLDLoss.Add(rangeColorPicker.ColorRangeElement);
                rangeColorPicker.Dock = DockStyle.Top;
                pnlWLDLossRanges.Controls.Add(rangeColorPicker);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnWLDDrawRangeNew_Click(object sender, EventArgs e)
        {
            try
            {
                ctlRangeColorPicker rangeColorPicker = new ctlRangeColorPicker();
                rangeColorPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeWLDDrawPicker_DeleteRangeEvent);
                _rangeWLDDraw.Add(rangeColorPicker.ColorRangeElement);
                rangeColorPicker.Dock = DockStyle.Top;
                pnlWLDDrawRanges.Controls.Add(rangeColorPicker);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void btnOUOverRangeNew_Click(object sender, EventArgs e)
        {
            try
            {
                ctlRangeColorPicker rangeColorPicker = new ctlRangeColorPicker();
                rangeColorPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeOUOverPicker_DeleteRangeEvent);
                _rangeOUOver.Add(rangeColorPicker.ColorRangeElement);
                rangeColorPicker.Dock = DockStyle.Top;
                pnlOUOverRanges.Controls.Add(rangeColorPicker);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }


        private void btnOUUnderRangeNew_Click(object sender, EventArgs e)
        {
            try
            {
                ctlRangeColorPicker rangeColorPicker = new ctlRangeColorPicker();
                rangeColorPicker.DeleteRangeEvent += new EventHandler<DeleteRangeEventArgs>(rangeOUUnderPicker_DeleteRangeEvent);
                _rangeOUUnder.Add(rangeColorPicker.ColorRangeElement);
                rangeColorPicker.Dock = DockStyle.Top;
                pnlOUUnderRanges.Controls.Add(rangeColorPicker);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }



        #region IConfiguration Member


        public string getXML()
        {
            return String.Empty;
        }

        #endregion

        private void btnNewColorStat_Click(object sender, EventArgs e)
        {
            try
            {
                net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.ctlStatisticRangeColorPicker ctl = new net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.ctlStatisticRangeColorPicker();
                ctl.DeleteStatColorRangeEvent += new EventHandler<net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteColoredRangeEventArgs>(ctl_DeleteStatColorRangeEvent);
                try
                {
                    _rangeStatColor.Add(ctl.StatColor.SortNumber, ctl.StatColor);
                }
                catch (ArgumentException)
                {
                    ctl.StatColor.SortNumber = _rangeStatColor.Keys.Last() + 1;
                    _rangeStatColor.Add(ctl.StatColor.SortNumber, ctl.StatColor);
                }
                ctl.Dock = DockStyle.Top;

                pnlStatColors.Controls.Add(ctl);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void ctl_DeleteStatColorRangeEvent(object sender, net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteColoredRangeEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    IAsyncResult result = this.BeginInvoke(new EventHandler<net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.DeleteColoredRangeEventArgs>(ctl_DeleteStatColorRangeEvent), new object[] { sender, e });
                    this.EndInvoke(result);
                }
                else
                {
                    net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.ctlStatisticRangeColorPicker rangeStatisticPicker = sender as net.sxtrader.bftradingstrategies.sxstatisticbase.GUI.ctlStatisticRangeColorPicker;
                    if (rangeStatisticPicker != null)
                    {

                        _rangeStatColor.Remove(rangeStatisticPicker.StatColor.SortNumber);
                    }

                    pnlStatColors.Controls.Remove(rangeStatisticPicker);
                    rangeStatisticPicker.Dispose();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }
    }
}
