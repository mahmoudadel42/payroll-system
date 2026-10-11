using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.Absences.Commands.CreateAbsences
{
    public sealed class CreateAbsencesCommandValidator : AbstractValidator<CreateAbsencesCommand>
    {
        public CreateAbsencesCommandValidator(TimeProvider time)
        {
            var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);

            RuleFor(x => x.EmployeeId).GreaterThan(0);
            RuleFor(x => x.Dates).NotEmpty().Must(d => d.Count <= 31);
            RuleForEach(x => x.Dates).LessThanOrEqualTo(today)
                .WithMessage(_ => AppMessages.Get("absence.futureDate"));
            RuleFor(x => x.Reason).MaximumLength(250);
        }
    }
}
