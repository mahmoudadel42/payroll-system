using Payroll.Application.Common.Models;
using Payroll.Domain.Entities;

namespace Payroll.Application.Common.Interfaces
{
    public interface IPayrollDataService
    {
        /// <summary>Calculates unsaved payslip snapshots for every employee eligible in the given month.</summary>
        Task<Result<IReadOnlyList<Payslip>>> BuildPayslipsAsync(
            int year, int month, int? departmentId, int? employeeId, CancellationToken ct);
    }
}
