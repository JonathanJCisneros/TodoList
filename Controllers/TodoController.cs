using Microsoft.AspNetCore.Mvc;
using TodoList.Models;
namespace TodoList.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

public class TodoController : Controller
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

    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if(!loggedIn)
        {
            return RedirectToAction("LoginOrRegister", "User");
        }
        return View("Dashboard");
    }
}