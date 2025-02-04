using Seven.Core.Models;
using Seven.Core.Rules;

namespace Seven.Core.Test.TestDoubles
{
    public class GameDummy : IReadonlyGame
    {
        public IReadonlyBoard Board => throw new NotImplementedException();

        public IEnumerable<IReadonlyPlayer> Players => throw new NotImplementedException();

        public Rule Rule => throw new NotImplementedException();

        public void PlayerWin(Player player)
        {
            throw new NotImplementedException();
        }

        public void PlayerLose(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
