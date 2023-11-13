using AuctionScrapper.Model;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AuctionScrapper.Controller
{
    public class LotExportController
    {
        public const string DefaultFilename = "alots.json";

        public string ExportFilename { get; private set; }

        public LotExportController(string exportFilename = DefaultFilename)
        {
            ExportFilename = exportFilename;
        }

        public void WriteLots(Lot[] lots)
        {
            var dicts = new Dictionary<string, dynamic>[lots.Length];
            for (var i = 0; i < lots.Length; i++)
            {
                dicts[i] = lots[i].AsDict();
            }
            var json = JsonSerializer.Serialize(dicts);
            File.WriteAllText(ExportFilename, json);
        }

        public void WriteLotImg(Lot lot, byte[] img)
        {
            File.WriteAllBytes(lot.ImgFilename, img);
        }
    }
}
