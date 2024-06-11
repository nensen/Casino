using Wallet.DataAccess.Models;

namespace Wallet.DataAccess.Repositories.Mock
{
    public class PlayerMockRepository : IPlayerRepository
    {
        public async Task<Player?> Get(Guid id)
        {
            return MemoryStorage.Players.SingleOrDefault(p => p.Id == id);
        }

        public async Task<Player> Create(Player player)
        {
            MemoryStorage.Players.Add(player);
            return player;
        }

        public async Task<Player> Update(Player player)
        {
            int index = MemoryStorage.Players.FindIndex(p => p.Id == player.Id);
            MemoryStorage.Players[index] = player;
            return player;
        }
    }
}