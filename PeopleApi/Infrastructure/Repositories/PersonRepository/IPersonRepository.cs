using PeopleApi.Domain;


namespace PeopleApi.Infrastructure.Repositories.PersonRepository
{
    public interface IPersonRepository<Person>
    {

        IList<Person> GetAll();

        Person GetById(long id);

        void Create(Person entity);

        void Update(Person entity);

        void Delete(long id);

        Task SavePeopleWithCSVAsync(Person person);

        bool BeUniqueEmail(string email);
    }
}
