using LibreCards.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibreCards.Core.Persistence
{
    public interface IDataStorage
    {
        IEnumerable<Card> DefaultCards { get; }
        IEnumerable<Template> DefaultTemplates { get; }

        Task AddFromUrl(string url);
    }
}
