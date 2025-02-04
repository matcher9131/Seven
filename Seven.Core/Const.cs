using System.Collections.ObjectModel;

namespace Seven.Core
{
    public static class Const
    {
        public static int SevenOfDiamonds { get; } = 32;
        public static ulong Sevens { get; } = 1UL << 6 | 1UL << 19 | 1UL << 32 | 1UL << 45;
    }
}
