using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.AddCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.UpdateCurrencyDTOs;
using DatabaseOperationsWithEFCore.Repository.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseOperationsWithEFCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            this._currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies([FromQuery] string? filterOnColumn, [FromQuery] string? filterKeyWord)
        {
            var response = await this._currencyService.GetAllCurrenciesAsync(columnName: filterOnColumn, filterKeyWord: filterKeyWord);

            if (response?.IsSuccess is false)
            {
                return NotFound(new { Message = "No currencies found." });
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCurrencyById([FromRoute] int id)
        {
            var response = await this._currencyService.GetCurrencyByIdAsync(id);
            if (response?.IsSuccess is false)
            {
                return NotFound(new { Message = $"Currency with ID {id} not found." });
            }
            return Ok(response);
        }

        [HttpGet("{title}")]
        public async Task<IActionResult> GetCurrencyByTitle([FromRoute] string title)
        {
            var response = await this._currencyService.GetCurrencyByTitleAsync(title);
            if (response?.IsSuccess is false)
            {
                return NotFound(new { Message = $"Currency with title '{title}' not found." });
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddCurrency([FromBody] AddNewCurrencyDto addCurrencyDto)
        {
            if (addCurrencyDto is null)
            {
                return BadRequest(new { Message = "Invalid currency data." });
            }
            var response = await this._currencyService.AddCurrencyAsync(addCurrencyDto: addCurrencyDto);
            if (response?.IsSuccess is false)
            {
                return BadRequest(response);
            }
            return CreatedAtAction(nameof(GetCurrencyById), new { id = response?.Response }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCurrencyById([FromRoute] int id, [FromBody] UpdateCurrencyDto updateCurrencyDto)
        {
            if (updateCurrencyDto is null)
            {
                return BadRequest(new { Message = "Invalid currency data." });
            }
            var response = await this._currencyService.UpdateCurrencyByIdAsync(id: id, updateCurrencyDto: updateCurrencyDto);
            if (response?.IsSuccess is false)
            {
                return NotFound(new { Message = $"Currency with ID {id} not found." });
            }
            return Ok(response);
        }

        [HttpPut("{title}")]
        public async Task<IActionResult> UpdateCurrencyByTitle([FromRoute] string title, [FromBody] UpdateCurrencyDto updateCurrencyDto)
        {
            if (updateCurrencyDto is null)
            {
                return BadRequest(new { Message = "Invalid currency data." });
            }
            var response = await this._currencyService.UpdateCurrencyByTitleAsync(title: title, updateCurrencyDto: updateCurrencyDto);
            if (response?.IsSuccess is false)
            {
                return NotFound(new { Message = $"Currency with title '{title}' not found." });
            }
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCurrencyById([FromRoute] int id)
        {
            var response = await this._currencyService.DeleteCurrencyByIdAsync(id: id);
            if (response?.IsSuccess is false)
            {
                return NotFound(new { Message = $"Currency with ID {id} not found." });
            }
            return Ok(response);
        }

        [HttpDelete("{title}")]
        public async Task<IActionResult> DeleteCurrencyByTitle([FromRoute] string title)
        {
            var response = await this._currencyService.DeleteCurrencyByTitleAsync(title: title);
            if (response?.IsSuccess is false)
            {
                return NotFound(new { Message = $"Currency with title '{title}' not found." });
            }
            return Ok(response);
        }
    }
}