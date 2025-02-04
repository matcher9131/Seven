using Seven.Core.Models;

namespace Seven.Core.Test.TestDoubles
{
    public class BoardStub(ulong cards) : IReadonlyBoard
    {
        public ulong Cards { get; } = cards;
    }
}
