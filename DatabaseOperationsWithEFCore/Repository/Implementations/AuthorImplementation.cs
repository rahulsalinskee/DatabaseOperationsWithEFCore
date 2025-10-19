using DatabaseOperationsWithEFCore.Data;
using DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.AddAuthorDTOs;
using DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.AuthorDTO;
using DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.UpdateAuthorDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;
using DatabaseOperationsWithEFCore.Mapper.Author;
using DatabaseOperationsWithEFCore.Models;
using DatabaseOperationsWithEFCore.Repository.Services;
using DatabaseOperationsWithEFCore.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseOperationsWithEFCore.Repository.Implementations
{
    public class AuthorImplementation : IAuthorService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IFilterService<Author> _filterOnAuthor;

        public AuthorImplementation(IFilterService<Author> filterOnAuthor, ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
            this._filterOnAuthor = filterOnAuthor;
        }

        public async Task<ResponseDto> AddNewAuthorAsync(AddAuthorDto addAuthorDto)
        {
            if (addAuthorDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Author data cannot be null!");
            }

            if (string.IsNullOrEmpty(addAuthorDto.Email) || string.IsNullOrWhiteSpace(addAuthorDto.Email))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Author email cannot be null or empty or white spaced!");
            }

            AuthorDto authorDto = new()
            {
                Name = addAuthorDto.Name,
                Email = addAuthorDto.Email,
            };

            var author = authorDto.FromAuthorDtoToAuthorModelExtension();

            await this._applicationDbContext.Authors.AddAsync(entity: author);
            await this._applicationDbContext.SaveChangesAsync();

            var addedAuthorDto = author.FromAuthorModelToAuthorDtoExtension();

            return Utility.GetResponse(responseData: addedAuthorDto, isSuccess: true, message: "Author added successfully.");
        }

        public async Task<ResponseDto> AddNewAuthorsAsync(AddAuthorsDto addNewAuthorsDto)
        {
            if (addNewAuthorsDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Invalid author data!");
            }

            if (addNewAuthorsDto.AuthorsDto.Any())
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No authors provided.");
            }

            if (addNewAuthorsDto.AuthorsDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Author list cannot be null!");
            }

            /* List to store validation errors */
            var validationErrors = new List<string>();

            /* List to store authors to be added */
            var authorsToAdd = new List<Author>();

            /* Get existing author emails for duplication check */
            var existingAuthorEmail = await _applicationDbContext.Authors.Select(author => author.Email.ToLower()).ToListAsync();

            /* Track Emails in current batch to prevent duplicates within the batch */
            var currentBatchEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var newAuthorDtoToAdd in addNewAuthorsDto.AuthorsDto)
            {
                if (newAuthorDtoToAdd is null)
                {
                    var errorMessage = "Author data cannot be null.";
                    if (addNewAuthorsDto.StopOnFirstError)
                    {
                        return Utility.GetResponse(responseData: null, isSuccess: false, message: errorMessage);
                    }
                    validationErrors.Add(errorMessage);
                    continue;
                }

                /* Check for duplicate in database */
                if (existingAuthorEmail.Contains(newAuthorDtoToAdd.Email.ToLower()))
                {
                    var errorMessage = $"Author with email '{newAuthorDtoToAdd.Email}' already exists in database.";
                    validationErrors.Add(errorMessage);

                    /* Stop on first error if configured */
                    if (addNewAuthorsDto.StopOnFirstError)
                    {
                        return Utility.GetResponse(responseData: null, isSuccess: false, message: errorMessage);
                    }
                    continue;
                }

                /* Check for duplicate within current batch */
                if (currentBatchEmails.Contains(newAuthorDtoToAdd.Email))
                {
                    var errorMessage = $"Duplicate Email '{newAuthorDtoToAdd.Email}' found in the provided list.";
                    validationErrors.Add(errorMessage);

                    /* Stop on first error if configured */
                    if (addNewAuthorsDto.StopOnFirstError)
                    {
                        return Utility.GetResponse(responseData: null, isSuccess: false, message: errorMessage);
                    }
                    continue;
                }

                currentBatchEmails.Add(newAuthorDtoToAdd.Email);

                AuthorDto authorDto = new()
                {
                    Name = newAuthorDtoToAdd.Name,
                    Email = newAuthorDtoToAdd.Email,
                };

                var author = authorDto.FromAuthorDtoToAuthorModelExtension();

                authorsToAdd.Add(author);
            }

            await this._applicationDbContext.Authors.AddRangeAsync(authorsToAdd);
            await this._applicationDbContext.SaveChangesAsync();

            var authorsDto = authorsToAdd.Select(author => author.FromAuthorModelToAuthorDtoExtension()).ToList();

            return Utility.GetResponse(responseData: new { AddedAuthors = authorsDto, ValidationErrors = validationErrors }, isSuccess: !validationErrors.Any(), message: validationErrors.Any() ? "Some authors were not added due to validation errors." : "All authors added successfully.");
        }

        public async Task<ResponseDto> DeleteAuthorByEmailAsync(string authorEmail)
        {
            if (string.IsNullOrEmpty(authorEmail) || string.IsNullOrWhiteSpace(authorEmail))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Author name cannot be null or empty or white spaced!");
            }

            var author = await this._applicationDbContext.Authors.FirstOrDefaultAsync(author => author.Email == authorEmail);

            if (author is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No such author found!");
            }
            else
            {
                this._applicationDbContext.Authors.Remove(author);
                await this._applicationDbContext.SaveChangesAsync();

                var authorDto = author.FromAuthorModelToAuthorDtoExtension();

                return Utility.GetResponse(responseData: authorDto, isSuccess: true, message: "Author deleted successfully.");
            }
        }

        public async Task<ResponseDto> GetAllAuthorsAsync(string? filterOnColumn = null, string? filterKeyWord = null)
        {
            var authors = await this._applicationDbContext.Authors.ToListAsync();

            if (authors is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Author is Null!");
            }

            if (!authors.Any())
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No authors found!");
            }
            else
            {
                var filteredAuthors = this._filterOnAuthor.ApplyFilterOn(queryOn: authors.AsQueryable(), columnName: filterOnColumn, filterKeyWord: filterKeyWord);

                if (!filteredAuthors.Any())
                {
                    return Utility.GetResponse(responseData: null, isSuccess: false, message: "No authors found with the specified filter criteria.");
                }

                var authorsDto = filteredAuthors?.Select(author => author.FromAuthorModelToAuthorDtoExtension()).ToList();

                return Utility.GetResponse(responseData: authorsDto, isSuccess: true, message: "Authors retrieved successfully.");
            }
        }

        public async Task<ResponseDto> GetAuthorByEmailAsync(string authorEmail)
        {
            if (string.IsNullOrEmpty(authorEmail) || string.IsNullOrWhiteSpace(authorEmail))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Author email cannot be null or empty or white spaced!");
            }
            var author = await this._applicationDbContext.Authors.FirstOrDefaultAsync(author => author.Email == authorEmail);

            if (author is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No such author found!");
            }
            else
            {
                var authorDto = author.FromAuthorModelToAuthorDtoExtension();
                return Utility.GetResponse(responseData: authorDto, isSuccess: true, message: "Author retrieved successfully.");
            }
        }

        public async Task<ResponseDto> UpdateAuthorByEmailAsync(string authorEmail, UpdateAuthorDto updateAuthorDto)
        {
            if (updateAuthorDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Author data cannot be null!");
            }

            if (string.IsNullOrEmpty(authorEmail) || string.IsNullOrWhiteSpace(authorEmail))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Author email cannot be null or empty or white spaced!");
            }

            var fetchedAuthorByEmail = await this._applicationDbContext.Authors.FirstOrDefaultAsync(author => author.Email == authorEmail);

            if (fetchedAuthorByEmail is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No such author found!");
            }
            else
            {
                fetchedAuthorByEmail.Name = updateAuthorDto.Name;
                fetchedAuthorByEmail.Email = updateAuthorDto.Email;

                this._applicationDbContext.Authors.Update(fetchedAuthorByEmail);
                await this._applicationDbContext.SaveChangesAsync();

                var fetchedAuthorByEmailDto = fetchedAuthorByEmail.FromAuthorModelToAuthorDtoExtension();

                return Utility.GetResponse(responseData: fetchedAuthorByEmailDto, isSuccess: true, message: "Author updated successfully.");
            }
        }
    }
}
