
using FluentValidation;
using PeopleApi.Infrastructure.Repositories.PersonRepository;
using PeopleApi.Presentation.Controllers;

namespace PeopleApi.Domain.Services
{
    public class CsvProcessingService : ICsvProcessingService
    {

        private readonly IPersonRepository<Person> _personRepository;
        private readonly ILogger<PersonController> _logger;
        private readonly IValidator<Person> _personValidator;

        public CsvProcessingService(IPersonRepository<Person> personRepository, ILogger<PersonController> logger, IValidator<Person> personValidator)
        {
            _personRepository = personRepository;
            _logger = logger;
            _personValidator = personValidator;
        }

        public async Task<OperationResult<string>> UploadFileAsync(IFormFile file)
        {
            var result = new OperationResult<string>();


            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(';');

                    if (parts.Length == 3 || parts.Length == 4)
                    {
                        var person = new Person
                        {
                            Name = parts[0],
                            Age = int.Parse(parts[1]),
                            Email = parts[2].TrimEnd(';')
                    };
                        var validationResult = await _personValidator.ValidateAsync(person);

                        if (validationResult.IsValid)
                        {
                            await _personRepository.SavePeopleWithCSVAsync(person);
                            result.Data = "Pessoas adicionadas com sucesso";
                            _logger.LogInformation($"Solicitado endpoint /api/Person/criar-pessoa-apartir-csv POST - Pessoa criada - Nome: {person.Name}, Idade: {person.Age}, Email: {person.Email}");
                        }
                        else
                        {
                            foreach (var error in validationResult.Errors)
                            {
                                _logger.LogError($"Erro ao validar pessoa: {string.Join(", ", validationResult.Errors)}");
                                result.Errors.Add(error.ErrorMessage);
                            }
                              
                        }
                    }
                }
            }
            return result;
        }
    }
}
