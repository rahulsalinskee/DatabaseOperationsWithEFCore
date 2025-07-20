using DatabaseOperationsWithEFCore.DTOs.BookDTOs.BookDTO;

namespace DatabaseOperationsWithEFCore.DTOs.LanguageDTOs.LanguageDTO
{
    public class LanguageDto
    {

        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<BookDto> BookDto { get; set; }
    }
}
