using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceProxy.Exceptions;

namespace ServiceProxy.services
{
    /// <summary>
    /// Used to parse the api description file
    /// </summary>
    public class FileParser
    {
        private readonly IEnumerable<ServiceDescription> _descriptions;

        public FileParser(string fileContent)
        {
            _descriptions = JsonConvert.DeserializeObject<IEnumerable<ServiceDescription>>(fileContent);

            if (_descriptions == null)
                throw new Exception("Failed to read the `api-description.json` file content.");
        }

        public ServiceDescription GetDescription(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new ServiceNotFoundException($"The service '{serviceName}' is not in the api description file.");

            var result = _descriptions.FirstOrDefault(x =>
                string.Equals(x.ServiceName, serviceName, StringComparison.InvariantCultureIgnoreCase));

            if (result is null)
                throw new ServiceNotFoundException($"The service '{serviceName}' is not in the api description file.");

            return result;
        }
    }
}
