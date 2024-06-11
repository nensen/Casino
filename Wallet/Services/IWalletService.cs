using Wallet.DataAccess.Models;

namespace Wallet.Services
{
    public interface IWalletService
    {
        Task<bool> UpdateTransactionLedger(Guid userId, Transaction transaction);

        Task<decimal> GetCurrentBalance(Guid userId);
    }
}