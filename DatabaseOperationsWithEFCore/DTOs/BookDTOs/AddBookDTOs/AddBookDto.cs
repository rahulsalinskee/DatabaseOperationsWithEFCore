using DatabaseOperationsWithEFCore.DTOs.LanguageDTOs.LanguageDTO;

namespace DatabaseOperationsWithEFCore.DTOs.BookDTOs.AddBookDTOs
{
    public class AddBookDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int NumberOfPages { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public int LanguageId { get; set; }
    }
}
