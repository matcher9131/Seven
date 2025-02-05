using Seven.Core.Engines;
using Seven.Core.Rules;
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
        int Play(IReadonlyGame game);
    }

    public class Player(Rule rule, ulong cards, IEngine engine) : IPlayer
    {
        private readonly Rule rule = rule;
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

        public int Play(IReadonlyGame game) {
            int card = this.engine.Next(game, this);
            if (card == -1)
            {
                ++this.NumPasses;
                if (this.NumPasses >= this.rule.NumPasses)
                {
                    game.PlayerLose(this);
                }
            }
            else
            {
                this.Cards ^= 1UL << card;
                if (this.NumCards == 0)
                {
                    game.PlayerWin(this);
                }
            }

            return card;
        }
    }
}
