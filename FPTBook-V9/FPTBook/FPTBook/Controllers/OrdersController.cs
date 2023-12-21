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
using Microsoft.AspNetCore.Authorization;

namespace FPTBook.Controllers
{

	public class OrdersController : Controller
	{
		private readonly ApplicationDbContext _context;

		public OrdersController(ApplicationDbContext context)
		{
			_context = context;
		}

		[Authorize(Roles = "Administrator")]
		// GET: Orders
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.Order.Include(o => o.Cart).Include(o => o.Status).Include(o => o.User);
			return View(await applicationDbContext.ToListAsync());
		}
		[Authorize(Roles = "Store Owner,Administrator")]
		public async Task<IActionResult> RecordIndex()
		{
			IEnumerable<Order> applicationDbContext = _context.Order.Include(o => o.Cart).Include(o => o.Status).Include(o => o.User);

			var currentUser = HttpContext.User;
			//if (currentUser.Identity.IsAuthenticated)
			//{
			//	if (User.IsInRole("Store Owner"))
			//	{
			//		var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
			//		applicationDbContext = applicationDbContext.Where(x => x.Cart.StoreOwnerId == userIdClaim.Value);
			//	}

			//}
			
			return View(applicationDbContext);
		}

		public async Task<IActionResult> Checkout(int bookId)
		{
			var applicationDbContext = _context.Order.Include(o => o.Cart).Include(o => o.Status).Include(o => o.User);
			ViewBag.book = _context.Book.Include(x => x.Category).Include(x => x.Publisher).FirstOrDefault(x => x.Id == bookId);

			return View();
		}
		public async Task<IActionResult> CheckoutForm()
		{
            var currentUser = HttpContext.User;
			if (currentUser.Identity.IsAuthenticated)
			{
				var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                ViewData["Cart"] = await _context.Cart.Where(x => x.UserId==userIdClaim.Value &x.IsCheckout==false).Include(x => x.User).Include(x => x.CartItems).ThenInclude(x => x.Book).ThenInclude(x => x.Category).ToListAsync();
            }
                
			return View();
		}

		// GET: Orders/Details/5
		[Authorize(Roles = "Store Owner,Administrator")]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Order == null)
			{
				return NotFound();
			}

			var order = await _context.Order
				.Include(o => o.Cart)
				.Include(o => o.Status)
				.Include(o => o.User)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> CartIndex( int? id)
		{
            if (id != null)
            {
                IEnumerable<Cart> applicationDbContext = _context.Cart.Include(o => o.CartItems).ThenInclude(x => x.Book).ThenInclude(x => x.Category).Where(x => x.Id==id);
                return View(applicationDbContext);
            }
            var currentUser = HttpContext.User;
			if (currentUser.Identity.IsAuthenticated)
			{
				var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
				IEnumerable<Cart> applicationDbContext = _context.Cart.Include(o => o.CartItems).ThenInclude(x=>x.Book).ThenInclude(x=>x.Category).Where(x => x.IsCheckout == false && x.UserId == userIdClaim.Value);
				return View(applicationDbContext);

			}
           
            return Problem();

		}
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> AddCart(int BookID, int quantity)
		{
			if (_context.Book.Any(x => x.Id == BookID))
			{
				var currentUser = HttpContext.User;
				if (currentUser.Identity.IsAuthenticated)
				{
					var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
					var exitCart = _context.Cart.FirstOrDefault(x => x.UserId == userIdClaim.Value && x.IsCheckout == false);
					if (exitCart ==null)
					{
						var cart =  new Cart() { IsCheckout = false ,UserId = userIdClaim.Value };
						_context.Cart.Add(cart);
						_context.SaveChanges();
						exitCart = _context.Cart.FirstOrDefault(x => x.UserId == userIdClaim.Value && x.IsCheckout == false);
					
					
					}
					var exitsCartItem = _context.CartItem.FirstOrDefault(x => x.BookID == BookID &&x.CartID==exitCart.Id && exitCart.UserId == userIdClaim.Value && exitCart.IsCheckout==false);
					if(exitsCartItem == null)
					{
						var newCartItem = new CartItem() { BookID = BookID,CartID = exitCart.Id, quantity = quantity };
						_context.CartItem.Add(newCartItem);
						_context.SaveChanges();
					}
					else
					{
						exitsCartItem.quantity = quantity;
						_context.CartItem.Update(exitsCartItem);
						_context.SaveChanges();
					}
					return RedirectToAction(nameof(CartIndex));


				}
			}
			return Problem("Can't find book ID");
		}

		
		[Authorize]
		[HttpPost]
	
		public async Task<IActionResult> Checkout([Bind("Address,Phone")] Order order)
		{
			order.StatusId = _context.Status.FirstOrDefault(x => x.name.Contains("processing")).Id;

            if (ModelState.IsValid)
			{
				var currentUser = HttpContext.User;
				if (currentUser.Identity.IsAuthenticated)
				{
					var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
					var cart = _context.Cart.FirstOrDefault(x=>x.IsCheckout==false&&x.UserId==userIdClaim.Value);
					if(cart == null)
					{
						return Problem();
					}

                    order.CartID = cart.Id;
                    cart.IsCheckout = true;
                    order.DateCheckOut = DateTime.Now;
					order.UserId = userIdClaim.Value;
                    _context.Cart.Update(cart);
                    _context.Order.Add(order);
					_context.SaveChanges();
				}

				return RedirectToAction("Index", "Home");
			}

	
			//ViewBag.book = _context.Book.Include(x => x.Category).Include(x => x.Publisher).FirstOrDefault(x => x.Id == order.bookID);
			return RedirectToAction(nameof(CheckoutForm));
		}

		// GET: Orders/Edit/5
		[Authorize(Roles = "Administrator")]
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
			//ViewData["bookID"] = new SelectList(_context.Book, "Id", "Id", order.bookID);

			return View(order);
		}

		// POST: Orders/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "Administrator")]
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
			ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Id", order.StatusId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
			return View(order);
		}

		[Authorize(Roles = "Store Owner,Administrator")]
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

		[Authorize(Roles = "Store Owner,Administrator")]
		[HttpPost, ActionName("ChangeStatus")]
		public async Task<IActionResult> ChangeStatus(int id, [Bind("Id,StatusId,UserId,Address,Phone,CartID,bookID,DateCheckOut,Quantity")] Order order)
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
				return RedirectToAction(nameof(RecordIndex));
			}

			ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Id", order.StatusId);

			return View(order);
		}

        public async Task<IActionResult> DeleteCart(int? id)
        {
			if (id == null)
			{
				return NotFound();
			}
            
			var cart = _context.CartItem.FirstOrDefault(x=>x.Id==id);
			if (cart == null)
			{
				return NotFound();
			}
			_context.CartItem.Remove(cart);
			_context.SaveChanges();

            return RedirectToAction(nameof(CartIndex));
        }
		public async Task<IActionResult> ChangeQuantity(int id,int quantity)
		{
			if (id == null)
			{
				return NotFound();
			}

			var cart = _context.CartItem.FirstOrDefault(x => x.Id == id);
			if (cart == null)
			{
				return NotFound();
			}
			cart.quantity = quantity;
			_context.CartItem.Update(cart);
			_context.SaveChanges();

			return RedirectToAction(nameof(CartIndex));
		}
		// GET: Orders/Delete/5
		[Authorize(Roles = "Store Owner,Administrator")]

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Order == null)
			{
				return NotFound();
			}

			var order = await _context.Order
				.Include(o => o.Cart)
				.Include(o => o.Status)
				.Include(o => o.User)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

		[Authorize(Roles = "Store Owner,Administrator")]
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
