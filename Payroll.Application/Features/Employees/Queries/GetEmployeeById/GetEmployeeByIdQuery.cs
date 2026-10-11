using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Employees.Model;

namespace Payroll.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed record GetEmployeeByIdQuery(int Id) : IRequest<Result<EmployeeDetailsDto>>;
}
