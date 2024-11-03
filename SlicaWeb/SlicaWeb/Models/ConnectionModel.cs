namespace SlicaWeb.Models
{
    public class ConnectionModel
    {
        public int SNo { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime? ConnectedSince { get; set; }
        public string FileName { get; set; }

        public string Bio { get; set; }

    }
}
