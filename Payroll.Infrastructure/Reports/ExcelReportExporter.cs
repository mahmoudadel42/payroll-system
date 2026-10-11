using System.Reflection;
using ClosedXML.Excel;
using Payroll.Application.Common.Interfaces;

namespace Payroll.Infrastructure.Reports
{
    public sealed class ExcelReportExporter : IReportExporter
    {
        public byte[] ToExcel<T>(string sheetName, IEnumerable<T> rows)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && IsSimple(p.PropertyType))
                .ToArray();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(sheetName);

            for (var col = 0; col < properties.Length; col++)
                sheet.Cell(1, col + 1).Value = properties[col].Name;

            var row = 2;
            foreach (var item in rows)
            {
                for (var col = 0; col < properties.Length; col++)
                    SetValue(sheet.Cell(row, col + 1), properties[col].GetValue(item));
                row++;
            }

            var header = sheet.Range(1, 1, 1, Math.Max(1, properties.Length));
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;
            sheet.SheetView.FreezeRows(1);
            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void SetValue(IXLCell cell, object? value)
        {
            switch (value)
            {
                case null:
                    return;
                case DateOnly date:
                    cell.Value = date.ToDateTime(TimeOnly.MinValue);
                    cell.Style.DateFormat.Format = "yyyy-MM-dd";
                    return;
                case DateTime dateTime:
                    cell.Value = dateTime;
                    cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                    return;
                case decimal number:
                    cell.Value = number;
                    cell.Style.NumberFormat.Format = "#,##0.00";
                    return;
                case int number:
                    cell.Value = number;
                    return;
                case bool flag:
                    cell.Value = flag;
                    return;
                default:
                    cell.Value = value.ToString();
                    return;
            }
        }

        private static bool IsSimple(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal)
                   || t == typeof(DateOnly) || t == typeof(DateTime);
        }
    }
}
