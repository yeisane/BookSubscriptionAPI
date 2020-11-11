using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.DTOModels
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string ServiceProvider { get; set; }
        public List<BookDto> Books { get; set; } = new List<BookDto>();
      
    }
}
