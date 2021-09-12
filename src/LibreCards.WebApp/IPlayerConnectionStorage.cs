
using LibreCards.Core.Entities.Client;

namespace LibreCards.WebApp;
public interface IPlayerConnectionStorage
{
    void AddConnection(string connectionId, PlayerModel player);

    PlayerModel GetByConnectionId(string connectionId);

    void RemoveConnection(string connectionId);

    bool ConnectionExists(string connectionId);

    bool UsernameIsTaken(string username);

    bool PlayerIdExists(Guid id);

    string GetConnectionIdByPlayerId(Guid id);

    IReadOnlyDictionary<string, PlayerModel> Connections { get; }
}
