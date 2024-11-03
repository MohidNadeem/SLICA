using Heron.MudCalendar;

namespace SlicaWeb.Models
{
    public class DashboardModel
    {
        public int TotalMeetings { get; set; } = 0;
        public int TotalConnections { get; set; } = 0;
        public int TotalRequests { get; set; } = 0;

        public List<MeetingModel> RecentMeetings { get; set; }

        public List<RequestModel> RecentRequests { get; set; }
        public List<CustomCalendarItem> Events = new List<CustomCalendarItem>();
        public List<LeaderBoardModel> Leaderboard { get; set; }
        public VideoModel DayVideo { get; set; }
    }
    public class CustomCalendarItem : CalendarItem
    {
        public string Status { get; set; }
    }
}
