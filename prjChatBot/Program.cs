using Microsoft.EntityFrameworkCore;
using prjChatBot.Models;


var builder = WebApplication.CreateBuilder(args);

//var port = environment.getenvironmentvariable("port") ?? "8081";
//builder.webhost.useurls($"http://*:{port}");

var cnstr = "Server=mssqlserver;Database=GeoDb;User=sa;Password=YourComplexPassword123!;Encrypt=True;TrustServerCertificate=True;";


builder.Services.AddDbContext<GeoDbContext>(options =>
    options.UseSqlServer(cnstr));


// ³]¸m CORS ¬Fµ¦
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});



builder.Services.AddMvc();

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GeoDbContext>();
    context.Database.Migrate();
}

// ±Ò¥Î CORS
app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.Run();
