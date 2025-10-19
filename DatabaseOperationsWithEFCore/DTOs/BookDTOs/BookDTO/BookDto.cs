using DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.AuthorDTO;
using DatabaseOperationsWithEFCore.Models;

namespace DatabaseOperationsWithEFCore.DTOs.BookDTOs.BookDTO
{
    public class BookDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int NumberOfPages { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public int LanguageID { get; set; }

        //public Language? Language { get; set; }

        public int? AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
