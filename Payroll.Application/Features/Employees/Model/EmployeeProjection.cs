using System.Linq.Expressions;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.Employees.Model
{
    public static class EmployeeProjection
    {
        public static Expression<Func<Employee, EmployeeDto>> ToDto(bool arabic) => e => new EmployeeDto
        {
            Id = e.Id,
            FullName = e.FullName,
            BirthDate = e.BirthDate,
            Address = e.Address,
            Phone = e.Phone,
            Email = e.Email,
            Grade = e.Grade,
            DepartmentId = e.DepartmentId,
            DepartmentName = arabic ? e.Department.NameAr : e.Department.NameEn,
            HireDate = e.HireDate
        };
    }
}
