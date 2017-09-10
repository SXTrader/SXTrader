using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.common
{
    public class PluginStatisticAnalyses : IPlugin, IDisposable
    {
        string strName = HistoryGraph.strStatisticAnalyses;
        string strFQName = HistoryGraph.strFQStatisticAnalyses;
        string strDescription = HistoryGraph.strDescriptionSA;
        string strAuthor = "SXTrader";
        string strVersion = "1.0.0";
        private bool _disposed = false;

        System.Windows.Forms.UserControl ctlMain = new ctlStatisticAnalyses();
        System.Windows.Forms.UserControl ctlConfig = new ctlConfiguration();
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
            get { return new Guid("d120ffc4-6536-42e2-8962-15a101dbd481"); }
        }

        public System.Windows.Forms.UserControl MainInterface
        {
            get { ((ctlStatisticAnalyses)ctlMain).PluginHost = objHost; return ctlMain; }
        }

        public System.Windows.Forms.UserControl ConfigurationInterface
        {
            get { return ctlConfig; }
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
                DebugWriter.Instance.WriteMessage("PluginStatisticsAnalyses", "Disposing");
                if (disposing)
                {
                    if (ctlMain != null)
                    {
                        ctlMain.Dispose();
                    }

                    if (ctlConfig != null)
                    {
                        ctlConfig.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
