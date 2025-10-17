using DatabaseOperationsWithEFCore.DTOs.AuthorDTOs.AuthorDTO;

namespace DatabaseOperationsWithEFCore.Mapper.Author
{
    public static class AuthorMapping
    {
        public static AuthorDto FromAuthorModelToAuthorDtoExtension(this Models.Author author)
        {
            return new AuthorDto()
            {
                Name = author.Name,
                Email = author.Email,
            };
        }

        public static Models.Author FromAuthorDtoToAuthorModelExtension(this AuthorDto authorDto)
        {
            return new Models.Author()
            {
                Name = authorDto.Name,
                Email = authorDto.Email,
            };
        }
    }
}
