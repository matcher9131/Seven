using Seven.Core.Engines;
using Seven.Core.Rules;
using System.Web;

namespace Seven.Core.Models
{
    public interface IReadonlyGame
    {
        IReadonlyBoard Board { get; }
        IEnumerable<IReadonlyPlayer> Players { get; }
        Rule Rule { get; }
        void SetPlayerRank(Player player, bool wins);
    }

    public class Game : IReadonlyGame
    {
        public Game(Rule rule, IEnumerable<IEngine> engines)
        {
            // TODO: players, currentPlayerIndexの初期化
            throw new NotImplementedException();

            this.Rule = rule;
        }

        private readonly Board board = new();
        public IReadonlyBoard Board => this.board;

        private readonly Player[] players;
        private int currentPlayerIndex;

        public IEnumerable<IReadonlyPlayer> Players
        {
            get
            {
                foreach (var player in this.players)
                {
                    yield return player;
                }
            }
        }

        public Rule Rule { get; }

        private Player CurrentPlayer => this.players[currentPlayerIndex];

        public void SetPlayerRank(Player player, bool wins)
        {
            if (wins)
            {
                for (int rank = 0; rank < this.players.Length; ++rank)
                {
                    if (this.players.All(p => p.Rank != rank))
                    {
                        player.Rank = rank;
                        return;
                    }
                }
                throw new InvalidOperationException();
            }
            else
            {
                for (int rank = this.players.Length - 1; rank >= 0; --rank)
                {
                    if (this.players.All(p => p.Rank != rank))
                    {
                        player.Rank = rank;
                        return;
                    }
                }
                throw new InvalidOperationException();
            }
        }

        public bool Play()
        {
            int card = this.CurrentPlayer.Play();
            if (card >= 0)
            {
                this.board.SetCard(card);
            }

            // 残り1人になったら順位をつけて終わる
            var unfinishedPlayer = this.players.SingleOrDefault(p => p.Rank == -1);
            if (unfinishedPlayer is not null)
            {
                SetPlayerRank(unfinishedPlayer, true);
                return true;
            }

            do
            {
                this.currentPlayerIndex = (this.currentPlayerIndex + 1) % this.players.Length;
            }
            while (this.CurrentPlayer.Rank >= 0);

            return false;
        }
    }
}
