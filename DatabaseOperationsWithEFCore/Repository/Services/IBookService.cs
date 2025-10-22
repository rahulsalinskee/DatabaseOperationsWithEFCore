using DatabaseOperationsWithEFCore.DTOs.BookDTOs.AddBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.DeleteBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.UpdateBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseOperationsWithEFCore.Repository.Services
{
    public interface IBookService
    {
        public Task<ResponseDto> GetAllBooksAsync(string? filterOnColumn, string? filterKeyWord);

        public Task<ResponseDto> GetAllBooksByEagerLoadingAsync(string? filterOnColumn, string? filterKeyWord);

        public Task<ResponseDto> GetBookByTitleAsync(string title);

        public Task<ResponseDto> GetFirstBookByExplicitLoadingUsingReferenceAsync();

        //public Task<ResponseDto> GetAllLanguagesByExplicitLoadingUsingCollectionAsync();

        public Task<ResponseDto> AddBookAsync(AddBookDto addBookDto);

        public Task<ResponseDto> AddBooksAsync(AddBooksDto addNewBooksDto);

        public Task<ResponseDto> UpdateBookByTitleAsync(string title, UpdateBookDto updateBookDto);

        public Task<ResponseDto> UpdateBookWithSingleDatabaseHitAsync(UpdateBookDto updateBookDto);

        public Task<ResponseDto> UpdateBooksWithoutDatabaseSingleHitAsync(UpdateBooksDto updateBooksDto);

        public Task<ResponseDto> UpdateBooksWithSingleHitInDatabaseAsync(UpdateBooksDto updateBooksDto);

        public Task<ResponseDto> DeleteBookByTitleAsync(string title);

        public Task<ResponseDto> DeleteBookByIdWithOneDataBaseHitAsync(int id);

        public Task<ResponseDto> DeleteBulkRecordsAsync(int fromBookIdToDelete, int toBookIdToDelete);

        public Task<ResponseDto> DeleteBulkRecordsWithOneDatabaseHitAsync(int fromBookIdToDelete, int toBookIdToDelete);
    }
}
