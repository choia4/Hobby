using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Hobby.Models
{
    public class Hobbies
    {
        [Key]
        public int HobbyId { get; set; }

        [Required]
        public string Name {get;set;}

        [Required]
        public string Description {get;set;}

        public List<HIU> HobbiesInUsers{get;set;} = new List<HIU>();

        public DateTime Created_At { get; set; } = DateTime.Now;
        public DateTime Updated_At { get; set; } = DateTime.Now;



    }
}