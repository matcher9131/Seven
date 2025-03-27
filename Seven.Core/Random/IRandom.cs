namespace Seven.Core.Random
{
    public interface IRandom
    {
        uint Next(uint maxValue);
        double NextDouble();
        void ShuffleList<T>(IList<T> list);
    }
}
