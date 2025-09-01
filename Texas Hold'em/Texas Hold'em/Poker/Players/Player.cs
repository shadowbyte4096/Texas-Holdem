using Project.Cards;
using Project.Poker.Players.PlayerViews;
using Project.Poker.Turn;

namespace Project.Poker.Players;

public abstract class Player : IPlayer
{
    public Player(string name, int funds)
    {
        Name = name;
        Funds = funds;
    }

    public string Name { get; private set; }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public int Funds { get; set; }

    public CardCollection Cards { get; set; } = new(2);

    public PlayerTurn TakeTurn(IHandRoundPlayerView currentHandState)
    {
        var possibleMoves = FindLegalTurnTypes(currentHandState.Round);
        var playerTurn = ChooseTurn(currentHandState, possibleMoves);

        while (!IsLegalMove(playerTurn, currentHandState.Round, possibleMoves))
        {
            playerTurn = ChooseTurn(currentHandState, possibleMoves);
        }
        Funds -= playerTurn.BetAmount;
        return playerTurn;
    }

    private List<TurnType> FindLegalTurnTypes(IBettingRoundPlayerView? bettingRound)
    {
        ArgumentNullException.ThrowIfNull(bettingRound);

        List<TurnType> turnTypes = [];
        var maxBet = bettingRound.Bets.Max(bet => bet.Value);
        var currentBet = bettingRound.Bets[this];
        var betDifference = maxBet - currentBet;

        if (Funds > 0)
        {
            turnTypes.Add(TurnType.AllIn);
        }
        if (betDifference < Funds)
        {
            turnTypes.Add(TurnType.Raise);
        }
        if (betDifference == 0)
        {
            turnTypes.Add(TurnType.Check);
        }
        else
        {
            if (betDifference <= Funds)
            {
                turnTypes.Add(TurnType.Call);
            }
            if (bettingRound.Bets.Count > 1)
            {
                turnTypes.Add(TurnType.Fold);
            }
        }
        return turnTypes;
    }

    protected abstract PlayerTurn ChooseTurn(IHandRoundPlayerView currentHandState, List<TurnType> possibleMoves);


    private bool IsLegalMove(PlayerTurn playerTurn, IBettingRoundPlayerView? bettingRound, List<TurnType>? possibleMoves = null)
    {
        possibleMoves ??= FindLegalTurnTypes(bettingRound);

        if (!possibleMoves.Contains(playerTurn.Type))
        {
            return false;
        }

        ArgumentNullException.ThrowIfNull(bettingRound);

        if (playerTurn.BetAmount > Funds)
        {
            return false;
        }

        var maxBet = bettingRound.Bets.Max(bet => bet.Value);
        var currentBet = bettingRound.Bets[this];
        var betDifference = maxBet - currentBet;

        switch (playerTurn.Type)
        {
            case TurnType.Fold:
            case TurnType.Check:
                if (playerTurn.BetAmount > 0)
                {
                    return false;
                }
                break;
            case TurnType.Call:
                if (playerTurn.BetAmount != betDifference)
                {
                    return false;
                }
                break;
            case TurnType.Raise:
                if (playerTurn.BetAmount < betDifference)
                {
                    return false;
                }
                break;
            case TurnType.AllIn:
                if (playerTurn.BetAmount <= 0)
                {
                    return false;
                }
                break;
        }

        return true;
    }
}
