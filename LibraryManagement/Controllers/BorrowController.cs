using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class BorrowController : Controller
    {
        private readonly LibraryContext _context;

        public BorrowController(LibraryContext context)
        {
            _context = context;  // Injection du contexte DB par le DI container
        }
        // GET: /Books/Index
        // GET: /Borrow/Create?bookId=5 
        public async Task<IActionResult> Create(int? bookId)
        {
            if (bookId == null || bookId == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided."; return View("NotFound");
            }

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                TempData["ErrorMessage"] = $"No book found with ID {bookId}.";
                return View("NotFound");
            }

            // Vérification que le livre est disponible 
            if (!book.IsAvailable)
            {
                TempData["ErrorMessage"] = $"The book '{book.Title}' is not available.";
                return View("NotAvailable");  // Vue d'erreur spécifique 
            }

            // Création du ViewModel avec les données pour la vue 
            var borrowViewModel = new BorrowViewModel
            {
                BookId = book.BookId,
                BookTitle = book.Title   // Rempli manuellement, pas depuis le formulaire 
            };
            return View(borrowViewModel);
        }

        // POST: /Borrow/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BorrowViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var book = await _context.Books.FindAsync(model.BookId);
            if (book == null || !book.IsAvailable) { return View("NotAvailable"); }

            // Création de l'enregistrement d'emprunt 
            var borrowRecord = new BorrowRecord
            {
                BookId = book.BookId,
                BorrowerName = model.BorrowerName,
                BorrowerEmail = model.BorrowerEmail,
                Phone = model.Phone,
                BorrowDate = DateTime.UtcNow  // Date automatique 
            };

            book.IsAvailable = false;              // Le livre n'est plus disponible 
            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();     // Une seule transaction pour les deux 

            TempData["SuccessMessage"] = $"Successfully borrowed: {book.Title}.";

            return RedirectToAction("Index", "Books");
        }

        // GET: /Borrow/Return?borrowRecordId=3 
        public async Task<IActionResult> Return(int? borrowRecordId)
        {
            var borrowRecord = await _context.BorrowRecords
                .Include(br => br.Book)   // Charge le livre associé 
                .FirstOrDefaultAsync(br => br.BorrowRecordId == borrowRecordId);

            if (borrowRecord == null) { return View("NotFound"); }

            // Vérification : déjà rendu ? 
            if (borrowRecord.ReturnDate != null)
            {
                TempData["ErrorMessage"] = $"Already returned: {borrowRecord.Book.Title}.";
                return View("AlreadyReturned");
            }

            var returnViewModel = new ReturnViewModel
            {
                BorrowRecordId = borrowRecord.BorrowRecordId,
                BookTitle = borrowRecord.Book.Title,
                BorrowerName = borrowRecord.BorrowerName,
                BorrowDate = borrowRecord.BorrowDate
            };
            return View(returnViewModel);
        }

        // POST: /Borrow/Return 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(ReturnViewModel model)
        {
            var borrowRecord = await _context.BorrowRecords
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.BorrowRecordId == model.BorrowRecordId);

            if (borrowRecord.ReturnDate != null) { return View("AlreadyReturned"); }

            borrowRecord.ReturnDate = DateTime.UtcNow;  // Enregistre la date de retour 
            borrowRecord.Book.IsAvailable = true;        // Rend le livre disponible 
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully returned: {borrowRecord.Book.Title}.";
            return RedirectToAction("Index", "Books");
        }

    }
}
