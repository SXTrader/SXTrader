
using System;
using System.Runtime.Serialization;
namespace net.sxtrader.bftradingstrategies.bfuestrategy.LayTheDraw2.Exceptions
{
    public class NoLTDTradeException : Exception
    {
        public NoLTDTradeException() : base()
        {}

        public NoLTDTradeException(string message) : base(message) { }

        public NoLTDTradeException(string message, Exception innerExeption) : base(message, innerExeption) { }

        public NoLTDTradeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}