using FluentValidation;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public async Task<OperationResult<string>> AddPeopleFromCSV(IFormFile file)
        {
            var result = new OperationResult<string>();
            var extension = file.FileName.Split('.').Last();

            if (file == null || file.Length == 0)
            {
                result.Errors.Add("Arquivo inválido ou vazio");
                return result;
            }
            else if (extension != "csv")
            {
                result.Errors.Add("Somente arquivos CSV são permitidos");
                return result;
            }
            else if (file.Length > 1048576)
            {
                result.Errors.Add("O tamanho do arquivo não pode exceder 1MB.");
                return result;
            }
            else
            {
                try
                {
                    var uploadResult = await _csvProcessingService.UploadFileAsync(file);
                    if (uploadResult.Success)
                    {
                        result.Data = "Pessoas adicionadas com sucesso";
                    }
                    else
                    {
                        result.Errors.AddRange(uploadResult.Errors);
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Ocorreu um erro ao processar o arquivo CSV: {ex.Message}");
                }
                return result;
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

        public Task<(IEnumerable<Person> data, int totalCount)> GetAllPeople(int page = 1, int pageSize = 10)
        {
            return _personRepository.GetAll(page, pageSize);
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
