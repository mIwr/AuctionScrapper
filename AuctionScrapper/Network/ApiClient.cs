using AuctionScrapper.Network.Target;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AuctionScrapper.Network
{
    public class ApiClient
    {
        private HttpClient _client;

        public ApiClient()
        {
            _client = new HttpClient();
        }

        internal string RequestJSON(ApiTarget target, Dictionary<string, dynamic> parameters)
        {
            var msg = BuildRequest(target, parameters);
            var response = _client.Send(msg);
            var responseStream = response.Content.ReadAsStream();
            if (responseStream == null)
            {
                return string.Empty;
            }
            var reader = new StreamReader(responseStream);
            var jsonStr = reader.ReadToEnd();
            reader.Close();
            return jsonStr;
        }

        internal byte[] DownloadData(string reqUriStr)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, reqUriStr);
            var response = _client.Send(msg);
            var responseStream = response.Content.ReadAsStream();
            if (responseStream == null)
            {
                return Array.Empty<byte>();
            }
            var reader = new BinaryReader(responseStream);
            var data = new byte[responseStream.Length];
            var index = 0;
            int readCount;
            var buffer = new byte[4096];
            do
            {
                readCount = reader.Read(buffer, index: 0, count: buffer.Length);
                if (readCount > 0)
                {
                    Array.Copy(buffer, sourceIndex: 0, data, destinationIndex: index, length: readCount);
                    index += readCount;
                }
            } while (readCount > 0);
            reader.Close();
            return data;
        }

        private HttpRequestMessage BuildRequest(ApiTarget target, Dictionary<string, dynamic> parameters)
        {
            var uriStr = target.BaseUrl() + target.Path();
            var msg = new HttpRequestMessage(target.Method(), uriStr);
            foreach (var entry in target.Headers())
            {
                msg.Headers.Add(entry.Key, entry.Value);
            }
            if (target.Method().Method == HttpMethod.Get.Method)
            {
                var pathSuffix = string.Empty;
                foreach (var entry in parameters)
                {
                    pathSuffix += entry.Key + '=' + entry.Value + '&';
                }
                if (!string.IsNullOrEmpty(pathSuffix))
                {
                    pathSuffix = '?' + pathSuffix.Remove(pathSuffix.Length - 1);
                    var newPath = msg.RequestUri?.AbsolutePath ?? string.Empty;
                    newPath += pathSuffix;
                    msg.RequestUri = new Uri(newPath);
                }
            }
            else if (target.Method().Method == HttpMethod.Post.Method)
            {
                var json = JsonSerializer.Serialize(parameters);
                msg.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(json));

            }
            return msg;
        }
    }
}
