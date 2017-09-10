using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.betfairif.mockups;
using net.sxtrader.bftradingstrategies.livescoreparser;
using System.Threading;
using net.sxtrader.bftradingstrategies.betfairif;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using BetFairIF.com.betfair.api.exchange;

namespace net.sxtrader.bftradingstrategies.BackThe4
{
    // Alles nur für Testzwecke
    class BFBT4PTradeStartedEventArgs : EventArgs
    {
        private Bet m_bet;
        private string m_match;
        public BFBT4PTradeStartedEventArgs(Bet bet, String match)
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

    class BFBT4PNoTradeEventArgs : EventArgs
    {
        private int m_marketId;
        private string m_match;
        private string m_reason;

        public BFBT4PNoTradeEventArgs(int marketId, string match, string reason)
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
    class BFBT4Preparer
    {
        private int m_id;
        private BFMarket m_market;
        private Thread m_threadPreparer;
        private LiveScoreParser m_parser;
        private Bet m_openBack;

        public event EventHandler<BFBT4PNoTradeEventArgs> NoTradePossible;
        public event EventHandler<BFBT4PTradeStartedEventArgs> TradeStarted;


        public BFBT4Preparer(BFMarket market, LiveScoreParser parser)
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

        private void preparer()
        {
            BT4ConfigurationRW m_config = new BT4ConfigurationRW();
            DateTime dtsPrepare = m_market.StartDTS.Subtract(new TimeSpan(0, 0, 90));
            TimeSpan timeToWait = dtsPrepare.Subtract(DateTime.Now);
            if (timeToWait.Ticks < 0)
                timeToWait = new TimeSpan(0, 0, 0);

            Thread.Sleep(timeToWait);

            CompleteMarketPrices marketPrices = new CompleteMarketPrices(m_id);

            /*
            if (marketPrices.MarketAmountMatched < m_config.MarketVolume)
            {
                EventHandler<BFLT4PNoTradeEventArgs> handler = NoTradePossible;
                if (handler != null)
                    handler(this, new BFLT4PNoTradeEventArgs(m_id, m_market.Match, LayThe4.strNoMarketVolume));
                return;
            }
            */
            double backOdds = marketPrices.getBestBackOfSelectionByOrderId(3);

            if (backOdds == 0.0)
            {
                EventHandler<BFBT4PNoTradeEventArgs> handler = NoTradePossible;
                if (handler != null)
                    handler(this, new BFBT4PNoTradeEventArgs(m_id, m_market.Match, "No Back Odds"));
                return;
            }            

            // Einsatz abholen
            double money = 10.0; //BankrollManager.Instance.getMoneyForLay(m_config.PercentBank, layOdds);
            
            String[] sepMatch = { " - " };
            String[] teams = m_market.Match.Split(sepMatch, StringSplitOptions.None);//SoccerMarketManager.Instance.getMatchById(m_id).Split(sepMatch,StringSplitOptions.None);
            IScore score = m_parser.injectBetfair(teams[0], teams[1]);
            if (score == null)
            {
                EventHandler<BFBT4PNoTradeEventArgs> handler = NoTradePossible;
                if (handler != null)
                    handler(this, new BFBT4PNoTradeEventArgs(m_id, m_market.Match, "No Mapping to Livescore"));
                return;
            }
            score.DecreaseRef();

            //TODO: durch reale Objekte ersetzen
            // und setzten
            int selectionId = marketPrices.getSelectionIdByOrderId(3);
            int asianId = marketPrices.getAsianIdOfSelection(selectionId);
            
            Bet bet = BetfairKom.Instance.placeBackBet(this.m_id, selectionId, asianId, backOdds, money);
            if (bet == null)
            {
                //TODO: Eventhandler;
                return;
            }
            
            if (bet.betStatus == BetFairIF.com.betfair.api.exchange.BetStatusEnum.M)
            {
                EventHandler<BFBT4PTradeStartedEventArgs> handler = TradeStarted;
                if (handler != null)
                    handler(this, new BFBT4PTradeStartedEventArgs(bet, m_market.Match));
                return;
            }
            else
            {
                m_openBack = bet;
                /*
                m_threadUnmatchedLayWatcher = new Thread(unmatchedLayWatcher);
                m_threadUnmatchedLayWatcher.IsBackground = true;
                m_threadUnmatchedLayWatcher.Start();
                 */

            }

            //TODO: Überwachung für nicht gesetzte Lays

        }
    }
}
