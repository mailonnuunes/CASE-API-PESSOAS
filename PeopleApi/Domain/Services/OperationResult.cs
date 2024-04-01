namespace PeopleApi.Domain.Services
{
    public class OperationResult<T>
    {
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool Success => Errors.Count == 0;
    }
}
