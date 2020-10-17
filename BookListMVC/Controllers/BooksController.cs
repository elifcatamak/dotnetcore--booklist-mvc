using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;

        // On post, we don't have to retrieve it (it will automatically be binded)
        [BindProperty]
        public Book Book { get; set; }

        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Id is needed for edit operation so it's nullable
        public IActionResult Upsert(int? id)
        {
            Book = new Book();

            if(id == null)
            {
                // For create
                return View(Book);
            }

            // For update
            Book = _db.Books.FirstOrDefault(b => b.Id == id);

            if(Book == null)
            {
                return NotFound();
            }

            return View(Book);
        }

        // With post methods we use ValidateAntiForgeryToken to use the built-in security to prevent some attacks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            // First thing we should check is if the model state is valid or not
            if (ModelState.IsValid)
            {
                if(Book.Id == 0)
                {
                    // Create
                    _db.Books.Add(Book);
                }
                else
                {
                    // Update
                    _db.Books.Update(Book);
                }

                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(Book);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
