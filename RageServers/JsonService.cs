using Newtonsoft.Json;
using System.Collections.Generic;

namespace RageServers
{
    public class JsonService
    {
        public static Dictionary<string, ServerInfo> DeserializeRageServerInfos(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, ServerInfo>>(json);
        }

        public static Dictionary<TK, TV> DeserializeRageServerInfos<TK, TV>(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<TK, TV>>(json);
        }
    }
}
