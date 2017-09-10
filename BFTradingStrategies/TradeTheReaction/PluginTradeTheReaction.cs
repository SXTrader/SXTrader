using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.plugin;
using System.Reflection;
using net.sxtrader.bftradingstrategies.ttr.GUI;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.SXFastBet;
using net.sxtrader.bftradingstrategies.ttr.GUI.Configuration;
using net.sxtrader.bftradingstrategies.SXAL;

namespace net.sxtrader.bftradingstrategies.ttr
{
    public class PluginTradeTheReaction : IPlugin, IDisposable
    {

        private IPluginHost _host;
        private System.Windows.Forms.UserControl _ctlTTR = new ctlTTR();
        private System.Windows.Forms.UserControl _ctlFB = new ctlFastBet();
        private System.Windows.Forms.UserControl _ctlCfg = new ctlConfiguration();
        LiveScoreParser _parser;
        LiveScore2Parser _parser2;
        private bool _disposed = false;
        #region IPlugin Member

        public IPluginHost Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
            }
        }

        public string Name
        {
            get { return TradeTheReaction.strName; }
        }

        public string FullQualifiedName
        {
            get { return TradeTheReaction.strFQTradeTheReaction; }
        }

        public string Description
        {
            get { return TradeTheReaction.strDescription; }
        }

        public string Author
        {
            get { return "SXTrader"; }
        }

        public string Version
        {
            get
            {
                Type type = this.GetType();
                Assembly assembly = Assembly.GetAssembly(type);
                AssemblyName assemblyName = assembly.GetName();
                Version version = assemblyName.Version;
                return version.ToString();
            }
        }

        public Guid GUID
        {
            get { return new Guid("{F195635A-2628-4c6c-986C-39062F2102A9}"); }
        }

        public System.Windows.Forms.UserControl MainInterface
        {
            get { return _ctlTTR; }
        }

        public System.Windows.Forms.UserControl ConfigurationInterface
        {
            get { return _ctlCfg; }
        }

        public System.Windows.Forms.UserControl FastBetInterface
        {
            get { return _ctlFB; }
        }

        public void Initialize(object[] parameters, net.sxtrader.bftradingstrategies.livescoreparser.LiveScoreParser parser)
        {
            if (parameters == null)
                throw new Exception(String.Format(TradeTheReaction.strNoParameter, "IPlugIn Initialize"));
            if (parameters.Length != 3)
                throw new Exception(String.Format(TradeTheReaction.strWrongParameterCount, "IPlugIn Initialize"));
            /*
            if (parameters[0].GetType() != typeof(LiveScoreParser))
                throw new Exception(String.Format(LayTheDraw.strWrongParameterType, 1, typeof(LiveScoreParser).ToString()));
             */
            if (parameters[0].GetType() != typeof(int))
                throw new Exception(String.Format(TradeTheReaction.strWrongParameterType, 2, typeof(int).ToString()));
            if (parameters[1].GetType() != typeof(LiveScore2Parser))
                throw new Exception(String.Format(TradeTheReaction.strWrongParameterType, 2, typeof(LiveScore2Parser).ToString()));
            if (parameters[2].GetType() != typeof(bool))
                throw new Exception(String.Format(TradeTheReaction.strWrongParameterType, 2, typeof(bool).ToString()));
            _parser = parser;//(LiveScoreParser)parameters[0];
            _parser2 = (LiveScore2Parser)parameters[1];
            SXALBetWatchdog.Instance.WaitTime = (int)parameters[0];
            
            ((ctlFastBet)_ctlFB).ConfirmFastBet = (bool)parameters[2];

            SXALKom.ShutdownRequest += new EventHandler<EventArgs>(BetfairKom_ShutdownRequest);
            
            ITradeMainGUI mainGUIInterface = _ctlTTR as ITradeMainGUI;
            if (mainGUIInterface != null)
            {
                mainGUIInterface.initWatcher(_parser, _parser2);
                mainGUIInterface.initHost(_host);


                ((ctlFastBet)_ctlFB).Watcher =  mainGUIInterface.Watcher as TTRWatcher; 
            }
            
            //((ctlFastBet)_ctlFB).Watcher = new TTRWatcher(_parser, _parser2);
            /*
            ((ctlLayDraw_v2)ctlLayDraw).initWatcher(m_parser, m_parser2);
            ((ctlLayDraw_v2)ctlLayDraw).initHost(objHost);
            ((ctlFastLay)ctlFastBet).DrawWatcher = ((ctlLayDraw_v2)ctlLayDraw).Watcher;
            ((IBFTSCommon)ctlLayDraw).ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(PlugInLayDraw_ExceptionMessageEvent);           
             */
        }

        void BetfairKom_ShutdownRequest(object sender, EventArgs e)
        {
            _host.Feedback("Shutdown", this);
            //throw new NotImplementedException();
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
                DebugWriter.Instance.WriteMessage("PluginTradeTheReaction", "Disposing");
                if (disposing)
                {
                    if (_ctlCfg != null)
                    {
                        _ctlCfg.Dispose();
                    }

                    if (_ctlFB != null)
                    {
                        _ctlFB.Dispose();
                    }

                    if (_ctlTTR != null)
                    {
                        _ctlTTR.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }
}