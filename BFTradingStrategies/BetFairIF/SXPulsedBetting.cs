using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using BetFairIF.com.betfair.api.exchange;
using System.Collections;


namespace net.sxtrader.bftradingstrategies.betfairif
{
    public class QueuedBetRequest
    {
        private BetTypeEnum _betType;
        private double _money;
        private double _odds;
        private int _marketId;
        private int _SelectionId;

        public BetTypeEnum BetType { get { return _betType; } }
        public double BetAmount { get { return _money; } }
        public double Odds { get { return _odds; } }
        public int MarketId { get { return _marketId; } }
        public int SelectionId { get { return _SelectionId; } }

        public QueuedBetRequest(BetTypeEnum betType, double betAmount, double odds, int marketId, int selectionId)
        {
            _betType = betType;
            _money = betAmount;
            _odds = odds;
            _marketId = marketId;
            _SelectionId = selectionId;
        }

    }

    public class BetEventArgs : EventArgs
    {
        Bet _bet;
        public Bet Bet
        {
            get { return _bet; }
        }

        public BetEventArgs(Bet bet)
        {
            _bet = bet;
        }

    }

    public static class SXPulsedBetting
    {
        public static event EventHandler<BetEventArgs> BetPlaced;

        private static Timer _timer = null;
        private static Queue<QueuedBetRequest> _queue = null;
        private static object _lockPulse = "lockPulse";
        private const int MAXBETSPERPULSE = 30;

        static SXPulsedBetting()
        {
            _queue = new Queue<QueuedBetRequest>();
            _timer = new Timer(30000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
        }

        public static void addBetRequestToQueue(QueuedBetRequest betRequest)
        {
            lock (_lockPulse)
            {
                _queue.Enqueue(betRequest);
            }
        }

        static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockPulse)
            {
                int i = 0;
                ArrayList bets = new ArrayList(30);
                if (_queue.Count == 0)
                    return;

                // Die Queue abarbeiten und maximal 30 Wettanfragen für die Verarbeitung vorbereiten
                while (_queue.Count > 0)
                {
                    if (i > MAXBETSPERPULSE)
                        break;
                    QueuedBetRequest betRequest = _queue.Dequeue();
                    
                    PlaceBets bet = new PlaceBets();
                    bet.betType = betRequest.BetType;
                    bet.marketId = betRequest.MarketId;
                    bet.selectionId = betRequest.SelectionId;
                    bet.size = betRequest.BetAmount;
                    bet.price = betRequest.Odds;

                    bet.asianLineId = 0;
                    bet.bspLiability = 0.0;
                    bet.betCategoryType = BetCategoryTypeEnum.E;
                    bet.betPersistenceType = BetPersistenceTypeEnum.NONE;

                    bets.Add(bet);
                    i++;
                }

                // In einen Array Umwandeln
                PlaceBets[] betsToPlace = (PlaceBets[])bets.ToArray(typeof(PlaceBets));

                // Wetten absetzen
                Bet[] placedBets =  BetfairKom.Instance.placeBets(betsToPlace);

                foreach (Bet placedBet in placedBets)
                {
                    EventHandler<BetEventArgs> betPlaced = BetPlaced;
                    if (betPlaced != null)
                        betPlaced(null, new BetEventArgs(placedBet));                   
                }

            }
        }
    }
}
