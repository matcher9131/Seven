using Moq;
using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Rules;

namespace Seven.Core.Test.Models
{
    public class PlayerTest
    {
        [Fact]
        public void NumCardsTest()
        {
            var engineMock = new Mock<IEngine>();
            var player = new Player(Rule.Standard, 0b0100000010000_1101000101100_0011100110111_1110000001001UL, engineMock.Object);
            Assert.Equal(21, player.NumCards);
        }

        [Fact]
        public void HasTest()
        {
            var engineMock = new Mock<IEngine>();
            var player = new Player(Rule.Standard, 0b0100000010000_1101000101100_0011100110111_1110000001001UL, engineMock.Object);
            Assert.True(player.Has(0));
            Assert.True(player.Has(15));
            Assert.False(player.Has(16));
        }

        [Fact]
        public void SetRankTest()
        {
            var engineMock = new Mock<IEngine>();
            var player = new Player(Rule.Standard, 0, engineMock.Object);
            player.SetRank(3);
            Assert.Equal(3, player.Rank);
        }

        [Fact]
        public void PutSevensTest()
        {
            var engineMock = new Mock<IEngine>();
            var player = new Player(Rule.Standard, 0b0000001000110_0001000000000_1000001000011_0000001000000UL, engineMock.Object);
            player.PutSevens();
            Assert.Equal(0b0000000000110_0001000000000_1000000000011_0000000000000UL, player.Cards);
        }

        [Fact]
        public void PlayTestLose()
        {
            var engineMock = new Mock<IEngine>();
            engineMock.Setup(engine => engine.Next(It.IsAny<IReadonlyGame>(), It.IsAny<IReadonlyPlayer>())).Returns(-1);
            // var engine = new EngineStub() { NextCard = -1 };
            var player = new Player(Rule.Standard, 0, engineMock.Object);
            var gameMock = new Mock<IReadonlyGame>();

            player.Play(gameMock.Object);

            Assert.Equal(1, player.NumPasses);

            player.Play(gameMock.Object);

            Assert.Equal(2, player.NumPasses);

            player.Play(gameMock.Object);

            Assert.Equal(3, player.NumPasses);
            gameMock.Verify(game => game.PlayerLose(player), Times.Never());

            player.Play(gameMock.Object);

            gameMock.Verify(game => game.PlayerLose(player), Times.Once());
        }

        [Fact]
        public void PlayTestWin()
        {
            var engineMock = new Mock<IEngine>();
            engineMock.SetupSequence(engine => engine.Next(It.IsAny<IReadonlyGame>(), It.IsAny<IReadonlyPlayer>()))
                    .Returns(0)
                    .Returns(1);
            var player = new Player(Rule.Standard, 0b11UL, engineMock.Object);
            var gameMock = new Mock<IReadonlyGame>();

            player.Play(gameMock.Object);

            Assert.Equal(0b10UL, player.Cards);
            gameMock.Verify(game => game.PlayerWin(player), Times.Never());

            player.Play(gameMock.Object);

            Assert.Equal(0UL, player.Cards);
            gameMock.Verify(game => game.PlayerWin(player), Times.Once());
        }
    }
}
