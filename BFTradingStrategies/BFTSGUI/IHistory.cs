using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.markus_heid.bftradingstrategies.plugin
{
    interface IHistory
    {
        void Historize(String module, DateTime dts, String match, int winloose);
    }
}
