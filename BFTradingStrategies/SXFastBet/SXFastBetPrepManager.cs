using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.SXFastBet
{
    class SXFastBetPrepObjSortedList : SortedList<long, SXFastBetPrepObj> { }
    public class SXPrepAlreadyInProgressExc : Exception { };

    /// <summary>
    /// Fehlerinformationen zu einem Fehlerevents
    /// </summary>
    public class PrepMgrBetErrorEventArgs : EventArgs
    {
        private String _message;

        public String Message { get { return _message; } }

        public PrepMgrBetErrorEventArgs(String message)
        {
            _message = message;
        }
    }

    /// <summary>
    /// Eventklasse welche Daten über eine neue getätigte Vorbereitungswette enthält
    /// </summary>
    public class PrepMgrNewBetEventArgs : EventArgs
    {
        private SXALBet _bet;

        public SXALBet Bet { get { return _bet; } }

        public PrepMgrNewBetEventArgs(SXALBet bet)
        {
            _bet = bet;
        }

    }

    /// <summary>
    /// Eventklasse welche Daten über eine stornierte Vorbereitungswette enthält
    /// </summary>
    public class PrepMgrCancelBetEventArgs : EventArgs
    {
        private SXALBet _bet;

        public SXALBet Bet { get { return _bet; } }

        public PrepMgrCancelBetEventArgs(SXALBet bet)
        {
            _bet = bet;
        }

    }

    /// <summary>
    /// Eventklasse welche Daten über eine abgeschlossenen, egal ob erfüllt oder nicht, Verbereitungslauf enthält
    /// </summary>
    public class PrepMgrPrepCompletedEvenArgs : EventArgs
    {

        private SXALBet _bet;

        public SXALBet Bet { get { return _bet; } }

        public PrepMgrPrepCompletedEvenArgs(SXALBet bet)
        {
            _bet = bet;
        }
    }


    public class SXFastBetPrepManager
    {
        private SXFastBetPrepObjSortedList _prepObjList = null;

        #region Ereignisse
        public event EventHandler<PrepMgrNewBetEventArgs> NewPrepBet;
        public event EventHandler<PrepMgrPrepCompletedEvenArgs> PreperationCompleted;
        public event EventHandler<PrepMgrCancelBetEventArgs> CancelPrepBet;
        public event EventHandler<PrepMgrBetErrorEventArgs> PrepBetError;
        #endregion


        public SXFastBetPrepManager()
        {
            _prepObjList = new SXFastBetPrepObjSortedList();
        }

        public bool isPrepInProgress(long marketId)
        {
            return _prepObjList.ContainsKey(marketId);
        }

        public void startPreperation(long marketId, double odds, SXFastBetSettings settings)
        {
            if(_prepObjList.ContainsKey(marketId))
                throw new SXPrepAlreadyInProgressExc();

            SXFastBetPrepObj prepObj = new SXFastBetPrepObj(marketId, odds, settings);
            _prepObjList.Add(marketId, prepObj);
            prepObj.CancelPrepBet += new EventHandler<CancelPrepBetEventArgs>(prepObj_CancelPrepBet);
            prepObj.NewPrepBet += new EventHandler<NewPrepBetEventArgs>(prepObj_NewPrepBet);
            prepObj.PrepBetError += new EventHandler<PrepBetErrorEventArgs>(prepObj_PrepBetError);
            prepObj.PreperationCompleed += new EventHandler<PreperationCompletedEvenArgs>(prepObj_PreperationCompleed);
            prepObj.prepare();
        }

        void prepObj_PreperationCompleed(object sender, PreperationCompletedEvenArgs e)
        {
            _prepObjList.Remove(e.Bet.MarketId);
            EventHandler<PrepMgrPrepCompletedEvenArgs> prepCompletedHandler = PreperationCompleted;
            if (prepCompletedHandler != null)
            {
                prepCompletedHandler(this, new PrepMgrPrepCompletedEvenArgs(e.Bet));
            }

        }

        void prepObj_PrepBetError(object sender, PrepBetErrorEventArgs e)
        {
            EventHandler<PrepMgrBetErrorEventArgs> prepBetError = PrepBetError;
            if (prepBetError != null)
            {
                prepBetError(this, new PrepMgrBetErrorEventArgs(e.Message));
            }
        }

        void prepObj_NewPrepBet(object sender, NewPrepBetEventArgs e)
        {
            EventHandler<PrepMgrNewBetEventArgs> prepNewBet = NewPrepBet;
            if (prepNewBet != null)
            {
                prepNewBet(this, new PrepMgrNewBetEventArgs(e.Bet));
            }
        }

        void prepObj_CancelPrepBet(object sender, CancelPrepBetEventArgs e)
        {
            EventHandler<PrepMgrCancelBetEventArgs> prepCancelBet = CancelPrepBet;
            if (prepCancelBet != null)
            {
                prepCancelBet(this, new PrepMgrCancelBetEventArgs(e.Bet));
            }
        }

    }
}
