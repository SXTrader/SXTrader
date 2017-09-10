using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.livescoreparser;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Diagnostics;
using System.Threading;

namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    // Abstrahierung der Wettbörsen Märkte
    // TODO: Eigentliche Wettbörsenfunktionalitäten aufrufen.
    public class SXALMarket
    {
        private string _name;
        private string _match;
        private string _devision;
        private long _id;
        private bool _inPlay;
        private DateTime _startDts;
        private SXALSelection[] _selections;
        private object _lockLivetickerConnector = "connectLiveticker";

        private bool _livetickerInitialized = false;

        public event EventHandler<SXALMarketLivetickerAddedEventArgs> MarketLivetickerAdded;
        public event EventHandler<SXALMarketRemoveEventArgs> MarketToBeRemoved;

        private delegate void AsyncLivetickerConnect(bool liveticker2connect);

        public long Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public String Match
        {
            get
            {
                return _match.Trim();
            }
            set
            {
                _match = value.Trim();
                _lockLivetickerConnector = "connectLiveticker" + _match;
            }
        }

        public String TeamA
        {
            get
            {
                String[] teams = _match.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (teams != null && teams.Length > 1)
                    return teams[0].Trim();

                return String.Empty;
            }
        }

        public String TeamB
        {
            get
            {
                String[] teams = _match.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (teams != null && teams.Length > 1)
                    return teams[1].Trim();

                return String.Empty;
            }
        }

        public String Devision
        {
            get
            {
                return _devision;
            }
            set
            {
                _devision = value;
            }
        }

        public DateTime StartDTS
        {
            get
            {
                return _startDts;
            }
            set
            {
                _startDts = value;
            }
        }

        public SXALSelection[] Selections
        {
            get { return _selections; }
            set{_selections = value;}
        }

        public bool IsGenericOverUnder
        {
            get
            {
                if (_name.StartsWith("Over/Under", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder05
        {
            get
            {
                if (_name.Equals("Over/Under 0.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder15
        {
            get
            {
                if (_name.Equals("Over/Under 1.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder25
        {
            get
            {
                if (_name.Equals("Over/Under 2.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder35
        {
            get
            {
                if (_name.Equals("Over/Under 3.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder45
        {
            get
            {
                if (_name.Equals("Over/Under 4.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder55
        {
            get
            {
                if (_name.Equals("Over/Under 5.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder65
        {
            get
            {
                if (_name.Equals("Over/Under 6.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder75
        {
            get
            {
                if (_name.Equals("Over/Under 7.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsOverUnder85
        {
            get
            {
                if (_name.Equals("Over/Under 8.5 Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsMatchOdds
        {
            get
            {
                if (_name.Equals("Match Odds", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsUnmanaged
        {
            get
            {
                if (_name.EndsWith("Unmanaged", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsTotalGoals
        {
            get
            {
                if (_name.Equals("Total Goals", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public bool IsScoreMarket
        {
            get
            {
                if (_name.Equals("Correct Score", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public Boolean InPlayMarket
        {
            get
            {
                return _inPlay;
            }
            set
            {
                _inPlay = value;
            }
        }

        public IScore Liveticker
        {
            get
            {
                //return SXMatchLivescoreLinkList.Instance[this.Match].HLScore;
                if (!HLList.Instance.ContainsKey(this.Match))
                    return null;
                return HLList.Instance[this.Match];
            }
        }

        public SXALMarket()
        {
        }

        public void doManualLivetickerConnection()
        {
            bool bScore1Connected = HLList.Instance[this.Match].IsScore1Connected();
            bool bScore2Connected = HLList.Instance[this.Match].IsScore2Connected();

            if (!bScore1Connected || !bScore2Connected)
            {
                LivetickerHelper.doManualLivetickerConnection(this.Match);
            }

            //irgendwelche Liveticker hinzugekommen?
            if (bScore1Connected == false && HLList.Instance[this.Match].Score1 !=null)
            {
                EventHandler<SXALMarketLivetickerAddedEventArgs> livetickerAdded = MarketLivetickerAdded;
                if (livetickerAdded != null)
                {
                    livetickerAdded(this, new SXALMarketLivetickerAddedEventArgs(this, true));
                }
            }

            if (bScore2Connected == false && HLList.Instance[this.Match].Score2 != null)
            {
                EventHandler<SXALMarketLivetickerAddedEventArgs> livetickerAdded = MarketLivetickerAdded;
                if (livetickerAdded != null)
                {
                    livetickerAdded(this, new SXALMarketLivetickerAddedEventArgs(this, true));
                }
            }
        }
     

        private void _liveticker_LiveScoreRemovedEvent(object sender, LiveScoreRemovedEventArgs e)
        {
            DebugWriter.Instance.WriteMessage("SXALMarket", String.Format("Livetickers for match {0} have been removed completely. Inform other Instances", this.Match));
            EventHandler<SXALMarketRemoveEventArgs> marketToBeRemove = MarketToBeRemoved;
            if (marketToBeRemove != null)
            {
                marketToBeRemove(this, new SXALMarketRemoveEventArgs(this));
            }


            if (HLList.Instance.ContainsValue(e.Liveticker))
            {
                int index = HLList.Instance.IndexOfValue(e.Liveticker);
                if (index != -1)
                {
                    HLList.Instance.ElementAt(index).Value.Dispose();
                    HLList.Instance.RemoveAt(index);
                }
            }
        }

        public void forceRemoveSignal()
        {
            EventHandler<SXALMarketRemoveEventArgs> marketToBeRemove = MarketToBeRemoved;
            if (marketToBeRemove != null)
            {
                marketToBeRemove(this, new SXALMarketRemoveEventArgs(this));
            }
        }

        public void initializeLiveticker()
        {
            if (_livetickerInitialized)
            {
                if (HLList.Instance.ContainsKey(this.Match))
                {
                    HLList.Instance[this.Match].LiveScoreRemovedEvent += _liveticker_LiveScoreRemovedEvent;
                    HLList.Instance[this.Match].LiveScoreAddedEvent += _liveticker_LiveScoreAddedEvent; 
                }
                return;
            }

            if (!this.InPlayMarket)
            {
                //Trace.WriteLine(String.Format("Market {0} Match {1} is not an Inplay Market", this.Name, this.Match));
                _livetickerInitialized = true;
                return;
            }

            if (!HLList.Instance.ContainsKey(this.Match))
            {
                IScore score2 = LiveScore2Parser.Instance.linkSportExchange(this.TeamA, this.TeamB);
                IScore score1 = LiveScoreParser.Instance.linkSportExchange(this.TeamA, this.TeamB);

                HLLiveScore _liveticker = new HLLiveScore(score1, score2);
                _liveticker.BetfairMatch = this.Match;
                
                _liveticker.LiveScoreRemovedEvent += _liveticker_LiveScoreRemovedEvent;
                _liveticker.LiveScoreAddedEvent += _liveticker_LiveScoreAddedEvent;

                HLList.Instance.Add(this.Match, _liveticker);
            }

            _livetickerInitialized = true;
        }

        void _liveticker_LiveScoreAddedEvent(object sender, EventArgs e)
        {
            EventHandler<SXALMarketLivetickerAddedEventArgs> added = MarketLivetickerAdded;
            if (added != null)
            {
                added(this, new SXALMarketLivetickerAddedEventArgs(this, false));
            }
        }      
    }

    public class SXALMarketLivetickerAddedEventArgs : EventArgs
    {
        private SXALMarket _market;
        private bool _noUpdateWait;

        public SXALMarket Market
        {
            get
            {
                return _market;
            }
        }

        public bool NoUpdateWait
        {
            get { return _noUpdateWait; }
        }

        public SXALMarketLivetickerAddedEventArgs(SXALMarket market, bool noUpdateWait)
        {
            _noUpdateWait = noUpdateWait;
            _market = market;
        }
    }

    public class SXALMarketRemoveEventArgs : EventArgs
    {
        SXALMarket _market;

        public SXALMarket Market
        {
            get
            {
                return _market;
            }
        }

        public SXALMarketRemoveEventArgs(SXALMarket market)
        {
            _market = market;
        }
    }
}

