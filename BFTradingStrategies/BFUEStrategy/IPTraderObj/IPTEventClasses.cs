using System;
namespace net.sxtrader.bftradingstrategies.bfuestrategy.IPTraderObj
{
    public class BFUEIPTElementSaveEventArgs : EventArgs
    {
        BFUEFBIPTraderConfigElement _element;
        public BFUEFBIPTraderConfigElement ConfigElement
        {
            get
            {
                return _element;
            }
        }

        public BFUEIPTElementSaveEventArgs(BFUEFBIPTraderConfigElement element)
        {
            _element = element;
        }
    }
    public class BFUEIPTElementDeleteEventArgs : EventArgs
    {
        BFUEFBIPTraderConfigElement _element;
        public BFUEFBIPTraderConfigElement ConfigElement
        {
            get
            {
                return _element;
            }
        }

        public BFUEIPTElementDeleteEventArgs(BFUEFBIPTraderConfigElement element)
        {
            _element = element;
        }
    }

    public class BFUEIPTElementEditEventArgs : EventArgs
    {
        BFUEFBIPTraderConfigElement _element;
        public BFUEFBIPTraderConfigElement ConfigElement
        {
            get
            {
                return _element;
            }
        }

        public BFUEIPTElementEditEventArgs(BFUEFBIPTraderConfigElement element)
        {
            _element = element;
        }
    }

    public class BFUEIPTraderDialogCloseEventArgs : EventArgs
    {
        BFUEFBIPTraderConfigList _list;
        public BFUEFBIPTraderConfigList ConfigList
        {
            get
            {
                return _list;
            }
        }

        public BFUEIPTraderDialogCloseEventArgs(BFUEFBIPTraderConfigList list)
        {
            _list = list;
        }
    }
}