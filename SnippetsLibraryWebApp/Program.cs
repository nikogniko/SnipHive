using Microsoft.EntityFrameworkCore;
using SnippetsLibraryWebApp.Models;
using SnippetsLibraryWebApp.Repository;


var builder = WebApplication.CreateBuilder(args);

// ������������ ���������� �� ���� �����
builder.Configuration.GetConnectionString("DefaultConnection");

// ������� ���� ������
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProgrammingLanguageRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<TagRepository>();
builder.Services.AddScoped<SnippetModel>();


var app = builder.Build();

// ������������ middleware (Configure the HTTP request pipeline.)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "addSnippet",
    pattern: "{controller=AddSnippet}/{action=AddSnippet}/{id?}");

app.Run();