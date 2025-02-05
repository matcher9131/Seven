using Seven.Core.Engines;
using Seven.Core.Models;

namespace Seven.Core.Test.TestDoubles
{
    public class EngineStub : EngineBase
    {
        public EngineStub() : base("EngineStub")
        {   
        }

        public int NextCard { get; set; }

        public override int Next(IReadonlyGame game, IReadonlyPlayer player)
        {
            return this.NextCard;
        }
    }
}
