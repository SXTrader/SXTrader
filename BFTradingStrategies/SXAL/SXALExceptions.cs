using System;
namespace net.sxtrader.bftradingstrategies.SXAL.Exceptions
{
    public class SXALThrottleExceededException : Exception { }

    public class SXALBetInProgressException : Exception
    {
        long _betId;
        public long BetId { get { return _betId; } }

        public SXALBetInProgressException()
        {
            _betId = 0;
        }

        public SXALBetInProgressException(long betId)
        {
            _betId = betId;
        }
    }
}