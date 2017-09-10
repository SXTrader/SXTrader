using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.sxstatisticbase;
using net.sxtrader.bftradingstrategies.tradeinterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.TradeStarter
{
    public enum TTRScores { ZEROZERO, ZEROONE, ZEROTWO, ZEROTHREE,
                            ONEZERO, ONEONE, ONETWO, ONETHREE,
                            TWOZERO, TWOONE, TWOTWO, TWOTHREE,
                            THREEZERO, THREEONE, THREETWO, THREETHREE, 
                            OTHERS, UNASSIGNED };
    public enum TTRRedCardWatch { NOREDCARD, EQUALCARD, TEAMAMORE, TEAMBMORE, UNASSIGNED };

    public enum TRADESTATE { OPENED, HEDGED, GREENED, NOMATTER, UNASSIGNED };

    public enum RELATIVEBETTINGTYPE { STAKE, WINNING, UNASSIGNED };

    //public enum TRADEMODE { BACK, LAY }

    public class ScoreList : List<TTRScores> 
    {
        public static String getScoreString(TTRScores score)
        {
            String strTmp = String.Empty;
            switch (score)
            {
                case TTRScores.ZEROZERO:
                    strTmp = "0 - 0";
                    break;
                case TTRScores.ZEROONE:
                    strTmp = "0 - 1";
                    break;
                case TTRScores.ZEROTWO:
                    strTmp = "0 - 2";
                    break;
                case TTRScores.ZEROTHREE:
                    strTmp = "0 - 3";
                    break;
                case TTRScores.ONEZERO:
                    strTmp = "1 - 0";
                    break;
                case TTRScores.ONEONE:
                    strTmp = "1 - 1";
                    break;
                case TTRScores.ONETWO:
                    strTmp = "1 - 2";
                    break;
                case TTRScores.ONETHREE:
                    strTmp = "1 - 3";
                    break;
                case TTRScores.TWOZERO:
                    strTmp = "2 - 0";
                    break;
                case TTRScores.TWOONE:
                    strTmp = "2 - 1";
                    break;
                case TTRScores.TWOTWO:
                    strTmp = "2 - 2";
                    break;
                case TTRScores.TWOTHREE:
                    strTmp = "2 - 3";
                    break;
                case TTRScores.THREEZERO:
                    strTmp = "3 - 0";
                    break;
                case TTRScores.THREEONE:
                    strTmp = "3 - 1";
                    break;
                case TTRScores.THREETWO:
                    strTmp = "3 - 2";
                    break;
                case TTRScores.THREETHREE:
                    strTmp = "3 - 3";
                    break;
                case TTRScores.OTHERS:
                    strTmp = "OTHERS";
                    break;
            }
            return strTmp;
        }
    }

    public class TradeStarterConfigList : List<TradeStarterConfigElement>
    {
    }

    public class TradeStarterConfigElement
    {
        private static int objCounter = 0;
        private int _objNumber;


        private TRADETYPE _tradingType = TRADETYPE.UNASSIGNED;
        private TRADETYPE _requiredTrade = TRADETYPE.UNASSIGNED;
        private TRADESTATE _requiredTradeState = TRADESTATE.UNASSIGNED;
        private TRADEMODE _tradeMode = TRADEMODE.BACK;
        private bool _redCardWatch;
        private TTRRedCardWatch _redCardWatchState = TTRRedCardWatch.UNASSIGNED;
        private ScoreList _scores;
        private int _lowGoalSum;
        private int _highGoalSum;
        private int _lowPlaytime;
        private int _highPlaytime;
        //private bool _active = false; Not in use yet
        private double _lowOdds;
        private double _highOdds;
        private bool _preplayTrade;
        private double _preplayLowOdds;
        private double _preplayHighOdds;
        private long _preplayLowVolume;
        private long _preplayHighVolume;
        private bool _inplayTrade;
        private long _lowVolume;
        private long _highVolume;
        private bool _setteledAllowed;
        private bool _unsetteledAllowed;
        private bool _alreadyTraded;
        private StatisticSelectionList _statistics;
        private TTRConfigurationRW _config;

        /// <summary>
        /// Welcher Trade soll durchgeführt werden?
        /// </summary>
        public TRADETYPE TradeType
        {
            get { return _tradingType; }
            set { _tradingType = value; _config.TradeOutRules.TradeType = _tradingType; }
        }

        /// <summary>
        /// Welche Trade muss schon existieren, damit
        /// diese Regel ausgeführt werden kann
        /// </summary>
        public TRADETYPE RequiredTrade
        {
            get { return _requiredTrade; }
            set { _requiredTrade = value; }
        }

        /// <summary>
        /// In welchen Status muss sich der benötigte Trade befinden
        /// damit die Regel ausgeführt werden kann
        /// </summary>
        public TRADESTATE RequiredTradeState
        {
            get { return _requiredTradeState; }
            set { _requiredTradeState = value; }
        }

        /// <summary>
        /// Handelt es sich um eine Back- oder Lay-First-Strategie?
        /// </summary>
        public TRADEMODE TradeMode
        {
            get { return _tradeMode; }
            set { _tradeMode = value; }
        }

        /// <summary>
        /// Überwache die Situation der Roten Karten in der Begegnung
        /// </summary>
        public bool RedCardWatch
        {
            get { return _redCardWatch; }
            set { _redCardWatch = value; }
        }

        /// <summary>
        /// Falls die Überwachung der Roten Karten aktiviert ist, die Regel 
        /// im Bezug auf deas Verhältnis der Roten Karten zwischen den Teams
        /// </summary>
        public TTRRedCardWatch RedCardWatchState
        {
            get { return _redCardWatchState; }
            set { _redCardWatchState = value; }
        }

        /// <summary>
        /// Gültige Spielstände
        /// </summary>
        public TTRScores[] Scores
        {
            get
            {
                return _scores.ToArray();
            }
        }

        /// <summary>
        /// Anzahl der Tore ab der diese Regel gültig ist
        /// </summary>
        public int LoGoalSum
        {
            get
            {
                return _lowGoalSum;
            }
            set
            {
                _lowGoalSum = value;
            }
        }

        /// <summary>
        /// Anzahl der Tore bis diese Regel gültig ist
        /// </summary>
        public int HiGoalSum
        {
            get
            {
                return _highGoalSum;
            }
            set
            {
                _highGoalSum = value;
            }
        }

        /// <summary>
        /// Spielzeit, ab der diese Regel überprüft werden soll
        /// </summary>
        public int LoPlaytime
        {
            get { return _lowPlaytime; }
            set { _lowPlaytime = value; }
        }

        /// <summary>
        /// Spielzeit, bis der diese Regel überprüft werden soll
        /// </summary>
        public int HiPlaytime
        {
            get { return _highPlaytime; }
            set { _highPlaytime = value; }
        }

        /// <summary>
        /// Untere Grenze der Quote Inplay
        /// </summary>
        public double LoOdds
        {
            get { return _lowOdds; }
            set { _lowOdds = value; }
        }

        /// <summary>
        /// Obere Grenze der Quote Inplay
        /// </summary>
        public double HiOdds
        {
            get { return _highOdds; }
            set { _highOdds = value; }
        }

        /// <summary>
        /// Es soll auch schon Preplay überprüft werden.
        /// </summary>
        public bool Preplay
        {
            get { return _preplayTrade; }
            set { _preplayTrade = value; }
        }

        /// <summary>
        /// Untere Grenze der Quote Preplay
        /// </summary>
        public double PreplayLoOdds
        {
            get { return _preplayLowOdds; }
            set { _preplayLowOdds = value; }
        }

        /// <summary>
        /// Obere Grenze der Quote Preplay
        /// </summary>
        public double PreplayHiOdds
        {
            get { return _preplayHighOdds; }
            set { _preplayHighOdds = value; }
        }

        /// <summary>
        /// Minimales Marktvolumen Preplay
        /// </summary>
        public long PreplayLoVolume
        {
            get { return _preplayLowVolume; }
            set { _preplayLowVolume = value; }
        }

        /// <summary>
        /// Maximales Marktvolumen Preplay
        /// </summary>
        public long PreplayHiVolume
        {
            get { return _preplayHighVolume; }
            set { _preplayHighVolume = value; }
        }

        /// <summary>
        /// Es Inplay überprüft werden.
        /// </summary>
        public bool Inplay
        {
            get { return _inplayTrade; }
            set { _inplayTrade = value; }
        }

        /// <summary>
        /// Minimales Marktvolumen
        /// </summary>
        public long LoVolume
        {
            get { return _lowVolume; }
            set { _lowVolume = value; }
        }

        /// <summary>
        /// Maximales Marktvolumen
        /// </summary>
        public long HiVolume
        {
            get { return _highVolume; }
            set { _highVolume = value; }
        }

        /// <summary>
        /// Schalter, der angibt, ob es erlaubt ist schon einen abgeschlossenen
        /// Trade auf dem Markt zu besitzen
        /// </summary>
        public bool SettledAllowed
        {
            get { return _setteledAllowed; }
            set { _setteledAllowed = value; }
        }

        /// <summary>
        /// Schalter, der angibt, ob es erlaubt ist, einen noch nicht abgeschlossenen
        /// Trade auf dem Markt zu besitzen
        /// </summary>
        public bool UnsettledAllowed
        {
            get { return _unsetteledAllowed; }
            set { _unsetteledAllowed = value; }
        }

        /// <summary>
        /// Schalter der Anzeigt, dass diese Regel bereits erfolgreich ausgeführt worden ist.
        /// </summary>
        public bool AlreadyTraded
        {
            get { return _alreadyTraded; }
            set { _alreadyTraded = value; }
        }

        /// <summary>
        /// Liste von Statistiken für die Regelüberprüfung.
        /// </summary>
        public StatisticSelectionElement[] Statistics
        {
            get { return _statistics.ToArray(); }
        }

        /// <summary>
        /// Allgemeine Einstellungen. Können speziell für dies Regel geändert werden
        /// </summary>
        public TTRConfigurationRW TradeConfig
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
            }
        }

        /// <summary>
        /// Eindeutige Nummer, die die Regel identifiziert.
        /// </summary>
        public int ElementNumber
        {
            get
            {
                return _objNumber;
            }
        }


        public void addScore(TTRScores score)
        {
            bool bFound = false;

            if (_scores.Contains(score))
                return;

            for (int i = 0; i < _scores.Count; i++)
            {
                if (_scores[i] == score)
                    bFound = true;
            }
            if (!bFound)
                _scores.Add(score);
        }

        public void removeScore(TTRScores score)
        {
            if(_scores.Contains(score))
                _scores.Remove(score);
        }

        public void clearScores()
        {
            _scores.Clear();
        }

        public void addStatistic(StatisticSelectionElement element)
        {
            if (_statistics.Contains(element))
                return;

            _statistics.Add(element);
        }

        public void removeStatistic(StatisticSelectionElement element)
        {
            if (_statistics.Contains(element))
                _statistics.Remove(element);
        }

        public static TradeStarterConfigElement createNew()
        {
            return new TradeStarterConfigElement();
        }

        private TradeStarterConfigElement()
        {
            _objNumber = ++objCounter;
            _scores = new ScoreList();
            _statistics = new StatisticSelectionList();
            _config = new TTRConfigurationRW();

            _highOdds = _lowOdds = _preplayLowOdds = _preplayHighOdds = 1.01;
            _highVolume = 10000000;

            _lowGoalSum = 0;
            _highGoalSum = 0;
        }

        public override string ToString()
        {
            String msg = String.Empty;
            if (_requiredTrade != null && _requiredTrade != TRADETYPE.UNASSIGNED)
            {
                msg += "Prerequsite: ";
                msg += "An existing Trade of Type ";
                switch (_requiredTrade)
                {
                    case TRADETYPE.OVER05:
                        msg += "Over 0.5";
                        break;
                    case TRADETYPE.OVER15:
                        msg += "Over 1.5";
                        break;
                    case TRADETYPE.OVER25:
                        msg += "Over 2.5";
                        break;
                    case TRADETYPE.OVER35:
                        msg += "Over 3.5";
                        break;
                    case TRADETYPE.OVER45:
                        msg += "Over 4.5";
                        break;
                    case TRADETYPE.OVER55:
                        msg += "Over 5.5";
                        break;
                    case TRADETYPE.OVER65:
                        msg += "Over 6.5";
                        break;
                    case TRADETYPE.OVER75:
                        msg += "Over 7.5";
                        break;
                    case TRADETYPE.OVER85:
                        msg += "Over 8.5";
                        break;
                    case TRADETYPE.SCORELINE00:
                        msg += "Scoreline 0 - 0";
                        break;
                    case TRADETYPE.SCORELINE01BACK:
                        msg += "Correct Score 0 - 1 Back";
                        break;
                    case TRADETYPE.SCORELINE01LAY:
                        msg += "Correct Score 0 - 1 Lay";
                        break;
                    case TRADETYPE.SCORELINE02BACK:
                        msg += "Correct Score 0 - 2 Back";
                        break;
                    case TRADETYPE.SCORELINE02LAY:
                        msg += "Correct Score 0 - 2 Lay";
                        break;
                    case TRADETYPE.SCORELINE03BACK:
                        msg += "Correct Score 0 - 3 Back";
                        break;
                    case TRADETYPE.SCORELINE03LAY:
                        msg += "Correct Score 0 - 3 Lay";
                        break;
                    case TRADETYPE.SCORELINE10BACK:
                        msg += "Correct Score 1 - 0 Back";
                        break;
                    case TRADETYPE.SCORELINE10LAY:
                        msg += "Correct Score 1 - 0 Lay";
                        break;
                    case TRADETYPE.SCORELINE11BACK:
                        msg += "Correct Score 1 - 1 Back";
                        break;
                    case TRADETYPE.SCORELINE11LAY:
                        msg += "Correct Score 1 - 1 Lay";
                        break;
                    case TRADETYPE.SCORELINE12BACK:
                        msg += "Correct Score 1 - 2 Back";
                        break;
                    case TRADETYPE.SCORELINE12LAY:
                        msg += "Correct Score 1 - 2 Lay";
                        break;
                    case TRADETYPE.SCORELINE13BACK:
                        msg += "Correct Score 1 - 3 Back";
                        break;
                    case TRADETYPE.SCORELINE13LAY:
                        msg += "Correct Score 1 - 3 Lay";
                        break;
                    case TRADETYPE.SCORELINE20BACK:
                        msg += "Correct Score 2 - 0 Back";
                        break;
                    case TRADETYPE.SCORELINE20LAY:
                        msg += "Correct Score 2 - 0 Lay";
                        break;
                    case TRADETYPE.SCORELINE21BACK:
                        msg += "Correct Score 2 - 1 Back";
                        break;
                    case TRADETYPE.SCORELINE21LAY:
                        msg += "Correct Score 2 - 1 Lay";
                        break;
                    case TRADETYPE.SCORELINE22BACK:
                        msg += "Correct Score 2 - 2 Back";
                        break;
                    case TRADETYPE.SCORELINE22LAY:
                        msg += "Correct Score 2 - 2 Lay";
                        break;
                    case TRADETYPE.SCORELINE23BACK:
                        msg += "Correct Score 2 - 3 Back";
                        break;
                    case TRADETYPE.SCORELINE23LAY:
                        msg += "Correct Score 2 -3 Lay";
                        break;
                    case TRADETYPE.SCORELINE30BACK:
                        msg += "Correct Score 3 - 0 Back";
                        break;
                    case TRADETYPE.SCORELINE30LAY:
                        msg += "Correct Score 3 - 0 Lay";
                        break;
                    case TRADETYPE.SCORELINE31BACK:
                        msg += "Correct Score 3 - 1 Back";
                        break;
                    case TRADETYPE.SCORELINE31LAY:
                        msg += "Correct Score 3 - 1 Lay";
                        break;
                    case TRADETYPE.SCORELINE32BACK:
                        msg += "Correct Score 3 - 2 Back";
                        break;
                    case TRADETYPE.SCORELINE32LAY:
                        msg += "Correct Score 3 - 2 Lay";
                        break;
                    case TRADETYPE.SCORELINE33BACK:
                        msg += "Correct Score 3 - 3 Back";
                        break;
                    case TRADETYPE.SCORELINE33LAY:
                        msg += "Correct Score 3 - 3 Lay";
                        break;
                    case TRADETYPE.SCORELINEOTHERBACK:
                        msg += "Correct Score Other Back";
                        break;
                    case TRADETYPE.SCORELINEOTHERLAY:
                        msg += "Correct Score Other Lay";
                        break;
                    case TRADETYPE.UNASSIGNED:
                        msg += "No Trade Type Assigned";
                        break;
                }

                msg += " and in the state of ";
                switch (_requiredTradeState)
                {
                    case TRADESTATE.OPENED:
                        msg += "Opened";
                        break;
                    case TRADESTATE.HEDGED:
                        msg += "Hedged";
                        break;
                    case TRADESTATE.GREENED:
                        msg += "Greened";
                        break;
                    case TRADESTATE.NOMATTER:
                        msg += "Trade State doesn't matter";
                        break;
                    case TRADESTATE.UNASSIGNED:
                        msg += "No Trade State Assigned";
                        break;
                }

                msg += "\r\n\r\n";
            }
            if (_preplayTrade)
            {
                msg += "Preplay: ";
                msg += "Odds between " + _preplayLowOdds.ToString() + " and " + _preplayHighOdds.ToString() + ". ";
                msg += "Market Volume betwwen " + _preplayLowVolume.ToString() + " and " + _preplayHighVolume.ToString() + ".";
                msg += "\r\n\r\n";
            }
            if (_inplayTrade)
            {
                msg += "Inplay: ";
                msg += "Playtime between " + _lowPlaytime.ToString() + " and " + _highPlaytime.ToString() + ". ";
                msg += "Odds between " + _lowOdds.ToString() + " and " + _highOdds.ToString() + ". ";
                msg += "Market Volume between " + _lowVolume.ToString() + " and " + _highVolume.ToString() + ". ";
                if(_lowGoalSum != -1 && _highGoalSum != -1)
                    msg += "Goal Sum between " + _lowGoalSum.ToString() + " and " + _highGoalSum.ToString() + ". ";
                if (_redCardWatch)
                {
                    msg += "Red Card Watch: ";
                    switch (_redCardWatchState)
                    {
                        case TTRRedCardWatch.EQUALCARD:
                            msg += "Both Teams equal number of Red Cards. ";
                            break;
                        case TTRRedCardWatch.NOREDCARD:
                            msg += "No Team has Red Cards. ";
                            break;
                        case TTRRedCardWatch.TEAMAMORE:
                            msg += "Team A has more Red Cards than Team B. ";
                            break;
                        case TTRRedCardWatch.TEAMBMORE:
                            msg += "Team B has more Red Cards than Team A. ";
                            break;
                        default:
                            msg += "Red Card Option is unspecified!";
                            break;
                    }
                }
                else
                {
                    msg += "No Red Card Watch. ";
                }

                msg += "\r\n\r\n";
            }

            msg += "Common: ";
            if (_setteledAllowed)
            {
                msg += "Also Start if there is an already settled Trade. ";
            }
            else
            {
                msg += "Do not Start if there is an already settled Trade. ";
            }

            if (_unsetteledAllowed)
            {
                msg += "Also Start if there is an unsettled Trade. ";
            }
            else
            {
                msg += "Do not Start if there is an unsettled Trade. ";
            }

            if (_statistics.Count > 0)
            {
                msg += "\r\n";
                msg += " Statistics: ";
            }

            foreach (StatisticSelectionElement statistic in _statistics)
            {
                msg += statistic.ToString();
            }            

            return msg;
        }
    }
}
