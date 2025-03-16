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
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust session timeout
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<DatabaseHelper>(provider =>
    new DatabaseHelper(builder.Configuration.GetConnectionString("DefaultConnection")));

// user scopes
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectServices>();
builder.Services.AddScoped<ProjectUserService>();

builder.Services.AddScoped<IUserProjectRepository, UserProjectRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

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

app.UseRouting();
app.UseSession();

app.UseAuthorization();
app.MapRazorPages();

app.Run();
