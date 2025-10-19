using DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.AddAuthorDTOs;
using DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.UpdateAuthorDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;

namespace DatabaseOperationsWithEFCore.Repository.Services
{
    public interface IAuthorService
    {
        public Task<ResponseDto> GetAllAuthorsAsync(string? filterOnColumn = null, string? filterKeyWord = null);

        public Task<ResponseDto> GetAuthorByEmailAsync(string authorEmail);

        public Task<ResponseDto> AddNewAuthorAsync(AddAuthorDto addAuthorDto);

        public Task<ResponseDto> AddNewAuthorsAsync(AddAuthorsDto addNewAuthorsDto);

        public Task<ResponseDto> UpdateAuthorByEmailAsync(string authorEmail, UpdateAuthorDto updateAuthorDto);

        public Task<ResponseDto> DeleteAuthorByEmailAsync(string authorEmail);
    }
}
