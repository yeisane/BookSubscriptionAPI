using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.DTOModels
{
    public class UserAddDto
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string ServiceProvider { get; set; }
      
    }
}
