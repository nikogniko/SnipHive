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
builder.Services.AddScoped<SnippetRepository>();


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

app.MapControllerRoute(
    name: "editSnippet",
    pattern: "{controller=EditSnippet}/{action=EditSnippet}/{id?}");

app.MapControllerRoute(
    name: "snippetDetails", 
    pattern: "{controlle=SnippetDetails}/{action=SnippetDetails}/{id?}");

app.MapControllerRoute(
    name: "personalSnippets",
    pattern: "{controlle=PersonalSnippets}/{action=PersonalSnippets}/{id?}");

app.MapControllerRoute(
    name: "savedSnippets",
    pattern: "{controlle=SavedSnippets}/{action=SavedSnippets}/{id?}");

app.Run();