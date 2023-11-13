using AuctionScrapper.Model;
using AuctionScrapper.Network.Target;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AuctionScrapper.Network
{
    internal static class ApiLot
    {
        public static Lot[] GetLotsBatchByApi(ApiClient client, HashSet<string>? seenIDs = null)
        {
            //POST https://neal.fun/api/auction-game/getItems
            //JSON FORM: {"seen":[]}
            var parameters = new Dictionary<string, dynamic>
            {
                ["seen"] = Array.Empty<string>()
            };
            if (seenIDs != null && seenIDs.Count > 0)
            {
                parameters["seen"] = seenIDs.ToArray();
            }
            var jsonStr = client.RequestJSON(ApiTarget.lots, parameters);
            var jsonArr = JsonSerializer.Deserialize<Dictionary<string, dynamic>[]>(jsonStr);
            if (jsonArr == null || jsonArr.Length == 0)
            {
                return Array.Empty<Lot>();
            }
            var lots = new List<Lot>();
            for (var i = 0; i < jsonArr.Length; i++)
            {
                var parsed = Lot.FromDict(jsonArr[i]);
                if (parsed == null)
                {
                    continue;
                }
                lots.Add(parsed);
            }
            return lots.ToArray();
        }

        public static byte[] LoadLotImageByApi(ApiClient client, string imgFilename)
        {
            //https://auction-game.neal.fun/j9srgdhc73sb255qd4ifb8.jpg
            var target = ApiTarget.lotImg;
            var uri = target.BaseUrl() + imgFilename;
            var data = client.DownloadData(uri);
            return data;
        }
    }
}
