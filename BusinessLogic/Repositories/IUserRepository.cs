using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetUsersPagedAsync(PagedResults paging, bool includebooks);
        Task<User> GetSingleUserAsync(int id);
        Task<User> LoginUser(LoginModel loginuser);
    }
}
