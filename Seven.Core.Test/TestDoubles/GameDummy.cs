using Seven.Core.Models;
using Seven.Core.Rules;

namespace Seven.Core.Test.TestDoubles
{
    public class GameDummy : IReadonlyGame
    {
        public IReadonlyBoard Board => throw new NotImplementedException();

        public IEnumerable<IOtherPlayer> Players => throw new NotImplementedException();

        public Rule Rule => throw new NotImplementedException();

        public void PlayerWin(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public void PlayerLose(IPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
