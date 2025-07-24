using FigureMeUp.Data;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Infrastructure;
using FigureMeUp.Services;
using FigureMeUp.Services.Core;
using FigureMeUp.Services.Core.Helpers;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//builder.Services.AddRepositories(typeof(IFiguresRepository).Assembly);
//builder.Services.AddUserDefinedServices(typeof(IFigureService).Assembly);

//builder.Services.AddRepositories(typeof(IPostsRepository).Assembly);
//builder.Services.AddUserDefinedServices(typeof(IPostService).Assembly);

//builder.Services.AddRepositories(typeof(IHashtagsRepository).Assembly);
//builder.Services.AddUserDefinedServices(typeof(IHashtagService).Assembly);

//builder.Services.AddScoped<IFiguresService, FigureService>();
//builder.Services.AddScoped<IPostsService, PostsService>();
//builder.Services.AddScoped<IHashtagsService, HashtagsService>();

// Add manual registration instead:
builder.Services.AddScoped<IFigureService, FigureService>();
builder.Services.AddScoped<IPostService, PostsService>();
builder.Services.AddScoped<IHashtagService, HashtagsService>();

// Keep the repository registrations:
builder.Services.AddRepositories(typeof(IFiguresRepository).Assembly);
builder.Services.AddRepositories(typeof(IPostsRepository).Assembly);
builder.Services.AddRepositories(typeof(IHashtagsRepository).Assembly);

builder.Services.AddScoped<HelperMetods>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
