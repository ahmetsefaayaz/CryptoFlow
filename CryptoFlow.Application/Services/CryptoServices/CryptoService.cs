using System.Text.Json;
using CryptoFlow.Application.Dtos.CoinDtos;
using CryptoFlow.Application.Interfaces.ICrypto;
using Microsoft.Extensions.Caching.Distributed;

namespace CryptoFlow.Application.Services.CryptoServices;

public class CryptoService: ICryptoService
{
    private readonly IDistributedCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string cacheKey = "CryptoLivePrices";
    
    public CryptoService(IDistributedCache cache, IHttpClientFactory httpClientFactory)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
    }
    
    
    public async Task<Dictionary<string, decimal>> GetLivePricesAsync()
    {
        var cachedPrices = await _cache.GetStringAsync(cacheKey);
        if(!string.IsNullOrEmpty(cachedPrices))
            return JsonSerializer.Deserialize<Dictionary<string, decimal>>(cachedPrices);
        
        var client = _httpClientFactory.CreateClient("CoinGecko");
        var response = await client.GetAsync("simple/price?ids=bitcoin,ethereum,solana,tether&vs_currencies=try");
        response.EnsureSuccessStatusCode();
        var apiContent = await response.Content.ReadAsStringAsync();
        var parsedData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(apiContent);
        var idToSymbolMap = new Dictionary<string, string>
        {
            { "bitcoin", "BTC" },
            { "ethereum", "ETH" },
            { "solana", "SOL" },
            { "tether", "USD" }
        };
        var livePrices = new Dictionary<string, decimal>();
        livePrices.Add("TRY", 1.0m);
        foreach (var coin in parsedData)
        {
            if(idToSymbolMap.TryGetValue(coin.Key, out var mySymbol))
            {
                livePrices.Add(mySymbol, coin.Value["try"]);
            }
        
        }
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(livePrices), cacheOptions);
        return livePrices;
        

    }
}