using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class JudgePicker : IJudgePicker
    {
        private Queue<Guid> _judgeQueue = new Queue<Guid>();

        public Guid CurrentJudgeId { get; private set; }

        public void PickNewJudge(IReadOnlyCollection<Player> players)
        {
            if (players is null)
                throw new ArgumentNullException(nameof(players));

            if (players.Count == 0)
                throw new ArgumentException("Cannot pick a judge because the player collection is empty.");
            
            FillQueueIfNeeded(players);

            RemoveNonExistentPlayers(players);

            CurrentJudgeId = _judgeQueue.Dequeue();
        }

        private void FillQueueIfNeeded(IReadOnlyCollection<Player> players)
        {
            if (_judgeQueue.Count == 0)
                _judgeQueue = new Queue<Guid>(players.Reverse().Select(p => p.Id));
        }

        private void RemoveNonExistentPlayers(IReadOnlyCollection<Player> players)
        {
            _judgeQueue = new Queue<Guid>(_judgeQueue.Where(id => players.Any(p => p.Id == id)));
        }
    }
}
