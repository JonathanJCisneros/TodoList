#pragma warning disable CS8618
namespace TodoList.Models;
using System.ComponentModel.DataAnnotations;


public class UserLogin
{
    [Required(ErrorMessage = "is required")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string LoginEmail {get; set;}

    [Required(ErrorMessage = "is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string LoginPassword {get; set;}
}