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
        if(loggedIn)
        {
            return RedirectToAction("Dashboard", "Todo");
        }
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
        var command = new MySqlCommand("SELECT Email FROM users WHERE Email = @Email", con);
        command.Parameters.AddWithValue("@Email", newUser.Email);
        var reader = command.ExecuteReader();
        while(reader.Read())
        {
            existingEmail = reader["Email"].ToString();
        }
        con.Close();
        
        if(existingEmail != null)
        {
            ModelState.AddModelError("Email", "is taken");
            return LoginOrRegister();
        }
        
        PasswordHasher<User> hashBrowns = new PasswordHasher<User>();
        newUser.Password = hashBrowns.HashPassword(newUser, newUser.Password);


        con.Open();
        var add = new MySqlCommand("INSERT INTO users(FirstName, LastName, Email, Password) VALUES(@First, @Last, @Email, @Password)", con);
        add.Parameters.AddWithValue("@First", newUser.FirstName);
        add.Parameters.AddWithValue("@Last", newUser.LastName);
        add.Parameters.AddWithValue("@Email", newUser.Email);
        add.Parameters.AddWithValue("@Password", newUser.Password);
        add.ExecuteNonQuery();

        var grabId = new MySqlCommand("SELECT UserId FROM users WHERE Email = @Email", con);
        grabId.Parameters.AddWithValue("@Email", newUser.Email);
        var read = grabId.ExecuteReader();
        int userId = 0;
        while(read.Read())
        {
            userId = Convert.ToInt32(read["UserId"]);
        }
        con.Close();

        HttpContext.Session.SetInt32("UserId", userId);
        HttpContext.Session.SetString("Name", newUser.FirstName);
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
        var command = new MySqlCommand("SELECT UserId, FirstName, Email, Password FROM users WHERE Email = @Email", con);
        command.Parameters.AddWithValue("@Email", user.LoginEmail);
        var reader = command.ExecuteReader();
        int userId = 0;
        string name = "";
        string? emailCheck = null;
        string pw = "";
        while(reader.Read())
        {
            userId = Convert.ToInt32(reader["UserId"]);
            name = reader["FirstName"].ToString();
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
        HttpContext.Session.SetString("Name", name);
        return RedirectToAction("Dashboard", "Todo");
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return LoginOrRegister();
    }
}