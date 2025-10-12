using DatabaseOperationsWithEFCore.DTOs.LanguageDTOs.LanguageDTO;

namespace DatabaseOperationsWithEFCore.DTOs.BookDTOs.UpdateBookDTOs
{
    public class UpdateBookDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int NumberOfPages { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int LanguageId { get; set; }
    }
}
