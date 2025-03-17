using Moq;
using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Rules;

namespace Seven.Core.Test.Engines
{
    public class EngineStandardRandomTest
    {
        [Fact(DisplayName = "RuleがStandardではないときにコンストラクターが例外を投げる")]
        public void CtorTestThrows()
        {
            var random = new Mock<IRandom>();
            var rule = new Rule(1, "Not Standard", 5, 3, true, false, false, WinPointMethod.Standard);
            var ex = Assert.Throws<NotSupportedException>(() => new EngineStandardRandom(rule, random.Object));

            Assert.Equal("This engine does not support the given rule.", ex.Message);
        }

        [Fact(DisplayName = "出せるカードがないときにパス回数にかかわらずパスをする")]
        public void NextTestForcedPass()
        {
            var board = new Mock<IBoard>();
            board.SetupGet(board => board.Cards).Returns(0b000000_1_000000__000000_1_000000__000000_1_000000__000000_1_000000UL);
            var game = new Mock<IReadonlyGame>();
            game.SetupGet(game => game.Board).Returns(board.Object);
            game.SetupGet(game => game.Rule).Returns(Rule.Standard);
            var player = new Mock<IReadonlyPlayer>();
            player.SetupGet(player => player.Cards).Returns(0b110000_0_001100__000010_0_001001__100010_0_011111__111110_0_000000UL);
            player.SetupGet(player => player.NumPasses).Returns(3);
            var random = new Mock<IRandom>();
            random.Setup(random => random.Next(It.IsAny<int>())).Returns(0);
            var engine = new EngineStandardRandom(Rule.Standard, random.Object);

            int actual = engine.Next(game.Object, player.Object);

            Assert.Equal(-1, actual);
        }

        [Fact(DisplayName = "パス回数に余裕があるときに0番目の候補がパスになる")]
        public void NextTestIndendedPass()
        {
            var board = new Mock<IBoard>();
            board.SetupGet(board => board.Cards).Returns(0b000000_1_000000__000000_1_000000__000000_1_000000__000000_1_000000UL);
            var game = new Mock<IReadonlyGame>();
            game.SetupGet(game => game.Board).Returns(board.Object);
            game.SetupGet(game => game.Rule).Returns(Rule.Standard);
            var player = new Mock<IReadonlyPlayer>();
            player.SetupGet(player => player.Cards).Returns(0b000001_0_100000__000001_0_100000__000001_0_100000__000001_0_100000UL);
            player.SetupGet(player => player.NumPasses).Returns(2);
            var random = new Mock<IRandom>();
            random.Setup(random => random.Next(It.IsAny<int>())).Returns(0);
            var engine = new EngineStandardRandom(Rule.Standard, random.Object);

            int actual = engine.Next(game.Object, player.Object);

            Assert.Equal(-1, actual);
        }

        [Fact(DisplayName = "パス回数に余裕がないときに0番目の候補がパスにならない")]
        public void NextTestNeverPass()
        {
            var board = new Mock<IBoard>();
            board.SetupGet(board => board.Cards).Returns(0b000000_1_000000__000000_1_000000__000000_1_000000__000000_1_000000UL);
            var game = new Mock<IReadonlyGame>();
            game.SetupGet(game => game.Board).Returns(board.Object);
            game.SetupGet(game => game.Rule).Returns(Rule.Standard);
            var player = new Mock<IReadonlyPlayer>();
            player.SetupGet(player => player.Cards).Returns(0b000001_0_100000__000001_0_100000__000001_0_100000__000001_0_100000UL);
            player.SetupGet(player => player.NumPasses).Returns(3);
            var random = new Mock<IRandom>();
            random.Setup(random => random.Next(It.IsAny<int>())).Returns(0);
            var engine = new EngineStandardRandom(Rule.Standard, random.Object);

            int actual = engine.Next(game.Object, player.Object);

            Assert.NotEqual(-1, actual);
        }

        [Fact(DisplayName = "出せるカードのみが候補になる")]
        public void NextTestCards()
        {
            var board = new Mock<IBoard>();
            board.SetupGet(board => board.Cards).Returns(0b000000_1_000000__000000_1_000000__000000_1_000000__000000_1_000000UL);
            var game = new Mock<IReadonlyGame>();
            game.SetupGet(game => game.Board).Returns(board.Object);
            game.SetupGet(game => game.Rule).Returns(Rule.Standard);
            var player = new Mock<IReadonlyPlayer>();
            player.SetupGet(player => player.Cards).Returns(0b000001_0_100000__000010_0_000000__001001_0_101110__111111_0_000011UL);
            player.SetupGet(player => player.NumPasses).Returns(3);
            var random = new Mock<IRandom>();
            random.SetupSequence(random => random.Next(It.IsAny<int>()))
                .Returns(0)
                .Returns(1)
                .Returns(2)
                .Returns(3)
                .Returns(4);
            var engine = new EngineStandardRandom(Rule.Standard, random.Object);

            int[] expectedValues = [7, 18, 20, 44, 46];

            for (int i = 0; i < expectedValues.Length; ++i)
            {
                int actual = engine.Next(game.Object, player.Object);

                Assert.Equal(expectedValues[i], actual);
            }
        }
    }
}
