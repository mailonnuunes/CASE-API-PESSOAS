namespace PeopleApi.Domain.Services
{
    public interface ICsvProcessingService
    {

        Task<OperationResult<string>> UploadFileAsync(IFormFile file);
    }
}
