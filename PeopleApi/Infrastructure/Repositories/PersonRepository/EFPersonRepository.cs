using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PeopleApi.Domain;
using PeopleApi.Domain.Services;
using PeopleApi.Infrastructure.Data.DbContexts;


namespace PeopleApi.Infrastructure.Repositories.PersonRepository
{
    public class EFPersonRepository<T> : IPersonRepository<Person>
    {

        protected PersonDbContext _context { get; set; }

        protected DbSet<Person> _dbSet;


        public EFPersonRepository(PersonDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Person>();
        }
        public void Create(Person entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            _dbSet.Remove(GetById(id));
            _context.SaveChanges();
        }

        public async Task<(IEnumerable<Person> data, int totalCount)> GetAll(int page, int pageSize)
        {
            var query = _dbSet.AsQueryable();
            var totalCount = await query.CountAsync();

            var paginatedData = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (paginatedData, totalCount);
        }

        public Person GetById(long id)
        {

            return _dbSet.FirstOrDefault(t => t.Id == id);
        }

        public void Update(Person entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public async Task<OperationResult<string>> SavePeopleWithCSVAsync(Person person)
        {
            var result = new OperationResult<string>();

            try
            {
                await _dbSet.AddAsync(person);
                await _context.SaveChangesAsync();

                result.Data = "Pessoas salvas com sucesso";
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Erro ao salvar pessoas: {ex.Message}");
            }

            return result;
        }

        public bool BeUniqueEmail(string email)
        {
            var existingPerson = _dbSet.Where(p => p.Email == email).FirstOrDefault();
            return existingPerson == null;

        }
    }
}
