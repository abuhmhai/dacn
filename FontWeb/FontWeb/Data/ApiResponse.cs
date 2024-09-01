namespace FontWeb.Data
{
    public class CandlestickData
    {
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public DateTime Time { get; set; } // Timestamp in seconds
    }

    public class ApiResponse
    {
        public List<CandlestickData> Candlestick { get; set; }
        public List<DataPoint> Ema { get; set; }
        public List<DataPoint> Rsi { get; set; }
    }

    public class DataPoint
    {
        public long Time { get; set; } // Unix timestamp
        public double Value { get; set; }
    }
}
