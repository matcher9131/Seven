using Seven.Core.Engines;
using System.Numerics;

namespace Seven.Core.Models
{
    public interface IReadonlyPlayer
    {
        int NumCards { get; }
        int NumPasses { get; }
    }

    public interface IPlayer : IReadonlyPlayer
    {
        ulong Cards { get; }
    }

    public class Player(ulong cards, IReadonlyGame game, IEngine engine) : IPlayer
    {
        private readonly IEngine engine = engine;

        // 0-12: スペード
        // 13-25: ハート
        // 26-38: ダイヤ
        // 39-51: クラブ
        // 52-53: ジョーカー
        public ulong Cards { get; private set; } = cards;

        private readonly IReadonlyGame game = game;

        public int NumCards => BitOperations.PopCount(this.Cards);

        public int NumPasses { get; private set; }

        public int Rank { get; set; } = -1;

        public int Play() {
            int card = this.engine.Next(this.game, this);
            if (card == -1)
            {
                ++this.NumPasses;
                if (this.NumPasses >= this.game.Rule.NumPasses)
                {
                    this.game.PlayerLose(this);
                }
            }
            else
            {
                this.Cards ^= 1UL << card;
                if (this.NumCards == 0)
                {
                    this.game.PlayerWin(this);
                }
            }

            return card;
        }
    }
}
