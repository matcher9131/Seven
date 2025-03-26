using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Seven.GA
{
    public static class Graph
    {
        private static readonly FrozenDictionary<int, ImmutableArray<int>> graph;
        private static readonly FrozenDictionary<int, int> inDegrees;
        private static readonly List<int> sources;

        static Graph()
        {
            graph = new Dictionary<int, ImmutableArray<int>>()
            {
                { -1, [] },
                { 1, [2, 4, 8, 16, 32] },
                { 2, [4, 8, 16, 32] },
                { 4, [8, 16, 32] },
                { 5, [2, 9, 10, 17, 20, 33, 40] },
                { 8, [16, 32] },
                { 9, [4, 17, 18, 33, 36] },
                { 10, [18, 20, 34, 40] },
                { 11, [2, 5, 10, 19, 22, 35, 44] },
                { 16, [32] },
                { 17, [8, 33, 34] },
                { 18, [34, 36] },
                { 19, [4, 9, 18, 35, 38] },
                { 20, [36, 40] },
                { 21, [10, 20, 37, 42] },
                { 22, [20, 38, 44] },
                { 23, [2, 5, 11, 20, 21, 22, 39, 46] },
                { 32, [] },
                { 33, [16] },
                { 34, [] },
                { 35, [8, 17, 34] },
                { 36, [] },
                { 37, [18, 36] },
                { 38, [36] },
                { 39, [4, 9, 19, 36, 37, 38] },
                { 40, [] },
                { 41, [20, 40] },
                { 42, [40] },
                { 43, [10, 21, 40, 41, 42] },
                { 44, [40] },
                { 45, [22, 40, 41, 44] },
                { 46, [40, 42, 44] },
                { 47, [2, 5, 11, 23, 40, 41, 42, 43, 44, 45, 46] },
            }.ToFrozenDictionary();

            Dictionary<int, int> tmpInDegree = graph.Keys.ToDictionary(x => x, _ => 0);
            foreach ((_, var neighbors) in graph)
            {
                foreach (int to in neighbors)
                {
                    ++tmpInDegree[to];
                }
            }
            inDegrees = tmpInDegree.ToFrozenDictionary();
            sources = [.. inDegrees.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key)];
        }
        public static ReadOnlySpan<int> GetVertexes() => graph.Keys.AsSpan();

        public static ReadOnlySpan<int> GetNeighbors(int from) => graph[from].AsSpan();

        public static Dictionary<int, int> GetInDegrees() => new(inDegrees);

        public static int[] GetSourceVertexes() => [.. sources];

        public static int NumVertexes => graph.Count;

    }
}
