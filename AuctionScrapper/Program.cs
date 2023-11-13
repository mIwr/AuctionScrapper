using AuctionScrapper.Controller;
using AuctionScrapper.Model;
using AuctionScrapper.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AuctionScrapper
{
    class Program
    {
        private const string _helpAppName = "Auction lots scrapper";
        private const string _help = _helpAppName + "\nUSAGE:\n" +
            "ascrapper {MODE}\n" +
            "MODE - auction lots scrapper work mode. Available values:" +
            "\t" + _infiniteModeKey + " - search lots info infinitely time\n" +
            "\t" + _firstEmptyModeKey + " - search lots info until empty response were got. After updates lots' images\n" +
            "\t" + _imgUpdateModeKey + " - Update existing lots' images only without searching new ones";
        private const string _infiniteModeKey = "infinite";
        private const string _firstEmptyModeKey = "firstEmpty";
        private const string _imgUpdateModeKey = "updateImages";

        private static string _mode = string.Empty;
        private static readonly ApiClient _client = new ApiClient();

        static void Main(string[] args)
        {
            if (!ProcessArgs(args))
            {
#if DEBUG
                Console.ReadKey();
#endif 
                return;
            }
            var exportController = new LotExportController();
            var lots = new Dictionary<string, Lot>();
            if (File.Exists(exportController.ExportFilename))
            {
                var lotsJson = File.ReadAllText(exportController.ExportFilename);
                var dicts = JsonSerializer.Deserialize<Dictionary<string, dynamic>[]>(lotsJson) ?? Array.Empty<Dictionary<string, dynamic>>();                
                for (var i = 0; i < dicts.Length; i++)
                {
                    var parsed = Lot.FromDict(dicts[i]);
                    if (parsed == null || lots.ContainsKey(parsed.ID))
                    {
                        continue;
                    }
                    lots.Add(parsed.ID, parsed);
                }
            }
            var lotsController = new ScrapController(initLots: lots);
            lots.Clear();
            var iteration = 1;
            switch (_mode)
            {
                case _infiniteModeKey:                    
                    while (true)
                    {
                        Console.WriteLine("Iteration #" + iteration.ToString());
                        lotsController.LoadBatch(_client);
                        exportController.WriteLots(lotsController.AllLots.Values.ToArray());
                        iteration++;
                    }
                case _firstEmptyModeKey:
                    while (true)
                    {
                        Console.Write("Iteration #" + iteration.ToString() + " -> ");
                        var countBefore = lotsController.LotsCount;
                        var items = lotsController.LoadBatch(_client);
                        Console.WriteLine(items.Length.ToString() + " lots downloaded; total - " + lotsController.LotsCount.ToString());
                        var countAfter = lotsController.LotsCount;
                        if (countAfter == countBefore)
                        {
                            break;
                        }                        
                        iteration++;
                    }
                    exportController.WriteLots(lotsController.AllLots.Values.ToArray());
                    LoadLotImages(lotsController.AllLots.Values.ToArray(), _client, exportController);
                    break;
                case _imgUpdateModeKey:
                    LoadLotImages(lotsController.AllLots.Values.ToArray(), _client, exportController);
                    break;
            }

#if DEBUG
            Console.ReadKey();
#endif
        }

        static bool ProcessArgs(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(_help);
                return false;
            }
            var operand = args[0];
            if (operand == _infiniteModeKey || operand == _firstEmptyModeKey || operand == _imgUpdateModeKey)
            {
                _mode = operand;
                return true;
            }
            Console.WriteLine("Unknown operand " + operand);
            Console.WriteLine(_help);
            return false;
        }

        static void LoadLotImages(Lot[] lots, ApiClient client, LotExportController exportController)
        {
            Console.WriteLine("Loading lots' image resources...");
            for (var i = 0; i < lots.Length; i++)
            {
                var lot = lots[i];
                Console.Write((i + 1).ToString() + '/' + lots.Length.ToString() + ": " + lot.ID + " (" + lot.Title + ") -> ");
                if (File.Exists(lot.ImgFilename))
                {
                    Console.WriteLine("Exists, skip");
                    continue;
                }
                var bytes = ApiLot.LoadLotImageByApi(client, lot.ImgFilename);
                Console.WriteLine(bytes.Length.ToString() + " bytes");
                exportController.WriteLotImg(lot, bytes);
            }
        }
    }
}
