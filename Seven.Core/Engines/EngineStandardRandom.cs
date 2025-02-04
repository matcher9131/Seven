﻿using Seven.Core.Models;
using Seven.Core.Rules;

namespace Seven.Core.Engines
{
    public class EngineStandardRandom : EngineBase
    {
        private readonly Random random = new();

        public EngineStandardRandom(Rule rule) : base("StandardRandom")
        {
            if (rule != Rule.Standard) throw new NotSupportedException("This engine does not support the given rule.");
        }

        public override int Next(IReadonlyGame game, IPlayer player)
        {
            List<int> options = [];
            if (player.NumPasses < game.Rule.NumPasses)
            {
                options.Add(-1);
            }
            for (int suit = 0; suit < 4; ++suit)
            {
                for (int num = 7; num < 13; ++num)
                {
                    int i = 13 * suit + num;
                    if ((player.Cards & 1UL << i) > 0)
                    {
                        options.Add(i);
                        break;
                    }
                    if ((game.Board.Cards & 1UL << i) == 0) break;
                }
                for (int num = 5; num >= 0; --num)
                {
                    int i = 13 * suit + num;
                    if ((player.Cards & 1UL << i) > 0)
                    {
                        options.Add(i);
                        break;
                    }
                    if ((game.Board.Cards & 1UL << i) == 0) break;
                }
            }
            return options[random.Next(options.Count)];
        }
    }
}
