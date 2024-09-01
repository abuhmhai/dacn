using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;


namespace FontWeb.Service
{
    public class Crypto
    {
        private readonly HttpClient _httpClient;

        public Crypto(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BitcoinData>> GetSolanaPrice()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest"),
                Headers = {
        { "X-CMC_PRO_API_KEY", "bbd1d838-7f76-4daa-899a-2b4807347fe3" }, // Thay bằng API key của bạn
        { "Accept", "application/json" }
    }
            };

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var solanaData = jsonDoc.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Where(item => item.GetProperty("symbol").GetString() == "SOL") // Lọc dữ liệu Solana
                    .Select(item => new BitcoinData
                    {
                        Timestamp = DateTime.Now, // Cần có thông tin thời gian thật sự từ API
                        Price = item.GetProperty("quote").GetProperty("USD").GetProperty("price").GetDecimal()
                    })
                    .ToList();

                // In ra console thông tin về mã SOL
                foreach (var data in solanaData)
                {
                    Console.WriteLine($"Timestamp: {data.Timestamp}, Price: {data.Price}");
                }

                return solanaData;
            }
            else
            {
                throw new HttpRequestException("Không thể lấy dữ liệu từ API.");
            }
        }

    }
    public class BitcoinData
    {
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
    }

}
