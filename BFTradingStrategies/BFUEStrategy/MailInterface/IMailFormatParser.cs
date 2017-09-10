using net.sxtrader.bftradingstrategies.bfuestrategy.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.MailInterface
{
    interface IMailFormatParser
    {
        /// <summary>
        /// Überprüft, ob die gefundene Tippmail eventuelle Vorraussetzungen erfüllt
        /// z.B. Datum, Art der Tipps.
        /// </summary>
        /// <param name="mail">Die Email - als Array von Zeilen</param>
        /// <returns>true: Voraussetzungen sind erfüllt
        ///          false: Voraussetzungen sind nicht erfüllt</returns>
        bool checkPrerequisite(String[] mail);

        /// <summary>
        /// Sucht in der Mail den Anfang der Tipps und entfernt die unnötigen vorausgehenden Zeilen
        /// </summary>
        /// <param name="mail">Die Email - als Array von Zeilen</param>
        /// /// <returns>true: Es wurde eine Tippsektion gefundne
        ///          false: Es wurde keine Tippsektion gefunden</returns>
        bool jumpToTipps(ref List<String> mail);

        /// <summary>
        /// Extrahiert aus der Tipp-Mail die einzelnen täglichen Tipps
        /// </summary>
        /// <param name="mail">Die Email - als Array von Zeilen</param>
        /// <returns>Der extrahierte Tipp</returns>
        BetgreenLTDTipp extractTipps(List<String> mail);

    }
}
