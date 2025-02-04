using Seven.Core.Models;
using Seven.Core.Test.TestDoubles;

namespace Seven.Core.Test.Models
{
    public class PlayerTest
    {
        [Fact]
        public void NumCardsTest()
        {
            var engine = new EngineMinCard();
            var game = new GameDummy();
            var player = new Player(0b0100000010000_1101000101100_0011100110111_1110000001001UL, game, engine);
            Assert.Equal(21, player.NumCards);
        }
    }
}
