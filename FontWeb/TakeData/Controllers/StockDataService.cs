using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Python.Runtime;

namespace TakeData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockDataService : ControllerBase
    {
        static StockDataService()
        {
            Runtime.PythonDLL = @"C:\Users\phanp\AppData\Local\Programs\Python\Python312\python312.dll";
            PythonEngine.Initialize();
        }

        [HttpGet]
        public IActionResult GetStockData()
        {
            List<StockDat> stockDataList = new List<StockDat>();

            try
            {
                using (Py.GIL())
                {
                    dynamic yf = Py.Import("yfinance");
                    dynamic data = yf.Ticker("BTC-USD");
                    dynamic hist = data.history(period: "1mo");

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
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok(stockDataList);
        }
    }
    public class StockDat
    {
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }
}

