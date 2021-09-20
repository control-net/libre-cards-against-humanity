using LibreCards.Core;
using LibreCards.Core.Entities;
using Xunit;

namespace LibreCards.Tests.Gameplay;

public class JudgePickerTests
{
    private readonly IJudgePicker _judgePicker;

    public JudgePickerTests()
    {
        _judgePicker = new JudgePicker();
    }

    [Fact]
    public void CurrentJudgeId_ShouldStartEmpty()
    {
        Assert.Equal(Guid.Empty, _judgePicker.CurrentJudgeId);
    }

    [Fact]
    public void PickNewJudge_Null_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _judgePicker.PickNewJudge(null));
    }

    [Fact]
    public void PickNewJudge_Empty_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _judgePicker.PickNewJudge(Array.Empty<Player>()));
    }

    [Fact]
    public void PickNewJudge_SinglePlayer_ShouldPickPlayer()
    {
        var player = new Player(Guid.NewGuid());

        _judgePicker.PickNewJudge(new[] { player });

        Assert.Equal(player.Id, _judgePicker.CurrentJudgeId);
    }

    [Fact]
    public void PickNewJudge_ShouldPickLastToFirst()
    {
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        var players = new[]
        {
            new Player(firstId),
            new Player(secondId)
        };

        _judgePicker.PickNewJudge(players);
        Assert.Equal(secondId, _judgePicker.CurrentJudgeId);

        _judgePicker.PickNewJudge(players);
        Assert.Equal(firstId, _judgePicker.CurrentJudgeId);
    }

    [Fact]
    public void PickNewJudge_ShouldPickupNewPlayer()
    {
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        var players = new[]
        {
            new Player(firstId),
            new Player(secondId)
        };

        _judgePicker.PickNewJudge(players);
        Assert.Equal(secondId, _judgePicker.CurrentJudgeId);

        _judgePicker.PickNewJudge(players);
        Assert.Equal(firstId, _judgePicker.CurrentJudgeId);

        var thirdId = Guid.NewGuid();
        players = new[]
        {
            new Player(firstId),
            new Player(secondId),
            new Player(thirdId)
        };

        _judgePicker.PickNewJudge(players);
        Assert.Equal(thirdId, _judgePicker.CurrentJudgeId);

        _judgePicker.PickNewJudge(players);
        Assert.Equal(secondId, _judgePicker.CurrentJudgeId);
    }

    [Fact]
    public void PickNewJudge_ShouldHandlePlayersLeaving()
    {
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var thirdId = Guid.NewGuid();

        var players = new[]
        {
            new Player(firstId),
            new Player(secondId),
            new Player(thirdId)
        };

        _judgePicker.PickNewJudge(players);
        Assert.Equal(thirdId, _judgePicker.CurrentJudgeId);

        players = new[]
        {
            new Player(firstId),
            new Player(thirdId)
        };

        _judgePicker.PickNewJudge(players);
        Assert.Equal(firstId, _judgePicker.CurrentJudgeId);
    }
}
