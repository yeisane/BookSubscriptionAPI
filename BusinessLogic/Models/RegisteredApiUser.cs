using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessLogic.Models
{
    [Table("RegisteredApiUsers")]
    public class RegisteredApiUser
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ApiKey { get; set; }
        [Required]
        public string Dns { get; set; }
    }
}
