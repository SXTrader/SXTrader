using System;
using System.Collections.Generic;
using System.Text;

namespace net.sxtrader.plugin
{
    public interface IHistory
    {
        void Historize(String segment, DateTime dts, String match, double winloose, bool test);
    }
}
