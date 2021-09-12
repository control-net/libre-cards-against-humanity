
using LibreCards.Core.Entities.Client;

namespace LibreCards.WebApp;
public class InMemoryPlayerConnectionStorage : IPlayerConnectionStorage
{
    private readonly IDictionary<string, PlayerModel> _players = new Dictionary<string, PlayerModel>();

    public IReadOnlyDictionary<string, PlayerModel> Connections => _players.ToDictionary(x => x.Key, x => x.Value);

    public void AddConnection(string connectionId, PlayerModel player)
    {
        if (ConnectionExists(connectionId))
            throw new ArgumentException("Connection already added", nameof(connectionId));

        if (UsernameIsTaken(player.Username))
            throw new ArgumentException("Username already taken", nameof(player));

        _players.Add(connectionId, player);
    }

    public bool ConnectionExists(string connectionId) => _players.ContainsKey(connectionId);

    public PlayerModel GetByConnectionId(string connectionId)
    {
        if (!_players.ContainsKey(connectionId))
            return null;

        return _players[connectionId];
    }

    public string GetConnectionIdByPlayerId(Guid id) => _players.FirstOrDefault(p => p.Value.Id == id).Key;

    public bool PlayerIdExists(Guid id) => _players.Any(p => p.Value.Id == id);

    public void RemoveConnection(string connectionId) => _players.Remove(connectionId);

    public bool UsernameIsTaken(string username) => _players.Any(p => p.Value.Username.ToLower() == username.ToLower());
}
