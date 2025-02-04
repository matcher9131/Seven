using Seven.Core.Engines;
using Seven.Core.Models;

namespace Seven.Core.Test.TestDoubles
{
    // 常に出せる最小のカードを出すEngine
    public class EngineMinCard : EngineBase
    {
        public EngineMinCard() : base("EngineMinCard")
        {
        }

        public override int Next(IReadonlyGame game, IPlayer player)
        {
            for (int suit = 0; suit < 4; ++suit)
            {
                for (int num = 5; num >= 0; --num)
                {
                    int i = 13 * suit + num;
                    if ((player.Cards & 1UL << i) > 0)
                    {
                        return i;
                    }
                    if ((game.Board.Cards & 1UL << i) == 0) break;
                }
                for (int num = 7; num < 13; ++num)
                {
                    int i = 13 * suit + num;
                    if ((player.Cards & 1UL << i) > 0)
                    {
                        return i;
                    }
                    if ((game.Board.Cards & 1UL << i) == 0) break;
                }
            }
            return -1;
        }
    }
}
