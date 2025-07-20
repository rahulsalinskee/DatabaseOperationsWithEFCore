using DatabaseOperationsWithEFCore.DTOs.BookPriceDTOs.BookPriceDTO;

namespace DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.CurrencyDTO
{
    public class CurrencyDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<BookPriceDto> BookPriceDto { get; set; }
    }
}
