using Casino.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Wallet.DataAccess.Models;
using Wallet.DataAccess.Repositories;
using Wallet.Services;

namespace Casino.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IPlayerRepository playerRepository;
        private readonly IWalletService walletService;

        public WalletController(
            IPlayerRepository playerRepository,
            IWalletService walletService)
        {
            this.playerRepository = playerRepository;
            this.walletService = walletService;
        }

        [HttpPost("{userId}/register")]
        public async Task<IActionResult> Register(Guid userId)
        {
            if ((await playerRepository.Get(userId)) != null)
            {
                return StatusCode(403, "User already exists");
            }

            await playerRepository.Create(new Player { Id = userId });
            return Ok();
        }

        [HttpGet("{userId}/balance")]
        public async Task<IActionResult> GetBalance(Guid userId)
        {
            var player = await playerRepository.Get(userId);

            if (player == null)
            {
                return StatusCode(404, "User does not exists");
            }

            return Ok(await walletService.GetCurrentBalance(userId));
        }

        [HttpPost("{userId}/transactions")]
        public async Task<IActionResult> CreditTransaction(Guid userId, [FromBody] TransactionViewModel transaction)
        {
            var player = await playerRepository.Get(userId);

            if (player == null)
            {
                return StatusCode(404, "User does not exists");
            }

            var accepted = await walletService.UpdateTransactionLedger(userId, new Transaction
            {
                Id = transaction.Id,
                Type = transaction.Type,
                Value = transaction.Value,
            });

            return accepted ? Ok("Accepted") : BadRequest("Rejected");
        }

        [HttpGet("{userId}/transactions")]
        public async Task<IActionResult> GetTransactions(Guid userId)
        {
            var player = await playerRepository.Get(userId);

            if (player == null)
            {
                return StatusCode(404, "User does not exists");
            }

            return Ok(new { transactions = player.Transactions.ToList() });
        }
    }
}