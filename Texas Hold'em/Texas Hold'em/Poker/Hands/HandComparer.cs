using Project.Cards;

namespace Project.Poker.Hands;
public class HandComparer : IComparer<IEnumerable<Card>>
{
    int IComparer<IEnumerable<Card>>.Compare(IEnumerable<Card>? x, IEnumerable<Card>? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }
        if (x == null)
        {
            return -1;
        }
        if (y == null)
        {
            return 1;
        }
        var handRankCompared = FindHandRank(x).CompareTo(FindHandRank(y));
        if (handRankCompared != 0)
        {
            return handRankCompared;
        }

        foreach (var (xCard, yCard) in x.OrderDescending().Zip(y.OrderDescending(), (xCard, yCard) => (xCard, yCard)))
        {
            var cardCompared = xCard.CompareTo(yCard);
            if (cardCompared != 0)
            {
                return cardCompared;
            }
        }
        return 0;
    }

    public static HandRank FindHandRank(IEnumerable<Card> hand) => _evaluators.Where(eval => eval.Value(hand)).Select(eval => eval.Key).Max();

    private static readonly Dictionary<HandRank, Func<IEnumerable<Card>, bool>> _evaluators = new()
    {
        { HandRank.RoyalFlush, IsRoyalFlush },
        { HandRank.StraightFlush, IsStraightFlush },
        { HandRank.FourOfAKind, IsFourOfAKind },
        { HandRank.FullHouse, IsFullHouse },
        { HandRank.Flush, IsFlush },
        { HandRank.Straight, IsStraight },
        { HandRank.ThreeOfAKind, IsThreeOfAKind },
        { HandRank.TwoPair, IsTwoPair },
        { HandRank.OnePair, IsOnePair },
        { HandRank.HighCard, IsHighCard },
    };

    private static bool IsRoyalFlush(IEnumerable<Card> hand)
    {
        Ranks[] royalFlushRanks = [Ranks.Ace, Ranks.King, Ranks.Queen, Ranks.Jack, Ranks.Ten];
        var royalFlushRanksInHand = hand.Where(card => royalFlushRanks.Contains(card.Rank)).ToArray();
        return IsStraightFlush(royalFlushRanksInHand);
    }

    private static bool IsStraightFlush(IEnumerable<Card> hand) => IsStraight(hand) && IsFlush(hand);

    private static bool IsFourOfAKind(IEnumerable<Card> hand) => hand.GroupBy(c => c.Rank).Any(g => g.Count() == 4);

    private static bool IsFullHouse(IEnumerable<Card> hand) => IsThreeOfAKind(hand) && IsOnePair(hand);

    private static bool IsFlush(IEnumerable<Card> hand) => hand.GroupBy(c => c.Suit).Any(g => g.Count() == 5);

    private static bool IsStraight(IEnumerable<Card> hand)
    {
        var values = hand.Select(c => (int) c.Rank).Order().Distinct().ToArray();
        for (var i = 0; i <= values.Length - 5; i++)
        {
            if (values[i + 4] - values[i] == 4)
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsThreeOfAKind(IEnumerable<Card> hand) => hand.GroupBy(c => c.Rank).Any(g => g.Count() == 3);

    private static bool IsTwoPair(IEnumerable<Card> hand) => hand.GroupBy(c => c.Rank).Where(g => g.Count() == 2).Count() == 2;

    private static bool IsOnePair(IEnumerable<Card> hand) => hand.GroupBy(c => c.Rank).Any(g => g.Count() == 2);

    private static bool IsHighCard(IEnumerable<Card> hand) => true;
}
