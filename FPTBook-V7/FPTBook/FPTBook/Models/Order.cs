using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook.Models
{
    public class Order
	{
		[Key]
		[ScaffoldColumn(false)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public int Id { get; set; }
	
		public int StatusId { get; set; }	
		public Status? Status { get; set; }
		[DisplayName("Date Check Out")]
		public DateTime DateCheckOut { get; set; }
		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }

		public int? CartID { get; set; }
		public Cart? Cart { get; set; }
	
		public string Address { get; set; }
		public int Phone { get; set; }

	}
}
