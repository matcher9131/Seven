using Seven.Core.Random;

namespace Seven.Core.Models
{
    public class Dealer
    {
        private IRandom? random;

        public void SetRandom(IRandom random)
        {
            this.random = random;
        }

        public ulong[] Deal(int numPlayers, bool containsJoker)
        {
            if (this.random is null) throw new InvalidOperationException("'random' is null.");

            int numCards = containsJoker ? 53 : 52;
            int[] playerNumCards = [.. Enumerable.Repeat(numCards / numPlayers, numPlayers)];
            int r = numCards % numPlayers;
            int offset = (int)this.random.Next((uint)numPlayers);
            for (int i = 0; i < r; ++i)
            {
                ++playerNumCards[(i + offset) % numPlayers];
            }

            int[] cards = [.. Enumerable.Range(0, numCards)];
            this.random.ShuffleList(cards);

            ulong[] dealtCards = new ulong[numPlayers];
            int startIndex = 0;
            for (int i = 0; i < numPlayers; ++i)
            {
                dealtCards[i] = cards[startIndex..(startIndex + playerNumCards[i])].Aggregate(0UL, (ulong acc, int currentCard) => acc | 1UL << currentCard);
                startIndex += playerNumCards[i];
            }
            return dealtCards;
        }
    }
}
