
namespace net.sxtrader.bftradingstrategies.SXALInterfaces
{
    public enum EXCHANGES
    {
        UNASSIGNED = 0,
        BETFAIR,
        BETDAQCOM,
        BETDAQUK,
        PINBET88
    };

    public enum LOADSTATE
    {
        UNLOADED = 1,
        LOADED = 2,
        ERROR = 3,
        UPDATEING = 4
    };

    public enum SETSTATE { UNSETTELED, SETTLING, SETTLED };

    public enum SXALSelectionIdEnum
    {
        DRAW,

        CSZEROZERO, CSZEROONE, CSZEROTWO, CSZEROTHREE,
        CSONEZERO, CSONEONE, CSONETWO, CSONETHREE,
        CSTWOZERO, CSTWOONE, CSTWOTWO, CSTWOTHREE,
        CSTHREEZERO, CSTHREEONE, CSTHREETWO, CSTHREETHREE,
        CSOTHER,

        OVER05,
        OVER15,
        OVER25,
        OVER35,
        OVER45,
        OVER55,
        OVER65,
        OVER75,
        OVER85,     
   
        UNKNOWN,
    };

    public enum SXALBetStatusEnum
    {

        /// <remarks> Unmatched </remarks>
        U,

        /// <remarks>Matched</remarks>
        M,

        /// <remarks>Settled</remarks> 
        S,

        /// <remarks>Canceled</remarks>
        C,

        /// <remarks>Voided</remarks>
        V,

        /// <remarks>Lapsed</remarks>
        L,

        /// <remarks>Partly Matched</remarks>
        MU,

        /// <remarks>Suspended</remarks>
        P,
    }

    public enum SXALBetTypeEnum
    {

        /// <remarks>Back</remarks>
        B,

        /// <remarks>Lay</remarks>
        L,
    }

    public enum SXALMarketStatusEnum
    {

        /// <remarks>Active</remarks>
        ACTIVE,

        /// <remarks>Inactive</remarks>
        INACTIVE,

        /// <remarks>Closed</remarks>
        CLOSED,

        /// <remarks>Suspended</remarks>
        SUSPENDED,

        /// <summary>
        /// completed
        /// </summary>
        COMPLETED,

        /// <summary>
        /// settled
        /// </summary>
        SETTLED,
    }

    public enum SXALMarketTypeEnum
    {

        /// <remarks/>
        O,

        /// <remarks/>
        L,

        /// <remarks/>
        R,

        /// <remarks/>
        A,

        /// <remarks/>
        NOT_APPLICABLE,
    }
}