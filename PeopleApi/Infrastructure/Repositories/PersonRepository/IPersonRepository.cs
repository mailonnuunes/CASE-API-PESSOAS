using PeopleApi.Domain;
using PeopleApi.Domain.Services;


namespace PeopleApi.Infrastructure.Repositories.PersonRepository
{
    public interface IPersonRepository<Person>
    {

        Task<(IEnumerable<Person> data, int totalCount)> GetAll(int page, int pageSize);

        Person GetById(long id);

        void Create(Person entity);

        void Update(Person entity);

        void Delete(long id);

        Task<OperationResult<string>> SavePeopleWithCSVAsync(Person person);

        bool BeUniqueEmail(string email);
    }
}
