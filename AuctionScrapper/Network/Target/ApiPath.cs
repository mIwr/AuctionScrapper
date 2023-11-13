namespace AuctionScrapper.Network.Target
{
    internal static class ApiPath
    {
        internal static string Path(this ApiTarget target)
        {
            switch (target)
            {
                case ApiTarget.lots: return "api/auction-game/getItems";
                case ApiTarget.lotImg: return string.Empty;
            }
            return string.Empty;
        }
    }
}
