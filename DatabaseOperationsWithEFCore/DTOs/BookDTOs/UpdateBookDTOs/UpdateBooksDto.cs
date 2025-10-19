namespace DatabaseOperationsWithEFCore.DTOs.BookDTOs.UpdateBookDTOs
{
    public class UpdateBooksDto
    {
        public ICollection<UpdateBookDto> UpdateBookDto { get; set; }
    }
}
