using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace wedding.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get ; set; }
        public string LastName { get ; set; }
        public string Email { get ; set; }

        [DataType(DataType.Password)]
        public string Password { get ; set; }

        public List<Attendees> Attending { get; set; }

        public List<Wedding> Weddings { get; set; }

        public User()
        {
            Attending = new List<Attendees>();
            Weddings = new List<Wedding>();
        }
    }
}