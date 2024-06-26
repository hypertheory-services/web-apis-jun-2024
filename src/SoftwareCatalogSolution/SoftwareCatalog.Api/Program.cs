using Marten;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using SoftwareCatalog.Api.Catalog;
using SoftwareCatalog.Api.Techs;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFeatureManagement();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddScoped<IAuthorizationHandler, ShouldBeCreatorOfNewSoftwareRequirementHandler>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsSoftwareCenterAdmin", policy =>
    {
        policy.RequireRole("SoftwareCenter");
        policy.RequireRole("Admin");
        policy.AddRequirements(new ShouldBeCreatorOfNewSoftwareRequirement());
    })
    .AddPolicy("IsSoftwareCenter", policy =>
    {
        policy.RequireRole("SoftwareCenter");
    });
// Add services to the container.
builder.Services.AddSingleton(() => TimeProvider.System);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => // dynamically generates much of your open api specification based on your code.
{


    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header with bearer token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",
                Name = "Bearer ",
                In = ParameterLocation.Header
            },[]
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    //options.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
    //options.EnableAnnotations();
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateTechRequestValidator>();

var connectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("Need A Connection String");
builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);
}).UseLightweightSessions();

builder.Services.AddFluentValidationRulesToSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Hey, if I get an HTTP "GET" to "/weatherforecast" - create the controller and call that method.
app.MapGet("/tacos", () => "Delicious");

if (await app.Services.GetRequiredService<IFeatureManager>().IsEnabledAsync("Catalog"))
{

    app.MapCatalogApi();
}
app.Run();
