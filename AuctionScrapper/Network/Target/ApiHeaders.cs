using System.Collections.Generic;

namespace AuctionScrapper.Network.Target
{
    internal static class ApiHeaders
    {
        internal static Dictionary<string, string> Headers(this ApiTarget target)
        {
            switch (target)
            {
                case ApiTarget.lots: return new Dictionary<string, string>();
                case ApiTarget.lotImg: return new Dictionary<string, string>();
            }
            return new Dictionary<string, string>();
        }
    }
}
