using CryptoFlow.Application.Dtos.OrderDtos;
using CryptoFlow.Application.Exceptions;
using CryptoFlow.Application.Interfaces.ICoin;
using CryptoFlow.Application.Interfaces.ICrypto;
using CryptoFlow.Application.Interfaces.IOrder;
using CryptoFlow.Application.Interfaces.IUnitOfWork;
using CryptoFlow.Application.Interfaces.IWallet;
using CryptoFlow.Application.Interfaces.IWalletItem;
using CryptoFlow.Application.Interfaces.MongoDb;
using CryptoFlow.Application.Services.OrderServices;
using CryptoFlow.Domain.Entities;
using CryptoFlow.Domain.Enums;
using Moq;

namespace CryptoFlow.xUnit;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Transaction>> _transactionRepositoryMock;
    private readonly Mock<ICryptoService> _cryptoServiceMock;
    
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<IWalletItemRepository> _walletItemRepositoryMock;
    private readonly Mock<ICoinRepository> _coinRepositoryMock;
    private readonly Mock<IOrderRepository>  _orderRepositoryMock;
    
    
    private readonly OrderService _orderService;
    public OrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _transactionRepositoryMock = new Mock<IGenericRepository<Transaction>>();
        _cryptoServiceMock = new Mock<ICryptoService>();
        
        
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _walletItemRepositoryMock = new Mock<IWalletItemRepository>();
        _coinRepositoryMock = new Mock<ICoinRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        
        _unitOfWorkMock.Setup(u => u.WalletRepository).Returns(_walletRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.WalletItemRepository).Returns(_walletItemRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.CoinRepository).Returns(_coinRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.OrderRepository).Returns(_orderRepositoryMock.Object);
        
        
        _orderService = new OrderService(_unitOfWorkMock.Object,
            _transactionRepositoryMock.Object,
            _cryptoServiceMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenBuyOrderIsValid()
    {
        var userId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var targetCoinId = Guid.NewGuid();

        var dto = new CreateOrderDto
        {
            PaymentCoinId = paymentId,
            OrderType = OrderType.Buy,
            Quantity = 1,
            TargetCoinId = targetCoinId
        };
        _walletRepositoryMock.Setup(x => x.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(new Wallet { UserId = userId, Id =  walletId });
        _walletItemRepositoryMock.Setup(x => x.GetWalletItemsByWalletIdAsync(walletId))
            .ReturnsAsync(new List<WalletItem>
            {
                new WalletItem { WalletId = walletId, CoinId = paymentId, Balance = 5000000}
            });
        _coinRepositoryMock.Setup(x => x.GetCoinByIdAsync(paymentId))
            .ReturnsAsync(new Coin { Id = paymentId, Symbol = "TRY" });
        _coinRepositoryMock.Setup(x => x.GetCoinByIdAsync(targetCoinId))
            .ReturnsAsync(new Coin { Id = targetCoinId, Symbol = "BTC" });
        _cryptoServiceMock.Setup(x => x.GetLivePricesAsync())
            .ReturnsAsync(new Dictionary<string, decimal>
            {
                {"TRY", 1m},
                {"BTC", 2000000m},
            });
        await _orderService.CreateOrderAsync(userId, dto);
        
        _orderRepositoryMock.Verify(o => o.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
        _transactionRepositoryMock.Verify(t => t.CreateAsync(It.IsAny<Transaction>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenSellOrderIsValid()
    {
        var userId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var targetCoinId = Guid.NewGuid();

        var dto = new CreateOrderDto
        {
            PaymentCoinId = paymentId,
            OrderType = OrderType.Sell,
            Quantity = 0.25m,
            TargetCoinId = targetCoinId
        };
        _walletRepositoryMock.Setup(x => x.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(new Wallet { UserId = userId, Id = walletId});
        
        _walletItemRepositoryMock.Setup(x => x.GetWalletItemsByWalletIdAsync(walletId))
            .ReturnsAsync(new List<WalletItem>
            {
                new WalletItem { WalletId = walletId, CoinId = targetCoinId, Balance = 1}
            });
        
        _coinRepositoryMock.Setup(x => x.GetCoinByIdAsync(paymentId))
            .ReturnsAsync(new Coin { Id = paymentId, Symbol = "TRY" });
        
        _coinRepositoryMock.Setup(x => x.GetCoinByIdAsync(targetCoinId))
            .ReturnsAsync(new Coin { Id = targetCoinId, Symbol = "BTC" });
        
        _cryptoServiceMock.Setup(x => x.GetLivePricesAsync())
            .ReturnsAsync(new Dictionary<string, decimal>
            {
                {"TRY", 1m},
                {"BTC", 2000000m},
            });
        await _orderService.CreateOrderAsync(userId, dto);
        
        _orderRepositoryMock.Verify(o => o.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
        _transactionRepositoryMock.Verify(t => t.CreateAsync(It.IsAny<Transaction>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    
    
    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenPaymentAndTargetCoinIsSame()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var paymentCoinId = Guid.NewGuid();
        var targetCoinId = paymentCoinId;

        var dto = new CreateOrderDto
        {
            OrderType = OrderType.Sell,
            Quantity = 2000m,
            TargetCoinId = targetCoinId,
            PaymentCoinId = paymentCoinId
        };
        
        _coinRepositoryMock.Setup(u => u.GetCoinByIdAsync(paymentCoinId))
            .ReturnsAsync(new Coin { Id = paymentCoinId, Symbol = "TRY" });
        _coinRepositoryMock.Setup(u => u.GetCoinByIdAsync(targetCoinId))
            .ReturnsAsync(new Coin { Id = targetCoinId, Symbol = "TRY" });
        _walletRepositoryMock.Setup(x => x.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(new  Wallet { UserId = userId, Id = walletId });
        _walletItemRepositoryMock.Setup(x => x.GetWalletItemsByWalletIdAsync(walletId))
            .ReturnsAsync(new List<WalletItem>
            {
                new WalletItem { WalletId = walletId, CoinId = paymentCoinId, Balance = 10000 }
            });
        _cryptoServiceMock.Setup(x => x.GetLivePricesAsync())
            .ReturnsAsync(new Dictionary<string, decimal>
            {
                { "TRY", 1m }
            });
        
        await Assert.ThrowsAsync<WrongInputException>(async () => await _orderService.CreateOrderAsync(userId, dto));
        
        _transactionRepositoryMock.Verify(o => o.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Never);
        _orderRepositoryMock.Verify(o => o.CreateOrderAsync(It.IsAny<Order>()), Times.Never);
        
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenWalletIsNotFound()
    {
        var userId = Guid.NewGuid();
    
        var dto = new CreateOrderDto
        {
            OrderType = OrderType.Buy,
            Quantity = 10m,
            TargetCoinId = Guid.NewGuid(),
            PaymentCoinId = Guid.NewGuid()
        };
        _walletRepositoryMock.Setup(x => x.GetWalletByUserIdAsync(userId))
            .ReturnsAsync((Wallet?)null);

        await Assert.ThrowsAsync<NotFoundException>(async () => 
            await _orderService.CreateOrderAsync(userId, dto));

        _orderRepositoryMock.Verify(o => o.CreateOrderAsync(It.IsAny<Order>()), Times.Never);
        _transactionRepositoryMock.Verify(t => t.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenPaymentFailed()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var paymentCoinId = Guid.NewGuid();
        var targetCoinId = Guid.NewGuid();

        var dto = new CreateOrderDto
        {
            OrderType = OrderType.Buy,
            Quantity = 5m,
            TargetCoinId = targetCoinId,
            PaymentCoinId = paymentCoinId
        };

        _walletRepositoryMock.Setup(x => x.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(new Wallet { Id = walletId, UserId = userId });
        _coinRepositoryMock.Setup(x => x.GetCoinByIdAsync(paymentCoinId))
            .ReturnsAsync(new Coin { Id = paymentCoinId, Symbol = "TRY" });
        _coinRepositoryMock.Setup(x => x.GetCoinByIdAsync(targetCoinId))
            .ReturnsAsync(new Coin { Id = targetCoinId, Symbol = "BTC" });

        _walletItemRepositoryMock.Setup(x => x.GetWalletItemsByWalletIdAsync(walletId))
            .ReturnsAsync(new List<WalletItem>
            {
                new WalletItem { WalletId = walletId, CoinId = paymentCoinId, Balance = 10m } 
            });

        _cryptoServiceMock.Setup(x => x.GetLivePricesAsync())
            .ReturnsAsync(new Dictionary<string, decimal>
            {
                { "TRY", 1m },
                { "BTC", 1000m }
            });

        await Assert.ThrowsAsync<WrongInputException>(async () => 
            await _orderService.CreateOrderAsync(userId, dto));

        _orderRepositoryMock.Verify(o => o.CreateOrderAsync(It.IsAny<Order>()), Times.Never);
        _transactionRepositoryMock.Verify(t => t.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }
    
    
}