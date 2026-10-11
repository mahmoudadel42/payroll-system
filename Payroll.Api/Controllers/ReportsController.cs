using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Features.Reports.Model;
using Payroll.Application.Features.Reports.Queries.GetAttendanceReport;
using Payroll.Application.Features.Reports.Queries.GetEmployeesReport;
using Payroll.Application.Features.Reports.Queries.GetIncentivesDeductionsReport;
using Payroll.Application.Features.Reports.Queries.GetSalariesReport;

namespace Payroll.Api.Controllers
{
    /// <remarks>Every report returns JSON by default, or an Excel file when called with <c>?format=xlsx</c>.</remarks>
    public sealed class ReportsController(IReportExporter exporter) : ApiControllerBase
    {
        [HttpGet("attendance")]
        [ProducesResponseType<AttendanceReportDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Attendance([FromQuery] GetAttendanceReportQuery query, string? format, CancellationToken ct)
        {
            var report = await Sender.Send(query, ct);
            return IsExcel(format)
                ? Excel("Attendance", report.Rows, $"attendance-{query.Year}-{query.Month:00}")
                : Ok(report);
        }

        [HttpGet("incentives-deductions")]
        [ProducesResponseType<IncentivesDeductionsReportDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> IncentivesDeductions([FromQuery] GetIncentivesDeductionsReportQuery query, string? format, CancellationToken ct)
        {
            var result = await Sender.Send(query, ct);
            return result.IsSuccess && IsExcel(format)
                ? Excel("IncentivesDeductions", result.Value!.Rows, $"incentives-deductions-{query.Year}-{query.Month:00}")
                : FromResult(result);
        }

        [HttpGet("employees")]
        [ProducesResponseType<EmployeesReportDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Employees([FromQuery] GetEmployeesReportQuery query, string? format, CancellationToken ct)
        {
            var report = await Sender.Send(query, ct);
            return IsExcel(format)
                ? Excel("Employees", report.Rows, $"employees-{report.AsOf:yyyy-MM-dd}")
                : Ok(report);
        }

        [HttpGet("salaries")]
        [ProducesResponseType<SalariesReportDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Salaries([FromQuery] GetSalariesReportQuery query, string? format, CancellationToken ct)
        {
            var result = await Sender.Send(query, ct);
            return result.IsSuccess && IsExcel(format)
                ? Excel("Salaries", result.Value!.Rows, $"salaries-{query.Year}-{query.Month:00}")
                : FromResult(result);
        }

        private static bool IsExcel(string? format) =>
            string.Equals(format, "xlsx", StringComparison.OrdinalIgnoreCase);

        private FileContentResult Excel<T>(string sheetName, IEnumerable<T> rows, string fileName) =>
            File(exporter.ToExcel(sheetName, rows), IReportExporter.ExcelContentType, $"{fileName}.xlsx");
    }
}
