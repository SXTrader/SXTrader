using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Threading;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXFastBet;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.common.Configurations;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.ttr.TradeStarter
{
    class AutoStarterBetAddedEventArgs : EventArgs
    {
        private long _marketId;
        private string _match;     

        public long MarketId
        {
            get { return _marketId; }
        }

        public AutoStarterBetAddedEventArgs(long marketId, string match)
        {
            _marketId = marketId;
            _match = match;
        }
    }

    class AutoStarterPrepObj : IDisposable
    {
        private TradeStarterConfigList _configList;
        private long _marketId;
        private string _match;
        private long _selectionId;
        private IScore _score;
        private TTRWatcher _watcher;
        private Thread _runner;
        private bool _disposed = false;
        
        private HistoricDataStatistic _statistic;
        private StatisticRuleChecker _statisticRuleChecker;

        public event EventHandler<AutoStarterBetAddedEventArgs> BetAddedEvent;

        public TradeStarterConfigList ConfigList
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

        public string Match
        {
            get
            {
                return _match;
            }
        }

        public AutoStarterPrepObj(long marketId, String match, IScore score, TTRWatcher watcher)
        {            
            _score = score;
            _score.IncreaseRef();
            _score.BackGoalEvent += new EventHandler<GoalBackEventArgs>(_score_BackGoalEvent);
            _score.GameEndedEvent += new EventHandler<net.sxtrader.bftradingstrategies.lsparserinterfaces.GameEndedEventArgs>(_score_GameEndedEvent);
            _score.PlaytimeTickEvent += new EventHandler<PlaytimeTickEventArgs>(_score_PlaytimeTickEvent);
            _score.RaiseGoalEvent += new EventHandler<GoalEventArgs>(_score_RaiseGoalEvent);

            _watcher = watcher;

            _marketId = marketId;            
            _match = match;

            _statistic = null;

            log("Constructing a new Inplay Trader Prepareing Object");
            if (_score != null)
            {
                log("Liveticker is linked");
            }

            SXMinutePulse.Instance.Pulse += Instance_Pulse;
            _runner = new Thread(runner);

        }

        void Instance_Pulse(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(preplayStarter));
        }

        ~AutoStarterPrepObj()
        {
            log("Deconstructing");
            _score.DecreaseRef();
            _score = null;
            SXMinutePulse.Instance.Pulse -= Instance_Pulse;
        }

        private void inplayStarter(Object stateInfo)
        {
            // Preplay => raus
            if (_score.StartDTS > DateTime.Now)
                return;

            // Ansonsten starten
            runner();

        }

        private void preplayStarter(Object stateInfo)
        {            
            // Inplay => rause
            if (_score.StartDTS < DateTime.Now)
                return;

            TTRConfigurationRW config = new TTRConfigurationRW();

            // Überprüfen, ob Preplay schon geprüft werden soll
            if (_score.StartDTS.Subtract(DateTime.Now).TotalMinutes > config.PreplayStartPoint)
                return;

            // Ansonsten starten
            runner();   
            
        }

        //TODO: umstellen auf Async?
        private async void runner()
        {
            //TimeSpan span = new TimeSpan(0, 0, 30);
            log("Check Rules for a match");

            if (_statistic == null)
            {
                //Statistik
                IHistoricDataService dataService = HistoricDataServiceFactory.getInstance(_score.TeamAId, _score.TeamBId, _score.TeamA, _score.TeamB);

                SAConfigurationRW config = new SAConfigurationRW();
                try
                {
                    _statistic = await dataService.GetStatistic(_score.TeamAId, _score.TeamBId, _score.TeamA, _score.TeamB, _score.League, config.NoOfData, config.AgeOfData);
                }
                catch (NoHistoricDataException)
                {
                    dataService = HistoricDataServiceFactory.getInstance(_score.TeamAId, _score.TeamBId, _score.TeamA, _score.TeamB);
                    try
                    {
                        _statistic = dataService.GetStatistic(_score.TeamAId, _score.TeamBId, _score.TeamA, _score.TeamB, _score.League, config.NoOfData, config.AgeOfData).Result;
                    }
                    catch (Exception e)
                    {
                        ExceptionWriter.Instance.WriteException(e);
                        _statistic = null;
                    }
                }
                catch (Exception e)
                {
                    ExceptionWriter.Instance.WriteException(e);
                    _statistic = null;
                }

                if (_statistic != null)
                    _statisticRuleChecker = new StatisticRuleChecker(_statistic, _score);
            }

            try
            {
                double odd = 0.0;
                TRADEMODE tradeMode = TRADEMODE.BACK;
                try
                {
                    foreach (TradeStarterConfigElement element in _configList)
                    {
                        if (_statistic == null)
                        {
                            log(String.Format("Statistic for match {0} is null", _match));
                            continue;
                        }

                        RuleChecker ruleChecker = RuleChecker.getInstance(_match, _statistic, _score, element.TradeConfig);
                        if (ruleChecker == null)
                        {
                            log("Did not get Rule Checker. Leaving");
                            continue;
                        }
                        if (!ruleChecker.check(element, _watcher, out odd, out tradeMode))
                            continue;

                        log("All checkst passed: Placing a lay Bet");
                        SXALMarket market = TTRHelper.GetMarketByTradeType(element.TradeType, _match);
                        if (market == null)
                        {
                            log("No market Found. Leaving!");
                            continue;
                        }
                        _marketId = market.Id;
                        _selectionId = TTRHelper.GetSelectionIdByTradeType(element.TradeType, market.Id);//TradeModuleToEnum.getSelectionIdFromTradeType(element.TradeType);
                        BetPlacer betPlacer = BetPlacer.getInstance(element, odd, _match, market.Id, _selectionId, _watcher, tradeMode);
                        if (betPlacer == null)
                        {
                            log("Did not get Bet Placer. Leaving!");
                            continue;
                        }
                        // Tradingbedingungen wurden erfüllt, also los
                        if (betPlacer.placeBet(/*odd, element.TradeConfig*/))
                        {
                            log("Placing of new Lay Bet succeeded");
                            log(String.Format("Setting Rule {0} to already executed", element.ElementNumber));

                            EventHandler<AutoStarterBetAddedEventArgs> handler = BetAddedEvent;
                            if (handler != null)
                            {
                                log("Notifying Listeners: New Bet Added");
                                handler(this, new AutoStarterBetAddedEventArgs(this.MarketId, this.Match));
                            }

                            element.AlreadyTraded = true;
                            break;
                        }
                        else
                        {
                            log("placing of new Back Bet failed");
                        }
                    }
                }
                catch (ASNoMarketPricesException) { log("Received a No Market Prices Exception: Stopping checking Rules until next iteration"); }
                catch (ASMarketSupendedException) { log("Received a Market Suspended Exception: Stopping checking Rules until next iteration"); }
                catch (ASMarketNotEnoughOddsException) { log("Received a Market Not Enought Selections Exception: Stopping checking Rules until next iteration"); }
                catch (ASMarketNoLayOddsException) { log("Received a Market No Lay Odds Exception: Stopping checking Rules until next iteration"); }
                catch (SXFastBetBelowMinStackException)
                {
                    log("Received a Fast Bet Below Min Stack Exception: Stopping checking Rules until next iteration");
                }
                catch (SXFastBetInsufficentFoundsExcpetion)
                {
                    log("Received a Fast Bet Insufficent Founds Exception: Stopping checking Rules until next iteration");
                }
                catch (InvalidOperationException ioe)
                {
                    ExceptionWriter.Instance.WriteException(ioe);
                }
            }
            catch (ThreadAbortException tae)
            {
                log("Retrieved a Thread Abort Exception");
                throw tae;
            }
        }

        void _score_RaiseGoalEvent(object sender, GoalEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _score_PlaytimeTickEvent(object sender, PlaytimeTickEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(inplayStarter));           
        }
       

        void _score_GameEndedEvent(object sender, net.sxtrader.bftradingstrategies.lsparserinterfaces.GameEndedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _score_BackGoalEvent(object sender, GoalBackEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void log(string message)
        {
            TradeLog.Instance.writeLog(_match, "TradeTheReaction", "StarterStarter", message);
        }
        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                if (!_disposed)
                {
                    DebugWriter.Instance.WriteMessage("AutoStarterPrepObj", String.Format("Disposing Starter for {0}", this._match));
                    if (_runner != null && SXThreadStateChecker.isStartedBackground(_runner))
                    //if (m_betStatusWatcherThread.ThreadState != (System.Threading.ThreadState.Background | System.Threading.ThreadState.Unstarted))
                    {
                        log("Stopping Starter Thread");
                        _runner.Abort();
                        _runner.Join();
                    }
                    _disposed = true;
                }
            }
            catch (Exception e)
            {
                ExceptionWriter.Instance.WriteException(e);
            }
        }
        #endregion
    }

    class ASNoMarketPricesException : Exception { }
    class ASMarketSupendedException : Exception { }
    class ASMarketNotEnoughOddsException : Exception { }
    class ASMarketNoLayOddsException : Exception { }

}
