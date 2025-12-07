namespace urunsatisportali.Helpers
{
    public static class CurrencyHelper
    {
        public static string FormatTurkishLira(decimal amount)
        {
            return $"{amount:N2} ₺";
        }
    }
}

