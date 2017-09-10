using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.plugin;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.betfairif;
using net.sxtrader.bftradingstrategies.sxhelper;

namespace net.sxtrader.bftradingstrategies.LayThe4
{
    public class PluginLay4 : IPlugin
    {
        string strName = LayThe4.strLayThe4;
        string strDescription = LayThe4.strLayThe4Descr;
        string strAuthor = "Markus Heid";
        string strVersion = "1.0.0";
        System.Windows.Forms.UserControl ctlLay4 = new cltLayThe4_v2();
        System.Windows.Forms.UserControl ctlConfiguration = new ctlConfigLT4();
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
            get { return LayThe4.strFQLayThe4; }
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
            get { return ctlLay4; }
        }

        public System.Windows.Forms.UserControl FastBetInterface
        {
            get { return null; }
        }

        public void Initialize(Object[] parameters, LiveScoreParser parser)
        {
            
            if (parameters == null)
                throw new Exception(String.Format(LayThe4.strNoParameter, "IPlugIn Initialize"));
            if (parameters.Length != 3)
                throw new Exception(String.Format(LayThe4.strWrongParameterCount, "IPlugIn Initialize"));
            /*
            if (parameters[0].GetType() != typeof(LiveScoreParser))
                throw new Exception(String.Format(LayThe4.strWrongParameterType, 1, typeof(LiveScoreParser).ToString()));            
             */
            if (parameters[0].GetType() != typeof(int))
                throw new Exception(String.Format(LayThe4.strWrongParameterType, 2, typeof(int).ToString()));
            if (parameters[1].GetType() != typeof(LiveScore2Parser))
                throw new Exception(String.Format(LayThe4.strWrongParameterType, 2, typeof(LiveScore2Parser).ToString()));
            
            m_parser = parser;//(LiveScoreParser)parameters[0];
            m_parser2 = (LiveScore2Parser)parameters[1];

            BetfairKom.ShutdownRequest += new EventHandler<EventArgs>(BetfairKom_ShutdownRequest);
            ((cltLayThe4_v2)ctlLay4).initWatcher(m_parser, m_parser2);
            ((cltLayThe4_v2)ctlLay4).initHost(objHost);
            ((IBFTSCommon)ctlLay4).ExceptionMessageEvent += new EventHandler<net.sxtrader.bftradingstrategies.sxhelper.SXExceptionMessageEventArgs>(PluginLay4_ExceptionMessageEvent);
            
        }

        void PluginLay4_ExceptionMessageEvent(object sender, net.sxtrader.bftradingstrategies.sxhelper.SXExceptionMessageEventArgs e)
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

        public void Dispose()
        {

        }

        public System.Windows.Forms.UserControl ConfigurationInterface
        {
            get { return ctlConfiguration; }
        }
        #endregion
    



public Guid GUID
{
    get { return new Guid("eba366d0-6b25-11de-8a39-0800200c9a66"); }
}

}
}
