using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Collections;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.ttr.TradeStarter;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls;
using System.Windows.Forms;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.Scoreline00.Controls;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder.Controls;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore.Controls;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.enums;
using net.sxtrader.muk.interfaces;
using net.sxtrader.bftradingstrategies.SXALInterfaces;
using System.Threading;
using System.Collections.Concurrent;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.muk;

namespace net.sxtrader.bftradingstrategies.ttr
{
    public class TradeCollection : SortedList<TRADETYPE, ITrade>
    {
    }

    public class TTRWatcher : SortedList<String, TradeCollection>, IBFTSCommon, IWatcher
    {
        private LiveScoreParser _LSparser;
        private LiveScore2Parser _LS2parser;
        //private bool _buildList = false;
        private object _lockBetsUpdate = "lockBetsUpdate";

        private object _lockExceptionMessage = "lockExceptionMessage";

        private System.Timers.Timer _queueTimer;
        private ConcurrentQueue<InsertQueueItem> _insertQueue;

        public TTRWatcher(LiveScoreParser parser, LiveScore2Parser parser2)
        {
            _LSparser = parser;
            _LS2parser = parser2;

            _insertQueue = new ConcurrentQueue<InsertQueueItem>();
            _queueTimer = new System.Timers.Timer(10000);
            _queueTimer.AutoReset = false;
            _queueTimer.Elapsed += _queueTimer_Elapsed;
            _queueTimer.Start();

            SXALBetWatchdog.Instance.BetsUpdated += new EventHandler<SXALBetsUpdatedEventArgs>(Instance_BetsUpdated);
            SXALBetWatchdog.Instance.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(ExceptionMessageEventHandler);
            SXALKom.Instance.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(ExceptionMessageEventHandler);
            parser.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(ExceptionMessageEventHandler);
            parser2.ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(ExceptionMessageEventHandler);

            SX5MinutePulse.Instance.Pulse += Instance_Pulse;
        }

        void Instance_Pulse(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            for(int i = 0; i < this.Count; i++)
            {
                TradeCollection coll = this.Values[i];

                for (int j = 0; j < coll.Count; j++)
                {
                    ITrade trade = coll.Values[j];
                    if (DateTime.Now.Subtract(trade.Score.StartDTS).TotalHours > 4)
                    {
                        EventHandler<net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs> handler = GamenEndedEvent;
                        if (handler != null)
                            handler(this, new net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs(trade.Match, trade.Score.StartDTS, 0.0/*e.WinLoose*/));
                        coll.RemoveAt(j--);
                        trade.Dispose();
                    }
                }

                if(coll.Count == 0)
                    this.RemoveAt(i--);
            }
        }

