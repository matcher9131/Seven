namespace Seven.Core
{
    public interface IRandom
    {
        int Next(int maxValue);
        double NextDouble();
    }

    public class StandardRandom : IRandom
    {
        private readonly Random random = new();
        public int Next(int maxValue)
        {
            return this.random.Next(maxValue);
        }

        public double NextDouble()
        {
            return this.random.NextDouble();
        }
    }
}
