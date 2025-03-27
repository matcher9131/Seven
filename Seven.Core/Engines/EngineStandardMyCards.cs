using Seven.Core.Models;
using Seven.Core.Random;
using Seven.Core.Rules;
using System.Collections.ObjectModel;

namespace Seven.Core.Engines
{
    // 自分のカードとパス回数のみを見て判断する
    public class EngineStandardMyCards : IEngine
    {
        private readonly ReadOnlyDictionary<int, int> priorityMap;
        private IRandom? random;

        public EngineStandardMyCards(Rule rule, ReadOnlyDictionary<int, int> priorityMap)
        {
            if (rule != Rule.Standard) throw new NotSupportedException("This engine does not support the given rule.");
            this.priorityMap = priorityMap;
        }

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
                    ++bitIndex;
                    playCard = cardIndex;
                }
                else if ((boardCards & 1UL << cardIndex) == 0)
                {
                    ++bitIndex;
                }
            }
            return (pattern, playCard);
        }

        public void SetRandom(IRandom random)
        {
            this.random = random;
        }

        public int Next(IReadonlyGame game, IReadonlyPlayer player)
        {
            if (this.random is null) throw new InvalidOperationException("'random' is null.");

            List<int> playCardOptions = [-1];
            int currentPriority = player.NumPasses == game.Rule.NumPasses ? int.MaxValue : this.priorityMap[-1];

            for (int suit = 0; suit < 4; ++suit)
            {
                for (int j = 0; j < 2; ++j)
                {
                    bool upper = j == 1;
                    if (!GetCanPlay(player.Cards, game.Board.Cards, suit, upper)) continue;
                    (int pattern, int playCard) = GetBitPatternAndPlayableCard(player.Cards, game.Board.Cards, suit, upper);
                    int priority = this.priorityMap[pattern];
                    if (priority < currentPriority)
                    {
                        currentPriority = priority;
                        playCardOptions.Clear();
                        playCardOptions.Add(playCard);
                    }
                    else if (priority == currentPriority)
                    {
                        playCardOptions.Add(playCard);
                    }
                }
            }

            return playCardOptions[(int)this.random.Next((uint)playCardOptions.Count)];
        }
    }
}