        private void _queueTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                processInsertBetQueue();
            }
            catch (Exception exc)
            {
                ExceptionWriter.Instance.WriteException(exc);
            }
            finally
            {
                //timer erneut Starten
                _queueTimer.Start();
            }
        }

        private void processInsertBetQueue()
        {
            //Nach Match / TRADETYPE sortierte Wetten, getrennt in Back und Lay
            SortedListQueueWorkItems theList = buildProcessList();
            processWorkItems(theList);
        }

        private void processWorkItems(SortedListQueueWorkItems theList)
        {
            DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Anzahl der zu verarbeiteten Worklistitems: {0}", theList.Count));
            foreach(String key in theList.Keys)
            {

                try
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Verarbeite Schlüssel {0}", key));
                    QueueWorkItem workItem = theList[key];

                    

                    //Falls noch kein Toplevel-Element vorhanden ist, dann erzeuge eines
                    if (!this.ContainsKey(workItem.Match.Trim()))
                    {
                        //Füge ein Toplevel-Element hinzu.
                        this.Add(workItem.Match.Trim(), new TradeCollection());
                    }
                   
                    //Trades: Update oder Insert
                    if (this[workItem.Match.Trim()].ContainsKey(workItem.TradeType))
                    {
                        //Update
                        updateTrade(workItem);
                    }
                    else
                    {
                        //TODO: Überprüfe, ob eine Wette eines Workitems nicht schon für einen anderen Trade beansprucht wird.
                        if(this.ContainsKey(workItem.Match.Trim()))
                        {
                            foreach(ITrade trade in this[workItem.Match.Trim()].Values)
                            {
                                for(int i = 0; i < workItem.Bets.Bets.Count; i++)
                                {
                                    //Wird diese Wette schon von einem anderen Trade beansprucht?
                                    if(trade.hasBet(workItem.Bets.Bets.Values[i].BetId))
                                    {
                                        //Ja, also steht diese Wette nicht für einen Neuen Trade zur Verfügung
                                        workItem.Bets.Bets.Remove(workItem.Bets.Bets.Values[i].BetId);
                                        i--;
                                    }
                                }
                            }
                        }
                        if (workItem.Bets.Bets.Count > 0)
                        {
                            //Insert
                            insertTrade(workItem);
                        }
                    }
                }
                catch(Exception exc)
                {
                    DebugWriter.Instance.WriteMessage("BFUEWatcher:processInsertBetQueue", String.Format("Ausnahmer bei der Verarbeitung des Schlüssels {0}", key));
                    ExceptionWriter.Instance.WriteException(exc);
                }
            }
        }

        private void updateTrade(QueueWorkItem item)
        {
            if(item.Configuration != null)
            {
                DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Aktualisiere Konfiguration für Trade {0} der Begegnung {1}", item.TradeType, item.Match));
                this[item.Match][item.TradeType].Config = item.Configuration;
            }

            if(item.Ticker != null)
            {
                DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Aktualisiere Ticker für Trade {0} der Begegnung {1}", item.TradeType, item.Match));
                this[item.Match][item.TradeType].Score = item.Ticker;
            }

            foreach(SXALBet bet in item.Bets.Bets.Values)
            {
                DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Aktualisieren/Hinzufügen von Wette {0} zu Trade {1} der Begegnung {2}", bet.BetId, item.TradeType, item.Match));
                this[item.Match][item.TradeType].addBet(bet, true);
            }
        }

        private void insertTrade(QueueWorkItem item)
        {
            DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Erzeuge Trade {0} der Begegnung {1}", item.TradeType, item.Match));
            LIVESCOREADDED withLivescore = LIVESCOREADDED.NONE;

            if (item.Configuration == null)
                item.Configuration = new TTRConfigurationRW();

            ITrade trade = constructTrade(item.TradeType, item.Match, item.Bets.Bets.Values[0], item.Configuration, item.Ticker, out withLivescore);

            if(trade == null)
            {
                DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Erzeugung Trade {0} der Begegnung {1} fehlgeschlagen", item.TradeType, item.Match));
                return;
            }

            trade.SetTimer += new EventHandler<SetTimerEventArgs>(trade_SetTimer);
            trade.StopTimer += new EventHandler<StopTimerEventArgs>(trade_StopTimer);
            trade.RunningStateChanged += new EventHandler<StateChangedEventArgs>(trade_RunningStateChanged);
            trade.TradeStateChanged += new EventHandler<StateChangedEventArgs>(trade_TradeStateChanged);
            trade.GameEndedEvent += new EventHandler<net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs>(trade_GameEndedEvent);
            trade.GoalScoredEvent += new EventHandler<GoalScoredEventArgs>(trade_GoalScoredEvent);
            trade.MessageEvent += new EventHandler<SXWMessageEventArgs>(trade_MessageEvent);
            trade.PlaytimeEvent += new EventHandler<PlaytimeEventArgs>(trade_PlaytimeEvent);
            trade.BetsChangedEvent += new EventHandler<BetsChangedEventArgs>(trade_BetsChangedEvent);


            this[item.Match].Add(item.TradeType, trade);
            
            if (trade != null)
            {
                raiseEventsNewTrade(trade, withLivescore);
            }

            trade.start();

        }

        private SortedListQueueWorkItems buildProcessList()
        {
            SortedListQueueWorkItems theList = new SortedListQueueWorkItems();
            InsertQueueItem queueItem = null;
            while (_insertQueue.TryDequeue(out queueItem))
            {
                String match = SXALSoccerMarketManager.Instance.getMatchById(queueItem.Bet.MarketId);

                //Überprüfe, ob TradeType gesetzt wurde, falls nicht versuche ihn zu bestimmen.
                TRADETYPE tradeType = queueItem.TradeType;
                if(tradeType == TRADETYPE.UNASSIGNED)
                {
                    DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1}: Tradetype wurde nicht vergegeben. Versuche Tradetype zu ermitteln.", queueItem.Bet.BetId, match));
                    tradeType = TTRHelper.GetTradeTypeByBetAndSelection(queueItem.Bet);
                }

                // Falls immer noch kein TradeType bestimmten werden konnte => Überspringe diese Wette
                if(tradeType == TRADETYPE.UNASSIGNED)
                {
                    DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1}: Tradetype konnte nicht ermittelt werden. Überspringe Wette", queueItem.Bet.BetId, match));
                    continue;
                }

                DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Wette {0} zu {1}: Tradetype its {2}", queueItem.Bet.BetId, match, tradeType));

                //Existiert bereits ein Eintrag für die aktuelle Wette?
                string key = String.Format("{0}/{1}", match, tradeType);
                QueueWorkItem workItem = null;
                if(theList.ContainsKey(key))
                {
                    DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Zu Schlüssel {0} existiert bereits ein Eintrag => Aktualisierung des Eintrages", key));
                    //Markt/Wette ist vorhanden
                    workItem = theList[key];

                    //Überprüfe, ob Wetter bereits vorhanden
                    if(workItem.Bets.Bets.ContainsKey(queueItem.Bet.BetId))
                    {
                        DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Wette {0} bereits in Schlüssel {1} enthalten => Aktualisierung des Eintrages", queueItem.Bet.BetId, key));
                        workItem.Bets.Bets[queueItem.Bet.BetId] = queueItem.Bet;
                    }
                    else
                    {
                        DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Wette {0} noch nicht in Schlüssel {1} enthalten => Einfüge", queueItem.Bet.BetId, key));
                        workItem.Bets.Bets.Add(queueItem.Bet.BetId, queueItem.Bet);
                    }

                    //Konfiguration und Ticker
                    if (queueItem.Configuration != null)
                    {
                        DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Schlüssel {0}. Aktualisieren Konfiguration", key));
                        workItem.Configuration = queueItem.Configuration;
                    }

                    if (queueItem.Ticker != null)
                    {
                        DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Schlüssel {0}. Aktualisieren Ticker",key));
                        workItem.Ticker = queueItem.Ticker;
                    }

                    if (tradeType != TRADETYPE.UNASSIGNED)
                    {
                        workItem.TradeType = tradeType;
                    }
                }
                else
                {
                    DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Zu Schlüssel {0} existiert noch kein Eintrag => Erstellung des Eintrages", key));
                    workItem = new QueueWorkItem();

                    //Wette Hinzufügen.
                    DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Füge Wette {0} zu Schlüssel {1} hinzu ",queueItem.Bet.BetId, key));
                    workItem.Bets.Bets.Add(queueItem.Bet.BetId, queueItem.Bet);

                    //Tradetyp Hinzufügen.
                    DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Tradetyp zu Schlüssel {0} ist {1}",key, tradeType));
                    workItem.TradeType = tradeType;

                    //Begegnung Hinzufügen
                    DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Begegnung zu Schlüssel {0} ist {1}", key, match));
                    workItem.Match = match;

                    if (queueItem.Configuration != null)
                    {
                        DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Schlüssel {0}. Aktualisieren Konfiguration", key));
                        workItem.Configuration = queueItem.Configuration;
                    }

                    if (queueItem.Ticker != null)
                    {
                        DebugWriter.Instance.WriteMessage("TTRWatcher:processInsertBetQueue", String.Format("Schlüssel {0}. Aktualisieren Ticker", key));
                        workItem.Ticker = queueItem.Ticker;
                    }

                    theList.Add(key, workItem);
                }
            }

            return theList;
        }


        public void setNewBet(TRADETYPE tradeType, SXALBet betDetail, TTRConfigurationRW config, IScore scoreToConnect)
        {
            String match = SXALSoccerMarketManager.Instance.getMatchById(betDetail.MarketId);

            InsertQueueItem item = new InsertQueueItem(tradeType, betDetail, config, scoreToConnect);
            _insertQueue.Enqueue(item);          
        }

        private void trade_PlaytimeEvent(object sender, PlaytimeEventArgs e)
        {
            // internes Event nach aussen veröffentlichen
            EventHandler<PlaytimeEventArgs> handler = PlaytimeEvent;
            if (handler != null)
                handler(this, new PlaytimeEventArgs(e.Match, e.Playtime));
        }

        private void trade_MessageEvent(object sender, SXWMessageEventArgs e)
        {
            EventHandler<SXWMessageEventArgs> handler = MessageEvent;
            if (handler != null)
                handler(this, new SXWMessageEventArgs(e.MessageDTS, e.Match, e.Message, e.Module));
        }

        private void trade_GoalScoredEvent(object sender, GoalScoredEventArgs e)
        {
            //TODO: sender soll als Argument in den Wrapper
            EventHandler<GoalScoredEventArgs> handler = GoalScoredEvent;
            if (handler != null)
                handler(this, new GoalScoredEventArgs(e.Team, e.ScoreA, e.ScoreB, e.Match));
        }

        private void trade_GameEndedEvent(object sender, net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs e)
        {
            EventHandler<net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs> handler = GamenEndedEvent;
            if (handler != null)
                handler(this, new net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs(e.Match, e.Dts, e.WinLoose));
        }

        private void trade_TradeStateChanged(object sender, StateChangedEventArgs e)
        {
            EventHandler<StateChangedEventArgs> handler = TradeStateChanged;
            if (handler != null)
                handler(this, new StateChangedEventArgs(e.NewState, e.OldState));
        }

        private void trade_RunningStateChanged(object sender, StateChangedEventArgs e)
        {
            EventHandler<StateChangedEventArgs> handler = RunningStateChanged;
            if (handler != null)
                handler(this, new StateChangedEventArgs(e.NewState, e.OldState));
        }

        private void trade_StopTimer(object sender, StopTimerEventArgs e)
        {
            EventHandler<StopTimerEventArgs> handler = StopTimer;
            if (handler != null)
                handler(this, new StopTimerEventArgs(e.Match, e.Trade));
        }

        private void trade_SetTimer(object sender, SetTimerEventArgs e)
        {
            EventHandler<SetTimerEventArgs> handler = SetTimer;
            if (handler != null)
            {
                if (e.UsePlaytime)
                {
                    handler(this, new SetTimerEventArgs(e.Match, e.Trade, e.Timer));
                }
                else
                {
                    TimeSpan span = e.Time;
                    handler(this, new SetTimerEventArgs(e.Match, e.Trade, span));
                }
            }
        }

        private void raiseEventsNewTrade(ITrade trade, LIVESCOREADDED withLivescore)
        {
            // Nachricht verbreiten
            EventHandler<SXWMessageEventArgs> message = MessageEvent;
            if (message != null)
                message(this, new SXWMessageEventArgs(DateTime.Now, trade.Match, TradeTheReaction.strMatchAdded, String.Format("{0}:{1}",TradeTheReaction.strName, trade.TradeTypeName)));
            
            if (withLivescore != LIVESCOREADDED.NONE)
            {
                //IScore tmpScore = score != null ? score : score2;
                EventHandler<TradeAddedEventArgs> handler = TradeAddedEvent;
                if (handler != null)
                    handler(this, new TradeAddedEventArgs(trade, withLivescore));
                    //handler(this, new TradeAddedEventArgs(trade.TeamA, trade.TeamB, trade.Score.getScore(), 0.0, 0.0, withLivescore, trade.MarketId));

                EventHandler<IScoreAddedEventArgs> addedHandler = IScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new IScoreAddedEventArgs(trade.Match, withLivescore, trade.MarketId));
                //addedHandler(this, new BFWIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB), withLivescore, betSet.Lay.MarketId));
            }
            else
            {
                EventHandler<TradeAddedEventArgs> handler = TradeAddedEvent;
                if (handler != null)
                    handler(this, new TradeAddedEventArgs(trade, withLivescore));
                    //handler(this, new TradeAddedEventArgs(trade.TeamA, trade.TeamB, "0 - 0", 0.0, 0.0, withLivescore, trade.MarketId));

                EventHandler<NoIScoreAddedEventArgs> addedHandler = NoIScoreAdded;
                if (addedHandler != null)
                    addedHandler(this, new NoIScoreAddedEventArgs(trade.Match));
                //                        addedHandler(this, new NoIScoreAddedEventArgs(String.Format("{0} - {1}", betSet.TeamA, betSet.TeamB)));
            }           
        }

        internal ITrade constructTrade(TRADETYPE tradeType, String match, SXALBet betDetail, TTRConfigurationRW config, IScore scoreToConnect, out LIVESCOREADDED withLivescore)
        {
            DebugWriter.Instance.WriteMessage("TTRWatcher:constructTrade", String.Format("Erzeuge einen Trade Typ {0} zu Begegnung {1}",tradeType, match));
            String teamA = null;
            String teamB = null;

            getTeamNames(match, out teamA, out teamB);
            
            IScore score = null;
            if (scoreToConnect != null)
            {
                DebugWriter.Instance.WriteMessage("TTRWatcher:constructTrade", String.Format("Trade Typ {0} zu Begegnung {1}: Liveticker vorgegeben => direkte Zuweisung", tradeType, match));
                score = scoreToConnect;
            }
            else
            {
                DebugWriter.Instance.WriteMessage("TTRWatcher:constructTrade", String.Format("Trade Typ {0} zu Begegnung {1}:  Liveticker vorgegeben nicht vorgegeben. Versuche automatisches Verbinden", tradeType, match));
                score = linkLiveScore(teamA, teamB);
            }
            withLivescore = LIVESCOREADDED.NONE;

            if (score != null)
            {
                if (((HLLiveScore)score).IsScore1Connected() && ((HLLiveScore)score).IsScore2Connected())
                {
                    DebugWriter.Instance.WriteMessage("TTRWatcher:constructTrade", String.Format("Trade Typ {0} zu Begegnung {1}:  Alle Liveticker verbunden", tradeType, match));
                    withLivescore = LIVESCOREADDED.ALL;
                }
                else if (((HLLiveScore)score).IsScore1Connected() || ((HLLiveScore)score).IsScore2Connected())
                {
                    DebugWriter.Instance.WriteMessage("TTRWatcher:constructTrade", String.Format("Trade Typ {0} zu Begegnung {1}:  Liveticker teilweise verbunden", tradeType, match));
                    withLivescore = LIVESCOREADDED.PARTLY;
                }
            }


            DebugWriter.Instance.WriteMessage("TTRWatcher:constructTrade", String.Format("Trade Typ {0} zu Begegnung {1}:  Beginne tatsächliche Erzeugung des Tradeobjekts", tradeType, match));
            ITrade trade = null;
            switch (tradeType)
            {
                case TRADETYPE.SCORELINE00:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new ScorelineTrade00(coll, score, match, config, tradeType);
                        break;
                    }
                    /*
                case TRADETYPE.OVER05:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.ZEROFIVE, tradeType);
                        break;
                    }
                case TRADETYPE.OVER15:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.ONEFIVE, tradeType);
                        break;
                    }
                case TRADETYPE.OVER25:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.TWOFIVE, tradeType);
                        break;
                    }
                case TRADETYPE.OVER35:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.THREEFIVE, tradeType);
                        break;
                    }
                case TRADETYPE.OVER45:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.FOURFIVE, tradeType);
                        break;
                    }
                case TRADETYPE.OVER55:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.FIVEFIVE, tradeType);
                        break;
                    }
                case TRADETYPE.OVER65:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.SIXFIVE, tradeType);
                        break;
                    }
                case TRADETYPE.OVER75:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.SEVENFIVE, tradeType);
                        break;
                    }
                case TRADETYPE.OVER85:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new OverUnderTrade(coll, score, match, config, OUTYPE.OVER, OUVALUE.EIGHTFIVE, tradeType);
                        break;
                    }
                     * 
                     */
                //TODO: OVER/UNDER!
                case TRADETYPE.SCORELINE01BACK:
                case TRADETYPE.SCORELINE01LAY:
                case TRADETYPE.SCORELINE02BACK:
                case TRADETYPE.SCORELINE02LAY:
                case TRADETYPE.SCORELINE03BACK:
                case TRADETYPE.SCORELINE03LAY:
                case TRADETYPE.SCORELINE10BACK:
                case TRADETYPE.SCORELINE10LAY:
                case TRADETYPE.SCORELINE11BACK:
                case TRADETYPE.SCORELINE11LAY:
                case TRADETYPE.SCORELINE12BACK:
                case TRADETYPE.SCORELINE12LAY:
                case TRADETYPE.SCORELINE13BACK:
                case TRADETYPE.SCORELINE13LAY:
                case TRADETYPE.SCORELINE20BACK:
                case TRADETYPE.SCORELINE20LAY:
                case TRADETYPE.SCORELINE21BACK:
                case TRADETYPE.SCORELINE21LAY:
                case TRADETYPE.SCORELINE22BACK:
                case TRADETYPE.SCORELINE22LAY:
                case TRADETYPE.SCORELINE23BACK:
                case TRADETYPE.SCORELINE23LAY:
                case TRADETYPE.SCORELINE30BACK:
                case TRADETYPE.SCORELINE30LAY:
                case TRADETYPE.SCORELINE31BACK:
                case TRADETYPE.SCORELINE31LAY:
                case TRADETYPE.SCORELINE32BACK:
                case TRADETYPE.SCORELINE32LAY:
                case TRADETYPE.SCORELINE33BACK:
                case TRADETYPE.SCORELINE33LAY:
                case TRADETYPE.SCORELINEOTHERBACK:
                case TRADETYPE.SCORELINEOTHERLAY:
                    {
                        SXALBetCollection coll = new SXALBetCollection();
                        coll.Bets.Add(betDetail.BetId, betDetail);
                        trade = new CorrectScoreTrade(coll, score, match, config, tradeType);
                        break;
                    }
            }
           
            return trade;
        }

        private IScore linkLiveScore(String teamA, String teamB)
        {
            LiveScoreParser parser = LiveScoreParser.Instance;
            IScore score = parser.linkSportExchange(teamA, teamB);

            LiveScore2Parser parser2 = LiveScore2Parser.Instance;
            IScore score2 = parser2.linkSportExchange(teamA, teamB);

            HLLiveScore hlScore = new HLLiveScore(score, score2);
            return hlScore;
        }

        private void getTeamNames(String match, out String teamA, out String teamB)
        {
            String[] sSeps = { " - ", " v " };

            String[] teams = match.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);
            /*if (teams.Length == 1)
                teams = market.Split(sSeps, StringSplitOptions.RemoveEmptyEntries);
             */
            teamA = teams[0].Trim();
            teamB = teams[teams.GetLength(0) - 1].Trim();
        }

        private void ExceptionMessageEventHandler(object sender, SXExceptionMessageEventArgs e)
        {
            lock (_lockExceptionMessage)
            {
                EventHandler<SXExceptionMessageEventArgs> handler = ExceptionMessageEvent;
                if (handler != null)
                {
                    handler(this, new SXExceptionMessageEventArgs(e));
                }
            }
        }

        private void Instance_BetsUpdated(object sender, SXALBetsUpdatedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.buildTradeListInternal));
            //buildTradeListInternal();
        }

        private ArrayList buildOverviewFragments(TradeCollection collTrades)
        {
            ArrayList arrList = new ArrayList();
            foreach (ITrade trade in collTrades.Values)
            {
                if (trade.GetType() == typeof(ScorelineTrade00))
                {
                    ctlSL00Overview ctlSL00 = new ctlSL00Overview();
                    ctlSL00.Trade = trade;
                    arrList.Add(ctlSL00);
                }
                else if (trade.GetType() == typeof(OverUnderTrade))
                {
                    ctlOUOverview ctlOU = new ctlOUOverview();
                    ctlOU.Trade = trade;
                    arrList.Add(ctlOU);
                }
                else if (trade.GetType() == typeof(CorrectScoreTrade))
                {
                    ctlCSOverview ctlCS = new ctlCSOverview();
                    ctlCS.Trade = trade;
                    arrList.Add(ctlCS);
                }
            }

            return arrList;
        }

        private void buildTradeListInternal(Object stateInfo)
        {
            lock (_lockBetsUpdate)
            {
                //_buildList = true;
                SXALKom betfairKom = SXALKom.Instance;

                //neuer Methode des Erkennens neuer Trades bzw. des Updates bestehender Trades mittels einer Parallelläufigen Queue
                SXALMUBet[] bets = SXALBetWatchdog.Instance.Bets;

                if (bets == null)
                    return;

                foreach (SXALMUBet bet in bets)
                {
                    try
                    {
                        SXALBet betDetail = betfairKom.getBetDetail(bet.BetId);
                        if (betDetail == null)
                            continue;

                        InsertQueueItem item = new InsertQueueItem(TRADETYPE.UNASSIGNED, betDetail, null, null);
                        _insertQueue.Enqueue(item);
                    }
                    catch(Exception e)
                    {
                        ExceptionWriter.Instance.WriteException(e);
                    }
                }


                /*
                List<ITrade> createdTrades = new List<ITrade>();
                List<ITrade> appendedTrades = new List<ITrade>();
                SortedList<int, LIVESCOREADDED> createLSState = new SortedList<int, LIVESCOREADDED>();
                // Initial alles lesen
                SXALMUBet[] bets = SXALBetWatchdog.Instance.Bets;

                if (bets == null)
                    return;

                foreach (SXALMUBet bet in bets)
                {
                    try
                    {
                        //Anhand des Wettmarktes unterscheiden, welche Art von Trade es ist.               
                        SXALBet betDetail = betfairKom.getBetDetail(bet.BetId);
                        if (betDetail == null)
                            continue;
                        String match = SXALSoccerMarketManager.Instance.getMatchById(bet.MarketId);
                        TradeLoader tradeLoader = TradeLoader.getInstance(betDetail);
                        if (tradeLoader == null)
                            continue;
                        LOADTRADEMODE loadMode = LOADTRADEMODE.UNASSIGNED;
                        LIVESCOREADDED livescoreMode = LIVESCOREADDED.NONE;
                        ITrade trade = tradeLoader.loadTrade(betDetail, this, out loadMode, out livescoreMode);
                        if (trade == null)
                            continue;
                        switch (loadMode)
                        {
                            case LOADTRADEMODE.APPEND:

                                this[trade.Match][trade.TradeType] = trade;
                                trade.start();
                                break;
                            case LOADTRADEMODE.CREATE:
                                this[trade.Match].Add(trade.TradeType, trade);
                                trade.SetTimer += new EventHandler<SetTimerEventArgs>(trade_SetTimer);
                                trade.StopTimer += new EventHandler<StopTimerEventArgs>(trade_StopTimer);
                                trade.RunningStateChanged += new EventHandler<StateChangedEventArgs>(trade_RunningStateChanged);
                                trade.TradeStateChanged += new EventHandler<StateChangedEventArgs>(trade_TradeStateChanged);
                                trade.GameEndedEvent += new EventHandler<net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs>(trade_GameEndedEvent);
                                trade.GoalScoredEvent += new EventHandler<GoalScoredEventArgs>(trade_GoalScoredEvent);
                                trade.MessageEvent += new EventHandler<SXWMessageEventArgs>(trade_MessageEvent);
                                trade.PlaytimeEvent += new EventHandler<PlaytimeEventArgs>(trade_PlaytimeEvent);
                                trade.BetsChangedEvent += new EventHandler<BetsChangedEventArgs>(trade_BetsChangedEvent);


                                raiseEventsNewTrade(trade, livescoreMode);

                                trade.start();
                                break;
                            default:
                                continue;
                        }
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                    }
                }
              */   
            } 
        }

        void trade_BetsChangedEvent(object sender, BetsChangedEventArgs e)
        {
            EventHandler<BetsChangedEventArgs> betChangedEvent = BetsChangedEvent;
            if (betChangedEvent != null)
                betChangedEvent(this, e);
        }
        #region IBFTSCommon Member

        public event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;

        #endregion

        #region IWatcher Member

        public event EventHandler<GoalScoredEventArgs> GoalScoredEvent;

        public event EventHandler<PlaytimeEventArgs> PlaytimeEvent;

        public event EventHandler<SXWMessageEventArgs> MessageEvent;

        public event EventHandler<net.sxtrader.bftradingstrategies.tradeinterfaces.GameEndedEventArgs> GamenEndedEvent;

        public event EventHandler<NoIScoreAddedEventArgs> NoIScoreAdded;

        public event EventHandler<IScoreAddedEventArgs> IScoreAdded;

        public event EventHandler<SetTimerEventArgs> SetTimer;

        public event EventHandler<StopTimerEventArgs> StopTimer;

        public event EventHandler<StateChangedEventArgs> RunningStateChanged;

        public event EventHandler<StateChangedEventArgs> TradeStateChanged;

        public event EventHandler<TradeAddedEventArgs> TradeAddedEvent;
 
        public event EventHandler<BetsChangedEventArgs> BetsChangedEvent;

        public void initializeTradeList()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.buildTradeListInternal));
            //buildTradeListInternal();
        }

        public Control[] getOverviewFragments(string match)
        {
            ArrayList arrList = new ArrayList();
            
            if (!this.ContainsKey(match))
                return (Control[])arrList.ToArray(typeof(Control));

            arrList = buildOverviewFragments(this[match]);

            return (Control[])arrList.ToArray(typeof(Control));
        }

        #endregion

        private class InsertQueueItem
        {
            private SXALBet _bet;
            private TTRConfigurationRW _configuration;
            private IScore _ticker;
            private TRADETYPE _tradeType;

            public SXALBet Bet { get { return _bet; } }
            public TTRConfigurationRW Configuration { get { return _configuration; } }
            public IScore Ticker { get { return _ticker; } }
            public TRADETYPE TradeType {get{return _tradeType;}}


            public InsertQueueItem(TRADETYPE tradeType, SXALBet bet, TTRConfigurationRW configuration, IScore ticker)
            {
                _bet = bet;
                _configuration = configuration;
                _ticker = ticker;
                _tradeType = tradeType;
            }
        }

        private class QueueWorkItem
        {
            private SXALBetCollection _bets = new SXALBetCollection();
            

            public SXALBetCollection Bets { get { return _bets; } }

            public TRADETYPE TradeType
            {
                get;
                set;
            }

            public TTRConfigurationRW Configuration
            {
                get;
                set;
            }

            public IScore Ticker
            {
                get;
                set;
            }

            public String Match
            {
                get;
                set;
            }
        }

        private class SortedListQueueWorkItems : SortedList<String, QueueWorkItem> { }
    }
}
