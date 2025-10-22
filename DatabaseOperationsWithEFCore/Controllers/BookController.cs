using DatabaseOperationsWithEFCore.DTOs.BookDTOs.AddBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.UpdateBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;
using DatabaseOperationsWithEFCore.Repository.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace DatabaseOperationsWithEFCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(IBookService bookService) : ControllerBase
    {
        [HttpGet("Books")]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery] string? filterOnColumn, [FromQuery] string? filterKeyWord)
        {
            var response = await bookService.GetAllBooksAsync(filterOnColumn: filterOnColumn, filterKeyWord: filterKeyWord);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("get-all-books-by-eager-loading-books/")]
        public async Task<IActionResult> GetAllBooksByEagerLoadingAsync([FromQuery] string? filterOnColumn = null, [FromQuery] string? filterKeyWord = null)
        {
            var response = await bookService.GetAllBooksByEagerLoadingAsync(filterOnColumn: filterOnColumn, filterKeyWord: filterKeyWord);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("book/{id:int}")]
        public async Task<IActionResult> GetBookByTitleAsync([FromRoute] string title)
        {
            var response = await bookService.GetBookByTitleAsync(title: title);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("first-book-by-explicit-loading-using-reference")]
        public async Task<IActionResult> GetFirstBookByExplicitLoadingUsingReferenceAsync()
        {
            var response = await bookService.GetFirstBookByExplicitLoadingUsingReferenceAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        //[HttpGet("books/all-languages-by-explicit-loading-using-collection")]
        //public async Task<IActionResult> GetAllLanguagesByExplicitLoadingUsingCollectionAsync()
        //{
        //    var response = await bookService.GetAllLanguagesByExplicitLoadingUsingCollectionAsync();

        //    if (response.IsSuccess)
        //    {
        //        return Ok(response);
        //    }

        //    return BadRequest(response);
        //}

        [HttpPost("add-book")]
        public async Task<IActionResult> AddBookAsync([FromBody] AddBookDto addBookDto)
        {
            var response = await bookService.AddBookAsync(addBookDto: addBookDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("books")]
        public async Task<IActionResult> AddBooksAsync([FromBody] AddBooksDto addBooksDto)
        {
            var response = await bookService.AddBooksAsync(addNewBooksDto: addBooksDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update-book/{title}")]
        public async Task<IActionResult> UpdateBookByTitleAsync([FromRoute] string title, [FromBody] UpdateBookDto updateBookDto)
        {
            var response = await bookService.UpdateBookByTitleAsync(title: title, updateBookDto: updateBookDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update-book-with-single-database-hit")]
        public async Task<IActionResult> UpdateBookWithSingleDatabaseHitAsync([FromBody] UpdateBookDto updateBookDto)
        {
            var response = await bookService.UpdateBookWithSingleDatabaseHitAsync(updateBookDto: updateBookDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update-books-without-database-single-hit")]
        public async Task<IActionResult> UpdateBooksWithoutDatabaseSingleHitAsync([FromBody] UpdateBooksDto updateBooksDto)
        {
            var response = await bookService.UpdateBooksWithoutDatabaseSingleHitAsync(updateBooksDto: updateBooksDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update-books-with-single-hit-in-database")]
        public async Task<IActionResult> UpdateBooksWithSingleHitInDatabaseAsync(UpdateBooksDto updateBooksDto)
        {
            var response = await bookService.UpdateBooksWithSingleHitInDatabaseAsync(updateBooksDto: updateBooksDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("delete-book/{title}")]
        public async Task<IActionResult> DeleteBookByTitleAsync([FromRoute] string title)
        {
            var response = await bookService.DeleteBookByTitleAsync(title: title);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpDelete("delete-book-with-one-database-hit/{id:int}")]
        public async Task<IActionResult> DeleteBookByIdWithOneDataBaseHitAsync([FromRoute] int id)
        {
            var response = await bookService.DeleteBookByIdWithOneDataBaseHitAsync(id: id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpDelete("delete-bulk-books/{fromBookIdToDelete:int}/{toBookIdToDelete:int}")]
        public async Task<IActionResult> DeleteBulkBooks([FromRoute] int fromBookIdToDelete, [FromRoute] int toBookIdToDelete)
        {
            var response = await bookService.DeleteBulkRecordsAsync(fromBookIdToDelete: fromBookIdToDelete, toBookIdToDelete: toBookIdToDelete);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpDelete("delete-bulk-records-with-one-database-hit/{fromBookIdToDelete:int}/{toBookIdToDelete:int}")]
        public async Task<IActionResult> DeleteBulkRecordsWithOneDatabaseHitAsync(int fromBookIdToDelete, int toBookIdToDelete)
        {
            var response = await bookService.DeleteBulkRecordsWithOneDatabaseHitAsync(fromBookIdToDelete: fromBookIdToDelete, toBookIdToDelete: toBookIdToDelete);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
    }
}
