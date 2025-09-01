using Project.Poker.Players;
using Project.Poker.Players.PlayerViews;

namespace Project.Poker;

public class Pot : IPotPlayerView
{
    public IEnumerable<IPlayer> Participants = [];

    public int Total;

    int IPotPlayerView.Total => Total;

    public void Distribute(IOrderedEnumerable<IPlayer> players)
    {
        foreach (var player in players)
        {
            if (Participants.Contains(player))
            {
                player.Funds += Total;
                Console.WriteLine($"{player.Name} was awarded {Total}");
                break;
            }
        }
    }

    public static List<Pot> FindPots(Dictionary<IPlayer, int> bets, IEnumerable<IPlayer> allIns)
    {
        var pots = new List<Pot>();
        var maxBet = bets.Values.Max();

        if (allIns.Any())
        {
            allIns = allIns.OrderBy(x => bets[x]);
            foreach (var allIn in allIns)
            {
                var allInAmount = bets[allIn];
                var participants = bets.Where(x => x.Value >= allInAmount).Select(x => x.Key);
                var folds = bets.Where(bet => bet.Value < allInAmount).Sum(bet => bet.Value);
                var noneFolds = allInAmount * participants.Count();
                pots.Add(new Pot { Participants = participants, Total = folds + noneFolds });
                bets = bets.ToDictionary(player => player.Key, player => Math.Max(0, player.Value - allInAmount));
            }
        }

        var mainPotParticipants = bets.Where(x => x.Value == maxBet).Select(x => x.Key);
        pots.Add(new Pot { Participants = mainPotParticipants, Total = bets.Values.Sum() });
        return pots;
    }
}
