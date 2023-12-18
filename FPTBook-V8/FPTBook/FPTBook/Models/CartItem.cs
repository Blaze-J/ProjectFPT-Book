using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FPTBook.Models
{
	public class CartItem
	{

		[Key]
		[ScaffoldColumn(false)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int BookID { get; set; }
		public Book Book { get; set; }

		public int CartID { get; set; }
		public virtual Cart Cart { get; set; }
		public int quantity { get; set; }
	}
}
