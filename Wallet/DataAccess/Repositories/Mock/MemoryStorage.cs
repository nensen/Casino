using Wallet.DataAccess.Models;

namespace Wallet.DataAccess.Repositories.Mock
{
    internal static class MemoryStorage
    {
        public static readonly List<Player> Players = new List<Player>();
    }
}