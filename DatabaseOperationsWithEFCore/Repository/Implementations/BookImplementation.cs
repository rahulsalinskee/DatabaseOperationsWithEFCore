using DatabaseOperationsWithEFCore.Data;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.AddBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.BookDTO;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.UpdateBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;
using DatabaseOperationsWithEFCore.Mapper.Book;
using DatabaseOperationsWithEFCore.Models;
using DatabaseOperationsWithEFCore.Repository.Services;
using DatabaseOperationsWithEFCore.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseOperationsWithEFCore.Repository.Implementations
{
    public class BookImplementation : IBookService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BookImplementation(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }

        public async Task<ResponseDto> AddBookAsync(AddBookDto addNewBookDto)
        {
            /* Validate the input for add new book */
            if (addNewBookDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Invalid book data!");
            }

            /* Check if the title is provided */
            if (string.IsNullOrEmpty(addNewBookDto.Title) || string.IsNullOrWhiteSpace(addNewBookDto.Title))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Book title is required!");
            }

            /* Checking for duplicate book */
            var isDuplicatedBook = Utility.IsDuplicated(propertyValue: addNewBookDto.Title, 
                existingEntities: _applicationDbContext.Books, propertySelector: book => book.Title);

            /* If duplicate book found, return duplication error */
            if (isDuplicatedBook)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Book title already exists!");
            }
            else
            {
                /* Create new Book DTO entity from add new book DTO */
                BookDto newBookDto = new()
                {
                    Title = addNewBookDto.Title,
                    Description = addNewBookDto?.Description,
                    CreatedOn = addNewBookDto.CreatedOn,
                    NumberOfPages = addNewBookDto.NumberOfPages,
                    LanguageID = addNewBookDto.LanguageId,
                    Author = addNewBookDto.Author,
                };

                /* Converting book DTO to book model */
                var newBook = newBookDto.FromBookDtoToBookModelExtension();

                /* Add new book model entity to track */
                await _applicationDbContext.Books.AddAsync(newBook);

                /* Save changes to database */
                await _applicationDbContext.SaveChangesAsync();

                /* Convert the saved book model back to book DTO to return as response */
                var addedNewBookDto = newBook.FromBookModelToBookDtoExtension();

                /* Return success response with added book DTO */
                return Utility.GetResponse(responseData: addedNewBookDto, isSuccess: true, message: "Book added successfully");
            }
        }

        public async Task<ResponseDto> AddBooksAsync(AddBooksDto addNewBooksDto)
        {
            /* Validate the input */
            if (addNewBooksDto is null || addNewBooksDto.Books is null || !addNewBooksDto.Books.Any())
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Invalid book data! No books provided.");
            }

            /* List to store validation errors */
            var validationErrors = new List<string>();

            /* List to store books to be added */
            var booksToAdd = new List<Book>();

            /* Get existing book titles for duplication check */
            var existingBookTitles = await _applicationDbContext.Books.Select(book => book.Title.ToLower()).ToListAsync();

            /* Track titles in current batch to prevent duplicates within the batch */
            var currentBatchTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var addBookDto in addNewBooksDto.Books)
            {
                /* Validate each book */
                if (string.IsNullOrWhiteSpace(addBookDto.Title))
                {
                    var errorMessage = "One or more books have empty titles.";
                    validationErrors.Add(errorMessage);

                    /* Stop on first error if configured */
                    if (addNewBooksDto.StopOnFirstError)
                    {
                        return Utility.GetResponse(responseData: null, isSuccess: false, message: errorMessage);
                    }
                    continue;
                }

                var title = addBookDto.Title;

                /* Check for duplicate in database */
                if (existingBookTitles.Contains(title.ToLower()))
                {
                    var errorMessage = $"Book with title '{title}' already exists in database.";
                    validationErrors.Add(errorMessage);

                    /* Stop on first error if configured */
                    if (addNewBooksDto.StopOnFirstError)
                    {
                        return Utility.GetResponse(responseData: null, isSuccess: false, message: errorMessage);
                    }
                    continue;
                }

                /* Check for duplicate within current batch */
                if (currentBatchTitles.Contains(title))
                {
                    var errorMessage = $"Duplicate title '{title}' found in the provided list.";
                    validationErrors.Add(errorMessage);

                    /* Stop on first error if configured */
                    if (addNewBooksDto.StopOnFirstError)
                    {
                        return Utility.GetResponse(responseData: null, isSuccess: false, message: errorMessage);
                    }
                    continue;
                }

                /* Add to current batch tracking */
                currentBatchTitles.Add(title);

                /* Create new Book DTO entity from add new book DTO */
                BookDto newBookDto = new()
                {
                    Title = title,
                    Description = addBookDto.Description?.Trim(),
                    CreatedOn = addBookDto.CreatedOn,
                    NumberOfPages = addBookDto.NumberOfPages,
                    LanguageID = addBookDto.LanguageId
                };

                /* Converting book DTO to book model */
                var newBook = newBookDto.FromBookDtoToBookModelExtension();

                /* Add to list of books to be inserted */
                booksToAdd.Add(newBook);
            }

            /* If ValidateAllBeforeInsert is true and there are errors, stop insertion */
            if (addNewBooksDto.ValidateAllBeforeInsert && validationErrors.Any())
            {
                return Utility.GetResponse
                (
                    responseData: new { ValidationErrors = validationErrors },
                    isSuccess: false,
                    message: $"Validation failed for {validationErrors.Count} books. No books were added."
                );
            }

            /* If no valid books to add, return error */
            if (!booksToAdd.Any())
            {
                var errorMessage = validationErrors.Any() 
                    ? $"No valid books to add. Errors: {string.Join("; ", validationErrors)}" 
                    : "No valid books to add.";
                
                return Utility.GetResponse(responseData: null, isSuccess: false, message: errorMessage);
            }

            /* Add all valid books to database in bulk */
            await _applicationDbContext.Books.AddRangeAsync(booksToAdd);

            /* Save changes to database */
            await _applicationDbContext.SaveChangesAsync();

            /* Convert the saved book models back to book DTOs to return as response */
            var addedBooksDtos = booksToAdd.Select(book => book.FromBookModelToBookDtoExtension()).ToList();

            /* Prepare response message */
            var successMessage = validationErrors.Any()
                ? $"{booksToAdd.Count} books added successfully. {validationErrors.Count} books skipped due to errors."
                : $"{booksToAdd.Count} books added successfully.";

            /* Return success response with added books DTOs */
            return Utility.GetResponse(
                responseData: new 
                { 
                    AddedBooks = addedBooksDtos,
                    TotalAdded = addedBooksDtos.Count,
                    TotalProvided = addNewBooksDto.Books.Count,
                    TotalSkipped = validationErrors.Count,
                    ValidationErrors = validationErrors.Any() ? validationErrors : null 
                }, 
                isSuccess: true, 
                message: successMessage);
        }

        public async Task<ResponseDto> DeleteBookByTitleAsync(string title)
        {
            var existingBook = await this._applicationDbContext.Books.FirstOrDefaultAsync(book => book.Title == title);

            if (existingBook is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No Book Found!");
            }
            else
            {
                /* Remove the existing book entity */
                this._applicationDbContext.Books.Remove(existingBook);

                /* Save changes to database */
                await this._applicationDbContext.SaveChangesAsync();

                /* Convert the deleted book model back to book DTO to return as response */
                var deletedBookDto = existingBook.FromBookModelToBookDtoExtension();

                /* Return success response with deleted book DTO */
                return Utility.GetResponse(responseData: deletedBookDto, isSuccess: true, message: "Book Deleted Successfully!");
            }
        }

        public async Task<ResponseDto> GetAllBooksAsync(string? filterOnColumn = null, string? filterKeyWord = null)
        {
            var books = await _applicationDbContext.Books.ToListAsync();

            if (books is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Books is null");
            }
            else
            {
                if (!books.Any())
                {
                    return Utility.GetResponse(responseData: null, isSuccess: false, message: "No books found");
                }
                else
                {
                    var booksDto = books.Select(book => book.FromBookModelToBookDtoExtension()).ToList();

                    return Utility.GetResponse(responseData: booksDto, isSuccess: true, message: "Books retrieved successfully");
                }
            }
        }

        public async Task<ResponseDto> GetBookByTitleAsync(string title)
        {
            var book = await _applicationDbContext.Books.FirstOrDefaultAsync(book => book.Title == title);

            if (book is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No Book Found!");
            }
            else
            {
                /* Convert book model to book DTO */
                var bookDto = book.FromBookModelToBookDtoExtension();

                /* Return the book DTO as response */
                return Utility.GetResponse(responseData: bookDto, isSuccess: true, message: "Book Retrieved Successfully!");
            }
        }

        public async Task<ResponseDto> UpdateBookByTitleAsync(string title, UpdateBookDto updateBookDto)
        {
            /* Validate the input for update book */
            if (updateBookDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Invalid book data!");
            }

            /* Check if the title is provided */
            if (string.IsNullOrEmpty(updateBookDto.Title) || string.IsNullOrWhiteSpace(updateBookDto.Title))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Book title can not be left blank or empty!");
            }

            /* Retrieve the existing book entity by title */
            var existingBook = await _applicationDbContext.Books.FirstOrDefaultAsync(book => book.Title == title);

            /* If the existing book does not exist, return not found error */
            if (existingBook is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Book with such title does not exists!");
            }
            else
            {
                /* Update the existing book model entity with new values from updateBookDto */
                existingBook.Title = updateBookDto.Title;
                existingBook.Description = updateBookDto.Description;
                existingBook.NumberOfPages = updateBookDto.NumberOfPages;
                existingBook.CreatedOn = updateBookDto.CreatedOn;
                existingBook.LanguageId = updateBookDto.LanguageId;

                /* Mark the entity as modified */
                _applicationDbContext.Books.Update(existingBook);

                /* Save changes to database */
                await _applicationDbContext.SaveChangesAsync();

                /* Convert the updated book model back to book DTO to return as response */
                var updatedBookDto = existingBook.FromBookModelToBookDtoExtension();

                /* Return success response with updated book DTO */
                return Utility.GetResponse(responseData: updatedBookDto, isSuccess: true, message: "Book updated successfully");
            }
        }
    }
}
