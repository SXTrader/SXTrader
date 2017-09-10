using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.common
{
    public class PluginLivescoreMapper : IPlugin, IDisposable
    {
        string strName = HistoryGraph.strMapper;
        string strFQName = HistoryGraph.strFQMapper;
        string strDescription = HistoryGraph.strDescriptionMapping;
        string strAuthor = "SXTrader";
        string strVersion = "1.0.0";
        private bool _disposed = false;

        System.Windows.Forms.UserControl ctlMapper = new ctlLivescores();
        IPluginHost objHost;

        #region IPlugin Member

        public IPluginHost Host
        {
            get
            {
                return objHost;
            }
            set
            {
                objHost = value;
            }
        }

        public string Name
        {
            get { return strName; }
        }

        public string FullQualifiedName
        {
            get { return strFQName; }
        }

        public string Description
        {
            get { return strDescription; }
        }

        public string Author
        {
            get { return strAuthor; }
        }

        public string Version
        {
            get { return strVersion; }
        }

        public Guid GUID
        {
            get { return new Guid("65114ccd-c995-4fcc-8174-7035f61ba091"); }
        }

        public System.Windows.Forms.UserControl MainInterface
        {
            get { return ctlMapper; }
        }

        public System.Windows.Forms.UserControl ConfigurationInterface
        {
            get { return null; }
        }

        public System.Windows.Forms.UserControl FastBetInterface
        {
            get { return null; }
        }

        public void Initialize(object[] parameters, net.sxtrader.bftradingstrategies.livescoreparser.LiveScoreParser parser)
        {
         
        }
        #endregion

        #region IDisposable Member
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DebugWriter.Instance.WriteMessage("PluginLivescoreMapper", "Disposing");
                if (disposing)
                {
                    if (ctlMapper != null)
                    {
                        ctlMapper.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
