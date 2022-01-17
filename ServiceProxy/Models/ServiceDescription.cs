using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServiceProxy.Models
{
    public class ServiceDescription
    {
        [JsonProperty("name")] public string ServiceName { get; set; }

        [JsonProperty("base_url")] public string BaseUrl { get; set; }

        [JsonProperty("apis")] public IEnumerable<ApiDescription> Apis { get; set; }

    }
}