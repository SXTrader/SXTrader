using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.plugin;

namespace net.sxtrader.bftradingstrategies.BFTSGUI
{
    /*
    public class AvailablePlugins : System.Collections.CollectionBase
    {
        public void Add(AvailablePlugin plugin)
        {
            this.List.Add(plugin);
        }

        public void Remove(AvailablePlugin plugin)
        {
            this.List.Remove(plugin);
        }

        public AvailablePlugin Find(String strNameOrPath)
        {
            AvailablePlugin toReturn = null;

            //Loop through all the plugins
            foreach (AvailablePlugin pluginOn in this.List)
            {
                //Find the one with the matching name or filename
                if ((pluginOn.Plugin.Name.Equals(strNameOrPath)) || pluginOn.AssemblyPath.Equals(strNameOrPath))
                {
                    toReturn = pluginOn;
                    break;
                }
            }
            return toReturn;
        }       
    }

    
    public class AvailablePlugin
    {
        private IPlugin m_plugin = null;
        private String m_assemblyPath = String.Empty;

        public IPlugin Plugin
        {
            get { return m_plugin; }
            set { m_plugin = value; }
        }
        public string AssemblyPath
        {
            get { return m_assemblyPath; }
            set { m_assemblyPath = value; }
        }
    }
     * */
}
