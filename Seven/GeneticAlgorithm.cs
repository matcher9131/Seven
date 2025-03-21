using Seven.Core;
using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Rules;

namespace Seven
{
    public class GeneticAlgorithm(IRandom random)
    {
        private const int N = 32;
        private const int K = 16;
        private readonly int[] indices = [.. Enumerable.Range(0, N)];

        private readonly IRandom random = random;

        private void RandomizeIndices()
        {
            for (int i = this.indices.Length - 1; i >= 0; --i)
            {
                int j = random.Next(i + 1);
                (this.indices[i], this.indices[j]) = (this.indices[j], this.indices[i]);
            }
        }

        public (int[] c1, int[] c2) Cross(int[] p1, int[] p2)
        {
            this.RandomizeIndices();
            int[] fromP1 = [.. this.indices[0..K].Select(i => p1[i])];
            int[] c1 = new int[N];
            int fromP1Index = 0;
            for (int i = 0; i < c1.Length; ++i)
            {
                c1[i] = fromP1.Contains(p2[i]) ? fromP1[fromP1Index++] : p2[i];
            }

            int[] fromP2 = [.. this.indices[0..K].Select(i => p2[i])];
            int[] c2 = new int[N];
            int fromP2Index = 0;
            for (int i = 0; i < c2.Length; ++i)
            {
                c2[i] = fromP2.Contains(p1[i]) ? fromP2[fromP2Index++] : p1[i];
            }

            return (c1, c2);
        }

        public static double Evaluate(Rule rule, Dealer dealer, Func<IEngine> engineFactory, Func<IEngine[]> oppositeEnginesFactory)
        {
            const int NumGames = 10000000;

            IEngine[] engines = [engineFactory(), .. oppositeEnginesFactory()];

            int numInvalidGames = 0;
            object lockObject = new();

            int sumPoint = Enumerable.Range(0, NumGames).AsParallel().Select(_ =>
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
            }).Sum();

            return sumPoint / (7.0 * (NumGames - numInvalidGames));
        }
    }
}
