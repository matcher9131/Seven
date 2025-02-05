using Seven.Core.Rules;

namespace Seven.Core.Models
{
    public interface IReadonlyGame
    {
        IReadonlyBoard Board { get; }
        IEnumerable<IOtherPlayer> Players { get; }
        Rule Rule { get; }
        void PlayerWin(IPlayer player);
        void PlayerLose(IPlayer player);
    }

    public class Game(Rule rule, IBoard board, IPlayer[] players) : IReadonlyGame
    {
        private bool initialized;
        private readonly IBoard board = board;
        public IReadonlyBoard Board => this.board;

        private readonly IPlayer[] players = players;
        private int currentPlayerIndex = -1;

        public IEnumerable<IOtherPlayer> Players
        {
            get
            {
                foreach (var player in this.players)
                {
                    yield return player;
                }
            }
        }

        public Rule Rule { get; } = rule;

        private IPlayer CurrentPlayer => this.players[currentPlayerIndex];

        private void SetPlayerRank(IPlayer player, bool wins)
        {
            if (wins)
            {
                for (int rank = 0; rank < this.players.Length; ++rank)
                {
                    if (this.players.All(p => p.Rank != rank))
                    {
                        player.SetRank(rank);
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
                        player.SetRank(rank);
                        return;
                    }
                }
                throw new InvalidOperationException();
            }
        }

        public void PlayerWin(IPlayer player)
        {
            this.SetPlayerRank(player, true);
        }

        public void PlayerLose(IPlayer player)
        {
            this.SetPlayerRank(player, false);
            this.board.SetCards(player.Cards);
        }

        public bool Play()
        {
            if (!this.initialized)
            {
                this.currentPlayerIndex = Array.FindIndex(this.players, p => p.Has(Const.SevenOfDiamonds));
                foreach (var player in this.players)
                {
                    player.PutSevens();
                }
                this.board.SetCards(Const.Sevens);
                this.initialized = true;
                return false;
            }

            int card = this.CurrentPlayer.Play(this);
            if (card >= 0)
            {
                this.board.SetCard(card);
            }

            // 残り1人になったら順位をつけて終わる
            var unfinishedPlayers = this.players.Where(p => p.Rank == -1);
            if (unfinishedPlayers.Count() == 1)
            {
                SetPlayerRank(unfinishedPlayers.First(), true);
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
