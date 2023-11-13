using AuctionScrapper.Model;
using AuctionScrapper.Network;
using System.Collections.Generic;
using System.Linq;

namespace AuctionScrapper.Controller
{
    public class ScrapController
    {
        private Dictionary<string, Lot> _lots;
        public int LotsCount
        {
            get
            {
                return _lots.Count;
            }
        }
        public Dictionary<string, Lot> AllLots
        {
            get
            {
                return new Dictionary<string, Lot>(_lots);
            }
        }

        public ScrapController(Dictionary<string, Lot> initLots)
        {
            _lots = new Dictionary<string, Lot>(initLots);
        }

        public void Update(Lot[] updLots)
        {
            foreach (var updLot in updLots)
            {
                if (!_lots.ContainsKey(updLot.ID))
                {
                    _lots.Add(updLot.ID, updLot);
                    continue;
                }
                _lots[updLot.ID] = updLot;
            }
        }

        public Lot[] LoadBatch(ApiClient client)
        {
            var existIDs = _lots.Keys.ToHashSet();
            var lots = ApiLot.GetLotsBatchByApi(client, existIDs);
            Update(lots);
            return lots;
        }
    }
}
