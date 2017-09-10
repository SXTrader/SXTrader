using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using BetFairIF.com.betfair.api;
using BetFairIF.com.betfair.api.exchange;
using System.IO;
using System.Windows.Forms;


namespace net.sxtrader.bftradingstrategies.betfairif.mockups
{
    public sealed class BetfairKomMockUp
    {
        private static volatile BetfairKomMockUp instance;
        private static Object syncRoot = new Object();

        private BetfairKomMockUp() { }

        public static BetfairKomMockUp Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BetfairKomMockUp();
                    }
                }

                return instance;
            }
        }

        public MockUpBet placeLayBet(int marketId, int selectionId, int asianId, double price, double money)
        {
            MockUpBet bet = new MockUpBet(BetTypeEnum.B);
            bet.marketId = marketId;
            bet.selectionId = selectionId;
            bet.avgPrice = price;
            bet.matchedSize = money;
            bet.asianId = asianId;
            return bet;
        }

        public MockUpBet placeBackBet(int marketId, int selectionId, int asianId, double price, double money)
        {
            MockUpBet bet = new MockUpBet(BetTypeEnum.B);
            bet.marketId = marketId;
            bet.selectionId = selectionId;
            bet.avgPrice = price;
            bet.matchedSize = money;
            bet.asianId = asianId;
            return bet;
        }

        public MockUpBet placeBackBet(int marketId, int selectionId, double price, double money)
        {
            MockUpBet bet = new MockUpBet(BetTypeEnum.B);
            bet.marketId = marketId;
            bet.selectionId = selectionId;
            bet.avgPrice = price;
            bet.matchedSize = money;
            return bet;
        }

        public MockUpBet placeLayBet(int marketId, int selectionId, double price, double money)
        {
            MockUpBet bet = new MockUpBet(BetTypeEnum.L);
            bet.marketId = marketId;
            bet.selectionId = selectionId;
            bet.avgPrice = price;
            bet.matchedSize = money;
            return bet;
        }
    }

    public class MockUpBet
    {
        private BetTypeEnum m_type;
        private int m_marketId;
        private int m_selectionId;
        private int m_asianId;
        private double m_avgPrice;
        private double m_matchedSize;
        private int m_betId;

        public MockUpBet(BetTypeEnum type)
        {
            m_type = type; ;
            Random rand = new Random();
            m_betId = rand.Next();
        }

        public int asianId
        {
            get
            {
                return m_asianId;
            }
            set
            {
                m_asianId = value;
            }
        }

        public int betId
        {
            get
            {
                return m_betId;
            }
        }

        public double matchedSize
        {
            get
            {
                return m_matchedSize;
            }
            set
            {
                m_matchedSize = value;
            }
        }

        public double avgPrice
        {
            get
            {
                return m_avgPrice;
            }
            set
            {
                m_avgPrice = value;
            }
        }

        public int selectionId
        {
            get
            {
                return m_selectionId;
            }
            set
            {
                m_selectionId = value;
            }
        }

        public int marketId
        {
            get
            {
                return m_marketId;
            }
            set
            {
                m_marketId = value;
            }
        }

        public BetStatusEnum betStatus
        {
            get
            {
                return BetStatusEnum.M;
            }
        }

        public BetTypeEnum betType
        {
            get
            {
                return m_type;
            }
        }

    }

    public class BetCollectionMockUp
    {
        SortedList<long, MockUpBet> m_bets = new SortedList<long, MockUpBet>();

        public SortedList<long, MockUpBet> Bets
        {
            get
            {
                return m_bets;
            }
        }

        public int SelectionId
        {
            get
            {
                if (m_bets.Count == 0)
                    return 0;
                else
                {
                    return m_bets.Values[0].selectionId;
                }
            }
        }

        public int MarketId
        {
            get
            {
                if (m_bets.Count == 0)
                    return 0;
                else
                {
                    return m_bets.Values[0].marketId;
                }
            }
        }

        public double RiskWin
        {
            get
            {
                if (m_bets.Count == 0)
                    return 0.0;
                else
                {
                    double riskWin = 0.0;
                    foreach (MockUpBet bet in m_bets.Values)
                    {
                        if (bet.betType == BetTypeEnum.L)
                        {
                            riskWin += (bet.avgPrice * bet.matchedSize) - bet.matchedSize;
                        }
                        else
                        {
                            riskWin += (bet.avgPrice * bet.matchedSize) - bet.matchedSize;
                        }
                    }
                    return riskWin;
                }
            }
        }

        public double BetSize
        {
            get
            {
                if (m_bets.Count == 0)
                    return 0.0;
                else
                {
                    double betSize = 0.0;
                    foreach (MockUpBet bet in m_bets.Values)
                    {
                        betSize += bet.matchedSize;
                    }
                    return betSize;
                }
            }
        }

        public double BetPrice
        {
            get
            {
                if (m_bets.Count == 0)
                    return 0.0;
                else
                {
                    double zaehler = 0.0;
                    double nenner = 0.0;
                    double mittel = 0.0;

                    foreach (MockUpBet bet in m_bets.Values)
                    {
                        zaehler += bet.matchedSize * bet.avgPrice;
                        nenner += bet.matchedSize;
                    }

                    mittel = zaehler / nenner;
                    return Math.Round(mittel, 2);
                }
            }
        }
        
    }
}