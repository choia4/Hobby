using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class LoginUser
{

    [EmailAddress]
    [Required]
    [Display(Name = "Email")]
    public string Email2 { get; set; }

    [DataType(DataType.Password)]
    [Required]
    [Display(Name = "Password")]
    [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
    public string Password2 { get; set; }
}
