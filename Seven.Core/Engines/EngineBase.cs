using Seven.Core.Models;

namespace Seven.Core.Engines
{
    public interface IEngine
    {
        string Name { get; }
        int Next(IReadonlyGame game, IPlayer player);
    }

    public abstract class EngineBase(string engineName) : IEngine
    {
        public string Name { get; } = engineName;
        public abstract int Next(IReadonlyGame game, IPlayer player);
    }
}
