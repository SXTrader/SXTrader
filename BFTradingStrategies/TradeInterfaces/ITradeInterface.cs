using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.eventargs;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.tradeinterfaces
{
    public enum TRADETYPE
    {
        SCORELINE00, OVER05, OVER15, OVER25, OVER35, OVER45, OVER55, OVER65, OVER75, OVER85,
        SCORELINE01BACK, SCORELINE01LAY, SCORELINE02BACK,
        SCORELINE02LAY, SCORELINE03BACK, SCORELINE03LAY, SCORELINE10BACK, SCORELINE10LAY, SCORELINE11BACK, SCORELINE11LAY,
        SCORELINE12BACK, SCORELINE12LAY, SCORELINE13BACK, SCORELINE13LAY, SCORELINE20BACK, SCORELINE20LAY, SCORELINE21BACK,
        SCORELINE21LAY, SCORELINE22BACK, SCORELINE22LAY, SCORELINE23BACK, SCORELINE23LAY, SCORELINE30BACK, SCORELINE30LAY,
        SCORELINE31BACK, SCORELINE31LAY, SCORELINE32BACK, SCORELINE32LAY, SCORELINE33BACK, SCORELINE33LAY, SCORELINEOTHERBACK,
        SCORELINEOTHERLAY, UNASSIGNED
    };

    public enum TRADEMODE
    {
        BACK, LAY, UNASSIGNED
    }

    public interface ITrade : IDisposable
    {
        event EventHandler<SetTimerEventArgs> SetTimer;
        event EventHandler<StopTimerEventArgs> StopTimer;
        event EventHandler<StateChangedEventArgs> RunningStateChanged;
        event EventHandler<StateChangedEventArgs> TradeStateChanged;
        event EventHandler<SXWMessageEventArgs> MessageEvent;
        event EventHandler<GoalScoredEventArgs> GoalScoredEvent;
        event EventHandler<PlaytimeEventArgs> PlaytimeEvent;
        event EventHandler<GameEndedEventArgs> GameEndedEvent;
        event EventHandler<BetsChangedEventArgs> BetsChangedEvent;
        //public event EventHandler<SXMessageEventArgs> MessageEvent;

        IScore Score { get; set; }
        String Match { get;}
        String TeamA { get;}
        String TeamB { get;}
        String TradeTypeName { get; }
        long MarketId { get; }
        IConfiguration Config { get; set; }
        TradeRunningState RunningState { get; set; }
        TradeMoneyState TradeState { get; set; }
        uint TradeId { get; }
        SXALBetCollection Back { get; }
        SXALBetCollection Lay { get; }
        TRADETYPE TradeType
        {
            get;
        }

        TRADEMODE TradeMode { get; }

        void addBet(SXALBet bet, bool withTradeCheck);
        double getInitialStake();
        double getWinnings();
        double getPLSnapshot();
        bool isSettled();
        void start();
        bool hasBet(long betId);
    }
}
