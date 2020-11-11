using BusinessLogic.DTOModels;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProviderService.Helpers
{
    public class PagingResponse<T>
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int totalRecords { get; set; }
        public int totalPages { get; set; }
        public string prevPage { get; set; } //for proper future paging url
        public string nextPage { get; set; } //for proper future paging url
        public IEnumerable<T> data { get; set; }
     }
}
