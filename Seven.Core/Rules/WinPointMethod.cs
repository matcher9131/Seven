namespace Seven.Core.Rules
{
    public enum WinPointMethod
    {
        Standard
    }

    public static class WinPoint
    {
        public static double GetWinPoint(WinPointMethod method, int rank) => method switch
        {
            WinPointMethod.Standard => rank == 0 ? 4
                : rank == 1 ? 2
                : rank == 2 ? 1
                : 0,
            _ => throw new NotSupportedException()
        };
    }
}
