using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using net.sxtrader.muk.eventargs;


namespace net.sxtrader.bftradingstrategies.tradeinterfaces
{
    public interface IWatcher
    {
        event EventHandler<GoalScoredEventArgs> GoalScoredEvent;
        event EventHandler<PlaytimeEventArgs> PlaytimeEvent;
        //public event EventHandler<BFWRiskWinChangedEventArgs> RiskWinChangedEvent;
        event EventHandler<SXWMessageEventArgs> MessageEvent;
        event EventHandler<GameEndedEventArgs> GamenEndedEvent;
        event EventHandler<NoIScoreAddedEventArgs> NoIScoreAdded;
        event EventHandler<IScoreAddedEventArgs> IScoreAdded;
        event EventHandler<SetTimerEventArgs> SetTimer;
        event EventHandler<StopTimerEventArgs> StopTimer;
        event EventHandler<StateChangedEventArgs> RunningStateChanged;
        event EventHandler<StateChangedEventArgs> TradeStateChanged;
        //        public static event EventHandler<BFWMatchNotFoundEventArgs> MatchNotFoundEvent;
        event EventHandler<TradeAddedEventArgs> TradeAddedEvent;
        event EventHandler<BetsChangedEventArgs> BetsChangedEvent;

        void initializeTradeList();

        Control[] getOverviewFragments(String match);
        
    }
}
