using System.Text;

namespace Project.Poker.Players.PlayerViews;

public interface IHandRoundPlayerView
{
    IReadOnlyList<IPlayerPlayerView> Players { get; }
    ICardCollectionPlayerView River { get; }
    IBettingRoundPlayerView? Round { get; }
    IReadOnlyList<IPotPlayerView> Pots { get; }

    string Status
    {
        get
        {
            StringBuilder status = new();
            _ = status.AppendLine($"River: {River}");
            _ = status.AppendLine();

            _ = status.AppendLine($"Pot: £{Pots.Sum(pot => pot.Total)}");
            _ = status.AppendLine();

            _ = status.AppendLine("Players:");
            foreach (var player in Players)
            {
                _ = status.AppendLine($"{player.Name}: £{player.Funds}");
            }
            _ = status.AppendLine();

            if (Round is not null)
            {
                _ = status.AppendLine("Bets:");
                foreach (var bets in Round.Bets)
                {
                    _ = status.AppendLine($"{bets.Key.Name}: £{bets.Value}");
                }
            }
            return status.ToString();
        }
    }
}
