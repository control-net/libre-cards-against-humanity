using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class CardRepository : ICardRepository
    {
        private IDataStorage _dataStorage;
        private int _drawnCards = 0;

        public CardRepository()
        {
            _dataStorage = new DataStorage();
        }

        public IEnumerable<Card> DrawCards(int count = 1)
        {
            if ((_drawnCards + count) > _dataStorage.DefaultCards.Count())
            {
                throw new IndexOutOfRangeException("Out of Response Cards");
            }
            var cards = _dataStorage.DefaultCards.Skip(_drawnCards).Take(count);
            _drawnCards += count;
            return cards;
        }
    }
}
