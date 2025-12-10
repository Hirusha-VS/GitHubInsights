using Tracker.Client.Dtos;
using Tracker.Client.Extensions;
using Tracker.Client.Models;

namespace Tracker.Client.Helpers
{
    public static class DateHelper
    {
        public static OverdueInfo GetOverdueInfo(ProjectItem item)
        {
            var endDate = item.GetEndDate();

            if (!endDate.HasValue)
            {
                return new OverdueInfo { IsOverdue = false, IsDueSoon = false, Message = "" };
            }

            var today = DateTime.Today;
            var daysUntilDue = (endDate.Value.Date - today).Days;

            if (daysUntilDue < 0)
            {
                // Overdue
                var daysOverdue = Math.Abs(daysUntilDue);
                return new OverdueInfo
                {
                    IsOverdue = true,
                    IsDueSoon = false,
                    Message = $"Overdue by {daysOverdue} day{(daysOverdue != 1 ? "s" : "")}"
                };
            }
            else if (daysUntilDue <= 2)
            {
                // Due soon (within 2 days)
                if (daysUntilDue == 0)
                    return new OverdueInfo { IsOverdue = false, IsDueSoon = true, Message = "Due today" };
                else if (daysUntilDue == 1)
                    return new OverdueInfo { IsOverdue = false, IsDueSoon = true, Message = "Due tomorrow" };
                else
                    return new OverdueInfo { IsOverdue = false, IsDueSoon = true, Message = $"Due in {daysUntilDue} days" };
            }

            return new OverdueInfo { IsOverdue = false, IsDueSoon = false, Message = "" };
        }

        public static string GetOverdueClass(ProjectItem item)
        {
            var overdueInfo = GetOverdueInfo(item);
            if (overdueInfo.IsOverdue)
                return "overdue";
            else if (overdueInfo.IsDueSoon)
                return "due-soon";
            return "";
        }
    }
}