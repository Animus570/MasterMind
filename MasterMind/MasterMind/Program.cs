using MasterMind.implementations;
using MasterMind.interfaces;

namespace MasterMind;

public sealed class Program
{
    public static void Main(string[] args)
    {
        IGameManager gamerTime = new GameManager(new GameLogic(), 10);
        gamerTime.Run();
    }
}
