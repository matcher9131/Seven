using Seven.Core.Models;
using Seven.Core.Random;

namespace Seven.Core.Engines
{
    public interface IEngine
    {
        void SetRandom(IRandom random);
        int Next(IReadonlyGame game, IReadonlyPlayer player);
    }
}
