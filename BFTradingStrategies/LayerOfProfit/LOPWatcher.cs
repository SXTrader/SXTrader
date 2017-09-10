using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.tippsters.LOP.Configuration;
using net.sxtrader.bftradingstrategies.tippsters.LOP.DataModel;
using net.sxtrader.bftradingstrategies.tippsters.LOP.MailInterface;
using net.sxtrader.bftradingstrategies.tippsters.Trade;
using net.sxtrader.muk;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace net.sxtrader.bftradingstrategies.tippsters
{
    class LOPWatcher : IBFTSCommon, IDisposable
    {
        /// <summary>
        /// Singleton Instanz
        /// </summary>
        private static volatile LOPWatcher _instance;
        /// <summary>
        /// Synchronisierter Zugriff auf die Singleton-Instanz
        /// </summary>
        private static Object _syncRoot = "syncRoot";

        private LOPMails _lopMails;
        private LOPTradeList _trades;
        private LOPTipp _lastTipp;
        private bool _disposed = false;
        private System.Timers.Timer _checkTimer;
        private Object _syncMailCheck = "syncMailCheck";

        public delegate void LOPNoSelectionDayHandler(object sender, LOPNoSelectionDayEventArgs e);
        public event LOPNoSelectionDayHandler NoSelectionDayEvent;

        public delegate void LOPNoTippFoundHandler(object sender, LOPNoTippFoundEventArgs e);
        public event LOPNoTippFoundHandler NoTippFoundEvent;

        public delegate void LOPTippPlacedHandler(object sender, LOPTippAddedEventArgs e);
        public event LOPTippPlacedHandler TippPlacedEvent;

        public delegate void LOPTippSettledHandler(object sender, LOPSettledEventArgs e);
        public event LOPTippSettledHandler TippSettledEvent;

        public delegate void LOPTippUpdateHandler(object sender, LOPTippAddedEventArgs e);
        public event LOPTippUpdateHandler TippUpdateEvent;

        public static LOPWatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new LOPWatcher();
                    }
                }

                return _instance;
            }
        }

        private LOPWatcher()
        {
            
            _lopMails = new LOPMails();
            _trades = new LOPTradeList();
            LOPConfigurationRW config = new LOPConfigurationRW();
            _checkTimer = new System.Timers.Timer(new TimeSpan(0, config.MailCheckInterval, 0).TotalMilliseconds);
            _checkTimer.Elapsed += _checkTimer_Elapsed;
            _checkTimer.Start();
                       
            SXALBetWatchdog.Instance.BetsUpdated += Instance_BetsUpdated;
            SXMinutePulse.Instance.Pulse += Instance_Pulse;
        }

        void Instance_Pulse(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(infoUpdater));
        }

        private void infoUpdater(Object stateInfo)
        {
            for (int i = 0; i < _trades.Count; i++)
            {
                if (TippUpdateEvent != null)
                {
                    TippUpdateEvent(this, new LOPTippAddedEventArgs(_trades[i].EventDate, _trades[i].Race, _trades[i].Horse, _trades[i].Currency, _trades[i].MatchedAmount,
                                        _trades[i].UnmatchedAmount, _trades[i].Risk, _trades[i].PotentialRisk, _trades[i].AvgPrice, _trades[i].TradeState));
                }
            }
        }

        public void triggerCheck()
        {
            _checkTimer_Elapsed(this, null);
        }

        private void initialLoadFromLazerus()
        {
            try
            {
                //1. Lade Einträge zu Modul aus Lazerus.
                LazerusList list = Lazerus.Instance.getEntriesByModuleName(SXALKom.Instance.getExchangeName(), LayerOfProfit.strName);

                //2. Überprüfe jeden Eintrag
                foreach (LazerusEntry entry in list)
                {
                    foreach (LazerusEntry.LazerusNode node in entry.ExistTrades)
                    {
                        List<SXALBet> listBet = new List<SXALBet>();
                        foreach (long betId in node.BetIds)
                        {
                            SXALBet bet = SXALKom.Instance.getBetDetail(betId);
                            switch (bet.BetStatus)
                            {
                                case SXALBetStatusEnum.M:
                                case SXALBetStatusEnum.MU:
                                case SXALBetStatusEnum.U:
                                    //Wette für neuen Trade
                                    listBet.Add(bet);
                                    break;
                                case SXALBetStatusEnum.S:
                                    //Wette für Trade
                                    //Wenn alle Wetten zu einen Trade 'settled' sind, so wird automatisch ein Historisierungseintrag erstellt
                                    listBet.Add(bet);
                                    break;
                                case SXALBetStatusEnum.C:
                                case SXALBetStatusEnum.L:
                                case SXALBetStatusEnum.V:
                                    //entferne Eintrag aus Lazerus
                                    Lazerus.Instance.removeBetFromTrade(SXALKom.Instance.getExchangeName(), LayerOfProfit.strName, Lazerus.ENTRYMODES.EXISTMODE, bet.BetId);
                                    break;
                            }

                        }

                        if (listBet.Count > 0)
                        {
                            //Neuen Trade erstellen
                            //TODO: Woher die Version nehmen?                            
                            LOPTrade trade = LOPTrade.CreateTrade(listBet.ToArray(), new LOPConfigurationRW(node.Settings));
                            if (trade != null)
                            {
                                trade.TradeSettledEvent += trade_TradeSettledEvent;
                                trade.ExceptionMessageEvent += lopTrade_ExceptionMessageEvent;
                                trade.MessageEvent += lopTrade_MessageEvent;
                                _trades.Add(trade);                                
                                if(TippPlacedEvent!=null)
                                {
                                    TippPlacedEvent(this, new LOPTippAddedEventArgs(trade.EventDate, trade.Race, trade.Horse, trade.Currency, trade.MatchedAmount, 
                                        trade.UnmatchedAmount, trade.Risk, trade.PotentialRisk, trade.AvgPrice, trade.TradeState));
                                }
                                //TODO: Benachrichtigung schicken
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
        }

        void trade_TradeSettledEvent(object sender, LOPTradeSettledEventArgs e)
        {            
            for (int i = 0; i < _trades.Count; i++)
            {
                if (_trades[i] == sender)
                {
                    if (TippSettledEvent != null)
                    {
                        TippSettledEvent(this, new LOPSettledEventArgs(_trades[i].EventDate, _trades[i].Race, _trades[i].Horse, LOPTrade.TRADESTATE.COMPLETED));
                    }

                    _lastTipp = new LOPTipp();
                    _lastTipp.EventDate = _trades[i].EventDate;
                    _lastTipp.Race = _trades[i].Race;
                    _lastTipp.Horse = _trades[i].Horse;

                    //Historisierung
                    _trades.RemoveAt(i);


                    break;
                }
            }
        }

        private void Instance_BetsUpdated(object sender, SXALBetsUpdatedEventArgs e)
        {
            
        }

        private async void _checkTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //lock (_syncMailCheck)
            {
                try
                {
                    LOPConfigurationRW config = new LOPConfigurationRW();
                    if (config.StrategyActive == false)
                        return;

                    LOPTipp tipp = await _lopMails.getLOPTipp();

                    if (tipp != null)
                    {
                        if (tipp.NoSelectionDay)
                        {
                            if (NoSelectionDayEvent != null)
                                NoSelectionDayEvent(this, new LOPNoSelectionDayEventArgs());
                            return;
                        }

                        if (tipp.NoTippFound)
                        {
                            if (NoTippFoundEvent != null)
                                NoTippFoundEvent(this, new LOPNoTippFoundEventArgs());
                            return;
                        }


                        //Gibt es schon einen Trade zu dieser Kombination?
                        foreach (LOPTrade trade in _trades)
                        {
                            if ((trade.Race == tipp.Race || tipp.Race.Contains(trade.Race)) && trade.Horse == tipp.Horse && trade.EventDate == tipp.EventDate)
                                return;
                        }


                        // Ist der gelesene Tipp auch der letzter schon abgeschlossene Tipp?
                        if (_lastTipp != null && _lastTipp.EventDate == tipp.EventDate && _lastTipp.Race == tipp.Race && _lastTipp.Horse == tipp.Horse)
                        {
                            return;
                        }

                        //Startzeitpunkt des Tipps schon vorbei?
                        if (tipp.EventDate.Subtract(DateTime.Now).TotalMilliseconds < 0)
                        {
                            if (MessageEvent != null)
                            {
                                MessageEvent(this, new SXWMessageEventArgs(DateTime.Now, LayerOfProfit.strWatcher, String.Format(LayerOfProfit.strEventInPast, tipp.Race, tipp.Horse, tipp.EventDate)
                                    , LayerOfProfit.strName));
                            }
                            return;
                        }

                        //Markt suchen
                        SXALMarket m = getMartket(tipp.EventDate, tipp.Race);
                        if (m == null)
                        {
                            if (MessageEvent != null)
                            {
                                MessageEvent(this, new SXWMessageEventArgs(DateTime.Now, LayerOfProfit.strWatcher, String.Format(LayerOfProfit.strNoMarketFound, tipp.Race, tipp.Horse, tipp.EventDate),
                                    LayerOfProfit.strName));
                            }
                            return;
                        }

                        //SelectionId für Pferd suchen
                        long selectionId = getHorse(m, tipp.Horse);
                        if (selectionId == -1)
                        {
                            if (MessageEvent != null)
                            {
                                MessageEvent(this, new SXWMessageEventArgs(DateTime.Now, LayerOfProfit.strWatcher, String.Format(LayerOfProfit.strHorseNotFound, tipp.Horse, tipp.Race, m.Id, tipp.EventDate),
                                    LayerOfProfit.strName));
                            }
                            return;
                        }

                        //Erstelle Trade
                        LOPTrade lopTrade = LOPTrade.CreateTrade(m, selectionId, tipp.Horse);
                        if (lopTrade != null)
                        {
                            lopTrade.TradeSettledEvent += trade_TradeSettledEvent;
                            lopTrade.ExceptionMessageEvent += lopTrade_ExceptionMessageEvent;
                            lopTrade.MessageEvent += lopTrade_MessageEvent;
                            _trades.Add(lopTrade);

                            lopTrade.TradeSettledEvent += trade_TradeSettledEvent;
                            if (TippPlacedEvent != null)
                            {
                                TippPlacedEvent(this, new LOPTippAddedEventArgs(lopTrade.EventDate, lopTrade.Race, lopTrade.Horse, lopTrade.Currency, lopTrade.MatchedAmount,
                                    lopTrade.UnmatchedAmount, lopTrade.Risk, lopTrade.PotentialRisk, lopTrade.AvgPrice, lopTrade.TradeState));
                            }

                            if (MessageEvent != null)
                            {
                                MessageEvent(this, new SXWMessageEventArgs(DateTime.Now, LayerOfProfit.strWatcher, String.Format(LayerOfProfit.strTradeAdded, tipp.Race, tipp.Horse, tipp.EventDate),
                                    LayerOfProfit.strName));
                            }
                        }
                    }
                    else
                    {
                        if (NoTippFoundEvent != null)
                            NoTippFoundEvent(this, new LOPNoTippFoundEventArgs());
                        return;
                    }

                }
                catch (Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private void lopTrade_MessageEvent(object sender, SXWMessageEventArgs e)
        {
            if (MessageEvent != null)
            {
                MessageEvent(this, e);
            }
                        
        }

        private void lopTrade_ExceptionMessageEvent(object sender, SXExceptionMessageEventArgs e)
        {
            if (ExceptionMessageEvent != null)
            {
                ExceptionMessageEvent(this, e);
            }
        }

        private SXALMarket getMartket(DateTime startDts, String race)
        {
            //Erster Schritt: genauer Übereinstimmung
            SXALMarket m = getMarketExecution(startDts, race);

            if (m == null)
            {
                //Zweiter Schritt: " ' " entfernen
                String tmpRace = race.Replace("\'", "");
                m = getMarketExecution(startDts, tmpRace);
            }

            if (m == null)
            {
                //Dritte Schritt: " ' " mit " " ersetzen
                String tmpRace = race.Replace("'", " ");
                m = getMarketExecution(startDts, tmpRace);
            }

            if (m == null)
            {
                // Vierter Schritt: Name des Rennens aufspliten und mit der ersten Komponente suchen (z.B. Hamilton Park => Hamiltion)
                String[] raceComponents = race.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (raceComponents.Length > 0)
                {
                    m = getMarketExecution(startDts, raceComponents[0]);
                }
            }

            return m;
        }

        private SXALMarket getMarketExecution(DateTime startDts, String race)
        {
            foreach (SXALMarket market in SXALHorseMarketManager.Instance.AllWinMarkets.Values)
            {
                if (market.Match.Equals(race, StringComparison.InvariantCultureIgnoreCase) && market.StartDTS == startDts)
                    return market;
            }
            return null;
        }

        private long getHorse(SXALMarket m, String horse)
        {
            long selectionId = -1;
            //Erster Schritt: Finde über direkten Namen
            selectionId = getHorseExecution(m, horse);

            if (selectionId == -1)
            {
                //Zweiter Schritt: " ' " entfernen
                String tmp = horse.Replace("'", "");
                selectionId = getHorseExecution(m, tmp);
            }

            if (selectionId == -1)
            {
                //Dritte Schritt: " ' " mit " " ersetzen
                String tmp = horse.Replace("'", " ");
                selectionId = getHorseExecution(m, tmp);
            }

            if (selectionId == -1)
            {
                String[] splitNames = horse.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String s in splitNames)
                {
                    selectionId = getHorseExecution2(m, s);
                    if (selectionId > 0)
                        break;
                }
            }

            return selectionId;
        }


        /// <summary>
        /// Ungenaue Suche nach einen Teil des Pferdenamens.
        /// Nötig, wenn sonst kein Pferd gefunden werden konnte
        /// </summary>
        /// <param name="m"></param>
        /// <param name="horseNamePart"></param>
        /// <returns></returns>
        private long getHorseExecution2(SXALMarket m, String horseNamePart)
        {
            int iCounter = 0;
            long selectionId = -1;
            if (m.Selections == null)
                m.Selections = SXALKom.Instance.getSelections(m);

            foreach (SXALSelection s in m.Selections)
            {
                if (s.Name.Contains(horseNamePart))
                {
                    iCounter++;
                    selectionId = s.Id;
                }
            }

            // Nur, wenn wir genau einen treffer gefunden haben;
            if (iCounter == 1)
            {
                return selectionId;
            }
            return -1;
        }

        private long getHorseExecution(SXALMarket m, String horse)
        {
            if (m.Selections == null)
                m.Selections = SXALKom.Instance.getSelections(m);
            foreach (SXALSelection s in m.Selections)
            {
                if (s.Name.Equals(horse, StringComparison.InvariantCultureIgnoreCase))
                {
                    return s.Id;
                }
            }

            foreach (SXALSelection s in m.Selections)
            {
                //Zweiter Schritt: " ' " entfernen
                String tmp = s.Name.Replace("'", "");
                if (tmp.Equals(horse, StringComparison.InvariantCultureIgnoreCase))
                    return s.Id;
            }

            foreach (SXALSelection s in m.Selections)
            {
                //Dritte Schritt: " ' " mit " " ersetzen
                String tmp = s.Name.Replace("'", " ");
                if (tmp.Equals(horse, StringComparison.InvariantCultureIgnoreCase))
                    return s.Id;
            }

            return -1;
        }

        #region IBFTSCommon
        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;
        public event EventHandler<SXWMessageEventArgs> MessageEvent;
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DebugWriter.Instance.WriteMessage("LOPWatcher", "Disposing");
                if (disposing)
                {
                    if (_checkTimer != null)
                        _checkTimer.Dispose();

                    if (_lopMails != null)
                        _lopMails.Dispose();

                    SXALBetWatchdog.Instance.BetsUpdated -= Instance_BetsUpdated;
                }

                _disposed = true;
            }
        }
        #endregion

      
    }

    class LOPNoSelectionDayEventArgs : EventArgs { }

    class LOPNoTippFoundEventArgs : EventArgs { }

    class LOPSettledEventArgs : EventArgs 
    {
        private DateTime _eventDate;
        private String _race;
        private String _horse;
        private LOPTrade.TRADESTATE _state;

        public DateTime EventDate
        {
            get { return _eventDate; }
        }

        public String Race
        {
            get { return _race; }
        }

        public String Horse
        {
            get { return _horse; }
        }

        public LOPTrade.TRADESTATE TradeState
        {
            get { return _state; }
        }

        public LOPSettledEventArgs(DateTime eventDate, String race, String horse, LOPTrade.TRADESTATE state)
        {
            _eventDate = eventDate;
            _race = race;
            _horse = horse;           
            _state = state;
        }
    }

    class LOPTippAddedEventArgs : EventArgs
    {
        private DateTime _eventDate;
        private String _race;
        private String _horse;
        private String _currency;
        private double _placedBetAmount;
        private double _openBetAmount;
        private double _risk;
        private double _potentialRisk;
        private double _avgOdds;
        private LOPTrade.TRADESTATE _state;

        public DateTime EventDate
        {
            get { return _eventDate; }
        }

        public String Race
        {
            get { return _race; }
        }

        public String Horse
        {
            get { return _horse; }
        }

        public String Currency
        {
            get { return _currency; }
        }

        public double PlacedAmount
        {
            get { return _placedBetAmount; }
        }

        public double OpenAmount
        {
            get { return _openBetAmount; }
        }

        public double Risk
        {
            get { return _risk; }
        }

        public double PotentialRisk
        {
            get { return _potentialRisk; }
        }

        public double AvgOdds
        {
            get { return _avgOdds; }
        }

        public LOPTrade.TRADESTATE TradeState
        {
            get { return _state; }
        }

        public LOPTippAddedEventArgs(DateTime eventDate, String race, String horse, String currency, double matchedAmount, double openAmount, double risk,
             double potentialRisk, double avgOdds, LOPTrade.TRADESTATE state)
        {
            _eventDate = eventDate;
            _race = race;
            _horse = horse;
            _currency = currency;
            _placedBetAmount = matchedAmount;
            _openBetAmount = openAmount;
            _risk = risk;
            _potentialRisk = potentialRisk;
            _avgOdds = avgOdds;
            _state = state;
        }
    }

    class LOPTippUpdateEventArgs : EventArgs
    {
        private DateTime _eventDate;
        private String _race;
        private String _horse;
        private String _currency;
        private double _placedBetAmount;
        private double _openBetAmount;
        private double _risk;
        private double _potentialRisk;
        private double _avgOdds;
        private LOPTrade.TRADESTATE _state;

        public DateTime EventDate
        {
            get { return _eventDate; }
        }

        public String Race
        {
            get { return _race; }
        }

        public String Horse
        {
            get { return _horse; }
        }

        public String Currency
        {
            get { return _currency; }
        }

        public double PlacedAmount
        {
            get { return _placedBetAmount; }
        }

        public double OpenAmount
        {
            get { return _openBetAmount; }
        }

        public double Risk
        {
            get { return _risk; }
        }

        public double PotentialRisk
        {
            get { return _potentialRisk; }
        }

        public double AvgOdds
        {
            get { return _avgOdds; }
        }

        public LOPTrade.TRADESTATE TradeState
        {
            get { return _state; }
        }

        public LOPTippUpdateEventArgs(DateTime eventDate, String race, String horse, String currency, double matchedAmount, double openAmount, double risk,
             double potentialRisk, double avgOdds, LOPTrade.TRADESTATE state)
        {
            _eventDate = eventDate;
            _race = race;
            _horse = horse;
            _currency = currency;
            _placedBetAmount = matchedAmount;
            _openBetAmount = openAmount;
            _risk = risk;
            _potentialRisk = potentialRisk;
            _avgOdds = avgOdds;
            _state = state;
        }
    }
}
