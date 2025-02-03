namespace Seven.Core
{
    public static class Util
    {
        private static Random random = new Random();

        public static ulong[] GetDealtCards(int numPlayers, bool containsJoker)
        {
            int numCards = containsJoker ? 53 : 52;
            int[] playerNumCards = Enumerable.Repeat(numCards / numPlayers, numPlayers).ToArray();
            int r = numCards % numPlayers;
            int offset = random.Next(numPlayers);
            for (int i = 0; i < r; ++i)
            {
                ++playerNumCards[(i + offset) % numPlayers];
            }

            int[] cards = Enumerable.Range(0, numCards).ToArray();
            // Fisher–Yates shuffle
            for (int i = cards.Length - 1; i > 0; --i)
            {
                int j = random.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }

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
