using System.Data.Common;
using System.Linq;
using Newtonsoft.Json;

namespace TownOfBlakulla.Core.Handlers
{
    public class JsonFileBasedPropertyRepository : IPropertyRepository
    {
        public T Load<T>(string propertyName)
        {
            if (!System.IO.Directory.Exists("props"))
            {
                return default(T);
            }

            try
            {
                var targetFile = System.IO.Path.Combine("props", $"{propertyName}.json");
                if (!System.IO.File.Exists(targetFile))
                {
                    return default(T);
                }
                var jsonData = System.IO.File.ReadAllText(targetFile);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch
            {
            }
            return default(T);
        }

        public void Save<T>(string propertyName, T value)
        {
            if (!System.IO.Directory.Exists("props"))
            {
                System.IO.Directory.CreateDirectory("props");
            }

            try
            {
                var targetFile = System.IO.Path.Combine("props", $"{propertyName}.json");
                var jsonData = JsonConvert.SerializeObject(value);
                System.IO.File.WriteAllText(targetFile, jsonData);
            }
            catch { }

        }
    }
}