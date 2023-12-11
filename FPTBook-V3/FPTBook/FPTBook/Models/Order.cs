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
		public string? UserId {  get; set; }
		public int StatusId { get; set; }	
		public Status? Status { get; set; }
		[DisplayName("Date Check Out")]
		public DateTime DateCheckOut { get; set; }
		public ApplicationUser? User { get; set; }
		public int bookID { get; set; }
		public Book? Book { get; set; }
		public int Quantity { get; set; }
		
	}
}
