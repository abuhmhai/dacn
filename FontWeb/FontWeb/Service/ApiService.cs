namespace FontWeb.Service
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<StockDats>> GetStockData()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/StockDataService");
                response.EnsureSuccessStatusCode(); // Ensure request was successful
                return await response.Content.ReadFromJsonAsync<List<StockDats>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<StockDats>();
            }
        }
    }
    public class StockDats
    {
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }
}
