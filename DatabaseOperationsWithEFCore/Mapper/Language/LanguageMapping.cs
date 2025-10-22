using DatabaseOperationsWithEFCore.DTOs.BookDTOs.BookDTO;
using DatabaseOperationsWithEFCore.DTOs.LanguageDTOs.LanguageDTO;

namespace DatabaseOperationsWithEFCore.Mapper.Language
{
    public static class LanguageMapping
    {
        public static Models.Language FromLanguageDtoToLanguageModelExtension(this LanguageDto languageDto)
        {
            return new Models.Language
            {
                Title = languageDto.Title,
                Description = languageDto.Description,
                Books = (ICollection<Models.Book>)languageDto.BooksDto
            };
        }

        public static LanguageDto FromLanguageModelToLanguageDtoExtension(this Models.Language language)
        {
            return new LanguageDto
            {
                Title = language.Title,
                Description = language.Description,
                BooksDto = (ICollection<BookDto>)language.Books
            };
        }
    }
}
