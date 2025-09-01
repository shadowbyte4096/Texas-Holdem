namespace Project.Poker.Players.PlayerViews;
public interface ICardCollectionPlayerView
{
    IReadOnlyList<ICardPlayerView> Cards { get; }
}
