using Microsoft.AspNetCore.Mvc;
using PeopleApi.Domain;
using PeopleApi.Domain.Services;

namespace PeopleApi.Application.Services.PersonService
{
    public interface IPersonService
    {
        Task<(IEnumerable<Person> data, int totalCount)> GetAllPeople(int page = 1, int pageSize = 10);

        Person GetPersonById(long id);

        void CreatePerson(Person person);

        void UpdatePerson(Person person);

        void DeletePerson(long id);

        Task<OperationResult<string>> AddPeopleFromCSV(IFormFile file);
    }
}
