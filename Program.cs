using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Bold.Licensing.BoldLicenseProvider.RegisterLicense("q3LYJ0Suo6shBSxZKTViCe3f068yztKj5K+YwAjOixI=");

builder.Services.AddControllers();

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddMvc(m => m.EnableEndpointRouting = false);

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

var photosPath = Path.Combine(Directory.GetCurrentDirectory(), "Photos");

if (Directory.Exists(photosPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(photosPath),
        RequestPath = "/Photos"
    });
}
else
{
    Console.WriteLine($"Photos directory not found at: {photosPath}");
}

try
{
    app.UseStaticFiles();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.UseMvc();

app.UseAuthorization();

app.MapControllers();

app.Run();
