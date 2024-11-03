namespace SlicaWeb.Models
{
    public class AgoraTokenModel
    {
        public int ID { get; set; }
        public string AppID { get; set; }
        public string ChannelName { get; set; }

        public string Token { get; set; }
        public string AppCertificate { get; set; }
        public int MeetingID { get; set; }

        public string MeetingName { get; set; }

        public int Duration { get; set; }
        public DateTime MeetingDateTime { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public int CreatedBy { get; set; }
    }
}
