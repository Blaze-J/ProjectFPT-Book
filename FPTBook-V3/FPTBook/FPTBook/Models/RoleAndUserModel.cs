using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FPTBook.Models
{
    public class RoleAndUserModel
    {
        public string rolename { get; set; }
        public List<ApplicationUser> user {  get; set; }
      
    }
}
