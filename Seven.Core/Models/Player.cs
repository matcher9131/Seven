using Seven.Core.Engines;
using Seven.Core.Rules;
using System.Numerics;

namespace Seven.Core.Models
{
    public interface IReadonlyPlayer
    {
        int NumCards { get; }
        int NumPasses { get; }
    }

    public class Player(ulong cards, IReadonlyGame game, IEngine engine) : IReadonlyPlayer
    {
        private readonly IReadonlyGame game = game;

        // 0-12: スペード
        // 13-25: ハート
        // 26-38: ダイヤ
        // 39-51: クラブ
        // 52-53: ジョーカー
        private ulong cards = cards;
        private readonly IEngine engine = engine;

        public int NumCards => BitOperations.PopCount(this.cards);

        public int NumPasses { get; private set; }

        public int Rank { get; set; } = -1;

        public int Play() {
            int card = this.engine.Next(this.cards);
            if (card == -1)
            {
                ++this.NumPasses;
                if (this.NumPasses >= this.game.Rule.NumPasses)
                {
                    this.game.SetPlayerRank(this, false);
                }
            }
            else
            {
                this.cards ^= 1UL << card;
                if (this.NumCards == 0)
                {
                    this.game.SetPlayerRank(this, true);
                }
            }

            return card;
        }
    }
}
