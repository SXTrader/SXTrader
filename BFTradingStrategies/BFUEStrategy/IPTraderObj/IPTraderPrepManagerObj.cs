using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXFastBet;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj
{
    class IPTraderPrepBetAddedEventArgs : EventArgs
    {
        private long _marketId;

        public long MarketId
        {
            get { return _marketId; }
        }

        public IPTraderPrepBetAddedEventArgs(long marketId)
        {
            _marketId = marketId;
        }
    }

    class IPTraderPrepManagerObj
    {
        private BFUEFBIPTraderConfigList _configList;
        private long _marketId;
        private long _selectionId;
        private IScore _score;
        private BFUEWatcher _watcher;
        private bool _recheck = false;
        private Thread _runner;

        private SXALBet _openBet = null;


        public event EventHandler<IPTraderPrepBetAddedEventArgs> BetAddedEvent;

        public BFUEFBIPTraderConfigList ConfigList
        {
            get
            {
                return _configList;
            }
            set
            {
                _configList = value;
            }
        }

        public long MarketId
        {
            get
            {
                return _marketId;
            }
        }

        public IPTraderPrepManagerObj(long marketId, IScore score, BFUEWatcher watcher)
        {
            try
            {
                _score = score;
                _score.IncreaseRef();
                _score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(_score_BackGoalEvent);
                _score.GameEndedEvent += new EventHandler<GameEndedEventArgs>(_score_GameEndedEvent);
                _score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(_score_PlaytimeTickEvent);
                _score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(_score_RaiseGoalEvent);

                _watcher = watcher;

                _marketId = marketId;

                log("Constructing a new Inplay Trader Prepareing Object");
                if (_score != null)
                {
                    log("Liveticker is linked");
                }
                _runner = new Thread(runner);
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        ~IPTraderPrepManagerObj()
        {
            log("Deconstructing");
            _score.DecreaseRef();
            _score = null;
        }

        private void runner()
        {
            //TimeSpan span = new TimeSpan(0, 0, 30);
            log("Check Rules for a match");
            try
            {
                    double odd = 0.0;
                    try
                    {
                        foreach (BFUEFBIPTraderConfigElement element in _configList)
                        {
                            if (!check(element, out odd))
                            {
                                continue;
                            }

                            log("All checkst passed: Placing a lay Bet");
                            // Tradingbedingungen wurden erfüllt, also los
                            if (placeBet(odd, element.TradeConfig))
                            {
                                log("Placing of new Lay Bet succeeded");
                                log(String.Format("Setting Rule {0} to already executed", element.ElementNumber));

                                EventHandler<IPTraderPrepBetAddedEventArgs> handler = BetAddedEvent;
                                if (handler != null)
                                {
                                    log("Notifying Listeners: New Bet Added");
                                    handler(this, new IPTraderPrepBetAddedEventArgs(this.MarketId));
                                }

                                element.AlreadyTraded = true;
                                break;
                            }
                            else
                            {
                                log("placing of new Lay Bet failed");
                            }
                        }
                    }
                    catch (IPTNoMarketPricesException) { log("Received a No Market Prices Exception: Stopping checking Rules until next iteration"); }
                    catch (IPTMarketSupendedException) { log("Received a Market Suspended Exception: Stopping checking Rules until next iteration"); }
                    catch (IPTMarketNotEnoughOddsException) { log("Received a Market Not Enought Selections Exception: Stopping checking Rules until next iteration"); }
                    catch (IPTMarketNoLayOddsException) { log("Received a Market No Lay Odds Exception: Stopping checking Rules until next iteration"); }
                    catch (SXFastBetBelowMinStackException)
                    {
                        log("Received a Fast Bet Below Min Stack Exception: Stopping checking Rules until next iteration");
                    }
                    catch (SXFastBetInsufficentFoundsExcpetion)
                    {
                        log("Received a Fast Bet Insufficent Founds Exception: Stopping checking Rules until next iteration");
                    }
                    catch (Exception exc)
                    {
                        log("Received an unspecified Exception. Please check ExceptionOutput.txt");
                        ExceptionWriter.Instance.WriteException(exc);
                    }
            }
            catch (ThreadAbortException tae)
            {
                log("Retrieved a Thread Abort Exception");
                throw tae;
            }
        }

        private bool placeBet(double odd, LTDConfigurationRW config)
        {

            if (_openBet != null)
            {
                _openBet = SXALKom.Instance.getBetDetail(_openBet.BetId);

                if (_openBet.BetStatus == SXALBetStatusEnum.M || _openBet.BetStatus == SXALBetStatusEnum.MU)
                {
                    log("Status of unmatched bet is matched. Calling Watcher with new bet");
                    _watcher.setNewBet(_openBet, config, _score);
                    _openBet = null;
                    return true;
                }
                else if (_openBet.BetStatus == SXALBetStatusEnum.U)
                {
                    log("Status of unmatched bet is still unmatched. Retrying");
                    return false;
                }

                log(String.Format("Stauts of former unmatched ist no {0}. Trying to place new bet", _openBet.BetStatus));
                _openBet = null;
            }

            //LTDConfigurationRW config = new LTDConfigurationRW();
            log("Reading Fast bet configuration for succeeded rule");
            SXFastBetSettings settings = new SXFastBetSettings();
            settings.CancelUnmatchedFlag = config.FastBetUnmatchedCancel;
            settings.FixedAmountFlag = config.FastBetFixedAmount;
            settings.FixedAmountValue = config.FastBetFixedAmountValue;
            settings.PercentAmounValue = config.FastBetPercentAmountValue;
            settings.TotalAmountFlag = config.FastBetPercentTotalAmount;
            settings.UnmatchedWaitSeconds = config.FastBetUnmatchedWaitSeconds;
            double moneyToBet = SXFastBetMoneyGetter.getMoney(settings);
            SXALMarket market = SXALSoccerMarketManager.Instance.InPlayMarkets[_marketId];
            

            

            //Nochmal überprüfen, Torstand
            log("Recheck Score");
            if (_score.getScoreState() == SCORESTATE.undraw)
            {
                log(String.Format("Score is {0} and therefore undraw: Leaving", _score.getScore()));
                return false;
            }

            log(String.Format("Placing a Lay Bet for the odd of {0}", odd));
            SXALBet bet = null;
            try
            {
                try
                {
                    bet = SXALKom.Instance.placeLayBet(_marketId, _selectionId, odd, moneyToBet);
                }
                catch (SXALNoBetBelowMinAllowedException)
                {
                    log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                    bet = SXALKom.Instance.placeLayBet(_marketId, _selectionId, odd, SXALKom.Instance.MinStake);
                }
                catch (SXALInsufficientFundsException)
                {
                    log("Not enough money on betting account. Leaving!");
                    return false;
                }
            }
            catch (SXALBetInProgressException bipe)
            {
                log(String.Format("Received a BetInProgressException with betId {0}. Rechecking Betstatus", bipe.BetId));
                //Betfair Zeit geben den Markt abzuschliessen
                Thread.Sleep(3000);
                SXALMUBet[] muBets = null;
                if (bipe.BetId == 0)
                    muBets = SXALKom.Instance.getBetsMU(this.MarketId);
                else
                    muBets = SXALKom.Instance.getBetMU(bipe.BetId);
                bet = recheckMUBets(muBets);

            }
            

            if (bet != null)
            {
                log("A bet was successfully placed on market");
                //Wettzustand
                if (bet.BetStatus == SXALBetStatusEnum.M)
                {
                    log("Betstatus is matched. Calling Watcher with new bet");
                    _watcher.setNewBet(bet, config, _score);
                    return true;
                }
                if (bet.BetStatus == SXALBetStatusEnum.MU)
                {
                    log("Betstatus is partly matched. Calling Watcher with new bet");
                    _watcher.setNewBet(bet, config, _score);
                    //TODO: was mit dem nicht erfüllten teil machen?
                    return true;
                }
                if (bet.BetStatus == SXALBetStatusEnum.U)
                {
                    try
                    {
                        log("Betstatus is unmatched. Trying to cancel bet");
                        if (!SXALKom.Instance.cancelBet(bet.BetId))
                        {
                            log("Canceling of Bet failed");
                            Thread.Sleep(5000);
                            bet = SXALKom.Instance.getBetDetail(bet.BetId);
                            if (bet.BetStatus == SXALBetStatusEnum.M)
                            {
                                log("Status of uncanceld bet is matched. Calling Watcher with new bet");
                                _watcher.setNewBet(bet, config, _score);
                                return true;
                            }
                            if (bet.BetStatus == SXALBetStatusEnum.MU)
                            {
                                log("Status of uncanceld Bet is partly matched. Calling Watcher with new bet");
                                _watcher.setNewBet(bet, config, _score);
                                //TODO: was mit dem nicht erfüllten teil machen?
                                return true;
                            }
                            if (bet.BetStatus == SXALBetStatusEnum.C)
                            {
                                log("Status of uncanceld Bet is canceled. Leaving");
                                //Wetter bereits anderweilig storniert
                                return false;
                            }
                            if (bet.BetStatus == SXALBetStatusEnum.U)
                            {
                                log("Status is still unmatched! Leaving");
                                _openBet = bet;
                                return false;
                            }
                            log(String.Format("Status of uncanceld bet is {0}. Leaving", bet.BetStatus.ToString()));
                            

                        }
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        log(String.Format("Received an exception with message {0}", exc.Message));
                    }
                    return false;
                }
            }
            else
            {
                Thread.Sleep(3000);

                SXALMUBet[] muBets = SXALKom.Instance.getBetsMU(this.MarketId); ;
                
                bet = recheckMUBets(muBets);

                if (bet == null)
                {
                    log("No bet was placed: Leaving!");
                }
                else
                {
                    // TODO: Ganz schlechter Stil! Codewiederholung! Bessere Lösung finden
                    log("A bet was successfully placed on market");
                    //Wettzustand
                    if (bet.BetStatus == SXALBetStatusEnum.M)
                    {
                        log("Betstatus is matched. Calling Watcher with new bet");
                        _watcher.setNewBet(bet, config, _score);
                        return true;
                    }
                    if (bet.BetStatus == SXALBetStatusEnum.MU)
                    {
                        log("Betstatus is partly matched. Calling Watcher with new bet");
                        _watcher.setNewBet(bet, config, _score);
                        //TODO: was mit dem nicht erfüllten teil machen?
                        return true;
                    }
                    if (bet.BetStatus == SXALBetStatusEnum.U)
                    {
                        log("Betstatus is unmatched. Trying to cancel bet");
                        try
                        {
                            if (!SXALKom.Instance.cancelBet(bet.BetId))
                            {
                                log("Canceling of Bet failed");
                                bet = SXALKom.Instance.getBetDetail(bet.BetId);
                                if (bet.BetStatus == SXALBetStatusEnum.M)
                                {
                                    log("Status of uncanceld bet is matched. Calling Watcher with new bet");
                                    _watcher.setNewBet(bet, config, _score);
                                    return true;
                                }
                                if (bet.BetStatus == SXALBetStatusEnum.MU)
                                {
                                    log("Status of uncanceld Bet is partly matched. Calling Watcher with new bet");
                                    _watcher.setNewBet(bet, config, _score);
                                    //TODO: was mit dem nicht erfüllten teil machen?
                                    return true;
                                }
                                if (bet.BetStatus == SXALBetStatusEnum.C)
                                {
                                    log("Status of uncanceld Bet is canceled. Leaving");
                                    //Wetter bereits anderweilig storniert
                                    return false;
                                }

                                log(String.Format("Status of uncanceld bet is {0}. Leaving", bet.BetStatus.ToString()));
                                //TODO: Und nu? Wette konnte aus irgendeinen Grund nicht storniert werden
                            }
                        }
                        catch (Exception exc)
                        {
                            ExceptionWriter.Instance.WriteException(exc);
                            log(String.Format("Received an exception with message {0}", exc.Message));
                        }
                        return false;
                    }
                }
            }

            return false;
        }

        private SXALBet recheckMUBets(SXALMUBet[] muBets)
        {
            SXALBet bet = null;
            try
            {
                if (muBets != null)
                {
                    foreach (SXALMUBet muBet in muBets)
                    {
                        //TODO: SelectionId für Drawmarkt noch in Konstante
                        if (muBet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, muBet.MarketId))
                            continue;

                        //Uns interessieren hier nur Lay-Wetten
                        if (muBet.BetType == SXALBetTypeEnum.B)
                            continue;

                        if (this._watcher.BetSet[MarketId] != null)
                        {
                            // Uns interessieren hier nur neue Wetten
                            if (((BFUEStrategy)this._watcher.BetSet[MarketId]).Lay.Bets[muBet.BetId] != null)
                                continue;
                        }

                        bet = SXALKom.Instance.getBetDetail(muBet.BetId);

                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }

            return bet;
        }

        private bool check(BFUEFBIPTraderConfigElement element, out double odd)
        {
            try
            {
                log(String.Format("Checking rule {0} ", element.ElementNumber));
                odd = 0.0;
                // Wurde bereits gehandelt
                if (element.AlreadyTraded)
                {
                    log("Rule has already been executed. Leaving!");
                    return false;
                }
                // Innerhalb der Zeit?
                if (!(_score.Playtime >= element.LoPlaytime && _score.Playtime <= element.HiPlayTime))
                {
                    log(String.Format("The playtime {0} is not between the configured interval {1} and {2}. Leaving", _score.Playtime, element.LoPlaytime, element.HiPlayTime));
                    return false;
                }
                // Spielstand überprüfen
                String score = _score.getScore();
                log(String.Format("Score is {0}", score));
                if (_score.getScoreState() == SCORESTATE.undraw)
                {
                    log("Score State is undraw. Leaving");
                    return false;
                }
                bool bFound = false;
                IPTScores configScore = IPTScores.UNASSIGNED;
                switch (score)
                {
                    case "0 - 0":
                        configScore = IPTScores.ZEROZERO;
                        break;
                    case "1 - 1":
                        configScore = IPTScores.ONEONE;
                        break;
                    case "2 - 2":
                        configScore = IPTScores.TWOTWO;
                        break;
                    case "3 - 3":
                        configScore = IPTScores.THREETHREE;
                        break;
                }
                foreach (IPTScores s in element.Scores)
                {
                    if (s == configScore)
                    {
                        log(String.Format("Score {0} is withing the configured scores", score));
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    log(String.Format("Score {0} not within the configured values. Leaving", score));
                    return false;
                }

                //Ggf. Rote Karten überprüfen
                if (element.RedCardWatch)
                {
                    log("Red Card observation is activated");
                    log(String.Format("Configured Red Card Rule is {0}", element.RedCardWatchState.ToString()));
                    switch (element.RedCardWatchState)
                    {
                        case IPTRedCardWatch.NOREDCARD:
                            {
                                if (_score.RedA > 0 || _score.RedB > 0)
                                {
                                    log("More than a red card for any team. Leaving");
                                    return false;
                                }
                                break;
                            }
                        case IPTRedCardWatch.EQUALCARD:
                            {
                                if (_score.RedA != _score.RedB)
                                {
                                    log("Both team have not same count of red cards. Leaving");
                                    return false;
                                }
                                break;
                            }
                        case IPTRedCardWatch.TEAMAMORE:
                            {
                                if (_score.RedA <= _score.RedB)
                                {
                                    log("Team A has less red cards then Team B. Leaving");
                                    return false;
                                }
                                break;
                            }
                        case IPTRedCardWatch.TEAMBMORE:
                            {
                                if (_score.RedB <= _score.RedA)
                                {
                                    log("Team B has less red cards then Team A. Leaving");
                                    return false;
                                }
                                break;
                            }
                    }

                    log("Red Card check passed successfully");
                }
                else
                {
                    log("Red Card observation is not activated");
                }


                // überprüfen, ob schon ein Trade auf den Markt vorhanden ist
                BFUEStrategy strategy = null;
                if (_watcher.BetSet.ContainsKey(_marketId))
                {
                    strategy = _watcher.BetSet[_marketId];
                }
                if (strategy != null)
                {
                    log("There's already a trade for market");
                    // Überprüfe, ob nicht abgeschlossener Trade erlaubt ist
                    if (strategy.State == SETSTATE.UNSETTELED && !element.UnsettledAllowed)
                    {
                        log("Unsettled Trades not allowed: leaving");
                        return false;
                    }
                    else
                    {
                        log("Unsettled Trades allowed: pass");
                    }
                    // Überprüfe, ob abgeschlossener Trade erlaubt ist
                    if (strategy.State == SETSTATE.SETTLED && !element.SettledAllowed)
                    {
                        log("Reopening settled Trades not allowed: Leaving");
                        return false;
                    }
                    else
                    {
                        log("Reopening settled Trades allowed: Pass");
                    }
                }

                //Quoten überprüfen
                log("Reading new market prices");
                SXALMarketPrices marketPrices = SXALKom.Instance.getMarketPrices(_marketId);

                // Keine Quoten
                if (marketPrices == null)
                {
                    log("There are no market prices: Leaving");
                    /*
                    if (message != null)
                        message(this, new BFMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strNoMarketPrices, LayTheDraw.strModule));
                    */
                    SXALMarket market = SXALSoccerMarketManager.Instance.InPlayMarkets[_marketId];
                    
                    throw new IPTNoMarketPricesException();
                }

                // Markt pausiert
                // Falls Markt pausiert => abwarten
                if (marketPrices.MarketStatus == SXALMarketStatusEnum.SUSPENDED)
                {
                    log("Market is suspended: Leaving");
                    /*
                    if (message != null)
                        message(this, new BFMessageEventArgs(DateTime.Now, m_marketName, LayTheDraw.strMarketSuspended, LayTheDraw.strModule));
                     */
                    SXALMarket market = SXALSoccerMarketManager.Instance.InPlayMarkets[_marketId];
                    
                    throw new IPTMarketSupendedException();
                }

                // Genug Quoten vorhanden?
                if (marketPrices.RunnerPrices.Length < 3)
                {
                    log("Not enough selections in market: Leaving");
                    SXALMarket market = SXALSoccerMarketManager.Instance.InPlayMarkets[_marketId];
                    
                    throw new IPTMarketNotEnoughOddsException();
                }

                // Quoten im Rahmen?
                log("Reading Prices");
                SXALRunnerPrices runnerPrice = marketPrices.RunnerPrices[2];
                if (runnerPrice.BestPricesToLay.Length > 0)
                {
                    SXALPrice priceLay = runnerPrice.BestPricesToLay[0];
                    log(String.Format("Odd for lay is {0}", priceLay.Price));
                    if (!(priceLay.Price >= element.LoOdds && priceLay.Price <= element.HiOdds))
                    {
                        log(String.Format("Odd {0} is not within interval of {1} and {2}: Leaving", priceLay.Price, element.LoOdds, element.HiOdds));
                        return false;
                    }

                    //Passt Marktvolumen?
                    if (!(runnerPrice.TotalAmountMatched > element.LoVolume && runnerPrice.TotalAmountMatched < element.HiVolume))
                    {
                        log(String.Format("Market Volume {0} is not within interval of {1} and {2}: Leaving", runnerPrice.TotalAmountMatched, element.LoVolume, element.HiVolume));
                        return false;
                    }

                    // Preis passt
                    _selectionId = marketPrices.RunnerPrices[2].SelectionId;
                    odd = priceLay.Price;
                }
                else
                {
                    log("Not enought Lay Prices: Leaving");
                    SXALMarket market = SXALSoccerMarketManager.Instance.InPlayMarkets[_marketId];
                    
                    throw new IPTMarketNoLayOddsException();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                odd = 0;
                return false;
            }
            return true;
        }

        void _score_RaiseGoalEvent(object sender, GoalEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _score_PlaytimeTickEvent(object sender, PlaytimeTickEventArgs e)
        {
            try
            {
                if (!SXThreadStateChecker.isStartedBackground(_runner))
                {
                    _runner = new Thread(runner);
                    _runner.IsBackground = true;
                    _runner.Start();
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void _score_GameEndedEvent(object sender, GameEndedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _score_BackGoalEvent(object sender, GoalBackEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void log(string message)
        {
            TradeLog.Instance.writeLog(SXALSoccerMarketManager.Instance.getMatchById(this.MarketId), "LayTheDraw", "InplayStarter", message);
        }
    }

    class IPTNoMarketPricesException : Exception { }
    class IPTMarketSupendedException : Exception { }
    class IPTMarketNotEnoughOddsException : Exception { }
    class IPTMarketNoLayOddsException : Exception { }
}
