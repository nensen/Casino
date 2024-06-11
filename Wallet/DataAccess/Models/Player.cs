using System.Net.Http.Headers;

namespace Wallet.DataAccess.Models
{
    public class Player
    {
        public Player()
        {
            Transactions = new LinkedList<Transaction>();
        }

        public Guid Id { get; set; }

        public LinkedList<Transaction> Transactions { get; set; }
    }
}