using Seven.Core;
using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Rules;

void ClearCurrentRow()
{
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, Console.CursorTop);
}

const int NumThreads = 8;

Rule rule = Rule.Standard;
IEngine[] engines = Enumerable.Range(0, rule.NumPlayers).Select<int, IEngine>((_, i) => i switch
{
    0 => new EngineStandardA(rule, new StandardRandom()),
    _ => new EngineStandardRandom(rule, new StandardRandom())
}).ToArray();

int[] points = Enumerable.Repeat(0, rule.NumPlayers).ToArray();

const int NumGames = 1000000;

// TODO: use Parallel.For 

Dealer dealer = new(new StandardRandom());
for (int gameIndex = 0; gameIndex < NumGames; ++gameIndex)
{
    if (gameIndex % 10000 == 0)
    {
        ClearCurrentRow();
        Console.Write($"{gameIndex} / {NumGames} games");
    }

    ulong[] cards = dealer.Deal(rule.NumPlayers, rule.ContainsJoker);
    Player[] players = engines.Zip(cards, (engine, card) => new Player(rule, card, engine)).ToArray();
    Board board = new();
    Game game = new(rule, board, players);
    const int MAX_PLAY_COUNT = 1000;
    for (int playIndex = 0; playIndex <  MAX_PLAY_COUNT; ++playIndex)
    {
        bool result = game.Play();
        if (result) break;
        if (playIndex == MAX_PLAY_COUNT - 1) throw new InvalidOperationException("Invalid game");
    }
    for (int playerIndex = 0; playerIndex < players.Length; ++playerIndex)
    {
        points[playerIndex] += players[playerIndex].Rank switch
        {
            0 => 4,
            1 => 2,
            2 => 1,
            _ => 0
        };
    }
}

ClearCurrentRow();
Console.WriteLine($"Results of {NumGames} games.");
for (int i = 0; i < points.Length; ++i)
{
    Console.WriteLine($"Player {i}: {points[i]} pts. ({engines[i].GetType().Name})");
}


