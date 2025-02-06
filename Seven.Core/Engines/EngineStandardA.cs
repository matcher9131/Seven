using Seven.Core.Models;
using Seven.Core.Rules;
using System.Numerics;

namespace Seven.Core.Engines
{
    // https://www.youtube.com/watch?v=GOAtomgEM4cで紹介されている行動パターン
    public class EngineStandardA : EngineBase
    {
        private readonly IRandom random;

        public EngineStandardA(Rule rule, IRandom random) : base("Standard A")
        {
            if (rule != Rule.Standard) throw new NotSupportedException("This engine does not support the given rule.");
            this.random = random;
        }

        // Valueが小さいほど優先すべき行動
        private static readonly IReadOnlyDictionary<int, int> PriorityMap = Enumerable.Range(-1, 33).ToDictionary(x =>
        {
            // パス: 4
            if (x == -1) return 4;
            if (x == 0) return int.MaxValue;
            int msb = 31 - BitOperations.LeadingZeroCount((uint)x);
            int msb2 = 31 - BitOperations.LeadingZeroCount((uint)(x ^= 1 << msb));
            if (msb2 == -1)
            {
                // 単品
                return msb switch
                {
                    0 => 1, // K
                    1 => 5, // Q
                    2 => 7, // J
                    3 => 9, // 10
                    4 => 11, // 9
                    5 => 12, // 8
                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                return (msb - msb2) switch
                {
                    1 => 2, // 階段
                    2 => 3, // 一間
                    3 => 6, // 二間
                    4 => 8, // 三間
                    5 => 10, // 四間
                    _ => throw new InvalidOperationException()
                };
            }
        }, x => x);

        private static readonly int[] Upper = [12, 11, 10, 9, 8, 7];
        private static readonly int[] Lower = [0, 1, 2, 3, 4, 5];

        private static bool GetCanPlay(ulong cards, ulong boardCards, int suit, bool upper)
        {
            var array = upper ? Upper : Lower;
            for (int i = array.Length - 1; i >= 0; --i)
            {
                int cardIndex = 13 * suit + array[i];
                if ((cards & 1UL << cardIndex) > 0) return true;
                if ((boardCards & 1UL << cardIndex) == 0) return false;
            }
            return false;
        }

        private static (int pattern, int playCard) GetBitPatternAndPlayableCard(ulong cards, ulong boardCards, int suit, bool upper)
        {
            int pattern = 0;
            int bitIndex = 0;
            int playCard = -1;
            foreach (int num in upper ? Upper : Lower)
            {
                int cardIndex = 13 * suit + num;
                if ((cards & 1UL << cardIndex) > 0)
                {
                    pattern |= 1 << bitIndex;
                    playCard = cardIndex;
                }
                else if ((boardCards & 1UL << cardIndex) == 0)
                {
                    ++bitIndex;
                }
            }
            return (pattern, playCard);
        }

        public override int Next(IReadonlyGame game, IReadonlyPlayer player)
        {
            List<int> playCardOptions = [-1];
            int currentPriority = player.NumPasses == game.Rule.NumPasses ? int.MaxValue : PriorityMap[-1];

            for (int suit = 0; suit < 4; ++suit)
            {
                for (int j = 0; j < 2; ++j)
                {
                    bool upper = j == 1;
                    if (!GetCanPlay(player.Cards, game.Board.Cards, suit, upper)) continue;
                    (int pattern, int playCard) = GetBitPatternAndPlayableCard(player.Cards, game.Board.Cards, suit, upper);
                    int priority = PriorityMap[pattern];
                    if (priority < currentPriority)
                    {
                        priority = currentPriority;
                        playCardOptions.Clear();
                        playCardOptions.Add(playCard);
                    }
                    else if (priority == currentPriority)
                    {
                        playCardOptions.Add(playCard);
                    }
                }
            }

            return playCardOptions[this.random.Next(playCardOptions.Count)];
        }
    }
}
