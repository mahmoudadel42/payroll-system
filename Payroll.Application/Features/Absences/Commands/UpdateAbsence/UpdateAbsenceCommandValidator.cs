using FluentValidation;
using Payroll.Application.Resources;

namespace Payroll.Application.Features.Absences.Commands.UpdateAbsence
{
    public sealed class UpdateAbsenceCommandValidator : AbstractValidator<UpdateAbsenceCommand>
    {
        public UpdateAbsenceCommandValidator(TimeProvider time)
        {
            var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);

            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Date).LessThanOrEqualTo(today)
                .WithMessage(_ => AppMessages.Get("absence.futureDate"));
            RuleFor(x => x.Reason).MaximumLength(250);
        }
    }
}
