using DatabaseOperationsWithEFCore.DTOs.BookDTOs.BookDTO;
using DatabaseOperationsWithEFCore.Models;

namespace DatabaseOperationsWithEFCore.DTOs.BookDTOs.DeleteBookDTOs
{
    public class DeleteBooksDto
    {
        public ICollection<DeleteBookDto>? DeleteBooks { get; set; }
    }
}
