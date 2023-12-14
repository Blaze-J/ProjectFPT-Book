using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;
using System.Reflection.Emit;

namespace FPTBook.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

        }
        public DbSet<FPTBook.Models.Category>? Category { get; set; }
        public DbSet<FPTBook.Models.Book>? Book { get; set; }
        public DbSet<FPTBook.Models.Order>? Order { get; set; }

        public DbSet<FPTBook.Models.Status>? Status { get; set; }
        public DbSet<FPTBook.Models.Publisher>? Publisher { get; set; }

    }
}