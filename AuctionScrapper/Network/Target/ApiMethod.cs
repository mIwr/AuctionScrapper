using System.Net.Http;

namespace AuctionScrapper.Network.Target
{
    internal static class ApiMethod
    {
        internal static HttpMethod Method(this ApiTarget target)
        {
            switch (target)
            {
                case ApiTarget.lots: return HttpMethod.Post;
                case ApiTarget.lotImg: return HttpMethod.Get;
            }
            return HttpMethod.Get;
        }
    }
}
