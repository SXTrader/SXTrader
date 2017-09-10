using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BetFairIF.com.betfair.api.exchange;


namespace net.sxtrader.bftradingstrategies.betfairif
{
    public class BetCollection
    {
        SortedList<long, Bet> m_bets = new SortedList<long,Bet>();

        public bool AllBetsCanceled
        {
            get
            {
                if (m_bets.Count == 0)
                    return false;

                bool bRet = true;
                foreach (Bet bet in m_bets.Values)
                {
                    if (bet.betStatus != BetStatusEnum.C && bet.betStatus != BetStatusEnum.L &&
                        bet.betStatus != BetStatusEnum.V)
                        bRet = false;
                }
                return bRet;
            }
        }

        public bool AllBetsUnmatched
        {
            get
            {
                if (m_bets.Count == 0)
                    return false;

                bool bRet = true;
                foreach (Bet bet in m_bets.Values)
                {
                    if (bet.betStatus != BetStatusEnum.U)
                        bRet = false;
                }
                return bRet;
            }
        }

        public SortedList<long,Bet>Bets
        {
            get
            {
                return m_bets;
            }
            set
            {
                m_bets = value;
            }
        }

        public int AsianId
        {
            get
            {
                if (m_bets.Count == 0)
                    return 0;
                else
                {
                    return m_bets.Values[0].asianLineId;
                }
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
                if(m_bets.Count == 0)
                    return 0;
                else
                {
                    return m_bets.Values[0].marketId;
                }
            }
        }

        public DateTime OldesDts
        {
            get
            {
                if (m_bets.Count == 0)
                    return DateTime.Now;
                else
                {
                    DateTime tmpDts = DateTime.Now;
                    foreach (Bet bet in m_bets.Values)
                    {
                        if (bet.matchedDate.Ticks < tmpDts.Ticks)
                            tmpDts = bet.matchedDate;
                    }
                    return tmpDts;
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
                    foreach (Bet bet in m_bets.Values)
                    {
                        if (bet.betType == BetTypeEnum.L)
                        {
                            double tmpValue = (bet.avgPrice * bet.matchedSize)-bet.matchedSize;
                            riskWin += tmpValue;
                        }
                        else
                        {
                            double tmpValue = (bet.avgPrice * bet.matchedSize) - bet.matchedSize;
                            riskWin += tmpValue;
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
                if(m_bets.Count == 0)
                    return 0.0;
                else
                {
                    double betSize = 0.0;
                    double betSizeMatched = 0.0;
                    foreach(Bet bet in m_bets.Values)
                    {

                        if (bet != null)
                        {
                            if (bet.betStatus != BetStatusEnum.M && bet.betStatus != BetStatusEnum.MU)
                                continue;
                            betSize += bet.matchedSize;
                        }

                        if (bet.matches.Count() > 0)
                        {
                            foreach (Match match in bet.matches)
                            {
                                betSizeMatched += match.sizeMatched;
                            }
                        }
                    }


                    if (betSizeMatched > betSize)
                        return betSizeMatched;

                    return betSize;
                }
            }
        }

        public double BetPrice
        {
            get
            {
                if(m_bets.Count == 0)
                    return 0.0;
                else
                {
                    double zaehler = 0.0;
                    double nenner = 0.0;
                    double mittel = 0.0;

                    foreach(Bet bet in m_bets.Values)
                    {
                        if (bet.betStatus != BetStatusEnum.M && bet.betStatus != BetStatusEnum.MU)
                            continue;
                        zaehler += bet.matchedSize * bet.avgPrice;
                        nenner += bet.matchedSize;
                    }

                    mittel = zaehler / nenner;
                    return Math.Round(mittel,2);
                }
            }
        }
/*
        public void add(Bet bet)
        {
            if(bet == null)
                return;

            Bet oldBet = m_bets[bet.betId];
            //Nur Falls diese BetId noch nicht existiert sie hinzufügen
            if(oldBet==null)
            {
                m_bets.Add(bet.betId,bet);
            }
            else
            {                
            }
        }

        public Bet getById(long betid)
        {
            return m_bets[betid];
        }
 */
        public void Reload()
        {
            SortedList<long, Bet> m_betsTmp = m_bets;
            
            for (int i = 0; i < m_bets.Values.Count; i++)
            {
                m_bets[m_bets.Values[i].betId] = BetfairKom.Instance.getBetDetail(m_bets.Values[i].betId);
            }
        }
    }
}
