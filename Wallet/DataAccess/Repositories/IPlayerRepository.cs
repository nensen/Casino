using Wallet.DataAccess.Models;

namespace Wallet.DataAccess.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player?> Get(Guid id);

        Task<Player> Create(Player player);
        
        Task<Player> Update(Player player);
    }
}