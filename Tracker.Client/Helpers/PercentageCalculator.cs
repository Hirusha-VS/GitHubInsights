namespace Tracker.Client.Helpers
{
    public static class PercentageCalculator
    {
        public static int CalculateCompletionRate(int done, int total)
        {
            if (total == 0) return 0;
            return (int)Math.Round((double)done / total * 100);
        }

        public static int CalculateDistribution(int count, int total)
        {
            if (total == 0) return 0;
            return (int)Math.Round((double)count / total * 100);
        }

        public static int GetBarHeight(int count, int total)
        {
            if (total == 0) return 0;
            var percentage = (int)Math.Round((double)count / total * 100);
            return Math.Max(5, percentage); // Minimum 5% height for visibility
        }
    }
}