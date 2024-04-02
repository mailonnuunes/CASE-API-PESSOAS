

using PeopleApi.Application.Dtos;
using System.Text.RegularExpressions;

namespace PeopleApi.Domain
{
    public class Person 
    {
        public Person()
        {
        }

        public Person(CreatePersonDto createPersonDto)
        {
            Name = createPersonDto.Name;
            Age = createPersonDto.Age;
            Email = createPersonDto.Email;
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }

    }
}
