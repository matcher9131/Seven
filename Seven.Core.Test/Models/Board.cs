using Seven.Core.Models;

namespace Seven.Core.Test.Models
{
    public class BoardTest
    {
        [Fact]
        public void ConstructorTest()
        {
            Board board = new();
            Assert.Equal(0b0000001000000_0000001000000_0000001000000_0000001000000UL, board.Cards);
        }

        [Fact]
        public void SetBoardTest()
        {
            Board board = new();
            board.SetCard(10);
            Assert.Equal(0b0000001000000_0000001000000_0000001000000_0010001000000UL, board.Cards);
        }
    }
}
