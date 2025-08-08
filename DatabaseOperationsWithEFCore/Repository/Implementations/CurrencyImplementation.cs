using DatabaseOperationsWithEFCore.Data;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.AddCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.CurrencyDTO;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.UpdateCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;
using DatabaseOperationsWithEFCore.Mapper.Currency;
using DatabaseOperationsWithEFCore.Models;
using DatabaseOperationsWithEFCore.Repository.Services;
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
            if (addNewCurrencyDto is null)
            {
                return GetResponse(responseData: null, isSuccess: false, message: "Currency data cannot be null.");
            }

            /* Validate the DTO properties */
            if (string.IsNullOrWhiteSpace(addNewCurrencyDto.Title))
            {
                return GetResponse(responseData: null, isSuccess: false, message: "Currency title cannot be null or empty.");
            }

            /* Check if a currency with the same title already exists */
            var isCurrencyDuplicated = IsCurrencyDuplicated(currencyDto: addNewCurrencyDto, currencyDtoType: CurrencyDtoType.AddCurrencyDto, existingCurrencies: this._applicationDbContext.Currencies);

            if (isCurrencyDuplicated)
            {
                return GetResponse(responseData: null, isSuccess: false, message: "Currency with the same title already exists.");
            }
            else
            {
                /* Create new Currency entity */
                Currency newCurrency = new()
                {
                    Title = addNewCurrencyDto.Title.Trim(),
                    Description = addNewCurrencyDto.Description?.Trim()
                    // Add other properties as needed
                };

                /* Add Currency */
                await _applicationDbContext.Currencies.AddAsync(newCurrency);

                /* Save Changes */
                await _applicationDbContext.SaveChangesAsync();

                /* Convert the saved Currency Model back to Currency DTO */
                var convertedCurrencyModelToCurrencyDto = newCurrency.FromCurrencyModelToCurrencyDtoExtension();

                /* Return Response */
                return GetResponse(responseData: convertedCurrencyModelToCurrencyDto, isSuccess: true, message: "Currency added successfully."); 
            }
        }

        public async Task<ResponseDto?> DeleteCurrencyByIdAsync(int id)
        {
            var currencyById = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Id == id);

            if (currencyById is null)
            {
                return GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {id}");
            }
            else
            {
                this._applicationDbContext.Currencies.Remove(currencyById);
                await this._applicationDbContext.SaveChangesAsync();
                return GetResponse(responseData: null, isSuccess: true, message: "Currency deleted successfully.");
            }
        }

        public async Task<ResponseDto?> DeleteCurrencyByTitleAsync(string title)
        {
            var currencyByTitle = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Title == title);

            if (currencyByTitle is null)
            {
                return GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {title}");
            }
            else
            {
                this._applicationDbContext.Currencies.Remove(currencyByTitle);
                await this._applicationDbContext.SaveChangesAsync();
                return GetResponse(responseData: null, isSuccess: true, message: "Currency deleted successfully.");
            }
        }

        public async Task<ResponseDto?> GetAllCurrenciesAsync(string? columnName = null, string? filterKeyWord = null)
        {
            var currencies = this._applicationDbContext.Currencies.Include(currency => currency.BookPrices).AsQueryable();

            if (currencies.Any())
            {
                var filteredCurrencies = this._filterOnCurrency.ApplyFilterOn(queryOn: currencies, columnName: columnName, filterKeyWord: filterKeyWord);

                /* Convert currency Model to Currency DTO */
                var results = await filteredCurrencies.Select(currency => currency.FromCurrencyModelToCurrencyDtoExtension()).ToListAsync();

                /* Return Response */
                return GetResponse(responseData: results, isSuccess: true, message: "Currencies retrieved successfully.");
            }
            else
            {
                return GetResponse(responseData: null, isSuccess: false, message: "No currencies found.");
            }
        }

        public async Task<ResponseDto?> GetCurrencyByIdAsync(int id)
        {
            var currencyById = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Id == id);

            if (currencyById is null)
            {
                return GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {id}");
            }
            else
            {
                var currencyDtoById = currencyById.FromCurrencyModelToCurrencyDtoExtension();

                return GetResponse(responseData: currencyDtoById, isSuccess: true, message: "Currency retrieved successfully.");
            }
        }

        public async Task<ResponseDto?> GetCurrencyByTitleAsync(string title)
        {
            //var currencyByTitle = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Title == title);
            var currencyByTitle = await this._applicationDbContext.Currencies.Where(currency => currency.Title == title).FirstOrDefaultAsync();

            if (currencyByTitle is null)
            {
                return GetResponse(responseData: null, isSuccess: false, message: $"No Currency Found with ID - {title}");
            }
            else
            {
                var currencyDtoByTitle = currencyByTitle.FromCurrencyModelToCurrencyDtoExtension();

                return GetResponse(responseData: currencyDtoByTitle, isSuccess: true, message: "Currency retrieved successfully.");
            }
        }

        public async Task<ResponseDto?> UpdateCurrencyByIdAsync(int id, UpdateCurrencyDto updateCurrencyDto)
        {
            if (updateCurrencyDto is null)
            {
                return GetResponse(responseData: null, isSuccess: false, message: "Update data cannot be null");
            }
            else
            {
                var currencyById = await this._applicationDbContext.Currencies.FindAsync(id);

                if (currencyById is null)
                {
                    return GetResponse(responseData: null, isSuccess: false, message: "Currency does not found!");
                }

                var existingCurrencies = this._applicationDbContext.Currencies.Where(currency => currency.Id != id);
                var isCurrencyDuplicated = IsCurrencyDuplicated(currencyDto: updateCurrencyDto, currencyDtoType: CurrencyDtoType.UpdateCurrencyDto, existingCurrencies: existingCurrencies);

                if (!isCurrencyDuplicated)
                {
                    currencyById.Title = updateCurrencyDto.Title;
                    currencyById.Description = updateCurrencyDto.Description;
                    var updatedCurrencyDto = currencyById.FromCurrencyModelToCurrencyDtoExtension();

                    await this._applicationDbContext.SaveChangesAsync();

                    return GetResponse(responseData: updatedCurrencyDto, isSuccess: true, message: "Currency updated successfully.");
                }
                else
                {
                    return GetResponse(responseData: null, isSuccess: false, message: "Currency with same tile exists!");
                }
            }
        }

        public async Task<ResponseDto?> UpdateCurrencyByTitleAsync(string title, UpdateCurrencyDto updateCurrencyDto)
        {
            if (updateCurrencyDto is null)
            {
                return GetResponse(responseData: null, isSuccess: false, message: "Update data cannot be null");
            }
            else
            {
                var currencyByTitle = await this._applicationDbContext.Currencies.FirstOrDefaultAsync(currency => currency.Title == title);

                if (currencyByTitle is null)
                {
                    return GetResponse(responseData: null, isSuccess: false, message: "Currency does not found!");
                }

                var existingCurrencies = this._applicationDbContext.Currencies.Where(currency => currency.Title != title);
                var isCurrencyDuplicated = IsCurrencyDuplicated(currencyDto: updateCurrencyDto, 
                    currencyDtoType: CurrencyDtoType.UpdateCurrencyDto, existingCurrencies: existingCurrencies);

                if (!isCurrencyDuplicated)
                {
                    currencyByTitle.Title = updateCurrencyDto.Title;
                    currencyByTitle.Description = updateCurrencyDto.Description;
                    var updatedCurrencyDto = currencyByTitle.FromCurrencyModelToCurrencyDtoExtension();

                    await this._applicationDbContext.SaveChangesAsync();

                    return GetResponse(responseData: updatedCurrencyDto, isSuccess: true, message: "Currency updated successfully.");
                }
                else
                {
                    return GetResponse(responseData: null, isSuccess: false, message: "Currency with same tile exists!");
                }
            }
        }

        private static ResponseDto? GetResponse(object? responseData, bool isSuccess, string message)
        {
            return new ResponseDto()
            {
                Response = responseData,
                IsSuccess = isSuccess,
                Message = message
            };
        }

        private static bool IsCurrencyDuplicated(object currencyDto, CurrencyDtoType currencyDtoType, IEnumerable<Currency> existingCurrencies)
        {
            string titleToCheck = currencyDtoType switch
            {
                CurrencyDtoType.AddCurrencyDto => ((AddNewCurrencyDto)currencyDto).Title,
                CurrencyDtoType.UpdateCurrencyDto => ((UpdateCurrencyDto)currencyDto).Title,
                _ => throw new ArgumentException("Invalid currency DTO type")
            };

            /* Old version of Switch Case statement of checking title
            string titleToCheck;
             
            switch (currencyDtoType)
            {
                case CurrencyDtoType.AddCurrencyDto:
                    titleToCheck = ((AddNewCurrencyDto)currencyDto).Title;
                    break;
                case CurrencyDtoType.UpdateCurrencyDto:
                    titleToCheck = ((UpdateCurrencyDto)currencyDto).Title;
                    break;
                default:
                    throw new ArgumentException("Invalid currency DTO type");
            }
             
            */

            var isCurrencyDuplicated =  existingCurrencies.Any(existingCurrency => existingCurrency.Title.Equals(titleToCheck, StringComparison.OrdinalIgnoreCase));

            return isCurrencyDuplicated;
        }

        private enum CurrencyDtoType
        {
            AddCurrencyDto,
            UpdateCurrencyDto
        }
    }
}
