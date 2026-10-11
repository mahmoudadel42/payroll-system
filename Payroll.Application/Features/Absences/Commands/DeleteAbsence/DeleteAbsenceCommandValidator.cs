using FluentValidation;

namespace Payroll.Application.Features.Absences.Commands.DeleteAbsence
{
    public sealed class DeleteAbsenceCommandValidator : AbstractValidator<DeleteAbsenceCommand>
    {
        public DeleteAbsenceCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
