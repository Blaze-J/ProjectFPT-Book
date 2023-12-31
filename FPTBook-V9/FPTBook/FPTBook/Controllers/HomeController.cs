﻿using FPTBook.Data;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace FPTBook.Controllers
{
    

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;

        }



        
        public async Task<IActionResult> Index( string? id, [FromQuery] string search,int? page)
        {
            string name = search;
            int pageSize = 6;
			var applicationDbContext = _context.Book.Include(b => b.Category).Where(x=>x.Category.IsRequest==false);
            if (id != null)
            {
                applicationDbContext =  _context.Book.Where(x=>x.CategoryId==id).Include(b => b.Category);
			}
            if (name != null)
            {
				applicationDbContext = applicationDbContext.Where(x=>x.Title.Contains(name)).Include(b => b.Category);
			}
            var maxPage = (int)Math.Ceiling(_context.Book.Count() / (double)pageSize);
            if (page > maxPage ||page ==0)
            {
                return NotFound();
            }
          
			applicationDbContext = applicationDbContext.Skip(pageSize * (page==null?0:page.Value-1)).Take(pageSize);
			  
			ViewData["MaxPage"] = maxPage;
			ViewData["Category"] = await _context.Category.Where(x=>x.IsRequest == false).ToListAsync();
			return View(await applicationDbContext.ToListAsync());
   
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}