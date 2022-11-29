using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TMWebApi.Repository;

using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

//Enable CORS
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod()
      .AllowAnyHeader());
});

//JSON Serializer
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
    .Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver   
    = new DefaultContractResolver());



// Add services to the container.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IToolmarkRepository, ToolmarkRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSwaggerGen(options => {
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "TM WEB API",
//        Version = "v1"
//    });
//});

//error builder.Services.AddDbContext<APIDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("EmployeeAppCon")));
// -- add connection string --
var connectionString = builder.Configuration.GetConnectionString("CompanyCOnnStr");
builder.Services.AddDbContext<APIDbContext>(options => options.UseSqlServer(connectionString));

var imagePath = builder.Configuration.GetSection("ImagePath")["PathName"];
var webApiInfo = builder.Configuration.GetSection("WebApiInfo")["Title"];
var versionName = builder.Configuration.GetSection("WebApiInfo")["VersionName"];

var app = builder.Build();

// add code for cors between 2 domains 
// old app.UseCors(options => options.AllowAnyOrigin());
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(versionName, webApiInfo);
        c.DocumentTitle = webApiInfo;
 //       c.DocExpansion(DocExpansion.List);
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// check imagefolder exists or not
string sImageFolder = Path.Combine(Directory.GetCurrentDirectory(), imagePath);
if (!(Directory.Exists(sImageFolder)))
{
    Directory.CreateDirectory(sImageFolder);
}


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), imagePath)),
    RequestPath = "/" + imagePath
});

app.Run();
