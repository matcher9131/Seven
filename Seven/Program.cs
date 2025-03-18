using Seven.Core;
using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Rules;

const int NumGames = 10000000;

Rule rule = Rule.Standard;
IEngine[] engines = Enumerable.Range(0, rule.NumPlayers).Select<int, IEngine>((_, i) => i switch
{
    0 => new EngineStandardA(rule, new StandardRandom()),
    _ => new EngineStandardRandom(rule, new StandardRandom())
}).ToArray();

int numInvalidGames = 0;
object lockObject = new();

int[] sumPoints = Enumerable.Range(0, NumGames).AsParallel().Select(_ =>
{
    int[] points = Enumerable.Repeat(0, rule.NumPlayers).ToArray();
    Dealer dealer = new(new StandardRandom());

    ulong[] cards = dealer.Deal(rule.NumPlayers, rule.ContainsJoker);
    Player[] players = engines.Zip(cards, (engine, card) => new Player(rule, card, engine)).ToArray();
    Board board = new();
    Game game = new(rule, board, players);
    const int MAX_PLAY_COUNT = 1000;
    for (int playIndex = 0; playIndex < MAX_PLAY_COUNT; ++playIndex)
    {
        bool result = game.Play();
        if (result) break;
        if (playIndex == MAX_PLAY_COUNT - 1)
        {
            lock(lockObject)
            {
                ++numInvalidGames;
            }
        }
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

    return points;
}).Aggregate((x, y) => x.Zip(y, (xi, yi) => xi + yi).ToArray());

Console.WriteLine($"Results of {NumGames} games.");
if (numInvalidGames > 0)
{
    Console.WriteLine($"{numInvalidGames} invalid games ({NumGames - numInvalidGames} valid games)");
}
for (int i = 0; i < sumPoints.Length; ++i)
{
    Console.WriteLine($"Player {i}: {sumPoints[i]} pts. ({engines[i].GetType().Name})");
}


