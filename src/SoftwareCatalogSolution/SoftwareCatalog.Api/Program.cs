using FluentValidation;
using SoftwareCatalog.Api.Techs;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<CreateTechRequestValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
// Hey, if I get an HTTP "GET" to "/weatherforecast" - create the controller and call that method.

app.Run();
