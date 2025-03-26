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

            while (inDegree.Count > 0)
            {
                List<int> nextSource = [];
                source.Shuffle(this.random);
                result.AddRange(source);
                foreach (int from in source)
                {
                    inDegree.Remove(from);
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

        public List<int[]> Generate(int size)
        {
            List<int[]> result = new(size);
            for (int i = 0; i < size; ++i)
            {
                result.Add([.. this.GenerateOne()]);
            }
            return result;
        }
    }
}
