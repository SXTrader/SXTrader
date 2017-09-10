using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace net.sxtrader.bftradingstrategies.bfuestrategy
{
    public class BFUEFBIPTraderConfigList : List<BFUEFBIPTraderConfigElement>
    {
    }

    public enum IPTScores{ ZEROZERO, ONEONE, TWOTWO, THREETHREE,UNASSIGNED};
    public enum IPTRedCardWatch { NOREDCARD, EQUALCARD, TEAMAMORE, TEAMBMORE, UNASSIGNED };
    public class ScoreList : List<IPTScores>{}
    public class BFUEFBIPTraderConfigElement
    {
        private static int objCounter = 0;
        private int _objNumber;
        private int _lowPlaytime;
        private int _highPlaytime;
        private  ScoreList _scores;
        private bool _active = false;
        private double _lowOdds;
        private double _highOdds;
        private long _lowVolume;
        private long _highVolume;
        private bool _setteledAllowed;
        private bool _unsetteledAllowed;
        private bool _alreadyTraded;
        private bool _redCardWatch =false;
        private IPTRedCardWatch _redCardWatchState = IPTRedCardWatch.UNASSIGNED;
        private LTDConfigurationRW _tradeConfig;

        public LTDConfigurationRW TradeConfig
        {
            get
            {
                return _tradeConfig;
            }
            set
            {
                _tradeConfig = value;
            }
        }

        public int ElementNumber
        {
            get
            {
                return _objNumber;
            }
        }

        public IPTRedCardWatch RedCardWatchState
        {
            get { return _redCardWatchState; }
            set { _redCardWatchState = value; }
        }

        public bool RedCardWatch
        {
            get { return _redCardWatch; }
            set { _redCardWatch = value; }
        }

        public bool AlreadyTraded
        {
            get
            {
                return _alreadyTraded;
            }
            set
            {
                _alreadyTraded = value;
            }
        }

        public long LoVolume
        {
            get
            {
                return _lowVolume;
            }
            set
            {
                _lowVolume = value;
            }
        }

        public long HiVolume
        {
            get
            {
                return _highVolume;
            }
            set
            {
                _highVolume = value;
            }
        }

        public int LoPlaytime
        {
            get
            {
                return _lowPlaytime;
            }
            set
            {
                _lowPlaytime = value;
            }
        }

        public int HiPlayTime
        {
            get
            {
                return _highPlaytime;
            }
            set
            {
                _highPlaytime = value;
            }
        }

        public double LoOdds
        {
            get
            {
                return _lowOdds;
            }
            set
            {
                _lowOdds = value;
            }
        }

        public double HiOdds
        {
            get
            {
                return _highOdds;
            }
            set
            {
                _highOdds = value;
            }
        }

        public bool SettledAllowed
        {
            get
            {
                return _setteledAllowed;
            }
            set
            {
                _setteledAllowed = value;
            }
        }

        public bool UnsettledAllowed
        {
            get
            {
                return _unsetteledAllowed;
            }
            set
            {
                _unsetteledAllowed = value;
            }
        }

        public IPTScores[] Scores
        {
            get
            {
                return _scores.ToArray();
            }
        }

        public void addScore(IPTScores score)
        {
            bool bFound = false;
            for (int i = 0; i < _scores.Count; i++)
            {
                if (_scores[i] == score)
                    bFound = true;
            }
            if (!bFound)
                _scores.Add(score);
        }

        public void removeScore(IPTScores score)
        {
            _scores.Remove(score);
        }

        public static BFUEFBIPTraderConfigElement createNew()
        {
            return new BFUEFBIPTraderConfigElement();
        }

        private BFUEFBIPTraderConfigElement()
        {
            _objNumber = ++objCounter;
            _scores = new ScoreList();
            _tradeConfig = new LTDConfigurationRW();
            _highOdds = _lowOdds = 1.01;
            _highVolume = 10000000;
        }

        public override String ToString()
        {
            String strReturn = String.Format("Start a trade\r\nIf Playtime is between {0} and {1}. ", _lowPlaytime, _highPlaytime);
            String scores = String.Empty;
            foreach (IPTScores score in _scores)
            {
                switch(score)
                {
                    case IPTScores.ZEROZERO:
                        scores += "0 - 0";
                        break;
                    case IPTScores.ONEONE:
                        scores += "1 - 1";
                        break;
                    case IPTScores.TWOTWO:
                        scores += "2 - 2";
                        break;
                    case IPTScores.THREETHREE:
                        scores += "3 - 3";
                        break;
                }
                scores += ", ";
            }
            if (scores.EndsWith(", "))
                scores = scores.Substring(0, scores.Length - 2);
            strReturn = strReturn + String.Format("And if scores are {0}.\r\n", scores);
            strReturn = strReturn + String.Format("And if odds are between {0} and {1}.\r\n", _lowOdds, _highOdds);
            strReturn = strReturn += String.Format("And if market volume is between {0} and {1}.\r\n", _lowVolume, _highVolume);
            if (_setteledAllowed)
            {
                strReturn = strReturn + "Reopen a finished trade if neccessary. ";
            }
            else
            {
                strReturn = strReturn + "But not if finished trade exists for market.\r\n";
            }

            if (_unsetteledAllowed)
            {
                strReturn = strReturn + "Even if there is an unfinished trade.\r\n";
            }
            else
            {
                strReturn = strReturn + "But not it unfinished trade exists for market\r\n";
            }

            if (_redCardWatch)
            {
                strReturn = strReturn + "Observe the Red Cards in Match and ";
                switch (_redCardWatchState)
                {
                    case IPTRedCardWatch.NOREDCARD:
                        {
                            strReturn = strReturn + "trade only if there is no Red Card at all.\r\n";
                            break;
                        }
                    case IPTRedCardWatch.EQUALCARD:
                        {
                            strReturn = strReturn + "trade only if both teams have an equal amount of red cards.\r\n";
                            break;
                        }
                    case IPTRedCardWatch.TEAMAMORE:
                        {
                            strReturn = strReturn + "trade only if Team A has more red cards than Team B.\r\n";
                            break;
                        }
                    case IPTRedCardWatch.TEAMBMORE:
                        {
                            strReturn = strReturn + "trade only if Team B has more red cards than Team A.\r\n";
                            break;
                        }
                    case IPTRedCardWatch.UNASSIGNED:
                        {
                            strReturn = strReturn + "no behavior has defined yet.\r\n";
                            break;
                        }
                }
            }
            else
            {
                strReturn = strReturn + "Don't Observe the Red Cards in Match.\r\n";
            }

            if (_alreadyTraded)
            {
                strReturn += "Step has already traded and won't be executed anymore!\r\n";
            }
            return strReturn;
        }
    }
}
