using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class ReturnViewModel
    {
        [Required]
        public int BorrowRecordId { get; set; }   // Identifie l'emprunt à clôturer 

        [BindNever]
        public string? BookTitle { get; set; }    // Affiché, jamais bindé 
        [BindNever]
        public string? BorrowerName { get; set; } // Affiché, jamais bindé 

        [BindNever]
        public DateTime? BorrowDate { get; set; } // Affiché, jamais bindé 
    }
}
