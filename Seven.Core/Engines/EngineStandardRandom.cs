using Seven.Core.Random;
using Seven.Core.Rules;
using System.Collections.ObjectModel;

namespace Seven.Core.Engines
{
    public class EngineStandardRandom : EngineStandardMyCards
    {
        private static ReadOnlyDictionary<int, int> PriorityMap => Enumerable.Range(-1, 65).ToDictionary(x => x, x => 0).AsReadOnly();

        public EngineStandardRandom(Rule rule, IRandom random) : base(rule, random, PriorityMap)
        {
            if (rule != Rule.Standard) throw new NotSupportedException("This engine does not support the given rule.");
        }
    }
}
