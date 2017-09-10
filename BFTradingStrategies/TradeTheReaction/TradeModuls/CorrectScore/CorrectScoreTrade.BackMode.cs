using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.muk.eventargs;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore
{
    partial class CorrectScoreTrade
    {
        class BackMode : TradeMode
        {
            private CorrectScoreTrade _correctScoreTrade;           

            public BackMode(CorrectScoreTrade correctScoreTrade)
            {
                _correctScoreTrade = correctScoreTrade;
                _correctScoreTrade._stoppable = true;
            }


            internal override void cancelOpenBets()
            {
                for (int i = 0; i < _correctScoreTrade._layBets.Bets.Count();i++ )
                {
                    SXALBet b = _correctScoreTrade._layBets.Bets.Values[i];
                    if (b.BetStatus == SXALBetStatusEnum.MU || b.BetStatus == SXALBetStatusEnum.U)
                    {
                        if (_correctScoreTrade.cancelBet(ref b))
                            _correctScoreTrade._layBets.Bets[b.BetId] = b;
                            //_correctScoreTrade._layBets.Bets.Values[i] = b;
                    }
                }
            }

            internal override void hedgeRunner() 
            {
                bool recheck = false;
                bool checkUnmatched = false;

                try
                {
                    _correctScoreTrade.log("Start Hedging Thread");

                    TimeSpan span = new TimeSpan(0, 0, 60);
                    bool playtimeRetry = false;
                    SXALBet layBet = null;
                    //Bestimmte Zeit warten oder auf Spielminute?
                    if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseWaitTime &&
                        _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseHedgeWaitTime)
                    {
                        span = new TimeSpan(0, 0, _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackHedgeWaitTime);
                    }

                    // Vor dem Starten der Funktion lesen wir mind. einmal alle Wetten neu
                    _correctScoreTrade.recheckBets();

                    //Falls es mind. eine offene Wette auf Lay-Seite gibt, dann gehen wir gleich in die Überprüfung
                    if (_correctScoreTrade._layBets.OneBetUnmatched)
                    {
                        checkUnmatched = true;
                    }

                    while (true)
                    {
                        _correctScoreTrade._stoppable = true;
                        EventHandler<BetsChangedEventArgs> betsChangedEvent = _correctScoreTrade.BetsChangedEvent;

                        if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseWaitTime &&
                            _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseHedgeWaitTime)
                        {
                            _correctScoreTrade.log(String.Format("Wait {0} before attempt to hedge", span.TotalSeconds));
                            EventHandler<SetTimerEventArgs> setTimerHandler = _correctScoreTrade.SetTimer;

                            if (setTimerHandler != null)
                            {
                                setTimerHandler(_correctScoreTrade, new SetTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade, span));
                            }

                            Thread.Sleep(span);
                            // Zukünftige Wiederholung
                            span = new TimeSpan(0, 0, 60);
                        }
                        else
                        {
                            //auf eine bestimmte Spielzeit warten
                            if (_correctScoreTrade.Score.Playtime < _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackHedgePlayTime && playtimeRetry
                                == false)
                            {
                                _correctScoreTrade.log(String.Format("Wait until playtime {0} before hedging. Current Playtime is {1}", _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackHedgePlayTime,
                                    _correctScoreTrade.Score.Playtime));
                                span = new TimeSpan(0, 0, 60);
                                _correctScoreTrade._stoppable = true;

                                EventHandler<SetTimerEventArgs> setTimerHandler = _correctScoreTrade.SetTimer;

                                if (setTimerHandler != null)
                                {
                                    setTimerHandler(_correctScoreTrade, new SetTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade, span));
                                }

                                Thread.Sleep(span);
                                continue;
                            }
                            else if (playtimeRetry == true)
                            {
                                span = new TimeSpan(0, 0, 60);
                                _correctScoreTrade._stoppable = true;

                                _correctScoreTrade.log(String.Format("Wait {0} seconds before retrying", span.TotalSeconds));

                                EventHandler<SetTimerEventArgs> setTimerHandler = _correctScoreTrade.SetTimer;

                                if (setTimerHandler != null)
                                {
                                    setTimerHandler(this, new SetTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade, span));
                                }

                                Thread.Sleep(span);
                            }
                        }

                        if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.NoTrade)
                        {
                            _correctScoreTrade.log(String.Format("No trading-Flag set. Rechecking in {0} sceconds", 60));
                            span = new TimeSpan(0, 0, 60);
                            _correctScoreTrade._stoppable = true;
                            Thread.Sleep(span);
                            continue;
                        }


                        // Kein Trading mehr nötig?
                        bool bContinue = true;
                        if (_correctScoreTrade.TradeType == TRADETYPE.SCORELINEOTHERBACK)
                        {
                            if (_correctScoreTrade.Score.ScoreA > 3 || _correctScoreTrade.Score.ScoreB > 3)
                                bContinue = false;
                        }
                        else
                        {
                            if (_correctScoreTrade.Score.ScoreA > CSTradeTypeToScoresList.GetScoreA(_correctScoreTrade.TradeType) ||
                           _correctScoreTrade.Score.ScoreB > CSTradeTypeToScoresList.GetScoreB(_correctScoreTrade.TradeType))
                                bContinue = false;
                        }
                        if (!bContinue)
                        {
                            _correctScoreTrade.log(String.Format("Score is {0}. No Trading possible. Leave Hedging!", _correctScoreTrade.Score.getScore()));
                            EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                            if (stopTimer != null)
                                stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));
                            return;
                        }


                        if(recheck)
                        {
                            _correctScoreTrade.recheckBets();
                            recheck = false;
                        }

                        //Falls es eine mind. eine Nichterfüllte Wette gab => Neu überprüfen
                        if (checkUnmatched)
                        {
                            _correctScoreTrade.recheckBets();
                            if (!_correctScoreTrade._layBets.OneBetUnmatched)
                            {
                                if (betsChangedEvent != null)
                                    betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                checkUnmatched = false;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        /*
                        if (layBet != null)
                        {

                            _correctScoreTrade.log("Rechecking an open LayBet");
                            SXALBetStatusEnum oldStatus = layBet.BetStatus;
                            layBet = SXAL.SXALKom.Instance.getBetDetail(layBet.BetId);
                            _correctScoreTrade.log(String.Format("Old Status was {0} and new status is {1}", oldStatus, layBet.BetStatus));
                            if (layBet.BetStatus == oldStatus && oldStatus != SXALBetStatusEnum.M)
                            {
                                playtimeRetry = true;
                                continue;
                            }

                            _correctScoreTrade.log("Status changed! Rechecking");
                            _correctScoreTrade.Lay.Bets[layBet.BetId] = layBet;

                            if (betsChangedEvent != null)
                                betsChangedEvent(this, new BetsChangedEventArgs(_correctScoreTrade));

                            _correctScoreTrade._stoppable = false;
                            break;
                        }
                        */


                        // Wartezeit abgelaufen oder Spielzeit erreicht
                        _correctScoreTrade._stoppable = false;


                        if (_correctScoreTrade.Back.Bets.Count == 0)
                        {
                            _correctScoreTrade.log("No Back Bets. Retrying!");
                            _correctScoreTrade._stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }

                        if (_correctScoreTrade._liveticker == null)
                        {
                            _correctScoreTrade.log("No Liveticker. Retrying!");
                            _correctScoreTrade._stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }

                        //Marktpreise holen
                        _correctScoreTrade.log("Read Market Prices/Odds");
                        SXALMarketPrices prices = null;
                        double layOdds = 0.0;
                        try
                        {
                            if (!_correctScoreTrade.getMarketPrices(out prices, true))
                            {
                                _correctScoreTrade.log("Can not retrieve market prices. Retrying");
                                _correctScoreTrade._stoppable = true;
                                playtimeRetry = true;
                                continue;
                            }


                            _correctScoreTrade.log("Get Best Odds for Laying");

                            if (!_correctScoreTrade.getBestLayOdds(prices, _correctScoreTrade.Back.SelectionId, out layOdds))
                            {
                                _correctScoreTrade.log("Can not retrieve Lay Odds. Retrying!");
                                _correctScoreTrade._stoppable = true;
                                playtimeRetry = true;
                                continue;
                            }

                            if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackCheckLayOdds &&
                                layOdds >= _correctScoreTrade.Back.BetPrice)
                            {
                                _correctScoreTrade.log(String.Format("The Lay Odd {0} is larger than the Back Odd {1}. Repeating", layOdds, _correctScoreTrade.Back.BetPrice));
                                _correctScoreTrade._stoppable = true;
                                playtimeRetry = true;
                                continue;
                            }

                            //Prozentüberwachung, falls notwendig
                            if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseOddsPercentage)
                            {
                                double targetOdds = _correctScoreTrade.Back.BetPrice * (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackHedgePercentage * 0.01);
                                targetOdds = (double)SXALKom.Instance.validateOdd((decimal)targetOdds);
                                if (layOdds > targetOdds)
                                {
                                    _correctScoreTrade.log(String.Format("The Lay Odds of {0} are larger than the minimum expected odds of {1}. Set Lay Odds to excpeted value", layOdds, targetOdds));
                                    layOdds = targetOdds;                                    
                                    _correctScoreTrade._stoppable = true;
                                }

                                /*
                                if (layOdds > targetOdds)
                                {
                                    _correctScoreTrade.log(String.Format("The Lay Odds of {0} are larger than the minimum expected odds of {1}. Don't hedge and repeat", layOdds, targetOdds));
                                    _correctScoreTrade._stoppable = true;
                                    playtimeRetry = true;
                                    continue;
                                }
                                 */
                            }

                        }
                        catch (SXALMarketDoesNotExistException mdnee)
                        {
                            ExceptionWriter.Instance.WriteException(mdnee);
                            EventHandler<SXWMessageEventArgs> message = _correctScoreTrade.MessageEvent;
                            String msg = String.Format("Market {0} does not exist! ExpectionMessage {1}. Leaving Trade!", _correctScoreTrade.MarketId, mdnee.Message);
                            _correctScoreTrade.log(msg);
                            if (message != null)
                            {
                                message(this, new SXWMessageEventArgs(DateTime.Now, _correctScoreTrade.Match, msg, "TradeTheReaction - Correct Score"));
                            }

                            EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                            if (stopTimer != null)
                                stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));

                            _correctScoreTrade._stoppable = true;

                            return;
                        }
                        catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
                        {
                            ExceptionWriter.Instance.WriteException(mnsnae);
                            EventHandler<SXWMessageEventArgs> message = _correctScoreTrade.MessageEvent;
                            String msg = String.Format("Market {0} neither suspended nor active! ExpectionMessage {1}. Leaving Trade!", _correctScoreTrade.MarketId, mnsnae.Message);
                            _correctScoreTrade.log(msg);
                            if (message != null)
                            {
                                message(this, new SXWMessageEventArgs(DateTime.Now, _correctScoreTrade.Match, msg, "TradeTheReaction - Correct Score"));
                            }

                            EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                            if (stopTimer != null)
                                stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));

                            _correctScoreTrade._stoppable = true;

                            return;
                        }
                        catch (SXALThrottleExceededException)
                        {
                            //Abfragelimit der Free API für Marktquoten und -preise wurde erreicht.
                            //Berechne Blind eine Hedgingquote ( 1/2 Einstiegsquote ) und versuche damit
                            // zu arbeiten.
                            double tmpOdds = _correctScoreTrade.Back.BetPrice - 1 / 2;
                            if (tmpOdds < 1) tmpOdds += 0.5;

                            layOdds = (double)SXALKom.Instance.validateOdd((decimal)tmpOdds);

                        }
                        double money = _correctScoreTrade.Back.BetSize - _correctScoreTrade.Lay.BetSize;

                        _correctScoreTrade.logBetAmount(String.Format("Calculating money for Lay. Existing Back Bets {0} {3} - Existing Lay Bets {1} {3} = {2} {3}",
                                _correctScoreTrade.Back.BetSize, _correctScoreTrade.Lay.BetSize, money, SXALBankrollManager.Instance.Currency));

                        if (money <= 0.0)
                        {
                            EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                            if (stopTimer != null)
                                stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));
                            _correctScoreTrade.log("No Money needed to bet. Leaving!");
                            _correctScoreTrade._stoppable = true;
                            return;
                        }

                        

                        try
                        {
                            try
                            {
                                layBet = SXALKom.Instance.placeLayBet(_correctScoreTrade._marketId, _correctScoreTrade.Back.SelectionId, layOdds, money);
                            }
                            catch (SXALNoBetBelowMinAllowedException)
                            {
                                _correctScoreTrade.log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                                layBet = SXALKom.Instance.placeLayBet(_correctScoreTrade._marketId, _correctScoreTrade.Back.SelectionId, layOdds, SXALKom.Instance.MinStake);
                            }
                            catch (SXALInsufficientFundsException)
                            {
                                _correctScoreTrade.log("Not enough money on betting account. Retrying!");
                                _correctScoreTrade._stoppable = true;
                                playtimeRetry = true;
                                recheck = true;
                                continue;
                            }


                        }
                        catch (SXALBetInProgressException bipe)
                        {
                            ExceptionWriter.Instance.WriteException(bipe);
                            _correctScoreTrade.log("Received a BetInProgressException. Reread Market");
                            layBet = _correctScoreTrade.checkBIP(bipe.BetId);
                        }

                        if (layBet == null)
                        {
                            //TODO: Markt nachlesen?
                            _correctScoreTrade.log("Lay Bet is null. Retrying!");
                            _correctScoreTrade._stoppable = true;
                            playtimeRetry = true;
                            recheck = true;
                            continue;
                        }

                        //Wettstatus überprüfen
                        switch (layBet.BetStatus)
                        {
                            case SXALBetStatusEnum.M:
                                {
                                    //TODO: Wette der Kollektion hinzufügen, Status setzen und Greening laufen lassen
                                    //_lay00.Bets.Add(layBet.betId, layBet);
                                    _correctScoreTrade.addBet(layBet, false);


                                    if (betsChangedEvent != null)
                                        betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                    _correctScoreTrade._stoppable = true;
                                    break;
                                }
                            case SXALBetStatusEnum.MU:
                                //_lay00.Bets.Add(layBet.betId, layBet);
                                _correctScoreTrade.addBet(layBet, false);
                                if (betsChangedEvent != null)
                                    betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                checkUnmatched = true;
                                playtimeRetry = true;
                                continue;
                                /*
                                _correctScoreTrade.addBet(layBet, false);
                                if (betsChangedEvent != null)
                                    betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                if (!_correctScoreTrade.tradeCancelBet(betsChangedEvent, ref layBet, ref playtimeRetry))
                                {
                                    continue;
                                }
                                 */
                            case SXALBetStatusEnum.U:
                                 _correctScoreTrade.addBet(layBet, false);
                                if (betsChangedEvent != null)
                                    betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                checkUnmatched = true;
                                playtimeRetry = true;
                                continue;                                
                            default:
                                _correctScoreTrade.log(String.Format("Bet Status is {0}. Repeating", layBet.BetStatus.ToString()));
                                _correctScoreTrade._stoppable = true;
                                playtimeRetry = true;
                                recheck = true;
                                continue;
                        }
                       
                        return;
                    }
                }
                catch (ThreadAbortException)
                {
                    EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                    if (stopTimer != null)
                        stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));

                    _correctScoreTrade._stoppable = true;

                    EventHandler<SXWMessageEventArgs> message = _correctScoreTrade.MessageEvent;
                    String msg = String.Format("Market {0} Match {1}! Hedge stopped!", _correctScoreTrade.MarketId, _correctScoreTrade.Match);
                    _correctScoreTrade.log(msg);
                    if (message != null)
                    {
                        message(this, new SXWMessageEventArgs(DateTime.Now, _correctScoreTrade.Match, msg, "TradeTheReaction - Correct Score"));
                    }

                }
                catch (Exception exc)
                {
                    EventHandler<SXWMessageEventArgs> message = _correctScoreTrade.MessageEvent;
                    String msg = String.Format("Market {0} Match {1}! ExpectionMessage {2}. Leaving Trade!", _correctScoreTrade.MarketId, _correctScoreTrade.Match, exc.Message);
                    _correctScoreTrade.log(msg);
                    if (message != null)
                    {
                        message(this, new SXWMessageEventArgs(DateTime.Now, _correctScoreTrade.Match, msg, "TradeTheReaction - Correct Score"));
                    }

                    EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                    if (stopTimer != null)
                        stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));

                    _correctScoreTrade._stoppable = true;
                }
                finally
                {
                     //Status aktualisieren;
                    _correctScoreTrade.TradeState.checkState();

                    EventHandler<StopTimerEventArgs> stopTimer2 = _correctScoreTrade.StopTimer;
                    if (stopTimer2 != null)
                        stopTimer2(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));
                }
            }

            internal override void greenRunner() 
            {
                bool recheck = false;
                bool checkUnmatched = false;

                try
                {
                    _correctScoreTrade.log("Start Greening Thread");

                    if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackOnlyHedge)
                    {
                        _correctScoreTrade.log("Leaving Greening Thread because only Hedging is configured");
                        return;
                    }

                    TimeSpan span = new TimeSpan(0, 0, 60);
                    bool playtimeRetry = false;
                    SXALBet layBet = null;
                    //Bestimmte Zeit warten oder auf Spielminute?
                    if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseWaitTime &&
                        _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseGreenWaitTime)
                    {
                        span = new TimeSpan(0, 0, _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackGreenWaittime);
                    }

                    // Vor dem Starten der Funktion lesen wir mind. einmal alle Wetten neu
                    _correctScoreTrade.recheckBets();

                    //Falls es mind. eine offene Wette auf Lay-Seite gibt, dann gehen wir gleich in die Überprüfung
                    if (_correctScoreTrade._layBets.OneBetUnmatched)
                    {
                        checkUnmatched = true;
                    }

                    while (true)
                    {
                        _correctScoreTrade._stoppable = true;

                        EventHandler<BetsChangedEventArgs> betsChangedEvent = _correctScoreTrade.BetsChangedEvent;
                        if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseWaitTime &&
                            _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseGreenWaitTime)
                        {
                            EventHandler<SetTimerEventArgs> setTimerHandler = _correctScoreTrade.SetTimer;

                            if (setTimerHandler != null)
                            {
                                setTimerHandler(_correctScoreTrade, new SetTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade, span));
                            }

                            _correctScoreTrade.log(String.Format("Wait {0} before attempt to green", span.TotalSeconds));
                            Thread.Sleep(span);
                            // Zukünftige Wiederholung
                            span = new TimeSpan(0, 0, 60);
                        }
                        else
                        {
                            //auf eine bestimmte Spielzeit warten
                            if (_correctScoreTrade.Score.Playtime < _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackGreenPlaytime
                                && playtimeRetry == false)
                            {
                                _correctScoreTrade.log(String.Format("Wait until playtime {0} before greening. Current Playtime is {1}", _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackGreenPlaytime,
                                    _correctScoreTrade.Score.Playtime));
                                span = new TimeSpan(0, 0, 60);
                                _correctScoreTrade._stoppable = true;

                                EventHandler<SetTimerEventArgs> setTimerHandler = _correctScoreTrade.SetTimer;

                                if (setTimerHandler != null)
                                {
                                    setTimerHandler(_correctScoreTrade, new SetTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade, _correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackGreenPlaytime.ToString()));
                                }

                                Thread.Sleep(span);
                                continue;
                            }
                            else if (playtimeRetry == true)
                            {
                                span = new TimeSpan(0, 0, 60);
                                _correctScoreTrade._stoppable = true;

                                _correctScoreTrade.log(String.Format("Wait {0} seconds before retrying", span.TotalSeconds));

                                EventHandler<SetTimerEventArgs> setTimerHandler = _correctScoreTrade.SetTimer;

                                if (setTimerHandler != null)
                                {
                                    setTimerHandler(this, new SetTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade, span));
                                }

                                Thread.Sleep(span);
                            }
                        }

                        if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.NoTrade)
                        {
                            _correctScoreTrade.log(String.Format("No trading-Flag set. Rechecking in {0} sceconds", 60));
                            span = new TimeSpan(0, 0, 60);
                            _correctScoreTrade._stoppable = true;
                            Thread.Sleep(span);
                            continue;
                        }


                        // Kein Trading mehr nötig?
                        bool bContinue = true;
                        if (_correctScoreTrade.TradeType == TRADETYPE.SCORELINEOTHERBACK)
                        {
                            if (_correctScoreTrade.Score.ScoreA > 3 || _correctScoreTrade.Score.ScoreB > 3)
                                bContinue = false;
                        }
                        else
                        {
                            if (_correctScoreTrade.Score.ScoreA > CSTradeTypeToScoresList.GetScoreA(_correctScoreTrade.TradeType) ||
                           _correctScoreTrade.Score.ScoreB > CSTradeTypeToScoresList.GetScoreB(_correctScoreTrade.TradeType))
                                bContinue = false;
                        }
                        if (!bContinue)
                        {
                            _correctScoreTrade.log(String.Format("Score is {0}. No Trading possible. Leave Greening!", _correctScoreTrade.Score.getScore()));
                            EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                            if (stopTimer != null)
                                stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));
                            _correctScoreTrade._stoppable = true;
                            return;
                        }

                        if (recheck)
                        {
                            _correctScoreTrade.recheckBets();
                            recheck = false;
                        }

                        //Falls es eine mind. eine Nichterfüllte Wette gab => Neu überprüfen
                        if (checkUnmatched)
                        {
                            _correctScoreTrade.recheckBets();
                            if (!_correctScoreTrade._layBets.OneBetUnmatched)
                            {
                                if (betsChangedEvent != null)
                                    betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                checkUnmatched = false;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }                        

                        // Wartezeit abgelaufen oder Spielzeit erreicht
                        _correctScoreTrade._stoppable = false;


                        if (_correctScoreTrade.Back.Bets.Count == 0)
                        {
                            _correctScoreTrade.log("No Back Bets. Retrying!");
                            _correctScoreTrade._stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }

                        if (_correctScoreTrade._liveticker == null)
                        {
                            _correctScoreTrade.log("No Liveticker. Retrying!");
                            _correctScoreTrade._stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }


                        //Marktpreise holen
                        SXALMarketPrices prices = null;
                        try
                        {
                            _correctScoreTrade.log("Read Market Prices/Odds");

                            if (!_correctScoreTrade.getMarketPrices(out prices, false))
                            {
                                _correctScoreTrade.log("Can not retrieve market prices. Retrying");
                                _correctScoreTrade._stoppable = true;
                                playtimeRetry = true;
                                continue;
                            }
                        }
                        catch (SXALMarketDoesNotExistException mdnee)
                        {
                            ExceptionWriter.Instance.WriteException(mdnee);
                            EventHandler<SXWMessageEventArgs> message = _correctScoreTrade.MessageEvent;
                            String msg = String.Format("Market {0} does not exist! ExpectionMessage {1}. Leaving Trade!", _correctScoreTrade.MarketId, mdnee.Message);
                            _correctScoreTrade.log(msg);
                            if (message != null)
                            {
                                message(this, new SXWMessageEventArgs(DateTime.Now, _correctScoreTrade.Match, msg, "TradeTheReaction - Correct Score"));
                            }

                            EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                            if (stopTimer != null)
                                stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));

                            _correctScoreTrade._stoppable = true;

                            return;
                        }
                        catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
                        {
                            ExceptionWriter.Instance.WriteException(mnsnae);
                            EventHandler<SXWMessageEventArgs> message = _correctScoreTrade.MessageEvent;
                            String msg = String.Format("Market {0} neither suspended nor active! ExpectionMessage {1}. Leaving Trade!", _correctScoreTrade.MarketId, mnsnae.Message);
                            _correctScoreTrade.log(msg);
                            if (message != null)
                            {
                                message(this, new SXWMessageEventArgs(DateTime.Now, _correctScoreTrade.Match, msg, "TradeTheReaction - Correct Score"));
                            }

                            EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                            if (stopTimer != null)
                                stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));

                            _correctScoreTrade._stoppable = true;

                            return;
                        }

                        _correctScoreTrade.log("Get Best Odds for Laying");
                        double layOdds = 0.0;
                        if (!_correctScoreTrade.getBestLayOdds(prices, _correctScoreTrade.Back.SelectionId, out layOdds))
                        {
                            _correctScoreTrade.log("Can not retrieve Lay Odds. Retrying!");
                            _correctScoreTrade._stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }

                        if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackCheckLayOdds && layOdds >= _correctScoreTrade.Back.BetPrice)
                        {
                            _correctScoreTrade.log(String.Format("The Lay Odd {0} is larger than the Back Odd {1}. Repeating", layOdds, _correctScoreTrade.Back.BetPrice));
                            _correctScoreTrade._stoppable = true;
                            playtimeRetry = true;
                            continue;
                        }

                        //Prozentüberwachung, falls notwendig
                        if (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackUseOddsPercentage)
                        {
                            double targetOdds = _correctScoreTrade.Back.BetPrice * (_correctScoreTrade._currentTradeOutSettings.TradeOutSettings.CSBackGreenPercentage * 0.01);
                            targetOdds = (double)SXALKom.Instance.validateOdd((decimal)targetOdds);

                            if (layOdds > targetOdds)
                            {
                                _correctScoreTrade.log(String.Format("The Lay Odds of {0} are larger than the minimum expected odds of {1}. Set Lay Odds to excpeted value", layOdds, targetOdds));
                                layOdds = targetOdds;
                                _correctScoreTrade._stoppable = true;
                            }
                        }

                        //double money = Back.BetSize - Lay.BetSize;
                        double money = ((_correctScoreTrade.Back.BetSize * _correctScoreTrade.Back.BetPrice - _correctScoreTrade.Back.BetSize) - (_correctScoreTrade.Lay.BetSize * _correctScoreTrade.Lay.BetPrice - _correctScoreTrade.Lay.BetSize)) / layOdds;

                        _correctScoreTrade.logBetAmount(String.Format("Calculating money for Lay. Existing Back Bets {0} {3} - Existing Lay Bets {1} {3} = {2} {3}",
                                _correctScoreTrade.Back.BetSize, _correctScoreTrade.Lay.BetSize, money, SXALBankrollManager.Instance.Currency));

                        if (money <= 0.0)
                        {
                            EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                            if (stopTimer != null)
                                stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));
                            _correctScoreTrade.log("No Money needed to bet. Leaving!");
                            _correctScoreTrade._stoppable = true;
                            return;
                        }

                        

                        try
                        {
                            try
                            {
                                layBet = SXALKom.Instance.placeLayBet(_correctScoreTrade._marketId, _correctScoreTrade.Back.SelectionId, layOdds, money);
                            }
                            catch (SXALNoBetBelowMinAllowedException)
                            {
                                _correctScoreTrade.log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                                layBet = SXALKom.Instance.placeLayBet(_correctScoreTrade._marketId, _correctScoreTrade.Back.SelectionId, layOdds, SXALKom.Instance.MinStake);
                            }
                            catch (SXALInsufficientFundsException)
                            {
                                _correctScoreTrade.log("Not enough money on betting account. Retrying!");
                                _correctScoreTrade._stoppable = true;
                                playtimeRetry = true;
                                recheck = true;
                                continue;
                            }
                        }
                        catch (SXALBetInProgressException bipe)
                        {
                            ExceptionWriter.Instance.WriteException(bipe);
                            _correctScoreTrade.log("Received a BetInProgressException. Reread Market");
                            layBet = _correctScoreTrade.checkBIP(bipe.BetId);
                        }

                        if (layBet == null)
                        {
                            //TODO: Markt nachlesen?
                            _correctScoreTrade.log("Lay Bet is null. Retrying!");
                            _correctScoreTrade._stoppable = true;
                            playtimeRetry = true;
                            recheck = true;
                            continue;
                        }

                        //Wettstatus überprüfen
                        switch (layBet.BetStatus)
                        {
                            case SXALBetStatusEnum.M:
                                {
                                    //TODO: Wette der Kollektion hinzufügen, Status setzen und Greening laufen lassen
                                    //_lay00.Bets.Add(layBet.betId, layBet);
                                    _correctScoreTrade.addBet(layBet, false);


                                    if (betsChangedEvent != null)
                                        betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                    _correctScoreTrade._stoppable = true;
                                    break;
                                }
                            case SXALBetStatusEnum.MU:
                                //_lay00.Bets.Add(layBet.betId, layBet);
                                 _correctScoreTrade.addBet(layBet, false);
                                if (betsChangedEvent != null)
                                    betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                checkUnmatched = true;
                                playtimeRetry = true;
                                continue;           
                            case SXALBetStatusEnum.U:
                                _correctScoreTrade.addBet(layBet, false);
                                if (betsChangedEvent != null)
                                    betsChangedEvent(_correctScoreTrade, new BetsChangedEventArgs(_correctScoreTrade));
                                checkUnmatched = true;
                                playtimeRetry = true;
                                continue;           
                            default:
                                _correctScoreTrade.log(String.Format("Bet Status is {0}. Repeating", layBet.BetStatus.ToString()));
                                _correctScoreTrade._stoppable = true;
                                playtimeRetry = true;
                                continue;
                        }
                       
                        return;
                    }
                }
                catch (ThreadAbortException)
                {
                    EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                    if (stopTimer != null)
                        stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));

                    _correctScoreTrade._stoppable = true;

                    EventHandler<SXWMessageEventArgs> message = _correctScoreTrade.MessageEvent;
                    String msg = String.Format("Market {0} Match {1}! Greening stopped!", _correctScoreTrade.MarketId, _correctScoreTrade.Match);
                    _correctScoreTrade.log(msg);
                    if (message != null)
                    {
                        message(this, new SXWMessageEventArgs(DateTime.Now, _correctScoreTrade.Match, msg, "TradeTheReaction - Correct Score"));
                    }

                }
                catch (Exception exc)
                {
                    EventHandler<SXWMessageEventArgs> message = _correctScoreTrade.MessageEvent;
                    String msg = String.Format("Market {0} Match {1}! ExpectionMessage {2}. Leaving Trade!", _correctScoreTrade.MarketId, _correctScoreTrade.Match, exc.Message);
                    _correctScoreTrade.log(msg);
                    if (message != null)
                    {
                        message(this, new SXWMessageEventArgs(DateTime.Now, _correctScoreTrade.Match, msg, "TradeTheReaction - Correct Score"));
                    }

                    EventHandler<StopTimerEventArgs> stopTimer = _correctScoreTrade.StopTimer;
                    if (stopTimer != null)
                        stopTimer(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));

                    _correctScoreTrade._stoppable = true;
                }
                finally
                {
                     //Status aktualisieren;
                    _correctScoreTrade.TradeState.checkState();

                    EventHandler<StopTimerEventArgs> stopTimer2 = _correctScoreTrade.StopTimer;
                    if (stopTimer2 != null)
                        stopTimer2(_correctScoreTrade, new StopTimerEventArgs(_correctScoreTrade._match, _correctScoreTrade));
                }
            }



            internal override double getInitialStake()
            {
                return _correctScoreTrade._backBets.BetSize;
            }

            internal override double getWinnings()
            {
                return _correctScoreTrade._backBets.RiskWin - _correctScoreTrade._layBets.BetSize;
            }

            internal override double getPLSnapshot()
            {
                double dMoney = 0.0;
                // Sonderfall CS Other Back
                if (_correctScoreTrade.TradeType == TRADETYPE.SCORELINEOTHERBACK)
                {
                    // Gewonnen wenn mind. ein Torzähle > 3
                    if (_correctScoreTrade.Score.ScoreA > 3 ||
                        _correctScoreTrade.Score.ScoreB > 3)
                    {
                        dMoney = (_correctScoreTrade._backBets.BetSize * _correctScoreTrade._backBets.BetPrice - _correctScoreTrade._backBets.BetSize) -
                            (_correctScoreTrade._layBets.BetSize * _correctScoreTrade._layBets.BetPrice - _correctScoreTrade._layBets.BetSize);
                    }
                    else
                    {
                        dMoney = _correctScoreTrade._layBets.BetSize - _correctScoreTrade._backBets.BetSize;
                    }
                }
                else
                {
                    if (_correctScoreTrade.Score.ScoreA == CSTradeTypeToScoresList.GetScoreA(_correctScoreTrade.TradeType) &&
                       _correctScoreTrade.Score.ScoreB == CSTradeTypeToScoresList.GetScoreB(_correctScoreTrade.TradeType))
                    {
                        dMoney = (_correctScoreTrade._backBets.BetSize * _correctScoreTrade._backBets.BetPrice - _correctScoreTrade._backBets.BetSize) -
                            (_correctScoreTrade._layBets.BetSize * _correctScoreTrade._layBets.BetPrice - _correctScoreTrade._layBets.BetSize);
                    }
                    else
                    {
                        dMoney = _correctScoreTrade._layBets.BetSize - _correctScoreTrade._backBets.BetSize;
                    }
                }


                dMoney = Math.Round(dMoney, 2);
                return dMoney;
            }


            internal override TRADEMODE TradeModeEnum
            {
                get { return TRADEMODE.BACK; }
            }
        }


    }
}
