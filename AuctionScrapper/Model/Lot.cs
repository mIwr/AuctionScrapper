using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AuctionScrapper.Model
{
    //{"title":"NYMPHÉAS","artist":"Claude Monet","medium":"Oil on canvas","date":"1905","signed":true,"img":"d8go8hccrnnhdcwted1w4u","price":54010000,"index":46,"id":"owpa5er"}
    /// <summary>
    /// Represents auction lot
    /// </summary>
    public class Lot
    {
        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Artist { get; private set; }
        public string ProdYear { get; private set; }
        public long Price { get; private set; }
        public string Img { get; private set; }

        public string ImgFilename
        {
            get
            {
                return Img + ".jpg";
            }
        }

        public bool ImgLoaded
        {
            get
            {
                return File.Exists(ImgFilename);
            }
        }

        private Lot(string id, string title, string subtitle, string artist, string prodYear, long lotPrice, string imgKey)
        {
            ID = id;
            Title = title;
            Subtitle = subtitle;
            Artist = artist;
            ProdYear = prodYear;
            Price = lotPrice;
            Img = imgKey;
        }

        public Dictionary<string, dynamic> AsDict()
        {
            var dict = new Dictionary<string, dynamic>
            {
                ["id"] = ID,
                ["title"] = Title,
                ["medium"] = Subtitle,
                ["artist"] = Artist,
                ["date"] = ProdYear,
                ["price"] = Price,
                ["img"] = Img
            };
            return dict;
        }

        public static Lot? FromDict(Dictionary<string, dynamic> dict)
        {
            if (!dict.ContainsKey("id") || !dict.ContainsKey("title") || !dict.ContainsKey("medium") || !dict.ContainsKey("artist") || !dict.ContainsKey("date") || !dict.ContainsKey("price") || !dict.ContainsKey("img"))
            {
                return null;
            }
            JsonElement jsonEl = dict["id"];
            var id = jsonEl.Deserialize<string>() ?? string.Empty;
            jsonEl = dict["title"];
            var title = jsonEl.Deserialize<string>() ?? string.Empty;
            jsonEl = dict["medium"];
            var subtitle = jsonEl.Deserialize<string>() ?? string.Empty;
            jsonEl = dict["artist"];
            var artist = jsonEl.Deserialize<string>() ?? string.Empty;
            jsonEl = dict["date"];
            var prodYear = jsonEl.Deserialize<string>() ?? string.Empty;
            jsonEl = dict["price"];
            long price = 0;
            if (!jsonEl.TryGetInt64(out price)) {
                jsonEl.TryGetDouble(out var doublePrice);
                price = Convert.ToInt64(doublePrice);
            }
            jsonEl = dict["img"];
            string imgKey = jsonEl.Deserialize<string>() ?? string.Empty;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(imgKey) || price <= 0)
            {
                return null;
            }
            return new Lot(id, title, subtitle, artist, prodYear, price, imgKey);
        }
    }
}
