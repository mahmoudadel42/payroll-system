namespace Payroll.Application.Common.Interfaces
{
    public interface IReportExporter
    {
        public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>Writes one worksheet whose columns are the public properties of <typeparamref name="T"/>.</summary>
        byte[] ToExcel<T>(string sheetName, IEnumerable<T> rows);
    }
}
