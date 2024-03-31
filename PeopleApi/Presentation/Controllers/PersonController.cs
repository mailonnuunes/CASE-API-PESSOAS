using Microsoft.AspNetCore.Mvc;
using PeopleApi.Application.Dtos;
using PeopleApi.Application.Services.PersonService;
using PeopleApi.Domain;
using System;


namespace PeopleApi.Presentation.Controllers
{

    [ApiController]
    [Route("Pessoas")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IPersonService personService, ILogger<PersonController> logger)
        {
            _personService = personService;
            _logger = logger;
        }

        [HttpGet("obter-todas-pessoas")]
        public IActionResult GetAllPeople()
        {
            _logger.LogInformation($"solicitado endpoint /api/Person/obter-todas-pessoas GET");
            return Ok(_personService.GetAllPeople());
        }
        [HttpGet("Obter-pessoa-por-id")]
        public IActionResult GetPersonById(int id)
        {
            _logger.LogInformation($"solicitado endpoint /api/Person/Obter-pessoa-por-id GET");
            return Ok(_personService.GetPersonById(id));
        }
        [HttpPost("Criar-pessoa")]
        public IActionResult CreatePerson(CreatePersonDto createPersonDto)
        {
            
            var person = new Person(createPersonDto); 
            _personService.CreatePerson(person);
            var newPersonId = person.Id;
            _logger.LogWarning($"Solicitado endpoint /api/Person/Criar-pessoa POST - Pessoa criada - ID: {newPersonId}, Nome: {person.Name}, Idade: {person.Age}, Email: {person.Email}");
            return CreatedAtAction(nameof(GetPersonById), new { id = newPersonId }, person);
        }


        [HttpPut("Atualizar-pessoa")]
        public IActionResult UpdatePerson(Person person)
        {
            _personService.UpdatePerson(person);
            _logger.LogInformation($"solicitado endpoint /api/Person/Atualizar-pessoa PUT - Pessoa atualizada - ID: {person.Id}");
            return NoContent();
        }
        [HttpDelete("Deletar-pessoa")]
        public IActionResult DeletePerson(int id)
        {
            _personService.DeletePerson(id);
            _logger.LogInformation($"solicitado endpoint /api/Person/Deletar-pessoa DELETE - Pessoa deletada - ID: {id}");
            return NoContent();
        }
        [HttpPost("criar-pessoa-apartir-csv")]
        public IActionResult AddPeopleFromCSV(IFormFile file) 
        {
            _personService.AddPeopleFromCSV(file);
            
            return Ok("pessoas adicionadas com sucesso");
          

        }
    }
}
