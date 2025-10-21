using DatabaseOperationsWithEFCore.Data;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.AddBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.BookDTO;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.DeleteBookDTOs;
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

        public async Task<ResponseDto> DeleteBookByIdWithOneDataBaseHitAsync(int id)
        {
            Book book = new() { Id = id };

            this._applicationDbContext.Entry<Book>(book).State = EntityState.Deleted;

            await this._applicationDbContext.SaveChangesAsync();

            return Utility.GetResponse(responseData: book, isSuccess: true, message: "Book Deleted Successfully!");
        }

        public async Task<ResponseDto> DeleteBulkRecordsAsync(int fromBookIdToDelete, int toBookIdToDelete)
        {
            if ((fromBookIdToDelete <= 0) || (toBookIdToDelete <= 0) || (fromBookIdToDelete > toBookIdToDelete) || (fromBookIdToDelete == 0 && toBookIdToDelete == 0))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Invalid book ID range provided for deletion.");
            }

            if (fromBookIdToDelete == toBookIdToDelete)
            {
                return await DeleteBookByIdWithOneDataBaseHitAsync(id: fromBookIdToDelete);
            }

            /* Check if any books exist in the range */
            var doesAnyBookExistInThisRange = await this._applicationDbContext.Books.AnyAsync(book => book.Id >= fromBookIdToDelete && book.Id <= toBookIdToDelete);

            if (!doesAnyBookExistInThisRange)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No books found in the specified ID range for deletion.");
            }

            /* Get all book IDs that exist within the range */
            var bookIdsExistWithinRangeToDelete = await this._applicationDbContext.Books.Where<Book>(book => book.Id >= fromBookIdToDelete && book.Id <= toBookIdToDelete).Select(book => book.Id).ToListAsync();

            /* Generate the complete range of IDs expected from parameters */
            var expectedBookIds = Enumerable.Range(fromBookIdToDelete, toBookIdToDelete - fromBookIdToDelete + 1).ToList();

            /* Find missing IDs in the range */
            var missingBookIds = expectedBookIds.Except(bookIdsExistWithinRangeToDelete).ToList();

            /* Check if all IDs in the range exist in the database */
            if (missingBookIds.Any())
            {
                return Utility.GetResponse(responseData: new { MissingBookIds = missingBookIds }, isSuccess: false, message: $"Cannot delete: The following book IDs are missing from the range: { string.Join(", ", missingBookIds) }");
            }

            /* Fetch books to delete (Already validated they all exist in database) */
            var booksInRange = await this._applicationDbContext.Books.Where(book => book.Id >= fromBookIdToDelete && book.Id <= toBookIdToDelete).ToListAsync();

            /* Deleting the range of Book Ids which needs to be deleted */
            this._applicationDbContext.Books.RemoveRange(booksInRange);

            await this._applicationDbContext.SaveChangesAsync();

            return Utility.GetResponse(responseData: new { BooksInRange = booksInRange }, isSuccess: true, message: $"{booksInRange} books are deleted successfully");
        }

        public async Task<ResponseDto> DeleteBulkRecordsWithOneDatabaseHitAsync(int fromBookIdToDelete, int toBookIdToDelete)
        {
            // Validate input range
            if ((fromBookIdToDelete <= 0) || (toBookIdToDelete <= 0) || (fromBookIdToDelete > toBookIdToDelete))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Invalid book ID range provided for deletion.");
            }

            if (fromBookIdToDelete == toBookIdToDelete)
            {
                return await DeleteBookByIdWithOneDataBaseHitAsync(id: fromBookIdToDelete);
            }

            // First, validate that all IDs exist (one query)
            var existingBookIds = await this._applicationDbContext.Books.Where(book => book.Id >= fromBookIdToDelete && book.Id <= toBookIdToDelete).Select(book => book.Id).ToListAsync();

            if (!existingBookIds.Any())
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No books found in the specified ID range for deletion.");
            }

            var expectedBookIds = Enumerable.Range(fromBookIdToDelete, toBookIdToDelete - fromBookIdToDelete + 1).ToList();
            var missingBookIds = expectedBookIds.Except(existingBookIds).ToList();

            if (missingBookIds.Any())
            {
                return Utility.GetResponse(
                    responseData: new { MissingBookIds = missingBookIds },
                    isSuccess: false,
                    message: $"Cannot delete: Missing book IDs: {string.Join(", ", missingBookIds)}");
            }

            /* Setting Entry state as Deleted */
            //this._applicationDbContext.Entry<Book>(new Book()).State = EntityState.Deleted;

            // Execute delete directly in database (EF Core 7+)
            var deletedCount = await this._applicationDbContext.Books.Where(book => book.Id >= fromBookIdToDelete && book.Id <= toBookIdToDelete).ExecuteDeleteAsync();

            return Utility.GetResponse(
                responseData: new { DeletedCount = deletedCount, DeletedBookIds = existingBookIds },
                isSuccess: true,
                message: $"{deletedCount} book(s) deleted successfully from ID {fromBookIdToDelete} to {toBookIdToDelete}");
        }

        public async Task<ResponseDto> GetAllBooksAsync(string? filterOnColumn, string? filterKeyWord)
        {
            var books = await _applicationDbContext.Books.AsNoTracking().ToListAsync();

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
                    var booksDto = books.Select(book => new { ResponseData = book.FromBookModelToBookDtoExtension(), AuthorName = book.Author?.Name ?? "Unknown" }).ToList();

                    return Utility.GetResponse(responseData: booksDto, isSuccess: true, message: "Books retrieved successfully");
                }
            }
        }

        public async Task<ResponseDto> GetAllBooksByEagerLoadingAsync(string? filterOnColumn, string? filterKeyWord)
        {
            var books = await this._applicationDbContext.Books.AsNoTracking().Include(book => book.Author).ToListAsync();

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
                    var booksDto = books.Select(book => new { ResponseData = book.FromBookModelToBookDtoExtension() }).ToList();
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

        public async Task<ResponseDto> UpdateBookWithSingleDatabaseHitAsync(UpdateBookDto updateBookDto)
        {
            BookDto bookDto = new()
            {
                Id = updateBookDto.Id, // ← MISSING - This is essential!
                Title = updateBookDto.Title,
                Description = updateBookDto.Description,
                NumberOfPages = updateBookDto.NumberOfPages,
                CreatedOn = updateBookDto.CreatedOn,
                LanguageID = updateBookDto.LanguageId,
                IsActive = updateBookDto.IsActive,
                AuthorId = updateBookDto.Author?.Id ?? updateBookDto.AuthorId,
                Author = updateBookDto.Author,
            };

            var book = bookDto.FromBookDtoToBookModelExtension();

            this._applicationDbContext.Entry<Book>(book).State = EntityState.Modified;

            await _applicationDbContext.SaveChangesAsync();

            var updatedBookDto = book.FromBookModelToBookDtoExtension();

            return Utility.GetResponse(responseData: updatedBookDto, isSuccess: true, message: "Book updated successfully");
        }

        public async Task<ResponseDto> UpdateBooksWithoutDatabaseSingleHitAsync(UpdateBooksDto updateBooksDto)
        {
            // Get IDs from the update request
            var bookIdsToUpdate = updateBooksDto.UpdateBookDto.Select(updateBooksDto => updateBooksDto.Id).ToList();

            // Fetch only the books that need to be updated
            var booksToUpdate = await this._applicationDbContext.Books.Where(book => bookIdsToUpdate.Contains(book.Id)).ToListAsync();

            // Create dictionary for fast lookup
            var updateDictionaryWithUpdateBookDto = updateBooksDto.UpdateBookDto.ToDictionary(updateBookDto => updateBookDto.Id);

            // Update each book
            foreach (var bookToUpdate in booksToUpdate)
            {
                if (updateDictionaryWithUpdateBookDto.TryGetValue(bookToUpdate.Id, out var updateData))
                {
                    bookToUpdate.Title = updateData.Title;
                    bookToUpdate.NumberOfPages += 10;
                }
            }

            // Save all changes in a single database hit
            await this._applicationDbContext.SaveChangesAsync();

            // Convert to DTOs for response
            var booksDto = booksToUpdate.Select(book => book.FromBookModelToBookDtoExtension()).ToList();

            return Utility.GetResponse(responseData: booksDto, isSuccess: true, message: "Books updated successfully");
        }

        public async Task<ResponseDto> UpdateBooksWithSingleHitInDatabaseAsync(UpdateBooksDto updateBooksDto)
        {
            /* 
            * ExecuteUpdateAsync = It can only set the same value or use simple expressions for all records. 
            * We cannot use it to update each book with a different title from the coming DTO. 
            */

            var rowsAffected = await this._applicationDbContext.Books.Where(book => book.AuthorId == null).
                ExecuteUpdateAsync(property => property
                    .SetProperty(book => book.Title, book => "Updated " + book.Title)
                    .SetProperty(book => book.NumberOfPages, book => book.NumberOfPages + 10)
                );

            return Utility.GetResponse(responseData: new { RowsAffected = rowsAffected }, isSuccess: true, message: $"{rowsAffected} books are updated successfully");
        }
    }
}
