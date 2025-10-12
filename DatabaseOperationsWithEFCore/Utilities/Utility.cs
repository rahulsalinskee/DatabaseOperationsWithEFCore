using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.AddCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.CurrencyDTOs.UpdateCurrencyDTOs;
using DatabaseOperationsWithEFCore.DTOs.ResponseDTOs;

namespace DatabaseOperationsWithEFCore.Utilities
{
    public static class Utility
    {
        /// <summary>
        /// Checks if an entity is duplicated based on a property value
        /// </summary>
        /// <typeparam name="TEntity">The entity type to check against</typeparam>
        /// <param name="propertyValue">The value to check for duplication</param>
        /// <param name="existingEntities">Collection of existing entities</param>
        /// <param name="propertySelector">Function to select the property from entity</param>
        /// <param name="comparisonType">String comparison type (default: OrdinalIgnoreCase)</param>
        /// <returns>True if duplicate exists, false otherwise</returns>
        public static bool IsDuplicated<TEntity>(string propertyValue, IEnumerable<TEntity> existingEntities, Func<TEntity, string> propertySelector, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                return false;
            }

            return existingEntities.Any(entity =>
            {
                var entityPropertyValue = propertySelector(entity);
                return entityPropertyValue?.Equals(propertyValue.Trim(), comparisonType) == true;
            });
        }

        /// <summary>
        /// Overload for checking duplication with DTO object and enum type
        /// </summary>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <typeparam name="TDtoType">The enum type for DTO classification</typeparam>
        /// <param name="dto">The DTO object to check</param>
        /// <param name="dtoType">The type of DTO</param>
        /// <param name="existingEntities">Collection of existing entities</param>
        /// <param name="dtoPropertyExtractor">Function to extract property from DTO based on type</param>
        /// <param name="entityPropertySelector">Function to select property from entity</param>
        /// <param name="comparisonType">String comparison type</param>
        /// <returns>True if duplicate exists, false otherwise</returns>
        public static bool IsDuplicated<TDto, TEntity, TDtoType>(TDto dto, TDtoType dtoType, IEnumerable<TEntity> existingEntities, Func<TDto, TDtoType, string> dtoPropertyExtractor, Func<TEntity, string> entityPropertySelector, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase) where TDtoType : Enum
        {
            var propertyValue = dtoPropertyExtractor(dto, dtoType);
            return IsDuplicated(propertyValue, existingEntities, entityPropertySelector, comparisonType);
        }

        public static ResponseDto GetResponse(object? responseData, bool isSuccess, string message)
        {
            return new ResponseDto()
            {
                Response = responseData,
                IsSuccess = isSuccess,
                Message = message
            };
        }
    }
}
