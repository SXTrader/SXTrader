using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using net.sxtrader.bftradingstrategies.betfairif;
//using net.sxtrader.bftradingstrategies.betfairif.mockups;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using BetFairIF.com.betfair.api.exchange;

namespace net.sxtrader.bftradingstrategies.LayThe4
{



    class BFLT4PTradeStartedEventArgs : EventArgs
    {
        private Bet m_bet;
        private string m_match;
        public BFLT4PTradeStartedEventArgs(Bet bet, String match)
        {
            m_bet = bet;
            m_match = match;
        }

        public String Match
        {
            get
            {
                return m_match;
            }
        }

        public Bet Bet
        {
            get
            {
                return m_bet;
            }
        }
    }

    class BFLT4PNoTradeEventArgs : EventArgs
    {
        private int m_marketId;
        private string m_match;
        private string m_reason;

        public BFLT4PNoTradeEventArgs(int marketId, string match, string reason)
        {
            m_marketId = marketId;
            m_match = match;
            m_reason = reason;
        }

        public int MarketId
        {
            get
            {
                return m_marketId;
            }
        }

        public string Match
        {
            get
            {
                return m_match;
            }
        }

        public string Reason
        {
            get
            {
                return m_reason;
            }
        }
    }

    class BFLT4Preparer
    {
        private int m_id;
        private BFMarket m_market;
        private Thread m_threadPreparer;
        private Thread m_threadUnmatchedLayWatcher;
        private LiveScoreParser m_parser;
        private Bet m_openLay;

        public event EventHandler<BFLT4PNoTradeEventArgs> NoTradePossible;
        public event EventHandler<BFLT4PTradeStartedEventArgs> TradeStarted;


        public BFLT4Preparer(BFMarket market, LiveScoreParser parser)
        {
            m_id = market.Id;
            m_market = market;
            m_parser = parser;
        }

        public void Start()
        {
            m_threadPreparer = new Thread(preparer);
            m_threadPreparer.IsBackground = true;
            m_threadPreparer.Start();
        }

        private void unmatchedLayWatcher()
        {
            DateTime dtsTimeOut = m_market.StartDTS.Subtract(new TimeSpan(0, 0, 5));
            TimeSpan timeToWait = dtsTimeOut.Subtract(DateTime.Now);
            if(timeToWait.Ticks < 0)
                timeToWait = new TimeSpan(0, 0, 0);

            Thread.Sleep(timeToWait);

            try
            {
                m_openLay = BetfairKom.Instance.getBetDetail(m_openLay.betId);

                BankrollManager.Instance.placedLayBet(m_openLay.matchedSize, m_openLay.avgPrice);
                if (m_openLay.betStatus == BetStatusEnum.M)
                {
                    EventHandler<BFLT4PTradeStartedEventArgs> handler = TradeStarted;
                    if (handler != null)
                        handler(this, new BFLT4PTradeStartedEventArgs(m_openLay, m_market.Match));
                    return;
                }
                else if (m_openLay.betStatus == BetStatusEnum.U)
                {
                    BetfairKom.Instance.cancelBet(m_openLay.betId);
                    EventHandler<BFLT4PNoTradeEventArgs> handler = NoTradePossible;
                    if (handler != null)
                        handler(this, new BFLT4PNoTradeEventArgs(m_id, m_market.Match, LayThe4.strLayUnmatched));
                    return;
                }
                else if (m_openLay.betStatus == BetStatusEnum.MU)
                {
                    BetfairKom.Instance.cancelBet(m_openLay.betId);
                    m_openLay = BetfairKom.Instance.getBetDetail(m_openLay.betId);
                    EventHandler<BFLT4PTradeStartedEventArgs> handler = TradeStarted;
                    if (handler != null)
                        handler(this, new BFLT4PTradeStartedEventArgs(m_openLay, m_market.Match));
                    return;
                }
            }
            catch 
            {
                //TODO Internal Errors melden
                return;
            }
        }

