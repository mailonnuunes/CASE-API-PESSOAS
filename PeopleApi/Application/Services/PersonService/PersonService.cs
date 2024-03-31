using FluentValidation;
using PeopleApi.Domain;
using PeopleApi.Domain.Services;
using PeopleApi.Infrastructure.Repositories.PersonRepository;

namespace PeopleApi.Application.Services.PersonService
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository<Person> _personRepository;

        private readonly ICsvProcessingService _csvProcessingService;

        private readonly IValidator<Person> _personValidator;

        public PersonService(IPersonRepository<Person> personRepository, ICsvProcessingService csvProcessingService, IValidator<Person> personValidator)
        {
            _personRepository = personRepository;
            _csvProcessingService = csvProcessingService;
            _personValidator = personValidator;
        }

        public void AddPeopleFromCSV(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);

            if (file == null || file.Length == 0)
            {
                Console.WriteLine("Arquivo inválido ou vazio");
                return;

            }
            else if (extension != ".csv")
            {
                Console.WriteLine("Por favor, importe somente arquivos csv");
                return;
            }
            else
            {
                _csvProcessingService.UploadFileAsync(file);
            }
        }

        public void CreatePerson(Person person)
        {

                _personRepository.Create(person);
            
        }

        public void DeletePerson(long id)
        {
            _personRepository.Delete(id);
        }

        public IList<Person> GetAllPeople()
        {
            return _personRepository.GetAll();
        }

        public Person GetPersonById(long id)
        {
            return _personRepository.GetById(id);
        }

        public void UpdatePerson(Person person)
        {
            var validationResult = _personValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                // Lidar com erros de validação aqui
                throw new ValidationException(validationResult.Errors);
            }
            else
            {
                _personRepository.Update(person);
            }
           
        }

    }
    
}
