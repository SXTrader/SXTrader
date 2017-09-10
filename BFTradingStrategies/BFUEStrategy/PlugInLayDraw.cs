using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.bfuestrategy.controls;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.eventargs;
using net.sxtrader.muk.interfaces;
//using BFUEStrategy;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    public class PluginLayDraw : IPlugin, IDisposable
    {
        #region Variablen
       
        //Declarations of all our internal plugin variables
        string strName = LayTheDraw.strName; //"Lay the Draw";
        string strDescription = LayTheDraw.strDescription;
        string strAuthor = "SXTrader";
        string strVersion = "1.0.0";
        System.Windows.Forms.UserControl ctlLayDraw = new ctlLayDraw_v2();
        System.Windows.Forms.UserControl ctlConfiguration = new ctlConfiguration();
        System.Windows.Forms.UserControl ctlFastBet = new ctlFastLay();
        IPluginHost objHost;
        LiveScoreParser m_parser;
        LiveScore2Parser m_parser2;
        private bool _disposed = false;
        
        #endregion

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
            get { return strName ; }
        }

        public string FullQualifiedName
        {
            get { return LayTheDraw.strFQLayTheDraw; }
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

        public System.Windows.Forms.UserControl MainInterface
        {
            get { return ctlLayDraw; }
        }

        public void Initialize(Object[] parameters, LiveScoreParser parser)
        {
            if (parameters == null)
                throw new Exception(String.Format(LayTheDraw.strNoParameter, "IPlugIn Initialize"));
            if (parameters.Length != 3)
                throw new Exception(String.Format(LayTheDraw.strWrongParameterCount, "IPlugIn Initialize"));            
            /*
            if (parameters[0].GetType() != typeof(LiveScoreParser))
                throw new Exception(String.Format(LayTheDraw.strWrongParameterType, 1, typeof(LiveScoreParser).ToString()));
             */
            if (parameters[0].GetType() != typeof(int))
                throw new Exception(String.Format(LayTheDraw.strWrongParameterType, 2, typeof(int).ToString()));
            if(parameters[1].GetType() != typeof(LiveScore2Parser))
                throw new Exception(String.Format(LayTheDraw.strWrongParameterType, 2, typeof(LiveScore2Parser).ToString()));
            if(parameters[2].GetType() != typeof(bool))
                throw new Exception(String.Format(LayTheDraw.strWrongParameterType, 2, typeof(bool).ToString()));
            m_parser = parser;//(LiveScoreParser)parameters[0];
            m_parser2 = (LiveScore2Parser)parameters[1];
            SXALBetWatchdog.Instance.WaitTime = (int)parameters[0];
            
            ((ctlFastLay)ctlFastBet).ConfirmFastBet = (bool)parameters[2];

            SXALKom.ShutdownRequest += new EventHandler<EventArgs>(BetfairKom_ShutdownRequest);
            ((ctlLayDraw_v2)ctlLayDraw).initWatcher(m_parser, m_parser2);
            ((ctlLayDraw_v2)ctlLayDraw).initHost(objHost);
            
            ((ctlFastLay)ctlFastBet).DrawWatcher = ((ctlLayDraw_v2)ctlLayDraw).Watcher;
            ((IBFTSCommon)ctlLayDraw).ExceptionMessageEvent += new EventHandler<SXExceptionMessageEventArgs>(PlugInLayDraw_ExceptionMessageEvent);
        }

        void PlugInLayDraw_ExceptionMessageEvent(object sender, SXExceptionMessageEventArgs e)
        {
            if (objHost != null)
            {
                objHost.ErrorMessage(e.MessageNumber, e.ToString());
            }
        }

        void BetfairKom_ShutdownRequest(object sender, EventArgs e)
        {
            objHost.Feedback("Shutdown", this);
            //throw new NotImplementedException();
        }
        #endregion

        #region IPlugin Member

        public System.Windows.Forms.UserControl FastBetInterface
        {
            get {return ctlFastBet; }
        }

        public System.Windows.Forms.UserControl ConfigurationInterface
        {
            get { return ctlConfiguration; }
        }

        public Guid GUID
        {
            get { return new Guid("3b0219a0-6b2c-11de-8a39-0800200c9a66"); }
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
                DebugWriter.Instance.WriteMessage("PluginLayDraw", "Disposing");
                if (disposing)
                {
                    if (ctlLayDraw != null)
                    {
                        ctlLayDraw.Dispose();
                    }

                    if (ctlConfiguration != null)
                    {
                        ctlConfiguration.Dispose();
                    }

                    if (ctlFastBet != null)
                    {
                        ctlFastBet.Dispose();
                    }
                }               
                _disposed = true;
            }
        }
        #endregion
    }
}
