using FPTBook.Data;
using FPTBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class Seed
{
	public async static void Initialize(IServiceProvider serviceProvider)
	{
		using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
		{
			string[] roles = new string[] { "Customer", "Administrator", "Store Owner" };

			var newrolelist = new List<IdentityRole>();
			foreach (string role in roles)
			{
				if (!context.Roles.Any(r => r.Name == role))
				{
					newrolelist.Add(new IdentityRole(role) { NormalizedName = role });
				}
			}
			context.Roles.AddRange(newrolelist);
			context.SaveChanges();
            ApplicationUser user = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FullName = "bunpro",
                Home_Address = "123"
            };

            ApplicationUser user1 = new ApplicationUser
            {
                UserName = "Storeowner@gmail.com",
                Email = "Storeowner@gmail.com",
                EmailConfirmed = true,
                FullName = "bunpro",
                Home_Address = "123"
            };
            ApplicationUser user3 = new ApplicationUser
            {
                UserName = "Storeowner1@gmail.com",
                Email = "Storeowner1@gmail.com",
                EmailConfirmed = true,
                FullName = "bunpro",
                Home_Address = "123"
            };
            ApplicationUser user2 = new ApplicationUser
            {
                UserName = "customer@gmail.com",
                Email = "customer@gmail.com",
                EmailConfirmed = true,
                FullName = "bunpro",
                Home_Address = "123"
            };
            createUser(context, serviceProvider, user, "Administrator");
            createUser(context, serviceProvider, user1, "Store Owner");
            createUser(context, serviceProvider, user2, "Customer");
            createUser(context, serviceProvider, user3, "Store Owner");
            CreateBookAndCategory(context);
			CreateStatus(context);
		}

	}
	private static void createUser(ApplicationDbContext context, IServiceProvider serviceProvider, ApplicationUser user,string role)
	{
		if (context.Users.Count() <=4)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			

			string userPassword = "Password123#";
			var userResult = userManager.CreateAsync(user, userPassword).Result;

			if (userResult.Succeeded)
			{
				// Assign the "Buyer" role to the user
				userManager.AddToRoleAsync(user, role).Wait();
			}
		}
	}

    private static void CreateStatus(ApplicationDbContext context)
    {
        if (context.Status.Count() == 0)
        {
			context.Status.AddRange(
				new List<Status>()
				{
                    new Status()
                    {
                        name = "Processing",
                        color="yellow"
                    },
                    new Status()
					{
						name = "Store Owner accept",
						color="green"
					},
                    new Status()
                    {
                        name = "Store Owner denied",
                        color="red"
                    },
                    new Status()
                    {
                        name = "Complete",
                        color="blue"
                    }
                });
        }
        
        context.SaveChanges();
    }
    private static void CreateBookAndCategory(ApplicationDbContext context)
	{
        if (context.Publisher.Count() == 0)
        {
            context.Publisher.AddRange(
               new List<Publisher> { new Publisher {  Name = "Kim dong", Address ="20 cong hoa" }, new Publisher { Name = "Kim dong 2", Address = "20 cong hoa" } }
           );
        }
        if (context.Category.Count() == 0)
		{
			context.Category.AddRange(
			   new List<Category> { new Category { Id = "C001", Name = "IT OOP", Description = "dsadasdsada" }, new Category { Id = "C002", Name = "Comic", Description = "dsadasdsada" } }
		   );
		}

		if (context.Book.Count() == 0)
		{
			var userId = context.Users.FirstOrDefault(x => x.UserName == "Storeowner1@gmail.com").Id;
            var userId1 = context.Users.FirstOrDefault(x => x.UserName == "Storeowner@gmail.com").Id;
            context.Book.AddRange(
				new List<Book> { 
					new Book {
						Title= "IT Book",
						CategoryId="C001",
						PublicationDate = DateTime.Now,
						ImageName= "51OV+q4yBkS._AC_UF1000,1000_QL80_.jpg" ,
						Price=100000,
						Author="bunpro",
                        PublisherId=2,
						StoreOwnerId=userId,
                        Description ="321732183127371"
					},

					new Book {
						Title="OBJECT ORIENTED PROGRAMMING",
						CategoryId="C001",
						PublicationDate = DateTime.Now,
						ImageName="618gXvIEizL._AC_UF1000,1000_QL80_.jpg",
						Price=120000,
						Author="bunpro",
                        PublisherId=1,
                        StoreOwnerId=userId1,
                        Description ="321732183127371"
					},
					new Book {
						Title="Doraemon Book",
						CategoryId="C002",
						PublicationDate = DateTime.Now,
						ImageName="Doraemon1.jpg",
						Price=120000,
						PublisherId=1,
						Author="bunpro",
                        StoreOwnerId=userId,
                        Description ="321732183127371"
					}
				}
			);
		}
		context.SaveChanges();
	}
}
