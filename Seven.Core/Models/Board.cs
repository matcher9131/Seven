namespace Seven.Core.Models
{
    public interface IReadonlyBoard
    {
        ulong Cards { get; }
    }

    public class Board : IReadonlyBoard
    {
        public Board()
        {
            this.Cards = 1UL << 6 | 1UL << 19 | 1UL << 32 | 1UL << 45;
        }

        public ulong Cards { get; private set; }

        public void SetCard(int card)
        {
            this.Cards |= 1UL << card;
        }
    }
}
