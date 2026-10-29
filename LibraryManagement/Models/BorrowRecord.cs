using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class BorrowRecord
    {
        [Key]
        public int BorrowRecordId { get; set; }    // Clé primaire 

        [Required]
        public int BookId { get; set; }             // Clé étrangère vers Book 

        [Required(ErrorMessage = "Please enter Borrower Name")]
        public string BorrowerName { get; set; }

        [Required(ErrorMessage = "Please enter Borrower Email Address")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email Address")]
        public string BorrowerEmail { get; set; }

        [Required(ErrorMessage = "Please enter Borrower Phone Number")]
        [Phone(ErrorMessage = "Please enter a valid Phone Number")]
        public string Phone { get; set; }

        [BindNever]
        [DataType(DataType.DateTime)]
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;  // Auto à la création

        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; }   // null = pas encore rendu 

        [BindNever]
        public Book Book { get; set; }              // Propriété de navigation 

    }
}
