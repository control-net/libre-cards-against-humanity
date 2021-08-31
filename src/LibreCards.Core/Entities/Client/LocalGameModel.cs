using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core.Entities.Client
{
    public class LocalGameModel
    {
        public bool JoinedLobby { get; set; }
        
        public Guid MyId { get; set; }
        
        public bool GameInProgress { get; set; }

        public ICollection<Guid> LobbyPlayers { get; set; } = new List<Guid>();

        public Guid JudgeId {  get; set; }

        public bool IsJudging => MyId == JudgeId;

        public string CurrentTemplate { get; set; }

        public int CurrentTemplateSlots { get; set; }

        public IEnumerable<string> Cards { get; set; } = new List<string>();

        public void OnPlayerJoined(Guid id)
        {
            LobbyPlayers.Add(id);
            Rerender?.Invoke();
        }

        public void OnPlayerLeft(Guid id)
        {
            LobbyPlayers.Remove(id);
            Rerender?.Invoke();
        }

        public void OnPlayerList(IEnumerable<Guid> ids)
        {
            LobbyPlayers = ids.ToList();
            Rerender?.Invoke();
        }

        public void OnUpdateTemplate(string template, int numOfSlots)
        {
            CurrentTemplate = template;
            CurrentTemplateSlots = numOfSlots;
            Rerender?.Invoke();
        }

        public void OnGameStarted(GameModel game)
        {
            JudgeId = game.JudgeId;
            GameInProgress = true;
            Rerender?.Invoke();
        }

        public void OnUpdateCards(IEnumerable<string> cards)
        {
            Cards = cards;
            Rerender?.Invoke();
        }

        public void OnIdAssigned(Guid id)
        {
            MyId = id;
            JoinedLobby = true;
            Rerender?.Invoke();
        }

        public Action Rerender { private get; set; }
    }
}
