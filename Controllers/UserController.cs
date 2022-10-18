using Microsoft.AspNetCore.Mvc;
using TodoList.Models;
namespace TodoList.Controllers;
using Microsoft.AspNetCore.Http;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

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


    private string db
    {
        get
        {
            return "Server=localhost;port=3306;userid=root;password=root;database=todolist_db;";
        }
    }

    
    [HttpGet("/")]
    [HttpGet("/home")]
    public IActionResult LoginOrRegister()
    {
        return View("LoginOrRegister");
    }


    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if(!ModelState.IsValid)
        {
            return LoginOrRegister();
        }

        string? existingEmail = null;
        using var con = new MySqlConnection(db);
        con.Open();
        var command = new MySqlCommand($"SELECT Email FROM users WHERE Email = '{newUser.Email}'", con);
        var reader = command.ExecuteReader();
        while(reader.Read())
        {
            existingEmail = reader["Email"].ToString();
        }
        con.Close();
        // Is there security issues with keeping the connection open throughout this method? Ex: Instead of opening and closing the MySQL Connection in this method twice, can I keep the connection open and just close it within the "if statement" below? 
        if(existingEmail != null)
        {
            ModelState.AddModelError("Email", "is taken");
            return LoginOrRegister();
        }
        
        PasswordHasher<User> hashBrowns = new PasswordHasher<User>();
        newUser.Password = hashBrowns.HashPassword(newUser, newUser.Password);
        
        con.Open();
        var add = new MySqlCommand($"INSERT INTO users(FirstName, LastName, Email, Password) VALUES('{newUser.FirstName}', '{newUser.LastName}', '{newUser.Email}', '{newUser.Password}')", con);
        add.ExecuteNonQuery();
        var grabId = new MySqlCommand($"SELECT UserId FROM users WHERE Email = '{newUser.Email}'", con);
        var read = grabId.ExecuteReader();
        int userId = 0;
        while(read.Read())
        {
            userId = Convert.ToInt32(read["UserId"]);
        }
        con.Close();
        HttpContext.Session.SetInt32("UserId", userId);
        return RedirectToAction("Dashboard", "Todo");
    }

    [HttpPost("/login")]
    public IActionResult Login(UserLogin user)
    {
        if(!ModelState.IsValid)
        {
            return LoginOrRegister();
        }

        using var con = new MySqlConnection(db);
        con.Open();
        var command = new MySqlCommand($"SELECT UserId, Email, Password FROM users WHERE Email = '{user.LoginEmail}'", con);
        var reader = command.ExecuteReader();
        int userId = 0;
        string? emailCheck = null;
        string pw = "";
        while(reader.Read())
        {
            userId = Convert.ToInt32(reader["UserId"]);
            emailCheck = reader["Email"].ToString();
            pw = reader["Password"].ToString();
        }
        con.Close();

        if(emailCheck == null)
        {
            ModelState.AddModelError("LoginEmail", "not found");
            return LoginOrRegister();
        }

        PasswordHasher<UserLogin> hashBrowns = new PasswordHasher<UserLogin>();
        PasswordVerificationResult pwCheck = hashBrowns.VerifyHashedPassword(user, pw, user.LoginPassword);

        if(pwCheck == 0)
        {
            ModelState.AddModelError("LoginPassword", "is invalid");
            return LoginOrRegister();
        }

        HttpContext.Session.SetInt32("UserId", userId);
        return RedirectToAction("Dashboard", "Todo");
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return LoginOrRegister();
    }
}