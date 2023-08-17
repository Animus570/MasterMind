namespace MasterMind.interfaces;

/// <summary>
/// Internal logic for MasterMind rules
/// </summary>
internal interface IGameLogic
{
    /// <summary>
    /// Generate a new sequence to guess
    /// </summary>
    internal void GenerateSequence();

    /// <summary>
    /// Check if the user has guessed correctly
    /// </summary>
    /// <param name="guess"></param>
    /// <returns></returns>
    internal bool CheckWinState(IEnumerable<int> guess);

    /// <summary>
    /// Get the validation back from the hidden numbers
    /// </summary>
    /// <param name="guess"></param>
    /// <returns></returns>
    internal IEnumerable<char> ValidateUserGuess(IEnumerable<int> guess);

    /// <summary>
    /// Reveal the hidden numbers at the end
    /// </summary>
    /// <returns></returns>
    internal string ShowHiddenSequence();
}
