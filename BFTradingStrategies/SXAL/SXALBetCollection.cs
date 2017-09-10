using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.SXAL
{
    public class SXALBetCollection
    {
        SortedList<long, SXALBet> m_bets = new SortedList<long, SXALBet>();

        public bool AllBetsCanceled
        {
            get
            {
                if (m_bets.Count == 0)
                    return false;

                bool bRet = true;
                foreach (SXALBet bet in m_bets.Values)
                {
                    if (bet.BetStatus != SXALBetStatusEnum.C && bet.BetStatus != SXALBetStatusEnum.L &&
                        bet.BetStatus != SXALBetStatusEnum.V)
                        bRet = false;
                }
                return bRet;
            }
        }

        public bool OneBetUnmatched
        {
            get
            {
                if (m_bets.Count == 0)
                    return false;
                
                foreach (SXALBet bet in m_bets.Values)
                {
                    if (bet.BetStatus == SXALBetStatusEnum.U || bet.BetStatus == SXALBetStatusEnum.MU)
                        return true;
                }
                return false;
            }
        }

        public bool AllBetsUnmatched
        {
            get
            {
                if (m_bets.Count == 0)
                    return false;

                bool bRet = true;
                foreach (SXALBet bet in m_bets.Values)
                {
                    if (bet.BetStatus != SXALBetStatusEnum.U)
                        bRet = false;
                }
                return bRet;
            }
        }

        public SortedList<long, SXALBet> Bets
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
                    return m_bets.Values[0].AsianLineId;
                }
            }
        }

        public long SelectionId
        {
            get
            {
                if (m_bets.Count == 0)
                    return 0;
                else
                {
                    return m_bets.Values[0].SelectionId;
                }
            }
        }

        public long MarketId
        {
            get
            {
                if (m_bets.Count == 0)
                    return 0;
                else
                {
                    return m_bets.Values[0].MarketId;
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
                    foreach (SXALBet bet in m_bets.Values)
                    {
                        if (bet.MatchedDate.Ticks < tmpDts.Ticks)
                            tmpDts = bet.MatchedDate;
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
                    foreach (SXALBet bet in m_bets.Values)
                    {
                        if (bet.BetType == SXALBetTypeEnum.L)
                        {
                            double tmpValue = (bet.AvgPrice * bet.MatchedSize) - bet.MatchedSize;
                            riskWin += tmpValue;
                        }
                        else
                        {
                            double tmpValue = (bet.AvgPrice * bet.MatchedSize) - bet.MatchedSize;
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
                if (m_bets.Count == 0)
                    return 0.0;
                else
                {
                    double betSize = 0.0;
                    double betSizeMatched = 0.0;
                    foreach (SXALBet bet in m_bets.Values)
                    {

                        if (bet != null)
                        {
                            if (bet.BetStatus != SXALBetStatusEnum.M && bet.BetStatus != SXALBetStatusEnum.MU)
                                continue;
                            betSize += bet.MatchedSize;
                        }

                        if (bet.Matches != null && bet.Matches.Count() > 0)
                        {
                            foreach (SXALMatch match in bet.Matches)
                            {
                                betSizeMatched += match.SizeMatched;
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
                if (m_bets.Count == 0)
                    return 0.0;
                else
                {
                    double zaehler = 0.0;
                    double nenner = 0.0;
                    double mittel = 0.0;

                    foreach (SXALBet bet in m_bets.Values)
                    {
                        if (bet.BetStatus != SXALBetStatusEnum.M && bet.BetStatus != SXALBetStatusEnum.MU)
                            continue;
                        zaehler += bet.MatchedSize * bet.AvgPrice;
                        nenner += bet.MatchedSize;
                    }

                    if (nenner != 0)
                        mittel = zaehler / nenner;
                    else
                        mittel = 0.0;
                    return Math.Round(mittel, 2);
                }
            }
        }
       
        public void Reload()
        {
            SortedList<long, SXALBet> m_betsTmp = m_bets;

            for (int i = 0; i < m_bets.Values.Count; i++)
            {
                m_bets[m_bets.Values[i].BetId] = SXALKom.Instance.getBetDetail(m_bets.Values[i].BetId);
            }
        }
    }
}
