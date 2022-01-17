using Newtonsoft.Json.Linq;
using ServiceProxy.Enums;
using ServiceProxy.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace ServiceProxy.services
{
    public class MockService
    {
        public ServiceDescription ServiceDescription { get; }

        public MockService(string serviceName, string fileLocation = "api-description.json")
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location);

            if (folder is null || !File.Exists(Path.Combine(folder, fileLocation)))
                throw new FileNotFoundException($"Can not open '{fileLocation}' file.");

            using var reader = new StreamReader(fileLocation);
            var parser = new FileParser(reader.ReadToEnd());
            ServiceDescription = parser.GetDescription(serviceName);
        }


        /// <summary>
        /// Gets the response from description file.
        /// </summary>
        /// <param name="url">The api url. For example /posts.</param>
        /// <param name="verb">The type of http verb. For example GET</param>
        /// <param name="inputJson">The request in form of json.</param>
        /// <returns>Returns the the api's response from description file.</returns>
        public string GetResponse(string url, HttpVerb verb, string inputJson)
        {
            var api = ServiceDescription.Apis.FirstOrDefault(x =>
                (string.Equals(x.ApiUrl, url, StringComparison.InvariantCultureIgnoreCase) ||
                    CheckUrlEquality(x.ApiUrl.ToString().ToLowerInvariant(), 
                        ServiceDescription.BaseUrl.ToLowerInvariant(), url.ToLowerInvariant()))
                && string.Equals(x.HttpVerb.ToString().ToLower(), verb.ToString().ToLower(),
                    StringComparison.InvariantCultureIgnoreCase));

            if (api is null)
                throw new ServiceNotFoundException($"No api definition found for url: {url}");

            if (api.RequiredFields != null && !api.RequiredFields.Any())
                return api.Response.ToString();

            if (!HasRequiredFields(inputJson, api.RequiredFields?.ToArray(), out var missedField))
                throw new NoRequiredFieldException($"The passed json has not required fields \"{missedField}\".", missedField);

            return api.Response?.ToString();
        }

        /// <summary>
        /// Checks whether the passed url is matched to a pattern of url or not.
        /// For example the "/some_url/1" should match to "/some_url/{integer}".
        /// If the <para>urlPattern</para> is a regular url, the equality of urls
        /// will be checked. 
        /// </summary>
        /// <param name="urlPattern">a regular url or a pattern of url.</param>
        /// <param name="baseUrl">The service url. For example https://myblog.com</param>
        /// <param name="apiUrl">A url that will be checked against pattern. For example /posts.</param>
        /// <returns>If the apiUrl matches with urlPattern, then the response will be returned from description file.</returns>
        private bool CheckUrlEquality(string urlPattern, string baseUrl, string apiUrl)
        {
            Regex wordRegex = new Regex("^{(\\w+)}$");
            Regex integerRegex = new Regex("^\\d+$");

            Uri patternUri = new Uri(new Uri(baseUrl), urlPattern);
            Uri toBeMatched = new Uri(new Uri(baseUrl), apiUrl);

            if (patternUri.Segments.Length != toBeMatched.Segments.Length)
                return false;

            for (int i = 0; i < patternUri.Segments.Length; i++)
            {
                var firstUrl = HttpUtility.UrlDecode(patternUri.Segments[i]).Replace("/", String.Empty);
                var secondUrl = toBeMatched.Segments[i].Replace("/", String.Empty);

                if (firstUrl == secondUrl) continue;

                // Can be changed to support other types like {string} or {date}
                if (!(wordRegex.Match(firstUrl).Groups[1].ToString() == "integer" &&
                      integerRegex.IsMatch(secondUrl)))
                    return false;
            }

            return true;
        }

        private static bool HasRequiredFields(string inputJson, string[] requiredFields, out string missedField)
        {
            try
            {
                var input = JObject.Parse(inputJson);

                foreach (var requiredField in requiredFields)
                {
                    var has = input.Value<JObject>()?.Properties()
                        .Any(x => x.Name.ToLower() == requiredField.ToLower());
                    if (has != null && !has.Value)
                    {
                        missedField = requiredField;
                        return false;
                    }
                }

                missedField = String.Empty;
                return true;
            }
            catch (System.Exception)
            {
                missedField = String.Empty;
                return false;
            }
        }
    }
}