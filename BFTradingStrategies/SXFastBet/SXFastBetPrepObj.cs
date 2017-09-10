using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxhelper;
using System.Web.Services.Protocols;
using System.Threading;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXAL.Exceptions;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.SXFastBet
{
    /// <summary>
    /// Fehlerinformationen zu einem Fehlerevents
    /// </summary>
    public class PrepBetErrorEventArgs : EventArgs
    {
        private String _message;

        public String Message { get { return _message; } }

        public PrepBetErrorEventArgs(String message)
        {
            _message = message;
        }
    }

    /// <summary>
    /// Eventklasse welche Daten über eine neue getätigte Vorbereitungswette enthält
    /// </summary>
    public class NewPrepBetEventArgs : EventArgs
    {
        private SXALBet _bet;

        public SXALBet Bet { get { return _bet; } }

        public NewPrepBetEventArgs(SXALBet bet)
        {
            _bet = bet;
        }

    }

    /// <summary>
    /// Eventklasse welche Daten über eine stornierte Vorbereitungswette enthält
    /// </summary>
    public class CancelPrepBetEventArgs : EventArgs
    {
        private SXALBet _bet;

        public SXALBet Bet { get { return _bet; } }

        public CancelPrepBetEventArgs(SXALBet bet)
        {
            _bet = bet;
        }

    }

    /// <summary>
    /// Eventklasse welche Daten über eine abgeschlossenen, egal ob erfüllt oder nicht, Verbereitungslauf enthält
    /// </summary>
    public class PreperationCompletedEvenArgs : EventArgs
    {
        
        private SXALBet _bet;

        public SXALBet Bet { get { return _bet; } }

        public PreperationCompletedEvenArgs(SXALBet bet)
        {
            _bet = bet;
        }
    }

    /// <summary>
    /// Tätigt eine konkrete Wette im Rahmen eines Fast Bet Moduls
    /// </summary>
    public class SXFastBetPrepObj
    {
        public enum PREPOBJSTATE {NONE, BETTING, WAITFORCOMPLETION, COMPLETE, CANCELATION, RUNNING};

        #region Klassenvariablen
        private long _marketId;
        private long _selectionId;
        private double _odds;
        private SXFastBetSettings _settings;
        private PREPOBJSTATE _state = PREPOBJSTATE.NONE;
        private Thread _preparer;
        private Thread _cancelThread;
        private Thread _unmatchedThread;
        private SXALBet _unmatchedBet;
        #endregion

        #region Ereignisse
        public event EventHandler<NewPrepBetEventArgs> NewPrepBet;
        public event EventHandler<PreperationCompletedEvenArgs> PreperationCompleed;
        public event EventHandler<CancelPrepBetEventArgs> CancelPrepBet;
        public event EventHandler<PrepBetErrorEventArgs> PrepBetError;
        #endregion

        public SXFastBetPrepObj(long marketId,double odds, SXFastBetSettings settings)
        {
            _marketId = marketId;
            _settings = settings;
            _odds = odds;
            _preparer = new Thread(preparerRunner);
        }

        private void unmatchedRunner()
        {
            while (true)
            {
                Thread.Sleep(_settings.UnmatchedWaitSeconds * 1000);
                _state = PREPOBJSTATE.RUNNING;
                //Details lesen
                if (_unmatchedBet == null)
                {
                    EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                    if (prepCompletedHandler != null)
                    {
                        prepCompletedHandler(this, new PreperationCompletedEvenArgs(_unmatchedBet));
                    }
                    _state = PREPOBJSTATE.COMPLETE;
                    return;
                }
                try
                {
                    _unmatchedBet = SXALKom.Instance.getBetDetail(_unmatchedBet.BetId);
                    if (_unmatchedBet.BetStatus == SXALBetStatusEnum.M)
                    {
                        EventHandler<NewPrepBetEventArgs> newPrepBetHandler = NewPrepBet;
                        if (newPrepBetHandler != null)
                        {
                            newPrepBetHandler(this, new NewPrepBetEventArgs(_unmatchedBet));
                        }
                        EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                        if (prepCompletedHandler != null)
                        {
                            prepCompletedHandler(this, new PreperationCompletedEvenArgs(_unmatchedBet));
                        }
                        _state = PREPOBJSTATE.COMPLETE;
                        return;
                    }
                    else if (_unmatchedBet.BetStatus == SXALBetStatusEnum.MU)
                    {
                        EventHandler<NewPrepBetEventArgs> newPrepBetHandler = NewPrepBet;
                        if (newPrepBetHandler != null)
                        {
                            newPrepBetHandler(this, new NewPrepBetEventArgs(_unmatchedBet));
                        }
                        _state = PREPOBJSTATE.NONE;
                        continue;
                    }
                    else if (_unmatchedBet.BetStatus == SXALBetStatusEnum.U)
                    {
                        _state = PREPOBJSTATE.NONE;
                        continue;
                    }
                    else
                    {
                        EventHandler<CancelPrepBetEventArgs> cancelPrepBetHandler = CancelPrepBet;
                        if (cancelPrepBetHandler != null)
                        {
                            cancelPrepBetHandler(this, new CancelPrepBetEventArgs(_unmatchedBet));
                        }

                        EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                        if (prepCompletedHandler != null)
                        {
                            prepCompletedHandler(this, new PreperationCompletedEvenArgs(_unmatchedBet));
                        }
                        _state = PREPOBJSTATE.COMPLETE;
                        return;
                    }
                }
                catch
                {
                }


            }
        }

        private void cancelRunner()
        {
            //Pseudoendlosschleife
            while (true)
            {
                Thread.Sleep(10000);
                _state = PREPOBJSTATE.RUNNING;
                //Details lesen
                if (_unmatchedBet == null)
                {
                    EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                    if (prepCompletedHandler != null)
                    {
                        prepCompletedHandler(this, new PreperationCompletedEvenArgs(_unmatchedBet));
                    }
                    _state = PREPOBJSTATE.COMPLETE;
                    return;
                }
                try
                {
                    _unmatchedBet = SXALKom.Instance.getBetDetail(_unmatchedBet.BetId);
                    // ist Wetter aktzeptiert
                    if (_unmatchedBet.BetStatus == SXALBetStatusEnum.M)
                    {
                        EventHandler<NewPrepBetEventArgs> newPrepBetHandler = NewPrepBet;
                        if (newPrepBetHandler != null)
                        {
                            newPrepBetHandler(this, new NewPrepBetEventArgs(_unmatchedBet));
                        }
                        EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                        if (prepCompletedHandler != null)
                        {
                            prepCompletedHandler(this, new PreperationCompletedEvenArgs(_unmatchedBet));
                        }
                        _state = PREPOBJSTATE.COMPLETE;
                        return;
                    }
                    // ist Wette teilakzeptiert
                    if (_unmatchedBet.BetStatus == SXALBetStatusEnum.MU)
                    {
                        EventHandler<NewPrepBetEventArgs> newPrepBetHandler = NewPrepBet;
                        if (newPrepBetHandler != null)
                        {
                            newPrepBetHandler(this, new NewPrepBetEventArgs(_unmatchedBet));
                        }


                        if (!SXALKom.Instance.cancelBet(_unmatchedBet.BetId))
                        {
                            continue;
                        }
                        else
                        {
                            EventHandler<CancelPrepBetEventArgs> cancelPrepBetHandler = CancelPrepBet;
                            if (cancelPrepBetHandler != null)
                            {
                                cancelPrepBetHandler(this, new CancelPrepBetEventArgs(_unmatchedBet));
                            }
                            _state = PREPOBJSTATE.COMPLETE;
                        }
                    }
                    // Wette ist immer noch nicht akzeptiert
                    if (_unmatchedBet.BetStatus == SXALBetStatusEnum.U)
                    {
                        continue;
                    }

                    EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler2 = PreperationCompleed;
                    if (prepCompletedHandler2 != null)
                    {
                        prepCompletedHandler2(this, new PreperationCompletedEvenArgs(_unmatchedBet));
                    }
                    // Alles andere i.O.
                    _state = PREPOBJSTATE.COMPLETE;
                    return;
                }
                catch(Exception exc)
                {
                    ExceptionWriter.Instance.WriteException(exc);
                }

            }
        }

        private void preparerRunner()
        {
            if (_state == PREPOBJSTATE.NONE)
            {
                //Pseudeendlosschleife
                while (true)
                {
                    double moneyToBet = 0.0;
                    try
                    {
                        moneyToBet = SXFastBetMoneyGetter.getMoney(_settings);
                    }
                    catch (SXFastBetBelowMinStackException)
                    {
                        EventHandler<PrepBetErrorEventArgs> prepBetErrorHandler = PrepBetError;
                        if (prepBetErrorHandler != null)
                        {
                            prepBetErrorHandler(this, new PrepBetErrorEventArgs(String.Format(SXFastBet.strBelowMinStakeErrorMessage, _marketId, SXALBankrollManager.Instance.MinStake)));
                        }
                        _state = PREPOBJSTATE.COMPLETE;
                        return;
                    }
                    catch (SXFastBetInsufficentFoundsExcpetion)
                    {
                        EventHandler<PrepBetErrorEventArgs> prepBetErrorHandler = PrepBetError;
                        if (prepBetErrorHandler != null)
                        {
                            prepBetErrorHandler(this, new PrepBetErrorEventArgs(String.Format(SXFastBet.strInsufficientFoundErrorMessage, _marketId, SXALBankrollManager.Instance.MinStake, SXALBankrollManager.Instance.Currency, "")));
                        }
                        return;
                    }
                    catch (Exception exc)
                    {
                        ExceptionWriter.Instance.WriteException(exc);
                        return;
                    }

                    // Hole Preisübersicht
                    // Setze Wette ab
                    _state = PREPOBJSTATE.BETTING;
                    try
                    {
                        SXALMarketPrices marketPrices = SXALKom.Instance.getMarketPrices(_marketId);
                        if (marketPrices.RunnerPrices.Length < 3)
                        {
                            _state = PREPOBJSTATE.NONE;
                            Thread.Sleep(60000);
                            continue;
                        }
                        _selectionId = marketPrices.RunnerPrices[2].SelectionId;
                        SXALBet newBet = null;
                        try
                        {
                            try
                            {
                                newBet = placeLayBet(_odds, moneyToBet);
                            }
                            catch (SXALNoBetBelowMinAllowedException)
                            {
                                //log("No Bet below MinStake allowed for the Exchange. Place Bet for MinStake instead");
                                newBet = placeLayBet(_odds, SXALKom.Instance.MinStake);
                            }
                            catch (SXALInsufficientFundsException sife)
                            {
                                ExceptionWriter.Instance.WriteException(sife);
                                continue;
                            }
                        }
                        catch (SXALBetInProgressException bipe)
                        {

                            //Betfair Zeit geben den Markt abzuschliessen
                            Thread.Sleep(3000);
                            SXALMUBet[] muBets = null;
                            if (bipe.BetId == 0)
                                muBets = SXALKom.Instance.getBetsMU(_marketId);
                            else
                                muBets = SXALKom.Instance.getBetMU(bipe.BetId);
                            if (muBets != null)
                            {
                                foreach (SXALMUBet muBet in muBets)
                                {
                                    //TODO: SelectionId für Drawmarkt noch in Konstante

                                    if (muBet.SelectionId != SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.DRAW, muBet.MarketId))
                                        continue;


                                    //Uns interessieren hier nur Lay-Wetten
                                    if (muBet.BetType == SXALBetTypeEnum.B)
                                        continue;

                                    //TODO: Bessere Lösung finden
                                    /*
                                    if (this._watcher.BetSet[MarketId] != null)
                                    {
                                        // Uns interessieren hier nur neue Wetten
                                        if (((BFUEStrategy)this._watcher.BetSet[MarketId]).Lay.Bets[muBet.betId] != null)
                                            continue;
                                    }
                                    */
                                    newBet = SXALKom.Instance.getBetDetail(muBet.BetId);

                                }
                            }
                        }

                        if (newBet == null)
                        {
                            _state = PREPOBJSTATE.NONE;
                            //TODO: Meldung nach aussen geben
                            Thread.Sleep(1000);
                            continue;
                        }

                        if (newBet.BetId == 0)
                        {
                            _state = PREPOBJSTATE.NONE;
                            throw new Exception();
                        }

                        //Falls Wette nicht erfüllt, anhand von Settings vorgehen
                        if (newBet.BetStatus == SXALBetStatusEnum.U)
                        {
                            _state = PREPOBJSTATE.WAITFORCOMPLETION;
                            if (_settings.CancelUnmatchedFlag)
                            {
                                _state = PREPOBJSTATE.CANCELATION;
                                if (!SXALKom.Instance.cancelBet(newBet.BetId))
                                {
                                    //Konnte nicht stornieren => Stornierungthread
                                    _unmatchedBet = newBet;
                                    _cancelThread = new Thread(cancelRunner);
                                    _cancelThread.IsBackground = true;
                                    _cancelThread.Start();
                                    return;
                                }
                                else
                                {
                                    EventHandler<CancelPrepBetEventArgs> cancelPrepBetHandler = CancelPrepBet;
                                    if (cancelPrepBetHandler != null)
                                    {
                                        cancelPrepBetHandler(this, new CancelPrepBetEventArgs(newBet));
                                    }

                                    EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                                    if (prepCompletedHandler != null)
                                    {
                                        prepCompletedHandler(this, new PreperationCompletedEvenArgs(newBet));
                                    }
                                    _state = PREPOBJSTATE.COMPLETE;
                                    return;
                                }
                            }
                            else
                            {
                                // Überwachungthread aufbauen
                                _unmatchedBet = newBet;
                                _unmatchedThread = new Thread(unmatchedRunner);
                                _unmatchedThread.IsBackground = true;
                                _unmatchedThread.Start();
                                return;
                            }
                        }
                        // Teilerfüllt
                        else if (newBet.BetStatus == SXALBetStatusEnum.MU)
                        {
                            _state = PREPOBJSTATE.WAITFORCOMPLETION;
                            EventHandler<NewPrepBetEventArgs> newPrepBetHandler = NewPrepBet;
                            if (newPrepBetHandler != null)
                            {
                                newPrepBetHandler(this, new NewPrepBetEventArgs(newBet));
                            }

                            if (_settings.CancelUnmatchedFlag)
                            {
                                _state = PREPOBJSTATE.CANCELATION;
                                if (!SXALKom.Instance.cancelBet(newBet.BetId))
                                {
                                    //Konnte nicht stornieren => Stornierungthread
                                    _unmatchedBet = newBet;
                                    _cancelThread = new Thread(cancelRunner);
                                    _cancelThread.IsBackground = true;
                                    _cancelThread.Start();
                                    return;
                                }
                                else
                                {
                                    EventHandler<CancelPrepBetEventArgs> cancelPrepBetHandler = CancelPrepBet;
                                    if (cancelPrepBetHandler != null)
                                    {
                                        cancelPrepBetHandler(this, new CancelPrepBetEventArgs(newBet));
                                    }

                                    EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                                    if (prepCompletedHandler != null)
                                    {
                                        prepCompletedHandler(this, new PreperationCompletedEvenArgs(newBet));
                                    }
                                    _state = PREPOBJSTATE.COMPLETE;
                                    return;
                                }
                            }
                            else
                            {
                                // Überwachungthread aufbauen
                                _unmatchedBet = newBet;
                                _unmatchedThread = new Thread(unmatchedRunner);
                                _unmatchedThread.IsBackground = true;
                                _unmatchedThread.Start();
                                return;
                            }
                        }
                        // Erfüllt
                        else if (newBet.BetStatus == SXALBetStatusEnum.M)
                        {
                            EventHandler<NewPrepBetEventArgs> newPrepBetHandler = NewPrepBet;
                            if (newPrepBetHandler != null)
                            {
                                newPrepBetHandler(this, new NewPrepBetEventArgs(newBet));
                            }
                            EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                            if (prepCompletedHandler != null)
                            {
                                prepCompletedHandler(this, new PreperationCompletedEvenArgs(newBet));
                            }
                            _state = PREPOBJSTATE.COMPLETE;
                            //TODO: Nachricht schicken
                            return;
                        }
                        // Alle andere Stati
                        else
                        {
                            EventHandler<PreperationCompletedEvenArgs> prepCompletedHandler = PreperationCompleed;
                            if (prepCompletedHandler != null)
                            {
                                prepCompletedHandler(this, new PreperationCompletedEvenArgs(newBet));
                            }
                            _state = PREPOBJSTATE.COMPLETE;
                            return;
                        }
                    }
                    catch (SXALMarketDoesNotExistException mdnee)
                    {
                        ExceptionWriter.Instance.WriteException(mdnee);
                        _state = PREPOBJSTATE.NONE;
                        return;
                    }
                    catch (SXALMarketNeitherSuspendedNorActiveException mnsnae)
                    {
                        ExceptionWriter.Instance.WriteException(mnsnae);
                        //EventHandler<SXMessageEventArgs> message = MessageEvent;                        
                        _state = PREPOBJSTATE.NONE;

                        return;
                    }
                    catch (SoapException soapExc)
                    {
                        _state = PREPOBJSTATE.NONE;
                        ExceptionWriter.Instance.WriteException(soapExc);

                        Thread.Sleep(1000);
                        //                    return;
                    }
                    catch (Exception exc)
                    {
                        _state = PREPOBJSTATE.NONE;
                        ExceptionWriter.Instance.WriteException(exc);
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        public void prepare()
        {
            if(sxhelper.SXThreadStateChecker.isStartedBackground(_preparer))
                return;

            _preparer.IsBackground = true;
            _preparer.Start();

            
        }

        private SXALBet placeLayBet(double price, double money)
        {
            return SXALKom.Instance.placeLayBet(_marketId, _selectionId, price, money);
        }
    }
}
