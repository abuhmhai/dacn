using Python.Runtime;
using System;
using System.Collections.Generic;

namespace FontWeb.Service
{
    public class StockData
    {
        public string Ticker { get; set; }
        public string Company_name { get; set; }
        public double? Last_price { get; set; }
        public double? open { get; set; }
        public double? DayLow { get; set; }
        public double? DayHigh { get; set; }
        public double? trailingEps { get; set; }
        public double? forwardEps { get; set; }
        public double? Change { get; set; }
        public double? Per_change { get; set; }
        public double? Volume { get; set; } // Thay đổi thành double?
        public double? Market_cap { get; set; } // Thay đổi thành double?

        public string FormattedChange => Change.HasValue ? Change.Value.ToString("F2") : "N/A";
        public string FormattedPerChange => Per_change.HasValue ? Per_change.Value.ToString("F2") : "N/A";
        public string FormattedVolume => Volume.HasValue ? FormatNumber(Volume.Value) : "N/A";
        public string FormattedMarketCap => Market_cap.HasValue ? FormatNumber(Market_cap.Value) : "N/A";
        public string FormattedOpen => open.HasValue ? open.Value.ToString("F2") : "N/A";
        public string FormattedTrailingEps => trailingEps.HasValue ? trailingEps.Value.ToString("F3") : "N/A";
        public string FormattedForwardEps => forwardEps.HasValue ? forwardEps.Value.ToString("F3") : "N/A";

        public static string FormatNumber(double number)
        {
            if (number >= 1_000_000_000_000)
            {
                return $"{number / 1_000_000_000_000:0.000}T"; // Định dạng dưới đơn vị nghìn tỷ (T) với 3 chữ số sau dấu phẩy
            }
            else if (number >= 1_000_000_000)
            {
                return $"{number / 1_000_000_000:0.000}B"; // Định dạng dưới đơn vị tỷ (B) với 3 chữ số sau dấu phẩy
            }
            else if (number >= 1_000_000)
            {
                return $"{number / 1_000_000:0.000}M"; // Định dạng dưới đơn vị triệu (M) với 3 chữ số sau dấu phẩy
            }
            else
            {
                return number.ToString("0.000"); // Định dạng số bình thường với 3 chữ số sau dấu phẩy
            }
        }

        public override string ToString()
        {
            return $"Symbol: {Ticker ?? "N/A"}, Company_name: {Company_name ?? "N/A"}, Last_price: {Last_price?.ToString("F2") ?? "N/A"}, Change: {FormattedChange}, Per_change: {FormattedPerChange}, Volume: {FormattedVolume}, Market_Cap: {FormattedMarketCap}, trailingEps: {trailingEps?.ToString("F3") ?? "N/A"}";
        }
    }

    public class StockServices
    {
        static StockServices()
        {
            // Set the path to the Python DLL
            Runtime.PythonDLL = @"C:\Users\phanp\AppData\Local\Programs\Python\Python312\python312.dll";
            PythonEngine.Initialize();
        }

        public List<StockData> GetStockData(List<string> tickers)
        {
            List<StockData> stockDataList = new List<StockData>();

            try
            {
                using (Py.GIL())
                {
                    // Import yfinance
                    dynamic yf = Py.Import("yfinance");

                    foreach (string ticker in tickers)
                    {
                        dynamic data = yf.Ticker(ticker);
                        dynamic info = data.info;

                        dynamic currentPrice = info.get("currentPrice", null);
                        dynamic previousClose = info.get("previousClose", null);

                        double? change = null;
                        double? percentChange = null;

                        if (currentPrice != null && previousClose != null)
                        {
                            change = (double)currentPrice - (double)previousClose;
                            percentChange = (change / (double)previousClose) * 100;
                        }

                        var stockData = new StockData
                        {
                            Ticker = info.get("symbol", null),
                            Company_name = info.get("shortName", null)?.split('.')[0],
                            Last_price = info.get("open", null) != null ? (double?)info.get("open", null) : null,
                            open = currentPrice,
                            Change = change,
                            Per_change = percentChange,
                            Volume = info.get("volume", null) != null ? (double?)info.get("volume", null) : null,
                            Market_cap = info.get("marketCap", null) != null ? (double?)info.get("marketCap", null) : null,
                            trailingEps = info.get("trailingEps", null) != null ? (double?)info.get("trailingEps", null) : null,
                            forwardEps = info.get("forwardEps", null) != null ? (double?)info.get("forwardEps", null) : null,
                            DayLow = info.get("dayLow", null) != null ? (double?)info.get("dayLow", null) : null,
                            DayHigh = info.get("dayHigh", null) != null ? (double?)info.get("dayHigh", null) : null
                        };

                        stockDataList.Add(stockData);

                        // Optionally print the data to console for debugging
                        PrintStockData(stockData);
                    }
                }
            }
            catch (PythonException ex)
            {
                // Handle Python errors
                Console.WriteLine($"Python error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other errors
                Console.WriteLine($"Error: {ex.Message}");
            }

            return stockDataList;
        }

        private void PrintStockData(StockData stockData)
        {
            Console.WriteLine(stockData.ToString());
        }
    }
}
