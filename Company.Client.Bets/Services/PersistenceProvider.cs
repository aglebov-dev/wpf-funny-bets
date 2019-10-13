using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Company.Client.Common.Interfaces;
using Newtonsoft.Json;

namespace Company.Client.Bets.Services
{
    class PersistenceProvider : IPersistenceProvider
    {
        string folderPath;
        public PersistenceProvider()
        {
            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BCL", "bet-processing");
            Directory.CreateDirectory(folderPath);
        }
        public Task Write<T>(string topic, Guid persistenceID, T data)
        {
            return Task.Run(() =>
            {
                var file = Path.Combine(folderPath, $"{GetFileName(topic, persistenceID)}.json");
                var json = data is string ? data as string : JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(file, json, Encoding.UTF8);
            });
        }
        public Task<string[]> Read(string topic)
        {
            return Task.Run(() => Directory.GetFiles(folderPath)
                .Select(file => new FileInfo(file))
                .Where(file => file.Name.StartsWith(TransformTopic(topic), true, CultureInfo.InvariantCulture))
                .Select(file => File.ReadAllText(file.FullName, Encoding.UTF8))
                .ToArray()
            );
        }
        public Task Delete(string topic, Guid persistenceID)
        {
            return Task.Run(() => {
                var path = GetFileName(topic, persistenceID);
                if (File.Exists(path))
                    File.Delete(path);
            });
        }

        private string GetFileName(string topic, Guid persistenceID) 
            => Path.Combine(folderPath, $"{TransformTopic(topic)}-{persistenceID}.json");
        private string TransformTopic(string topic) 
            => Regex.Replace(topic, "[<>:\"\\/|?* ]", string.Empty);
    }
}
