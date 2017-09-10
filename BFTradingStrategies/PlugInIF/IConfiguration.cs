using System;
using System.Collections.Generic;
using System.Text;

namespace net.sxtrader.plugin
{
    public interface IConfiguration
    {
        void save();
        String getXML();        
    }
}
