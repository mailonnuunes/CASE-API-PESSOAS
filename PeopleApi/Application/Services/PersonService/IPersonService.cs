using PeopleApi.Domain;

namespace PeopleApi.Application.Services.PersonService
{
    public interface IPersonService
    {
        IList<Person> GetAllPeople();

        Person GetPersonById(long id);

        void CreatePerson(Person person);

        void UpdatePerson(Person person);

        void DeletePerson(long id);

        void AddPeopleFromCSV(IFormFile file);
    }
}
