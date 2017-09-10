using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.tippsters.Configration;
using net.sxtrader.muk;
using net.sxtrader.muk.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace net.sxtrader.bftradingstrategies.tippsters.Trade
{
    class TLLTradeList : List<TLLTrade> { }
    sealed class TLLTrade : IDisposable, IBFTSCommon
    {
        public enum TRADESTATE { STARTED, COMPLETED, TRADING, NONRUNNER, NOTTRADING };

        private TLLConfigurationRW _settings;
        private SXALMarket _market;
        private long _selectionId = 0;
        private TRADESTATE _state;

        private SXALSortedBetList _openBets;
        private SXALSortedBetList _matchedBets;
        private SXALSortedBetList _settledBets;
        private SXALSortedBetList _invalidBets;

        private String _horse;
        private String _race;
        private DateTime _dts;
        private double _maxOdds;

        private bool _disposed = false;

        private object _syncLock = "syncLock";

        #region Events
        public delegate void TLLTradeSettledHandler(object sender, TLLTradeSettledEventArgs e);
        public event TLLTradeSettledHandler TradeSettledEvent;
        #endregion

        #region Attribute
        public String Horse { get { return _horse; } }
        public String Race { get { return _race; } }
        public DateTime EventDate { get { return _dts; } }
        public double MaxOdds{get{return _maxOdds;}}

        public TRADESTATE TradeState
        {
            get { return _state; }
        }

        public String Currency
        {
            get { return SXALBankrollManager.Instance.Currency; }
        }

        public double MatchedAmount
        {
            get
            {
                double money = 0.0;
                foreach (SXALBet b in _matchedBets.Values)
                {
                    money += b.MatchedSize;
                }
                return Math.Round(money, 2);
            }
        }

        public double UnmatchedAmount
        {
            get
            {
                double money = 0.0;
                foreach (SXALBet b in _openBets.Values)
                {
                    money += b.RemainingSize;
                }
                return Math.Round(money, 2);
            }
        }

        public double Risk
        {
            get
            {
                double risk = 0.0;

                foreach (SXALBet b in _matchedBets.Values)
                {
                    risk += (b.MatchedSize * b.AvgPrice - b.MatchedSize);
                }

                return Math.Round(risk, 2);
            }
        }

        public double PotentialRisk
        {
            get
            {
                double risk = 0.0;

                foreach (SXALBet b in _openBets.Values)
                {
                    risk += (b.RemainingSize * b.Price - b.RemainingSize);
                }

                return Math.Round(risk, 2);
            }
        }

        public double AvgPrice
        {
            get
            {
                double avgPrice = 0.0;

                if (_matchedBets.Values.Count > 0)
                {
                    foreach (SXALBet b in _matchedBets.Values)
                    {
                        avgPrice += b.AvgPrice;
                    }
                    avgPrice = avgPrice / _matchedBets.Values.Count;
                }

                return Math.Round(avgPrice, 2);
            }
        }
        #endregion

        private TLLTrade()
        {
             log("Constructing a new Trade");

            _state = TRADESTATE.STARTED;
            _openBets = new SXALSortedBetList();
            _matchedBets = new SXALSortedBetList();
            _settledBets = new SXALSortedBetList();
            _invalidBets = new SXALSortedBetList();

            _settings = new TLLConfigurationRW();

            SXMinutePulse.Instance.Pulse += Instance_Pulse;
            SX5MinutePulse.Instance.Pulse += Instance_5Pulse;
        }

        private TLLTrade(SXALMarket market, long selectionId, string horseName, double maxOdds)
        {
            _state = TRADESTATE.STARTED;

            _market = market;
            _selectionId = selectionId;
            _horse = horseName;
            _race = _market.Match;
            _dts = _market.StartDTS;
            _maxOdds = maxOdds;

            _syncLock = getSyncLockName();

            log("Constructing a new Trade");

            _settings = new TLLConfigurationRW();
            _openBets = new SXALSortedBetList();
            _matchedBets = new SXALSortedBetList();
            _settledBets = new SXALSortedBetList();
            _invalidBets = new SXALSortedBetList();


            // Schaue, ob es schon wetten zu diesem Markt und Pferd gibt.
            SXALMUBet[] muBets = SXALKom.Instance.getBetsMU(market.Id);
            if (muBets != null)
            {
                foreach (SXALMUBet b in muBets)
                {
                    if (b.BetType != SXALBetTypeEnum.L)
                    {
                        // Keine lay => uninteressant
                        continue;
                    }

                    SXALBet existBet = SXALKom.Instance.getBetDetail(b.BetId);
                    if (existBet != null)
                    {
                        logBetDetails(existBet);
                        switch (existBet.BetStatus)
                        {
                            case SXALBetStatusEnum.M:
                                _matchedBets.Add(existBet.BetId, existBet);
                                break;
                            case SXALBetStatusEnum.MU:
                                _matchedBets.Add(existBet.BetId, existBet);
                                _openBets.Add(existBet.BetId, existBet);
                                break;
                            case SXALBetStatusEnum.U:
                                _openBets.Add(existBet.BetId, existBet);
                                break;
                            case SXALBetStatusEnum.S:
                                _settledBets.Add(existBet.BetId, existBet);
                                break;
                            default:
                                _invalidBets.Add(existBet.BetId, existBet);
                                break;
                        }
                    }
                }
            }

            //Pferd ist Non-runner?
            if (isHorseNonRunner())
            {
                // Benachrichtigungen schicken
                log("Horse is a Non Starter. No Trade possible!");
                _state = TRADESTATE.NONRUNNER;
                return;
            }

            //Non-Runner Observation
            if (checkNonRunners())
            {
                //Benachrichtigung 
                log("Too many Non Starter. Do not place Trade");
                _state = TRADESTATE.NOTTRADING;
                return;
            }

            //Wette setzen
            //Vorraussetzung erfüllt?
            if (!checkBetPrerequisites())
            {
                //Benachrichtigung?
                // Puls aufsetzen
                SXMinutePulse.Instance.Pulse += Instance_Pulse;
                SX5MinutePulse.Instance.Pulse += Instance_5Pulse;
                return;
            }

            //Preise holen
            log("Reading Prices");
            SXALMarketPrices prices = SXALKom.Instance.getMarketPrices(_market.Id);

            if (prices == null)
            {
                //Benachrichtigung?
                log("No prices found. Leaving");
                // Puls aufsetzen
                SXMinutePulse.Instance.Pulse += Instance_Pulse;
                SX5MinutePulse.Instance.Pulse += Instance_5Pulse;
                return;
            }

            log("Getting Odds for Trade");
            double odds = getOdds(prices);

            if (odds < 1.01)
            {
                log("Odds are invalid. Throwing Exception");
                throw new TLLInvalidOddsException(_horse, _race, _dts);
            }

            log(String.Format("Odd for Trades is {0}", odds));
            double money = calculateMoneyToBet(odds);


            if (money < SXALKom.Instance.MinStake && !SXALKom.Instance.SupportsBelowMinStakeBetting)
            {
                log("Bet Amount is below Minimum Stake and Sport Exchange does not support betting below minimum amount");
                // Puls aufsetzen
                SXMinutePulse.Instance.Pulse += Instance_Pulse;
                SX5MinutePulse.Instance.Pulse += Instance_5Pulse;
                _state = TRADESTATE.TRADING;
                //throw new LOPBelowMinStakeException(_race, _horse, _dts, money, SXALKom.Instance.getExchangeName());
            }


            log("Place Bet");
            //platziere Wette
            if (money > 0.1)
            {
                SXALBet bet = placeBet(odds, money);
                if (bet != null)
                {
                    logBetDetails(bet);
                    log(String.Format("Bet {0} placed. Status is {1}", bet.BetId, bet.BetStatus.ToString()));
                    ICollection icMatched = _matchedBets;
                    ICollection icOpen = _openBets;

                    switch (bet.BetStatus)
                    {
                        case SXALBetStatusEnum.M:
                            lock (icMatched.SyncRoot)
                            {
                                _matchedBets.Add(bet.BetId, bet);
                            }
                            break;
                        case SXALBetStatusEnum.MU:
                            lock (icMatched.SyncRoot)
                            {
                                _matchedBets.Add(bet.BetId, bet);
                            }
                            lock (icOpen.SyncRoot)
                            {
                                _openBets.Add(bet.BetId, bet);
                            }
                            break;
                        case SXALBetStatusEnum.U:
                            lock (icOpen.SyncRoot)
                            {
                                _openBets.Add(bet.BetId, bet);
                            }
                            break;
                        case SXALBetStatusEnum.S:
                            _settledBets.Add(bet.BetId, bet);
                            break;
                        default:
                            _invalidBets.Add(bet.BetId, bet);
                            break;
                    }
                }
            }

            // Puls aufsetzen
            SXMinutePulse.Instance.Pulse += Instance_Pulse;
            SX5MinutePulse.Instance.Pulse += Instance_5Pulse;
            _state = TRADESTATE.TRADING;
        }



        private TLLTrade(SXALBet[] bets, TLLConfigurationRW settings)
        {
            try
            {
                _state = TRADESTATE.STARTED;
                _settings = settings;

                if (bets.Count() > 0)
                {
                    _race = SXALHorseMarketManager.Instance.AllMarketsChaotic[bets[0].MarketId].Name;
                    _horse = bets[0].SelectionName;
                    _dts = SXALHorseMarketManager.Instance.AllMarketsChaotic[bets[0].MarketId].StartDTS;
                    

                    _market = SXALHorseMarketManager.Instance.AllMarketsChaotic[bets[0].MarketId];
                    _selectionId = bets[0].SelectionId;

                    _syncLock = getSyncLockName();
                }
                log("Constructing new Trade by existing bets");

                _openBets = new SXALSortedBetList();
                _matchedBets = new SXALSortedBetList();
                _settledBets = new SXALSortedBetList();
                _invalidBets = new SXALSortedBetList();


                foreach (SXALBet bet in bets)
                {
                    log(String.Format("Analysing Bet {0}", bet.BetId));

                    if (bet.BetType != SXALBetTypeEnum.L)
                    {
                        log(String.Format("Bet {0} is not a Lay Bet. Skipping", bet.BetId));
                    }

                    if (_selectionId != 0 && _selectionId != bet.SelectionId)
                    {
                        //Hier ist etwas schief gegangen. SelectionId muss für jede Wette übereinstimmen
                        //Ausnahme auslösen
                        throw new TLLSelectionIDMismatchException(_selectionId, bet.SelectionId);
                    }

                    log(String.Format("Status of Bet {0} is {1}", bet.BetId, bet.BetStatus.ToString()));
                    logBetDetails(bet);
                    switch (bet.BetStatus)
                    {
                        case SXALBetStatusEnum.M:
                            _matchedBets.Add(bet.BetId, bet);
                            break;
                        case SXALBetStatusEnum.MU:
                            _matchedBets.Add(bet.BetId, bet);
                            _openBets.Add(bet.BetId, bet);
                            break;
                        case SXALBetStatusEnum.U:
                            _openBets.Add(bet.BetId, bet);
                            break;
                        case SXALBetStatusEnum.S:
                            _settledBets.Add(bet.BetId, bet);
                            break;
                        default:
                            _invalidBets.Add(bet.BetId, bet);
                            break;
                    }
                }

                SXMinutePulse.Instance.Pulse += Instance_Pulse;
                SX5MinutePulse.Instance.Pulse += Instance_5Pulse;

                _state = TRADESTATE.TRADING;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                throw exc;
            }
        }


        private void Instance_Pulse(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(matchedBetsWatcher));
            ThreadPool.QueueUserWorkItem(new WaitCallback(openBetsWatcher));
        }

        private void Instance_5Pulse(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(marketWatcher));
        }

        private void marketWatcher(Object stateInfo)
        {
            lock (_syncLock)
            {
                try
                {
                    log("Observing Market Status");

                    SXALMarketLite marketLite = SXALKom.Instance.getMarketInfo(_market.Id);
                    if (marketLite == null)
                    {
                        log("Could not retriev market status. Skipping");
                    }

                    if (marketLite.MarketStatus == SXALMarketStatusEnum.SETTLED || marketLite.MarketStatus == SXALMarketStatusEnum.CLOSED)
                    {
                        log("Market is settled. Calculating Results");

                        //Abrechnung und Signal nach außen, dass Trade beendet
                        if (TradeSettledEvent != null)
                        {
                            TradeSettledEvent(this, new TLLTradeSettledEventArgs());
                        }


                        SXMinutePulse.Instance.Pulse -= Instance_Pulse;
                        SX5MinutePulse.Instance.Pulse -= Instance_5Pulse;

                        _state = TRADESTATE.COMPLETED;
                    }
                }
                catch (SXALMarketDoesNotExistException)
                {
                    //Dieser Fehler cann auftreten, wenn das Update auf den Markt zu spät kommt und dieser schon erloschen ist.
                    //Dies passiert häufig bei Pferdemärkte
                    //Jetzt  versuchen wir über die Wetten zu gehen
                    for (int i = 0; i < _matchedBets.Values.Count; i++)
                    {
                        SXALBet newBet = null;
                        if (checkBetMove(_matchedBets.Values[i], out newBet))
                        {
                            _matchedBets.RemoveAt(i);
                            if (i > 0)
                                i--;
                        }
                    }

                    for (int i = 0; i < _openBets.Values.Count; i++)
                    {
                        SXALBet newBet = null;
                        if (checkBetMove(_openBets.Values[i], out newBet))
                        {
                            _openBets.RemoveAt(i);
                            if (i > 0)
                                i--;
                        }
                    }

                    //Nur wenn wir wirklich keine gesetzten oder offenen Wetten mehr haben
                    if (_matchedBets.Count == 0 && _openBets.Count == 0)
                    {
                        log("Market is settled. Calculating Results");

                        //Abrechnung und Signal nach außen, dass Trade beendet
                        if (TradeSettledEvent != null)
                        {
                            TradeSettledEvent(this, new TLLTradeSettledEventArgs());
                        }


                        SXMinutePulse.Instance.Pulse -= Instance_Pulse;
                        SX5MinutePulse.Instance.Pulse -= Instance_5Pulse;

                        _state = TRADESTATE.COMPLETED;
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }



        private void openBetsWatcher(Object stateInfo)
        {
            lock (_syncLock)
            {
                try
                {
                    TLLConfigurationRW config = new TLLConfigurationRW();
                    if (config.StrategyActive == false)
                    {
                        log("Strategy is not active");
                        return;
                    }

                    log("Observing Open Bets");

                    if (_state == TRADESTATE.COMPLETED)
                    {
                        log("Trade already completed");
                        return;
                    }

                    if (!checkBetPrerequisites())
                    {
                        log("Prerequisites are not met");
                        return;
                    }

                    if (nonRunnerObservation(_openBets))
                    {
                        for (int i = 0; i < _openBets.Values.Count; i++)
                        {
                            log(String.Format("Cancel Bet {0}", _openBets.Values[i].BetId));
                            if (SXALKom.Instance.cancelBet(_openBets.Values[i].BetId))
                            {
                                SXALBet newBet = null;
                                if (checkBetMove(_openBets.Values[i], out newBet))
                                {
                                    if (newBet != null && newBet.BetStatus != SXALBetStatusEnum.MU)
                                    {
                                        _openBets.RemoveAt(i);
                                        i--;
                                    }
                                }
                            }
                        }

                        //Benachrichtigung                     
                        return;
                    }

                    if (checkNonRunners())
                    {
                        log("To many Non Runners. Cancel all bets");


                        for (int i = 0; i < _openBets.Values.Count; i++)
                        {
                            log(String.Format("Cancel Bet {0}", _openBets.Values[i].BetId));
                            if (SXALKom.Instance.cancelBet(_openBets.Values[i].BetId))
                            {
                                SXALBet newBet = null;
                                if (checkBetMove(_openBets.Values[i], out newBet))
                                {
                                    if (newBet != null && newBet.BetStatus != SXALBetStatusEnum.MU)
                                    {
                                        _openBets.RemoveAt(i);
                                        i--;
                                    }
                                }
                            }
                        }

                        //Benachrichtigung                     
                        _state = TRADESTATE.NOTTRADING;
                        return;
                    }

                    List<SXALBet> listBetsToRemove = new List<SXALBet>();
                    //Überprüfe offene Wetten und verschiebe in richtige Liste
                    for (int i = 0; i < _openBets.Values.Count; i++)                    
                    {
                        log(String.Format("Checking state of open Bet {0}", _openBets.Values[i].BetId));
                        SXALBet newBet = null;
                        if (checkBetMove(_openBets.Values[i], out newBet))
                        {
                            if (newBet != null && newBet.BetStatus != SXALBetStatusEnum.MU)
                            {
                                log(String.Format("Removing bet {0} from the List of Open Bets", _openBets.Values[i].BetId));
                                listBetsToRemove.Add(_openBets.Values[i]);
                            }
                        }
                    }

                    foreach (SXALBet b in listBetsToRemove)
                    {
                        _openBets.Remove(b.BetId);
                    }

                    // Muss eine Wette storniert werden, z.B. weil bessere Quoten?
                    SXALMarketPrices prices = SXALKom.Instance.getMarketPrices(_market.Id);

                    //Zweiter Runde: Sind wir in der Final Time?
                    if (_settings.UseFinalTime && _dts.Subtract(DateTime.Now).TotalMinutes <= _settings.FinalTime)
                    {
                        log("Final Betting Time. Canceling open Bets");
                        //Wir stornieren alle offenen Wetten
                        for (int i = 0; i < _openBets.Values.Count; i++)                        
                        {
                            if (_openBets.Values[i].Price == _maxOdds)
                            {
                                log(String.Format("Bet {0} has alread maximul odds of {0}. Bet will not be canceled", _openBets.Values[i].BetId, _maxOdds));
                                continue;
                            }
                            log(String.Format("Cancel bet {0}", _openBets.Values[i].BetId));
                            SXALKom.Instance.cancelBet(_openBets.Values[i].BetId);
                            SXALBet newBet = null;
                            if (checkBetMove(_openBets.Values[i], out newBet))
                            {
                                if (newBet != null && newBet.BetStatus != SXALBetStatusEnum.MU)
                                {
                                    log(String.Format("Removing bet {0} from the List of Open Bets", _openBets.Values[i].BetId));
                                    listBetsToRemove.Add(_openBets.Values[i]);
                                }
                            }
                        }

                        foreach (SXALBet b in listBetsToRemove)
                        {
                            _openBets.Remove(b.BetId);
                        }
                    }
                    else
                    {
                        log("Checking prices of open bets");
                        if (prices == null)
                        {
                            //loggen und Nachricht
                            log("Could not retrieve new prices. Leaving");
                            return;
                        }

                        for (int i = 0; i < _openBets.Values.Count; i++)                        
                        {
                            log(String.Format("Check if it is neccessary to cancel bet {0}", _openBets.Values[i].BetId));
                            if (checkBetCancel(_openBets.Values[i], prices))
                            {
                                log(String.Format("Cancel Bet {0}", _openBets.Values[i].BetId));
                                SXALKom.Instance.cancelBet(_openBets.Values[i].BetId);
                                SXALBet newBet = null;
                                if (checkBetMove(_openBets.Values[i], out newBet))
                                {
                                    if (newBet != null && newBet.BetStatus != SXALBetStatusEnum.MU)
                                    {
                                        listBetsToRemove.Add(_openBets.Values[i]);
                                    }
                                }
                            }
                        }

                        foreach (SXALBet b in listBetsToRemove)
                        {
                            log(String.Format("Remove bet {0} from list of open bets", b.BetId));
                            _openBets.Remove(b.BetId);
                        }
                    }

                    // Quoten holen
                    log("Get new odds");
                    double odds = getOdds(prices);

                    double moneyToBet = calculateMoneyToBet(odds);
                    if (moneyToBet > 0)
                    {
                        log("Place a new bet");
                        SXALBet bet = placeBet(odds, moneyToBet);
                        if (bet != null)
                        {
                            log(String.Format("Bet {0} placed. Status is {1}", bet.BetId, bet.BetStatus.ToString()));
                            logBetDetails(bet);
                            switch (bet.BetStatus)
                            {
                                case SXALBetStatusEnum.M:
                                    _matchedBets.Add(bet.BetId, bet);
                                    break;
                                case SXALBetStatusEnum.MU:
                                    _matchedBets.Add(bet.BetId, bet);
                                    _openBets.Add(bet.BetId, bet);
                                    break;
                                case SXALBetStatusEnum.U:
                                    _openBets.Add(bet.BetId, bet);
                                    break;
                                case SXALBetStatusEnum.S:
                                    _settledBets.Add(bet.BetId, bet);
                                    break;
                                default:
                                    _invalidBets.Add(bet.BetId, bet);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private double calculateMoneyToBet(double odds)
        {
            //Ausstehende Setzbetrag holen
            double bettedMoney = 0.0;
            log("Calculating Bet Amount");
            for (int i = 0; i < _matchedBets.Values.Count; i++)            
            {
                logBetDetails(_matchedBets.Values[i]);
                if (_settings.BetType)
                {
                    bettedMoney += _matchedBets.Values[i].MatchedSize;
                    logBetAmount(String.Format("Betted Amount {0}", bettedMoney));
                }
                else
                {
                    bettedMoney += (_matchedBets.Values[i].MatchedSize * _matchedBets.Values[i].Price - _matchedBets.Values[i].MatchedSize);
                    logBetAmount(String.Format("Betted Amount {0}", bettedMoney));
                }
            }

            for (int i = 0; i < _openBets.Values.Count; i++)            
            {
                logBetDetails(_openBets.Values[i]);
                if (_settings.BetType)
                {
                    bettedMoney += _openBets.Values[i].RemainingSize;
                    logBetAmount(String.Format("Betted Amount {0}", bettedMoney));
                }
                else
                {
                    bettedMoney += (_openBets.Values[i].RemainingSize * _openBets.Values[i].Price - _openBets.Values[i].RemainingSize);
                    logBetAmount(String.Format("Betted Amount {0}", bettedMoney));
                }
            }

            bettedMoney = _settings.BetAmount - bettedMoney;
            logBetAmount(String.Format("Amount still needed to be bet {0}", bettedMoney));
            // Muss nochmal gewettet werden
            if (bettedMoney > 0)
            {
                log("Calculate money to be bet relativly to the configuration");
                bettedMoney = getMoney(odds, bettedMoney);
                logBetAmount(String.Format("Money needed to be betted {0}", bettedMoney));
            }

            return bettedMoney;
        }

        private void matchedBetsWatcher(Object stateInfo)
        {
            lock (_syncLock)
            {
                try
                {
                    log("Observing Matched Bets");

                    if (_state == TRADESTATE.COMPLETED)
                    {
                        log("Trade already completed");
                        return;
                    }

                    if (nonRunnerObservation(_matchedBets))
                    {
                        return;
                    }

                    List<SXALBet> listBetsToRemove = new List<SXALBet>();

                    for (int i = 0; i < _matchedBets.Values.Count; i++)                    
                    {
                        SXALBet newBet = null;
                        if (checkBetMove(_matchedBets.Values[i], out newBet))
                        {
                            listBetsToRemove.Add(_matchedBets.Values[i]);
                        }


                        foreach (SXALBet b2 in listBetsToRemove)
                        {
                            _matchedBets.Remove(b2.BetId);
                            //Benachrichtigung schicken.
                        }
                    }

                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private bool nonRunnerObservation(SXALSortedBetList list)
        {
            log("Nonrunner Observation");
            List<SXALBet> listBetsToRemove = new List<SXALBet>();
            ICollection ic = list;
            //Pferd ist Non-runner?
            if (isHorseNonRunner() && _state != TRADESTATE.NONRUNNER)
            {
                log("Horse is now a non runner");
                // Benachrichtigungen schicken
                log(String.Format("Changing State from {0} to {1}", _state.ToString(), TRADESTATE.NONRUNNER.ToString()));
                _state = TRADESTATE.NONRUNNER;

                // Überprüfe, ob sich Status der Wette geändert hat, z.B. weil Pferd jetzt Non-Runner
                // oder Rennen beendet
                foreach (SXALBet b in list.Values)
                {
                    log(String.Format("Nonrunner Observation: Checking Bet {0}", b.BetId));
                    SXALBet newBet = null;
                    if (checkBetMove(b, out newBet))
                    {
                        listBetsToRemove.Add(b);
                    }
                }

                foreach (SXALBet b in listBetsToRemove)
                {
                    lock (ic.SyncRoot)
                    {
                        list.Remove(b.BetId);
                    }
                }

                return true;
            }
            else if (_state == TRADESTATE.NONRUNNER) return true;

            return false;
        }

        private bool checkBetCancel(SXALBet bet, SXALMarketPrices prices)
        {
            log(String.Format("Check if open bet {0} needs to be canceled", bet.BetId));

            //marktpreis für Selektion filtern
            SXALRunnerPrices runnerPrice = null;
            foreach (SXALRunnerPrices r in prices.RunnerPrices)
            {
                if (r.SelectionId == _selectionId)
                {
                    runnerPrice = r;
                    break;
                }
            }

            if (runnerPrice == null)
            {
                //loggen
                log(String.Format("Could not find odds for bet {0} selection {1}. Returning (Not Cancel)", bet.BetId, bet.SelectionId));
                return false;
            }

            // offene Wette + Final Time + Quote unterhalb von Maximum Odds
           // if (_settings.UseFinalTime && _dts.Subtract(DateTime.Now).TotalMinutes <= _settings.FinalTime &&
           //     _dts.Subtract(DateTime.Now).TotalMinutes > 0 && bet.Price < _maxOdds)
            // offene Wette + Final Time + Quote unterhalb von Maximum Odds
            if (_settings.UseFinalTime && _dts.Subtract(DateTime.Now).TotalMinutes <= _settings.FinalTime &&
                _dts.Subtract(DateTime.Now).TotalMinutes > 0 && (bet.Price < _settings.MaximumLayOdd ||
                (bet.Price < runnerPrice.BestPricesToBack[0].Price && runnerPrice.BestPricesToBack[0].Price <= _settings.MaximumLayOdd)))
            {
                log(String.Format("Final Time! Bet {0} needs to be canceled", bet.BetId));
                return true;
            }

           

            // Gap Tradin + Wir sind noch auf die Beste Quote und zwar als einziger Anbieter und zwischen der Besten und Zweitbesten Quoten existiert
            // eine Lücke
            if (_settings.UseStepTrading && runnerPrice.BestPricesToBack != null && runnerPrice.BestPricesToBack.Length >= 2 &&
                runnerPrice.BestPricesToBack[0].Price == bet.Price && runnerPrice.BestPricesToBack[0].Stake <= bet.RemainingSize)
            {
                double diff = runnerPrice.BestPricesToBack[0].Price - runnerPrice.BestPricesToBack[1].Price;
                decimal tick = SXALKom.Instance.getValidOddIncrement((decimal)runnerPrice.BestPricesToBack[0].Price);//SXTools.getValidOddIncrement((decimal)runnerPrice.BestPricesToBack[0].Price);

                double diffTick = diff / (double)tick;
                diffTick = Math.Round(diffTick, 2);
                if (diffTick > _settings.StepTradingGap)
                {
                    log(String.Format("Using Gap Trading and a gap was detected between the Back Odds. First Back Price {1}. Second Back Price {2}. Bet {0} needs to be canceled",
                    bet.BetId, bet.Price, runnerPrice.BestPricesToBack[1].Price));

                    // Den index 0 entfernen
                    runnerPrice.removeBackPrice(0);

                    return true;
                }
            }
            // Gap Trading + Neue Quoten über unsere Wettquoten, aber noch unterhalb/gleich von Maximum Qdds
            if (_settings.UseStepTrading && runnerPrice.BestPricesToBack != null && runnerPrice.BestPricesToBack.Length > 0 &&
                runnerPrice.BestPricesToBack[0].Price > bet.Price && runnerPrice.BestPricesToBack[0].Price <= _maxOdds)
            {
                log(String.Format("Using Gap Trading and a gap was detected. Bet {0} Price {1}. Back Price {2}. Bet {0} needs to be canceled",
                    bet.BetId, bet.Price, runnerPrice.BestPricesToBack[0].Price));
                return true;
            }

            log(String.Format("Bet {0} does not need to be canceled", bet.BetId));
            return false;
        }

        private bool checkBetMove(SXALBet oldBet, out SXALBet newBet)
        {
            log(String.Format("Check if bet {0} needs to be moved into an other list", oldBet.BetId));
            bool bRet = false;
            log(String.Format("Reread details for bet {0}", oldBet.BetId));
            newBet = SXALKom.Instance.getBetDetail(oldBet.BetId);
            if (newBet != null && newBet.BetStatus != oldBet.BetStatus)
            {
                log(String.Format("Status of Bet {0} has changed from {1} to {2}. Move to fitting list", newBet.BetId, oldBet.BetStatus, newBet.BetStatus));
                bRet = true;
                switch (newBet.BetStatus)
                {
                    case SXALBetStatusEnum.M:
                        if (_matchedBets.ContainsKey(newBet.BetId))
                        {
                            _matchedBets[newBet.BetId] = newBet;
                        }
                        else
                        {
                            _matchedBets.Add(newBet.BetId, newBet);
                        }
                        break;
                    case SXALBetStatusEnum.MU:
                        if (_matchedBets.ContainsKey(newBet.BetId))
                        {
                            _matchedBets[newBet.BetId] = newBet;
                        }
                        else
                        {
                            _matchedBets.Add(newBet.BetId, newBet);
                        }
                        if (_openBets.ContainsKey(newBet.BetId))
                        {
                            _openBets[newBet.BetId] = newBet;
                        }
                        else
                        {
                            _openBets.Add(newBet.BetId, newBet);
                        }
                        break;
                    case SXALBetStatusEnum.S:
                        if (_settledBets.ContainsKey(newBet.BetId))
                        {
                            _settledBets[newBet.BetId] = newBet;
                        }
                        else
                        {
                            _settledBets.Add(newBet.BetId, newBet);
                        }
                        break;
                    case SXALBetStatusEnum.U:
                        if (_openBets.ContainsKey(newBet.BetId))
                        {
                            _openBets[newBet.BetId] = newBet;
                        }
                        else
                        {
                            _openBets.Add(newBet.BetId, newBet);
                        }
                        break;
                    default:
                        if (_invalidBets.ContainsKey(newBet.BetId))
                        {
                            _invalidBets[newBet.BetId] = newBet;
                        }
                        else
                        {
                            _invalidBets.Add(newBet.BetId, newBet);
                        }
                        break;
                }
            }
            else if (newBet != null)
            {
                ;
            }
            else
            {
                log(String.Format("Could not reread Bet {0}. Leaving", oldBet.BetId));
            }
            return bRet;
        }

        public static TLLTrade CreateTrade(SXALBet[] bets, TLLConfigurationRW settings)
        {
            return new TLLTrade(bets, settings);
        }

        public static TLLTrade CreateTrade(SXALMarket market, long selectionId, string horseName, double maxOdds)
        {
            return new TLLTrade(market, selectionId, horseName, maxOdds);
        }

        private SXALBet placeBet(double odds, double money)
        {
            try
            {
                return SXALKom.Instance.placeLayBet(_market, _selectionId, 0, _settings.KeepInRunning, odds, money);
            }
            catch (SXALNoBetBelowMinAllowedException)
            {
                throw new TLLBelowMinStakeException(_race, _horse, _dts, money, SXALKom.Instance.getExchangeName());
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            return null;
        }

        // Überprüft anhand der Konfiguration, ob die Vorraussetzung für die Wettabgabe erfüllt sind
        private bool checkBetPrerequisites()
        {
            log("Check Prerequisites for Trading");
            // Innerhalb zulässigen Intervall für Wettabgabe?
            if (_dts.Subtract(DateTime.Now).TotalMinutes > _settings.BeginTime)
            {
                log(String.Format("Time until Start of Race is bigger than customized start time of {0}", _settings.BeginTime));
                return false;
            }
            log("All Prerequisites passed.");
            return true;
        }

        private bool isHorseNonRunner()
        {
            log(String.Format("Check if horse {0} is a non runner", _horse));
            foreach (SXALSelection s in _market.Selections)
            {
                if (_selectionId == s.Id || _horse.Equals(s.Name, StringComparison.InvariantCultureIgnoreCase))
                    return s.IsNonStarter;
            }

            //Pferd wurde nicht gefunden ==> Ausnahme
            throw new TLLHorseNotFoundExcpetion(_horse, _race, _market.Id);
        }

        private bool checkNonRunners()
        {
            log("Check number of non runners if customized");
            // Ist eine Nonrunner-Beobachtung erwünscht?
            if (_settings.NonStarterObservation)
            {
                log("Non Runner Count is Customized");
                int countNR = 0;
                //Hole aktuelle Informationen zu den Starten
                SXALSelection[] selections = SXALKom.Instance.getSelections(_market);
                foreach (SXALSelection s in selections)
                {
                    if (s.IsNonStarter)
                        countNR++;
                }

                log(String.Format("Number of Non Runners is {0}", countNR));

                //Abhängig vom Starterfeld  auswerten
                if (selections.Length <= TLLConfigurationRW.FIELD1SIZE && countNR >= _settings.NonRunnerCount1)
                {
                    log(String.Format("Number of Non Runners {0} is bigger as bigger than allowed number {1} for field size 1", countNR, _settings.NonRunnerCount1));
                    return true;
                }

                if (selections.Length <= TLLConfigurationRW.FIELD2SIZE && countNR >= _settings.NonRunnerCount2)
                {
                    log(String.Format("Number of Non Runners {0} is bigger as bigger than allowed number {1} for field size 2", countNR, _settings.NonRunnerCount2));
                    return true;
                }

                if (selections.Length <= TLLConfigurationRW.FIELD3SIZE && countNR >= _settings.NonRunnerCount3)
                {
                    log(String.Format("Number of Non Runners {0} is bigger as bigger than allowed number {1} for field size 3", countNR, _settings.NonRunnerCount3));
                    return true;
                }

                if (selections.Length <= TLLConfigurationRW.FIELD4SIZE && countNR >= _settings.NonRunnerCount4)
                {
                    log(String.Format("Number of Non Runners {0} is bigger as bigger than allowed number {1} for field size 4", countNR, _settings.NonRunnerCount4));
                    return true;
                }

                if (selections.Length > TLLConfigurationRW.FIELD4SIZE && countNR >= _settings.NonRunnerCount5)
                {
                    log(String.Format("Number of Non Runners {0} is bigger as bigger than allowed number {1} for field size 5", countNR, _settings.NonRunnerCount5));
                    return true;
                }
            }
            else
            {
                log("No Non Runner Count customized.");
            }
            return false;
        }

        private double getMoney(double odds)
        {
            return getMoney(odds, _settings.BetAmount);
        }

        private double getMoney(double odds, double money)
        {
            log("Get Money According to Customizing");
            if (_settings.BetType)
            {
                log("Money is 'Money to Bet'");
                logBetAmount(String.Format("Money to Bet is {0}", money));
                return money;
            }
            else
            {
                log("Money is 'Money as Risk'");
                double betAmount = money / (odds - 1);
                logBetAmount(String.Format("Money to Bet is {0}", Math.Round(betAmount, 2)));
                return Math.Round(betAmount, 2);
            }
        }

        private double getOdds(SXALMarketPrices prices)
        {
            log("Getting Odds");
            double odds = 0.0;
            SXALRunnerPrices runnerPrice = null;
            foreach (SXALRunnerPrices r in prices.RunnerPrices)
            {
                if (r.SelectionId == _selectionId)
                {
                    runnerPrice = r;
                    break;
                }
            }

            if (runnerPrice == null)
            {
                log("Could not find odds. Throwing an Error");
                throw new TLLNoRunnerPricesException(_race, _horse, _dts);
            }

            // Erste Option: benutzen wir Final Time und es wir sind in diesen Intervall?
            if (_settings.UseFinalTime && _dts.Subtract(DateTime.Now).TotalMinutes <= _settings.FinalTime)
            {
                log(String.Format("Final Time is customized and reached. Final Time is {0}", _settings.FinalTime));
                //Wir schauen uns die Lay-Quoten an
                //Keine LayQuoten vorhanden
                if (runnerPrice.BestPricesToLay == null || runnerPrice.BestPricesToLay.Length == 0)
                {
                    log(String.Format("There are no lay odds. Setting Odds to Maximum Odds {0}", _maxOdds));
                    odds = _maxOdds;
                }
                //Layquoten vorhanden
                else if (runnerPrice.BestPricesToLay[0].Price > _maxOdds)
                {
                    log(String.Format("Lay Odds found but Odds {0} are bigger than Maximum Odds {1}. Setting Odds to Maximum Odds {1}",
                        runnerPrice.BestPricesToLay[0].Price, _maxOdds));
                    odds = _maxOdds;
                }
                else
                {
                    log(String.Format("Setting Odds to {0}", runnerPrice.BestPricesToLay[0].Price));
                    odds = runnerPrice.BestPricesToLay[0].Price;
                }
            }
            //2. Option Keine Quoten vorhanden
            else if ((runnerPrice.BestPricesToBack == null && runnerPrice.BestPricesToLay == null) || (runnerPrice.BestPricesToBack == null && runnerPrice.BestPricesToLay.Length == 0)
                || (runnerPrice.BestPricesToBack.Length == 0 && runnerPrice.BestPricesToLay == null) || (runnerPrice.BestPricesToBack.Length == 0 && runnerPrice.BestPricesToLay.Length == 0))
            {
                log(String.Format("Neither Back nor Lay Odds given. Setting Odds to half Maximum Odds {0}", _maxOdds / 2));
                odds = _maxOdds / 2;
            }
            //3. Keine Lay-Quoten vorhanden
            else if (runnerPrice.BestPricesToLay == null || runnerPrice.BestPricesToLay.Length == 0)
            {
                log("No Lay Odds given");
                if (runnerPrice.BestPricesToBack[0].Price >= _maxOdds)
                {
                    log(String.Format("Back Odds greater than Maximum Odds. Setting Odds to Maximum Odds", _maxOdds));
                    odds = _maxOdds;
                }
                else
                {
                    decimal incr = SXALKom.Instance.getValidOddIncrement((decimal)runnerPrice.BestPricesToBack[0].Price);
                    odds = (double)(((decimal)runnerPrice.BestPricesToBack[0].Price) + incr);
                    odds = (double)SXALKom.Instance.validateOdd((decimal)odds);
                    log(String.Format("Back Odds smaller than Maximum Odds. Setting Odds to Back Odds + 1 Tick {0}", odds));
                }
            }
            //4. Keine Back-Quoten vorhanden
            else if (runnerPrice.BestPricesToBack == null || runnerPrice.BestPricesToBack.Length == 0)
            {
                log("No Back Odds given");
                if (runnerPrice.BestPricesToLay[0].Price >= _maxOdds)
                {
                    log(String.Format("Lay Odds greater than Maximum Odds. Setting Odds to Maximum Odds", _maxOdds));
                    odds = _maxOdds;
                }
                else
                {
                    decimal decr = SXALKom.Instance.getValidOddIncrement((decimal)runnerPrice.BestPricesToLay[0].Price);
                    odds = (double)(((decimal)runnerPrice.BestPricesToLay[0].Price) - decr);
                    odds = (double)SXALKom.Instance.validateOdd((decimal)odds);
                    log(String.Format("Lay Odds smaller than Maximum Odds. Setting Odds to Back Odds - 1 Tick {0}", odds));
                }
            }
            //5. Sowohl Lay als auch Back-Quoten sind vorhanden
            else
            {
                log("Back and Lay odds Given");
                //Unterschied ermitteln:
                double diff = runnerPrice.BestPricesToLay[0].Price - runnerPrice.BestPricesToBack[0].Price;
                decimal tick = SXALKom.Instance.getValidOddIncrement((decimal)runnerPrice.BestPricesToBack[0].Price);

                double diffTick = diff / (double)tick;
                // Wir nutzen Intervall-Trading und Back-Quote ist unterhalb maximalquote
                if (_settings.UseStepTrading && diffTick >= _settings.StepTradingGap && runnerPrice.BestPricesToBack[0].Price < _maxOdds)
                {
                    log(String.Format("Gap Trading allowed and Gap between Back and Lay is {0}. Customized Gap is {1}. Back Odds smaller than Maximum Odds {2}",
                        diffTick, _settings.StepTradingGap, _maxOdds));
                    decimal incr = SXALKom.Instance.getValidOddIncrement((decimal)runnerPrice.BestPricesToBack[0].Price);
                    odds = (double)(((decimal)runnerPrice.BestPricesToBack[0].Price) + incr);
                    odds = (double)SXALKom.Instance.validateOdd((decimal)odds);
                    log(String.Format("Back Odds smaller than Maximum Odds. Setting Odds to Back Odds + 1 Tick {0}", odds));
                }
                else
                {
                    //Lay-Quote ist überhalb Maximalquote
                    if (runnerPrice.BestPricesToLay[0].Price >= _maxOdds)
                    {
                        odds = _maxOdds;
                        log(String.Format("Lay Odds greater or equal than Maximum Odds. Setting Odds to Maximum Odds", _maxOdds));
                    }
                    else
                    {
                        //Lay-Quote ist unterhabl Maximalquote
                        odds = runnerPrice.BestPricesToLay[0].Price;
                        log(String.Format("Lay Odds smaller than Maximum Odds. Setting Odds to Maximum Odds", odds));
                    }
                }
            }
            return odds;
        }

        public void log(string message)
        {
            try
            {
                String id = String.Format("{0}_{1}_{2}", this._race, this._dts.ToString(), this._horse);
                TradeLog.Instance.writeLog(id, "The Low Lay", "Trader", String.Format("ID {0}: {1}", id, message));
            }
            catch { }
        }

        public void logBetAmount(string message)
        {
            try
            {
                String id = String.Format("{0}_{1}_{2}", this._race, this._dts.ToString(), this._horse);
                TradeLog.Instance.writeBetAmountLog(id, "The Low Lay", "Trader", String.Format("ID {0}: {1}", id, message));
            }
            catch { }
        }

        public void logBetDetails(SXALBet b)
        {
            try
            {
                String id = String.Format("{0}_{1}_{2}", this._race, this._dts.ToString(), this._horse);
                TradeLog.Instance.writeLog(id, "The Low Lay", "Trader", String.Format("Bet ID: {0};Bet Type: {1}; Bet Status: {2}", b.BetId, b.BetType, b.BetStatus));
               
                TradeLog.Instance.writeLog(id, "The Low Lay", "Trader", String.Format("Price: {0}; Average Price: {1}",b.Price,b.AvgPrice));
                TradeLog.Instance.writeLog(id, "The Low Lay", "Trader", String.Format("Requested Size: {2}; Matched Size: {0}; Remaining Size: {1}", b.MatchedSize, b.RemainingSize, b.RequestedSize));
                TradeLog.Instance.writeLog(id, "The Low Lay", "Trader", String.Format("Placed Date: {0}; Matched Date: {1}; Settled Date: {2}; Cancelled Date: {3}", b.PlacedDate, b.MatchedDate, b.SettledDate, b.CancelledDate));
                TradeLog.Instance.writeLog(id, "The Low Lay", "Trader", String.Format("Profit & Loss: {0}", b.ProfitAndLoss));

                if (b.Matches == null || b.Matches.Length == 0)
                {
                    TradeLog.Instance.writeLog(id, "The Low Lay", "Trader", "No Detailed Matches");
                }
                else
                {
                    foreach (SXALMatch m in b.Matches)
                    {
                        TradeLog.Instance.writeLog(id, "The Low Lay", "Trader", String.Format("Size Match: {0}", m.SizeMatched));
                    }
                }

            }
            catch { }
        }

        private String getSyncLockName()
        {
            return String.Format("syncLock_{0}_{1}_{2}", this._race, this._dts.ToString(), this._horse);
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DebugWriter.Instance.WriteMessage("TLLTrade", "Disposing");
                if (disposing)
                {
                    if (_settings != null)
                    {
                        _settings.Dispose();
                        _settings = null;
                    }

                    SXMinutePulse.Instance.Pulse -= Instance_Pulse;
                    SX5MinutePulse.Instance.Pulse -= Instance_5Pulse;
                }
            }
        }
        #endregion

        #region IBFTSCommon Members
        public event EventHandler<muk.eventargs.SXExceptionMessageEventArgs> ExceptionMessageEvent;

        public event EventHandler<muk.eventargs.SXWMessageEventArgs> MessageEvent;
        #endregion
    }

    class TLLBelowMinStakeException : Exception
    {
        private TLLBelowMinStakeException() : base() { }

        public TLLBelowMinStakeException(String race, String horse, DateTime dts, double amount, String sportExchange)
            : base(String.Format(TheLowLay.strBelowMinStake, horse, race, dts, amount, sportExchange))
        {
        }
    }

    class TLLNoRunnerPricesException : Exception
    {
        private TLLNoRunnerPricesException() : base() { }

        public TLLNoRunnerPricesException(String race, String horse, DateTime dts)
            : base(String.Format(TheLowLay.strNoPriceInformation, horse, race, dts))
        {
        }
    }

    class TLLSelectionIDMismatchException : Exception
    {
        private TLLSelectionIDMismatchException()
            : base()
        {
        }

        public TLLSelectionIDMismatchException(long selectionId1, long selectionId2)
            : base(String.Format(TheLowLay.strSelectionIdMismatch, selectionId1, selectionId2))
        {

        }
    }

    class TLLHorseNotFoundExcpetion : Exception
    {
        private TLLHorseNotFoundExcpetion() : base() { }


        public TLLHorseNotFoundExcpetion(String horse, String race, long marketId) : base(String.Format(TheLowLay.strHorseNotFound, horse, race, marketId)) { }
    }

    class TLLInvalidOddsException : Exception
    {
        private TLLInvalidOddsException() : base() { }

        public TLLInvalidOddsException(String horse, String race, DateTime dts) : base(String.Format(TheLowLay.strInvalidOdds, horse, race, dts)) { }
    }

    class TLLTradeSettledEventArgs : EventArgs
    {
    }
}
