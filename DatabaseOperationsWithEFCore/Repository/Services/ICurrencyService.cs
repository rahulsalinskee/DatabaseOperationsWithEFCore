using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.AddCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.UpdateCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;

namespace DatabaseOperationsWithEFCore.Repository.Services
{
    public interface ICurrencyService
    {
        public Task<ResponseDto?> GetAllCurrenciesAsync(string? columnName = null, string? filterKeyWord = null);

        public Task<ResponseDto?> GetCurrencyByIdAsync(int id);

        public Task<ResponseDto?> GetCurrenciesByIdsAsync(IEnumerable<int> ids);

        public Task<ResponseDto?> GetCurrencyByTitleAsync(string title);

        public Task<ResponseDto?> AddCurrencyAsync(AddNewCurrencyDto addCurrencyDto);

        public Task<ResponseDto?> UpdateCurrencyByIdAsync(int id, UpdateCurrencyDto updateCurrencyDto);

        public Task<ResponseDto?> UpdateCurrencyByTitleAsync(string title, UpdateCurrencyDto updateCurrencyDto);

        public Task<ResponseDto?> DeleteCurrencyByIdAsync(int id);

        public Task<ResponseDto?> DeleteCurrencyByTitleAsync(string title);

        public Task<ResponseDto?> DeleteCurrencyByIdAndTitleAsync(int id, string title);
    }
}
