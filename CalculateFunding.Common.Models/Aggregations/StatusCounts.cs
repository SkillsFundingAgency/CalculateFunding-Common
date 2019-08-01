namespace CalculateFunding.Common.Models.Aggregations
{
    public class StatusCounts
    {
        public int Approved { get; set; }

        public int Updated { get; set; }

        public int Draft { get; set; }

        public int Total => Approved + Updated + Draft;
    }
}
