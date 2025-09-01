using Project.Cards;
using Project.Poker.Players.PlayerViews;
using Project.Poker.Turn;

namespace Project.Poker.Players;

public interface IPlayer : IPlayerPlayerView
{
    CardCollection Cards { get; set; }

    new int Funds { get; set; }

    PlayerTurn TakeTurn(IHandRoundPlayerView currentHandState);

    string Status => $"{Name}'s cards: {Cards}\n";
}
