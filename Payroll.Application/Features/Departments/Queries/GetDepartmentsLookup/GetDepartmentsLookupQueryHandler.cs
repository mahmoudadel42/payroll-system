using MediatR;
using Microsoft.EntityFrameworkCore;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Localization;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Queries.GetDepartmentsLookup
{
    public sealed class GetDepartmentsLookupQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetDepartmentsLookupQuery, IReadOnlyList<LookupDto>>
    {
        public async Task<IReadOnlyList<LookupDto>> Handle(GetDepartmentsLookupQuery request, CancellationToken ct)
        {
            var arabic = CurrentCulture.IsArabic;
            return await db.Departments.AsNoTracking()
                .OrderBy(d => arabic ? d.NameAr : d.NameEn)
                .Select(d => new LookupDto(d.Id, arabic ? d.NameAr : d.NameEn))
                .ToListAsync(ct);
        }
    }
}
