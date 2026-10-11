using System.Globalization;

namespace Payroll.Application.Common.Localization
{
    public static class CurrentCulture
    {
        public static bool IsArabic => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";
    }
}
