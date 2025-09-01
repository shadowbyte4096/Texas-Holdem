using Project.Poker.Players.PlayerViews;
using Project.Poker.Turn;

namespace Project.Poker.Players;

public class BotPlayer(string name, int funds) : Player(name, funds)
{
    protected override PlayerTurn ChooseTurn(IHandRoundPlayerView currentHandState, List<TurnType> possibleMoves) => throw new NotImplementedException();
}
