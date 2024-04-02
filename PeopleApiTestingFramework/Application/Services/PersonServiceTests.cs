using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using PeopleApi.Application.Services.PersonService;
using PeopleApi.Domain;
using PeopleApi.Domain.Services;
using PeopleApi.Infrastructure.Repositories.PersonRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace PeopleApi.Tests.Application.Services
{
    public class PersonServiceTests
    {
        private readonly PersonService _personService;
        private readonly Mock<IPersonRepository<Person>> _personRepositoryMock;
        private readonly Mock<IValidator<Person>> _validatorMock;
        private readonly Mock<ICsvProcessingService> _csvProcessingServiceMock;

        public PersonServiceTests()
        {
            _personRepositoryMock = new Mock<IPersonRepository<Person>>();
            _validatorMock = new Mock<IValidator<Person>>();
            _csvProcessingServiceMock = new Mock<ICsvProcessingService>();
            _personService = new PersonService(_personRepositoryMock.Object,_csvProcessingServiceMock.Object,_validatorMock.Object);
        }


        [Fact]
        public void POST_SendingInvalidUserToCreation()
        {
            
            var peopleList = new List<Person>
    {
        new Person { Name = "Bruce Wayne", Age = 35, Email = "batman@gotham.com" },
        new Person { Name = "Clark Kent", Age = 30, Email = "superman@metropolis.com" }
        
    };

            // Configura o comportamento do mock para o método BeUniqueEmail
            _personRepositoryMock.Setup(repo => repo.BeUniqueEmail(It.IsAny<string>()))
                                .Returns((string email) => !peopleList.Any(p => p.Email == email));

            // Cria um objeto Person com um e-mail que já existe no mock do repositório
            var personData = new Person()
            {
                Name = "Mailon Azeve1@",
                Age = 0,
                Email = "batman@gotham.com"
            };

          
            var createPerson = _personService.CreatePerson(personData);

            // Verificar se o método CreatePerson retorna false (não foi possível criar a pessoa)
            Assert.False(createPerson);
        }
        [Fact]
        public void Post_SendingValidUserToCreation()
        {
            // Lista simulada de pessoas no repositório
            var peopleList = new List<Person>
    {
        new Person { Name = "Bruce Wayne", Age = 35, Email = "batman@gotham.com" },
        new Person { Name = "Clark Kent", Age = 30, Email = "superman@metropolis.com" }
        
    };

            
            _personRepositoryMock.Setup(repo => repo.BeUniqueEmail(It.IsAny<string>()))
                                .Returns((string email) => !peopleList.Any(p => p.Email == email));

            // Criar um objeto Person com um e-mail único
            var personData = new Person()
            {
                Name = "Mailon Azevedo",
                Age = 22,
                Email = "newemail@example.com"
            };

            var createPerson = _personService.CreatePerson(personData);

            // Verificar se o método CreatePerson retorna true (foi possível criar a pessoa)
            Assert.True(createPerson);
        }
        [Fact]
        public void PUT_EditingPersonCorrectly()
        {
            // Arrange
            var peopleList = new List<Person>
    {
        new Person { Id = 1, Name = "Bruce Wayne", Age = 35, Email = "batman@gotham.com" },
        new Person { Id = 2, Name = "Clark Kent", Age = 30, Email = "superman@metropolis.com" }
    };

            _personRepositoryMock.Setup(repo => repo.GetById(It.IsAny<int>()))
                                 .Returns((int id) => peopleList.FirstOrDefault(p => p.Id == id));

            
            _personRepositoryMock.Setup(repo => repo.BeUniqueEmail(It.IsAny<string>()))
                                 .Returns((string email) => !peopleList.Any(p => p.Email == email));

            _validatorMock.Setup(v => v.Validate(It.IsAny<Person>())).Returns(new FluentValidation.Results.ValidationResult());

            var personToUpdate = new Person
            {
                Id = 1, 
                Name = "Updated Name",
                Age = 30,
                Email = "superman@metropolis.com" // Define um email que já existe na lista para simular duplicidade
            };

            // Act
            var result = _personService.UpdatePerson(personToUpdate);

            // Assert
            Assert.False(result); // Verifica se a operação de atualização falhou devido ao email duplicado
        }
        [Fact]
        public void PUT_EditingPersonIncorrectly()
        {
            // Arrange
            var peopleList = new List<Person>
    {
        new Person { Id = 1, Name = "Bruce Wayne", Age = 35, Email = "batman@gotham.com" },
        new Person { Id = 2, Name = "Clark Kent", Age = 30, Email = "superman@metropolis.com" }
    };

            _personRepositoryMock.Setup(repo => repo.GetById(It.IsAny<int>()))
                                 .Returns((int id) => peopleList.FirstOrDefault(p => p.Id == id));

            
            _personRepositoryMock.Setup(repo => repo.BeUniqueEmail(It.IsAny<string>()))
                                 .Returns((string email) => !peopleList.Any(p => p.Email == email));

            _validatorMock.Setup(v => v.Validate(It.IsAny<Person>())).Returns(new FluentValidation.Results.ValidationResult());

            var personToUpdate = new Person
            {
                Id = 1, // Id da pessoa que será atualizada
                Name = "Updated Name",
                Age = 30,
                Email = "teste@metropolis.com" 
            };

            // Act
            var result = _personService.UpdatePerson(personToUpdate);

            // Assert
            Assert.True(result); // Verifica se a operação de atualização falhou devido ao email duplicado
        }
        [Fact]
        public void DELETE_PersonInvalidId()
        {
            // Arrange
            var peopleList = new List<Person>
    {
        new Person { Id = 1, Name = "Bruce Wayne", Age = 35, Email = "batman@gotham.com" },
        new Person { Id = 2, Name = "Clark Kent", Age = 30, Email = "superman@metropolis.com" }
    };

            var invalidPersonId = 999; // ID que não corresponde a nenhuma pessoa na lista
            _personRepositoryMock.Setup(repo => repo.GetById(invalidPersonId))
                                 .Returns((Person)null); // Retornar null para simular que o ID não existe na lista

            // Act
            var result = _personService.DeletePerson(invalidPersonId);

            // Assert
            Assert.False(result.Success); // Verifica se a exclusão falhou devido ao ID inválido
        }
        [Fact]
        public async Task POST_SavePeopleWithCSVAsyncValidFileSuccess()
        {

            var peopleList = new List<Person>
{
    new Person { Name = "John Doe", Age = 30, Email = "john.doe@example.com" },
    new Person { Name = "Jane Smith", Age = 25, Email = "jane.smith@example.com" }
};

            var csvContent = new StringBuilder();
            csvContent.AppendLine("Name,Age,Email");

            foreach (var person in peopleList)
            {
                csvContent.AppendLine($"{person.Name},{person.Age},{person.Email}");
            }

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent.ToString()));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            _csvProcessingServiceMock.Setup(c => c.UploadFileAsync(It.IsAny<IFormFile>()))
                                     .ReturnsAsync(new OperationResult<string> { Data = "Pessoas adicionadas com sucesso" });


            var operationResult = await _personService.AddPeopleFromCSV(fileMock.Object);


            Assert.NotNull(operationResult);
            Assert.Equal("Pessoas adicionadas com sucesso", operationResult.Data);
            Assert.Empty(operationResult.Errors);

        }

        [Fact]
        public async Task POST_SavePeopleWithCSVAsyncInvalidFileErrorReturned()
        {

            var peopleList = new List<Person>
    {
        new Person { Name = "John Doe", Age = 30, Email = "john.doe@example.com" },
        new Person { Name = "Jane Smith", Age = 25, Email = "jane.smith@example.com" }
    };

            var csvContent = new StringBuilder();
            csvContent.AppendLine("Name,Age,Email");

            foreach (var person in peopleList)
            {
                csvContent.AppendLine($"{person.Name},{person.Age},{person.Email}");
            }

            csvContent.AppendLine("Invalid line");

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent.ToString()));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            _csvProcessingServiceMock.Setup(c => c.UploadFileAsync(It.IsAny<IFormFile>()))
                                     .ThrowsAsync(new Exception("Erro ao processar o arquivo CSV"));

            // Act
            var operationResult = await _personService.AddPeopleFromCSV(fileMock.Object);

            // Assert
            Assert.False(operationResult.Success);
            Assert.Contains("Erro ao processar o arquivo CSV", operationResult.Errors[0]);

        }

    }
}
