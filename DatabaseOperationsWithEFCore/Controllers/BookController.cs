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
        [HttpGet("GetAllBooks")]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery] string? filterOnColumn, [FromQuery] string? filterKeyWord)
        {
            var response = await bookService.GetAllBooksAsync(filterOnColumn: filterOnColumn, filterKeyWord: filterKeyWord);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("GetBookById/{id:int}")]
        public async Task<IActionResult> GetBookByTitleAsync([FromRoute] string title)
        {
            var response = await bookService.GetBookByTitleAsync(title: title);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

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

        [HttpPut("UpdateBook/{title}")]
        public async Task<IActionResult> UpdateBookByTitleAsync([FromRoute] string title, [FromBody] UpdateBookDto updateBookDto)
        {
            var response = await bookService.UpdateBookByTitleAsync(title: title, updateBookDto: updateBookDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("UpdateBookWithSingleDatabaseHit")]
        public async Task<IActionResult> UpdateBookWithSingleDatabaseHitAsync([FromBody] UpdateBookDto updateBookDto)
        {
            var response = await bookService.UpdateBookWithSingleDatabaseHitAsync(updateBookDto: updateBookDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("UpdateBooksWithoutDatabaseSingleHit")]
        public async Task<IActionResult> UpdateBooksWithoutDatabaseSingleHitAsync([FromBody] UpdateBooksDto updateBooksDto)
        {
            var response = await bookService.UpdateBooksWithoutDatabaseSingleHitAsync(updateBooksDto: updateBooksDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("UpdateBooksWithSingleHitInDatabase")]
        public async Task<IActionResult> UpdateBooksWithSingleHitInDatabaseAsync(UpdateBooksDto updateBooksDto)
        {
            var response = await bookService.UpdateBooksWithSingleHitInDatabaseAsync(updateBooksDto: updateBooksDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("DeleteBook/{title}")]
        public async Task<IActionResult> DeleteBookByTitleAsync([FromRoute] string title)
        {
            var response = await bookService.DeleteBookByTitleAsync(title: title);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpDelete("DeleteBookWithOneDataBaseHit/{id:int}")]
        public async Task<IActionResult> DeleteBookByIdWithOneDataBaseHitAsync([FromRoute] int id)
        {
            var response = await bookService.DeleteBookByIdWithOneDataBaseHitAsync(id: id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpDelete("DeleteBulkBooks/{fromBookIdToDelete:int}/{toBookIdToDelete:int}")]
        public async Task<IActionResult> DeleteBulkBooks([FromRoute] int fromBookIdToDelete, [FromRoute] int toBookIdToDelete)
        {
            var response = await bookService.DeleteBulkRecordsAsync(fromBookIdToDelete: fromBookIdToDelete, toBookIdToDelete: toBookIdToDelete);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpDelete("DeleteBulkRecordsWithOneDatabaseHit/{fromBookIdToDelete:int}/{toBookIdToDelete:int}")]
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
