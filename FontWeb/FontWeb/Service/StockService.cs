using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FontWeb.Service
{
    public class StockDat
    {
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public double Volume { get; set; }
        public DateTime Date { get; set; }
    }

    public class StockService
    {
        // Static constructor to ensure Python is initialized only once
        static StockService()
        {
            Runtime.PythonDLL = @"C:\Users\phanp\AppData\Local\Programs\Python\Python312\python312.dll";
            PythonEngine.Initialize();
        }

        public List<StockDat> GetStockData()
        {
            // Khởi tạo danh sách để lưu dữ liệu
            List<StockDat> stockDataList = new List<StockDat>();

            try
            {
                using (Py.GIL())
                {
                    // Import yfinance và lấy dữ liệu
                    dynamic yf = Py.Import("yfinance");
                    dynamic data = yf.Ticker("BTC-USD");
                    dynamic hist = data.history(period: "1mo");

                    // Lặp qua các hàng trong DataFrame lịch sử
                    foreach (dynamic row in hist.itertuples())
                    {
                        StockDat stockData = new StockDat
                        {
                            Date = DateTime.Parse(row.Index.ToString()),
                            Open = (double)row.Open,
                            Low = (double)row.Low,
                            Close = (double)row.Close,
                            High = (double)row.High,
                            Volume = (double)row.Volume
                        };

                        stockDataList.Add(stockData);
                        // In dữ liệu ra console
                        Console.WriteLine($"Date: {stockData.Date}, Open: {stockData.Open}, High: {stockData.High}, Low: {stockData.Low}, Close: {stockData.Close}, Volume: {stockData.Volume}");
                    }
                }
            }
            catch (PythonException ex)
            {
                Console.WriteLine($"Python error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return stockDataList;
        }

        ~StockService()
        {
            PythonEngine.Shutdown();
        }
    }

    //public List<StockDat> Candlestick { get; set; }
    //private readonly HttpClient _httpClient;

    //public StockService(HttpClient httpClient)
    //{
    //    _httpClient = httpClient;
    //}

    //public async Task<List<StockDat>> GetStockData(string ticker, string interval, int emaPeriod, int rsiPeriod)
    //{
    //    var url = $"api/data/{ticker}/{interval}/{emaPeriod}/{rsiPeriod}";
    //    var response = await _httpClient.GetFromJsonAsync<List<StockDat>>(url);
    //    return response;
    //}
}

