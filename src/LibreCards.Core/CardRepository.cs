using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class CardRepository : ICardRepository
    {
        private IDataStorage _dataStorage;

        public CardRepository(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public IEnumerable<Card> DrawCards(int count = 1)
            => Enumerable.Range(0, count).Select(_ => _dataStorage.DefaultCards.Random());
    }
}
