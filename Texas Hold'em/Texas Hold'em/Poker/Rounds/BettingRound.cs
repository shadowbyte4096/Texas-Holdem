using Project.Poker.Players;
using Project.Poker.Players.PlayerViews;
using Project.Poker.Turn;
using System.Collections.ObjectModel;

namespace Project.Poker.Rounds;

public class BettingRound : IBettingRoundPlayerView
{
    private readonly List<IPlayer> _playersInRound;

    private readonly IHandRoundPlayerView _handView;

    private IPlayer _currentPlayer;

    private readonly Dictionary<IPlayer, int> _bets;

    private readonly List<IPlayer> PlayersAllIn = [];

    private int topBet => _bets.Max(kv => kv.Value);

    public BettingRound(HandRound hand, IPlayer startingPlayer)
    {
        _handView = hand;
        _playersInRound = [.. hand.Players];
        _bets = [];
        foreach (var player in _playersInRound)
        {
            _bets.Add(player, 0);
        }
        _currentPlayer = startingPlayer;
    }

    public (Dictionary<IPlayer, int> Bets, List<IPlayer> PlayersStillIn, List<IPlayer> AllIns, IPlayer LastPlayer) PerformBettingRound()
    {
        var PlayersLeft = RotatePlayersToStart(_currentPlayer);

        while (PlayersLeft.TryDequeue(out var currentPlayer))
        {
            _currentPlayer = currentPlayer;
            Console.WriteLine($"It is {currentPlayer.Name}s turn.\n");
            var playerTurn = currentPlayer.TakeTurn(_handView);
            switch (playerTurn.Type)
            {
                case TurnType.Fold:
                    _ = _playersInRound.Remove(currentPlayer);
                    Console.WriteLine($"{currentPlayer.Name} Folded.");
                    break;
                case TurnType.Check:
                    Console.WriteLine($"{currentPlayer.Name} Checked.");
                    break;
                case TurnType.Call:
                    _bets[currentPlayer] += playerTurn.BetAmount;
                    Console.WriteLine($"{currentPlayer.Name} Called.");
                    break;
                case TurnType.Raise:
                    _bets[currentPlayer] += playerTurn.BetAmount;
                    Console.WriteLine($"{currentPlayer.Name} Raised.");
                    PlayersLeft = RotatePlayersToStart(currentPlayer);
                    _ = PlayersLeft.Dequeue();
                    break;
                case TurnType.AllIn:
                    _bets[currentPlayer] += playerTurn.BetAmount;
                    _ = _playersInRound.Remove(currentPlayer);
                    PlayersAllIn.Add(currentPlayer);
                    Console.WriteLine($"{currentPlayer.Name} Went all in.");
                    break;
            }
            Console.Clear();
        }

        return (_bets, _playersInRound, PlayersAllIn, _currentPlayer);
    }

    private Queue<IPlayer> RotatePlayersToStart(IPlayer startPlayer)
    {
        while (startPlayer != _playersInRound[0])
        {
            _playersInRound.Add(_playersInRound[0]);
            _playersInRound.RemoveAt(0);
        }
        return new Queue<IPlayer>(_playersInRound);
    }

    IPlayerPlayerView IBettingRoundPlayerView.CurrentTurn => _currentPlayer;

    IReadOnlyList<IPlayerPlayerView> IBettingRoundPlayerView.PlayersInRound => _playersInRound;

    IReadOnlyDictionary<IPlayerPlayerView, int> IBettingRoundPlayerView.Bets => new ReadOnlyDictionary<IPlayerPlayerView, int>(_bets.ToDictionary(kvp => (IPlayerPlayerView) kvp.Key, kvp => kvp.Value));
}
