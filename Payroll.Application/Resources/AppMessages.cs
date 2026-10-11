using System.Globalization;
using System.Resources;

namespace Payroll.Application.Resources
{
    public static class AppMessages
    {
        private static readonly ResourceManager Manager =
            new("Payroll.Application.Resources.Messages", typeof(AppMessages).Assembly);

        public static string Get(string code, string? fallback = null) =>
            Manager.GetString(code, CultureInfo.CurrentUICulture)
            ?? (code.EndsWith(".notFound", StringComparison.Ordinal) ? Manager.GetString("notFound", CultureInfo.CurrentUICulture) : null)
            ?? fallback
            ?? code;
    }
}
