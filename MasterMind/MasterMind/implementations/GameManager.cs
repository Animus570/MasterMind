using MasterMind.interfaces;

namespace MasterMind.implementations;

/// <inheritdoc cref="IGameManager"/>
internal class GameManager : IGameManager
{
    private const string INTRO_PROMPT = "SHALL WE PLAY A GAME? (Y/N) : ";
    private const string INPUT_PROMPT = "PLEASE ENTER FOUR NUMBERS SEPARATED BY SPACES : ";
    private const string PLAY_AGAIN_PROMPT = "WOULD YOU LIKE TO PLAY AGAIN? (Y/N) : ";
    private const string VICTORY_MESSAGE = "YOU HAVE BEATEN ME";
    private const string INSTRUCTIONS = "ENTER FOUR NON-ZERO NUMBERS SEPARATED BY A SPACE THEN PRESS ENTER TO PERFORM A GUESS\nIF YOU WISH TO QUIT TYPE \"EXIT\" INTO MY CONSOLE";
    private const string PARSING_ERROR_MESSAGE = "A PARSING ERROR HAS OCCURED PLEASE TRY TO RE-ENTER YOUR GUESS";
    private const string INVALID_NUMBER_SELECTION_MESSAGE = "INVALID NUMBER HAS BEEN SELECTED ONLY ONE THROUGH SIX ARE VALID";
    private const string INPUT_COUNT_MISMATCH = "THE NUMBER OF USER INPUTS IS INVALID PLEASE REMEMBER FOUR NUMBERS ONLY";
    private const string EXIT_MESSAGE_01 = "PERHAPS ANOTHER TIME";
    private const string EXIT_MESSAGE_02 = "THANK YOU FOR PLAYING I HOPE WE CAN PLAY AGAIN SOON!";
    private const string EXIT_CASE = "exit";

    private readonly HashSet<string> ValidResponses = new(StringComparer.OrdinalIgnoreCase) { "y", "n" };
    private readonly IGameLogic _gameLogic;
    private readonly uint _maxAttempts;

    private Dictionary<int, string> _previousAttempts = new();

    private sbyte _currentAttempts;

    internal GameManager(IGameLogic gameLogic, uint maxAttempts) : this(gameLogic) => _maxAttempts = maxAttempts;
    private GameManager(IGameLogic gameLogic) => _gameLogic = gameLogic;

    /// <inheritdoc />
    void IGameManager.Run()
    {
        var shouldGameStart = PlayPrompt(INTRO_PROMPT);
        
        if (!shouldGameStart)
        {
            Console.WriteLine(EXIT_MESSAGE_01);
            return;
        }

        PrintInstructions();

        bool keepGoing;
        do
        {
            InitializeGame();
            keepGoing = PlayRound();
            keepGoing = CheckIfWantingToPlayAgain(keepGoing);
            if (keepGoing)
            {
                ResetGame();
            }
        }
        while (keepGoing);

        Console.WriteLine(EXIT_MESSAGE_02);
    }

    #region "Internal Logic"

    private bool PlayRound()
    {
        KeyValuePair<bool, IEnumerable<int>> userInput;
        bool userHasWon = false;
        bool userWishesToContinue;
        do
        {
            DrawBoard();
            userInput = CollectUserInput();
            userWishesToContinue = userInput.Key;

            if (userWishesToContinue)
            {
                userHasWon = _gameLogic.CheckWinState(userInput.Value);
                if (!userHasWon)
                {
                    _previousAttempts.Add(_currentAttempts + 1, $"{string.Join(" ", userInput.Value)} -> {string.Join(" ", _gameLogic.ValidateUserGuess(userInput.Value))}");
                }
            }
        }
        while (DetermineIfTurnsRemain() && !userHasWon && userWishesToContinue);

        DrawBoard();

        if (!userHasWon && userWishesToContinue && !DetermineIfTurnsRemain())
        {
            PrintGameOver();
        }
        if (userHasWon)
        {
            Console.WriteLine(VICTORY_MESSAGE);
        }

        return userWishesToContinue;
    }

    private bool DetermineIfTurnsRemain() => ++_currentAttempts < _maxAttempts;

    private static void PrintInstructions() => Console.WriteLine(INSTRUCTIONS);

    private void PrintGameOver() => Console.WriteLine($"You failed to guess my numbers...{_gameLogic.ShowHiddenSequence()}{Environment.NewLine}Shall we play again? (Y/N)");

    private void DrawBoard()
    {
        Console.Clear();
        // draw previous attempts should any exist
        if (_previousAttempts.Any()) Console.WriteLine("Failed Attempts");
        foreach (var elem in _previousAttempts)
        {
            Console.WriteLine($"{elem.Key} | {elem.Value}");
            Console.WriteLine("----------------------");
        }
    }

    private bool CheckIfWantingToPlayAgain(bool userDesireToContinue)
    {
        if (!userDesireToContinue) return false;
        return PlayPrompt(PLAY_AGAIN_PROMPT);
    }

    private void InitializeGame() => _gameLogic.GenerateSequence();

    private void ResetGame()
    {
        Console.Clear();
        _currentAttempts = 0;
        _previousAttempts.Clear();
        _gameLogic.GenerateSequence();
    }

    private bool PlayPrompt(string promptMessage)
    {
        string? userInput;
        
        do
        {
            Console.Write(promptMessage);
            userInput = Console.ReadLine();
        }
        while (!ValidResponses.Contains(userInput??string.Empty) || string.Equals(EXIT_CASE, userInput??string.Empty, StringComparison.OrdinalIgnoreCase));

        return userInput.Equals("y", StringComparison.OrdinalIgnoreCase);
    }

    private KeyValuePair<bool, IEnumerable<int>> CollectUserInput()
    {
        string? userInput;
        IEnumerable<int> response = new List<int>();
        
        do
        {
            Console.WriteLine(INPUT_PROMPT);
            userInput = Console.ReadLine();

            if (userInput != null && userInput.Equals(EXIT_CASE, StringComparison.OrdinalIgnoreCase))
            {
                return new KeyValuePair<bool, IEnumerable<int>>(false, Array.Empty<int>());
            }

        } while (userInput != null && !UserGuessInputValidation(userInput, out response));

        return new KeyValuePair<bool, IEnumerable<int>>(true, response);
    }

    private bool UserGuessInputValidation(string input, out IEnumerable<int> parsedResponse)
    {
        try
        {
            parsedResponse = input.Trim().Split(" ").Select(elem => int.Parse(elem));
            if(parsedResponse.Any(elem => elem < 1 || elem > 6))
            {
                Console.WriteLine(INVALID_NUMBER_SELECTION_MESSAGE);
                return false;
            }
            if (parsedResponse.Count() != 4)
            {
                Console.WriteLine(INPUT_COUNT_MISMATCH);
                return false;
            }
            return true;
        }
        catch (Exception)
        { /*really just need to swallow any exceptions here for smooth operations*/ }

        Console.WriteLine(PARSING_ERROR_MESSAGE);
        parsedResponse = Array.Empty<int>();
        return false;
    }
    #endregion
}
