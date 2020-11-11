using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessLogic.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string ServiceProvider { get; set; }
        public IEnumerable<UserBook> UserBooks { get; set; } 
    }
}
