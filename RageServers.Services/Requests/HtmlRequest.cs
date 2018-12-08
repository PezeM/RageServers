using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RageServers.Services.Requests
{
    public class HtmlRequest
    {
        private static HttpClient _client = new HttpClient();

        public async Task<Dictionary<string, ServerInfo>> GetServersAsync()
        {
            try
            {
                var response = await _client.GetStringAsync("https://cdn.rage.mp/master/");
                return JsonService.DeserializeRageServerInfos<string, ServerInfo>(response);
            }
            catch (HttpRequestException e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
