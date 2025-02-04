namespace Seven.Core
{
    public interface IRandom
    {
        int Next(int maxValue);
    }

    public class StandardRandom : IRandom
    {
        private readonly Random random = new();
        public int Next(int maxValue)
        {
            return this.random.Next(maxValue);
        }
    }
}
