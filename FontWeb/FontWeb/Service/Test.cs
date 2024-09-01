using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YahooFinanceApi;

public class Test
{
    public async Task<(List<CandleStickDataPoint> CandlestickData, List<IndicatorDataPoint> EMAData, List<IndicatorDataPoint> RSIData)> FetchStockData(string ticker, string interval, int emaPeriod, int rsiPeriod)
    {
        var endDate = DateTime.Now;
        var startDate = interval switch
        {
            "1d" => endDate.AddYears(-5),
            "1wk" => endDate.AddYears(-5),
            "1mo" => endDate.AddYears(-5),
            _ => endDate.AddYears(-1)
        };

        var history = await Yahoo.GetHistoricalAsync(ticker, startDate, endDate, GetResolution(interval));
        var closePrices = history.Select(c => c.Close).ToList();

        var ema = CalculateEma(closePrices, emaPeriod);
        var rsi = CalculateRsi(closePrices, rsiPeriod);

        var candlestickData = history.Select(q => new CandleStickDataPoint(q.DateTime, q.Open, q.High, q.Low, q.Close)).ToList();
        var emaData = ema.Select((value, index) => new IndicatorDataPoint(history.ElementAt(index).DateTime, value)).ToList();
        var rsiData = rsi.Select((value, index) => new IndicatorDataPoint(history.ElementAt(index).DateTime, value)).ToList();

        return (candlestickData, emaData, rsiData);
    }

    private List<decimal> CalculateEma(List<decimal> prices, int period)
    {
        var ema = new List<decimal>();
        decimal multiplier = 2m / (period + 1);
        decimal? prevEma = null;

        for (int i = 0; i < prices.Count; i++)
        {
            if (i < period - 1)
            {
                ema.Add(0); // insufficient data for EMA
            }
            else if (i == period - 1)
            {
                var sum = prices.Take(period).Sum();
                var sma = sum / period;
                ema.Add(sma);
                prevEma = sma;
            }
            else
            {
                var currentEma = ((prices[i] - prevEma.Value) * multiplier) + prevEma.Value;
                ema.Add(currentEma);
                prevEma = currentEma;
            }
        }

        return ema;
    }

    private List<decimal> CalculateRsi(List<decimal> prices, int period)
    {
        if (prices == null || prices.Count < period)
        {
            throw new ArgumentException("Insufficient data to calculate RSI.");
        }

        var gains = new List<decimal>();
        var losses = new List<decimal>();

        // Calculate gains and losses
        for (int i = 1; i < prices.Count; i++)
        {
            var change = prices[i] - prices[i - 1];
            if (change > 0)
                gains.Add(change);
            else
                losses.Add(-change);
        }

        var rsiValues = new List<decimal>();

        for (int i = period - 1; i < prices.Count - 1; i++)
        {
            var currentGains = gains.Skip(i - period + 1).Take(period).ToList();
            var currentLosses = losses.Skip(i - period + 1).Take(period).ToList();

            // Ensure there are elements before calculating averages
            if (currentGains.Count > 0 && currentLosses.Count > 0)
            {
                var averageGain = currentGains.Average();
                var averageLoss = currentLosses.Average();

                if (averageLoss == 0)
                {
                    rsiValues.Add(100); // RSI is 100 if there are no losses
                }
                else
                {
                    var rs = averageGain / averageLoss;
                    var rsi = 100 - (100 / (1 + rs));
                    rsiValues.Add(rsi);
                }
            }
            else
            {
                // Handle cases where we don't have enough data to calculate RSI
                rsiValues.Add(0); // Or some other default value or logic
            }
        }

        return rsiValues;
    }



    private Period GetResolution(string interval) =>
        interval switch
        {
            "1d" => Period.Daily,
            "1wk" => Period.Weekly,
            "1mo" => Period.Monthly,
            _ => Period.Daily
        };
}

public class CandleStickDataPoint
{
    public DateTime Time { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }

    public CandleStickDataPoint(DateTime time, decimal open, decimal high, decimal low, decimal close)
    {
        Time = time;
        Open = open;
        High = high;
        Low = low;
        Close = close;
    }
}

public class IndicatorDataPoint
{
    public DateTime Time { get; set; }
    public decimal Value { get; set; }

    public IndicatorDataPoint(DateTime time, decimal value)
    {
        Time = time;
        Value = value;
    }
}
