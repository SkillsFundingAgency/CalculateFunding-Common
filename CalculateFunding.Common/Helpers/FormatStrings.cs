namespace CalculateFunding.Common.Helpers
{
    public static class FormatStrings
    {
        public const string DateTimeFormatString = "dd MMMM hh:mm tt";

        public const string DateFormatString = "d/MM/yyyy";

        public const string DateFullMonthFormatString = "dd MMMM yyyy";

        public const string TimeFormatString = "HH:mm";

        public const string MoneyFormatString = "{0:n}";

        public static string FundingPeriodString(this string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length != 4)
            {
                return input;
            }

            return $"20{input.Substring(0, 2)}/{input.Substring(2, 2)}";
        }

        public static string GetPreviousFundingPeriod(string fundingPeriod, int yearsBack)
        {
            //FundingPeriod will be in format AY-2425
            string[] parts = fundingPeriod.Split('-');
            string prefix = parts[0];
            string yearPart = parts[1];

            // Extract the start and end years from the year part
            int startYear = int.Parse(yearPart.Substring(0, 2));
            int endYear = int.Parse(yearPart.Substring(2, 2));

            // Subtract the yearsBack value from the start and end year
            startYear -= yearsBack;
            endYear -= yearsBack;

            string newYearPart = $"{startYear:D2}{endYear:D2}";

            return $"{prefix}-{newYearPart}";
        }
    }
}
