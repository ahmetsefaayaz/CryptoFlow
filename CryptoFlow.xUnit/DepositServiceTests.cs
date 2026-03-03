using CryptoFlow.Application.Dtos.DepositDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.ICoin;
using CryptoFlow.Application.Interfaces.ICrypto;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.IWallet;
using CryptoFlow.Application.Interfaces.IWalletItem;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Application.Services.WalletItemServices;
using CryptoFlow.Domain.Entities;
using Moq;

namespace CryptoFlow.xUnit;

public class DepositServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Transaction>> _transactionRepositoryMock;
    private readonly Mock<ICryptoService>  _cryptoServiceMock;
    
    private readonly Mock<ICoinRepository> _coinRepositoryMock;
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<IWalletItemRepository> _walletItemRepositoryMock;
    
    private readonly IDepositService _depositService;

    public DepositServiceTests()
    {
        _transactionRepositoryMock = new Mock<IGenericRepository<Transaction>>();
        _cryptoServiceMock = new Mock<ICryptoService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _coinRepositoryMock = new Mock<ICoinRepository>();
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _walletItemRepositoryMock = new Mock<IWalletItemRepository>();
        
        _depositService = new DepositService(
            _unitOfWorkMock.Object,
            _transactionRepositoryMock.Object,
            _cryptoServiceMock.Object);
        
        _unitOfWorkMock.Setup(x => x.CoinRepository).Returns(_coinRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.WalletRepository).Returns(_walletRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.WalletItemRepository).Returns(_walletItemRepositoryMock.Object);
    }

    [Fact]
    public async Task DepositAsync_ShouldDeposit_WhenInputIsValid()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var coinId = Guid.NewGuid();
        var dto = new DepositDto
        {
            CoinId = coinId,
            Amount = 1000
        };
        _walletRepositoryMock.Setup(c => c.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(new Wallet{Id = walletId, UserId =  userId, WalletItems =  new List<WalletItem>()});
        _coinRepositoryMock.Setup(c => c.GetCoinByIdAsync(coinId))
            .ReturnsAsync(new Coin{Id = coinId, Symbol = "TRY"});
        _cryptoServiceMock.Setup(c => c.GetLivePricesAsync())
            .ReturnsAsync(new Dictionary<string, decimal>
            {
                {"TRY", 1m}
            });
        await _depositService.DepositAsync(userId, dto);
        _transactionRepositoryMock.Verify(t => t.CreateAsync(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _walletItemRepositoryMock.Verify(x => x.CreateWalletItemAsync(It.IsAny<WalletItem>()), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldWithdraw_WhenInputIsValid()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var coinId = Guid.NewGuid();
        var dto = new WithdrawDto
        {
            CoinId = coinId,
            Amount = 1000
        };
        _coinRepositoryMock.Setup(c => c.GetCoinByIdAsync(coinId))
            .ReturnsAsync(new Coin{Id = coinId, Symbol = "TRY"});
        
        _walletRepositoryMock.Setup(c => c.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(new Wallet{Id = walletId, UserId =  userId,  WalletItems =  new List<WalletItem>
            {
                new WalletItem{CoinId = coinId, Balance = 10000, Id =  Guid.NewGuid()}
            }});
        _cryptoServiceMock.Setup(c => c.GetLivePricesAsync())
            .ReturnsAsync(new Dictionary<string, decimal>
            {
                { "TRY", 1m }
            });
        await _depositService.WithdrawAsync(userId, dto);
        _transactionRepositoryMock.Verify(t => t.CreateAsync(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        



    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrowException_WhenBalanceIsNotEnough()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var coinId = Guid.NewGuid();
        var dto = new WithdrawDto
        {
            CoinId = coinId,
            Amount = 1000
        };
        _coinRepositoryMock.Setup(c => c.GetCoinByIdAsync(coinId))
            .ReturnsAsync(new Coin{Id = coinId, Symbol = "TRY"});
        
        _walletRepositoryMock.Setup(c => c.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(new Wallet{Id = walletId, UserId =  userId,  WalletItems =  new List<WalletItem>
            {
                new WalletItem{CoinId = coinId, Balance = 100, Id =  Guid.NewGuid()}
            }});
        _cryptoServiceMock.Setup(c => c.GetLivePricesAsync())
            .ReturnsAsync(new Dictionary<string, decimal>
            {
                { "TRY", 1m }
            });
        await Assert.ThrowsAsync<WrongInputException>(async () => await _depositService.WithdrawAsync(userId, dto));
        
        _transactionRepositoryMock.Verify(t => t.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);

    }
    
    
}