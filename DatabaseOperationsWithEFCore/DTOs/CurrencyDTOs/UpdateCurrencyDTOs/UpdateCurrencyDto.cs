using System.ComponentModel.DataAnnotations;

namespace DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.UpdateCurrencyDTOs
{
    public class UpdateCurrencyDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
