using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.sxtrader.bftradingstrategies.bfuestrategy.MailInterface;

namespace SXTraderUnitTests
{
    [TestClass]
    public class BetgreenTests
    {
        [TestMethod, Description("Leere Email Nachricht")]
        public void emptyEmailMessage()
        {
            LTDMail mail = new LTDMail();
            mail.getBetgreenLTDTipps("mail.sxtrader.net", 143, false, "donation@sxtrader.net", "pxZwF3g!hrq)");
            Assert.AreEqual(1, 0);
        }
        [TestMethod, Description("Email ohne Tipp-Identifizierer")]
        public void noTippStartTagEmailMessage()
        {
        }

        [TestMethod, Description("Email mit Nachricht \"No LTD Trades\"")]
        public void noTippTodayEmailMessage()
        {
        }

        [TestMethod, Description("Email mit 1 Tipp")]
        public void oneTippEmailMessage()
        {
        }

        [TestMethod, Description("Email mit 4 Tipps")]
        public void fourTippEmailMessage()
        {
        }

        [TestMethod, Description("Email mit 4 Tipps, wobei einer kein LTD")]
        public void fourTippWithOneNoLTDEmailMessage()
        {
        }
    }
}
