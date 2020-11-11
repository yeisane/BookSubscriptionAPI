using BusinessLogic.DTOModels;
using BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public class UserBookRepository : Repository<UserBook>, IUserBookRepository
    {
        
        public UserBookRepository(BookServiceContext context) : base(context)
        {}
        
        public BookServiceContext BookServiceContext
        {
            get { return Context as BookServiceContext; }
        }

        public async Task<User> GetUserBooksAsync(int userid)
        {
            User user = await BookServiceContext.Users
                .Include(c => c.UserBooks)
                .ThenInclude(c => c.Book)
                .FirstOrDefaultAsync(c => c.Id == userid);

            
            return user;
        }

      
    }
}
