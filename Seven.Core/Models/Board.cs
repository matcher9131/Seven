namespace Seven.Core.Models
{
    public interface IReadonlyBoard
    {
        ulong Cards { get; }
    }

    public interface IBoard : IReadonlyBoard
    {
        void SetCard(int card);
        void SetCards(ulong cards);
    }

    public class Board : IBoard
    {
        public ulong Cards { get; private set; }

        public void SetCard(int card)
        {
            this.Cards |= 1UL << card;
        }

        public void SetCards(ulong cards)
        {
            this.Cards |= cards;
        }
    }
}
