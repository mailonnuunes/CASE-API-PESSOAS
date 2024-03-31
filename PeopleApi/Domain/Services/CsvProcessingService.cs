
using PeopleApi.Infrastructure.Repositories.PersonRepository;
using PeopleApi.Presentation.Controllers;

namespace PeopleApi.Domain.Services
{
    public class CsvProcessingService : ICsvProcessingService
    {

        private readonly IPersonRepository<Person> _personRepository;
        private readonly ILogger<PersonController> _logger;

        public CsvProcessingService(IPersonRepository<Person> personRepository, ILogger<PersonController> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        public async Task UploadFileAsync(IFormFile file)
        {

            
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(';');

                    if (parts.Length == 3)
                    {
                        var person = new Person
                        {
                            Name = parts[0],
                            Age = int.Parse(parts[1]),
                            Email = parts[2],
                    };
                        _logger.LogInformation($"solicitado endpoint /api/Person/criar-pessoa-apartir-csv POST - Pessoa criada - ID: {person.Id}, Nome: {person.Name}, Idade: {person.Age}, Email: {person.Email}");
                        await _personRepository.SavePeopleWithCSVAsync(person);
                       
                    };
                        
                    }
                }
            }

        }
    }

