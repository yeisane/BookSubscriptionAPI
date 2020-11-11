using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Repositories
{
    public class RegisteredApiUserRepository : Repository<RegisteredApiUser>, IRegisteredApiUserRepository
    {
        public RegisteredApiUserRepository(BookServiceContext context) : base(context)
        {}
       
    }
}
