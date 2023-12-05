using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Data;
using FPTBook.Models;
using System.Security.Claims;
using System.Net;

namespace FPTBook.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Order.Include(o => o.Book).Include(o => o.Status).Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> RecordIndex()
        {
            IEnumerable<Order> applicationDbContext = _context.Order.Include(o => o.Book).Include(o => o.Status).Include(o => o.User);

            var currentUser = HttpContext.User;
            if (currentUser.Identity.IsAuthenticated)
            {
                if(User.IsInRole("Store Owner")){
                    var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                    applicationDbContext = applicationDbContext.Where(x => x.Book.StoreOwnerId == userIdClaim.Value);
                }
            
            }
            return View( applicationDbContext);
        }
        public async Task<IActionResult> Checkout(int bookId)
        {
            var applicationDbContext = _context.Order.Include(o => o.Book).Include(o => o.Status).Include(o => o.User);
            ViewBag.book = _context.Book.Include(x=>x.Category).Include(x=>x.Publisher).FirstOrDefault(x=>x.Id ==bookId);
            return View();
        }
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Book)
                .Include(o => o.Status)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([Bind("bookID,Quantity")] Order order)
        {
            if (ModelState.IsValid)
            {
                var currentUser = HttpContext.User;
                if (currentUser.Identity.IsAuthenticated)
                {
                    var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                    order.DateCheckOut = DateTime.Now;
                    order.UserId = userIdClaim.Value;
                    order.StatusId = _context.Status.FirstOrDefault(x => x.name == "Processing").Id;
                    _context.Add(order);

                    await _context.SaveChangesAsync();
                }
                    
                return RedirectToAction("Index", "Home");
            }

			ViewData["bookID"] = new SelectList(_context.Book, "Id", "Id", order.bookID);
			ViewBag.book = _context.Book.Include(x => x.Category).Include(x => x.Publisher).FirstOrDefault(x => x.Id == order.bookID    );
			return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["bookID"] = new SelectList(_context.Book, "Id", "Id", order.bookID);
                                                                                                                                                                                    
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,StatusId,DateCheckOut,bookID")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["bookID"] = new SelectList(_context.Book, "Id", "Id", order.bookID);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Id", order.StatusId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

         public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "name", order.StatusId);
            return View(order);
        }

      

        [HttpPost, ActionName("ChangeStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, [Bind("StatusId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ChangeStatus));
            }
            
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Id", order.StatusId);

            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Book)
                .Include(o => o.Status)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
