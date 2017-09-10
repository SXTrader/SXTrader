using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.SXFastBet;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using System.Threading;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder
{
    class OverUnderBetPlacer : BetPlacer
    {
        const double ABSOLUTEMINIMUM = 0.1;
        public OverUnderBetPlacer(TRADETYPE tradeType, double odd, String match, long marketId, long selectionId, TTRWatcher watcher,
            TTRConfigurationRW config, TRADEMODE tradeMode)
            : base(tradeType, odd, match, marketId, selectionId, watcher, config, tradeMode)
        {
        }
        public override bool placeBet()
        {
            double moneyToBet = 0.0;
            bool bRet = false;
            if (_config.RelativeBetAmount)
            {
                bRet = placeBetRelative(out moneyToBet);               
            }
            else if (_config.FastBetFixedAmount)
            {
                bRet = placeBetAbsolut(out moneyToBet);
            }
            else
            {
                bRet = placeBetTarget(out moneyToBet);
            }

            if (!bRet)
            {
                return false;
            }

            SXALMarket bfMarket = TTRHelper.GetMarketByTradeType(_tradeType, _match);


            if (bfMarket == null)
            {
                log(String.Format("Can not find the market {0} for Match {1} in Markets. Leaving!", _tradeType, _match));
                return false;
            }

            _marketId = bfMarket.Id;


            log(String.Format("Placing a Back Bet for the odd of {0}", _odd));
            SXALBet bet = null;
            try
            {
                bool keepBet = _config.KeepInplay;

                if (_tradeMode == TRADEMODE.BACK)
                    try
                    {
                        bet = SXALKom.Instance.placeBackBet(_marketId, _selectionId, 0, keepBet, _odd, moneyToBet);
                    }
                    catch (SXALNoBetBelowMinAllowedException)
                    {
                        log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                        bet = SXALKom.Instance.placeBackBet(_marketId, _selectionId, 0, keepBet, _odd, SXALKom.Instance.MinStake);
                    }
                    catch (SXALInsufficientFundsException)
                    {
                        log("Not enough money on betting account. Retrying!");
                        return false;
                    }
                else
                {
                    try
                    {
                        bet = SXALKom.Instance.placeLayBet(_marketId, _selectionId, 0, keepBet, _odd, moneyToBet);
                    }
                    catch (SXALNoBetBelowMinAllowedException)
                    {
                        log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                        bet = SXALKom.Instance.placeLayBet(_marketId, _selectionId, 0, keepBet, _odd, SXALKom.Instance.MinStake);
                    }
                    catch (SXALInsufficientFundsException)
                    {
                        log("Not enough money on betting account. Leaving!");
                        return false;
                    }
                }
               
            }
            catch (SXALBetInProgressException bipe)
            {
                log(String.Format("Received a BetInProgressException with betId {0}. Rechecking Betstatus", bipe.BetId));
                //Betfair Zeit geben den Markt abzuschliessen
                Thread.Sleep(3000);
                SXALMUBet[] muBets = null;
                if (bipe.BetId == 0)
                    muBets = SXALKom.Instance.getBetsMU(_marketId);
                else
                    muBets = SXALKom.Instance.getBetMU(bipe.BetId);
                if (muBets != null)
                {
                    foreach (SXALMUBet muBet in muBets)
                    {
                        if (muBet.SelectionId != TTRHelper.GetSelectionIdByTradeType(_tradeType, _marketId))
                            continue;

                        //Uns interessieren hier nur Lay-Wetten
                        if (muBet.BetType == SXALBetTypeEnum.B)
                            continue;

                        if (_watcher.ContainsKey(_match))
                        {
                            if (_watcher[_match].ContainsKey(_tradeType))
                            {
                                // Uns interessieren hier nur neue Wetten                                
                                if (((OverUnderTrade)_watcher[_match][_tradeType]).Back.Bets.ContainsKey(muBet.BetId))
                                    continue;
                            }
                        }

                        bet = SXALKom.Instance.getBetDetail(muBet.BetId);

                    }
                }
            }

            if (bet != null)
            {
                log("A bet was successfully placed on market");
                if (_config.KeepUnmatched)
                {
                    log("Keeping of unmatched bets checked");
                    if (bet.BetStatus != SXALBetStatusEnum.C && bet.BetStatus != SXALBetStatusEnum.L &&
                        bet.BetStatus != SXALBetStatusEnum.V)
                    {
                        log(String.Format("Betstatus is {0}. Calling Wather with bet", bet.BetStatus));
                        _watcher.setNewBet(_tradeType, bet, _config, _score);
                        return true;
                    }
                    else
                    {
                        log(String.Format("Betstatus is {0}. Retrying", bet.BetStatus));
                        return false;
                    }
                }
                else
                {
                    //Wettzustand
                    if (bet.BetStatus == SXALBetStatusEnum.M)
                    {
                        log("Betstatus is matched. Calling Watcher with new bet");
                        _watcher.setNewBet(_tradeType, bet, _config, _score);
                        return true;
                    }
                    if (bet.BetStatus == SXALBetStatusEnum.MU)
                    {
                        log("Betstatus is partly matched. Calling Watcher with new bet");
                        _watcher.setNewBet(_tradeType, bet, _config, _score);
                        //TODO: was mit dem nicht erfüllten teil machen?
                        return true;
                    }
                    if (bet.BetStatus == SXALBetStatusEnum.U)
                    {
                        log("Betstatus is unmatched. Trying to cancel bet");
                        while (true)
                        {
                            try
                            {
                                if (!SXALKom.Instance.cancelBet(bet.BetId))
                                {
                                    log("Canceling of Bet failed");
                                    bet = SXALKom.Instance.getBetDetail(bet.BetId);
                                    if (bet.BetStatus == SXALBetStatusEnum.M)
                                    {
                                        log("Status of uncanceld bet is matched. Calling Watcher with new bet");
                                        _watcher.setNewBet(_tradeType, bet, _config, _score);
                                        return true;
                                    }
                                    if (bet.BetStatus == SXALBetStatusEnum.MU)
                                    {
                                        log("Status of uncanceld Bet is partly matched. Calling Watcher with new bet");
                                        _watcher.setNewBet(_tradeType, bet, _config, _score);
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
                                        log("Status of Bet is still unmatched. retrying");
                                        Thread.Sleep(500);
                                        continue;
                                    }

                                    log(String.Format("Status of uncanceld bet is {0}. Leaving", bet.BetStatus.ToString()));
                                    //TODO: Und nu? Wette konnte aus irgendeinen Grund nicht storniert werden
                                }
                            }                            
                            catch (Exception exc)
                            {
                                ExceptionWriter.Instance.WriteException(exc);
                                log(String.Format("Received an Exception with Message {0}", exc.Message));
                            }
                            return false;
                        }
                    }
                }
            }
            else
            {
                log("No bet was placed: Leaving!");
            }

            return false;
        }
    }
}
