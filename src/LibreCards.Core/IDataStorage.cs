using LibreCards.Core.Entities;
using System.Collections.Generic;

namespace LibreCards.Core
{
    public interface IDataStorage
    {
        IEnumerable<Card> DefaultCards { get; }
    }
}
