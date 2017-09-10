using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveScoreCrawlerGen2
{
    public class CrawlerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private StatisticCrawler _crawler;

        public CrawlerViewModel()
        {
            _crawler = new StatisticCrawler();
            _crawler.StartWork();  
        }

    }
}
