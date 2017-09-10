using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.betfairif;
using BetFairIF.com.betfair.api.exchange;

namespace net.sxtrader.bftradingstrategies.BFLTDScratch
{
    public class BFLTDScratchAutoSelector : IBFTSCommon
    {
        private Thread _autoSelectorThread;
        private TimeSpan _autoSelectorSpan;
        private object _lock = "lock";
        private SortedList<int, BFLTDScratchStrategy> _ltdScratchColl;


        public BFLTDScratchAutoSelector()
        {
            _ltdScratchColl = new SortedList<int, BFLTDScratchStrategy>();
            _autoSelectorSpan = new TimeSpan(0, 1, 0);
            _autoSelectorThread = new Thread(autoSelectorRunner);
            _autoSelectorThread.IsBackground = true;
            _autoSelectorThread.Start();
        }

        private void autoSelectorRunner()
        {
            lock (_lock)
            {
                while (true)
                {
                    Thread.Sleep(_autoSelectorSpan);

                    //Liste aller Spiele aufbauen, die innerhalb der nächsten Stunde starten und noch nicht
                    //gehandelt worden sind
                    foreach (BFMarket bfmarket in SoccerMarketManager.Instance.RegularMarkets.Values)
                    {
                        if (bfmarket.Id != 104676357)
                            continue;
                        /*
                        // Für die prinzipielle Auswahl ist erst einmal nur der Markt Match Odds interessant
                        if (!bfmarket.IsMatchOdds)
                            continue;

                        // gestartete Märkte sind uninteressant, da sie nicht mehr gehandelt werden können
                        if (bfmarket.StartDTS < DateTime.Now)
                            continue;

                        // Nur die Märkte, welche innerhalb der nächsten Stunde starten
                        if (bfmarket.StartDTS.Subtract(DateTime.Now).TotalMinutes > 60)
                        //if (DateTime.Now.Subtract(bfmarket.StartDTS).TotalMinutes > 60)
                            continue;
                    
                        // Markt darf noch nicht in Strategieliste sein
                        if (_ltdScratchColl.ContainsKey(bfmarket.Id))
                            continue;
                        */
                        // Ein Treffer, schauen ob ein Trade möglich und ggf. anlegen
                        tryToTrade(bfmarket);
                    }


                    //Liste aller Spiele aufbauen, die innerhalb der nächsten Stunde starten und noch nicht
                    //gehandelt worden sind
                    foreach (BFMarket bfmarket in SoccerMarketManager.Instance.InPlayMarkets.Values)
                    {
                        /*
                        // Für die prinzipielle Auswahl ist erst einmal nur der Markt Match Odds interessant
                        if (!bfmarket.IsMatchOdds)
                            continue;

                        // gestartete Märkte sind uninteressant, da sie nicht mehr gehandelt werden können
                        if (bfmarket.StartDTS < DateTime.Now)
                            continue;

                        // Nur die Märkte, welche innerhalb der nächsten Stunde starten
                        if (bfmarket.StartDTS.Subtract(DateTime.Now).TotalMinutes > 60)
                            //if (DateTime.Now.Subtract(bfmarket.StartDTS).TotalMinutes > 60)
                            continue;

                        // Markt darf noch nicht in Strategieliste sein
                        if (_ltdScratchColl.ContainsKey(bfmarket.Id))
                            continue;
                        */
                        if (bfmarket.Id != 104676357)
                            continue;
                        // Ein Treffer, schauen ob ein Trade möglich und ggf. anlegen
                        tryToTrade(bfmarket);
                    }
                }
            }
        }

