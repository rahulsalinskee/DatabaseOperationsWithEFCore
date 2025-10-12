using DatabaseOperationsWithEFCore.DTOs.BookDTOs.AddBookDTOs;
using DatabaseOperationsWithEFCore.DTOs.BookDTOs.UpdateBookDTOs;
using DatabaseOperationsWithEFCore.Repository.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBookAsync([FromBody] AddBookDto addBookDto)
        {
            var response = await bookService.AddBookAsync(addBookDto: addBookDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("AddBooks")]
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
    }
}
