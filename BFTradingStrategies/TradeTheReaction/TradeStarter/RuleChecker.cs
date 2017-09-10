using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.TradeStarter
{
    abstract class RuleChecker 
    {
        protected String _match = String.Empty;
        protected HistoricDataStatistic _statistic;
        protected IScore _score;
        protected StatisticRuleChecker _statisticChecker;
        protected TTRConfigurationRW _config;

        public static RuleChecker getInstance(String match, HistoricDataStatistic statistic, IScore score, TTRConfigurationRW config)
        {
            if (score.StartDTS < DateTime.Now)
                return new InplayRuleChecker(match, statistic, score, config);
            else
                return new PreplayRuleChecker(match, statistic, score, config);
        }

        public RuleChecker(string match, HistoricDataStatistic statistic, IScore score, TTRConfigurationRW config)
        {
            _match = match;
            _statistic = statistic;
            _score = score;
            _statisticChecker = new StatisticRuleChecker(_statistic, _score);
            _config = config;
        }

        public abstract bool check(TradeStarterConfigElement element,TTRWatcher watcher, out double odds, out TRADEMODE tradeMode);

        protected bool checkGoalSum(TradeStarterConfigElement element)
        {
            if (element.LoGoalSum == -1 && element.HiGoalSum == -1)
                return true;
            if (!(_score.ScoreA + _score.ScoreB >= element.LoGoalSum && _score.ScoreA + _score.ScoreB <= element.HiGoalSum))
            {
                log(String.Format("The GoalSum {0} is not between the configured interval {1} and {2}. Leaving", _score.ScoreA + _score.ScoreB, element.LoGoalSum, element.HiGoalSum));
                return false;
            }
            return true;
        }

        protected bool checkPrerequisites(TradeStarterConfigElement element, TTRWatcher watcher)
        {
            if (element.RequiredTrade != TRADETYPE.UNASSIGNED)
            {
                //Überprüfe, ob der benötigte Trade existiert.
                if (!watcher.ContainsKey(_match))
                {
                    log(String.Format("The required Trade {0} does not exist for match {1}. Leaving!", element.RequiredTrade.ToString(), _match));
                    return false;
                }

                SXALMarket tmpMarket = TTRHelper.GetMarketByTradeType(element.RequiredTrade, _match);

                if (tmpMarket == null)
                {
                    log(String.Format("The required Trade {0} does not exist for match {1}. Leaving!", element.RequiredTrade.ToString(), _match));
                    return false;
                }

                if (!watcher[_match].ContainsKey(element.RequiredTrade))
                {
                    log(String.Format("The required Trade {0} does not exist for match {1}. Leaving!", element.RequiredTrade.ToString(), _match));
                    return false;
                }

                //Überprüfe, ob benötigter Trade im passenden Zustand ist
                switch (element.RequiredTradeState)
                {
                    case TRADESTATE.UNASSIGNED:
                        log(String.Format("The Required Status for Trade {0} in match {1} has to be different as UNASSIGNED.Leaving!", element.RequiredTrade.ToString(), _match));
                        return false;
                    case TRADESTATE.NOMATTER:
                        //Immer gültig
                        break;
                    case TRADESTATE.OPENED:
                        if(watcher[_match][element.RequiredTrade].TradeState.GetType() != typeof(TradeMoneyOpenState))
                        {
                            log(String.Format("The Required Status for Trade {0} in match {1} is not OPENED. Leaving!", element.RequiredTrade.ToString(), _match));
                            return false;
                        }
                        break;
                    case TRADESTATE.HEDGED:
                        if (watcher[_match][element.RequiredTrade].TradeState.GetType() != typeof(TradeMoneyHedgedState))
                        {
                            log(String.Format("The Required Status for Trade {0} in match {1} is not HEDGED. Leaving!", element.RequiredTrade.ToString(), _match));
                            return false;
                        }
                        break;
                    case TRADESTATE.GREENED:
                        if (watcher[_match][element.RequiredTrade].TradeState.GetType() != typeof(TradeMoneyGreenedState))
                        {
                            log(String.Format("The Required Status for Trade {0} in match {1} is not GREENED. Leaving!", element.RequiredTrade.ToString(), _match));
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        protected bool checkAlreadyTraded(TradeStarterConfigElement element)
        {
            // Wurde bereits gehandelt
            if (element.AlreadyTraded)
            {
                log("Rule has already been executed. Leaving!");
                return false;
            }

            return true;
        }

        protected bool checkForExistingTrade(TradeStarterConfigElement element, TRADETYPE tradeType, TTRWatcher watcher)
        {
            // Überprüfen, ob schon ein Trade vorhanden
            TradeCollection coll = null;
            if (watcher.ContainsKey(_match))
                coll = watcher[_match];
            if (coll != null)
            {
                if (tradeType!= TRADETYPE.UNASSIGNED)
                {
                    if (coll.ContainsKey(tradeType))
                    {
                        // Auch der Richtige Typ?
                        if (TradeModuleToEnum.getEnum(coll[tradeType]) == element.TradeType)
                        {
                            // Ist überhaupt ein Trade erlaubt?
                            if (!element.SettledAllowed && !element.UnsettledAllowed)
                            {
                                log("Rule does not allow an existing trade. Leaving!");
                                return false;
                            }

                            ITrade trade = coll[tradeType];

                            // Nicht abgeschlossene Trades
                            if (!element.UnsettledAllowed && trade.TradeState.GetType() != typeof(TradeMoneyGreenedState))
                            {
                                log("Rule does not allow unsettled Trades. Leaving!");
                                return false;
                            }
                            if (!element.SettledAllowed && trade.isSettled()/*trade.TradeState.GetType() == typeof(TradeMoneyGreenedState)*/)
                            {
                                log("Rule does not allow settled Trades. Leaving!");
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        protected bool checkStatistics(TradeStarterConfigElement element)
        {
            // Allgemeine Statistiken
            if (element.Statistics.Length > 0)
            {
                if (_statistic == null)
                {
                    log("No Statistic loaded. Leaving!");
                    return false;
                }

                foreach (StatisticSelectionElement statisticElement in element.Statistics)
                {
                    if (!_statisticChecker.check(statisticElement))
                    {
                        log(String.Format("Statistic Rule '{0}' failed. Leaving!", statisticElement.ToString()));
                        return false;
                    }
                }

            }
            return true;
        }

        protected bool checkPlaytime(TradeStarterConfigElement element)
        {
            if (!(_score.Playtime >= element.LoPlaytime && _score.Playtime <= element.HiPlaytime))
            {
                log(String.Format("The playtime {0} is not between the configured interval {1} and {2}. Leaving", _score.Playtime, element.LoPlaytime, element.HiPlaytime));
                return false;
            }
            return true;
        }

        protected bool checkScore(TradeStarterConfigElement element)
        {
            String score = _score.getScore();
            log(String.Format("Score is {0}", score));
            /*
            if (_score.getScoreState() == SCORESTATE.undraw)
            {
                log("Score State is undraw. Leaving");
                return false;
            }
             * */
            bool bFound = false;
            TTRScores configScore = TTRScores.UNASSIGNED;
            switch (score)
            {
                case "0 - 0":
                    configScore = TTRScores.ZEROZERO;
                    break;
                case "0 - 1":
                    configScore = TTRScores.ZEROONE;
                    break;
                case "0 - 2":
                    configScore = TTRScores.ZEROTWO;
                    break;
                case "0 - 3":
                    configScore = TTRScores.ZEROTHREE;
                    break;
                case "1 - 0":
                    configScore = TTRScores.ONEZERO;
                    break;
                case "1 - 1":
                    configScore = TTRScores.ONEONE;
                    break;
                case "1 - 2":
                    configScore = TTRScores.ONETWO;
                    break;
                case "1 - 3":
                    configScore = TTRScores.ONETHREE;
                    break;
                case "2 - 0":
                    configScore = TTRScores.TWOZERO;
                    break;
                case "2 - 1":
                    configScore = TTRScores.TWOONE;
                    break;
                case "2 - 2":
                    configScore = TTRScores.TWOTWO;
                    break;
                case "2 - 3":
                    configScore = TTRScores.TWOTHREE;
                    break;
                case "3 - 0":
                    configScore = TTRScores.THREEZERO;
                    break;
                case "3 - 1":
                    configScore = TTRScores.THREEONE;
                    break;
                case "3 - 2":
                    configScore = TTRScores.THREETWO;
                    break;
                case "3 - 3":
                    configScore = TTRScores.THREETHREE;
                    break;
            }
            foreach (TTRScores s in element.Scores)
            {
                if (s == configScore)
                {
                    log(String.Format("Score {0} is withing the configured scores", score));
                    bFound = true;
                    break;
                }
            }

            if (element.Scores.Length == 0)
            {
                //TODO: Ist das wirklich so sinnvoll? Was ist mit Konfigurationsfehlern?
                bFound = true;
            }

            if (!bFound)
            {
                log(String.Format("Score {0} not within the configured values. Leaving", score));
                return false;
            }
            return true;
        }

        protected bool checkRedCard(TradeStarterConfigElement element)
        {
            //Ggf. Rote Karten überprüfen
            if (element.RedCardWatch)
            {
                log("Red Card observation is activated");
                log(String.Format("Configured Red Card Rule is {0}", element.RedCardWatchState.ToString()));
                switch (element.RedCardWatchState)
                {
                    case TTRRedCardWatch.NOREDCARD:
                        {
                            if (_score.RedA > 0 || _score.RedB > 0)
                            {
                                log("More than a red card for any team. Leaving");
                                return false;
                            }
                            break;
                        }
                    case TTRRedCardWatch.EQUALCARD:
                        {
                            if (_score.RedA != _score.RedB)
                            {
                                log("Both team have not same count of red cards. Leaving");
                                return false;
                            }
                            break;
                        }
                    case TTRRedCardWatch.TEAMAMORE:
                        {
                            if (_score.RedA <= _score.RedB)
                            {
                                log("Team A has less red cards then Team B. Leaving");
                                return false;
                            }
                            break;
                        }
                    case TTRRedCardWatch.TEAMBMORE:
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
            return true;
        }

        protected bool checkMarket(double oddsLo, double oddsHi, double volumeLo, double volumeHi,
            TRADETYPE tradeType, SXALMarket bfMarket, out double odds, out TRADEMODE tradeMode)
        {
            odds = 0.0;
            tradeMode = TRADEMODE.BACK;
            if (bfMarket == null)
                return false;

                        

            //Quoten und Marktvolumen
            log("Reading new market prices");
            try
            {
                SXALMarketPrices marketPrices = SXALKom.Instance.getMarketPrices(bfMarket.Id, true);
            
                // Keine Quoten
                if (marketPrices == null)
                {
                    log("There are no market prices: Leaving");
                    throw new ASNoMarketPricesException();
                }

                // Markt pausiert
                // Falls Markt pausiert => abwarten
                if (marketPrices.MarketStatus == SXALMarketStatusEnum.SUSPENDED)
                {
                    log("Market is suspended: Leaving");
                    throw new ASMarketSupendedException();
                }

                // Genug Quoten vorhanden?
                if (marketPrices.RunnerPrices.Length < 1)
                {
                    log("Not enough selections in market: Leaving");
                    throw new ASMarketNotEnoughOddsException();
                }

                // Quoten im Rahmen?
                log("Reading Prices");
                //TODO: SelektionID!
                long selectionId = TTRHelper.GetSelectionIdByTradeType(tradeType, bfMarket.Id);
                SXALRunnerPrices runnerPrice = null;
                foreach (SXALRunnerPrices runnerPrices in marketPrices.RunnerPrices)
                {
                    if (runnerPrices.SelectionId == selectionId)
                    {
                        runnerPrice = runnerPrices;
                        break;
                    }
                }

                if ((tradeMode = TTRHelper.IsTradeTypeBackMode(tradeType)) == TRADEMODE.BACK)
                    return checkBackMarket(oddsLo, oddsHi, volumeLo, volumeHi, runnerPrice, out odds);
                else
                    return checkLayMarket(oddsLo, oddsHi, volumeLo, volumeHi, runnerPrice, out odds);
            }
            catch (SXALMarketDoesNotExistException mdnee)
            {
                ExceptionWriter.Instance.WriteException(mdnee);                
                String msg = String.Format("Market {0} for match {1} does not exist! ExpectionMessage {2}. Skipping Rule Check!",bfMarket.Id, _match, mdnee.Message);
                log(msg);               

                return false;
            }
            catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
            {
                ExceptionWriter.Instance.WriteException(mnsnae);                
                String msg = String.Format("Market {0}  for match {1} is neither suspended nor active! ExpectionMessage {2}. Leaving Trade!", bfMarket.Id, _match, mnsnae.Message);
                log(msg);               

                return false;
            }
            catch (SXALThrottleExceededException tee)
            {
                log("Limit for requestion market prices exceeded. Leaving and Retrying");
                return false;
            }
        }

        protected bool checkBackMarket(double oddsLo, double oddsHi, double volumeLo, double volumeHi,
            SXALRunnerPrices runnerPrice, out double odds)
        {
            odds = 0.0;
            SXALPrice priceBack = null;
            if (_config.UseBackLayTicks)
            {
                if (runnerPrice.BestPricesToLay.Length > 0)
                {
                    priceBack = runnerPrice.BestPricesToLay[0];
                }
                else
                {
                    log("Not enought Lay Prices: Leaving");
                    throw new ASMarketNoLayOddsException();
                }
            }
            else
            {
                if (runnerPrice.BestPricesToBack.Length > 0)
                {
                    priceBack = runnerPrice.BestPricesToBack[0];                    
                }
                else
                {
                    log("Not enought Back Prices: Leaving");
                    throw new ASMarketNoLayOddsException();
                }
            }

            log(String.Format("Odd for back is {0}", priceBack.Price));
            if (!(priceBack.Price >= oddsLo && priceBack.Price <= oddsHi))
            {
                log(String.Format("Odd {0} is not within interval of {1} and {2}: Leaving", priceBack.Price, oddsLo, oddsHi));
                return false;
            }

            //Passt Marktvolumen?
            if (!(runnerPrice.TotalAmountMatched >= volumeLo && runnerPrice.TotalAmountMatched <= volumeHi))
            {
                log(String.Format("Market Volume {0} is not within interval of {1} and {2}: Leaving", runnerPrice.TotalAmountMatched, volumeLo, volumeHi));
                return false;
            }
            
            odds = priceBack.Price;

            if (_config.UseBackLayTicks)
            {
                log("The Usage of Back/Lay Ticks is set");
                log(String.Format("Ticks are {0}", _config.BackLayTicks));

                //ACHTUNG: Das Dekrement muss jedes mal frisch ausgerechnet werden,
                // da es sich bei überschreitung einer Intervalgrenze verändern kann
                for (int i = 0; i < _config.BackLayTicks; i++)
                {
                    double decrement = (double)SXALKom.Instance.getValidOddIncrement((decimal)odds);
                    log(String.Format("Odds will be adjusted by the absolute value of {0}", decrement));
                    odds = odds - decrement;
                    odds = (double)SXALKom.Instance.validateOdd((decimal)odds);
                }      
                
                log(String.Format("Adjusted Odds are {0}", odds));
            }

            return true;
        }

        protected bool checkLayMarket(double oddsLo, double oddsHi, double volumeLo, double volumeHi,
            SXALRunnerPrices runnerPrice, out double odds)
        {
            odds = 0.0;
            SXALPrice priceLay = null;
            if (_config.UseBackLayTicks)
            {
                if (runnerPrice.BestPricesToBack.Length > 0)
                {
                    priceLay = runnerPrice.BestPricesToBack[0];
                }
                else
                {
                    log("Not enought Lay Prices: Leaving");
                    throw new ASMarketNoLayOddsException();
                }
            }
            else
            {
                if (runnerPrice.BestPricesToLay.Length > 0)
                {
                    priceLay = runnerPrice.BestPricesToLay[0];                    
                }
                else
                {
                    log("Not enought Back Prices: Leaving");
                    throw new ASMarketNoLayOddsException();
                }
            }

            log(String.Format("Odd for back is {0}", priceLay.Price));
            if (!(priceLay.Price >= oddsLo && priceLay.Price <= oddsHi))
            {
                log(String.Format("Odd {0} is not within interval of {1} and {2}: Leaving", priceLay.Price, oddsLo, oddsHi));
                return false;
            }

            //Passt Marktvolumen?
            if (!(runnerPrice.TotalAmountMatched > volumeLo && runnerPrice.TotalAmountMatched < volumeHi))
            {
                log(String.Format("Market Volume {0} is not within interval of {1} and {2}: Leaving", runnerPrice.TotalAmountMatched, volumeLo, volumeHi));
                return false;
            }

            odds = priceLay.Price;

            if (_config.UseBackLayTicks)
            {
                log("The Usage of Back/Lay Ticks is set");
                log(String.Format("Ticks are {0}", _config.BackLayTicks));
                //ACHTUNG: Das Inkrement muss jedes mal frisch ausgerechnet werden,
                // da es sich bei überschreitung einer Intervalgrenze verändern kann
                for (int i = 0; i < _config.BackLayTicks; i++)
                {
                    double increment = (double)SXALKom.Instance.getValidOddIncrement((decimal)odds);
                    log(String.Format("Odds will be adjusted by the absolute value of {0}", increment));
                    odds = odds + increment;
                    odds = (double)SXALKom.Instance.validateOdd((decimal)odds);
                }                
                
                log(String.Format("Adjusted Odds are {0}", odds));
            }

            return true;
        }


        

        protected void log(string message)
        {
            TradeLog.Instance.writeLog(_match, "TradeTheReaction", "Starter", message);
        }
    }

    class InplayRuleChecker : RuleChecker
    {
        public InplayRuleChecker(string match, HistoricDataStatistic statistic, IScore score, TTRConfigurationRW config )
            : base(match, statistic, score, config)
        {                       
        }

        public override bool check(TradeStarterConfigElement element, TTRWatcher watcher, out double odds, out TRADEMODE tradeMode)
        {
            odds = 0.0;
            tradeMode = TRADEMODE.BACK;
            //Keine Inplay Check
            if (!element.Inplay)
                return false;

            log(String.Format("Inplay Checking rule {0} ", element.ElementNumber));

            try
            {

                if (!checkPrerequisites(element, watcher))
                    return false;

                if (!checkAlreadyTraded(element))
                    return false;

                if (!checkPlaytime(element))
                    return false;

                if (!checkGoalSum(element))
                    return false;

                if (!checkScore(element))
                    return false;

                if (!checkRedCard(element))
                    return false;


                SXALMarket bfMarket = TTRHelper.GetMarketByTradeType(element.TradeType, _match);                

                if (bfMarket != null)
                {
                    if (!checkForExistingTrade(element, element.TradeType, watcher))
                        return false;
                }


                if (!checkStatistics(element))
                    return false;


                if (!checkMarket(element.LoOdds, element.HiOdds, element.LoVolume, element.HiVolume,
                    element.TradeType, bfMarket, out odds, out tradeMode))
                    return false;

                return true;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                return false;
            }
        }
    }

    class PreplayRuleChecker : RuleChecker
    {        
        
        public PreplayRuleChecker(string match, HistoricDataStatistic statistic, IScore score, TTRConfigurationRW config )
            : base(match, statistic, score, config)
        {                       
        }

        public override bool check(TradeStarterConfigElement element, TTRWatcher watcher, out double odds, out TRADEMODE tradeMode)
        {
            odds = 0.0;
            tradeMode = TRADEMODE.BACK;
            try
            {
                //Kein Preplay
                if (!element.Preplay)
                    return false;


                TTRConfigurationRW config = new TTRConfigurationRW();

                // Überprüfen, ob Preplay schon geprüft werden soll
                if (_score.StartDTS.Subtract(DateTime.Now).TotalMinutes > config.PreplayStartPoint)
                    return false;

                log(String.Format("Preplay Checking rule {0} ", element.ElementNumber));


                if (!checkAlreadyTraded(element))
                    return false;


                SXALMarket bfMarket = TTRHelper.GetMarketByTradeType(element.TradeType, _match);//null;                

                if (!checkPrerequisites(element, watcher))
                    return false;

                if (bfMarket != null)
                {
                    if (!checkForExistingTrade(element, element.TradeType, watcher))
                        return false;
                }

                if (!checkForExistingTrade(element, element.TradeType, watcher))
                    return false;


                if (!checkStatistics(element))
                    return false;


                if (!checkMarket(element.PreplayLoOdds, element.PreplayHiOdds, element.PreplayLoVolume, element.PreplayHiVolume,
                    element.TradeType, bfMarket, out odds, out tradeMode))
                    return false;

                return true;
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
                return false;
            }
        }
    }

    static internal class TradeModuleToEnum
    {
        public static TRADETYPE getEnum(ITrade trade)
        {
            Type type = trade.GetType();
            if (type == typeof(ScorelineTrade00))
                return TRADETYPE.SCORELINE00;
            else if (type == typeof(OverUnderTrade))
            {
                OverUnderTrade tmp = trade as OverUnderTrade;
                switch (tmp.OverUnderType)
                {
                    case OUTYPE.OVER:
                        {
                            switch (tmp.OverUnderValue)
                            {
                                case OUVALUE.ZEROFIVE:
                                    return TRADETYPE.OVER05;
                                case OUVALUE.ONEFIVE:
                                    return TRADETYPE.OVER15;
                                case OUVALUE.TWOFIVE:
                                    return TRADETYPE.OVER25;
                                case OUVALUE.THREEFIVE:
                                    return TRADETYPE.OVER35;
                                case OUVALUE.FOURFIVE:
                                    return TRADETYPE.OVER45;
                                case OUVALUE.FIVEFIVE:
                                    return TRADETYPE.OVER55;
                                case OUVALUE.SIXFIVE:
                                    return TRADETYPE.OVER65;
                                case OUVALUE.SEVENFIVE:
                                    return TRADETYPE.OVER75;
                                case OUVALUE.EIGHTFIVE:
                                    return TRADETYPE.OVER85;
                            }
                        }
                        break;
                }
            }
            else if (type == typeof(CorrectScoreTrade))
            {
                CorrectScoreTrade tmp = trade as CorrectScoreTrade;
                return tmp.TradeType;
            }
            return TRADETYPE.UNASSIGNED;
        }

       
    }

    class StatisticRuleChecker
    {
        private HistoricMatchList _directBoth;
        private HistoricMatchList _directTeamAHome;
        private HistoricMatchList _directTeamAAway;
        private HistoricMatchList _teamABoth;
        private HistoricMatchList _teamAHome;
        private HistoricMatchList _teamAAway;
        private HistoricMatchList _teamBBoth;
        private HistoricMatchList _teamBHome;
        private HistoricMatchList _teamBAway;
        private ulong _teamAId;
        private ulong _teamBId;

        public StatisticRuleChecker(HistoricDataStatistic overallStatistics, IScore score)
        {
            if (score == null)
                return;
            _teamAId = score.TeamAId;
            _teamBId = score.TeamBId;

            _directBoth = overallStatistics.Direct;
            _teamABoth = overallStatistics.TeamA;
            _teamBBoth = overallStatistics.TeamB;

            _directTeamAHome = new HistoricMatchList();
            _directTeamAAway = new HistoricMatchList();
            foreach (LSHistoricMatch match in _directBoth)
            {
                if (match.TeamAId == score.TeamAId)
                    _directTeamAHome.Add(match);

                if (match.TeamBId == score.TeamAId)
                    _directTeamAAway.Add(match);


            }


            _teamAHome = new HistoricMatchList();
            _teamAAway = new HistoricMatchList();
            foreach (LSHistoricMatch match in _teamABoth)
            {
                if (match.TeamAId == score.TeamAId)
                    _teamAHome.Add(match);

                if (match.TeamBId == score.TeamAId)
                    _teamAAway.Add(match);
            }

            _teamBHome = new HistoricMatchList();
            _teamBAway = new HistoricMatchList();

            foreach (LSHistoricMatch match in _teamBBoth) 
            {
                if (match.TeamAId == score.TeamBId)
                    _teamBHome.Add(match);

                if (match.TeamBId == score.TeamBId)
                    _teamBAway.Add(match);
            }

        }

        public bool check(StatisticSelectionElement statisticRule)
        {
            HistoricMatchList theList = null;
            ulong teamId = 0;
            switch (statisticRule.Team)
            {
                case STATISTICTEAM.BOTH:
                    teamId = _teamAId;
                    switch (statisticRule.HomeAway)
                    {
                        case STATISTICHOMEAWAY.BOTH:                            
                            theList = _directBoth;
                            break;
                        case STATISTICHOMEAWAY.HOME:                            
                            theList = _directTeamAHome;
                            break;
                        case STATISTICHOMEAWAY.AWAY:                            
                            theList = _directTeamAAway;
                            break;
                    }
                    break;
                case STATISTICTEAM.TEAMA:
                    teamId = _teamAId;
                    switch (statisticRule.HomeAway)
                    {
                        case STATISTICHOMEAWAY.BOTH:
                            theList = _teamABoth;
                            break;
                        case STATISTICHOMEAWAY.HOME:
                            theList = _teamAHome;
                            break;
                        case STATISTICHOMEAWAY.AWAY:
                            theList = _teamAAway;
                            break;
                    }
                    break;
                case STATISTICTEAM.TEAMB:
                    teamId = _teamBId;
                    switch (statisticRule.HomeAway)
                    {
                        case STATISTICHOMEAWAY.BOTH:
                            theList = _teamBBoth;
                            break;
                        case STATISTICHOMEAWAY.HOME:
                            theList = _teamBHome;
                            break;
                        case STATISTICHOMEAWAY.AWAY:
                            theList = _teamBAway;
                            break;
                    }
                    break;
            }

            if (theList != null)
            {
                return internalStatisticCheck(statisticRule, theList, teamId);
            }
            return false;
        }

        private bool internalStatisticCheck(StatisticSelectionElement statisticRule, HistoricMatchList theList, ulong teamId)
        {
            switch (statisticRule.Statistic)
            {
                case STATISTICTYPE.AVGFIRSTGOAL:
                    if (theList.AvgFirstGoalMinute >= statisticRule.LoRange && theList.AvgFirstGoalMinute <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.AVGGOALS:
                    if (theList.AvgGoals >= statisticRule.LoRange && theList.AvgGoals <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.DRAW:                    
                    if (theList.getWLD(teamId).DrawPercent >= statisticRule.LoRange && theList.getWLD(teamId).DrawPercent <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.EARLIESTFIRSTGOAL:
                    if (theList.EarlierstFirstGoal >= statisticRule.LoRange && theList.EarlierstFirstGoal <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.LATESTFIRSTGOAL:
                    if (theList.LatestFirstGoal >= statisticRule.LoRange && theList.LatestFirstGoal <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.LOSS:
                    if (theList.getWLD(teamId).LossPercent >= statisticRule.LoRange && theList.getWLD(teamId).LossPercent <= statisticRule.HiRange)
                        return true;
                    else return false;                    
                case STATISTICTYPE.OVER05:
                    if (theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ZEROPOINTFIVE).OverPercent >= statisticRule.LoRange &&
                        theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ZEROPOINTFIVE).OverPercent <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.OVER15:
                    if (theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ONEPOINTFIVE).OverPercent >= statisticRule.LoRange &&
                        theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.ONEPOINTFIVE).OverPercent <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.OVER25:
                    if (theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.TWOPOINTFIVE).OverPercent >= statisticRule.LoRange &&
                        theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.TWOPOINTFIVE).OverPercent <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.OVER35:
                    if (theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.THREEPOINTFIVE).OverPercent >= statisticRule.LoRange &&
                        theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.THREEPOINTFIVE).OverPercent <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.OVER45:
                    if (theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.FOURPOINTFIVE).OverPercent >= statisticRule.LoRange &&
                        theList.getOverUnder(HistoricOverUnderData.OVERUNDERTYPES.FOURPOINTFIVE).OverPercent <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE00:
                    if (theList.getScorePercentage(SCORES.ZEROZERO, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.ZEROZERO, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE01:
                    if (theList.getScorePercentage(SCORES.ZEROONE, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.ZEROONE, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE02:
                    if (theList.getScorePercentage(SCORES.ZEROTWO, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.ZEROTWO, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE03:
                    if (theList.getScorePercentage(SCORES.ZEROTHREE, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.ZEROTHREE, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE10:
                    if (theList.getScorePercentage(SCORES.ONEZERO, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.ONEZERO, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE11:
                    if (theList.getScorePercentage(SCORES.ONEONE, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.ONEONE, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE12:
                    if (theList.getScorePercentage(SCORES.ONETWO, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.ONETWO, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE13:
                    if (theList.getScorePercentage(SCORES.ONETHREE, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.ONETHREE, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE20:
                    if (theList.getScorePercentage(SCORES.TWOZERO, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.TWOZERO, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE21:
                    if (theList.getScorePercentage(SCORES.TWOONE, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.TWOONE, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE22:
                    if (theList.getScorePercentage(SCORES.TWOTWO, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.TWOTWO, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE23:
                    if (theList.getScorePercentage(SCORES.TWOTHREE, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.TWOTHREE, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE30:
                    if (theList.getScorePercentage(SCORES.THREEZERO, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.THREEZERO, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE31:
                    if (theList.getScorePercentage(SCORES.THREEONE, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.THREEONE, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE32:
                    if (theList.getScorePercentage(SCORES.THREETWO, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.THREETWO, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.SCORE33:
                    if (theList.getScorePercentage(SCORES.THREETHREE, teamId) >= statisticRule.LoRange &&
                        theList.getScorePercentage(SCORES.THREETHREE, teamId) <= statisticRule.HiRange)
                        return true;
                    else return false;
                case STATISTICTYPE.WIN:
                    if (theList.getWLD(teamId).WinPercent >= statisticRule.LoRange && theList.getWLD(teamId).WinPercent <= statisticRule.HiRange)
                        return true;
                    else return false;      
                case STATISTICTYPE.NOOFDATA:
                    if (theList.NoOfData >= statisticRule.LoRange && theList.NoOfData <= statisticRule.HiRange)
                        return true;
                    else return false;
            }
            return false;
        }
    }
}
