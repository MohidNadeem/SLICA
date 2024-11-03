namespace SlicaWeb.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }
        public int UserId { get; set; }
        public bool IsRead { get; set; }
        public bool Status { get; set; }

        public DateTime Date { get; set; }
        public int? SenderId { get; set; }

        public string ElaspedTime { get; set; }

    }
}
