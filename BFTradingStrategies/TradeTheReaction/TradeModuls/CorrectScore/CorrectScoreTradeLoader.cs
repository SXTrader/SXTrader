using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.enums;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore
{
    class CorrectScoreTradeLoader : TradeLoader
    {
        public override ITrade loadTrade(SXALBet bet, TTRWatcher watcher, out LOADTRADEMODE loadMode, out LIVESCOREADDED livescoreMode)
        {
            String match = SXALSoccerMarketManager.Instance.getMatchById(bet.MarketId);
            loadMode = LOADTRADEMODE.UNASSIGNED;
            livescoreMode = LIVESCOREADDED.NONE;
            ITrade trade = null;
            //Das Match schon in irgendeiner Form vorhanden?
            if (!watcher.ContainsKey(match))
            {
                watcher.Add(match, new TradeCollection());
            }

            //Match ist vorhanden, aber ist der Markt schon vorhanden?
            TRADETYPE tradeType = TTRHelper.GetTradeTypeByBetAndSelection(bet);
            /*
            foreach (ITrade trade2 in watcher[match].Values)
            {    
                // Was soll das überhaupt?
                    if (trade2.Back.Bets.ContainsKey(bet.BetId))
                    {
                        loadMode = LOADTRADEMODE.APPEND;
                        trade2.addBet(bet);
                        return trade2;
                    }

                    if (trade2.Lay.Bets.ContainsKey(bet.BetId))
                    {
                        loadMode = LOADTRADEMODE.APPEND;
                        trade2.addBet(bet);
                        return trade2;
                    }
            }
             */
            if (!watcher[match].ContainsKey(tradeType))
            {
                //Nicht vorhanden => Also sicherlich auch der Trade nicht => Neuer Trade konzepieren               
                trade = watcher.constructTrade(tradeType, match, bet, new TTRConfigurationRW(), null, out livescoreMode);
                loadMode = LOADTRADEMODE.CREATE;
            }
            else
            {
                //Vorhanden, aber ist die Wette schon vorhanden?
                ITrade tmpTrade = watcher[match][tradeType];
                SXALBetCollection tmpColl = null;
                if (bet.BetType == SXALBetTypeEnum.L)
                {
                    tmpColl = tmpTrade.Lay;
                }
                else
                {
                    tmpColl = tmpTrade.Back;
                }

                if (!tmpColl.Bets.ContainsKey(bet.BetId))
                {
                    //Wette noch nicht vorhandne, also hinzufügen
                    tmpTrade.addBet(bet, true);
                    //tmpColl.Bets.Add(bet.betId, bet);
                    trade = tmpTrade;
                    loadMode = LOADTRADEMODE.APPEND;
                }
                else
                {
                    //Wette ist vorhanden, aber hat sich irgendetwas an der Wette geändert?
                    SXALBet tmpBet = tmpColl.Bets[bet.BetId];
                    if (tmpBet.BetStatus != bet.BetStatus || tmpBet.AvgPrice != bet.AvgPrice || tmpBet.MatchedSize != bet.MatchedSize
                        || tmpBet.PlacedDate != bet.PlacedDate || tmpBet.Price != bet.Price || tmpBet.RemainingSize != bet.RemainingSize
                        || tmpBet.RequestedSize != bet.RequestedSize)
                    {
                        // Es hat sich was geändert, also update
                        tmpTrade.addBet(bet,true);
                        //tmpColl.Bets[bet.betId] = bet;
                        trade = tmpTrade;
                        loadMode = LOADTRADEMODE.APPEND;
                    }
                    else
                    {
                        //nichts geändert
                        trade = null;
                        loadMode = LOADTRADEMODE.UNASSIGNED;
                    }
                }
            }


            return trade;

        }
    }
}
