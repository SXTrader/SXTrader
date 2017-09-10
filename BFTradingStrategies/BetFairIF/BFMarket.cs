using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.betfairif
{
    // Abstrahierung der Betfair Märkte
    public class BFMarket
    {
        private string name;
        private string match;
        private string devision;
        private int id;
        private bool inPlay;
        private DateTime startDts;

        public int Id
        {
            get
            {
                return id;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public String Match
        {
            get
            {
                return match;
            }
        }

        public String Devision
        {
            get
            {
                return devision;
            }
        }

        public DateTime StartDTS
        {
            get
            {
                return startDts;
            }
        }

        public bool IsGenericOverUnder
        {
            get
            {
                if (name.StartsWith("Over/Under", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder05
        {
            get
            {
                if (name.Equals("Over/Under 0.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder15
        {
            get
            {
                if (name.Equals("Over/Under 1.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder25
        {
            get
            {
                if (name.Equals("Over/Under 2.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder35
        {
            get
            {
                if (name.Equals("Over/Under 3.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder45
        {
            get
            {
                if (name.Equals("Over/Under 4.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder55
        {
            get
            {
                if (name.Equals("Over/Under 5.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder65
        {
            get
            {
                if (name.Equals("Over/Under 6.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder75
        {
            get
            {
                if (name.Equals("Over/Under 7.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder85
        {
            get
            {
                if (name.Equals("Over/Under 8.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsMatchOdds
        {
            get
            {
                if (name.Equals("Match Odds",StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsUnmanaged
        {
            get
            {
                if (name.EndsWith("Unmanaged", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsTotalGoals
        {
            get
            {
                if (name.Equals("Total Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsScoreMarket
        {
            get
            {
                if (name.Equals("Correct Score", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public Boolean InPlayMarket
        {
            get
            {
                return inPlay;
            }
        }

        public BFMarket(string plainMarketData)
        {
            char[] cSeps = {'~'};
            String[] marketDatas = plainMarketData.Split(cSeps);
            if (marketDatas.Length < 2)
                throw new FormatException("Invalid MarketData from GetAllMarkets");
            try
            {
                id = Int32.Parse(marketDatas[0]);
            }
            catch (Exception e)
            {
                
                ExceptionWriter.Instance.WriteException(e);
                return;
            }
            name = marketDatas[1]; 

            if (name.Equals("Total Goals", StringComparison.OrdinalIgnoreCase))
            {
                string asianId = marketDatas[7];
            }

            if (name.StartsWith("Correct Score", StringComparison.OrdinalIgnoreCase))
            {
                
            }


            if (marketDatas.Length < 5)
                return;
            match = BFMarket.SplitFullMarketName(marketDatas[5]);
            devision = BFMarket.SplitDevision(marketDatas[5]);

            startDts = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);
            startDts = startDts.AddMilliseconds(Double.Parse(marketDatas[4]));
            startDts = startDts.ToLocalTime();
            //startDts = new DateTime(marketDatas[5]);
            if (marketDatas.Length < 16)
            {
                inPlay = false;
                return;
            }

            if (marketDatas[15] == "Y")
            {
                inPlay = true;
            }
            else
            {
                inPlay = false;
            }
        }

        private static String SplitFullMarketName(String fullmarketname)
        {
            char[] cSeps = { '\\' };
            String[] splittedMarketName = fullmarketname.Split(cSeps);
            splittedMarketName[splittedMarketName.Length - 1] = splittedMarketName[splittedMarketName.Length - 1].Replace(" v ", " - ");
            return splittedMarketName[splittedMarketName.Length - 1];
        }

        private static String SplitDevision(String fullmarketname)
        {
            char[] cSeps = { '\\' };
            String[] splittedMarketName = fullmarketname.Split(cSeps);
            splittedMarketName[splittedMarketName.Length - 1] = splittedMarketName[splittedMarketName.Length - 1].Replace(" v ", " - ");

            return splittedMarketName[2];
        }
    }
}
