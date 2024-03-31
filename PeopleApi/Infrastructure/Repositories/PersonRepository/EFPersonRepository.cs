using Microsoft.EntityFrameworkCore;
using PeopleApi.Domain;
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

        public IList<Person> GetAll()
        {
            return _dbSet.ToList();
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

       public async Task SavePeopleWithCSVAsync(Person person)
        {
          await  _dbSet.AddAsync(person);
         await  _context.SaveChangesAsync();
        }

       public bool BeUniqueEmail(string email)
        {
            var existingPerson = _dbSet.Where(p => p.Email == email).FirstOrDefault();
            return existingPerson == null;

        }
    }
}
