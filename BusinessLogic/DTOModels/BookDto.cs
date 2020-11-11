using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.DTOModels
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public Decimal Price { get; set; }
    }
}
