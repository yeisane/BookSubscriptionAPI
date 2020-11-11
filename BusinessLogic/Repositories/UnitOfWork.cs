using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookServiceContext _context;

        public UnitOfWork(BookServiceContext context)
        {
            _context = context;
            Books = new BookRepository(_context);
            Users = new UserRepository(_context);
            UserBooks = new UserBookRepository(_context);
            RegisteredApiUsers = new RegisteredApiUserRepository(_context);
        }
        public IBookRepository Books { get; private set; }

        public IUserRepository Users { get; private set; }
        public IUserBookRepository UserBooks { get; private set; }
        public IRegisteredApiUserRepository RegisteredApiUsers { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
