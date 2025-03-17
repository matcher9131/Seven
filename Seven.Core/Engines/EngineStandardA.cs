using Seven.Core.Models;
using Seven.Core.Rules;
using System.Collections.ObjectModel;
using System.Numerics;

namespace Seven.Core.Engines
{
    // https://www.youtube.com/watch?v=GOAtomgEM4cで紹介されている行動パターン
    public class EngineStandardA : EngineStandardMyCards
    {
        private static ReadOnlyDictionary<int, int> PriorityMap => Enumerable.Range(-1, 65).ToDictionary(x => x, x =>
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
        }).AsReadOnly();

        public EngineStandardA(Rule rule, IRandom random) : base(rule, random, PriorityMap)
        {
            if (rule != Rule.Standard) throw new NotSupportedException("This engine does not support the given rule.");
        }
    }
}
