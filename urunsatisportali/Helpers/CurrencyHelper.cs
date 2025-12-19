using System.Globalization;

namespace urunsatisportali.Helpers
{
    public static class CurrencyHelper
    {
        public static string FormatCurrency(decimal amount, string currency)
        {
            // Always format as Turkish Lira regardless of input currency
            return string.Format(CultureInfo.GetCultureInfo("tr-TR"), "{0:C}", amount);
        }

        public static string FormatTurkishLira(decimal amount)
        {
            return string.Format(CultureInfo.GetCultureInfo("tr-TR"), "{0:C}", amount);
        }
    }
}

