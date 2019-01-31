using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wedding.Models
{
    public class Attendees
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("user")]
        public int User_Id { get; set; }
        public User user { get; set; }

        [ForeignKey("wedding")]
        public int Wedding_Id { get; set; }
        public Wedding wedding { get; set; }
    }
}