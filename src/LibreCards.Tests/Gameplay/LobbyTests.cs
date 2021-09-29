using LibreCards.Core;
using LibreCards.Core.Entities;
using Moq;
using Xunit;

namespace LibreCards.Tests.Gameplay;

public class LobbyTests
{
    private readonly Mock<IGameStatus> _gameStatusMock;
    private readonly ILobby _lobby;

    public LobbyTests()
    {
        _gameStatusMock = new Mock<IGameStatus>();
        _lobby = new Lobby(_gameStatusMock.Object);
    }

    [Fact]
    public void MinimumPlayerCount_ShouldDefaultToThree()
    {
        Assert.Equal(3, _lobby.MinimumPlayerCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MinimumPlayerCount_ToZeroOrLower_ShouldThrow(int count)
    {
        Assert.Throws<ArgumentException>(() => _lobby.SetMinimumPlayerCount(count));
    }

    [Fact]
    public void MinimumPlayerCount_OverMaximum_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _lobby.SetMinimumPlayerCount(100));
    }

    [Fact]
    public void MinimumPlayerCount_ShouldSetCount()
    {
        var expected = 7;
        _lobby.SetMinimumPlayerCount(expected);

        Assert.Equal(expected, _lobby.MinimumPlayerCount);
    }

    [Fact]
    public void MaximumPlayerCount_ShouldDefaultToTen()
    {
        Assert.Equal(10, _lobby.MaximumPlayerCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MaximumPlayerCount_ToZeroOrLower_ShouldThrow(int count)
    {
        Assert.Throws<ArgumentException>(() => _lobby.SetMaximumPlayerCount(count));
    }

    [Fact]
    public void MaximumPlayerCount_UnderPlayerCount_ShouldThrow()
    {
        var playerCount = 4;

        for (var i = 0; i < playerCount; i++)
            _lobby.AddPlayer(new Player(Guid.NewGuid()));

        Assert.Throws<ArgumentException>(() => _lobby.SetMaximumPlayerCount(playerCount - 1));
    }

    [Fact]
    public void MaximumPlayerCount_UnderMinimum_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _lobby.SetMaximumPlayerCount(1));
    }

    [Fact]
    public void MaximumPlayerCount_ShouldSetCount()
    {
        const int expected = 20;

        _lobby.SetMaximumPlayerCount(expected);

        Assert.Equal(expected, _lobby.MaximumPlayerCount);
    }

    [Fact]
    public void HasEnoughPlayers_ShouldBeFalseByDefault()
    {
        Assert.False(_lobby.HasEnoughPlayers);
    }

    [Fact]
    public void HasEnoughPlayer_ShouldReturnCorrectResult()
    {
        for (var i = 0; i < _lobby.MinimumPlayerCount; i++)
            _lobby.AddPlayer(new Player(Guid.NewGuid()));

        Assert.True(_lobby.HasEnoughPlayers);
    }

    [Fact]
    public void Players_ShouldStartEmpty()
    {
        Assert.Equal(0, _lobby.Players.Count);
    }

    [Fact]
    public void AddPlayer_NullShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _lobby.AddPlayer(null));
    }

    [Fact]
    public void AddPlayer_ShouldAddPlayer()
    {
        var expectedId = Guid.NewGuid();

        _lobby.AddPlayer(new Player(expectedId));

        Assert.Single(_lobby.Players);
        Assert.Equal(expectedId, _lobby.Players.First().Id);
    }

    [Theory]
    [InlineData(GameState.Playing)]
    [InlineData(GameState.Judging)]
    public void AddPlayer_WhileGameIsInProgress_ShouldThrow(GameState state)
    {
        _gameStatusMock.Setup(s => s.CurrentState).Returns(state);

        Assert.Throws<InvalidOperationException>(() => _lobby.AddPlayer(new Player(Guid.NewGuid())));
    }

    [Fact]
    public void AddPlayer_OverMaxCount_ShouldThrow()
    {
        for (var i = 0; i < _lobby.MaximumPlayerCount; i++)
            _lobby.AddPlayer(new Player(Guid.NewGuid()));

        Assert.Throws<InvalidOperationException>(() => _lobby.AddPlayer(new Player(Guid.NewGuid())));
    }

    [Fact]
    public void RemovePlayer_InvalidId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _lobby.RemovePlayer(Guid.Empty));
    }

    [Fact]
    public void RemovePlayer_ShouldRemovePlayer()
    {
        var id = Guid.NewGuid();
        _lobby.AddPlayer(new Player(id));

        _lobby.RemovePlayer(id);

        Assert.Empty(_lobby.Players);
    }

    [Fact]
    public void LobbyOwnerId_EmptyLobby_ShouldReturnDefault()
    {
        var expected = Guid.Empty;

        Assert.Equal(expected, _lobby.OwnerId);
    }

    [Fact]
    public void LobbyOwnerId_SinglePlayer_ShouldReturnTheirId()
    {
        var expected = Guid.NewGuid();
        _lobby.AddPlayer(new Player(expected));

        Assert.Equal(expected, _lobby.OwnerId);
    }

    [Fact]
    public void LobbyOwnerId_ManyPlayers_ShouldReturnFirstPlayerId()
    {
        var expected = Guid.NewGuid();
        _lobby.AddPlayer(new Player(expected));
        _lobby.AddPlayer(new Player(Guid.NewGuid()));

        Assert.Equal(expected, _lobby.OwnerId);
    }

    [Fact]
    public void LobbyOwnerId_ManyPlayersFirstLeaves_ShouldReturnSecondPlayerId()
    {
        var first = Guid.NewGuid();
        var expected = Guid.NewGuid();
        _lobby.AddPlayer(new Player(first));
        _lobby.AddPlayer(new Player(expected));
        _lobby.AddPlayer(new Player(Guid.NewGuid()));
        _lobby.RemovePlayer(first);

        Assert.Equal(expected, _lobby.OwnerId);
    }
}
