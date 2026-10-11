using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.PayrollRuns.Model
{
    internal static class PayrollRunErrors
    {
        public static Error NotFound(int id) => Error.NotFound("payrollRun", id);

        public static Error Locked =>
            Error.Rule("payroll.locked", "Approved payroll runs cannot be modified.");
    }
}
