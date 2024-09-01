using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using IronPython.Runtime;

namespace FontWeb.Service
{
    public class CryptoData
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public long CmcRank { get; set; }
        public decimal Price { get; set; }
        public decimal Change1 { get; set; }
        public decimal Change24 { get; set; }
        public decimal Change7 { get; set; }
        public decimal MarketCap { get; set; }
        public decimal Volume24 { get; set; }
        public long MarketPairs { get; set; }
        public long CirculatingSupply { get; set; }
        public long MaxSupply { get; set; }
        public long TotalSupply { get; set; }
        public decimal MarketCapDominance { get; set; }

    }

    public class CryptoService
    {
        private readonly HttpClient _client;

        public CryptoService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<CryptoData>> GetCryptoDataByIdAsync(List<int> ids)
        {
            string url = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest";
            string apiKey = "bbd1d838-7f76-4daa-899a-2b4807347fe3"; // Thay thế YOUR_API_KEY bằng API key thực sự của bạn

            var parameters = new
            {
                start = "1",
                limit = "5000",
                convert = "USD"
            };

            _client.DefaultRequestHeaders.Clear(); // Xóa các header cũ trước khi thêm mới
            _client.DefaultRequestHeaders.Add("Accepts", "application/json");
            _client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);

            var response = await _client.GetAsync($"{url}?start={parameters.start}&limit={parameters.limit}&convert={parameters.convert}");

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(responseString);

                if (data.ContainsKey("data"))
                {
                    JArray cryptos = (JArray)data["data"];
                    var result = new List<CryptoData>();

                    foreach (var crypto in cryptos)
                    {
                        var id = crypto["id"]?.ToObject<int>();
                        if (id != null && ids.Contains(id.Value))
                        {
                            var quote = crypto["quote"]?["USD"];

                            if (quote != null)
                            {
                                result.Add(new CryptoData
                                {
                                    Symbol = crypto["symbol"]?.ToString(),
                                    Name = crypto["name"]?.ToString(),
                                    Price = quote["price"]?.ToObject<decimal>() ?? 0,
                                    Change1 = quote["percent_change_1h"]?.ToObject<decimal>() ?? 0,
                                    Change24 = quote["percent_change_24h"]?.ToObject<decimal>() ?? 0,
                                    Change7 = quote["percent_change_7d"]?.ToObject<decimal>() ?? 0,
                                    MarketCap = quote["market_cap"]?.ToObject<decimal>() ?? 0,
                                    Volume24 = quote["volume_24h"]?.ToObject<decimal>() ?? 0,
                                    CirculatingSupply = crypto["circulating_supply"]?.ToObject<long>() ?? 0
                                });
                            }
                        }
                    }

                    return result;
                }
            }
            else
            {
                Console.WriteLine($"Lỗi HTTP: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            return new List<CryptoData>();
        }

        public async Task<CryptoData> GetCryptoDataBySymbolAsync(string symbol)
        {
            string url = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest";
            string apiKey = "bbd1d838-7f76-4daa-899a-2b4807347fe3"; // Thay thế YOUR_API_KEY bằng API key thực sự của bạn

            var parameters = new
            {
                start = "1",
                limit = "5000",
                convert = "USD"
            };

            _client.DefaultRequestHeaders.Clear(); // Xóa các header cũ trước khi thêm mới
            _client.DefaultRequestHeaders.Add("Accepts", "application/json");
            _client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);

            var response = await _client.GetAsync($"{url}?start={parameters.start}&limit={parameters.limit}&convert={parameters.convert}");

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(responseString);

                if (data.ContainsKey("data"))
                {
                    JArray cryptos = (JArray)data["data"];

                    foreach (var crypto in cryptos)
                    {
                        if (crypto["symbol"]?.ToString() == symbol.ToUpper())
                        {
                            var quote = crypto["quote"]?["USD"];

                            if (quote != null)
                            {
                                var cryptoData = new CryptoData
                                {
                                    Symbol = crypto["symbol"]?.ToString(),
                                    Name = crypto["name"]?.ToString(),
                                    CmcRank = crypto["cmc_rank"]?.ToObject<long>() ?? 0,
                                    Price = quote["price"]?.ToObject<decimal>() ?? 0,
                                    Change1 = quote["percent_change_1h"]?.ToObject<decimal>() ?? 0,
                                    Change24 = quote["percent_change_24h"]?.ToObject<decimal>() ?? 0,
                                    Change7 = quote["percent_change_7d"]?.ToObject<decimal>() ?? 0,
                                    MarketCap = quote["market_cap"]?.ToObject<decimal>() ?? 0,
                                    Volume24 = quote["volume_24h"]?.ToObject<decimal>() ?? 0,
                                    MarketPairs = crypto["num_market_pairs"]?.ToObject<long>() ?? 0,
                                    CirculatingSupply = crypto["circulating_supply"]?.ToObject<long>() ?? 0,
                                    MaxSupply = crypto["max_supply"]?.ToObject<long>() ?? 0,
                                    TotalSupply = crypto["total_supply"]?.ToObject<long>() ?? 0,
                                    MarketCapDominance = quote["market_cap_dominance"]?.ToObject<decimal>() ?? 0
                                };
                                // In dữ liệu
                                Console.WriteLine($"Symbol: {cryptoData.CmcRank}");
                                Console.WriteLine($"Symbol: {cryptoData.MarketPairs}");
                                Console.WriteLine($"Symbol: {cryptoData.Symbol}");
                                Console.WriteLine($"Name: {cryptoData.Name}");
                                Console.WriteLine($"Price: {cryptoData.Price:C}");
                                Console.WriteLine($"Change (1h): {cryptoData.Change1}%");
                                Console.WriteLine($"Change (24h): {cryptoData.Change24}%");
                                Console.WriteLine($"Change (7d): {cryptoData.Change7}%");
                                Console.WriteLine($"Market Cap: {cryptoData.MarketCap:C}");
                                Console.WriteLine($"Volume (24h): {cryptoData.Volume24:C}");
                                Console.WriteLine($"Circulating Supply: {cryptoData.CirculatingSupply}");
                                Console.WriteLine($"Max Supply: {cryptoData.MaxSupply}");
                                Console.WriteLine($"Total Supply: {cryptoData.TotalSupply}");
                                Console.WriteLine($"Market Cap Dominance: {cryptoData.MarketCapDominance}%");

                                return cryptoData;
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Lỗi HTTP: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            return null;
        }


    }
}
