using FluentValidation;
using PeopleApi.Application.Dtos;
using PeopleApi.Infrastructure.Repositories.PersonRepository;

namespace PeopleApi.Domain.Services
{

    public class PersonDtoValidator : AbstractValidator<CreatePersonDto> 
    {
        private readonly IPersonRepository<Person> _personRepository;
        public PersonDtoValidator(IPersonRepository<Person> personRepository)
        {
            _personRepository = personRepository;

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("O campo 'nome' é obrigatório.")
                .Matches("^[a-zA-ZÀ-ÿ ]+$").WithMessage("O campo 'nome' deve conter apenas letras e espaços.")
                .MaximumLength(50).WithMessage("O campo 'nome' deve ter no máximo 50 caracteres.");
            RuleFor(p => p.Age)
                 .NotEmpty().WithMessage("O campo 'idade' é obrigatório.")
                 .Must(age => int.TryParse(age.ToString(), out _)).WithMessage("A 'idade' deve conter apenas números.")
                 .LessThanOrEqualTo(150).WithMessage("A 'idade' deve ser menor ou igual a 150.")
                 .GreaterThan(0).WithMessage("A 'idade' deve ser maior que 0.");
            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("O campo 'email' é obrigatório.")
                .MaximumLength(150).WithMessage("O campo 'email' deve ter no máximo 150 caracteres.")
                .Matches(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("O 'email' fornecido é inválido.")
               .Must(BeUniqueEmail).WithMessage("Este 'email' já está em uso.");
        }

        private bool BeUniqueEmail(string email)
        {
            return _personRepository.BeUniqueEmail(email);
        }
    }
    }
    

