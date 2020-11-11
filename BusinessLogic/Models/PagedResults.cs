using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Models
{
    public class PagedResults
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
}
