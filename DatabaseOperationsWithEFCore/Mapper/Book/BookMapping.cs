using DatabaseOperationsWithEFCore.DTOs.BookDTOs.BookDTO;
using DatabaseOperationsWithEFCore.Models;

namespace DatabaseOperationsWithEFCore.Mapper.Book
{
    public static class BookMapping
    {
        /// <summary>
        /// Converts a <see cref="BookDto"/> instance to a <see cref="Models.Book"/> instance.
        /// </summary>
        /// <param name="bookDto"></param>
        /// <returns></returns>
        public static Models.Book FromBookDtoToBookModelExtension(this BookDto bookDto)
        {
            return new Models.Book()
            {
                Title = bookDto.Title,
                Description = bookDto.Description,
                NumberOfPages = bookDto.NumberOfPages,
                IsActive = bookDto.IsActive,
                CreatedOn = bookDto.CreatedOn,
                LanguageId = bookDto.LanguageID,
                //Language = bookDto.Language,
                AuthorId = bookDto.Author?.Id,
                Author = bookDto.Author,
            };
        }

        /// <summary>
        /// Converts a <see cref="Models.Book"/> instance to a <see cref="BookDto"/> instance.
        /// </summary>
        /// <param name="book">The <see cref="Models.Book"/> instance to convert. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="BookDto"/> instance containing the mapped data from the specified <see cref="Models.Book"/>.</returns>
        public static BookDto FromBookModelToBookDtoExtension(this Models.Book book)
        {
            return new BookDto()
            {
                Title = book.Title,
                Description = book.Description,
                NumberOfPages = book.NumberOfPages,
                IsActive = book.IsActive,
                CreatedOn = book.CreatedOn,
                LanguageID = book.LanguageId,
                //Language = book.Language,
                AuthorId = book.Author?.Id,
                Author = book.Author,
            };
        }
    }
}
