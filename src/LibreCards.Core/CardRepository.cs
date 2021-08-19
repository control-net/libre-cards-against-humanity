using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace LibreCards.Core
{
    public class CardRepository : ICardRepository
    {
        private readonly string _filePath = Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory)
            .Parent
            .Parent
            .Parent
            .FullName,
            @"LibreCards.Core\default-cards.json");
        private int _responseCardsIndex;

        public IEnumerable<Card> DrawCards(int count = 1)
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException();
            }

            byte[] rawFileContent;
            try
            {
                rawFileContent = File.ReadAllBytes(_filePath);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            JsonFileStructure jsonInterpretation;
            try
            {
                jsonInterpretation = JsonSerializer.Deserialize<JsonFileStructure>(rawFileContent);
            }
            catch (JsonException)
            {
                throw new JsonException("Cards collection could not be deserialized");
            }

            return GetResponseCards(jsonInterpretation, count);
        }
        private IEnumerable<Card> GetResponseCards(JsonFileStructure jsonFileStructure, int count = 1)
        {
            if ((_responseCardsIndex + count) > jsonFileStructure.Responses.Count())
            {
                throw new IndexOutOfRangeException("Out of Response Cards");
            }
            for (int i = 0; i < count; i++)
            {
                var card = new Card { Id = _responseCardsIndex, Text = jsonFileStructure.Responses[_responseCardsIndex] };
                _responseCardsIndex++;
                yield return card;
            }
        }
    }
}
