using net.sxtrader.muk.eventargs;
using System;
namespace net.sxtrader.muk.interfaces
{

    public interface IBFTSCommon
    {
        event EventHandler<SXExceptionMessageEventArgs> ExceptionMessageEvent;
        event EventHandler<SXWMessageEventArgs> MessageEvent;
    }
}