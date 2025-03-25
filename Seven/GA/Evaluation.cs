using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Rules;

namespace Seven.GA
{
    public class Evaluation(Rule rule, Dealer dealer)
    {
        private readonly Rule rule = rule;
        private readonly Dealer dealer = dealer;

        public double Evaluate(IEngine engine, Func<IEngine[]> oppositeEnginesFactory)
        {
            const int NumGames = 10000000;

            IEngine[] engines = [engine, .. oppositeEnginesFactory()];

            int numInvalidGames = 0;
            object lockObject = new();

            int sumPoint = Enumerable.Range(0, NumGames).AsParallel().Sum(_ =>
            {
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
                        lock (lockObject)
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
