using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Random;
using Seven.Core.Rules;

namespace Seven.GA
{
    public class Evaluation(Rule rule, Func<IRandom> randomFactory)
    {
        private readonly Rule rule = rule;
        private readonly Func<IRandom> randomFactory = randomFactory;

        public double Evaluate(IEngine engine, IEngine[] oppositeEngines)
        {
            const int NumGames = 100000;

            IEngine[] engines = [engine, .. oppositeEngines];
            Dealer dealer = new();

            int numInvalidGames = 0;
            object numInvalidGamesLockObject = new();

            int sumPoint = Enumerable.Range(0, NumGames).AsParallel().Sum(_ =>
            {
                IRandom gameRandom = this.randomFactory();
                dealer.SetRandom(gameRandom);
                engine.SetRandom(gameRandom);
                foreach (var oppositeEngine in oppositeEngines)
                {
                    oppositeEngine.SetRandom(gameRandom);
                }
                
                ulong[] cards = dealer.Deal(rule.NumPlayers, rule.ContainsJoker);
                Player[] players = [.. engines.Zip(cards, (engine, card) => new Player(rule, card, engine))];
                Board board = new();
                Game game = new(rule, board, players);
                const int MAX_PLAY_COUNT = 1000;
                for (int playIndex = 0; playIndex < MAX_PLAY_COUNT; ++playIndex)
                {
                    bool result = game.Play();
                    if (result) break;
                    if (playIndex == MAX_PLAY_COUNT - 1)
                    {
                        lock (numInvalidGamesLockObject)
                        {
                            ++numInvalidGames;
                        }
                    }
                }

                return players[0].Rank switch
                {
                    0 => 4,
                    1 => 2,
                    2 => 1,
                    _ => 0
                };
            });

            return sumPoint / (7.0 * (NumGames - numInvalidGames));
        }
    }
}
