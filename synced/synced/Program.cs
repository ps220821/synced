using Microsoft.EntityFrameworkCore;
using synced_BBL.Interfaces;
using synced_BBL.Services;
using synced_DAL;
using synced_DAL.Interfaces;
using synced_DAL.Repositories;
using synced_DALL.Interfaces;
using synced_DALL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddScoped<DatabaseHelper>(provider =>
    new DatabaseHelper(builder.Configuration.GetConnectionString("DefaultConnection")));

// user scopes
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectServices>();

builder.Services.AddScoped<IProjectUserService,ProjectUserService>();
builder.Services.AddScoped<IUserProjectRepository, UserProjectRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddScoped<ITaskCommentService, TaskCommentService>(); // Register the interface with the service
builder.Services.AddScoped<ItaskCommentRepository, TaskCommentRepository>();




builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = false; // For testing; consider true for security in production
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "SyncedSession"; // Optional: custom name for clarity
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); // Must be before UseRouting
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();