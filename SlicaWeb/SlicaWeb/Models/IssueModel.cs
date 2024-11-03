using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SlicaWeb.Models
{
    public class IssueModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage = "Name is Mandatory")]

        public string Name { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is Mandatory")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }
        [DisplayName("Phone No")]
        [Required(ErrorMessage = "Phone No is Mandatory")]

        public string PhoneNo { get; set; }
        [DisplayName("Category")]
        [Required(ErrorMessage = "Category is Mandatory")]

        public int CategoryID { get; set; } = 1;
        public string Category { get; set; }

        public string Status { get; set; }
        public DateTime? ReportedDate { get; set; }

        public DateTime? ResolvedDate  { get; set; }
        [DisplayName("Subject")]
        [Required(ErrorMessage = "Subject is Mandatory")]
        public string Subject { get; set; }
        [DisplayName("Description")]
        [Required(ErrorMessage = "Description is Mandatory")]
        public string Description { get; set; }

    }
}
