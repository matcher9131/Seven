using Seven.Core.Engines;
using System.Numerics;

namespace Seven.Core.Models
{
    public interface IOtherPlayer
    {
        int NumCards { get; }
        int NumPasses { get; }
        int Rank { get; }
    }

    public interface IReadonlyPlayer : IOtherPlayer
    {
        // 0-12: スペード
        // 13-25: ハート
        // 26-38: ダイヤ
        // 39-51: クラブ
        // 52-53: ジョーカー
        ulong Cards { get; }
    }

    public interface IPlayer : IReadonlyPlayer
    {
        bool Has(int card);
        void SetRank(int rank);
        void PutSevens();
        int Play();
    }

    public class Player(ulong cards, IReadonlyGame game, IEngine engine) : IPlayer
    {
        private readonly IReadonlyGame game = game;
        private readonly IEngine engine = engine;

        public ulong Cards { get; private set; } = cards;

        public int NumCards => BitOperations.PopCount(this.Cards);

        public bool Has(int card) => (this.Cards & 1UL << card) > 0;

        public int NumPasses { get; private set; }

        public int Rank { get; private set; } = -1;

        public void SetRank(int rank) => this.Rank = rank;

        public void PutSevens()
        {
            this.Cards ^= Const.Sevens;
        }

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
