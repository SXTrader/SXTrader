using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.tradeinterfaces;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.Scoreline00;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.OverUnder;
using net.sxtrader.bftradingstrategies.ttr.TradeModuls.CorrectScore;
using net.sxtrader.bftradingstrategies.SXAL;
using net.sxtrader.muk.enums;
using net.sxtrader.bftradingstrategies.SXALInterfaces;

namespace net.sxtrader.bftradingstrategies.ttr.TradeStarter
{
    internal enum LOADTRADEMODE { APPEND, CREATE, UNASSIGNED }
    /// <summary>
    /// Abstrakte Basisklasse: Jedes Tradinmodul soll die Möglichkeit haben gemeldete Wetten auf ihre Existenz in
    /// einen vorhandenen Trade zu überprüfen, um ggf. den Trade anzupassen bzw. einen neuen Trade zu erstellen
    /// </summary>
    abstract class TradeLoader
    {
        public static TradeLoader getInstance(SXALBet bet)
        {
            SXALMarket bfMarket = SXALSoccerMarketManager.Instance.getMarketById(bet.MarketId,true);
            if (bfMarket != null && bfMarket.IsScoreMarket && bet.SelectionId == (int)SXALKom.Instance.getSelectionId(SXALSelectionIdEnum.CSZEROZERO, bet.MarketId))
            {
                return new Scoreline00TradeLoader();
            }
            else if (bfMarket != null && bfMarket.IsGenericOverUnder)
                return new OverUnderTradeLoader();
            else if (bfMarket != null && bfMarket.IsScoreMarket)
                return new CorrectScoreTradeLoader();
            return null;
        }
        public abstract ITrade loadTrade(SXALBet bet,TTRWatcher watcher, out LOADTRADEMODE loadMode, out LIVESCOREADDED livescoreMode);
    }
}
