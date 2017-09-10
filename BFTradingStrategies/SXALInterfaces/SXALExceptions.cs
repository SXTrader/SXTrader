using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    public class SXALNoBetBelowMinAllowedException : Exception
    {
        public SXALNoBetBelowMinAllowedException(): base() { }
        public SXALNoBetBelowMinAllowedException(String message) : base(message) { }
    }

    public class SXALMarketDoesNotExistException : Exception
    {
        public SXALMarketDoesNotExistException() : base () {}
        public SXALMarketDoesNotExistException(String message) : base(message) { }
    }

    public class SXALInsufficientFundsException : Exception
    {
        public SXALInsufficientFundsException() : base() { }
        public SXALInsufficientFundsException(String message) : base(message) { }
    }

    public class SXALEventDoesNotExistException : Exception
    {
        public SXALEventDoesNotExistException() : base() { }
        public SXALEventDoesNotExistException(String message) : base(message) { }
    }

    public class SXALMaxInputRecordExceedException : Exception
    {
        public SXALMaxInputRecordExceedException() : base() { }
        public SXALMaxInputRecordExceedException(String message) : base(message) { }
    }

    public class SXALMarketNeitherSuspendedNorActiveException : Exception
    {
        public SXALMarketNeitherSuspendedNorActiveException() : base() { }
        public SXALMarketNeitherSuspendedNorActiveException(String message) : base(message) { }
    }

    public class SXALBetDoesNotExistException : Exception
    {
        public SXALBetDoesNotExistException() : base() { }
        public SXALBetDoesNotExistException(String message) : base(message) { }
    }

    public class SXALPunterBetMismatchException : Exception
    {
        public SXALPunterBetMismatchException() : base() { }
        public SXALPunterBetMismatchException(String message) : base(message) { }
    }

    public class SXALDuplicateBetSpecifiedException : Exception
    {
        public SXALDuplicateBetSpecifiedException() : base() { }
        public SXALDuplicateBetSpecifiedException(String message) : base(message) { }
    }

    public class SXALMarketNotActiveException : Exception
    {
        public SXALMarketNotActiveException() : base() { }
        public SXALMarketNotActiveException(String message) : base(message) { }
    }

    public class SXALSelectionNotActiveException : Exception
    {
        public SXALSelectionNotActiveException() : base() { }
        public SXALSelectionNotActiveException(String message) : base(message) { }
    }

    public class SXALResetOccurredException : Exception
    {
        public SXALResetOccurredException() : base() { }
        public SXALResetOccurredException(String message) : base(message) { }
    }
}
