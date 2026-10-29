using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibraryManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;  // Injection du contexte DB par le DI container
        }
        // GET: /Books/Index 
        public async Task<IActionResult> Index()
        {
            try
            {
                var books = await _context.Books
                    .Include(b => b.BorrowRecords)  // Charge les emprunts liés 
                    .AsNoTracking()                  // Lecture seule, plus performant
                    .ToListAsync();
                return View(books);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the books.";
                return View("Error");
            }
        }

        // GET: /Books/Details/5 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided."; return View("NotFound");
            }
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(m => m.BookId == id);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {id}."; return View("NotFound");
                }
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred."; return View("Error");
            }
        }

        // GET: /Books/Create 
        public IActionResult Create() { return View(); }

        // POST: /Books/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // BookId et IsAvailable ignorés grâce à [BindNever] 
                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Successfully added: {book.Title}.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error adding book."; return View(book);
                }
            }
            return View(book);  // Retourne la vue avec les erreurs de validation
        }

        // GET: /Books/Edit/5 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0) { return View("NotFound"); }
            var book = await
            _context.Books.AsNoTracking().FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null) { return View("NotFound"); }
            return View(book);
        }

        // POST: /Books/Edit/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Book book)
        {
            if (ModelState.IsValid)
            {
                var existingBook = await _context.Books.FindAsync(id);
                if (existingBook == null) { return View("NotFound"); }
                // On met à jour manuellement les champs modifiables 
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.PublishedDate = book.PublishedDate;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Successfully updated: { book.Title}."; 
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: /Books/Delete/5 
        // GET: /Books/Delete/5 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided.";
                return View("NotFound");
            }
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(m => m.BookId == id);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {id}.";
                    return View("NotFound");
                }
                return View(book);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred.";
                return View("Error");
            }
        }

        // POST: /Books/Delete/5 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) { return View("NotFound"); }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Successfully deleted: { book.Title}."; 
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id) => _context.Books.Any(e => e.BookId ==id);
    }
}
