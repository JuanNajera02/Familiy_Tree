using Familiy_Tree;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

ServiceConfigurator.ConfigureDBOptions(builder);
ServiceConfigurator.ConfigureRepositories(builder);
ServiceConfigurator.ConfigureBusiness(builder);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression(ServiceConfigurator.ConfigureCompression);
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder
            .WithOrigins("http://localhost:4200") // Permite solicitudes desde este origen
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Agrega los servicios de Swagger
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseResponseCompression();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = string.Empty;
});



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
