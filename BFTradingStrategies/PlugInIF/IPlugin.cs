using System;
using System.Collections.Generic;
using System.Text;
using net.sxtrader.bftradingstrategies.livescoreparser;

namespace net.sxtrader.plugin
{
    public interface IPlugin
    {
        IPluginHost Host { get; set; }

        string Name { get; }
        string FullQualifiedName { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
        System.Guid GUID { get; }

        System.Windows.Forms.UserControl MainInterface { get; }
        System.Windows.Forms.UserControl ConfigurationInterface { get; }
        System.Windows.Forms.UserControl FastBetInterface { get; }

        void Initialize(Object[] parameters, LiveScoreParser parser);
    }

    public interface IPluginHost
    {
        IPlugin[] PluginsArray {get;}

        void Feedback(string Feedback, IPlugin Plugin);
        void ErrorMessage(int errorNumber, string message);
        void AccountUpdate(double balance, double available, string currency);
    }
}