        private void tryToTrade(BFMarket bfmarket)
        {
            // den Correct Score Market noch finden
            BFMarket bfmarketCS = SoccerMarketManager.Instance.getCSMarketByMatch(bfmarket.Match);
            if (bfmarketCS == null)
                return;

            //TODO: Zu setzenden Betrag aus customizing 
            double money = 2.0;

            //Marktpreise lesen
            MarketPrices mpMatchOdds = BetfairKom.Instance.getMarketPrices(bfmarket.Id);
            MarketPrices mpCorrectScore = BetfairKom.Instance.getMarketPrices(bfmarketCS.Id);


            double ltdOdds = getBestLayOdds(mpMatchOdds, BetfairKom.DRAWSELECTIONID);
            double laiablity = money * ltdOdds - money;

            double zerotozeroOdds = getBestBackOdds(mpCorrectScore, (int)BetfairKom.SCORESELECTIONIDS.ZEROTOZERO);
            double onetooneOdds = getBestBackOdds(mpCorrectScore, (int)BetfairKom.SCORESELECTIONIDS.ONETOONE);
            double twototwoOdds = getBestBackOdds(mpCorrectScore, (int)BetfairKom.SCORESELECTIONIDS.TWOTOTWO);
            double threetothreeOdds = getBestBackOdds(mpCorrectScore, (int)BetfairKom.SCORESELECTIONIDS.THREETOTHREE);

            //TODO: Tatsächliche Kommissionrate dynamisch errechnen
            double toBeat = laiablity + (laiablity * 0.05);
            double moneyAvailable = 2.0 - (2.0 * 0.05);

            //Einsatz 0 - 0
            double zerotozeroMoney = toBeat / (zerotozeroOdds - 1);
            //Einsatz 1 - 1
            double onetooneMoney = toBeat / (onetooneOdds - 1);
            //Einstaz 2 - 2
            double twototwoMoney = toBeat / (twototwoOdds - 1);
            //Einsatz 3 - 3
            double threetothreeMoney = toBeat / (threetothreeOdds - 1);


            if (zerotozeroMoney + onetooneMoney + twototwoMoney + threetothreeMoney > moneyAvailable)
                return;


            if (zerotozeroMoney + onetooneMoney + twototwoMoney + threetothreeMoney + laiablity > BankrollManager.Instance.AvailibleBalance)
                return;

            // Ansonsten können wir jetzt Wetten
            zerotozeroMoney = Math.Round(zerotozeroMoney, 2);
            onetooneMoney = Math.Round(onetooneMoney, 2);
            twototwoMoney = Math.Round(twototwoMoney, 2);
            threetothreeMoney = Math.Round(threetothreeMoney, 2);
            Bet ltdBet = BetfairKom.Instance.placeLayBet(mpMatchOdds.marketId, BetfairKom.DRAWSELECTIONID, ltdOdds, money);

            Bet zerotozeroBet = null;
            if (zerotozeroMoney < BankrollManager.Instance.MinStake)
            {
                zerotozeroBet = BetfairKom.Instance.placeBackBetBelowMin(mpCorrectScore.marketId, (int)BetfairKom.SCORESELECTIONIDS.ZEROTOZERO, 0, zerotozeroOdds, zerotozeroMoney);
            }
            else
            {
                zerotozeroBet = BetfairKom.Instance.placeBackBet(mpCorrectScore.marketId, (int)BetfairKom.SCORESELECTIONIDS.ZEROTOZERO, zerotozeroOdds, zerotozeroMoney);
            }

            Bet onetooneBet = null;
            if (zerotozeroMoney < BankrollManager.Instance.MinStake)
            {
                onetooneBet = BetfairKom.Instance.placeBackBetBelowMin(mpCorrectScore.marketId, (int)BetfairKom.SCORESELECTIONIDS.ONETOONE, 0, onetooneOdds, onetooneMoney);
            }
            else
            {
                onetooneBet = BetfairKom.Instance.placeBackBet(mpCorrectScore.marketId, (int)BetfairKom.SCORESELECTIONIDS.ONETOONE, onetooneOdds, onetooneMoney);
            }

            Bet twototwoBet = null;
            if (twototwoMoney < BankrollManager.Instance.MinStake)
            {
                twototwoBet = BetfairKom.Instance.placeBackBetBelowMin(mpCorrectScore.marketId, (int)BetfairKom.SCORESELECTIONIDS.TWOTOTWO, 0, twototwoOdds, twototwoMoney);
            }
            else
            {
                twototwoBet = BetfairKom.Instance.placeBackBet(mpCorrectScore.marketId, (int)BetfairKom.SCORESELECTIONIDS.TWOTOTWO, twototwoOdds, twototwoMoney);
            }

            Bet threetothreeBet = null;
            if (threetothreeMoney < BankrollManager.Instance.MinStake)
            {
                threetothreeBet = BetfairKom.Instance.placeBackBetBelowMin(mpCorrectScore.marketId, (int)BetfairKom.SCORESELECTIONIDS.THREETOTHREE, 0, threetothreeOdds, threetothreeMoney);
            }
            else
            {
                threetothreeBet = BetfairKom.Instance.placeBackBet(mpCorrectScore.marketId, (int)BetfairKom.SCORESELECTIONIDS.THREETOTHREE, threetothreeOdds, threetothreeMoney);
            }

            _ltdScratchColl.Add(mpMatchOdds.marketId, new BFLTDScratchStrategy());

            BankrollManager.Instance.localSubtract(laiablity + zerotozeroMoney + onetooneMoney + twototwoMoney + threetothreeMoney);
        }


        private double getBestBackOdds(MarketPrices prices, int selectionId)
        {
            try
            {
                foreach (RunnerPrices runnerPrices in prices.runnerPrices)
                {
                    if (runnerPrices.selectionId != selectionId)
                        continue;
                    if (runnerPrices.bestPricesToBack.Length == 0)
                        throw new Exception();

                    Price price = runnerPrices.bestPricesToBack[0];
                    return price.price;
                }
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteException(e);
            }
            throw new Exception();
        }

        private double getBestLayOdds(MarketPrices prices, int selectionId)
        {
            try
            {
                foreach (RunnerPrices runnerPrices in prices.runnerPrices)
                {
                    if (runnerPrices.selectionId != selectionId)
                        continue;
                    if (runnerPrices.bestPricesToLay.Length == 0)
                        throw new Exception();

                    Price price = runnerPrices.bestPricesToLay[0];
                    return price.price;
                }
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteException(e);
            }
            throw new Exception();
        }

        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion
    }
}
