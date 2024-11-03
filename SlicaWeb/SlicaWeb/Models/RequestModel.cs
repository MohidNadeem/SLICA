namespace SlicaWeb.Models
{
    public class RequestModel
    {

        public int SNo { get; set; }

        public int Id { get; set; }
        public int userId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
        public string Status { get; set; }


        public string FileName { get; set; }
        public string Bio { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public int SenderID { get; set; }

        public int ReceiverID { get; set; }
    }
}
