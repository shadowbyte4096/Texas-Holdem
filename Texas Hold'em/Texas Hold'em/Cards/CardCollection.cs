using Project.Poker.Players.PlayerViews;
using System.Collections;

namespace Project.Cards;
public class CardCollection : IEnumerable<Card>, ICardCollectionPlayerView
{
    public CardCollection(int max) => Max = max;

    private readonly List<Card> _cards = [];

    public int Max;

    public bool IsFull => Count >= Max;

    public Card this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Max);
            return _cards[index];
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Max);
            _cards[index] = value;
        }
    }

    public bool TryAdd(Card item)
    {
        if (IsFull)
        {
            return false;
        }
        _cards.Add(item);
        return true;
    }

    public int Count => _cards.Count;

    IReadOnlyList<ICardPlayerView> ICardCollectionPlayerView.Cards => _cards.Cast<ICardPlayerView>().ToList().AsReadOnly();
    public IEnumerator<Card> GetEnumerator() => _cards.GetEnumerator();
    public int IndexOf(Card item) => _cards.IndexOf(item);
    public bool Remove(Card item) => _cards.Remove(item);
    public void RemoveAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Max);
        _cards.RemoveAt(index);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => string.Join(", ", _cards);
}
