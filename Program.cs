using TodoList.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


var app = builder.Build();

app.UseSession();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
