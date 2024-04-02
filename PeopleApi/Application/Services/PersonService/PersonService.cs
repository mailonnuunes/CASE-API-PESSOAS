using FluentValidation;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeopleApi.Domain;
using PeopleApi.Domain.Services;
using PeopleApi.Infrastructure.Repositories.PersonRepository;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

        public bool CreatePerson(Person person)
        {
            if (person.Age <= 0 || person.Age > 150)
            {
                return false;

            }

            if (!Regex.IsMatch(person.Name, @"^[a-zA-Z\s]+$") || string.IsNullOrEmpty(person.Name))
            {
                return false;
                
            }

            if (!Regex.IsMatch(person.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return false;
                
            }

            // Se todas as condições de validação passarem, cria a pessoa
            _personRepository.Create(person);
             return true;

        }

        public ServiceResult<bool> DeletePerson(long id)
        {
            var personToDelete = _personRepository.GetById(id);
            if (personToDelete != null)
            {
                _personRepository.Delete(id);
                return new ServiceResult<bool> { Success = true, Message = "Pessoa excluída com sucesso" };
            }
            else
            {
                return new ServiceResult<bool> { Success = false, Message = "Pessoa não encontrada, exclusão não realizada" };
            }
        }

        public Task<(IEnumerable<Person> data, int totalCount)> GetAllPeople(int page = 1, int pageSize = 10)
        {
            return _personRepository.GetAll(page, pageSize);
        }

        public ServiceResult<Person> GetPersonById(long id)
        {
            var person = _personRepository.GetById(id);
            if (person != null)
            {
                return new ServiceResult<Person>{ Success = true, Data = person };
            }
            else
            {
                return new ServiceResult<Person> { Success = false, Message = "Pessoa não encontrada" };
            }
        }

        public bool UpdatePerson(Person person)
        {
            var validationResult = _personValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            if (_personRepository.BeUniqueEmail(person.Email))
            {
                if (person.Age <= 0 || person.Age > 150)
                {
                    return false;
                }

                if (!Regex.IsMatch(person.Name, @"^[a-zA-Z\s]+$") || string.IsNullOrEmpty(person.Name))
                {
                    return false;
                }

                if (!Regex.IsMatch(person.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    return false;
                }

                _personRepository.Update(person);
                return true;
            }
            else
            {
                return false; // Retorna false se o email já existir na lista
            }
        }
    }
        }



    

