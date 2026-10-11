namespace Payroll.Application.Features.Absences.Model
{
    public sealed record AbsenceDto
    {
        public int Id { get; init; }
        public int EmployeeId { get; init; }
        public string EmployeeName { get; init; } = string.Empty;
        public string DepartmentName { get; init; } = string.Empty;
        public DateOnly Date { get; init; }
        public string? Reason { get; init; }
    }
}
