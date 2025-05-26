using HIMIS_API.Data;
using HIMIS_API.Models;
using HIMIS_API.Services.LoginServices.Implementation;
using HIMIS_API.Services.LoginServices.Interface;
using HIMIS_API.Services.MongoServices;
using HIMIS_API.Services.ProgressServices.Implementation;
using HIMIS_API.Services.ProgressServices.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContextData>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("sqlconnection")));


builder.Services.AddDbContext<DbContextWeb>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("sqlconnectionWebsite")));


builder.Services.AddDbContext<DbContextEMS>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("emsSqlconnectionWebsite")));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddScoped<IMainSchemeService, MainSchemeService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();

//builder.Services.AddScoped<MongoDbContext>();

builder.Services.AddScoped(provider =>
{
    //var connectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
    //var databaseName = "HIMIS";  // Your database name
    //var collectionName = "WorkPhysicalProgressFiles";  // Your collection name
    //return new MongoDbContext(connectionString, databaseName, collectionName);

    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MongoDbConnection");
    var databaseName = "HIMIS";  // Your database name
    var collectionName = "WorkPhysicalProgressFiles";  // Your collection name
    return new MongoDbContext(connectionString, databaseName, collectionName);

});
builder.Services.AddScoped<WorkPhysicalProgressService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseCors("corsapp");

app.UseAuthorization();

app.MapControllers();

app.Run();





