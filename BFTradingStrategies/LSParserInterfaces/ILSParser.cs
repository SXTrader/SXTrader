using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.lsparserinterfaces
{
    public interface ILSParser
    {
        event EventHandler<LiveScoreStateEventArgs> LiveScoreStateChangedEvent;
        event EventHandler<LiveScoreCheckCountDownEventArgs> LiveScoreCheckCountDownEvent;
        event EventHandler<LiveScoreAddedEventArgs> LiveScoreAddedEvent;
        event EventHandler<LiveScoreRemovedEventArgs> LiveScoreRemovedEvent;

        IScore linkSportExchange(String teamAName, String teamBName);
    }

    public class LiveScoreRemovedEventArgs : EventArgs
    {
        private IScore _liveticker;
        public IScore Liveticker { get { return _liveticker; } }
        public LiveScoreRemovedEventArgs(IScore liveticker)
        {
            _liveticker = liveticker;
        }
    }

    public class LiveScoreAddedEventArgs : EventArgs
    {
        private IScore _liveticker;
        public IScore Liveticker { get { return _liveticker; } }
        public LiveScoreAddedEventArgs(IScore liveticker)
        {
            _liveticker = liveticker;
        }
    }

    public class LiveScoreCheckCountDownEventArgs : EventArgs
    {
        String _livescore;
        int _checkCountDown;

        public String Livescore
        {
            get
            {
                return _livescore;
            }
        }

        public int Countdown
        {
            get
            {
                return _checkCountDown;
            }
        }

        public LiveScoreCheckCountDownEventArgs(String livescore, int countdown)
        {
            _livescore = livescore;
            _checkCountDown = countdown;
        }
    }

    public class LiveScoreStateEventArgs : EventArgs
    {
        String _livescore;
        Boolean _state;

        public String Livescore
        {
            get
            {
                return _livescore;
            }
        }

        public Boolean State
        {
            get
            {
                return _state;
            }
        }

        public LiveScoreStateEventArgs(String livescore, Boolean state)
        {
            _livescore = livescore;
            _state = state;
        }
    }
}
