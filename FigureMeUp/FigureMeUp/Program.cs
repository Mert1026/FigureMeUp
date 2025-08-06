//using FigureMeUp.Data;
//using FigureMeUp.Data.Repositories.Interfaces;
//using FigureMeUp.Infrastructure;
//using FigureMeUp.Services;
//using FigureMeUp.Services.Core;
//using FigureMeUp.Services.Core.Helpers;
//using FigureMeUp.Services.Core.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddControllersWithViews();

////builder.Services.AddRepositories(typeof(IFiguresRepository).Assembly);
////builder.Services.AddUserDefinedServices(typeof(IFigureService).Assembly);

////builder.Services.AddRepositories(typeof(IPostsRepository).Assembly);
////builder.Services.AddUserDefinedServices(typeof(IPostService).Assembly);

////builder.Services.AddRepositories(typeof(IHashtagsRepository).Assembly);
////builder.Services.AddUserDefinedServices(typeof(IHashtagService).Assembly);

////builder.Services.AddScoped<IFiguresService, FigureService>();
////builder.Services.AddScoped<IPostsService, PostsService>();
////builder.Services.AddScoped<IHashtagsService, HashtagsService>();

//// Add manual registration instead:
//builder.Services.AddScoped<IFigureService, FigureService>();
//builder.Services.AddScoped<IPostService, PostsService>();
//builder.Services.AddScoped<IHashtagService, HashtagsService>();

//// Keep the repository registrations:
//builder.Services.AddRepositories(typeof(IFiguresRepository).Assembly);
//builder.Services.AddRepositories(typeof(IPostsRepository).Assembly);
//builder.Services.AddRepositories(typeof(IHashtagsRepository).Assembly);

//builder.Services.AddScoped<HelperMetods>();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

//app.Run();

using FigureMeUp.Data;
using FigureMeUp.Data.Repositories.Interfaces;
using FigureMeUp.Infrastructure;
using FigureMeUp.Services;
using FigureMeUp.Services.Core;
using FigureMeUp.Services.Core.Helpers;
using FigureMeUp.Services.Core.Helpers.Interfaces;
using FigureMeUp.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Disable email confirmation for development
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;

    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Service registrations
builder.Services.AddScoped<IFigureService, FigureService>();
builder.Services.AddScoped<IPostService, PostsService>();
builder.Services.AddScoped<IHashtagService, HashtagsService>();

//repotata
builder.Services.AddRepositories(typeof(IFiguresRepository).Assembly);
builder.Services.AddRepositories(typeof(IPostsRepository).Assembly);
builder.Services.AddRepositories(typeof(IHashtagsRepository).Assembly);

builder.Services.AddScoped<IHelperMetods, HelperMetods>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await FigureMeUp.Data.Seeders.IdentitySeeder.SeedRolesAsync(services);
        await FigureMeUp.Data.Seeders.IdentitySeeder.AssignAdminRoleAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Use(async (context, next) =>
{
    try
    {
        await next();

        // Handle 404 Not Found
        if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
        {
            context.Request.Path = "/error/404";
            await next();
        }
    }
    catch (Exception ex)
    {
        // Handle unhandled exception (500)
        if (!context.Response.HasStarted)
        {
            context.Response.Clear();
            context.Response.StatusCode = 500;
            context.Request.Path = "/error/500";
            await next();
        }

        // Optionally log the exception (ex) here using ILogger
    }
});


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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