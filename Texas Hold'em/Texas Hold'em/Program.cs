using Project.Poker.Players;
using Project.Poker.Rounds;

public class Program
{
    public static void Main(string[] args)
    {
        List<IPlayer> players =
        [
            new HumanPlayer("Player 1", 1000),
            new HumanPlayer("Player 2", 1000),
        ];

        var hand = new HandRound(players);
        hand.PerformHand();
    }
}
