using DatabaseOperationsWithEFCore.Models;

namespace DatabaseOperationsWithEFCore.DTOs.BookDTOs.UpdateBookDTOs
{
    public class UpdateBookDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int NumberOfPages { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int LanguageId { get; set; }

        //public Language? Language { get; set; }

        public int? AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
