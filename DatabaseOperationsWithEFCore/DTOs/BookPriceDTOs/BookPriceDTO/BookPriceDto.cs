using DatabaseOperationsWithEFCore.DTOs.BookDTOs.BookDTO;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.CurrencyDTO;

namespace DatabaseOperationsWithEFCore.DTOs.BookPriceDTOs.BookPriceDTO
{
    public class BookPriceDto
    {
        public int Amount { get; set; }

        public int BookId { get; set; }

        public BookDto BookDto { get; set; }

        public CurrencyDto CurrencyDto { get; set; }
    }
}
