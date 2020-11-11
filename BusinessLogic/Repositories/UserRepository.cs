using BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        
        public UserRepository(BookServiceContext context) : base(context)
        {}
        public async Task<IEnumerable<User>> GetUsersPagedAsync(PagedResults paging, bool includebooks=false)
        {
            var user = await BookServiceContext.Users
                        .Skip((paging.PageIndex - 1) * paging.PageSize)
                        .Take(paging.PageSize)
                        .ToListAsync();

            if (includebooks)
            {
                user = await BookServiceContext.Users
                            .Include(c => c.UserBooks)
                            .ThenInclude(c => c.Book)
                            .Skip((paging.PageIndex - 1) * paging.PageSize)
                            .Take(paging.PageSize)
                            .ToListAsync();
            }

            return user;
        }

        public async Task<User> GetSingleUserAsync(int id)
        {
           var user = await BookServiceContext.Users
                            .Include(c => c.UserBooks)
                            .ThenInclude(c => c.Book)                            
                            .FirstOrDefaultAsync( x => x.Id == id);         

            return user;
        }

        public async Task<User> LoginUser(LoginModel loginuser)
        {
            var user = await BookServiceContext.Users.FirstOrDefaultAsync(
                            u => u.Email.ToLower().Trim() == loginuser.Email.ToLower().Trim()
                            && u.Password == loginuser.Password);

            if (user == null) return null;

            return user;
        }

        public BookServiceContext BookServiceContext
        {
            get { return Context as BookServiceContext; }
        }
    }
}
