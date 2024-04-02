namespace PeopleApi.Domain.Services
{
    public class ServiceResult<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
