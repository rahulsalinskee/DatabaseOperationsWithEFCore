using DatabaseOperationsWithEFCore.Data;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.AddCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.CurrencyDTO;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.UpdateCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;
using DatabaseOperationsWithEFCore.Mapper.Currency;
using DatabaseOperationsWithEFCore.Models;
using DatabaseOperationsWithEFCore.Repository.Services;
using DatabaseOperationsWithEFCore.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseOperationsWithEFCore.Repository.Implementations
{
    public class CurrencyImplementation : ICurrencyService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IFilterService<Currency> _filterOnCurrency;

        public CurrencyImplementation(ApplicationDbContext applicationDbContext, IFilterService<Currency> filterOnCurrency)
        {
            this._applicationDbContext = applicationDbContext;
            this._filterOnCurrency = filterOnCurrency;
        }

        public async Task<ResponseDto?> AddCurrencyAsync(AddNewCurrencyDto addNewCurrencyDto)
        {
            /* Check if new currency DTO is null */
            if (addNewCurrencyDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "New currency data cannot be null.");
            }

            /* Check if new currency Title is null or blank or whitespace */
            if (string.IsNullOrWhiteSpace(addNewCurrencyDto.Title) || string.IsNullOrEmpty(addNewCurrencyDto.Title))
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Currency title cannot be null or empty or white space.");
            }

            /* Check if a currency with the same title already exists */
            var isCurrencyDuplicated = Utility.IsDuplicated(propertyValue: addNewCurrencyDto.Title, existingEntities: this._applicationDbContext.Currencies, propertySelector: currency => currency.Title);

            if (isCurrencyDuplicated)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Currency with the same title already exists.");
            }
            else
            {
                /* Create new Currency entity */
                CurrencyDto newCurrencyDto = new()
                {
                    Title = addNewCurrencyDto.Title.Trim(),
                    Description = addNewCurrencyDto.Description?.Trim()
                };

                /* Convert Currency DTO to Currency Model */
                var newCurrency = newCurrencyDto.FromCurrencyDtoToCurrencyModelExtension();

                /* Add Currency */
                await _applicationDbContext.Currencies.AddAsync(newCurrency);

                /* Save Changes */
                await _applicationDbContext.SaveChangesAsync();

                /* Convert the saved Currency Model back to Currency DTO */
                var convertedCurrencyModelToCurrencyDto = newCurrency.FromCurrencyModelToCurrencyDtoExtension();

                /* Return Response */
                return Utility.GetResponse(responseData: convertedCurrencyModelToCurrencyDto, isSuccess: true, message: "Currency added successfully.");
            }
        }

        public async Task<ResponseDto?> DeleteCurrencyByIdAsync(int id)
        {
            var currencyById = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Id == id);

            if (currencyById is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {id}");
            }
            else
            {
                this._applicationDbContext.Currencies.Remove(currencyById);
                await this._applicationDbContext.SaveChangesAsync();
                return Utility.GetResponse(responseData: null, isSuccess: true, message: "Currency deleted successfully.");
            }
        }

        public async Task<ResponseDto?> DeleteCurrencyByTitleAsync(string title)
        {
            var currencyByTitle = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Title == title);

            if (currencyByTitle is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {title}");
            }
            else
            {
                this._applicationDbContext.Currencies.Remove(currencyByTitle);
                await this._applicationDbContext.SaveChangesAsync();
                return Utility.GetResponse(responseData: null, isSuccess: true, message: "Currency deleted successfully.");
            }
        }

        public async Task<ResponseDto?> DeleteCurrencyByIdAndTitleAsync(int id, string title)
        {
            var currencyByIdAndTitle = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Id == id && currency.Title == title);
            if (currencyByIdAndTitle is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {id} and Title - {title}");
            }
            else
            {
                this._applicationDbContext.Currencies.Remove(currencyByIdAndTitle);
                await this._applicationDbContext.SaveChangesAsync();
                return Utility.GetResponse(responseData: null, isSuccess: true, message: "Currency deleted successfully.");
            }
        }

        public async Task<ResponseDto?> GetAllCurrenciesAsync(string? columnName = null, string? filterKeyWord = null)
        {
            var currencies = this._applicationDbContext.Currencies.Include(currency => currency.BookPrices).Select(currency => new Currency() { Id = currency.Id, Title = currency.Title }).AsQueryable();

            if (currencies.Any())
            {
                var filteredCurrencies = this._filterOnCurrency.ApplyFilterOn(queryOn: currencies, columnName: columnName, filterKeyWord: filterKeyWord);

                var results = await filteredCurrencies.Select(currency => currency.FromCurrencyModelToCurrencyDtoExtension()).ToListAsync();

                return Utility.GetResponse(responseData: results, isSuccess: true, message: "Currencies retrieved successfully.");
            }
            else
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No currencies found.");
            }
        }

        public async Task<ResponseDto?> GetCurrencyByIdAsync(int id)
        {
            var currencyById = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Id == id);

            if (currencyById is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {id}");
            }
            else
            {
                var currencyDtoById = currencyById.FromCurrencyModelToCurrencyDtoExtension();

                return Utility.GetResponse(responseData: currencyDtoById, isSuccess: true, message: "Currency retrieved successfully.");
            }
        }

        public async Task<ResponseDto?> GetCurrenciesByIdsAsync(IEnumerable<int> ids)
        {
            if (ids is null || !ids.Any())
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "No IDs provided for currency retrieval.");
            }
            else
            {
                var currenciesWithMultipleIds = await this._applicationDbContext.Currencies.Where(currency => ids.Contains(currency.Id)).ToListAsync();

                if (currenciesWithMultipleIds.Any())
                {
                    var currenciesDto = currenciesWithMultipleIds.Select(currency => currency.FromCurrencyModelToCurrencyDtoExtension()).ToList();

                    return Utility.GetResponse(responseData: currenciesDto, isSuccess: true, message: "Currencies retrieved successfully.");
                }
                else
                {
                    return Utility.GetResponse(responseData: null, isSuccess: false, message: "No Currencies found with the provided IDs.");
                }
            }
        }

        public async Task<ResponseDto?> GetCurrencyByTitleAsync(string title)
        {
            var currencyByTitle = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Title == title);

            if (currencyByTitle is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {title}");
            }
            else
            {
                var currencyDtoByTitle = currencyByTitle.FromCurrencyModelToCurrencyDtoExtension();

                return Utility.GetResponse(responseData: currencyDtoByTitle, isSuccess: true, message: "Currency retrieved successfully.");
            }
        }

        public async Task<ResponseDto?> UpdateCurrencyByIdAsync(int id, UpdateCurrencyDto updateCurrencyDto)
        {
            if (updateCurrencyDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Update data cannot be null");
            }
            else
            {
                var currencyById = await this._applicationDbContext.Currencies.FindAsync(id);

                if (currencyById is null)
                {
                    return Utility.GetResponse(responseData: null, isSuccess: false, message: "Currency does not found!");
                }

                var existingCurrencies = this._applicationDbContext.Currencies.Where(currency => currency.Id != id);
                var isCurrencyDuplicated = Utility.IsDuplicated(propertyValue: updateCurrencyDto.Title, existingEntities: existingCurrencies, propertySelector: currency => currency.Title);

                if (!isCurrencyDuplicated)
                {
                    currencyById.Title = updateCurrencyDto.Title;
                    currencyById.Description = updateCurrencyDto.Description;
                    var updatedCurrencyDto = currencyById.FromCurrencyModelToCurrencyDtoExtension();

                    await this._applicationDbContext.SaveChangesAsync();

                    return Utility.GetResponse(responseData: updatedCurrencyDto, isSuccess: true, message: "Currency updated successfully.");
                }
                else
                {
                    return Utility.GetResponse(responseData: null, isSuccess: false, message: "Currency with same tile exists!");
                }
            }
        }

        public async Task<ResponseDto?> UpdateCurrencyByTitleAsync(string title, UpdateCurrencyDto updateCurrencyDto)
        {
            if (updateCurrencyDto is null)
            {
                return Utility.GetResponse(responseData: null, isSuccess: false, message: "Update data cannot be null");
            }
            else
            {
                var currencyByTitle = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Title == title);

                if (currencyByTitle is null)
                {
                    return Utility.GetResponse(responseData: null, isSuccess: false, message: "Currency does not found!");
                }

                var existingCurrencies = this._applicationDbContext.Currencies.Where(currency => currency.Title != title);
                var isCurrencyDuplicated = Utility.IsDuplicated(propertyValue: updateCurrencyDto.Title, existingEntities: existingCurrencies, propertySelector: currency => currency.Title);

                if (!isCurrencyDuplicated)
                {
                    currencyByTitle.Title = updateCurrencyDto.Title;
                    currencyByTitle.Description = updateCurrencyDto.Description;
                    var updatedCurrencyDto = currencyByTitle.FromCurrencyModelToCurrencyDtoExtension();

                    await this._applicationDbContext.SaveChangesAsync();

                    return Utility.GetResponse(responseData: updatedCurrencyDto, isSuccess: true, message: "Currency updated successfully.");
                }
                else
                {
                    return Utility.GetResponse(responseData: null, isSuccess: false, message: "Currency with same tile exists!");
                }
            }
        }
    }
}
