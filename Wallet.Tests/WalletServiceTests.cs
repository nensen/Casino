using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Moq;
using Wallet.DataAccess.Models;
using Wallet.DataAccess.Repositories;
using Wallet.Services;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Wallet.Tests
{
    public class WalletServiceTests
    {
        private readonly Mock<IPlayerRepository> mockPlayerRepository;
        private readonly WalletService walletService;

        public WalletServiceTests()
        {
            mockPlayerRepository = new Mock<IPlayerRepository>();
            walletService = new WalletService(mockPlayerRepository.Object);
        }

        public static IEnumerable<object[]> GetBalanceTestData()
        {
            yield return new object[]
            {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Deposit },
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 50, Type = TransactionType.Stake },
                        new Transaction { Id = Guid.NewGuid(), Accepted = false, Value = 200, Type = TransactionType.Deposit }
                    })
                },
                50m
            };

            yield return new object[]
            {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 200, Type = TransactionType.Deposit },
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Win }
                    })
                },
                300m
            };

            yield return new object[]
            {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 150, Type = TransactionType.Deposit },
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 50, Type = TransactionType.Stake },
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Win },
                        new Transaction { Id = Guid.NewGuid(), Accepted = false, Value = 100, Type = TransactionType.Stake }
                    })
                },
                200m
            };

            yield return new object[]
          {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 150, Type = TransactionType.Deposit },
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 50, Type = TransactionType.Stake },
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Win },
                        new Transaction { Id = Guid.NewGuid(), Accepted = false, Value = 100, Type = TransactionType.Stake }
                    })
                },
                200m
          };
        }

        [Theory]
        [MemberData(nameof(GetBalanceTestData))]
        public async Task GetCurrentBalance_CalculatesBalanceCorrectly(Player player, decimal expectedBalance)
        {
            // Arrange
            var userId = Guid.NewGuid();
            mockPlayerRepository.Setup(repo => repo.Get(userId)).ReturnsAsync(player);

            // Act
            var balance = await walletService.GetCurrentBalance(userId);

            // Assert
            Assert.Equal(expectedBalance, balance);
        }

        public static IEnumerable<object[]> GetBalanceTestDataForUpdateLedger()
        {
            yield return new object[]
            {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 200, Type = TransactionType.Deposit },
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Stake },
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Win }
                    })
                },
                new Transaction { Id = Guid.NewGuid(), Value = 100, Type = TransactionType.Win },
                300,
            };

            yield return new object[]
            {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Deposit },
                    })
                },
                new Transaction { Id = Guid.NewGuid(), Value = 101, Type = TransactionType.Stake },
                100,
            };

            yield return new object[]
            {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 200, Type = TransactionType.Deposit },
                        new Transaction { Id = Guid.Parse("48f551ca-eb56-4131-a5cd-c1d29b21d9ae"), Accepted = true, Value = 100, Type = TransactionType.Stake },
                    })
                },
                new Transaction { Id = Guid.Parse("48f551ca-eb56-4131-a5cd-c1d29b21d9ae"), Value = 100, Type = TransactionType.Stake },
                100,
            };

            yield return new object[]
            {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 200, Type = TransactionType.Deposit },
                        new Transaction { Id = Guid.Parse("48f551ca-eb56-4131-a5cd-c1d29b21d9ae"), Accepted = true, Value = 100, Type = TransactionType.Stake },
                    })
                },
                new Transaction { Id = Guid.Parse("48f551ca-eb56-4131-a5cd-c1d29b21d9ae"), Value = 100, Type = TransactionType.Stake },
                100,
            };

            yield return new object[]
           {
                new Player
                {
                    Transactions = new LinkedList<Transaction>(new[]
                    {
                        new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Deposit },
                        new Transaction { Id = Guid.Parse("48f551ca-eb56-4131-a5cd-c1d29b21d9ae"), Accepted = false, Value = 200, Type = TransactionType.Stake },
                        new Transaction { Id = Guid.Parse("2a31ce3f-b789-4c2d-ab8d-8945a2e6c299"), Accepted = true, Value = 300, Type = TransactionType.Win },
                        new Transaction { Id = Guid.Parse("48f551ca-eb56-4131-a5cd-c1d29b21d9ae"), Accepted = false, Value = 200, Type = TransactionType.Stake },
                        new Transaction { Id = Guid.Parse("2a31ce3f-b789-4c2d-ab8d-8945a2e6c299"), Accepted = false, Value = 300, Type = TransactionType.Win },
                    })
                },
                new Transaction { Id = Guid.NewGuid(), Value = 100, Type = TransactionType.Win },
                500,
           };
        }

        [Theory]
        [MemberData(nameof(GetBalanceTestDataForUpdateLedger))]
        public async Task UpdateTransactionLedger_PositiveBalance_ReturnBalanceCorrectly(Player player, Transaction transaction, decimal expectedBalance)
        {
            // Arrange
            var userId = Guid.NewGuid();
            mockPlayerRepository.Setup(repo => repo.Get(userId)).ReturnsAsync(player);

            // Act
            var result = await walletService.UpdateTransactionLedger(userId, transaction);
            var balance = await walletService.GetCurrentBalance(userId);

            // Assert
            Assert.Equal(balance, expectedBalance);
        }

        [Fact]
        public async Task UpdateTransactionLedger_NegativeBalance_ReturnBalanceCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var transaction = new Transaction { Id = Guid.NewGuid(), Value = 200, Type = TransactionType.Stake };

            var player = new Player
            {
                Transactions = new LinkedList<Transaction>(new[]
                {
                    new Transaction { Id = Guid.NewGuid(), Accepted = true, Value = 100, Type = TransactionType.Deposit },
                })
            };

            mockPlayerRepository.Setup(repo => repo.Get(userId)).ReturnsAsync(player);

            // Act
            var result = await walletService.UpdateTransactionLedger(userId, transaction);
            var balance = await walletService.GetCurrentBalance(userId);

            // Assert
            Assert.False(result);
            Assert.Equal(100, balance);
        }

        [Fact]
        public async Task GetCurrentBalance_UserDoesNotExist_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            mockPlayerRepository.Setup(repo => repo.Get(userId)).ReturnsAsync((Player)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => walletService.GetCurrentBalance(userId));
            Assert.Equal("User does not exist", exception.Message);
        }

        [Fact]
        public async Task UpdateTransactionLedger_UserDoesNotExist_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var transaction = new Transaction { Id = Guid.NewGuid(), Value = 100, Type = TransactionType.Deposit };

            mockPlayerRepository.Setup(repo => repo.Get(userId)).ReturnsAsync((Player)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => walletService.UpdateTransactionLedger(userId, transaction));
            Assert.Equal("User does not exist", exception.Message);
        }
    }
}