using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.SXFastBet
{
    /// <summary>
    /// Einstellungen im Rahmen eines Fast Bets
    /// </summary>
    public class SXFastBetSettings
    {
        /// <summary>
        /// Schalter, ob jedesmal fester Betrag genommen wird, oder relativer Betrag abhängig vom Kontostand
        /// TRUE: Fester Betrag
        /// FALSE: Relativer Betrag
        /// </summary>
        public Boolean FixedAmountFlag { get; set; }

        /// <summary>
        /// Wert des fest zu setzenden Betrages
        /// </summary>
        public Double FixedAmountValue { get; set; }

        /// <summary>
        /// Prozentualer Werte des relativen Betrages 
        /// </summary>
        public int PercentAmounValue { get; set; }

        /// <summary>
        /// Schalter, welcher angibt auf was sich der prozentualler Wert bezieht
        /// TRUE: Gesamtkontostand
        /// FALSE: Verfügbarer Kontostand
        /// </summary>
        public Boolean TotalAmountFlag { get; set; }

        /// <summary>
        /// Schalter, welcher angibt wie mit nicht erfüllten Wetten zu verfahren ist
        /// TRUE: Stornieren
        /// FALSE: Gegebene Zeit warten und neu überprüfen
        /// </summary>
        public Boolean CancelUnmatchedFlag { get; set; }

        /// <summary>
        /// Falls CancelUnmatchedFlag = FALSE die Anzahl der Sekunden die gewartet wird
        /// vor neuer Überprüfung
        /// </summary>
        public int UnmatchedWaitSeconds { get; set; }



    }
}
