#pragma warning disable CS8618
namespace TodoList.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int UserId {get; set;}


    [Required(ErrorMessage = "is required")]
    [MinLength(2, ErrorMessage = "must be at least 2 characters")]
    [Display(Name = "First Name")]
    public string FirstName {get; set;}


    [Required(ErrorMessage = "is required")]
    [MinLength(2, ErrorMessage = "must be at least 2 characters")]
    [Display(Name = "Last Name")]
    public string LastName {get; set;}


    [Required(ErrorMessage = "is required")]
    [EmailAddress(ErrorMessage = "must be a valid Email address")]
    [Display(Name = "Email")]
    public string Email {get; set;}


    [Required(ErrorMessage = "is required")]
    [MinLength(8, ErrorMessage = "must be at least 8 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password {get; set;}


    [Required(ErrorMessage = "is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "doesn't match Password")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword {get; set;}


    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
}