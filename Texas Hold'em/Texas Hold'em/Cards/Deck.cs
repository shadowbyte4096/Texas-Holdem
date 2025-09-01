namespace Project.Cards;

public class Deck
{
    private readonly Queue<Card> _cards;

    private Deck(IEnumerable<Card> cards) => _cards = new(cards);

    public int CardsLeft => _cards.Count;

    public Card NextCard() => _cards.Dequeue();

    public bool TryGetNextCard(out Card card) => _cards.TryDequeue(out card);

    public bool TryAddNextCard(CardCollection collection)
    {
        if (collection.IsFull)
        {
            return false;
        }
        if (TryGetNextCard(out var newCard) && collection.TryAdd(newCard))
        {
            return true;
        }
        return false;
    }

    public bool DealToCollection(CardCollection collection, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            if (collection.IsFull)
            {
                return false;
            }
            if (!TryAddNextCard(collection))
            {
                return false;
            }
        }
        return true;
    }

    public bool DealToNewCollection(int max, out CardCollection collection)
    {
        collection = new CardCollection(max);
        return DealToCollection(collection, max);
    }

    public static Deck SortedDeck()
    {
        List<Card> cards = [.. Card.AllTypes()];
        cards.Sort();
        return new Deck(cards);
    }

    public static Deck RandomDeck()
    {
        var cards = Card.AllTypes();
        //no IEnumberable.Shuffle yet :(
        Random rng = new();
        List<Card> shuffledCards = [.. cards.OrderBy(_ => rng.Next())];
        return new Deck(shuffledCards);
    }
}
