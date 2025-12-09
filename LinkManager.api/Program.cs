using LinkManager.Api.Extensions;
using LinkManager.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Register infrastructure (DbContext, services)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register authentication & authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "LinkManager API is running securely!");

// Map endpoints from separate modules
app.MapUserEndpoints();
app.MapLinkEndpoints();

// Enable Swagger middleware in development
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.Run();