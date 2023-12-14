using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Data;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FPTBook.Controllers
{
	public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

		// GET: Categories
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Index([FromQuery] string? search)
        {
            if(_context.Category == null)
            {
				return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
			}

            IEnumerable<Category> categories =  _context.Category.Include(x => x.StoreOwner);
            if (!String.IsNullOrEmpty(search))
            {
				categories = categories.Where(x=>x.Name.Contains(search));

			}
            return View(categories);
                        
        }

		[Authorize(Roles = "Administrator,Store Owner")]
		public async Task<IActionResult> ListCategory([FromQuery] string? search)
		{
            

			 if (_context.Category == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
			}

			IEnumerable<Category> categories = _context.Category.Where(x => x.IsRequest == false).Include(x => x.StoreOwner);
			if (!String.IsNullOrEmpty(search))
			{
				categories = categories.Where(x => x.Name.Contains(search));

			}
			return View(categories);
		}

		[Authorize(Roles = "Administrator,Store Owner")]
		// GET: Categories/Details/5
		public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

		// GET: Categories/Create

		[Authorize(Roles = "Administrator")]
		public IActionResult Create()
        {
            return View();
        }

		// POST: Categories/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.IsRequest = false;

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

		// GET: Categories/Edit/5
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

		// POST: Categories/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "Administrator")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,IsRequest,StoreOwnerId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

		// GET: Categories/Delete/5
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

		[Authorize(Roles = "Administrator")]
		// POST: Categories/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



		[Authorize(Roles = "Administrator,Store Owner")]
		public IActionResult RequestNewCategory()
		{
			return View();
		}

		[Authorize(Roles = "Administrator,Store Owner")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RequestNewCategory([Bind("Id,Name,Description")] Category category)
		{
			if (ModelState.IsValid)
			{
				var currentUser = HttpContext.User;
                if (currentUser.Identity.IsAuthenticated)
                {
                    var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                    category.IsRequest = true;
                    category.StoreOwnerId = userIdClaim.Value;
			        _context.Add(category);
					await _context.SaveChangesAsync();
					
				}
				return RedirectToAction(nameof(Index));
			}
			return View(category);
		}

		private bool CategoryExists(string id)
        {
          return (_context.Category?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
