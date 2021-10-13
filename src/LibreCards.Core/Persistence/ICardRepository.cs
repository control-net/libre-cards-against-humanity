using LibreCards.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibreCards.Core.Persistence
{
    public interface ICardRepository
    {
        IEnumerable<Card> DrawCards(int count = 1);
        Template DrawTemplate();
        Task AddFromUrl(string url);
    }
}
