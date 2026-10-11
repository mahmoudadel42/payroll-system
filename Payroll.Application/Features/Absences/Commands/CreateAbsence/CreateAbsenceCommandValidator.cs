using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.Absences.Commands.CreateAbsence
{
    public sealed class CreateAbsenceCommandValidator : AbstractValidator<CreateAbsenceCommand>
    {
        public CreateAbsenceCommandValidator(TimeProvider time)
        {
            var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);

            RuleFor(x => x.EmployeeId).GreaterThan(0);
            RuleFor(x => x.Date).LessThanOrEqualTo(today)
                .WithMessage(_ => AppMessages.Get("absence.futureDate"));
            RuleFor(x => x.Reason).MaximumLength(250);
        }
    }
}
