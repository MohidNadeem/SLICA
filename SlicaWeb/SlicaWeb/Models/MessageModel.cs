namespace SlicaWeb.Models
{
    public class MessageModel
    {  
            public long Id { get; set; }
            public int SenderID { get; set; }
            public int ReceiverID { get; set; }
            public string Message { get; set; }
            public DateTime CreatedDate { get; set; }

            public string SenderFirstName { get; set; }

            public string ReceiverFirstName { get; set; }
        public string SenderLastName { get; set; }

        public string ReceiverLastName { get; set; }

        public string SenderFileName { get; set; }

        public string ReceiverFileName { get; set; }

    }
}
