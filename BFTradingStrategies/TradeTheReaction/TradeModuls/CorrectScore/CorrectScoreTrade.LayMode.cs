using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.Helper;

namespace net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore
{
    partial class CorrectScoreTrade
    {
        class LayMode : TradeMode
        {
            private CorrectScoreTrade _correctScoreTrade;

            public LayMode(CorrectScoreTrade correctScoreTrade)
            {
                _correctScoreTrade = correctScoreTrade;
                _correctScoreTrade._stoppable = true;
            }

            internal override void hedgeRunner()
            {
            }

            internal override void greenRunner()
            {                
            }

            internal override double getInitialStake()
            {
                return _correctScoreTrade._layBets.BetSize;
            }

            internal override double getWinnings()
            {
                return _correctScoreTrade._layBets.BetSize - _correctScoreTrade._backBets.BetSize;
            }

            internal override double getPLSnapshot()
            {
                double dMoney = 0.0;
                // Sonderfall CS Other Back
                if (_correctScoreTrade.TradeType == TRADETYPE.SCORELINEOTHERBACK)
                {
                    // Gewonnen wenn kein Zähle > 3
                    if (_correctScoreTrade.Score.ScoreA <= 3 ||
                        _correctScoreTrade.Score.ScoreB <= 3)
                    {
                        dMoney = _correctScoreTrade._layBets.BetSize - _correctScoreTrade._backBets.BetSize;
                        /*dMoney = (_correctScoreTrade._backBets.BetSize * _correctScoreTrade._backBets.BetPrice - _correctScoreTrade._backBets.BetSize) -
                            (_correctScoreTrade._layBets.BetSize * _correctScoreTrade._layBets.BetPrice - _correctScoreTrade._layBets.BetSize);*/
                    }
                    else
                    {
                        dMoney = (_correctScoreTrade._backBets.BetSize * _correctScoreTrade._backBets.BetPrice - _correctScoreTrade._backBets.BetSize) -
                            (_correctScoreTrade._layBets.BetSize * _correctScoreTrade._layBets.BetPrice - _correctScoreTrade._layBets.BetSize);
                        
                    }
                }
                else
                {
                    // Verloren, wenn ergebnis erreicht. 
                    if (_correctScoreTrade.Score.ScoreA == CSTradeTypeToScoresList.GetScoreA(_correctScoreTrade.TradeType) &&
                       _correctScoreTrade.Score.ScoreB == CSTradeTypeToScoresList.GetScoreB(_correctScoreTrade.TradeType))
                    {
                        dMoney = (_correctScoreTrade._backBets.BetSize * _correctScoreTrade._backBets.BetPrice - _correctScoreTrade._backBets.BetSize) -
                            (_correctScoreTrade._layBets.BetSize * _correctScoreTrade._layBets.BetPrice - _correctScoreTrade._layBets.BetSize);
                    }
                    else
                    {
                        dMoney = _correctScoreTrade._layBets.BetSize - _correctScoreTrade._backBets.BetSize;
                    }
                }


                dMoney = Math.Round(dMoney, 2);
                return dMoney;
            }

            internal override void cancelOpenBets()
            {
            }

            internal override TRADEMODE TradeModeEnum
            {
                get { return TRADEMODE.LAY; }
            }
        }
    }
}
