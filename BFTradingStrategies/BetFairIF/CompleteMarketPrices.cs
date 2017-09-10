using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.betfairif;
using System.Globalization;

namespace net.sxtrader.bftradingstrategies.betfairif
{
    public class CompleteMarketPrices
    {
        private const int SELECTIONAMOUNTINDEX = 2;
        private const int SELECTIONIDINDEX = 0;
        private const int ORDERIDINDEX = 1;
        private const int ASIANIDINDEX = 7;

        private int m_marketId;
        private double m_marketSum;
        private SortedList<int, Selektion> m_selektionen;
        

        public CompleteMarketPrices(int marketId)
        {            
            m_marketId = marketId;
            m_selektionen = new SortedList<int, Selektion>();
            //Marktvolumen lesen
            String rawMarketData = BetfairKom.Instance.getCompleteMarketPrices(marketId);
            char[] sepsMarkets = { ':' };
            char[] sepSelection = {'~'};
            // Auftrennen in Markt- und Selectiondaten
            if(rawMarketData != null)
            {
                String[] stage1 = rawMarketData.Split(sepsMarkets);
                for (int i = 1; i < stage1.Length; i++)
                {
                    // Auftrennen in Selektions- und Quotendaten
                    char[] sepStag1 = { '|' };
                    String[] stage2 = stage1[i].Split(sepStag1);
                    if (stage2.Length < 2)
                        continue;
                    // Selektionsdaten vereinzeln
                    String[] stage3 = stage2[0].Split(sepSelection);

                    // Fall Selektionsdaten <= 3, dann wurde Selektion entfernt => weiter
                    if (stage3.Length <= 3)
                        continue;

                    // Selektion speichern 
                    
                    Selektion selektion = new Selektion(int.Parse(stage3[SELECTIONIDINDEX]), int.Parse(stage3[ORDERIDINDEX], CultureInfo.InvariantCulture), int.Parse(stage3[ASIANIDINDEX]));
                    selektion.AmountMatched = Double.Parse(stage3[SELECTIONAMOUNTINDEX], CultureInfo.InvariantCulture);
                    selektion.addSelectionOdds(stage2[1]);
                    m_selektionen.Add(selektion.SelectionId, selektion);
                    // Marktsumme addieren
                    m_marketSum += Double.Parse(stage3[SELECTIONAMOUNTINDEX], CultureInfo.InvariantCulture);                    
                }
            }
        }

        public double MarketAmountMatched
        {
            get
            {
                return m_marketSum;
            }
        }

        public Double getAmountOfSelection(int selectionId)
        {
            if (m_selektionen.Count < selectionId)
                return 0.0;
            return m_selektionen[selectionId].AmountMatched;
        }

        public Double getAmountOfSelectionByOrderId(int orderId)
        {
            foreach (Selektion selektion in m_selektionen.Values)
            {
                if (selektion.OrderId == orderId)
                {
                    return selektion.AmountMatched;
                }
            }
            return 0.0;
        }

        public int getAsianIdOfSelection(int selectionId)
        {
            if (m_selektionen.Count == 0)
                return 0;
            return m_selektionen[selectionId].AsianId;
        }

        public Double getBestLayOfSelection(int selectionId)
        {
            if (m_selektionen.Count == 0)
                return 0.0;
            return m_selektionen[selectionId].getBestLay();
        }

        public Double getBestBackOfSelectionByOrderId(int orderId)
        {
            foreach (Selektion selektion in m_selektionen.Values)
            {
                if (selektion.OrderId == orderId)
                {
                    return selektion.getBestBack();
                }
            }
            return 0.0;
        }

        public Double getBestLayOfSelectionByOrderId(int orderId)
        {
            foreach (Selektion selektion in m_selektionen.Values)
            {
                if (selektion.OrderId == orderId)
                {
                    return selektion.getBestLay();
                }
            }
            return 0.0;
        }

        public int getSelectionIdByOrderId(int orderId)
        {
            foreach (Selektion selektion in m_selektionen.Values)
            {
                if (selektion.OrderId == orderId)
                {
                    return selektion.SelectionId;
                }
            }
            return 0;
        }
    }

    class Selektion
    {
        private int m_selectionId;
        private int m_asianId;
        private int m_orderId;
        private double m_amountMatched;
        private SortedList<double, SelektionQuote> m_quoten;

        public Selektion(int selectionId, int orderId, int asianId)
        {
            m_selectionId = selectionId;
            m_orderId = orderId;
            m_asianId = asianId;
            m_quoten = new SortedList<double, SelektionQuote>();
        }

        public int AsianId
        {
            get
            {
                return m_asianId;
            }
        }

        public int SelectionId
        {
            get
            {
                return m_selectionId;
            }
        }

        public int OrderId
        {
            get
            {
                return m_orderId;
            }
        }

        public double AmountMatched
        {
            get
            {
                return m_amountMatched;
            }
            set
            {
                m_amountMatched = value;
            }
        }

        public void addSelectionOdds(string rawData)
        {
            char[] sepOdds = { '~' };
            
            String[] odds = rawData.Split(sepOdds);
            for (int i = 0; i < odds.Length; i += 5)
            {
                if (odds[i].Length == 0)
                    break;
                
                SelektionQuote quote = new SelektionQuote(Double.Parse(odds[i], CultureInfo.InvariantCulture), Double.Parse(odds[i + 1], CultureInfo.InvariantCulture), double.Parse(odds[i + 2], CultureInfo.InvariantCulture));
                m_quoten.Add(quote.Quote, quote);
            }
        }

        public double getBestLay()
        {
            foreach (SelektionQuote quote in m_quoten.Values)
            {
                if (quote.BackAmount == 0.0 && quote.LayAmount != 0.0)
                    return quote.Quote;
            }
            return 0.0;
        }

        public double getBestBack()
        {
            double tmpQuote = 0.0;
            foreach (SelektionQuote quote in m_quoten.Values)
            {
                if (quote.LayAmount == 0.0 && quote.BackAmount != 0.0)
                {
                    if (quote.Quote > tmpQuote)
                        tmpQuote = quote.Quote;
                }
            }
            return tmpQuote;
        }
    }

    class SelektionQuote
    {
        private double m_quote;
        private double m_backAmount;
        private double m_layAmount;

        public SelektionQuote(double quote, double backAmount, double layAmount)
        {
            m_quote = quote;
            m_backAmount = backAmount;
            m_layAmount = layAmount;
        }

        public double Quote
        {
            get
            {
                return m_quote;
            }
        }

        public double BackAmount
        {
            get
            {
                return m_backAmount;
            }
        }

        public double LayAmount
        {
            get
            {
                return m_layAmount;
            }
        }
    }
}
