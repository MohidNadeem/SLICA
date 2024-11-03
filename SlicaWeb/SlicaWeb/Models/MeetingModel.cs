using System.ComponentModel.DataAnnotations;

namespace SlicaWeb.Models
{
    public class MeetingModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is Mandatory")]
        public string Title { get; set; }

        public int SNo { get; set; }
        public string Code { get; set; }

        [Required(ErrorMessage = "Start DateTime is Mandatory")]

        public DateTime? StartDateTime { get; set; }
        public string Host { get; set; }

        public string Status { get; set; }
        [Required(ErrorMessage = "Duration is Mandatory")]
        public int Duration { get; set; }

        public bool IsHost { get; set; }
        public bool IsEdit { get; set; } = false;
        public bool IsPin { get; set; } = false;

        public bool IsJoin { get; set; } = false;
        public bool IsActive { get; set; }

        public string Participant { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}
