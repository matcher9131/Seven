namespace Seven.Core.Rules
{
    public record Rule(
        int Version,
        string Name,
        int NumPlayers,
        int NumPasses,
        bool ContainsJoker,
        bool IsContinuousEdges,
        bool AllowedFromEdgeOnly,
        WinPointMethod WinPointMethod
    )
    {
        public static Rule Standard => new(1, "Standard", 5, 3, false, false, false, WinPointMethod.Standard);
    }
}
