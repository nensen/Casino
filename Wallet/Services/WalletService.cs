using Wallet.DataAccess.Models;
using Wallet.DataAccess.Repositories;

namespace Wallet.Services
{
    public class WalletService : IWalletService
    {
        private readonly IPlayerRepository playerRepository;

        public WalletService(IPlayerRepository playerRepository)
        {
            this.playerRepository = playerRepository;
        }

        public async Task<bool> UpdateTransactionLedger(Guid userId, Transaction transaction)
        {
            var player = await playerRepository.Get(userId) ?? throw new Exception("User does not exist");
            decimal currentvalue = 0;

            for (var node = player.Transactions.First; node != null; node = node.Next)
            {
                if (node.Value.Id == transaction.Id)
                {
                    // Already processed transaction
                    return node.Value.Accepted;
                }

                // Do not account for rejected transactions because they were invalid
                if (node.Value.Accepted)
                {
                    if (node.Value.Type == TransactionType.Win || node.Value.Type == TransactionType.Deposit)
                    {
                        currentvalue += node.Value.Value;
                    }
                    else
                    {
                        currentvalue -= node.Value.Value;
                    }
                }
            }

            transaction.Accepted = (transaction.Type == TransactionType.Win || transaction.Type == TransactionType.Deposit) ||
                (transaction.Type == TransactionType.Stake && currentvalue - transaction.Value >= 0);

            player.Transactions.AddLast(transaction);
            await playerRepository.Update(player);

            return transaction.Accepted;
        }

        public async Task<decimal> GetCurrentBalance(Guid userId)
        {
            var player = await playerRepository.Get(userId) ?? throw new Exception("User does not exist");
            decimal currentvalue = 0;

            for (var node = player.Transactions.First; node != null; node = node.Next)
            {
                if (node.Value.Accepted)
                {
                    if (node.Value.Type == TransactionType.Win || node.Value.Type == TransactionType.Deposit)
                    {
                        currentvalue += node.Value.Value;
                    }
                    else
                    {
                        currentvalue -= node.Value.Value;
                    }
                }
            }

            return currentvalue;
        }
    }
}