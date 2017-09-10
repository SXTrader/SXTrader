using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    public static class LivetickerHelper
    {
        public static void doManualLivetickerConnection(String match)
        {
            frmManualAdd dlg = new frmManualAdd();
            dlg.Match = match;
            DialogResult result = DialogResult.None;
            //Fall 1: Liveticker 1 ist nicht verbunden

            if (!HLList.Instance[match].IsScore1Connected())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    HLList.Instance[match].Score1 = dlg.Livescore;
                    // Lokale XML-Datei schreiben
                    if (dlg.Livescore.TeamA != "Team None" && dlg.Livescore.TeamB != "Team None")
                        LiveScoreParser.WriteLocalXml(match, dlg.Livescore);
                }
            }

            if (!HLList.Instance[match].IsScore2Connected())
            {
                dlg.IsLiveScore2 = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    HLList.Instance[match].Score2 = dlg.Livescore;

                    // Lokale XML-Datei schreiben
                    if (dlg.Livescore.TeamA != "Team None" && dlg.Livescore.TeamB != "Team None")
                        LiveScore2Parser.writeLocalXml(match, dlg.Livescore);
                }
            }
        }
    }
}
