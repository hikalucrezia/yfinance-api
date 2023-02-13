using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace yfinance
{
    /// <summary>
    /// All date types represented as UTC
    /// </summary>
    public class Yfp
    {
        private static readonly string baseUrl = "https://query1.finance.yahoo.com/v7/finance/chart/";
        public List<YfpSymbol> LastSymbols;
        public string LastResponceSource;
        public Yfp()
        {
            LastSymbols = new List<YfpSymbol>();
            LastResponceSource = "";
        }
        public List<YfpSymbol> GetSymbols(string symbolName, DateRange dateRange, Interval interval)
        {
            string url = baseUrl + symbolName + "?range=" + dateRange.ToString().Replace("_", "")
                + "&interval=" + interval.ToString().Replace("_", "") + "&indicators=quote&includeTimestamps=true";
            string source;
            try
            {
                HttpClient client1 = new HttpClient();
                var t1 = client1.GetStringAsync(url);
                source = t1.Result;
            }
            catch (Exception ex)
            {
                throw new YfpException(ex.Message, ex);
            }
            List<YfpSymbol> l;
            try
            {
                JObject obj = JObject.Parse(source);
                JArray timestamps = (JArray)obj["chart"]["result"][0]["timestamp"];
                JObject indicators = (JObject)obj["chart"]["result"][0]["indicators"];
                JObject quotes = (JObject)indicators["quote"][0];
                l = new List<YfpSymbol>(timestamps.Count);
                for (int i = 0; i < timestamps.Count; i++)
                {
                    int timestamp = (int)timestamps[i];
                    decimal? high = quotes["high"][i].Type == JTokenType.Null ? null : (decimal)quotes["high"][i];
                    decimal? low = quotes["low"][i].Type == JTokenType.Null ? null : (decimal)quotes["low"][i];
                    decimal? open = quotes["open"][i].Type == JTokenType.Null ? null : (decimal)quotes["open"][i];
                    decimal? close = quotes["close"][i].Type == JTokenType.Null ? null : (decimal)quotes["close"][i];
                    long? volume = quotes["volume"][i].Type == JTokenType.Null ? null : (long)quotes["volume"][i];
                    var symbol = new YfpSymbol(timestamp, high, low, open, close, volume);
                    l.Add(symbol);
                }
            }
            catch (Exception ex)
            {
                throw new YfpException(ex.Message, ex);
            }
            LastSymbols = l;
            LastResponceSource = source;
            return l;
        }
    }
    /// <summary>
    /// Represents the data fetched from yahoo
    /// </summary>
    public class YfpSymbol
    {
        public DateTime Dt { get; set; }
        public int TimeStamp { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public decimal? Open { get; set; }
        public decimal? Close { get; set; }
        public long? Volume { get; set; }
        public YfpSymbol(int timestamp, decimal? high, decimal? low, decimal? open, decimal? close, long? volume)
        {
            Dt = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            TimeStamp = timestamp;
            Low = low;
            High = high;
            Open = open;
            Close = close;
            Volume = volume;
        }
    }

    [Serializable()]
    public class YfpException : Exception
    {
        public YfpException() : base()
        {
        }

        public YfpException(string message)
        : base(message)
        {
        }

        public YfpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected YfpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Determines the range of data to acquire
    /// </summary>
    public enum DateRange
    {
        _1d = 1,
        _5d = 2,
        _1mo = 3,
        _3mo = 4,
        _6mo = 5,
        _1y = 6,
        _2y = 7,
        _5y = 8,
        _10y = 9,
        _ytd = 10,
        _max = 11
    }

    /// <summary>
    /// Determines the interval between each data
    /// </summary>
    public enum Interval
    {
        _1m = 1,
        _2m = 2,
        _5m = 3,
        _15m = 4,
        _30m = 5,
        _60m = 6,
        _90m = 7,
        _1h = 8,
        _1d = 9,
        _5d = 10,
        _1wk = 11,
        _1mo = 12,
        _3mo = 13
    }
}