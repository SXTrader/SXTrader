using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.ttr.Configuration;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using net.sxtrader.bftradingstrategies.sxhelper;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.Scoreline00;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.SXFastBet;
using net.sxtrader.bftradingstrategies.ttr.Helper;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.TradeStarter
{
    abstract class BetPlacer
    {
        const double ABSOLUTEMINIMUM = 0.1;

        protected IScore _score;
        protected long _selectionId;
        protected long _marketId;
        protected TTRWatcher _watcher;
        protected double _odd;
        protected TTRConfigurationRW _config;
        protected String _match;
        protected TRADETYPE _tradeType;
        protected TRADEMODE _tradeMode;

        public static BetPlacer getInstance(TradeStarterConfigElement element, double odd, String match, long marketId, long selectionId,
            TTRWatcher watcher, TRADEMODE tradeMode)
        {
            switch (element.TradeType)
            {
                case TRADETYPE.SCORELINE00:
                    return new Scoreline00BetPlacer(element.TradeType, odd, match, marketId, selectionId, watcher, element.TradeConfig, tradeMode);
                case TRADETYPE.OVER05:
                case TRADETYPE.OVER15:
                case TRADETYPE.OVER25:
                case TRADETYPE.OVER35:
                case TRADETYPE.OVER45:
                case TRADETYPE.OVER55:
                case TRADETYPE.OVER65:
                case TRADETYPE.OVER75:
                case TRADETYPE.OVER85:
                    return new OverUnderBetPlacer(element.TradeType,odd, match, marketId, selectionId, watcher, element.TradeConfig, tradeMode);
                case TRADETYPE.SCORELINE01BACK:
                case TRADETYPE.SCORELINE01LAY:
                case TRADETYPE.SCORELINE02BACK:
                case TRADETYPE.SCORELINE02LAY:
                case TRADETYPE.SCORELINE03BACK:
                case TRADETYPE.SCORELINE03LAY:
                case TRADETYPE.SCORELINE10BACK:
                case TRADETYPE.SCORELINE10LAY:
                case TRADETYPE.SCORELINE11BACK:
                case TRADETYPE.SCORELINE11LAY:
                case TRADETYPE.SCORELINE12BACK:
                case TRADETYPE.SCORELINE12LAY:
                case TRADETYPE.SCORELINE13BACK:
                case TRADETYPE.SCORELINE13LAY:
                case TRADETYPE.SCORELINE20BACK:
                case TRADETYPE.SCORELINE20LAY:
                case TRADETYPE.SCORELINE21BACK:
                case TRADETYPE.SCORELINE21LAY:
                case TRADETYPE.SCORELINE22BACK:
                case TRADETYPE.SCORELINE22LAY:
                case TRADETYPE.SCORELINE23BACK:
                case TRADETYPE.SCORELINE23LAY:
                case TRADETYPE.SCORELINE30BACK:
                case TRADETYPE.SCORELINE30LAY:
                case TRADETYPE.SCORELINE31BACK:
                case TRADETYPE.SCORELINE31LAY:
                case TRADETYPE.SCORELINE32BACK:
                case TRADETYPE.SCORELINE32LAY:
                case TRADETYPE.SCORELINE33BACK:
                case TRADETYPE.SCORELINE33LAY:
                case TRADETYPE.SCORELINEOTHERBACK:
                case TRADETYPE.SCORELINEOTHERLAY:
                    return new CorrectScoreBetPlacer(element.TradeType, odd, match, marketId, selectionId, watcher, element.TradeConfig, tradeMode);
            }
            return null;
        }

        public BetPlacer(TRADETYPE tradeType, double odd, String match, long marketId, long selectionId, TTRWatcher watcher, TTRConfigurationRW config, TRADEMODE tradeMode)
        {
            _odd = odd;
            _match = match;
            _marketId = marketId;
            _selectionId = selectionId;
            _watcher = watcher;
            _config = config;
            _tradeType = tradeType;
            _tradeMode = tradeMode;
        }

        protected bool placeBetTarget(out double moneyToBet)
        {
            moneyToBet = 0.0;
            log("Calculated Fast bet configuration for succeeded rule relatvie so targeed Win/Risk");
            double target = 0.0;
            if (_config.FastBetTargetFixedAmount)
            {
                target = _config.FastBetTargetFixedAmountValue;
                logBetAmount(String.Format("Targeted Risk/Win is fixed: {0} {1}", target, SXALBankrollManager.Instance.Currency));
            }
            else
            {
                target = (SXALBankrollManager.Instance.TotalBalance * (_config.FastBetTargetPercentAmountValue / 100));
                logBetAmount(String.Format("Targeted Risk/Win is calculated relative to total balance: ({0} * ({1}/100))", SXALBankrollManager.Instance.TotalBalance, _config.FastBetTargetPercentAmountValue));
                logBetAmount(String.Format("Calculated Targeted Risk/Win is {1} {1}", target, SXALBankrollManager.Instance.Currency));
            }

            
            moneyToBet = (target / (_odd - 1));
            logBetAmount(String.Format("Calculate Money To Bet = ({0} / ({1} - 1))", target, _odd));
            moneyToBet = Math.Round(moneyToBet, 2);
            logBetAmount(String.Format("Money to Bet is {0} {1}", moneyToBet, SXALBankrollManager.Instance.Currency));

            if (moneyToBet < ABSOLUTEMINIMUM)
            {
                log(String.Format("Can not place a  Bet for Match {0} as calclulated bet size is below the minimum ob {1}. Leaving!", _match, ABSOLUTEMINIMUM));
                return false;
            }

            return true;
        }

        protected bool placeBetAbsolut(out double moneyToBet)
        {
            moneyToBet = 0.0;
            log("Reading Fast bet configuration for succeeded rule");
            SXFastBetSettings settings = new SXFastBetSettings();
            //settings.CancelUnmatchedFlag = _config.FastBetUnmatchedCancel;
            settings.FixedAmountFlag = _config.FastBetFixedAmount;
            settings.FixedAmountValue = _config.FastBetFixedAmountValue;
            settings.PercentAmounValue = _config.FastBetPercentAmountValue;
            settings.TotalAmountFlag = _config.FastBetPercentTotalAmount;
            moneyToBet = SXFastBetMoneyGetter.getMoney(settings);
            logBetAmount(String.Format("Fixed Bet Amount is {0} {1}", moneyToBet, SXALBankrollManager.Instance.Currency));
            return true;
        }

        protected bool placeBetRelative(out double moneyToBet)
        {
            moneyToBet = 0.0;
            log("Calculating Money to Bet relativly to an exisiting Trade");
            SXALMarket relativeMarket = TTRHelper.GetMarketByTradeType(_config.RelativeTradeType, _match);

            if (relativeMarket == null)
            {
                log(String.Format("Can not find the Relative Trade {0} for Match {1} in Markets. Leaving!", _config.RelativeTradeType, _match));
                return false;
            }

            if (_watcher[_match] == null)
            {
                log(String.Format("Can not find the Required Trade {0} for Match {1} in existing Trades. Leaving!", _config.RelativeTradeType, _match));
                return false;
            }

            if (_watcher[_match][_config.RelativeTradeType] == null)
            {
                log(String.Format("Can not find the Required Trade {0} for Match {1} in existing Trades. Leaving!", _config.RelativeTradeType, _match));
                return false;
            }

            ITrade relativeTrade = _watcher[_match][_config.RelativeTradeType];

            switch (_config.RelativeBetType)
            {
                case RELATIVEBETTINGTYPE.STAKE:
                    moneyToBet = relativeTrade.getInitialStake() * (_config.RelativeBetSize / 100);
                    break;
                case RELATIVEBETTINGTYPE.WINNING:
                    moneyToBet = relativeTrade.getWinnings() * (((double)_config.RelativeBetSize) / 100);
                    break;
            }

            if (moneyToBet < ABSOLUTEMINIMUM)
            {
                log(String.Format("Can not place a  Bet for Match {0} as calclulated bet size is below the minimum ob {1}. Leaving!", _match, ABSOLUTEMINIMUM));
                return false;
            }

            return true;
        }

        protected void log(string message)
        {
            try
            {
                TradeLog.Instance.writeLog(_match, "TradeTheReaction", "BetPlacer", message);
            }
            catch
            { }
        }

        private void logBetAmount(string message)
        {
            try
            {
                TradeLog.Instance.writeBetAmountLog(this._match, "TradeTheReaction", "Trader", String.Format("ID {0}: {1}", message));
            }
            catch { }
        }

        
        public abstract bool placeBet();
    }
}
