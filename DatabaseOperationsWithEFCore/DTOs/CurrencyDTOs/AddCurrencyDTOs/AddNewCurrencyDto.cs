using System.ComponentModel.DataAnnotations;

namespace DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.AddCurrencyDTOs
{
    public class AddNewCurrencyDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
