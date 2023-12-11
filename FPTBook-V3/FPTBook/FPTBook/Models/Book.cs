 using Microsoft.EntityFrameworkCore;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTBook.Models
{
    public class Book
    {
        [Key]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime? PublicationDate { get; set; }
        
        public string? Description { get; set; }
        public int Price { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image")]
        public string? ImageName { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile? ImageFile { get; set; }

        [ForeignKey("Category")]
        public string CategoryId { get; set; }

        public  Category?  Category { get; set; }
		public int? PublisherId { get; set; }
		public Publisher? Publisher { get; set; }
        public string? StoreOwnerId {  get; set; }
        public ApplicationUser? StoreOwner { get; set; }
    }
}
