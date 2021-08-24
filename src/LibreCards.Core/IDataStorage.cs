using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibreCards.Core
{
    interface IDataStorage
    {
        IEnumerable<Card> DefaultCards { get; }
    }
}
