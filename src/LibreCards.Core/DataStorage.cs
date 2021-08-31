using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Reflection;
using System.Linq;

namespace LibreCards.Core
{
    public class DataStorage : IDataStorage
    {
        public IEnumerable<Card> DefaultCards { get; private set; }

        private readonly string _filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "default-cards.json");

        public DataStorage()
        {
            DefaultCards = GetDefaultCardsFromFile();
        }

        private IEnumerable<Card> GetDefaultCardsFromFile()
        {
            var bytes = GetFileBytes(_filePath);
            var jsonFileStructure = DeserializeToObjectFromJson(bytes);

            return jsonFileStructure.Cards.Select((text, id) => new Card { Id = id, Text = text });
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
