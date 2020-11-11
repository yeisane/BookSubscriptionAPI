using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IUserRepository Users { get; }
        IUserBookRepository UserBooks { get; }
        IRegisteredApiUserRepository RegisteredApiUsers { get; }
        Task<int> CompleteAsync();

    }
}
