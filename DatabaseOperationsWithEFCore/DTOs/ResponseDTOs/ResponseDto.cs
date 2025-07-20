namespace DatabaseOperationsWithEFCore.DTOs.ResponseDTOs
{
    public class ResponseDto
    {
        public object? Response { get; set; }

        public bool IsSuccess { get; set; } = default;

        public string Message { get; set; } = string.Empty;
    }
}
