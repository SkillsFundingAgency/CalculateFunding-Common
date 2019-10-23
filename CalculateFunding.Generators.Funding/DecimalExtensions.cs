namespace CalculateFunding.Generators.Funding
{
    public static class DecimalExtensions
    {
        public static decimal? AddValueIfNotNull(this decimal? value1, decimal? value2)
        {
            if (value1 == null)
                return value2;

            if (value2 == null)
                return value1;

            return value2 + value1;
        }
    }
}