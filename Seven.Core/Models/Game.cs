using Seven.Core.Engines;
using Seven.Core.Rules;

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
        public Game(Rule rule, IEngine[] engines)
        {
            this.Rule = rule;

            int numPlayers = engines.Length;
            ulong[] dealtCards = Util.GetDealtCards(numPlayers, rule.ContainsJoker);
            // ダイヤの7を持つプレイヤーから開始
            this.currentPlayerIndex = Array.FindIndex(dealtCards, cards => (cards & 1UL << 32) > 0);
            if (this.currentPlayerIndex == -1) throw new InvalidOperationException();

            this.players = new Player[numPlayers];
            for (int i = 0; i < numPlayers; ++i)
            {
                // 各スートの7を除いた手札を配る
                this.players[i] = new Player(dealtCards[i] ^ 0b0000001000000_0000001000000_0000001000000_0000001000000UL, this, engines[i]);
            }
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

        public void PlayerWin(Player player)
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

        public void PlayerLose(Player player)
        {
            for (int rank = this.players.Length - 1; rank >= 0; --rank)
            {
                if (this.players.All(p => p.Rank != rank))
                {
                    player.Rank = rank;
                }
            }
            if (player.Rank == -1) throw new InvalidOperationException();
            this.board.Cards |= player.Cards;
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
