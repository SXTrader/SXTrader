using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.tradeinterfaces
{
    public abstract class TradeStateAbstract
    {
        protected ITrade _trade;
        private static bool bBeta = true;

        public event EventHandler<StateChangedEventArgs> StateChanged;

        public ITrade Trade
        {
            get { return _trade; }
        }

        public abstract void checkState();
     
        protected virtual void OnStateChanged(StateChangedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<StateChangedEventArgs> handler = StateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        protected void log(string message)
        {
            if (!bBeta)
                return;
            try
            {
                TradeLog.Instance.writeLog(this.Trade.Match, "TradeState", "StateWatcher", String.Format("ID {0}: {1}", this.Trade.TradeId, message));
            }
            catch { }
        }
    }


    #region Status für den Laufzustand
    public abstract class TradeRunningState : TradeStateAbstract { }

    public class TradeRunningCreadedState : TradeRunningState
    {
        public TradeRunningCreadedState(ITrade trade)
        {
            _trade = trade;
        }

        public TradeRunningCreadedState(TradeRunningState state)
        {
            _trade = state.Trade;
        }

        public override string ToString()
        {
            return "CREATED";
        }

        public override void checkState()
        {
            ;
        }

        protected override void OnStateChanged(StateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }

    }

    public class TradeRunningRunningState : TradeRunningState
    {
        public TradeRunningRunningState(ITrade trade)
        {
            _trade = trade;
        }

        public TradeRunningRunningState(TradeRunningState state)
        {
            _trade = state.Trade;
        }

        public override string ToString()
        {
            return "RUNNING";
        }

        public override void checkState()
        {
            ;
        }

        protected override void OnStateChanged(StateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }
    }

    public class TradeRunningStoppedState : TradeRunningState
    {
        public TradeRunningStoppedState(ITrade trade)
        {
            _trade = trade;
        }

        public TradeRunningStoppedState(TradeRunningState state)
        {
            _trade = state.Trade;
        }

        public override string ToString()
        {
            return "STOPPED";
        }

        public override void checkState()
        {
            ;
        }

        protected override void OnStateChanged(StateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }
    }
    #endregion

    #region Status für den Handelszustand
    public abstract class TradeMoneyState : TradeStateAbstract 
    {
        public override void checkState()
        {
            //Noch keine Wetten =>
            if (_trade.Back.Bets.Count == 0 && _trade.Lay.Bets.Count == 0)
                return;


            log(String.Format("Trade State is {0}", this.ToString()));

            log(String.Format("Size of Back Bets {0}. Size of Lay Bets {1}", _trade.Back.BetSize, _trade.Lay.BetSize));

            if (_trade.Back.Bets.Count == 0 && _trade.Lay.Bets.Count != 0 && _trade.Lay.AllBetsUnmatched)
            {
                log("All Bets are unmatcheD");
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyUnmatchedState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }
            else if (_trade.Back.Bets.Count != 0 && _trade.Lay.Bets.Count == 0 && _trade.Back.AllBetsUnmatched)
            {
                log("All Bets are unmatcheD");
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyUnmatchedState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }
            else if (_trade.Back.AllBetsUnmatched && _trade.Lay.AllBetsUnmatched)
            {
                log("All Bets are unmatcheD");
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyUnmatchedState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }

            if (_trade.Back.Bets.Count == 0 && _trade.Lay.Bets.Count != 0 && _trade.Lay.AllBetsCanceled)
            {
                log("All Bets are unmatcheD");
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyCanceledState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }
            else if (_trade.Back.Bets.Count != 0 && _trade.Lay.Bets.Count == 0 && _trade.Back.AllBetsCanceled)
            {
                log("All Bets are unmatcheD");
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyCanceledState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }
            else if (_trade.Back.AllBetsCanceled && _trade.Lay.AllBetsCanceled)
            {
                log("All Bets are unmatcheD");
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyCanceledState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }

            if (_trade.Back.BetSize == _trade.Lay.BetSize)
            {
                log("Back Betsize is eqaul to Lay Betsize");
                //_trade.TradeState = new TradeMoneyHedgedState(this);
                //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyHedgedState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }


            if (_trade.TradeMode == TRADEMODE.BACK)
            {
                if (_trade.Lay.BetSize > _trade.Back.BetSize)
                {
                    log("Lay Betsize is greater than Back Betsize");
                    //_trade.TradeState = new TradeMoneyGreenedState(this);
                    //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                    StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyGreenedState(this.Trade), this);
                    log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                    OnStateChanged(e);
                    return;
                }
            }
            else
            {
                if (_trade.Lay.BetSize < _trade.Back.BetSize)
                {
                    log("Back Betsize is greater than Lay Betsize");
                    //_trade.TradeState = new TradeMoneyGreenedState(this);
                    //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                    StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyGreenedState(this.Trade), this);
                    log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                    OnStateChanged(e);
                    return;
                }
            }

            // Kein andere Status gefunden, dann per default auf Open

            StateChangedEventArgs eDefault = new StateChangedEventArgs(new TradeMoneyOpenState(this.Trade), this);
            log(String.Format("Trade State changed to {0}", eDefault.NewState.ToString()));
            OnStateChanged(eDefault);
        }

        protected override void OnStateChanged(StateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }
    }

    public class TradeMoneyCreatedState : TradeMoneyState
    {
        public TradeMoneyCreatedState(ITrade trade)
        {
            _trade = trade;
        }

        public TradeMoneyCreatedState(TradeMoneyState state)
        {
            _trade = state.Trade;
        }

        public override string ToString()
        {
            return "CREATED";
        }


        /*
        public override void checkState()
        {
            log(String.Format("Trade State is {0}", this.ToString()));            
            //_trade.TradeState = new TradeMoneyOpenState(this);
            //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);            
            StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyOpenState(this.Trade), this);            
            log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
            OnStateChanged(e);
        }
         * */

      
    }

    public class TradeMoneyCanceledState : TradeMoneyState
    {
        public TradeMoneyCanceledState(ITrade trade)
        {
            _trade = trade;
        }


        public TradeMoneyCanceledState(TradeMoneyState state)
        {
            _trade = state.Trade;
        }
    }

    public class TradeMoneyUnmatchedState : TradeMoneyState
    {

        public TradeMoneyUnmatchedState(ITrade trade) 
        {
            _trade = trade;
        }


        public TradeMoneyUnmatchedState(TradeMoneyState state)
        {
            _trade = state.Trade;
        }

        public override string ToString()
        {
            return "UNMATCHED";
        }

    }

    public class TradeMoneyOpenState : TradeMoneyState
    {
        public TradeMoneyOpenState(ITrade trade)
        {
            _trade = trade;
        }

        public TradeMoneyOpenState(TradeMoneyState state)
        {
            _trade = state.Trade;
        }

        public override string ToString()
        {
            return "OPENED";
        }
/*
        public override void checkState()
        {
            //Noch keine Wetten =>
            if (_trade.Back.Bets.Count == 0 || _trade.Lay.Bets.Count == 0)
                return;


            log(String.Format("Trade State is {0}", this.ToString()));

            log(String.Format("Size of Back Bets {0}. Size of Lay Bets {1}", _trade.Back.BetSize, _trade.Lay.BetSize));
            if (_trade.Back.BetSize == _trade.Lay.BetSize)
            {
                log("Back Betsize is eqaul to Lay Betsize");
                //_trade.TradeState = new TradeMoneyHedgedState(this);
                //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyHedgedState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }

            if (_trade.Lay.BetSize > _trade.Back.BetSize)
            {
                log("Lay Betsize is greater than Back Betsize");
                //_trade.TradeState = new TradeMoneyGreenedState(this);
                //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyGreenedState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }
        }

        protected override void OnStateChanged(StateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }
 * */
    }

    public class TradeMoneyHedgedState : TradeMoneyState
    {
        public TradeMoneyHedgedState(ITrade trade)
        {
            _trade = trade;
        }

        public TradeMoneyHedgedState(TradeMoneyState state)
        {
            _trade = state.Trade;
        }

        public override string ToString()
        {
            return "HEDGED";
        }
/*
        public override void checkState()
        {
            log(String.Format("Trade State is {0}", this.ToString()));

            log(String.Format("Size of Back Bets {0}. Size of Lay Bets {1}", _trade.Back.BetSize, _trade.Lay.BetSize));

            if (_trade.Lay.BetSize > _trade.Back.BetSize)
            {
                log("Lay Betsize is greater than Back Betsize");
                //_trade.TradeState = new TradeMoneyGreenedState(this);
                //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyGreenedState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }

            if (_trade.Lay.BetSize < _trade.Back.BetSize)
            {
                log("Lay Betsize is lower than Back Betsize");
                //_trade.TradeState = new TradeMoneyOpenState(this);
                //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyOpenState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }
        }

        protected override void OnStateChanged(StateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }
 * */
    }

    public class TradeMoneyGreenedState : TradeMoneyState
    {
        public TradeMoneyGreenedState(ITrade trade)
        {
            _trade = trade;
        }

        public TradeMoneyGreenedState(TradeMoneyState state)
        {
            _trade = state.Trade;
        }

        public override string ToString()
        {
            return "GREENED";
        }
/*
        public override void checkState()
        {
            log(String.Format("Trade State is {0}", this.ToString()));

            log(String.Format("Size of Back Bets {0}. Size of Lay Bets {1}", _trade.Back.BetSize, _trade.Lay.BetSize));

            if (_trade.Lay.BetSize < _trade.Back.BetSize)
            {
                log("Lay Betsize is lower than Back Betsize");
                //_trade.TradeState = new TradeMoneyOpenState(this);
                //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyOpenState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }

            if (_trade.Lay.BetSize == _trade.Back.BetSize)
            {
                log("Lay Betsize is equal Back Betsize");
                //_trade.TradeState = new TradeMoneyHedgedState(this);
                //StateChangedEventArgs e = new StateChangedEventArgs(_trade.TradeState, this);
                StateChangedEventArgs e = new StateChangedEventArgs(new TradeMoneyHedgedState(this.Trade), this);
                log(String.Format("Trade State changed to {0}", e.NewState.ToString()));
                OnStateChanged(e);
                return;
            }
        
        }

        protected override void OnStateChanged(StateChangedEventArgs e)
        {
            base.OnStateChanged(e);
        }
        */
    }
    #endregion
}
