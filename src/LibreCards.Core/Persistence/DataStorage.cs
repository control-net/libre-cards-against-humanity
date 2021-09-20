using LibreCards.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace LibreCards.Core.Persistence
{
    public class DataStorage : IDataStorage
    {
        public IEnumerable<Card> DefaultCards { get; private set; }

        public IEnumerable<Template> DefaultTemplates { get; private set; }

        private readonly string _filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "default-cards.json");

        public DataStorage()
        {
            LoadDefaultCardsFromFile();
        }

        private void LoadDefaultCardsFromFile()
        {
            var bytes = GetFileBytes(_filePath);
            var jsonFileStructure = DeserializeToObjectFromJson(bytes);

            DefaultCards = jsonFileStructure.Cards.Select((text, id) => new Card { Id = id, Text = text });
            DefaultTemplates = jsonFileStructure.Templates.Select(content => new Template(content));
        }

        private JsonFileStructure DeserializeToObjectFromJson(byte[] bytes)
        {
            try
            {
                return JsonSerializer.Deserialize<JsonFileStructure>(bytes);
            }
            catch (JsonException)
            {
                throw new JsonException("Json could not be deserialized");
            }
        }

        private byte[] GetFileBytes(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            
            return File.ReadAllBytes(path);
        }
    }
}
