using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace FontWeb.Data
{
    public class StockDataServiceCsv
    {
        public List<StockChartCsvData> GetStockDataFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null, // Bỏ qua kiểm tra tiêu đề
                MissingFieldFound = null, // Bỏ qua các trường thiếu
                BadDataFound = null, // Bỏ qua dữ liệu không hợp lệ
                Delimiter = "," // Xác định dấu phân cách
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = new List<StockChartCsvData>();

                // Đọc tiêu đề đầu tiên để xác định các trường
                csv.Read();
                csv.ReadHeader();

                // Đọc từng bản ghi một
                while (csv.Read())
                {
                    var record = new StockChartCsvData();

                    // Đọc các trường và xử lý các giá trị null
                    record.Date = ParseDate(csv.GetField("Date"));
                    record.Open = ParseDouble(csv.GetField("Open"));
                    record.High = ParseDouble(csv.GetField("High"));
                    record.Low = ParseDouble(csv.GetField("Low"));
                    record.Close = ParseDouble(csv.GetField("Close"));
                    record.AdjClose = ParseDouble(csv.GetField("AdjClose"));
                    record.Volume = ParseLong(csv.GetField("Volume"));

                    // Kiểm tra tính hợp lệ của bản ghi trước khi thêm vào danh sách
                    if (IsValid(record))
                    {
                        records.Add(record);
                    }
                }

                return records;
            }
        }

        // Phương thức để phân tích ngày từ chuỗi
        private DateTime ParseDate(string dateText)
        {
            if (DateTime.TryParse(dateText, out var date))
            {
                return date;
            }
            return default(DateTime);
        }

        // Phương thức để phân tích số thực từ chuỗi
        private double ParseDouble(string doubleText)
        {
            if (double.TryParse(doubleText, out var number))
            {
                return number;
            }
            return 0;
        }

        // Phương thức để phân tích số nguyên dài từ chuỗi
        private long ParseLong(string longText)
        {
            if (long.TryParse(longText, out var number))
            {
                return number;
            }
            return 0;
        }

        // Phương thức kiểm tra tính hợp lệ của bản ghi
        private bool IsValid(StockChartCsvData record)
        {
            // Kiểm tra các giá trị và trả về true nếu tất cả giá trị hợp lệ, false nếu không
            return record.Date != default(DateTime) &&
                   record.Open != 0 &&
                   record.High != 0 &&
                   record.Low != 0 &&
                   record.Close != 0 &&
                   record.AdjClose != 0 &&
                   record.Volume != 0;
        }
    }
}
