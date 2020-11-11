using BusinessLogic.DTOModels;
using BusinessLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProviderService.Helpers
{
       public static  class PagedResultsHelper<T>
    {
        public static PagingResponse<T> PagingResponse(T[] data, int pageIndex, int pageSize, int totalRecords)
        {

            var totalPages = ((double)totalRecords / (double)pageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            string nextPage = (pageIndex >= 1 && pageIndex < roundedTotalPages) ? (pageIndex + 1).ToString() : null;
            string previousPage = (pageIndex - 1 >= 1 &&  pageIndex <= roundedTotalPages) ? (pageIndex - 1).ToString() : null;

            var response = new PagingResponse<T>
            {
                data = data,
                pageIndex = pageIndex,
                pageSize = pageSize,
                totalPages = roundedTotalPages,
                nextPage = nextPage,
                prevPage = previousPage,
                totalRecords = totalRecords
            };

            return response;
            
        }
    }
}
