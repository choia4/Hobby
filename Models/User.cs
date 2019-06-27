using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Hobby.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MinLength(2, ErrorMessage = "First Name must be 2 characters or longer!")]
        public string First_Name { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Last Name must be 2 characters or longer!")]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "User name must be at least 3 characters in length ")]
        [MaxLength(15, ErrorMessage= "User name cannot be longer than 15 characters in length")]
        public string Username {get;set;}

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public List<HIU> UsersInHobbies{get;set;} = new List<HIU>();

        [DataType(DataType.Password)]
        [Required]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string Password { get; set; }


        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string Confirm { get; set; }

        public DateTime Created_At { get; set; } = DateTime.Now;
        public DateTime Updated_At { get; set; } = DateTime.Now;



    }
}