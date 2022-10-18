using Microsoft.AspNetCore.Mvc;
using TodoList.Models;
namespace TodoList.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

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

    private string db
    {
        get
        {
            return "Server=localhost;port=3306;userid=root;password=root;database=todolist_db;";
        }
    }

    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if(!loggedIn)
        {
            return RedirectToAction("LoginOrRegister", "User");
        }
        List<Todo> todoList = new List<Todo>();
        using var con = new MySqlConnection(db);
        con.Open();
        var command = new MySqlCommand("SELECT * FROM todos WHERE todos.UserId = @id ORDER BY DueDate ASC", con);
        command.Parameters.AddWithValue("@id", (int)uid);
        var reader = command.ExecuteReader();
        while(reader.Read())
        {
            Todo todo = new Todo();
            todo.TodoId = Convert.ToInt32(reader["TodoId"]);
            todo.Name = reader["Name"].ToString();
            todo.Description = reader["Description"].ToString();
            todo.DueDate = Convert.ToDateTime(reader["DueDate"]);
            todo.Status = reader["Status"].ToString();
            todo.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
            todo.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

            todoList.Add(todo);
        }
        con.Close();
        return View("Dashboard", todoList);
    }

    [HttpGet("/new")]
    public IActionResult NewTodo()
    {
        if(!loggedIn)
        {
            return RedirectToAction("LoginOrRegister", "User");
        }

        return View("NewTodo");
    }

    [HttpPost("/add")]
    public IActionResult AddTodo(Todo newTodo)
    {
        newTodo.UserId = (int)uid;
        if(!ModelState.IsValid)
        {
            return NewTodo();
        }
        using var con = new MySqlConnection(db);
        con.Open();
        var command = new MySqlCommand("INSERT INTO todos(Name, Description, DueDate, Status, UserId) VALUES(@Name, @Description, @Date, @Status, @UserId)", con);
        command.Parameters.AddWithValue("@Name", newTodo.Name);
        command.Parameters.AddWithValue("@Description", newTodo.Description);
        command.Parameters.AddWithValue("@Date", newTodo.DueDate);
        command.Parameters.AddWithValue("@Status", newTodo.Status);
        command.Parameters.AddWithValue("@UserId", newTodo.UserId);
        command.ExecuteNonQuery();
        con.Close();
        return RedirectToAction("Dashboard");
    }

    [HttpGet("/view/{todoId}")]
    public IActionResult ViewOne(int todoId)
    {
        if(!loggedIn)
        {
            return RedirectToAction("LoginOrRegister", "User");
        }

        using var con = new MySqlConnection(db);
        con.Open();
        var command = new MySqlCommand("SELECT * FROM todos WHERE TodoId = @id", con);
        command.Parameters.AddWithValue("@id", todoId);
        var reader = command.ExecuteReader();
        Todo todo = new Todo();
        while(reader.Read())
        {
            todo.TodoId = Convert.ToInt32(reader["TodoId"]);
            todo.Name = reader["Name"].ToString();
            todo.Description = reader["Description"].ToString();
            todo.DueDate = Convert.ToDateTime(reader["DueDate"]);
            todo.Status = reader["Status"].ToString();
            todo.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
            todo.UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]);
        }
        con.Close();

        return View("ViewOne", todo);
    }

    [HttpGet("/resolve/{todoId}")]
    public IActionResult Resolve(int todoId)
    {
        if(!loggedIn)
        {
            return RedirectToAction("LoginOrRegister", "User");
        }
        using var con = new MySqlConnection(db);
        con.Open();
        var command = new MySqlCommand("UPDATE todos SET Status = 'Resolved' WHERE TodoId = @id", con);
        command.Parameters.AddWithValue("@id", todoId);
        command.ExecuteNonQuery();
        con.Close();

        return RedirectToAction("Dashboard");
    }

    [HttpGet("/delete/{todoId}")]
    public IActionResult DeleteOne(int todoId)
    {
        if(!loggedIn)
        {
            return RedirectToAction("LoginOrRegister", "User");
        }
        using var con = new MySqlConnection(db);
        con.Open();
        var command = new MySqlCommand("DELETE FROM todos WHERE TodoId = @id", con);
        command.Parameters.AddWithValue("@id", todoId);
        command.ExecuteNonQuery();
        con.Close();

        return RedirectToAction("Dashboard");
    }
}