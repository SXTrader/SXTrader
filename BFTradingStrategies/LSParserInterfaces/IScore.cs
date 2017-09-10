using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.lsparserinterfaces
{
    public enum SCORESTATE { initdraw = 0, draw = 1, undraw = 2 };

    public enum LIVESTATES
    {
        First = 1, HT = 2, Second = 3, End = 4, Fifth,
        Sixth, Seventh, Eighth, Ninth, Tenth, Eleventh, Twelves, Thirdteenth,
        Fourteenth, Fiveteenth, Sixteenth, Seventeenth, Eigthteenth
    };

    public interface IScore : IDisposable
    {
        event EventHandler<GoalEventArgs> RaiseGoalEvent;
        event EventHandler<GoalBackEventArgs> BackGoalEvent;
        event EventHandler<PlaytimeTickEventArgs> PlaytimeTickEvent;
        event EventHandler<GameEndedEventArgs> GameEndedEvent;
        event EventHandler<RedCardEventArgs> RedCardEvent;

        bool Ended {get;}
        LIVESTATES MatchState { get; }
        String TeamA { get; }
        String TeamB { get; }
        ulong TeamAId { get; }
        ulong TeamBId { get; }
        String League { get; }
        int Playtime { get; }
        String BetfairMatch { get; set; }

        int ScoreA { get; }
        int ScoreB { get; }
        int RedA { get; }
        int RedB { get; }
        DateTime StartDTS { get; }
        
        Boolean isRunning();
        String getScore();
        String getLiveMatch();
        IScore IncreaseRef();
        void DecreaseRef();
        SCORESTATE getScoreState();
        
    }
}
