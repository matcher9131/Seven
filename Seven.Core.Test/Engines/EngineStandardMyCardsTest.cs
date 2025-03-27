using Moq;
using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Random;
using Seven.Core.Rules;
using System.Collections.ObjectModel;
using System.Numerics;

namespace Seven.Core.Test.Engines
{
    public class EngineStandardMyCardsTest
    {
        // 最もA, Kに近いものの優先度を高くする（パスは優先度最低）
        // ※失格プレイヤーのカードの分だけ端に寄せる（例：Kが場に出ているときのQはK扱い）
        private static ReadOnlyDictionary<int, int> PriorityMap => Enumerable.Range(-1, 65).ToDictionary(
            x => x,
            x => x switch
            {
                -1 => 100,
                int pattern => BitOperations.TrailingZeroCount(pattern),
            }
        ).AsReadOnly();

        [Fact(DisplayName = "RuleがStandardではないときにコンストラクターが例外を投げる")]
        public void CtorTestThrows()
        {
            var random = new Mock<IRandom>();
            var rule = new Rule(1, "Not Standard", 5, 3, true, false, false, WinPointMethod.Standard);
            var ex = Assert.Throws<NotSupportedException>(() => new EngineStandardMyCards(rule, random.Object, PriorityMap));

            Assert.Equal("This engine does not support the given rule.", ex.Message);
        }

        [Fact(DisplayName = "出せるカードがないときにパス回数にかかわらずパスをする")]
        public void NextTestForcedPass()
        {
            var board = new Mock<IBoard>();
            board.SetupGet(board => board.Cards).Returns(0b000000_1_100000__000011_1_111010__001111_1_111110__110100_1_000111UL);
            var game = new Mock<IReadonlyGame>();
            game.SetupGet(game => game.Board).Returns(board.Object);
            game.SetupGet(game => game.Rule).Returns(Rule.Standard);
            var player = new Mock<IReadonlyPlayer>();
            player.SetupGet(player => player.Cards).Returns(0b000010_0_000100__001000_0_000001__000000_0_000000__001000_0_010000UL);
            player.SetupGet(player => player.NumPasses).Returns(3);
            var random = new Mock<IRandom>();
            random.Setup(random => random.Next(It.IsAny<uint>())).Returns(0);
            var engine = new EngineStandardMyCards(Rule.Standard, random.Object, PriorityMap);

            int actual = engine.Next(game.Object, player.Object);

            Assert.Equal(-1, actual);
        }

        [Fact(DisplayName = "出せるカードがあるときに与えられた優先度に応じてカードを出す")]
        public void NextTestPlayable()
        {
            var board = new Mock<IBoard>();
            board.SetupGet(board => board.Cards).Returns(0b000000_1_100000__000011_1_111110__011111_1_110011__110000_1_110111UL);
            var game = new Mock<IReadonlyGame>();
            game.SetupGet(game => game.Board).Returns(board.Object);
            game.SetupGet(game => game.Rule).Returns(Rule.Standard);
            var player = new Mock<IReadonlyPlayer>();
            player.SetupGet(player => player.Cards).Returns(0b000001_0_010000__000100_0_000001__100000_0_001000__001000_0_001000UL);
            var random = new Mock<IRandom>();
            random.SetupSequence(random => random.Next(It.IsAny<uint>()))
                .Returns(0)
                .Returns(1)
                .Returns(2);
            var engine = new EngineStandardMyCards(Rule.Standard, random.Object, PriorityMap);

            int[] expectedValues = [3, 25, 26];

            for (int i = 0; i < expectedValues.Length; ++i)
            {
                int actual = engine.Next(game.Object, player.Object);

                Assert.Equal(expectedValues[i], actual);
            }

            player.SetupGet(player => player.Cards).Returns(0b000001_0_010000__000000_0_000000__000000_0_000000__000000_0_000000UL);
            random.Setup(random => random.Next(It.IsAny<uint>())).Returns(0);

            int actual2 = engine.Next(game.Object, player.Object);

            Assert.Equal(43, actual2);
        }
    }
}
