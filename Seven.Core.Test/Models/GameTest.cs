using Moq;
using Seven.Core.Engines;
using Seven.Core.Models;

namespace Seven.Core.Test.Models
{
    public class GameTest
    {
        [Fact]
        public void PlayerWinTest()
        {
            var boardMock = new Mock<IBoard>();
            var playerMocks = Enumerable.Range(0, 5).Select(_ => new Mock<IPlayer>()).ToArray();
            playerMocks[0].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[1].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[2].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[3].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[4].SetupGet(player => player.Rank).Returns(-1);
            var game = new Game(Rules.Rule.Standard, boardMock.Object, playerMocks.Select(playerMock => playerMock.Object).ToArray());

            game.PlayerWin(playerMocks[3].Object);

            playerMocks[3].Verify(player => player.SetRank(0), Times.Once);
        }

        [Fact]
        public void PlayerWinTest2()
        {
            var boardMock = new Mock<IBoard>();
            var playerMocks = Enumerable.Range(0, 5).Select(_ => new Mock<IPlayer>()).ToArray();
            playerMocks[0].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[1].SetupGet(player => player.Rank).Returns(0);
            playerMocks[2].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[3].SetupGet(player => player.Rank).Returns(4);
            playerMocks[4].SetupGet(player => player.Rank).Returns(1);
            var game = new Game(Rules.Rule.Standard, boardMock.Object, playerMocks.Select(playerMock => playerMock.Object).ToArray());

            game.PlayerWin(playerMocks[2].Object);

            playerMocks[2].Verify(player => player.SetRank(2), Times.Once);
        }

        [Fact]
        public void PlayerLoseTest()
        {
            var boardMock = new Mock<IBoard>();
            var playerMocks = Enumerable.Range(0, 5).Select(_ => new Mock<IPlayer>()).ToArray();
            playerMocks[0].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[1].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[2].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[3].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[4].SetupGet(player => player.Rank).Returns(-1);
            var game = new Game(Rules.Rule.Standard, boardMock.Object, playerMocks.Select(playerMock => playerMock.Object).ToArray());

            game.PlayerLose(playerMocks[3].Object);

            playerMocks[3].Verify(player => player.SetRank(4), Times.Once);
        }

        [Fact]
        public void PlayerLoseTest2()
        {
            var boardMock = new Mock<IBoard>();
            var playerMocks = Enumerable.Range(0, 5).Select(_ => new Mock<IPlayer>()).ToArray();
            playerMocks[0].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[1].SetupGet(player => player.Rank).Returns(0);
            playerMocks[2].SetupGet(player => player.Rank).Returns(-1);
            playerMocks[3].SetupGet(player => player.Rank).Returns(4);
            playerMocks[4].SetupGet(player => player.Rank).Returns(1);
            var game = new Game(Rules.Rule.Standard, boardMock.Object, playerMocks.Select(playerMock => playerMock.Object).ToArray());

            game.PlayerLose(playerMocks[2].Object);

            playerMocks[2].Verify(player => player.SetRank(3), Times.Once);
        }

        [Fact]
        public void PlayTestInitialize()
        {
            var boardMock = new Mock<IBoard>();
            var playerMocks = Enumerable.Range(0, 5).Select(_ => new Mock<IPlayer>()).ToArray();
            playerMocks[0].Setup(player => player.Has(Const.SevenOfDiamonds)).Returns(false);
            playerMocks[1].Setup(player => player.Has(Const.SevenOfDiamonds)).Returns(true);
            playerMocks[2].Setup(player => player.Has(Const.SevenOfDiamonds)).Returns(false);
            playerMocks[3].Setup(player => player.Has(Const.SevenOfDiamonds)).Returns(false);
            playerMocks[4].Setup(player => player.Has(Const.SevenOfDiamonds)).Returns(false);
            var game = new Game(Rules.Rule.Standard, boardMock.Object, playerMocks.Select(playerMock => playerMock.Object).ToArray());

            bool result = game.Play();

            Assert.False(result);
            foreach (var playerMock in playerMocks)
            {
                playerMock.Verify(player => player.PutSevens(), Times.Once());
            }
        }

        [Fact]
        public void PlayTestNextPlayer()
        {
            var boardMock = new Mock<IBoard>();
            var playerMocks = Enumerable.Range(0, 5).Select(_ => new Mock<IPlayer>()).ToArray();
            var game = new Game(Rules.Rule.Standard, boardMock.Object, playerMocks.Select(playerMock => playerMock.Object).ToArray());
            for (int i = 0; i < playerMocks.Length; ++i)
            {
                // players[3]が勝ち抜け、players[4]が失格、その他未確定
                playerMocks[i].SetupGet(player => player.Rank).Returns(i == 3 ? 0 : i == 4 ? 4 : -1);
                // players[2]からはじまる
                playerMocks[i].Setup(player => player.Has(Const.SevenOfDiamonds)).Returns(i == 2);
                playerMocks[i].Setup(player => player.Play(game)).Returns(0);
            }
            // Initialize
            game.Play();

            bool result = game.Play();
            Assert.False(result);
            playerMocks[2].Verify(player => player.Play(game), Times.Once());
            playerMocks[0].Verify(player => player.Play(game), Times.Never());

            result = game.Play();
            Assert.False(result);
            playerMocks[0].Verify(player => player.Play(game), Times.Once());
            playerMocks[3].Verify(player => player.Play(game), Times.Never());
            playerMocks[4].Verify(player => player.Play(game), Times.Never());
        }

        [Fact]
        public void PlayTestFinish()
        {
            // 各プレイヤーに与える順位
            int[] ranks = [4, 2, -1, 0, 1];
            var boardMock = new Mock<IBoard>();
            var playerMocks = Enumerable.Range(0, 5).Select(_ => new Mock<IPlayer>()).ToArray();
            var game = new Game(Rules.Rule.Standard, boardMock.Object, playerMocks.Select(playerMock => playerMock.Object).ToArray());
            for (int i = 0; i < playerMocks.Length; ++i)
            {
                playerMocks[i].SetupGet(player => player.Rank).Returns(ranks[i]);
                // players[2]からはじまる
                playerMocks[i].Setup(player => player.Has(Const.SevenOfDiamonds)).Returns(i == 2);
            }
            // Initialize
            game.Play();

            bool result = game.Play();
            Assert.True(result);
            playerMocks[2].Verify(player => player.SetRank(3), Times.Once());
        }
    }
}
