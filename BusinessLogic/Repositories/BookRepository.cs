using BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        
        public BookRepository(BookServiceContext context) : base(context)
        {}
        async Task<IEnumerable<Book>> IBookRepository.GetBooksPagedAsync(PagedResults paging)
        {
            return await BookServiceContext.Books
                .OrderBy(b => b.Id)
                .Skip((paging.PageIndex - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();
        }

        public BookServiceContext BookServiceContext
        {
            get { return Context as BookServiceContext; }
        }
    }
}
