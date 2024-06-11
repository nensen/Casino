using System.ComponentModel.DataAnnotations;
using Wallet.DataAccess.Models;

namespace Casino.ViewModels
{
    public class TransactionViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [EnumDataType(typeof(TransactionType))]
        public TransactionType Type { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value out of range")]
        public decimal Value { get; set; }
    }
}