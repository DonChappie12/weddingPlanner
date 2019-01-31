using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wedding.Models
{
    public class Wedding
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="Must Provide Name")]
        public string WedderOne { get; set; }

        [Required(ErrorMessage="Must Provide Name")]
        public string WedderTwo { get; set; }

        [Required(ErrorMessage="Must Provide a Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage="Must Provide an Address")]
        public string WeddingAddress { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set;}
        
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set;}

        [ForeignKey("user")]
        public int User_Id { get; set; }
        public User user { get; set; }

        public List<Attendees> Attendee { get; set; }
        public Wedding()
        {
            Attendee = new List<Attendees>();
        }
    }
}