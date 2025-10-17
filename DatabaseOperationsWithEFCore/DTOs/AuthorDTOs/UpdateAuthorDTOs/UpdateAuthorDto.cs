using System.ComponentModel.DataAnnotations;

namespace DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.UpdateAuthorDTOs
{
    public class UpdateAuthorDto
    {
        public string Name { get; set; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}
