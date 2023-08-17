using MasterMind.interfaces;

namespace MasterMind.implementations;

/// <inheritdoc cref="IGameLogic"/>
internal sealed class GameLogic : IGameLogic
{
    private const int SEQUENCE_LENGTH = 4;
    private readonly Random _numberGen = new Random();
    private int[] _currentBoard;

    // based on game rules a count of 4 with only + is the win condition so any - invalidates the win
    /// <inheritdoc />
    bool IGameLogic.CheckWinState(IEnumerable<int> guess)
    {
        var validationResult = InternalValidator(guess);
        return validationResult.Count() == SEQUENCE_LENGTH && !validationResult.Any(elem => elem == '-');
    }

    /// <inheritdoc />
    void IGameLogic.GenerateSequence()
    {
        _currentBoard = new int[SEQUENCE_LENGTH];
        for (var i = 0; i < _currentBoard.Length; i++)
        {
            //converting to byte to save space memory will always be in acceptable ranges
            _currentBoard[i] = (byte)_numberGen.Next(1, 7);
        }
    }

    /// <inheritdoc />
    string IGameLogic.ShowHiddenSequence() => $"{string.Join(", " , _currentBoard)}";

    /// <inheritdoc />
    IEnumerable<char> IGameLogic.ValidateUserGuess(IEnumerable<int> guess) => InternalValidator(guess);

    private IEnumerable<char> InternalValidator(IEnumerable<int> guess)
    {
        var tmp = new List<char>(SEQUENCE_LENGTH);

        var currPos = 0;

        foreach (var elem in guess)
        {
            if (_currentBoard.Contains(elem))
            {
                tmp.Add(_currentBoard[currPos] == elem ? '+' : '-');
            }
            currPos++;
        }

        return tmp.OrderBy(elem => elem == '-');
    }
}