        private void preparer()
        {
            LT4ConfigurationRW m_config = new LT4ConfigurationRW();
            DateTime dtsPrepare = m_market.StartDTS.Subtract(new TimeSpan(0, 0, m_config.TradeStart));
            TimeSpan timeToWait = dtsPrepare.Subtract(DateTime.Now);
            if (timeToWait.Ticks < 0)
                timeToWait = new TimeSpan(0, 0, 0);

            Thread.Sleep(timeToWait);

            CompleteMarketPrices marketPrices = new CompleteMarketPrices(m_id);

            if (marketPrices.MarketAmountMatched < m_config.MarketVolume)
            {
                EventHandler<BFLT4PNoTradeEventArgs> handler = NoTradePossible;
                if (handler != null)
                    handler(this, new BFLT4PNoTradeEventArgs(m_id, m_market.Match, LayThe4.strNoMarketVolume));
                return;
            }

            double layOdds = marketPrices.getBestLayOfSelectionByOrderId(3);

            if(layOdds == 0.0)
            {
                EventHandler<BFLT4PNoTradeEventArgs> handler = NoTradePossible;
                if (handler != null)
                    handler(this, new BFLT4PNoTradeEventArgs(m_id, m_market.Match, LayThe4.strNoLayOdds));
                return;
            }
            // Falls gewünscht: Ist Markt innerhalb der passenden Quoten
            if (!(m_config.MinOdds == 0.0 && m_config.MaxOdds == 0.0) && m_config.MaxOdds != 0.0)
            {
                if (marketPrices.getBestLayOfSelectionByOrderId(3) < m_config.MinOdds ||
                   marketPrices.getBestLayOfSelectionByOrderId(3) > m_config.MaxOdds)
                {
                    EventHandler<BFLT4PNoTradeEventArgs> handler = NoTradePossible;
                    if (handler != null)
                        handler(this, new BFLT4PNoTradeEventArgs(m_id, m_market.Match, String.Format(LayThe4.strLayOddsOutOfRange, m_config.MinOdds, m_config.MaxOdds)));
                    return;
                }
            }

            // Einsatz abholen
            double money = BankrollManager.Instance.getMoneyForLay(m_config.PercentBank, layOdds);

            // Ist Einsatz innerhalb der Vorgaben?
            if (money < m_config.MinAmount)
            {
                EventHandler<BFLT4PNoTradeEventArgs> handler = NoTradePossible;
                if (handler != null)
                    handler(this, new BFLT4PNoTradeEventArgs(m_id, m_market.Match, String.Format(LayThe4.strNoMoneyAvailible, money, m_config.MinAmount)));
                return;
            }

            if (money > m_config.MaxAmount)
            {
                money = m_config.MaxAmount;
            }

            if (money < BankrollManager.Instance.MinStake)
            {
                EventHandler<BFLT4PNoTradeEventArgs> handler = NoTradePossible;
                if (handler != null)
                    handler(this, new BFLT4PNoTradeEventArgs(m_id, m_market.Match, String.Format(LayThe4.strBelowMinStake, money, BankrollManager.Instance.MinStake)));
                return;
            }


            String[] sepMatch = {" - "};
            String[] teams = m_market.Match.Split(sepMatch, StringSplitOptions.None);//SoccerMarketManager.Instance.getMatchById(m_id).Split(sepMatch,StringSplitOptions.None);
            IScore score = m_parser.injectBetfair(teams[0], teams[1]);
            if (score == null)
            {
                EventHandler<BFLT4PNoTradeEventArgs> handler = NoTradePossible;
                if (handler != null)
                    handler(this, new BFLT4PNoTradeEventArgs(m_id, m_market.Match, LayThe4.strNoLVMap));
                return;
            }
            score.DecreaseRef();

            //TODO: durch reale Objekte ersetzen
            // und setzten
            int selectionId = marketPrices.getSelectionIdByOrderId(3);
            int asianId = marketPrices.getAsianIdOfSelection(selectionId);
            Bet bet = BetfairKom.Instance.placeLayBet(this.m_id, selectionId,asianId,layOdds,money);
            if (bet == null)
            {
                timeToWait = new TimeSpan(0, 0, 30);
                return;
            }
            BankrollManager.Instance.placedLayBet(bet.matchedSize, bet.avgPrice);
            if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
            {                
                EventHandler<BFLT4PTradeStartedEventArgs> handler = TradeStarted;
                if (handler != null)
                    handler(this, new BFLT4PTradeStartedEventArgs(bet, m_market.Match));
                return;
            }
            else
            {
                m_openLay = bet;
                m_threadUnmatchedLayWatcher = new Thread(unmatchedLayWatcher);
                m_threadUnmatchedLayWatcher.IsBackground = true;
                m_threadUnmatchedLayWatcher.Start();

            }

            //TODO: Überwachung für nicht gesetzte Lays

        }
    }
}
