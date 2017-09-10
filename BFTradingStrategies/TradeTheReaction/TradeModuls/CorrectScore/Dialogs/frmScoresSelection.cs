using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Dialogs
{
    public partial class frmScoresSelection : Form
    {
        public TTRScores[] Scores
        {
            get
            {
                ScoreList list = new ScoreList();
                if (chk00.Checked)
                    list.Add(TTRScores.ZEROZERO);
                if (chk01.Checked)
                    list.Add(TTRScores.ZEROONE);
                if (chk02.Checked)
                    list.Add(TTRScores.ZEROTWO);
                if (chk03.Checked)
                    list.Add(TTRScores.ZEROTHREE);
                if (chk10.Checked)
                    list.Add(TTRScores.ONEZERO);
                if (chk11.Checked)
                    list.Add(TTRScores.ONEONE);
                if (chk12.Checked)
                    list.Add(TTRScores.ONETWO);
                if (chk13.Checked)
                    list.Add(TTRScores.ONETHREE);
                if (chk20.Checked)
                    list.Add(TTRScores.TWOZERO);
                if (chk21.Checked)
                    list.Add(TTRScores.TWOONE);
                if (chk22.Checked)
                    list.Add(TTRScores.TWOTWO);
                if (chk23.Checked)
                    list.Add(TTRScores.TWOTHREE);
                if (chk30.Checked)
                    list.Add(TTRScores.THREEZERO);
                if (chk31.Checked)
                    list.Add(TTRScores.THREEONE);
                if (chk32.Checked)
                    list.Add(TTRScores.THREETWO);
                if (chk33.Checked)
                    list.Add(TTRScores.THREETHREE);
                if (chkOthers.Checked)
                    list.Add(TTRScores.OTHERS);

                return list.ToArray();
            }
            set
            {
                foreach (TTRScores score in value)
                {
                    switch (score)
                    {
                        case TTRScores.ZEROZERO:
                            chk00.Checked = true;
                            break;
                        case TTRScores.ZEROONE:
                            chk01.Checked = true;
                            break;
                        case TTRScores.ZEROTWO:
                            chk02.Checked = true;
                            break;
                        case TTRScores.ZEROTHREE:
                            chk03.Checked = true;
                            break;
                        case TTRScores.ONEZERO:
                            chk10.Checked = true;
                            break;
                        case TTRScores.ONEONE:
                            chk11.Checked = true;
                            break;
                        case TTRScores.ONETWO:
                            chk12.Checked = true;
                            break;
                        case TTRScores.ONETHREE:
                            chk13.Checked = true;
                            break;
                        case TTRScores.TWOZERO:
                            chk20.Checked = true;
                            break;
                        case TTRScores.TWOONE:
                            chk21.Checked = true;
                            break;
                        case TTRScores.TWOTWO:
                            chk22.Checked = true;
                            break;
                        case TTRScores.TWOTHREE:
                            chk23.Checked = true;
                            break;
                        case TTRScores.THREEZERO:
                            chk30.Checked = true;
                            break;
                        case TTRScores.THREEONE:
                            chk31.Checked = true;
                            break;
                        case TTRScores.THREETWO:
                            chk32.Checked = true;
                            break;
                        case TTRScores.THREETHREE:
                            chk33.Checked = true;
                            break;
                        case TTRScores.OTHERS:
                            chkOthers.Checked = true;
                            break;
                    }
                }
            }
        }

        public frmScoresSelection()
        {
            InitializeComponent();
        }       
    }
}
