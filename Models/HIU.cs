using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Hobby.Models
{
    public class HIU
    {
        [Key]
        public int HIUId { get; set; }

        public User User {get;set;}
        
        public int UserId {get;set;}

        public Hobbies Hobby {get;set;}

        public int HobbyId {get;set;}

        public string Proficiency {get;set;}

        public DateTime Created_At { get; set; } = DateTime.Now;
        public DateTime Updated_At { get; set; } = DateTime.Now;



    }
}