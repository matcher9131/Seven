using Seven.Core.Models;
using Seven.Core.Rules;
using Seven.Core.Test.TestData;

namespace Seven.Core.Test.Models
{
    public class PlayerTest
    {
        [Fact]
        public void NumCardsTest()
        {
            var engine = new TestEngine();
            var game = new Game(Rule.Standard, [engine]);
            var player = new Player(0b0100000010000_1101000101100_0011100110111_1110000001001UL, game, engine);
            Assert.Equal(21, player.NumCards);
        }
    }
}
