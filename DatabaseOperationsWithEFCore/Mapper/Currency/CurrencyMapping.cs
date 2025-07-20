using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.CurrencyDTO;
using DatabaseOperationsWithEFCore.Models;
using System.Runtime.CompilerServices;

namespace DatabaseOperationsWithEFCore.Mapper.Currency
{
    public static class CurrencyMapping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currencyDto"></param>
        /// <returns></returns>
        public static Models.Currency FromCurrencyDtoToCurrencyModelExtension(this CurrencyDto currencyDto)
        {
            return new Models.Currency()
            {
                Title = currencyDto.Title,
                Description = currencyDto.Description
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static CurrencyDto FromCurrencyModelToCurrencyDtoExtension(this Models.Currency currency)
        {
            return new CurrencyDto()
            {
                Title = currency.Title,
                Description = currency.Description
            };
        }
    }
}
