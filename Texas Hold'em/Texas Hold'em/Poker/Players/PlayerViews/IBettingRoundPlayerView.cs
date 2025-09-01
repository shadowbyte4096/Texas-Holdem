namespace Project.Poker.Players.PlayerViews;

public interface IBettingRoundPlayerView
{
    IPlayerPlayerView CurrentTurn { get; }
    IReadOnlyList<IPlayerPlayerView> PlayersInRound { get; }
    IReadOnlyDictionary<IPlayerPlayerView, int> Bets { get; }
}
