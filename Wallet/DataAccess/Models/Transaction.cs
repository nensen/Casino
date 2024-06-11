namespace Wallet.DataAccess.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public TransactionType Type { get; set; }

        public decimal Value { get; set; }

        public bool Accepted { get; set; }
    }
}