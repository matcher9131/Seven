namespace Seven.Core.Test.TestDoubles
{
    public class FixedRandom : IRandom
    {
        public int ReturnValue { get; set; }

        public int Next(int maxValue) => this.ReturnValue;
    }
}
