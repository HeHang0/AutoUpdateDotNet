using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
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
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("AutoUpdate.Core");
                    var response = await client.SendAsync(request);
                    if(response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return JsonDocument.Parse(result)?.RootElement;
                    }else
                    {
                        Logger.Log.LogWarning("HttpGet Status: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.LogError("HttpGet Error: " + ex.Message);
            }
            return null;
        }

        public static async Task<bool> DownloadFile(string url, string filePath, CancellationToken? token = null, IProgress<int> progress = null)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("AutoUpdate.Core");
                    using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            return false;
                        }

                        var contentLength = response.Content.Headers.ContentLength;

                        using (var downloadStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            var totalRead = 0L;
                            var buffer = new byte[8192];
                            var isMoreToRead = true;
                            int percentage = 0;
                            do
                            {
                                if (token?.IsCancellationRequested ?? false)
                                {
                                    Logger.Log.LogWarning("Http Download be Canceled");
                                    return false;
                                }
                                var read = await downloadStream.ReadAsync(buffer, 0, buffer.Length);
                                if (read == 0)
                                {
                                    isMoreToRead = false;
                                }
                                else
                                {
                                    await fileStream.WriteAsync(buffer, 0, read);

                                    totalRead += read;
                                    if (contentLength.HasValue)
                                    {
                                        int progressPercentage = (int)((double)totalRead / contentLength.Value * 100);
                                        if(progressPercentage > percentage)
                                        {
                                            percentage = progressPercentage;
                                            progress?.Report(progressPercentage);
                                        }
                                    }
                                }
                            } while (isMoreToRead);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.LogError("Http Download Error: " + ex.Message);
                return false;
            }

            return true;
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
