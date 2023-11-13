namespace AuctionScrapper.Network.Target
{
    internal static class ApiBaseUrl
    {
        internal static string BaseUrl(this ApiTarget target)
        {
            switch(target)
            {
                case ApiTarget.lots: return "https://neal.fun/";
                case ApiTarget.lotImg: return "https://auction-game.neal.fun/";
            }
            return string.Empty;
        }
    }
}
