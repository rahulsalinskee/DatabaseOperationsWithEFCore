using DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.AuthorDTO;
using System.ComponentModel.DataAnnotations;

namespace DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.AddAuthorDTOs
{
    public class AddAuthorsDto
    {
        [Required(ErrorMessage = "Books list cannot be null or empty")]
        [MinLength(1, ErrorMessage = "At least one book must be provided")]
        public ICollection<AuthorDto>? AuthorsDto { get; set; }

        /// <summary>
        /// If true, stops the entire operation if any book fails validation
        /// If false, inserts valid books and reports errors for invalid ones
        /// </summary>
        public bool StopOnFirstError { get; set; } = false;

        /// <summary>
        /// If true, validates all books before inserting any
        /// If false, validates and inserts in a single pass
        /// </summary>
        public bool ValidateAllBeforeInsert { get; set; } = true;
    }
}
