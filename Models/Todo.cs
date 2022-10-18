#pragma warning disable CS8618
namespace TodoList.Models;
using System.ComponentModel.DataAnnotations;

public class Todo
{
    [Key]
    public int TodoId {get; set;}

    [Required(ErrorMessage = "is required")]
    [MinLength(4, ErrorMessage = "must be a minimum of 4 characters long")]
    [Display(Name = "Title")]
    public string Name {get; set;}

    [Required(ErrorMessage = "is required")]
    [MinLength(8, ErrorMessage = "must be a minimum of 8 characters long")]
    [Display(Name = "Description")]
    public string Description {get; set;}

    [Required(ErrorMessage = "is required")]
    [MyDate(ErrorMessage = "must be a future date")]
    [Display(Name = "Date")]
    public DateTime DueDate {get; set;}

    [Required]
    [Display(Name = "Status")]
    public string Status {get; set;} = "Unresolved";

    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
}


public class MyDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        DateTime d = Convert.ToDateTime(value);
        return d >= DateTime.Now;
    }
}