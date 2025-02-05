using Seven.Core.Models;

namespace Seven.Core.Test.Models
{
    public class BoardTest
    {
        [Fact]
        public void SetCardTest()
        {
            Board board = new();
            board.SetCard(10);
            Assert.Equal(0b0000000000000_0000000000000_0000000000000_0010000000000UL, board.Cards);
        }

        [Fact]
        public void SetCardsTest()
        {
            Board board = new();
            board.SetCards(0b0110110011010UL);
            Assert.Equal(0b0110110011010UL, board.Cards);
            board.SetCards(0b1001000111010UL);
            Assert.Equal(0b1111110111010UL, board.Cards);
        }
    }
}
