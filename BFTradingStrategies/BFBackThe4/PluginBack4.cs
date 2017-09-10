using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.betfairif;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.BackThe4
{
    public class PluginBack4 : IPlugin
    {
        string strName = BackThe4.strBackThe4;
        string strFQName = BackThe4.strFQBackThe4;
        string strDescription = BackThe4.strBackThe4Descr;
        string strAuthor = "Markus Heid";
        string strVersion = "1.0.0";
        //System.Windows.Forms.UserControl ctlBack4 = new ctlBackThe4();
        System.Windows.Forms.UserControl ctlBack4 = new ctlBackThe4_v2();
        System.Windows.Forms.UserControl ctlConfiguration = new ctlConfigBT4();
        IPluginHost objHost;
        LiveScoreParser m_parser;
        LiveScore2Parser m_parser2;
         
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
            get { return new Guid("39B1CE95-A1ED-41F5-8536-AF2F383DB572"); }
        }

        public System.Windows.Forms.UserControl MainInterface
        {
            get { return ctlBack4; }
        }

        public System.Windows.Forms.UserControl ConfigurationInterface
        {
            get { return ctlConfiguration; }
        }

        public System.Windows.Forms.UserControl FastBetInterface
        {
            get { return null; }
        }

        public void Initialize(object[] parameters, LiveScoreParser parser)
        {
            DebugWriter.Instance.WriteMessage("BackThe4", "Plugin Initializing");
            if (parameters == null)
                throw new Exception(String.Format(BackThe4.strNoParameter, "IPlugIn Initialize"));
            if (parameters.Length != 3)
                throw new Exception(String.Format(BackThe4.strWrongParameterCount, "IPlugIn Initialize"));
            /*
            if (parameters[0].GetType() != typeof(LiveScoreParser))
                throw new Exception(String.Format(BackThe4.strWrongParameterType, 1, typeof(LiveScoreParser).ToString()));
             */
            if (parameters[0].GetType() != typeof(int))
                throw new Exception(String.Format(BackThe4.strWrongParameterType, 2, typeof(int).ToString()));
            if (parameters[1].GetType() != typeof(LiveScore2Parser))
                throw new Exception(String.Format(BackThe4.strWrongParameterType, 2, typeof(LiveScore2Parser).ToString()));

            m_parser = parser;//(LiveScoreParser)parameters[0];
            m_parser2 = (LiveScore2Parser)parameters[1];

            BetWatchdog.Instance.WaitTime = (int)parameters[0];

            BetfairKom.ShutdownRequest += new EventHandler<EventArgs>(BetfairKom_ShutdownRequest);

            ((ctlBackThe4_v2)ctlBack4).initWatcher(m_parser, m_parser2);
            ((ctlBackThe4_v2)ctlBack4).initHost(objHost);
            ((IBFTSCommon)ctlBack4).ExceptionMessageEvent += new EventHandler<net.sxtrader.bftradingstrategies.sxhelper.SXExceptionMessageEventArgs>(PluginBack4_ExceptionMessageEvent);
             
        }

        void PluginBack4_ExceptionMessageEvent(object sender, net.sxtrader.bftradingstrategies.sxhelper.SXExceptionMessageEventArgs e)
        {
            if (objHost != null)
            {
                objHost.ErrorMessage(e.MessageNumber, e.ToString());
            }

        }

        public void Dispose()
        {            
        }

        #endregion

        void BetfairKom_ShutdownRequest(object sender, EventArgs e)
        {
            objHost.Feedback("Shutdown", this);
            //throw new NotImplementedException();
        }
    }
}
