using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoUpdate.Core
{
    static class Utils
    {

        public static async Task<JsonElement?> HttpGet(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
                    if(response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return JsonDocument.Parse(result)?.RootElement;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static async Task<bool> DownloadFile(string url, string filePath)
        {
            try
            {
                using (var httpClient = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    var response = await httpClient.SendAsync(request);
                    if(!response.IsSuccessStatusCode)
                    {
                        return false;
                    }
                    using (Stream contentStream = await (response.Content.ReadAsStreamAsync()),
                        stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static JsonElement? ArrayFirst(this JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Array && jsonElement.GetArrayLength() > 0)
            {
                return jsonElement[0];
            }
            return null;
        }
    }
}
