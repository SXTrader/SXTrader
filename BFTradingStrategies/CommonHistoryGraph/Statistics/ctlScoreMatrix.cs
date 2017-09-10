using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.common.Configurations;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.sxstatisticbase;

namespace net.sxtrader.bftradingstrategies.common.Statistics
{
    public enum SCOREMATRIXTYPE {UNASSIGNED=0, DIRECT, SINGLE};

    public partial class ctlScoreMatrix : UserControl
    {
        private int _00;
        private int _01;
        private int _02;
        private int _03;
        private int _10;
        private int _11;
        private int _12;
        private int _13;
        private int _20;
        private int _21;
        private int _22;
        private int _23;
        private int _30;
        private int _31;
        private int _32;
        private int _33;
        private int _others;

        public SCOREMATRIXTYPE MatrixType { get; set; }
        public HistoricMatchList MatchList { get; set; }
        public string TeamAName { get; set; }
        public string TeamBName { get; set; }
        public ulong TeamAId { get; set; }
        public ulong TeamBId { get; set; }

        public ctlScoreMatrix()
        {
            InitializeComponent();
            MatrixType = SCOREMATRIXTYPE.UNASSIGNED;
        }

        public ctlScoreMatrix(SCOREMATRIXTYPE matrixType)
        {
            InitializeComponent();
            MatrixType = matrixType;
        }

        public void loadMatrix()
        {
            try
            {
                if (MatchList == null)
                    return;



                double tmpZero = 0.0;
                txt00.Text = txt01.Text = txt02.Text = txt03.Text =
                    txt10.Text = txt11.Text = txt12.Text = txt13.Text =
                    txt20.Text = txt21.Text = txt22.Text = txt23.Text =
                    txt30.Text = txt31.Text = txt32.Text = txt33.Text =
                    txtOther.Text = tmpZero.ToString() + " %";


                txt00.BackColor = txt01.BackColor = txt02.BackColor = txt03.BackColor =
                    txt10.BackColor = txt11.BackColor = txt12.BackColor = txt13.BackColor =
                    txt20.BackColor = txt21.BackColor = txt22.BackColor = txt23.BackColor =
                    txt30.BackColor = txt31.BackColor = txt32.BackColor = txt33.BackColor =
                    txtOther.BackColor = SystemColors.Control;

                if (MatchList.Count == 0)
                    return;

                _00 = _01 = _02 = _03 = _10 = _11 = _12 = _13 = _20 = _21 = _22 = _23 = _30 = _31 = _32 = _33 = _others = 0;

                switch (MatrixType)
                {
                    case SCOREMATRIXTYPE.UNASSIGNED:
                        {
                            return;
                        }
                    case SCOREMATRIXTYPE.DIRECT:
                        {
                            lblHorizontal.Text = TeamBName;
                            lblVertical.Text = TeamAName;
                            break;
                        }
                    case SCOREMATRIXTYPE.SINGLE:
                        {
                            lblHorizontal.Text = HistoryGraph.strOpponents;
                            lblVertical.Text = TeamAName;
                            break;
                        }
                }

                foreach (LSHistoricMatch match in MatchList)
                {
                    //Prinzipiell müßte es genügen über Team A Id zu suchen;
                    if (match.TeamAId == TeamAId)
                    {
                        caluclateScore(match.ScoreA, match.ScoreB);
                    }
                    else
                    {
                        caluclateScore(match.ScoreB, match.ScoreA);
                    }
                }

                fillTextValues(txt00, _00);
                fillTextValues(txt01, _01);
                fillTextValues(txt02, _02);
                fillTextValues(txt03, _03);
                fillTextValues(txt10, _10);
                fillTextValues(txt11, _11);
                fillTextValues(txt12, _12);
                fillTextValues(txt13, _13);
                fillTextValues(txt20, _20);
                fillTextValues(txt21, _21);
                fillTextValues(txt22, _22);
                fillTextValues(txt23, _23);
                fillTextValues(txt30, _30);
                fillTextValues(txt31, _31);
                fillTextValues(txt32, _32);
                fillTextValues(txt33, _33);
                fillTextValues(txtOther, _others);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void fillTextValues(TextBox txt, double scoreCounter)
        {
            try
            {
                SAConfigurationRW config = new SAConfigurationRW();
                RangeColorList colorList = config.RangeColors;
                txt.BackColor = SystemColors.Control;

                double percent = scoreCounter / MatchList.Count * 100;
                foreach (RangeColorElement colorElement in colorList)
                {
                    if (percent >= colorElement.Lo && percent <= colorElement.Hi)
                    {
                        txt.BackColor = Color.FromArgb(colorElement.Color);
                        break;
                    }
                }
                txt.Text = String.Format("{0} %", Math.Round(percent, 2));
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void caluclateScore(uint scoreA, uint scoreB)
        {
            try
            {
                switch (scoreA)
                {
                    case 0:
                        {
                            switch (scoreB)
                            {
                                case 0:
                                    {
                                        _00++;
                                        break;
                                    }
                                case 1:
                                    {
                                        _01++;
                                        break;
                                    }
                                case 2:
                                    {
                                        _02++;
                                        break;
                                    }
                                case 3:
                                    {
                                        _03++;
                                        break;
                                    }
                                default:
                                    {
                                        _others++;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 1:
                        {
                            switch (scoreB)
                            {
                                case 0:
                                    {
                                        _10++;
                                        break;
                                    }
                                case 1:
                                    {
                                        _11++;
                                        break;
                                    }
                                case 2:
                                    {
                                        _12++;
                                        break;
                                    }
                                case 3:
                                    {
                                        _13++;
                                        break;
                                    }
                                default:
                                    {
                                        _others++;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 2:
                        {
                            switch (scoreB)
                            {
                                case 0:
                                    {
                                        _20++;
                                        break;
                                    }
                                case 1:
                                    {
                                        _21++;
                                        break;
                                    }
                                case 2:
                                    {
                                        _22++;
                                        break;
                                    }
                                case 3:
                                    {
                                        _23++;
                                        break;
                                    }
                                default:
                                    {
                                        _others++;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 3:
                        {
                            switch (scoreB)
                            {
                                case 0:
                                    {
                                        _30++;
                                        break;
                                    }
                                case 1:
                                    {
                                        _31++;
                                        break;
                                    }
                                case 2:
                                    {
                                        _32++;
                                        break;
                                    }
                                case 3:
                                    {
                                        _33++;
                                        break;
                                    }
                                default:
                                    {
                                        _others++;
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            _others++;
                            break;
                        }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        private void ctlScoreMatrix_Load(object sender, EventArgs e)
        {

        }
    }
}
