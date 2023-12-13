using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Data;
using FPTBook.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FPTBook.Controllers
{
    [Authorize(Roles = "Administrator,Store Owner")]
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Books
        public async Task<IActionResult> Index([FromQuery] string? search)
        {
            IEnumerable<Book> applicationDbContext = _context.Book.Include(b => b.Category).Include(x => x.Publisher).Include(x=>x.StoreOwner);
            if (!String.IsNullOrWhiteSpace(search))
            {
                applicationDbContext = applicationDbContext.Where(x => x.Title.Contains(search));
            }
            var currentUser = HttpContext.User;
            if (currentUser.Identity.IsAuthenticated)
            {
                var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                if (User.IsInRole("Store Owner"))
                {
                    applicationDbContext = applicationDbContext.Where(x => x.StoreOwner.Id == userIdClaim.Value);
                }
            }


            return View(applicationDbContext);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publisher.ToList(), "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Category.Where(x => x.IsRequest == false || x.IsRequest == null), "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Author,Description,Price,ImageFile,PublisherId,CategoryId")] Book book)
        {
            if (ModelState.IsValid)
            {
                var currentUser = HttpContext.User;
                if (currentUser.Identity.IsAuthenticated)
                {
                    var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
                    string extension = Path.GetExtension(book.ImageFile.FileName);
                    book.PublicationDate = DateTime.Now;
                    book.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    book.StoreOwnerId = userIdClaim.Value;
                    string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Name", book.PublisherId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Id", book.PublisherId);
            ViewData["CategoryId"] = new SelectList(_context.Category.Where(x => x.IsRequest == false || x.IsRequest == null), "Id", "Id", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Description,Price,ImageName,PublisherId,PublicationDate,CategoryId,StoreOwnerId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            
            ViewData["PublisherId"] = new SelectList(_context.Publisher, "Id", "Id", book.PublisherId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", book.CategoryId);
			var message = string.Join(" | ", ModelState.Values
	  .SelectMany(v => v.Errors)
	  .Select(e => e.ErrorMessage));
			return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                if (book.ImageName != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", book.ImageName);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
