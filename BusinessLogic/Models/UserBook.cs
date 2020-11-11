using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BusinessLogic.Models
{
    [Table("UserBooks")]
    public class UserBook
    { 
            public int BookId { get; set; }
            public Book Book { get; set; }
            public int UserId { get; set; }
            public User User { get; set; } 
    }
}
