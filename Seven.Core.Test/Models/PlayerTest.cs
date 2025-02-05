using Moq;
using Seven.Core.Models;
using Seven.Core.Rules;
using Seven.Core.Test.TestDoubles;

namespace Seven.Core.Test.Models
{
    public class PlayerTest
    {
        [Fact]
        public void NumCardsTest()
        {
            var engine = new EngineStub();
            var player = new Player(Rule.Standard, 0b0100000010000_1101000101100_0011100110111_1110000001001UL, engine);
            Assert.Equal(21, player.NumCards);
        }

        [Fact]
        public void HasTest()
        {
            var engine = new EngineStub();
            var player = new Player(Rule.Standard, 0b0100000010000_1101000101100_0011100110111_1110000001001UL, engine);
            Assert.True(player.Has(0));
            Assert.True(player.Has(15));
            Assert.False(player.Has(16));
        }

        [Fact]
        public void SetRankTest()
        {
            var engine = new EngineStub();
            var player = new Player(Rule.Standard, 0, engine);
            player.SetRank(3);
            Assert.Equal(3, player.Rank);
        }

        [Fact]
        public void PutSevensTest()
        {
            var engine = new EngineStub();
            var player = new Player(Rule.Standard, 0b0000001000110_0001000000000_1000001000011_0000001000000UL, engine);
            player.PutSevens();
            Assert.Equal(0b0000000000110_0001000000000_1000000000011_0000000000000UL, player.Cards);
        }

        [Fact]
        public void PlayTestLose()
        {
            var engine = new EngineStub() { NextCard = -1 };
            var player = new Player(Rule.Standard, 0, engine);
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
            var engine = new EngineStub();
            var player = new Player(Rule.Standard, 0b11UL, engine);
            var gameMock = new Mock<IReadonlyGame>();
            engine.NextCard = 0;

            player.Play(gameMock.Object);

            Assert.Equal(0b10UL, player.Cards);
            gameMock.Verify(game => game.PlayerWin(player), Times.Never());

            engine.NextCard = 1;
            player.Play(gameMock.Object);

            Assert.Equal(0UL, player.Cards);
            gameMock.Verify(game => game.PlayerWin(player), Times.Once());
        }
    }
}
