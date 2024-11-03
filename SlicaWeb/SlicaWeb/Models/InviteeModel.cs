using System.ComponentModel.DataAnnotations;

namespace SlicaWeb.Models
{
    public class InviteeModel
    {
        public int UserID { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public bool IsHost { get; set; }
        [Required(ErrorMessage ="Please Select Invitees")]
        public string Invitees { get; set; }
        public DateTime? LastInvitationDate { get; set; }
        public int SNo { get; set; }
    }
}
