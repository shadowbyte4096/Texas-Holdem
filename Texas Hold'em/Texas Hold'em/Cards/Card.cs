using Project.Poker.Players.PlayerViews;

namespace Project.Cards;

public struct Card : IComparable<Card>, ICardPlayerView
{
    public Ranks Rank { get; set; }
    public Suits Suit { get; set; }

    public readonly Colours Colour => Suit switch
    {
        Suits.Clubs => Colours.Black,
        Suits.Diamonds => Colours.Red,
        Suits.Hearts => Colours.Red,
        Suits.Spades => Colours.Black,
        _ => Colours.Black,
    };

    public static IEnumerable<Card> AllTypes()
    {
        var suits = (IEnumerable<Suits>) Enum.GetValues(typeof(Suits));
        var ranks = (IEnumerable<Ranks>) Enum.GetValues(typeof(Ranks));
        var cards = suits.SelectMany(suit => ranks.Select(rank => new Card() { Suit = suit, Rank = rank }));
        return cards;
    }

    public readonly int CompareTo(Card other)
    {
        if (Rank > other.Rank)
        {
            return -1;
        }
        else if (Rank < other.Rank)
        {
            return 1;
        }
        if (Suit > other.Suit)
        {
            return -1;
        }
        else if (Suit < other.Suit)
        {
            return 1;
        }
        return 0;
    }

    public override readonly string ToString() => $"{Rank.Description()} of {Suit.Description()}";
}
