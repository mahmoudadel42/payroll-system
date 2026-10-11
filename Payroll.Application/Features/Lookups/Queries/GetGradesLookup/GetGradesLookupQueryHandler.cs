using MediatR;
using Payroll.Application.Common.Localization;
using Payroll.Application.Common.Models;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.Lookups.Queries.GetGradesLookup
{
    public sealed class GetGradesLookupQueryHandler : IRequestHandler<GetGradesLookupQuery, IReadOnlyList<LookupDto>>
    {
        private static readonly Dictionary<JobGrade, string> ArabicNames = new()
        {
            [JobGrade.First] = "الأولى",
            [JobGrade.Second] = "الثانية",
            [JobGrade.Third] = "الثالثة"
        };

        public Task<IReadOnlyList<LookupDto>> Handle(GetGradesLookupQuery request, CancellationToken ct)
        {
            var arabic = CurrentCulture.IsArabic;
            IReadOnlyList<LookupDto> grades = Enum.GetValues<JobGrade>()
                .Select(g => new LookupDto((int)g, arabic ? ArabicNames[g] : g.ToString()))
                .ToList();
            return Task.FromResult(grades);
        }
    }
}
