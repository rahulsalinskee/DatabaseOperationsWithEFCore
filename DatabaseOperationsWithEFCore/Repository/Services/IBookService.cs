using DatabaseOperationsWithEFCore.DTOs.BookDTOs.AddBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.UpdateBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseOperationsWithEFCore.Repository.Services
{
    public interface IBookService
    {
        public Task<ResponseDto> GetAllBooksAsync(string? filterOnColumn = null, string? filterKeyWord = null);

        public Task<ResponseDto> GetBookByTitleAsync(string title);

        public Task<ResponseDto> AddBookAsync(AddBookDto addBookDto);

        public Task<ResponseDto> AddBooksAsync(AddBooksDto addNewBooksDto);

        public Task<ResponseDto> UpdateBookByTitleAsync(string title, UpdateBookDto updateBookDto);

        public Task<ResponseDto> DeleteBookByTitleAsync(string title);
    }
}
