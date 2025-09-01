using Project.Cards;

namespace Project.Poker.Players.PlayerViews;

public interface ICardPlayerView
{
    Ranks Rank { get; }
    Suits Suit { get; }
    Colours Colour { get; }
}
