using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.SXFastBet
{
    public class SXFastBetInsufficentFoundsExcpetion : Exception { }
    public class SXFastBetBelowMinStackException : Exception { }
    public static class SXFastBetMoneyGetter
    {
        public static double getMoney(SXFastBetSettings settings)
        {
            double dMoney = 0.0;
            // Unterscheiden, ob Fester Betrag oder relativer Betrag
            if (settings.FixedAmountFlag)
            {
                dMoney = Math.Round(settings.FixedAmountValue, 2);
                // Überprüfen, ob Einsatz unter Mindesteinsatz
                /*
                if (dMoney < BankrollManager.Instance.MinStake)
                {
                    throw new SXFastBetBelowMinStackException();
                }
                */

                // Überprüfen, ob Geldbetrag größer als vorhandener Geldbetrag ist
                if (dMoney > SXALBankrollManager.Instance.AvailibleBalance)
                {
                    throw new SXFastBetInsufficentFoundsExcpetion();
                }
            }
            else
            {
                // Unterscheiden, ob Relativer Betrag von Gesamt- oder verfügbaren Kontostand
                if (settings.TotalAmountFlag)
                {
                    dMoney = SXALBankrollManager.Instance.TotalBalance * (settings.PercentAmounValue * 0.01f);
                    dMoney = Math.Round(dMoney, 2);
                }
                else
                {
                    dMoney = SXALBankrollManager.Instance.AvailibleBalance * (settings.PercentAmounValue * 0.01f);
                    dMoney = Math.Round(dMoney, 2);
                }

                // Überprüfen, ob Einsatz unter Mindesteinsatz
                /*
                if (dMoney < BankrollManager.Instance.MinStake)
                {
                    throw new SXFastBetBelowMinStackException();
                }
                */
                // Überprüfen, ob Geldbetrag größer als vorhandener Geldbetrag ist
                if (dMoney > SXALBankrollManager.Instance.AvailibleBalance)
                {
                    throw new SXFastBetInsufficentFoundsExcpetion();
                }
            }

            return dMoney;
        }

    }
}
