namespace PeopleApi.Domain.Services
{
    public interface ICsvProcessingService
    {

        Task UploadFileAsync(IFormFile file);
    }
}
