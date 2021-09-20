using LibreCards.Core.Entities;
using System.Collections.Generic;

namespace LibreCards.Core.Persistence
{
    public interface IDataStorage
    {
        IEnumerable<Card> DefaultCards { get; }
        IEnumerable<Template> DefaultTemplates { get; }
    }
}
