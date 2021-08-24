using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LibreCards.Core
{
    class DataStorage : IDataStorage
    {
        public IEnumerable<Card> DefaultCards => GetDefaultCards();
        private readonly string _filePath = Path.Combine(
        Directory.GetParent(Environment.CurrentDirectory)
        .Parent
        .Parent
        .Parent
        .FullName,
        @"LibreCards.Core\default-cards.json");

        private IEnumerable<Card> GetDefaultCards()
        {
            var bytes = GetFileBytes(_filePath);
            var jsonFileStructure = DeserializeToObjectFromJson(bytes);
            int cardIndex = 0;
            Card[] cards = new Card[jsonFileStructure.Responses.Length + jsonFileStructure.Templates.Length];
            foreach (var response in jsonFileStructure.Responses)
            {
                cards[cardIndex] = new Card { Id = cardIndex++, Text = response };
            }
            foreach (var template in jsonFileStructure.Templates)
            {
                cards[cardIndex] = new Card { Id = cardIndex++, Text = template };
            }
            return cards;
        }

        private JsonFileStructure DeserializeToObjectFromJson(byte[] bytes)
        {
            JsonFileStructure jsonInterpretation;
            try
            {
                jsonInterpretation = JsonSerializer.Deserialize<JsonFileStructure>(bytes);
            }
            catch (JsonException)
            {
                throw new JsonException("Json could not be deserialized");
            }
            return jsonInterpretation;
        }

        private byte[] GetFileBytes(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            byte[] rawFileContent;
            try
            {
                rawFileContent = File.ReadAllBytes(path);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            return rawFileContent;
        }
    }
}
