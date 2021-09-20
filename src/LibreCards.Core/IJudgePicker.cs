using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;

namespace LibreCards.Core
{
    public interface IJudgePicker
    {
        Guid CurrentJudgeId { get; }

        void PickNewJudge(IReadOnlyCollection<Player> players);
    }
}
