namespace SlicaWeb.Models
{
    public class VideoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }

        public int Views { get; set; }
        public int Likes { get; set; }

        public bool IsLike { get; set; }
    }
}
