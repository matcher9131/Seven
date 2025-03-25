using Seven.Core;

namespace Seven.GA
{
    public class InitialPopulation(IRandom random)
    {
        private readonly IRandom random = random;

        private List<int> GenerateOne()
        {
            Dictionary<int, int> inDegree = Graph.GetInDegrees();
            List<int> result = [];
            List<int> source = Graph.GetSourceVertexes();

            while (source.Count > 0)
            {
                List<int> nextSource = [];
                source.Shuffle(this.random);
                result.AddRange(source);
                foreach (int from in source)
                {
                    foreach (int to in Graph.GetNeighbors(from))
                    {
                        --inDegree[to];
                        if (inDegree[to] == 0)
                        {
                            nextSource.Add(to);
                        }
                    }
                }
                source = nextSource;
            }

            return result;
        }

        public int[][] Generate(int size)
        {
            int[][] result = new int[size][];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = [.. this.GenerateOne()];
            }
            return result;
        }
    }
}
