using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServiceProxy.Models
{
    public class ApiDescription
    {
        [JsonProperty("url")] public string ApiUrl { get; set; }
        [JsonProperty("http_verb")] public string HttpVerb { get; set; }
        [JsonProperty("required_fields")] public IEnumerable<string> RequiredFields { get; set; }
        [JsonProperty("response")] public JToken Response { get; set; }
    }
}