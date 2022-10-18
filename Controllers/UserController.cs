using Microsoft.AspNetCore.Mvc;
using TodoList.Models;
namespace TodoList.Controllers;
using Microsoft.AspNetCore.Http;

public class UserController : Controller
{
    private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("UserId");
        }
    }

    private bool loggedIn
    {
        get
        {
            return uid != null;
        }
    }
    
    [HttpGet("/")]
    [HttpGet("/home")]
    public IActionResult LoginOrRegister()
    {
        return View("LoginOrRegister");
    }
}