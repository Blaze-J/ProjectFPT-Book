using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FPTBook.Models
{
    public class Category
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [DisplayName("Category Name")]
        public string Name { get; set; }
        public string? Description { get; set; }

        public bool IsRequest {  get; set; }
        public List<Book>? Books { get; set; }

        public string? StoreOwnerId { get; set; }

        public ApplicationUser? StoreOwner { get; set; }
    }
}
