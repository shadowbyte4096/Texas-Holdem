using Project.Poker.Players.PlayerViews;
using Project.Poker.Turn;
using System.Diagnostics.CodeAnalysis;

namespace Project.Poker.Players;

public class HumanPlayer(string name, int funds) : Player(name, funds)
{
    public const string RaiseText =
        "Input amount to raise by or type 'back' to change turn type.";


    protected override PlayerTurn ChooseTurn(IHandRoundPlayerView currentHandState, List<TurnType> possibleMoves)
    {
        Console.WriteLine(currentHandState.Status);
        Console.WriteLine(((IPlayer) this).Status);

        var betDifference = 0;
        if (currentHandState.Round is not null)
        {
            var maxBet = currentHandState.Round.Bets.Max(bet => bet.Value);
            var currentBet = currentHandState.Round.Bets[this];
            betDifference = maxBet - currentBet;
        }


        possibleMoves = [.. possibleMoves.Order()];
        PlayerTurn turn = new();
        if (!TryGetTurnTypeInput(possibleMoves, out var turnType))
        {
            return ChooseTurn(currentHandState, possibleMoves);
        }
        turn.Type = turnType.Value;
        if (turnType == TurnType.Call)
        {
            if (currentHandState.Round != null)
            {
                turn.BetAmount = betDifference;
            }
        }
        if (turnType == TurnType.Raise)
        {
            if (!GetRaiseInput(betDifference, out turn.BetAmount))
            {
                return ChooseTurn(currentHandState, possibleMoves);
            }
        }
        if (turn.Type == TurnType.AllIn)
        {
            turn.BetAmount = Funds;
        }
        return turn;
    }

    private static bool GetRaiseInput(int betDifference, out int raiseAmount)
    {
        Console.WriteLine(RaiseText);
        var input = Console.ReadLine();

        string[] backStrings = { "b", "B", "back", "Back" };
        if (backStrings.Contains(input))
        {
            raiseAmount = 0;
            return false;
        }

        if (int.TryParse(input, out raiseAmount))
        {
            if (raiseAmount > betDifference)
            {
                return true;
            }
            Console.WriteLine($"Must be greater than {betDifference}");
        }
        else
        {
            Console.WriteLine("Must be integer");
        }
        return GetRaiseInput(betDifference, out raiseAmount);
    }

    private static bool TryGetTurnTypeInput(List<TurnType> possibleMoves, [NotNullWhen(true)] out TurnType? turnType)
    {
        Console.WriteLine("Input turn type:");
        for (var i = 0; i < possibleMoves.Count; i++)
        {
            var possibleMove = possibleMoves[i];
            Console.WriteLine($"{i + 1}. {possibleMove}");
        }

        var input = Console.ReadLine();

        for (var i = 0; i < possibleMoves.Count(); i++)
        {
            var possibleMove = possibleMoves[i];
            string[] acceptableAnswers = {
                $"{i + 1}",
                $"{i + 1}.",
                $"{possibleMove.ToString().ToLower()[0]}",
                $"{possibleMove.ToString()[0]}",
                $"{possibleMove.ToString().ToLower()}",
                $"{possibleMove}",
                $"{i + 1}." +
                $"{possibleMove}"
            };
            if (acceptableAnswers.Contains(input))
            {
                turnType = possibleMove;
                return true;
            }
        }

        turnType = null;
        return false;
    }
}
