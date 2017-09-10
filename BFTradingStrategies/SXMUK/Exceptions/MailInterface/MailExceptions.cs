using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.muk.Exceptions.MailInterface
{
    public class NoIMAPConnectionException : Exception
    {
    }

    public class IMAPLogInFailedException : Exception
    {
    }

    public class NoEventTimeException : Exception
    {
    }

    public class GenericImapException : Exception
    {
        public GenericImapException()
            : base()
        {

        }

        public GenericImapException(String message)
            : base(message)
        {
        }

        public GenericImapException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
