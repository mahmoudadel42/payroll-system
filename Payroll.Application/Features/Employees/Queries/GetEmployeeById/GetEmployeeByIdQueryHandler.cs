using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Employees.Model;
using Payroll.Domain.Services;

namespace Payroll.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed class GetEmployeeByIdQueryHandler(IApplicationDbContext db, TimeProvider time)
        : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDetailsDto>>
    {
        public async Task<Result<EmployeeDetailsDto>> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
        {
            var employee = await db.Employees.AsNoTracking()
                .Where(e => e.Id == request.Id)
                .Select(EmployeeProjection.ToDto(CurrentCulture.IsArabic))
                .FirstOrDefaultAsync(ct);
            if (employee is null)
                return Error.NotFound("employee", request.Id);

            var baseSalary = await db.GradeSalaries.AsNoTracking()
                .Where(g => g.Grade == employee.Grade)
                .Select(g => (decimal?)g.BaseSalary)
                .FirstOrDefaultAsync(ct);

            var incentive = await db.Departments.AsNoTracking()
                .Where(d => d.Id == employee.DepartmentId)
                .Select(d => d.IncentivePercentage)
                .FirstOrDefaultAsync(ct);

            var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);
            return new EmployeeDetailsDto(employee, baseSalary, incentive, PayrollCalculator.CompletedYears(employee.HireDate, today));
        }
    }
}
