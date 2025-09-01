using Project.Cards;
using Project.Poker.Hands;
using Project.Poker.Players;
using Project.Poker.Players.PlayerViews;

namespace Project.Poker.Rounds;

public class HandRound : IHandRoundPlayerView
{
    public HandRound(List<IPlayer> players) => Players = players;

    private CardCollection _river = new(5);

    public List<IPlayer> Players;

    private BettingRound? _bettingRound;

    private readonly List<Pot> _pots = [];

    public void PerformHand()
    {
        _river = new(5);
        _bettingRound = null;
        var startingPlayer = 0;
        var deck = Deck.RandomDeck();

        DealToPlayers(deck);

        void NextBettingRound(int amountToFillRiver)
        {
            _ = deck.DealToCollection(_river, amountToFillRiver);
            _bettingRound = new(this, Players[startingPlayer]);
            var (RoundPot, PlayersStillIn, AllIns, LastPlayer) = _bettingRound.PerformBettingRound();
            _pots.AddRange(Pot.FindPots(RoundPot, AllIns));

            while (Players[startingPlayer] != LastPlayer)
            {
                startingPlayer = (startingPlayer + 1) % Players.Count;
            }
            do
            {
                startingPlayer = (startingPlayer + 1) % Players.Count;
            }
            while (!PlayersStillIn.Contains(Players[startingPlayer]));
            Players = PlayersStillIn;
        }

        NextBettingRound(0);
        NextBettingRound(3);
        NextBettingRound(1);
        NextBettingRound(1);

        ScoreAndDistribute();
        return;
    }

    private void DealToPlayers(Deck deck)
    {
        foreach (var player in Players)
        {
            _ = deck.DealToCollection(player.Cards, 2);
        }
    }

    private void ScoreAndDistribute()
    {
        var playersAndHands = Players.ToDictionary(player => player, player => player.Cards.Concat(_river));
        Console.WriteLine($"River: {_river}");
        foreach (var (player, hand) in playersAndHands)
        {
            Console.WriteLine($"{player.Name} - {player.Cards} - {HandComparer.FindHandRank(hand)}");
        }
        var orderedPlayers = Players.OrderByDescending(player => playersAndHands[player], new HandComparer());
        foreach (var pot in _pots)
        {
            pot.Distribute(orderedPlayers);
        }
    }

    ICardCollectionPlayerView IHandRoundPlayerView.River => _river;
    IBettingRoundPlayerView? IHandRoundPlayerView.Round => _bettingRound;
    IReadOnlyList<IPlayerPlayerView> IHandRoundPlayerView.Players => Players;
    IReadOnlyList<IPotPlayerView> IHandRoundPlayerView.Pots => _pots;
}
