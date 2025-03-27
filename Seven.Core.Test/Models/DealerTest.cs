using Moq;
using Seven.Core.Models;
using Seven.Core.Random;
using System.Numerics;

namespace Seven.Core.Test.Models
{
    public class DealerTest
    {
        [Fact]
        public void DealTest()
        {
            int numPlayers = 5;
            var randomMock = new Mock<IRandom>();
            randomMock.Setup(random => random.Next(It.IsAny<uint>())).Returns(3);
            var dealer = new Dealer(randomMock.Object);
            ulong[] dealtCards = dealer.Deal(numPlayers, true);
            Assert.Equal(11, BitOperations.PopCount(dealtCards[0]));
            Assert.Equal(10, BitOperations.PopCount(dealtCards[1]));
            Assert.Equal(10, BitOperations.PopCount(dealtCards[2]));
            Assert.Equal(11, BitOperations.PopCount(dealtCards[3]));
            Assert.Equal(11, BitOperations.PopCount(dealtCards[4]));
        }

        [Fact]
        public void DealTest2()
        {
            int numPlayers = 5;
            var randomMock = new Mock<IRandom>();
            randomMock.Setup(random => random.Next(It.IsAny<uint>())).Returns(0);
            var dealer = new Dealer(randomMock.Object);
            ulong[] dealtCards = dealer.Deal(numPlayers, false);
            Assert.Equal(0b0000000000000_0000000000000_0000000000000_0111111111110UL, dealtCards[0]);
            Assert.Equal(0b0000000000000_0000000000000_0001111111111_1000000000000UL, dealtCards[1]);
            Assert.Equal(0b0000000000000_0000001111111_1110000000000_0000000000000UL, dealtCards[2]);
            Assert.Equal(0b0000000001111_1111110000000_0000000000000_0000000000000UL, dealtCards[3]);
            Assert.Equal(0b1111111110000_0000000000000_0000000000000_0000000000001UL, dealtCards[4]);
        }
    }
}
