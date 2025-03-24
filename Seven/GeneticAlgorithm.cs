using Seven.Core;
using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Rules;
using static Seven.Helper;

namespace Seven
{
    public class GeneticAlgorithm(IRandom random)
    {
        private const int N = 32;
        private const int K = 16;
        private readonly int[] indices = [.. Enumerable.Range(0, N)];

        const int MutationPercent = 10;

        private readonly IRandom random = random;

        private void RandomizeIndices()
        {
            // 全体をランダムにシャッフルしてからOrderBasedCrossoverのために先頭K個を昇順にソートする
            for (int i = this.indices.Length - 1; i > 0; --i)
            {
                int j = random.Next(i + 1);
                (this.indices[i], this.indices[j]) = (this.indices[j], this.indices[i]);
            }
            Array.Sort(this.indices, 0, K);
        }

        public (int[] c1, int[] c2) Cross(int[] p1, int[] p2)
        {
            this.RandomizeIndices();
            int[] c1 = OrderBasedCrossover(p1, p2, this.indices.AsSpan()[..K]);
            int[] c2 = OrderBasedCrossover(p2, p1, this.indices.AsSpan()[..K]);

            return (c1, c2);
        }

        public static int[] Mutate(int[] genes, IRandom random)
        {
            int l = random.Next(N);
            int r = random.Next(N);
            if (l > r)
            {
                (l, r) = (r, l);
            }
            ++r;
            int i = random.Next(N - (r - l));
            return CutAndInsert(genes, l, r, i);
        }

        public static double Evaluate(Rule rule, Dealer dealer, Func<IEngine> engineFactory, Func<IEngine[]> oppositeEnginesFactory)
        {
            const int NumGames = 10000000;

            IEngine[] engines = [engineFactory(), .. oppositeEnginesFactory()];

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
