using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    public class SXALRunnerPrices
    {
        public long SelectionId { get; set; }
        public String SelectionName { get; set; }
        public int AsianLineId { get; set; }

        public double TotalAmountMatched { get; set; }

        public SXALPrice[] BestPricesToBack { get; set; }
        public SXALPrice[] BestPricesToLay { get; set; }

        public void removeBackPrice(int index)
        {
            ArrayList alPrices = new ArrayList();
            
            if(BestPricesToBack != null)
            {
                for(int i = 0; i < BestPricesToBack.Length;i++)
                {
                    if(i==index)
                        continue;
                    alPrices.Add(BestPricesToBack[i]);
                }
            }

            BestPricesToBack = (SXALPrice[])alPrices.ToArray(typeof(SXALPrice));
        }

        public void removeLayPrice(int index)
        {
            ArrayList alPrices = new ArrayList();

            if (BestPricesToLay != null)
            {
                for (int i = 0; i < BestPricesToLay.Length; i++)
                {
                    if (i == index)
                        continue;
                    alPrices.Add(BestPricesToLay[i]);
                }
            }

            BestPricesToLay = (SXALPrice[])alPrices.ToArray(typeof(SXALPrice));
        }
    }
}
