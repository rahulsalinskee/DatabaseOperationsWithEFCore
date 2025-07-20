using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.CurrencyDTO;

namespace DatabaseOperationsWithEFCore.Repository.Services
{
    public interface IFilterService<T>
    {
        public IQueryable<T?> ApplyFilterOn(IQueryable<T?> queryOn, string? columnName, string? filterKeyWord);
    }
}
