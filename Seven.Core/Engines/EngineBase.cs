using Seven.Core.Models;

namespace Seven.Core.Engines
{
    public interface IEngine
    {
        int Next(IReadonlyGame game, IReadonlyPlayer player);
    }

    public abstract class EngineBase() : IEngine
    {
        public abstract int Next(IReadonlyGame game, IReadonlyPlayer player);
    }
}
