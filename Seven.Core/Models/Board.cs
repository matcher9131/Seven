namespace Seven.Core.Models
{
    // Spade, Heart, Diamond, Club: ビット列で下から[4i, 4i+3]桁目が表す数をc_iとするとき、人c_iが数iのカードを持つ
    // （カードが場に出ているときはb_i = 0xF）
    // Joker: 下からi桁目のビットが1のとき、人iがジョーカーを持つ
    // NumPasses: ビット列で下から[4i, 4i+3]桁目が表す数をp_iとするとき、人iの現在のパス回数がp_i回
    // Ranks: ビット列で下から[4i, 4i+3]桁目が表す数をr_iとするとき、人iの順位がr_i(0-indexed)
    // （順位未確定の場合はr_i = 0xF）
    public record Board(ulong Spade, ulong Heart, ulong Diamond, ulong Club, uint Joker, ulong NumPasses, ulong Ranks)
    {
        private static int GetHex(ulong x, int i)
        {
            return (int)(x >> 4 * i & 0xFul);
        }

        public int GetNumPasses(int player)
        {
            return GetHex(NumPasses, player);
        }

        public IEnumerable<int> GetCards(int player)
        {
            for (int i = 0; i < 13; ++i)
            {
                if (GetHex(Spade, i) == player) yield return i;
            }
            for (int i = 0; i < 13; ++i)
            {
                if (GetHex(Heart, i) == player) yield return 13 + i;
            }
            for (int i = 0; i < 13; ++i)
            {
                if (GetHex(Diamond, i) == player) yield return 26 + i;
            }
            for (int i = 0; i < 13; ++i)
            {
                if (GetHex(Club, i) == player) yield return 39 + i;
            }
            if ((Joker & 1u << player) > 0) yield return 53;
        }

        public IEnumerable<int> GetPlayableCards(int player)
        {
            return GetCards(player).Where(card =>
            {
                // TODO: JokerのPlayable条件
                if (card == 53) return true;

                int suit = card / 13, num = card % 13;
                ulong target = suit switch
                {
                    0 => Spade,
                    1 => Heart,
                    2 => Diamond,
                    3 => Club,
                    _ => throw new InvalidOperationException()
                };
                return (num > 7 ? Enumerable.Range(8, num - 7) : Enumerable.Range(num, 7 - num)).All(i => GetHex(target, i) == 0xF);
            });
        }
    }
}
