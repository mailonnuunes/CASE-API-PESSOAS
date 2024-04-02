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
        public async Task<IActionResult> GetAllPeople(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Solicitado endpoint /api/Person/obter-todas-pessoas GET");

            var (people, totalCount) = await _personService.GetAllPeople(page, pageSize);

            var response = new
            {
                Data = people,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page
            };

            return Ok(response);
        }
        [HttpGet("Obter-pessoa-por-id")]
        public IActionResult GetPersonById(int id)
        {
            _logger.LogInformation($"solicitado endpoint /api/Person/Obter-pessoa-por-id GET");
            var result = _personService.GetPersonById(id);

            if (result.Success)
            {
                return Ok(result.Data); // Retorna 200 OK com os dados da pessoa
            }
            else
            {
                return NotFound(result.Message); // Retorna 404 Not Found com a mensagem de erro
            }
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

            _logger.LogInformation($"solicitado endpoint /api/Person/Deletar-pessoa DELETE - Pessoa deletada - ID: {id}");
            var result = _personService.DeletePerson(id);

            if (result.Success)
            {
                return NoContent(); // Retorna 200 OK com a mensagem de sucesso
            }
            else
            {
                return NotFound(result.Message); // Retorna 404 Not Found com a mensagem de erro
            }
        }

        [HttpPost("criar-pessoa-apartir-csv")]
        public async Task<IActionResult> AddPeopleFromCSV(IFormFile file)
        {
            var result = await _personService.AddPeopleFromCSV(file);

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}
