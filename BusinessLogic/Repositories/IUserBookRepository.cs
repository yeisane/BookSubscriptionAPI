using BusinessLogic.DTOModels;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public interface IUserBookRepository : IRepository<UserBook>
    {
       Task<User> GetUserBooksAsync(int userid);
    }
}
